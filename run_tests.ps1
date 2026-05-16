param (
    [string]$TestFilter = ""
)

# Allow overriding Unity path via environment variable, otherwise fallback to default Windows installation
$unityPath = $env:UNITY_PATH
if ([string]::IsNullOrWhiteSpace($unityPath)) {
    $unityPath = "C:\Program Files\Unity\Hub\Editor\6000.4.5f1\Editor\Unity.exe"
}

if (-Not (Test-Path $unityPath) -And $IsWindows) {
    Write-Warning "Unity not found at $unityPath. Ensure it is installed or set the UNITY_PATH environment variable."
}

$projectPath = $PSScriptRoot
$logFile = Join-Path $PSScriptRoot "UnityLog_Script.txt"
$testResults = Join-Path $PSScriptRoot "Results_Script.xml"

Write-Host "Starting Unity to run tests..."
Write-Host "Unity Path: $unityPath"
Write-Host "Project Path: $projectPath"

$arguments = @("-runTests", "-batchmode", "-projectPath", "`"$projectPath`"", "-logFile", "`"$logFile`"", "-testResults", "`"$testResults`"", "-testPlatform", "EditMode")

if (-Not [string]::IsNullOrWhiteSpace($TestFilter)) {
    Write-Host "Applying test filter: $TestFilter"
    $arguments += "-testFilter"
    $arguments += "`"$TestFilter`""
}

$process = Start-Process -FilePath $unityPath -ArgumentList $arguments -PassThru -Wait
Write-Host "Unity finished with exit code $($process.ExitCode)"

if ($process.ExitCode -ne 0) {
    Write-Host "Some tests failed or an error occurred. Check Results_Script.xml and UnityLog_Script.txt for details." -ForegroundColor Red
} else {
    Write-Host "Tests completed successfully." -ForegroundColor Green
}
