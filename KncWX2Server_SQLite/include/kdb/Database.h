// kdb/Database.h - the single SQLite 3.54 database file.
//
// Replaces the fifteen ODBC DSNs (DC_ACCOUNT, DC_GAME, DC_LOG, DC_SMS, ...) and the stored
// procedure protocol of the 2014 build with one WAL-mode .db file and typed C++ functions.
#pragma once

#include <cstdint>
#include <functional>
#include <map>
#include <memory>
#include <mutex>
#include <optional>
#include <string>
#include <variant>
#include <vector>

struct sqlite3;
struct sqlite3_stmt;

namespace knc::db {

// A single SQL value. Blobs travel through std::string to keep copying cheap.
class Value {
public:
    enum class Type { kNull, kInt, kReal, kText, kBlob };
    using Storage = std::variant<std::monostate, std::int64_t, double, std::string>;

    Value() = default;
    Value(std::nullptr_t) {}
    Value(std::int64_t v) : type_(Type::kInt), value_(v) {}
    Value(int v) : type_(Type::kInt), value_(static_cast<std::int64_t>(v)) {}
    Value(long long v) : type_(Type::kInt), value_(static_cast<std::int64_t>(v)) {}
    Value(unsigned int v) : type_(Type::kInt), value_(static_cast<std::int64_t>(v)) {}
    Value(unsigned long long v) : type_(Type::kInt), value_(static_cast<std::int64_t>(v)) {}
    Value(bool v) : type_(Type::kInt), value_(static_cast<std::int64_t>(v ? 1 : 0)) {}
    Value(double v) : type_(Type::kReal), value_(v) {}
    Value(float v) : type_(Type::kReal), value_(static_cast<double>(v)) {}
    Value(std::string v, bool isBlob = false)
        : type_(isBlob ? Type::kBlob : Type::kText), value_(std::move(v)) {}
    Value(const char* v) : type_(Type::kText), value_(std::string(v ? v : "")) {}

    Type type() const { return type_; }
    bool isNull() const { return type_ == Type::kNull; }
    std::int64_t asInt(std::int64_t def = 0) const;
    double asReal(double def = 0.0) const;
    std::string asText(const std::string& def = {}) const;
    std::string asBlob() const;
    bool asBool() const { return asInt() != 0; }

private:
    Type type_ = Type::kNull;
    Storage value_{};
};

using Row = std::map<std::string, Value>;
using Rows = std::vector<Row>;

class Statement;

// RAII wrapper around one sqlite3 handle. The server uses a single handle guarded by a recursive
// mutex; WAL allows a second read-only connection for tools/backups when needed.
class Database {
public:
    struct Config {
        std::string path = "data/knc.db";
        bool readOnly = false;
        int busyTimeoutMs = 5000;
        int cacheSizeKb = 65536;        // 64 MiB page cache
        bool synchronousNormal = true;  // WAL + NORMAL is durable enough for a game server
        bool foreignKeys = true;
        int walAutoCheckpointPages = 2000;
        bool memoryTempStore = true;
        bool logSql = false;
        bool memory = false;  // ":memory:" for tests
    };

    static Database& instance();

    bool open(const Config& config, std::string* error = nullptr);
    void close();
    bool isOpen() const { return handle_ != nullptr; }
    const Config& config() const { return config_; }

    // Executes one or more statements without parameters.
    bool exec(const std::string& sql, std::string* error = nullptr);

    bool run(const std::string& sql, const std::vector<Value>& params, std::string* error = nullptr);
    Rows query(const std::string& sql, const std::vector<Value>& params, std::string* error = nullptr);
    // Cheap single-value query helper.
    Value queryValue(const std::string& sql, const std::vector<Value>& params = {},
                     const Value& def = Value());

    std::int64_t lastInsertId() const;
    int changes() const;
    std::string lastError() const;

    class Transaction {
    public:
        explicit Transaction(Database& db, bool immediate = true);
        ~Transaction();
        bool commit();
        void rollback();
        bool active() const { return active_; }

    private:
        Database& db_;
        bool active_ = false;
        bool committed_ = false;
    };

    bool transaction(const std::function<bool()>& fn, bool immediate = true);

    bool integrityCheck(std::string* report);
    std::int64_t userVersion();
    bool setUserVersion(std::int64_t version);

    std::recursive_mutex& mutex() { return mutex_; }
    sqlite3* raw() { return handle_; }

public:
    // A Database instance is standalone; the server additionally exposes a process wide singleton
    // through instance(). Tests create their own instances over temporary files.
    Database() = default;
    ~Database();
    Database(const Database&) = delete;
    Database& operator=(const Database&) = delete;

    friend class Statement;

private:

    sqlite3* handle_ = nullptr;
    Config config_{};
    mutable std::recursive_mutex mutex_;
};

class Statement {
public:
    Statement(Database& db, const std::string& sql);
    ~Statement();
    Statement(const Statement&) = delete;
    Statement& operator=(const Statement&) = delete;

    bool valid() const { return stmt_ != nullptr; }
    bool bind(const std::vector<Value>& params);
    bool step();
    bool done() const { return done_; }
    // True when the last step() ended in DONE (not ROW and not an error).
    bool succeeded() const { return done_ && error_.empty(); }
    const std::string& error() const { return error_; }

    Value column(int index) const;
    std::string columnName(int index) const;
    int columnCount() const;
    Row row() const;
    void reset();

    const std::string& sql() const { return sql_; }

private:
    Database& db_;
    std::string sql_;
    sqlite3_stmt* stmt_ = nullptr;
    bool done_ = false;
    std::string error_;
};

}  // namespace knc::db
