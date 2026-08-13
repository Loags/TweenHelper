using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Controls the prefab-authored UI showcase and its safe preview lifecycle.
    /// </summary>
    public class PresetShowcaseSpawner2D : MonoBehaviour
    {
        [Header("Tabs")]
        [SerializeField] private Button recipesTabButton;
        [SerializeField] private Button presetsTabButton;
        [SerializeField] private Button collectionsTabButton;
        [SerializeField] private Button destinationsTabButton;
        [SerializeField] private Button feedbackTabButton;
        [SerializeField] private Button uiSequencesTabButton;
        [SerializeField] private Button textValuesTabButton;
        [SerializeField] private GameObject recipesPanel;
        [SerializeField] private GameObject presetsPanel;

        [Header("Recipe Library")]
        [SerializeField] private Transform recipeContent;
        [SerializeField] private UIRecipeCard recipeCardPrefab;

        [Header("Preset Library")]
        [SerializeField] private Transform presetContent;
        [SerializeField] private UIPresetListItem presetListItemPrefab;
        [SerializeField] private TMP_InputField searchInput;
        [SerializeField] private TMP_Dropdown familyDropdown;
        [SerializeField] private TMP_Dropdown targetDropdown;
        [SerializeField] private TMP_Text visibleCountText;

        [Header("Preview")]
        [SerializeField] private Image presetImage;
        [SerializeField] private TextMeshProUGUI animatedText;
        [SerializeField] private GameObject collectionPreviewRoot;
        [SerializeField] private GameObject[] collectionTargets;
        [SerializeField] private GameObject destinationPreviewRoot;
        [SerializeField] private GameObject destinationTarget;
        [SerializeField] private RectTransform destinationStartMarker;
        [SerializeField] private RectTransform destinationEndMarker;
        [SerializeField] private GameObject destinationCurvedPath;
        [SerializeField] private GameObject uiSequencePreviewRoot;
        [SerializeField] private GameObject toastSequenceTarget;
        [SerializeField] private GameObject modalSequenceGroup;
        [SerializeField] private GameObject modalSequenceBackdrop;
        [SerializeField] private GameObject modalSequencePanel;
        [SerializeField] private GameObject[] modalSequenceControls;
        [SerializeField] private GameObject tooltipSequenceTarget;
        [SerializeField] private GameObject dropdownSequencePanel;
        [SerializeField] private GameObject[] dropdownSequenceEntries;
        [SerializeField] private GameObject tabSequenceGroup;
        [SerializeField] private GameObject tabSequenceOutgoing;
        [SerializeField] private GameObject tabSequenceIncoming;
        [SerializeField] private GameObject textValuePreviewRoot;
        [SerializeField] private TMP_Text typewriterText;
        [SerializeField] private TMP_Text numberText;
        [SerializeField] private TMP_Text characterText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text selectionNameText;
        [SerializeField] private TMP_Text selectionDescriptionText;
        [SerializeField] private TMP_Text codeExampleText;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button copyButton;
        [SerializeField] private DemoInstructionsPanel instructionsPanel;

        [Header("Colors")]
        [SerializeField] private Color imageHoverColor = new Color(1f, 0.9f, 0.6f, 1f);
        [SerializeField] private Color textHoverColor = new Color(0.7f, 0.9f, 1f, 1f);
        [SerializeField] private Color disabledTextColor = new Color(0.65f, 0.65f, 0.65f, 0.55f);

        private const float DestinationArcHeight = 145f;
        private const float DestinationBezierControlAHeight = 180f;
        private const float DestinationBezierControlBHeight = 58f;

        private static readonly RecipeDefinition[] Recipes =
        {
            new RecipeDefinition(UIRecipeKind.UIAppear, "Pop and fade a UI element into view."),
            new RecipeDefinition(UIRecipeKind.UIAppearSoft, "A gentler appear animation."),
            new RecipeDefinition(UIRecipeKind.UIDisappear, "Pop and fade a UI element out."),
            new RecipeDefinition(UIRecipeKind.UIDisappearSoft, "A gentler disappear animation."),
            new RecipeDefinition(UIRecipeKind.UIHover, "Scale and tint for hover feedback."),
            new RecipeDefinition(UIRecipeKind.UIHoverSoft, "Subtle hover feedback."),
            new RecipeDefinition(UIRecipeKind.UIPress, "Press and release feedback."),
            new RecipeDefinition(UIRecipeKind.UIPressHard, "Stronger press feedback."),
            new RecipeDefinition(UIRecipeKind.UIAttention, "Draw attention to an element."),
            new RecipeDefinition(UIRecipeKind.UIAttentionSoft, "Gentle attention motion."),
            new RecipeDefinition(UIRecipeKind.UIAttentionHard, "Strong attention motion."),
            new RecipeDefinition(UIRecipeKind.UIDisabled, "Animate into a disabled visual state."),
            new RecipeDefinition(UIRecipeKind.UIEnabled, "Restore the enabled visual state.")
        };

        private static readonly CollectionRecipeDefinition[] CollectionRecipes =
        {
            new CollectionRecipeDefinition(CollectionRecipeKind.ListStaggerIn, "Reveal a list with staggered pop-and-fade entrances."),
            new CollectionRecipeDefinition(CollectionRecipeKind.ListStaggerOut, "Dismiss a list in a staggered sequence."),
            new CollectionRecipeDefinition(CollectionRecipeKind.GridWave, "Reveal a grid one column at a time."),
            new CollectionRecipeDefinition(CollectionRecipeKind.GridRipple, "Pulse outward from the center of a grid."),
            new CollectionRecipeDefinition(CollectionRecipeKind.LoadingDots, "Loop a soft pulse across three loading dots."),
            new CollectionRecipeDefinition(CollectionRecipeKind.GridDiagonalWave, "Reveal diagonals from the top-left toward the bottom-right."),
            new CollectionRecipeDefinition(CollectionRecipeKind.GridSpiral, "Reveal grid items in a clockwise outside-in spiral."),
            new CollectionRecipeDefinition(CollectionRecipeKind.GridCheckerboard, "Pulse alternating checkerboard cells in two phases."),
            new CollectionRecipeDefinition(CollectionRecipeKind.CollectionBurstIn, "Launch all items from the center into their authored positions."),
            new CollectionRecipeDefinition(CollectionRecipeKind.CollectionBurstOut, "Scatter items away from the center while shrinking and fading."),
            new CollectionRecipeDefinition(CollectionRecipeKind.CollectionGatherTo, "Gather all items into one point while shrinking and fading.")
        };

        private static readonly DestinationRecipeDefinition[] DestinationRecipes =
        {
            new DestinationRecipeDefinition(DestinationRecipeKind.Arc, "Move through a signed vertical arc to an anchored destination."),
            new DestinationRecipeDefinition(DestinationRecipeKind.Bezier, "Follow a cubic anchored-position path with two explicit controls."),
            new DestinationRecipeDefinition(DestinationRecipeKind.Hop, "Anticipate, hop, land with a squash, and restore scale."),
            new DestinationRecipeDefinition(DestinationRecipeKind.Spring, "Pass the destination along the travel direction, then settle exactly."),
            new DestinationRecipeDefinition(DestinationRecipeKind.MagneticSnap, "Pull away before accelerating past and settling on the destination."),
            new DestinationRecipeDefinition(DestinationRecipeKind.PathThrough, "Traverse anchored waypoints with Catmull-Rom interpolation."),
            new DestinationRecipeDefinition(DestinationRecipeKind.Spiral, "Progress through a closing anchored spiral without endpoint jumps."),
            new DestinationRecipeDefinition(DestinationRecipeKind.MultiHop, "Advance through three diminishing hops and land exactly.")
        };

        private static readonly FeedbackRecipeDefinition[] FeedbackRecipes =
        {
            new FeedbackRecipeDefinition(FeedbackRecipeKind.ErrorReject, "Reject an action with a sharp shake, tilt, and red flash."),
            new FeedbackRecipeDefinition(FeedbackRecipeKind.DamageHit, "Communicate damage with a hit shake, grounded squash, recoil, and red flash."),
            new FeedbackRecipeDefinition(FeedbackRecipeKind.SuccessConfirm, "Confirm success with a pop, two diminishing bounces, and green flash."),
            new FeedbackRecipeDefinition(FeedbackRecipeKind.RewardReveal, "Reveal a reward with anticipation, relative spin, overshoot, pulse, and gold flash."),
            new FeedbackRecipeDefinition(FeedbackRecipeKind.PickupCollect, "Punch, arc, shrink, and fade into an anchored collection destination."),
            new FeedbackRecipeDefinition(FeedbackRecipeKind.HealReceive, "Communicate healing with a lift, restorative stretch, settle, and green flash."),
            new FeedbackRecipeDefinition(FeedbackRecipeKind.ShieldBlock, "Compress and recoil opposite a supplied impact direction."),
            new FeedbackRecipeDefinition(FeedbackRecipeKind.CriticalHit, "Combine a white-hot flash, heavy squash, recoil, and aftershock."),
            new FeedbackRecipeDefinition(FeedbackRecipeKind.CooldownReady, "Announce a ready ability with a flip, pop, lift, and cyan flash."),
            new FeedbackRecipeDefinition(FeedbackRecipeKind.LevelUp, "Celebrate progression with lift, spin, staged pulses, and gold flash."),
            new FeedbackRecipeDefinition(FeedbackRecipeKind.LowHealthWarning, "Play one finite double-beat warning cycle.")
        };

        private static readonly UISequenceRecipeDefinition[] UISequenceRecipes =
        {
            new UISequenceRecipeDefinition(UISequenceRecipeKind.ToastShow, "Slide, fade, overshoot, and settle a toast on its authored state."),
            new UISequenceRecipeDefinition(UISequenceRecipeKind.ToastHide, "Anticipate before sliding and fading a toast out."),
            new UISequenceRecipeDefinition(UISequenceRecipeKind.ModalOpen, "Fade the backdrop, open the panel, and stagger controls in."),
            new UISequenceRecipeDefinition(UISequenceRecipeKind.ModalClose, "Stagger controls out before dismissing the panel and backdrop."),
            new UISequenceRecipeDefinition(UISequenceRecipeKind.TooltipShow, "Subtly raise, scale, and fade a tooltip into view."),
            new UISequenceRecipeDefinition(UISequenceRecipeKind.TooltipHide, "Move and fade a tooltip out with restrained scale motion."),
            new UISequenceRecipeDefinition(UISequenceRecipeKind.DropdownOpen, "Expand from the authored pivot and stagger entries into view."),
            new UISequenceRecipeDefinition(UISequenceRecipeKind.DropdownClose, "Stagger entries out and compress toward the authored pivot."),
            new UISequenceRecipeDefinition(UISequenceRecipeKind.TabSwitch, "Overlap outgoing and incoming content in one controlled transition."),
            new UISequenceRecipeDefinition(UISequenceRecipeKind.DrawerShow, "Slide a drawer in from the left screen edge."),
            new UISequenceRecipeDefinition(UISequenceRecipeKind.DrawerHide, "Slide a drawer back through the left screen edge."),
            new UISequenceRecipeDefinition(UISequenceRecipeKind.BottomSheetShow, "Raise a bottom sheet with overshoot and backdrop fade."),
            new UISequenceRecipeDefinition(UISequenceRecipeKind.BottomSheetHide, "Dismiss a bottom sheet and backdrop below the screen."),
            new UISequenceRecipeDefinition(UISequenceRecipeKind.PagePush, "Push one page out while the next enters from the opposite side."),
            new UISequenceRecipeDefinition(UISequenceRecipeKind.PageCrossFade, "Cross-fade pages with restrained depth scaling.")
        };

        private static readonly TextValueRecipeDefinition[] TextValueRecipes =
        {
            new TextValueRecipeDefinition(TextValueRecipeKind.TypewriterReveal, "Reveal rich TextMesh Pro content without modifying its text string."),
            new TextValueRecipeDefinition(TextValueRecipeKind.TypewriterHide, "Hide currently visible TextMesh Pro content character by character."),
            new TextValueRecipeDefinition(TextValueRecipeKind.NumberCountUp, "Count upward with culture-aware numeric formatting."),
            new TextValueRecipeDefinition(TextValueRecipeKind.NumberCountDown, "Use the same count operation for a decreasing value."),
            new TextValueRecipeDefinition(TextValueRecipeKind.TextCharacterStaggerIn, "Reveal visible characters with directional offset, alpha, and scale."),
            new TextValueRecipeDefinition(TextValueRecipeKind.TextWave, "Send a finite wave across characters and restore the original mesh."),
            new TextValueRecipeDefinition(TextValueRecipeKind.ScoreIncrease, "Combine score counting with a restrained punch and color flash."),
            new TextValueRecipeDefinition(TextValueRecipeKind.TextCharacterStaggerOut, "Hide visible characters in reverse order with offset, alpha, and scale."),
            new TextValueRecipeDefinition(TextValueRecipeKind.TextCharacterBounce, "Send a traveling bounce across visible characters."),
            new TextValueRecipeDefinition(TextValueRecipeKind.TextColorSweep, "Sweep a temporary highlight through per-character vertex colors."),
            new TextValueRecipeDefinition(TextValueRecipeKind.TextGlitch, "Apply a deterministic seeded offset, scale, and color glitch."),
            new TextValueRecipeDefinition(TextValueRecipeKind.TextEmphasis, "Lift, scale, and color a selected visible-character range."),
            new TextValueRecipeDefinition(TextValueRecipeKind.TextScrambleReveal, "Resolve substitute glyphs into the untouched rich-text source.")
        };

        private static readonly string[] CollectionOrderNames =
        {
            "First to last",
            "Last to first",
            "From center",
            "To center",
            "Random (seeded)"
        };

        private readonly List<UIPresetListItem> _presetRows = new List<UIPresetListItem>();
        private readonly List<UIRecipeCard> _recipeCards = new List<UIRecipeCard>();
        private readonly List<UIRecipeCard> _collectionCards = new List<UIRecipeCard>();
        private readonly List<UIRecipeCard> _destinationCards = new List<UIRecipeCard>();
        private readonly List<UIRecipeCard> _feedbackCards = new List<UIRecipeCard>();
        private readonly List<UIRecipeCard> _uiSequenceCards = new List<UIRecipeCard>();
        private readonly List<UIRecipeCard> _textValueCards = new List<UIRecipeCard>();
        private readonly List<string> _targetOptionNames = new List<string>();
        private UIStateSnapshot _imageState;
        private UIStateSnapshot _textState;
        private UIStateSnapshot[] _collectionStates;
        private UIStateSnapshot _destinationState;
        private UIStateSnapshot _toastSequenceState;
        private UIStateSnapshot _modalBackdropState;
        private UIStateSnapshot _modalPanelState;
        private UIStateSnapshot[] _modalControlStates;
        private UIStateSnapshot _tooltipSequenceState;
        private UIStateSnapshot _dropdownPanelState;
        private UIStateSnapshot[] _dropdownEntryStates;
        private UIStateSnapshot _tabOutgoingState;
        private UIStateSnapshot _tabIncomingState;
        private TMPTextPreviewSnapshot _typewriterTextState;
        private TMPTextPreviewSnapshot _numberTextState;
        private TMPTextPreviewSnapshot _characterTextState;
        private TMPTextPreviewSnapshot _scoreTextState;
        private GameObject[] _listTargets;
        private GameObject[] _gridTargets;
        private GameObject[] _loadingDotTargets;
        private TweenHandle _activeTween;
        private ITweenPreset _selectedPreset;
        private UIRecipeKind _selectedRecipe = UIRecipeKind.UIAppear;
        private CollectionRecipeKind _selectedCollectionRecipe = CollectionRecipeKind.ListStaggerIn;
        private DestinationRecipeKind _selectedDestinationRecipe = DestinationRecipeKind.Arc;
        private FeedbackRecipeKind _selectedFeedbackRecipe = FeedbackRecipeKind.ErrorReject;
        private UISequenceRecipeKind _selectedUISequenceRecipe = UISequenceRecipeKind.ToastShow;
        private TextValueRecipeKind _selectedTextValueRecipe = TextValueRecipeKind.TypewriterReveal;
        private StaggerOrder _selectedCollectionOrder = StaggerOrder.FirstToLast;
        private ShowcaseMode _mode = ShowcaseMode.Recipes;
        private int _selectedTargetIndex;
        private bool _initialized;

        private GameObject PreviewTarget => targetDropdown.value == 1 ? animatedText.gameObject : presetImage.gameObject;

        private void Awake()
        {
            _imageState = UIStateSnapshot.Capture(presetImage.gameObject);
            _textState = UIStateSnapshot.Capture(animatedText.gameObject);
            _collectionStates = CaptureStates(collectionTargets);
            _destinationState = UIStateSnapshot.Capture(destinationTarget);
            _toastSequenceState = UIStateSnapshot.Capture(toastSequenceTarget);
            _modalBackdropState = UIStateSnapshot.Capture(modalSequenceBackdrop);
            _modalPanelState = UIStateSnapshot.Capture(modalSequencePanel);
            _modalControlStates = CaptureStates(modalSequenceControls);
            _tooltipSequenceState = UIStateSnapshot.Capture(tooltipSequenceTarget);
            _dropdownPanelState = UIStateSnapshot.Capture(dropdownSequencePanel);
            _dropdownEntryStates = CaptureStates(dropdownSequenceEntries);
            _tabOutgoingState = UIStateSnapshot.Capture(tabSequenceOutgoing);
            _tabIncomingState = UIStateSnapshot.Capture(tabSequenceIncoming);
            _typewriterTextState = TMPTextPreviewSnapshot.Capture(typewriterText);
            _numberTextState = TMPTextPreviewSnapshot.Capture(numberText);
            _characterTextState = TMPTextPreviewSnapshot.Capture(characterText);
            _scoreTextState = TMPTextPreviewSnapshot.Capture(scoreText);
            _listTargets = CopyTargets(6);
            _gridTargets = CopyTargets(9);
            _loadingDotTargets = CopyTargets(3);
            for (int i = 0; i < targetDropdown.options.Count; i++) _targetOptionNames.Add(targetDropdown.options[i].text);
            WireControls();
            BuildContent();
        }

        private void OnEnable()
        {
            ResetTargets();
            ShowRecipes();
            instructionsPanel.SetContent("TweenHelper 2D Showcase", "Choose UI Recipes, Collections, Destinations, Gameplay Feedback, UI Sequences, Text & Values, or the 2D Preset Library. Select an entry, then replay or reset the preview.");
        }

        private void OnDisable() => StopPlayback();

#if ENABLE_LEGACY_INPUT_MANAGER
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) ReplaySelected();
        }
