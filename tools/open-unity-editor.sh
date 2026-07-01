#!/usr/bin/env bash
# Open Retro Football '76 in Unity Editor (or install Unity first).
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
UNITY_PKG="$HOME/Downloads/Unity-2022.3.62f3c1.pkg"

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
  echo "Unity Editor not installed."
  if [[ -f "$UNITY_PKG" ]]; then
    echo "Opening installer: $UNITY_PKG"
    open "$UNITY_PKG"
  else
    echo "Opening Unity Hub — install 2022.3 LTS, then re-run this script."
    open -a "Unity Hub" "unityhub://project/open?path=$ROOT"
  fi
  echo ""
  echo "After install:"
  echo "  $ROOT/tools/open-unity-editor.sh"
  exit 0
fi

echo "Opening project in Unity Editor..."
"$UNITY_BIN" -projectPath "$ROOT" &
disown 2>/dev/null || true
echo "Unity Editor starting with: $ROOT"
echo "In Editor: Retro Football → Generate MVP Scenes → open Boot.unity → Play"
