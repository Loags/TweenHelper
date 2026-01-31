using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Flips the target 180 degrees on the X axis using additive local rotation.
    /// <para>
    /// Rotates by <c>(180, 0, 0)</c> using <c>RotateMode.LocalAxisAdd</c> with <c>Ease.InOutQuad</c>.
    /// LocalAxisAdd ensures the rotation is added to the current orientation rather than targeting
    /// an absolute angle, making it safe to chain multiple flips.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Card flip, page turn, reveal animation, object inspection.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipX").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipXPreset : CodePreset
    {
        public override string PresetName => "FlipX";
        public override string Description => "180° flip on X axis";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutQuad);
            var ease = ResolveEase(presetOptions, Ease.InOutQuad);
            return target.transform.DOLocalRotate(new Vector3(180f, 0f, 0f), GetDuration(duration), RotateMode.LocalAxisAdd)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Flips the target 180 degrees on the Y axis using additive local rotation.
    /// <para>
    /// Rotates by <c>(0, 180, 0)</c> using <c>RotateMode.LocalAxisAdd</c> with <c>Ease.InOutQuad</c>.
    /// LocalAxisAdd ensures the rotation is added to the current orientation rather than targeting
    /// an absolute angle, making it safe to chain multiple flips.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Horizontal card flip, character turn-around, mirror reveal, about-face.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipY").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipYPreset : CodePreset
    {
        public override string PresetName => "FlipY";
        public override string Description => "180° flip on Y axis";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutQuad);
            var ease = ResolveEase(presetOptions, Ease.InOutQuad);
            return target.transform.DOLocalRotate(new Vector3(0f, 180f, 0f), GetDuration(duration), RotateMode.LocalAxisAdd)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }
}
