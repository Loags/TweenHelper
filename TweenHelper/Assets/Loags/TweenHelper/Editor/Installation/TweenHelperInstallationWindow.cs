using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace LB.TweenHelper.Installation.Editor
{
    public sealed class TweenHelperInstallationWindow : EditorWindow
    {
        private const string AssetRoot = "Assets/Loags/TweenHelper/";
        private const string GalleryPath = AssetRoot + "Samples/TweenHelper Demos/Scenes/TweenHelperAnimationGallery.unity";
        private const string DotweenUrl = "https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676";
        private static string ShownKey => $"LB.TweenHelper.Installation.Shown.{Hash128.Compute(Application.dataPath)}.{TweenHelperDependencySetup.Version}";

        [MenuItem("Tools/Tween Helper/Setup", false, 0)]
        public static void Open()
        {
            var window = GetWindow<TweenHelperInstallationWindow>();
            window.titleContent = new GUIContent("Tween Helper Setup");
            window.minSize = new Vector2(580, 640);
            window.Show();
            EditorPrefs.SetBool(ShownKey, true);
        }

        internal static void OpenOnce()
        {
            if (Application.isBatchMode || EditorApplication.isPlayingOrWillChangePlaymode || EditorPrefs.GetBool(ShownKey, false)) return;
            Open();
        }

        private void OnEnable()
        {
            TweenHelperDependencySetup.StatusChanged += RefreshStatus;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private void OnDisable()
        {
            TweenHelperDependencySetup.StatusChanged -= RefreshStatus;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        }

        public void CreateGUI()
        {
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetRoot + "Editor/Installation/TweenHelperInstallationWindow.uxml");
            tree.CloneTree(rootVisualElement);
            rootVisualElement.Q<Image>("brand-logo").image = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetRoot + "Editor/TweenHelperLogo.png");
            rootVisualElement.Q<Label>("version-label").text = $"Version {TweenHelperDependencySetup.Version}  |  Unity {Application.unityVersion}";
            rootVisualElement.Q<Button>("refresh-button").clicked += TweenHelperDependencySetup.Refresh;
            rootVisualElement.Q<Button>("dotween-store-button").clicked += () => Application.OpenURL(DotweenUrl);
            rootVisualElement.Q<Button>("dotween-setup-button").clicked += () => OpenMenu("Tools/Demigiant/DOTween Utility Panel", "Install DOTween's Editor libraries, then open its Utility Panel and run Setup DOTween.");
            rootVisualElement.Q<Button>("ui-button").clicked += () => UnityEditor.PackageManager.UI.Window.Open("com.unity.ugui");
            rootVisualElement.Q<Button>("tmp-button").clicked += () => OpenMenu("Window/TextMeshPro/Import TMP Essential Resources", "Install Unity UI first, then use Window > TextMeshPro > Import TMP Essential Resources.");
            rootVisualElement.Q<Button>("documentation-button").clicked += () => AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<TextAsset>(AssetRoot + "Documentation/Installation.md"));
            rootVisualElement.Q<Button>("browser-button").clicked += () => OpenMenu("Tools/Tween Helper/Preset Browser", "Complete the dependency steps and wait for Unity to finish compiling.");
            rootVisualElement.Q<Button>("gallery-button").clicked += OpenGallery;
            RefreshStatus();
        }

        private void OnFocus()
        {
            if (!EditorApplication.isCompiling && !EditorApplication.isUpdating) RefreshStatus();
        }

        private void OnPlayModeChanged(PlayModeStateChange state) => RefreshStatus();

        private void RefreshStatus()
        {
            if (rootVisualElement.Q<Label>("setup-status") == null) return;
            TweenHelperDependencyStatus status = TweenHelperDependencySetup.ReadStatus();
            bool editing = !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            string installedVersion = string.IsNullOrEmpty(status.DotweenVersion) ? "Not installed" : $"Installed runtime: {status.DotweenVersion}";
            SetStatus("dotween-status", status.DotweenReady, status.DotweenReady ? $"Ready — runtime {status.DotweenVersion}, Editor library and module loader found." : $"{installedVersion}. Requires runtime 1.3.030 or newer, Editor library and module loader.");
            SetStatus("ui-status", status.UiReady, status.UiReady ? "Ready — Unity UI and TextMesh Pro code are available." : "Install Unity UI (com.unity.ugui). In Unity 6 it includes TextMesh Pro.");
            SetStatus("tmp-status", status.ResourcesReady, status.ResourcesReady ? "Ready — TMP Settings and the gallery's Liberation Sans font are available." : "Import TMP Essential Resources, including settings and fonts. In the importer, keep all essentials selected and click Import.");
            SetStatus("setup-status", status.SamplesReady, status.SamplesReady ? "Setup complete. You're ready to explore Tween Helper." : "Complete the steps below before opening the sample gallery.");
            rootVisualElement.Q<Button>("dotween-store-button").text = status.DotweenReady ? "Check for updates" : "Get / update DOTween";
            rootVisualElement.Q<Button>("dotween-setup-button").SetEnabled(editing && status.HasDotweenEditor);
            rootVisualElement.Q<Button>("ui-button").SetEnabled(editing);
            rootVisualElement.Q<Button>("tmp-button").SetEnabled(editing && status.UiReady);
            rootVisualElement.Q<Button>("browser-button").SetEnabled(editing && status.SamplesReady);
            rootVisualElement.Q<Button>("gallery-button").SetEnabled(editing && status.SamplesReady);
        }

        private void SetStatus(string name, bool ready, string text)
        {
            Label label = rootVisualElement.Q<Label>(name);
            label.text = text;
            label.EnableInClassList("status-ready", ready);
            label.EnableInClassList("status-action", !ready);
        }

        private static void OpenMenu(string path, string instructions)
        {
            if (!EditorApplication.ExecuteMenuItem(path)) EditorUtility.DisplayDialog("Tween Helper Setup", instructions, "OK");
        }

        private void OpenGallery()
        {
            if (!TweenHelperDependencySetup.ReadStatus().SamplesReady || EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(GalleryPath);
        }
    }
}
