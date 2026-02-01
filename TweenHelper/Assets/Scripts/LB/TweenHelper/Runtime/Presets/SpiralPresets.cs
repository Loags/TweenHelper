using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Spirals the target upward in a helical path that tightens as it rises, combining circular XZ motion with vertical climb.
    /// <para>
    /// Uses <c>DOTween.To</c> on a 0→1 progress value. At each frame, computes a helix position:
    /// X/Z from cos/sin of <c>progress * 2π * turns</c> (default 2 turns) scaled by <c>radius * (1 - progress)</c>
    /// (shrinking radius), Y from <c>progress * height</c> (default 2 units). Serialized fields:
    /// <c>turns=2</c>, <c>height=2</c>, <c>radius=0.5</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.5s | <b>Default ease:</b> OutQuad<br/>
    /// <b>Easing override:</b> Primary ease controls progress curve (affects spiral speed distribution).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Item ascension, magical effect, tornado-style rise, collectible pickup trail.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Spiral").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpiralPreset : CodePreset
    {
        public override string PresetName => "Spiral";
        public override string Description => "Spirals upward combining rotation and height";
        public override float DefaultDuration => 1.5f;


        [SerializeField] private float turns = 2f;
        [SerializeField] private float height = 2f;
        [SerializeField] private float radius = 0.5f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var startPos = t.localPosition;
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutQuad);
            var ease = ResolveEase(presetOptions, Ease.OutQuad);

            float progress = 0f;
            return DOTween.To(() => progress, x =>
                {
                    progress = x;
                    var rad = progress * Mathf.PI * 2f * turns;
                    t.localPosition = startPos + new Vector3(
                        Mathf.Cos(rad) * radius * (1f - progress),
                        progress * height,
                        Mathf.Sin(rad) * radius * (1f - progress)
                    );
                }, 1f, dur)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Internal factory for Spiral variants sharing the same helical path structure.
    /// </summary>
    internal static class SpiralFactory
    {
        public static Tween Create(GameObject target, float turns, float height, float radius, float duration, TweenOptions options)
        {
            var t = target.transform;
            var startPos = t.localPosition;
            var ease = options.Ease ?? Ease.OutQuad;
            var presetOptions = options.Ease.HasValue ? options : options.SetEase(ease);

            float progress = 0f;
            return DOTween.To(() => progress, x =>
                {
                    progress = x;
                    var rad = progress * Mathf.PI * 2f * turns;
                    t.localPosition = startPos + new Vector3(
                        Mathf.Cos(rad) * radius * (1f - progress),
                        progress * height,
                        Mathf.Sin(rad) * radius * (1f - progress)
                    );
                }, 1f, duration)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Gentle upward spiral with smaller radius.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.2s | <b>Default ease:</b> OutQuad
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpiralSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpiralSoftPreset : CodePreset
    {
        public override string PresetName => "SpiralSoft";
        public override string Description => "Gentle upward spiral with smaller radius";
        public override float DefaultDuration => 1.2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SpiralFactory.Create(target, 1.5f, 1.2f, 0.3f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Dramatic upward spiral with wider radius.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 2.0s | <b>Default ease:</b> OutQuad
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpiralHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpiralHardPreset : CodePreset
    {
        public override string PresetName => "SpiralHard";
        public override string Description => "Dramatic upward spiral with wider radius";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SpiralFactory.Create(target, 3f, 3f, 0.7f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Scales the target from zero to original while spinning 360 degrees on Y, creating a whirl-in entrance.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c>. Builds a parallel sequence: scale to original with
    /// <c>Ease.OutBack</c> (overshoot entrance), joined with 360° Y rotation using
    /// <c>RotateMode.FastBeyond360</c> and <c>Ease.Linear</c> (uniform spin).
    /// The inverse of SpinScaleOut (which shrinks while spinning).
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.0s | <b>Default ease:</b> OutBack (scale), Linear (spin)<br/>
    /// <b>Easing override:</b> Primary ease controls scale; secondary ease controls spin.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Magical entrance, vortex appearance, power-up spawn, dramatic reveal.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SwirlIn").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SwirlInPreset : CodePreset
    {
        public override string PresetName => "SwirlIn";
        public override string Description => "Spin and scale in from zero";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var start = ResolveStartScale(options, Vector3.zero);
            var scaleTarget = ResolveTargetScale(options, originalScale);
            t.localScale = start;
            var dur = GetDuration(duration, options);
            var scaleEase = ResolveEase(options, Ease.OutBack);
            var spinEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);

            return DOTween.Sequence()
                .Append(t.DOScale(scaleTarget, dur).SetEase(scaleEase))
                .Join(t.DORotate(new Vector3(0f, 360f, 0f), dur, RotateMode.FastBeyond360).SetEase(spinEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Internal factory for SwirlIn variants sharing the same scale-from-zero + spin structure.
    /// </summary>
    internal static class SwirlInFactory
    {
        public static Tween Create(GameObject target, float spinDegrees, Ease scaleEaseDefault, float duration, TweenOptions options)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var start = options.StartScale ?? Vector3.zero;
            var scaleTarget = options.TargetScale ?? originalScale;
            t.localScale = start;
            var scaleEase = options.Ease ?? scaleEaseDefault;
            var spinEase = options.SecondaryEase ?? options.Ease ?? Ease.Linear;
            var presetOptions = options.Ease.HasValue ? options : options.SetEase(scaleEase);

            return DOTween.Sequence()
                .Append(t.DOScale(scaleTarget, duration).SetEase(scaleEase))
                .Join(t.DORotate(new Vector3(0f, spinDegrees, 0f), duration, RotateMode.FastBeyond360).SetEase(spinEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Gentle spin and scale in from zero with less rotation.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.8s | <b>Default ease:</b> OutQuad (scale), Linear (spin)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SwirlInSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SwirlInSoftPreset : CodePreset
    {
        public override string PresetName => "SwirlInSoft";
        public override string Description => "Gentle spin and scale in from zero";
        public override float DefaultDuration => 0.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SwirlInFactory.Create(target, 270f, Ease.OutQuad, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Dramatic spin and scale in from zero with extra rotation.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.3s | <b>Default ease:</b> OutBack (scale), Linear (spin)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SwirlInHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SwirlInHardPreset : CodePreset
    {
        public override string PresetName => "SwirlInHard";
        public override string Description => "Dramatic spin and scale in from zero";
        public override float DefaultDuration => 1.3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SwirlInFactory.Create(target, 540f, Ease.OutBack, GetDuration(duration, options), options);
        }
    }
}
