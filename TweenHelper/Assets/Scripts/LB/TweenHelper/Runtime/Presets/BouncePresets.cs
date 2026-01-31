using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Performs a squash-and-stretch scale pulse, compressing and elongating before settling back to original scale.
    /// <para>
    /// Builds a 4-step sequence: (1) squash wide/short <c>(1.3x, 0.7y)</c> at 15% duration,
    /// (2) stretch tall/narrow <c>(0.8x, 1.2y)</c> at 15%, (3) slight overshoot <c>(1.1x, 0.9y)</c> at 15%,
    /// (4) settle to original scale at 25% with <c>Ease.OutElastic</c>. Z scale is unchanged.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutElastic (final step only)<br/>
    /// <b>Easing override:</b> Primary ease replaces OutElastic on the final settle step.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Character land impact, rubber-ball effect, playful UI feedback, cartoon-style emphasis.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Squash").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BouncePreset : CodePreset
    {
        public override string PresetName => "Squash";
        public override string Description => "Squash and stretch pulse";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutElastic);
            var ease = ResolveEase(presetOptions, Ease.OutElastic);

            // Squash and stretch style bounce
            return DOTween.Sequence()
                .Append(t.DOScale(new Vector3(originalScale.x * 1.3f, originalScale.y * 0.7f, originalScale.z), dur * 0.15f))
                .Append(t.DOScale(new Vector3(originalScale.x * 0.8f, originalScale.y * 1.2f, originalScale.z), dur * 0.15f))
                .Append(t.DOScale(new Vector3(originalScale.x * 1.1f, originalScale.y * 0.9f, originalScale.z), dur * 0.15f))
                .Append(t.DOScale(originalScale, dur * 0.25f).SetEase(ease))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Bounces the target on the Y axis with three progressively smaller hops using a sequence.
    /// <para>
    /// Builds a 6-step Y-axis sequence from the current base Y: hop 1 rises <c>+1.5</c> (15% duration each way),
    /// hop 2 rises <c>+0.8</c> (12% each way), hop 3 rises <c>+0.3</c> (10% each way).
    /// Up phases use <c>Ease.OutQuad</c> (decelerating rise), down phases use <c>Ease.InQuad</c> (accelerating fall).
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.8s | <b>Default ease:</b> OutQuad (up), InQuad (down)<br/>
    /// <b>Easing override:</b> Primary ease controls up phases; secondary ease controls down phases.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Ball bounce, character hop, playful feedback, item arrival.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Bounce").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PositionalBouncePreset : CodePreset
    {
        public override string PresetName => "Bounce";
        public override string Description => "Positional Y bounce with decreasing hops";
        public override float DefaultDuration => 0.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var baseY = t.localPosition.y;
            var dur = GetDuration(duration);
            var upEase = ResolveEase(options, Ease.OutQuad);
            var downEase = ResolveSecondaryEase(options, Ease.InQuad);
            var presetOptions = MergeWithDefaultEase(options, upEase);

            return DOTween.Sequence()
                // Hop 1 (highest)
                .Append(t.DOLocalMoveY(baseY + 1.5f, dur * 0.15f).SetEase(upEase))
                .Append(t.DOLocalMoveY(baseY, dur * 0.15f).SetEase(downEase))
                // Hop 2 (medium)
                .Append(t.DOLocalMoveY(baseY + 0.8f, dur * 0.12f).SetEase(upEase))
                .Append(t.DOLocalMoveY(baseY, dur * 0.12f).SetEase(downEase))
                // Hop 3 (small)
                .Append(t.DOLocalMoveY(baseY + 0.3f, dur * 0.1f).SetEase(upEase))
                .Append(t.DOLocalMoveY(baseY, dur * 0.1f).SetEase(downEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Light positional Y bounce with small hops.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutQuad (up), InQuad (down)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("BounceSmall").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BounceSmallPreset : CodePreset
    {
        public override string PresetName => "BounceSmall";
        public override string Description => "Light bounce";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var baseY = t.localPosition.y;
            var dur = GetDuration(duration);
            var upEase = ResolveEase(options, Ease.OutQuad);
            var downEase = ResolveSecondaryEase(options, Ease.InQuad);
            var presetOptions = MergeWithDefaultEase(options, upEase);

            return DOTween.Sequence()
                .Append(t.DOLocalMoveY(baseY + 0.75f, dur * 0.15f).SetEase(upEase))
                .Append(t.DOLocalMoveY(baseY, dur * 0.15f).SetEase(downEase))
                .Append(t.DOLocalMoveY(baseY + 0.4f, dur * 0.12f).SetEase(upEase))
                .Append(t.DOLocalMoveY(baseY, dur * 0.12f).SetEase(downEase))
                .Append(t.DOLocalMoveY(baseY + 0.15f, dur * 0.1f).SetEase(upEase))
                .Append(t.DOLocalMoveY(baseY, dur * 0.1f).SetEase(downEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Medium positional Y bounce with moderate hops.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.7s | <b>Default ease:</b> OutQuad (up), InQuad (down)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("BounceMedium").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BounceMediumPreset : CodePreset
    {
        public override string PresetName => "BounceMedium";
        public override string Description => "Medium bounce";
        public override float DefaultDuration => 0.7f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var baseY = t.localPosition.y;
            var dur = GetDuration(duration);
            var upEase = ResolveEase(options, Ease.OutQuad);
            var downEase = ResolveSecondaryEase(options, Ease.InQuad);
            var presetOptions = MergeWithDefaultEase(options, upEase);

            return DOTween.Sequence()
                .Append(t.DOLocalMoveY(baseY + 1.1f, dur * 0.15f).SetEase(upEase))
                .Append(t.DOLocalMoveY(baseY, dur * 0.15f).SetEase(downEase))
                .Append(t.DOLocalMoveY(baseY + 0.6f, dur * 0.12f).SetEase(upEase))
                .Append(t.DOLocalMoveY(baseY, dur * 0.12f).SetEase(downEase))
                .Append(t.DOLocalMoveY(baseY + 0.22f, dur * 0.1f).SetEase(upEase))
                .Append(t.DOLocalMoveY(baseY, dur * 0.1f).SetEase(downEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Heavy positional Y bounce with tall hops.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> OutQuad (up), InQuad (down)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("BounceLarge").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BounceLargePreset : CodePreset
    {
        public override string PresetName => "BounceLarge";
        public override string Description => "Heavy bounce with tall hops";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var baseY = t.localPosition.y;
            var dur = GetDuration(duration);
            var upEase = ResolveEase(options, Ease.OutQuad);
            var downEase = ResolveSecondaryEase(options, Ease.InQuad);
            var presetOptions = MergeWithDefaultEase(options, upEase);

            return DOTween.Sequence()
                .Append(t.DOLocalMoveY(baseY + 2.5f, dur * 0.15f).SetEase(upEase))
                .Append(t.DOLocalMoveY(baseY, dur * 0.15f).SetEase(downEase))
                .Append(t.DOLocalMoveY(baseY + 1.3f, dur * 0.12f).SetEase(upEase))
                .Append(t.DOLocalMoveY(baseY, dur * 0.12f).SetEase(downEase))
                .Append(t.DOLocalMoveY(baseY + 0.5f, dur * 0.1f).SetEase(upEase))
                .Append(t.DOLocalMoveY(baseY, dur * 0.1f).SetEase(downEase))
                .WithDefaults(presetOptions, target);
        }
    }
}
