#!/bin/bash
UNITY_PATH="${UNITY_PATH:-/opt/unity/Editor/Unity}"
PROJECT_PATH="$(pwd)"
LOG_FILE="${PROJECT_PATH}/UnityLog_AllTests.txt"
TEST_RESULTS="${PROJECT_PATH}/Results_AllTests.xml"

[ -f "$LOG_FILE" ] && rm "$LOG_FILE"
[ -f "$TEST_RESULTS" ] && rm "$TEST_RESULTS"

echo "Starting Unity to run ALL Ability System tests..."
"$UNITY_PATH" "-runTests" "-batchmode" "-projectPath" "$PROJECT_PATH" "-logFile" "$LOG_FILE" "-testResults" "$TEST_RESULTS" "-testPlatform" "EditMode"
EXIT_CODE=$?

if [ -f "$TEST_RESULTS" ]; then
    echo "Tests completed. Results saved to $TEST_RESULTS"
    # Basic parsing using awk/sed since xmllint might not be available and grep -P is not always available natively on macOS
    TOTAL=$(awk -F'"' '/<test-run/{for(i=1;i<=NF;i++) if($i~/total=/) print $(i+1)}' "$TEST_RESULTS")
    PASSED=$(awk -F'"' '/<test-run/{for(i=1;i<=NF;i++) if($i~/passed=/) print $(i+1)}' "$TEST_RESULTS")
    FAILED=$(awk -F'"' '/<test-run/{for(i=1;i<=NF;i++) if($i~/failed=/) print $(i+1)}' "$TEST_RESULTS")
    INCONCLUSIVE=$(awk -F'"' '/<test-run/{for(i=1;i<=NF;i++) if($i~/inconclusive=/) print $(i+1)}' "$TEST_RESULTS")
    SKIPPED=$(awk -F'"' '/<test-run/{for(i=1;i<=NF;i++) if($i~/skipped=/) print $(i+1)}' "$TEST_RESULTS")

    echo "Total: ${TOTAL:-0}, Passed: ${PASSED:-0}, Failed: ${FAILED:-0}, Inconclusive: ${INCONCLUSIVE:-0}, Skipped: ${SKIPPED:-0}"

    if [ "${FAILED:-0}" -gt 0 ]; then
        echo -e "\033[0;31mFAILED TESTS:\033[0m"
        grep -B 1 '<failure>' "$TEST_RESULTS" | grep '<test-case' | awk -F'fullname="' '{print $2}' | awk -F'"' '{print $1}' | while read -r test; do
            echo -e "\033[0;31m- $test\033[0m"
        done
    fi
else
    echo -e "\033[1;33mUnity failed to produce test results. Check $LOG_FILE for errors.\033[0m"
    tail -n 50 "$LOG_FILE"
fi
exit $EXIT_CODE
