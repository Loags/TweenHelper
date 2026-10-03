using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace LB.TweenHelper.Installation.Editor
{
    [InitializeOnLoad]
    internal static class TweenHelperDependencySetup
    {
        internal const string Version = "1.3.0-rc.5";
        private const string DependencySymbol = "TWEEN_HELPER_DEPENDENCIES_READY";
        private static readonly Version MinimumDotweenVersion = new Version(1, 3, 30);

        static TweenHelperDependencySetup() => ScheduleRefresh();

        internal static void ScheduleRefresh()
        {
            EditorApplication.delayCall -= Refresh;
            EditorApplication.delayCall += Refresh;
        }

        [MenuItem("Tools/Tween Helper/Validate/Dependencies", false, 19)]
        private static void Validate()
        {
            Refresh();
            bool ready = GetStatus(out string message);
            EditorUtility.DisplayDialog(ready ? "Tween Helper Dependencies Ready" : "Tween Helper Dependencies Required", message, "OK");
        }

        private static void Refresh()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                ScheduleRefresh();
                return;
            }

            NamedBuildTarget target = NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
            var symbols = new List<string>(PlayerSettings.GetScriptingDefineSymbols(target).Split(';', StringSplitOptions.RemoveEmptyEntries));
            bool ready = GetStatus(out _);
            bool changed = ready ? !symbols.Contains(DependencySymbol) : symbols.Contains(DependencySymbol);
            if (!changed) return;
            if (ready) symbols.Add(DependencySymbol);
            else symbols.RemoveAll(symbol => symbol == DependencySymbol);
            PlayerSettings.SetScriptingDefineSymbols(target, string.Join(";", symbols));
        }

        private static bool GetStatus(out string message)
        {
            var missing = new List<string>();
            Type dotween = Type.GetType("DG.Tweening.DOTween, DOTween");
            bool hasDotweenFile = AssetDatabase.FindAssets("DOTween").Any(guid => AssetDatabase.GUIDToAssetPath(guid).EndsWith("/DOTween.dll", StringComparison.OrdinalIgnoreCase));
            bool hasEditorLibrary = Type.GetType("DG.DOTweenEditor.DOTweenEditorPreview, DOTweenEditor") != null;
            bool hasModuleLoader = AssetDatabase.FindAssets("DOTweenModuleUtils").Any(guid => AssetDatabase.GUIDToAssetPath(guid).EndsWith("/DOTweenModuleUtils.cs", StringComparison.OrdinalIgnoreCase));
            string runtimeVersion = dotween?.GetField("Version")?.GetValue(null) as string ?? dotween?.GetProperty("Version")?.GetValue(null) as string;
            if (!hasDotweenFile || !hasEditorLibrary || !hasModuleLoader || !System.Version.TryParse(runtimeVersion, out Version version) || version < MinimumDotweenVersion)
            {
                missing.Add("Install DOTween Free 1.3.030 or newer separately, including its core, Editor libraries and module loader, and run its Setup DOTween utility.");
            }
            if (Type.GetType("UnityEngine.UI.Graphic, UnityEngine.UI") == null || Type.GetType("TMPro.TMP_Text, Unity.TextMeshPro") == null)
            {
                missing.Add("Install Unity UI (uGUI), including TextMesh Pro, through Unity Package Manager.");
            }

            bool ready = missing.Count == 0;
            message = ready
                ? $"DOTween runtime {runtimeVersion}, its module loader and Unity UI/TextMesh Pro are available. Tween Helper does not require DOTween's optional UI/Sprite extensions or generated module assembly definitions. Import TMP Essential Resources before opening the sample gallery."
                : string.Join("\n\n", missing) + "\n\nTween Helper's runtime, Editor tools and samples remain disabled until these dependencies are ready. Existing project scripting symbols are preserved.";
            return ready;
        }
    }

    internal sealed class TweenHelperDependencyPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) => TweenHelperDependencySetup.ScheduleRefresh();
    }
}
