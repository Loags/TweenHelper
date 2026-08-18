using System;
using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    internal enum GameplayStateAnimation
    {
        AbilityCharging,
        DodgeRoll,
        StunStart,
        StunEnd
    }

    internal static class GameplayStateFeedbackUtility
    {
        public static Tween Create(GameObject target, GameplayStateAnimation animation, float duration, Color? accentColor, TweenOptions options)
        {
            ValidateRequest(target, duration, options);
            float strength = ResolveStrength(options);
            var state = new State(target);
            return NormalizedTweenTimeline.Create(
                target,
                duration,
                options.SetEase(Ease.Linear),
                state.Initialize,
                progress => Apply(state, animation, progress, strength, accentColor),
                state.Restore,
                state.Restore,
                state.Restore,
                state.Restore);
        }

        private static void Apply(State state, GameplayStateAnimation animation, float progress, float strength, Color? accentColor)
        {
            switch (animation)
            {
                case GameplayStateAnimation.AbilityCharging:
                    ApplyAbilityCharging(state, progress, strength, accentColor ?? new Color(0.2f, 0.72f, 1f, 1f));
                    return;
                case GameplayStateAnimation.DodgeRoll:
                    ApplyDodgeRoll(state, progress, strength);
                    return;
                case GameplayStateAnimation.StunStart:
                    ApplyStunStart(state, progress, strength, accentColor ?? new Color(1f, 0.78f, 0.12f, 1f));
                    return;
                case GameplayStateAnimation.StunEnd:
                    ApplyStunEnd(state, progress, strength, accentColor ?? new Color(0.42f, 0.9f, 1f, 1f));
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(animation), animation, "Unknown gameplay-state animation.");
            }
        }

        private static void ApplyAbilityCharging(State state, float progress, float strength, Color accent)
        {
            float lift = Mathf.Sin(progress * Mathf.PI);
            float energy = Mathf.Sin(progress * Mathf.PI * 4f) * (1f - progress);
            float distance = state.IsUi ? 8f : 0.08f;
            Vector3 offset = Vector3.up * (lift * distance * strength);
            float scale = 1f + (lift * 0.12f + energy * 0.025f) * strength;
            float angle = energy * 4f * strength;
            state.Apply(offset, Vector3.one * scale, state.IsUi ? Quaternion.Euler(0f, 0f, angle) : Quaternion.Euler(0f, angle, 0f), accent, lift * 0.68f);
        }

        private static void ApplyDodgeRoll(State state, float progress, float strength)
        {
            float travel = Mathf.Sin(progress * Mathf.PI);
            float distance = (state.IsUi ? 34f : 0.34f) * strength;
            Vector3 offset = Vector3.right * travel * distance;
            float angle = DOVirtual.EasedValue(0f, 360f, progress, Ease.InOutCubic) * strength;
            float compression = Mathf.Sin(progress * Mathf.PI * 2f) * 0.08f * strength;
            Vector3 scale = new Vector3(1f + compression, 1f - compression, 1f);
            Quaternion rotation = state.IsUi ? Quaternion.Euler(0f, 0f, -angle) : Quaternion.Euler(0f, angle, 0f);
            state.Apply(offset, scale, rotation, default, 0f);
        }

        private static void ApplyStunStart(State state, float progress, float strength, Color accent)
        {
            float envelope = Mathf.Pow(1f - progress, 1.5f);
            float wobble = Mathf.Sin(progress * Mathf.PI * 8f) * envelope;
            float drop = Mathf.Sin(progress * Mathf.PI) * (state.IsUi ? 7f : 0.07f) * strength;
            float scale = 1f - Mathf.Sin(progress * Mathf.PI) * 0.08f * strength;
            Quaternion rotation = Quaternion.Euler(0f, 0f, wobble * 12f * strength);
            state.Apply(Vector3.down * drop, Vector3.one * scale, rotation, accent, Mathf.Sin(progress * Mathf.PI) * 0.74f);
        }

        private static void ApplyStunEnd(State state, float progress, float strength, Color accent)
        {
            float recovery = Mathf.Sin(progress * Mathf.PI);
            float rebound = Mathf.Sin(progress * Mathf.PI * 4f) * Mathf.Pow(1f - progress, 2f);
            float scale = 1f + recovery * 0.14f * strength;
            float distance = (state.IsUi ? 10f : 0.1f) * strength;
            state.Apply(Vector3.up * recovery * distance, Vector3.one * scale, Quaternion.Euler(0f, 0f, rebound * 4f * strength), accent, recovery * 0.62f);
        }

        private static float ResolveStrength(TweenOptions options)
        {
            float strength = options.Strength ?? 1f;
            ValidateFinite(strength, nameof(TweenOptions.Strength));
            if (strength < 0f) throw new ArgumentOutOfRangeException(nameof(TweenOptions.Strength), strength, "Strength cannot be negative.");
            return strength;
        }

        private static void ValidateRequest(GameObject target, float duration, TweenOptions options)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            ValidateFinite(duration, nameof(duration));
            if (duration <= 0f) throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration must be greater than zero.");
            if (options.SpeedBased == true) throw new NotSupportedException("Gameplay-state feedback does not support speed-based timing.");
        }

        private static void ValidateFinite(float value, string parameterName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new ArgumentOutOfRangeException(parameterName, value, "Value must be finite.");
        }

        private sealed class State
        {
            private readonly GameObject _target;
            private DestinationMotionUtility.PositionBinding _positionBinding;
            private TweenTargetUtility.TweenColorBinding _colorBinding;
            private bool _hasColor;
            private Vector3 _position;
            private Vector3 _scale;
            private Quaternion _rotation;
            private Color _color;

            public State(GameObject target)
            {
                _target = target;
            }

            public bool IsUi { get; private set; }

            public void Initialize()
            {
                IsUi = _target.transform is RectTransform;
                _positionBinding = new DestinationMotionUtility.PositionBinding(_target, true);
                _position = _positionBinding.Get();
                _scale = _target.transform.localScale;
                _rotation = _target.transform.localRotation;
                _hasColor = TweenTargetUtility.TryGetColorBinding(_target, out _colorBinding);
                if (_hasColor) _color = _colorBinding.GetColor();
            }

            public void Apply(Vector3 positionOffset, Vector3 scaleMultiplier, Quaternion rotationOffset, Color accent, float accentStrength)
            {
                _positionBinding.Set(_position + positionOffset);
                _target.transform.localScale = Vector3.Scale(_scale, scaleMultiplier);
                _target.transform.localRotation = _rotation * rotationOffset;
                if (!_hasColor || accentStrength <= 0f) return;
                accent.a = _color.a;
                _colorBinding.SetColor(Color.LerpUnclamped(_color, accent, Mathf.Clamp01(accentStrength)));
            }

            public void Restore()
            {
                if (_target == null) return;
                _positionBinding.Set(_position);
                _target.transform.localScale = _scale;
                _target.transform.localRotation = _rotation;
                if (_hasColor) _colorBinding.Restore(_color);
            }
        }
    }
}
