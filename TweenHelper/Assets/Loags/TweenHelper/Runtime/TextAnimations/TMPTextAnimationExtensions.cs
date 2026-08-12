using System;
using TMPro;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>One-line TextMesh Pro text and value animations backed by composable TweenBuilder operations.</summary>
    public static class TMPTextAnimationExtensions
    {
        public static TweenHandle TypewriterReveal(this TMP_Text text, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TypewriterReveal(duration).Play();

        public static TweenHandle TypewriterHide(this TMP_Text text, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TypewriterHide(duration).Play();

        public static TweenHandle NumberCountTo(this TMP_Text text, double fromValue, double toValue, string format = "0", float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).NumberCountTo(fromValue, toValue, format, duration).Play();

        public static TweenHandle NumberCountTo(this TMP_Text text, double fromValue, double toValue, Func<double, string> formatter, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).NumberCountTo(fromValue, toValue, formatter, duration).Play();

        public static TweenHandle TextCharacterStaggerIn(this TMP_Text text, UISequenceDirection direction = UISequenceDirection.Up, float distance = 18f, float characterStagger = 0.025f, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextCharacterStaggerIn(direction, distance, characterStagger, duration).Play();

        public static TweenHandle TextWave(this TMP_Text text, UISequenceDirection direction = UISequenceDirection.Up, float amplitude = 12f, int waveCount = 1, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextWave(direction, amplitude, waveCount, duration).Play();

        public static TweenHandle ScoreIncrease(this TMP_Text text, double fromValue, double toValue, string format = "0", float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).ScoreIncrease(fromValue, toValue, format, duration, flashColor).Play();

        public static TweenHandle ScoreIncrease(this TMP_Text text, double fromValue, double toValue, Func<double, string> formatter, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).ScoreIncrease(fromValue, toValue, formatter, duration, flashColor).Play();

        private static TMP_Text RequireText(TMP_Text text) => text != null ? text : throw new ArgumentNullException(nameof(text));
    }
}
