# Step 2 — Open & Play in Unity

## If Unity Editor is still installing
Unity Hub → **Installs** → wait for **2022.3 LTS** to finish.

## Open the project
1. Unity Hub → **Projects** → **Open** → select:
   `~/Desktop/football-china-2050`
2. First open takes a few minutes (package import).

## Generate UI scenes (one-time)
Menu bar: **Retro Football → Generate MVP Scenes**

This creates `MainMenu`, `TeamSelect`, and `Match` scenes and adds them to Build Settings.

## Play
1. Open `Assets/Scenes/Boot.unity`
2. Press **▶ Play** (top centre)
3. Pick two historical teams → watch the match

## Build for device
- **macOS:** File → Build Settings → macOS → Build and Run
- **iOS:** Switch to iOS → Build → open in Xcode → Run on simulator/device

## Play now (no Unity)
Browser version (already running if server started):
```bash
cd ~/Desktop/football-china-2050
python3 -m http.server 8765
```
Open http://localhost:8765/tools/play/
