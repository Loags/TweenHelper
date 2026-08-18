using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    internal static class UIWorldProjectionUtility
    {
        public static Tween CreateArc(GameObject target, Vector3 worldSource, RectTransform uiTarget, float height, float duration, Camera worldCamera, bool lockDestination, TweenOptions options)
        {
            ValidateFinite(height, nameof(height));
            return CreatePath(target, worldSource, uiTarget, duration, worldCamera, lockDestination, options,
                (state, progress) => DestinationMotionUtility.EvaluateArc(state.Start, state.End, height * state.Strength, progress));
        }

        public static Tween CreateHop(GameObject target, Vector3 worldSource, RectTransform uiTarget, float height, float duration, Camera worldCamera, bool lockDestination, TweenOptions options)
        {
            ValidateFinite(height, nameof(height));
            ValidateRequest(target, worldSource, uiTarget, duration, options);
            ProjectionState state = null;
            Vector3 invocationScale = default;
            float strength = ResolveStrength(options);
            return NormalizedTweenTimeline.Create(
                target,
                duration,
                options.SetEase(Ease.Linear),
                () =>
                {
                    state = new ProjectionState(target, worldSource, uiTarget, worldCamera, lockDestination, strength);
                    invocationScale = target.transform.localScale;
                    state.Initialize();
                },
                value =>
                {
                    state.RefreshDestination();
                    float travel = EaseValue(value, options.Ease ?? Ease.InOutCubic);
                    state.Set(DestinationMotionUtility.EvaluateArc(state.Start, state.End, height * strength, travel));
                    target.transform.localScale = Vector3.Scale(invocationScale, EvaluateHopScale(value, strength));
                },
                () =>
                {
                    state.Set(state.End);
                    target.transform.localScale = invocationScale;
                },
                () => state.Restore(invocationScale),
                () => state.Restore(invocationScale),
                () => state.Restore(invocationScale));
        }

        public static Tween CreateBezier(GameObject target, Vector3 worldSource, Vector3 controlA, Vector3 controlB, RectTransform uiTarget, float duration, Camera worldCamera, bool lockDestination, TweenOptions options)
        {
            ValidateVector(controlA, nameof(controlA));
            ValidateVector(controlB, nameof(controlB));
            Vector3 resolvedControlA = default;
            Vector3 resolvedControlB = default;
            return CreatePath(target, worldSource, uiTarget, duration, worldCamera, lockDestination, options,
                (state, progress) =>
                {
                    if (progress <= 0f)
                    {
                        resolvedControlA = state.WorldToMotionPoint(controlA);
                        resolvedControlB = state.WorldToMotionPoint(controlB);
                    }

                    float inverse = 1f - progress;
                    return inverse * inverse * inverse * state.Start
                        + 3f * inverse * inverse * progress * resolvedControlA
                        + 3f * inverse * progress * progress * resolvedControlB
                        + progress * progress * progress * state.End;
                });
        }

        public static Tween CreatePickup(GameObject target, Vector3 worldSource, RectTransform uiTarget, float? arcHeight, float duration, Camera worldCamera, bool lockDestination, TweenOptions options)
        {
            if (arcHeight.HasValue) ValidateFinite(arcHeight.Value, nameof(arcHeight));
            ValidateRequest(target, worldSource, uiTarget, duration, options);
            float strength = ResolveStrength(options);
            ProjectionState state = null;
            Vector3 invocationScale = default;
            TweenTargetUtility.TweenAlphaBinding alphaBinding = default;
            bool hasAlpha = false;
            float invocationAlpha = 1f;
            return NormalizedTweenTimeline.Create(
                target,
                duration,
                options.SetEase(Ease.Linear),
                () =>
                {
                    state = new ProjectionState(target, worldSource, uiTarget, worldCamera, lockDestination, strength);
                    invocationScale = target.transform.localScale;
                    hasAlpha = TweenTargetUtility.TryGetAlphaBinding(target, out alphaBinding);
                    if (hasAlpha) invocationAlpha = alphaBinding.GetAlpha();
                    state.Initialize();
                },
                value =>
                {
                    state.RefreshDestination();
                    float travel = EaseValue(value, options.Ease ?? Ease.InOutCubic);
                    float height = (arcHeight ?? (target.transform is RectTransform ? 145f : 2f)) * strength;
                    state.Set(DestinationMotionUtility.EvaluateArc(state.Start, state.End, height, travel));
                    float scale = value <= 0.12f
                        ? Mathf.LerpUnclamped(1f, 1f + 0.16f * strength, EaseValue(value / 0.12f, Ease.OutBack))
                        : value <= 0.52f
                            ? Mathf.LerpUnclamped(1f + 0.16f * strength, 1f, EaseValue((value - 0.12f) / 0.4f, Ease.OutQuad))
                            : Mathf.LerpUnclamped(1f, 0f, EaseValue((value - 0.52f) / 0.48f, Ease.InCubic));
                    target.transform.localScale = invocationScale * scale;
                    if (hasAlpha)
                    {
                        float alpha = value <= 0.56f ? invocationAlpha : Mathf.LerpUnclamped(invocationAlpha, 0f, EaseValue((value - 0.56f) / 0.44f, Ease.InCubic));
                        alphaBinding.SetAlpha(alpha);
                    }
                },
                () =>
                {
                    state.Set(state.End);
                    target.transform.localScale = Vector3.zero;
                    if (hasAlpha) alphaBinding.SetAlpha(0f);
                },
                () => RestorePickup(state, target, invocationScale, hasAlpha, alphaBinding, invocationAlpha),
                () => RestorePickup(state, target, invocationScale, hasAlpha, alphaBinding, invocationAlpha),
                () => RestorePickup(state, target, invocationScale, hasAlpha, alphaBinding, invocationAlpha));
        }

        public static Tween CreatePathThrough(GameObject target, Vector3 worldSource, IReadOnlyList<Vector3> worldWaypoints, RectTransform uiTarget, DestinationPathInterpolation interpolation, float duration, Camera worldCamera, bool lockDestination, TweenOptions options)
        {
            if (worldWaypoints == null) throw new ArgumentNullException(nameof(worldWaypoints));
            var resolved = new List<Vector3>(worldWaypoints.Count + 2);
            return CreatePath(target, worldSource, uiTarget, duration, worldCamera, lockDestination, options,
                (state, progress) =>
                {
                    if (progress <= 0f)
                    {
                        resolved.Clear();
                        resolved.Add(state.Start);
                        for (int i = 0; i < worldWaypoints.Count; i++) resolved.Add(state.WorldToMotionPoint(worldWaypoints[i]));
                        resolved.Add(state.End);
                    }
                    else
                    {
                        resolved[resolved.Count - 1] = state.End;
                    }

                    return EvaluatePath(resolved, interpolation, progress);
                });
        }

        public static IReadOnlyList<Vector3> SnapshotWorldPoints(IEnumerable<Vector3> worldWaypoints)
        {
            if (worldWaypoints == null) throw new ArgumentNullException(nameof(worldWaypoints));
            var snapshot = new List<Vector3>();
            foreach (Vector3 waypoint in worldWaypoints)
            {
                ValidateVector(waypoint, nameof(worldWaypoints));
                snapshot.Add(waypoint);
            }

            return snapshot;
        }

        private static Tween CreatePath(GameObject target, Vector3 worldSource, RectTransform uiTarget, float duration, Camera worldCamera, bool lockDestination, TweenOptions options, Func<ProjectionState, float, Vector3> evaluator)
        {
            ValidateRequest(target, worldSource, uiTarget, duration, options);
            float strength = ResolveStrength(options);
            ProjectionState state = null;
            return NormalizedTweenTimeline.Create(
                target,
                duration,
                options.SetEase(Ease.Linear),
                () =>
                {
                    state = new ProjectionState(target, worldSource, uiTarget, worldCamera, lockDestination, strength);
                    state.Initialize();
                    evaluator(state, 0f);
                },
                value =>
                {
                    state.RefreshDestination();
                    float progress = EaseValue(value, options.Ease ?? Ease.InOutCubic);
                    state.Set(evaluator(state, progress));
                },
                () => state.Set(state.End),
                () => state.Restore(),
                () => state.Restore(),
                () => state.Restore());
        }

        private static Vector3 EvaluatePath(IReadOnlyList<Vector3> points, DestinationPathInterpolation interpolation, float progress)
        {
            if (points.Count == 1) return points[0];
            float scaled = Mathf.Clamp01(progress) * (points.Count - 1);
            int segment = Mathf.Min(Mathf.FloorToInt(scaled), points.Count - 2);
            float segmentProgress = scaled - segment;
            if (interpolation == DestinationPathInterpolation.Linear) return Vector3.LerpUnclamped(points[segment], points[segment + 1], segmentProgress);
            if (interpolation != DestinationPathInterpolation.CatmullRom) throw new ArgumentOutOfRangeException(nameof(interpolation), interpolation, "Unknown path interpolation.");

            Vector3 previous = points[Mathf.Max(0, segment - 1)];
            Vector3 start = points[segment];
            Vector3 end = points[segment + 1];
            Vector3 next = points[Mathf.Min(points.Count - 1, segment + 2)];
            float squared = segmentProgress * segmentProgress;
            float cubed = squared * segmentProgress;
            return 0.5f * ((2f * start) + (-previous + end) * segmentProgress
                + (2f * previous - 5f * start + 4f * end - next) * squared
                + (-previous + 3f * start - 3f * end + next) * cubed);
        }

        private static void RestorePickup(ProjectionState state, GameObject target, Vector3 invocationScale, bool hasAlpha, TweenTargetUtility.TweenAlphaBinding alphaBinding, float invocationAlpha)
        {
            state.Restore();
            if (target == null) return;
            target.transform.localScale = invocationScale;
            if (hasAlpha) alphaBinding.Restore(invocationAlpha);
        }

        private static Vector3 EvaluateHopScale(float progress, float strength)
        {
            Vector3 anticipation = Vector3.one + (new Vector3(1.08f, 0.84f, 1.08f) - Vector3.one) * strength;
            Vector3 landing = Vector3.one + (new Vector3(1.12f, 0.76f, 1.12f) - Vector3.one) * strength;
            if (progress <= 0.12f) return Vector3.LerpUnclamped(Vector3.one, anticipation, EaseValue(progress / 0.12f, Ease.InQuad));
            if (progress <= 0.82f) return Vector3.LerpUnclamped(anticipation, Vector3.one, EaseValue((progress - 0.12f) / 0.7f, Ease.OutBack));
            if (progress <= 0.9f) return Vector3.LerpUnclamped(Vector3.one, landing, EaseValue((progress - 0.82f) / 0.08f, Ease.OutQuad));
            return Vector3.LerpUnclamped(landing, Vector3.one, EaseValue((progress - 0.9f) / 0.1f, Ease.OutBack));
        }

        private static float ResolveStrength(TweenOptions options)
        {
            float strength = options.Strength ?? 1f;
            ValidateFinite(strength, nameof(TweenOptions.Strength));
            if (strength < 0f) throw new ArgumentOutOfRangeException(nameof(TweenOptions.Strength), strength, "Strength cannot be negative.");
            return strength;
        }

        private static void ValidateRequest(GameObject target, Vector3 worldSource, RectTransform uiTarget, float duration, TweenOptions options)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (uiTarget == null) throw new ArgumentNullException(nameof(uiTarget));
            ValidateVector(worldSource, nameof(worldSource));
            ValidateFinite(duration, nameof(duration));
            if (duration <= 0f) throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration must be greater than zero.");
            if (options.SpeedBased == true) throw new NotSupportedException("World-to-UI motion does not support speed-based timing.");
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

        private static float EaseValue(float progress, Ease ease) => DOVirtual.EasedValue(0f, 1f, Mathf.Clamp01(progress), ease);

        private sealed class ProjectionState
        {
            private readonly GameObject _target;
            private readonly Vector3 _worldSource;
            private readonly RectTransform _uiTarget;
            private readonly Camera _requestedWorldCamera;
            private readonly bool _lockDestination;
            private readonly bool _isUiMotion;
            private readonly RectTransform _animatedRect;
            private DestinationMotionUtility.PositionBinding _binding;
            private RectTransform _motionParent;
            private Camera _worldCamera;
            private Camera _uiCamera;
            private Vector3 _invocationPosition;
            private float _worldDepth;

            public ProjectionState(GameObject target, Vector3 worldSource, RectTransform uiTarget, Camera requestedWorldCamera, bool lockDestination, float strength)
            {
                _target = target;
                _worldSource = worldSource;
                _uiTarget = uiTarget;
                _requestedWorldCamera = requestedWorldCamera;
                _lockDestination = lockDestination;
                _animatedRect = target.transform as RectTransform;
                _isUiMotion = _animatedRect != null;
                Strength = strength;
            }

            public Vector3 Start { get; private set; }
            public Vector3 End { get; private set; }
            public float Strength { get; }

            public void Initialize()
            {
                Canvas targetCanvas = _uiTarget.GetComponentInParent<Canvas>();
                if (targetCanvas == null) throw new InvalidOperationException($"UI target '{_uiTarget.name}' must belong to a Canvas.");
                _uiCamera = targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : targetCanvas.worldCamera;
                _worldCamera = _requestedWorldCamera != null ? _requestedWorldCamera : targetCanvas.worldCamera != null ? targetCanvas.worldCamera : Camera.main;
                if (_worldCamera == null) throw new InvalidOperationException("World-to-UI motion requires an explicit world camera, a Canvas worldCamera, or Camera.main.");

                _binding = new DestinationMotionUtility.PositionBinding(_target, _isUiMotion);
                _invocationPosition = _binding.Get();
                if (_isUiMotion)
                {
                    _motionParent = _animatedRect.parent as RectTransform;
                    if (_motionParent == null) throw new InvalidOperationException($"Animated RectTransform '{_animatedRect.name}' must have a RectTransform parent.");
                    Start = ScreenToMotionPoint(RectTransformUtility.WorldToScreenPoint(_worldCamera, _worldSource));
                }
                else
                {
                    Start = _worldSource;
                    _worldDepth = _worldCamera.WorldToScreenPoint(_worldSource).z;
                    if (_worldDepth <= 0f) throw new InvalidOperationException("World source must be in front of the resolved world camera.");
                }

                End = ResolveDestination();
                _binding.Set(Start);
            }

            public Vector3 WorldToMotionPoint(Vector3 worldPoint)
            {
                if (!_isUiMotion) return worldPoint;
                return ScreenToMotionPoint(RectTransformUtility.WorldToScreenPoint(_worldCamera, worldPoint));
            }

            public void RefreshDestination()
            {
                if (!_lockDestination) End = ResolveDestination();
            }

            public void Set(Vector3 position) => _binding.Set(position);
            public void Restore() => _binding.Set(_invocationPosition);
            public void Restore(Vector3 scale)
            {
                Restore();
                if (_target != null) _target.transform.localScale = scale;
            }

            private Vector3 ResolveDestination()
            {
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_uiCamera, _uiTarget.position);
                if (_isUiMotion) return ScreenToMotionPoint(screenPoint);
                return _worldCamera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, _worldDepth));
            }

            private Vector3 ScreenToMotionPoint(Vector2 screenPoint)
            {
                Canvas motionCanvas = _animatedRect.GetComponentInParent<Canvas>();
                if (motionCanvas == null) throw new InvalidOperationException($"Animated RectTransform '{_animatedRect.name}' must belong to a Canvas.");
                Camera motionCamera = motionCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : motionCanvas.worldCamera;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_motionParent, screenPoint, motionCamera, out Vector2 localPoint))
                {
                    throw new InvalidOperationException($"Could not convert screen point to parent space for '{_animatedRect.name}'.");
                }

                return new Vector3(localPoint.x, localPoint.y, _animatedRect.anchoredPosition3D.z);
            }
        }
    }
}
