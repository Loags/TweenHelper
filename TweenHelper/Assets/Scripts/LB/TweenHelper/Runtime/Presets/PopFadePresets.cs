using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Internal factory for composite pop + fade presets.
    /// </summary>
    internal static class PopFadeFactory
    {
        private static TweenOptions WithDefaultEase(TweenOptions options, Ease defaultEase)
            => options.Ease.HasValue ? options : options.SetEase(defaultEase);

        private static float ResolveDuration(float? duration, float defaultDuration, TweenOptions options)
            => duration ?? options.Duration ?? defaultDuration;

        public static Tween CreatePopInFade(GameObject target, float? duration, float defaultDuration, TweenOptions options, Ease defaultScaleEase)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var startScale = options.StartScale ?? Vector3.zero;
            var scaleTarget = options.TargetScale ?? originalScale;
            t.localScale = startScale;

            var dur = ResolveDuration(duration, defaultDuration, options);
            var presetOptions = WithDefaultEase(options, defaultScaleEase);
            var scaleEase = presetOptions.Ease ?? defaultScaleEase;
            var seq = DOTween.Sequence();

            seq.Append(t.DOScale(scaleTarget, dur).SetEase(scaleEase));

            var fadeTween = CodePreset.CreateFadeTweenStatic(target, CodePreset.ResolveTargetAlphaStatic(options, 1f), dur);
            if (fadeTween != null)
            {
                CodePreset.SetAlphaStatic(target, CodePreset.ResolveStartAlphaStatic(options, 0f));
                seq.Join(fadeTween.SetEase(Ease.Linear));
            }

            return seq.WithDefaults(presetOptions, target);
        }

        public static Tween CreatePopOutFade(
            GameObject target,
            float? duration,
            float defaultDuration,
            TweenOptions options,
            Ease defaultScaleEase,
            float? explicitOvershoot = null)
        {
            var t = target.transform;
            var dur = ResolveDuration(duration, defaultDuration, options);
            var presetOptions = WithDefaultEase(options, defaultScaleEase);
            var scaleEase = presetOptions.Ease ?? defaultScaleEase;
            var endScale = options.TargetScale ?? Vector3.zero;
            var seq = DOTween.Sequence();

            if (explicitOvershoot.HasValue)
            {
                seq.Join(t.DOScale(endScale, dur).SetEase(scaleEase, explicitOvershoot.Value));
            }
            else
            {
                seq.Join(t.DOScale(endScale, dur).SetEase(scaleEase));
            }

            var fadeTween = CodePreset.CreateFadeTweenStatic(target, CodePreset.ResolveTargetAlphaStatic(options, 0f), dur);
            if (fadeTween != null)
            {
                seq.Join(fadeTween.SetEase(Ease.Linear));
            }

            return seq.WithDefaults(presetOptions, target);
        }

        public static Tween CreateExplode(
            GameObject target,
            float? duration,
            float defaultDuration,
            TweenOptions options,
            Ease defaultScaleEase,
            float scaleMultiplier)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var dur = ResolveDuration(duration, defaultDuration, options);
            var scaleEase = options.Ease ?? defaultScaleEase;
            var fadeEase = options.SecondaryEase ?? options.Ease ?? Ease.Linear;
            var presetOptions = WithDefaultEase(options, defaultScaleEase);
            var explodeTarget = options.TargetScale ?? (originalScale * scaleMultiplier);
            var seq = DOTween.Sequence();

            seq.Join(t.DOScale(explodeTarget, dur).SetEase(scaleEase));

            var fadeTween = CodePreset.CreateFadeTweenStatic(target, CodePreset.ResolveTargetAlphaStatic(options, 0f), dur);
            if (fadeTween != null)
            {
                seq.Join(fadeTween.SetEase(fadeEase));
            }

            return seq.WithDefaults(presetOptions, target);
        }
    }

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
    /// <b>Easing override:</b> Primary ease controls scale; fade is always Linear.<br/>
    /// <b>Scale override:</b> StartScale replaces zero; TargetScale replaces original scale.<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 0; TargetAlpha replaces 1.
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
            return PopFadeFactory.CreatePopInFade(target, duration, DefaultDuration, options, Ease.OutCubic);
        }

        public override bool CanApplyTo(GameObject target)
        {
            return target != null;
        }
    }

    /// <summary>
    /// Soft scales the target from zero to original while simultaneously fading in.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 2.5s | <b>Default ease:</b> OutSine (scale), Linear (fade)<br/>
    /// <b>Scale override:</b> StartScale replaces zero; TargetScale replaces original scale.<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 0; TargetAlpha replaces 1.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopInFadeSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopInFadeSoftPreset : CodePreset
    {
        public override string PresetName => "PopInFadeSoft";
        public override string Description => "Soft scales and fades in together";
        public override float DefaultDuration => 2.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PopFadeFactory.CreatePopInFade(target, duration, DefaultDuration, options, Ease.OutSine);
        }

        public override bool CanApplyTo(GameObject target)
        {
            return target != null;
        }
    }

    /// <summary>
    /// Hard scales the target from zero to original while simultaneously fading in.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.4s | <b>Default ease:</b> OutQuart (scale), Linear (fade)<br/>
    /// <b>Scale override:</b> StartScale replaces zero; TargetScale replaces original scale.<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 0; TargetAlpha replaces 1.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopInFadeHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopInFadeHardPreset : CodePreset
    {
        public override string PresetName => "PopInFadeHard";
        public override string Description => "Hard scales and fades in together";
        public override float DefaultDuration => 1.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PopFadeFactory.CreatePopInFade(target, duration, DefaultDuration, options, Ease.OutQuart);
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
    /// <b>Easing override:</b> Primary ease controls scale; fade is always Linear.<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.<br/>
    /// <b>Alpha override:</b> TargetAlpha replaces 0.
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
            return PopFadeFactory.CreatePopOutFade(target, duration, DefaultDuration, options, Ease.InCubic);
        }

        public override bool CanApplyTo(GameObject target)
        {
            return target != null;
        }
    }

    /// <summary>
    /// Soft scales the target down to zero while simultaneously fading out.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 1.6s | <b>Default ease:</b> InSine (scale), Linear (fade)<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.<br/>
    /// <b>Alpha override:</b> TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopOutFadeSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopOutFadeSoftPreset : CodePreset
    {
        public override string PresetName => "PopOutFadeSoft";
        public override string Description => "Soft scales down and fades out together";
        public override float DefaultDuration => 1.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PopFadeFactory.CreatePopOutFade(target, duration, DefaultDuration, options, Ease.InSine);
        }

        public override bool CanApplyTo(GameObject target)
        {
            return target != null;
        }
    }

    /// <summary>
    /// Hard scales the target down to zero while simultaneously fading out.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.8s | <b>Default ease:</b> InQuart (scale), Linear (fade)<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.<br/>
    /// <b>Alpha override:</b> TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopOutFadeHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopOutFadeHardPreset : CodePreset
    {
        public override string PresetName => "PopOutFadeHard";
        public override string Description => "Hard scales down and fades out together";
        public override float DefaultDuration => 0.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PopFadeFactory.CreatePopOutFade(target, duration, DefaultDuration, options, Ease.InQuart);
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
    /// <b>Easing override:</b> Primary ease controls scale; fade is always Linear.<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.<br/>
    /// <b>Alpha override:</b> TargetAlpha replaces 0.
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
            return PopFadeFactory.CreatePopOutFade(target, duration, DefaultDuration, options, Ease.InBack);
        }

        public override bool CanApplyTo(GameObject target)
        {
            return target != null;
        }
    }

    /// <summary>
    /// Soft scales the target down to zero while fading out, with mild anticipation overshoot on scale.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 1.6s | <b>Default ease:</b> InBack (overshoot 2.5, scale), Linear (fade)<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.<br/>
    /// <b>Alpha override:</b> TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopOutFadeOvershootSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopOutFadeOvershootSoftPreset : CodePreset
    {
        public override string PresetName => "PopOutFadeOvershootSoft";
        public override string Description => "Soft scales down and fades out with mild overshoot";
        public override float DefaultDuration => 1.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PopFadeFactory.CreatePopOutFade(target, duration, DefaultDuration, options, Ease.InBack, explicitOvershoot: 2.5f);
        }

        public override bool CanApplyTo(GameObject target)
        {
            return target != null;
        }
    }

    /// <summary>
    /// Hard scales the target down to zero while fading out, with strong anticipation overshoot on scale.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.8s | <b>Default ease:</b> InBack (overshoot 6.0, scale), Linear (fade)<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.<br/>
    /// <b>Alpha override:</b> TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopOutFadeOvershootHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopOutFadeOvershootHardPreset : CodePreset
    {
        public override string PresetName => "PopOutFadeOvershootHard";
        public override string Description => "Hard scales down and fades out with strong overshoot";
        public override float DefaultDuration => 0.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PopFadeFactory.CreatePopOutFade(target, duration, DefaultDuration, options, Ease.InBack, explicitOvershoot: 6.0f);
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
    /// <b>Easing override:</b> Primary ease controls scale; secondary ease controls fade.<br/>
    /// <b>Scale override:</b> TargetScale replaces the expanded scale (originalScale × 1.5).<br/>
    /// <b>Alpha override:</b> TargetAlpha replaces 0.
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
            return PopFadeFactory.CreateExplode(target, duration, DefaultDuration, options, Ease.OutQuad, 1.5f);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Gentle explosion effect with slower expansion and fade.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.8s | <b>Default ease:</b> OutSine (scale), Linear (fade)<br/>
    /// <b>Scale override:</b> TargetScale replaces the expanded scale (originalScale × 1.3).<br/>
    /// <b>Alpha override:</b> TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("ExplodeSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ExplodeSoftPreset : CodePreset
    {
        public override string PresetName => "ExplodeSoft";
        public override string Description => "Gentle scale up and fade out";
        public override float DefaultDuration => 0.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PopFadeFactory.CreateExplode(target, duration, DefaultDuration, options, Ease.OutSine, 1.3f);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Aggressive explosion effect with rapid expansion and fade.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.4s | <b>Default ease:</b> OutCubic (scale), Linear (fade)<br/>
    /// <b>Scale override:</b> TargetScale replaces the expanded scale (originalScale × 2.0).<br/>
    /// <b>Alpha override:</b> TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("ExplodeHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ExplodeHardPreset : CodePreset
    {
        public override string PresetName => "ExplodeHard";
        public override string Description => "Aggressive scale up and fade out";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PopFadeFactory.CreateExplode(target, duration, DefaultDuration, options, Ease.OutCubic, 2.0f);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }
}
