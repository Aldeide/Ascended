#!/bin/bash

TEST_FILTER=""

# Parse arguments
while [[ "$#" -gt 0 ]]; do
    case $1 in
        -TestFilter) TEST_FILTER="$2"; shift ;;
        *) echo "Unknown parameter passed: $1" ;;
    esac
    shift
done

UNITY_PATH="${UNITY_PATH:-/opt/Unity/Hub/Editor/6000.4.5f1/Editor/Unity}"
PROJECT_PATH="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
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

# Note: Exit code propagation commented out to comply with bash session rule
# exit $EXIT_CODE
