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
            GameplayState,
            SequenceMacro,
            UISequence,
            TextValueAnimation,
            ProgressAnimation,
            CameraFeedback,
            EngineProperty,
            Preset
        }

        private enum PreviewKind
        {
            Ui,
            World,
            List,
            Grid,
            IncompleteGrid,
            WorldCollection,
            LoadingDots,
            DestinationWorld,
            DestinationUi,
            WorldToUi,
            UISequence,
            TextValue,
            WorldTextValue,
            Progress,
            CameraFeedback,
            EngineProperty
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
            CollectionGatherTo,
            GridConcentricIn,
            GridConcentricOut,
            GridQuadrantSweep,
            ListAccordion,
            CollectionOrbitIn,
            CollectionOrbitOut,
            LoadingRing,
            LoadingRibbon,
            StaggerPresetByName,
            StaggerCustomAnimate
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
            MultiHopLocalToUi,
            ArcToUiProjected,
            HopToUiProjected,
            BezierToUiProjected,
            PathThroughUiProjected
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
            LowHealthWarning,
            PickupCollectToUi,
            AbilityCharging,
            AbilityReady,
            DodgeRoll,
            StunStart,
            StunEnd,
            BuffApplied,
            DebuffApplied,
            ResourceDepleted,
            ResourceRecovered,
            ObjectiveUnlocked
        }

        private enum SequenceMacroReviewKind
        {
            CriticalHit,
            RewardReveal,
            WarningLoop,
            CutsceneUiEntrance
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
            Breathing,
            RackFocus,
            CollectLandingKick
        }

        private enum ProgressReviewKind
        {
            ImageFillTo,
            SliderFillTo,
            ImageFillFromTo,
            SliderFillFromTo,
            ImageValueFillTo,
            SliderValueFillTo,
            ImageFillDrain,
            SliderFillDrain,
            ImageFillCharge,
            SliderFillCharge,
            ImageFillAlertPulse,
            SliderFillAlertPulse,
            ImageFillAndText,
            SliderFillAndText,
            ProgressHook
        }

        private enum EnginePropertyReviewKind
        {
            AudioVolume,
            AudioPitch,
            LightIntensity,
            LightColor,
            ParticleEmissionRate,
            MaterialFloat,
            MaterialColor,
            TorchFlicker,
            ScannerPulse
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
            public string VariantKey;
            public ReviewKind Kind;
            public ITweenPreset Preset;
            public PreviewKind Preview;
            public CollectionReviewKind CollectionKind;
            public DestinationReviewKind DestinationKind;
            public FeedbackReviewKind FeedbackKind;
            public SequenceMacroReviewKind SequenceMacroKind;
            public UISequenceReviewKind UISequenceKind;
            public TextValueReviewKind TextValueKind;
            public ProgressReviewKind ProgressKind;
            public CameraFeedbackReviewKind CameraFeedbackKind;
            public EnginePropertyReviewKind EnginePropertyKind;
            public UISequenceDirection Direction;
            public GridDiagonalDirection DiagonalDirection;
            public GridSpiralDirection SpiralDirection;
            public DestinationPathInterpolation PathInterpolation = DestinationPathInterpolation.CatmullRom;
            public int GridColumns = 3;
            public int OriginIndex = -1;
            public float SignedMagnitude = 1f;
            public bool Inverted;
            public bool UseDefaultDistance;
            public bool UseBackdrop;
            public bool ReverseDirection;

            public bool UsesUiTarget => Preview == PreviewKind.Ui;
            public bool UsesCollectionPreview => Preview == PreviewKind.List || Preview == PreviewKind.Grid || Preview == PreviewKind.IncompleteGrid || Preview == PreviewKind.WorldCollection || Preview == PreviewKind.LoadingDots;
            public bool UsesDestinationPreview => Preview == PreviewKind.DestinationWorld || Preview == PreviewKind.DestinationUi || Preview == PreviewKind.WorldToUi;
            public bool UsesUISequencePreview => Preview == PreviewKind.UISequence;
            public bool UsesTextValuePreview => Preview == PreviewKind.TextValue || Preview == PreviewKind.WorldTextValue;
            public bool UsesProgressPreview => Preview == PreviewKind.Progress;
            public bool UsesCameraFeedbackPreview => Preview == PreviewKind.CameraFeedback;
            public bool UsesEnginePropertyPreview => Preview == PreviewKind.EngineProperty;
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
        private static readonly Color ReviewBlueColor = new Color(0.1f, 0.58f, 0.95f);

        [Header("Preview Targets")]
        [SerializeField] private GameObject uiTarget;
        [SerializeField] private GameObject worldTarget;

        [Header("Collection Preview")]
        [SerializeField] private GameObject collectionPreviewRoot;
        [SerializeField] private GameObject listPreviewGroup;
        [SerializeField] private GameObject gridPreviewGroup;
        [SerializeField] private GameObject incompleteGridPreviewGroup;
        [SerializeField] private GameObject worldCollectionPreviewRoot;
        [SerializeField] private GameObject loadingDotsPreviewGroup;
        [SerializeField] private GameObject[] listTargets;
        [SerializeField] private GameObject[] gridTargets;
        [SerializeField] private GameObject[] incompleteGridTargets;
        [SerializeField] private GameObject[] worldCollectionTargets;
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
        [SerializeField] private GameObject drawerSequenceBackdrop;

        [Header("Text & Value Preview")]
        [SerializeField] private GameObject textValuePreviewRoot;
        [SerializeField] private TMP_Text typewriterText;
        [SerializeField] private TMP_Text numberText;
        [SerializeField] private TMP_Text characterText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private GameObject worldTextValuePreviewRoot;
        [SerializeField] private TMP_Text worldCharacterText;

        [Header("Progress Preview")]
        [SerializeField] private GameObject progressPreviewRoot;
        [SerializeField] private GameObject progressImageGroup;
        [SerializeField] private Image progressImage;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TMP_Text progressValueText;
        [SerializeField] private TMP_Text progressEventText;

        [Header("Camera Feedback Preview")]
        [SerializeField] private Camera feedbackCamera;
        [SerializeField] private Transform cameraFocusTarget;

        [Header("Engine Property Preview")]
        [SerializeField] private GameObject enginePropertyPreviewRoot;
        [SerializeField] private GameObject enginePropertyWorldRoot;
        [SerializeField] private AudioSource engineAudioSource;
        [SerializeField] private Light engineLight;
        [SerializeField] private ParticleSystem engineParticles;
        [SerializeField] private Renderer engineRenderer;
        [SerializeField] private Image enginePropertyMeter;
        [SerializeField] private TMP_Text enginePropertyValueText;

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
        private TargetSnapshot[] _incompleteGridSnapshots;
        private TargetSnapshot[] _worldCollectionSnapshots;
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
        private TargetSnapshot _drawerSequenceBackdropSnapshot;
        private TMPTextSnapshot _typewriterTextSnapshot;
        private TMPTextSnapshot _numberTextSnapshot;
        private TMPTextSnapshot _characterTextSnapshot;
        private TMPTextSnapshot _scoreTextSnapshot;
        private TMPTextSnapshot _worldCharacterTextSnapshot;
        private CameraSnapshot _feedbackCameraSnapshot;
        private TargetSnapshot _progressImageSnapshot;
        private TargetSnapshot _progressSliderSnapshot;
        private float _progressImageFill;
        private float _progressSliderValue;
        private Color _progressImageColor;
        private float _engineAudioVolume;
        private float _engineAudioPitch;
        private float _engineLightIntensity;
        private Color _engineLightColor;
        private float _engineParticleEmissionRate;
        private TargetSnapshot _engineRendererSnapshot;
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
            _incompleteGridSnapshots = CaptureTargets(incompleteGridTargets);
            _worldCollectionSnapshots = CaptureTargets(worldCollectionTargets);
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
            _drawerSequenceBackdropSnapshot = TargetSnapshot.Capture(drawerSequenceBackdrop);
            _typewriterTextSnapshot = TMPTextSnapshot.Capture(typewriterText);
            _numberTextSnapshot = TMPTextSnapshot.Capture(numberText);
            _characterTextSnapshot = TMPTextSnapshot.Capture(characterText);
            _scoreTextSnapshot = TMPTextSnapshot.Capture(scoreText);
            _worldCharacterTextSnapshot = TMPTextSnapshot.Capture(worldCharacterText);
            _feedbackCameraSnapshot = CameraSnapshot.Capture(feedbackCamera);
            _progressImageSnapshot = TargetSnapshot.Capture(progressImage.gameObject);
            _progressSliderSnapshot = TargetSnapshot.Capture(progressSlider.gameObject);
            _progressImageFill = progressImage.fillAmount;
            _progressSliderValue = progressSlider.normalizedValue;
            _progressImageColor = progressImage.color;
            _engineAudioVolume = engineAudioSource.volume;
            _engineAudioPitch = engineAudioSource.pitch;
            _engineLightIntensity = engineLight.intensity;
            _engineLightColor = engineLight.color;
            _engineParticleEmissionRate = engineParticles.emission.rateOverTimeMultiplier;
            _engineRendererSnapshot = TargetSnapshot.Capture(engineRenderer.gameObject);
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

        private void LateUpdate()
        {
            if (progressPreviewRoot.activeSelf && _items.Count > 0) UpdateProgressReadout(CurrentItem.ProgressKind);
            if (enginePropertyPreviewRoot.activeSelf && _items.Count > 0) UpdateEnginePropertyReadout(CurrentItem.EnginePropertyKind);
        }

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
            AddCollectionRecipe(CollectionReviewKind.GridConcentricIn, "Reveals grid rings from the outside toward the center.", PreviewKind.Grid);
            AddCollectionRecipe(CollectionReviewKind.GridConcentricOut, "Dismisses grid rings from the center toward the outside.", PreviewKind.Grid);
            AddCollectionRecipe(CollectionReviewKind.GridQuadrantSweep, "Sweeps the four grid quadrants clockwise from the top-left.", PreviewKind.Grid);
            AddCollectionRecipe(CollectionReviewKind.ListAccordion, "Expands list items from a collapsed accordion stack.", PreviewKind.List);
            AddCollectionRecipe(CollectionReviewKind.CollectionOrbitIn, "Orbits collection items inward before settling on their authored positions.", PreviewKind.Grid);
            AddCollectionRecipe(CollectionReviewKind.CollectionOrbitOut, "Orbits collection items outward while shrinking and fading.", PreviewKind.Grid);
            AddCollectionRecipe(CollectionReviewKind.LoadingRing, "Loops a circular loading displacement across grid items.", PreviewKind.Grid);
            AddCollectionRecipe(CollectionReviewKind.LoadingRibbon, "Loops a traveling ribbon displacement across list items.", PreviewKind.List);

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
            AddDestinationMotion(DestinationReviewKind.ArcToUiProjected, "ArcToUI", "Projects a world-space source into the review canvas and arcs into a live UI destination.", PreviewKind.WorldToUi);
            AddDestinationMotion(DestinationReviewKind.HopToUiProjected, "HopToUI", "Projects a world-space source into UI space, adds hop squash, and lands exactly on the UI destination.", PreviewKind.WorldToUi);
            AddDestinationMotion(DestinationReviewKind.BezierToUiProjected, "BezierToUI", "Projects world-space controls into the canvas and follows them into the UI destination.", PreviewKind.WorldToUi);
            AddDestinationMotion(DestinationReviewKind.PathThroughUiProjected, "PathThroughUI", "Projects world landmarks into a canvas path and finishes on the locked UI destination.", PreviewKind.WorldToUi);

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
            AddFeedbackSequence(FeedbackReviewKind.PickupCollectToUi, "PickupCollectToUI", "Projects a world pickup into the canvas, then arcs, shrinks, and fades it into the UI destination.", PreviewKind.WorldToUi);

            AddGameplayState(FeedbackReviewKind.AbilityCharging, "Ability Charging", "Builds anticipation with a rising pulse and charged accent.", PreviewKind.DestinationUi);
            AddGameplayState(FeedbackReviewKind.AbilityReady, "Ability Ready", "Confirms readiness with a crisp pop, lift, and cyan flash.", PreviewKind.DestinationUi);
            AddGameplayState(FeedbackReviewKind.DodgeRoll, "Dodge Roll", "Compresses, rotates, and returns the target through a finite evasive roll.", PreviewKind.DestinationWorld);
            AddGameplayState(FeedbackReviewKind.StunStart, "Stun Start", "Introduces a staggered tilt, squash, and stun-color flash.", PreviewKind.DestinationWorld);
            AddGameplayState(FeedbackReviewKind.StunEnd, "Stun End", "Releases the stunned state with a recovering stretch and settle.", PreviewKind.DestinationWorld);
            AddGameplayState(FeedbackReviewKind.BuffApplied, "Buff Applied", "Lifts and pulses the target with a positive status accent.", PreviewKind.DestinationUi);
            AddGameplayState(FeedbackReviewKind.DebuffApplied, "Debuff Applied", "Drops and shakes the target with a negative status accent.", PreviewKind.DestinationUi);
            AddGameplayState(FeedbackReviewKind.ResourceDepleted, "Resource Depleted", "Collapses and fades the target to communicate an exhausted resource.", PreviewKind.DestinationUi);
            AddGameplayState(FeedbackReviewKind.ResourceRecovered, "Resource Recovered", "Restores the target with a rising scale pulse and recovery flash.", PreviewKind.DestinationUi);
            AddGameplayState(FeedbackReviewKind.ObjectiveUnlocked, "Objective Unlocked", "Celebrates a newly available objective with lift, spin, and a gold accent.", PreviewKind.DestinationWorld);

            AddSequenceMacro(SequenceMacroReviewKind.CriticalHit, "Critical Hit Sequence", "Runs the reusable critical-hit macro with directional impact and aftershock.", PreviewKind.DestinationWorld);
            AddSequenceMacro(SequenceMacroReviewKind.RewardReveal, "Reward Reveal Sequence", "Runs the reusable reward-reveal macro with anticipation and celebration.", PreviewKind.DestinationWorld);
            AddSequenceMacro(SequenceMacroReviewKind.WarningLoop, "Warning Loop Sequence", "Plays one finite warning-loop macro cycle for caller-controlled repetition.", PreviewKind.DestinationUi);
            AddSequenceMacro(SequenceMacroReviewKind.CutsceneUiEntrance, "Cutscene UI Entrance Sequence", "Introduces a cutscene UI element with a composed cinematic entrance.", PreviewKind.DestinationUi);

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

            AddProgressAnimation(ProgressReviewKind.ImageFillTo, "Image Fill To", "Animates Image.fillAmount from its current normalized value to 85%.");
            AddProgressAnimation(ProgressReviewKind.SliderFillTo, "Slider Fill To", "Animates Slider.normalizedValue from its current value to 85%.");
            AddProgressAnimation(ProgressReviewKind.ImageFillFromTo, "Image Fill From To", "Sets an explicit 15% image-fill start and animates to 82%.");
            AddProgressAnimation(ProgressReviewKind.SliderFillFromTo, "Slider Fill From To", "Sets an explicit 15% slider start and animates to 82%.");
            AddProgressAnimation(ProgressReviewKind.ImageValueFillTo, "Image Value Fill To", "Keeps an Image fill and formatted percentage text synchronized.");
            AddProgressAnimation(ProgressReviewKind.SliderValueFillTo, "Slider Value Fill To", "Keeps a Slider value and formatted percentage text synchronized.");
            AddProgressAnimation(ProgressReviewKind.ImageFillDrain, "Image Fill Drain", "Drains an Image fill quickly with impact motion and a red accent.");
            AddProgressAnimation(ProgressReviewKind.SliderFillDrain, "Slider Fill Drain", "Drains a Slider quickly with impact motion and a red accent.");
            AddProgressAnimation(ProgressReviewKind.ImageFillCharge, "Image Fill Charge", "Charges an Image fill with an overshoot-and-settle pulse.");
            AddProgressAnimation(ProgressReviewKind.SliderFillCharge, "Slider Fill Charge", "Charges a Slider with an overshoot-and-settle pulse.");
            AddProgressAnimation(ProgressReviewKind.ImageFillAlertPulse, "Image Fill Alert Pulse", "Plays the finite critical-threshold warning on a low Image fill.");
            AddProgressAnimation(ProgressReviewKind.SliderFillAlertPulse, "Slider Fill Alert Pulse", "Plays the finite critical-threshold warning on a low Slider value.");
            AddProgressAnimation(ProgressReviewKind.ImageFillAndText, "Image Fill And Text", "Animates an Image fill and paired percentage on one timeline.");
            AddProgressAnimation(ProgressReviewKind.SliderFillAndText, "Slider Fill And Text", "Animates a Slider and paired percentage on one timeline.");
            AddProgressAnimation(ProgressReviewKind.ProgressHook, "On Progress Hook", "Fires a visible callback once when the fill timeline crosses 50%.");

            AddCameraFeedback(CameraFeedbackReviewKind.Impact, "Camera Impact", "Applies a sharp deterministic position and rotation impact before restoring the exact camera pose.");
            AddCameraFeedback(CameraFeedbackReviewKind.Recoil, "Camera Recoil", "Kicks the camera backward and upward, adds a small aftershock, and settles exactly.");
            AddCameraFeedback(CameraFeedbackReviewKind.LandingImpact, "Camera Landing Impact", "Combines a downward bump, roll aftershock, and field-of-view kick for a heavy landing.");
            AddCameraFeedback(CameraFeedbackReviewKind.FovKick, "Camera FOV Kick", "Widens the field of view quickly and restores the exact captured value.");
            AddCameraFeedback(CameraFeedbackReviewKind.FocusZoom, "Camera Focus Zoom", "Temporarily moves and aims toward the review target while narrowing the field of view.");
            AddCameraFeedback(CameraFeedbackReviewKind.Breathing, "Camera Breathing", "Plays one subtle finite position, rotation, and field-of-view breathing cycle.");
            AddCameraFeedback(CameraFeedbackReviewKind.RackFocus, "Camera Rack Focus", "Shifts aim and field of view toward the gameplay focus target, then restores the camera.");
            AddCameraFeedback(CameraFeedbackReviewKind.CollectLandingKick, "Collect Landing Camera Kick", "Adds the short camera micro-kick intended for pickup-to-UI completion.");

            AddEngineProperty(EnginePropertyReviewKind.AudioVolume, "Audio Volume To", "Animates AudioSource.volume and mirrors the live value in the preview meter.");
            AddEngineProperty(EnginePropertyReviewKind.AudioPitch, "Audio Pitch To", "Animates AudioSource.pitch and mirrors the live value in the preview meter.");
            AddEngineProperty(EnginePropertyReviewKind.LightIntensity, "Light Intensity To", "Animates a point light's intensity while the preview meter tracks it.");
            AddEngineProperty(EnginePropertyReviewKind.LightColor, "Light Color To", "Transitions a point light from warm amber to cyan.");
            AddEngineProperty(EnginePropertyReviewKind.ParticleEmissionRate, "Particle Emission Rate To", "Raises ParticleSystem emission while the live rate is displayed.");
            AddEngineProperty(EnginePropertyReviewKind.MaterialFloat, "Material Float To", "Animates the renderer material-block smoothness scalar without cloning its material.");
            AddEngineProperty(EnginePropertyReviewKind.MaterialColor, "Material Color To", "Animates the renderer material-block base color without mutating the shared material.");
            AddEngineProperty(EnginePropertyReviewKind.TorchFlicker, "Torch Flicker", "Plays a finite layered light-intensity flicker and restores its baseline.");
            AddEngineProperty(EnginePropertyReviewKind.ScannerPulse, "Scanner Pulse", "Pulses light intensity and color through a targeting-style cyan accent.");

            AddReviewCoverageItems();

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
                    Preview = PresetReviewCompatibility.IsSuitable(preset) ? PreviewKind.Ui : PreviewKind.World
                });
            }

            RebuildFilteredItems();
        }

        private void AddReviewCoverageItems()
        {
            AddCollectionReviewCoverage();
            AddDestinationReviewCoverage();
            AddUISequenceReviewCoverage();
            AddTextValueReviewCoverage();
            AddFeedbackReviewCoverage();
            AddCameraReviewCoverage();
        }

        private void AddCollectionReviewCoverage()
        {
            ReviewItem item = AddCollectionRecipe(CollectionReviewKind.GridDiagonalWave, "Starts at the top-right and reveals diagonals through the bottom-left.", PreviewKind.Grid, "TopRightToBottomLeft", "Grid Diagonal Wave - Top Right to Bottom Left");
            item.DiagonalDirection = GridDiagonalDirection.TopRightToBottomLeft;
            item = AddCollectionRecipe(CollectionReviewKind.GridDiagonalWave, "Starts at the bottom-left and reveals diagonals through the top-right.", PreviewKind.Grid, "BottomLeftToTopRight", "Grid Diagonal Wave - Bottom Left to Top Right");
            item.DiagonalDirection = GridDiagonalDirection.BottomLeftToTopRight;
            item = AddCollectionRecipe(CollectionReviewKind.GridDiagonalWave, "Starts at the bottom-right and reveals diagonals through the top-left.", PreviewKind.Grid, "BottomRightToTopLeft", "Grid Diagonal Wave - Bottom Right to Top Left");
            item.DiagonalDirection = GridDiagonalDirection.BottomRightToTopLeft;

            item = AddCollectionRecipe(CollectionReviewKind.GridSpiral, "Starts at the top-left, winds counter-clockwise around the outside, and finishes at the center.", PreviewKind.Grid, "OutsideInCounterClockwise", "Grid Spiral - Outside In Counter-Clockwise");
            item.SpiralDirection = GridSpiralDirection.OutsideInCounterClockwise;
            item = AddCollectionRecipe(CollectionReviewKind.GridSpiral, "Starts at the center, winds clockwise through expanding rings, and finishes at the outside edge.", PreviewKind.Grid, "InsideOutClockwise", "Grid Spiral - Inside Out Clockwise");
            item.SpiralDirection = GridSpiralDirection.InsideOutClockwise;
            item = AddCollectionRecipe(CollectionReviewKind.GridSpiral, "Starts at the center, winds counter-clockwise through expanding rings, and finishes at the outside edge.", PreviewKind.Grid, "InsideOutCounterClockwise", "Grid Spiral - Inside Out Counter-Clockwise");
            item.SpiralDirection = GridSpiralDirection.InsideOutCounterClockwise;
            item = AddCollectionRecipe(CollectionReviewKind.GridCheckerboard, "Pulses the opposite checkerboard cells first, then completes the second phase.", PreviewKind.Grid, "Inverted", "Grid Checkerboard - Inverted");
            item.Inverted = true;

            item = AddCollectionRecipe(CollectionReviewKind.GridRipple, "Pulses outward from the top-left corner origin.", PreviewKind.Grid, "CornerOrigin", "Grid Ripple - Corner Origin");
            item.OriginIndex = 0;
            item = AddCollectionRecipe(CollectionReviewKind.GridRipple, "Pulses outward from the top-center edge origin.", PreviewKind.Grid, "EdgeOrigin", "Grid Ripple - Edge Origin");
            item.OriginIndex = 1;
            AddStaggerVariant(CollectionReviewKind.StaggerPresetByName, "Resolves PulseScaleSoft dynamically by its registered preset name.", PreviewKind.List, "PresetByName", "Stagger - Preset By Name");
            AddStaggerVariant(CollectionReviewKind.StaggerCustomAnimate, "Runs a custom alternating scale-and-rotation punch factory for every list item.", PreviewKind.List, "CustomAnimate", "Stagger - Custom Animate Factory");
            AddCollectionRecipe(CollectionReviewKind.CollectionBurstIn, "Launches world-space objects from the shared origin into their authored positions.", PreviewKind.WorldCollection, "World", "Collection Burst In - World");
            item = AddCollectionRecipe(CollectionReviewKind.CollectionBurstOut, "Scatters world-space objects using the automatic 1.2-unit default distance.", PreviewKind.WorldCollection, "World", "Collection Burst Out - World Default Distance");
            item.UseDefaultDistance = true;
            AddCollectionRecipe(CollectionReviewKind.CollectionGatherTo, "Gathers world-space objects into one exact destination while shrinking and fading.", PreviewKind.WorldCollection, "World", "Collection Gather To - World");
            item = AddCollectionRecipe(CollectionReviewKind.CollectionBurstOut, "Scatters UI cells using the automatic 120-canvas-unit default distance.", PreviewKind.Grid, "DefaultDistanceUI", "Collection Burst Out - UI Default Distance");
            item.UseDefaultDistance = true;
            AddCollectionRecipe(CollectionReviewKind.GridDiagonalWave, "Traverses a three-column, eight-item grid and preserves the incomplete final row.", PreviewKind.IncompleteGrid, "IncompleteGrid", "Grid Diagonal Wave - Incomplete Grid");
            AddCollectionRecipe(CollectionReviewKind.GridSpiral, "Traverses a three-column, eight-item spiral without skipping the incomplete final row.", PreviewKind.IncompleteGrid, "IncompleteGrid", "Grid Spiral - Incomplete Grid");
        }

        private void AddDestinationReviewCoverage()
        {
            ReviewItem item = AddDestinationMotion(DestinationReviewKind.PathThrough3D, "PathThrough 3D - Linear", "Traverses every world-space waypoint with straight linear segments and an exact final endpoint.", PreviewKind.DestinationWorld, "Linear");
            item.PathInterpolation = DestinationPathInterpolation.Linear;
            item = AddDestinationMotion(DestinationReviewKind.PathLocalThroughUi, "PathLocalThrough UI - Linear", "Traverses every anchored waypoint with straight linear segments and an exact final endpoint.", PreviewKind.DestinationUi, "Linear");
            item.PathInterpolation = DestinationPathInterpolation.Linear;

            item = AddDestinationMotion(DestinationReviewKind.ArcTo3D, "ArcTo 3D - Downward", "Uses a negative world-space height to arc downward before landing exactly.", PreviewKind.DestinationWorld, "Downward");
            item.SignedMagnitude = -1f;
            item = AddDestinationMotion(DestinationReviewKind.ArcLocalToUi, "ArcLocalTo UI - Downward", "Uses a negative anchored height to arc downward before landing exactly.", PreviewKind.DestinationUi, "Downward");
            item.SignedMagnitude = -1f;
            item = AddDestinationMotion(DestinationReviewKind.HopTo3D, "HopTo 3D - Downward", "Uses a negative world-space hop height while preserving exact landing and scale restoration.", PreviewKind.DestinationWorld, "Downward");
            item.SignedMagnitude = -1f;
            item = AddDestinationMotion(DestinationReviewKind.HopLocalToUi, "HopLocalTo UI - Downward", "Uses a negative anchored hop height while preserving exact landing and scale restoration.", PreviewKind.DestinationUi, "Downward");
            item.SignedMagnitude = -1f;
            item = AddDestinationMotion(DestinationReviewKind.MultiHopTo3D, "MultiHopTo 3D - Downward", "Runs three diminishing negative world-space hops before landing exactly.", PreviewKind.DestinationWorld, "Downward");
            item.SignedMagnitude = -1f;
            item = AddDestinationMotion(DestinationReviewKind.MultiHopLocalToUi, "MultiHopLocalTo UI - Downward", "Runs three diminishing negative anchored hops before landing exactly.", PreviewKind.DestinationUi, "Downward");
            item.SignedMagnitude = -1f;
            item = AddDestinationMotion(DestinationReviewKind.SpiralTo3D, "SpiralTo 3D - Reverse Winding", "Uses negative revolutions to reverse the world-space spiral winding.", PreviewKind.DestinationWorld, "ReverseWinding");
            item.SignedMagnitude = -1f;
            item = AddDestinationMotion(DestinationReviewKind.SpiralLocalToUi, "SpiralLocalTo UI - Reverse Winding", "Uses negative revolutions to reverse the anchored spiral winding.", PreviewKind.DestinationUi, "ReverseWinding");
            item.SignedMagnitude = -1f;
        }

        private void AddUISequenceReviewCoverage()
        {
            AddUISequenceDirectionVariant(UISequenceReviewKind.ToastShow, UISequenceDirection.Down, "Begins above the authored position and travels down into view.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.ToastShow, UISequenceDirection.Left, "Begins to the right and travels left into view.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.ToastShow, UISequenceDirection.Right, "Begins to the left and travels right into view.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.ToastHide, UISequenceDirection.Down, "Anticipates before traveling down and fading out.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.ToastHide, UISequenceDirection.Left, "Anticipates before traveling left and fading out.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.ToastHide, UISequenceDirection.Right, "Anticipates before traveling right and fading out.");

            AddUISequenceDirectionVariant(UISequenceReviewKind.TooltipShow, UISequenceDirection.Down, "Moves down, scales, and fades into its authored position.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.TooltipShow, UISequenceDirection.Left, "Moves left, scales, and fades into its authored position.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.TooltipShow, UISequenceDirection.Right, "Moves right, scales, and fades into its authored position.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.TooltipHide, UISequenceDirection.Down, "Moves down and fades out with restrained scale motion.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.TooltipHide, UISequenceDirection.Left, "Moves left and fades out with restrained scale motion.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.TooltipHide, UISequenceDirection.Right, "Moves right and fades out with restrained scale motion.");

            AddUISequenceDirectionVariant(UISequenceReviewKind.TabSwitch, UISequenceDirection.Up, "Moves outgoing content up while incoming content arrives from below.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.TabSwitch, UISequenceDirection.Down, "Moves outgoing content down while incoming content arrives from above.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.TabSwitch, UISequenceDirection.Right, "Moves outgoing content right while incoming content arrives from the left.");

            AddUISequenceDirectionVariant(UISequenceReviewKind.DrawerShow, UISequenceDirection.Up, "Enters from the top edge and settles on the authored drawer position.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.DrawerShow, UISequenceDirection.Down, "Enters from the bottom edge and settles on the authored drawer position.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.DrawerShow, UISequenceDirection.Right, "Enters from the right edge while fading in the optional backdrop.", true);
            AddUISequenceDirectionVariant(UISequenceReviewKind.DrawerHide, UISequenceDirection.Up, "Exits through the top edge while fading out.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.DrawerHide, UISequenceDirection.Down, "Exits through the bottom edge while fading out.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.DrawerHide, UISequenceDirection.Right, "Exits through the right edge while dismissing the optional backdrop.", true);

            AddUISequenceDirectionVariant(UISequenceReviewKind.PagePush, UISequenceDirection.Up, "Pushes the outgoing page up while the incoming page arrives from below.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.PagePush, UISequenceDirection.Down, "Pushes the outgoing page down while the incoming page arrives from above.");
            AddUISequenceDirectionVariant(UISequenceReviewKind.PagePush, UISequenceDirection.Right, "Pushes the outgoing page right for the back-navigation review path.");
        }

        private void AddTextValueReviewCoverage()
        {
            AddTextDirectionVariants(TextValueReviewKind.TextCharacterStaggerIn, "Character Stagger In", "Reveals visible characters with directional movement, alpha, and scale from");
            AddTextDirectionVariants(TextValueReviewKind.TextCharacterStaggerOut, "Character Stagger Out", "Hides visible characters in reverse order with directional movement toward");
            AddTextDirectionVariants(TextValueReviewKind.TextWave, "Text Wave", "Sends a finite character wave toward");
            AddTextDirectionVariants(TextValueReviewKind.TextCharacterBounce, "Character Bounce", "Sends a finite traveling character bounce toward");
            AddTextDirectionVariants(TextValueReviewKind.TextEmphasis, "Text Emphasis", "Moves and emphasizes the selected character range toward");

            AddTextValueAnimation(TextValueReviewKind.TextCharacterStaggerIn, "Reveals a world TextMesh Pro mesh with upward per-character displacement and exact restoration.", PreviewKind.WorldTextValue, UISequenceDirection.Up, "World", "World TMP Character Stagger In");
            AddTextValueAnimation(TextValueReviewKind.TextColorSweep, "Sweeps world TextMesh Pro vertex colors and restores the exact mesh baseline.", PreviewKind.WorldTextValue, UISequenceDirection.Up, "World", "World TMP Color Sweep");
            AddTextValueAnimation(TextValueReviewKind.TextScrambleReveal, "Resolves a world TextMesh Pro source string without damaging its final content.", PreviewKind.WorldTextValue, UISequenceDirection.Up, "World", "World TMP Scramble Reveal");
        }

        private void AddFeedbackReviewCoverage()
        {
            AddFeedbackSequence(FeedbackReviewKind.HealReceive, "Heal Receive UI", "Communicates UI healing with anchored lift, stretch, pulse, and a green flash.", PreviewKind.DestinationUi);
            ReviewItem item = AddFeedbackSequence(FeedbackReviewKind.ShieldBlock, "Shield Block UI", "Covers the UI target branch with impact and recoil opposite the world review direction.", PreviewKind.DestinationUi);
            item.ReverseDirection = true;
            item = AddFeedbackSequence(FeedbackReviewKind.CriticalHit, "Critical Hit UI", "Covers the UI target branch with a reversed directional recoil and decaying aftershock.", PreviewKind.DestinationUi);
            item.ReverseDirection = true;
            AddFeedbackSequence(FeedbackReviewKind.CooldownReady, "Cooldown Ready 3D", "Announces a world-space ready ability with a relative flip, pop, lift, and cyan flash.", PreviewKind.DestinationWorld);
            AddFeedbackSequence(FeedbackReviewKind.LevelUp, "Level Up 3D", "Celebrates world-space progression with lift, relative spin, staged pulses, and gold flash.", PreviewKind.DestinationWorld);
            AddFeedbackSequence(FeedbackReviewKind.LowHealthWarning, "Low Health Warning 3D", "Plays one finite world-space double-beat warning cycle for caller-controlled looping.", PreviewKind.DestinationWorld);
        }

        private void AddCameraReviewCoverage()
        {
            ReviewItem item = AddCameraFeedback(CameraFeedbackReviewKind.FovKick, "Camera FOV Kick In", "Narrows the field of view quickly and restores the exact captured value.", "In");
            item.SignedMagnitude = -1f;
        }

        private void AddUISequenceDirectionVariant(UISequenceReviewKind kind, UISequenceDirection direction, string description, bool useBackdrop = false)
        {
            AddUISequence(kind, description, direction, direction.ToString(), $"{(kind == UISequenceReviewKind.TabSwitch ? "Tab Switch" : SplitPascalCase(kind.ToString()))} {direction}", useBackdrop);
        }

        private void AddTextDirectionVariants(TextValueReviewKind kind, string operationName, string descriptionPrefix)
        {
            UISequenceDirection[] directions = { UISequenceDirection.Down, UISequenceDirection.Left, UISequenceDirection.Right };
            for (int i = 0; i < directions.Length; i++)
            {
                UISequenceDirection direction = directions[i];
                AddTextValueAnimation(kind, $"{descriptionPrefix} {direction.ToString().ToLowerInvariant()} and restores the exact text state.", PreviewKind.TextValue, direction, direction.ToString(), $"{operationName} {direction}");
            }
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

        private ReviewItem AddCollectionRecipe(CollectionReviewKind kind, string description, PreviewKind preview, string variantKey = null, string name = null)
        {
            return AddCollectionItem(kind, description, preview, ReviewKind.CollectionRecipe, variantKey, name);
        }

        private ReviewItem AddStaggerVariant(CollectionReviewKind kind, string description, PreviewKind preview, string variantKey = null, string name = null)
        {
            return AddCollectionItem(kind, description, preview, ReviewKind.StaggerVariant, variantKey, name);
        }

        private ReviewItem AddCollectionItem(CollectionReviewKind kind, string description, PreviewKind preview, ReviewKind reviewKind, string variantKey, string name)
        {
            var item = new ReviewItem
            {
                Id = BuildReviewId("Collection", kind.ToString(), variantKey),
                Name = name ?? kind.ToString(),
                Description = description,
                VariantKey = variantKey,
                Kind = reviewKind,
                Preview = preview,
                CollectionKind = kind
            };
            _allItems.Add(item);
            return item;
        }

        private ReviewItem AddDestinationMotion(DestinationReviewKind kind, string name, string description, PreviewKind preview, string variantKey = null)
        {
            var item = new ReviewItem
            {
                Id = BuildReviewId("Destination", kind.ToString(), variantKey),
                Name = name,
                Description = description,
                VariantKey = variantKey,
                Kind = ReviewKind.DestinationMotion,
                Preview = preview,
                DestinationKind = kind
            };
            _allItems.Add(item);
            return item;
        }

        private ReviewItem AddFeedbackSequence(FeedbackReviewKind kind, string name, string description, PreviewKind preview)
        {
            string variant = preview == PreviewKind.DestinationUi ? "UI" : preview == PreviewKind.WorldToUi ? "WorldToUI" : "World";
            var item = new ReviewItem
            {
                Id = $"Feedback:{kind}:{variant}",
                Name = name,
                Description = description,
                VariantKey = variant,
                Kind = ReviewKind.FeedbackSequence,
                Preview = preview,
                FeedbackKind = kind
            };
            _allItems.Add(item);
            return item;
        }

        private ReviewItem AddGameplayState(FeedbackReviewKind kind, string name, string description, PreviewKind preview)
        {
            ReviewItem item = AddFeedbackSequence(kind, name, description, preview);
            item.Kind = ReviewKind.GameplayState;
            item.Id = BuildReviewId("GameplayState", kind.ToString(), preview.ToString());
            return item;
        }

        private ReviewItem AddSequenceMacro(SequenceMacroReviewKind kind, string name, string description, PreviewKind preview)
        {
            var item = new ReviewItem
            {
                Id = BuildReviewId("SequenceMacro", kind.ToString(), preview.ToString()),
                Name = name,
                Description = description,
                Kind = ReviewKind.SequenceMacro,
                Preview = preview,
                SequenceMacroKind = kind
            };
            _allItems.Add(item);
            return item;
        }

        private ReviewItem AddUISequence(UISequenceReviewKind kind, string description, UISequenceDirection? direction = null, string variantKey = null, string name = null, bool useBackdrop = false)
        {
            var item = new ReviewItem
            {
                Id = BuildReviewId("UISequence", kind.ToString(), variantKey),
                Name = name ?? (kind == UISequenceReviewKind.TabSwitch ? "Tab Switch" : SplitPascalCase(kind.ToString())),
                Description = description,
                VariantKey = variantKey,
                Kind = ReviewKind.UISequence,
                Preview = PreviewKind.UISequence,
                UISequenceKind = kind,
                Direction = direction ?? GetDefaultUISequenceDirection(kind),
                UseBackdrop = useBackdrop
            };
            _allItems.Add(item);
            return item;
        }

        private ReviewItem AddTextValueAnimation(TextValueReviewKind kind, string description, PreviewKind preview = PreviewKind.TextValue, UISequenceDirection direction = UISequenceDirection.Up, string variantKey = null, string name = null)
        {
            var item = new ReviewItem
            {
                Id = BuildReviewId("TextValue", kind.ToString(), variantKey),
                Name = name ?? SplitPascalCase(kind.ToString()),
                Description = description,
                VariantKey = variantKey,
                Kind = ReviewKind.TextValueAnimation,
                Preview = preview,
                TextValueKind = kind,
                Direction = direction
            };
            _allItems.Add(item);
            return item;
        }

        private ReviewItem AddProgressAnimation(ProgressReviewKind kind, string name, string description)
        {
            var item = new ReviewItem
            {
                Id = BuildReviewId("Progress", kind.ToString(), null),
                Name = name,
                Description = description,
                Kind = ReviewKind.ProgressAnimation,
                Preview = PreviewKind.Progress,
                ProgressKind = kind
            };
            _allItems.Add(item);
            return item;
        }

        private ReviewItem AddCameraFeedback(CameraFeedbackReviewKind kind, string name, string description, string variantKey = null)
        {
            var item = new ReviewItem
            {
                Id = BuildReviewId("CameraFeedback", kind.ToString(), variantKey),
                Name = name,
                Description = description,
                VariantKey = variantKey,
                Kind = ReviewKind.CameraFeedback,
                Preview = PreviewKind.CameraFeedback,
                CameraFeedbackKind = kind
            };
            _allItems.Add(item);
            return item;
        }

        private ReviewItem AddEngineProperty(EnginePropertyReviewKind kind, string name, string description)
        {
            var item = new ReviewItem
            {
                Id = BuildReviewId("EngineProperty", kind.ToString(), null),
                Name = name,
                Description = description,
                Kind = ReviewKind.EngineProperty,
                Preview = PreviewKind.EngineProperty,
                EnginePropertyKind = kind
            };
            _allItems.Add(item);
            return item;
        }

        private static string BuildReviewId(string category, string semanticName, string variantKey)
            => string.IsNullOrEmpty(variantKey) ? $"{category}:{semanticName}" : $"{category}:{semanticName}:{variantKey}";

        private static UISequenceDirection GetDefaultUISequenceDirection(UISequenceReviewKind kind)
        {
            if (kind == UISequenceReviewKind.TabSwitch || kind == UISequenceReviewKind.DrawerShow || kind == UISequenceReviewKind.DrawerHide || kind == UISequenceReviewKind.PagePush) return UISequenceDirection.Left;
            return UISequenceDirection.Up;
        }

        public void ReplayCurrent()
        {
            if (_items.Count == 0) return;
            StopPlayback();
            ResetTargets();
            if (CurrentItem.UsesCollectionPreview)
            {
                _activeTween = PlayCollection(CurrentItem);
                return;
            }

            if (CurrentItem.UsesDestinationPreview)
            {
                if (CurrentItem.Kind == ReviewKind.DestinationMotion) _activeTween = PlayDestinationMotion(CurrentItem);
                else if (CurrentItem.Kind == ReviewKind.SequenceMacro) _activeTween = PlaySequenceMacro(CurrentItem);
                else _activeTween = PlayFeedbackSequence(CurrentItem);
                return;
            }

            if (CurrentItem.UsesUISequencePreview)
            {
                _activeTween = PlayUISequence(CurrentItem);
                return;
            }

            if (CurrentItem.UsesTextValuePreview)
            {
                _activeTween = PlayTextValueAnimation(CurrentItem);
                return;
            }

            if (CurrentItem.UsesProgressPreview)
            {
                _activeTween = PlayProgressAnimation(CurrentItem);
                return;
            }

            if (CurrentItem.UsesCameraFeedbackPreview)
            {
                _activeTween = PlayCameraFeedback(CurrentItem);
                return;
            }

            if (CurrentItem.UsesEnginePropertyPreview)
            {
                _activeTween = PlayEngineProperty(CurrentItem);
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
            worldCollectionPreviewRoot.SetActive(false);
            destinationWorldRoot.SetActive(false);
            destinationUiRoot.SetActive(false);
            uiSequencePreviewRoot.SetActive(false);
            textValuePreviewRoot.SetActive(false);
            worldTextValuePreviewRoot.SetActive(false);
            progressPreviewRoot.SetActive(false);
            enginePropertyPreviewRoot.SetActive(false);
            enginePropertyWorldRoot.SetActive(false);
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
            float? strength = CurrentItem.UsesUiTarget ? PresetReviewCompatibility.GetCanvasPreviewStrength(preset) : null;
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

        private TweenHandle PlayCollection(ReviewItem item)
        {
            CollectionReviewKind kind = item.CollectionKind;
            GameObject[] targets = GetCollectionTargets(item.Preview);
            GameObject owner = GetCollectionOwner(item.Preview);
            switch (kind)
            {
                case CollectionReviewKind.ListStaggerIn:
                    return targets.ListStaggerIn(owner);
                case CollectionReviewKind.ListStaggerOut:
                    return targets.ListStaggerOut(owner);
                case CollectionReviewKind.GridWave:
                    return targets.GridWave(owner, item.GridColumns);
                case CollectionReviewKind.GridRipple:
                    return targets.GridRipple(owner, item.GridColumns, item.OriginIndex);
                case CollectionReviewKind.LoadingDots:
                    return targets.LoadingDots(owner);
                case CollectionReviewKind.OrderFirstToLast:
                    return PlayOrder(targets, owner, StaggerOrder.FirstToLast);
                case CollectionReviewKind.OrderLastToFirst:
                    return PlayOrder(targets, owner, StaggerOrder.LastToFirst);
                case CollectionReviewKind.OrderFromCenter:
                    return PlayOrder(targets, owner, StaggerOrder.FromCenter);
                case CollectionReviewKind.OrderToCenter:
                    return PlayOrder(targets, owner, StaggerOrder.ToCenter);
                case CollectionReviewKind.OrderRandom:
                    return PlayOrder(targets, owner, StaggerOrder.Random, 1729);
                case CollectionReviewKind.GridWaveRightToLeft:
                    return targets.GridWave(owner, item.GridColumns, GridWaveDirection.RightToLeft);
                case CollectionReviewKind.GridWaveTopToBottom:
                    return targets.GridWave(owner, item.GridColumns, GridWaveDirection.TopToBottom);
                case CollectionReviewKind.GridWaveBottomToTop:
                    return targets.GridWave(owner, item.GridColumns, GridWaveDirection.BottomToTop);
                case CollectionReviewKind.GridDiagonalWave:
                    return targets.GridDiagonalWave(owner, item.GridColumns, item.DiagonalDirection, 0.34f, 0.085f);
                case CollectionReviewKind.GridSpiral:
                    return targets.GridSpiral(owner, item.GridColumns, item.SpiralDirection, 0.32f, 0.07f);
                case CollectionReviewKind.GridCheckerboard:
                    return targets.GridCheckerboard(owner, item.GridColumns, item.Inverted, 0.4f, 0.2f);
                case CollectionReviewKind.CollectionBurstIn:
                    return targets.CollectionBurstIn(owner, Vector3.zero, 0.58f, 0.055f, item.Preview != PreviewKind.WorldCollection);
                case CollectionReviewKind.CollectionBurstOut:
                {
                    float? distance = item.UseDefaultDistance ? null : 170f;
                    return targets.CollectionBurstOut(owner, Vector3.zero, distance, 0.52f, 0.045f, item.Preview != PreviewKind.WorldCollection);
                }
                case CollectionReviewKind.CollectionGatherTo:
                    return targets.CollectionGatherTo(owner, Vector3.zero, 0.62f, 0.055f, item.Preview != PreviewKind.WorldCollection);
                case CollectionReviewKind.GridConcentricIn:
                    return targets.GridConcentricIn(owner, item.GridColumns, 0.34f, 0.085f);
                case CollectionReviewKind.GridConcentricOut:
                    return targets.GridConcentricOut(owner, item.GridColumns, 0.32f, 0.075f);
                case CollectionReviewKind.GridQuadrantSweep:
                    return targets.GridQuadrantSweep(owner, item.GridColumns, GridQuadrantSweepDirection.ClockwiseFromTopLeft, 0.34f, 0.16f);
                case CollectionReviewKind.ListAccordion:
                    return targets.ListAccordion(owner, 0.5f, 0.055f);
                case CollectionReviewKind.CollectionOrbitIn:
                    return targets.CollectionOrbitIn(owner, Vector3.zero, null, 0.72f, 0.045f);
                case CollectionReviewKind.CollectionOrbitOut:
                    return targets.CollectionOrbitOut(owner, Vector3.zero, null, 0.68f, 0.04f);
                case CollectionReviewKind.LoadingRing:
                    return targets.LoadingRing(owner, 18f, 1.1f);
                case CollectionReviewKind.LoadingRibbon:
                    return targets.LoadingRibbon(owner, 22f, 1.2f);
                case CollectionReviewKind.StaggerPresetByName:
                    return targets.TweenStagger(owner)
                        .PresetByName("PulseScaleSoft", 0.38f)
                        .Order(StaggerOrder.FromCenter)
                        .DelayBetween(0.12f)
                        .Play();
                case CollectionReviewKind.StaggerCustomAnimate:
                    return targets.TweenStagger(owner)
                        .Animate((target, index) =>
                        {
                            float direction = index % 2 == 0 ? 1f : -1f;
                            var sequence = DOTween.Sequence();
                            sequence.Join(target.transform.DOPunchScale(Vector3.one * 0.16f, 0.42f, 6, 0.55f));
                            sequence.Join(target.transform.DOPunchRotation(new Vector3(0f, 0f, direction * 14f), 0.42f, 6, 0.55f));
                            return sequence;
                        })
                        .Order(StaggerOrder.FirstToLast)
                        .DelayBetween(0.1f)
                        .Play();
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown collection review item.");
            }
        }

        private TweenHandle PlayOrder(GameObject[] targets, GameObject owner, StaggerOrder order, int seed = 0)
        {
            return targets.TweenStagger(owner)
                .Preset<PulseScalePreset>(0.36f)
                .Order(order)
                .DelayBetween(0.14f)
                .Seed(seed)
                .Play();
        }

        private GameObject[] GetCollectionTargets(PreviewKind preview)
        {
            switch (preview)
            {
                case PreviewKind.List: return listTargets;
                case PreviewKind.Grid: return gridTargets;
                case PreviewKind.IncompleteGrid: return incompleteGridTargets;
                case PreviewKind.WorldCollection: return worldCollectionTargets;
                case PreviewKind.LoadingDots: return loadingDotTargets;
                default: throw new ArgumentOutOfRangeException(nameof(preview), preview, "Preview does not contain a collection fixture.");
            }
        }

        private GameObject GetCollectionOwner(PreviewKind preview)
        {
            switch (preview)
            {
                case PreviewKind.List: return listPreviewGroup;
                case PreviewKind.Grid: return gridPreviewGroup;
                case PreviewKind.IncompleteGrid: return incompleteGridPreviewGroup;
                case PreviewKind.WorldCollection: return worldCollectionPreviewRoot;
                case PreviewKind.LoadingDots: return loadingDotsPreviewGroup;
                default: throw new ArgumentOutOfRangeException(nameof(preview), preview, "Preview does not contain a collection owner.");
            }
        }

        private TweenHandle PlayDestinationMotion(ReviewItem item)
        {
            DestinationReviewKind kind = item.DestinationKind;
            bool usesProjectedUi = item.Preview == PreviewKind.WorldToUi;
            bool usesUi = item.Preview == PreviewKind.DestinationUi || usesProjectedUi;
            GameObject target = usesUi ? destinationUiTarget : destinationWorldTarget;
            Vector3 start = usesProjectedUi ? destinationWorldStartMarker.position : usesUi ? destinationUiStartMarker.anchoredPosition3D : destinationWorldStartMarker.position;
            Vector3 destination = usesUi ? destinationUiEndMarker.anchoredPosition3D : destinationWorldEndMarker.position;
            float height = (usesUi ? DestinationUiArcHeight : DestinationWorldArcHeight) * item.SignedMagnitude;
            target.transform.localScale = usesUi ? _destinationUiSnapshot.LocalScale : _destinationWorldSnapshot.LocalScale;

            if (usesProjectedUi)
            {
                Vector3 controlA = start + new Vector3(-0.7f, 2.4f, 0f);
                Vector3 controlB = start + new Vector3(1.8f, 1.2f, 0f);
                Vector3[] waypoints =
                {
                    start + new Vector3(-0.9f, 1.9f, 0f),
                    start + new Vector3(1.3f, 2.5f, 0f),
                    start + new Vector3(2.4f, 0.7f, 0f)
                };
                switch (kind)
                {
                    case DestinationReviewKind.ArcToUiProjected:
                        return target.Tween().ArcToUI(start, destinationUiEndMarker, DestinationUiArcHeight, 1.45f, feedbackCamera).Play();
                    case DestinationReviewKind.HopToUiProjected:
                        return target.Tween().HopToUI(start, destinationUiEndMarker, DestinationUiArcHeight, 1.5f, feedbackCamera).Play();
                    case DestinationReviewKind.BezierToUiProjected:
                        return target.Tween().BezierToUI(start, controlA, controlB, destinationUiEndMarker, 1.55f, feedbackCamera).Play();
                    case DestinationReviewKind.PathThroughUiProjected:
                        return target.Tween().PathThroughUI(start, waypoints, destinationUiEndMarker, item.PathInterpolation, 1.7f, feedbackCamera).Play();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown world-to-UI destination review item.");
                }
            }

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
                    return target.Tween().PathThrough(GetPathWaypoints(false, start, destination), item.PathInterpolation, 1.65f).Play();
                case DestinationReviewKind.PathLocalThroughUi:
                    return target.Tween().PathLocalThrough(GetPathWaypoints(true, start, destination), item.PathInterpolation, 1.65f).Play();
                case DestinationReviewKind.SpiralTo3D:
                    return target.Tween().SpiralTo(destination, 1.1f, 1.75f * item.SignedMagnitude, 1.65f).Play();
                case DestinationReviewKind.SpiralLocalToUi:
                    return target.Tween().SpiralLocalTo(destination, 92f, 1.75f * item.SignedMagnitude, 1.65f).Play();
                case DestinationReviewKind.MultiHopTo3D:
                    return target.Tween().MultiHopTo(destination, height, 3, 1.15f, 1.65f).Play();
                case DestinationReviewKind.MultiHopLocalToUi:
                    return target.Tween().MultiHopLocalTo(destination, height, 3, 1.15f, 1.65f).Play();
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown destination-motion review item.");
            }
        }

        private TweenHandle PlayFeedbackSequence(ReviewItem item)
        {
            FeedbackReviewKind kind = item.FeedbackKind;
            bool usesProjectedUi = item.Preview == PreviewKind.WorldToUi;
            bool usesUi = item.Preview == PreviewKind.DestinationUi || usesProjectedUi;
            GameObject target = usesUi ? destinationUiTarget : destinationWorldTarget;
            Vector3 start = usesProjectedUi ? destinationWorldStartMarker.position : usesUi ? destinationUiStartMarker.anchoredPosition3D : destinationWorldStartMarker.position;
            Vector3 destination = usesUi ? destinationUiEndMarker.anchoredPosition3D : destinationWorldEndMarker.position;
            Vector3 previewPosition = kind == FeedbackReviewKind.PickupCollect ? start : Vector3.Lerp(start, destination, 0.5f);
            target.transform.localScale = usesUi ? _destinationUiSnapshot.LocalScale : _destinationWorldSnapshot.LocalScale;

            if (usesProjectedUi) return target.PickupCollectToUI(start, destinationUiEndMarker, DestinationUiArcHeight, 1.45f, feedbackCamera);
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
                    return target.ShieldBlock(item.ReverseDirection ? Vector3.left : Vector3.right, 0.8f);
                case FeedbackReviewKind.CriticalHit:
                    return target.CriticalHit(item.ReverseDirection ? new Vector3(-1f, 0.2f, 0f) : new Vector3(1f, -0.2f, 0f), 0.9f);
                case FeedbackReviewKind.CooldownReady:
                    return target.CooldownReady(1f);
                case FeedbackReviewKind.LevelUp:
                    return target.LevelUp(1.35f);
                case FeedbackReviewKind.LowHealthWarning:
                    return target.LowHealthWarning(1.1f);
                case FeedbackReviewKind.AbilityCharging:
                    return target.AbilityCharging(1.35f);
                case FeedbackReviewKind.AbilityReady:
                    return target.AbilityReady(1f);
                case FeedbackReviewKind.DodgeRoll:
                    return target.DodgeRoll(1.1f);
                case FeedbackReviewKind.StunStart:
                    return target.StunStart(0.95f);
                case FeedbackReviewKind.StunEnd:
                    return target.StunEnd(0.9f);
                case FeedbackReviewKind.BuffApplied:
                    return target.BuffApplied(1f);
                case FeedbackReviewKind.DebuffApplied:
                    return target.DebuffApplied(1f);
                case FeedbackReviewKind.ResourceDepleted:
                    return target.ResourceDepleted(1f);
                case FeedbackReviewKind.ResourceRecovered:
                    return target.ResourceRecovered(1f);
                case FeedbackReviewKind.ObjectiveUnlocked:
                    return target.ObjectiveUnlocked(1.25f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown feedback review item.");
            }
        }

        private TweenHandle PlaySequenceMacro(ReviewItem item)
        {
            bool usesUi = item.Preview == PreviewKind.DestinationUi;
            GameObject target = usesUi ? destinationUiTarget : destinationWorldTarget;
            Vector3 previewPosition = usesUi
                ? Vector3.Lerp(destinationUiStartMarker.anchoredPosition3D, destinationUiEndMarker.anchoredPosition3D, 0.5f)
                : Vector3.Lerp(destinationWorldStartMarker.position, destinationWorldEndMarker.position, 0.5f);
            target.transform.localScale = usesUi ? _destinationUiSnapshot.LocalScale : _destinationWorldSnapshot.LocalScale;
            if (usesUi) ((RectTransform)target.transform).anchoredPosition3D = previewPosition;
            else target.transform.position = previewPosition;

            switch (item.SequenceMacroKind)
            {
                case SequenceMacroReviewKind.CriticalHit:
                    return target.CriticalHitSequence(new Vector3(1f, -0.2f, 0f), 1.05f);
                case SequenceMacroReviewKind.RewardReveal:
                    return target.RewardRevealSequence(1.4f);
                case SequenceMacroReviewKind.WarningLoop:
                    return target.WarningLoopSequence(1.2f);
                case SequenceMacroReviewKind.CutsceneUiEntrance:
                    return target.CutsceneUIEntranceSequence(1.35f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(item.SequenceMacroKind), item.SequenceMacroKind, "Unknown sequence-macro review item.");
            }
        }

        private TweenHandle PlayUISequence(ReviewItem item)
        {
            UISequenceReviewKind kind = item.UISequenceKind;
            UISequenceDirection direction = item.Direction;
            switch (kind)
            {
                case UISequenceReviewKind.ToastShow:
                    return toastSequenceTarget.ToastShow(direction, 70f, 0.56f);
                case UISequenceReviewKind.ToastHide:
                    return toastSequenceTarget.ToastHide(direction, 70f, 0.42f);
                case UISequenceReviewKind.ModalOpen:
                    return modalSequencePanel.ModalOpen(modalSequenceBackdrop, modalSequenceControls, 0.72f, 0.08f);
                case UISequenceReviewKind.ModalClose:
                    return modalSequencePanel.ModalClose(modalSequenceBackdrop, modalSequenceControls, 0.62f, 0.08f);
                case UISequenceReviewKind.TooltipShow:
                    return tooltipSequenceTarget.TooltipShow(direction, 24f, 0.4f);
                case UISequenceReviewKind.TooltipHide:
                    return tooltipSequenceTarget.TooltipHide(direction, 24f, 0.32f);
                case UISequenceReviewKind.DropdownOpen:
                    return dropdownSequencePanel.DropdownOpen(dropdownSequenceEntries, 0.58f, 0.065f);
                case UISequenceReviewKind.DropdownClose:
                    return dropdownSequencePanel.DropdownClose(dropdownSequenceEntries, 0.48f, 0.065f);
                case UISequenceReviewKind.TabSwitch:
                    return tabSequenceOutgoing.TabSwitchTo(tabSequenceIncoming, direction, 120f, 0.62f);
                case UISequenceReviewKind.DrawerShow:
                    return dropdownSequencePanel.DrawerShow(direction, item.UseBackdrop ? drawerSequenceBackdrop : null, 420f, 0.68f);
                case UISequenceReviewKind.DrawerHide:
                    return dropdownSequencePanel.DrawerHide(direction, item.UseBackdrop ? drawerSequenceBackdrop : null, 420f, 0.52f);
                case UISequenceReviewKind.BottomSheetShow:
                    return modalSequencePanel.BottomSheetShow(modalSequenceBackdrop, 460f, 0.76f);
                case UISequenceReviewKind.BottomSheetHide:
                    return modalSequencePanel.BottomSheetHide(modalSequenceBackdrop, 460f, 0.62f);
                case UISequenceReviewKind.PagePush:
                    return tabSequenceOutgoing.PagePushTo(tabSequenceIncoming, direction, 640f, 0.72f);
                case UISequenceReviewKind.PageCrossFade:
                    return tabSequenceOutgoing.PageCrossFadeTo(tabSequenceIncoming, 0.06f, 0.62f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown UI-sequence review item.");
            }
        }

        private TweenHandle PlayTextValueAnimation(ReviewItem item)
        {
            TextValueReviewKind kind = item.TextValueKind;
            bool usesWorldText = item.Preview == PreviewKind.WorldTextValue;
            TMP_Text characterTarget = usesWorldText ? worldCharacterText : characterText;
            float characterDistance = usesWorldText ? 0.65f : 28f;
            float waveAmplitude = usesWorldText ? 0.5f : 22f;
            float bounceAmplitude = usesWorldText ? 0.55f : 24f;
            float emphasisDistance = usesWorldText ? 0.35f : 12f;
            ConfigureTextValuePreview(item);
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
                    return characterTarget.TextCharacterStaggerIn(item.Direction, characterDistance, 0.045f, 1.05f);
                case TextValueReviewKind.TextWave:
                    return characterTarget.TextWave(item.Direction, waveAmplitude, 1, 1.25f);
                case TextValueReviewKind.ScoreIncrease:
                    return scoreText.ScoreIncrease(1200d, 1475d, "N0", 1.2f);
                case TextValueReviewKind.TextCharacterStaggerOut:
                    return characterTarget.TextCharacterStaggerOut(item.Direction, 30f, 0.045f, 1.05f);
                case TextValueReviewKind.TextCharacterBounce:
                    return characterTarget.TextCharacterBounce(item.Direction, bounceAmplitude, 1.2f);
                case TextValueReviewKind.TextColorSweep:
                    return characterTarget.TextColorSweep(new Color(0.18f, 0.9f, 1f), 2.4f, 1.2f);
                case TextValueReviewKind.TextGlitch:
                    return characterTarget.TextGlitch(9f, 1729, 0.9f);
                case TextValueReviewKind.TextEmphasis:
                    return characterTarget.TextEmphasis(item.Direction, emphasisDistance, 0, 9, new Color(1f, 0.7f, 0.12f), 0.9f);
                case TextValueReviewKind.TextScrambleReveal:
                    return characterTarget.TextScrambleReveal(1729, 1.35f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown text/value review item.");
            }
        }

        private TweenHandle PlayProgressAnimation(ReviewItem item)
        {
            progressEventText.text = item.ProgressKind == ProgressReviewKind.ProgressHook ? "WAITING FOR 50%" : string.Empty;
            switch (item.ProgressKind)
            {
                case ProgressReviewKind.ImageFillTo:
                    return progressImage.FillTo(0.85f, 1.15f);
                case ProgressReviewKind.SliderFillTo:
                    return progressSlider.FillTo(0.85f, 1.15f);
                case ProgressReviewKind.ImageFillFromTo:
                    return progressImage.FillFromTo(0.15f, 0.82f, 1.15f);
                case ProgressReviewKind.SliderFillFromTo:
                    return progressSlider.FillFromTo(0.15f, 0.82f, 1.15f);
                case ProgressReviewKind.ImageValueFillTo:
                    return progressImage.ValueFillTo(0.92f, progressValueText, "P0", 1.2f);
                case ProgressReviewKind.SliderValueFillTo:
                    return progressSlider.ValueFillTo(0.92f, progressValueText, "P0", 1.2f);
                case ProgressReviewKind.ImageFillDrain:
                    return progressImage.FillDrain(0.12f, 0.8f);
                case ProgressReviewKind.SliderFillDrain:
                    return progressSlider.FillDrain(0.12f, 0.8f);
                case ProgressReviewKind.ImageFillCharge:
                    return progressImage.FillCharge(0.92f, 1.15f);
                case ProgressReviewKind.SliderFillCharge:
                    return progressSlider.FillCharge(0.92f, 1.15f);
                case ProgressReviewKind.ImageFillAlertPulse:
                    return progressImage.FillAlertPulse(0.3f, 1.15f);
                case ProgressReviewKind.SliderFillAlertPulse:
                    return progressSlider.FillAlertPulse(0.3f, 1.15f);
                case ProgressReviewKind.ImageFillAndText:
                    return progressImage.FillAndText(0.08f, 0.9f, progressValueText, "P0", 1.25f);
                case ProgressReviewKind.SliderFillAndText:
                    return progressSlider.FillAndText(0.08f, 0.9f, progressValueText, "P0", 1.25f);
                case ProgressReviewKind.ProgressHook:
                    return progressImage.FillFromTo(0.08f, 0.94f, 1.35f).OnProgress(0.5f, () => progressEventText.text = "50% HOOK FIRED");
                default:
                    throw new ArgumentOutOfRangeException(nameof(item.ProgressKind), item.ProgressKind, "Unknown progress review item.");
            }
        }

        private void UpdateProgressReadout(ProgressReviewKind kind)
        {
            float value = UsesSliderProgress(kind) ? progressSlider.normalizedValue : progressImage.fillAmount;
            progressValueText.text = value.ToString("P0");
        }

        private TweenHandle PlayCameraFeedback(ReviewItem item)
        {
            CameraFeedbackReviewKind kind = item.CameraFeedbackKind;
            switch (kind)
            {
                case CameraFeedbackReviewKind.Impact:
                    return feedbackCamera.CameraImpact(0.24f, 3.2f, 0.58f);
                case CameraFeedbackReviewKind.Recoil:
                    return feedbackCamera.CameraRecoil(0.48f, 5.5f, 0.72f);
                case CameraFeedbackReviewKind.LandingImpact:
                    return feedbackCamera.CameraLandingImpact(0.32f, 4.5f, 0.78f);
                case CameraFeedbackReviewKind.FovKick:
                    return feedbackCamera.CameraFovKick(11f * item.SignedMagnitude, 0.68f);
                case CameraFeedbackReviewKind.FocusZoom:
                    return feedbackCamera.CameraFocusZoom(cameraFocusTarget, 1.8f, 9f, 1.15f);
                case CameraFeedbackReviewKind.Breathing:
                    return feedbackCamera.CameraBreathing(0.06f, 0.55f, 0.75f, 3.2f);
                case CameraFeedbackReviewKind.RackFocus:
                    return feedbackCamera.CameraRackFocus(cameraFocusTarget, 7f, 1.25f);
                case CameraFeedbackReviewKind.CollectLandingKick:
                    return feedbackCamera.CollectLandingCameraKick(0.72f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown camera-feedback review item.");
            }
        }

        private TweenHandle PlayEngineProperty(ReviewItem item)
        {
            switch (item.EnginePropertyKind)
            {
                case EnginePropertyReviewKind.AudioVolume:
                    return engineAudioSource.AudioVolumeTo(0.95f, 1.2f);
                case EnginePropertyReviewKind.AudioPitch:
                    return engineAudioSource.AudioPitchTo(1.6f, 1.2f);
                case EnginePropertyReviewKind.LightIntensity:
                    return engineLight.LightIntensityTo(3.2f, 1.2f);
                case EnginePropertyReviewKind.LightColor:
                    return engineLight.LightColorTo(new Color(0.1f, 0.9f, 1f), 1.2f);
                case EnginePropertyReviewKind.ParticleEmissionRate:
                    return engineParticles.ParticleEmissionRateTo(32f, 1.3f);
                case EnginePropertyReviewKind.MaterialFloat:
                    return engineRenderer.MaterialFloatTo("_Smoothness", 0.95f, 1.2f);
                case EnginePropertyReviewKind.MaterialColor:
                    return engineRenderer.MaterialColorTo("_BaseColor", new Color(1f, 0.22f, 0.65f, 1f), 1.2f);
                case EnginePropertyReviewKind.TorchFlicker:
                    return engineLight.TorchFlicker(0.42f, 2.2f);
                case EnginePropertyReviewKind.ScannerPulse:
                    return engineLight.ScannerPulse(new Color(0.05f, 1f, 0.92f), 2f, 1.4f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(item.EnginePropertyKind), item.EnginePropertyKind, "Unknown engine-property review item.");
            }
        }

        private void UpdateEnginePropertyReadout(EnginePropertyReviewKind kind)
        {
            float normalized;
            string value;
            Color color = ReviewBlueColor;
            switch (kind)
            {
                case EnginePropertyReviewKind.AudioVolume:
                    normalized = engineAudioSource.volume;
                    value = $"VOLUME  {engineAudioSource.volume:0.00}";
                    break;
                case EnginePropertyReviewKind.AudioPitch:
                    normalized = Mathf.InverseLerp(-3f, 3f, engineAudioSource.pitch);
                    value = $"PITCH  {engineAudioSource.pitch:0.00}";
                    break;
                case EnginePropertyReviewKind.LightIntensity:
                case EnginePropertyReviewKind.TorchFlicker:
                    normalized = Mathf.InverseLerp(0f, 3.5f, engineLight.intensity);
                    value = $"INTENSITY  {engineLight.intensity:0.00}";
                    color = engineLight.color;
                    break;
                case EnginePropertyReviewKind.LightColor:
                case EnginePropertyReviewKind.ScannerPulse:
                    normalized = Mathf.InverseLerp(0f, 3.5f, engineLight.intensity);
                    value = $"LIGHT  #{ColorUtility.ToHtmlStringRGB(engineLight.color)}";
                    color = engineLight.color;
                    break;
                case EnginePropertyReviewKind.ParticleEmissionRate:
                    normalized = Mathf.InverseLerp(0f, 32f, engineParticles.emission.rateOverTimeMultiplier);
                    value = $"EMISSION  {engineParticles.emission.rateOverTimeMultiplier:0.0}/s";
                    break;
                case EnginePropertyReviewKind.MaterialFloat:
                {
                    var block = new MaterialPropertyBlock();
                    engineRenderer.GetPropertyBlock(block);
                    float smoothness = block.isEmpty ? engineRenderer.sharedMaterial.GetFloat("_Smoothness") : block.GetFloat("_Smoothness");
                    normalized = smoothness;
                    value = $"SMOOTHNESS  {smoothness:0.00}";
                    break;
                }
                case EnginePropertyReviewKind.MaterialColor:
                {
                    var block = new MaterialPropertyBlock();
                    engineRenderer.GetPropertyBlock(block);
                    color = block.isEmpty ? engineRenderer.sharedMaterial.GetColor("_BaseColor") : block.GetColor("_BaseColor");
                    normalized = 1f;
                    value = $"BASE COLOR  #{ColorUtility.ToHtmlStringRGB(color)}";
                    break;
                }
                default:
                    normalized = 0f;
                    value = string.Empty;
                    break;
            }

            enginePropertyMeter.fillAmount = Mathf.Clamp01(normalized);
            enginePropertyMeter.color = color;
            enginePropertyValueText.text = value;
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
            KillTargets(incompleteGridTargets);
            KillTargetTweens(worldCollectionPreviewRoot);
            KillTargets(worldCollectionTargets);
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
            KillTargetTweens(drawerSequenceBackdrop);
            KillTargetTweens(typewriterText);
            KillTargetTweens(numberText);
            KillTargetTweens(characterText);
            KillTargetTweens(scoreText);
            KillTargetTweens(worldCharacterText);
            KillTargetTweens(progressImage);
            KillTargetTweens(progressSlider);
            KillTargetTweens(feedbackCamera);
            KillTargetTweens(engineAudioSource);
            KillTargetTweens(engineLight);
            KillTargetTweens(engineParticles);
            KillTargetTweens(engineRenderer);
        }

        private void ResetTargets()
        {
            _uiSnapshot.Apply(uiTarget);
            _worldSnapshot.Apply(worldTarget);
            ApplySnapshots(listTargets, _listSnapshots);
            ApplySnapshots(gridTargets, _gridSnapshots);
            ApplySnapshots(incompleteGridTargets, _incompleteGridSnapshots);
            ApplySnapshots(worldCollectionTargets, _worldCollectionSnapshots);
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
            _drawerSequenceBackdropSnapshot.Apply(drawerSequenceBackdrop);
            _typewriterTextSnapshot.Apply(typewriterText);
            _numberTextSnapshot.Apply(numberText);
            _characterTextSnapshot.Apply(characterText);
            _scoreTextSnapshot.Apply(scoreText);
            _worldCharacterTextSnapshot.Apply(worldCharacterText);
            progressImage.fillAmount = _progressImageFill;
            progressSlider.normalizedValue = _progressSliderValue;
            _progressImageSnapshot.Apply(progressImage.gameObject);
            _progressSliderSnapshot.Apply(progressSlider.gameObject);
            progressImage.color = _progressImageColor;
            progressValueText.text = _progressImageFill.ToString("P0");
            progressEventText.text = string.Empty;
            _feedbackCameraSnapshot.Apply(feedbackCamera);
            engineAudioSource.volume = _engineAudioVolume;
            engineAudioSource.pitch = _engineAudioPitch;
            engineLight.intensity = _engineLightIntensity;
            engineLight.color = _engineLightColor;
            ParticleSystem.EmissionModule emission = engineParticles.emission;
            emission.rateOverTimeMultiplier = _engineParticleEmissionRate;
            _engineRendererSnapshot.Apply(engineRenderer.gameObject);
        }

        private void ApplyPreviewVisibility(PreviewKind preview)
        {
            uiTarget.SetActive(preview == PreviewKind.Ui);
            worldTarget.SetActive(preview == PreviewKind.World || preview == PreviewKind.CameraFeedback);
            bool showCollection = preview == PreviewKind.List || preview == PreviewKind.Grid || preview == PreviewKind.IncompleteGrid || preview == PreviewKind.LoadingDots;
            collectionPreviewRoot.SetActive(showCollection);
            listPreviewGroup.SetActive(preview == PreviewKind.List);
            gridPreviewGroup.SetActive(preview == PreviewKind.Grid);
            incompleteGridPreviewGroup.SetActive(preview == PreviewKind.IncompleteGrid);
            worldCollectionPreviewRoot.SetActive(preview == PreviewKind.WorldCollection);
            loadingDotsPreviewGroup.SetActive(preview == PreviewKind.LoadingDots);
            bool showWorldToUi = preview == PreviewKind.WorldToUi;
            destinationWorldRoot.SetActive(preview == PreviewKind.DestinationWorld || showWorldToUi);
            destinationUiRoot.SetActive(preview == PreviewKind.DestinationUi || showWorldToUi);
            destinationWorldTarget.SetActive(preview == PreviewKind.DestinationWorld);
            destinationUiTarget.SetActive(preview == PreviewKind.DestinationUi || showWorldToUi);
            bool showUISequence = preview == PreviewKind.UISequence;
            uiSequencePreviewRoot.SetActive(showUISequence);
            if (showUISequence) ConfigureUISequencePreview(CurrentItem);
            bool showTextValue = preview == PreviewKind.TextValue;
            textValuePreviewRoot.SetActive(showTextValue);
            worldTextValuePreviewRoot.SetActive(preview == PreviewKind.WorldTextValue);
            if (CurrentItem.UsesTextValuePreview) ConfigureTextValuePreview(CurrentItem);
            progressPreviewRoot.SetActive(preview == PreviewKind.Progress);
            if (CurrentItem.UsesProgressPreview) ConfigureProgressPreview(CurrentItem.ProgressKind);
            bool showEngineProperty = preview == PreviewKind.EngineProperty;
            enginePropertyPreviewRoot.SetActive(showEngineProperty);
            enginePropertyWorldRoot.SetActive(showEngineProperty);
            if (showEngineProperty)
            {
                engineParticles.Play(true);
                UpdateEnginePropertyReadout(CurrentItem.EnginePropertyKind);
            }
        }

        private void ConfigureProgressPreview(ProgressReviewKind kind)
        {
            bool usesSlider = UsesSliderProgress(kind);
            progressImageGroup.SetActive(!usesSlider);
            progressSlider.gameObject.SetActive(usesSlider);
            progressValueText.text = (usesSlider ? progressSlider.normalizedValue : progressImage.fillAmount).ToString("P0");
            progressEventText.text = kind == ProgressReviewKind.ProgressHook ? "REPLAY TO TEST CALLBACK" : string.Empty;
        }

        private static bool UsesSliderProgress(ProgressReviewKind kind)
        {
            return kind == ProgressReviewKind.SliderFillTo ||
                   kind == ProgressReviewKind.SliderFillFromTo ||
                   kind == ProgressReviewKind.SliderValueFillTo ||
                   kind == ProgressReviewKind.SliderFillDrain ||
                   kind == ProgressReviewKind.SliderFillCharge ||
                   kind == ProgressReviewKind.SliderFillAlertPulse ||
                   kind == ProgressReviewKind.SliderFillAndText;
        }

        private void ConfigureUISequencePreview(ReviewItem item)
        {
            UISequenceReviewKind kind = item.UISequenceKind;
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
            drawerSequenceBackdrop.SetActive((kind == UISequenceReviewKind.DrawerShow || kind == UISequenceReviewKind.DrawerHide) && item.UseBackdrop);
        }

        private void ConfigureTextValuePreview(ReviewItem item)
        {
            TextValueReviewKind kind = item.TextValueKind;
            bool usesWorldText = item.Preview == PreviewKind.WorldTextValue;
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
            typewriterText.gameObject.SetActive(!usesWorldText && showTypewriter);
            numberText.gameObject.SetActive(!usesWorldText && showNumber);
            characterText.gameObject.SetActive(!usesWorldText && showCharacter);
            scoreText.gameObject.SetActive(!usesWorldText && kind == TextValueReviewKind.ScoreIncrease);
            worldCharacterText.gameObject.SetActive(usesWorldText && showCharacter);
        }

        private void ConfigureDestinationGuides(ReviewItem item)
        {
            bool isDestination = item.Kind == ReviewKind.DestinationMotion;
            bool isPickup = item.Kind == ReviewKind.FeedbackSequence && (item.FeedbackKind == FeedbackReviewKind.PickupCollect || item.FeedbackKind == FeedbackReviewKind.PickupCollectToUi);
            bool isWorldToUi = item.Preview == PreviewKind.WorldToUi;
            bool showMarkers = isDestination || isPickup;
            bool showPath = !isWorldToUi && (isPickup || (isDestination && UsesCurvedPath(item.DestinationKind)));
            destinationWorldStartMarker.gameObject.SetActive(showMarkers && (item.Preview == PreviewKind.DestinationWorld || isWorldToUi));
            destinationWorldEndMarker.gameObject.SetActive(showMarkers && item.Preview == PreviewKind.DestinationWorld);
            destinationUiStartMarker.gameObject.SetActive(showMarkers && item.Preview == PreviewKind.DestinationUi);
            destinationUiEndMarker.gameObject.SetActive(showMarkers && (item.Preview == PreviewKind.DestinationUi || isWorldToUi));
            if (showPath) UpdateDestinationPath(item);
            destinationWorldCurvedPath.SetActive(showPath && item.Preview == PreviewKind.DestinationWorld);
            destinationUiCurvedPath.SetActive(showPath && item.Preview == PreviewKind.DestinationUi);
        }

        private void UpdateDestinationPath(ReviewItem item)
        {
            bool usesUi = item.Preview == PreviewKind.DestinationUi;
            DestinationReviewKind kind = item.Kind == ReviewKind.DestinationMotion ? item.DestinationKind : DestinationReviewKind.ArcTo3D;
            Transform pathRoot = usesUi ? destinationUiCurvedPath.transform : destinationWorldCurvedPath.transform;
            Vector3 start = usesUi ? destinationUiStartMarker.anchoredPosition3D : destinationWorldStartMarker.position;
            Vector3 destination = usesUi ? destinationUiEndMarker.anchoredPosition3D : destinationWorldEndMarker.position;
            float height = (usesUi ? DestinationUiArcHeight : DestinationWorldArcHeight) * item.SignedMagnitude;
            GetBezierControls(usesUi, start, destination, out Vector3 controlA, out Vector3 controlB);
            Vector3[] waypoints = GetPathWaypoints(usesUi, start, destination);

            for (int i = 0; i < pathRoot.childCount; i++)
            {
                float progress = (i + 1f) / (pathRoot.childCount + 1f);
                Vector3 point;
                if (IsBezier(kind)) point = EvaluateBezier(start, controlA, controlB, destination, progress);
                else if (IsPath(kind)) point = EvaluatePath(start, waypoints, progress, item.PathInterpolation);
                else if (IsSpiral(kind)) point = EvaluateSpiral(start, destination, usesUi ? 92f : 1.1f, 1.75f * item.SignedMagnitude, progress, usesUi);
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

        private static Vector3 EvaluatePath(Vector3 start, Vector3[] waypoints, float progress, DestinationPathInterpolation interpolation)
        {
            float scaled = Mathf.Clamp01(progress) * waypoints.Length;
            int segment = Mathf.Min(Mathf.FloorToInt(scaled), waypoints.Length - 1);
            float localProgress = progress >= 1f ? 1f : scaled - segment;
            Vector3 pointA = segment == 0 ? start : waypoints[segment - 1];
            Vector3 pointB = waypoints[segment];
            if (interpolation == DestinationPathInterpolation.Linear) return Vector3.LerpUnclamped(pointA, pointB, localProgress);
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
            if (item.Kind == ReviewKind.DestinationMotion) return item.Preview == PreviewKind.WorldToUi ? "DESTINATION MOTION / WORLD TO UI" : item.Preview == PreviewKind.DestinationUi ? "DESTINATION MOTION / UI" : "DESTINATION MOTION / 3D";
            if (item.Kind == ReviewKind.FeedbackSequence) return item.Preview == PreviewKind.WorldToUi ? "GAMEPLAY FEEDBACK / WORLD TO UI" : item.Preview == PreviewKind.DestinationUi ? "GAMEPLAY FEEDBACK / UI" : "GAMEPLAY FEEDBACK / 3D";
            if (item.Kind == ReviewKind.GameplayState) return item.Preview == PreviewKind.DestinationUi ? "GAMEPLAY STATE / UI" : "GAMEPLAY STATE / 3D";
            if (item.Kind == ReviewKind.SequenceMacro) return item.Preview == PreviewKind.DestinationUi ? "SEQUENCE MACRO / UI" : "SEQUENCE MACRO / 3D";
            if (item.Kind == ReviewKind.UISequence) return "PRODUCTION UI SEQUENCE";
            if (item.Kind == ReviewKind.TextValueAnimation) return item.Preview == PreviewKind.WorldTextValue ? "TEXT & VALUE ANIMATION / WORLD TMP" : "TEXT & VALUE ANIMATION / UI TMP";
            if (item.Kind == ReviewKind.ProgressAnimation) return "PROGRESS & VALUE ANIMATION";
            if (item.Kind == ReviewKind.CameraFeedback) return "CAMERA FEEDBACK";
            if (item.Kind == ReviewKind.EngineProperty) return "ENGINE PROPERTY ANIMATION";
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

        private static void KillTargetTweens(Component target)
        {
            if (target == null) return;
            KillTargetTweens(target.gameObject);
        }
    }
}
