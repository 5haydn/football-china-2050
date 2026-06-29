#!/usr/bin/env bash
# Waits for Unity pkg download, then opens the macOS installer (you enter password in GUI).
set -euo pipefail

PKG="$HOME/Downloads/Unity-2022.3.62f3c1.pkg"
EXPECTED=4516931467
ROOT="$(cd "$(dirname "$0")/.." && pwd)"

echo "Waiting for Unity download to finish (~4.5 GB)..."
while [[ -f "$PKG" ]]; do
  size=$(stat -f%z "$PKG" 2>/dev/null || echo 0)
  pct=$(( size * 100 / EXPECTED ))
  echo "  $(du -h "$PKG" | cut -f1) (~${pct}%)"
  if (( size >= EXPECTED - 50000000 )); then
    echo "Download complete. Opening installer..."
    open "$PKG"
    echo "After Unity installs: run $ROOT/tools/run-local.sh"
    exit 0
  fi
  sleep 30
done

echo "Pkg not found at $PKG"
exit 1
