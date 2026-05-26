$unityPath = if ($env:UNITY_PATH) { $env:UNITY_PATH } else { "C:\Program Files\Unity\Hub\Editor\6000.4.5f1\Editor\Unity.exe" }
$projectPath = $PSScriptRoot
$logFile = Join-Path $PSScriptRoot "UnityLog_AllTests.txt"
$testResults = Join-Path $PSScriptRoot "Results_AllTests.xml"

if (Test-Path $logFile) { Remove-Item $logFile }
if (Test-Path $testResults) { Remove-Item $testResults }

Write-Host "Starting Unity to run ALL Ability System tests..."
Start-Process -FilePath $unityPath -ArgumentList "-runTests", "-batchmode", "-projectPath", "$projectPath", "-logFile", "$logFile", "-testResults", "$testResults", "-testPlatform", "EditMode" -Wait

if (Test-Path $testResults) {
    Write-Host "Tests completed. Results saved to $testResults"
    [xml]$xml = Get-Content $testResults
    $total = $xml."test-run".total
    $passed = $xml."test-run".passed
    $failed = $xml."test-run".failed
    $inconclusive = $xml."test-run".inconclusive
    $skipped = $xml."test-run".skipped
    
    Write-Host "Total: $total, Passed: $passed, Failed: $failed, Inconclusive: $inconclusive, Skipped: $skipped"
    
    if ($failed -gt 0) {
        Write-Host "FAILED TESTS:" -ForegroundColor Red
        $xml.SelectNodes("//test-case[@result='Failed']") | ForEach-Object {
            Write-Host "- $($_.fullname)" -ForegroundColor Red
            Write-Host "  Message: $($_.failure.message.InnerText)"
        }
    }
} else {
    Write-Host "Unity failed to produce test results. Check $logFile for errors." -ForegroundColor Yellow
    Get-Content $logFile -Tail 50
}
