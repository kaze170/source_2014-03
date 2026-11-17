#!/usr/bin/env bash
# ---------------------------------------------------------------------------
# run_tests.sh - the portable conformance harness.
#
# The shipping build is VC++26 with real C++ modules (see CMakeLists.txt).
# That toolchain is not available on Linux CI, so every .cppm also compiles as
# a plain header under -DKNC_CLASSIC (the dual-build contract documented in
# src/knc_prelude.hpp). This script builds the test binaries that way and runs
# them, so the thing we verify is literally the thing we ship - same source
# files, same code paths, only the module plumbing differs.
#
# Usage:  tests/run_tests.sh [-j N]
# ---------------------------------------------------------------------------
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
BUILD="${BUILD_DIR:-${ROOT}/.build}"
CXX="${CXX:-g++}"
CXXFLAGS="${CXXFLAGS:--std=c++20 -Wall -Wextra -Wpedantic -O1 -g}"
JOBS="${JOBS:-$(getconf _NPROCESSORS_ONLN 2>/dev/null || echo 4)}"

while getopts "j:" opt; do
  case "$opt" in
    j) JOBS="$OPTARG" ;;
    *) echo "usage: $0 [-j N]" >&2; exit 2 ;;
  esac
done

mkdir -p "${BUILD}/obj"

# --- vendored zlib (needed by knc.proto.serbuffer) -------------------------
ZLIB_SRC="${ROOT}/third_party/zlib"
ZLIB_LIB="${BUILD}/libz.a"
ZLIB_UNITS=(adler32 compress crc32 deflate infback inffast inflate inftrees
            trees uncompr zutil)

needs_zlib=0
if [[ ! -f "${ZLIB_LIB}" ]]; then
  needs_zlib=1
else
  for unit in "${ZLIB_UNITS[@]}"; do
    if [[ "${ZLIB_SRC}/${unit}.c" -nt "${ZLIB_LIB}" ]]; then needs_zlib=1; break; fi
  done
fi

if (( needs_zlib )); then
  echo ">> building vendored zlib"
  pids=()
  for unit in "${ZLIB_UNITS[@]}"; do
    "${CXX%++}cc" -O2 -w -DHAVE_UNISTD_H -c "${ZLIB_SRC}/${unit}.c" \
      -o "${BUILD}/obj/${unit}.o" 2>/dev/null ||
      cc -O2 -w -DHAVE_UNISTD_H -c "${ZLIB_SRC}/${unit}.c" -o "${BUILD}/obj/${unit}.o" &
    pids+=($!)
    while (( $(jobs -rp | wc -l) >= JOBS )); do wait -n; done
  done
  wait
  ar rcs "${ZLIB_LIB}" "${BUILD}"/obj/*.o
fi

# --- test binaries ----------------------------------------------------------
TESTS=(test_crypto test_serializer test_proto test_event_ids test_packets test_dispatch)

echo ">> compiling tests (-DKNC_CLASSIC)"
pids=()
for t in "${TESTS[@]}"; do
  (
    ${CXX} ${CXXFLAGS} -DKNC_CLASSIC -I"${ZLIB_SRC}" \
      "${ROOT}/tests/${t}.cpp" "${ZLIB_LIB}" -o "${BUILD}/${t}"
  ) &
  pids+=($!)
  while (( $(jobs -rp | wc -l) >= JOBS )); do wait -n; done
done
fail=0
for pid in "${pids[@]}"; do wait "$pid" || fail=1; done
if (( fail )); then
  echo "!! compilation failed" >&2
  exit 1
fi

echo
status=0
total_suites=0
failed_suites=0
for t in "${TESTS[@]}"; do
  total_suites=$((total_suites + 1))
  if ! "${BUILD}/${t}"; then
    status=1
    failed_suites=$((failed_suites + 1))
  fi
  echo
done

echo "==========================================================="
if (( status == 0 )); then
  echo "ALL ${total_suites} SUITES PASSED"
else
  echo "${failed_suites} of ${total_suites} SUITES FAILED"
fi
echo "==========================================================="
exit "${status}"
