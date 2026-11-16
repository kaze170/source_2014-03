#include "kdb/Database.h"

#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <filesystem>

#include "kcore/Log.h"
#include "sqlite3.h"

namespace knc::db {
namespace {

std::string sqliteError(sqlite3* handle) {
    return handle ? std::string(sqlite3_errmsg(handle)) : std::string("no database handle");
}

void assign(std::string* out, const std::string& value) {
    if (out) *out = value;
}

}  // namespace

std::int64_t Value::asInt(std::int64_t def) const {
    switch (type_) {
        case Type::kInt:
            return std::get<std::int64_t>(value_);
        case Type::kReal:
            return static_cast<std::int64_t>(std::get<double>(value_));
        case Type::kText:
        case Type::kBlob: {
            const auto& s = std::get<std::string>(value_);
            return s.empty() ? def : std::strtoll(s.c_str(), nullptr, 10);
        }
        default:
            return def;
    }
}

double Value::asReal(double def) const {
    switch (type_) {
        case Type::kInt:
            return static_cast<double>(std::get<std::int64_t>(value_));
        case Type::kReal:
            return std::get<double>(value_);
        case Type::kText:
        case Type::kBlob: {
            const auto& s = std::get<std::string>(value_);
            return s.empty() ? def : std::strtod(s.c_str(), nullptr);
        }
        default:
            return def;
    }
}

std::string Value::asText(const std::string& def) const {
    switch (type_) {
        case Type::kText:
        case Type::kBlob:
            return std::get<std::string>(value_);
        case Type::kInt:
            return std::to_string(std::get<std::int64_t>(value_));
        case Type::kReal: {
            char buf[64];
            std::snprintf(buf, sizeof(buf), "%.17g", std::get<double>(value_));
            return buf;
        }
        default:
            return def;
    }
}

std::string Value::asBlob() const { return asText(); }

// ---------------------------------------------------------------------------
Database& Database::instance() {
    static Database db;
    return db;
}

Database::~Database() { close(); }

bool Database::open(const Config& config, std::string* error) {
    std::lock_guard<std::recursive_mutex> guard(mutex_);
    if (handle_) return true;

    config_ = config;
    if (config_.memory) config_.path = ":memory:";

    if (!config_.readOnly && !config_.memory) {
        std::error_code ec;
        const auto parent = std::filesystem::path(config_.path).parent_path();
        if (!parent.empty()) std::filesystem::create_directories(parent, ec);
    }

    const int flags = config_.readOnly ? SQLITE_OPEN_READONLY : (SQLITE_OPEN_READWRITE | SQLITE_OPEN_CREATE);
    const int rc = sqlite3_open_v2(config_.path.c_str(), &handle_, flags | SQLITE_OPEN_FULLMUTEX, nullptr);
    if (rc != SQLITE_OK) {
        assign(error, "cannot open " + config_.path + ": " + sqliteError(handle_));
        if (handle_) {
            sqlite3_close(handle_);
            handle_ = nullptr;
        }
        return false;
    }

    sqlite3_busy_timeout(handle_, config_.busyTimeoutMs);

    std::string pragmas;
    if (!config_.memory) pragmas += "PRAGMA journal_mode=WAL;";
    pragmas += config_.synchronousNormal ? "PRAGMA synchronous=NORMAL;" : "PRAGMA synchronous=FULL;";
    pragmas += std::string("PRAGMA foreign_keys=") + (config_.foreignKeys ? "ON;" : "OFF;");
    pragmas += "PRAGMA cache_size=-" + std::to_string(config_.cacheSizeKb) + ";";
    if (!config_.memory) {
        pragmas += "PRAGMA wal_autocheckpoint=" + std::to_string(config_.walAutoCheckpointPages) + ";";
        pragmas += "PRAGMA mmap_size=268435456;";
        pragmas += "PRAGMA journal_size_limit=67108864;";
    }
    pragmas += std::string("PRAGMA temp_store=") + (config_.memoryTempStore ? "MEMORY;" : "FILE;");

    std::string pragmaError;
    if (!exec(pragmas, &pragmaError)) {
        assign(error, "pragma failure: " + pragmaError);
        close();
        return false;
    }

    if (!config_.memory) {
        const Rows rows = query("PRAGMA journal_mode;", {});
        const std::string mode = rows.empty() ? std::string() : rows.front().begin()->second.asText();
        if (mode != "wal") {
            KLOG(LogLevel::kWarn, "db") << "journal_mode is '" << mode << "' (expected wal)";
        }
    }

    KLOG(LogLevel::kInfo, "db") << "opened " << config_.path << " (SQLite " << sqlite3_libversion() << ')';
    return true;
}

void Database::close() {
    std::lock_guard<std::recursive_mutex> guard(mutex_);
    if (!handle_) return;
    if (!config_.memory) {
        // Fold the WAL back into the main file so a copy is a single file again.
        sqlite3_wal_checkpoint_v2(handle_, nullptr, SQLITE_CHECKPOINT_TRUNCATE, nullptr, nullptr);
    }
    sqlite3_close_v2(handle_);
    handle_ = nullptr;
}

bool Database::exec(const std::string& sql, std::string* error) {
    std::lock_guard<std::recursive_mutex> guard(mutex_);
    if (!handle_) {
        assign(error, "database not open");
        return false;
    }
    if (config_.logSql) KLOG(LogLevel::kDebug, "db") << "exec: " << sql;
    char* message = nullptr;
    const int rc = sqlite3_exec(handle_, sql.c_str(), nullptr, nullptr, &message);
    if (rc != SQLITE_OK) {
        assign(error, message ? message : sqliteError(handle_));
        if (message) sqlite3_free(message);
        return false;
    }
    if (message) sqlite3_free(message);
    return true;
}

bool Database::run(const std::string& sql, const std::vector<Value>& params, std::string* error) {
    std::lock_guard<std::recursive_mutex> guard(mutex_);
    Statement stmt(*this, sql);
    if (!stmt.valid()) {
        assign(error, lastError());
        return false;
    }
    if (!stmt.bind(params)) {
        assign(error, lastError());
        return false;
    }
    while (stmt.step()) {
    }
    if (!stmt.succeeded()) {
        assign(error, stmt.error().empty() ? lastError() : stmt.error());
        return false;
    }
    return true;
}

Rows Database::query(const std::string& sql, const std::vector<Value>& params, std::string* error) {
    std::lock_guard<std::recursive_mutex> guard(mutex_);
    Rows rows;
    Statement stmt(*this, sql);
    if (!stmt.valid()) {
        assign(error, lastError());
        return rows;
    }
    if (!stmt.bind(params)) {
        assign(error, lastError());
        return rows;
    }
    while (stmt.step()) rows.push_back(stmt.row());
    if (!stmt.succeeded()) assign(error, stmt.error().empty() ? lastError() : stmt.error());
    return rows;
}

Value Database::queryValue(const std::string& sql, const std::vector<Value>& params, const Value& def) {
    const Rows rows = query(sql, params);
    if (rows.empty() || rows.front().empty()) return def;
    return rows.front().begin()->second;
}

std::int64_t Database::lastInsertId() const {
    std::lock_guard<std::recursive_mutex> guard(mutex_);
    return handle_ ? sqlite3_last_insert_rowid(handle_) : 0;
}

int Database::changes() const {
    std::lock_guard<std::recursive_mutex> guard(mutex_);
    return handle_ ? sqlite3_changes(handle_) : 0;
}

std::string Database::lastError() const {
    std::lock_guard<std::recursive_mutex> guard(mutex_);
    return sqliteError(handle_);
}

bool Database::integrityCheck(std::string* report) {
    const Rows rows = query("PRAGMA integrity_check;", {});
    std::string text;
    for (const auto& row : rows) {
        for (const auto& entry : row) {
            if (!text.empty()) text += "\n";
            text += entry.second.asText();
        }
    }
    assign(report, text);
    return text == "ok";
}

std::int64_t Database::userVersion() {
    const Value value = queryValue("PRAGMA user_version;", {});
    return value.asInt();
}

bool Database::setUserVersion(std::int64_t version) {
    return exec("PRAGMA user_version=" + std::to_string(version) + ";");
}

Database::Transaction::Transaction(Database& db, bool immediate) : db_(db) {
    std::string err;
    active_ = db_.exec(immediate ? "BEGIN IMMEDIATE;" : "BEGIN;", &err);
    if (!active_) KLOG(LogLevel::kError, "db") << "begin failed: " << err;
}

Database::Transaction::~Transaction() {
    if (active_ && !committed_) db_.exec("ROLLBACK;");
}

bool Database::Transaction::commit() {
    if (!active_) return false;
    std::string err;
    if (!db_.exec("COMMIT;", &err)) {
        KLOG(LogLevel::kError, "db") << "commit failed: " << err;
        return false;
    }
    active_ = false;
    committed_ = true;
    return true;
}

void Database::Transaction::rollback() {
    if (!active_) return;
    db_.exec("ROLLBACK;");
    active_ = false;
}

bool Database::transaction(const std::function<bool()>& fn, bool immediate) {
    Transaction tx(*this, immediate);
    if (!tx.active()) return false;
    bool ok = false;
    try {
        ok = fn();
    } catch (const std::exception& ex) {
        KLOG(LogLevel::kError, "db") << "transaction body threw: " << ex.what();
        ok = false;
    }
    if (!ok) {
        tx.rollback();
        return false;
    }
    return tx.commit();
}

// ---------------------------------------------------------------------------
Statement::Statement(Database& db, const std::string& sql) : db_(db), sql_(sql) {
    std::lock_guard<std::recursive_mutex> guard(db_.mutex_);
    if (!db_.handle_) return;
    const int rc = sqlite3_prepare_v2(db_.handle_, sql.c_str(), -1, &stmt_, nullptr);
    if (rc != SQLITE_OK) {
        KLOG(LogLevel::kError, "db") << "prepare failed: " << sqlite3_errmsg(db_.handle_) << " in: " << sql;
        stmt_ = nullptr;
    }
}

Statement::~Statement() {
    std::lock_guard<std::recursive_mutex> guard(db_.mutex_);
    if (stmt_) sqlite3_finalize(stmt_);
}

bool Statement::bind(const std::vector<Value>& params) {
    std::lock_guard<std::recursive_mutex> guard(db_.mutex_);
    if (!stmt_) return false;
    const int expected = sqlite3_bind_parameter_count(stmt_);
    if (expected != static_cast<int>(params.size())) {
        KLOG(LogLevel::kError, "db") << "bind count mismatch: " << expected << " vs " << params.size() << " in "
                                     << sql_;
        return false;
    }
    sqlite3_reset(stmt_);
    sqlite3_clear_bindings(stmt_);
    for (int i = 0; i < expected; ++i) {
        const auto& value = params[static_cast<std::size_t>(i)];
        const int index = i + 1;
        int rc = SQLITE_OK;
        switch (value.type()) {
            case Value::Type::kNull:
                rc = sqlite3_bind_null(stmt_, index);
                break;
            case Value::Type::kInt:
                rc = sqlite3_bind_int64(stmt_, index, value.asInt());
                break;
            case Value::Type::kReal:
                rc = sqlite3_bind_double(stmt_, index, value.asReal());
                break;
            case Value::Type::kText: {
                const std::string text = value.asText();
                rc = sqlite3_bind_text(stmt_, index, text.c_str(), static_cast<int>(text.size()),
                                       SQLITE_TRANSIENT);
                break;
            }
            case Value::Type::kBlob: {
                const std::string blob = value.asBlob();
                rc = sqlite3_bind_blob(stmt_, index, blob.data(), static_cast<int>(blob.size()),
                                       SQLITE_TRANSIENT);
                break;
            }
        }
        if (rc != SQLITE_OK) return false;
    }
    return true;
}

bool Statement::step() {
    std::lock_guard<std::recursive_mutex> guard(db_.mutex_);
    if (!stmt_ || done_) return false;
    const int rc = sqlite3_step(stmt_);
    if (rc == SQLITE_ROW) return true;
    if (rc == SQLITE_DONE) {
        done_ = true;
        return false;
    }
    error_ = sqlite3_errmsg(db_.handle_);
    KLOG(LogLevel::kError, "db") << "step failed: " << error_ << " in: " << sql_;
    done_ = true;
    return false;
}

Value Statement::column(int index) const {
    std::lock_guard<std::recursive_mutex> guard(db_.mutex_);
    if (!stmt_) return Value();
    switch (sqlite3_column_type(stmt_, index)) {
        case SQLITE_INTEGER:
            return Value(sqlite3_column_int64(stmt_, index));
        case SQLITE_FLOAT:
            return Value(sqlite3_column_double(stmt_, index));
        case SQLITE_NULL:
            return Value();
        case SQLITE_BLOB: {
            const auto* p = static_cast<const char*>(sqlite3_column_blob(stmt_, index));
            const int n = sqlite3_column_bytes(stmt_, index);
            return Value(std::string(p ? p : "", static_cast<std::size_t>(n)), true);
        }
        default: {
            const auto* p = reinterpret_cast<const char*>(sqlite3_column_text(stmt_, index));
            return Value(std::string(p ? p : ""));
        }
    }
}

std::string Statement::columnName(int index) const {
    std::lock_guard<std::recursive_mutex> guard(db_.mutex_);
    if (!stmt_) return {};
    const auto* name = sqlite3_column_name(stmt_, index);
    return name ? name : "";
}

int Statement::columnCount() const {
    std::lock_guard<std::recursive_mutex> guard(db_.mutex_);
    return stmt_ ? sqlite3_column_count(stmt_) : 0;
}

Row Statement::row() const {
    Row result;
    const int count = columnCount();
    for (int i = 0; i < count; ++i) result.emplace(columnName(i), column(i));
    return result;
}

void Statement::reset() {
    std::lock_guard<std::recursive_mutex> guard(db_.mutex_);
    if (!stmt_) return;
    sqlite3_reset(stmt_);
    sqlite3_clear_bindings(stmt_);
    done_ = false;
    error_.clear();
}

}  // namespace knc::db
