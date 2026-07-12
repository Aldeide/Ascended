# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

- **Language:** C# / .NET
- **Engine:** Unity 6000.4.5f1
- **Type:** Third-person sci-fi co-op roguelite (multiplayer)

## Key Priorities

- **Performance is critical** — always favor low-overhead, cache-friendly, and GC-friendly solutions. Avoid allocations in hot paths (Update loops, ability ticks, attribute calculations).
- **Network robustness is important** — code must handle latency, packet loss, desync, and disconnections gracefully. Follow the server-authoritative model with client prediction described below.

## Build & Tests

- Open `Ascended.sln` in Rider or Visual Studio. Unity handles incremental C# compilation automatically.
- Run tests via **Window > Testing > Test Runner** in the Unity Editor.
- Alternatively, run tests locally via PowerShell using `.\run_tests.ps1` (see the **Local Testing** section in `README.md` for environment setup and usage instructions).
- To run a single test: select it in the Test Runner and click **Run Selected**.
- **Always run all tests before committing.**
- Test assemblies use NUnit + Moq and live alongside their system under `Assets/Systems/[SystemName]/Test/`.
- Test naming convention: `MethodOrFeature_Scenario_ExpectedResult`.

## Integrations

- **Steam Integration:** The project uses `Facepunch.Steamworks`. The Steam AppID is defined in `steam_appid.txt` in the root directory. This is required for network lobby systems to function properly.

## Documentation

- Project docs live at `Assets/Documentation/`. Key files:
  - `Overview.md` — architecture philosophy and directory map
  - `Networking/Strategy.md` — networking patterns and sync models
  - `Systems/Ability_System.md` — GAS deep-dive
  - `Systems/Item_System.md` — item/equipment architecture
  - `Systems/AI_Architecture.md`, `UI/Architecture.md`, etc.
- **Always keep documentation up to date when making any code changes.**
- **Documentation Updater Persona:** When altering local testing, environments, or scripts, keep docs executable and accurate, and include specific "quick verification" steps. See `.jules/documentation_updater.md` for full details.

## Architecture Overview

The project follows a strict **Core vs. Extensions** split:

- `Assets/Systems/` — reusable, engine-agnostic domain logic (could be extracted to a package). Never put game-specific logic here.
- `Assets/SystemsExtensions/` — game-specific implementations that extend core systems (e.g., concrete ability classes, `AvatarSlots`).
- `Assets/Gameplay/` — high-level game rules, lobby, and player controller logic.
- `Assets/Interface/` — UI Toolkit (UXML/USS) and semi-MVC C# controllers.

### Ability System (GAS)

The central hub is `AbilitySystemManager` (`IAbilitySystem`), which coordinates:

| Sub-manager | Responsibility |
|---|---|
| `AbilityManager` | Grant, remove, and lifecycle of ability instances |
| `EffectManager` | Apply, tick, stack, and suspend `GameplayEffect`s |
| `AttributeSetManager` | Attribute lookup, modifier aggregation, snapshots |
| `TagManager` | Active gameplay tag tracking (blocking/requirements) |
| `CueManager` | Visual/audio feedback dispatch |
| `ReplicationManager` | Network sync via `IReplicationManager` interface |

**Attribute formula:** `CurrentValue = (BaseValue + ΣAdditive) × ΠMultiplicative` (Override wins over all).

**Ability activation sequence (predicted):**
1. Client generates `PredictionKey`, snapshots attributes, validates tags/cooldown/cost.
2. Client applies cost effect locally and sends `ServerRpc`.
3. Server validates and responds: on success the snapshot is cleared; on failure attributes are rolled back and predicted effects are retracted.

### Networking

Architecture is **server-authoritative with client-side prediction**. Networking is decoupled from domain logic via the Replication Manager pattern:

```
Core Logic Manager → triggers events → IReplicationManager → NetworkBehaviour → RPCs → Remote Clients
```

- State changes follow **Request → Server Validation → Execution → Replication**.
- Inventory syncs as delta updates (`NotifyClientAddItem` / `NotifyClientRemoveItem`).
- Cues are replicated globally; a predicted cue on the instigating client is suppressed on the server replication to avoid double-play.

### Item System

- `ItemDefinition` (ScriptableObject) acts as a factory for runtime `IBaseItem` instances.
- Equipment → `AbilityManager.GrantAbility()` + `EffectManager` passive stat effects wired automatically on equip.
- `ItemLibrary` singleton loads all item assets via `Resources.LoadAll` for ID→definition lookups.

### AI System

Modular framework based on **Sensors → Goals → Actions** (GOAP 3.0). Lives in `Assets/AISystem/`.

### UI

UI Toolkit (UXML/USS) preferred for all interfaces. Semi-MVC pattern: controllers in `Assets/Interface/`.

## Debugging Rules

- **When iterating on a bug, always start by adding logs to pinpoint and verify the root cause before attempting any fix.**
- **Always find the root cause of a bug — never fix the symptom.**

## Response Format

Always display the number of tokens used at the end of every response.
