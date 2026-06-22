#!/bin/bash
TEST_FILTER=""

while [[ "$#" -gt 0 ]]; do
    case $1 in
        -TestFilter) TEST_FILTER="$2"; shift ;;
        *) echo "Unknown parameter passed: $1"; exit 1 ;;
    esac
    shift
done

UNITY_PATH="${UNITY_PATH:-/opt/unity/Editor/Unity}"
PROJECT_PATH="$(pwd)"
LOG_FILE="${PROJECT_PATH}/UnityLog_Script.txt"
TEST_RESULTS="${PROJECT_PATH}/Results_Script.xml"

ARGS=("-runTests" "-batchmode" "-projectPath" "$PROJECT_PATH" "-logFile" "$LOG_FILE" "-testResults" "$TEST_RESULTS" "-testPlatform" "EditMode")

if [ -n "$TEST_FILTER" ]; then
    ARGS+=("-testFilter" "$TEST_FILTER")
fi

echo "Starting Unity to run tests..."
"$UNITY_PATH" "${ARGS[@]}"
EXIT_CODE=$?
echo "Unity finished with exit code $EXIT_CODE"
exit $EXIT_CODE
