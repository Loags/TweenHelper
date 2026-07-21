using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Internal factory for pop scale presets.
    /// </summary>
    internal static class PopFactory
    {
        private static TweenOptions WithDefaultEase(TweenOptions options, Ease defaultEase)
            => options.Ease.HasValue ? options : options.SetEase(defaultEase);

        private static float ResolveDuration(float? duration, float defaultDuration, TweenOptions options)
            => duration ?? options.Duration ?? defaultDuration;

        public static Tween CreatePopIn(GameObject target, float? duration, float defaultDuration, TweenOptions options, Ease defaultEase)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var startScale = options.StartScale ?? Vector3.zero;
            var scaleTarget = options.TargetScale ?? originalScale;
            t.localScale = startScale;

            var presetOptions = WithDefaultEase(options, defaultEase);
            var ease = presetOptions.Ease ?? defaultEase;
            var dur = ResolveDuration(duration, defaultDuration, options);

            return t.DOScale(scaleTarget, dur)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }

        public static Tween CreatePopInOvershoot(GameObject target, float? duration, float defaultDuration, TweenOptions options, float defaultOvershoot)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var startScale = options.StartScale ?? Vector3.zero;
            var scaleTarget = options.TargetScale ?? originalScale;
            t.localScale = startScale;

            var dur = ResolveDuration(duration, defaultDuration, options);
            var presetOptions = WithDefaultEase(options, Ease.OutQuad);
            var overshootScale = scaleTarget * (1.0f + defaultOvershoot * (presetOptions.Overshoot ?? 1f));

            var seq = DOTween.Sequence();
            seq.Append(t.DOScale(overshootScale, dur).SetEase(Ease.OutQuad));
            seq.Append(t.DOScale(scaleTarget, dur * 0.5f).SetEase(Ease.InOutSine));
            return seq.WithDefaults(presetOptions, target);
        }

        public static Tween CreatePopOut(GameObject target, float? duration, float defaultDuration, TweenOptions options, Ease defaultEase)
        {
            var presetOptions = WithDefaultEase(options, defaultEase);
            var ease = presetOptions.Ease ?? defaultEase;
            var endScale = options.TargetScale ?? Vector3.zero;
            var dur = ResolveDuration(duration, defaultDuration, options);

            return target.transform.DOScale(endScale, dur)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }

        public static Tween CreatePopOutOvershoot(GameObject target, float? duration, float defaultDuration, TweenOptions options, float defaultOvershoot)
        {
            var t = target.transform;
            var dur = ResolveDuration(duration, defaultDuration, options);
            var presetOptions = WithDefaultEase(options, Ease.InBack);
            var ease = presetOptions.Ease ?? Ease.InBack;
            var overshoot = defaultOvershoot * (presetOptions.Overshoot ?? 1f);
            var endScale = options.TargetScale ?? Vector3.zero;

            var seq = DOTween.Sequence();
            seq.Join(t.DOScale(endScale, dur).SetEase(ease, overshoot));
            return seq.WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Scales cleanly from zero to original scale without overshoot.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c> and animates to original scale using <c>Ease.OutQuad</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces OutQuad.<br/>
    /// <b>Scale override:</b> StartScale replaces zero; TargetScale replaces original scale.
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
            return PopFactory.CreatePopIn(target, duration, DefaultDuration, options, Ease.OutQuad);
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
    /// <b>Easing override:</b> Primary ease applies to the sequence.<br/>
    /// <b>Scale override:</b> StartScale replaces zero; TargetScale replaces original scale (overshoot is relative to TargetScale).
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
            return PopFactory.CreatePopInOvershoot(target, duration, DefaultDuration, options, DefaultOvershoot);
        }
    }

    /// <summary>
    /// Scales the target down to zero with a clean deceleration, no anticipation overshoot.
    /// <para>
    /// Animates scale to <c>Vector3.zero</c> using <c>Ease.InCubic</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.4s | <b>Default ease:</b> InCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces InCubic.<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.
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
            return PopFactory.CreatePopOut(target, duration, DefaultDuration, options, Ease.InCubic);
        }
    }

    /// <summary>
    /// Soft exit scaling to zero with a slow InSine ease.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.6s | <b>Default ease:</b> InSine<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.
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
            return PopFactory.CreatePopOut(target, duration, DefaultDuration, options, Ease.InSine);
        }
    }

    /// <summary>
    /// Hard exit scaling to zero with a snappy InQuart ease.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.25s | <b>Default ease:</b> InQuart<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.
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
            return PopFactory.CreatePopOut(target, duration, DefaultDuration, options, Ease.InQuart);
        }
    }

    /// <summary>
    /// Scales the target down to zero with a brief anticipation pull-back.
    /// <para>
    /// Animates scale to <c>Vector3.zero</c> using <c>Ease.InBack</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.4s | <b>Default ease:</b> InBack<br/>
    /// <b>Easing override:</b> Primary ease replaces InBack.<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.
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
            return PopFactory.CreatePopOutOvershoot(target, duration, DefaultDuration, options, DefaultOvershoot);
        }
    }

    /// <summary>
    /// Scales the target down to zero with a mild anticipation pull-back (overshoot 2.5).
    /// <para>
    /// Animates scale to <c>Vector3.zero</c> using <c>Ease.InBack</c> with overshoot 2.5.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.6s | <b>Default ease:</b> InBack (overshoot 2.5)<br/>
    /// <b>Easing override:</b> Primary ease replaces InBack; overshoot parameter is preserved.<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.
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
            return PopFactory.CreatePopOutOvershoot(target, duration, DefaultDuration, options, DefaultOvershoot);
        }
    }

    /// <summary>
    /// Scales the target down to zero with a strong anticipation pull-back (overshoot 6.0).
    /// <para>
    /// Animates scale to <c>Vector3.zero</c> using <c>Ease.InBack</c> with overshoot 6.0.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.25s | <b>Default ease:</b> InBack (overshoot 6.0)<br/>
    /// <b>Easing override:</b> Primary ease replaces InBack; overshoot parameter is preserved.<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.
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
            return PopFactory.CreatePopOutOvershoot(target, duration, DefaultDuration, options, DefaultOvershoot);
        }
    }

    /// <summary>
    /// Gentle entrance scaling from zero with a slow OutSine ease, no overshoot.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.8s | <b>Default ease:</b> OutSine<br/>
    /// <b>Scale override:</b> StartScale replaces zero; TargetScale replaces original scale.
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
            return PopFactory.CreatePopIn(target, duration, DefaultDuration, options, Ease.OutSine);
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
    /// <b>Easing override:</b> Primary ease applies to the sequence.<br/>
    /// <b>Scale override:</b> StartScale replaces zero; TargetScale replaces original scale (overshoot is relative to TargetScale).
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
            return PopFactory.CreatePopInOvershoot(target, duration, DefaultDuration, options, DefaultOvershoot);
        }
    }

    /// <summary>
    /// Fast entrance scaling from zero with a snappy OutQuart ease, no overshoot.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.3s | <b>Default ease:</b> OutQuart<br/>
    /// <b>Scale override:</b> StartScale replaces zero; TargetScale replaces original scale.
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
            return PopFactory.CreatePopIn(target, duration, DefaultDuration, options, Ease.OutQuart);
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
    /// <b>Easing override:</b> Primary ease applies to the sequence.<br/>
    /// <b>Scale override:</b> StartScale replaces zero; TargetScale replaces original scale (overshoot is relative to TargetScale).
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
            return PopFactory.CreatePopInOvershoot(target, duration, DefaultDuration, options, DefaultOvershoot);
        }
    }
}
