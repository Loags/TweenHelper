using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace LB.TweenHelper.Installation.Editor
{
    [InitializeOnLoad]
    internal static class TweenHelperDependencySetup
    {
        internal const string Version = "1.3.0-rc.5";
        private const string DependencySymbol = "TWEEN_HELPER_DEPENDENCIES_READY";
        private static readonly Version MinimumDotweenVersion = new Version(1, 3, 30);
        internal static event Action StatusChanged;

        static TweenHelperDependencySetup()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.EnteredEditMode) ScheduleRefresh();
            };
            ScheduleRefresh();
        }

        internal static void ScheduleRefresh()
        {
            EditorApplication.delayCall -= Refresh;
            EditorApplication.delayCall += Refresh;
        }

        [MenuItem("Tools/Tween Helper/Validate/Dependencies", false, 19)]
        private static void Validate()
        {
            Refresh();
            TweenHelperInstallationWindow.Open();
        }

        internal static void Refresh()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                ScheduleRefresh();
                return;
            }

            NamedBuildTarget target = NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
            var symbols = new List<string>(PlayerSettings.GetScriptingDefineSymbols(target).Split(';', StringSplitOptions.RemoveEmptyEntries));
            bool ready = ReadStatus().CoreReady;
            bool changed = ready ? !symbols.Contains(DependencySymbol) : symbols.Contains(DependencySymbol);
            StatusChanged?.Invoke();
            if (!changed)
            {
                TweenHelperInstallationWindow.OpenOnce();
                return;
            }
            if (ready) symbols.Add(DependencySymbol);
            else symbols.RemoveAll(symbol => symbol == DependencySymbol);
            PlayerSettings.SetScriptingDefineSymbols(target, string.Join(";", symbols));
        }

        internal static TweenHelperDependencyStatus ReadStatus()
        {
            Type dotween = Type.GetType("DG.Tweening.DOTween, DOTween");
            bool hasDotweenFile = AssetDatabase.FindAssets("DOTween").Any(guid => AssetDatabase.GUIDToAssetPath(guid).EndsWith("/DOTween.dll", StringComparison.OrdinalIgnoreCase));
            bool hasEditorLibrary = Type.GetType("DG.DOTweenEditor.DOTweenEditorPreview, DOTweenEditor") != null;
            bool hasModuleLoader = AssetDatabase.FindAssets("DOTweenModuleUtils").Any(guid => AssetDatabase.GUIDToAssetPath(guid).EndsWith("/DOTweenModuleUtils.cs", StringComparison.OrdinalIgnoreCase));
            string runtimeVersion = dotween?.GetField("Version")?.GetValue(null) as string ?? dotween?.GetProperty("Version")?.GetValue(null) as string;
            bool dotweenReady = hasDotweenFile && hasEditorLibrary && hasModuleLoader && System.Version.TryParse(runtimeVersion, out Version version) && version >= MinimumDotweenVersion;
            bool uiReady = Type.GetType("UnityEngine.UI.Graphic, UnityEngine.UI") != null && Type.GetType("TMPro.TMP_Text, Unity.TextMeshPro") != null;
            Type settingsType = Type.GetType("TMPro.TMP_Settings, Unity.TextMeshPro");
            Type fontType = Type.GetType("TMPro.TMP_FontAsset, Unity.TextMeshPro");
            string fontPath = AssetDatabase.GUIDToAssetPath("8f586378b4e144a9851e7b34d9b748ee");
            bool resourcesReady = uiReady && settingsType != null && fontType != null && Resources.Load("TMP Settings", settingsType) != null && !string.IsNullOrEmpty(fontPath) && AssetDatabase.LoadAssetAtPath(fontPath, fontType) != null;
            return new TweenHelperDependencyStatus(runtimeVersion, hasEditorLibrary, dotweenReady, uiReady, resourcesReady);
        }
    }

    internal sealed class TweenHelperDependencyStatus
    {
        internal string DotweenVersion { get; }
        internal bool HasDotweenEditor { get; }
        internal bool DotweenReady { get; }
        internal bool UiReady { get; }
        internal bool ResourcesReady { get; }
        internal bool CoreReady => DotweenReady && UiReady;
        internal bool SamplesReady => CoreReady && ResourcesReady;

        internal TweenHelperDependencyStatus(string dotweenVersion, bool hasDotweenEditor, bool dotweenReady, bool uiReady, bool resourcesReady)
        {
            DotweenVersion = dotweenVersion;
            HasDotweenEditor = hasDotweenEditor;
            DotweenReady = dotweenReady;
            UiReady = uiReady;
            ResourcesReady = resourcesReady;
        }
    }

    internal sealed class TweenHelperDependencyPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) => TweenHelperDependencySetup.ScheduleRefresh();
    }
}
