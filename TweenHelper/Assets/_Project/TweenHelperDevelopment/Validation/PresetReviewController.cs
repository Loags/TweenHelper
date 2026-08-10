using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper.Demo
{
    public class PresetReviewController : MonoBehaviour
    {
        private enum ReviewStatus
        {
            Unreviewed,
            Failed,
            Passed
        }

        private enum ReviewKind
        {
            UiRecipe,
            CollectionRecipe,
            StaggerVariant,
            Preset
        }

        private enum PreviewKind
        {
            Ui,
            World,
            List,
            Grid,
            LoadingDots
        }

        private enum CollectionReviewKind
        {
            ListStaggerIn,
            ListStaggerOut,
            GridWave,
            GridRipple,
            LoadingDots,
            OrderFirstToLast,
            OrderLastToFirst,
            OrderFromCenter,
            OrderToCenter,
            OrderRandom,
            GridWaveRightToLeft,
            GridWaveTopToBottom,
            GridWaveBottomToTop
        }

        private enum ReviewFilter
        {
            All,
            Unreviewed,
            Failed
        }

        private sealed class ReviewItem
        {
            public string Id;
            public string Name;
            public string Description;
            public ReviewKind Kind;
            public ITweenPreset Preset;
            public PreviewKind Preview;
            public CollectionReviewKind CollectionKind;

            public bool UsesUiTarget => Preview == PreviewKind.Ui;
            public bool UsesCollectionPreview => Preview == PreviewKind.List || Preview == PreviewKind.Grid || Preview == PreviewKind.LoadingDots;
        }

        private readonly struct TargetSnapshot
        {
            public readonly Vector3 LocalPosition;
            public readonly Vector3 LocalScale;
            public readonly Quaternion LocalRotation;
            public readonly Color Color;
            public readonly float CanvasGroupAlpha;

            private TargetSnapshot(Vector3 localPosition, Vector3 localScale, Quaternion localRotation, Color color, float canvasGroupAlpha)
            {
                LocalPosition = localPosition;
                LocalScale = localScale;
                LocalRotation = localRotation;
                Color = color;
                CanvasGroupAlpha = canvasGroupAlpha;
            }

            public static TargetSnapshot Capture(GameObject target)
            {
                var graphic = target.GetComponent<Graphic>();
                var renderer = target.GetComponent<Renderer>();
                var canvasGroup = target.GetComponent<CanvasGroup>();
                Color color = graphic != null ? graphic.color : renderer.material.color;
                return new TargetSnapshot(target.transform.localPosition, target.transform.localScale, target.transform.localRotation, color, canvasGroup != null ? canvasGroup.alpha : 1f);
            }

            public void Apply(GameObject target)
            {
                target.transform.localPosition = LocalPosition;
                target.transform.localScale = LocalScale;
                target.transform.localRotation = LocalRotation;

                var graphic = target.GetComponent<Graphic>();
                if (graphic != null) graphic.color = Color;

                var renderer = target.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.SetPropertyBlock(null);
                    renderer.material.color = Color;
                }

                var canvasGroup = target.GetComponent<CanvasGroup>();
                if (canvasGroup != null) canvasGroup.alpha = CanvasGroupAlpha;
            }
        }

        private const string StatusKeyPrefix = "TweenHelper.PresetReview.Status.";
        private const float AutoReplayDelaySeconds = 0.5f;
        private static readonly Color UnreviewedColor = new Color(0.48f, 0.55f, 0.68f);
        private static readonly Color FailedColor = new Color(1f, 0.29f, 0.34f);
        private static readonly Color PassedColor = new Color(0.2f, 0.85f, 0.53f);

        [Header("Preview Targets")]
        [SerializeField] private GameObject uiTarget;
        [SerializeField] private GameObject worldTarget;

        [Header("Collection Preview")]
        [SerializeField] private GameObject collectionPreviewRoot;
        [SerializeField] private GameObject listPreviewGroup;
        [SerializeField] private GameObject gridPreviewGroup;
        [SerializeField] private GameObject loadingDotsPreviewGroup;
        [SerializeField] private GameObject[] listTargets;
        [SerializeField] private GameObject[] gridTargets;
        [SerializeField] private GameObject[] loadingDotTargets;

        [Header("Information")]
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text categoryText;
        [SerializeField] private TMP_Text positionText;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text totalsText;

        [Header("Controls")]
        [SerializeField] private Button previousButton;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button failedButton;
        [SerializeField] private Button passedButton;

        [Header("Filters")]
        [SerializeField] private Toggle allFilterToggle;
        [SerializeField] private Toggle unreviewedFilterToggle;
        [SerializeField] private Toggle failedFilterToggle;

        private readonly List<ReviewItem> _allItems = new List<ReviewItem>();
        private readonly List<ReviewItem> _items = new List<ReviewItem>();
        private TargetSnapshot _uiSnapshot;
        private TargetSnapshot _worldSnapshot;
        private TargetSnapshot[] _listSnapshots;
        private TargetSnapshot[] _gridSnapshots;
        private TargetSnapshot[] _loadingDotSnapshots;
        private TweenHandle _activeTween;
        private Coroutine _delayedReplay;
        private ReviewFilter _activeFilter;
        private int _currentIndex;

        private ReviewItem CurrentItem => _items[_currentIndex];

        private void Awake()
        {
            _uiSnapshot = TargetSnapshot.Capture(uiTarget);
            _worldSnapshot = TargetSnapshot.Capture(worldTarget);
            _listSnapshots = CaptureTargets(listTargets);
            _gridSnapshots = CaptureTargets(gridTargets);
            _loadingDotSnapshots = CaptureTargets(loadingDotTargets);
            WireControls();
            BuildReviewItems();
            ShowCurrentItem();
        }

        private void OnDisable() => StopPlayback();

#if ENABLE_LEGACY_INPUT_MANAGER
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) ShowPrevious();
            if (Input.GetKeyDown(KeyCode.RightArrow)) ShowNext();
            if (Input.GetKeyDown(KeyCode.Space)) ReplayCurrent();
            if (Input.GetKeyDown(KeyCode.X)) MarkFailed();
            if (Input.GetKeyDown(KeyCode.C)) MarkPassed();
        }
