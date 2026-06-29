#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
BUILD_APP="$ROOT/build/RetroFootball76.app"
UNITY_PKG="$HOME/Downloads/Unity-2022.3.62f3c1.pkg"
UNITY_URL="https://download.unitychina.cn/download_unity/1623fc0bbb97/MacEditorInstaller/Unity-2022.3.62f3c1.pkg"

find_unity() {
  for dir in /Applications/Unity/Hub/Editor/*/Unity.app; do
    if [[ -x "$dir/Contents/MacOS/Unity" ]]; then
      echo "$dir/Contents/MacOS/Unity"
      return 0
    fi
  done
  return 1
}

if [[ -x "$BUILD_APP/Contents/MacOS/RetroFootball76" ]] || [[ -d "$BUILD_APP" ]]; then
  echo "Launching local build: $BUILD_APP"
  open "$BUILD_APP"
  exit 0
fi

UNITY_BIN="$(find_unity || true)"
if [[ -z "${UNITY_BIN:-}" ]]; then
  echo "Unity Editor not found. Downloading 2022.3 LTS (~4.5 GB)..."
  if [[ ! -f "$UNITY_PKG" ]]; then
    curl -L --progress-bar -o "$UNITY_PKG" "$UNITY_URL"
  fi
  echo "Installing Unity Editor (may take a few minutes)..."
  sudo installer -pkg "$UNITY_PKG" -target /
  UNITY_BIN="$(find_unity || true)"
fi

if [[ -z "${UNITY_BIN:-}" ]]; then
  echo "Unity install failed. Open Unity Hub → Installs → Install 2022.3 LTS, then re-run:"
  echo "  $ROOT/tools/run-local.sh"
  open -a "Unity Hub" "unityhub://project/open?path=$ROOT"
  exit 1
fi

echo "Building macOS player with Unity..."
"$UNITY_BIN" -batchmode -nographics -quit \
  -projectPath "$ROOT" \
  -executeMethod RetroFootball76.Editor.BuildPlayer.BuildMacOS \
  -logFile /tmp/retro-football-build.log

echo "Launching $BUILD_APP"
open "$BUILD_APP"
