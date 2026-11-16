# KncWX2Server_SQLite - portable build (Linux/GCC and MinGW). The MSVC 2026 build lives in build/vcxproj.
#
#   make            build the server, the tools and the test runner
#   make test       build and run the full test suite
#   make check      verify the packet layouts against the 2014 sources + the database schema
#   make clean
#
# The Visual Studio solution is authoritative for the Windows target; this Makefile exists so the
# exact same sources can be compiled and verified in CI (and in this repository's sandbox).

CXX      ?= g++
CC       ?= gcc
CXXSTD   ?= c++23
BUILD    ?= build/linux
BIN      := $(BUILD)/bin
OBJ      := $(BUILD)/obj

CXXFLAGS ?= -std=$(CXXSTD) -O2 -g -Wall -Wextra -Wno-unused-parameter -MMD -MP
CXXFLAGS += -Iinclude -Ithird_party/zlib -Ithird_party/sqlite
CFLAGS   ?= -O2 -g
CFLAGS   += -Ithird_party/zlib

LDFLAGS  ?=
LDLIBS   := -lpthread -ldl

SQLITE_SRC := third_party/sqlite/sqlite3.c
ZLIB_SRC   := $(wildcard third_party/zlib/*.c)
SRC        := $(wildcard src/*/*.cpp)
TEST_SRC   := $(wildcard tests/*.cpp)

LIB_OBJ    := $(patsubst %.cpp,$(OBJ)/%.o,$(SRC))
ZLIB_OBJ   := $(patsubst %.c,$(OBJ)/%.o,$(ZLIB_SRC))
SQLITE_OBJ := $(OBJ)/third_party/sqlite/sqlite3.o

SERVER_SRC := $(wildcard server/*.cpp)
SERVER_OBJ := $(patsubst %.cpp,$(OBJ)/%.o,$(SERVER_SRC))
TOOL_SRC   := $(wildcard tools_src/*.cpp)
TOOL_OBJ   := $(patsubst %.cpp,$(OBJ)/%.o,$(TOOL_SRC))

.PHONY: all server tests tools test clean dirs gen check verify

all: dirs server tests tools

dirs:
	@mkdir -p $(BIN) $(OBJ)

server: dirs $(BIN)/kncserver
tests: dirs $(BIN)/knc_tests
tools: dirs $(BIN)/knc_dbtool

$(BIN)/kncserver: $(LIB_OBJ) $(SERVER_OBJ) $(ZLIB_OBJ) $(SQLITE_OBJ)
	@mkdir -p $(dir $@)
	$(CXX) $(CXXFLAGS) $(LDFLAGS) -o $@ $^ $(LDLIBS)

$(BIN)/knc_tests: $(LIB_OBJ) $(filter-out $(OBJ)/tests/TestFramework.o,$(patsubst %.cpp,$(OBJ)/%.o,$(TEST_SRC))) $(OBJ)/tests/TestFramework.o $(ZLIB_OBJ) $(SQLITE_OBJ)
	@mkdir -p $(dir $@)
	$(CXX) $(CXXFLAGS) $(LDFLAGS) -o $@ $^ $(LDLIBS)

$(BIN)/knc_dbtool: $(LIB_OBJ) $(TOOL_OBJ) $(ZLIB_OBJ) $(SQLITE_OBJ)
	@mkdir -p $(dir $@)
	$(CXX) $(CXXFLAGS) $(LDFLAGS) -o $@ $^ $(LDLIBS)

$(OBJ)/%.o: %.cpp
	@mkdir -p $(dir $@)
	$(CXX) $(CXXFLAGS) -c $< -o $@

$(OBJ)/third_party/zlib/%.o: third_party/zlib/%.c
	@mkdir -p $(dir $@)
	$(CC) $(CFLAGS) -c $< -o $@

$(OBJ)/third_party/sqlite/sqlite3.o: third_party/sqlite/sqlite3.c
	@mkdir -p $(dir $@)
	$(CC) $(CFLAGS) -O2 -c $< -o $@

test: tests
	$(BIN)/knc_tests

# Regenerates the checked-in generated sources. Run after editing sql/*.sql, the 2014 headers in
# ../KncWX2Server/Common, or the resource tables in ../KncWX2Server/ServerResource.
ORIGINAL    ?= ../KncWX2Server
RESOURCES   ?= $(ORIGINAL)/ServerResource

gen:
	python3 tools/gen_eventids.py $(ORIGINAL)/Common include/kproto/EventId.gen.h
	python3 tools/gen_featureflags.py $(ORIGINAL)/Common/ServerDefine.h include/kproto/FeatureFlags.gen.h
	python3 tools/gen_schema.py
	python3 tools/gen_exp_table.py
	python3 tools/gen_stat_table.py $(RESOURCES)/StatTable.lua

# Formats the database tool check: init a scratch database and verify it.
# The packet guard rail: every reimplemented layout must still match the 2014 serializers, and the
# comparator itself is proven to catch a broken layout before its verdict is trusted.
check: tools
	python3 tools/verify_packets.py --self-test
	python3 tools/verify_packets.py
	rm -rf $(BUILD)/checkdb && mkdir -p $(BUILD)/checkdb
	$(BIN)/knc_dbtool --db $(BUILD)/checkdb/knc.db init
	$(BIN)/knc_dbtool --db $(BUILD)/checkdb/knc.db check

clean:
	rm -rf $(BUILD)

-include $(shell find $(OBJ) -name '*.d' 2>/dev/null)
