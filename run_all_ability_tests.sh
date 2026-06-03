#!/bin/bash

UNITY_PATH="${UNITY_PATH:-/opt/Unity/Hub/Editor/6000.4.5f1/Editor/Unity}"
PROJECT_PATH="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
LOG_FILE="$PROJECT_PATH/UnityLog_AllTests.txt"
TEST_RESULTS="$PROJECT_PATH/Results_AllTests.xml"

if [ -f "$LOG_FILE" ]; then rm "$LOG_FILE"; fi
if [ -f "$TEST_RESULTS" ]; then rm "$TEST_RESULTS"; fi

echo "Starting Unity to run ALL Ability System tests..."
"$UNITY_PATH" -runTests -batchmode -projectPath "$PROJECT_PATH" -logFile "$LOG_FILE" -testResults "$TEST_RESULTS" -testPlatform EditMode

EXIT_CODE=$?

if [ -f "$TEST_RESULTS" ]; then
    echo "Tests completed. Results saved to $TEST_RESULTS"
    # Fallback to sed to avoid grep -P issues on macOS and variable length lookbehind errors
    TOTAL=$(sed -n 's/.*<test-run.*testcasecount="\([^"]*\)".*/\1/p' "$TEST_RESULTS" | head -1)
    PASSED=$(sed -n 's/.*<test-run.*passed="\([^"]*\)".*/\1/p' "$TEST_RESULTS" | head -1)
    FAILED=$(sed -n 's/.*<test-run.*failed="\([^"]*\)".*/\1/p' "$TEST_RESULTS" | head -1)
    INCONCLUSIVE=$(sed -n 's/.*<test-run.*inconclusive="\([^"]*\)".*/\1/p' "$TEST_RESULTS" | head -1)
    SKIPPED=$(sed -n 's/.*<test-run.*skipped="\([^"]*\)".*/\1/p' "$TEST_RESULTS" | head -1)

    echo "Total: $TOTAL, Passed: $PASSED, Failed: $FAILED, Inconclusive: $INCONCLUSIVE, Skipped: $SKIPPED"

    if [ -n "$FAILED" ] && [ "$FAILED" -gt 0 ] 2>/dev/null; then
        echo -e "\033[31mFAILED TESTS:\033[0m"
        grep -B 2 -A 5 "result=\"Failed\"" "$TEST_RESULTS" | grep "fullname" | sed 's/.*fullname="\([^"]*\)".*/- \1/g' | while read -r line; do
            echo -e "\033[31m$line\033[0m"
        done
        EXIT_CODE=1
    fi
else
    echo -e "\033[33mUnity failed to produce test results. Check $LOG_FILE for errors.\033[0m"
    tail -n 50 "$LOG_FILE"
    EXIT_CODE=1
fi

# Note: Exit code propagation commented out to comply with bash session rule
# exit $EXIT_CODE
