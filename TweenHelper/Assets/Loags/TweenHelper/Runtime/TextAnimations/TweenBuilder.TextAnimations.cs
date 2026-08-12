using System;
using UnityEngine;

namespace LB.TweenHelper
{
    public partial class TweenBuilder
    {
        /// <summary>Reveals TextMesh Pro content character by character without changing the text string.</summary>
        public TweenBuilder TypewriterReveal(float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTypewriter(_gameObject, true, ResolveTextAnimationDuration(duration, options, 0.85f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Hides currently visible TextMesh Pro content character by character.</summary>
        public TweenBuilder TypewriterHide(float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTypewriter(_gameObject, false, ResolveTextAnimationDuration(duration, options, 0.65f), options), applyBuilderOptions: false);
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

        /// <summary>Reveals visible TextMesh Pro characters with directional offset, alpha, and stagger.</summary>
        public TweenBuilder TextCharacterStaggerIn(UISequenceDirection direction = UISequenceDirection.Up, float distance = 18f, float characterStagger = 0.025f, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateCharacterStagger(_gameObject, direction, distance, characterStagger, ResolveTextAnimationDuration(duration, options, 0.65f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Sends one or more finite directional waves across visible TextMesh Pro characters.</summary>
        public TweenBuilder TextWave(UISequenceDirection direction = UISequenceDirection.Up, float amplitude = 12f, int waveCount = 1, float? duration = null)
        {
            AddStep(options => TMPTextAnimationUtility.CreateTextWave(_gameObject, direction, amplitude, waveCount, ResolveTextAnimationDuration(duration, options, 0.8f), options), applyBuilderOptions: false);
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
