# GEMINI.md

This file provides guidance to Antigravity (the Gemini-based AI assistant) when working with code in this repository.

## Build & Verification

- **Compilation Verification**: Before completing any C# changes, always run the dotnet CLI build to check for compilation errors:
  ```powershell
  dotnet build Ascended.sln
  ```
  Ensure the build completes successfully with **0 errors**.
- **Local Testing**: Run edit-mode and play-mode tests locally using the PowerShell script:
  ```powershell
  .\run_tests.ps1
  ```
  Ensure all tests pass before proposing commits.

## Key Priorities

- **Performance**: Favor low-overhead, cache-friendly, and GC-friendly solutions. Avoid allocating objects in hot paths (Update loops, FixedUpdate, Job executions).
- **Decoupled Architecture**: Maintain the separation of core systems (`Assets/Systems/`) and game extensions (`Assets/SystemsExtensions/`). Use interfaces to prevent circular assembly dependencies.
- **NGO authoritativeness**: Follow the server-authoritative with client-side prediction model. Use `NetworkVariable` and RPCs for replicating inputs or gameplay state.

## Response Format

Always display the number of tokens used at the end of every response.
