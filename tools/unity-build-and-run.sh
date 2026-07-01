#!/usr/bin/env bash
# Generate scenes, build macOS player, and launch — requires Unity 2022.3 LTS.
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
BUILD_APP="$ROOT/build/RetroFootball76.app"
LOG="/tmp/retro-football-unity.log"

find_unity() {
  if [[ -x "/Applications/Unity/Unity.app/Contents/MacOS/Unity" ]]; then
    echo "/Applications/Unity/Unity.app/Contents/MacOS/Unity"
    return 0
  fi
  for app in /Applications/Unity/Hub/Editor/*/Unity.app; do
    if [[ -x "$app/Contents/MacOS/Unity" ]]; then
      echo "$app/Contents/MacOS/Unity"
      return 0
    fi
  done
  return 1
}

UNITY_BIN="$(find_unity || true)"
if [[ -z "${UNITY_BIN:-}" ]]; then
  echo "Unity Editor not found. Run: $ROOT/tools/open-unity-editor.sh"
  exit 1
fi

echo "Generating scenes + building macOS app (Unity)..."
"$UNITY_BIN" -batchmode -nographics -quit \
  -projectPath "$ROOT" \
  -executeMethod RetroFootball76.Editor.BuildPlayer.BuildMacOS \
  -logFile "$LOG"

if [[ ! -d "$BUILD_APP" ]]; then
  echo "Build failed. See: $LOG"
  tail -30 "$LOG"
  exit 1
fi

echo "Launching Unity build: $BUILD_APP"
open "$BUILD_APP"
