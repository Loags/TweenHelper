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
            DestinationMotion,
            FeedbackSequence,
            Preset
        }

        private enum PreviewKind
        {
            Ui,
            World,
            List,
            Grid,
            LoadingDots,
            DestinationWorld,
            DestinationUi
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

        private enum DestinationReviewKind
        {
            ArcTo3D,
            ArcLocalToUi,
            BezierTo3D,
            BezierLocalToUi,
            HopTo3D,
            HopLocalToUi,
            SpringTo3D,
            SpringLocalToUi,
            MagneticSnapTo3D,
            MagneticSnapLocalToUi
        }

        private enum FeedbackReviewKind
        {
            ErrorReject,
            DamageHit,
            SuccessConfirm,
            RewardReveal,
            PickupCollect
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
            public DestinationReviewKind DestinationKind;
            public FeedbackReviewKind FeedbackKind;

            public bool UsesUiTarget => Preview == PreviewKind.Ui;
            public bool UsesCollectionPreview => Preview == PreviewKind.List || Preview == PreviewKind.Grid || Preview == PreviewKind.LoadingDots;
            public bool UsesDestinationPreview => Preview == PreviewKind.DestinationWorld || Preview == PreviewKind.DestinationUi;
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
        private const float DestinationWorldArcHeight = 2.1f;
        private const float DestinationUiArcHeight = 175f;
        private const float DestinationWorldBezierControlAHeight = 2.5f;
        private const float DestinationWorldBezierControlBHeight = 0.8f;
        private const float DestinationUiBezierControlAHeight = 210f;
        private const float DestinationUiBezierControlBHeight = 70f;
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

        [Header("Destination Motion Preview")]
        [SerializeField] private GameObject destinationWorldRoot;
        [SerializeField] private GameObject destinationWorldTarget;
        [SerializeField] private Transform destinationWorldStartMarker;
        [SerializeField] private Transform destinationWorldEndMarker;
        [SerializeField] private GameObject destinationWorldCurvedPath;
        [SerializeField] private GameObject destinationUiRoot;
        [SerializeField] private GameObject destinationUiTarget;
        [SerializeField] private RectTransform destinationUiStartMarker;
        [SerializeField] private RectTransform destinationUiEndMarker;
        [SerializeField] private GameObject destinationUiCurvedPath;

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
        private TargetSnapshot _destinationWorldSnapshot;
        private TargetSnapshot _destinationUiSnapshot;
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
            _destinationWorldSnapshot = TargetSnapshot.Capture(destinationWorldTarget);
            _destinationUiSnapshot = TargetSnapshot.Capture(destinationUiTarget);
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

            AddDestinationMotion(DestinationReviewKind.ArcTo3D, "ArcTo 3D", "Moves through a signed world-space Y arc and lands exactly at the destination.", PreviewKind.DestinationWorld);
            AddDestinationMotion(DestinationReviewKind.ArcLocalToUi, "ArcLocalTo UI", "Moves an anchored UI target through a signed local Y arc.", PreviewKind.DestinationUi);
            AddDestinationMotion(DestinationReviewKind.BezierTo3D, "BezierTo 3D", "Follows a cubic world-space Bezier path using two explicit controls.", PreviewKind.DestinationWorld);
            AddDestinationMotion(DestinationReviewKind.BezierLocalToUi, "BezierLocalTo UI", "Follows a cubic anchored-position Bezier path using two local controls.", PreviewKind.DestinationUi);
            AddDestinationMotion(DestinationReviewKind.HopTo3D, "HopTo 3D", "Anticipates, follows a world-space hop, squashes on landing, and restores scale.", PreviewKind.DestinationWorld);
            AddDestinationMotion(DestinationReviewKind.HopLocalToUi, "HopLocalTo UI", "Hops an anchored UI target, lands with a small squash, and restores scale.", PreviewKind.DestinationUi);
            AddDestinationMotion(DestinationReviewKind.SpringTo3D, "SpringTo 3D", "Passes a world-space destination along the travel direction, then settles exactly.", PreviewKind.DestinationWorld);
            AddDestinationMotion(DestinationReviewKind.SpringLocalToUi, "SpringLocalTo UI", "Passes an anchored destination, then settles without positional drift.", PreviewKind.DestinationUi);
            AddDestinationMotion(DestinationReviewKind.MagneticSnapTo3D, "MagneticSnapTo 3D", "Pulls away before accelerating past and settling on a world-space destination.", PreviewKind.DestinationWorld);
            AddDestinationMotion(DestinationReviewKind.MagneticSnapLocalToUi, "MagneticSnapLocalTo UI", "Pulls an anchored target away before snapping past and settling on its destination.", PreviewKind.DestinationUi);

            AddFeedbackSequence(FeedbackReviewKind.ErrorReject, "ErrorReject 3D", "Rejects an action with a sharp shake, tilt, and red flash before restoring the exact baseline.", PreviewKind.DestinationWorld);
            AddFeedbackSequence(FeedbackReviewKind.ErrorReject, "ErrorReject UI", "Rejects a UI action with an anchored shake, tilt, and red flash before restoring the exact baseline.", PreviewKind.DestinationUi);
            AddFeedbackSequence(FeedbackReviewKind.DamageHit, "DamageHit 3D", "Communicates damage with a hit shake, grounded squash, recoil, and red flash.", PreviewKind.DestinationWorld);
            AddFeedbackSequence(FeedbackReviewKind.DamageHit, "DamageHit UI", "Communicates UI damage with an anchored hit shake, grounded squash, recoil, and red flash.", PreviewKind.DestinationUi);
            AddFeedbackSequence(FeedbackReviewKind.SuccessConfirm, "SuccessConfirm 3D", "Confirms success with a pop, two diminishing bounces, and green flash.", PreviewKind.DestinationWorld);
            AddFeedbackSequence(FeedbackReviewKind.SuccessConfirm, "SuccessConfirm UI", "Confirms UI success with a pop, two diminishing anchored bounces, and green flash.", PreviewKind.DestinationUi);
            AddFeedbackSequence(FeedbackReviewKind.RewardReveal, "RewardReveal 3D", "Reveals a reward with anticipation, a relative spin, overshoot, pulse, and gold flash.", PreviewKind.DestinationWorld);
            AddFeedbackSequence(FeedbackReviewKind.RewardReveal, "RewardReveal UI", "Reveals a UI reward while preserving its existing orientation and final layout state.", PreviewKind.DestinationUi);
            AddFeedbackSequence(FeedbackReviewKind.PickupCollect, "PickupCollectTo 3D", "Punches, arcs, shrinks, and fades into an exact world-space collection destination.", PreviewKind.DestinationWorld);
            AddFeedbackSequence(FeedbackReviewKind.PickupCollect, "PickupCollectLocalTo UI", "Punches, arcs, shrinks, and fades into an exact anchored collection destination.", PreviewKind.DestinationUi);

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

        private void AddDestinationMotion(DestinationReviewKind kind, string name, string description, PreviewKind preview)
        {
            _allItems.Add(new ReviewItem
            {
                Id = "Destination:" + kind,
                Name = name,
                Description = description,
                Kind = ReviewKind.DestinationMotion,
                Preview = preview,
                DestinationKind = kind
            });
        }

        private void AddFeedbackSequence(FeedbackReviewKind kind, string name, string description, PreviewKind preview)
        {
            string variant = preview == PreviewKind.DestinationUi ? "UI" : "World";
            _allItems.Add(new ReviewItem
            {
                Id = $"Feedback:{kind}:{variant}",
                Name = name,
                Description = description,
                Kind = ReviewKind.FeedbackSequence,
                Preview = preview,
                FeedbackKind = kind
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

            if (CurrentItem.UsesDestinationPreview)
            {
                _activeTween = CurrentItem.Kind == ReviewKind.FeedbackSequence
                    ? PlayFeedbackSequence(CurrentItem.FeedbackKind)
                    : PlayDestinationMotion(CurrentItem.DestinationKind);
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
            ConfigureDestinationGuides(CurrentItem);
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
            destinationWorldRoot.SetActive(false);
            destinationUiRoot.SetActive(false);
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

        private TweenHandle PlayDestinationMotion(DestinationReviewKind kind)
        {
            bool usesUi = CurrentItem.Preview == PreviewKind.DestinationUi;
            GameObject target = usesUi ? destinationUiTarget : destinationWorldTarget;
            Vector3 start = usesUi ? destinationUiStartMarker.anchoredPosition3D : destinationWorldStartMarker.position;
            Vector3 destination = usesUi ? destinationUiEndMarker.anchoredPosition3D : destinationWorldEndMarker.position;
            float height = usesUi ? DestinationUiArcHeight : DestinationWorldArcHeight;
            target.transform.localScale = usesUi ? _destinationUiSnapshot.LocalScale : _destinationWorldSnapshot.LocalScale;

            if (usesUi) ((RectTransform)target.transform).anchoredPosition3D = start;
            else target.transform.position = start;

            switch (kind)
            {
                case DestinationReviewKind.ArcTo3D:
                    return target.Tween().ArcTo(destination, height, 1.35f).Play();
                case DestinationReviewKind.ArcLocalToUi:
                    return target.Tween().ArcLocalTo(destination, height, 1.35f).Play();
                case DestinationReviewKind.BezierTo3D:
                {
                    GetBezierControls(false, start, destination, out Vector3 controlA, out Vector3 controlB);
                    return target.Tween().BezierTo(destination, controlA, controlB, 1.5f).Play();
                }
                case DestinationReviewKind.BezierLocalToUi:
                {
                    GetBezierControls(true, start, destination, out Vector3 controlA, out Vector3 controlB);
                    return target.Tween().BezierLocalTo(destination, controlA, controlB, 1.5f).Play();
                }
                case DestinationReviewKind.HopTo3D:
                    return target.Tween().HopTo(destination, height, 1.5f).Play();
                case DestinationReviewKind.HopLocalToUi:
                    return target.Tween().HopLocalTo(destination, height, 1.5f).Play();
                case DestinationReviewKind.SpringTo3D:
                    return target.Tween().SpringTo(destination, 1.1f, 0.65f).Play();
                case DestinationReviewKind.SpringLocalToUi:
                    return target.Tween().SpringLocalTo(destination, 1.1f, 48f).Play();
                case DestinationReviewKind.MagneticSnapTo3D:
                    return target.Tween().MagneticSnapTo(destination, 1.25f, 0.5f, 0.4f).Play();
                case DestinationReviewKind.MagneticSnapLocalToUi:
                    return target.Tween().MagneticSnapLocalTo(destination, 1.25f, 42f, 32f).Play();
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown destination-motion review item.");
            }
        }

        private TweenHandle PlayFeedbackSequence(FeedbackReviewKind kind)
        {
            bool usesUi = CurrentItem.Preview == PreviewKind.DestinationUi;
            GameObject target = usesUi ? destinationUiTarget : destinationWorldTarget;
            Vector3 start = usesUi ? destinationUiStartMarker.anchoredPosition3D : destinationWorldStartMarker.position;
            Vector3 destination = usesUi ? destinationUiEndMarker.anchoredPosition3D : destinationWorldEndMarker.position;
            Vector3 previewPosition = kind == FeedbackReviewKind.PickupCollect ? start : Vector3.Lerp(start, destination, 0.5f);
            target.transform.localScale = usesUi ? _destinationUiSnapshot.LocalScale : _destinationWorldSnapshot.LocalScale;

            if (usesUi) ((RectTransform)target.transform).anchoredPosition3D = previewPosition;
            else target.transform.position = previewPosition;

            switch (kind)
            {
                case FeedbackReviewKind.ErrorReject:
                    return target.ErrorReject(0.72f);
                case FeedbackReviewKind.DamageHit:
                    return target.DamageHit(0.68f);
                case FeedbackReviewKind.SuccessConfirm:
                    return target.SuccessConfirm(0.95f);
                case FeedbackReviewKind.RewardReveal:
                    return target.RewardReveal(1.28f);
                case FeedbackReviewKind.PickupCollect:
                    return usesUi
                        ? target.PickupCollectLocalTo(destination, DestinationUiArcHeight, 1.35f)
                        : target.PickupCollectTo(destination, DestinationWorldArcHeight, 1.35f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown feedback review item.");
            }
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
            KillTargetTweens(destinationWorldTarget);
            KillTargetTweens(destinationUiTarget);
        }

        private void ResetTargets()
        {
            _uiSnapshot.Apply(uiTarget);
            _worldSnapshot.Apply(worldTarget);
            ApplySnapshots(listTargets, _listSnapshots);
            ApplySnapshots(gridTargets, _gridSnapshots);
            ApplySnapshots(loadingDotTargets, _loadingDotSnapshots);
            _destinationWorldSnapshot.Apply(destinationWorldTarget);
            _destinationUiSnapshot.Apply(destinationUiTarget);
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
            destinationWorldRoot.SetActive(preview == PreviewKind.DestinationWorld);
            destinationUiRoot.SetActive(preview == PreviewKind.DestinationUi);
        }

        private void ConfigureDestinationGuides(ReviewItem item)
        {
            bool isDestination = item.Kind == ReviewKind.DestinationMotion;
            bool isPickup = item.Kind == ReviewKind.FeedbackSequence && item.FeedbackKind == FeedbackReviewKind.PickupCollect;
            bool showMarkers = isDestination || isPickup;
            bool showPath = isPickup || (isDestination && UsesCurvedPath(item.DestinationKind));
            destinationWorldStartMarker.gameObject.SetActive(showMarkers && item.Preview == PreviewKind.DestinationWorld);
            destinationWorldEndMarker.gameObject.SetActive(showMarkers && item.Preview == PreviewKind.DestinationWorld);
            destinationUiStartMarker.gameObject.SetActive(showMarkers && item.Preview == PreviewKind.DestinationUi);
            destinationUiEndMarker.gameObject.SetActive(showMarkers && item.Preview == PreviewKind.DestinationUi);
            if (showPath) UpdateDestinationPath(isDestination && IsBezier(item.DestinationKind));
            destinationWorldCurvedPath.SetActive(showPath && item.Preview == PreviewKind.DestinationWorld);
            destinationUiCurvedPath.SetActive(showPath && item.Preview == PreviewKind.DestinationUi);
        }

        private void UpdateDestinationPath(bool usesBezier)
        {
            bool usesUi = CurrentItem.Preview == PreviewKind.DestinationUi;
            Transform pathRoot = usesUi ? destinationUiCurvedPath.transform : destinationWorldCurvedPath.transform;
            Vector3 start = usesUi ? destinationUiStartMarker.anchoredPosition3D : destinationWorldStartMarker.position;
            Vector3 destination = usesUi ? destinationUiEndMarker.anchoredPosition3D : destinationWorldEndMarker.position;
            float height = usesUi ? DestinationUiArcHeight : DestinationWorldArcHeight;
            GetBezierControls(usesUi, start, destination, out Vector3 controlA, out Vector3 controlB);

            for (int i = 0; i < pathRoot.childCount; i++)
            {
                float progress = (i + 1f) / (pathRoot.childCount + 1f);
                Vector3 point = usesBezier ? EvaluateBezier(start, controlA, controlB, destination, progress) : EvaluateArc(start, destination, height, progress);
                if (usesUi) ((RectTransform)pathRoot.GetChild(i)).anchoredPosition3D = point;
                else pathRoot.GetChild(i).position = point;
            }
        }

        private static void GetBezierControls(bool usesUi, Vector3 start, Vector3 destination, out Vector3 controlA, out Vector3 controlB)
        {
            float controlAHeight = usesUi ? DestinationUiBezierControlAHeight : DestinationWorldBezierControlAHeight;
            float controlBHeight = usesUi ? DestinationUiBezierControlBHeight : DestinationWorldBezierControlBHeight;
            controlA = Vector3.Lerp(start, destination, 0.3f) + Vector3.up * controlAHeight;
            controlB = Vector3.Lerp(start, destination, 0.72f) + Vector3.up * controlBHeight;
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

        private static bool IsBezier(DestinationReviewKind kind) => kind == DestinationReviewKind.BezierTo3D || kind == DestinationReviewKind.BezierLocalToUi;

        private static bool UsesCurvedPath(DestinationReviewKind kind)
        {
            return kind == DestinationReviewKind.ArcTo3D ||
                   kind == DestinationReviewKind.ArcLocalToUi ||
                   kind == DestinationReviewKind.BezierTo3D ||
                   kind == DestinationReviewKind.BezierLocalToUi ||
                   kind == DestinationReviewKind.HopTo3D ||
                   kind == DestinationReviewKind.HopLocalToUi;
        }

        private static string GetCategoryLabel(ReviewItem item)
        {
            if (item.Kind == ReviewKind.UiRecipe) return "UI RECIPE";
            if (item.Kind == ReviewKind.CollectionRecipe) return "COLLECTION RECIPE";
            if (item.Kind == ReviewKind.StaggerVariant) return "STAGGER VARIANT";
            if (item.Kind == ReviewKind.DestinationMotion) return item.Preview == PreviewKind.DestinationUi ? "DESTINATION MOTION / UI" : "DESTINATION MOTION / 3D";
            if (item.Kind == ReviewKind.FeedbackSequence) return item.Preview == PreviewKind.DestinationUi ? "GAMEPLAY FEEDBACK / UI" : "GAMEPLAY FEEDBACK / 3D";
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
