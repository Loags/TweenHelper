using UnityEditor;
using UnityEngine;

namespace LB.TweenHelper.Setup.Editor
{
    [InitializeOnLoad]
    internal static class TweenHelperSetupBootstrapper
    {
        private const string PackageVersion = "1.0.0";
        private const string SessionKey = "LB.TweenHelper.Setup.AutoOpenScheduled";

        static TweenHelperSetupBootstrapper()
        {
            if (Application.isBatchMode || SessionState.GetBool(SessionKey, false)) return;

            SessionState.SetBool(SessionKey, true);
            EditorApplication.delayCall += TryOpenSetupWindow;
        }

        internal static string GetDoNotShowAgainKey()
        {
            string projectId = Hash128.Compute(Application.dataPath).ToString();
            return $"LB.TweenHelper.Setup.DoNotShow.{projectId}.{PackageVersion}";
        }

        private static void TryOpenSetupWindow()
        {
            if (Application.isBatchMode || EditorPrefs.GetBool(GetDoNotShowAgainKey(), false)) return;

            if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.delayCall += TryOpenSetupWindow;
                return;
            }

            TweenHelperSetupWindow.OpenAutomatically();
        }
    }
}
