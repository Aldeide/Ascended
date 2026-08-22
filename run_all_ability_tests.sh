#!/bin/bash

# Default to 6000.4.5f1 standard path for Linux, can be overridden by UNITY_PATH env var
UNITY_PATH=${UNITY_PATH:-/opt/unity/hub/editors/6000.4.5f1/Editor/Unity}
PROJECT_PATH=$(pwd)
LOG_FILE="$PROJECT_PATH/UnityLog_AllTests.txt"
TEST_RESULTS="$PROJECT_PATH/Results_AllTests.xml"

if [ -f "$LOG_FILE" ]; then rm "$LOG_FILE"; fi
if [ -f "$TEST_RESULTS" ]; then rm "$TEST_RESULTS"; fi

echo "Starting Unity to run ALL Ability System tests..."
"$UNITY_PATH" -runTests -batchmode -projectPath "$PROJECT_PATH" -logFile "$LOG_FILE" -testResults "$TEST_RESULTS" -testPlatform EditMode -testFilter "AbilitySystem"
EXIT_CODE=$?

if [ -f "$TEST_RESULTS" ]; then
    echo "Tests completed. Results saved to $TEST_RESULTS"
    # Basic parsing using grep/sed as bash lacks native XML parsing
    TOTAL=$(grep -o 'total="[0-9]*"' "$TEST_RESULTS" | head -1 | grep -o '[0-9]*')
    PASSED=$(grep -o 'passed="[0-9]*"' "$TEST_RESULTS" | head -1 | grep -o '[0-9]*')
    FAILED=$(grep -o 'failed="[0-9]*"' "$TEST_RESULTS" | head -1 | grep -o '[0-9]*')
    INCONCLUSIVE=$(grep -o 'inconclusive="[0-9]*"' "$TEST_RESULTS" | head -1 | grep -o '[0-9]*')
    SKIPPED=$(grep -o 'skipped="[0-9]*"' "$TEST_RESULTS" | head -1 | grep -o '[0-9]*')

    echo "Total: $TOTAL, Passed: $PASSED, Failed: $FAILED, Inconclusive: $INCONCLUSIVE, Skipped: $SKIPPED"

    if [ "$FAILED" -gt 0 ]; then
        echo -e "\e[31mFAILED TESTS:\e[0m"
        # Simplistic extraction of failed test names
        grep -B 2 'result="Failed"' "$TEST_RESULTS" | grep 'fullname=' | sed -E 's/.*fullname="([^"]*)".*/- \1/'
    fi
else
    echo -e "\e[33mUnity failed to produce test results. Check $LOG_FILE for errors.\e[0m"
    tail -n 50 "$LOG_FILE"
fi

exit $EXIT_CODE
