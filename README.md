# Retro Football '76

Free Unity game for **iOS** and **macOS** using **historical squads from ~50 years ago** (1974 World Cup + 1976 European Championship).

## Quick Start

1. Install [Unity Hub](https://unity.com/download) + **Unity 2022.3 LTS** (Personal — free).
2. Open this folder as a project in Unity Hub.
3. Menu: **Retro Football → Generate MVP Scenes** (creates Boot, MainMenu, TeamSelect, Match).
4. Open `Assets/Scenes/Boot.unity` and press **Play**.
5. Pick two historical teams → watch the stat-driven match simulation.

## Architecture

See [ARCHITECTURE.md](./ARCHITECTURE.md) for layers, data model, match engine, and build pipeline.

## Historical Data

Offline JSON in `Assets/StreamingAssets/Historical/`:

| File | Content |
|------|---------|
| `tournaments.json` | 1974 WC, 1976 Euro |
| `teams.json` | 8 squads — Netherlands, West Germany, Poland, Brazil, Czechoslovakia, Yugoslavia |

Player ratings are derived indices from public historical records (not licensed modern stats).

## Build Targets

### macOS
1. File → Build Settings → **macOS**
2. Architecture: Apple Silicon + Intel
3. Build and Run

### iOS
1. File → Build Settings → **iOS** → Switch Platform
2. Player Settings → Signing Team (Apple ID — free developer account)
3. Build → open Xcode project → Run on simulator or device

## Project Layout

```
Assets/Scripts/
  Core/       GameBootstrap, GameSession, BootLoader
  Data/       HistoricalDatabase, models
  Match/      MatchSimulator (5-a-side arcade)
  UI/         Menus, HUD, team select
  Platform/   Touch vs keyboard input
  Editor/     Scene generator menu
Assets/StreamingAssets/Historical/   runtime JSON
```

## Cost

- Unity Personal (free under revenue threshold)
- No paid assets, no backend, no IAP in MVP
- Apple Developer account optional for device testing (simulator is free)

## License

MIT — see LICENSE. Historical player names are factual records; no official FIFA/UEFA branding.
