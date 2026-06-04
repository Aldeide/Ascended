#!/bin/bash

# Determine UNITY_PATH
if [ -n "$UNITY_PATH" ]; then
    UNITY_BIN="$UNITY_PATH"
elif [ "$(uname)" == "Darwin" ]; then
    UNITY_BIN="/Applications/Unity/Hub/Editor/6000.4.5f1/Unity.app/Contents/MacOS/Unity"
else
    UNITY_BIN="$HOME/Unity/Hub/Editor/6000.4.5f1/Editor/Unity"
fi

PROJECT_PATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
LOG_FILE="$PROJECT_PATH/UnityLog_AllTests.txt"
TEST_RESULTS="$PROJECT_PATH/Results_AllTests.xml"

if [ -f "$LOG_FILE" ]; then rm "$LOG_FILE"; fi
if [ -f "$TEST_RESULTS" ]; then rm "$TEST_RESULTS"; fi

echo "Starting Unity to run ALL Ability System tests..."
"$UNITY_BIN" -runTests -batchmode -projectPath "$PROJECT_PATH" -logFile "$LOG_FILE" -testResults "$TEST_RESULTS" -testPlatform EditMode

if [ -f "$TEST_RESULTS" ]; then
    echo "Tests completed. Results saved to $TEST_RESULTS"
    # Basic parsing using sed/grep
    TOTAL=$(grep -oE 'total="[0-9]+"' "$TEST_RESULTS" | head -1 | grep -oE '[0-9]+')
    PASSED=$(grep -oE 'passed="[0-9]+"' "$TEST_RESULTS" | head -1 | grep -oE '[0-9]+')
    FAILED=$(grep -oE 'failed="[0-9]+"' "$TEST_RESULTS" | head -1 | grep -oE '[0-9]+')
    INCONCLUSIVE=$(grep -oE 'inconclusive="[0-9]+"' "$TEST_RESULTS" | head -1 | grep -oE '[0-9]+')
    SKIPPED=$(grep -oE 'skipped="[0-9]+"' "$TEST_RESULTS" | head -1 | grep -oE '[0-9]+')

    echo "Total: ${TOTAL:-0}, Passed: ${PASSED:-0}, Failed: ${FAILED:-0}, Inconclusive: ${INCONCLUSIVE:-0}, Skipped: ${SKIPPED:-0}"

    if [ "${FAILED:-0}" -gt 0 ]; then
        echo -e "\e[31mFAILED TESTS:\e[0m"
        # Parse failed test case names (simplified)
        grep -E '<test-case.*result="Failed"' "$TEST_RESULTS" | sed -n 's/.*fullname="\([^"]*\)".*/- \1/p'
    fi
else
    echo -e "\e[33mUnity failed to produce test results. Check $LOG_FILE for errors.\e[0m"
    tail -n 50 "$LOG_FILE"
fi
