# Retro Football '76 — Quick Start

## Unity (primary)

### 1. Install Editor (one-time)
Unity Hub → **Installs** → **2022.3 LTS**

Or open the downloaded installer:
```bash
open ~/Downloads/Unity-2022.3.62f3c1.pkg
```

### 2. Open project in Editor
```bash
cd ~/Desktop/football-china-2050
./tools/open-unity-editor.sh
```

### 3. Generate scenes (one-time in Editor)
Menu: **Retro Football → Generate MVP Scenes**

### 4. Play in Editor
1. Open `Assets/Scenes/Boot.unity`
2. Press **▶ Play**
3. Pick teams → watch 3D match

### 5. Build & run macOS app (no Editor UI)
```bash
./tools/run-local.sh
```
(or `./tools/unity-build-and-run.sh`)

### Unity keyboard shortcuts
| Screen | Keys |
|--------|------|
| Main menu | Space/Enter — start · Esc — quit |
| Team select | ← → home · ↑ ↓ away · Space — start · Esc — back |
| Match | Esc — main menu |

---

## Native macOS fallback (SceneKit — no Unity)
```bash
./tools/run-native.sh
```

---

## Browser demo (optional)
```bash
cd ~/Desktop/football-china-2050
python3 -m http.server 8765
```
Open http://localhost:8765/tools/play/ — Space to play, arrows to change teams.
