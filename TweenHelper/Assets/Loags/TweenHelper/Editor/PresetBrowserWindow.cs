using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LB.TweenHelper.Editor
{
    public sealed class PresetBrowserWindow : EditorWindow
    {
        private const string AllAnimations = "All animations";
        private const string AllFamilies = "All families";
        private const string SetupStylePath = "Assets/Loags/TweenHelper/Editor/Setup/TweenHelperSetupWindow.uss";
        private const string BrowserStylePath = "Assets/Loags/TweenHelper/Editor/PresetBrowserWindow.uss";
        private const string LogoPath = "Assets/Loags/TweenHelper/Editor/Setup/Branding/TweenHelperLogo-v2.png";

        private readonly List<PresetBrowserEntry> _entries = new List<PresetBrowserEntry>();
        private readonly List<PresetBrowserEntry> _visibleEntries = new List<PresetBrowserEntry>();
        private readonly List<string> _categories = new List<string> { AllAnimations };
        private readonly List<string> _families = new List<string>();
        private readonly Dictionary<PresetBrowserCollectionKind, int> _collectionOptionSelections = new Dictionary<PresetBrowserCollectionKind, int>();
        private readonly PresetBrowserPreview _preview = new PresetBrowserPreview();

        private ToolbarSearchField _searchField;
        private PopupField<string> _categoryPopup;
        private PopupField<string> _familyPopup;
        private ListView _listView;
        private Label _catalogCount;
        private Label _visibleCount;
        private Label _entryBadge;
        private Label _entryName;
        private Label _entryDescription;
        private Label _categoryValue;
        private Label _familyValue;
        private Label _intensityValue;
        private Label _directionValue;
        private Label _axisValue;
        private Label _durationValue;
        private VisualElement _intensityRow;
        private VisualElement _directionRow;
        private VisualElement _axisRow;
        private VisualElement _collectionOptionCard;
        private Label _collectionOptionLabel;
        private PopupField<string> _collectionOptionPopup;
        private TextField _exampleField;
        private Button _copyButton;
        private Button _previewButton;
        private Label _previewStatus;
        private IMGUIContainer _previewContainer;
        private PresetBrowserEntry _selectedEntry;
        private bool _wasPlaying;

        [MenuItem("Tools/Tween Helper/Preset Browser", false, 1)]
        public static void Open()
        {
            var window = GetWindow<PresetBrowserWindow>();
            window.titleContent = new GUIContent("Tween Helper Browser");
            window.minSize = new Vector2(980f, 600f);
            window.Show();
        }

        private void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload += DisposePreview;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload -= DisposePreview;
            DisposePreview();
        }

        private void OnProjectChange()
        {
            if (_listView != null) RefreshEntries();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange _) => ResetPreviewForSelection();

        public void CreateGUI()
        {
            rootVisualElement.Clear();
            rootVisualElement.AddToClassList("window-root");
            rootVisualElement.AddToClassList("preset-browser-root");

            StyleSheet setupStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(SetupStylePath);
            StyleSheet browserStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(BrowserStylePath);
            if (setupStyle != null) rootVisualElement.styleSheets.Add(setupStyle);
            if (browserStyle != null) rootVisualElement.styleSheets.Add(browserStyle);

            BuildHeader();
            BuildFilters();
            BuildContent();
            RefreshEntries();
        }

        private void BuildHeader()
        {
            var header = new VisualElement();
            header.AddToClassList("browser-hero");

            Texture2D logo = AssetDatabase.LoadAssetAtPath<Texture2D>(LogoPath);
            if (logo != null)
            {
                var logoImage = new Image { image = logo, scaleMode = ScaleMode.ScaleToFit };
                logoImage.AddToClassList("browser-logo");
                header.Add(logoImage);
            }

            var copy = new VisualElement();
            copy.AddToClassList("browser-hero-copy");
            var title = new Label("Preset Browser");
            title.AddToClassList("browser-title");
            var subtitle = new Label("Explore every preset, recipe, and high-level animation without touching the active scene.");
            subtitle.AddToClassList("browser-subtitle");
            copy.Add(title);
            copy.Add(subtitle);
            header.Add(copy);

            _catalogCount = new Label();
            _catalogCount.AddToClassList("version-pill");
            _catalogCount.AddToClassList("catalog-pill");
            header.Add(_catalogCount);
            rootVisualElement.Add(header);
        }

        private void BuildFilters()
        {
            var filterCard = new VisualElement();
            filterCard.AddToClassList("browser-filter-card");

            _searchField = new ToolbarSearchField();
            _searchField.tooltip = "Search names and descriptions";
            _searchField.AddToClassList("browser-search");
            _searchField.RegisterValueChangedCallback(_ => ApplyFilters());
            filterCard.Add(_searchField);

            _categoryPopup = new PopupField<string>("Category", _categories, 0);
            _categoryPopup.AddToClassList("browser-popup");
            _categoryPopup.RegisterValueChangedCallback(_ =>
            {
                RefreshFamilyChoices();
                ApplyFilters();
            });
            filterCard.Add(_categoryPopup);

            _families.Add(AllFamilies);
            _familyPopup = new PopupField<string>("Family", _families, 0);
            _familyPopup.AddToClassList("browser-popup");
            _familyPopup.RegisterValueChangedCallback(_ => ApplyFilters());
            filterCard.Add(_familyPopup);

            var refreshButton = new Button(RefreshEntries) { text = "Refresh" };
            refreshButton.AddToClassList("secondary-button");
            refreshButton.AddToClassList("browser-refresh-button");
            filterCard.Add(refreshButton);
            rootVisualElement.Add(filterCard);
        }

        private void BuildContent()
        {
            var content = new VisualElement();
            content.AddToClassList("browser-content");
            BuildListPanel(content);
            BuildDetailsPanel(content);
            BuildPreviewPanel(content);
            rootVisualElement.Add(content);
        }

        private void BuildListPanel(VisualElement parent)
        {
            var panel = new VisualElement();
            panel.AddToClassList("section-card");
            panel.AddToClassList("browser-list-panel");

            var heading = new VisualElement();
            heading.AddToClassList("browser-panel-heading");
            var title = new Label("Animations");
            title.AddToClassList("section-title");
            _visibleCount = new Label();
            _visibleCount.AddToClassList("browser-muted");
            heading.Add(title);
            heading.Add(_visibleCount);
            panel.Add(heading);

            _listView = new ListView
            {
                fixedItemHeight = 50f,
                selectionType = SelectionType.Single,
                virtualizationMethod = CollectionVirtualizationMethod.FixedHeight,
                makeItem = MakeListRow,
                bindItem = BindListRow
            };
            _listView.AddToClassList("browser-list");
            _listView.selectionChanged += OnListSelectionChanged;
            panel.Add(_listView);
            parent.Add(panel);
        }

        private void BuildDetailsPanel(VisualElement parent)
        {
            var panel = new VisualElement();
            panel.AddToClassList("section-card");
            panel.AddToClassList("browser-details-panel");

            _entryBadge = new Label();
            _entryBadge.AddToClassList("browser-entry-badge");
            _entryName = new Label("Select an animation");
            _entryName.AddToClassList("browser-entry-title");
            _entryDescription = new Label("Choose an entry from the list to inspect its metadata and copy a usage example.");
            _entryDescription.AddToClassList("browser-entry-description");
            panel.Add(_entryBadge);
            panel.Add(_entryName);
            panel.Add(_entryDescription);

            var metadata = new VisualElement();
            metadata.AddToClassList("browser-metadata");
            _categoryValue = AddMetadataRow(metadata, "Category", out _);
            _familyValue = AddMetadataRow(metadata, "Family", out _);
            _intensityValue = AddMetadataRow(metadata, "Intensity", out _intensityRow);
            _directionValue = AddMetadataRow(metadata, "Direction", out _directionRow);
            _axisValue = AddMetadataRow(metadata, "Axis / plane", out _axisRow);
            _durationValue = AddMetadataRow(metadata, "Duration", out _);
            panel.Add(metadata);

            _collectionOptionCard = new VisualElement();
            _collectionOptionCard.AddToClassList("browser-option-card");
            _collectionOptionLabel = new Label();
            _collectionOptionLabel.AddToClassList("browser-field-title");
            _collectionOptionPopup = new PopupField<string>(new List<string> { "Default" }, 0);
            _collectionOptionPopup.AddToClassList("browser-option-popup");
            _collectionOptionPopup.RegisterValueChangedCallback(OnCollectionOptionChanged);
            _collectionOptionCard.Add(_collectionOptionLabel);
            _collectionOptionCard.Add(_collectionOptionPopup);
            _collectionOptionCard.style.display = DisplayStyle.None;
            panel.Add(_collectionOptionCard);

            var exampleHeading = new Label("Fluent API");
            exampleHeading.AddToClassList("browser-field-title");
            panel.Add(exampleHeading);

            _exampleField = new TextField { multiline = true, isReadOnly = true };
            _exampleField.AddToClassList("browser-example-field");
            panel.Add(_exampleField);

            _copyButton = new Button(CopyExample) { text = "Copy Example" };
            _copyButton.AddToClassList("secondary-button");
            _copyButton.SetEnabled(false);
            panel.Add(_copyButton);
            parent.Add(panel);
        }

        private void BuildPreviewPanel(VisualElement parent)
        {
            var panel = new VisualElement();
            panel.AddToClassList("section-card");
            panel.AddToClassList("browser-preview-panel");

            var eyebrow = new Label("IN-WINDOW PREVIEW");
            eyebrow.AddToClassList("browser-preview-eyebrow");
            var title = new Label("Isolated preview stage");
            title.AddToClassList("section-title");
            var description = new Label("The preview uses temporary target-specific fixtures and never reads from or modifies the active scene.");
            description.AddToClassList("browser-entry-description");
            panel.Add(eyebrow);
            panel.Add(title);
            panel.Add(description);

            var viewport = new VisualElement();
            viewport.AddToClassList("browser-preview-viewport");
            _previewContainer = new IMGUIContainer(DrawPreview);
            _previewContainer.AddToClassList("browser-preview-canvas");
            viewport.Add(_previewContainer);
            panel.Add(viewport);

            _previewStatus = new Label("Select an animation to prepare its preview.");
            _previewStatus.AddToClassList("browser-preview-status");
            panel.Add(_previewStatus);

            _previewButton = new Button(PlayPreview) { text = "Preview" };
            _previewButton.AddToClassList("primary-button");
            _previewButton.AddToClassList("browser-preview-button");
            _previewButton.SetEnabled(false);
            panel.Add(_previewButton);
            parent.Add(panel);
        }

        private static VisualElement MakeListRow()
        {
            var row = new VisualElement();
            row.AddToClassList("browser-list-row");
            var text = new VisualElement();
            text.AddToClassList("browser-list-copy");
            var name = new Label { name = "entry-name" };
            name.AddToClassList("browser-list-name");
            var metadata = new Label { name = "entry-metadata" };
            metadata.AddToClassList("browser-list-metadata");
            text.Add(name);
            text.Add(metadata);
            var badge = new Label { name = "entry-badge" };
            badge.AddToClassList("browser-list-badge");
            row.Add(text);
            row.Add(badge);
            return row;
        }

        private void BindListRow(VisualElement row, int index)
        {
            if (index < 0 || index >= _visibleEntries.Count) return;
            PresetBrowserEntry entry = _visibleEntries[index];
            row.Q<Label>("entry-name").text = entry.Name;
            row.Q<Label>("entry-metadata").text = string.IsNullOrEmpty(entry.Family) ? entry.Category : entry.Family;
            Label badge = row.Q<Label>("entry-badge");
            badge.text = entry.Badge;
            badge.EnableInClassList("browser-list-badge-collection", !entry.IsPreset);
            row.tooltip = entry.Description;
        }

        private void OnListSelectionChanged(IEnumerable<object> selection)
        {
            PresetBrowserEntry entry = selection.OfType<PresetBrowserEntry>().FirstOrDefault();
            if (entry != null) SelectEntry(entry);
        }

        private void RefreshEntries()
        {
            string selectedId = _selectedEntry?.Id;
            _entries.Clear();
            _entries.AddRange(PresetBrowserCatalog.Build());
            int presetCount = _entries.Count(entry => entry.IsPreset);
            int additionalCount = _entries.Count - presetCount;
            _catalogCount.text = $"{presetCount} presets  /  {additionalCount} additional animations";
            RefreshCategoryChoices();
            RefreshFamilyChoices();
            ApplyFilters(selectedId);
        }

        private void RefreshCategoryChoices()
        {
            if (_categoryPopup == null) return;
            string previous = _categoryPopup.value;
            _categories.Clear();
            _categories.Add(AllAnimations);
            _categories.AddRange(_entries
                .Select(entry => entry.Category)
                .Where(category => !string.IsNullOrEmpty(category))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(category => category, StringComparer.Ordinal));
            _categoryPopup.choices = new List<string>(_categories);
            _categoryPopup.SetValueWithoutNotify(_categories.Contains(previous) ? previous : AllAnimations);
        }

        private void RefreshFamilyChoices()
        {
            if (_familyPopup == null) return;
            string previous = _familyPopup.value;
            string category = _categoryPopup?.value ?? AllAnimations;
            _families.Clear();
            _families.Add(AllFamilies);
            _families.AddRange(_entries
                .Where(entry => MatchesCategory(entry, category))
                .Select(entry => entry.Family)
                .Where(family => !string.IsNullOrEmpty(family))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(family => family, StringComparer.Ordinal));
            _familyPopup.choices = new List<string>(_families);
            _familyPopup.SetValueWithoutNotify(_families.Contains(previous) ? previous : AllFamilies);
        }

        private void ApplyFilters(string preferredId = null)
        {
            if (_listView == null) return;
            string selectedId = preferredId ?? _selectedEntry?.Id;
            string search = _searchField?.value?.Trim() ?? string.Empty;
            string category = _categoryPopup?.value ?? AllAnimations;
            string family = _familyPopup?.value ?? AllFamilies;

            _visibleEntries.Clear();
            _visibleEntries.AddRange(_entries.Where(entry =>
                MatchesCategory(entry, category) &&
                (family == AllFamilies || string.Equals(entry.Family, family, StringComparison.Ordinal)) &&
                (string.IsNullOrEmpty(search) ||
                 entry.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                 entry.Description.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                 entry.Category.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                 entry.Family.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)));

            _listView.itemsSource = _visibleEntries;
            _listView.Rebuild();
            _visibleCount.text = $"{_visibleEntries.Count} shown";

            int selectedIndex = string.IsNullOrEmpty(selectedId) ? -1 : _visibleEntries.FindIndex(entry => entry.Id == selectedId);
            if (selectedIndex < 0 && _visibleEntries.Count > 0) selectedIndex = 0;
            if (selectedIndex >= 0)
            {
                _listView.SetSelection(selectedIndex);
                SelectEntry(_visibleEntries[selectedIndex]);
            }
            else
            {
                ClearSelection();
            }
        }

        private static bool MatchesCategory(PresetBrowserEntry entry, string category)
        {
            return category == AllAnimations || string.Equals(entry.Category, category, StringComparison.Ordinal);
        }

        private void SelectEntry(PresetBrowserEntry entry)
        {
            _selectedEntry = entry;
            _entryBadge.text = entry.Badge;
            _entryBadge.EnableInClassList("browser-entry-badge-collection", !entry.IsPreset);
            _entryName.text = entry.Name;
            _entryDescription.text = entry.Description;
            _categoryValue.text = entry.Category;
            _familyValue.text = entry.Family;
            SetOptionalMetadata(_intensityRow, _intensityValue, entry.Intensity);
            SetOptionalMetadata(_directionRow, _directionValue, entry.Direction);
            SetOptionalMetadata(_axisRow, _axisValue, entry.AxisOrPlane);
            _durationValue.text = entry.Duration;
            ConfigureCollectionOptions(entry);
            RefreshConfiguredEntry();
            _copyButton.SetEnabled(true);
            _previewButton.SetEnabled(true);
            ResetPreviewForSelection();
        }

        private void ClearSelection()
        {
            _selectedEntry = null;
            _entryBadge.text = string.Empty;
            _entryName.text = "No matching animations";
            _entryDescription.text = "Adjust the search or filters to continue browsing.";
            _categoryValue.text = string.Empty;
            _familyValue.text = string.Empty;
            _durationValue.text = string.Empty;
            SetOptionalMetadata(_intensityRow, _intensityValue, string.Empty);
            SetOptionalMetadata(_directionRow, _directionValue, string.Empty);
            SetOptionalMetadata(_axisRow, _axisValue, string.Empty);
            _collectionOptionCard.style.display = DisplayStyle.None;
            _exampleField.SetValueWithoutNotify(string.Empty);
            _copyButton.SetEnabled(false);
            _previewButton.SetEnabled(false);
            _preview.SetEntry(null);
            _previewStatus.text = "No animation selected.";
            _previewContainer.MarkDirtyRepaint();
        }

        private void ResetPreviewForSelection()
        {
            _preview.SetEntry(_selectedEntry, GetSelectedCollectionOption());
            _wasPlaying = false;
            if (_previewButton == null || _previewStatus == null) return;
            _previewButton.text = "Preview";
            _previewStatus.RemoveFromClassList("browser-preview-error");
            _previewStatus.text = _selectedEntry == null ? "No animation selected." : "Ready to preview.";
            _previewContainer?.MarkDirtyRepaint();
        }

        private void PlayPreview()
        {
            if (_selectedEntry == null) return;

            try
            {
                _preview.Play();
                _wasPlaying = true;
                _previewButton.text = "Preview Again";
                _previewStatus.RemoveFromClassList("browser-preview-error");
                _previewStatus.text = "Playing inside the isolated preview stage.";
            }
            catch (Exception exception)
            {
                _wasPlaying = false;
                _previewStatus.text = exception.Message;
                _previewStatus.AddToClassList("browser-preview-error");
                Debug.LogException(exception);
            }

            _previewContainer.MarkDirtyRepaint();
        }

        private void CopyExample()
        {
            if (_selectedEntry == null) return;
            EditorGUIUtility.systemCopyBuffer = _exampleField.value;
            ShowNotification(new GUIContent("Example copied"));
        }

        private void ConfigureCollectionOptions(PresetBrowserEntry entry)
        {
            List<string> choices = GetCollectionOptionChoices(entry.CollectionKind);
            bool visible = choices.Count > 0;
            _collectionOptionCard.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            if (!visible) return;

            _collectionOptionLabel.text = GetCollectionOptionLabel(entry.CollectionKind);
            _collectionOptionPopup.choices = choices;
            int selectedIndex = _collectionOptionSelections.TryGetValue(entry.CollectionKind, out int rememberedIndex) ? rememberedIndex : 0;
            selectedIndex = Mathf.Clamp(selectedIndex, 0, choices.Count - 1);
            _collectionOptionPopup.SetValueWithoutNotify(choices[selectedIndex]);
        }

        private void OnCollectionOptionChanged(ChangeEvent<string> changeEvent)
        {
            if (_selectedEntry == null) return;
            int selectedIndex = _collectionOptionPopup.choices.IndexOf(changeEvent.newValue);
            _collectionOptionSelections[_selectedEntry.CollectionKind] = Mathf.Max(selectedIndex, 0);
            RefreshConfiguredEntry();
            ResetPreviewForSelection();
        }

        private void RefreshConfiguredEntry()
        {
            if (_selectedEntry == null) return;
            int optionIndex = GetSelectedCollectionOption();
            SetOptionalMetadata(_directionRow, _directionValue, GetConfiguredDirection(_selectedEntry, optionIndex));
            _exampleField.SetValueWithoutNotify(GetConfiguredExample(_selectedEntry, optionIndex));
        }

        private int GetSelectedCollectionOption()
        {
            if (_selectedEntry == null) return 0;
            return _collectionOptionSelections.TryGetValue(_selectedEntry.CollectionKind, out int selectedIndex) ? selectedIndex : 0;
        }

        private static string GetCollectionOptionLabel(PresetBrowserCollectionKind kind)
        {
            switch (kind)
            {
                case PresetBrowserCollectionKind.GridDiagonalWave:
                case PresetBrowserCollectionKind.GridSpiral:
                    return "Direction";
                case PresetBrowserCollectionKind.GridCheckerboard:
                    return "Phase";
                default:
                    return string.Empty;
            }
        }

        private static List<string> GetCollectionOptionChoices(PresetBrowserCollectionKind kind)
        {
            switch (kind)
            {
                case PresetBrowserCollectionKind.GridDiagonalWave:
                    return new List<string>
                    {
                        "Top-left to bottom-right",
                        "Top-right to bottom-left",
                        "Bottom-left to top-right",
                        "Bottom-right to top-left"
                    };
                case PresetBrowserCollectionKind.GridSpiral:
                    return new List<string>
                    {
                        "Outside in, clockwise",
                        "Outside in, counter-clockwise",
                        "Inside out, clockwise",
                        "Inside out, counter-clockwise"
                    };
                case PresetBrowserCollectionKind.GridCheckerboard:
                    return new List<string> { "Normal", "Inverted" };
                default:
                    return new List<string>();
            }
        }

        private static string GetConfiguredDirection(PresetBrowserEntry entry, int optionIndex)
        {
            List<string> choices = GetCollectionOptionChoices(entry.CollectionKind);
            return choices.Count == 0 ? entry.Direction : choices[Mathf.Clamp(optionIndex, 0, choices.Count - 1)];
        }

        private static string GetConfiguredExample(PresetBrowserEntry entry, int optionIndex)
        {
            switch (entry.CollectionKind)
            {
                case PresetBrowserCollectionKind.GridDiagonalWave:
                    return $"items.GridDiagonalWave(owner, columns: 3, direction: GridDiagonalDirection.{(GridDiagonalDirection)Mathf.Clamp(optionIndex, 0, 3)});";
                case PresetBrowserCollectionKind.GridSpiral:
                    return $"items.GridSpiral(owner, columns: 3, direction: GridSpiralDirection.{(GridSpiralDirection)Mathf.Clamp(optionIndex, 0, 3)});";
                case PresetBrowserCollectionKind.GridCheckerboard:
                    return optionIndex == 0 ? "items.GridCheckerboard(owner, columns: 3);" : "items.GridCheckerboard(owner, columns: 3, inverted: true);";
                default:
                    return entry.Example;
            }
        }

        private void OnEditorUpdate()
        {
            bool updated = _preview.Update();
            if (updated) _previewContainer?.MarkDirtyRepaint();
            if (_wasPlaying && !_preview.IsPlaying)
            {
                _wasPlaying = false;
                _previewStatus.text = "Preview complete. Press Preview Again to replay.";
                _previewContainer?.MarkDirtyRepaint();
            }
        }

        private void DrawPreview()
        {
            Rect rect = GUILayoutUtility.GetRect(10f, 10000f, 10f, 10000f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            _preview.Draw(rect);
        }

        private void DisposePreview() => _preview.Dispose();

        private static Label AddMetadataRow(VisualElement parent, string label, out VisualElement row)
        {
            row = new VisualElement();
            row.AddToClassList("browser-metadata-row");
            var key = new Label(label);
            key.AddToClassList("browser-metadata-key");
            var value = new Label();
            value.AddToClassList("browser-metadata-value");
            row.Add(key);
            row.Add(value);
            parent.Add(row);
            return value;
        }

        private static void SetOptionalMetadata(VisualElement row, Label valueLabel, string value)
        {
            bool visible = !string.IsNullOrEmpty(value);
            row.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            valueLabel.text = value ?? string.Empty;
        }
    }
}
