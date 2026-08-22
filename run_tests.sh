#!/bin/bash

# Parse arguments
TEST_FILTER=""

while [[ "$#" -gt 0 ]]; do
    case $1 in
        -TestFilter|-testFilter) TEST_FILTER="$2"; shift ;;
        *) echo "Unknown parameter passed: $1"; exit 1 ;;
    esac
    shift
done

# Fallback path if environment variable is not set
UNITY_PATH=${UNITY_PATH:-"C:\Program Files\Unity\Hub\Editor\6000.4.5f1\Editor\Unity.exe"}

PROJECT_PATH=$(pwd)
LOG_FILE="$PROJECT_PATH/UnityLog_Script.txt"
TEST_RESULTS="$PROJECT_PATH/Results_Script.xml"

ARGUMENTS=("-runTests" "-batchmode" "-projectPath" "$PROJECT_PATH" "-logFile" "$LOG_FILE" "-testResults" "$TEST_RESULTS" "-testPlatform" "EditMode")

if [ -n "$TEST_FILTER" ]; then
    ARGUMENTS+=("-testFilter" "$TEST_FILTER")
fi

echo "Starting Unity to run tests..."
"$UNITY_PATH" "${ARGUMENTS[@]}"
EXIT_CODE=$?
echo "Unity finished with exit code $EXIT_CODE"
