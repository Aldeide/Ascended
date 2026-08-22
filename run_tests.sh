#!/bin/bash
TEST_FILTER=""
while [[ "$#" -gt 0 ]]; do
    case $1 in
        -TestFilter) TEST_FILTER="$2"; shift ;;
    esac
    shift
done
UNITY_PATH=${UNITY_PATH:-"C:\Program Files\Unity\Hub\Editor\6000.4.5f1\Editor\Unity.exe"}
PROJECT_PATH=$(pwd)
LOG_FILE="$PROJECT_PATH/UnityLog_Script.txt"
TEST_RESULTS="$PROJECT_PATH/Results_Script.xml"
ARGS=("-runTests" "-batchmode" "-projectPath" "$PROJECT_PATH" "-logFile" "$LOG_FILE" "-testResults" "$TEST_RESULTS" "-testPlatform" "EditMode")
if [ -n "$TEST_FILTER" ]; then
    ARGS+=("-testFilter" "$TEST_FILTER")
fi
echo "Starting Unity to run tests..."
"$UNITY_PATH" "${ARGS[@]}"
echo "Unity finished with exit code $?"
