# Ascended

 Third-person sci-fi co-op roguelite.

## Local Testing

We use automated tests to verify code stability before integration. You can run these tests locally using the provided PowerShell script.

### Prerequisites

1.  **Environment Variable:** Ensure the `UNITY_PATH` environment variable is set to the absolute path of your Unity executable.
    *   **Default Fallback:** If not set, the script will default to `"C:\Program Files\Unity\Hub\Editor\6000.4.5f1\Editor\Unity.exe"`.
    *   **To set it (PowerShell example):**
        ```powershell
        $env:UNITY_PATH = "C:\Path\To\Your\Unity\Editor\Unity.exe"
        ```

### Running Tests

*   **Run all tests:**
    ```powershell
    .\run_tests.ps1
    ```

*   **Run specific tests (Filter):**
    You can run specific test categories or methods by using the `-TestFilter` parameter.
    ```powershell
    .\run_tests.ps1 -TestFilter "ChargesAbilityTests"
    ```

### Quick Verification
*   **Command:** `.\run_tests.ps1`
*   **Expected Output:** `Unity finished with exit code 0`
*   **Common Failure:** If the script fails to start Unity, ensure your `UNITY_PATH` environment variable is correct and points to an existing Unity `6000.4.5f1` installation.
