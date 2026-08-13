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
            UISequence,
            TextValueAnimation,
            CameraFeedback,
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
            DestinationUi,
            UISequence,
            TextValue,
            CameraFeedback
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
            GridWaveBottomToTop,
            GridDiagonalWave,
            GridSpiral,
            GridCheckerboard,
            CollectionBurstIn,
            CollectionBurstOut,
            CollectionGatherTo
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
            MagneticSnapLocalToUi,
            PathThrough3D,
            PathLocalThroughUi,
            SpiralTo3D,
            SpiralLocalToUi,
            MultiHopTo3D,
            MultiHopLocalToUi
        }

        private enum FeedbackReviewKind
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

        private enum UISequenceReviewKind
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

        private enum TextValueReviewKind
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

        private enum CameraFeedbackReviewKind
        {
            Impact,
            Recoil,
            LandingImpact,
            FovKick,
            FocusZoom,
            Breathing
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
            public UISequenceReviewKind UISequenceKind;
            public TextValueReviewKind TextValueKind;
            public CameraFeedbackReviewKind CameraFeedbackKind;

            public bool UsesUiTarget => Preview == PreviewKind.Ui;
            public bool UsesCollectionPreview => Preview == PreviewKind.List || Preview == PreviewKind.Grid || Preview == PreviewKind.LoadingDots;
            public bool UsesDestinationPreview => Preview == PreviewKind.DestinationWorld || Preview == PreviewKind.DestinationUi;
            public bool UsesUISequencePreview => Preview == PreviewKind.UISequence;
            public bool UsesTextValuePreview => Preview == PreviewKind.TextValue;
            public bool UsesCameraFeedbackPreview => Preview == PreviewKind.CameraFeedback;
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

        private readonly struct TMPTextSnapshot
        {
            public readonly string Text;
            public readonly int MaxVisibleCharacters;
            public readonly Vector3 LocalPosition;
            public readonly Vector3 LocalScale;
            public readonly Quaternion LocalRotation;
            public readonly Color Color;

            private TMPTextSnapshot(string text, int maxVisibleCharacters, Vector3 localPosition, Vector3 localScale, Quaternion localRotation, Color color)
            {
                Text = text;
                MaxVisibleCharacters = maxVisibleCharacters;
                LocalPosition = localPosition;
                LocalScale = localScale;
                LocalRotation = localRotation;
                Color = color;
            }

            public static TMPTextSnapshot Capture(TMP_Text text)
            {
                Transform transform = text.transform;
                return new TMPTextSnapshot(text.text, text.maxVisibleCharacters, transform.localPosition, transform.localScale, transform.localRotation, text.color);
            }

            public void Apply(TMP_Text text)
            {
                Transform transform = text.transform;
                text.text = Text;
                text.maxVisibleCharacters = MaxVisibleCharacters;
                text.color = Color;
                transform.localPosition = LocalPosition;
                transform.localScale = LocalScale;
                transform.localRotation = LocalRotation;
                text.ForceMeshUpdate();
            }
        }

        private readonly struct CameraSnapshot
        {
            public readonly Vector3 LocalPosition;
            public readonly Quaternion LocalRotation;
            public readonly float FieldOfView;

            private CameraSnapshot(Vector3 localPosition, Quaternion localRotation, float fieldOfView)
            {
                LocalPosition = localPosition;
                LocalRotation = localRotation;
                FieldOfView = fieldOfView;
            }

            public static CameraSnapshot Capture(Camera camera)
                => new CameraSnapshot(camera.transform.localPosition, camera.transform.localRotation, camera.fieldOfView);

            public void Apply(Camera camera)
            {
                camera.transform.localPosition = LocalPosition;
                camera.transform.localRotation = LocalRotation;
                camera.fieldOfView = FieldOfView;
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

        [Header("UI Sequence Preview")]
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

        [Header("Text & Value Preview")]
        [SerializeField] private GameObject textValuePreviewRoot;
        [SerializeField] private TMP_Text typewriterText;
        [SerializeField] private TMP_Text numberText;
        [SerializeField] private TMP_Text characterText;
        [SerializeField] private TMP_Text scoreText;

        [Header("Camera Feedback Preview")]
        [SerializeField] private Camera feedbackCamera;
        [SerializeField] private Transform cameraFocusTarget;

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
        private TargetSnapshot _toastSequenceSnapshot;
        private TargetSnapshot _modalSequenceBackdropSnapshot;
        private TargetSnapshot _modalSequencePanelSnapshot;
        private TargetSnapshot[] _modalSequenceControlSnapshots;
        private TargetSnapshot _tooltipSequenceSnapshot;
        private TargetSnapshot _dropdownSequencePanelSnapshot;
        private TargetSnapshot[] _dropdownSequenceEntrySnapshots;
        private TargetSnapshot _tabSequenceOutgoingSnapshot;
        private TargetSnapshot _tabSequenceIncomingSnapshot;
        private TMPTextSnapshot _typewriterTextSnapshot;
        private TMPTextSnapshot _numberTextSnapshot;
        private TMPTextSnapshot _characterTextSnapshot;
        private TMPTextSnapshot _scoreTextSnapshot;
        private CameraSnapshot _feedbackCameraSnapshot;
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
            _toastSequenceSnapshot = TargetSnapshot.Capture(toastSequenceTarget);
            _modalSequenceBackdropSnapshot = TargetSnapshot.Capture(modalSequenceBackdrop);
            _modalSequencePanelSnapshot = TargetSnapshot.Capture(modalSequencePanel);
            _modalSequenceControlSnapshots = CaptureTargets(modalSequenceControls);
            _tooltipSequenceSnapshot = TargetSnapshot.Capture(tooltipSequenceTarget);
            _dropdownSequencePanelSnapshot = TargetSnapshot.Capture(dropdownSequencePanel);
            _dropdownSequenceEntrySnapshots = CaptureTargets(dropdownSequenceEntries);
            _tabSequenceOutgoingSnapshot = TargetSnapshot.Capture(tabSequenceOutgoing);
            _tabSequenceIncomingSnapshot = TargetSnapshot.Capture(tabSequenceIncoming);
            _typewriterTextSnapshot = TMPTextSnapshot.Capture(typewriterText);
            _numberTextSnapshot = TMPTextSnapshot.Capture(numberText);
            _characterTextSnapshot = TMPTextSnapshot.Capture(characterText);
            _scoreTextSnapshot = TMPTextSnapshot.Capture(scoreText);
            _feedbackCameraSnapshot = CameraSnapshot.Capture(feedbackCamera);
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
            AddCollectionRecipe(CollectionReviewKind.GridDiagonalWave, "Reveals grid diagonals from the top-left toward the bottom-right.", PreviewKind.Grid);
            AddCollectionRecipe(CollectionReviewKind.GridSpiral, "Reveals grid items in a clockwise outside-in spiral.", PreviewKind.Grid);
            AddCollectionRecipe(CollectionReviewKind.GridCheckerboard, "Pulses alternating checkerboard cells in two coordinated phases.", PreviewKind.Grid);
            AddCollectionRecipe(CollectionReviewKind.CollectionBurstIn, "Launches every grid item from the collection center into its authored position.", PreviewKind.Grid);
            AddCollectionRecipe(CollectionReviewKind.CollectionBurstOut, "Scatters grid items away from the collection center while shrinking and fading them.", PreviewKind.Grid);
            AddCollectionRecipe(CollectionReviewKind.CollectionGatherTo, "Gathers every grid item into one destination while shrinking and fading them.", PreviewKind.Grid);

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
            AddDestinationMotion(DestinationReviewKind.PathThrough3D, "PathThrough 3D", "Traverses two world-space waypoints with Catmull-Rom interpolation and an exact final endpoint.", PreviewKind.DestinationWorld);
            AddDestinationMotion(DestinationReviewKind.PathLocalThroughUi, "PathLocalThrough UI", "Traverses anchored waypoints with Catmull-Rom interpolation and an exact final endpoint.", PreviewKind.DestinationUi);
            AddDestinationMotion(DestinationReviewKind.SpiralTo3D, "SpiralTo 3D", "Progresses through a closing world-space spiral without jumping at either endpoint.", PreviewKind.DestinationWorld);
            AddDestinationMotion(DestinationReviewKind.SpiralLocalToUi, "SpiralLocalTo UI", "Progresses through a closing anchored spiral and lands on the exact destination.", PreviewKind.DestinationUi);
            AddDestinationMotion(DestinationReviewKind.MultiHopTo3D, "MultiHopTo 3D", "Advances through three diminishing world-space hops before landing exactly.", PreviewKind.DestinationWorld);
            AddDestinationMotion(DestinationReviewKind.MultiHopLocalToUi, "MultiHopLocalTo UI", "Advances through three diminishing anchored hops before landing exactly.", PreviewKind.DestinationUi);

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
            AddFeedbackSequence(FeedbackReviewKind.HealReceive, "Heal Receive 3D", "Communicates healing with a lift, restorative stretch, settling pulse, and green flash.", PreviewKind.DestinationWorld);
            AddFeedbackSequence(FeedbackReviewKind.ShieldBlock, "Shield Block 3D", "Compresses and recoils opposite a supplied impact direction with a blue shield flash.", PreviewKind.DestinationWorld);
            AddFeedbackSequence(FeedbackReviewKind.CriticalHit, "Critical Hit 3D", "Combines a white-hot impact, grounded squash, directional recoil, and decaying aftershock.", PreviewKind.DestinationWorld);
            AddFeedbackSequence(FeedbackReviewKind.CooldownReady, "Cooldown Ready UI", "Announces a ready ability with a relative flip, pop, lift, and cyan flash.", PreviewKind.DestinationUi);
            AddFeedbackSequence(FeedbackReviewKind.LevelUp, "Level Up UI", "Celebrates progression with lift, a relative spin, staged pulses, and gold flash.", PreviewKind.DestinationUi);
            AddFeedbackSequence(FeedbackReviewKind.LowHealthWarning, "Low Health Warning UI", "Plays one finite double-beat warning cycle suitable for caller-controlled looping.", PreviewKind.DestinationUi);

            AddUISequence(UISequenceReviewKind.ToastShow, "Slides, fades, overshoots, and settles a toast on its exact authored state.");
            AddUISequence(UISequenceReviewKind.ToastHide, "Anticipates before sliding and fading a toast out of view.");
            AddUISequence(UISequenceReviewKind.ModalOpen, "Fades the backdrop, opens the panel, and staggers controls into view.");
            AddUISequence(UISequenceReviewKind.ModalClose, "Staggers controls out before dismissing the panel and backdrop.");
            AddUISequence(UISequenceReviewKind.TooltipShow, "Subtly raises, scales, and fades a tooltip into view.");
            AddUISequence(UISequenceReviewKind.TooltipHide, "Moves and fades a tooltip out with restrained scale motion.");
            AddUISequence(UISequenceReviewKind.DropdownOpen, "Expands a dropdown from its pivot and staggers its entries into view.");
            AddUISequence(UISequenceReviewKind.DropdownClose, "Staggers entries out and compresses the dropdown toward its pivot.");
            AddUISequence(UISequenceReviewKind.TabSwitch, "Overlaps outgoing and incoming tab content while preserving both authored positions.");
            AddUISequence(UISequenceReviewKind.DrawerShow, "Slides a drawer in from the left screen edge and settles on its authored position.");
            AddUISequence(UISequenceReviewKind.DrawerHide, "Slides a drawer back through the left screen edge while fading it out.");
            AddUISequence(UISequenceReviewKind.BottomSheetShow, "Raises a bottom sheet with a small overshoot while fading its backdrop in.");
            AddUISequence(UISequenceReviewKind.BottomSheetHide, "Anticipates upward before dismissing the sheet and backdrop below the screen.");
            AddUISequence(UISequenceReviewKind.PagePush, "Pushes the outgoing page left while the incoming page enters from the right.");
            AddUISequence(UISequenceReviewKind.PageCrossFade, "Cross-fades pages with restrained depth scaling and overlapping timing.");

            AddTextValueAnimation(TextValueReviewKind.TypewriterReveal, "Reveals rich TextMesh Pro content character by character without exposing markup.");
            AddTextValueAnimation(TextValueReviewKind.TypewriterHide, "Hides currently visible TextMesh Pro content in reverse character order.");
            AddTextValueAnimation(TextValueReviewKind.NumberCountUp, "Counts from 0 to 1,250 and writes the exact formatted destination.");
            AddTextValueAnimation(TextValueReviewKind.NumberCountDown, "Counts from 1,250 to 0 using the same direction-independent operation.");
            AddTextValueAnimation(TextValueReviewKind.TextCharacterStaggerIn, "Reveals visible characters with directional movement, alpha, scale, and compressed stagger timing.");
            AddTextValueAnimation(TextValueReviewKind.TextWave, "Sends a finite wave across visible characters and restores the exact mesh baseline.");
            AddTextValueAnimation(TextValueReviewKind.ScoreIncrease, "Counts a score upward with a temporary scale punch and gold flash.");
            AddTextValueAnimation(TextValueReviewKind.TextCharacterStaggerOut, "Hides visible characters in reverse order with directional movement, scale, and alpha.");
            AddTextValueAnimation(TextValueReviewKind.TextCharacterBounce, "Sends a finite traveling bounce across visible characters and restores the mesh baseline.");
            AddTextValueAnimation(TextValueReviewKind.TextColorSweep, "Sweeps a cyan highlight across per-character vertex colors and restores the original colors.");
            AddTextValueAnimation(TextValueReviewKind.TextGlitch, "Applies a deterministic seeded offset, scale, and two-color glitch before exact restoration.");
            AddTextValueAnimation(TextValueReviewKind.TextEmphasis, "Temporarily lifts, scales, and colors a selected visible-character range.");
            AddTextValueAnimation(TextValueReviewKind.TextScrambleReveal, "Resolves deterministic substitute glyphs into the untouched rich-text source string.");

            AddCameraFeedback(CameraFeedbackReviewKind.Impact, "Camera Impact", "Applies a sharp deterministic position and rotation impact before restoring the exact camera pose.");
            AddCameraFeedback(CameraFeedbackReviewKind.Recoil, "Camera Recoil", "Kicks the camera backward and upward, adds a small aftershock, and settles exactly.");
            AddCameraFeedback(CameraFeedbackReviewKind.LandingImpact, "Camera Landing Impact", "Combines a downward bump, roll aftershock, and field-of-view kick for a heavy landing.");
            AddCameraFeedback(CameraFeedbackReviewKind.FovKick, "Camera FOV Kick", "Widens the field of view quickly and restores the exact captured value.");
            AddCameraFeedback(CameraFeedbackReviewKind.FocusZoom, "Camera Focus Zoom", "Temporarily moves and aims toward the review target while narrowing the field of view.");
            AddCameraFeedback(CameraFeedbackReviewKind.Breathing, "Camera Breathing", "Plays one subtle finite position, rotation, and field-of-view breathing cycle.");

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

        private void AddUISequence(UISequenceReviewKind kind, string description)
        {
            _allItems.Add(new ReviewItem
            {
                Id = "UISequence:" + kind,
                Name = kind == UISequenceReviewKind.TabSwitch ? "Tab Switch" : SplitPascalCase(kind.ToString()),
                Description = description,
                Kind = ReviewKind.UISequence,
                Preview = PreviewKind.UISequence,
                UISequenceKind = kind
            });
        }

        private void AddTextValueAnimation(TextValueReviewKind kind, string description)
        {
            _allItems.Add(new ReviewItem
            {
                Id = "TextValue:" + kind,
                Name = SplitPascalCase(kind.ToString()),
                Description = description,
                Kind = ReviewKind.TextValueAnimation,
                Preview = PreviewKind.TextValue,
                TextValueKind = kind
            });
        }

        private void AddCameraFeedback(CameraFeedbackReviewKind kind, string name, string description)
        {
            _allItems.Add(new ReviewItem
            {
                Id = "CameraFeedback:" + kind,
                Name = name,
                Description = description,
                Kind = ReviewKind.CameraFeedback,
                Preview = PreviewKind.CameraFeedback,
                CameraFeedbackKind = kind
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

            if (CurrentItem.UsesUISequencePreview)
            {
                _activeTween = PlayUISequence(CurrentItem.UISequenceKind);
                return;
            }

            if (CurrentItem.UsesTextValuePreview)
            {
                _activeTween = PlayTextValueAnimation(CurrentItem.TextValueKind);
                return;
            }

            if (CurrentItem.UsesCameraFeedbackPreview)
            {
                _activeTween = PlayCameraFeedback(CurrentItem.CameraFeedbackKind);
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
            uiSequencePreviewRoot.SetActive(false);
            textValuePreviewRoot.SetActive(false);
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
                case CollectionReviewKind.GridDiagonalWave:
                    return gridTargets.GridDiagonalWave(gridPreviewGroup, 3, GridDiagonalDirection.TopLeftToBottomRight, 0.34f, 0.085f);
                case CollectionReviewKind.GridSpiral:
                    return gridTargets.GridSpiral(gridPreviewGroup, 3, GridSpiralDirection.OutsideInClockwise, 0.32f, 0.07f);
                case CollectionReviewKind.GridCheckerboard:
                    return gridTargets.GridCheckerboard(gridPreviewGroup, 3, false, 0.4f, 0.2f);
                case CollectionReviewKind.CollectionBurstIn:
                    return gridTargets.CollectionBurstIn(gridPreviewGroup, Vector3.zero, 0.58f, 0.055f);
                case CollectionReviewKind.CollectionBurstOut:
                    return gridTargets.CollectionBurstOut(gridPreviewGroup, Vector3.zero, 170f, 0.52f, 0.045f);
                case CollectionReviewKind.CollectionGatherTo:
                    return gridTargets.CollectionGatherTo(gridPreviewGroup, Vector3.zero, 0.62f, 0.055f);
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
                case DestinationReviewKind.PathThrough3D:
                    return target.Tween().PathThrough(GetPathWaypoints(false, start, destination), DestinationPathInterpolation.CatmullRom, 1.65f).Play();
                case DestinationReviewKind.PathLocalThroughUi:
                    return target.Tween().PathLocalThrough(GetPathWaypoints(true, start, destination), DestinationPathInterpolation.CatmullRom, 1.65f).Play();
                case DestinationReviewKind.SpiralTo3D:
                    return target.Tween().SpiralTo(destination, 1.1f, 1.75f, 1.65f).Play();
                case DestinationReviewKind.SpiralLocalToUi:
                    return target.Tween().SpiralLocalTo(destination, 92f, 1.75f, 1.65f).Play();
                case DestinationReviewKind.MultiHopTo3D:
                    return target.Tween().MultiHopTo(destination, 2.1f, 3, 1.15f, 1.65f).Play();
                case DestinationReviewKind.MultiHopLocalToUi:
                    return target.Tween().MultiHopLocalTo(destination, 175f, 3, 1.15f, 1.65f).Play();
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
                case FeedbackReviewKind.HealReceive:
                    return target.HealReceive(1f);
                case FeedbackReviewKind.ShieldBlock:
                    return target.ShieldBlock(Vector3.right, 0.8f);
                case FeedbackReviewKind.CriticalHit:
                    return target.CriticalHit(new Vector3(1f, -0.2f, 0f), 0.9f);
                case FeedbackReviewKind.CooldownReady:
                    return target.CooldownReady(1f);
                case FeedbackReviewKind.LevelUp:
                    return target.LevelUp(1.35f);
                case FeedbackReviewKind.LowHealthWarning:
                    return target.LowHealthWarning(1.1f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown feedback review item.");
            }
        }

        private TweenHandle PlayUISequence(UISequenceReviewKind kind)
        {
            switch (kind)
            {
                case UISequenceReviewKind.ToastShow:
                    return toastSequenceTarget.ToastShow(UISequenceDirection.Up, 70f, 0.56f);
                case UISequenceReviewKind.ToastHide:
                    return toastSequenceTarget.ToastHide(UISequenceDirection.Up, 70f, 0.42f);
                case UISequenceReviewKind.ModalOpen:
                    return modalSequencePanel.ModalOpen(modalSequenceBackdrop, modalSequenceControls, 0.72f, 0.08f);
                case UISequenceReviewKind.ModalClose:
                    return modalSequencePanel.ModalClose(modalSequenceBackdrop, modalSequenceControls, 0.62f, 0.08f);
                case UISequenceReviewKind.TooltipShow:
                    return tooltipSequenceTarget.TooltipShow(UISequenceDirection.Up, 24f, 0.4f);
                case UISequenceReviewKind.TooltipHide:
                    return tooltipSequenceTarget.TooltipHide(UISequenceDirection.Up, 24f, 0.32f);
                case UISequenceReviewKind.DropdownOpen:
                    return dropdownSequencePanel.DropdownOpen(dropdownSequenceEntries, 0.58f, 0.065f);
                case UISequenceReviewKind.DropdownClose:
                    return dropdownSequencePanel.DropdownClose(dropdownSequenceEntries, 0.48f, 0.065f);
                case UISequenceReviewKind.TabSwitch:
                    return tabSequenceOutgoing.TabSwitchTo(tabSequenceIncoming, UISequenceDirection.Left, 120f, 0.62f);
                case UISequenceReviewKind.DrawerShow:
                    return dropdownSequencePanel.DrawerShow(UISequenceDirection.Left, null, 420f, 0.68f);
                case UISequenceReviewKind.DrawerHide:
                    return dropdownSequencePanel.DrawerHide(UISequenceDirection.Left, null, 420f, 0.52f);
                case UISequenceReviewKind.BottomSheetShow:
                    return modalSequencePanel.BottomSheetShow(modalSequenceBackdrop, 460f, 0.76f);
                case UISequenceReviewKind.BottomSheetHide:
                    return modalSequencePanel.BottomSheetHide(modalSequenceBackdrop, 460f, 0.62f);
                case UISequenceReviewKind.PagePush:
                    return tabSequenceOutgoing.PagePushTo(tabSequenceIncoming, UISequenceDirection.Left, 640f, 0.72f);
                case UISequenceReviewKind.PageCrossFade:
                    return tabSequenceOutgoing.PageCrossFadeTo(tabSequenceIncoming, 0.06f, 0.62f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown UI-sequence review item.");
            }
        }

        private TweenHandle PlayTextValueAnimation(TextValueReviewKind kind)
        {
            ConfigureTextValuePreview(kind);
            switch (kind)
            {
                case TextValueReviewKind.TypewriterReveal:
                    return typewriterText.TypewriterReveal(1.2f);
                case TextValueReviewKind.TypewriterHide:
                    return typewriterText.TypewriterHide(1f);
                case TextValueReviewKind.NumberCountUp:
                    return numberText.NumberCountTo(0d, 1250d, "N0", 1.15f);
                case TextValueReviewKind.NumberCountDown:
                    return numberText.NumberCountTo(1250d, 0d, "N0", 1.15f);
                case TextValueReviewKind.TextCharacterStaggerIn:
                    return characterText.TextCharacterStaggerIn(UISequenceDirection.Up, 28f, 0.045f, 1.05f);
                case TextValueReviewKind.TextWave:
                    return characterText.TextWave(UISequenceDirection.Up, 22f, 1, 1.25f);
                case TextValueReviewKind.ScoreIncrease:
                    return scoreText.ScoreIncrease(1200d, 1475d, "N0", 1.2f);
                case TextValueReviewKind.TextCharacterStaggerOut:
                    return characterText.TextCharacterStaggerOut(UISequenceDirection.Up, 30f, 0.045f, 1.05f);
                case TextValueReviewKind.TextCharacterBounce:
                    return characterText.TextCharacterBounce(UISequenceDirection.Up, 24f, 1.2f);
                case TextValueReviewKind.TextColorSweep:
                    return characterText.TextColorSweep(new Color(0.18f, 0.9f, 1f), 2.4f, 1.2f);
                case TextValueReviewKind.TextGlitch:
                    return characterText.TextGlitch(9f, 1729, 0.9f);
                case TextValueReviewKind.TextEmphasis:
                    return characterText.TextEmphasis(UISequenceDirection.Up, 12f, 0, 9, new Color(1f, 0.7f, 0.12f), 0.9f);
                case TextValueReviewKind.TextScrambleReveal:
                    return characterText.TextScrambleReveal(1729, 1.35f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown text/value review item.");
            }
        }

        private TweenHandle PlayCameraFeedback(CameraFeedbackReviewKind kind)
        {
            switch (kind)
            {
                case CameraFeedbackReviewKind.Impact:
                    return feedbackCamera.CameraImpact(0.24f, 3.2f, 0.58f);
                case CameraFeedbackReviewKind.Recoil:
                    return feedbackCamera.CameraRecoil(0.48f, 5.5f, 0.72f);
                case CameraFeedbackReviewKind.LandingImpact:
                    return feedbackCamera.CameraLandingImpact(0.32f, 4.5f, 0.78f);
                case CameraFeedbackReviewKind.FovKick:
                    return feedbackCamera.CameraFovKick(11f, 0.68f);
                case CameraFeedbackReviewKind.FocusZoom:
                    return feedbackCamera.CameraFocusZoom(cameraFocusTarget, 1.8f, 9f, 1.15f);
                case CameraFeedbackReviewKind.Breathing:
                    return feedbackCamera.CameraBreathing(0.06f, 0.55f, 0.75f, 3.2f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown camera-feedback review item.");
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
            KillTargetTweens(toastSequenceTarget);
            KillTargetTweens(modalSequencePanel);
            KillTargetTweens(modalSequenceBackdrop);
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
            KillTargetTweens(feedbackCamera.gameObject);
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
            _toastSequenceSnapshot.Apply(toastSequenceTarget);
            _modalSequenceBackdropSnapshot.Apply(modalSequenceBackdrop);
            _modalSequencePanelSnapshot.Apply(modalSequencePanel);
            ApplySnapshots(modalSequenceControls, _modalSequenceControlSnapshots);
            _tooltipSequenceSnapshot.Apply(tooltipSequenceTarget);
            _dropdownSequencePanelSnapshot.Apply(dropdownSequencePanel);
            ApplySnapshots(dropdownSequenceEntries, _dropdownSequenceEntrySnapshots);
            _tabSequenceOutgoingSnapshot.Apply(tabSequenceOutgoing);
            _tabSequenceIncomingSnapshot.Apply(tabSequenceIncoming);
            _typewriterTextSnapshot.Apply(typewriterText);
            _numberTextSnapshot.Apply(numberText);
            _characterTextSnapshot.Apply(characterText);
            _scoreTextSnapshot.Apply(scoreText);
            _feedbackCameraSnapshot.Apply(feedbackCamera);
        }

        private void ApplyPreviewVisibility(PreviewKind preview)
        {
            uiTarget.SetActive(preview == PreviewKind.Ui);
            worldTarget.SetActive(preview == PreviewKind.World || preview == PreviewKind.CameraFeedback);
            bool showCollection = preview == PreviewKind.List || preview == PreviewKind.Grid || preview == PreviewKind.LoadingDots;
            collectionPreviewRoot.SetActive(showCollection);
            listPreviewGroup.SetActive(preview == PreviewKind.List);
            gridPreviewGroup.SetActive(preview == PreviewKind.Grid);
            loadingDotsPreviewGroup.SetActive(preview == PreviewKind.LoadingDots);
            destinationWorldRoot.SetActive(preview == PreviewKind.DestinationWorld);
            destinationUiRoot.SetActive(preview == PreviewKind.DestinationUi);
            bool showUISequence = preview == PreviewKind.UISequence;
            uiSequencePreviewRoot.SetActive(showUISequence);
            if (showUISequence) ConfigureUISequencePreview(CurrentItem.UISequenceKind);
            bool showTextValue = preview == PreviewKind.TextValue;
            textValuePreviewRoot.SetActive(showTextValue);
            if (showTextValue) ConfigureTextValuePreview(CurrentItem.TextValueKind);
        }

        private void ConfigureUISequencePreview(UISequenceReviewKind kind)
        {
            bool showToast = kind == UISequenceReviewKind.ToastShow || kind == UISequenceReviewKind.ToastHide;
            bool showModal = kind == UISequenceReviewKind.ModalOpen || kind == UISequenceReviewKind.ModalClose || kind == UISequenceReviewKind.BottomSheetShow || kind == UISequenceReviewKind.BottomSheetHide;
            bool showTooltip = kind == UISequenceReviewKind.TooltipShow || kind == UISequenceReviewKind.TooltipHide;
            bool showDropdown = kind == UISequenceReviewKind.DropdownOpen || kind == UISequenceReviewKind.DropdownClose || kind == UISequenceReviewKind.DrawerShow || kind == UISequenceReviewKind.DrawerHide;
            toastSequenceTarget.SetActive(showToast);
            modalSequenceGroup.SetActive(showModal);
            tooltipSequenceTarget.SetActive(showTooltip);
            dropdownSequencePanel.SetActive(showDropdown);
            bool showPages = kind == UISequenceReviewKind.TabSwitch || kind == UISequenceReviewKind.PagePush || kind == UISequenceReviewKind.PageCrossFade;
            tabSequenceGroup.SetActive(showPages);
        }

        private void ConfigureTextValuePreview(TextValueReviewKind kind)
        {
            bool showTypewriter = kind == TextValueReviewKind.TypewriterReveal || kind == TextValueReviewKind.TypewriterHide;
            bool showNumber = kind == TextValueReviewKind.NumberCountUp || kind == TextValueReviewKind.NumberCountDown;
            bool showCharacter = kind == TextValueReviewKind.TextCharacterStaggerIn ||
                                 kind == TextValueReviewKind.TextWave ||
                                 kind == TextValueReviewKind.TextCharacterStaggerOut ||
                                 kind == TextValueReviewKind.TextCharacterBounce ||
                                 kind == TextValueReviewKind.TextColorSweep ||
                                 kind == TextValueReviewKind.TextGlitch ||
                                 kind == TextValueReviewKind.TextEmphasis ||
                                 kind == TextValueReviewKind.TextScrambleReveal;
            typewriterText.gameObject.SetActive(showTypewriter);
            numberText.gameObject.SetActive(showNumber);
            characterText.gameObject.SetActive(showCharacter);
            scoreText.gameObject.SetActive(kind == TextValueReviewKind.ScoreIncrease);
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
            if (showPath) UpdateDestinationPath(isDestination ? item.DestinationKind : DestinationReviewKind.ArcTo3D);
            destinationWorldCurvedPath.SetActive(showPath && item.Preview == PreviewKind.DestinationWorld);
            destinationUiCurvedPath.SetActive(showPath && item.Preview == PreviewKind.DestinationUi);
        }

        private void UpdateDestinationPath(DestinationReviewKind kind)
        {
            bool usesUi = CurrentItem.Preview == PreviewKind.DestinationUi;
            Transform pathRoot = usesUi ? destinationUiCurvedPath.transform : destinationWorldCurvedPath.transform;
            Vector3 start = usesUi ? destinationUiStartMarker.anchoredPosition3D : destinationWorldStartMarker.position;
            Vector3 destination = usesUi ? destinationUiEndMarker.anchoredPosition3D : destinationWorldEndMarker.position;
            float height = usesUi ? DestinationUiArcHeight : DestinationWorldArcHeight;
            GetBezierControls(usesUi, start, destination, out Vector3 controlA, out Vector3 controlB);
            Vector3[] waypoints = GetPathWaypoints(usesUi, start, destination);

            for (int i = 0; i < pathRoot.childCount; i++)
            {
                float progress = (i + 1f) / (pathRoot.childCount + 1f);
                Vector3 point;
                if (IsBezier(kind)) point = EvaluateBezier(start, controlA, controlB, destination, progress);
                else if (IsPath(kind)) point = EvaluatePath(start, waypoints, progress);
                else if (IsSpiral(kind)) point = EvaluateSpiral(start, destination, usesUi ? 92f : 1.1f, 1.75f, progress, usesUi);
                else if (IsMultiHop(kind)) point = EvaluateMultiHop(start, destination, height, 3, 1.15f, progress);
                else point = EvaluateArc(start, destination, height, progress);
                if (usesUi) ((RectTransform)pathRoot.GetChild(i)).anchoredPosition3D = point;
                else pathRoot.GetChild(i).position = point;
            }
        }

        private static Vector3[] GetPathWaypoints(bool usesUi, Vector3 start, Vector3 destination)
        {
            float height = usesUi ? DestinationUiArcHeight : DestinationWorldArcHeight;
            float horizontal = usesUi ? 75f : 0.9f;
            return new[]
            {
                Vector3.Lerp(start, destination, 0.32f) + Vector3.up * height + Vector3.left * horizontal,
                Vector3.Lerp(start, destination, 0.68f) + Vector3.up * height * 0.28f + Vector3.right * horizontal,
                destination
            };
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

        private static Vector3 EvaluateSpiral(Vector3 start, Vector3 destination, float radius, float revolutions, float progress, bool usesUi)
        {
            Vector3 basePosition = Vector3.LerpUnclamped(start, destination, DOVirtual.EasedValue(0f, 1f, progress, Ease.InOutCubic));
            Vector3 axis = destination - start;
            if (axis.sqrMagnitude <= 0.000001f) return basePosition;
            axis.Normalize();
            Vector3 basisA;
            Vector3 basisB;
            if (usesUi)
            {
                basisA = Vector3.right;
                basisB = Vector3.up;
            }
            else
            {
                Vector3 reference = Mathf.Abs(Vector3.Dot(axis, Vector3.up)) > 0.92f ? Vector3.right : Vector3.up;
                basisA = Vector3.Cross(axis, reference).normalized;
                basisB = Vector3.Cross(axis, basisA).normalized;
            }

            float angle = progress * revolutions * Mathf.PI * 2f;
            float envelope = Mathf.Sin(progress * Mathf.PI);
            return basePosition + (basisA * Mathf.Cos(angle) + basisB * Mathf.Sin(angle)) * radius * envelope;
        }

        private static Vector3 EvaluateMultiHop(Vector3 start, Vector3 destination, float height, int hopCount, float decay, float progress)
        {
            float travel = DOVirtual.EasedValue(0f, 1f, progress, Ease.InOutCubic);
            float bounce = Mathf.Abs(Mathf.Sin(progress * hopCount * Mathf.PI));
            float envelope = Mathf.Pow(1f - Mathf.Clamp01(progress), decay);
            return Vector3.LerpUnclamped(start, destination, travel) + Vector3.up * height * bounce * envelope;
        }

        private static bool IsBezier(DestinationReviewKind kind) => kind == DestinationReviewKind.BezierTo3D || kind == DestinationReviewKind.BezierLocalToUi;

        private static bool IsPath(DestinationReviewKind kind) => kind == DestinationReviewKind.PathThrough3D || kind == DestinationReviewKind.PathLocalThroughUi;

        private static bool IsSpiral(DestinationReviewKind kind) => kind == DestinationReviewKind.SpiralTo3D || kind == DestinationReviewKind.SpiralLocalToUi;

        private static bool IsMultiHop(DestinationReviewKind kind) => kind == DestinationReviewKind.MultiHopTo3D || kind == DestinationReviewKind.MultiHopLocalToUi;

        private static bool UsesCurvedPath(DestinationReviewKind kind)
        {
            return kind == DestinationReviewKind.ArcTo3D ||
                   kind == DestinationReviewKind.ArcLocalToUi ||
                   kind == DestinationReviewKind.BezierTo3D ||
                   kind == DestinationReviewKind.BezierLocalToUi ||
                   kind == DestinationReviewKind.HopTo3D ||
                   kind == DestinationReviewKind.HopLocalToUi ||
                   IsPath(kind) ||
                   IsSpiral(kind) ||
                   IsMultiHop(kind);
        }

        private static string GetCategoryLabel(ReviewItem item)
        {
            if (item.Kind == ReviewKind.UiRecipe) return "UI RECIPE";
            if (item.Kind == ReviewKind.CollectionRecipe) return "COLLECTION RECIPE";
            if (item.Kind == ReviewKind.StaggerVariant) return "STAGGER VARIANT";
            if (item.Kind == ReviewKind.DestinationMotion) return item.Preview == PreviewKind.DestinationUi ? "DESTINATION MOTION / UI" : "DESTINATION MOTION / 3D";
            if (item.Kind == ReviewKind.FeedbackSequence) return item.Preview == PreviewKind.DestinationUi ? "GAMEPLAY FEEDBACK / UI" : "GAMEPLAY FEEDBACK / 3D";
            if (item.Kind == ReviewKind.UISequence) return "PRODUCTION UI SEQUENCE";
            if (item.Kind == ReviewKind.TextValueAnimation) return "TEXT & VALUE ANIMATION";
            if (item.Kind == ReviewKind.CameraFeedback) return "CAMERA FEEDBACK";
            return item.UsesUiTarget ? "2D / UI PRESET" : "3D / WORLD PRESET";
        }

        private static string SplitPascalCase(string value)
        {
            var result = new System.Text.StringBuilder(value.Length + 4);
            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0 && char.IsUpper(value[i])) result.Append(' ');
                result.Append(value[i]);
            }

            return result.ToString();
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
