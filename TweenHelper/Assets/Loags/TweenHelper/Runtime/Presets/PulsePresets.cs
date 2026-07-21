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
    /// <b>Easing override:</b> Primary ease controls scale-up; secondary ease controls scale-down.<br/>
    /// <b>Strength override:</b> Multiplies scale pulse intensity (default 1.0).
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
            var dur = GetDuration(duration, options);
            var strength = ResolveStrength(options);
            var upEase = ResolveEase(options, Ease.OutQuad);
            var downEase = ResolveSecondaryEase(options, Ease.InQuad);
            var presetOptions = MergeWithDefaultEase(options, upEase);

            return DOTween.Sequence()
                .Append(t.DOScale(originalScale * (1f + 0.14f * strength), dur * 0.4f).SetEase(upEase))
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
            var strength = CodePreset.ResolveStrengthStatic(options);
            var upEase = options.Ease ?? Ease.OutQuad;
            var downEase = options.SecondaryEase ?? options.Ease ?? Ease.InQuad;
            var presetOptions = options.Ease.HasValue ? options : options.SetEase(upEase);

            return DOTween.Sequence()
                .Append(t.DOScale(originalScale * (1f + (peak - 1f) * strength), duration * 0.4f).SetEase(upEase))
                .Append(t.DOScale(originalScale, duration * 0.6f).SetEase(downEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Soft scale bump to 1.08x then back to original, for understated feedback.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.25s | <b>Default ease:</b> OutQuad (up), InQuad (down)<br/>
    /// <b>Easing override:</b> Primary ease controls scale-up; secondary ease controls scale-down.<br/>
    /// <b>Strength override:</b> Multiplies scale pulse intensity (default 1.0).
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
            return PulseScaleFactory.Create(target, 1.08f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Bold scale bump to 1.25x then back to original, for emphatic feedback.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.35s | <b>Default ease:</b> OutQuad (up), InQuad (down)<br/>
    /// <b>Easing override:</b> Primary ease controls scale-up; secondary ease controls scale-down.<br/>
    /// <b>Strength override:</b> Multiplies scale pulse intensity (default 1.0).
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
            return PulseScaleFactory.Create(target, 1.25f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Internal factory for PulseScaleFade variants with scale pulse + alpha dip/return behavior.
    /// </summary>
    internal static class PulseScaleFadeFactory
    {
        public static Tween Create(GameObject target, float peak, float dipAlphaDefault, float duration, TweenOptions options)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var strength = CodePreset.ResolveStrengthStatic(options);
            var upEase = options.Ease ?? Ease.OutQuad;
            var downEase = options.SecondaryEase ?? options.Ease ?? Ease.InQuad;
            var presetOptions = options.Ease.HasValue ? options : options.SetEase(upEase);

            var seq = DOTween.Sequence();
            seq.Append(t.DOScale(originalScale * (1f + (peak - 1f) * strength), duration * 0.4f).SetEase(upEase));
            seq.Append(t.DOScale(originalScale, duration * 0.6f).SetEase(downEase));

            float startAlpha = CodePreset.ResolveStartAlphaStatic(options, 1f);
            float dipAlpha = CodePreset.ResolveTargetAlphaStatic(options, dipAlphaDefault);

            CodePreset.SetAlphaStatic(target, startAlpha);
            var fadeOut = CodePreset.CreateFadeTweenStatic(target, dipAlpha, duration * 0.4f);
            var fadeIn = CodePreset.CreateFadeTweenStatic(target, startAlpha, duration * 0.6f);
            if (fadeOut != null && fadeIn != null)
            {
                var fadeSeq = DOTween.Sequence()
                    .Append(fadeOut.SetEase(Ease.Linear))
                    .Append(fadeIn.SetEase(Ease.Linear));

                seq.Join(fadeSeq);
                seq.OnComplete(() => CodePreset.SetAlphaStatic(target, startAlpha));
            }

            return seq.WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Pulse-scale feedback with a brief alpha dip, then returns to full alpha.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.84s | <b>Default ease:</b> OutQuad/InQuad (scale), Linear (fade)<br/>
    /// <b>Easing override:</b> Primary ease controls scale-up; secondary ease controls scale-down.<br/>
    /// <b>Strength override:</b> Multiplies scale pulse intensity (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces start/end alpha (default 1); TargetAlpha replaces dip alpha (default 0.6).<br/>
    /// If no fadeable component exists, only the scale pulse is played.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PulseScaleFade").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PulseScaleFadePreset : CodePreset
    {
        public override string PresetName => "PulseScaleFade";
        public override string Description => "Pulse scale with alpha dip and return";
        public override float DefaultDuration => 0.84f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PulseScaleFadeFactory.Create(target, 1.14f, 0.6f, GetDuration(duration, options), options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Soft pulse-scale feedback with a mild alpha dip, then returns to full alpha.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.75s | <b>Default ease:</b> OutQuad/InQuad (scale), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies scale pulse intensity (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces start/end alpha; TargetAlpha replaces dip alpha (default 0.75).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PulseScaleFadeSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PulseScaleFadeSoftPreset : CodePreset
    {
        public override string PresetName => "PulseScaleFadeSoft";
        public override string Description => "Soft pulse scale with alpha dip and return";
        public override float DefaultDuration => 0.75f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PulseScaleFadeFactory.Create(target, 1.08f, 0.75f, GetDuration(duration, options), options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Hard pulse-scale feedback with a deep alpha dip, then returns to full alpha.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 1.05s | <b>Default ease:</b> OutQuad/InQuad (scale), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies scale pulse intensity (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces start/end alpha; TargetAlpha replaces dip alpha (default 0.4).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PulseScaleFadeHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PulseScaleFadeHardPreset : CodePreset
    {
        public override string PresetName => "PulseScaleFadeHard";
        public override string Description => "Bold pulse scale with deep alpha dip and return";
        public override float DefaultDuration => 1.05f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PulseScaleFadeFactory.Create(target, 1.25f, 0.4f, GetDuration(duration, options), options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }
}
