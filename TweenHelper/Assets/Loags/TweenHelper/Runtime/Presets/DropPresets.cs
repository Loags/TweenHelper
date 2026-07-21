using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Internal factory for drop-in bounce landing presets.
    /// </summary>
    internal static class DropInFactory
    {
        public static Tween Create(
            GameObject target,
            float duration,
            TweenOptions options,
            float dropHeightBase,
            Ease defaultFallEase,
            Ease defaultBounceEase,
            float fallRatio,
            float[] bounceHeights,
            float[] bounceUpRatios,
            float[] bounceDownRatios,
            float tailIntervalRatio = 0f)
        {
            var t = target.transform;
            var targetY = t.localPosition.y;
            var strength = CodePreset.ResolveStrengthStatic(options);
            var dropHeight = dropHeightBase * strength;
            t.localPosition = t.localPosition + Vector3.up * dropHeight;

            var fallEase = options.Ease ?? defaultFallEase;
            var bounceEase = options.SecondaryEase ?? options.Ease ?? defaultBounceEase;
            var presetOptions = options.SetEase(fallEase);

            var seq = DOTween.Sequence()
                .Append(t.DOLocalMoveY(targetY, duration * fallRatio).SetEase(fallEase));

            for (int i = 0; i < bounceHeights.Length; i++)
            {
                seq.Append(t.DOLocalMoveY(targetY + dropHeight * bounceHeights[i], duration * bounceUpRatios[i]).SetEase(bounceEase));
                seq.Append(t.DOLocalMoveY(targetY, duration * bounceDownRatios[i]).SetEase(fallEase));
            }

            if (tailIntervalRatio > 0f)
            {
                seq.AppendInterval(duration * tailIntervalRatio);
            }

            return seq.WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Internal factory for bounce-land presets with squash impacts.
    /// </summary>
    internal static class BounceLandFactory
    {
        public static Tween Create(
            GameObject target,
            float duration,
            TweenOptions options,
            float dropHeightBase,
            Ease defaultFallEase,
            Ease defaultBounceEase,
            Ease squashOutEase,
            Ease squashInEase,
            float squash1Amount,
            float squash2Amount)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var targetY = t.localPosition.y;
            var strength = CodePreset.ResolveStrengthStatic(options);
            var dropHeight = dropHeightBase * strength;
            t.localPosition = t.localPosition + Vector3.up * dropHeight;

            var fallEase = options.Ease ?? defaultFallEase;
            var bounceEase = options.SecondaryEase ?? options.Ease ?? defaultBounceEase;
            var presetOptions = options.Ease.HasValue ? options : options.SetEase(fallEase);

            var squash1 = new Vector3(
                originalScale.x * (1f + squash1Amount * strength),
                originalScale.y * (1f - squash1Amount * strength),
                originalScale.z);
            var squash2 = new Vector3(
                originalScale.x * (1f + squash2Amount * strength),
                originalScale.y * (1f - squash2Amount * strength),
                originalScale.z);

            return DOTween.Sequence()
                .Append(t.DOLocalMoveY(targetY, duration * 0.3f).SetEase(fallEase))
                .Append(t.DOScale(squash1, duration * 0.05f).SetEase(squashOutEase))
                .Append(t.DOScale(originalScale, duration * 0.05f).SetEase(squashInEase))
                .Append(t.DOLocalMoveY(targetY + dropHeight * 0.35f, duration * 0.12f).SetEase(bounceEase))
                .Append(t.DOLocalMoveY(targetY, duration * 0.12f).SetEase(fallEase))
                .Append(t.DOScale(squash2, duration * 0.04f).SetEase(squashOutEase))
                .Append(t.DOScale(originalScale, duration * 0.04f).SetEase(squashInEase))
                .Append(t.DOLocalMoveY(targetY + dropHeight * 0.1f, duration * 0.09f).SetEase(bounceEase))
                .Append(t.DOLocalMoveY(targetY, duration * 0.09f).SetEase(fallEase))
                .Append(t.DOScale(originalScale, duration * 0.05f).SetEase(squashOutEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Internal factory for cartoon bounce presets with three hops and squash impacts.
    /// </summary>
    internal static class BounceCartoonFactory
    {
        public static Tween Create(
            GameObject target,
            float duration,
            TweenOptions options,
            Ease defaultFallEase,
            Ease defaultHopEase,
            Ease squashOutEase,
            Ease squashInEase,
            float hop1,
            float hop2,
            float hop3,
            float squash1Amount,
            float squash2Amount,
            float squash3Amount)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var baseY = t.localPosition.y;
            var strength = CodePreset.ResolveStrengthStatic(options);
            var fallEase = options.Ease ?? defaultFallEase;
            var hopEase = options.SecondaryEase ?? options.Ease ?? defaultHopEase;
            var presetOptions = options.Ease.HasValue ? options : options.SetEase(fallEase);

            var squash1 = new Vector3(originalScale.x * (1f + squash1Amount * strength), originalScale.y * (1f - squash1Amount * strength), originalScale.z);
            var squash2 = new Vector3(originalScale.x * (1f + squash2Amount * strength), originalScale.y * (1f - squash2Amount * strength), originalScale.z);
            var squash3 = new Vector3(originalScale.x * (1f + squash3Amount * strength), originalScale.y * (1f - squash3Amount * strength), originalScale.z);

            return DOTween.Sequence()
                .Append(t.DOLocalMoveY(baseY + hop1 * strength, duration * 0.15f).SetEase(hopEase))
                .Append(t.DOLocalMoveY(baseY, duration * 0.15f).SetEase(fallEase))
                .Append(t.DOScale(squash1, duration * 0.04f).SetEase(squashOutEase))
                .Append(t.DOScale(originalScale, duration * 0.04f).SetEase(squashInEase))
                .Append(t.DOLocalMoveY(baseY + hop2 * strength, duration * 0.1f).SetEase(hopEase))
                .Append(t.DOLocalMoveY(baseY, duration * 0.1f).SetEase(fallEase))
                .Append(t.DOScale(squash2, duration * 0.03f).SetEase(squashOutEase))
                .Append(t.DOScale(originalScale, duration * 0.03f).SetEase(squashInEase))
                .Append(t.DOLocalMoveY(baseY + hop3 * strength, duration * 0.08f).SetEase(hopEase))
                .Append(t.DOLocalMoveY(baseY, duration * 0.08f).SetEase(fallEase))
                .Append(t.DOScale(squash3, duration * 0.03f).SetEase(squashOutEase))
                .Append(t.DOScale(originalScale, duration * 0.03f).SetEase(squashInEase))
                .AppendInterval(duration * 0.14f)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Drops the target from 8 units above with three progressively smaller bounces on landing.
    /// <para>
    /// Offsets Y by <c>+8</c>, then builds a 7-step Y-axis sequence: fall to target (40%), then three
    /// bounce pairs (up/down) with decreasing heights: 30%→10%→3% of drop height. Fall phases use
    /// <c>Ease.InQuad</c> (accelerating), bounce-up phases use <c>Ease.OutQuad</c> (decelerating).
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.2s | <b>Default ease:</b> InQuad (fall), OutQuad (bounce)<br/>
    /// <b>Easing override:</b> Primary ease controls fall phases; secondary ease controls bounce-up phases.<br/>
    /// <b>Strength override:</b> Multiplies drop height and squash intensity (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Object drop entrance, physics-style landing, dramatic item reveal, game piece placement.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("DropIn").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class DropInPreset : CodePreset
    {
        public override string PresetName => "DropIn";
        public override string Description => "Falls from above with bounce on landing";
        public override float DefaultDuration => 1.2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return DropInFactory.Create(
                target,
                GetDuration(duration, options),
                options,
                8f,
                Ease.InQuad,
                Ease.OutQuad,
                0.4f,
                new[] { 0.3f, 0.1f, 0.03f },
                new[] { 0.15f, 0.1f, 0.05f },
                new[] { 0.15f, 0.1f, 0.05f });
        }
    }

    /// <summary>
    /// Gentle drop from 6 units above with soft bounce on landing.
    /// <para>
    /// Offsets Y by <c>+6</c>, then builds a sequence with <c>InSine</c> fall and 3 soft bounces
    /// with heights at 25%, 8%, 2% of drop height. Uses <c>OutSine</c> for bounce-up phases.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.5s | <b>Default ease:</b> InSine (fall), OutSine (bounce)<br/>
    /// <b>Strength override:</b> Multiplies drop height and squash intensity (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("DropInSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class DropInSoftPreset : CodePreset
    {
        public override string PresetName => "DropInSoft";
        public override string Description => "Gentle drop with soft bounce on landing";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return DropInFactory.Create(
                target,
                GetDuration(duration, options),
                options,
                6f,
                Ease.InSine,
                Ease.OutSine,
                0.4f,
                new[] { 0.25f, 0.08f, 0.02f },
                new[] { 0.15f, 0.1f, 0.05f },
                new[] { 0.15f, 0.1f, 0.05f });
        }
    }

    /// <summary>
    /// Heavy drop from 10 units above with sharper bounce decay, creating a weighty landing effect.
    /// <para>
    /// Offsets Y by <c>+10</c>, then builds a sequence with <c>InCubic</c> fall and 2 bounces
    /// with sharper height decay than standard DropIn.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.9s | <b>Default ease:</b> InCubic (fall), OutQuad (bounce)<br/>
    /// <b>Strength override:</b> Multiplies drop height and squash intensity (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("DropInHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class DropInHardPreset : CodePreset
    {
        public override string PresetName => "DropInHard";
        public override string Description => "Heavy drop with sharp bounce decay";
        public override float DefaultDuration => 0.9f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return DropInFactory.Create(
                target,
                GetDuration(duration, options),
                options,
                10f,
                Ease.InCubic,
                Ease.OutQuad,
                0.45f,
                new[] { 0.15f, 0.04f },
                new[] { 0.13f, 0.1f },
                new[] { 0.13f, 0.1f },
                tailIntervalRatio: 0.09f);
        }
    }

    /// <summary>
    /// Drops the target from 5 units above with two bounces, applying squash-stretch deformation on each impact.
    /// <para>
    /// Offsets Y by <c>+5</c>, then builds a multi-step sequence combining position and scale:
    /// (1) fall to target (30%), (2) squash on impact <c>(1.3x, 0.7y)</c> then restore (5%+5%),
    /// (3) bounce 1 up to 35% height (12%+12%), (4) smaller squash <c>(1.15x, 0.85y)</c> then restore (4%+4%),
    /// (5) bounce 2 up to 10% height (9%+9%), (6) final scale settle (5%).
    /// Falls use <c>Ease.InQuad</c>, bounces use <c>Ease.OutQuad</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 2.0s | <b>Default ease:</b> InQuad (fall), OutQuad (bounce)<br/>
    /// <b>Easing override:</b> Primary ease controls falls; secondary ease controls bounce-ups.<br/>
    /// <b>Strength override:</b> Multiplies drop height and squash intensity (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Heavy object landing, character ground pound, cartoon drop, dramatic arrival.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("BounceLand").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BounceLandPreset : CodePreset
    {
        public override string PresetName => "BounceLand";
        public override string Description => "Drop with bounce and squash-stretch on landing";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return BounceLandFactory.Create(target, GetDuration(duration, options), options, 5f, Ease.InQuad, Ease.OutQuad, Ease.OutQuad, Ease.InQuad, 0.3f, 0.15f);
        }
    }

    /// <summary>
    /// Cartoon-style bounce with a big hop, squash-stretch on landing, and three decreasing hops.
    /// <para>
    /// Builds a multi-step sequence: initial hop to 2.5 units, squash on landing (1.4x wide, 0.6y tall),
    /// then 3 decreasing hops with progressively smaller squash. Total duration 1.4s.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.4s | <b>Default ease:</b> InQuad (fall), OutQuad (hop)<br/>
    /// <b>Strength override:</b> Multiplies drop height and squash intensity (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("BounceCartoon").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BounceCartoonPreset : CodePreset
    {
        public override string PresetName => "BounceCartoon";
        public override string Description => "Cartoon bounce with squash-stretch on landing";
        public override float DefaultDuration => 1.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return BounceCartoonFactory.Create(target, GetDuration(duration, options), options, Ease.InQuad, Ease.OutQuad, Ease.OutQuad, Ease.InQuad, 2.5f, 1.2f, 0.5f, 0.4f, 0.25f, 0.12f);
        }
    }

    /// <summary>
    /// Gentle drop from 4 units above with soft bounce and mild squash-stretch on landing.
    /// <para>
    /// Offsets Y by <c>+4</c>, then builds a multi-step sequence with <c>InSine</c> fall
    /// and gentle squash deformation <c>(1.2x, 0.8y)</c> / <c>(1.1x, 0.9y)</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 2.4s | <b>Default ease:</b> InSine (fall), OutSine (bounce)<br/>
    /// <b>Strength override:</b> Multiplies drop height and squash intensity (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("BounceLandSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BounceLandSoftPreset : CodePreset
    {
        public override string PresetName => "BounceLandSoft";
        public override string Description => "Gentle drop with soft bounce and mild squash-stretch";
        public override float DefaultDuration => 2.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return BounceLandFactory.Create(target, GetDuration(duration, options), options, 4f, Ease.InSine, Ease.OutSine, Ease.OutSine, Ease.InSine, 0.2f, 0.1f);
        }
    }

    /// <summary>
    /// Heavy drop from 7 units above with sharp bounce and exaggerated squash-stretch on landing.
    /// <para>
    /// Offsets Y by <c>+7</c>, then builds a multi-step sequence with <c>InCubic</c> fall
    /// and strong squash deformation <c>(1.45x, 0.55y)</c> / <c>(1.25x, 0.75y)</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.6s | <b>Default ease:</b> InCubic (fall), OutQuad (bounce)<br/>
    /// <b>Strength override:</b> Multiplies drop height and squash intensity (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("BounceLandHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BounceLandHardPreset : CodePreset
    {
        public override string PresetName => "BounceLandHard";
        public override string Description => "Heavy drop with sharp bounce and exaggerated squash-stretch";
        public override float DefaultDuration => 1.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return BounceLandFactory.Create(target, GetDuration(duration, options), options, 7f, Ease.InCubic, Ease.OutQuad, Ease.OutQuad, Ease.InQuad, 0.45f, 0.25f);
        }
    }

    /// <summary>
    /// Soft cartoon-style bounce with smaller hops and gentler squash-stretch.
    /// <para>
    /// Builds a multi-step sequence: initial hop to 1.5 units, then decreasing hops (0.7, 0.3)
    /// with mild squash deformation. Uses <c>InSine</c>/<c>OutSine</c> easing.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.8s | <b>Default ease:</b> InSine (fall), OutSine (hop)<br/>
    /// <b>Strength override:</b> Multiplies drop height and squash intensity (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("BounceCartoonSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BounceCartoonSoftPreset : CodePreset
    {
        public override string PresetName => "BounceCartoonSoft";
        public override string Description => "Soft cartoon bounce with gentle squash-stretch";
        public override float DefaultDuration => 1.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return BounceCartoonFactory.Create(target, GetDuration(duration, options), options, Ease.InSine, Ease.OutSine, Ease.OutSine, Ease.InSine, 1.5f, 0.7f, 0.3f, 0.25f, 0.15f, 0.08f);
        }
    }

    /// <summary>
    /// Hard cartoon-style bounce with exaggerated hops and strong squash-stretch.
    /// <para>
    /// Builds a multi-step sequence: initial hop to 3.5 units, then decreasing hops (1.8, 0.8)
    /// with heavy squash deformation. Uses <c>InCubic</c>/<c>OutQuad</c> easing.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.1s | <b>Default ease:</b> InCubic (fall), OutQuad (hop)<br/>
    /// <b>Strength override:</b> Multiplies drop height and squash intensity (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("BounceCartoonHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BounceCartoonHardPreset : CodePreset
    {
        public override string PresetName => "BounceCartoonHard";
        public override string Description => "Hard cartoon bounce with exaggerated squash-stretch";
        public override float DefaultDuration => 1.1f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return BounceCartoonFactory.Create(target, GetDuration(duration, options), options, Ease.InCubic, Ease.OutQuad, Ease.OutQuad, Ease.InQuad, 3.5f, 1.8f, 0.8f, 0.55f, 0.35f, 0.18f);
        }
    }
}
