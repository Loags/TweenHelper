using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    internal static class DestinationMotionUtility
    {
        private const float HopAnticipationRatio = 0.12f;
        private const float HopTakeoffRestoreRatio = 0.12f;
        private const float HopFlightRatio = 0.74f;
        private const float HopLandingRatio = 0.1f;
        private const float HopRestoreRatio = 0.14f;
        private const float SpringTravelRatio = 0.72f;
        private const float MagneticPullbackRatio = 0.16f;
        private const float MagneticTravelRatio = 0.6f;

        public static Tween CreateArc(GameObject target, Vector3 destination, float height, float duration, TweenOptions options, bool local)
        {
            ValidateRequest(destination, duration);
            ValidateFinite(height, nameof(height));
            var binding = new PositionBinding(target, local);
            return CreatePathTween(target, binding, destination, duration, options, (start, progress) => EvaluateArc(start, destination, height, progress), false);
        }

        public static Tween CreateBezier(GameObject target, Vector3 destination, Vector3 controlA, Vector3 controlB, float duration, TweenOptions options, bool local)
        {
            ValidateRequest(destination, duration);
            ValidateVector(controlA, nameof(controlA));
            ValidateVector(controlB, nameof(controlB));
            var binding = new PositionBinding(target, local);
            return CreatePathTween(target, binding, destination, duration, options, (start, progress) => EvaluateBezier(start, controlA, controlB, destination, progress), false);
        }

        public static Tween CreateHop(GameObject target, Vector3 destination, float height, float duration, TweenOptions options, bool local)
        {
            ValidateRequest(destination, duration);
            ValidateFinite(height, nameof(height));
            var binding = new PositionBinding(target, local);
            Transform transform = target.transform;
            Vector3 originalScale = transform.localScale;
            Vector3 anticipationScale = Vector3.Scale(originalScale, new Vector3(1.06f, 0.9f, 1.06f));
            Vector3 landingScale = Vector3.Scale(originalScale, new Vector3(1.12f, 0.8f, 1.12f));
            Ease anticipationEase = options.SecondaryEase ?? Ease.InQuad;
            Ease landingEase = options.TertiaryEase ?? options.SecondaryEase ?? Ease.OutQuad;
            var groundedPose = new GroundedHopPose(binding, transform, originalScale, GetGroundAnchor(transform), local);
            bool scaleIsRestored = false;
            bool hopStarted = false;
            Vector3 hopStart = default;
            float flightProgress = 0f;

            Tween flight = DOTween.To(() => flightProgress, value =>
            {
                flightProgress = value;
                float progress = NormalizeProgress(value);
                Vector3 position = Vector3.LerpUnclamped(hopStart, destination, progress);
                groundedPose.SetBasePosition(position + Vector3.up * (4f * height * progress * (1f - progress)));
            }, 1f, duration * HopFlightRatio);

            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                if (!hopStarted)
                {
                    hopStart = binding.Get();
                    hopStarted = true;
                }

                groundedPose.Initialize(hopStart);
            });
            sequence.Append(CreateGroundedScaleTween(groundedPose, anticipationScale, duration * HopAnticipationRatio).SetEase(anticipationEase));
            sequence.Append(flight);
            float takeoffStart = duration * HopAnticipationRatio;
            sequence.Insert(takeoffStart, CreateGroundedScaleTween(groundedPose, originalScale, duration * HopTakeoffRestoreRatio).SetEase(Ease.OutBack));
            float landingStart = duration * (HopAnticipationRatio + HopFlightRatio - HopLandingRatio);
            sequence.Insert(landingStart, CreateGroundedScaleTween(groundedPose, landingScale, duration * HopLandingRatio).SetEase(landingEase));
            sequence.Append(CreateGroundedScaleTween(groundedPose, originalScale, duration * HopRestoreRatio).SetEase(Ease.OutBack));
            sequence.AppendCallback(() =>
            {
                groundedPose.RestoreAt(destination);
                scaleIsRestored = true;
            });
            sequence.OnRewind(() =>
            {
                if (hopStarted && transform != null) groundedPose.RestoreAt(hopStart);
                scaleIsRestored = false;
            });

            ConfigureTween(sequence, options.SetEase(Ease.Linear), target);
            sequence.OnKill(() =>
            {
                if (scaleIsRestored || transform == null) return;
                if (hopStarted) groundedPose.RestoreCurrentBase();
                else transform.localScale = originalScale;
            });
            ApplyExactEndpoint(sequence, binding, destination, options, () => hopStart, () => hopStarted);
            sequence.Pause();
            return sequence;
        }

        public static Tween CreateSpring(GameObject target, Vector3 destination, float duration, float overshoot, TweenOptions options, bool local)
        {
            ValidateRequest(destination, duration);
            ValidateNonNegative(overshoot, nameof(overshoot));
            var binding = new PositionBinding(target, local);
            Ease travelEase = options.Ease ?? Ease.OutCubic;
            Ease settleEase = options.SecondaryEase ?? options.Ease ?? Ease.OutBack;

            return CreatePathTween(target, binding, destination, duration, options, (start, progress) =>
            {
                Vector3 direction = GetDirection(start, destination);
                Vector3 overshootPosition = destination + direction * overshoot;
                if (progress <= SpringTravelRatio)
                {
                    float phase = progress / SpringTravelRatio;
                    return Vector3.LerpUnclamped(start, overshootPosition, EvaluateEase(phase, travelEase));
                }

                float settle = (progress - SpringTravelRatio) / (1f - SpringTravelRatio);
                return Vector3.LerpUnclamped(overshootPosition, destination, EvaluateEase(settle, settleEase));
            }, true);
        }

        public static Tween CreateMagneticSnap(GameObject target, Vector3 destination, float duration, float pullback, float overshoot, TweenOptions options, bool local)
        {
            ValidateRequest(destination, duration);
            ValidateNonNegative(pullback, nameof(pullback));
            ValidateNonNegative(overshoot, nameof(overshoot));
            var binding = new PositionBinding(target, local);
            Ease pullbackEase = options.SecondaryEase ?? Ease.OutSine;
            Ease travelEase = options.Ease ?? Ease.InCubic;
            Ease settleEase = options.TertiaryEase ?? options.SecondaryEase ?? Ease.OutBack;
            float travelEnd = MagneticPullbackRatio + MagneticTravelRatio;

            return CreatePathTween(target, binding, destination, duration, options, (start, progress) =>
            {
                Vector3 direction = GetDirection(start, destination);
                Vector3 pullbackPosition = start - direction * pullback;
                Vector3 overshootPosition = destination + direction * overshoot;

                if (progress <= MagneticPullbackRatio)
                {
                    float phase = progress / MagneticPullbackRatio;
                    return Vector3.LerpUnclamped(start, pullbackPosition, EvaluateEase(phase, pullbackEase));
                }

                if (progress <= travelEnd)
                {
                    float phase = (progress - MagneticPullbackRatio) / MagneticTravelRatio;
                    return Vector3.LerpUnclamped(pullbackPosition, overshootPosition, EvaluateEase(phase, travelEase));
                }

                float settle = (progress - travelEnd) / (1f - travelEnd);
                return Vector3.LerpUnclamped(overshootPosition, destination, EvaluateEase(settle, settleEase));
            }, true);
        }

        public static IReadOnlyList<Vector3> SnapshotWaypoints(IEnumerable<Vector3> waypoints)
        {
            if (waypoints == null) throw new ArgumentNullException(nameof(waypoints));
            var snapshot = new List<Vector3>();
            int index = 0;
            foreach (Vector3 waypoint in waypoints)
            {
                ValidateVector(waypoint, $"waypoint at index {index}");
                snapshot.Add(waypoint);
                index++;
            }

            if (snapshot.Count == 0) throw new ArgumentException("At least one waypoint is required.", nameof(waypoints));
            return snapshot;
        }

        public static Tween CreateWaypointPath(GameObject target, IReadOnlyList<Vector3> waypoints, DestinationPathInterpolation interpolation, float duration, TweenOptions options, bool local)
        {
            if (waypoints == null) throw new ArgumentNullException(nameof(waypoints));
            if (waypoints.Count == 0) throw new ArgumentException("At least one waypoint is required.", nameof(waypoints));
            for (int i = 0; i < waypoints.Count; i++) ValidateVector(waypoints[i], $"waypoint at index {i}");
            if (!Enum.IsDefined(typeof(DestinationPathInterpolation), interpolation)) throw new ArgumentOutOfRangeException(nameof(interpolation));
            ValidateRequest(waypoints[waypoints.Count - 1], duration);
            ValidateNormalizedTiming(options, "Waypoint paths");
            var binding = new PositionBinding(target, local);
            Vector3 destination = waypoints[waypoints.Count - 1];
            return CreatePathTween(target, binding, destination, duration, options, (start, progress) => EvaluateWaypointPath(start, waypoints, interpolation, progress), false);
        }

        public static Tween CreateSpiral(GameObject target, Vector3 destination, float radius, float revolutions, float duration, TweenOptions options, bool local)
        {
            ValidateRequest(destination, duration);
            ValidateNonNegative(radius, nameof(radius));
            ValidateFinite(revolutions, nameof(revolutions));
            ValidateNormalizedTiming(options, "Spiral motion");
            float strength = ResolveStrength(options);
            var binding = new PositionBinding(target, local);
            Ease travelEase = options.Ease ?? Ease.InOutCubic;

            return CreatePathTween(target, binding, destination, duration, options, (start, progress) =>
            {
                float travel = EvaluateEase(progress, travelEase);
                Vector3 basePosition = Vector3.LerpUnclamped(start, destination, travel);
                Vector3 axis = destination - start;
                if (axis.sqrMagnitude <= 0.000001f || radius <= 0f || Mathf.Approximately(revolutions, 0f)) return basePosition;
                axis.Normalize();
                GetSpiralBasis(axis, binding.IsRectTransform, out Vector3 basisA, out Vector3 basisB);
                float envelope = Mathf.Sin(progress * Mathf.PI);
                float angle = progress * revolutions * Mathf.PI * 2f;
                Vector3 radial = basisA * Mathf.Cos(angle) + basisB * Mathf.Sin(angle);
                return basePosition + radial * (radius * strength * envelope);
            }, true);
        }

        public static Tween CreateMultiHop(GameObject target, Vector3 destination, float height, int hopCount, float decay, float duration, TweenOptions options, bool local)
        {
            ValidateRequest(destination, duration);
            ValidateFinite(height, nameof(height));
            ValidateNonNegative(decay, nameof(decay));
            if (hopCount <= 0) throw new ArgumentOutOfRangeException(nameof(hopCount), hopCount, "Hop count must be greater than zero.");
            ValidateNormalizedTiming(options, "Multi-hop motion");
            float strength = ResolveStrength(options);
            var binding = new PositionBinding(target, local);
            Ease travelEase = options.Ease ?? Ease.InOutCubic;

            return CreatePathTween(target, binding, destination, duration, options, (start, progress) =>
            {
                float travel = EvaluateEase(progress, travelEase);
                Vector3 position = Vector3.LerpUnclamped(start, destination, travel);
                float bounce = Mathf.Abs(Mathf.Sin(progress * hopCount * Mathf.PI));
                float envelope = Mathf.Pow(1f - Mathf.Clamp01(progress), decay);
                return position + Vector3.up * (height * strength * bounce * envelope);
            }, true);
        }

        private static Tween CreatePathTween(GameObject target, PositionBinding binding, Vector3 destination, float duration, TweenOptions options, Func<Vector3, float, Vector3> evaluator, bool evaluatesInternalEases, bool applyOptions = true)
        {
            float progress = 0f;
            bool initialized = false;
            Vector3 start = default;

            void Initialize()
            {
                if (initialized) return;
                start = binding.Get();
                initialized = true;
            }

            var tween = DOTween.To(() => progress, value =>
            {
                progress = value;
                Initialize();
                binding.Set(evaluator(start, NormalizeProgress(value)));
            }, 1f, duration);
            tween.OnStart(Initialize);

            TweenOptions configuredOptions = evaluatesInternalEases ? options.SetEase(Ease.Linear) : options;
            if (applyOptions) ConfigureTween(tween, configuredOptions, target);
            ApplyExactEndpoint(tween, binding, destination, applyOptions ? options : default, () => start, () => initialized);
            tween.Pause();
            return tween;
        }

        private static void ConfigureTween<T>(T tween, TweenOptions options, GameObject target) where T : Tween
        {
            tween.WithDefaults(options, target);
        }

        private static void ApplyExactEndpoint(Tween tween, PositionBinding binding, Vector3 destination, TweenOptions options, Func<Vector3> startGetter = null, Func<bool> initializedGetter = null)
        {
            tween.OnComplete(() =>
            {
                if (EndsAtStart(options) && startGetter != null && (initializedGetter == null || initializedGetter()))
                {
                    binding.Set(startGetter());
                    return;
                }

                binding.Set(destination);
            });
        }

        private static bool EndsAtStart(TweenOptions options)
        {
            int loops = options.Loops ?? 1;
            return loops > 0 && options.LoopType == LoopType.Yoyo && loops % 2 == 0;
        }

        private static float NormalizeProgress(float value)
        {
            if (value >= 0f && value <= 1f) return value;
            float normalized = value - Mathf.Floor(value);
            return value > 0f && Mathf.Approximately(normalized, 0f) ? 1f : normalized;
        }

        private static float EvaluateEase(float progress, Ease ease) => DOVirtual.EasedValue(0f, 1f, Mathf.Clamp01(progress), ease);

        private static Tween CreateGroundedScaleTween(GroundedHopPose groundedPose, Vector3 scale, float duration)
        {
            return DOTween.To(() => groundedPose.Scale, groundedPose.SetScale, scale, duration);
        }

        internal static Vector3 GetGroundAnchor(Transform transform)
        {
            if (transform is RectTransform rectTransform)
            {
                Rect rect = rectTransform.rect;
                return new Vector3(rect.center.x, rect.yMin, 0f);
            }

            Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return new Vector3(0f, -0.5f, 0f);

            Matrix4x4 worldToTarget = transform.worldToLocalMatrix;
            Bounds targetBounds = default;
            bool hasBounds = false;

            foreach (Renderer renderer in renderers)
            {
                Bounds rendererBounds = renderer.localBounds;
                Matrix4x4 rendererToTarget = worldToTarget * renderer.transform.localToWorldMatrix;

                for (int x = -1; x <= 1; x += 2)
                {
                    for (int y = -1; y <= 1; y += 2)
                    {
                        for (int z = -1; z <= 1; z += 2)
                        {
                            Vector3 corner = rendererBounds.center + Vector3.Scale(rendererBounds.extents, new Vector3(x, y, z));
                            Vector3 targetPoint = rendererToTarget.MultiplyPoint3x4(corner);
                            if (hasBounds) targetBounds.Encapsulate(targetPoint);
                            else
                            {
                                targetBounds = new Bounds(targetPoint, Vector3.zero);
                                hasBounds = true;
                            }
                        }
                    }
                }
            }

            if (!hasBounds) return new Vector3(0f, -0.5f, 0f);
            float bottom = transform.localScale.y >= 0f ? targetBounds.min.y : targetBounds.max.y;
            return new Vector3(targetBounds.center.x, bottom, targetBounds.center.z);
        }

        private static Vector3 EvaluateBezier(Vector3 start, Vector3 controlA, Vector3 controlB, Vector3 destination, float progress)
        {
            float inverse = 1f - progress;
            return inverse * inverse * inverse * start +
                   3f * inverse * inverse * progress * controlA +
                   3f * inverse * progress * progress * controlB +
                   progress * progress * progress * destination;
        }

        internal static Vector3 EvaluateArc(Vector3 start, Vector3 destination, float height, float progress)
        {
            Vector3 position = Vector3.LerpUnclamped(start, destination, progress);
            return position + Vector3.up * (4f * height * progress * (1f - progress));
        }

        private static Vector3 GetDirection(Vector3 start, Vector3 destination)
        {
            Vector3 delta = destination - start;
            return delta.sqrMagnitude > Mathf.Epsilon ? delta.normalized : Vector3.zero;
        }

        private static Vector3 EvaluateWaypointPath(Vector3 start, IReadOnlyList<Vector3> waypoints, DestinationPathInterpolation interpolation, float progress)
        {
            if (progress >= 1f) return waypoints[waypoints.Count - 1];
            float scaled = Mathf.Clamp01(progress) * waypoints.Count;
            int segment = Mathf.Min(Mathf.FloorToInt(scaled), waypoints.Count - 1);
            float segmentProgress = scaled - segment;
            Vector3 pointA = segment == 0 ? start : waypoints[segment - 1];
            Vector3 pointB = waypoints[segment];
            if (interpolation == DestinationPathInterpolation.Linear) return Vector3.LerpUnclamped(pointA, pointB, segmentProgress);

            Vector3 previous = segment <= 1 ? start : waypoints[segment - 2];
            Vector3 next = segment + 1 < waypoints.Count ? waypoints[segment + 1] : pointB;
            return EvaluateCatmullRom(previous, pointA, pointB, next, segmentProgress);
        }

        private static Vector3 EvaluateCatmullRom(Vector3 previous, Vector3 start, Vector3 end, Vector3 next, float progress)
        {
            float square = progress * progress;
            float cube = square * progress;
            return 0.5f * ((2f * start) + (-previous + end) * progress + (2f * previous - 5f * start + 4f * end - next) * square + (-previous + 3f * start - 3f * end + next) * cube);
        }

        private static void GetSpiralBasis(Vector3 axis, bool isUi, out Vector3 basisA, out Vector3 basisB)
        {
            if (isUi)
            {
                basisA = Vector3.right;
                basisB = Vector3.up;
                return;
            }

            Vector3 reference = Mathf.Abs(Vector3.Dot(axis, Vector3.up)) > 0.92f ? Vector3.right : Vector3.up;
            basisA = Vector3.Cross(axis, reference).normalized;
            basisB = Vector3.Cross(axis, basisA).normalized;
        }

        private static float ResolveStrength(TweenOptions options)
        {
            float strength = options.Strength ?? 1f;
            ValidateFinite(strength, nameof(TweenOptions.Strength));
            if (strength < 0f) throw new ArgumentOutOfRangeException(nameof(TweenOptions.Strength), strength, "Strength cannot be negative.");
            return strength;
        }

        private static void ValidateNormalizedTiming(TweenOptions options, string operation)
        {
            if (options.SpeedBased == true) throw new NotSupportedException($"{operation} does not support speed-based timing.");
        }

        private static void ValidateRequest(Vector3 destination, float duration)
        {
            ValidateVector(destination, nameof(destination));
            ValidateFinite(duration, nameof(duration));
            if (duration <= 0f) throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration must be greater than zero.");
        }

        private static void ValidateNonNegative(float value, string parameterName)
        {
            ValidateFinite(value, parameterName);
            if (value < 0f) throw new ArgumentOutOfRangeException(parameterName, value, "Value cannot be negative.");
        }

        private static void ValidateVector(Vector3 value, string parameterName)
        {
            ValidateFinite(value.x, parameterName);
            ValidateFinite(value.y, parameterName);
            ValidateFinite(value.z, parameterName);
        }

        private static void ValidateFinite(float value, string parameterName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new ArgumentOutOfRangeException(parameterName, value, "Value must be finite.");
        }

        internal sealed class PositionBinding
        {
            private readonly Transform _transform;
            private readonly RectTransform _rectTransform;
            private readonly bool _local;

            public PositionBinding(GameObject target, bool local)
            {
                _transform = target.transform;
                _rectTransform = local ? target.GetComponent<RectTransform>() : null;
                _local = local;
            }

            public bool IsRectTransform => _rectTransform != null;

            public Vector3 Get()
            {
                if (!_local) return _transform.position;
                return _rectTransform != null ? _rectTransform.anchoredPosition3D : _transform.localPosition;
            }

            public void Set(Vector3 position)
            {
                if (!_local)
                {
                    _transform.position = position;
                    return;
                }

                if (_rectTransform != null)
                {
                    _rectTransform.anchoredPosition3D = position;
                    return;
                }

                _transform.localPosition = position;
            }
        }

        internal sealed class GroundedHopPose
        {
            private readonly PositionBinding _binding;
            private readonly Transform _transform;
            private readonly Vector3 _originalScale;
            private readonly Vector3 _groundAnchor;
            private readonly bool _local;
            private Vector3 _basePosition;
            private Vector3 _scale;
            private bool _isInitialized;

            public GroundedHopPose(PositionBinding binding, Transform transform, Vector3 originalScale, Vector3 groundAnchor, bool local)
            {
                _binding = binding;
                _transform = transform;
                _originalScale = originalScale;
                _groundAnchor = groundAnchor;
                _local = local;
                _basePosition = binding.Get();
                _scale = originalScale;
            }

            public Vector3 Scale => _scale;

            public void Initialize(Vector3 basePosition)
            {
                _basePosition = basePosition;
                _scale = _originalScale;
                _isInitialized = true;
                Apply();
            }

            public void SetBasePosition(Vector3 basePosition)
            {
                _basePosition = basePosition;
                if (_isInitialized) Apply();
            }

            public void SetScale(Vector3 scale)
            {
                _scale = scale;
                if (_isInitialized) Apply();
            }

            public void RestoreAt(Vector3 basePosition)
            {
                _basePosition = basePosition;
                _scale = _originalScale;
                Apply();
            }

            public void RestoreCurrentBase()
            {
                _scale = _originalScale;
                Apply();
            }

            private void Apply()
            {
                _transform.localScale = _scale;
                _binding.Set(_basePosition + GetGroundingOffset());
            }

            private Vector3 GetGroundingOffset()
            {
                Vector3 targetLocalOffset = Vector3.Scale(_groundAnchor, _originalScale - _scale);
                Vector3 parentLocalOffset = _transform.localRotation * targetLocalOffset;
                if (_local || _transform.parent == null) return parentLocalOffset;
                return _transform.parent.TransformVector(parentLocalOffset);
            }
        }
    }
}
