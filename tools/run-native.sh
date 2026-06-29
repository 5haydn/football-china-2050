#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
export FOOTBALL_ROOT="$ROOT"
cd "$ROOT/tools/RetroFootballMac"
swift build -c release 2>&1
exec swift run -c release RetroFootballMac
