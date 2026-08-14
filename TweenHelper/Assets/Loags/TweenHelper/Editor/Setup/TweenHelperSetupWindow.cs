using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace LB.TweenHelper.Setup.Editor
{
    public sealed class TweenHelperSetupWindow : EditorWindow
    {
        private const string PackageVersion = "1.1.0";
        private const string SupportEmail = "Info@Loags.de";
        private const string DotweenUrl = "https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676";
        private const string DocumentationPath = "Assets/Loags/TweenHelper/Documentation/Installation.md";
        private const string UxmlPath = "Assets/Loags/TweenHelper/Editor/Setup/TweenHelperSetupWindow.uxml";
        private const string StylePath = "Assets/Loags/TweenHelper/Editor/Setup/TweenHelperSetupWindow.uss";
        private const string LogoPath = "Assets/Loags/TweenHelper/Editor/Setup/Branding/TweenHelperLogo-v2.png";
        private const string ValidatedDotweenPackageVersionText = "1.2.825";
        private const string ValidatedDotweenRuntimeVersionText = "1.3.030";
        private const int MaximumTagCount = 5;

        private static readonly Version ValidatedDotweenRuntimeVersion = new Version(1, 3, 30);

        private static readonly string[] AvailableTags =
        {
            "Bug", "Feature Request", "Installation", "Builder API", "Presets", "UI",
            "2D", "3D", "URP", "Editor Tools", "Performance", "Documentation"
        };

        private readonly List<Toggle> _tagToggles = new List<Toggle>();

        private Label _dotweenStatus;
        private Label _urpStatus;
        private Label _uguiStatus;
        private Label _tmpStatus;
        private Label _tagCount;
        private Label _templateHint;
        private Label _supportStatus;
        private TextField _userEmail;
        private TextField _message;
        private Button _useTemplateButton;
        private Toggle _includeTweenHelperVersion;
        private Toggle _includeUnityVersion;
        private Toggle _includeOperatingSystem;
        private Toggle _includeRenderPipeline;
        private Toggle _doNotShowAgain;
        private string _suggestedTemplate = string.Empty;
        private bool _showingTemplatePreview;

        [MenuItem("Tools/Tween Helper/Setup & Support", false, 0)]
        public static void Open()
        {
            var window = GetWindow<TweenHelperSetupWindow>();
            window.titleContent = new GUIContent("Tween Helper Setup");
            window.minSize = new Vector2(640f, 680f);
            window.Show();
        }

        internal static void OpenAutomatically()
        {
            Open();
            EditorPrefs.SetBool(TweenHelperSetupBootstrapper.GetDoNotShowAgainKey(), true);
        }

        public void CreateGUI()
        {
            VisualTreeAsset layout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(StylePath);

            if (layout == null)
            {
                rootVisualElement.Add(new HelpBox("Tween Helper setup UI could not be loaded. Reimport the package and reopen this window.", HelpBoxMessageType.Error));
                return;
            }

            layout.CloneTree(rootVisualElement);
            if (styleSheet != null) rootVisualElement.styleSheets.Add(styleSheet);

            BindControls();
            BuildTagToggles();
            RegisterCallbacks();
            LoadLogo();
            RefreshInstallationStatus();
        }

        private void BindControls()
        {
            _dotweenStatus = rootVisualElement.Q<Label>("dotween-status");
            _urpStatus = rootVisualElement.Q<Label>("urp-status");
            _uguiStatus = rootVisualElement.Q<Label>("ugui-status");
            _tmpStatus = rootVisualElement.Q<Label>("tmp-status");
            _tagCount = rootVisualElement.Q<Label>("tag-count");
            _templateHint = rootVisualElement.Q<Label>("template-hint");
            _supportStatus = rootVisualElement.Q<Label>("support-status");
            _userEmail = rootVisualElement.Q<TextField>("user-email");
            _message = rootVisualElement.Q<TextField>("message");
            _message.verticalScrollerVisibility = ScrollerVisibility.Auto;
            _useTemplateButton = rootVisualElement.Q<Button>("use-template-button");
            _includeTweenHelperVersion = rootVisualElement.Q<Toggle>("include-tweenhelper-version");
            _includeUnityVersion = rootVisualElement.Q<Toggle>("include-unity-version");
            _includeOperatingSystem = rootVisualElement.Q<Toggle>("include-operating-system");
            _includeRenderPipeline = rootVisualElement.Q<Toggle>("include-render-pipeline");
            _doNotShowAgain = rootVisualElement.Q<Toggle>("do-not-show-again");
            _doNotShowAgain.value = EditorPrefs.GetBool(TweenHelperSetupBootstrapper.GetDoNotShowAgainKey(), false);
        }

        private void BuildTagToggles()
        {
            VisualElement tagContainer = rootVisualElement.Q<VisualElement>("tag-container");
            foreach (string tag in AvailableTags)
            {
                var toggle = new Toggle(tag);
                toggle.AddToClassList("tag-toggle");
                toggle.RegisterValueChangedCallback(evt => OnTagChanged(toggle, evt.newValue));
                _tagToggles.Add(toggle);
                tagContainer.Add(toggle);
            }

            UpdateTagCount();
            UpdateMessageTemplate();
        }

        private void RegisterCallbacks()
        {
            rootVisualElement.Q<Button>("refresh-button").clicked += RefreshInstallationStatus;
            rootVisualElement.Q<Button>("dotween-button").clicked += () => Application.OpenURL(DotweenUrl);
            rootVisualElement.Q<Button>("package-manager-button").clicked += OpenPackageManager;
            rootVisualElement.Q<Button>("tmp-button").clicked += OpenTmpImporter;
            rootVisualElement.Q<Button>("documentation-button").clicked += OpenDocumentation;
            rootVisualElement.Q<Button>("preset-browser-button").clicked += OpenPresetBrowser;
            rootVisualElement.Q<Button>("prepare-email-button").clicked += PrepareEmail;
            rootVisualElement.Q<Button>("copy-report-button").clicked += CopyReport;
            _useTemplateButton.clicked += UseSuggestedTemplate;
            _message.RegisterValueChangedCallback(OnMessageChanged);
            _message.RegisterCallback<KeyDownEvent>(OnMessageKeyDown, TrickleDown.TrickleDown);
            _message.RegisterCallback<ExecuteCommandEvent>(OnMessageCommand, TrickleDown.TrickleDown);
            _message.RegisterCallback<FocusOutEvent>(_ => RestoreTemplatePreviewIfEmpty());
            _doNotShowAgain.RegisterValueChangedCallback(evt => EditorPrefs.SetBool(TweenHelperSetupBootstrapper.GetDoNotShowAgainKey(), evt.newValue));
        }

        private void LoadLogo()
        {
            Texture2D logo = AssetDatabase.LoadAssetAtPath<Texture2D>(LogoPath);
            rootVisualElement.Q<Image>("brand-logo").image = logo;
            rootVisualElement.Q<Label>("version-label").text = $"Version {PackageVersion}";
        }

        private void RefreshInstallationStatus()
        {
            SetStatus(_dotweenStatus, GetDotweenStatus());
            SetStatus(_urpStatus, GetRenderPipelineStatus());
            SetStatus(_uguiStatus, GetPackageStatus("com.unity.ugui", "Unity UI is installed", "Unity UI is not installed"));
            bool hasTmpEssentials = AssetDatabase.FindAssets("t:TMP_Settings").Length > 0;
            SetStatus(_tmpStatus, new SetupStatus(hasTmpEssentials, hasTmpEssentials ? "TextMesh Pro essentials are available" : "Import TextMesh Pro Essential Resources"));
        }

        private static SetupStatus GetDotweenStatus()
        {
            Type dotweenType = Type.GetType("DG.Tweening.DOTween, DOTween");
            if (dotweenType == null) return new SetupStatus(false, $"Install DOTween separately; validated package: {ValidatedDotweenPackageVersionText}");

            PropertyInfo versionProperty = dotweenType.GetProperty("Version", BindingFlags.Public | BindingFlags.Static);
            FieldInfo versionField = dotweenType.GetField("Version", BindingFlags.Public | BindingFlags.Static);
            string versionText = versionProperty?.GetValue(null) as string ?? versionField?.GetValue(null) as string;
            bool versionValid = Version.TryParse(versionText, out Version version) && version >= ValidatedDotweenRuntimeVersion;
            Type modulesType = Type.GetType("DG.Tweening.DOTweenModuleUI, DOTween.Modules");

            if (!versionValid) return new SetupStatus(false, $"DOTween runtime {versionText ?? "unknown"} is older than tested runtime {ValidatedDotweenRuntimeVersionText}");
            if (modulesType == null) return new SetupStatus(false, $"DOTween {versionText} found; run Setup DOTween to generate modules");
            return new SetupStatus(true, $"DOTween runtime {versionText} is ready; validated package {ValidatedDotweenPackageVersionText}");
        }

        private static SetupStatus GetRenderPipelineStatus()
        {
            RenderPipelineAsset activePipeline = GraphicsSettings.currentRenderPipeline;
            if (activePipeline == null) return new SetupStatus(true, "Built-in Render Pipeline is active and supported");
            string pipelineName = activePipeline.GetType().Name;
            if (pipelineName.IndexOf("Universal", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return new SetupStatus(true, "Universal Render Pipeline is active and supported");
            }

            return new SetupStatus(false, $"{pipelineName} is active; HDRP and custom pipelines are untested");
        }

        private static SetupStatus GetPackageStatus(string packageName, string installedMessage, string missingMessage)
        {
            bool installed = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages().Any(package => package.name == packageName);
            return new SetupStatus(installed, installed ? installedMessage : missingMessage);
        }

        private static void SetStatus(Label label, SetupStatus status)
        {
            label.text = status.Message;
            label.EnableInClassList("status-ready", status.IsReady);
            label.EnableInClassList("status-action", !status.IsReady);
        }

        private void OnTagChanged(Toggle changedToggle, bool selected)
        {
            if (selected && _tagToggles.Count(toggle => toggle.value) > MaximumTagCount)
            {
                changedToggle.SetValueWithoutNotify(false);
                _supportStatus.text = $"Choose up to {MaximumTagCount} tags.";
                _supportStatus.EnableInClassList("support-error", true);
            }

            UpdateTagCount();
            UpdateMessageTemplate();
        }

        private void UpdateTagCount()
        {
            int selectedCount = _tagToggles.Count(toggle => toggle.value);
            _tagCount.text = $"{selectedCount}/{MaximumTagCount} selected";
        }

        private void OnMessageChanged(ChangeEvent<string> evt)
        {
            if (_showingTemplatePreview) return;
            RestoreTemplatePreviewIfEmpty();
        }

        private void UpdateMessageTemplate()
        {
            List<string> tags = GetSelectedTags();
            bool hasTags = tags.Count > 0;
            _useTemplateButton.SetEnabled(hasTags);

            if (!hasTags)
            {
                _suggestedTemplate = "Select one or more tags to see a suggested message template.";
                _templateHint.text = "Choose tags to create a relevant starting point.";
                RefreshTemplatePreview();
                return;
            }

            _suggestedTemplate = BuildSuggestedTemplate(tags);
            string templateName = GetTemplateName(tags);
            _templateHint.text = $"Scrollable {templateName} preview based on: {string.Join(", ", tags)}";
            RefreshTemplatePreview();
        }

        private void UseSuggestedTemplate()
        {
            List<string> tags = GetSelectedTags();
            if (tags.Count == 0) return;

            if (!_showingTemplatePreview && !string.IsNullOrWhiteSpace(_message.value) &&
                !EditorUtility.DisplayDialog("Replace Current Message?", "Using the suggested template will replace the message already entered.", "Replace", "Cancel"))
            {
                return;
            }

            HideTemplatePreview();
            _message.value = _suggestedTemplate;
            _message.Focus();
            SetSupportSuccess("Suggested template inserted. Replace the bracketed prompts with your details.");
        }

        private void OnMessageKeyDown(KeyDownEvent evt)
        {
            if (!_showingTemplatePreview || !IsEditingKey(evt)) return;
            HideTemplatePreview();
        }

        private void OnMessageCommand(ExecuteCommandEvent evt)
        {
            if (!_showingTemplatePreview) return;
            if (evt.commandName == "Paste" || evt.commandName == "Cut" || evt.commandName == "Delete") HideTemplatePreview();
        }

        private static bool IsEditingKey(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Backspace || evt.keyCode == KeyCode.Delete || evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter) return true;
            if ((evt.ctrlKey || evt.commandKey) && (evt.keyCode == KeyCode.V || evt.keyCode == KeyCode.X)) return true;
            return evt.character != '\0' && !char.IsControl(evt.character);
        }

        private void RefreshTemplatePreview()
        {
            if (_showingTemplatePreview || string.IsNullOrEmpty(_message.value)) ShowTemplatePreview();
        }

        private void RestoreTemplatePreviewIfEmpty()
        {
            if (!_showingTemplatePreview && string.IsNullOrEmpty(_message.value)) ShowTemplatePreview();
        }

        private void ShowTemplatePreview()
        {
            _showingTemplatePreview = true;
            _message.AddToClassList("message-template-preview");
            _message.SetValueWithoutNotify(_suggestedTemplate);
        }

        private void HideTemplatePreview()
        {
            if (!_showingTemplatePreview) return;

            _showingTemplatePreview = false;
            _message.RemoveFromClassList("message-template-preview");
            _message.SetValueWithoutNotify(string.Empty);
        }

        private List<string> GetSelectedTags() => _tagToggles.Where(toggle => toggle.value).Select(toggle => toggle.label).ToList();

        private static string GetTemplateName(ICollection<string> tags)
        {
            List<string> reportTypes = tags.Where(IsReportTypeTag).ToList();
            if (reportTypes.Count > 1) return "combined support template";
            if (reportTypes.Count == 0) return "support template";

            if (reportTypes[0] == "Bug") return "bug report template";
            if (reportTypes[0] == "Feature Request") return "feature request template";
            if (reportTypes[0] == "Installation") return "installation template";
            if (reportTypes[0] == "Performance") return "performance report template";
            if (reportTypes[0] == "Documentation") return "documentation feedback template";
            return "support template";
        }

        private static string BuildSuggestedTemplate(ICollection<string> tags)
        {
            var sections = new List<string>();
            foreach (string tag in tags)
            {
                if (tag == "Bug")
                {
                    sections.Add("BUG REPORT\n\nWhat happened:\n[Describe the issue.]\n\nSteps to reproduce:\n1. [First step]\n2. [Second step]\n\nExpected result:\n[What should happen?]\n\nActual result:\n[What happened instead?]");
                }
                else if (tag == "Feature Request")
                {
                    sections.Add("FEATURE REQUEST\n\nRequested feature:\n[Describe what you would like Tween Helper to do.]\n\nCurrent workflow or problem:\n[What is difficult today?]\n\nSuggested behavior:\n[How should the feature work?]\n\nBenefit:\n[How would this improve your project?]");
                }
                else if (tag == "Installation")
                {
                    sections.Add("INSTALLATION\n\nInstallation issue:\n[Describe where setup stops or behaves unexpectedly.]\n\nSteps already completed:\n[List what you installed or configured.]\n\nMessage shown by Unity:\n[Paste the relevant message, without private project data.]\n\nExpected result:\n[What were you trying to achieve?]");
                }
                else if (tag == "Performance")
                {
                    sections.Add("PERFORMANCE\n\nPerformance concern:\n[Describe the slowdown or unexpected resource usage.]\n\nWhen it occurs:\n[Explain the relevant Tween Helper workflow.]\n\nExpected performance:\n[Describe the expected behavior.]\n\nAdditional observations:\n[Add any repeatable details.]");
                }
                else if (tag == "Documentation")
                {
                    sections.Add("DOCUMENTATION\n\nPage or topic:\n[Name the relevant documentation.]\n\nWhat was unclear or missing:\n[Describe the problem.]\n\nSuggested improvement:\n[Explain what information would help.]");
                }
            }

            if (sections.Count == 0)
            {
                sections.Add("QUESTION OR FEEDBACK\n\nGoal:\n[Describe what you are trying to achieve.]\n\nCurrent behavior:\n[Explain what happens now.]\n\nDesired result:\n[Explain what you need help with.]");
            }

            string template = string.Join("\n\n--------------------\n\n", sections);
            List<string> contextTags = tags.Where(tag => !IsReportTypeTag(tag)).ToList();
            return contextTags.Count == 0 ? template : $"{template}\n\n--------------------\n\nRELEVANT AREAS\n{string.Join(", ", contextTags)}";
        }

        private static bool IsReportTypeTag(string tag) => tag == "Bug" || tag == "Feature Request" || tag == "Installation" || tag == "Performance" || tag == "Documentation";

        private void PrepareEmail()
        {
            if (!TryBuildReport(out string subject, out string body)) return;

            EditorGUIUtility.systemCopyBuffer = body;
            string mailto = $"mailto:{SupportEmail}?subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(body)}";
            Application.OpenURL(mailto);
            SetSupportSuccess("The report was copied and opened in your email client. Review it, then press Send.");
        }

        private void CopyReport()
        {
            if (!TryBuildReport(out string subject, out string body)) return;

            EditorGUIUtility.systemCopyBuffer = $"Subject: {subject}\n\n{body}";
            SetSupportSuccess("The complete report was copied to the clipboard.");
        }

        private bool TryBuildReport(out string subject, out string body)
        {
            subject = string.Empty;
            body = string.Empty;

            if (!IsValidEmail(_userEmail.value))
            {
                SetSupportError("Enter a valid email address so support can reply.");
                return false;
            }

            List<string> tags = GetSelectedTags();
            if (tags.Count == 0)
            {
                SetSupportError("Select at least one report tag.");
                return false;
            }

            if (_showingTemplatePreview || string.IsNullOrWhiteSpace(_message.value))
            {
                SetSupportError("Describe the bug, feature request, or question.");
                return false;
            }

            subject = $"[Tween Helper][{string.Join("][", tags)}]";
            var builder = new StringBuilder();
            builder.AppendLine("Tween Helper support request");
            builder.AppendLine();
            builder.AppendLine($"Reply email: {_userEmail.value.Trim()}");
            builder.AppendLine($"Tags: {string.Join(", ", tags)}");
            builder.AppendLine();
            builder.AppendLine("Message:");
            builder.AppendLine(_message.value.Trim());
            AppendOptionalMetadata(builder);
            body = builder.ToString();
            return true;
        }

        private void AppendOptionalMetadata(StringBuilder builder)
        {
            var metadata = new List<string>();
            if (_includeTweenHelperVersion.value) metadata.Add($"Tween Helper: {PackageVersion}");
            if (_includeUnityVersion.value) metadata.Add($"Unity: {Application.unityVersion}");
            if (_includeOperatingSystem.value) metadata.Add($"Operating system: {SystemInfo.operatingSystem}");
            if (_includeRenderPipeline.value) metadata.Add($"Render pipeline: {GetRenderPipelineName()}");
            if (metadata.Count == 0) return;

            builder.AppendLine();
            builder.AppendLine("User-selected Tween Helper environment information:");
            foreach (string line in metadata) builder.AppendLine($"- {line}");
        }

        private static string GetRenderPipelineName()
        {
            var pipeline = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
            return pipeline == null ? "Built-in Render Pipeline" : pipeline.GetType().Name;
        }

        private static bool IsValidEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            try
            {
                var address = new MailAddress(value.Trim());
                return string.Equals(address.Address, value.Trim(), StringComparison.OrdinalIgnoreCase);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void SetSupportError(string message)
        {
            _supportStatus.text = message;
            _supportStatus.EnableInClassList("support-error", true);
            _supportStatus.EnableInClassList("support-success", false);
        }

        private void SetSupportSuccess(string message)
        {
            _supportStatus.text = message;
            _supportStatus.EnableInClassList("support-error", false);
            _supportStatus.EnableInClassList("support-success", true);
        }

        private static void OpenPackageManager()
        {
            EditorApplication.ExecuteMenuItem("Window/Package Management/Package Manager");
        }

        private static void OpenTmpImporter()
        {
            EditorApplication.ExecuteMenuItem("Window/TextMeshPro/Import TMP Essential Resources");
        }

        private static void OpenDocumentation()
        {
            UnityEngine.Object documentation = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(DocumentationPath);
            if (documentation != null) AssetDatabase.OpenAsset(documentation);
        }

        private static void OpenPresetBrowser()
        {
            if (!EditorApplication.ExecuteMenuItem("Tools/Tween Helper/Preset Browser"))
            {
                EditorUtility.DisplayDialog("Preset Browser Unavailable", "Finish the required DOTween setup, allow Unity to compile, and try again.", "OK");
            }
        }

        private readonly struct SetupStatus
        {
            public SetupStatus(bool isReady, string message)
            {
                IsReady = isReady;
                Message = message;
            }

            public bool IsReady { get; }
            public string Message { get; }
        }
    }
}