#endif

        private void WireControls()
        {
            previousButton.onClick.AddListener(ShowPrevious);
            replayButton.onClick.AddListener(ReplayCurrent);
            nextButton.onClick.AddListener(ShowNext);
            failedButton.onClick.AddListener(MarkFailed);
            passedButton.onClick.AddListener(MarkPassed);
            allFilterToggle.onValueChanged.AddListener(selected => SetFilter(ReviewFilter.All, selected));
            unreviewedFilterToggle.onValueChanged.AddListener(selected => SetFilter(ReviewFilter.Unreviewed, selected));
            failedFilterToggle.onValueChanged.AddListener(selected => SetFilter(ReviewFilter.Failed, selected));
        }

        private void BuildReviewItems()
        {
            AddRecipe("UIAppear", "Pop and fade a UI element into view.");
            AddRecipe("UIAppearSoft", "A gentler appear animation.");
            AddRecipe("UIDisappear", "Pop and fade a UI element out.");
            AddRecipe("UIDisappearSoft", "A gentler disappear animation.");
            AddRecipe("UIHover", "Scale and tint for hover feedback.");
            AddRecipe("UIHoverSoft", "Subtle hover feedback.");
            AddRecipe("UIPress", "Press and release feedback.");
            AddRecipe("UIPressHard", "Stronger press feedback.");
            AddRecipe("UIAttention", "Draw attention to a UI element.");
            AddRecipe("UIAttentionSoft", "Subtle attention feedback.");
            AddRecipe("UIAttentionHard", "Strong attention feedback.");
            AddRecipe("UIDisabled", "Transition a UI element to its disabled state.");
            AddRecipe("UIEnabled", "Restore a disabled UI element to its enabled state.");

            AddCollectionRecipe(CollectionReviewKind.ListStaggerIn, "Staggers a list into view from first item to last.", PreviewKind.List);
            AddCollectionRecipe(CollectionReviewKind.ListStaggerOut, "Staggers a list out of view from last item to first.", PreviewKind.List);
            AddCollectionRecipe(CollectionReviewKind.GridWave, "Reveals a grid in a left-to-right wave.", PreviewKind.Grid);
            AddCollectionRecipe(CollectionReviewKind.GridRipple, "Pulses outward from the center of a grid.", PreviewKind.Grid);
            AddCollectionRecipe(CollectionReviewKind.LoadingDots, "Loops a soft pulse across three loading dots.", PreviewKind.LoadingDots);
            AddStaggerVariant(CollectionReviewKind.OrderFirstToLast, "Applies delays from the first collection item to the last.", PreviewKind.List);
            AddStaggerVariant(CollectionReviewKind.OrderLastToFirst, "Applies delays from the last collection item to the first.", PreviewKind.List);
            AddStaggerVariant(CollectionReviewKind.OrderFromCenter, "Starts at the center pair and moves toward both edges.", PreviewKind.List);
            AddStaggerVariant(CollectionReviewKind.OrderToCenter, "Starts at both edges and moves toward the center pair.", PreviewKind.List);
            AddStaggerVariant(CollectionReviewKind.OrderRandom, "Uses a deterministic shuffled order with review seed 1729.", PreviewKind.List);
            AddStaggerVariant(CollectionReviewKind.GridWaveRightToLeft, "Reveals grid columns from right to left.", PreviewKind.Grid);
            AddStaggerVariant(CollectionReviewKind.GridWaveTopToBottom, "Reveals grid rows from top to bottom.", PreviewKind.Grid);
            AddStaggerVariant(CollectionReviewKind.GridWaveBottomToTop, "Reveals grid rows from bottom to top.", PreviewKind.Grid);

            TweenPresetRegistry.Refresh();
            foreach (ITweenPreset preset in TweenPresetRegistry.Presets.OrderBy(item => item.PresetName, StringComparer.Ordinal))
            {
                _allItems.Add(new ReviewItem
                {
                    Id = "Preset:" + preset.PresetName,
                    Name = preset.PresetName,
                    Description = preset.Description,
                    Kind = ReviewKind.Preset,
                    Preset = preset,
                    Preview = UIPresetCompatibility.IsSuitable(preset) ? PreviewKind.Ui : PreviewKind.World
                });
            }

            RebuildFilteredItems();
        }

        private void AddRecipe(string name, string description)
        {
            _allItems.Add(new ReviewItem
            {
                Id = "Recipe:" + name,
                Name = name,
                Description = description,
                Kind = ReviewKind.UiRecipe,
                Preview = PreviewKind.Ui
            });
        }

        private void AddCollectionRecipe(CollectionReviewKind kind, string description, PreviewKind preview)
        {
            AddCollectionItem(kind, description, preview, ReviewKind.CollectionRecipe);
        }

        private void AddStaggerVariant(CollectionReviewKind kind, string description, PreviewKind preview)
        {
            AddCollectionItem(kind, description, preview, ReviewKind.StaggerVariant);
        }

        private void AddCollectionItem(CollectionReviewKind kind, string description, PreviewKind preview, ReviewKind reviewKind)
        {
            _allItems.Add(new ReviewItem
            {
                Id = "Collection:" + kind,
                Name = kind.ToString(),
                Description = description,
                Kind = reviewKind,
                Preview = preview,
                CollectionKind = kind
            });
        }

        public void ReplayCurrent()
        {
            if (_items.Count == 0) return;
            StopPlayback();
            ResetTargets();
            if (CurrentItem.UsesCollectionPreview)
            {
                _activeTween = PlayCollection(CurrentItem.CollectionKind);
                return;
            }

            GameObject target = CurrentItem.UsesUiTarget ? uiTarget : worldTarget;
            _activeTween = CurrentItem.Kind == ReviewKind.UiRecipe ? PlayRecipe(CurrentItem.Name) : PlayPreset(CurrentItem.Preset, target);
        }

        public void ShowPrevious()
        {
            if (_currentIndex <= 0) return;
            _currentIndex--;
            ShowCurrentItem();
        }

        public void ShowNext()
        {
            if (_currentIndex >= _items.Count - 1) return;
            _currentIndex++;
            ShowCurrentItem();
        }

        public void MarkFailed()
        {
            if (_items.Count > 0) SetCurrentStatus(ReviewStatus.Failed);
        }

        public void MarkPassed()
        {
            if (_items.Count > 0) SetCurrentStatus(ReviewStatus.Passed);
        }

        private void SetCurrentStatus(ReviewStatus status)
        {
            ReviewItem reviewedItem = CurrentItem;
            PlayerPrefs.SetInt(StatusKeyPrefix + reviewedItem.Id, (int)status);
            PlayerPrefs.Save();

            int nextIndex = MatchesFilter(reviewedItem) ? _currentIndex + 1 : _currentIndex;
            RebuildFilteredItems();
            _currentIndex = Mathf.Clamp(nextIndex, 0, Mathf.Max(0, _items.Count - 1));
            ShowCurrentItem();
            if (_items.Count > 0 && CurrentItem.Id != reviewedItem.Id) ScheduleDelayedReplay();
        }

        private void SetFilter(ReviewFilter filter, bool selected)
        {
            if (!selected) return;

            string selectedId = _items.Count > 0 ? CurrentItem.Id : null;
            _activeFilter = filter;
            RebuildFilteredItems();
            _currentIndex = string.IsNullOrEmpty(selectedId) ? 0 : _items.FindIndex(item => item.Id == selectedId);
            if (_currentIndex < 0) _currentIndex = 0;
            ShowCurrentItem();
        }

        private void RebuildFilteredItems()
        {
            _items.Clear();
            for (int i = 0; i < _allItems.Count; i++)
            {
                if (MatchesFilter(_allItems[i])) _items.Add(_allItems[i]);
            }
        }

        private bool MatchesFilter(ReviewItem item)
        {
            ReviewStatus status = GetStatus(item);
            if (_activeFilter == ReviewFilter.Unreviewed) return status == ReviewStatus.Unreviewed;
            if (_activeFilter == ReviewFilter.Failed) return status == ReviewStatus.Failed;
            return true;
        }

        private void ShowCurrentItem()
        {
            StopPlayback();
            ResetTargets();
            if (_items.Count == 0)
            {
                ShowEmptyFilter();
                return;
            }

            ApplyPreviewVisibility(CurrentItem.Preview);
            itemNameText.text = CurrentItem.Name;
            descriptionText.text = string.IsNullOrWhiteSpace(CurrentItem.Description) ? "No description provided." : CurrentItem.Description;
            categoryText.text = GetCategoryLabel(CurrentItem);
            positionText.text = $"{_currentIndex + 1} / {_items.Count}";
            previousButton.interactable = _currentIndex > 0;
            nextButton.interactable = _currentIndex < _items.Count - 1;
            RefreshStatus();
        }

        private void ShowEmptyFilter()
        {
            uiTarget.SetActive(false);
            worldTarget.SetActive(false);
            collectionPreviewRoot.SetActive(false);
            itemNameText.text = "No animations";
            descriptionText.text = "No animations currently match this review filter.";
            categoryText.text = "FILTER EMPTY";
            positionText.text = "0 / 0";
            statusText.text = "SELECT ANOTHER FILTER";
            statusText.color = UnreviewedColor;
            previousButton.interactable = false;
            replayButton.interactable = false;
            nextButton.interactable = false;
            failedButton.interactable = false;
            passedButton.interactable = false;
            RefreshTotals();
        }

        private void RefreshStatus()
        {
            replayButton.interactable = true;
            failedButton.interactable = true;
            passedButton.interactable = true;
            ReviewStatus status = GetStatus(CurrentItem);
            Color statusColor = GetStatusColor(status);
            statusText.text = status == ReviewStatus.Unreviewed ? "NOT REVIEWED" : status == ReviewStatus.Failed ? "NEEDS WORK" : "IMPLEMENTATION OK";
            statusText.color = statusColor;
            failedButton.image.color = status == ReviewStatus.Failed ? FailedColor : new Color(0.42f, 0.16f, 0.2f);
            passedButton.image.color = status == ReviewStatus.Passed ? PassedColor : new Color(0.12f, 0.35f, 0.25f);

            RefreshTotals();
        }

        private void RefreshTotals()
        {
            int passed = 0;
            int failed = 0;
            for (int i = 0; i < _allItems.Count; i++)
            {
                ReviewStatus itemStatus = GetStatus(_allItems[i]);
                if (itemStatus == ReviewStatus.Passed) passed++;
                if (itemStatus == ReviewStatus.Failed) failed++;
            }

            totalsText.text = $"Reviewed {passed + failed}/{_allItems.Count}   |   Correct {passed}   |   Needs work {failed}";
        }

        private static ReviewStatus GetStatus(ReviewItem item) => (ReviewStatus)PlayerPrefs.GetInt(StatusKeyPrefix + item.Id, (int)ReviewStatus.Unreviewed);

        private static Color GetStatusColor(ReviewStatus status)
        {
            if (status == ReviewStatus.Failed) return FailedColor;
            if (status == ReviewStatus.Passed) return PassedColor;
            return UnreviewedColor;
        }

        private TweenHandle PlayPreset(ITweenPreset preset, GameObject target)
        {
            if (preset == null || !preset.CanApplyTo(target)) return null;
            var builder = target.Tween();
            float? strength = CurrentItem.UsesUiTarget ? UIPresetCompatibility.GetCanvasPreviewStrength(preset) : null;
            if (strength.HasValue) builder.WithOptions(TweenOptions.WithStrength(strength.Value));
            return builder.Preset(preset).Play();
        }

        private TweenHandle PlayRecipe(string recipeName)
        {
            Color hoverColor = new Color(0.5f, 0.88f, 1f);
            Color disabledColor = new Color(0.4f, 0.45f, 0.55f, 0.5f);

            switch (recipeName)
            {
                case "UIAppear": return uiTarget.UIAppear();
                case "UIAppearSoft": return uiTarget.UIAppearSoft();
                case "UIDisappear": return uiTarget.UIDisappear();
                case "UIDisappearSoft": return uiTarget.UIDisappearSoft();
                case "UIHover": return uiTarget.UIHover(hoverColor: hoverColor);
                case "UIHoverSoft": return uiTarget.UIHoverSoft(hoverColor: hoverColor);
                case "UIPress": return uiTarget.UIPress();
                case "UIPressHard": return uiTarget.UIPressHard();
                case "UIAttention": return uiTarget.UIAttention();
                case "UIAttentionSoft": return uiTarget.UIAttentionSoft();
                case "UIAttentionHard": return uiTarget.UIAttentionHard();
                case "UIDisabled": return uiTarget.UIDisabled(disabledColor: disabledColor);
                case "UIEnabled":
                    uiTarget.UIDisabled(0.01f, disabledColor).Complete();
                    return uiTarget.UIEnabled();
                default: return null;
            }
        }

        private TweenHandle PlayCollection(CollectionReviewKind kind)
        {
            switch (kind)
            {
                case CollectionReviewKind.ListStaggerIn:
                    return listTargets.ListStaggerIn(listPreviewGroup);
                case CollectionReviewKind.ListStaggerOut:
                    return listTargets.ListStaggerOut(listPreviewGroup);
                case CollectionReviewKind.GridWave:
                    return gridTargets.GridWave(gridPreviewGroup, 3);
                case CollectionReviewKind.GridRipple:
                    return gridTargets.GridRipple(gridPreviewGroup, 3);
                case CollectionReviewKind.LoadingDots:
                    return loadingDotTargets.LoadingDots(loadingDotsPreviewGroup);
                case CollectionReviewKind.OrderFirstToLast:
                    return PlayOrder(StaggerOrder.FirstToLast);
                case CollectionReviewKind.OrderLastToFirst:
                    return PlayOrder(StaggerOrder.LastToFirst);
                case CollectionReviewKind.OrderFromCenter:
                    return PlayOrder(StaggerOrder.FromCenter);
                case CollectionReviewKind.OrderToCenter:
                    return PlayOrder(StaggerOrder.ToCenter);
                case CollectionReviewKind.OrderRandom:
                    return PlayOrder(StaggerOrder.Random, 1729);
                case CollectionReviewKind.GridWaveRightToLeft:
                    return gridTargets.GridWave(gridPreviewGroup, 3, GridWaveDirection.RightToLeft);
                case CollectionReviewKind.GridWaveTopToBottom:
                    return gridTargets.GridWave(gridPreviewGroup, 3, GridWaveDirection.TopToBottom);
                case CollectionReviewKind.GridWaveBottomToTop:
                    return gridTargets.GridWave(gridPreviewGroup, 3, GridWaveDirection.BottomToTop);
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown collection review item.");
            }
        }

        private TweenHandle PlayOrder(StaggerOrder order, int seed = 0)
        {
            return listTargets.TweenStagger(listPreviewGroup)
                .Preset<PulseScalePreset>(0.36f)
                .Order(order)
                .DelayBetween(0.14f)
                .Seed(seed)
                .Play();
        }

        private void ScheduleDelayedReplay()
        {
            CancelDelayedReplay();
            string scheduledItemId = CurrentItem.Id;
            _delayedReplay = StartCoroutine(ReplayAfterDelay(scheduledItemId));
        }

        private IEnumerator ReplayAfterDelay(string scheduledItemId)
        {
            yield return new WaitForSecondsRealtime(AutoReplayDelaySeconds);
            _delayedReplay = null;
            if (_items.Count == 0 || CurrentItem.Id != scheduledItemId) yield break;
            ReplayCurrent();
        }

        private void CancelDelayedReplay()
        {
            if (_delayedReplay == null) return;
            StopCoroutine(_delayedReplay);
            _delayedReplay = null;
        }

        private void StopPlayback()
        {
            CancelDelayedReplay();
            _activeTween?.Kill();
            _activeTween = null;
            KillTargetTweens(uiTarget);
            KillTargetTweens(worldTarget);
            KillTargetTweens(collectionPreviewRoot);
            KillTargets(listTargets);
            KillTargets(gridTargets);
            KillTargets(loadingDotTargets);
        }

        private void ResetTargets()
        {
            _uiSnapshot.Apply(uiTarget);
            _worldSnapshot.Apply(worldTarget);
            ApplySnapshots(listTargets, _listSnapshots);
            ApplySnapshots(gridTargets, _gridSnapshots);
            ApplySnapshots(loadingDotTargets, _loadingDotSnapshots);
        }

        private void ApplyPreviewVisibility(PreviewKind preview)
        {
            uiTarget.SetActive(preview == PreviewKind.Ui);
            worldTarget.SetActive(preview == PreviewKind.World);
            bool showCollection = preview == PreviewKind.List || preview == PreviewKind.Grid || preview == PreviewKind.LoadingDots;
            collectionPreviewRoot.SetActive(showCollection);
            listPreviewGroup.SetActive(preview == PreviewKind.List);
            gridPreviewGroup.SetActive(preview == PreviewKind.Grid);
            loadingDotsPreviewGroup.SetActive(preview == PreviewKind.LoadingDots);
        }

        private static string GetCategoryLabel(ReviewItem item)
        {
            if (item.Kind == ReviewKind.UiRecipe) return "UI RECIPE";
            if (item.Kind == ReviewKind.CollectionRecipe) return "COLLECTION RECIPE";
            if (item.Kind == ReviewKind.StaggerVariant) return "STAGGER VARIANT";
            return item.UsesUiTarget ? "2D / UI PRESET" : "3D / WORLD PRESET";
        }

        private static TargetSnapshot[] CaptureTargets(GameObject[] targets)
        {
            var snapshots = new TargetSnapshot[targets.Length];
            for (int i = 0; i < targets.Length; i++) snapshots[i] = TargetSnapshot.Capture(targets[i]);
            return snapshots;
        }

        private static void ApplySnapshots(GameObject[] targets, TargetSnapshot[] snapshots)
        {
            for (int i = 0; i < targets.Length; i++) snapshots[i].Apply(targets[i]);
        }

        private static void KillTargets(GameObject[] targets)
        {
            for (int i = 0; i < targets.Length; i++) KillTargetTweens(targets[i]);
        }

        private static void KillTargetTweens(GameObject target)
        {
            if (target == null) return;
            DOTween.Kill(target, false);
            DOTween.Kill(target.transform, false);

            var graphic = target.GetComponent<Graphic>();
            if (graphic != null) DOTween.Kill(graphic, false);

            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null) DOTween.Kill(canvasGroup, false);

            var renderer = target.GetComponent<Renderer>();
            if (renderer != null) DOTween.Kill(renderer, false);
        }
    }
}
