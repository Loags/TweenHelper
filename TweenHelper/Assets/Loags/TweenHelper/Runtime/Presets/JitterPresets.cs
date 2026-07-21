using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Produces a tight, rapid vibration effect using DOTween's position shake with very high frequency.
    /// <para>
    /// Uses <c>DOShakePosition</c> with strength <c>0.06</c>, vibrato <c>35</c>, randomness <c>90°</c>,
    /// snapping disabled, and fade-out enabled.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.25s | <b>Default ease:</b> DOTween shake default<br/>
    /// <b>Easing override:</b> No ease override (shake tweens use internal decay).<br/>
    /// <b>Strength override:</b> Multiplies jitter range (default 1.0).
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
        public override float DefaultDuration => 0.25f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOShakePosition(GetDuration(duration, options), 0.06f * strength, 35, 90f, false, true)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Soft rapid vibration with very low amplitude and moderate vibrato.
    /// <para>
    /// Uses <c>DOShakePosition</c> with strength <c>0.04</c>, vibrato <c>30</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.2s<br/>
    /// <b>Strength override:</b> Multiplies jitter range (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("JitterSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class JitterSoftPreset : CodePreset
    {
        public override string PresetName => "JitterSoft";
        public override string Description => "Soft rapid vibration";
        public override float DefaultDuration => 0.2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOShakePosition(GetDuration(duration, options), 0.04f * strength, 30, 90f, false, true)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Intense rapid vibration with high amplitude and extreme vibrato.
    /// <para>
    /// Uses <c>DOShakePosition</c> with strength <c>0.15</c>, vibrato <c>50</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.4s<br/>
    /// <b>Strength override:</b> Multiplies jitter range (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("JitterHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class JitterHardPreset : CodePreset
    {
        public override string PresetName => "JitterHard";
        public override string Description => "Intense rapid vibration";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOShakePosition(GetDuration(duration, options), 0.15f * strength, 50, 90f, false, true)
                .WithDefaults(options, target);
        }
    }
}
