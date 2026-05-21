using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using System.IO;
using System.Linq;

namespace AbilityGraph.Tests.Editor
{
    public class AutoTestRunner : AssetPostprocessor
    {
        private static TestRunnerApi s_Api;
        private static TestCallbacks s_Callbacks;

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            // Only trigger if a C# script was imported/changed
            bool hasCsChanges = false;// importedAssets.Any(path => path.EndsWith(".cs"));
            if (!hasCsChanges) return;

            Debug.Log("[AutoTestRunner] C# script changes detected. Running EditMode tests...");
            
            s_Api = ScriptableObject.CreateInstance<TestRunnerApi>();
            s_Callbacks = new TestCallbacks();
            s_Api.RegisterCallbacks(s_Callbacks);
            
            var filter = new Filter { testMode = TestMode.EditMode };
            s_Api.Execute(new ExecutionSettings(filter));
        }

        private class TestCallbacks : ICallbacks
        {
            private System.Text.StringBuilder _report = new System.Text.StringBuilder();

            public void RunStarted(ITestAdaptor testsToRun)
            {
                _report.AppendLine($"RUN STARTED AT {System.DateTime.Now}");
                _report.AppendLine($"Total tests to run: {testsToRun.TestCaseCount}");
                _report.AppendLine();
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                _report.AppendLine($"RUN FINISHED");
                _report.AppendLine($"Passed: {result.PassCount}");
                _report.AppendLine($"Failed: {result.FailCount}");
                _report.AppendLine($"Inconclusive: {result.InconclusiveCount}");
                _report.AppendLine($"Skipped: {result.SkipCount}");

                string reportPath = Path.Combine(Directory.GetCurrentDirectory(), "unity_editor_test_log.txt");
                try
                {
                    File.WriteAllText(reportPath, _report.ToString());
                    Debug.Log($"[AutoTestRunner] Tests finished. Report written to {reportPath}. Passed={result.PassCount}, Failed={result.FailCount}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[AutoTestRunner] Failed to write report: {ex.Message}");
                }
                
                s_Api = null;
                s_Callbacks = null;
            }

            public void TestStarted(ITestAdaptor test) {}
            public void TestFinished(ITestResultAdaptor result)
            {
                if (result.TestStatus == TestStatus.Failed)
                {
                    _report.AppendLine($"[FAILED] {result.FullName}");
                    _report.AppendLine($"Message: {result.Message}");
                    _report.AppendLine($"StackTrace: {result.StackTrace}");
                    _report.AppendLine(new string('-', 40));
                }
            }
        }
    }
}
