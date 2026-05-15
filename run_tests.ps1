param (
    [string]$unityPath = "C:\Program Files\Unity\Hub\Editor\6000.4.5f1\Editor\Unity.exe",
    [string]$testFilter = "ChargesAbilityTests",
    [string]$testPlatform = "EditMode",
    [string]$logFile = "UnityLog_Script.txt",
    [string]$testResults = "Results_Script.xml"
)

$projectPath = $PSScriptRoot

Write-Host "Starting Unity to run tests..."
Write-Host "Project Path: $projectPath"
Write-Host "Unity Path: $unityPath"
Write-Host "Test Filter: $testFilter"

$process = Start-Process -FilePath $unityPath -ArgumentList "-runTests", "-batchmode", "-projectPath", "`"$projectPath`"", "-logFile", "`"$logFile`"", "-testResults", "`"$testResults`"", "-testPlatform", $testPlatform -PassThru -Wait
Write-Host "Unity finished with exit code $($process.ExitCode)"
