#!/bin/bash

# Parse arguments
TEST_FILTER=""

while [[ "$#" -gt 0 ]]; do
    case $1 in
        -TestFilter) TEST_FILTER="$2"; shift ;;
        *) echo "Unknown parameter passed: $1"; exit 1 ;;
    esac
    shift
done

# Check for UNITY_PATH environment variable
if [ -z "$UNITY_PATH" ]; then
    echo "Error: UNITY_PATH environment variable is not set."
    echo "Please set it to the absolute path of your Unity executable."
    echo "Example (macOS): export UNITY_PATH=\"/Applications/Unity/Hub/Editor/6000.4.5f1/Unity.app/Contents/MacOS/Unity\""
    echo "Example (Linux): export UNITY_PATH=\"/opt/Unity/Hub/Editor/6000.4.5f1/Editor/Unity\""
    exit 1
fi

PROJECT_PATH=$(pwd)
LOG_FILE="$PROJECT_PATH/UnityLog_Script.txt"
TEST_RESULTS="$PROJECT_PATH/Results_Script.xml"

# Build arguments array
ARGS=(
    "-runTests"
    "-batchmode"
    "-projectPath" "$PROJECT_PATH"
    "-logFile" "$LOG_FILE"
    "-testResults" "$TEST_RESULTS"
    "-testPlatform" "EditMode"
)

if [ -n "$TEST_FILTER" ]; then
    ARGS+=("-testFilter" "$TEST_FILTER")
fi

echo "Starting Unity to run tests..."
"$UNITY_PATH" "${ARGS[@]}"
EXIT_CODE=$?

echo "Unity finished with exit code $EXIT_CODE"
