using System;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper
{
    internal enum ValueAnimationStyle
    {
        Standard,
        Drain,
        Charge,
        Alert
    }

    internal static class ValueAnimationUtility
    {
        public static Tween Create(GameObject target, float? start, float end, TMP_Text valueText, Func<float, string> formatter, ValueAnimationStyle style, float threshold, Color? accentColor, float duration, TweenOptions options)
        {
            ValidateRequest(target, start, end, formatter, style, threshold, duration, options);
            var state = new ValueAnimationState(target, valueText, formatter);
            float strength = ResolveStrength(options);
            return NormalizedTweenTimeline.Create(
                target,
                duration,
                options.SetEase(Ease.Linear),
                () => state.Initialize(start, style, threshold),
                progress => state.Apply(progress, end, style, strength, accentColor, options.Ease),
                () => state.Complete(end, style),
                state.RestoreAll,
                state.RestoreAll,
                state.RestoreAll,
                state.RestoreVisuals);
        }

        public static Func<float, string> CreateFormatter(string format)
        {
            if (string.IsNullOrEmpty(format)) throw new ArgumentException("Format cannot be null or empty.", nameof(format));
            return value => value.ToString(format, CultureInfo.CurrentCulture);
        }

        private static float ResolveStrength(TweenOptions options)
        {
            float strength = options.Strength ?? 1f;
            ValidateFinite(strength, nameof(TweenOptions.Strength));
            if (strength < 0f) throw new ArgumentOutOfRangeException(nameof(TweenOptions.Strength), strength, "Strength cannot be negative.");
            return strength;
        }

        private static void ValidateRequest(GameObject target, float? start, float end, Func<float, string> formatter, ValueAnimationStyle style, float threshold, float duration, TweenOptions options)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            if (start.HasValue) ValidateNormalized(start.Value, nameof(start));
            ValidateNormalized(end, nameof(end));
            if (style == ValueAnimationStyle.Alert) ValidateNormalized(threshold, nameof(threshold));
            ValidateFinite(duration, nameof(duration));
            if (duration <= 0f) throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration must be greater than zero.");
            if (options.SpeedBased == true) throw new NotSupportedException("Value animations do not support speed-based timing.");
        }

        private static void ValidateNormalized(float value, string parameterName)
        {
            ValidateFinite(value, parameterName);
            if (value < 0f || value > 1f) throw new ArgumentOutOfRangeException(parameterName, value, "Value must be normalized between zero and one.");
        }

        private static void ValidateFinite(float value, string parameterName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new ArgumentOutOfRangeException(parameterName, value, "Value must be finite.");
        }

        private sealed class ValueAnimationState
        {
            private static readonly Color DrainColor = new Color(1f, 0.22f, 0.16f, 1f);
            private static readonly Color ChargeColor = new Color(0.16f, 0.82f, 1f, 1f);
            private static readonly Color AlertColor = new Color(1f, 0.12f, 0.08f, 1f);

            private readonly GameObject _target;
            private readonly TMP_Text _valueText;
            private readonly Func<float, string> _formatter;
            private ValueBinding _binding;
            private TweenTargetUtility.TweenColorBinding _colorBinding;
            private bool _hasColor;
            private bool _alertEnabled;
            private float _invocationValue;
            private float _startValue;
            private Vector3 _invocationScale;
            private Vector3 _invocationLocalPosition;
            private Color _invocationColor;
            private string _invocationText;

            public ValueAnimationState(GameObject target, TMP_Text valueText, Func<float, string> formatter)
            {
                _target = target;
                _valueText = valueText;
                _formatter = formatter;
            }

            public void Initialize(float? start, ValueAnimationStyle style, float threshold)
            {
                _binding = new ValueBinding(_target);
                _invocationValue = _binding.GetNormalized();
                _startValue = start ?? _invocationValue;
                _invocationScale = _target.transform.localScale;
                _invocationLocalPosition = _target.transform.localPosition;
                _hasColor = TweenTargetUtility.TryGetColorBinding(_target, out _colorBinding);
                if (_hasColor) _invocationColor = _colorBinding.GetColor();
                if (_valueText != null) _invocationText = _valueText.text;
                _alertEnabled = style != ValueAnimationStyle.Alert || _invocationValue <= threshold;
                _binding.SetNormalized(_startValue);
                SetText(_startValue);
            }

            public void Apply(float progress, float end, ValueAnimationStyle style, float strength, Color? accentColor, Ease? requestedEase)
            {
                if (style == ValueAnimationStyle.Alert)
                {
                    ApplyAlert(progress, strength, accentColor ?? AlertColor);
                    return;
                }

                Ease defaultEase = style == ValueAnimationStyle.Drain ? Ease.InCubic : style == ValueAnimationStyle.Charge ? Ease.OutCubic : Ease.InOutCubic;
                float valueProgress = DOVirtual.EasedValue(0f, 1f, progress, requestedEase ?? defaultEase);
                float value = Mathf.LerpUnclamped(_startValue, end, valueProgress);
                _binding.SetNormalized(value);
                SetText(value);
                if (style == ValueAnimationStyle.Drain) ApplyDrain(progress, strength, accentColor ?? DrainColor);
                else if (style == ValueAnimationStyle.Charge) ApplyCharge(progress, strength, accentColor ?? ChargeColor);
            }

            public void Complete(float end, ValueAnimationStyle style)
            {
                if (style == ValueAnimationStyle.Alert)
                {
                    RestoreVisuals();
                    return;
                }

                _binding.SetNormalized(end);
                SetText(end);
                RestoreVisuals();
            }

            public void RestoreAll()
            {
                _binding.SetNormalized(_invocationValue);
                if (_valueText != null) _valueText.text = _invocationText;
                RestoreVisuals();
            }

            public void RestoreVisuals()
            {
                if (_target == null) return;
                _target.transform.localScale = _invocationScale;
                _target.transform.localPosition = _invocationLocalPosition;
                if (_hasColor) _colorBinding.Restore(_invocationColor);
            }

            private void ApplyDrain(float progress, float strength, Color accent)
            {
                float envelope = Mathf.Pow(1f - progress, 2.2f);
                float shake = Mathf.Sin(progress * Mathf.PI * 12f) * envelope * 4f * strength;
                _target.transform.localPosition = _invocationLocalPosition + Vector3.right * shake;
                float pulse = Mathf.Sin(Mathf.Clamp01(progress / 0.35f) * Mathf.PI) * envelope;
                _target.transform.localScale = _invocationScale * (1f + pulse * 0.06f * strength);
                SetAccent(accent, envelope * 0.72f);
            }

            private void ApplyCharge(float progress, float strength, Color accent)
            {
                float pulse = Mathf.Sin(progress * Mathf.PI);
                float settle = Mathf.Sin(progress * Mathf.PI * 3f) * (1f - progress);
                _target.transform.localScale = _invocationScale * (1f + (pulse * 0.1f + settle * 0.025f) * strength);
                SetAccent(accent, pulse * 0.68f);
            }

            private void ApplyAlert(float progress, float strength, Color accent)
            {
                if (!_alertEnabled) return;
                float first = Pulse(progress, 0.02f, 0.3f);
                float second = Pulse(progress, 0.36f, 0.7f) * 0.82f;
                float pulse = Mathf.Max(first, second);
                _target.transform.localScale = _invocationScale * (1f + pulse * 0.12f * strength);
                SetAccent(accent, pulse * 0.82f);
            }

            private void SetAccent(Color accent, float intensity)
            {
                if (!_hasColor) return;
                accent.a = _invocationColor.a;
                _colorBinding.SetColor(Color.LerpUnclamped(_invocationColor, accent, Mathf.Clamp01(intensity)));
            }

            private void SetText(float normalizedValue)
            {
                if (_valueText != null) _valueText.text = _formatter(normalizedValue);
            }

            private static float Pulse(float progress, float start, float end)
            {
                if (progress <= start || progress >= end) return 0f;
                return Mathf.Sin((progress - start) / (end - start) * Mathf.PI);
            }
        }

        private sealed class ValueBinding
        {
            private readonly Image _image;
            private readonly Slider _slider;

            public ValueBinding(GameObject target)
            {
                _slider = target.GetComponent<Slider>();
                if (_slider == null) _image = target.GetComponent<Image>();
                if (_slider == null && _image == null) throw new InvalidOperationException($"Value animation target '{target.name}' requires a Slider or Image component on the same GameObject.");
            }

            public float GetNormalized() => _slider != null ? _slider.normalizedValue : _image.fillAmount;

            public void SetNormalized(float value)
            {
                value = Mathf.Clamp01(value);
                if (_slider != null) _slider.normalizedValue = value;
                else _image.fillAmount = value;
            }
        }
    }
}
