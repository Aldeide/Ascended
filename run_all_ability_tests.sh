#!/bin/bash
UNITY_PATH="${UNITY_PATH:-/usr/bin/unity}"
PROJECT_PATH="$(pwd)"
LOG_FILE="$PROJECT_PATH/UnityLog_AllTests.txt"
TEST_RESULTS="$PROJECT_PATH/Results_AllTests.xml"

rm -f "$LOG_FILE" "$TEST_RESULTS"

echo "Starting Unity to run ALL Ability System tests..."
"$UNITY_PATH" -runTests -batchmode -projectPath "$PROJECT_PATH" -logFile "$LOG_FILE" -testResults "$TEST_RESULTS" -testPlatform EditMode

if [ -f "$TEST_RESULTS" ]; then
    echo "Tests completed. Results saved to $TEST_RESULTS"
    # Basic parsing for bash using sed (avoid grep -P for cross-platform)
    TOTAL=$(sed -n 's/.*total="\([^"]*\)".*/\1/p' "$TEST_RESULTS" | head -n1)
    PASSED=$(sed -n 's/.*passed="\([^"]*\)".*/\1/p' "$TEST_RESULTS" | head -n1)
    FAILED=$(sed -n 's/.*failed="\([^"]*\)".*/\1/p' "$TEST_RESULTS" | head -n1)
    INCONCLUSIVE=$(sed -n 's/.*inconclusive="\([^"]*\)".*/\1/p' "$TEST_RESULTS" | head -n1)
    SKIPPED=$(sed -n 's/.*skipped="\([^"]*\)".*/\1/p' "$TEST_RESULTS" | head -n1)

    echo "Total: $TOTAL, Passed: $PASSED, Failed: $FAILED, Inconclusive: $INCONCLUSIVE, Skipped: $SKIPPED"
    if [ "$FAILED" -gt 0 ]; then
        echo -e "\033[0;31mFAILED TESTS:\033[0m"
    fi
else
    echo -e "\033[0;33mUnity failed to produce test results. Check $LOG_FILE for errors.\033[0m"
    tail -n 50 "$LOG_FILE"
fi
