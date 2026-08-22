#!/bin/bash

# Check for UNITY_PATH environment variable
if [ -z "$UNITY_PATH" ]; then
    echo "Error: UNITY_PATH environment variable is not set."
    echo "Please set it to the absolute path of your Unity executable."
    echo "Example (macOS): export UNITY_PATH=\"/Applications/Unity/Hub/Editor/6000.4.5f1/Unity.app/Contents/MacOS/Unity\""
    echo "Example (Linux): export UNITY_PATH=\"/opt/Unity/Hub/Editor/6000.4.5f1/Editor/Unity\""
    exit 1
fi

PROJECT_PATH=$(pwd)
LOG_FILE="$PROJECT_PATH/UnityLog_AllTests.txt"
TEST_RESULTS="$PROJECT_PATH/Results_AllTests.xml"

# Remove old logs if they exist
[ -f "$LOG_FILE" ] && rm "$LOG_FILE"
[ -f "$TEST_RESULTS" ] && rm "$TEST_RESULTS"

echo "Starting Unity to run ALL Ability System tests..."
"$UNITY_PATH" -runTests -batchmode -projectPath "$PROJECT_PATH" -logFile "$LOG_FILE" -testResults "$TEST_RESULTS" -testPlatform EditMode

if [ -f "$TEST_RESULTS" ]; then
    echo "Tests completed. Results saved to $TEST_RESULTS"

    # Extract the line containing the main test-run tag
    test_run_line=$(sed -n '/<test-run /p' "$TEST_RESULTS" | head -n 1)

    # Extract values using awk based on attributes
    total=$(echo "$test_run_line" | awk -F'total="' '{print $2}' | awk -F'"' '{print $1}')
    passed=$(echo "$test_run_line" | awk -F'passed="' '{print $2}' | awk -F'"' '{print $1}')
    failed=$(echo "$test_run_line" | awk -F'failed="' '{print $2}' | awk -F'"' '{print $1}')
    inconclusive=$(echo "$test_run_line" | awk -F'inconclusive="' '{print $2}' | awk -F'"' '{print $1}')
    skipped=$(echo "$test_run_line" | awk -F'skipped="' '{print $2}' | awk -F'"' '{print $1}')

    echo "Total: $total, Passed: $passed, Failed: $failed, Inconclusive: $inconclusive, Skipped: $skipped"

    if [ "$failed" -gt 0 ] 2>/dev/null; then
        # Check if grep natively supports terminal color, if not fallback
        echo -e "\033[31mFAILED TESTS:\033[0m"
        # Since we cannot use xml parser safely, use sed/awk to find test-case tags with result="Failed"
        # and print the fullname and message.
        awk '
        /<test-case .*result="Failed"/ {
            match($0, /fullname="([^"]+)"/, arr)
            fullname = arr[1]
            if (fullname == "") {
                # Fallback if match function is not fully supported
                split($0, a, "fullname=\"")
                split(a[2], b, "\"")
                fullname = b[1]
            }
            print "\033[31m- " fullname "\033[0m"
            in_failed = 1
            next
        }
        in_failed && /<message>/ {
            split($0, a, "<!\\[CDATA\\[")
            split(a[2], b, "\\]\\]>")
            msg = b[1]
            print "  Message: " msg
            in_failed = 0
        }
        ' "$TEST_RESULTS"
    fi
else
    echo -e "\033[33mUnity failed to produce test results. Check $LOG_FILE for errors.\033[0m"
    tail -n 50 "$LOG_FILE"
fi
