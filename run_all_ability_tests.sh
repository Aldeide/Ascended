#!/bin/bash

UNITY_PATH=${UNITY_PATH:-"C:\Program Files\Unity\Hub\Editor\6000.4.5f1\Editor\Unity.exe"}
PROJECT_PATH=$(pwd)
LOG_FILE="$PROJECT_PATH/UnityLog_AllTests.txt"
TEST_RESULTS="$PROJECT_PATH/Results_AllTests.xml"

if [ -f "$LOG_FILE" ]; then
    rm "$LOG_FILE"
fi
if [ -f "$TEST_RESULTS" ]; then
    rm "$TEST_RESULTS"
fi

echo "Starting Unity to run ALL Ability System tests..."
"$UNITY_PATH" -runTests -batchmode -projectPath "$PROJECT_PATH" -logFile "$LOG_FILE" -testResults "$TEST_RESULTS" -testPlatform EditMode

if [ -f "$TEST_RESULTS" ]; then
    echo "Tests completed. Results saved to $TEST_RESULTS"

    # We use basic sed/grep for max cross-platform compat since -P is not available on standard macos
    TOTAL=$(grep "<test-run" "$TEST_RESULTS" | head -n 1 | sed -n 's/.*total="\([^"]*\)".*/\1/p')
    PASSED=$(grep "<test-run" "$TEST_RESULTS" | head -n 1 | sed -n 's/.*passed="\([^"]*\)".*/\1/p')
    FAILED=$(grep "<test-run" "$TEST_RESULTS" | head -n 1 | sed -n 's/.*failed="\([^"]*\)".*/\1/p')
    INCONCLUSIVE=$(grep "<test-run" "$TEST_RESULTS" | head -n 1 | sed -n 's/.*inconclusive="\([^"]*\)".*/\1/p')
    SKIPPED=$(grep "<test-run" "$TEST_RESULTS" | head -n 1 | sed -n 's/.*skipped="\([^"]*\)".*/\1/p')

    TOTAL=${TOTAL:-0}
    PASSED=${PASSED:-0}
    FAILED=${FAILED:-0}
    INCONCLUSIVE=${INCONCLUSIVE:-0}
    SKIPPED=${SKIPPED:-0}

    echo "Total: $TOTAL, Passed: $PASSED, Failed: $FAILED, Inconclusive: $INCONCLUSIVE, Skipped: $SKIPPED"

    if [ "$FAILED" -gt 0 ]; then
        echo -e "\e[31mFAILED TESTS:\e[0m"
        # Just indicate failures, robust XML parsing in pure bash is notoriously flaky
        echo -e "\e[31mPlease review $TEST_RESULTS for details on which tests failed.\e[0m"
    fi
else
    echo -e "\e[33mUnity failed to produce test results. Check $LOG_FILE for errors.\e[0m"
    if [ -f "$LOG_FILE" ]; then
        tail -n 50 "$LOG_FILE"
    fi
fi
