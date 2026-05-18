param (
    [string]$TestFilter = ""
)

$unityPath = if ($env:UNITY_PATH) { $env:UNITY_PATH } else { "C:\Program Files\Unity\Hub\Editor\6000.4.5f1\Editor\Unity.exe" }
$projectPath = $PSScriptRoot
$logFile = Join-Path $PSScriptRoot "UnityLog_Script.txt"
$testResults = Join-Path $PSScriptRoot "Results_Script.xml"

$arguments = @("-runTests", "-batchmode", "-projectPath", $projectPath, "-logFile", $logFile, "-testResults", $testResults, "-testPlatform", "EditMode")

if ($TestFilter) {
    $arguments += "-testFilter", $TestFilter
}

Write-Host "Starting Unity to run tests..."
$process = Start-Process -FilePath $unityPath -ArgumentList $arguments -PassThru -Wait
Write-Host "Unity finished with exit code $($process.ExitCode)"
