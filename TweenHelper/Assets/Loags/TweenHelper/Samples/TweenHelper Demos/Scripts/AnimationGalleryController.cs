using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper.Demo
{
    public sealed class AnimationGalleryController : MonoBehaviour
    {
        [Header("Category Navigation")]
        [SerializeField] private Button[] categoryButtons;
        [SerializeField] private TMP_Text[] categoryCountTexts;

        [Header("Animation List")]
        [SerializeField] private TMP_InputField searchInput;
        [SerializeField] private Dropdown familyDropdown;
        [SerializeField] private Transform listContent;
        [SerializeField] private GameObject listItemPrefab;
        [SerializeField] private TMP_Text visibleCountText;

        [Header("Details")]
        [SerializeField] private TMP_Text categoryText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text apiKindText;
        [SerializeField] private TMP_Text targetBadgeText;
        [SerializeField] private AnimationGalleryOptionView[] optionViews;
        [SerializeField] private AnimationGalleryCodePresenter codePresenter;

        [Header("Playback")]
        [SerializeField] private AnimationGalleryPlayer player;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button copyButton;

        [Header("Presentation Mode")]
        [SerializeField] private Button presentationModeButton;
        [SerializeField] private TMP_Text presentationModeButtonText;
        [SerializeField] private GameObject navigationChrome;
        [SerializeField] private GameObject detailsChrome;
        [SerializeField] private RectTransform contentArea;

        private readonly Dictionary<AnimationGalleryCategory, string> _selectionByCategory = new Dictionary<AnimationGalleryCategory, string>();
        private readonly Dictionary<string, int[]> _optionsByEntry = new Dictionary<string, int[]>();
        private readonly List<AnimationGalleryListItem> _rows = new List<AnimationGalleryListItem>();
        private readonly List<AnimationGalleryEntry> _visibleEntries = new List<AnimationGalleryEntry>();
        private IReadOnlyList<AnimationGalleryEntry> _catalog;
        private AnimationGalleryCategory _category;
        private AnimationGalleryConfiguration _configuration;
        private bool _presentationMode;

        private void Start()
        {
            _catalog = AnimationGalleryCatalog.Build();
            WireControls();
            PopulateCategoryCounts();
            ShowCategory(AnimationGalleryCategory.Presets);
        }

        private void WireControls()
        {
            for (int i = 0; i < categoryButtons.Length; i++)
            {
                AnimationGalleryCategory category = (AnimationGalleryCategory)i;
                categoryButtons[i].onClick.AddListener(() => ShowCategory(category));
            }

            searchInput.onValueChanged.AddListener(_ => RebuildVisibleEntries());
            familyDropdown.onValueChanged.AddListener(_ => RebuildVisibleEntries());
            previousButton.onClick.AddListener(ShowPrevious);
            replayButton.onClick.AddListener(Replay);
            resetButton.onClick.AddListener(ResetCurrent);
            nextButton.onClick.AddListener(ShowNext);
            copyButton.onClick.AddListener(codePresenter.Copy);
            presentationModeButton.onClick.AddListener(TogglePresentationMode);
        }

        private void PopulateCategoryCounts()
        {
            for (int i = 0; i < categoryCountTexts.Length; i++)
            {
                AnimationGalleryCategory category = (AnimationGalleryCategory)i;
                categoryCountTexts[i].text = _catalog.Count(entry => entry.Category == category).ToString();
            }
        }

        private void ShowCategory(AnimationGalleryCategory category)
        {
            _category = category;
            for (int i = 0; i < categoryButtons.Length; i++) categoryButtons[i].interactable = i != (int)category;
            searchInput.SetTextWithoutNotify(string.Empty);
            PopulateFamilyFilter();
            familyDropdown.gameObject.SetActive(category == AnimationGalleryCategory.Presets);
            RebuildVisibleEntries();
        }

        private void PopulateFamilyFilter()
        {
            familyDropdown.ClearOptions();
            var options = new List<string> { "All families" };
            options.AddRange(_catalog.Where(entry => entry.Category == AnimationGalleryCategory.Presets)
                .Select(entry => entry.Family)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(family => family, StringComparer.Ordinal));
            familyDropdown.AddOptions(options);
            familyDropdown.SetValueWithoutNotify(0);
        }

        private void RebuildVisibleEntries()
        {
            _visibleEntries.Clear();
            string search = searchInput.text.Trim();
            string family = familyDropdown.value <= 0 ? string.Empty : familyDropdown.options[familyDropdown.value].text;
            _visibleEntries.AddRange(_catalog.Where(entry => entry.Category == _category)
                .Where(entry => string.IsNullOrEmpty(search) || entry.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 || entry.Description.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                .Where(entry => string.IsNullOrEmpty(family) || entry.Family == family));

            RebuildRows();
            visibleCountText.text = $"{_visibleEntries.Count} shown";
            previousButton.interactable = _visibleEntries.Count > 1;
            nextButton.interactable = _visibleEntries.Count > 1;

            string rememberedId = _selectionByCategory.TryGetValue(_category, out string id) ? id : string.Empty;
            AnimationGalleryEntry selection = _visibleEntries.FirstOrDefault(entry => entry.Id == rememberedId) ?? _visibleEntries.FirstOrDefault();
            if (selection != null) Select(selection, true);
        }

        private void RebuildRows()
        {
            foreach (AnimationGalleryListItem row in _rows) Destroy(row.gameObject);
            _rows.Clear();
            foreach (AnimationGalleryEntry entry in _visibleEntries)
            {
                GameObject rowObject = Instantiate(listItemPrefab, listContent);
                rowObject.SetActive(true);
                AnimationGalleryListItem row = rowObject.GetComponent<AnimationGalleryListItem>();
                row.Configure(entry, () => Select(entry, true));
                _rows.Add(row);
            }
        }

        private void Select(AnimationGalleryEntry entry, bool play)
        {
            _selectionByCategory[_category] = entry.Id;
            int[] optionIndices = _optionsByEntry.TryGetValue(entry.Id, out int[] remembered) ? remembered : null;
            _configuration = new AnimationGalleryConfiguration(entry, optionIndices);
            categoryText.text = GetCategoryName(entry.Category);
            nameText.text = entry.Name;
            descriptionText.text = entry.Description;
            apiKindText.text = entry.ApiKind == AnimationGalleryApiKind.Preset ? "PRESET" : entry.ApiKind == AnimationGalleryApiKind.Recipe ? "RECIPE" : "BUILDER OPERATION";
            targetBadgeText.text = entry.TargetBadge;
            ConfigureOptions();
            codePresenter.Show(_configuration);
            RefreshSelectedRows();
            if (play) player.Play(_configuration);
        }

        private void ConfigureOptions()
        {
            for (int i = 0; i < optionViews.Length; i++)
            {
                if (i >= _configuration.Entry.Options.Count)
                {
                    optionViews[i].Hide();
                    continue;
                }

                int optionIndex = i;
                optionViews[i].Configure(_configuration.Entry.Options[i], _configuration.OptionIndices[i], value => ChangeOption(optionIndex, value));
            }
        }

        private void ChangeOption(int optionIndex, int valueIndex)
        {
            _configuration = _configuration.WithOption(optionIndex, valueIndex);
            _optionsByEntry[_configuration.Entry.Id] = _configuration.OptionIndices.ToArray();
            codePresenter.Show(_configuration);
            player.Play(_configuration);
        }

        private void RefreshSelectedRows()
        {
            for (int i = 0; i < _rows.Count; i++) _rows[i].SetSelected(_visibleEntries[i].Id == _configuration.Entry.Id);
        }

        private void ShowPrevious() => SelectRelative(-1);
        private void ShowNext() => SelectRelative(1);

        private void SelectRelative(int delta)
        {
            int index = _visibleEntries.FindIndex(entry => entry.Id == _configuration.Entry.Id);
            int nextIndex = (index + delta + _visibleEntries.Count) % _visibleEntries.Count;
            Select(_visibleEntries[nextIndex], true);
        }

        private void Replay() => player.Play(_configuration);
        private void ResetCurrent() => player.ResetPreview(_configuration);

        private void TogglePresentationMode()
        {
            _presentationMode = !_presentationMode;
            navigationChrome.SetActive(!_presentationMode);
            detailsChrome.SetActive(!_presentationMode);
            contentArea.offsetMin = new Vector2(_presentationMode ? 24f : 580f, contentArea.offsetMin.y);
            presentationModeButtonText.text = _presentationMode ? "Exit presentation" : "Presentation mode";
        }

        private static string GetCategoryName(AnimationGalleryCategory category)
        {
            switch (category)
            {
                case AnimationGalleryCategory.UIRecipes: return "UI Recipes";
                case AnimationGalleryCategory.DestinationMotion: return "Destination Motion";
                case AnimationGalleryCategory.GameplayFeedback: return "Gameplay Feedback";
                case AnimationGalleryCategory.UISequences: return "UI Sequences";
                case AnimationGalleryCategory.TextAndValues: return "Text & Values";
                case AnimationGalleryCategory.CameraFeedback: return "Camera Feedback";
                default: return category.ToString();
            }
        }
    }
}
