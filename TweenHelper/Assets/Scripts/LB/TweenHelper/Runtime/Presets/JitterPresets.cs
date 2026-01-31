using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Produces a tight, rapid vibration effect using DOTween's position shake with very high frequency.
    /// <para>
    /// Uses <c>DOShakePosition</c> with strength <c>0.08</c>, vibrato <c>40</c>, randomness <c>90°</c>,
    /// snapping disabled, and fade-out enabled. The very low strength with extremely high vibrato produces
    /// a subtle, nervous tremor rather than a dramatic shake.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.3s | <b>Default ease:</b> DOTween shake default<br/>
    /// <b>Easing override:</b> No ease override (shake tweens use internal decay).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Nervous tremor, electrical buzz, cold shiver, error micro-shake.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Jitter").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class JitterPreset : CodePreset
    {
        public override string PresetName => "Jitter";
        public override string Description => "Tight rapid vibration";
        public override float DefaultDuration => 0.3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOShakePosition(GetDuration(duration), 0.08f, 40, 90f, false, true)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Subtle rapid vibration with very low amplitude and moderate vibrato.
    /// <para>
    /// Uses <c>DOShakePosition</c> with strength <c>0.04</c>, vibrato <c>30</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.2s
    /// </para>
    /// Usage: <c>transform.Tween().Preset("JitterSmall").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class JitterSmallPreset : CodePreset
    {
        public override string PresetName => "JitterSmall";
        public override string Description => "Subtle rapid vibration";
        public override float DefaultDuration => 0.2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOShakePosition(GetDuration(duration), 0.04f, 30, 90f, false, true)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Medium rapid vibration with moderate amplitude and vibrato.
    /// <para>
    /// Uses <c>DOShakePosition</c> with strength <c>0.06</c>, vibrato <c>35</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.25s
    /// </para>
    /// Usage: <c>transform.Tween().Preset("JitterMedium").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class JitterMediumPreset : CodePreset
    {
        public override string PresetName => "JitterMedium";
        public override string Description => "Medium rapid vibration";
        public override float DefaultDuration => 0.25f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOShakePosition(GetDuration(duration), 0.06f, 35, 90f, false, true)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Intense rapid vibration with high amplitude and extreme vibrato.
    /// <para>
    /// Uses <c>DOShakePosition</c> with strength <c>0.15</c>, vibrato <c>50</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.4s
    /// </para>
    /// Usage: <c>transform.Tween().Preset("JitterLarge").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class JitterLargePreset : CodePreset
    {
        public override string PresetName => "JitterLarge";
        public override string Description => "Intense rapid vibration";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOShakePosition(GetDuration(duration), 0.15f, 50, 90f, false, true)
                .WithDefaults(options, target);
        }
    }
}
