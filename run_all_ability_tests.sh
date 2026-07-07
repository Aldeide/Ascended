#!/bin/bash
UNITY_PATH="${UNITY_PATH:-/usr/bin/unity}"
PROJECT_PATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
LOG_FILE="$PROJECT_PATH/UnityLog_AllTests.txt"
TEST_RESULTS="$PROJECT_PATH/Results_AllTests.xml"
if [ -f "$LOG_FILE" ]; then rm "$LOG_FILE"; fi
if [ -f "$TEST_RESULTS" ]; then rm "$TEST_RESULTS"; fi
echo "Starting Unity to run ALL Ability System tests..."
"$UNITY_PATH" -runTests -batchmode -projectPath "$PROJECT_PATH" -logFile "$LOG_FILE" -testResults "$TEST_RESULTS" -testPlatform EditMode
if [ -f "$TEST_RESULTS" ]; then
    echo "Tests completed. Results saved to $TEST_RESULTS"
    if command -v xmllint >/dev/null 2>&1; then
        TOTAL=$(xmllint --xpath 'string(//test-run/@total)' "$TEST_RESULTS" 2>/dev/null || echo "0")
        PASSED=$(xmllint --xpath 'string(//test-run/@passed)' "$TEST_RESULTS" 2>/dev/null || echo "0")
        FAILED=$(xmllint --xpath 'string(//test-run/@failed)' "$TEST_RESULTS" 2>/dev/null || echo "0")
        INCONCLUSIVE=$(xmllint --xpath 'string(//test-run/@inconclusive)' "$TEST_RESULTS" 2>/dev/null || echo "0")
        SKIPPED=$(xmllint --xpath 'string(//test-run/@skipped)' "$TEST_RESULTS" 2>/dev/null || echo "0")
        echo "Total: $TOTAL, Passed: $PASSED, Failed: $FAILED, Inconclusive: $INCONCLUSIVE, Skipped: $SKIPPED"
        if [ "$FAILED" -gt 0 ]; then
            echo -e "\033[31mFAILED TESTS:\033[0m"
            # Mac-compatible simple parsing for failed tests using sed
            sed -n 's/.*<test-case[^>]*result="Failed"[^>]*fullname="\([^"]*\)".*/\1/p' "$TEST_RESULTS" | while read -r test_name; do
                echo -e "\033[31m- $test_name\033[0m"
            done
        fi
    else
        echo "xmllint not found, displaying raw results file:"
        cat "$TEST_RESULTS"
    fi
else
    echo -e "\033[33mUnity failed to produce test results. Check $LOG_FILE for errors.\033[0m"
    tail -n 50 "$LOG_FILE"
fi
