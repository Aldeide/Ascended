#!/bin/bash
UNITY_PATH=${UNITY_PATH:-"C:\Program Files\Unity\Hub\Editor\6000.4.5f1\Editor\Unity.exe"}
PROJECT_PATH=$(pwd)
LOG_FILE="$PROJECT_PATH/UnityLog_AllTests.txt"
TEST_RESULTS="$PROJECT_PATH/Results_AllTests.xml"
rm -f "$LOG_FILE" "$TEST_RESULTS"
echo "Starting Unity to run ALL Ability System tests..."
"$UNITY_PATH" -runTests -batchmode -projectPath "$PROJECT_PATH" -logFile "$LOG_FILE" -testResults "$TEST_RESULTS" -testPlatform EditMode
if [ -f "$TEST_RESULTS" ]; then
    echo "Tests completed. Results saved to $TEST_RESULTS"
    TOTAL=$(sed -n 's/.*total="\([^"]*\)".*/\1/p' "$TEST_RESULTS" | head -n 1)
    PASSED=$(sed -n 's/.*passed="\([^"]*\)".*/\1/p' "$TEST_RESULTS" | head -n 1)
    FAILED=$(sed -n 's/.*failed="\([^"]*\)".*/\1/p' "$TEST_RESULTS" | head -n 1)
    INCONCLUSIVE=$(sed -n 's/.*inconclusive="\([^"]*\)".*/\1/p' "$TEST_RESULTS" | head -n 1)
    SKIPPED=$(sed -n 's/.*skipped="\([^"]*\)".*/\1/p' "$TEST_RESULTS" | head -n 1)
    echo "Total: $TOTAL, Passed: $PASSED, Failed: $FAILED, Inconclusive: $INCONCLUSIVE, Skipped: $SKIPPED"

    if [ "$FAILED" -gt 0 ]; then
        echo -e "\e[31mFAILED TESTS:\e[0m"
        # Simple parsing using sed - cross platform, no grep -P. Generic message extraction.
        sed -n '/<test-case/ { /result="Failed"/ { s/.*fullname="\([^"]*\)".*/- \1/p; n; s/.*<message>.*\[\(.*\)\]\]><\/message>.*/  Message: \1/p; s/.*<message>\(.*\)<\/message>.*/  Message: \1/p } }' "$TEST_RESULTS"
    fi
else
    echo -e "\e[33mUnity failed to produce test results. Check $LOG_FILE for errors.\e[0m"
    tail -n 50 "$LOG_FILE"
fi
