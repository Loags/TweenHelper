using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Performs a quick scale bump up to 1.14x then back to original, ideal for tap or click feedback.
    /// <para>
    /// Builds a 2-step sequence: (1) scale to <c>originalScale * 1.14</c> over 40% duration with <c>Ease.OutQuad</c>,
    /// (2) return to original scale over 60% duration with <c>Ease.InQuad</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.28s | <b>Default ease:</b> OutQuad (up), InQuad (down)<br/>
    /// <b>Easing override:</b> Primary ease controls scale-up; secondary ease controls scale-down.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Button tap feedback, toggle state change, counter increment, selection highlight.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PulseScale").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PulseScalePreset : CodePreset
    {
        public override string PresetName => "PulseScale";
        public override string Description => "Quick scale bump for interactive feedback";
        public override float DefaultDuration => 0.28f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var dur = GetDuration(duration);
            var upEase = ResolveEase(options, Ease.OutQuad);
            var downEase = ResolveSecondaryEase(options, Ease.InQuad);
            var presetOptions = MergeWithDefaultEase(options, upEase);

            return DOTween.Sequence()
                .Append(t.DOScale(originalScale * 1.14f, dur * 0.4f).SetEase(upEase))
                .Append(t.DOScale(originalScale, dur * 0.6f).SetEase(downEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Internal factory for PulseScale variants sharing the same 2-phase up/down sequence structure.
    /// </summary>
    internal static class PulseScaleFactory
    {
        public static Tween Create(GameObject target, float peak, float duration, TweenOptions options)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var upEase = options.Ease ?? Ease.OutQuad;
            var downEase = options.SecondaryEase ?? options.Ease ?? Ease.InQuad;
            var presetOptions = options.Ease.HasValue ? options : options.SetEase(upEase);

            return DOTween.Sequence()
                .Append(t.DOScale(originalScale * peak, duration * 0.4f).SetEase(upEase))
                .Append(t.DOScale(originalScale, duration * 0.6f).SetEase(downEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Soft scale bump to 1.08x then back to original, for understated feedback.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.25s | <b>Default ease:</b> OutQuad (up), InQuad (down)<br/>
    /// <b>Easing override:</b> Primary ease controls scale-up; secondary ease controls scale-down.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PulseScaleSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PulseScaleSoftPreset : CodePreset
    {
        public override string PresetName => "PulseScaleSoft";
        public override string Description => "Soft scale bump for light feedback";
        public override float DefaultDuration => 0.25f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PulseScaleFactory.Create(target, 1.08f, GetDuration(duration), options);
        }
    }

    /// <summary>
    /// Bold scale bump to 1.25x then back to original, for emphatic feedback.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.35s | <b>Default ease:</b> OutQuad (up), InQuad (down)<br/>
    /// <b>Easing override:</b> Primary ease controls scale-up; secondary ease controls scale-down.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PulseScaleHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PulseScaleHardPreset : CodePreset
    {
        public override string PresetName => "PulseScaleHard";
        public override string Description => "Bold scale bump for emphatic feedback";
        public override float DefaultDuration => 0.35f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PulseScaleFactory.Create(target, 1.25f, GetDuration(duration), options);
        }
    }
}
