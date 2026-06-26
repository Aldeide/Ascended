---
name: verify_dotnet_compilation
description: Triggered when compiling, verifying, building, or testing C# C-sharp code changes in the project.
---

# Verifying C# Changes with dotnet CLI

Always verify your C# changes using the `dotnet build` command before completing a task:

1. Run the compilation check from the project root:
   ```powershell
   dotnet build Ascended.sln
   ```
2. Parse the command output to ensure there are **0 errors**.
3. Fix any compilation warnings or errors in the newly created or modified files.
