using System;
using TMPro;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>One-line TextMesh Pro text and value animations backed by composable TweenBuilder operations.</summary>
    public static class TMPTextAnimationExtensions
    {
        public static TweenHandle TypewriterReveal(this TMP_Text text, TextAnimationUnit unit = TextAnimationUnit.Character, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TypewriterReveal(unit, duration).Play();

        public static TweenHandle TypewriterHide(this TMP_Text text, TextAnimationUnit unit = TextAnimationUnit.Character, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TypewriterHide(unit, duration).Play();

        public static TweenHandle NumberCountTo(this TMP_Text text, double fromValue, double toValue, string format = "0", float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).NumberCountTo(fromValue, toValue, format, duration).Play();

        public static TweenHandle NumberCountTo(this TMP_Text text, double fromValue, double toValue, Func<double, string> formatter, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).NumberCountTo(fromValue, toValue, formatter, duration).Play();

        public static TweenHandle TextStaggerIn(this TMP_Text text, TextAnimationUnit unit = TextAnimationUnit.Character,
            StaggerOrder order = StaggerOrder.FirstToLast, UISequenceDirection direction = UISequenceDirection.Up, float distance = 18f,
            float unitStagger = 0.025f, int seed = 1337, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextStaggerIn(unit, order, direction, distance, unitStagger, seed, duration).Play();

        public static TweenHandle TextStaggerOut(this TMP_Text text, TextAnimationUnit unit = TextAnimationUnit.Character,
            StaggerOrder order = StaggerOrder.LastToFirst, UISequenceDirection direction = UISequenceDirection.Up, float distance = 18f,
            float unitStagger = 0.025f, int seed = 1337, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextStaggerOut(unit, order, direction, distance, unitStagger, seed, duration).Play();

        public static TweenHandle TextWave(this TMP_Text text, UISequenceDirection direction = UISequenceDirection.Up, float amplitude = 12f, int waveCount = 1, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextWave(direction, amplitude, waveCount, duration).Play();

        public static TweenHandle TextCharacterBounce(this TMP_Text text, UISequenceDirection direction = UISequenceDirection.Up, float amplitude = 14f, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextCharacterBounce(direction, amplitude, duration).Play();

        public static TweenHandle TextColorSweep(this TMP_Text text, Color? highlightColor = null, float width = 2.5f, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextColorSweep(highlightColor, width, duration).Play();

        public static TweenHandle TextGlitch(this TMP_Text text, float distance = 6f, int seed = 1337, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextGlitch(distance, seed, duration).Play();

        public static TweenHandle TextEmphasis(this TMP_Text text, UISequenceDirection direction = UISequenceDirection.Up, float amplitude = 8f, int startCharacter = 0, int characterCount = -1, Color? highlightColor = null, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextEmphasis(direction, amplitude, startCharacter, characterCount, highlightColor, duration).Play();

        public static TweenHandle TextWiggle(this TMP_Text text, float distance = 3f, float rotation = 3f, int seed = 1337, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextWiggle(distance, rotation, seed, duration).Play();

        public static TweenHandle TextFloat(this TMP_Text text, UISequenceDirection direction = UISequenceDirection.Up, float amplitude = 8f, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextFloat(direction, amplitude, duration).Play();

        public static TweenHandle TextSwing(this TMP_Text text, float angle = 12f, TextGlyphPivot pivot = TextGlyphPivot.Top, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextSwing(angle, pivot, duration).Play();

        public static TweenHandle TextPulse(this TMP_Text text, float scaleAmount = 0.12f, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextPulse(scaleAmount, duration).Play();

        public static TweenHandle TextScatterIn(this TMP_Text text, TextAnimationUnit unit = TextAnimationUnit.Character,
            StaggerOrder order = StaggerOrder.FirstToLast, float distance = 36f, float rotation = 25f,
            float unitStagger = 0.025f, int seed = 1337, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextScatterIn(unit, order, distance, rotation, unitStagger, seed, duration).Play();

        public static TweenHandle TextScatterOut(this TMP_Text text, TextAnimationUnit unit = TextAnimationUnit.Character,
            StaggerOrder order = StaggerOrder.LastToFirst, float distance = 36f, float rotation = 25f,
            float unitStagger = 0.025f, int seed = 1337, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextScatterOut(unit, order, distance, rotation, unitStagger, seed, duration).Play();

        public static TweenHandle TextRotateIn(this TMP_Text text, TextAnimationUnit unit = TextAnimationUnit.Character,
            StaggerOrder order = StaggerOrder.FirstToLast, float angle = 90f, float unitStagger = 0.025f,
            int seed = 1337, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextRotateIn(unit, order, angle, unitStagger, seed, duration).Play();

        public static TweenHandle TextRotateOut(this TMP_Text text, TextAnimationUnit unit = TextAnimationUnit.Character,
            StaggerOrder order = StaggerOrder.LastToFirst, float angle = 90f, float unitStagger = 0.025f,
            int seed = 1337, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextRotateOut(unit, order, angle, unitStagger, seed, duration).Play();

        public static TweenHandle TextShear(this TMP_Text text, float amount = 0.28f, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextShear(amount, duration).Play();

        public static TweenHandle TextTrackingPulse(this TMP_Text text, float distance = 10f, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextTrackingPulse(distance, duration).Play();

        public static TweenHandle TextImpactRipple(this TMP_Text text, Vector2 impactPoint, float radius = 120f,
            float amplitude = 16f, float scaleAmount = 0.08f, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextImpactRipple(impactPoint, radius, amplitude, scaleAmount, duration).Play();

        public static TweenHandle TextScrambleReveal(this TMP_Text text, int seed = 1337, float? duration = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).TextScrambleReveal(seed, duration).Play();

        public static TweenHandle ScoreIncrease(this TMP_Text text, double fromValue, double toValue, string format = "0", float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).ScoreIncrease(fromValue, toValue, format, duration, flashColor).Play();

        public static TweenHandle ScoreIncrease(this TMP_Text text, double fromValue, double toValue, Func<double, string> formatter, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => RequireText(text).Tween().WithOptions(options).ScoreIncrease(fromValue, toValue, formatter, duration, flashColor).Play();

        private static TMP_Text RequireText(TMP_Text text) => text != null ? text : throw new ArgumentNullException(nameof(text));
    }
}
