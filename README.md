# C# Cross-platform Desktop UI Framework

Minimal Electron.NET-inspired desktop host + Blazor UI client demonstrating FE/BE .NET version matrix compatibility, multi-window sync, TTL, eviction, statistics, and shared quality tooling.

## Branches (exactly 12)

See `docs/version-matrix.md`. Same-version branches are forbidden.

## Build

```bash
python build.py
# or
./build.sh
```

## Layout

- `frontend_csharp/` Desktop UI shell client
- `backend_csharp/` Desktop API + IPC bridge + orchestration
- `shared/` DTOs / version / config models
- `desktop/` In-memory UI store (canonical)
- `distribution/` Window routing + sync (canonical)
- `quality/` Shared Scenario 2 tools

Conceptual inspiration only: [Electron.NET](https://github.com/ElectronNET/Electron.NET).
