using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper.Demo
{
    public sealed class AnimationGalleryPlayer : MonoBehaviour
    {
        [Header("Routing")]
        [SerializeField] private AnimationGalleryPreviewRouter previewRouter;
        [SerializeField] private GameObject[] resetScopes;

        [Header("Collections")]
        [SerializeField] private GameObject listOwner;
        [SerializeField] private GameObject[] listTargets;
        [SerializeField] private GameObject gridOwner;
        [SerializeField] private GameObject[] gridTargets;
        [SerializeField] private GameObject loadingDotsOwner;
        [SerializeField] private GameObject[] loadingDotTargets;

        [Header("Destination And Feedback")]
        [SerializeField] private GameObject destinationUiTarget;
        [SerializeField] private RectTransform destinationUiStart;
        [SerializeField] private RectTransform destinationUiEnd;
        [SerializeField] private GameObject destinationWorldTarget;
        [SerializeField] private Transform destinationWorldStart;
        [SerializeField] private Transform destinationWorldEnd;

        [Header("UI Sequences")]
        [SerializeField] private GameObject toastTarget;
        [SerializeField] private GameObject modalBackdrop;
        [SerializeField] private GameObject modalPanel;
        [SerializeField] private GameObject[] modalControls;
        [SerializeField] private GameObject tooltipTarget;
        [SerializeField] private GameObject dropdownPanel;
        [SerializeField] private GameObject[] dropdownEntries;
        [SerializeField] private GameObject tabOutgoing;
        [SerializeField] private GameObject tabIncoming;
        [SerializeField] private GameObject drawerBackdrop;

        [Header("Text And Values")]
        [SerializeField] private TMP_Text typewriterText;
        [SerializeField] private TMP_Text numberText;
        [SerializeField] private TMP_Text characterText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text worldCharacterText;

        [Header("Camera")]
        [SerializeField] private Camera previewCamera;
        [SerializeField] private Transform cameraFocusTarget;

        private readonly List<ComponentSnapshot> _snapshots = new List<ComponentSnapshot>();
        private TweenHandle _activeTween;

        private static readonly UISequenceDirection[] Directions =
        {
            UISequenceDirection.Up,
            UISequenceDirection.Down,
            UISequenceDirection.Left,
            UISequenceDirection.Right
        };

        private static readonly StaggerOrder[] Orders =
        {
            StaggerOrder.FirstToLast,
            StaggerOrder.LastToFirst,
            StaggerOrder.FromCenter,
            StaggerOrder.ToCenter,
            StaggerOrder.Random
        };

        private static readonly GridWaveDirection[] GridDirections =
        {
            GridWaveDirection.LeftToRight,
            GridWaveDirection.RightToLeft,
            GridWaveDirection.TopToBottom,
            GridWaveDirection.BottomToTop
        };

        private static readonly GridDiagonalDirection[] DiagonalDirections =
        {
            GridDiagonalDirection.TopLeftToBottomRight,
            GridDiagonalDirection.TopRightToBottomLeft,
            GridDiagonalDirection.BottomLeftToTopRight,
            GridDiagonalDirection.BottomRightToTopLeft
        };

        private static readonly GridSpiralDirection[] SpiralDirections =
        {
            GridSpiralDirection.OutsideInClockwise,
            GridSpiralDirection.OutsideInCounterClockwise,
            GridSpiralDirection.InsideOutClockwise,
            GridSpiralDirection.InsideOutCounterClockwise
        };

        private void Awake()
        {
            CaptureBaseline();
            previewRouter.HideAll();
        }

        private void OnDisable() => StopAndReset();

        public void Play(AnimationGalleryConfiguration configuration)
        {
            StopAndReset();
            previewRouter.Show(configuration);
            _activeTween = PlayInternal(configuration);
        }

        public void ResetPreview(AnimationGalleryConfiguration configuration)
        {
            StopAndReset();
            previewRouter.Show(configuration);
        }

        private TweenHandle PlayInternal(AnimationGalleryConfiguration configuration)
        {
            AnimationGalleryEntry entry = configuration.Entry;
            if (entry.Operation == AnimationGalleryOperation.Preset)
            {
                GameObject target = previewRouter.ResolvePresetTarget(entry);
                return target.Tween().Preset(entry.Preset).Play();
            }

            switch (entry.Category)
            {
                case AnimationGalleryCategory.UIRecipes: return PlayUIRecipe(entry.Operation);
                case AnimationGalleryCategory.Collections: return PlayCollection(configuration);
                case AnimationGalleryCategory.DestinationMotion: return PlayDestination(configuration);
                case AnimationGalleryCategory.GameplayFeedback: return PlayFeedback(configuration);
                case AnimationGalleryCategory.UISequences: return PlayUISequence(configuration);
                case AnimationGalleryCategory.TextAndValues: return PlayTextAndValue(configuration);
                case AnimationGalleryCategory.CameraFeedback: return PlayCamera(configuration);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private TweenHandle PlayUIRecipe(AnimationGalleryOperation operation)
        {
            GameObject target = previewRouter.UiTarget;
            switch (operation)
            {
                case AnimationGalleryOperation.UIAppear: return target.UIAppear();
                case AnimationGalleryOperation.UIAppearSoft: return target.UIAppearSoft();
                case AnimationGalleryOperation.UIDisappear: return target.UIDisappear();
                case AnimationGalleryOperation.UIDisappearSoft: return target.UIDisappearSoft();
                case AnimationGalleryOperation.UIHover: return target.UIHover();
                case AnimationGalleryOperation.UIHoverSoft: return target.UIHoverSoft();
                case AnimationGalleryOperation.UIPress: return target.UIPress();
                case AnimationGalleryOperation.UIPressHard: return target.UIPressHard();
                case AnimationGalleryOperation.UIAttention: return target.UIAttention();
                case AnimationGalleryOperation.UIAttentionSoft: return target.UIAttentionSoft();
                case AnimationGalleryOperation.UIAttentionHard: return target.UIAttentionHard();
                case AnimationGalleryOperation.UIDisabled: return target.UIDisabled();
                case AnimationGalleryOperation.UIEnabled: return target.UIEnabled();
                default: throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }
        }

        private TweenHandle PlayCollection(AnimationGalleryConfiguration configuration)
        {
            int orderIndex = Math.Max(0, configuration.GetIndex(AnimationGalleryOptionKind.Order));
            switch (configuration.Entry.Operation)
            {
                case AnimationGalleryOperation.ListStaggerIn:
                    return listTargets.TweenStagger(listOwner).Preset<PopInFadePreset>(0.42f).Order(Orders[orderIndex]).DelayBetween(0.08f).Seed(1729).Play();
                case AnimationGalleryOperation.ListStaggerOut:
                    return listTargets.TweenStagger(listOwner).Preset<PopOutFadePreset>(0.34f).Order(Orders[orderIndex]).DelayBetween(0.06f).Seed(1729).Play();
                case AnimationGalleryOperation.GridWave:
                    return gridTargets.GridWave(gridOwner, 3, GridDirections[Math.Max(0, configuration.GetIndex(AnimationGalleryOptionKind.GridDirection))]);
                case AnimationGalleryOperation.GridRipple:
                    return gridTargets.GridRipple(gridOwner, 3);
                case AnimationGalleryOperation.LoadingDots:
                    return loadingDotTargets.LoadingDots(loadingDotsOwner, 0.42f, 0.16f, 1);
                case AnimationGalleryOperation.GridDiagonalWave:
                    return gridTargets.GridDiagonalWave(gridOwner, 3, DiagonalDirections[Math.Max(0, configuration.GetIndex(AnimationGalleryOptionKind.DiagonalPattern))]);
                case AnimationGalleryOperation.GridSpiral:
                    return gridTargets.GridSpiral(gridOwner, 3, SpiralDirections[Math.Max(0, configuration.GetIndex(AnimationGalleryOptionKind.SpiralPattern))]);
                case AnimationGalleryOperation.GridCheckerboard:
                    return gridTargets.GridCheckerboard(gridOwner, 3, configuration.GetIndex(AnimationGalleryOptionKind.Phase) == 1);
                case AnimationGalleryOperation.CollectionBurstIn:
                    return gridTargets.CollectionBurstIn(gridOwner, Vector3.zero);
                case AnimationGalleryOperation.CollectionBurstOut:
                    return gridTargets.CollectionBurstOut(gridOwner, Vector3.zero);
                case AnimationGalleryOperation.CollectionGatherTo:
                    return gridTargets.CollectionGatherTo(gridOwner, Vector3.zero);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private TweenHandle PlayDestination(AnimationGalleryConfiguration configuration)
        {
            bool world = IsWorld(configuration);
            GameObject target = world ? destinationWorldTarget : destinationUiTarget;
            Vector3 start = world ? destinationWorldStart.position : destinationUiStart.anchoredPosition3D;
            Vector3 destination = world ? destinationWorldEnd.position : destinationUiEnd.anchoredPosition3D;
            float sign = configuration.GetIndex(AnimationGalleryOptionKind.MotionVariant) == 1 ? -1f : 1f;
            float height = (world ? 2.1f : 175f) * sign;
            if (world) target.transform.position = start;
            else ((RectTransform)target.transform).anchoredPosition3D = start;

            switch (configuration.Entry.Operation)
            {
                case AnimationGalleryOperation.ArcTo: return world ? target.Tween().ArcTo(destination, height).Play() : target.Tween().ArcLocalTo(destination, height).Play();
                case AnimationGalleryOperation.BezierTo:
                    GetBezierControls(world, start, destination, out Vector3 controlA, out Vector3 controlB);
                    return world ? target.Tween().BezierTo(destination, controlA, controlB).Play() : target.Tween().BezierLocalTo(destination, controlA, controlB).Play();
                case AnimationGalleryOperation.HopTo: return world ? target.Tween().HopTo(destination, height).Play() : target.Tween().HopLocalTo(destination, height).Play();
                case AnimationGalleryOperation.SpringTo: return world ? target.Tween().SpringTo(destination).Play() : target.Tween().SpringLocalTo(destination).Play();
                case AnimationGalleryOperation.MagneticSnapTo: return world ? target.Tween().MagneticSnapTo(destination).Play() : target.Tween().MagneticSnapLocalTo(destination).Play();
                case AnimationGalleryOperation.PathThrough:
                    var interpolation = configuration.GetIndex(AnimationGalleryOptionKind.Interpolation) == 0 ? DestinationPathInterpolation.Linear : DestinationPathInterpolation.CatmullRom;
                    Vector3[] waypoints = GetWaypoints(world, start, destination);
                    return world ? target.Tween().PathThrough(waypoints, interpolation).Play() : target.Tween().PathLocalThrough(waypoints, interpolation).Play();
                case AnimationGalleryOperation.SpiralTo: return world ? target.Tween().SpiralTo(destination, 1.1f * sign).Play() : target.Tween().SpiralLocalTo(destination, 92f * sign).Play();
                case AnimationGalleryOperation.MultiHopTo: return world ? target.Tween().MultiHopTo(destination, height, 3).Play() : target.Tween().MultiHopLocalTo(destination, height, 3).Play();
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private TweenHandle PlayFeedback(AnimationGalleryConfiguration configuration)
        {
            bool world = IsWorld(configuration);
            GameObject target = world ? destinationWorldTarget : destinationUiTarget;
            Vector3 start = world ? destinationWorldStart.position : destinationUiStart.anchoredPosition3D;
            Vector3 destination = world ? destinationWorldEnd.position : destinationUiEnd.anchoredPosition3D;
            Vector3 center = Vector3.Lerp(start, destination, 0.5f);
            if (world) target.transform.position = center;
            else ((RectTransform)target.transform).anchoredPosition3D = center;
            bool reverse = configuration.GetIndex(AnimationGalleryOptionKind.ImpactDirection) == 1;

            switch (configuration.Entry.Operation)
            {
                case AnimationGalleryOperation.ErrorReject: return target.ErrorReject();
                case AnimationGalleryOperation.DamageHit: return target.DamageHit();
                case AnimationGalleryOperation.SuccessConfirm: return target.SuccessConfirm();
                case AnimationGalleryOperation.RewardReveal: return target.RewardReveal();
                case AnimationGalleryOperation.PickupCollect:
                    if (world) target.transform.position = start;
                    else ((RectTransform)target.transform).anchoredPosition3D = start;
                    return world ? target.PickupCollectTo(destination) : target.PickupCollectLocalTo(destination);
                case AnimationGalleryOperation.HealReceive: return target.HealReceive();
                case AnimationGalleryOperation.ShieldBlock: return target.ShieldBlock(reverse ? Vector3.left : Vector3.right);
                case AnimationGalleryOperation.CriticalHit: return target.CriticalHit(reverse ? Vector3.left : Vector3.right);
                case AnimationGalleryOperation.CooldownReady: return target.CooldownReady();
                case AnimationGalleryOperation.LevelUp: return target.LevelUp();
                case AnimationGalleryOperation.LowHealthWarning: return target.LowHealthWarning();
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private TweenHandle PlayUISequence(AnimationGalleryConfiguration configuration)
        {
            UISequenceDirection direction = Directions[Math.Max(0, configuration.GetIndex(AnimationGalleryOptionKind.Direction))];
            bool backdrop = configuration.GetIndex(AnimationGalleryOptionKind.Backdrop) != 1;
            switch (configuration.Entry.Operation)
            {
                case AnimationGalleryOperation.ToastShow: return toastTarget.ToastShow(direction);
                case AnimationGalleryOperation.ToastHide: return toastTarget.ToastHide(direction);
                case AnimationGalleryOperation.ModalOpen: return modalPanel.ModalOpen(modalBackdrop, modalControls);
                case AnimationGalleryOperation.ModalClose: return modalPanel.ModalClose(modalBackdrop, modalControls);
                case AnimationGalleryOperation.TooltipShow: return tooltipTarget.TooltipShow(direction);
                case AnimationGalleryOperation.TooltipHide: return tooltipTarget.TooltipHide(direction);
                case AnimationGalleryOperation.DropdownOpen: return dropdownPanel.DropdownOpen(dropdownEntries);
                case AnimationGalleryOperation.DropdownClose: return dropdownPanel.DropdownClose(dropdownEntries);
                case AnimationGalleryOperation.TabSwitch: return tabOutgoing.TabSwitchTo(tabIncoming, direction);
                case AnimationGalleryOperation.DrawerShow: return dropdownPanel.DrawerShow(direction, backdrop ? drawerBackdrop : null);
                case AnimationGalleryOperation.DrawerHide: return dropdownPanel.DrawerHide(direction, backdrop ? drawerBackdrop : null);
                case AnimationGalleryOperation.BottomSheetShow: return modalPanel.BottomSheetShow(modalBackdrop);
                case AnimationGalleryOperation.BottomSheetHide: return modalPanel.BottomSheetHide(modalBackdrop);
                case AnimationGalleryOperation.PagePush: return tabOutgoing.PagePushTo(tabIncoming, direction);
                case AnimationGalleryOperation.PageCrossFade: return tabOutgoing.PageCrossFadeTo(tabIncoming);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private TweenHandle PlayTextAndValue(AnimationGalleryConfiguration configuration)
        {
            bool world = configuration.GetValue(AnimationGalleryOptionKind.TargetContext) == "World";
            TMP_Text target = world ? worldCharacterText : characterText;
            UISequenceDirection direction = Directions[Math.Max(0, configuration.GetIndex(AnimationGalleryOptionKind.Direction))];
            float distance = world ? 0.65f : 28f;
            switch (configuration.Entry.Operation)
            {
                case AnimationGalleryOperation.TypewriterReveal: return typewriterText.TypewriterReveal();
                case AnimationGalleryOperation.TypewriterHide: return typewriterText.TypewriterHide();
                case AnimationGalleryOperation.NumberCountUp: return numberText.NumberCountTo(0d, 1250d, "N0");
                case AnimationGalleryOperation.NumberCountDown: return numberText.NumberCountTo(1250d, 0d, "N0");
                case AnimationGalleryOperation.TextCharacterStaggerIn: return target.TextCharacterStaggerIn(direction, distance);
                case AnimationGalleryOperation.TextWave: return target.TextWave(direction, world ? 0.5f : 22f, 1);
                case AnimationGalleryOperation.ScoreIncrease: return scoreText.ScoreIncrease(1200d, 1475d, "N0");
                case AnimationGalleryOperation.TextCharacterStaggerOut: return target.TextCharacterStaggerOut(direction, distance);
                case AnimationGalleryOperation.TextCharacterBounce: return target.TextCharacterBounce(direction, world ? 0.55f : 24f);
                case AnimationGalleryOperation.TextColorSweep: return target.TextColorSweep(new Color(0.18f, 0.9f, 1f));
                case AnimationGalleryOperation.TextGlitch: return target.TextGlitch(seed: 1729);
                case AnimationGalleryOperation.TextEmphasis: return target.TextEmphasis(direction, world ? 0.35f : 12f, 0, 9, new Color(1f, 0.7f, 0.12f));
                case AnimationGalleryOperation.TextScrambleReveal: return target.TextScrambleReveal(seed: 1729);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private TweenHandle PlayCamera(AnimationGalleryConfiguration configuration)
        {
            switch (configuration.Entry.Operation)
            {
                case AnimationGalleryOperation.CameraImpact: return previewCamera.CameraImpact();
                case AnimationGalleryOperation.CameraRecoil: return previewCamera.CameraRecoil();
                case AnimationGalleryOperation.CameraLandingImpact: return previewCamera.CameraLandingImpact();
                case AnimationGalleryOperation.CameraFovKick: return previewCamera.CameraFovKick(configuration.GetIndex(AnimationGalleryOptionKind.MotionVariant) == 1 ? -11f : 11f);
                case AnimationGalleryOperation.CameraFocusZoom: return previewCamera.CameraFocusZoom(cameraFocusTarget);
                case AnimationGalleryOperation.CameraBreathing: return previewCamera.CameraBreathing(duration: 3.2f);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void CaptureBaseline()
        {
            _snapshots.Clear();
            foreach (GameObject resetScope in resetScopes)
            {
                foreach (Transform target in resetScope.GetComponentsInChildren<Transform>(true)) _snapshots.Add(ComponentSnapshot.Capture(target));
            }
        }

        private void StopAndReset()
        {
            _activeTween?.Kill();
            _activeTween = null;
            foreach (ComponentSnapshot snapshot in _snapshots) snapshot.Apply();
        }

        private static bool IsWorld(AnimationGalleryConfiguration configuration)
            => configuration.GetValue(AnimationGalleryOptionKind.TargetContext) == "World";

        private static void GetBezierControls(bool world, Vector3 start, Vector3 destination, out Vector3 controlA, out Vector3 controlB)
        {
            float heightA = world ? 2.5f : 210f;
            float heightB = world ? 0.8f : 70f;
            controlA = Vector3.Lerp(start, destination, 0.33f) + Vector3.up * heightA;
            controlB = Vector3.Lerp(start, destination, 0.66f) + Vector3.up * heightB;
        }

        private static Vector3[] GetWaypoints(bool world, Vector3 start, Vector3 destination)
        {
            float height = world ? 1.7f : 135f;
            return new[]
            {
                Vector3.Lerp(start, destination, 0.28f) + Vector3.up * height,
                Vector3.Lerp(start, destination, 0.58f) - Vector3.up * height * 0.45f,
                destination
            };
        }

        private sealed class ComponentSnapshot
        {
            private readonly Transform _transform;
            private readonly Vector3 _position;
            private readonly Quaternion _rotation;
            private readonly Vector3 _scale;
            private readonly bool _active;
            private readonly Graphic _graphic;
            private readonly Color _graphicColor;
            private readonly CanvasGroup _canvasGroup;
            private readonly float _alpha;
            private readonly TMP_Text _text;
            private readonly string _content;
            private readonly int _maxVisibleCharacters;
            private readonly Renderer _renderer;
            private readonly Color _rendererColor;
            private readonly Camera _camera;
            private readonly float _fieldOfView;

            private ComponentSnapshot(Transform transform)
            {
                _transform = transform;
                _position = transform.localPosition;
                _rotation = transform.localRotation;
                _scale = transform.localScale;
                _active = transform.gameObject.activeSelf;
                _graphic = transform.GetComponent<Graphic>();
                _graphicColor = _graphic == null ? Color.white : _graphic.color;
                _canvasGroup = transform.GetComponent<CanvasGroup>();
                _alpha = _canvasGroup == null ? 1f : _canvasGroup.alpha;
                _text = transform.GetComponent<TMP_Text>();
                _content = _text == null ? string.Empty : _text.text;
                _maxVisibleCharacters = _text == null ? 0 : _text.maxVisibleCharacters;
                _renderer = transform.GetComponent<Renderer>();
                _rendererColor = _renderer == null ? Color.white : _renderer.material.color;
                _camera = transform.GetComponent<Camera>();
                _fieldOfView = _camera == null ? 60f : _camera.fieldOfView;
            }

            public static ComponentSnapshot Capture(Transform transform) => new ComponentSnapshot(transform);

            public void Apply()
            {
                DOTween.Kill(_transform);
                DOTween.Kill(_transform.gameObject);
                _transform.localPosition = _position;
                _transform.localRotation = _rotation;
                _transform.localScale = _scale;
                if (_graphic != null) _graphic.color = _graphicColor;
                if (_canvasGroup != null) _canvasGroup.alpha = _alpha;
                if (_text != null)
                {
                    _text.text = _content;
                    _text.maxVisibleCharacters = _maxVisibleCharacters;
                    _text.ForceMeshUpdate();
                }
                if (_renderer != null)
                {
                    _renderer.SetPropertyBlock(null);
                    _renderer.material.color = _rendererColor;
                }
                if (_camera != null) _camera.fieldOfView = _fieldOfView;
                _transform.gameObject.SetActive(_active);
            }
        }
    }
}
