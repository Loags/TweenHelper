using System;
using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    internal static class FeedbackSequenceUtility
    {
        private static readonly Color ErrorColor = new Color(0.95f, 0.14f, 0.12f, 1f);
        private static readonly Color DamageColor = new Color(1f, 0.08f, 0.04f, 1f);
        private static readonly Color SuccessColor = new Color(0.12f, 0.9f, 0.32f, 1f);
        private static readonly Color RewardColor = new Color(1f, 0.68f, 0.08f, 1f);
        private static readonly Color HealColor = new Color(0.18f, 1f, 0.48f, 1f);
        private static readonly Color ShieldColor = new Color(0.2f, 0.72f, 1f, 1f);
        private static readonly Color CriticalColor = new Color(1f, 0.12f, 0.04f, 1f);
        private static readonly Color ReadyColor = new Color(0.22f, 0.88f, 1f, 1f);
        private static readonly Color LevelColor = new Color(1f, 0.74f, 0.12f, 1f);
        private static readonly Color LowHealthColor = new Color(1f, 0.05f, 0.08f, 1f);

        public static Tween CreateErrorReject(GameObject target, float duration, Color? flashColor, TweenOptions options)
        {
            float strength = ResolveStrength(options);
            Color resolvedColor = flashColor ?? ErrorColor;
            return CreateTransient(target, duration, options, false, (state, progress) =>
            {
                float envelope = Mathf.Pow(1f - progress, 1.35f);
                float wave = Mathf.Sin(progress * Mathf.PI * 7f) * envelope;
                float distance = (state.IsUi ? 34f : 0.34f) * strength;
                float angle = -wave * 7f * strength;
                state.SetPose(Vector3.right * (wave * distance), state.BaseScale, Quaternion.Euler(0f, 0f, angle));
                state.SetFlash(resolvedColor, FlashEnvelope(progress, 0.12f, 0.72f) * 0.72f);
            });
        }

        public static Tween CreateDamageHit(GameObject target, float duration, Color? flashColor, TweenOptions options)
        {
            float strength = ResolveStrength(options);
            Color resolvedColor = flashColor ?? DamageColor;
            return CreateTransient(target, duration, options, true, (state, progress) =>
            {
                float envelope = Mathf.Pow(1f - progress, 1.8f);
                float shake = Mathf.Sin(progress * Mathf.PI * 9f) * envelope;
                float distance = (state.IsUi ? 20f : 0.2f) * strength;
                Vector3 offset = new Vector3(shake * distance, Mathf.Sin(progress * Mathf.PI * 5f) * distance * 0.12f * envelope, 0f);
                Vector3 scaleMultiplier = EvaluateDamageScale(progress, strength);
                state.SetGroundedPose(offset, Vector3.Scale(state.BaseScale, scaleMultiplier), Quaternion.identity);
                state.SetFlash(resolvedColor, FlashEnvelope(progress, 0.07f, 0.58f) * 0.88f);
            });
        }

        public static Tween CreateSuccessConfirm(GameObject target, float duration, Color? flashColor, TweenOptions options)
        {
            float strength = ResolveStrength(options);
            Color resolvedColor = flashColor ?? SuccessColor;
            return CreateTransient(target, duration, options, false, (state, progress) =>
            {
                float height = (state.IsUi ? 28f : 0.28f) * strength;
                float verticalOffset = EvaluateSuccessBounce(progress) * height;
                Vector3 scaleMultiplier = EvaluateSuccessScale(progress, strength);
                state.SetPose(Vector3.up * verticalOffset, Vector3.Scale(state.BaseScale, scaleMultiplier), Quaternion.identity);
                state.SetFlash(resolvedColor, FlashEnvelope(progress, 0.18f, 0.76f) * 0.62f);
            });
        }

        public static Tween CreateRewardReveal(GameObject target, float duration, Color? flashColor, TweenOptions options)
        {
            float strength = ResolveStrength(options);
            Color resolvedColor = flashColor ?? RewardColor;
            return CreateTransient(target, duration, options, false, (state, progress) =>
            {
                float height = (state.IsUi ? 18f : 0.18f) * strength;
                float lift = progress < 0.78f ? Parabola(progress / 0.78f) * height : 0f;
                Vector3 scaleMultiplier = EvaluateRewardScale(progress, strength);
                float angle = EvaluateRewardAngle(progress) * strength;
                Quaternion rotationOffset = state.IsUi ? Quaternion.Euler(0f, 0f, angle) : Quaternion.Euler(0f, angle, 0f);
                state.SetPose(Vector3.up * lift, Vector3.Scale(state.BaseScale, scaleMultiplier), rotationOffset);
                state.SetFlash(resolvedColor, Mathf.Sin(Mathf.Clamp01(progress) * Mathf.PI) * 0.58f);
            });
        }

        public static Tween CreateHealReceive(GameObject target, float duration, Color? flashColor, TweenOptions options)
        {
            float strength = ResolveStrength(options);
            Color resolvedColor = flashColor ?? HealColor;
            return CreateTransient(target, duration, options, false, (state, progress) =>
            {
                float height = (state.IsUi ? 24f : 0.24f) * strength;
                float lift = Mathf.Sin(progress * Mathf.PI) * height;
                Vector3 stretch = ScaleMagnitude(new Vector3(0.94f, 1.14f, 0.94f), strength);
                Vector3 settle = ScaleMagnitude(Vector3.one * 1.07f, strength);
                Vector3 scale = progress <= 0.3f
                    ? Vector3.LerpUnclamped(Vector3.one, stretch, EvaluateEase(progress / 0.3f, Ease.OutCubic))
                    : progress <= 0.66f
                        ? Vector3.LerpUnclamped(stretch, settle, EvaluateEase((progress - 0.3f) / 0.36f, Ease.InOutSine))
                        : Vector3.LerpUnclamped(settle, Vector3.one, EvaluateEase((progress - 0.66f) / 0.34f, Ease.OutBack));
                state.SetPose(Vector3.up * lift, Vector3.Scale(state.BaseScale, scale), Quaternion.identity);
                state.SetFlash(resolvedColor, FlashEnvelope(progress, 0.22f, 0.88f) * 0.72f);
            });
        }

        public static Tween CreateShieldBlock(GameObject target, Vector3 impactDirection, float duration, Color? flashColor, TweenOptions options)
        {
            Vector3 direction = ResolveDirection(impactDirection, nameof(impactDirection));
            float strength = ResolveStrength(options);
            Color resolvedColor = flashColor ?? ShieldColor;
            return CreateTransient(target, duration, options, false, (state, progress) =>
            {
                float envelope = Mathf.Pow(1f - progress, 2.1f);
                float recoilDistance = (state.IsUi ? 30f : 0.3f) * strength;
                float rebound = Mathf.Sin(progress * Mathf.PI * 3f) * envelope;
                Vector3 offset = -direction * recoilDistance * (0.65f * envelope + 0.35f * rebound);
                Vector3 compressed = ScaleMagnitude(new Vector3(0.86f, 1.08f, 1.08f), strength);
                Vector3 scale = progress <= 0.16f
                    ? Vector3.LerpUnclamped(Vector3.one, compressed, EvaluateEase(progress / 0.16f, Ease.OutQuad))
                    : Vector3.LerpUnclamped(compressed, Vector3.one, EvaluateEase((progress - 0.16f) / 0.84f, Ease.OutElastic));
                float angle = -direction.x * 8f * strength * envelope;
                state.SetPose(offset, Vector3.Scale(state.BaseScale, scale), Quaternion.Euler(0f, 0f, angle));
                state.SetFlash(resolvedColor, FlashEnvelope(progress, 0.08f, 0.62f) * 0.84f);
            });
        }

        public static Tween CreateCriticalHit(GameObject target, Vector3 impactDirection, float duration, Color? flashColor, TweenOptions options)
        {
            Vector3 direction = ResolveDirection(impactDirection, nameof(impactDirection));
            float strength = ResolveStrength(options);
            Color resolvedColor = flashColor ?? CriticalColor;
            return CreateTransient(target, duration, options, true, (state, progress) =>
            {
                float envelope = Mathf.Pow(1f - progress, 1.55f);
                float distance = (state.IsUi ? 46f : 0.46f) * strength;
                float aftershock = Mathf.Sin(progress * Mathf.PI * 7f) * envelope;
                Vector3 offset = -direction * distance * (envelope * 0.72f + aftershock * 0.28f);
                Vector3 impactScale = ScaleMagnitude(new Vector3(1.22f, 0.68f, 1.22f), strength);
                Vector3 recoilScale = ScaleMagnitude(new Vector3(0.9f, 1.16f, 0.9f), strength);
                Vector3 scale = progress <= 0.11f
                    ? Vector3.LerpUnclamped(Vector3.one, impactScale, EvaluateEase(progress / 0.11f, Ease.OutQuad))
                    : progress <= 0.38f
                        ? Vector3.LerpUnclamped(impactScale, recoilScale, EvaluateEase((progress - 0.11f) / 0.27f, Ease.OutCubic))
                        : Vector3.LerpUnclamped(recoilScale, Vector3.one, EvaluateEase((progress - 0.38f) / 0.62f, Ease.OutBack));
                Color flash = Color.LerpUnclamped(Color.white, resolvedColor, Mathf.Clamp01((progress - 0.06f) / 0.3f));
                state.SetGroundedPose(offset, Vector3.Scale(state.BaseScale, scale), Quaternion.Euler(0f, 0f, -direction.x * 5f * envelope));
                state.SetFlash(flash, FlashEnvelope(progress, 0.04f, 0.58f));
            });
        }

        public static Tween CreateCooldownReady(GameObject target, float duration, Color? flashColor, TweenOptions options)
        {
            float strength = ResolveStrength(options);
            Color resolvedColor = flashColor ?? ReadyColor;
            return CreateTransient(target, duration, options, false, (state, progress) =>
            {
                float angle = EvaluateEase(progress, Ease.OutCubic) * 360f * strength;
                float pulse = Mathf.Sin(progress * Mathf.PI);
                float settle = Mathf.Sin(progress * Mathf.PI * 2f) * (1f - progress);
                float scale = 1f + (0.16f * pulse + 0.035f * settle) * strength;
                Quaternion rotation = state.IsUi ? Quaternion.Euler(0f, 0f, angle) : Quaternion.Euler(0f, angle, 0f);
                state.SetPose(Vector3.up * (state.IsUi ? 7f : 0.07f) * pulse * strength, state.BaseScale * scale, rotation);
                state.SetFlash(resolvedColor, Mathf.Sin(progress * Mathf.PI) * 0.72f);
            });
        }

        public static Tween CreateLevelUp(GameObject target, float duration, Color? flashColor, TweenOptions options)
        {
            float strength = ResolveStrength(options);
            Color resolvedColor = flashColor ?? LevelColor;
            return CreateTransient(target, duration, options, false, (state, progress) =>
            {
                float height = (state.IsUi ? 42f : 0.42f) * strength;
                float lift = Mathf.Sin(progress * Mathf.PI) * height;
                float pulses = Mathf.Sin(progress * Mathf.PI * 3f) * (1f - progress);
                float reveal = Mathf.Sin(progress * Mathf.PI);
                float scale = 1f + (0.17f * reveal + 0.045f * pulses) * strength;
                float angle = EvaluateEase(progress, Ease.InOutCubic) * 360f * strength;
                Quaternion rotation = state.IsUi ? Quaternion.Euler(0f, 0f, angle) : Quaternion.Euler(0f, angle, 0f);
                state.SetPose(Vector3.up * lift, state.BaseScale * scale, rotation);
                state.SetFlash(resolvedColor, Mathf.Clamp01(reveal * 0.64f + Mathf.Max(0f, pulses) * 0.18f));
            });
        }

        public static Tween CreateLowHealthWarning(GameObject target, float duration, Color? flashColor, TweenOptions options)
        {
            float strength = ResolveStrength(options);
            Color resolvedColor = flashColor ?? LowHealthColor;
            return CreateTransient(target, duration, options, false, (state, progress) =>
            {
                float firstBeat = Pulse(progress, 0.04f, 0.28f);
                float secondBeat = Pulse(progress, 0.34f, 0.62f) * 0.78f;
                float beat = Mathf.Max(firstBeat, secondBeat);
                float scale = 1f + 0.15f * beat * strength;
                state.SetPose(Vector3.zero, state.BaseScale * scale, Quaternion.identity);
                state.SetFlash(resolvedColor, beat * 0.82f);
            });
        }

        public static Tween CreatePickupCollect(GameObject target, Vector3 destination, float? arcHeight, float duration, TweenOptions options, bool local)
        {
            ValidateRequest(duration);
            ValidateVector(destination, nameof(destination));
            if (arcHeight.HasValue) ValidateFinite(arcHeight.Value, nameof(arcHeight));
            float strength = ResolveStrength(options);
            var state = new FeedbackState(target, local);
            return NormalizedTweenTimeline.Create(
                target,
                duration,
                options.SetEase(Ease.Linear),
                () => state.Initialize(false, false, true),
                progress =>
                {
                    float travel = EvaluateEase(progress, options.Ease ?? Ease.InOutCubic);
                    float resolvedHeight = (arcHeight ?? (state.IsUi ? 145f : 2f)) * strength;
                    Vector3 position = DestinationMotionUtility.EvaluateArc(state.BasePosition, destination, resolvedHeight, travel);
                    float scale = EvaluatePickupScale(progress, strength);
                    float alpha = progress <= 0.56f ? state.BaseAlpha : Mathf.LerpUnclamped(state.BaseAlpha, 0f, EvaluateEase((progress - 0.56f) / 0.44f, Ease.InCubic));
                    float angle = EvaluateEase(progress, Ease.InOutSine) * 180f * strength;
                    Quaternion rotationOffset = state.IsUi ? Quaternion.Euler(0f, 0f, angle) : Quaternion.Euler(0f, angle, 0f);
                    state.SetPickupPose(position, state.BaseScale * scale, rotationOffset, alpha);
                },
                () => state.SetPickupEndpoint(destination),
                state.RestoreAll,
                state.RestoreAll,
                state.RestoreVisuals);
        }

        private static Tween CreateTransient(GameObject target, float duration, TweenOptions options, bool grounded, Action<FeedbackState, float> evaluator)
        {
            ValidateRequest(duration);
            var state = new FeedbackState(target, true);
            return NormalizedTweenTimeline.Create(
                target,
                duration,
                options.SetEase(Ease.Linear),
                () => state.Initialize(grounded, true, false),
                progress => evaluator(state, progress),
                state.RestoreAll,
                state.RestoreAll,
                state.RestoreAll,
                state.RestoreAll);
        }

        private static Vector3 EvaluateDamageScale(float progress, float strength)
        {
            Vector3 squash = ScaleMagnitude(new Vector3(1.15f, 0.78f, 1.15f), strength);
            Vector3 recoil = ScaleMagnitude(new Vector3(0.94f, 1.08f, 0.94f), strength);
            if (progress <= 0.12f) return Vector3.LerpUnclamped(Vector3.one, squash, EvaluateEase(progress / 0.12f, Ease.OutQuad));
            if (progress <= 0.46f) return Vector3.LerpUnclamped(squash, recoil, EvaluateEase((progress - 0.12f) / 0.34f, Ease.OutCubic));
            return Vector3.LerpUnclamped(recoil, Vector3.one, EvaluateEase((progress - 0.46f) / 0.54f, Ease.OutBack));
        }

        private static Vector3 EvaluateSuccessScale(float progress, float strength)
        {
            Vector3 pop = ScaleMagnitude(Vector3.one * 1.16f, strength);
            Vector3 settle = ScaleMagnitude(Vector3.one * 0.96f, strength);
            if (progress <= 0.24f) return Vector3.LerpUnclamped(Vector3.one, pop, EvaluateEase(progress / 0.24f, Ease.OutBack));
            if (progress <= 0.52f) return Vector3.LerpUnclamped(pop, settle, EvaluateEase((progress - 0.24f) / 0.28f, Ease.InOutSine));
            return Vector3.LerpUnclamped(settle, Vector3.one, EvaluateEase((progress - 0.52f) / 0.48f, Ease.OutBack));
        }

        private static float EvaluateSuccessBounce(float progress)
        {
            if (progress >= 0.12f && progress <= 0.5f) return Parabola((progress - 0.12f) / 0.38f);
            if (progress >= 0.54f && progress <= 0.84f) return Parabola((progress - 0.54f) / 0.3f) * 0.42f;
            return 0f;
        }

        private static Vector3 EvaluateRewardScale(float progress, float strength)
        {
            Vector3 anticipation = ScaleMagnitude(new Vector3(1.08f, 0.82f, 1.08f), strength);
            Vector3 reveal = ScaleMagnitude(Vector3.one * 1.2f, strength);
            Vector3 pulse = ScaleMagnitude(Vector3.one * 0.94f, strength);
            if (progress <= 0.13f) return Vector3.LerpUnclamped(Vector3.one, anticipation, EvaluateEase(progress / 0.13f, Ease.InQuad));
            if (progress <= 0.46f) return Vector3.LerpUnclamped(anticipation, reveal, EvaluateEase((progress - 0.13f) / 0.33f, Ease.OutBack));
            if (progress <= 0.72f) return Vector3.LerpUnclamped(reveal, pulse, EvaluateEase((progress - 0.46f) / 0.26f, Ease.InOutSine));
            return Vector3.LerpUnclamped(pulse, Vector3.one, EvaluateEase((progress - 0.72f) / 0.28f, Ease.OutBack));
        }

        private static float EvaluateRewardAngle(float progress)
        {
            if (progress <= 0.13f) return Mathf.LerpUnclamped(0f, -8f, EvaluateEase(progress / 0.13f, Ease.InQuad));
            return Mathf.LerpUnclamped(-8f, 360f, EvaluateEase((progress - 0.13f) / 0.87f, Ease.OutCubic));
        }

        private static float EvaluatePickupScale(float progress, float strength)
        {
            float punch = 1f + 0.16f * strength;
            if (progress <= 0.12f) return Mathf.LerpUnclamped(1f, punch, EvaluateEase(progress / 0.12f, Ease.OutBack));
            if (progress <= 0.32f) return Mathf.LerpUnclamped(punch, 1f, EvaluateEase((progress - 0.12f) / 0.2f, Ease.OutQuad));
            if (progress <= 0.52f) return 1f;
            return Mathf.LerpUnclamped(1f, 0f, EvaluateEase((progress - 0.52f) / 0.48f, Ease.InCubic));
        }

        private static float FlashEnvelope(float progress, float peak, float end)
        {
            if (progress <= peak) return EvaluateEase(progress / peak, Ease.OutQuad);
            if (progress >= end) return 0f;
            return 1f - EvaluateEase((progress - peak) / (end - peak), Ease.InQuad);
        }

        private static float Pulse(float progress, float start, float end)
        {
            if (progress <= start || progress >= end) return 0f;
            return Mathf.Sin((progress - start) / (end - start) * Mathf.PI);
        }

        private static Vector3 ResolveDirection(Vector3 direction, string parameterName)
        {
            ValidateVector(direction, parameterName);
            if (direction.sqrMagnitude <= 0.000001f) throw new ArgumentException("Impact direction cannot be zero.", parameterName);
            return direction.normalized;
        }

        private static Vector3 ScaleMagnitude(Vector3 scale, float strength) => Vector3.one + (scale - Vector3.one) * strength;
        private static float Parabola(float progress) => 4f * Mathf.Clamp01(progress) * (1f - Mathf.Clamp01(progress));
        private static float EvaluateEase(float progress, Ease ease) => DOVirtual.EasedValue(0f, 1f, Mathf.Clamp01(progress), ease);

        private static float ResolveStrength(TweenOptions options)
        {
            float strength = options.Strength ?? 1f;
            ValidateFinite(strength, nameof(TweenOptions.Strength));
            if (strength < 0f) throw new ArgumentOutOfRangeException(nameof(TweenOptions.Strength), strength, "Strength cannot be negative.");
            return strength;
        }

        private static void ValidateRequest(float duration)
        {
            ValidateFinite(duration, nameof(duration));
            if (duration <= 0f) throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration must be greater than zero.");
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

        private sealed class FeedbackState
        {
            private readonly GameObject _target;
            private readonly bool _local;
            private DestinationMotionUtility.PositionBinding _positionBinding;
            private DestinationMotionUtility.GroundedHopPose _groundedPose;
            private TweenTargetUtility.TweenColorBinding _colorBinding;
            private TweenTargetUtility.TweenAlphaBinding _alphaBinding;
            private bool _hasColor;
            private bool _hasAlpha;
            private bool _initialized;

            public FeedbackState(GameObject target, bool local)
            {
                _target = target;
                _local = local;
            }

            public bool IsUi { get; private set; }
            public Vector3 BasePosition { get; private set; }
            public Vector3 BaseScale { get; private set; }
            public Quaternion BaseRotation { get; private set; }
            public Color BaseColor { get; private set; }
            public float BaseAlpha { get; private set; } = 1f;

            public void Initialize(bool grounded, bool captureColor, bool captureAlpha)
            {
                if (_initialized) return;
                Transform transform = _target.transform;
                _positionBinding = new DestinationMotionUtility.PositionBinding(_target, _local);
                IsUi = transform is RectTransform;
                BasePosition = _positionBinding.Get();
                BaseScale = transform.localScale;
                BaseRotation = transform.localRotation;

                if (grounded)
                {
                    _groundedPose = new DestinationMotionUtility.GroundedHopPose(_positionBinding, transform, BaseScale, DestinationMotionUtility.GetGroundAnchor(transform), _local);
                    _groundedPose.Initialize(BasePosition);
                }

                if (captureColor && TweenTargetUtility.TryGetColorBinding(_target, out _colorBinding))
                {
                    BaseColor = _colorBinding.GetColor();
                    _hasColor = true;
                }

                if (captureAlpha && TweenTargetUtility.TryGetAlphaBinding(_target, out _alphaBinding))
                {
                    BaseAlpha = _alphaBinding.GetAlpha();
                    _hasAlpha = true;
                }

                _initialized = true;
            }

            public void SetPose(Vector3 positionOffset, Vector3 scale, Quaternion rotationOffset)
            {
                if (!_initialized || _target == null) return;
                Transform transform = _target.transform;
                _positionBinding.Set(BasePosition + positionOffset);
                transform.localScale = scale;
                transform.localRotation = BaseRotation * rotationOffset;
            }

            public void SetGroundedPose(Vector3 positionOffset, Vector3 scale, Quaternion rotationOffset)
            {
                if (!_initialized || _target == null) return;
                _target.transform.localRotation = BaseRotation * rotationOffset;
                _groundedPose.SetBasePosition(BasePosition + positionOffset);
                _groundedPose.SetScale(scale);
            }

            public void SetFlash(Color flashColor, float intensity)
            {
                if (!_hasColor || _target == null) return;
                flashColor.a = BaseColor.a;
                _colorBinding.SetColor(Color.LerpUnclamped(BaseColor, flashColor, Mathf.Clamp01(intensity)));
            }

            public void SetPickupPose(Vector3 position, Vector3 scale, Quaternion rotationOffset, float alpha)
            {
                if (!_initialized || _target == null) return;
                _positionBinding.Set(position);
                Transform transform = _target.transform;
                transform.localScale = scale;
                transform.localRotation = BaseRotation * rotationOffset;
                if (_hasAlpha) _alphaBinding.SetAlpha(alpha);
            }

            public void SetPickupEndpoint(Vector3 destination)
            {
                if (!_initialized || _target == null) return;
                _positionBinding.Set(destination);
                Transform transform = _target.transform;
                transform.localScale = Vector3.zero;
                transform.localRotation = BaseRotation;
                if (_hasAlpha) _alphaBinding.SetAlpha(0f);
            }

            public void RestoreAll()
            {
                if (!_initialized || _target == null) return;
                _target.transform.localRotation = BaseRotation;
                if (_groundedPose != null) _groundedPose.RestoreAt(BasePosition);
                else
                {
                    _positionBinding.Set(BasePosition);
                    _target.transform.localScale = BaseScale;
                }

                RestoreBindings();
            }

            public void RestoreVisuals()
            {
                if (!_initialized || _target == null) return;
                Transform transform = _target.transform;
                transform.localScale = BaseScale;
                transform.localRotation = BaseRotation;
                RestoreBindings();
            }

            private void RestoreBindings()
            {
                if (_hasColor) _colorBinding.Restore(BaseColor);
                if (_hasAlpha) _alphaBinding.Restore(BaseAlpha);
            }
        }
    }
}
