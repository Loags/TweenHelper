using System;
using UnityEngine;

namespace LB.TweenHelper
{
    public partial class TweenBuilder
    {
        /// <summary>Reveals TextMesh Pro content by character, word, or line without changing the text string.</summary>
        public TweenBuilder TypewriterReveal(TextAnimationUnit unit = TextAnimationUnit.Character, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTypewriter(_gameObject, true, unit, ResolveTextAnimationDuration(duration, options, 0.85f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Hides currently visible TextMesh Pro content by character, word, or line.</summary>
        public TweenBuilder TypewriterHide(TextAnimationUnit unit = TextAnimationUnit.Character, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTypewriter(_gameObject, false, unit, ResolveTextAnimationDuration(duration, options, 0.65f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Counts between two numeric values and formats the result with the current culture.</summary>
        public TweenBuilder NumberCountTo(double fromValue, double toValue, string format = "0", float? duration = null)
            => NumberCountTo(fromValue, toValue, TMPTextAnimationUtility.CreateFormatter(format), duration);

        /// <summary>Counts between two numeric values using a caller-provided display formatter.</summary>
        public TweenBuilder NumberCountTo(double fromValue, double toValue, Func<double, string> formatter, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateNumberCount(_gameObject, fromValue, toValue, formatter, ResolveTextAnimationDuration(duration, options, 0.8f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Reveals TMP characters, words, or lines with directional movement and ordered stagger.</summary>
        public TweenBuilder TextStaggerIn(TextAnimationUnit unit = TextAnimationUnit.Character, StaggerOrder order = StaggerOrder.FirstToLast,
            UISequenceDirection direction = UISequenceDirection.Up, float distance = 18f, float unitStagger = 0.025f, int seed = 1337, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTextStagger(_gameObject, unit, order, direction, distance, unitStagger, seed,
                ResolveTextAnimationDuration(duration, options, 0.65f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Hides TMP characters, words, or lines with directional movement and ordered stagger.</summary>
        public TweenBuilder TextStaggerOut(TextAnimationUnit unit = TextAnimationUnit.Character, StaggerOrder order = StaggerOrder.LastToFirst,
            UISequenceDirection direction = UISequenceDirection.Up, float distance = 18f, float unitStagger = 0.025f, int seed = 1337, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTextStaggerOut(_gameObject, unit, order, direction, distance, unitStagger, seed,
                ResolveTextAnimationDuration(duration, options, 0.58f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Sends one or more finite directional waves across visible TextMesh Pro characters.</summary>
        public TweenBuilder TextWave(UISequenceDirection direction = UISequenceDirection.Up, float amplitude = 12f, int waveCount = 1, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTextWave(_gameObject, direction, amplitude, waveCount, ResolveTextAnimationDuration(duration, options, 0.8f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Sends a finite bounce across visible TextMesh Pro characters.</summary>
        public TweenBuilder TextCharacterBounce(UISequenceDirection direction = UISequenceDirection.Up, float amplitude = 14f, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateCharacterBounce(_gameObject, direction, amplitude, ResolveTextAnimationDuration(duration, options, 0.72f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Sweeps a temporary highlight color across visible TextMesh Pro characters.</summary>
        public TweenBuilder TextColorSweep(Color? highlightColor = null, float width = 2.5f, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateColorSweep(_gameObject, highlightColor, width, ResolveTextAnimationDuration(duration, options, 0.78f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Applies a finite deterministic offset, scale, and color glitch to visible characters.</summary>
        public TweenBuilder TextGlitch(float distance = 6f, int seed = 1337, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateGlitch(_gameObject, distance, seed, ResolveTextAnimationDuration(duration, options, 0.52f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Emphasizes all or part of a label with a temporary lift, scale pulse, and color accent.</summary>
        public TweenBuilder TextEmphasis(UISequenceDirection direction = UISequenceDirection.Up, float amplitude = 8f, int startCharacter = 0, int characterCount = -1, Color? highlightColor = null, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateEmphasis(_gameObject, direction, amplitude, startCharacter, characterCount, highlightColor, ResolveTextAnimationDuration(duration, options, 0.55f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Applies one smooth deterministic per-glyph position and rotation cycle.</summary>
        public TweenBuilder TextWiggle(float distance = 3f, float rotation = 3f, int seed = 1337, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTextWiggle(_gameObject, distance, rotation, seed,
                ResolveTextAnimationDuration(duration, options, 0.65f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Applies one smooth directional per-glyph floating cycle.</summary>
        public TweenBuilder TextFloat(UISequenceDirection direction = UISequenceDirection.Up, float amplitude = 8f, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTextFloat(_gameObject, direction, amplitude,
                ResolveTextAnimationDuration(duration, options, 0.9f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Applies one per-glyph swing cycle around the selected pivot.</summary>
        public TweenBuilder TextSwing(float angle = 12f, TextGlyphPivot pivot = TextGlyphPivot.Top, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTextSwing(_gameObject, angle, pivot,
                ResolveTextAnimationDuration(duration, options, 0.8f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Applies one phase-offset per-glyph scale cycle.</summary>
        public TweenBuilder TextPulse(float scaleAmount = 0.12f, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTextPulse(_gameObject, scaleAmount,
                ResolveTextAnimationDuration(duration, options, 0.7f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Scatters TMP groups from deterministic poses into their authored layout.</summary>
        public TweenBuilder TextScatterIn(TextAnimationUnit unit = TextAnimationUnit.Character, StaggerOrder order = StaggerOrder.FirstToLast,
            float distance = 36f, float rotation = 25f, float unitStagger = 0.025f, int seed = 1337, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTextScatter(_gameObject, true, unit, order, distance, rotation, unitStagger, seed,
                ResolveTextAnimationDuration(duration, options, 0.75f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Scatters TMP groups from their authored layout into deterministic poses.</summary>
        public TweenBuilder TextScatterOut(TextAnimationUnit unit = TextAnimationUnit.Character, StaggerOrder order = StaggerOrder.LastToFirst,
            float distance = 36f, float rotation = 25f, float unitStagger = 0.025f, int seed = 1337, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTextScatter(_gameObject, false, unit, order, distance, rotation, unitStagger, seed,
                ResolveTextAnimationDuration(duration, options, 0.65f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Rotates TMP groups into their authored layout with ordered stagger.</summary>
        public TweenBuilder TextRotateIn(TextAnimationUnit unit = TextAnimationUnit.Character, StaggerOrder order = StaggerOrder.FirstToLast,
            float angle = 90f, float unitStagger = 0.025f, int seed = 1337, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTextRotate(_gameObject, true, unit, order, angle, unitStagger, seed,
                ResolveTextAnimationDuration(duration, options, 0.65f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Rotates TMP groups out of their authored layout with ordered stagger.</summary>
        public TweenBuilder TextRotateOut(TextAnimationUnit unit = TextAnimationUnit.Character, StaggerOrder order = StaggerOrder.LastToFirst,
            float angle = 90f, float unitStagger = 0.025f, int seed = 1337, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTextRotate(_gameObject, false, unit, order, angle, unitStagger, seed,
                ResolveTextAnimationDuration(duration, options, 0.58f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Applies a finite horizontal glyph shear and returns to the authored mesh.</summary>
        public TweenBuilder TextShear(float amount = 0.28f, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTextShear(_gameObject, amount,
                ResolveTextAnimationDuration(duration, options, 0.6f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Expands glyph positions from the label center and returns without changing TMP layout.</summary>
        public TweenBuilder TextTrackingPulse(float distance = 10f, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTextTrackingPulse(_gameObject, distance,
                ResolveTextAnimationDuration(duration, options, 0.7f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Propagates one finite radial reaction from a point in TMP-local coordinates.</summary>
        public TweenBuilder TextImpactRipple(Vector2 impactPoint, float radius = 120f, float amplitude = 16f,
            float scaleAmount = 0.08f, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTextImpactRipple(_gameObject, impactPoint, radius, amplitude, scaleAmount,
                ResolveTextAnimationDuration(duration, options, 0.75f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Resolves deterministic substitute glyphs into the original rich TextMesh Pro content.</summary>
        public TweenBuilder TextScrambleReveal(int seed = 1337, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateScrambleReveal(_gameObject, seed, ResolveTextAnimationDuration(duration, options, 0.9f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Counts a score upward while applying a scale punch and temporary color flash.</summary>
        public TweenBuilder ScoreIncrease(double fromValue, double toValue, string format = "0", float? duration = null, Color? flashColor = null)
            => ScoreIncrease(fromValue, toValue, TMPTextAnimationUtility.CreateFormatter(format), duration, flashColor);

        /// <summary>Counts a score upward with custom formatting, a scale punch, and a temporary color flash.</summary>
        public TweenBuilder ScoreIncrease(double fromValue, double toValue, Func<double, string> formatter, float? duration = null, Color? flashColor = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateScoreIncrease(_gameObject, fromValue, toValue, formatter, flashColor, ResolveTextAnimationDuration(duration, options, 0.9f), options), applyBuilderOptions: false);
            return this;
        }

        private static float ResolveTextAnimationDuration(float? duration, TweenOptions options, float fallback) => duration ?? options.Duration ?? fallback;
    }
}
