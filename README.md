# Ascended

 Third-person sci-fi co-op roguelite.

## Local Testing

We use automated tests to verify code stability before integration. You can run these tests locally using the provided Bash/PowerShell scripts.

### Prerequisites

1.  **Environment Variable:** Ensure the `UNITY_PATH` environment variable is set to the absolute path of your Unity executable.
    *   **Default Fallback:** If not set, the script will default to `"C:\Program Files\Unity\Hub\Editor\6000.4.5f1\Editor\Unity.exe"` on Windows, or `/opt/unity/hub/editors/6000.4.5f1/Editor/Unity` on Linux/macOS.
    *   **To set it (Windows/PowerShell):**
        ```powershell
        $env:UNITY_PATH = "C:\Path\To\Your\Unity\Editor\Unity.exe"
        ```
    *   **To set it (Linux/macOS):**
        ```bash
        export UNITY_PATH="/opt/unity/hub/editors/6000.4.5f1/Editor/Unity"
        ```

### Running Tests

*   **Run all tests:**
    ```bash
    ./run_tests.sh
    # or on Windows:
    # .\run_tests.ps1
    ```


*   **Run all ability system tests:**
    ```bash
    ./run_all_ability_tests.sh
    # or on Windows:
    # .\run_all_ability_tests.ps1
    ```
*   **Run specific tests (Filter):**
    You can run specific test categories or methods by using the `-TestFilter` parameter.
    ```bash
    ./run_tests.sh -TestFilter "ChargesAbilityTests"
    # or on Windows:
    # .\run_tests.ps1 -TestFilter "ChargesAbilityTests"
    ```

### Quick Verification
*   **Command:** `./run_tests.sh` (or `.\run_tests.ps1` on Windows)
*   **Expected Output:** `Unity finished with exit code 0`
*   **Common Failure:** If the script fails to start Unity, ensure your `UNITY_PATH` environment variable is correct and points to an existing Unity `6000.4.5f1` installation.

*   **Ability Tests Verification:**
    *   **Command:** `./run_all_ability_tests.sh` (or `.\run_all_ability_tests.ps1` on Windows)
    *   **Expected Output:** `Total: X, Passed: X, Failed: 0, Inconclusive: 0, Skipped: 0`
    *   **Common Failure:** If the script fails to start Unity, ensure your `UNITY_PATH` environment variable is correct and points to an existing Unity `6000.4.5f1` installation.
