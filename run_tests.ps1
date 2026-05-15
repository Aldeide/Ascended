$unityPath = "C:\Program Files\Unity\Hub\Editor\6000.4.5f1\Editor\Unity.exe"
$projectPath = $PSScriptRoot
$testFilter = "ChargesAbilityTests"
$logFile = Join-Path $PSScriptRoot "UnityLog_Script.txt"
$testResults = Join-Path $PSScriptRoot "Results_Script.xml"

Write-Host "Starting Unity to run tests..."
$process = Start-Process -FilePath $unityPath -ArgumentList "-runTests", "-batchmode", "-projectPath", $projectPath, "-logFile", $logFile, "-testResults", $testResults, "-testPlatform", "EditMode" -PassThru -Wait
Write-Host "Unity finished with exit code $($process.ExitCode)"
