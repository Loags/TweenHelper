using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Scales cleanly from zero to original scale without overshoot.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c> and animates to original scale using <c>Ease.OutQuad</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces OutQuad.
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
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var start = ResolveStartScale(options, Vector3.zero);
            var scaleTarget = ResolveTargetScale(options, originalScale);
            t.localScale = start;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutQuad);
            var ease = ResolveEase(presetOptions, Ease.OutQuad);

            return t.DOScale(scaleTarget, GetDuration(duration, options))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Scales the target from zero past its original scale (1.2x), then settles back.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c>, scales up to <c>originalScale * 1.2</c> in the first half,
    /// then eases back to <c>originalScale</c> in the second half.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.0s | <b>Default ease:</b> OutQuad (scale-up), InOutSine (settle)<br/>
    /// <b>Easing override:</b> Primary ease applies to the sequence.
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
        public override float DefaultDuration => 1.0f;
        public override float DefaultOvershoot => 0.2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var start = ResolveStartScale(options, Vector3.zero);
            var scaleTarget = ResolveTargetScale(options, originalScale);
            t.localScale = start;
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutQuad);
            var overshootScale = scaleTarget * (1.0f + DefaultOvershoot * ResolveOvershootMultiplier(presetOptions));

            var seq = DOTween.Sequence();
            seq.Append(t.DOScale(overshootScale, dur).SetEase(Ease.OutQuad));
            seq.Append(t.DOScale(scaleTarget, dur * 0.5f).SetEase(Ease.InOutSine));
            return seq.WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Scales the target down to zero with a clean deceleration, no anticipation overshoot.
    /// <para>
    /// Animates scale to <c>Vector3.zero</c> using <c>Ease.InCubic</c>.
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
            var endScale = ResolveTargetScale(options, Vector3.zero);
            return target.transform.DOScale(endScale, GetDuration(duration, options))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Soft exit scaling to zero with a slow InSine ease.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.6s | <b>Default ease:</b> InSine
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopOutSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopOutSoftPreset : CodePreset
    {
        public override string PresetName => "PopOutSoft";
        public override string Description => "Soft scale exit, no anticipation";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.InSine);
            var ease = ResolveEase(presetOptions, Ease.InSine);
            var endScale = ResolveTargetScale(options, Vector3.zero);
            return target.transform.DOScale(endScale, GetDuration(duration, options))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Hard exit scaling to zero with a snappy InQuart ease.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.25s | <b>Default ease:</b> InQuart
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopOutHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopOutHardPreset : CodePreset
    {
        public override string PresetName => "PopOutHard";
        public override string Description => "Hard scale exit, no anticipation";
        public override float DefaultDuration => 0.25f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.InQuart);
            var ease = ResolveEase(presetOptions, Ease.InQuart);
            var endScale = ResolveTargetScale(options, Vector3.zero);
            return target.transform.DOScale(endScale, GetDuration(duration, options))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Scales the target down to zero with a brief anticipation pull-back.
    /// <para>
    /// Animates scale to <c>Vector3.zero</c> using <c>Ease.InBack</c>.
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
        public override float DefaultOvershoot => 1.70158f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.InBack);
            var ease = ResolveEase(presetOptions, Ease.InBack);
            var os = DefaultOvershoot * ResolveOvershootMultiplier(presetOptions);

            var endScale = ResolveTargetScale(options, Vector3.zero);
            var seq = DOTween.Sequence();
            seq.Join(t.DOScale(endScale, dur).SetEase(ease, os));
            return seq.WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Scales the target down to zero with a mild anticipation pull-back (overshoot 2.5).
    /// <para>
    /// Animates scale to <c>Vector3.zero</c> using <c>Ease.InBack</c> with overshoot 2.5.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.6s | <b>Default ease:</b> InBack (overshoot 2.5)<br/>
    /// <b>Easing override:</b> Primary ease replaces InBack; overshoot parameter is preserved.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Gentle UI dismissal with subtle anticipation, soft dialog close, understated exit.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopOutOvershootSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopOutOvershootSoftPreset : CodePreset
    {
        public override string PresetName => "PopOutOvershootSoft";
        public override string Description => "Soft scale exit with mild overshoot";
        public override float DefaultDuration => 0.6f;
        public override float DefaultOvershoot => 2.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.InBack);
            var ease = ResolveEase(presetOptions, Ease.InBack);
            var os = DefaultOvershoot * ResolveOvershootMultiplier(presetOptions);

            var endScale = ResolveTargetScale(options, Vector3.zero);
            var seq = DOTween.Sequence();
            seq.Join(t.DOScale(endScale, dur).SetEase(ease, os));
            return seq.WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Scales the target down to zero with a strong anticipation pull-back (overshoot 6.0).
    /// <para>
    /// Animates scale to <c>Vector3.zero</c> using <c>Ease.InBack</c> with overshoot 6.0.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.25s | <b>Default ease:</b> InBack (overshoot 6.0)<br/>
    /// <b>Easing override:</b> Primary ease replaces InBack; overshoot parameter is preserved.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Emphatic UI dismissal, punchy item collection, dramatic dialog close with strong wind-up.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopOutOvershootHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopOutOvershootHardPreset : CodePreset
    {
        public override string PresetName => "PopOutOvershootHard";
        public override string Description => "Hard scale exit with strong overshoot";
        public override float DefaultDuration => 0.25f;
        public override float DefaultOvershoot => 6.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.InBack);
            var ease = ResolveEase(presetOptions, Ease.InBack);
            var os = DefaultOvershoot * ResolveOvershootMultiplier(presetOptions);

            var endScale = ResolveTargetScale(options, Vector3.zero);
            var seq = DOTween.Sequence();
            seq.Join(t.DOScale(endScale, dur).SetEase(ease, os));
            return seq.WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Gentle entrance scaling from zero with a slow OutSine ease, no overshoot.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.8s | <b>Default ease:</b> OutSine
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
            var start = ResolveStartScale(options, Vector3.zero);
            var scaleTarget = ResolveTargetScale(options, originalScale);
            t.localScale = start;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutSine);
            var ease = ResolveEase(presetOptions, Ease.OutSine);

            return t.DOScale(scaleTarget, GetDuration(duration, options))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Scales the target from zero past its original scale (1.1x), then gently settles back.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c>, scales up to <c>originalScale * 1.1</c> in the first half,
    /// then eases back to <c>originalScale</c> in the second half.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.2s | <b>Default ease:</b> OutQuad (scale-up), InOutSine (settle)<br/>
    /// <b>Easing override:</b> Primary ease applies to the sequence.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Gentle bouncy UI entrance, subtle item spawn, soft notification pop.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopInOvershootSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopInOvershootSoftPreset : CodePreset
    {
        public override string PresetName => "PopInOvershootSoft";
        public override string Description => "Gentle scale entrance with mild OutBack overshoot";
        public override float DefaultDuration => 1.2f;
        public override float DefaultOvershoot => 0.1f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var start = ResolveStartScale(options, Vector3.zero);
            var scaleTarget = ResolveTargetScale(options, originalScale);
            t.localScale = start;
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutQuad);
            var overshootScale = scaleTarget * (1.0f + DefaultOvershoot * ResolveOvershootMultiplier(presetOptions));

            var seq = DOTween.Sequence();
            seq.Append(t.DOScale(overshootScale, dur).SetEase(Ease.OutQuad));
            seq.Append(t.DOScale(scaleTarget, dur * 0.5f).SetEase(Ease.InOutSine));
            return seq.WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Fast entrance scaling from zero with a snappy OutQuart ease, no overshoot.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.3s | <b>Default ease:</b> OutQuart
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
            var start = ResolveStartScale(options, Vector3.zero);
            var scaleTarget = ResolveTargetScale(options, originalScale);
            t.localScale = start;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutQuart);
            var ease = ResolveEase(presetOptions, Ease.OutQuart);

            return t.DOScale(scaleTarget, GetDuration(duration, options))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Scales the target from zero past its original scale (1.4x), then snaps back.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c>, scales up to <c>originalScale * 1.4</c> in the first half,
    /// then eases back to <c>originalScale</c> in the second half.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.8s | <b>Default ease:</b> OutQuad (scale-up), InOutSine (settle)<br/>
    /// <b>Easing override:</b> Primary ease applies to the sequence.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Punchy UI entrance, dramatic item spawn, emphatic notification pop.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopInOvershootHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopInOvershootHardPreset : CodePreset
    {
        public override string PresetName => "PopInOvershootHard";
        public override string Description => "Snappy scale entrance with strong overshoot";
        public override float DefaultDuration => 0.8f;
        public override float DefaultOvershoot => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var start = ResolveStartScale(options, Vector3.zero);
            var scaleTarget = ResolveTargetScale(options, originalScale);
            t.localScale = start;
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutQuad);
            var overshootScale = scaleTarget * (1.0f + DefaultOvershoot * ResolveOvershootMultiplier(presetOptions));

            var seq = DOTween.Sequence();
            seq.Append(t.DOScale(overshootScale, dur).SetEase(Ease.OutQuad));
            seq.Append(t.DOScale(scaleTarget, dur * 0.5f).SetEase(Ease.InOutSine));
            return seq.WithDefaults(presetOptions, target);
        }
    }
}
