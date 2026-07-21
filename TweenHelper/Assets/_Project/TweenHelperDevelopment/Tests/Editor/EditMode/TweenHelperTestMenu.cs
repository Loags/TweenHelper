using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace LB.TweenHelper.Tests.Editor
{
    internal static class TweenHelperTestMenu
    {
        private const string LogPrefix = "[TweenHelperTests]";

        private static TestRunnerApi _api;
        private static TestCallbacks _callbacks;

        [MenuItem("Tools/TweenHelper/Tests/Run EditMode Tests")]
        private static void RunEditModeTests()
        {
            RunTests(new Filter
            {
                testMode = TestMode.EditMode,
                assemblyNames = new[] { "LB.TweenHelper.EditorTests" }
            });
        }

        [MenuItem("Tools/TweenHelper/Tests/Run PlayMode Tests")]
        private static void RunPlayModeTests()
        {
            RunTests(new Filter
            {
                testMode = TestMode.PlayMode,
                assemblyNames = new[] { "LB.TweenHelper.PlayTests" }
            });
        }

        private static void RunTests(Filter filter)
        {
            _api = ScriptableObject.CreateInstance<TestRunnerApi>();
            _callbacks = new TestCallbacks();
            _api.RegisterCallbacks(_callbacks);
            _api.Execute(new ExecutionSettings(filter));
        }

        private sealed class TestCallbacks : ICallbacks
        {
            public void RunStarted(ITestAdaptor testsToRun) => Debug.Log($"{LogPrefix} Started {testsToRun.TestCaseCount} tests.");

            public void RunFinished(ITestResultAdaptor result)
            {
                Debug.Log($"{LogPrefix} Finished: {result.ResultState}, passed={result.PassCount}, failed={result.FailCount}, skipped={result.SkipCount}.");
                _api.UnregisterCallbacks(this);
                Object.DestroyImmediate(_api);
                _api = null;
                _callbacks = null;
            }

            public void TestStarted(ITestAdaptor test)
            {
                if (!test.HasChildren) Debug.Log($"{LogPrefix} Running: {test.FullName}");
            }

            public void TestFinished(ITestResultAdaptor result)
            {
                if (!result.HasChildren && result.ResultState != "Passed")
                {
                    Debug.LogError($"{LogPrefix} {result.FullName}: {result.ResultState}\n{result.Message}\n{result.StackTrace}");
                }
            }
        }
    }
}
