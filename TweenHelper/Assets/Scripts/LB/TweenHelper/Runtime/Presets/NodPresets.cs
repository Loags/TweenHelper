using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Tilts the target 15 degrees forward on the X axis then springs back to the original rotation.
    /// <para>
    /// Builds a 2-step sequence: (1) rotate to <c>original + (15, 0, 0)</c> over 40% duration with
    /// <c>Ease.OutQuad</c>, (2) return to original rotation over 60% duration with <c>Ease.OutBack</c>
    /// (springy overshoot on return). Same structure as Tilt but operates on the X axis.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.4s | <b>Default ease:</b> OutQuad (lean), OutBack (return)<br/>
    /// <b>Easing override:</b> Primary ease controls forward lean; secondary ease controls springy return.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Agreeing nod, acknowledgment gesture, bow, forward peek.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Nod").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NodPreset : CodePreset
    {
        public override string PresetName => "Nod";
        public override string Description => "X-axis tilt forward then spring back";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalRot = t.localEulerAngles;
            var dur = GetDuration(duration);
            var leanEase = ResolveEase(options, Ease.OutQuad);
            var returnEase = ResolveSecondaryEase(options, Ease.OutBack);
            var presetOptions = MergeWithDefaultEase(options, leanEase);

            return DOTween.Sequence()
                .Append(t.DOLocalRotate(originalRot + new Vector3(15f, 0f, 0f), dur * 0.4f).SetEase(leanEase))
                .Append(t.DOLocalRotate(originalRot, dur * 0.6f).SetEase(returnEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Internal factory for Nod variants sharing the same tilt-and-spring structure.
    /// </summary>
    internal static class NodFactory
    {
        public static Tween Create(GameObject target, float angle, float duration, TweenOptions options)
        {
            var t = target.transform;
            var originalRot = t.localEulerAngles;
            var leanEase = options.Ease ?? Ease.OutQuad;
            var returnEase = options.SecondaryEase ?? options.Ease ?? Ease.OutBack;
            var presetOptions = options.Ease.HasValue ? options : options.SetEase(leanEase);

            return DOTween.Sequence()
                .Append(t.DOLocalRotate(originalRot + new Vector3(angle, 0f, 0f), duration * 0.4f).SetEase(leanEase))
                .Append(t.DOLocalRotate(originalRot, duration * 0.6f).SetEase(returnEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Subtle forward tilt and spring back on X axis.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.3s | <b>Default ease:</b> OutQuad (lean), OutBack (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("NodSmall").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NodSmallPreset : CodePreset
    {
        public override string PresetName => "NodSmall";
        public override string Description => "Subtle forward tilt and spring back";
        public override float DefaultDuration => 0.3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return NodFactory.Create(target, 8f, GetDuration(duration), options);
        }
    }

    /// <summary>
    /// Medium forward tilt and spring back on X axis.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.35s | <b>Default ease:</b> OutQuad (lean), OutBack (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("NodMedium").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NodMediumPreset : CodePreset
    {
        public override string PresetName => "NodMedium";
        public override string Description => "Medium forward tilt and spring back";
        public override float DefaultDuration => 0.35f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return NodFactory.Create(target, 11f, GetDuration(duration), options);
        }
    }

    /// <summary>
    /// Deep forward tilt and spring back on X axis.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutQuad (lean), OutBack (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("NodLarge").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NodLargePreset : CodePreset
    {
        public override string PresetName => "NodLarge";
        public override string Description => "Deep forward tilt and spring back";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return NodFactory.Create(target, 25f, GetDuration(duration), options);
        }
    }
}
