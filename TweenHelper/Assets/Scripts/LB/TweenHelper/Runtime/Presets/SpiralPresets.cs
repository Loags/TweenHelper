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
            var dur = GetDuration(duration);
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
            t.localScale = Vector3.zero;
            var dur = GetDuration(duration);
            var scaleEase = ResolveEase(options, Ease.OutBack);
            var spinEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);

            return DOTween.Sequence()
                .Append(t.DOScale(originalScale, dur).SetEase(scaleEase))
                .Join(t.DORotate(new Vector3(0f, 360f, 0f), dur, RotateMode.FastBeyond360).SetEase(spinEase))
                .WithDefaults(presetOptions, target);
        }
    }
}
