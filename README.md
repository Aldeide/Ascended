# Ascended

 Third-person sci-fi co-op roguelite.

## Local Development & Testing

To run unit tests locally, you can use the provided `run_tests.ps1` PowerShell script.

### Quick Verification

**Run all EditMode tests:**
```powershell
.\run_tests.ps1
```

**Run specific tests or change the Unity path:**
```powershell
.\run_tests.ps1 -unityPath "C:\Your\Unity\Path.exe" -testPlatform "PlayMode"
```

**Expected Output:**
- Unity starts in headless mode and executes tests.
- Results are saved to `Results_Script.xml`.
- Logs are written to `UnityLog_Script.txt`.

**Common Failure Modes & Fixes:**
- **Script execution disabled:** If you receive an error about running scripts being disabled on this system, open PowerShell as Administrator and run `Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned`.
