using System;
using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    internal static class DestinationMotionUtility
    {
        private const float HopAnticipationRatio = 0.12f;
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
            return CreatePathTween(target, binding, destination, duration, options, (start, progress) =>
            {
                Vector3 position = Vector3.LerpUnclamped(start, destination, progress);
                return position + Vector3.up * (4f * height * progress * (1f - progress));
            }, false);
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
            Vector3 anticipationScale = Vector3.Scale(originalScale, new Vector3(1.05f, 0.92f, 1.05f));
            Vector3 landingScale = Vector3.Scale(originalScale, new Vector3(1.1f, 0.84f, 1.1f));
            Ease anticipationEase = options.SecondaryEase ?? Ease.InQuad;
            Ease landingEase = options.TertiaryEase ?? options.SecondaryEase ?? Ease.OutQuad;
            bool scaleIsRestored = false;
            bool hopStarted = false;
            Vector3 hopStart = default;

            Tween flight = CreatePathTween(target, binding, destination, duration * HopFlightRatio, options, (start, progress) =>
            {
                Vector3 position = Vector3.LerpUnclamped(start, destination, progress);
                return position + Vector3.up * (4f * height * progress * (1f - progress));
            }, false, false);

            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                if (!hopStarted)
                {
                    hopStart = binding.Get();
                    hopStarted = true;
                }

            });
            sequence.Append(transform.DOScale(anticipationScale, duration * HopAnticipationRatio).SetEase(anticipationEase));
            sequence.Append(flight);
            float landingStart = duration * (HopAnticipationRatio + HopFlightRatio - HopLandingRatio);
            sequence.Insert(landingStart, transform.DOScale(landingScale, duration * HopLandingRatio).SetEase(landingEase));
            sequence.Append(transform.DOScale(originalScale, duration * HopRestoreRatio).SetEase(Ease.OutBack));
            sequence.AppendCallback(() =>
            {
                binding.Set(destination);
                transform.localScale = originalScale;
                scaleIsRestored = true;
            });
            sequence.OnRewind(() => scaleIsRestored = false);

            ConfigureTween(sequence, options.SetEase(Ease.Linear), target);
            sequence.OnKill(() =>
            {
                if (!scaleIsRestored && transform != null) transform.localScale = originalScale;
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

        private static Vector3 EvaluateBezier(Vector3 start, Vector3 controlA, Vector3 controlB, Vector3 destination, float progress)
        {
            float inverse = 1f - progress;
            return inverse * inverse * inverse * start +
                   3f * inverse * inverse * progress * controlA +
                   3f * inverse * progress * progress * controlB +
                   progress * progress * progress * destination;
        }

        private static Vector3 GetDirection(Vector3 start, Vector3 destination)
        {
            Vector3 delta = destination - start;
            return delta.sqrMagnitude > Mathf.Epsilon ? delta.normalized : Vector3.zero;
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

        private sealed class PositionBinding
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
    }
}
