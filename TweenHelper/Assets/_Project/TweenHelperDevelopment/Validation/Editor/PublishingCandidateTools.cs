using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LB.TweenHelper.Editor
{
    public static class PublishingCandidateTools
    {
        [MenuItem("Tools/Tween Helper Dev/Publishing/Finalize Sample References")]
        public static void FinalizeSampleReferences()
        {
            const string path = "Assets/Loags/TweenHelper/Samples/TweenHelper Demos/Prefabs/UI/ProgressRewardDemo.prefab";
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                TweenPlayer player = root.GetComponent<TweenPlayer>();
                var image = player.Bindings[0].Target.GetComponent<UnityEngine.UI.Image>();
                image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                if (image.sprite == null) throw new System.InvalidOperationException("Built-in UI sprite was not found.");
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }

        [MenuItem("Tools/Tween Helper Dev/Publishing/Export Candidate")]
        public static void Export()
        {
            Directory.CreateDirectory("ReleaseArtifacts");
            string source = File.ReadAllText("Assets/Loags/TweenHelper/Editor/Setup/TweenHelperPackageInfo.cs");
            string version = System.Text.RegularExpressions.Regex.Match(source, "Version = \"([^\"]+)\"").Groups[1].Value;
            if (string.IsNullOrEmpty(version)) throw new System.InvalidOperationException("Package version is missing.");
            AssetDatabase.ExportPackage("Assets/Loags/TweenHelper", "ReleaseArtifacts/TweenHelper-" + version + ".unitypackage", ExportPackageOptions.Recurse);
            Debug.Log("Publishing candidate exported.");
        }

        [MenuItem("Tools/Tween Helper Dev/Publishing/Build Mono Smoke")]
        public static void BuildMono() => QueueBuild(ScriptingImplementation.Mono2x);

        [MenuItem("Tools/Tween Helper Dev/Publishing/Build IL2CPP Smoke")]
        public static void BuildIl2Cpp() => QueueBuild(ScriptingImplementation.IL2CPP);

        private static void QueueBuild(ScriptingImplementation backend)
        {
            void Run()
            {
                EditorApplication.update -= Run;
                Build(backend);
            }
            EditorApplication.update += Run;
        }

        private static void Build(ScriptingImplementation backend)
        {
            const string scenePath = "Assets/_Project/TweenHelperDevelopment/Validation/Scenes/PublishingSmoke.unity";
            if (!File.Exists(scenePath))
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                EditorSceneManager.SaveScene(scene, scenePath);
                EditorSceneManager.CloseScene(scene, true);
            }
            var target = NamedBuildTarget.Standalone;
            ScriptingImplementation previousBackend = PlayerSettings.GetScriptingBackend(target);
            ManagedStrippingLevel previousStripping = PlayerSettings.GetManagedStrippingLevel(target);
            try
            {
                PlayerSettings.SetScriptingBackend(target, backend);
                PlayerSettings.SetManagedStrippingLevel(target, backend == ScriptingImplementation.IL2CPP ? ManagedStrippingLevel.High : ManagedStrippingLevel.Disabled);
                string directory = "Builds/PublishingSmoke/" + backend;
                Directory.CreateDirectory(directory);
                BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = new[] { scenePath }, locationPathName = directory + "/TweenHelperSmoke.exe", target = BuildTarget.StandaloneWindows64, options = BuildOptions.DetailedBuildReport });
                Directory.CreateDirectory("ReleaseArtifacts/RoadmapValidation");
                File.WriteAllText("ReleaseArtifacts/RoadmapValidation/Build-" + backend + ".txt", $"Result: {report.summary.result}\nErrors: {report.summary.totalErrors}\nWarnings: {report.summary.totalWarnings}\nSize: {report.summary.totalSize}\nTime: {report.summary.totalTime}\nUnity: {Application.unityVersion}\nStripping: {PlayerSettings.GetManagedStrippingLevel(target)}\n");
                if (report.summary.result != BuildResult.Succeeded) throw new BuildFailedException("Publishing smoke build failed.");
            }
            finally
            {
                PlayerSettings.SetScriptingBackend(target, previousBackend);
                PlayerSettings.SetManagedStrippingLevel(target, previousStripping);
            }
        }
    }
}
