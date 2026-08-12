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

## Documentation Updater Skill

Maintain project documentation (README, operational guides, reference docs) as a first-class part of shipping changes. Keep documentation accurate and executable as code evolves.

**When to use**
Activate this Skill when:
* A change alters how to run/test locally (development environment setup, containerization, bootstrapping).
* You add/remove/change environment variables, secrets, or configuration files.
* You change environment/deployment assumptions used by scripts or tools.
* You add/modify operational scripts (deployment, data seeding, logging, maintenance tasks).
* You update integrations with external services or APIs.
* You modify infrastructure setup or deployment processes.

**Principles**
* Prefer updating existing docs over adding new ones. Add a new doc only when it clearly reduces confusion.
* Keep docs executable. Commands must match actual scripts/tools; paths must match the project structure.
* Respect environment requirements. When docs tell users to run commands/tests, include the correct environment setup steps (e.g., sourcing environment files, activating virtual environments, loading configuration).
* Document "what & why", not internal trivia. Focus on user outcomes and maintenance workflows.
* Cross-link instead of duplicating. Use references to maintain a single source of truth.

**Workflow**
1) **Determine doc impact from the change**
Identify what category the change falls into and what docs it likely affects.

2) **Update the smallest set of docs that restores accuracy**
Update only the relevant sections; avoid broad rewrites.
Keep headings and tone consistent with the existing file.
Prefer adding links from main documentation to detailed reference docs for deep dives, instead of duplicating instructions.
3) **Add "quick verification" steps**
When documenting new or changed workflows, include:
* The exact command(s) to run (use project-standard task runners or scripts).
* The expected output/behavior at a high level (1–2 bullets).
* Any common failure mode and how to fix it (1 short subsection max).
4) **Check for drift and broken references**
Confirm referenced scripts/commands exist in the project.
Confirm file paths and directory references exist in the repository.
Verify external links are still valid and accessible.

## Documentation

- Project docs live at `Assets/Documentation/`. Key files:
  - `Overview.md` — architecture philosophy and directory map
  - `Networking/Strategy.md` — networking patterns and sync models
  - `Systems/Ability_System.md` — GAS deep-dive
  - `Systems/Item_System.md` — item/equipment architecture
  - `Systems/AI_Architecture.md`, `UI/Architecture.md`, etc.
- **Documentation Updater rules**: Keep docs executable, include environment requirements, and provide quick verification steps for any changed workflows.
- **Always keep documentation up to date when making any code changes.**

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
