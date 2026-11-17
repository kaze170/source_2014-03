# ---------------------------------------------------------------------------
# KncWX2ServerNG - single-process rewrite of the 2014 KncWX2Server.
#
# Ship target : Visual Studio 2026 (VC++26, MSVC v145+), C++26, named modules.
# Test target : any C++20 compiler, via the KNC_CLASSIC header mode
#               (see tests/run_tests.sh and src/knc_prelude.hpp).
#
# Configure (Windows, from a Developer PowerShell):
#   cmake -S . -B build -G "Visual Studio 18 2026" -A x64
#   cmake --build build --config Release
#
# Configure (Linux conformance build, headers not modules):
#   cmake -S . -B build -DKNCNG_CLASSIC_MODE=ON
#   cmake --build build && ctest --test-dir build --output-on-failure
# ---------------------------------------------------------------------------
cmake_minimum_required(VERSION 3.28)   # 3.28+ = stable C++ module support

project(KncWX2ServerNG
        VERSION 0.1.0
        DESCRIPTION "Single-process rewrite of KncWX2Server (2014), client-compatible"
        LANGUAGES C CXX)

option(KNCNG_CLASSIC_MODE
       "Compile the .cppm sources as plain headers instead of C++ modules (portable conformance build)"
       OFF)
option(KNCNG_BUILD_TESTS "Build the conformance test suite" ON)

if(NOT CMAKE_BUILD_TYPE AND NOT CMAKE_CONFIGURATION_TYPES)
  set(CMAKE_BUILD_TYPE Release CACHE STRING "" FORCE)
endif()

set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)
set(CMAKE_C_STANDARD 11)

# ---------------------------------------------------------------------------
# Third-party, vendored on purpose
# ---------------------------------------------------------------------------
# Both are checked into third_party/ rather than fetched at configure time:
# this repository has to build offline and reproducibly, and the exact SQLite
# version is part of the spec.

# --- zlib 1.3.1 ------------------------------------------------------------
# Required by KSerBuffer's compressed payloads. The wire format embeds a raw
# zlib stream, so the library cannot be swapped for a different codec.
add_library(knc_zlib STATIC
  third_party/zlib/adler32.c
  third_party/zlib/compress.c
  third_party/zlib/crc32.c
  third_party/zlib/deflate.c
  third_party/zlib/infback.c
  third_party/zlib/inffast.c
  third_party/zlib/inflate.c
  third_party/zlib/inftrees.c
  third_party/zlib/trees.c
  third_party/zlib/uncompr.c
  third_party/zlib/zutil.c
)
target_include_directories(knc_zlib PUBLIC third_party/zlib)
if(MSVC)
  target_compile_definitions(knc_zlib PRIVATE _CRT_SECURE_NO_WARNINGS _CRT_NONSTDC_NO_DEPRECATE)
  target_compile_options(knc_zlib PRIVATE /w)
else()
  target_compile_definitions(knc_zlib PRIVATE HAVE_UNISTD_H)
  target_compile_options(knc_zlib PRIVATE -w)
endif()

# --- SQLite 3.54.1 ---------------------------------------------------------
# Replaces the five ODBC/MSSQL DB worker threads of the original.
add_library(knc_sqlite STATIC third_party/sqlite/sqlite3.c)
target_include_directories(knc_sqlite PUBLIC third_party/sqlite)
target_compile_definitions(knc_sqlite PUBLIC
  SQLITE_THREADSAFE=1              # one process, many worker threads
  SQLITE_ENABLE_FTS5               # chat / name search
  SQLITE_ENABLE_JSON1
  SQLITE_ENABLE_RTREE
  SQLITE_DQS=0                     # no double-quoted string literals: a
                                   # mistyped column name must be an error,
                                   # not a silent string
  SQLITE_DEFAULT_FOREIGN_KEYS=1
  SQLITE_OMIT_DEPRECATED
  SQLITE_USE_URI=1
  SQLITE_DEFAULT_WAL_SYNCHRONOUS=1
  SQLITE_LIKE_DOESNT_MATCH_BLOBS
  SQLITE_MAX_EXPR_DEPTH=0
)
if(MSVC)
  target_compile_options(knc_sqlite PRIVATE /w)
else()
  target_compile_options(knc_sqlite PRIVATE -w)
  find_package(Threads REQUIRED)
  target_link_libraries(knc_sqlite PUBLIC Threads::Threads ${CMAKE_DL_LIBS})
endif()

# ---------------------------------------------------------------------------
# The protocol core
# ---------------------------------------------------------------------------
set(KNCNG_MODULE_SOURCES
  src/core/types.cppm
  src/crypto/des.cppm
  src/crypto/md5.cppm
  src/proto/serializer.cppm
  src/proto/serbuffer.cppm
  src/proto/security.cppm
  src/proto/framing.cppm
  src/proto/event_ids.cppm
  src/proto/event.cppm
  src/proto/packet_io.cppm
  src/proto/packets.cppm
  src/proto/dispatch.cppm
)

add_library(knc_core STATIC)
target_link_libraries(knc_core PUBLIC knc_zlib)
target_include_directories(knc_core PUBLIC src)

if(KNCNG_CLASSIC_MODE)
  # Header mode: the .cppm files are consumed with #include, so the library
  # itself has no translation units of its own beyond a placeholder.
  target_compile_definitions(knc_core PUBLIC KNC_CLASSIC=1)
  target_compile_features(knc_core PUBLIC cxx_std_20)
  target_sources(knc_core PRIVATE src/classic_tu.cpp)
  set_source_files_properties(${KNCNG_MODULE_SOURCES} PROPERTIES HEADER_FILE_ONLY ON)
else()
  # Module mode: the real ship configuration.
  target_compile_features(knc_core PUBLIC cxx_std_26)
  target_sources(knc_core PUBLIC FILE_SET CXX_MODULES FILES ${KNCNG_MODULE_SOURCES})
endif()

if(MSVC)
  target_compile_options(knc_core PRIVATE /W4 /permissive- /utf-8 /EHsc)
  target_compile_definitions(knc_core PUBLIC
    WIN32_LEAN_AND_MEAN NOMINMAX UNICODE _UNICODE)
else()
  target_compile_options(knc_core PRIVATE -Wall -Wextra -Wpedantic)
endif()

# ---------------------------------------------------------------------------
# Tests
# ---------------------------------------------------------------------------
# The test sources deliberately #include the .cppm files directly in
# KNC_CLASSIC mode. That keeps one source of truth for both build modes and
# lets the suite run on toolchains without module support.
if(KNCNG_BUILD_TESTS)
  enable_testing()
  foreach(suite IN ITEMS test_crypto test_serializer test_proto test_event_ids
                         test_packets test_dispatch)
    add_executable(${suite} tests/${suite}.cpp)
    target_compile_features(${suite} PRIVATE cxx_std_20)
    target_compile_definitions(${suite} PRIVATE KNC_CLASSIC=1)
    target_link_libraries(${suite} PRIVATE knc_zlib)
    target_include_directories(${suite} PRIVATE src)
    if(MSVC)
      target_compile_options(${suite} PRIVATE /W4 /utf-8 /EHsc)
    else()
      target_compile_options(${suite} PRIVATE -Wall -Wextra)
    endif()
    add_test(NAME ${suite} COMMAND ${suite})
  endforeach()
endif()
