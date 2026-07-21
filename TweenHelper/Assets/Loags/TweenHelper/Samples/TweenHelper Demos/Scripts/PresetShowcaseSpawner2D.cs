using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Controls the prefab-authored two-tab UI showcase and its safe preview lifecycle.
    /// </summary>
    public class PresetShowcaseSpawner2D : MonoBehaviour
    {
        [Header("Tabs")]
        [SerializeField] private Button recipesTabButton;
        [SerializeField] private Button presetsTabButton;
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

        private readonly List<UIPresetListItem> _presetRows = new List<UIPresetListItem>();
        private UIStateSnapshot _imageState;
        private UIStateSnapshot _textState;
        private TweenHandle _activeTween;
        private ITweenPreset _selectedPreset;
        private UIRecipeKind _selectedRecipe = UIRecipeKind.UIAppear;
        private bool _showingRecipes = true;
        private bool _initialized;

        private GameObject PreviewTarget => targetDropdown.value == 1 ? animatedText.gameObject : presetImage.gameObject;

        private void Awake()
        {
            _imageState = UIStateSnapshot.Capture(presetImage.gameObject);
            _textState = UIStateSnapshot.Capture(animatedText.gameObject);
            WireControls();
            BuildContent();
        }

        private void OnEnable()
        {
            ResetTargets();
            ShowRecipes();
            instructionsPanel.SetContent("TweenHelper 2D Showcase", "Choose UI Recipes or the 2D Preset Library. Select an entry, choose Image or Text, then replay or reset the preview.");
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
            _showingRecipes = true;
            recipesPanel.SetActive(true);
            presetsPanel.SetActive(false);
            SelectRecipe(_selectedRecipe, false);
        }

        public void ShowPresets()
        {
            StopPlayback();
            ResetTargets();
            _showingRecipes = false;
            recipesPanel.SetActive(false);
            presetsPanel.SetActive(true);
            RefreshPresetRows();
            if (_selectedPreset != null) UpdatePresetDetails(_selectedPreset);
        }

        public void ReplaySelected()
        {
            if (_showingRecipes) PlayRecipe(_selectedRecipe);
            else PlaySelectedPreset();
        }

        public void ResetPreview()
        {
            StopPlayback();
            ResetTargets();
        }

        public void CopyCodeExample() => GUIUtility.systemCopyBuffer = codeExampleText.text;

        private void ChangeTarget()
        {
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
            if (!_showingRecipes && !selectedPresetIsVisible && firstVisiblePreset != null)
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
        }

        private void ResetTargets()
        {
            _imageState.Apply(presetImage.gameObject);
            _textState.Apply(animatedText.gameObject);
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
