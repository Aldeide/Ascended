# Ascended

 Third-person sci-fi co-op roguelite.

## Local Development & Testing

We provide local testing capabilities both via Unity Test Framework (in Unity) and standalone .NET Core environments.
For deep details into the system architecture, see the [Project Overview Document](Assets/Documentation/Overview.md).

### Unity Tests (Windows/PowerShell)

You can run your NUnit test cases inside the Unity Editor locally via `run_tests.ps1`.
This runs the Editor in batchmode, executing EditMode tests.

**Execution Command:**
```powershell
# Run all tests
.\run_tests.ps1

# Run tests with a specific filter
.\run_tests.ps1 -TestFilter "ChargesAbilityTests"
```

**Expected Output:**
- An exit code of `0` indicating test success.
- `UnityLog_Script.txt` will contain the full Editor debug log.
- `Results_Script.xml` will be generated with standard NUnit test results.

**Common Failure Modes:**
- `Unity not found at...`: The script uses a default installation path. Set the `UNITY_PATH` environment variable to override it:
  `$env:UNITY_PATH="D:\Unity\6000.4.5f1\Editor\Unity.exe"`

### Standalone .NET Tests (Linux/CI)

When the Unity Editor CLI isn't available (e.g. CI environments or specific Linux setups), you can test engine-agnostic logic using `dotnet test`.

**Quick Verification Steps:**
1. Create a dummy NUnit project:
   ```bash
   dotnet new nunit -n DummyTest
   ```
2. Add your scripts/tests to the test runner and execute:
   ```bash
   cd DummyTest && dotnet test
   ```
3. Remove the project after to avoid artifact commits:
   ```bash
   rm -rf DummyTest
   ```
