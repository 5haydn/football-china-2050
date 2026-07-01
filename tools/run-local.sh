#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"

echo "Use Unity to play this game:"
echo "  Editor:  $ROOT/tools/open-unity-editor.sh"
echo "  Build:   $ROOT/tools/unity-build-and-run.sh"
echo ""

exec "$ROOT/tools/unity-build-and-run.sh"
