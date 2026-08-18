using System;
using TMPro;
using UnityEngine;

namespace LB.TweenHelper
{
    public partial class TweenBuilder
    {
        /// <summary>Animates an Image fillAmount or Slider normalized value to a target.</summary>
        public TweenBuilder FillTo(float target, float? duration = null)
            => AddValueStep(null, target, null, ValueAnimationUtility.CreateFormatter("P0"), ValueAnimationStyle.Standard, 0f, null, duration, 0.55f);

        /// <summary>Animates an Image fillAmount or Slider normalized value between explicit normalized values.</summary>
        public TweenBuilder FillFromTo(float start, float end, float? duration = null)
            => AddValueStep(start, end, null, ValueAnimationUtility.CreateFormatter("P0"), ValueAnimationStyle.Standard, 0f, null, duration, 0.55f);

        /// <summary>Animates a normalized fill value and optional paired TextMesh Pro label.</summary>
        public TweenBuilder ValueFillTo(float target, TMP_Text valueText = null, string format = "P0", float? duration = null)
            => AddValueStep(null, target, valueText, ValueAnimationUtility.CreateFormatter(format), ValueAnimationStyle.Standard, 0f, null, duration, 0.6f);

        /// <summary>Quickly drains a normalized fill value with a diminishing impact pulse.</summary>
        public TweenBuilder FillDrain(float target, float? duration = null, Color? accentColor = null)
            => AddValueStep(null, target, null, ValueAnimationUtility.CreateFormatter("P0"), ValueAnimationStyle.Drain, 0f, accentColor, duration, 0.42f);

        /// <summary>Charges a normalized fill value with an overshoot-and-settle visual response.</summary>
        public TweenBuilder FillCharge(float target, float? duration = null, Color? accentColor = null)
            => AddValueStep(null, target, null, ValueAnimationUtility.CreateFormatter("P0"), ValueAnimationStyle.Charge, 0f, accentColor, duration, 0.68f);

        /// <summary>Plays one finite alert pulse when the current normalized value is at or below the threshold.</summary>
        public TweenBuilder FillAlertPulse(float threshold, float? duration = null, Color? alertColor = null)
            => AddValueStep(null, threshold, null, ValueAnimationUtility.CreateFormatter("P0"), ValueAnimationStyle.Alert, threshold, alertColor, duration, 0.86f);

        /// <summary>Animates a fill target and formatted TextMesh Pro value on one synchronized timeline.</summary>
        public TweenBuilder FillAndText(float start, float end, TMP_Text valueText, string format = "P0", float? duration = null)
            => FillAndText(start, end, valueText, ValueAnimationUtility.CreateFormatter(format), duration);

        /// <summary>Animates a fill target and custom-formatted TextMesh Pro value on one synchronized timeline.</summary>
        public TweenBuilder FillAndText(float start, float end, TMP_Text valueText, Func<float, string> formatter, float? duration = null)
            => AddValueStep(start, end, valueText, formatter, ValueAnimationStyle.Standard, 0f, null, duration, 0.65f);

        private TweenBuilder AddValueStep(float? start, float end, TMP_Text valueText, Func<float, string> formatter, ValueAnimationStyle style, float threshold, Color? accentColor, float? duration, float fallback)
        {
            AddStep(options => ValueAnimationUtility.Create(_gameObject, start, end, valueText, formatter, style, threshold, accentColor, ResolveTextAnimationDuration(duration, options, fallback), options), applyBuilderOptions: false);
            return this;
        }
    }
}
