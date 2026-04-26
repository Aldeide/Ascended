$unityPath = "C:\Program Files\Unity\Hub\Editor\6000.2.0b12\Editor\Unity.exe"
$projectPath = "c:\Users\Anthony\Desktop\Ascended\Ascended"
$testFilter = "CueManagerTests"
$logFile = "UnityLog_Script.txt"
$testResults = "Results_Script.xml"

Write-Host "Starting Unity to run tests..."
& $unityPath -runTests -batchmode -nographics -projectPath $projectPath -logFile $logFile -testResults $testResults
Write-Host "Unity finished with exit code $LASTEXITCODE"
