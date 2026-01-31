using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Scales the target from zero to original while simultaneously fading in from transparent to opaque.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c> and alpha to <c>0</c>. Builds a parallel sequence:
    /// scale uses <c>Ease.OutCubic</c>, fade uses <c>Ease.Linear</c> (so alpha doesn't rush ahead of scale).
    /// If no fadeable component exists, only the scale animation plays. Always returns a valid tween
    /// (CanApplyTo returns true for any non-null target).
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 2.0s | <b>Default ease:</b> OutCubic (scale), Linear (fade)<br/>
    /// <b>Easing override:</b> Primary ease controls scale; fade is always Linear.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> UI element entrance, dialog appearance, smooth combined reveal, material-design entrance.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopInFade").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopInFadePreset : CodePreset
    {
        public override string PresetName => "PopInFade";
        public override string Description => "Scales and fades in together";
        public override float DefaultDuration => 2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;

            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);
            var seq = DOTween.Sequence();

            seq.Append(t.DOScale(originalScale, dur).SetEase(ease));

            var fadeTween = CreateFadeTween(target, 1f, dur);
            if (fadeTween != null)
            {
                SetAlpha(target, 0f);
                // Use Linear ease for fade so alpha doesn't rush ahead of scale
                seq.Join(fadeTween.SetEase(Ease.Linear));
            }

            return seq.WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target)
        {
            return target != null;
        }
    }

    /// <summary>
    /// Scales the target down to zero while simultaneously fading out to transparent.
    /// <para>
    /// Builds a parallel sequence: scale to <c>Vector3.zero</c> with <c>Ease.InCubic</c> (no anticipation),
    /// fade to <c>0</c> with <c>Ease.Linear</c>. If no fadeable component exists, only the scale animation plays.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 1.2s | <b>Default ease:</b> InCubic (scale), Linear (fade)<br/>
    /// <b>Easing override:</b> Primary ease controls scale; fade is always Linear.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> UI element dismissal, dialog close, combined exit, material-design exit.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopOutFade").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopOutFadePreset : CodePreset
    {
        public override string PresetName => "PopOutFade";
        public override string Description => "Scales down and fades out together, no anticipation";
        public override float DefaultDuration => 1.2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.InCubic);
            var ease = ResolveEase(presetOptions, Ease.InCubic);

            var seq = DOTween.Sequence();
            seq.Join(t.DOScale(Vector3.zero, dur).SetEase(ease));

            var fadeTween = CreateFadeTween(target, 0f, dur);
            if (fadeTween != null)
            {
                fadeTween.SetEase(Ease.Linear);
                seq.Join(fadeTween);
            }

            return seq.WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target)
        {
            return target != null;
        }
    }

    /// <summary>
    /// Scales the target down to zero while fading out, with anticipation overshoot on scale.
    /// <para>
    /// Builds a parallel sequence: scale to <c>Vector3.zero</c> with <c>Ease.InBack</c> (anticipation),
    /// joined with fade to <c>0</c> using <c>Ease.Linear</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 1.2s | <b>Default ease:</b> InBack (scale), Linear (fade)<br/>
    /// <b>Easing override:</b> Primary ease controls scale; fade is always Linear.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> UI element dismissal with anticipation, dialog close with overshoot, combined exit.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopOutFadeOvershoot").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopOutFadeOvershootPreset : CodePreset
    {
        public override string PresetName => "PopOutFadeOvershoot";
        public override string Description => "Scales down and fades out with anticipation overshoot";
        public override float DefaultDuration => 1.2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.InBack);
            var ease = ResolveEase(presetOptions, Ease.InBack);

            var seq = DOTween.Sequence();
            seq.Join(t.DOScale(Vector3.zero, dur).SetEase(ease));

            var fadeTween = CreateFadeTween(target, 0f, dur);
            if (fadeTween != null)
            {
                fadeTween.SetEase(Ease.Linear);
                seq.Join(fadeTween);
            }

            return seq.WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target)
        {
            return target != null;
        }
    }

    /// <summary>
    /// Scales the target up to 1.5x its original size while simultaneously fading out, creating a burst/explosion effect.
    /// <para>
    /// Builds a parallel sequence: scale to <c>originalScale * 1.5</c> with <c>Ease.OutQuad</c>
    /// (fast initial burst), joined with fade to <c>0</c> using <c>Ease.Linear</c>.
    /// If no fadeable component exists, only the scale animation plays.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutQuad (scale), Linear (fade)<br/>
    /// <b>Easing override:</b> Primary ease controls scale; secondary ease controls fade.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Explosion effect, particle burst, destruction animation, energy release.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Explode").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ExplodePreset : CodePreset
    {
        public override string PresetName => "Explode";
        public override string Description => "Scale up and fade out simultaneously";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var dur = GetDuration(duration);
            var scaleEase = ResolveEase(options, Ease.OutQuad);
            var fadeEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);

            var seq = DOTween.Sequence();
            seq.Join(t.DOScale(originalScale * 1.5f, dur).SetEase(scaleEase));

            var fadeTween = CreateFadeTween(target, 0f, dur);
            if (fadeTween != null)
            {
                seq.Join(fadeTween.SetEase(fadeEase));
            }

            return seq.WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }
}
