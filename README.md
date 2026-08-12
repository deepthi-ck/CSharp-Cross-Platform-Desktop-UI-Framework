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

## Run the UI (two terminals)

```powershell
# Terminal 1 — Desktop API / IPC host
$env:ASPNETCORE_URLS="http://localhost:5081"
dotnet run --project backend_csharp/backend_csharp.csproj -c Release

# Terminal 2 — Blazor WebAssembly UI
dotnet run --project frontend_csharp/frontend_csharp.csproj
```

Open **http://localhost:5171**

Pages (top navigation): **Home** → **Open** → **Inspect** → **Update** → **Close** → **Stats**

API: `http://localhost:5081` · Health: `http://localhost:5081/health`

The UI uses built-in Blazor WebAssembly routing, layout, and `HttpClient` only.

## Layout

- `frontend_csharp/` Multi-page Blazor WASM desktop shell client
- `backend_csharp/` Desktop API + IPC bridge + orchestration
- `shared/` DTOs / version / config models
- `desktop/` In-memory UI store (canonical)
- `distribution/` Window routing + sync (canonical)
- `quality/` Shared Scenario 2 tools

Conceptual inspiration only: [Electron.NET](https://github.com/ElectronNET/Electron.NET).
