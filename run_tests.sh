#!/bin/bash

# Parse arguments
TEST_FILTER=""
while [[ "$#" -gt 0 ]]; do
    case $1 in
        -TestFilter|-testFilter) TEST_FILTER="$2"; shift ;;
        *) echo "Unknown parameter passed: $1"; break ;;
    esac
    shift
done

# Determine UNITY_PATH
if [ -n "$UNITY_PATH" ]; then
    UNITY_BIN="$UNITY_PATH"
elif [ "$(uname)" == "Darwin" ]; then
    UNITY_BIN="/Applications/Unity/Hub/Editor/6000.4.5f1/Unity.app/Contents/MacOS/Unity"
else
    UNITY_BIN="$HOME/Unity/Hub/Editor/6000.4.5f1/Editor/Unity"
fi

PROJECT_PATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
LOG_FILE="$PROJECT_PATH/UnityLog_Script.txt"
TEST_RESULTS="$PROJECT_PATH/Results_Script.xml"

# Build arguments array
ARGS=(-runTests -batchmode -projectPath "$PROJECT_PATH" -logFile "$LOG_FILE" -testResults "$TEST_RESULTS" -testPlatform EditMode)

if [ -n "$TEST_FILTER" ]; then
    ARGS+=("-testFilter" "$TEST_FILTER")
fi

echo "Starting Unity to run tests..."
"$UNITY_BIN" "${ARGS[@]}"
EXIT_CODE=$?
echo "Unity finished with exit code $EXIT_CODE"
