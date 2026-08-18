using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper
{
    /// <summary>One-line normalized progress animations for Image and Slider targets.</summary>
    public static class ValueAnimationExtensions
    {
        public static TweenHandle FillTo(this Image image, float target, float? duration = null, TweenOptions options = default)
            => Require(image).gameObject.Tween().WithOptions(options).FillTo(target, duration).Play();

        public static TweenHandle FillTo(this Slider slider, float target, float? duration = null, TweenOptions options = default)
            => Require(slider).gameObject.Tween().WithOptions(options).FillTo(target, duration).Play();

        public static TweenHandle FillFromTo(this Image image, float start, float end, float? duration = null, TweenOptions options = default)
            => Require(image).gameObject.Tween().WithOptions(options).FillFromTo(start, end, duration).Play();

        public static TweenHandle FillFromTo(this Slider slider, float start, float end, float? duration = null, TweenOptions options = default)
            => Require(slider).gameObject.Tween().WithOptions(options).FillFromTo(start, end, duration).Play();

        public static TweenHandle ValueFillTo(this Image image, float target, TMP_Text valueText = null, string format = "P0", float? duration = null, TweenOptions options = default)
            => Require(image).gameObject.Tween().WithOptions(options).ValueFillTo(target, valueText, format, duration).Play();

        public static TweenHandle ValueFillTo(this Slider slider, float target, TMP_Text valueText = null, string format = "P0", float? duration = null, TweenOptions options = default)
            => Require(slider).gameObject.Tween().WithOptions(options).ValueFillTo(target, valueText, format, duration).Play();

        public static TweenHandle FillDrain(this Image image, float target, float? duration = null, Color? accentColor = null, TweenOptions options = default)
            => Require(image).gameObject.Tween().WithOptions(options).FillDrain(target, duration, accentColor).Play();

        public static TweenHandle FillDrain(this Slider slider, float target, float? duration = null, Color? accentColor = null, TweenOptions options = default)
            => Require(slider).gameObject.Tween().WithOptions(options).FillDrain(target, duration, accentColor).Play();

        public static TweenHandle FillCharge(this Image image, float target, float? duration = null, Color? accentColor = null, TweenOptions options = default)
            => Require(image).gameObject.Tween().WithOptions(options).FillCharge(target, duration, accentColor).Play();

        public static TweenHandle FillCharge(this Slider slider, float target, float? duration = null, Color? accentColor = null, TweenOptions options = default)
            => Require(slider).gameObject.Tween().WithOptions(options).FillCharge(target, duration, accentColor).Play();

        public static TweenHandle FillAlertPulse(this Image image, float threshold, float? duration = null, Color? alertColor = null, TweenOptions options = default)
            => Require(image).gameObject.Tween().WithOptions(options).FillAlertPulse(threshold, duration, alertColor).Play();

        public static TweenHandle FillAlertPulse(this Slider slider, float threshold, float? duration = null, Color? alertColor = null, TweenOptions options = default)
            => Require(slider).gameObject.Tween().WithOptions(options).FillAlertPulse(threshold, duration, alertColor).Play();

        public static TweenHandle FillAndText(this Image image, float start, float end, TMP_Text valueText, string format = "P0", float? duration = null, TweenOptions options = default)
            => Require(image).gameObject.Tween().WithOptions(options).FillAndText(start, end, valueText, format, duration).Play();

        public static TweenHandle FillAndText(this Slider slider, float start, float end, TMP_Text valueText, string format = "P0", float? duration = null, TweenOptions options = default)
            => Require(slider).gameObject.Tween().WithOptions(options).FillAndText(start, end, valueText, format, duration).Play();

        private static T Require<T>(T component) where T : Component => component != null ? component : throw new ArgumentNullException(nameof(component));
    }
}
