Perform a full pre-commit pipeline, then commit and push. The commit message is: $ARGUMENTS

Follow these steps in order. Stop and report clearly if any step fails — do not proceed to the next step.

## Step 1 — Build C#

Find the Unity executable. Check common locations in order:
1. Read `ProjectSettings/ProjectVersion.txt` to get the Unity version string (e.g. `2022.3.10f1`).
2. Look for the executable at `C:\Program Files\Unity\Hub\Editor\<version>\Editor\Unity.exe`.
3. If not found there, search `C:\Program Files\Unity\Hub\Editor\` for the closest matching version folder.

Run a headless compilation check:
```
"<unity_exe>" -batchmode -quit -projectPath "c:\RepoGit\Ascended" -logFile "c:\RepoGit\Ascended\Logs\build_check.log" -executeMethod UnityEditor.SyncVS.SyncSolution
```

If the Unity executable cannot be found, fall back to:
```
dotnet build Ascended.sln --configuration Debug
```
Note: this may produce warnings about Unity-specific references — treat compiler errors as failures, warnings as acceptable.

If the build step fails: show the first error from the log and stop.

## Step 2 — Run Unit Tests

Before running tests, check for and kill any running Unity processes that would lock the project:
```powershell
Get-Process Unity -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 2
```
If Unity processes were found and killed, log a warning: "Killed running Unity instance(s) to allow headless test run."

Run EditMode tests headlessly using the same Unity executable found in Step 1:
```
"<unity_exe>" -batchmode -runTests -testPlatform EditMode -projectPath "c:\RepoGit\Ascended" -testResults "c:\RepoGit\Ascended\Logs\test_results.xml" -logFile "c:\RepoGit\Ascended\Logs\test_run.log"
```

Parse `Logs\test_results.xml` after the run. Report a summary: total / passed / failed / skipped.

If any test fails: list the failing test names and stop. Do not commit with failing tests.

## Step 3 — Sync with remote (stash → pull → reapply)

Run these git commands:

1. Check for local changes: `git status --short`
2. If there are local changes, stash them: `git stash push -m "pre-commit-stash"`
3. Pull the latest: `git pull --rebase`
4. If the stash was created, reapply it: `git stash pop`
5. If `git stash pop` reports conflicts: list the conflicting files and stop. Ask the user to resolve conflicts manually before continuing.

## Step 4 — Commit and push

1. Stage all changes: `git add -A`
2. Show a summary of what will be committed: `git diff --cached --stat`
3. If $ARGUMENTS is empty, generate a commit message from the staged diff: read `git diff --cached --stat` and `git diff --cached -- *.cs *.md | head -200` to understand the changes, then write a message of **6–8 words** that captures the essence (e.g. "Add targeting system and IDataManager interface"). Do not ask the user — just use the generated message.
4. Commit:
```
git commit -m "<commit message from $ARGUMENTS>

Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>"
```
5. Push: `git push`

Report success with the commit hash and the push result.
