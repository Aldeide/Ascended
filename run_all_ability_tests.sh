#!/bin/bash

if [ -z "$UNITY_PATH" ]; then
    UNITY_PATH="/usr/bin/unity"
fi

PROJECT_PATH=$(pwd)
LOG_FILE="$PROJECT_PATH/UnityLog_AllTests.txt"
TEST_RESULTS="$PROJECT_PATH/Results_AllTests.xml"

rm -f "$LOG_FILE"
rm -f "$TEST_RESULTS"

echo "Starting Unity to run ALL Ability System tests..."
"$UNITY_PATH" -runTests -batchmode -projectPath "$PROJECT_PATH" -logFile "$LOG_FILE" -testResults "$TEST_RESULTS" -testPlatform EditMode
EXIT_CODE=$?

if [ -f "$TEST_RESULTS" ]; then
    echo "Tests completed. Results saved to $TEST_RESULTS"
    # Basic parsing using sed/grep for max compat
    TOTAL=$(grep 'test-run' "$TEST_RESULTS" | grep -o 'total="[^"]*"' | head -1 | cut -d'"' -f2)
    PASSED=$(grep 'test-run' "$TEST_RESULTS" | grep -o 'passed="[^"]*"' | head -1 | cut -d'"' -f2)
    FAILED=$(grep 'test-run' "$TEST_RESULTS" | grep -o 'failed="[^"]*"' | head -1 | cut -d'"' -f2)
    INCONCLUSIVE=$(grep 'test-run' "$TEST_RESULTS" | grep -o 'inconclusive="[^"]*"' | head -1 | cut -d'"' -f2)
    SKIPPED=$(grep 'test-run' "$TEST_RESULTS" | grep -o 'skipped="[^"]*"' | head -1 | cut -d'"' -f2)

    echo "Total: ${TOTAL:-0}, Passed: ${PASSED:-0}, Failed: ${FAILED:-0}, Inconclusive: ${INCONCLUSIVE:-0}, Skipped: ${SKIPPED:-0}"

    if [ "${FAILED:-0}" -gt 0 ]; then
        echo -e "\e[31mFAILED TESTS:\e[0m"
        grep 'result="Failed"' "$TEST_RESULTS" | grep -o 'fullname="[^"]*"' | cut -d'"' -f2 | while read -r name; do
            echo -e "\e[31m- $name\e[0m"
        done
    fi
else
    echo -e "\e[33mUnity failed to produce test results. Check $LOG_FILE for errors.\e[0m"
    tail -n 50 "$LOG_FILE" 2>/dev/null
fi

exit $EXIT_CODE
