using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    internal static class BounceMotionUtility
    {
        private const float CanvasDistanceMultiplier = 40f;

        public static float GetBaseY(GameObject target)
        {
            return TweenTargetUtility.TryGetRectTransform(target, out var rectTransform)
                ? rectTransform.anchoredPosition.y
                : target.transform.localPosition.y;
        }

        public static float GetDistanceMultiplier(GameObject target)
        {
            return TweenTargetUtility.TryGetRectTransform(target, out _) ? CanvasDistanceMultiplier : 1f;
        }

        public static Tween CreateYTween(GameObject target, float y, float duration)
        {
            return TweenTargetUtility.TryGetRectTransform(target, out var rectTransform)
                ? rectTransform.DOAnchorPosY(y, duration)
                : target.transform.DOLocalMoveY(y, duration);
        }
    }

    /// <summary>
    /// Internal factory for squash-and-stretch pulse presets.
    /// </summary>
    internal static class SquashFactory
    {
        public static Tween Create(
            GameObject target,
            float duration,
            TweenOptions options,
            Ease finalEaseDefault,
            float squash1,
            float stretch,
            float squash2)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var strength = CodePreset.ResolveStrengthStatic(options);
            var finalEase = options.Ease ?? finalEaseDefault;
            var sequenceOptions = options.SetEase(Ease.Linear);

            return DOTween.Sequence()
                .Append(t.DOScale(new Vector3(originalScale.x * (1f + squash1 * strength), originalScale.y * (1f - squash1 * strength), originalScale.z), duration * 0.15f))
                .Append(t.DOScale(new Vector3(originalScale.x * (1f - stretch * strength), originalScale.y * (1f + stretch * strength), originalScale.z), duration * 0.15f))
                .Append(t.DOScale(new Vector3(originalScale.x * (1f + squash2 * strength), originalScale.y * (1f - squash2 * strength), originalScale.z), duration * 0.15f))
                .Append(t.DOScale(originalScale, duration * 0.25f).SetEase(finalEase))
                .WithDefaults(sequenceOptions, target);
        }
    }

    /// <summary>
    /// Internal factory for positional Y-bounce presets.
    /// </summary>
    internal static class PositionalBounceFactory
    {
        public static Tween Create(GameObject target, float duration, TweenOptions options, float hop1, float hop2, float hop3)
        {
            var baseY = BounceMotionUtility.GetBaseY(target);
            var strength = CodePreset.ResolveStrengthStatic(options);
            var distanceScale = strength * BounceMotionUtility.GetDistanceMultiplier(target);
            var upEase = options.Ease ?? Ease.OutQuad;
            var downEase = options.SecondaryEase ?? options.Ease ?? Ease.InQuad;
            var sequenceOptions = options.SetEase(Ease.Linear);

            return DOTween.Sequence()
                .Append(BounceMotionUtility.CreateYTween(target, baseY + hop1 * distanceScale, duration * 0.2f).SetEase(upEase))
                .Append(BounceMotionUtility.CreateYTween(target, baseY, duration * 0.2f).SetEase(downEase))
                .Append(BounceMotionUtility.CreateYTween(target, baseY + hop2 * distanceScale, duration * 0.17f).SetEase(upEase))
                .Append(BounceMotionUtility.CreateYTween(target, baseY, duration * 0.17f).SetEase(downEase))
                .Append(BounceMotionUtility.CreateYTween(target, baseY + hop3 * distanceScale, duration * 0.13f).SetEase(upEase))
                .Append(BounceMotionUtility.CreateYTween(target, baseY, duration * 0.13f).SetEase(downEase))
                .WithDefaults(sequenceOptions, target);
        }
    }

    /// <summary>
    /// Performs a squash-and-stretch scale pulse, compressing and elongating before settling back to original scale.
    /// <para>
    /// Builds a 4-step sequence: (1) squash wide/short <c>(1.3x, 0.7y)</c> at 15% duration,
    /// (2) stretch tall/narrow <c>(0.8x, 1.2y)</c> at 15%, (3) slight overshoot <c>(1.1x, 0.9y)</c> at 15%,
    /// (4) settle to original scale at 25% with <c>Ease.OutElastic</c>. Z scale is unchanged.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutElastic (final step only)<br/>
    /// <b>Easing override:</b> Primary ease replaces OutElastic on the final settle step.<br/>
    /// <b>Strength override:</b> Multiplies bounce height and squash intensity (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Character land impact, rubber-ball effect, playful UI feedback, cartoon-style emphasis.
    /// </para>
    /// Usage: <c>transform.Tween().Preset<BouncePreset>().Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BouncePreset : CodePreset
    {
        public override string PresetName => "Squash";
        public override string Description => "Squash and stretch pulse";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SquashFactory.Create(target, GetDuration(duration, options), options, Ease.OutElastic, 0.3f, 0.2f, 0.1f);
        }
    }

    /// <summary>
    /// Soft squash-and-stretch with smaller deformation.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutElastic (final step only)<br/>
    /// <b>Strength override:</b> Multiplies bounce height and squash intensity (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset<SquashSoftPreset>().Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SquashSoftPreset : CodePreset
    {
        public override string PresetName => "SquashSoft";
        public override string Description => "Soft squash and stretch pulse";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SquashFactory.Create(target, GetDuration(duration, options), options, Ease.OutElastic, 0.15f, 0.1f, 0.05f);
        }
    }

    /// <summary>
    /// Hard squash-and-stretch with exaggerated deformation.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.7s | <b>Default ease:</b> OutElastic (final step only)<br/>
    /// <b>Strength override:</b> Multiplies bounce height and squash intensity (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset<SquashHardPreset>().Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SquashHardPreset : CodePreset
    {
        public override string PresetName => "SquashHard";
        public override string Description => "Hard squash and stretch pulse";
        public override float DefaultDuration => 0.7f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SquashFactory.Create(target, GetDuration(duration, options), options, Ease.OutElastic, 0.5f, 0.35f, 0.15f);
        }
    }

    /// <summary>
    /// Bounces the target on the Y axis with three progressively smaller hops using a sequence.
    /// <para>
    /// Builds a 6-step Y-axis sequence: hop 1 rises <c>+1.1</c>, hop 2 rises <c>+0.6</c>, hop 3 rises <c>+0.22</c>.
    /// RectTransform targets use canvas-appropriate pixel distances automatically.
    /// Up phases use <c>Ease.OutQuad</c>, down phases use <c>Ease.InQuad</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> OutQuad (up), InQuad (down)<br/>
    /// <b>Easing override:</b> Primary ease controls up phases; secondary ease controls down phases.<br/>
    /// <b>Strength override:</b> Multiplies bounce height and squash intensity (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Ball bounce, character hop, playful feedback, item arrival.
    /// </para>
    /// Usage: <c>transform.Tween().Preset<PositionalBouncePreset>().Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PositionalBouncePreset : CodePreset
    {
        public override string PresetName => "Bounce";
        public override string Description => "Positional Y bounce with decreasing hops";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PositionalBounceFactory.Create(target, GetDuration(duration, options), options, 1.1f, 0.6f, 0.22f);
        }
    }

    /// <summary>
    /// Soft positional Y bounce with small hops.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.9s | <b>Default ease:</b> OutQuad (up), InQuad (down)<br/>
    /// <b>Strength override:</b> Multiplies bounce height and squash intensity (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset<BounceSoftPreset>().Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BounceSoftPreset : CodePreset
    {
        public override string PresetName => "BounceSoft";
        public override string Description => "Soft bounce";
        public override float DefaultDuration => 0.9f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PositionalBounceFactory.Create(target, GetDuration(duration, options), options, 0.75f, 0.4f, 0.15f);
        }
    }

    /// <summary>
    /// Heavy positional Y bounce with tall hops.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.3s | <b>Default ease:</b> OutQuad (up), InQuad (down)<br/>
    /// <b>Strength override:</b> Multiplies bounce height and squash intensity (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset<BounceHardPreset>().Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BounceHardPreset : CodePreset
    {
        public override string PresetName => "BounceHard";
        public override string Description => "Heavy bounce with tall hops";
        public override float DefaultDuration => 1.3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PositionalBounceFactory.Create(target, GetDuration(duration, options), options, 2.5f, 1.3f, 0.5f);
        }
    }
}
