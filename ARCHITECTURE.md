# Retro Football '76 — Architecture

Free Unity game for **iOS** and **macOS**, built on **real historical squads** from ~50 years ago (1974 World Cup + 1976 European Championship).

## Goals

| Goal | Choice |
|------|--------|
| Cost | Unity Personal (free), no paid assets, no backend |
| Platforms | iOS + macOS from one project (Universal 2D) |
| Data | Offline JSON — public historical records, no live APIs |
| MVP scope | Arcade 5-a-side matches, team pick, stat-driven simulation |

## High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation (UI)                        │
│  MainMenu → TeamSelect → MatchHUD → PostMatch               │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│                   Match Controller                          │
│  Kickoff → Possession → Actions → Goals → Full Time         │
└──────────────────────────┬──────────────────────────────────┘
                           │
        ┌──────────────────┼──────────────────┐
        ▼                  ▼                  ▼
┌───────────────┐  ┌───────────────┐  ┌───────────────┐
│ Match Engine  │  │  Simulation   │  │ Input / Touch │
│ (turn-based   │  │  (ratings +   │  │ iOS + macOS   │
│  arcade loop) │  │   RNG events) │  │ keyboard/mouse│
└───────┬───────┘  └───────┬───────┘  └───────────────┘
        │                  │
        └────────┬─────────┘
                 ▼
┌─────────────────────────────────────────────────────────────┐
│                     Data Layer                              │
│  HistoricalDatabase → TeamData, PlayerData, TournamentData  │
│  JSON in StreamingAssets/Historical/                        │
└─────────────────────────────────────────────────────────────┘
```

## Folder Structure

```
Assets/
├── Scenes/
│   ├── Boot.unity          # loads data, goes to menu
│   ├── MainMenu.unity
│   ├── TeamSelect.unity
│   └── Match.unity
├── Scripts/
│   ├── Core/               # GameBootstrap, SceneLoader, GameConstants
│   ├── Data/               # Models + HistoricalDatabase loader
│   ├── Match/              # MatchController, MatchSimulator, MatchState
│   ├── UI/                 # Menus, HUD, team cards
│   └── Platform/           # InputRouter (touch vs keyboard)
├── Data/Historical/        # Editor reference copies
└── StreamingAssets/
    └── Historical/         # Runtime JSON (teams, tournaments)
Packages/manifest.json
ProjectSettings/
```

## Data Model

```csharp
Tournament { id, name, year, hostNation }
Team       { id, name, nationCode, year, formation, players[] }
Player     { id, name, position, overall, pace, shoot, pass, defend, stamina }
MatchState { homeTeam, awayTeam, minute, homeScore, awayScore, events[] }
MatchEvent { minute, type, playerName, description }
```

**Historical sources (public record, offline snapshots):**
- 1974 FIFA World Cup (West Germany hosts; Netherlands, West Germany, Poland, Brazil, etc.)
- 1976 UEFA European Championship (Czechoslovakia, West Germany, Netherlands, Yugoslavia)

Ratings are **derived indices** (1–99) from era-appropriate roles — not licensed modern stats.

## Match Simulation (MVP)

Arcade **5-a-side**, turn-based **possession phases** (fast to ship, no physics tuning):

1. Each phase: attacking team rolls `attackPower` vs defending `defendPower`.
2. Outcomes: goal chance, shot saved, turnover, foul (set piece).
3. Player weights: ST/CF boost shoot, MF boost pass, DF boost defend.
4. Stamina drains per phase; subs not in MVP.
5. 2 × 3-minute halves (accelerated clock) → full-time screen.

Deterministic option: seed RNG from match ID for replays.

## Platform Layer

| Platform | Input | Build |
|----------|-------|-------|
| iOS | Touch (tap actions, swipe optional) | IL2CPP, min iOS 14 |
| macOS | Keyboard (WASD/Space) + mouse | Mono/IL2CPP, Apple Silicon + Intel |

`InputRouter` abstracts `ITouchInput` / `IKeyboardInput` so Match UI stays shared.

## Unity Stack (100% free)

- **Unity 2022.3 LTS** (Personal)
- **Universal Render Pipeline (2D)** — lightweight sprites
- **TextMeshPro** — bundled
- **Newtonsoft.Json** (`com.unity.nuget.newtonsoft-json`) — JSON load
- No Unity Gaming Services, no ads SDK, no IAP in MVP

## Scene Flow

```
Boot → MainMenu → TeamSelect (pick 1974/1976 squad) → Match → PostMatch → MainMenu
```

`GameBootstrap` (DontDestroyOnLoad) holds `HistoricalDatabase` + selected teams.

## Build Pipeline

1. Open project in Unity Hub 2022.3 LTS.
2. **macOS:** File → Build Settings → macOS → Build.
3. **iOS:** Switch platform → add signing team in Player Settings → Build to Xcode → Run on device/simulator.

CI (optional later): GameCI + GitHub Actions (free for public repos).

## Security & Licensing

- No network calls in MVP.
- Historical names/facts are factual records; no official FIFA/UEFA branding in UI.
- App title: **Retro Football '76** (generic).

## Roadmap After MVP

1. Full 11-a-side pitch + simple AI movement
2. 1978 WC + club legends expansion packs (JSON only)
3. Local multiplayer (same device)
4. Game Center / Steam achievements (still free tier)

## Implementation Status (this repo)

| Module | Status |
|--------|--------|
| Data models + JSON loader | ✅ C# |
| Historical squads 1974/1976 | ✅ JSON |
| Match simulator | ✅ C# |
| Unity scenes / sprites | 🔲 Open in Unity Editor |
| iOS/macOS builds | 🔲 Requires Unity + Xcode |
