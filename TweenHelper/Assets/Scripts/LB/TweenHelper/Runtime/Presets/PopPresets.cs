using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Scales cleanly from zero to original scale without overshoot, providing a smooth entrance.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c> and animates to original scale using <c>Ease.OutCubic</c>.
    /// Decelerates smoothly to the final value without exceeding it.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces OutCubic.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> UI element entrance, item spawn, notification pop-up, dialog appearance.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopIn").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopInPreset : CodePreset
    {
        public override string PresetName => "PopIn";
        public override string Description => "Scales from 0 to original scale, no overshoot";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);

            return t.DOScale(originalScale, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Scales the target from zero to its original scale with elastic overshoot, creating a snappy entrance effect.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c>, then animates to the stored original scale using
    /// <c>Ease.OutBack</c> with an overshoot parameter of <c>1.7</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutBack (1.7 overshoot)<br/>
    /// <b>Easing override:</b> Primary ease replaces OutBack.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Bouncy UI entrance, item spawn with pop, playful notification.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopInOvershoot").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopInOvershootPreset : CodePreset
    {
        public override string PresetName => "PopInOvershoot";
        public override string Description => "Scales from 0 to original scale with overshoot";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutBack);
            var ease = ResolveEase(presetOptions, Ease.OutBack);

            return t.DOScale(originalScale, GetDuration(duration))
                .SetEase(ease, 1.7f)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Scales the target down to zero with a clean deceleration, no anticipation overshoot.
    /// <para>
    /// Animates scale to <c>Vector3.zero</c> using <c>Ease.InCubic</c> for a smooth exit
    /// without the brief scale-up that InBack produces.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.4s | <b>Default ease:</b> InCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces InCubic.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> UI element dismissal, item collection, dialog close, notification dismiss.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopOut").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopOutPreset : CodePreset
    {
        public override string PresetName => "PopOut";
        public override string Description => "Scales to 0, no anticipation";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.InCubic);
            var ease = ResolveEase(presetOptions, Ease.InCubic);
            return target.transform.DOScale(Vector3.zero, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Scales the target down to zero with a brief anticipation pull-back, creating a satisfying exit effect.
    /// <para>
    /// Animates scale to <c>Vector3.zero</c> using <c>Ease.InBack</c>, which briefly scales slightly
    /// larger before shrinking to zero, giving a natural "wind-up" feel.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.4s | <b>Default ease:</b> InBack<br/>
    /// <b>Easing override:</b> Primary ease replaces InBack.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> UI element dismissal with anticipation, item collection, dialog close with overshoot.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopOutOvershoot").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopOutOvershootPreset : CodePreset
    {
        public override string PresetName => "PopOutOvershoot";
        public override string Description => "Scales to 0 with anticipation overshoot";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.InBack);
            var ease = ResolveEase(presetOptions, Ease.InBack);
            return target.transform.DOScale(Vector3.zero, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Gentle entrance scaling from zero with a slow OutSine ease, no overshoot.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c> and animates to original scale using <c>Ease.OutSine</c>.
    /// Slower and softer than PopIn for a calm, understated entrance.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.8s | <b>Default ease:</b> OutSine<br/>
    /// <b>Easing override:</b> Primary ease replaces OutSine.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopInSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopInSoftPreset : CodePreset
    {
        public override string PresetName => "PopInSoft";
        public override string Description => "Gentle scale entrance, no overshoot";
        public override float DefaultDuration => 0.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutSine);
            var ease = ResolveEase(presetOptions, Ease.OutSine);

            return t.DOScale(originalScale, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Medium-speed entrance scaling from zero with OutQuad ease, no overshoot.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c> and animates to original scale using <c>Ease.OutQuad</c>.
    /// Sits between PopInSoft (OutSine, 0.8s) and PopInHard (OutQuart, 0.3s) in speed and feel.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces OutQuad.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopInMedium").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopInMediumPreset : CodePreset
    {
        public override string PresetName => "PopInMedium";
        public override string Description => "Medium-speed scale entrance, no overshoot";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutQuad);
            var ease = ResolveEase(presetOptions, Ease.OutQuad);

            return t.DOScale(originalScale, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Gentle entrance scaling from zero with mild OutBack overshoot (overshoot param 1.2).
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c> and animates to original scale using <c>Ease.OutBack</c>
    /// with overshoot parameter <c>1.2</c>, producing a subtle bounce past the target before settling.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.8s | <b>Default ease:</b> OutBack (overshoot 1.2)<br/>
    /// <b>Easing override:</b> Primary ease replaces OutBack.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopInOvershootSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopInOvershootSoftPreset : CodePreset
    {
        public override string PresetName => "PopInOvershootSoft";
        public override string Description => "Gentle scale entrance with mild OutBack overshoot";
        public override float DefaultDuration => 0.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutBack);
            var ease = ResolveEase(presetOptions, Ease.OutBack);

            return t.DOScale(originalScale, GetDuration(duration))
                .SetEase(ease, 1.2f)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Medium-speed entrance scaling from zero with OutBack overshoot (overshoot param 1.5).
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c> and animates to original scale using <c>Ease.OutBack</c>
    /// with overshoot parameter <c>1.5</c>. Sits between PopInOvershootSoft (1.2) and PopInOvershootHard (2.2).
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutBack (overshoot 1.5)<br/>
    /// <b>Easing override:</b> Primary ease replaces OutBack.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopInOvershootMedium").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopInOvershootMediumPreset : CodePreset
    {
        public override string PresetName => "PopInOvershootMedium";
        public override string Description => "Medium-speed scale entrance with moderate overshoot";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutBack);
            var ease = ResolveEase(presetOptions, Ease.OutBack);

            return t.DOScale(originalScale, GetDuration(duration))
                .SetEase(ease, 1.5f)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Fast entrance scaling from zero with a snappy OutQuart ease, no overshoot.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c> and animates to original scale using <c>Ease.OutQuart</c>.
    /// Faster and punchier than PopIn but without any overshoot.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.3s | <b>Default ease:</b> OutQuart<br/>
    /// <b>Easing override:</b> Primary ease replaces OutQuart.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopInHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopInHardPreset : CodePreset
    {
        public override string PresetName => "PopInHard";
        public override string Description => "Fast scale entrance, no overshoot";
        public override float DefaultDuration => 0.3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutQuart);
            var ease = ResolveEase(presetOptions, Ease.OutQuart);

            return t.DOScale(originalScale, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Snappy entrance scaling from zero with OutBack and high overshoot parameter for a hard pop.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c>, then animates to original scale using
    /// <c>Ease.OutBack</c> with overshoot <c>2.2</c> for a pronounced snap.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.4s | <b>Default ease:</b> OutBack (2.2 overshoot)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopInOvershootHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopInOvershootHardPreset : CodePreset
    {
        public override string PresetName => "PopInOvershootHard";
        public override string Description => "Snappy scale entrance with strong overshoot";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutBack);
            var ease = ResolveEase(presetOptions, Ease.OutBack);

            return t.DOScale(originalScale, GetDuration(duration))
                .SetEase(ease, 2.2f)
                .WithDefaults(presetOptions, target);
        }
    }
}
