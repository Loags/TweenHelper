using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Toggleable console UI for filtering animation presets in the demo scene.
    /// Press Tab or click the button to open/close. Type commands to filter visible presets.
    /// </summary>
    public class PresetConsole : MonoBehaviour
    {
        private const float PanelHeight = 300f;
        private const string HelpText =
            "=== Preset Console ===\n" +
            "Commands:\n" +
            "  show <family...>  Show only specified families (e.g. \"show Pop Shake\")\n" +
            "  show all          Show all presets\n" +
            "  hide <family...>  Hide families (e.g. \"hide Jitter Wobble\")\n" +
            "  filter <text>     Show presets whose name contains <text>\n" +
            "  list              List all family names\n" +
            "  count             Show visible / total preset count\n" +
            "  status            Show current filter state\n" +
            "  reset             Reset all animations\n" +
            "  clear             Clear console output\n" +
            "  help              Show this help\n\n" +
            "Tab: autocomplete | Up/Down: command history\n" +
            "Escape or Tab on empty input to close.";

        private Canvas _canvas;
        private GameObject _panelObj;
        private TextMeshProUGUI _outputText;
        private TMP_InputField _inputField;
        private Button _toggleButton;
        private TextMeshProUGUI _toggleButtonLabel;
        private ScrollRect _scrollRect;
        private FlyCamera _flyCamera;

        private static readonly string[] Commands = { "show", "hide", "filter", "list", "count", "status", "reset", "clear", "help" };

        private bool _isOpen;
        private readonly HashSet<string> _hiddenFamilies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private string _filterText;
        private Coroutine _scrollCoroutine;

        private readonly List<string> _commandHistory = new List<string>();
        private int _historyIndex = -1;

        private List<string> _completionCandidates = new List<string>();
        private int _completionIndex = -1;
        private string _completionPrefix;

        private void Start()
        {
            CreateUI();
            _flyCamera = FindFirstObjectByType<FlyCamera>();
            SetPanelOpen(false);
        }

        private void Update()
        {
            if (_isOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                SetPanelOpen(false);
                return;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (_isOpen)
                {
                    var text = _inputField.text;
                    if (string.IsNullOrEmpty(text))
                    {
                        SetPanelOpen(false);
                    }
                    else
                    {
                        HandleAutoComplete();
                    }
                }
                else
                {
                    SetPanelOpen(true);
                }
            }

            if (_isOpen && _commandHistory.Count > 0)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (_historyIndex < 0)
                        _historyIndex = _commandHistory.Count - 1;
                    else if (_historyIndex > 0)
                        _historyIndex--;

                    SetInputText(_commandHistory[_historyIndex]);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (_historyIndex >= 0)
                    {
                        _historyIndex++;
                        if (_historyIndex >= _commandHistory.Count)
                        {
                            _historyIndex = -1;
                            SetInputText("");
                        }
                        else
                        {
                            SetInputText(_commandHistory[_historyIndex]);
                        }
                    }
                }
            }
        }

        private void SetPanelOpen(bool open)
        {
            _isOpen = open;
            _panelObj.SetActive(open);

            if (open)
            {
                _inputField.text = "";
                _inputField.ActivateInputField();
                _inputField.Select();
                if (_flyCamera != null) _flyCamera.enabled = false;
            }
            else
            {
                if (_flyCamera != null) _flyCamera.enabled = true;
            }

            _toggleButtonLabel.text = open ? "Console (Tab) [Open]" : "Console (Tab)";
        }

        private void OnSubmit(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            var trimmed = text.Trim();
            _commandHistory.Add(trimmed);
            _historyIndex = -1;

            ResetCompletion();
            _inputField.text = "";
            _inputField.ActivateInputField();

            ExecuteCommand(trimmed);
        }

        private void ExecuteCommand(string raw)
        {
            var parts = raw.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            var cmd = parts[0].ToLowerInvariant();
            var arg = parts.Length > 1 ? parts[1].Trim() : "";

            switch (cmd)
            {
                case "show":
                    HandleShow(arg);
                    break;
                case "hide":
                    HandleHide(arg);
                    break;
                case "filter":
                    HandleFilter(arg);
                    break;
                case "list":
                    HandleList();
                    break;
                case "count":
                    HandleCount();
                    break;
                case "status":
                    HandleStatus();
                    break;
                case "reset":
                    HandleReset();
                    break;
                case "clear":
                    _outputText.text = "";
                    break;
                case "help":
                    _outputText.text = HelpText;
                    ScrollToBottom();
                    break;
                default:
                    AppendOutput($"Unknown command: {cmd}. Type 'help' for usage.");
                    break;
            }
        }

        private void HandleShow(string arg)
        {
            if (string.IsNullOrWhiteSpace(arg))
            {
                AppendOutput("Usage: show <family...> or show all");
                return;
            }

            _filterText = null;

            if (arg.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                _hiddenFamilies.Clear();
                AppendOutput("Showing all presets.");
            }
            else
            {
                var spawner = PresetShowcaseSpawner.Instance;
                if (spawner == null) return;

                var allFamilies = spawner.GetAllFamilyNames();
                var tokens = arg.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var matched = new List<string>();
                var invalid = new List<string>();

                foreach (var token in tokens)
                {
                    var match = allFamilies.FirstOrDefault(f => f.Equals(token, StringComparison.OrdinalIgnoreCase));
                    if (match != null)
                        matched.Add(match);
                    else
                        invalid.Add(token);
                }

                if (matched.Count == 0)
                {
                    AppendOutput($"Unknown family: {string.Join(", ", invalid)}. Type 'list' to see available families.");
                    return;
                }

                _hiddenFamilies.Clear();
                var matchedSet = new HashSet<string>(matched, StringComparer.OrdinalIgnoreCase);
                foreach (var family in allFamilies)
                {
                    if (!matchedSet.Contains(family))
                        _hiddenFamilies.Add(family);
                }

                var msg = $"Showing families: {string.Join(", ", matched)}.";
                if (invalid.Count > 0)
                    msg += $" Unknown: {string.Join(", ", invalid)}.";
                AppendOutput(msg);
            }

            ApplyFilter();
        }

        private void HandleHide(string arg)
        {
            if (string.IsNullOrWhiteSpace(arg))
            {
                AppendOutput("Usage: hide <family...>");
                return;
            }

            _filterText = null;

            var spawner = PresetShowcaseSpawner.Instance;
            if (spawner == null) return;

            var allFamilies = spawner.GetAllFamilyNames();
            var tokens = arg.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var matched = new List<string>();
            var invalid = new List<string>();

            foreach (var token in tokens)
            {
                var match = allFamilies.FirstOrDefault(f => f.Equals(token, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                    matched.Add(match);
                else
                    invalid.Add(token);
            }

            if (matched.Count == 0)
            {
                AppendOutput($"Unknown family: {string.Join(", ", invalid)}. Type 'list' to see available families.");
                return;
            }

            foreach (var family in matched)
                _hiddenFamilies.Add(family);

            var msg = $"Hidden families: {string.Join(", ", matched)}.";
            if (invalid.Count > 0)
                msg += $" Unknown: {string.Join(", ", invalid)}.";
            AppendOutput(msg);
            ApplyFilter();
        }

        private void HandleFilter(string arg)
        {
            if (string.IsNullOrWhiteSpace(arg))
            {
                AppendOutput("Usage: filter <text>");
                return;
            }

            _hiddenFamilies.Clear();
            _filterText = arg;
            AppendOutput($"Filtering presets containing '{arg}'.");
            ApplyFilter();
        }

        private void HandleList()
        {
            var spawner = PresetShowcaseSpawner.Instance;
            if (spawner == null) return;

            var families = spawner.GetAllFamilyNames();
            AppendOutput("Families:\n  " + string.Join("\n  ", families));
        }

        private void HandleCount()
        {
            var spawner = PresetShowcaseSpawner.Instance;
            if (spawner == null) return;

            AppendOutput($"Showing {spawner.GetVisibleCount()} / {spawner.GetTotalCount()} presets.");
        }

        private void HandleStatus()
        {
            var spawner = PresetShowcaseSpawner.Instance;
            var lines = new List<string>();

            if (!string.IsNullOrEmpty(_filterText))
                lines.Add($"Filter active: '{_filterText}'");

            if (_hiddenFamilies.Count > 0)
                lines.Add($"Hidden families: {string.Join(", ", _hiddenFamilies.OrderBy(f => f))}");

            if (lines.Count == 0)
                lines.Add("No filters active (showing all).");

            if (spawner != null)
                lines.Add($"Showing {spawner.GetVisibleCount()} / {spawner.GetTotalCount()} presets.");

            AppendOutput(string.Join("\n", lines));
        }

        private void HandleReset()
        {
            var manager = AnimationResetManager.Instance;
            if (manager != null)
            {
                manager.ResetAll();
                AppendOutput("Reset all animations.");
            }
            else
            {
                AppendOutput("No animation reset manager found.");
            }
        }

        private void ApplyFilter()
        {
            var spawner = PresetShowcaseSpawner.Instance;
            if (spawner == null) return;

            spawner.RelayoutWithFilter(presetName =>
            {
                if (!string.IsNullOrEmpty(_filterText))
                {
                    return presetName.IndexOf(_filterText, StringComparison.OrdinalIgnoreCase) >= 0;
                }

                var family = spawner.GetPresetFamilyName(presetName);
                return family == null || !_hiddenFamilies.Contains(family);
            });
        }

        private void HandleAutoComplete()
        {
            var text = _inputField.text;

            // If we already have candidates and the prefix hasn't changed, cycle to next
            if (_completionCandidates.Count > 0 && _completionPrefix != null &&
                text.StartsWith(_completionPrefix, StringComparison.OrdinalIgnoreCase) &&
                _completionCandidates.Contains(text))
            {
                _completionIndex = (_completionIndex + 1) % _completionCandidates.Count;
                SetInputText(_completionCandidates[_completionIndex]);
                return;
            }

            // Build new candidate list
            _completionCandidates.Clear();
            _completionIndex = -1;

            var parts = text.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts.Length > 0 ? parts[0] : "";

            if (parts.Length <= 1 && !text.EndsWith(" "))
            {
                // Completing the command name
                _completionPrefix = "";
                foreach (var c in Commands)
                {
                    if (c.StartsWith(cmd, StringComparison.OrdinalIgnoreCase))
                    {
                        _completionCandidates.Add(c);
                    }
                }
            }
            else
            {
                // Completing the argument
                var cmdLower = cmd.ToLowerInvariant();
                var argPartial = parts.Length > 1 ? parts[1] : "";
                _completionPrefix = cmd + " ";

                if (cmdLower == "show" || cmdLower == "hide")
                {
                    var candidates = GetFamilyCandidates(argPartial);
                    if (cmdLower == "show")
                    {
                        // "all" is a valid argument for show
                        if ("all".StartsWith(argPartial, StringComparison.OrdinalIgnoreCase))
                        {
                            candidates.Insert(0, "all");
                        }
                    }

                    foreach (var c in candidates)
                    {
                        _completionCandidates.Add(cmd + " " + c);
                    }
                }
                else if (cmdLower == "filter")
                {
                    // Complete with preset names
                    var spawner = PresetShowcaseSpawner.Instance;
                    if (spawner != null)
                    {
                        var presetNames = spawner.GetAllPresetNames();
                        foreach (var name in presetNames)
                        {
                            if (name.StartsWith(argPartial, StringComparison.OrdinalIgnoreCase))
                            {
                                _completionCandidates.Add(cmd + " " + name);
                            }
                        }
                    }
                }
            }

            if (_completionCandidates.Count > 0)
            {
                _completionIndex = 0;
                SetInputText(_completionCandidates[0]);
            }
        }

        private List<string> GetFamilyCandidates(string partial)
        {
            var spawner = PresetShowcaseSpawner.Instance;
            if (spawner == null) return new List<string>();

            return spawner.GetAllFamilyNames()
                .Where(f => f.StartsWith(partial, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private void SetInputText(string text)
        {
            _inputField.text = text;
            _inputField.caretPosition = text.Length;
            _inputField.selectionAnchorPosition = text.Length;
            _inputField.selectionFocusPosition = text.Length;
        }

        private void ResetCompletion()
        {
            _completionCandidates.Clear();
            _completionIndex = -1;
            _completionPrefix = null;
        }

        private void AppendOutput(string text)
        {
            if (string.IsNullOrEmpty(_outputText.text))
            {
                _outputText.text = text;
            }
            else
            {
                _outputText.text += "\n" + text;
            }

            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            if (_scrollCoroutine != null) StopCoroutine(_scrollCoroutine);
            _scrollCoroutine = StartCoroutine(ScrollToBottomNextFrame());
        }

        private IEnumerator ScrollToBottomNextFrame()
        {
            yield return null;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
            _scrollRect.verticalNormalizedPosition = 0f;
            _scrollCoroutine = null;
        }

        private void CreateUI()
        {
            // Canvas
            var canvasObj = new GameObject("PresetConsoleCanvas");
            canvasObj.transform.SetParent(transform);
            _canvas = canvasObj.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            // Toggle button
            CreateToggleButton(canvasObj.transform);

            // Panel
            CreatePanel(canvasObj.transform);
        }

        private void CreateToggleButton(Transform parent)
        {
            var btnObj = new GameObject("ToggleButton");
            btnObj.transform.SetParent(parent, false);

            var btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.5f, 0f);
            btnRect.anchorMax = new Vector2(0.5f, 0f);
            btnRect.pivot = new Vector2(0.5f, 0f);
            btnRect.anchoredPosition = new Vector2(0f, 5f);
            btnRect.sizeDelta = new Vector2(180f, 30f);

            var btnImage = btnObj.AddComponent<Image>();
            btnImage.color = new Color(0.15f, 0.15f, 0.15f, 0.85f);

            _toggleButton = btnObj.AddComponent<Button>();
            _toggleButton.onClick.AddListener(() => SetPanelOpen(!_isOpen));

            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(btnObj.transform, false);
            var labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            _toggleButtonLabel = labelObj.AddComponent<TextMeshProUGUI>();
            _toggleButtonLabel.text = "Console (Tab)";
            _toggleButtonLabel.fontSize = 14f;
            _toggleButtonLabel.alignment = TextAlignmentOptions.Center;
            _toggleButtonLabel.color = Color.white;
        }

        private void CreatePanel(Transform parent)
        {
            _panelObj = new GameObject("ConsolePanel");
            _panelObj.transform.SetParent(parent, false);

            var panelRect = _panelObj.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0f, 0f);
            panelRect.anchorMax = new Vector2(1f, 0f);
            panelRect.pivot = new Vector2(0.5f, 0f);
            panelRect.anchoredPosition = new Vector2(0f, 40f);
            panelRect.sizeDelta = new Vector2(0f, PanelHeight);

            var panelImage = _panelObj.AddComponent<Image>();
            panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            var panelLayout = _panelObj.AddComponent<VerticalLayoutGroup>();
            panelLayout.padding = new RectOffset(10, 10, 10, 10);
            panelLayout.spacing = 5f;
            panelLayout.childForceExpandWidth = true;
            panelLayout.childForceExpandHeight = false;
            panelLayout.childControlHeight = true;
            panelLayout.childControlWidth = true;

            // Scroll view for output
            CreateScrollView(_panelObj.transform);

            // Input field
            CreateInputField(_panelObj.transform);
        }

        private void CreateScrollView(Transform parent)
        {
            var scrollObj = new GameObject("ScrollView");
            scrollObj.transform.SetParent(parent, false);

            var scrollRect = scrollObj.AddComponent<RectTransform>();
            var scrollLayout = scrollObj.AddComponent<LayoutElement>();
            scrollLayout.flexibleHeight = 1f;

            var scrollImage = scrollObj.AddComponent<Image>();
            scrollImage.color = new Color(0.05f, 0.05f, 0.05f, 0.8f);

            _scrollRect = scrollObj.AddComponent<ScrollRect>();
            _scrollRect.horizontal = false;
            _scrollRect.vertical = true;
            _scrollRect.movementType = ScrollRect.MovementType.Clamped;
            _scrollRect.scrollSensitivity = 30f;

            var mask = scrollObj.AddComponent<Mask>();
            mask.showMaskGraphic = true;

            // Viewport
            var viewportObj = new GameObject("Viewport");
            viewportObj.transform.SetParent(scrollObj.transform, false);
            var viewportRect = viewportObj.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            // Content
            var contentObj = new GameObject("Content");
            contentObj.transform.SetParent(viewportObj.transform, false);
            var contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.sizeDelta = new Vector2(0f, 0f);

            var contentFitter = contentObj.AddComponent<ContentSizeFitter>();
            contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var contentLayout = contentObj.AddComponent<VerticalLayoutGroup>();
            contentLayout.padding = new RectOffset(10, 10, 5, 5);
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = true;

            // Output text
            var textObj = new GameObject("OutputText");
            textObj.transform.SetParent(contentObj.transform, false);

            _outputText = textObj.AddComponent<TextMeshProUGUI>();
            _outputText.fontSize = 13f;
            _outputText.color = new Color(0.8f, 0.9f, 0.8f, 1f);
            _outputText.alignment = TextAlignmentOptions.TopLeft;
            _outputText.textWrappingMode = TextWrappingModes.Normal;
            _outputText.text = HelpText;

            var textFitter = textObj.AddComponent<ContentSizeFitter>();
            textFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _scrollRect.content = contentRect;
            _scrollRect.viewport = viewportRect;

            // Vertical scrollbar
            var scrollbarObj = new GameObject("Scrollbar");
            scrollbarObj.transform.SetParent(scrollObj.transform, false);
            var scrollbarRect = scrollbarObj.AddComponent<RectTransform>();
            scrollbarRect.anchorMin = new Vector2(1f, 0f);
            scrollbarRect.anchorMax = new Vector2(1f, 1f);
            scrollbarRect.pivot = new Vector2(1f, 0.5f);
            scrollbarRect.sizeDelta = new Vector2(8f, 0f);
            scrollbarRect.anchoredPosition = Vector2.zero;

            var scrollbarImage = scrollbarObj.AddComponent<Image>();
            scrollbarImage.color = new Color(0.15f, 0.15f, 0.15f, 0.5f);

            var scrollbar = scrollbarObj.AddComponent<Scrollbar>();
            scrollbar.direction = Scrollbar.Direction.BottomToTop;

            var handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(scrollbarObj.transform, false);
            var handleRect = handleObj.AddComponent<RectTransform>();
            handleRect.anchorMin = Vector2.zero;
            handleRect.anchorMax = Vector2.one;
            handleRect.offsetMin = Vector2.zero;
            handleRect.offsetMax = Vector2.zero;

            var handleImage = handleObj.AddComponent<Image>();
            handleImage.color = new Color(0.5f, 0.5f, 0.5f, 0.6f);

            scrollbar.handleRect = handleRect;
            scrollbar.targetGraphic = handleImage;

            _scrollRect.verticalScrollbar = scrollbar;
            _scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;

            // Shrink viewport to leave room for scrollbar
            viewportRect.offsetMax = new Vector2(-10f, 0f);
        }

        private void CreateInputField(Transform parent)
        {
            var inputObj = new GameObject("InputField");
            inputObj.transform.SetParent(parent, false);

            var inputLayout = inputObj.AddComponent<LayoutElement>();
            inputLayout.minHeight = 30f;
            inputLayout.preferredHeight = 30f;

            var inputImage = inputObj.AddComponent<Image>();
            inputImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

            _inputField = inputObj.AddComponent<TMP_InputField>();
            _inputField.lineType = TMP_InputField.LineType.SingleLine;

            // Text area
            var textAreaObj = new GameObject("TextArea");
            textAreaObj.transform.SetParent(inputObj.transform, false);
            var textAreaRect = textAreaObj.AddComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.offsetMin = new Vector2(8f, 2f);
            textAreaRect.offsetMax = new Vector2(-8f, -2f);

            // The RectMask2D clips the text within the text area
            textAreaObj.AddComponent<RectMask2D>();

            // Input text
            var inputTextObj = new GameObject("Text");
            inputTextObj.transform.SetParent(textAreaObj.transform, false);
            var inputTextRect = inputTextObj.AddComponent<RectTransform>();
            inputTextRect.anchorMin = Vector2.zero;
            inputTextRect.anchorMax = Vector2.one;
            inputTextRect.offsetMin = Vector2.zero;
            inputTextRect.offsetMax = Vector2.zero;

            var inputTMP = inputTextObj.AddComponent<TextMeshProUGUI>();
            inputTMP.fontSize = 14f;
            inputTMP.color = Color.white;
            inputTMP.alignment = TextAlignmentOptions.MidlineLeft;

            // Placeholder
            var placeholderObj = new GameObject("Placeholder");
            placeholderObj.transform.SetParent(textAreaObj.transform, false);
            var placeholderRect = placeholderObj.AddComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = Vector2.zero;
            placeholderRect.offsetMax = Vector2.zero;

            var placeholderTMP = placeholderObj.AddComponent<TextMeshProUGUI>();
            placeholderTMP.fontSize = 14f;
            placeholderTMP.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
            placeholderTMP.fontStyle = FontStyles.Italic;
            placeholderTMP.alignment = TextAlignmentOptions.MidlineLeft;
            placeholderTMP.text = "Type a command... (help for usage)";

            _inputField.textViewport = textAreaRect;
            _inputField.textComponent = inputTMP;
            _inputField.placeholder = placeholderTMP;

            _inputField.onSubmit.AddListener(OnSubmit);
            _inputField.onValueChanged.AddListener(OnInputChanged);
        }

        private void OnInputChanged(string text)
        {
            // Only reset if the change wasn't from autocomplete itself
            if (_completionCandidates.Count > 0 && _completionIndex >= 0 &&
                _completionIndex < _completionCandidates.Count &&
                text == _completionCandidates[_completionIndex])
            {
                return;
            }

            ResetCompletion();
        }
    }
}