#endif

        private void WireControls()
        {
            recipesTabButton.onClick.AddListener(ShowRecipes);
            presetsTabButton.onClick.AddListener(ShowPresets);
            collectionsTabButton.onClick.AddListener(ShowCollections);
            destinationsTabButton.onClick.AddListener(ShowDestinations);
            feedbackTabButton.onClick.AddListener(ShowFeedback);
            uiSequencesTabButton.onClick.AddListener(ShowUISequences);
            textValuesTabButton.onClick.AddListener(ShowTextValues);
            replayButton.onClick.AddListener(ReplaySelected);
            resetButton.onClick.AddListener(ResetPreview);
            copyButton.onClick.AddListener(CopyCodeExample);
            searchInput.onValueChanged.AddListener(_ => RefreshPresetRows());
            familyDropdown.onValueChanged.AddListener(_ => RefreshPresetRows());
            targetDropdown.onValueChanged.AddListener(_ => ChangeTarget());
        }

        private void BuildContent()
        {
            if (_initialized) return;
            _initialized = true;

            for (int i = 0; i < Recipes.Length; i++)
            {
                var definition = Recipes[i];
                var card = Instantiate(recipeCardPrefab, recipeContent);
                card.Configure(definition.Kind, definition.Description, SelectRecipe);
                _recipeCards.Add(card);
            }

            for (int i = 0; i < CollectionRecipes.Length; i++)
            {
                var definition = CollectionRecipes[i];
                var card = Instantiate(recipeCardPrefab, recipeContent);
                CollectionRecipeKind kind = definition.Kind;
                card.Configure(kind.ToString(), definition.Description, () => SelectCollectionRecipe(kind));
                _collectionCards.Add(card);
            }

            for (int i = 0; i < DestinationRecipes.Length; i++)
            {
                var definition = DestinationRecipes[i];
                var card = Instantiate(recipeCardPrefab, recipeContent);
                DestinationRecipeKind kind = definition.Kind;
                card.Configure(kind.ToString(), definition.Description, () => SelectDestinationRecipe(kind));
                _destinationCards.Add(card);
            }

            for (int i = 0; i < FeedbackRecipes.Length; i++)
            {
                var definition = FeedbackRecipes[i];
                var card = Instantiate(recipeCardPrefab, recipeContent);
                FeedbackRecipeKind kind = definition.Kind;
                card.Configure(kind.ToString(), definition.Description, () => SelectFeedbackRecipe(kind));
                _feedbackCards.Add(card);
            }

            for (int i = 0; i < UISequenceRecipes.Length; i++)
            {
                var definition = UISequenceRecipes[i];
                var card = Instantiate(recipeCardPrefab, recipeContent);
                UISequenceRecipeKind kind = definition.Kind;
                card.Configure(SplitPascalCase(kind.ToString()), definition.Description, () => SelectUISequenceRecipe(kind));
                _uiSequenceCards.Add(card);
            }

            for (int i = 0; i < TextValueRecipes.Length; i++)
            {
                var definition = TextValueRecipes[i];
                var card = Instantiate(recipeCardPrefab, recipeContent);
                TextValueRecipeKind kind = definition.Kind;
                card.Configure(SplitPascalCase(kind.ToString()), definition.Description, () => SelectTextValueRecipe(kind));
                _textValueCards.Add(card);
            }

            TweenPresetRegistry.ScanForCodePresets();
            var presets = UIPresetCompatibility.GetSuitablePresets(TweenPresetRegistry.Presets);
            for (int i = 0; i < presets.Count; i++)
            {
                var row = Instantiate(presetListItemPrefab, presetContent);
                row.Configure(presets[i], SelectPreset);
                _presetRows.Add(row);
            }

            BuildFamilyOptions(presets);
            if (presets.Count > 0) SelectPreset(presets[0], false);
            RefreshPresetRows();

            if (presets.Count != UIPresetCompatibility.ExpectedPresetCount)
            {
                Debug.LogWarning($"TweenHelper 2D Showcase expected {UIPresetCompatibility.ExpectedPresetCount} UI-suitable presets but discovered {presets.Count}. Review UIPresetCompatibility after changing the registry.");
            }
        }

        private void BuildFamilyOptions(List<ITweenPreset> presets)
        {
            var families = new SortedSet<string>(StringComparer.Ordinal) { "All families" };
            for (int i = 0; i < presets.Count; i++) families.Add(PresetFamilyClassifier.GetFamilyName(presets[i].PresetName));
            familyDropdown.ClearOptions();
            familyDropdown.AddOptions(new List<string>(families));
            familyDropdown.value = 0;
        }

        public void ShowRecipes()
        {
            StopPlayback();
            ResetTargets();
            _mode = ShowcaseMode.Recipes;
            recipesPanel.SetActive(true);
            presetsPanel.SetActive(false);
            SetCardVisibility(ShowcaseMode.Recipes);
            ResetRecipeScrollPosition();
            SetPreviewMode(false, false);
            RestoreTargetOptions();
            SelectRecipe(_selectedRecipe, false);
        }

        public void ShowPresets()
        {
            StopPlayback();
            ResetTargets();
            _mode = ShowcaseMode.Presets;
            recipesPanel.SetActive(false);
            presetsPanel.SetActive(true);
            SetPreviewMode(false, false);
            RestoreTargetOptions();
            RefreshPresetRows();
            if (_selectedPreset != null) UpdatePresetDetails(_selectedPreset);
        }

        public void ShowCollections()
        {
            StopPlayback();
            ResetTargets();
            _mode = ShowcaseMode.Collections;
            recipesPanel.SetActive(true);
            presetsPanel.SetActive(false);
            SetCardVisibility(ShowcaseMode.Collections);
            ResetRecipeScrollPosition();
            SetPreviewMode(true, false);
            ShowCollectionOrderOptions();
            SelectCollectionRecipe(_selectedCollectionRecipe, false);
        }

        public void ShowDestinations()
        {
            StopPlayback();
            ResetTargets();
            _mode = ShowcaseMode.Destinations;
            recipesPanel.SetActive(true);
            presetsPanel.SetActive(false);
            SetCardVisibility(ShowcaseMode.Destinations);
            ResetRecipeScrollPosition();
            SetPreviewMode(false, true);
            ShowDestinationTargetOption();
            SelectDestinationRecipe(_selectedDestinationRecipe, false);
        }

        public void ShowFeedback()
        {
            StopPlayback();
            ResetTargets();
            _mode = ShowcaseMode.Feedback;
            recipesPanel.SetActive(true);
            presetsPanel.SetActive(false);
            SetCardVisibility(ShowcaseMode.Feedback);
            ResetRecipeScrollPosition();
            SetPreviewMode(false, true);
            ShowDestinationTargetOption();
            SelectFeedbackRecipe(_selectedFeedbackRecipe, false);
        }

        public void ShowUISequences()
        {
            StopPlayback();
            ResetTargets();
            _mode = ShowcaseMode.UISequences;
            recipesPanel.SetActive(true);
            presetsPanel.SetActive(false);
            SetCardVisibility(ShowcaseMode.UISequences);
            ResetRecipeScrollPosition();
            SetPreviewMode(false, false, true);
            ShowUISequenceTargetOption();
            SelectUISequenceRecipe(_selectedUISequenceRecipe, false);
        }

        public void ShowTextValues()
        {
            StopPlayback();
            ResetTargets();
            _mode = ShowcaseMode.TextValues;
            recipesPanel.SetActive(true);
            presetsPanel.SetActive(false);
            SetCardVisibility(ShowcaseMode.TextValues);
            ResetRecipeScrollPosition();
            SetPreviewMode(false, false, false, true);
            ShowTextValueTargetOption();
            SelectTextValueRecipe(_selectedTextValueRecipe, false);
        }

        public void ReplaySelected()
        {
            if (_mode == ShowcaseMode.Recipes) PlayRecipe(_selectedRecipe);
            else if (_mode == ShowcaseMode.Presets) PlaySelectedPreset();
            else if (_mode == ShowcaseMode.Collections) PlayCollectionRecipe(_selectedCollectionRecipe);
            else if (_mode == ShowcaseMode.Destinations) PlayDestinationRecipe(_selectedDestinationRecipe);
            else if (_mode == ShowcaseMode.Feedback) PlayFeedbackRecipe(_selectedFeedbackRecipe);
            else if (_mode == ShowcaseMode.UISequences) PlayUISequenceRecipe(_selectedUISequenceRecipe);
            else PlayTextValueRecipe(_selectedTextValueRecipe);
        }

        public void ResetPreview()
        {
            StopPlayback();
            ResetTargets();
            if (_mode == ShowcaseMode.Feedback) PositionFeedbackTarget(_selectedFeedbackRecipe);
        }

        public void CopyCodeExample() => GUIUtility.systemCopyBuffer = codeExampleText.text;

        private void ChangeTarget()
        {
            if (_mode == ShowcaseMode.Collections)
            {
                _selectedCollectionOrder = (StaggerOrder)targetDropdown.value;
                ResetPreview();
                SelectCollectionRecipe(_selectedCollectionRecipe, false);
                return;
            }

            if (_mode == ShowcaseMode.Destinations || _mode == ShowcaseMode.Feedback || _mode == ShowcaseMode.UISequences || _mode == ShowcaseMode.TextValues) return;

            _selectedTargetIndex = targetDropdown.value;
            ResetPreview();
            RefreshPresetRows();
        }

        private void SelectRecipe(UIRecipeKind recipe) => SelectRecipe(recipe, true);

        private void SelectRecipe(UIRecipeKind recipe, bool play)
        {
            _selectedRecipe = recipe;
            var definition = Recipes[(int)recipe];
            selectionNameText.text = recipe.ToString();
            selectionDescriptionText.text = definition.Description;
            codeExampleText.text = $"target.{recipe}();";
            if (play) PlayRecipe(recipe);
        }

        private void SelectPreset(ITweenPreset preset) => SelectPreset(preset, true);

        private void SelectPreset(ITweenPreset preset, bool play)
        {
            _selectedPreset = preset;
            UpdatePresetDetails(preset);
            if (play) PlaySelectedPreset();
        }

        private void SelectCollectionRecipe(CollectionRecipeKind recipe)
        {
            _selectedCollectionOrder = recipe == CollectionRecipeKind.ListStaggerOut ? StaggerOrder.LastToFirst : StaggerOrder.FirstToLast;
            targetDropdown.SetValueWithoutNotify((int)_selectedCollectionOrder);
            SelectCollectionRecipe(recipe, true);
        }

        private void SelectCollectionRecipe(CollectionRecipeKind recipe, bool play)
        {
            _selectedCollectionRecipe = recipe;
            var definition = CollectionRecipes[(int)recipe];
            selectionNameText.text = recipe.ToString();
            selectionDescriptionText.text = definition.Description;
            codeExampleText.text = GetCollectionCodeExample(recipe);
            ConfigureCollectionLayout(recipe);
            targetDropdown.interactable = recipe == CollectionRecipeKind.ListStaggerIn || recipe == CollectionRecipeKind.ListStaggerOut;
            if (play) PlayCollectionRecipe(recipe);
        }

        private void SelectDestinationRecipe(DestinationRecipeKind recipe) => SelectDestinationRecipe(recipe, true);

        private void SelectDestinationRecipe(DestinationRecipeKind recipe, bool play)
        {
            _selectedDestinationRecipe = recipe;
            var definition = DestinationRecipes[(int)recipe];
            selectionNameText.text = recipe == DestinationRecipeKind.MagneticSnap ? "Magnetic Snap" : recipe.ToString();
            selectionDescriptionText.text = definition.Description;
            codeExampleText.text = GetDestinationCodeExample(recipe);
            destinationStartMarker.gameObject.SetActive(true);
            destinationEndMarker.gameObject.SetActive(true);
            if (UsesCurvedPath(recipe)) UpdateDestinationPath(recipe);
            destinationCurvedPath.SetActive(UsesCurvedPath(recipe));
            if (play) PlayDestinationRecipe(recipe);
        }

        private void SelectFeedbackRecipe(FeedbackRecipeKind recipe) => SelectFeedbackRecipe(recipe, true);

        private void SelectFeedbackRecipe(FeedbackRecipeKind recipe, bool play)
        {
            _selectedFeedbackRecipe = recipe;
            var definition = FeedbackRecipes[(int)recipe];
            selectionNameText.text = SplitPascalCase(recipe.ToString());
            selectionDescriptionText.text = definition.Description;
            codeExampleText.text = GetFeedbackCodeExample(recipe);
            bool usesDestination = recipe == FeedbackRecipeKind.PickupCollect;
            PositionFeedbackTarget(recipe);
            destinationStartMarker.gameObject.SetActive(usesDestination);
            destinationEndMarker.gameObject.SetActive(usesDestination);
            destinationCurvedPath.SetActive(usesDestination);
            if (usesDestination) UpdateDestinationPath(false);
            if (play) PlayFeedbackRecipe(recipe);
        }

        private void SelectUISequenceRecipe(UISequenceRecipeKind recipe) => SelectUISequenceRecipe(recipe, true);

        private void SelectUISequenceRecipe(UISequenceRecipeKind recipe, bool play)
        {
            _selectedUISequenceRecipe = recipe;
            var definition = UISequenceRecipes[(int)recipe];
            selectionNameText.text = recipe == UISequenceRecipeKind.TabSwitch ? "Tab Switch" : SplitPascalCase(recipe.ToString());
            selectionDescriptionText.text = definition.Description;
            codeExampleText.text = GetUISequenceCodeExample(recipe);
            ConfigureUISequencePreview(recipe);
            if (play) PlayUISequenceRecipe(recipe);
        }

        private void SelectTextValueRecipe(TextValueRecipeKind recipe) => SelectTextValueRecipe(recipe, true);

        private void SelectTextValueRecipe(TextValueRecipeKind recipe, bool play)
        {
            _selectedTextValueRecipe = recipe;
            var definition = TextValueRecipes[(int)recipe];
            selectionNameText.text = SplitPascalCase(recipe.ToString());
            selectionDescriptionText.text = definition.Description;
            codeExampleText.text = GetTextValueCodeExample(recipe);
            ConfigureTextValuePreview(recipe);
            if (play) PlayTextValueRecipe(recipe);
        }

        private void UpdatePresetDetails(ITweenPreset preset)
        {
            float? previewStrength = UIPresetCompatibility.GetCanvasPreviewStrength(preset);
            selectionNameText.text = preset.PresetName;
            selectionDescriptionText.text = previewStrength.HasValue
                ? $"{preset.Description} | Canvas preview uses {previewStrength.Value:0.#}x movement strength"
                : preset.Description;
            codeExampleText.text = previewStrength.HasValue
                ? $"target.Tween().WithOptions(TweenOptions.WithStrength({previewStrength.Value:0.#}f)).Preset<{preset.GetType().Name}>().Play();"
                : $"target.Tween().Preset<{preset.GetType().Name}>().Play();";
        }

        private void PlaySelectedPreset()
        {
            if (_selectedPreset == null || !_selectedPreset.CanApplyTo(PreviewTarget)) return;
            StopPlayback();
            ResetTarget(PreviewTarget);
            var builder = PreviewTarget.Tween();
            float? previewStrength = UIPresetCompatibility.GetCanvasPreviewStrength(_selectedPreset);
            if (previewStrength.HasValue) builder.WithOptions(TweenOptions.WithStrength(previewStrength.Value));
            _activeTween = builder.Preset(_selectedPreset).Play();
        }

        private TweenHandle PlayRecipe(UIRecipeKind recipe)
        {
            StopPlayback();
            var target = PreviewTarget;
            ResetTarget(target);

            switch (recipe)
            {
                case UIRecipeKind.UIAppear: return _activeTween = target.UIAppear();
                case UIRecipeKind.UIAppearSoft: return _activeTween = target.UIAppearSoft();
                case UIRecipeKind.UIDisappear: return _activeTween = target.UIDisappear();
                case UIRecipeKind.UIDisappearSoft: return _activeTween = target.UIDisappearSoft();
                case UIRecipeKind.UIHover: return _activeTween = target.UIHover(hoverColor: GetHoverColor(target));
                case UIRecipeKind.UIHoverSoft: return _activeTween = target.UIHoverSoft(hoverColor: GetHoverColor(target));
                case UIRecipeKind.UIPress: return _activeTween = target.UIPress();
                case UIRecipeKind.UIPressHard: return _activeTween = target.UIPressHard();
                case UIRecipeKind.UIAttention: return _activeTween = target.UIAttention();
                case UIRecipeKind.UIAttentionSoft: return _activeTween = target.UIAttentionSoft();
                case UIRecipeKind.UIAttentionHard: return _activeTween = target.UIAttentionHard();
                case UIRecipeKind.UIDisabled: return _activeTween = target.UIDisabled(disabledColor: GetDisabledColor(target));
                case UIRecipeKind.UIEnabled:
                    target.UIDisabled(0.01f, GetDisabledColor(target)).Complete();
                    return _activeTween = target.UIEnabled();
                default: return null;
            }
        }

        private TweenHandle PlayCollectionRecipe(CollectionRecipeKind recipe)
        {
            StopPlayback();
            ResetCollectionTargets();
            ConfigureCollectionLayout(recipe);

            switch (recipe)
            {
                case CollectionRecipeKind.ListStaggerIn:
                    return _activeTween = _listTargets.TweenStagger(collectionPreviewRoot)
                        .Preset<PopInFadePreset>(0.32f)
                        .Order(_selectedCollectionOrder)
                        .DelayBetween(0.06f)
                        .Seed(1729)
                        .Play();
                case CollectionRecipeKind.ListStaggerOut:
                    return _activeTween = _listTargets.TweenStagger(collectionPreviewRoot)
                        .Preset<PopOutFadePreset>(0.26f)
                        .Order(_selectedCollectionOrder)
                        .DelayBetween(0.04f)
                        .Seed(1729)
                        .Play();
                case CollectionRecipeKind.GridWave:
                    return _activeTween = _gridTargets.GridWave(collectionPreviewRoot, 3);
                case CollectionRecipeKind.GridRipple:
                    return _activeTween = _gridTargets.GridRipple(collectionPreviewRoot, 3);
                case CollectionRecipeKind.LoadingDots:
                    return _activeTween = _loadingDotTargets.LoadingDots(collectionPreviewRoot);
                case CollectionRecipeKind.GridDiagonalWave:
                    return _activeTween = _gridTargets.GridDiagonalWave(collectionPreviewRoot, 3, GridDiagonalDirection.TopLeftToBottomRight, 0.34f, 0.08f);
                case CollectionRecipeKind.GridSpiral:
                    return _activeTween = _gridTargets.GridSpiral(collectionPreviewRoot, 3, GridSpiralDirection.OutsideInClockwise, 0.32f, 0.065f);
                case CollectionRecipeKind.GridCheckerboard:
                    return _activeTween = _gridTargets.GridCheckerboard(collectionPreviewRoot, 3, false, 0.4f, 0.2f);
                case CollectionRecipeKind.CollectionBurstIn:
                    return _activeTween = _gridTargets.CollectionBurstIn(collectionPreviewRoot, Vector3.zero, 0.56f, 0.05f);
                case CollectionRecipeKind.CollectionBurstOut:
                    return _activeTween = _gridTargets.CollectionBurstOut(collectionPreviewRoot, Vector3.zero, 160f, 0.5f, 0.045f);
                case CollectionRecipeKind.CollectionGatherTo:
                    return _activeTween = _gridTargets.CollectionGatherTo(collectionPreviewRoot, Vector3.zero, 0.6f, 0.05f);
                default:
                    return null;
            }
        }

        private TweenHandle PlayDestinationRecipe(DestinationRecipeKind recipe)
        {
            StopPlayback();
            _destinationState.Apply(destinationTarget);
            var targetRect = (RectTransform)destinationTarget.transform;
            Vector3 start = destinationStartMarker.anchoredPosition3D;
            Vector3 destination = destinationEndMarker.anchoredPosition3D;
            targetRect.anchoredPosition3D = start;

            switch (recipe)
            {
                case DestinationRecipeKind.Arc:
                    return _activeTween = destinationTarget.Tween().ArcLocalTo(destination, DestinationArcHeight, 1.2f).Play();
                case DestinationRecipeKind.Bezier:
                {
                    GetBezierControls(start, destination, out Vector3 controlA, out Vector3 controlB);
                    return _activeTween = destinationTarget.Tween().BezierLocalTo(destination, controlA, controlB, 1.35f).Play();
                }
                case DestinationRecipeKind.Hop:
                    return _activeTween = destinationTarget.Tween().HopLocalTo(destination, DestinationArcHeight, 1.35f).Play();
                case DestinationRecipeKind.Spring:
                    return _activeTween = destinationTarget.Tween().SpringLocalTo(destination, 1f, 38f).Play();
                case DestinationRecipeKind.MagneticSnap:
                    return _activeTween = destinationTarget.Tween().MagneticSnapLocalTo(destination, 1.1f, 32f, 26f).Play();
                case DestinationRecipeKind.PathThrough:
                    return _activeTween = destinationTarget.Tween().PathLocalThrough(GetPathWaypoints(start, destination), DestinationPathInterpolation.CatmullRom, 1.5f).Play();
                case DestinationRecipeKind.Spiral:
                    return _activeTween = destinationTarget.Tween().SpiralLocalTo(destination, 82f, 1.75f, 1.5f).Play();
                case DestinationRecipeKind.MultiHop:
                    return _activeTween = destinationTarget.Tween().MultiHopLocalTo(destination, DestinationArcHeight, 3, 1.15f, 1.5f).Play();
                default:
                    return null;
            }
        }

        private TweenHandle PlayFeedbackRecipe(FeedbackRecipeKind recipe)
        {
            StopPlayback();
            PositionFeedbackTarget(recipe);
            Vector3 destination = destinationEndMarker.anchoredPosition3D;

            switch (recipe)
            {
                case FeedbackRecipeKind.ErrorReject:
                    return _activeTween = destinationTarget.ErrorReject(0.72f);
                case FeedbackRecipeKind.DamageHit:
                    return _activeTween = destinationTarget.DamageHit(0.68f);
                case FeedbackRecipeKind.SuccessConfirm:
                    return _activeTween = destinationTarget.SuccessConfirm(0.95f);
                case FeedbackRecipeKind.RewardReveal:
                    return _activeTween = destinationTarget.RewardReveal(1.28f);
                case FeedbackRecipeKind.PickupCollect:
                    return _activeTween = destinationTarget.PickupCollectLocalTo(destination, DestinationArcHeight, 1.35f);
                case FeedbackRecipeKind.HealReceive:
                    return _activeTween = destinationTarget.HealReceive(0.95f);
                case FeedbackRecipeKind.ShieldBlock:
                    return _activeTween = destinationTarget.ShieldBlock(Vector3.right, 0.76f);
                case FeedbackRecipeKind.CriticalHit:
                    return _activeTween = destinationTarget.CriticalHit(new Vector3(1f, -0.2f, 0f), 0.86f);
                case FeedbackRecipeKind.CooldownReady:
                    return _activeTween = destinationTarget.CooldownReady(0.95f);
                case FeedbackRecipeKind.LevelUp:
                    return _activeTween = destinationTarget.LevelUp(1.3f);
                case FeedbackRecipeKind.LowHealthWarning:
                    return _activeTween = destinationTarget.LowHealthWarning(1.05f);
                default:
                    return null;
            }
        }

        private TweenHandle PlayUISequenceRecipe(UISequenceRecipeKind recipe)
        {
            StopPlayback();
            ResetUISequenceTargets();
            ConfigureUISequencePreview(recipe);

            switch (recipe)
            {
                case UISequenceRecipeKind.ToastShow:
                    return _activeTween = toastSequenceTarget.ToastShow(UISequenceDirection.Up, 64f, 0.5f);
                case UISequenceRecipeKind.ToastHide:
                    return _activeTween = toastSequenceTarget.ToastHide(UISequenceDirection.Up, 64f, 0.38f);
                case UISequenceRecipeKind.ModalOpen:
                    return _activeTween = modalSequencePanel.ModalOpen(modalSequenceBackdrop, modalSequenceControls, 0.68f, 0.075f);
                case UISequenceRecipeKind.ModalClose:
                    return _activeTween = modalSequencePanel.ModalClose(modalSequenceBackdrop, modalSequenceControls, 0.58f, 0.075f);
                case UISequenceRecipeKind.TooltipShow:
                    return _activeTween = tooltipSequenceTarget.TooltipShow(UISequenceDirection.Up, 22f, 0.36f);
                case UISequenceRecipeKind.TooltipHide:
                    return _activeTween = tooltipSequenceTarget.TooltipHide(UISequenceDirection.Up, 22f, 0.28f);
                case UISequenceRecipeKind.DropdownOpen:
                    return _activeTween = dropdownSequencePanel.DropdownOpen(dropdownSequenceEntries, 0.54f, 0.06f);
                case UISequenceRecipeKind.DropdownClose:
                    return _activeTween = dropdownSequencePanel.DropdownClose(dropdownSequenceEntries, 0.44f, 0.06f);
                case UISequenceRecipeKind.TabSwitch:
                    return _activeTween = tabSequenceOutgoing.TabSwitchTo(tabSequenceIncoming, UISequenceDirection.Left, 108f, 0.58f);
                case UISequenceRecipeKind.DrawerShow:
                    return _activeTween = dropdownSequencePanel.DrawerShow(UISequenceDirection.Left, null, 390f, 0.64f);
                case UISequenceRecipeKind.DrawerHide:
                    return _activeTween = dropdownSequencePanel.DrawerHide(UISequenceDirection.Left, null, 390f, 0.48f);
                case UISequenceRecipeKind.BottomSheetShow:
                    return _activeTween = modalSequencePanel.BottomSheetShow(modalSequenceBackdrop, 430f, 0.72f);
                case UISequenceRecipeKind.BottomSheetHide:
                    return _activeTween = modalSequencePanel.BottomSheetHide(modalSequenceBackdrop, 430f, 0.58f);
                case UISequenceRecipeKind.PagePush:
                    return _activeTween = tabSequenceOutgoing.PagePushTo(tabSequenceIncoming, UISequenceDirection.Left, 600f, 0.68f);
                case UISequenceRecipeKind.PageCrossFade:
                    return _activeTween = tabSequenceOutgoing.PageCrossFadeTo(tabSequenceIncoming, 0.06f, 0.58f);
                default:
                    return null;
            }
        }

        private TweenHandle PlayTextValueRecipe(TextValueRecipeKind recipe)
        {
            StopPlayback();
            ResetTextValueTargets();
            ConfigureTextValuePreview(recipe);

            switch (recipe)
            {
                case TextValueRecipeKind.TypewriterReveal:
                    return _activeTween = typewriterText.TypewriterReveal(1.15f);
                case TextValueRecipeKind.TypewriterHide:
                    return _activeTween = typewriterText.TypewriterHide(0.95f);
                case TextValueRecipeKind.NumberCountUp:
                    return _activeTween = numberText.NumberCountTo(0d, 1250d, "N0", 1.1f);
                case TextValueRecipeKind.NumberCountDown:
                    return _activeTween = numberText.NumberCountTo(1250d, 0d, "N0", 1.1f);
                case TextValueRecipeKind.TextCharacterStaggerIn:
                    return _activeTween = characterText.TextCharacterStaggerIn(UISequenceDirection.Up, 26f, 0.04f, 1f);
                case TextValueRecipeKind.TextWave:
                    return _activeTween = characterText.TextWave(UISequenceDirection.Up, 20f, 1, 1.2f);
                case TextValueRecipeKind.ScoreIncrease:
                    return _activeTween = scoreText.ScoreIncrease(1200d, 1475d, "N0", 1.15f);
                case TextValueRecipeKind.TextCharacterStaggerOut:
                    return _activeTween = characterText.TextCharacterStaggerOut(UISequenceDirection.Up, 28f, 0.04f, 1f);
                case TextValueRecipeKind.TextCharacterBounce:
                    return _activeTween = characterText.TextCharacterBounce(UISequenceDirection.Up, 22f, 1.15f);
                case TextValueRecipeKind.TextColorSweep:
                    return _activeTween = characterText.TextColorSweep(new Color(0.18f, 0.9f, 1f), 2.4f, 1.15f);
                case TextValueRecipeKind.TextGlitch:
                    return _activeTween = characterText.TextGlitch(8f, 1729, 0.86f);
                case TextValueRecipeKind.TextEmphasis:
                    return _activeTween = characterText.TextEmphasis(UISequenceDirection.Up, 11f, 0, 9, new Color(1f, 0.7f, 0.12f), 0.86f);
                case TextValueRecipeKind.TextScrambleReveal:
                    return _activeTween = characterText.TextScrambleReveal(1729, 1.3f);
                default:
                    return null;
            }
        }

        private void PositionFeedbackTarget(FeedbackRecipeKind recipe)
        {
            Vector3 start = destinationStartMarker.anchoredPosition3D;
            Vector3 destination = destinationEndMarker.anchoredPosition3D;
            _destinationState.Apply(destinationTarget);
            ((RectTransform)destinationTarget.transform).anchoredPosition3D = recipe == FeedbackRecipeKind.PickupCollect ? start : Vector3.Lerp(start, destination, 0.5f);
        }

        private Color GetHoverColor(GameObject target) => target == animatedText.gameObject ? textHoverColor : imageHoverColor;

        private Color GetDisabledColor(GameObject target) => target == animatedText.gameObject ? disabledTextColor : new Color(0.45f, 0.45f, 0.45f, 0.55f);

        private void RefreshPresetRows()
        {
            string search = searchInput.text ?? string.Empty;
            string family = familyDropdown.options.Count > 0 ? familyDropdown.options[familyDropdown.value].text : "All families";
            int visible = 0;
            ITweenPreset firstVisiblePreset = null;
            bool selectedPresetIsVisible = false;

            for (int i = 0; i < _presetRows.Count; i++)
            {
                var preset = _presetRows[i].Preset;
                bool matchesSearch = preset.PresetName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
                bool matchesFamily = family == "All families" || PresetFamilyClassifier.GetFamilyName(preset.PresetName) == family;
                bool compatible = preset.CanApplyTo(PreviewTarget);
                bool show = matchesSearch && matchesFamily && compatible;
                _presetRows[i].gameObject.SetActive(show);
                if (!show) continue;

                firstVisiblePreset ??= preset;
                selectedPresetIsVisible |= preset == _selectedPreset;
                visible++;
            }

            visibleCountText.text = $"{visible} / {_presetRows.Count} presets";
            if (_mode == ShowcaseMode.Presets && !selectedPresetIsVisible && firstVisiblePreset != null)
            {
                SelectPreset(firstVisiblePreset, false);
            }
        }

        private void StopPlayback()
        {
            _activeTween?.Kill();
            _activeTween = null;
            KillTargetTweens(presetImage.gameObject);
            KillTargetTweens(animatedText.gameObject);
            DOTween.Kill(collectionPreviewRoot, false);
            for (int i = 0; i < collectionTargets.Length; i++) KillTargetTweens(collectionTargets[i]);
            KillTargetTweens(destinationTarget);
            KillTargetTweens(toastSequenceTarget);
            KillTargetTweens(modalSequenceBackdrop);
            KillTargetTweens(modalSequencePanel);
            KillTargets(modalSequenceControls);
            KillTargetTweens(tooltipSequenceTarget);
            KillTargetTweens(dropdownSequencePanel);
            KillTargets(dropdownSequenceEntries);
            KillTargetTweens(tabSequenceOutgoing);
            KillTargetTweens(tabSequenceIncoming);
            KillTargetTweens(typewriterText.gameObject);
            KillTargetTweens(numberText.gameObject);
            KillTargetTweens(characterText.gameObject);
            KillTargetTweens(scoreText.gameObject);
        }

        private void ResetTargets()
        {
            _imageState.Apply(presetImage.gameObject);
            _textState.Apply(animatedText.gameObject);
            ResetCollectionTargets();
            _destinationState.Apply(destinationTarget);
            ResetUISequenceTargets();
            ResetTextValueTargets();
        }

        private void ResetTarget(GameObject target)
        {
            if (target == animatedText.gameObject) _textState.Apply(target);
            else _imageState.Apply(target);
        }

        internal int AuditStepCount => Recipes.Length;

        internal string GetAuditStepLabel(int index) => Recipes[NormalizeStepIndex(index)].Kind.ToString();

        internal TweenHandle PlayAuditStep(int index)
        {
            targetDropdown.SetValueWithoutNotify(0);
            return PlayRecipe(Recipes[NormalizeStepIndex(index)].Kind);
        }

        internal void ResetAuditState() => ResetPreview();

        internal AnimationPresetDisplay.ResetVerificationResult VerifyAuditResetState(float positionTolerance = 0.001f, float scaleTolerance = 0.001f, float rotationAngleTolerance = 0.1f, float colorTolerance = 0.01f)
        {
            var imageResult = VerifyTargetState(presetImage.gameObject, _imageState, "Image", positionTolerance, scaleTolerance, rotationAngleTolerance, colorTolerance);
            var textResult = VerifyTargetState(animatedText.gameObject, _textState, "Text", positionTolerance, scaleTolerance, rotationAngleTolerance, colorTolerance);
            return new AnimationPresetDisplay.ResetVerificationResult
            {
                TransformMatches = imageResult.TransformMatches && textResult.TransformMatches,
                AlphaMatches = imageResult.AlphaMatches && textResult.AlphaMatches,
                NoActiveTweens = imageResult.NoActiveTweens && textResult.NoActiveTweens,
                PositionError = Mathf.Max(imageResult.PositionError, textResult.PositionError),
                ScaleError = Mathf.Max(imageResult.ScaleError, textResult.ScaleError),
                RotationAngleError = Mathf.Max(imageResult.RotationAngleError, textResult.RotationAngleError),
                Details = JoinDetails(imageResult.Details, textResult.Details)
            };
        }

        private static int NormalizeStepIndex(int index)
        {
            int normalized = index % Recipes.Length;
            return normalized < 0 ? normalized + Recipes.Length : normalized;
        }

        private void SetCardVisibility(ShowcaseMode mode)
        {
            for (int i = 0; i < _recipeCards.Count; i++) _recipeCards[i].gameObject.SetActive(mode == ShowcaseMode.Recipes);
            for (int i = 0; i < _collectionCards.Count; i++) _collectionCards[i].gameObject.SetActive(mode == ShowcaseMode.Collections);
            for (int i = 0; i < _destinationCards.Count; i++) _destinationCards[i].gameObject.SetActive(mode == ShowcaseMode.Destinations);
            for (int i = 0; i < _feedbackCards.Count; i++) _feedbackCards[i].gameObject.SetActive(mode == ShowcaseMode.Feedback);
            for (int i = 0; i < _uiSequenceCards.Count; i++) _uiSequenceCards[i].gameObject.SetActive(mode == ShowcaseMode.UISequences);
            for (int i = 0; i < _textValueCards.Count; i++) _textValueCards[i].gameObject.SetActive(mode == ShowcaseMode.TextValues);
        }

        private void ResetRecipeScrollPosition()
        {
            var contentRect = (RectTransform)recipeContent;
            Vector2 position = contentRect.anchoredPosition;
            position.y = 0f;
            contentRect.anchoredPosition = position;
        }

        private void SetPreviewMode(bool showCollection, bool showDestination, bool showUISequence = false, bool showTextValue = false)
        {
            presetImage.gameObject.SetActive(!showCollection && !showDestination && !showUISequence && !showTextValue);
            animatedText.gameObject.SetActive(!showCollection && !showDestination && !showUISequence && !showTextValue);
            collectionPreviewRoot.SetActive(showCollection);
            destinationPreviewRoot.SetActive(showDestination);
            uiSequencePreviewRoot.SetActive(showUISequence);
            textValuePreviewRoot.SetActive(showTextValue);
        }

        private void RestoreTargetOptions()
        {
            if (targetDropdown.options.Count != _targetOptionNames.Count || targetDropdown.options[0].text != _targetOptionNames[0])
            {
                targetDropdown.ClearOptions();
                targetDropdown.AddOptions(_targetOptionNames);
            }

            targetDropdown.SetValueWithoutNotify(_selectedTargetIndex);
            targetDropdown.interactable = true;
        }

        private void ShowCollectionOrderOptions()
        {
            targetDropdown.ClearOptions();
            targetDropdown.AddOptions(new List<string>(CollectionOrderNames));
            targetDropdown.SetValueWithoutNotify((int)_selectedCollectionOrder);
        }

        private void ShowDestinationTargetOption()
        {
            targetDropdown.ClearOptions();
            targetDropdown.AddOptions(new List<string> { "Anchored UI" });
            targetDropdown.SetValueWithoutNotify(0);
            targetDropdown.interactable = false;
        }

        private void ShowUISequenceTargetOption()
        {
            targetDropdown.ClearOptions();
            targetDropdown.AddOptions(new List<string> { "Production UI" });
            targetDropdown.SetValueWithoutNotify(0);
            targetDropdown.interactable = false;
        }

        private void ShowTextValueTargetOption()
        {
            targetDropdown.ClearOptions();
            targetDropdown.AddOptions(new List<string> { "TextMesh Pro" });
            targetDropdown.SetValueWithoutNotify(0);
            targetDropdown.interactable = false;
        }

        private void ConfigureUISequencePreview(UISequenceRecipeKind recipe)
        {
            bool showToast = recipe == UISequenceRecipeKind.ToastShow || recipe == UISequenceRecipeKind.ToastHide;
            bool showModal = recipe == UISequenceRecipeKind.ModalOpen || recipe == UISequenceRecipeKind.ModalClose || recipe == UISequenceRecipeKind.BottomSheetShow || recipe == UISequenceRecipeKind.BottomSheetHide;
            bool showTooltip = recipe == UISequenceRecipeKind.TooltipShow || recipe == UISequenceRecipeKind.TooltipHide;
            bool showDropdown = recipe == UISequenceRecipeKind.DropdownOpen || recipe == UISequenceRecipeKind.DropdownClose || recipe == UISequenceRecipeKind.DrawerShow || recipe == UISequenceRecipeKind.DrawerHide;
            toastSequenceTarget.SetActive(showToast);
            modalSequenceGroup.SetActive(showModal);
            tooltipSequenceTarget.SetActive(showTooltip);
            dropdownSequencePanel.SetActive(showDropdown);
            tabSequenceGroup.SetActive(recipe == UISequenceRecipeKind.TabSwitch || recipe == UISequenceRecipeKind.PagePush || recipe == UISequenceRecipeKind.PageCrossFade);
        }

        private void ConfigureTextValuePreview(TextValueRecipeKind recipe)
        {
            bool showTypewriter = recipe == TextValueRecipeKind.TypewriterReveal || recipe == TextValueRecipeKind.TypewriterHide;
            bool showNumber = recipe == TextValueRecipeKind.NumberCountUp || recipe == TextValueRecipeKind.NumberCountDown;
            bool showCharacter = recipe == TextValueRecipeKind.TextCharacterStaggerIn ||
                                 recipe == TextValueRecipeKind.TextWave ||
                                 recipe == TextValueRecipeKind.TextCharacterStaggerOut ||
                                 recipe == TextValueRecipeKind.TextCharacterBounce ||
                                 recipe == TextValueRecipeKind.TextColorSweep ||
                                 recipe == TextValueRecipeKind.TextGlitch ||
                                 recipe == TextValueRecipeKind.TextEmphasis ||
                                 recipe == TextValueRecipeKind.TextScrambleReveal;
            typewriterText.gameObject.SetActive(showTypewriter);
            numberText.gameObject.SetActive(showNumber);
            characterText.gameObject.SetActive(showCharacter);
            scoreText.gameObject.SetActive(recipe == TextValueRecipeKind.ScoreIncrease);
        }

        private void ConfigureCollectionLayout(CollectionRecipeKind recipe)
        {
            bool isList = recipe == CollectionRecipeKind.ListStaggerIn || recipe == CollectionRecipeKind.ListStaggerOut;
            int activeCount = isList ? _listTargets.Length : recipe == CollectionRecipeKind.LoadingDots ? _loadingDotTargets.Length : _gridTargets.Length;

            for (int i = 0; i < collectionTargets.Length; i++)
            {
                bool active = i < activeCount;
                collectionTargets[i].SetActive(active);
                if (!active) continue;

                var rect = (RectTransform)collectionTargets[i].transform;
                TMP_Text label = collectionTargets[i].GetComponentInChildren<TMP_Text>();
                if (isList)
                {
                    rect.sizeDelta = new Vector2(66f, 66f);
                    rect.anchoredPosition = new Vector2((i - 2.5f) * 82f, 72f);
                    label.gameObject.SetActive(true);
                }
                else if (recipe == CollectionRecipeKind.LoadingDots)
                {
                    rect.sizeDelta = new Vector2(36f, 36f);
                    rect.anchoredPosition = new Vector2((i - 1) * 72f, 72f);
                    label.gameObject.SetActive(false);
                }
                else
                {
                    int row = i / 3;
                    int column = i % 3;
                    rect.sizeDelta = new Vector2(62f, 62f);
                    rect.anchoredPosition = new Vector2((column - 1) * 82f, (1 - row) * 82f + 72f);
                    label.gameObject.SetActive(true);
                }
            }
        }

        private string GetCollectionCodeExample(CollectionRecipeKind recipe)
        {
            switch (recipe)
            {
                case CollectionRecipeKind.ListStaggerIn:
                    return $"items.TweenStagger(owner).Preset<PopInFadePreset>(0.32f).Order(StaggerOrder.{_selectedCollectionOrder}).DelayBetween(0.06f).Play();";
                case CollectionRecipeKind.ListStaggerOut:
                    return $"items.TweenStagger(owner).Preset<PopOutFadePreset>(0.26f).Order(StaggerOrder.{_selectedCollectionOrder}).DelayBetween(0.04f).Play();";
                case CollectionRecipeKind.GridWave:
                    return "items.GridWave(owner, columns: 3);";
                case CollectionRecipeKind.GridRipple:
                    return "items.GridRipple(owner, columns: 3);";
                case CollectionRecipeKind.LoadingDots:
                    return "dots.LoadingDots(owner);";
                case CollectionRecipeKind.GridDiagonalWave:
                    return "items.GridDiagonalWave(owner, columns: 3);";
                case CollectionRecipeKind.GridSpiral:
                    return "items.GridSpiral(owner, columns: 3);";
                case CollectionRecipeKind.GridCheckerboard:
                    return "items.GridCheckerboard(owner, columns: 3);";
                case CollectionRecipeKind.CollectionBurstIn:
                    return "items.CollectionBurstIn(owner, origin);";
                case CollectionRecipeKind.CollectionBurstOut:
                    return "items.CollectionBurstOut(owner, origin);";
                case CollectionRecipeKind.CollectionGatherTo:
                    return "items.CollectionGatherTo(owner, destination);";
                default:
                    return string.Empty;
            }
        }

        private static string GetDestinationCodeExample(DestinationRecipeKind recipe)
        {
            switch (recipe)
            {
                case DestinationRecipeKind.Arc:
                    return "target.Tween().ArcLocalTo(destination, 145f, 1.2f).Play();";
                case DestinationRecipeKind.Bezier:
                    return "target.Tween().BezierLocalTo(destination, controlA, controlB, 1.35f).Play();";
                case DestinationRecipeKind.Hop:
                    return "target.Tween().HopLocalTo(destination, 145f, 1.35f).Play();";
                case DestinationRecipeKind.Spring:
                    return "target.Tween().SpringLocalTo(destination, 1f, 38f).Play();";
                case DestinationRecipeKind.MagneticSnap:
                    return "target.Tween().MagneticSnapLocalTo(destination, 1.1f, 32f, 26f).Play();";
                case DestinationRecipeKind.PathThrough:
                    return "target.Tween().PathLocalThrough(waypoints).Play();";
                case DestinationRecipeKind.Spiral:
                    return "target.Tween().SpiralLocalTo(destination, 82f).Play();";
                case DestinationRecipeKind.MultiHop:
                    return "target.Tween().MultiHopLocalTo(destination, 145f, hopCount: 3).Play();";
                default:
                    return string.Empty;
            }
        }

        private static string GetFeedbackCodeExample(FeedbackRecipeKind recipe)
        {
            switch (recipe)
            {
                case FeedbackRecipeKind.ErrorReject:
                    return "target.ErrorReject();";
                case FeedbackRecipeKind.DamageHit:
                    return "target.DamageHit();";
                case FeedbackRecipeKind.SuccessConfirm:
                    return "target.SuccessConfirm();";
                case FeedbackRecipeKind.RewardReveal:
                    return "target.RewardReveal();";
                case FeedbackRecipeKind.PickupCollect:
                    return "target.PickupCollectLocalTo(destination);";
                case FeedbackRecipeKind.HealReceive:
                    return "target.HealReceive();";
                case FeedbackRecipeKind.ShieldBlock:
                    return "target.ShieldBlock(impactDirection);";
                case FeedbackRecipeKind.CriticalHit:
                    return "target.CriticalHit(impactDirection);";
                case FeedbackRecipeKind.CooldownReady:
                    return "icon.CooldownReady();";
                case FeedbackRecipeKind.LevelUp:
                    return "badge.LevelUp();";
                case FeedbackRecipeKind.LowHealthWarning:
                    return "health.LowHealthWarning();";
                default:
                    return string.Empty;
            }
        }

        private static string GetUISequenceCodeExample(UISequenceRecipeKind recipe)
        {
            switch (recipe)
            {
                case UISequenceRecipeKind.ToastShow:
                    return "toast.ToastShow();";
                case UISequenceRecipeKind.ToastHide:
                    return "toast.ToastHide();";
                case UISequenceRecipeKind.ModalOpen:
                    return "panel.ModalOpen(backdrop, controls);";
                case UISequenceRecipeKind.ModalClose:
                    return "panel.ModalClose(backdrop, controls);";
                case UISequenceRecipeKind.TooltipShow:
                    return "tooltip.TooltipShow();";
                case UISequenceRecipeKind.TooltipHide:
                    return "tooltip.TooltipHide();";
                case UISequenceRecipeKind.DropdownOpen:
                    return "dropdown.DropdownOpen(entries);";
                case UISequenceRecipeKind.DropdownClose:
                    return "dropdown.DropdownClose(entries);";
                case UISequenceRecipeKind.TabSwitch:
                    return "outgoing.TabSwitchTo(incoming);";
                case UISequenceRecipeKind.DrawerShow:
                    return "drawer.DrawerShow(UISequenceDirection.Left, backdrop);";
                case UISequenceRecipeKind.DrawerHide:
                    return "drawer.DrawerHide(UISequenceDirection.Left, backdrop);";
                case UISequenceRecipeKind.BottomSheetShow:
                    return "sheet.BottomSheetShow(backdrop);";
                case UISequenceRecipeKind.BottomSheetHide:
                    return "sheet.BottomSheetHide(backdrop);";
                case UISequenceRecipeKind.PagePush:
                    return "outgoing.PagePushTo(incoming);";
                case UISequenceRecipeKind.PageCrossFade:
                    return "outgoing.PageCrossFadeTo(incoming);";
                default:
                    return string.Empty;
            }
        }

        private static string GetTextValueCodeExample(TextValueRecipeKind recipe)
        {
            switch (recipe)
            {
                case TextValueRecipeKind.TypewriterReveal:
                    return "label.TypewriterReveal();";
                case TextValueRecipeKind.TypewriterHide:
                    return "label.TypewriterHide();";
                case TextValueRecipeKind.NumberCountUp:
                    return "score.NumberCountTo(0, 1250, format: \"N0\");";
                case TextValueRecipeKind.NumberCountDown:
                    return "timer.NumberCountTo(1250, 0, format: \"N0\");";
                case TextValueRecipeKind.TextCharacterStaggerIn:
                    return "label.TextCharacterStaggerIn(UISequenceDirection.Up);";
                case TextValueRecipeKind.TextWave:
                    return "label.TextWave(amplitude: 12f);";
                case TextValueRecipeKind.ScoreIncrease:
                    return "score.ScoreIncrease(1200, 1475, format: \"N0\");";
                case TextValueRecipeKind.TextCharacterStaggerOut:
                    return "label.TextCharacterStaggerOut(UISequenceDirection.Up);";
                case TextValueRecipeKind.TextCharacterBounce:
                    return "label.TextCharacterBounce();";
                case TextValueRecipeKind.TextColorSweep:
                    return "label.TextColorSweep(highlightColor);";
                case TextValueRecipeKind.TextGlitch:
                    return "label.TextGlitch(seed: 1729);";
                case TextValueRecipeKind.TextEmphasis:
                    return "label.TextEmphasis(startCharacter: 0, characterCount: 9);";
                case TextValueRecipeKind.TextScrambleReveal:
                    return "label.TextScrambleReveal(seed: 1729);";
                default:
                    return string.Empty;
            }
        }

        private static bool UsesCurvedPath(DestinationRecipeKind recipe)
        {
            return recipe == DestinationRecipeKind.Arc ||
                   recipe == DestinationRecipeKind.Bezier ||
                   recipe == DestinationRecipeKind.Hop ||
                   recipe == DestinationRecipeKind.PathThrough ||
                   recipe == DestinationRecipeKind.Spiral ||
                   recipe == DestinationRecipeKind.MultiHop;
        }

        private void UpdateDestinationPath(DestinationRecipeKind recipe)
        {
            Vector3 start = destinationStartMarker.anchoredPosition3D;
            Vector3 destination = destinationEndMarker.anchoredPosition3D;
            GetBezierControls(start, destination, out Vector3 controlA, out Vector3 controlB);
            Vector3[] waypoints = GetPathWaypoints(start, destination);
            Transform pathRoot = destinationCurvedPath.transform;

            for (int i = 0; i < pathRoot.childCount; i++)
            {
                float progress = (i + 1f) / (pathRoot.childCount + 1f);
                Vector3 point;
                if (recipe == DestinationRecipeKind.Bezier) point = EvaluateBezier(start, controlA, controlB, destination, progress);
                else if (recipe == DestinationRecipeKind.PathThrough) point = EvaluatePath(start, waypoints, progress);
                else if (recipe == DestinationRecipeKind.Spiral) point = EvaluateSpiral(start, destination, 82f, 1.75f, progress);
                else if (recipe == DestinationRecipeKind.MultiHop) point = EvaluateMultiHop(start, destination, DestinationArcHeight, 3, 1.15f, progress);
                else point = EvaluateArc(start, destination, DestinationArcHeight, progress);
                ((RectTransform)pathRoot.GetChild(i)).anchoredPosition3D = point;
            }
        }

        private void UpdateDestinationPath(bool usesBezier)
        {
            Vector3 start = destinationStartMarker.anchoredPosition3D;
            Vector3 destination = destinationEndMarker.anchoredPosition3D;
            GetBezierControls(start, destination, out Vector3 controlA, out Vector3 controlB);
            Transform pathRoot = destinationCurvedPath.transform;

            for (int i = 0; i < pathRoot.childCount; i++)
            {
                float progress = (i + 1f) / (pathRoot.childCount + 1f);
                Vector3 point = usesBezier
                    ? EvaluateBezier(start, controlA, controlB, destination, progress)
                    : EvaluateArc(start, destination, DestinationArcHeight, progress);
                ((RectTransform)pathRoot.GetChild(i)).anchoredPosition3D = point;
            }
        }

        private static Vector3[] GetPathWaypoints(Vector3 start, Vector3 destination)
        {
            return new[]
            {
                Vector3.Lerp(start, destination, 0.32f) + Vector3.up * DestinationArcHeight + Vector3.left * 68f,
                Vector3.Lerp(start, destination, 0.68f) + Vector3.up * DestinationArcHeight * 0.28f + Vector3.right * 68f,
                destination
            };
        }

        private static void GetBezierControls(Vector3 start, Vector3 destination, out Vector3 controlA, out Vector3 controlB)
        {
            controlA = Vector3.Lerp(start, destination, 0.3f) + Vector3.up * DestinationBezierControlAHeight;
            controlB = Vector3.Lerp(start, destination, 0.72f) + Vector3.up * DestinationBezierControlBHeight;
        }

        private static Vector3 EvaluateArc(Vector3 start, Vector3 destination, float height, float progress)
        {
            return Vector3.LerpUnclamped(start, destination, progress) + Vector3.up * (4f * height * progress * (1f - progress));
        }

        private static Vector3 EvaluateBezier(Vector3 start, Vector3 controlA, Vector3 controlB, Vector3 destination, float progress)
        {
            float inverse = 1f - progress;
            return inverse * inverse * inverse * start + 3f * inverse * inverse * progress * controlA + 3f * inverse * progress * progress * controlB + progress * progress * progress * destination;
        }

        private static Vector3 EvaluatePath(Vector3 start, Vector3[] waypoints, float progress)
        {
            float scaled = Mathf.Clamp01(progress) * waypoints.Length;
            int segment = Mathf.Min(Mathf.FloorToInt(scaled), waypoints.Length - 1);
            float localProgress = progress >= 1f ? 1f : scaled - segment;
            Vector3 pointA = segment == 0 ? start : waypoints[segment - 1];
            Vector3 pointB = waypoints[segment];
            Vector3 previous = segment <= 1 ? start : waypoints[segment - 2];
            Vector3 next = segment + 1 < waypoints.Length ? waypoints[segment + 1] : pointB;
            float square = localProgress * localProgress;
            float cube = square * localProgress;
            return 0.5f * ((2f * pointA) + (-previous + pointB) * localProgress + (2f * previous - 5f * pointA + 4f * pointB - next) * square + (-previous + 3f * pointA - 3f * pointB + next) * cube);
        }

        private static Vector3 EvaluateSpiral(Vector3 start, Vector3 destination, float radius, float revolutions, float progress)
        {
            float travel = DOVirtual.EasedValue(0f, 1f, progress, Ease.InOutCubic);
            float angle = progress * revolutions * Mathf.PI * 2f;
            float envelope = Mathf.Sin(progress * Mathf.PI);
            Vector3 radial = Vector3.right * Mathf.Cos(angle) + Vector3.up * Mathf.Sin(angle);
            return Vector3.LerpUnclamped(start, destination, travel) + radial * radius * envelope;
        }

        private static Vector3 EvaluateMultiHop(Vector3 start, Vector3 destination, float height, int hopCount, float decay, float progress)
        {
            float travel = DOVirtual.EasedValue(0f, 1f, progress, Ease.InOutCubic);
            float bounce = Mathf.Abs(Mathf.Sin(progress * hopCount * Mathf.PI));
            float envelope = Mathf.Pow(1f - Mathf.Clamp01(progress), decay);
            return Vector3.LerpUnclamped(start, destination, travel) + Vector3.up * height * bounce * envelope;
        }

        private static string SplitPascalCase(string value)
        {
            for (int i = 1; i < value.Length; i++)
            {
                if (char.IsUpper(value[i])) return value.Insert(i, " ");
            }

            return value;
        }

        private GameObject[] CopyTargets(int count)
        {
            var targets = new GameObject[count];
            Array.Copy(collectionTargets, targets, count);
            return targets;
        }

        private static UIStateSnapshot[] CaptureStates(GameObject[] targets)
        {
            var states = new UIStateSnapshot[targets.Length];
            for (int i = 0; i < targets.Length; i++) states[i] = UIStateSnapshot.Capture(targets[i]);
            return states;
        }

        private void ResetCollectionTargets()
        {
            for (int i = 0; i < collectionTargets.Length; i++) _collectionStates[i].Apply(collectionTargets[i]);
        }

        private void ResetUISequenceTargets()
        {
            _toastSequenceState.Apply(toastSequenceTarget);
            _modalBackdropState.Apply(modalSequenceBackdrop);
            _modalPanelState.Apply(modalSequencePanel);
            ApplyStates(modalSequenceControls, _modalControlStates);
            _tooltipSequenceState.Apply(tooltipSequenceTarget);
            _dropdownPanelState.Apply(dropdownSequencePanel);
            ApplyStates(dropdownSequenceEntries, _dropdownEntryStates);
            _tabOutgoingState.Apply(tabSequenceOutgoing);
            _tabIncomingState.Apply(tabSequenceIncoming);
        }

        private void ResetTextValueTargets()
        {
            _typewriterTextState.Apply(typewriterText);
            _numberTextState.Apply(numberText);
            _characterTextState.Apply(characterText);
            _scoreTextState.Apply(scoreText);
        }

        private static void ApplyStates(GameObject[] targets, UIStateSnapshot[] states)
        {
            for (int i = 0; i < targets.Length; i++) states[i].Apply(targets[i]);
        }

        private static void KillTargets(GameObject[] targets)
        {
            for (int i = 0; i < targets.Length; i++) KillTargetTweens(targets[i]);
        }

        private static void KillTargetTweens(GameObject target)
        {
            DOTween.Kill(target, false);
            DOTween.Kill(target.transform, false);
            var graphic = target.GetComponent<Graphic>();
            if (graphic != null) DOTween.Kill(graphic, false);
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null) DOTween.Kill(canvasGroup, false);
        }

        private static AnimationPresetDisplay.ResetVerificationResult VerifyTargetState(GameObject target, UIStateSnapshot expected, string label, float positionTolerance, float scaleTolerance, float rotationAngleTolerance, float colorTolerance)
        {
            float positionError = Vector2.Distance(((RectTransform)target.transform).anchoredPosition, expected.AnchoredPosition);
            float scaleError = Vector3.Distance(target.transform.localScale, expected.Scale);
            float rotationError = Quaternion.Angle(target.transform.localRotation, expected.Rotation);
            bool transformMatches = positionError <= positionTolerance && scaleError <= scaleTolerance && rotationError <= rotationAngleTolerance;
            var graphic = target.GetComponent<Graphic>();
            float colorError = graphic != null ? MaxColorDifference(graphic.color, expected.Color) : 0f;
            bool colorMatches = colorError <= colorTolerance;
            bool noActiveTweens = !DOTween.IsTweening(target) && !DOTween.IsTweening(target.transform) && (graphic == null || !DOTween.IsTweening(graphic));
            return new AnimationPresetDisplay.ResetVerificationResult
            {
                TransformMatches = transformMatches,
                AlphaMatches = colorMatches,
                NoActiveTweens = noActiveTweens,
                PositionError = positionError,
                ScaleError = scaleError,
                RotationAngleError = rotationError,
                Details = transformMatches && colorMatches && noActiveTweens ? string.Empty : $"{label} reset mismatch."
            };
        }

        private static float MaxColorDifference(Color current, Color expected)
        {
            return Mathf.Max(Mathf.Abs(current.r - expected.r), Mathf.Abs(current.g - expected.g), Mathf.Abs(current.b - expected.b), Mathf.Abs(current.a - expected.a));
        }

        private static string JoinDetails(string first, string second)
        {
            if (string.IsNullOrEmpty(first)) return second;
            if (string.IsNullOrEmpty(second)) return first;
            return first + " | " + second;
        }

        private readonly struct RecipeDefinition
        {
            public readonly UIRecipeKind Kind;
            public readonly string Description;

            public RecipeDefinition(UIRecipeKind kind, string description)
            {
                Kind = kind;
                Description = description;
            }
        }

        private readonly struct CollectionRecipeDefinition
        {
            public readonly CollectionRecipeKind Kind;
            public readonly string Description;

            public CollectionRecipeDefinition(CollectionRecipeKind kind, string description)
            {
                Kind = kind;
                Description = description;
            }
        }

        private readonly struct DestinationRecipeDefinition
        {
            public readonly DestinationRecipeKind Kind;
            public readonly string Description;

            public DestinationRecipeDefinition(DestinationRecipeKind kind, string description)
            {
                Kind = kind;
                Description = description;
            }
        }

        private readonly struct FeedbackRecipeDefinition
        {
            public readonly FeedbackRecipeKind Kind;
            public readonly string Description;

            public FeedbackRecipeDefinition(FeedbackRecipeKind kind, string description)
            {
                Kind = kind;
                Description = description;
            }
        }

        private readonly struct UISequenceRecipeDefinition
        {
            public readonly UISequenceRecipeKind Kind;
            public readonly string Description;

            public UISequenceRecipeDefinition(UISequenceRecipeKind kind, string description)
            {
                Kind = kind;
                Description = description;
            }
        }

        private readonly struct TextValueRecipeDefinition
        {
            public readonly TextValueRecipeKind Kind;
            public readonly string Description;

            public TextValueRecipeDefinition(TextValueRecipeKind kind, string description)
            {
                Kind = kind;
                Description = description;
            }
        }

        private enum ShowcaseMode
        {
            Recipes,
            Presets,
            Collections,
            Destinations,
            Feedback,
            UISequences,
            TextValues
        }

        private enum CollectionRecipeKind
        {
            ListStaggerIn,
            ListStaggerOut,
            GridWave,
            GridRipple,
            LoadingDots,
            GridDiagonalWave,
            GridSpiral,
            GridCheckerboard,
            CollectionBurstIn,
            CollectionBurstOut,
            CollectionGatherTo
        }

        private enum DestinationRecipeKind
        {
            Arc,
            Bezier,
            Hop,
            Spring,
            MagneticSnap,
            PathThrough,
            Spiral,
            MultiHop
        }

        private enum FeedbackRecipeKind
        {
            ErrorReject,
            DamageHit,
            SuccessConfirm,
            RewardReveal,
            PickupCollect,
            HealReceive,
            ShieldBlock,
            CriticalHit,
            CooldownReady,
            LevelUp,
            LowHealthWarning
        }

        private enum UISequenceRecipeKind
        {
            ToastShow,
            ToastHide,
            ModalOpen,
            ModalClose,
            TooltipShow,
            TooltipHide,
            DropdownOpen,
            DropdownClose,
            TabSwitch,
            DrawerShow,
            DrawerHide,
            BottomSheetShow,
            BottomSheetHide,
            PagePush,
            PageCrossFade
        }

        private enum TextValueRecipeKind
        {
            TypewriterReveal,
            TypewriterHide,
            NumberCountUp,
            NumberCountDown,
            TextCharacterStaggerIn,
            TextWave,
            ScoreIncrease,
            TextCharacterStaggerOut,
            TextCharacterBounce,
            TextColorSweep,
            TextGlitch,
            TextEmphasis,
            TextScrambleReveal
        }

        private readonly struct TMPTextPreviewSnapshot
        {
            public readonly string Text;
            public readonly int MaxVisibleCharacters;
            public readonly Vector3 Scale;
            public readonly Quaternion Rotation;
            public readonly Vector2 AnchoredPosition;
            public readonly Color Color;

            private TMPTextPreviewSnapshot(string text, int maxVisibleCharacters, Vector3 scale, Quaternion rotation, Vector2 anchoredPosition, Color color)
            {
                Text = text;
                MaxVisibleCharacters = maxVisibleCharacters;
                Scale = scale;
                Rotation = rotation;
                AnchoredPosition = anchoredPosition;
                Color = color;
            }

            public static TMPTextPreviewSnapshot Capture(TMP_Text text)
            {
                return new TMPTextPreviewSnapshot(text.text, text.maxVisibleCharacters, text.transform.localScale, text.transform.localRotation, text.rectTransform.anchoredPosition, text.color);
            }

            public void Apply(TMP_Text text)
            {
                text.text = Text;
                text.maxVisibleCharacters = MaxVisibleCharacters;
                text.color = Color;
                text.transform.localScale = Scale;
                text.transform.localRotation = Rotation;
                text.rectTransform.anchoredPosition = AnchoredPosition;
                text.ForceMeshUpdate();
            }
        }

        private readonly struct UIStateSnapshot
        {
            public readonly Vector3 Scale;
            public readonly Quaternion Rotation;
            public readonly Vector2 AnchoredPosition;
            public readonly Color Color;

            private UIStateSnapshot(Vector3 scale, Quaternion rotation, Vector2 anchoredPosition, Color color)
            {
                Scale = scale;
                Rotation = rotation;
                AnchoredPosition = anchoredPosition;
                Color = color;
            }

            public static UIStateSnapshot Capture(GameObject target)
            {
                var graphic = target.GetComponent<Graphic>();
                return new UIStateSnapshot(target.transform.localScale, target.transform.localRotation, ((RectTransform)target.transform).anchoredPosition, graphic.color);
            }

            public void Apply(GameObject target)
            {
                target.transform.localScale = Scale;
                target.transform.localRotation = Rotation;
                ((RectTransform)target.transform).anchoredPosition = AnchoredPosition;
                target.GetComponent<Graphic>().color = Color;
                var canvasGroup = target.GetComponent<CanvasGroup>();
                if (canvasGroup != null) canvasGroup.alpha = 1f;
            }
        }
    }
}
