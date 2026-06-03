# Ascended

 Third-person sci-fi co-op roguelite.

## Local Testing

We use automated tests to verify code stability before integration. You can run these tests locally using the provided PowerShell scripts (Windows) or Bash scripts (macOS/Linux).

### Prerequisites

1.  **Environment Variable:** Ensure the `UNITY_PATH` environment variable is set to the absolute path of your Unity executable.
    *   **Windows Default Fallback:** If not set, the script defaults to `"C:\Program Files\Unity\Hub\Editor\6000.4.5f1\Editor\Unity.exe"`.
    *   **macOS/Linux Default Fallback:** If not set, the bash script defaults to `/opt/Unity/Hub/Editor/6000.4.5f1/Editor/Unity` (adjust based on your Linux/macOS install path).
    *   **To set it (PowerShell Windows example):**
        ```powershell
        $env:UNITY_PATH = "C:\Path\To\Your\Unity\Editor\Unity.exe"
        ```
    *   **To set it (Bash macOS/Linux example):**
        ```bash
        export UNITY_PATH="/Applications/Unity/Hub/Editor/6000.4.5f1/Unity.app/Contents/MacOS/Unity"
        ```

### Running Tests

*   **Run all tests:**
    ```powershell
    # Windows
    .\run_tests.ps1
    ```
    ```bash
    # macOS/Linux
    ./run_tests.sh
    ```

*   **Run all ability system tests:**
    ```powershell
    # Windows
    .\run_all_ability_tests.ps1
    ```
    ```bash
    # macOS/Linux
    ./run_all_ability_tests.sh
    ```

*   **Run specific tests (Filter):**
    You can run specific test categories or methods by using the `-TestFilter` parameter.
    ```powershell
    # Windows
    .\run_tests.ps1 -TestFilter "ChargesAbilityTests"
    ```
    ```bash
    # macOS/Linux
    ./run_tests.sh -TestFilter "ChargesAbilityTests"
    ```

### Quick Verification
*   **Command:** `.\run_tests.ps1` (or `./run_tests.sh`)
*   **Expected Output:** `Unity finished with exit code 0`
*   **Common Failure:** If the script fails to start Unity, ensure your `UNITY_PATH` environment variable is correct and points to an existing Unity `6000.4.5f1` installation.

*   **Ability Tests Verification:**
    *   **Command:** `.\run_all_ability_tests.ps1` (or `./run_all_ability_tests.sh`)
    *   **Expected Output:** `Total: X, Passed: X, Failed: 0, Inconclusive: 0, Skipped: 0`
    *   **Common Failure:** If the script fails to start Unity, ensure your `UNITY_PATH` environment variable is correct and points to an existing Unity `6000.4.5f1` installation.
