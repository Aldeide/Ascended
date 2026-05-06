$unityPath = "C:\Program Files\Unity\Hub\Editor\6000.4.5f1\Editor\Unity.exe"
$projectPath = "c:\Users\Anthony\Desktop\Ascended\Ascended"
$testFilter = "ChargesAbilityTests"
$logFile = "UnityLog_Script.txt"
$testResults = "Results_Script.xml"

Write-Host "Starting Unity to run tests..."
$process = Start-Process -FilePath $unityPath -ArgumentList "-runTests", "-batchmode", "-nographics", "-projectPath", $projectPath, "-logFile", $logFile, "-testResults", $testResults, "-testFilter", $testFilter -PassThru -Wait
Write-Host "Unity finished with exit code $($process.ExitCode)"
