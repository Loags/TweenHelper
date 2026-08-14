using System;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace LB.TweenHelper
{
    internal static class TMPTextAnimationUtility
    {
        private static readonly Color ScoreFlashColor = new Color(1f, 0.72f, 0.16f, 1f);
        private static readonly Color SweepColor = new Color(0.18f, 0.88f, 1f, 1f);
        private static readonly Color EmphasisColor = new Color(1f, 0.72f, 0.16f, 1f);

        public static Func<double, string> CreateFormatter(string format)
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            return value => value.ToString(format, CultureInfo.CurrentCulture);
        }

        public static Tween CreateTypewriter(GameObject target, bool reveal, float duration, TweenOptions options)
        {
            ValidateRequest(target, duration, options);
            TMP_Text text = RequireText(target);
            var state = new TextVisibilityState(text, reveal);
            Ease ease = options.Ease ?? Ease.Linear;

            return CreateTimeline(target, duration, options, state.Initialize, progress => state.Apply(EaseValue(progress, ease)), state.Complete, state.Restore, null);
        }

        public static Tween CreateNumberCount(GameObject target, double fromValue, double toValue, Func<double, string> formatter, float duration, TweenOptions options)
        {
            ValidateRequest(target, duration, options);
            ValidateNumberRange(fromValue, toValue);
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            TMP_Text text = RequireText(target);
            var state = new NumberTextState(text, fromValue, toValue, formatter);
            Ease ease = options.Ease ?? Ease.OutCubic;

            return CreateTimeline(target, duration, options, state.Initialize, progress => state.Apply(EaseValue(progress, ease)), state.Complete, state.Restore, null);
        }

        public static Tween CreateCharacterStagger(GameObject target, UISequenceDirection direction, float distance, float characterStagger, float duration, TweenOptions options)
        {
            ValidateRequest(target, duration, options);
            ValidateFinite(distance, nameof(distance));
            ValidateFinite(characterStagger, nameof(characterStagger));
            if (distance < 0f) throw new ArgumentOutOfRangeException(nameof(distance), distance, "Distance cannot be negative.");
            if (characterStagger < 0f) throw new ArgumentOutOfRangeException(nameof(characterStagger), characterStagger, "Character stagger cannot be negative.");
            Vector3 directionVector = DirectionVector(direction);
            float strength = ResolveStrength(options);
            var state = new TMPCharacterMeshState(RequireText(target));
            Ease ease = options.Ease ?? Ease.Linear;

            return CreateTimeline(target, duration, options, state.Initialize, progress => state.ApplyStagger(EaseValue(progress, ease), directionVector, distance, characterStagger, duration, strength), state.Restore, state.Restore, state.Restore);
        }

        public static Tween CreateCharacterStaggerOut(GameObject target, UISequenceDirection direction, float distance, float characterStagger, float duration, TweenOptions options)
        {
            ValidateCharacterMotion(target, direction, distance, characterStagger, duration, options, out TMP_Text text, out Vector3 directionVector, out float strength);
            var state = new TMPCharacterMeshState(text);
            int invocationVisibleCharacters = int.MaxValue;
            Ease ease = options.Ease ?? Ease.Linear;

            void Initialize()
            {
                invocationVisibleCharacters = text.maxVisibleCharacters;
                text.maxVisibleCharacters = int.MaxValue;
                state.Initialize();
            }

            void Restore()
            {
                state.Restore();
                text.maxVisibleCharacters = invocationVisibleCharacters;
            }

            void Complete()
            {
                state.Restore();
                text.maxVisibleCharacters = 0;
            }

            return CreateTimeline(target, duration, options, Initialize, progress => state.ApplyStaggerOut(EaseValue(progress, ease), directionVector, distance, characterStagger, duration, strength), Complete, Restore, Restore);
        }

        public static Tween CreateTextWave(GameObject target, UISequenceDirection direction, float amplitude, int waveCount, float duration, TweenOptions options)
        {
            ValidateRequest(target, duration, options);
            ValidateFinite(amplitude, nameof(amplitude));
            if (amplitude < 0f) throw new ArgumentOutOfRangeException(nameof(amplitude), amplitude, "Amplitude cannot be negative.");
            if (waveCount <= 0) throw new ArgumentOutOfRangeException(nameof(waveCount), waveCount, "Wave count must be greater than zero.");
            Vector3 directionVector = DirectionVector(direction);
            float strength = ResolveStrength(options);
            var state = new TMPCharacterMeshState(RequireText(target));
            Ease ease = options.Ease ?? Ease.InOutSine;

            return CreateTimeline(target, duration, options, state.Initialize, progress => state.ApplyWave(EaseValue(progress, ease), directionVector, amplitude, waveCount, strength), state.Restore, state.Restore, state.Restore);
        }

        public static Tween CreateCharacterBounce(GameObject target, UISequenceDirection direction, float amplitude, float duration, TweenOptions options)
        {
            ValidateRequest(target, duration, options);
            ValidateFinite(amplitude, nameof(amplitude));
            if (amplitude < 0f) throw new ArgumentOutOfRangeException(nameof(amplitude), amplitude, "Amplitude cannot be negative.");
            Vector3 directionVector = DirectionVector(direction);
            float strength = ResolveStrength(options);
            var state = new TMPCharacterMeshState(RequireText(target));
            Ease ease = options.Ease ?? Ease.InOutSine;

            return CreateTimeline(target, duration, options, state.Initialize, progress => state.ApplyBounce(EaseValue(progress, ease), directionVector, amplitude, strength), state.Restore, state.Restore, state.Restore);
        }

        public static Tween CreateColorSweep(GameObject target, Color? highlightColor, float width, float duration, TweenOptions options)
        {
            ValidateRequest(target, duration, options);
            ValidateFinite(width, nameof(width));
            if (width <= 0f) throw new ArgumentOutOfRangeException(nameof(width), width, "Sweep width must be greater than zero.");
            float strength = ResolveStrength(options);
            var state = new TMPCharacterMeshState(RequireText(target));
            Ease ease = options.Ease ?? Ease.InOutSine;

            return CreateTimeline(target, duration, options, state.Initialize, progress => state.ApplyColorSweep(EaseValue(progress, ease), highlightColor ?? SweepColor, width, strength), state.Restore, state.Restore, state.Restore);
        }

        public static Tween CreateGlitch(GameObject target, float distance, int seed, float duration, TweenOptions options)
        {
            ValidateRequest(target, duration, options);
            ValidateFinite(distance, nameof(distance));
            if (distance < 0f) throw new ArgumentOutOfRangeException(nameof(distance), distance, "Distance cannot be negative.");
            float strength = ResolveStrength(options);
            var state = new TMPCharacterMeshState(RequireText(target));
            Ease ease = options.Ease ?? Ease.Linear;

            return CreateTimeline(target, duration, options, state.Initialize, progress => state.ApplyGlitch(EaseValue(progress, ease), distance, seed, strength), state.Restore, state.Restore, state.Restore);
        }

        public static Tween CreateEmphasis(GameObject target, UISequenceDirection direction, float amplitude, int startCharacter, int characterCount, Color? highlightColor, float duration, TweenOptions options)
        {
            ValidateRequest(target, duration, options);
            ValidateFinite(amplitude, nameof(amplitude));
            if (amplitude < 0f) throw new ArgumentOutOfRangeException(nameof(amplitude), amplitude, "Amplitude cannot be negative.");
            if (startCharacter < 0) throw new ArgumentOutOfRangeException(nameof(startCharacter), startCharacter, "Start character cannot be negative.");
            if (characterCount == 0 || characterCount < -1) throw new ArgumentOutOfRangeException(nameof(characterCount), characterCount, "Character count must be -1 or greater than zero.");
            Vector3 directionVector = DirectionVector(direction);
            float strength = ResolveStrength(options);
            var state = new TMPCharacterMeshState(RequireText(target));
            Ease ease = options.Ease ?? Ease.InOutSine;

            return CreateTimeline(target, duration, options, state.Initialize, progress => state.ApplyEmphasis(EaseValue(progress, ease), directionVector, amplitude, startCharacter, characterCount, highlightColor ?? EmphasisColor, strength), state.Restore, state.Restore, state.Restore);
        }

        public static Tween CreateScrambleReveal(GameObject target, int seed, float duration, TweenOptions options)
        {
            ValidateRequest(target, duration, options);
            var state = new ScrambleTextState(RequireText(target), seed);
            Ease ease = options.Ease ?? Ease.Linear;
            return CreateTimeline(target, duration, options, state.Initialize, progress => state.Apply(EaseValue(progress, ease)), state.Complete, state.Restore, state.Restore);
        }

        public static Tween CreateScoreIncrease(GameObject target, double fromValue, double toValue, Func<double, string> formatter, Color? flashColor, float duration, TweenOptions options)
        {
            ValidateRequest(target, duration, options);
            ValidateNumberRange(fromValue, toValue);
            if (toValue < fromValue) throw new ArgumentOutOfRangeException(nameof(toValue), toValue, "ScoreIncrease requires a destination greater than or equal to the starting value.");
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            float strength = ResolveStrength(options);
            var state = new ScoreTextState(RequireText(target), fromValue, toValue, formatter, flashColor ?? ScoreFlashColor);
            Ease countEase = options.Ease ?? Ease.OutCubic;

            void Evaluate(float progress)
            {
                state.ApplyValue(EaseValue(progress, countEase));
                state.ApplyVisual(EvaluateScoreScale(progress, strength), FlashEnvelope(progress) * Mathf.Clamp01(strength));
            }

            return CreateTimeline(target, duration, options, state.Initialize, Evaluate, state.Complete, state.RestoreAll, state.RestoreVisuals);
        }

        private static Tween CreateTimeline(GameObject owner, float duration, TweenOptions options, Action initialize, Action<float> evaluate, Action complete, Action rewind, Action interruptedKill)
        {
            return NormalizedTweenTimeline.Create(
                owner,
                duration,
                options.SetEase(Ease.Linear),
                initialize,
                evaluate,
                complete,
                rewind,
                rewind,
                interruptedKill,
                () => evaluate(0f));
        }

        private static TMP_Text RequireText(GameObject target)
        {
            var text = target.GetComponent<TMP_Text>();
            if (text == null) throw new InvalidOperationException($"Text animation target '{target.name}' requires a TMP_Text component on the same GameObject.");
            return text;
        }

        private static void ValidateCharacterMotion(GameObject target, UISequenceDirection direction, float distance, float characterStagger, float duration, TweenOptions options, out TMP_Text text, out Vector3 directionVector, out float strength)
        {
            ValidateRequest(target, duration, options);
            ValidateFinite(distance, nameof(distance));
            ValidateFinite(characterStagger, nameof(characterStagger));
            if (distance < 0f) throw new ArgumentOutOfRangeException(nameof(distance), distance, "Distance cannot be negative.");
            if (characterStagger < 0f) throw new ArgumentOutOfRangeException(nameof(characterStagger), characterStagger, "Character stagger cannot be negative.");
            text = RequireText(target);
            directionVector = DirectionVector(direction);
            strength = ResolveStrength(options);
        }

        private static Vector3 DirectionVector(UISequenceDirection direction)
        {
            switch (direction)
            {
                case UISequenceDirection.Up: return Vector3.up;
                case UISequenceDirection.Down: return Vector3.down;
                case UISequenceDirection.Left: return Vector3.left;
                case UISequenceDirection.Right: return Vector3.right;
                default: throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown text animation direction.");
            }
        }

        private static float EvaluateScoreScale(float progress, float strength)
        {
            float peak = 1f + 0.16f * strength;
            float settle = Mathf.Max(0.1f, 1f - 0.035f * strength);
            if (progress <= 0.24f) return Mathf.LerpUnclamped(1f, peak, EaseValue(progress / 0.24f, Ease.OutBack));
            if (progress <= 0.58f) return Mathf.LerpUnclamped(peak, settle, EaseValue((progress - 0.24f) / 0.34f, Ease.InOutSine));
            return Mathf.LerpUnclamped(settle, 1f, EaseValue((progress - 0.58f) / 0.42f, Ease.OutBack));
        }

        private static float FlashEnvelope(float progress)
        {
            if (progress <= 0.16f) return EaseValue(progress / 0.16f, Ease.OutQuad);
            if (progress >= 0.76f) return 0f;
            return 1f - EaseValue((progress - 0.16f) / 0.6f, Ease.InQuad);
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
            if (options.SpeedBased == true) throw new NotSupportedException("Text and value animations do not support speed-based timing.");
        }

        private static void ValidateNumberRange(double fromValue, double toValue)
        {
            ValidateFinite(fromValue, nameof(fromValue));
            ValidateFinite(toValue, nameof(toValue));
            ValidateFinite(toValue - fromValue, "numberRange");
        }

        private static void ValidateFinite(float value, string parameterName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new ArgumentOutOfRangeException(parameterName, value, "Value must be finite.");
        }

        private static void ValidateFinite(double value, string parameterName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) throw new ArgumentOutOfRangeException(parameterName, value, "Value must be finite.");
        }

        private static float EaseValue(float progress, Ease ease)
            => DOVirtual.EasedValue(0f, 1f, Mathf.Clamp01(progress), ease);

        private sealed class TextVisibilityState
        {
            private readonly TMP_Text _text;
            private readonly bool _reveal;
            private int _invocationVisibleCharacters;
            private int _start;
            private int _end;

            public TextVisibilityState(TMP_Text text, bool reveal)
            {
                _text = text;
                _reveal = reveal;
            }

            public void Initialize()
            {
                _text.ForceMeshUpdate();
                _invocationVisibleCharacters = _text.maxVisibleCharacters;
                int characterCount = _text.textInfo.characterCount;
                _start = _reveal ? 0 : Mathf.Min(_invocationVisibleCharacters, characterCount);
                if (_invocationVisibleCharacters == int.MaxValue) _start = characterCount;
                _end = _reveal ? characterCount : 0;
            }

            public void Apply(float progress)
            {
                _text.maxVisibleCharacters = Mathf.RoundToInt(Mathf.LerpUnclamped(_start, _end, progress));
            }

            public void Complete()
            {
                _text.maxVisibleCharacters = _reveal ? int.MaxValue : 0;
            }

            public void Restore()
            {
                _text.maxVisibleCharacters = _invocationVisibleCharacters;
            }
        }

        private sealed class NumberTextState
        {
            private readonly TMP_Text _text;
            private readonly double _fromValue;
            private readonly double _toValue;
            private readonly Func<double, string> _formatter;
            private string _invocationText;
            private string _lastText;

            public NumberTextState(TMP_Text text, double fromValue, double toValue, Func<double, string> formatter)
            {
                _text = text;
                _fromValue = fromValue;
                _toValue = toValue;
                _formatter = formatter;
            }

            public void Initialize()
            {
                _invocationText = _text.text;
            }

            public void Apply(float progress)
            {
                SetText(Format(Lerp(_fromValue, _toValue, progress)));
            }

            public void Complete()
            {
                SetText(Format(_toValue));
            }

            public void Restore()
            {
                SetText(_invocationText);
            }

            private string Format(double value) => _formatter(value) ?? string.Empty;

            private void SetText(string value)
            {
                if (_lastText == value && _text.text == value) return;
                _lastText = value;
                _text.text = value;
            }
        }

        private sealed class ScoreTextState
        {
            private readonly TMP_Text _text;
            private readonly Transform _transform;
            private readonly double _fromValue;
            private readonly double _toValue;
            private readonly Func<double, string> _formatter;
            private readonly Color _flashColor;
            private string _invocationText;
            private string _lastText;
            private Vector3 _baseScale;
            private Quaternion _baseRotation;
            private Color _baseColor;

            public ScoreTextState(TMP_Text text, double fromValue, double toValue, Func<double, string> formatter, Color flashColor)
            {
                _text = text;
                _transform = text.transform;
                _fromValue = fromValue;
                _toValue = toValue;
                _formatter = formatter;
                _flashColor = flashColor;
            }

            public void Initialize()
            {
                _invocationText = _text.text;
                _baseScale = _transform.localScale;
                _baseRotation = _transform.localRotation;
                _baseColor = _text.color;
            }

            public void ApplyValue(float progress)
            {
                SetText(Format(Lerp(_fromValue, _toValue, progress)));
            }

            public void ApplyVisual(float scale, float flash)
            {
                _transform.localScale = _baseScale * scale;
                _transform.localRotation = _baseRotation;
                Color color = _flashColor;
                color.a = _baseColor.a;
                _text.color = Color.LerpUnclamped(_baseColor, color, Mathf.Clamp01(flash));
            }

            public void Complete()
            {
                SetText(Format(_toValue));
                RestoreVisuals();
            }

            public void RestoreAll()
            {
                SetText(_invocationText);
                RestoreVisuals();
            }

            public void RestoreVisuals()
            {
                _transform.localScale = _baseScale;
                _transform.localRotation = _baseRotation;
                _text.color = _baseColor;
            }

            private string Format(double value) => _formatter(value) ?? string.Empty;

            private void SetText(string value)
            {
                if (_lastText == value && _text.text == value) return;
                _lastText = value;
                _text.text = value;
            }
        }

        private sealed class ScrambleTextState
        {
            private const string Symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789#$%&?";
            private readonly TMP_Text _text;
            private readonly int _seed;
            private string _sourceText;
            private int _invocationVisibleCharacters;
            private int[] _stringIndices;
            private int _frame;

            public ScrambleTextState(TMP_Text text, int seed)
            {
                _text = text;
                _seed = seed;
            }

            public void Initialize()
            {
                _sourceText = _text.text;
                _invocationVisibleCharacters = _text.maxVisibleCharacters;
                _text.maxVisibleCharacters = int.MaxValue;
                _text.ForceMeshUpdate();
                var indices = new int[_text.textInfo.characterCount];
                int count = 0;
                for (int i = 0; i < _text.textInfo.characterCount; i++)
                {
                    TMP_CharacterInfo character = _text.textInfo.characterInfo[i];
                    if (!character.isVisible || character.stringLength != 1 || character.index < 0 || character.index >= _sourceText.Length) continue;
                    indices[count++] = character.index;
                }

                _stringIndices = new int[count];
                Array.Copy(indices, _stringIndices, count);
            }

            public void Apply(float progress)
            {
                if (_stringIndices.Length == 0)
                {
                    _text.text = _sourceText;
                    return;
                }

                int revealed = Mathf.Clamp(Mathf.FloorToInt(progress * (_stringIndices.Length + 1)), 0, _stringIndices.Length);
                int frame = Mathf.FloorToInt(progress * 24f);
                var characters = _sourceText.ToCharArray();
                for (int i = revealed; i < _stringIndices.Length; i++)
                {
                    int symbol = PositiveModulo(_seed + i * 17 + frame * 31, Symbols.Length);
                    characters[_stringIndices[i]] = Symbols[symbol];
                }

                string value = new string(characters);
                if (_frame == frame && _text.text == value) return;
                _frame = frame;
                _text.text = value;
            }

            public void Complete()
            {
                _text.text = _sourceText;
                _text.maxVisibleCharacters = int.MaxValue;
            }

            public void Restore()
            {
                _text.text = _sourceText;
                _text.maxVisibleCharacters = _invocationVisibleCharacters;
            }

            private static int PositiveModulo(int value, int modulus)
            {
                int result = value % modulus;
                return result < 0 ? result + modulus : result;
            }
        }

        private static double Lerp(double fromValue, double toValue, float progress)
            => fromValue + (toValue - fromValue) * progress;
    }
}
