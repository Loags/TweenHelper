using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Gentle Z-axis pendulum loop. Swings continuously through its center without snapping.
    /// <para>
    /// <b>Type:</b> Looping (sequence) | <b>Default duration:</b> 2.8s | <b>Default ease:</b> Sine-based continuous swing<br/>
    /// <b>Easing override:</b> Primary ease controls center transitions; secondary ease controls the full sweep.<br/>
    /// <b>Strength override:</b> Multiplies swing angle (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Pendulum swing, cradle rock, hanging sign, idle sway, metronome.
    /// </para>
    /// Usage: <c>transform.Tween().Preset<PendulumZPreset>().Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PendulumZPreset : CodePreset
    {
        public override string PresetName => "PendulumZ";
        public override string Description => "Gentle Z-axis pendulum loop";
        public override float DefaultDuration => 2.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PendulumFactory.Create(target, Vector3.forward, 6f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Internal factory for Pendulum variants sharing the same sequence loop structure.
    /// </summary>
    internal static class PendulumFactory
    {
        public static Tween Create(GameObject target, Vector3 axis, float angle, float duration, TweenOptions options)
        {
            axis = axis == Vector3.zero ? Vector3.forward : axis.normalized;
            var strength = CodePreset.ResolveStrengthStatic(options);
            var t = target.transform;
            var originalRot = t.localEulerAngles;
            var quarterDuration = duration * 0.25f;
            var halfDuration = duration * 0.5f;
            var centerOutEase = options.Ease ?? Ease.OutSine;
            var fullSweepEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutSine;
            var centerInEase = options.Ease ?? Ease.InSine;

            var delta = axis * (angle * strength);

            return DOTween.Sequence()
                .Append(t.DOLocalRotate(originalRot + delta, quarterDuration).SetEase(centerOutEase))
                .Append(t.DOLocalRotate(originalRot - delta, halfDuration).SetEase(fullSweepEase))
                .Append(t.DOLocalRotate(originalRot, quarterDuration).SetEase(centerInEase))
                .SetLoops(-1, LoopType.Restart)
                .WithLoopDefaults(options, target, applyDelayThisCycle: true)
                .SetEase(Ease.Linear);
        }

        public static Tween Create(GameObject target, float angle, float duration, TweenOptions options)
        {
            return Create(target, Vector3.forward, angle, duration, options);
        }
    }

    /// <summary>
    /// Soft Z-axis pendulum loop with small angle.
    /// <para>
    /// <b>Type:</b> Looping (sequence) | <b>Default duration:</b> 2.5s | <b>Default ease:</b> InOutSine<br/>
    /// <b>Strength override:</b> Multiplies swing angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset<PendulumZSoftPreset>().Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PendulumZSoftPreset : CodePreset
    {
        public override string PresetName => "PendulumZSoft";
        public override string Description => "Soft Z-axis pendulum loop";
        public override float DefaultDuration => 2.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PendulumFactory.Create(target, 4f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Wide Z-axis pendulum loop with large angle.
    /// <para>
    /// <b>Type:</b> Looping (sequence) | <b>Default duration:</b> 3.5s | <b>Default ease:</b> InOutSine<br/>
    /// <b>Strength override:</b> Multiplies swing angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset<PendulumZHardPreset>().Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PendulumZHardPreset : CodePreset
    {
        public override string PresetName => "PendulumZHard";
        public override string Description => "Wide Z-axis pendulum loop";
        public override float DefaultDuration => 3.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PendulumFactory.Create(target, 14f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Gentle X-axis pendulum loop. Nods continuously through its center without snapping.
    /// <para>
    /// <b>Type:</b> Looping (sequence) | <b>Default duration:</b> 2.8s | <b>Default ease:</b> Sine-based continuous swing<br/>
    /// <b>Easing override:</b> Primary ease controls center transitions; secondary ease controls the full sweep.<br/>
    /// <b>Strength override:</b> Multiplies swing angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset<PendulumXPreset>().Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PendulumXPreset : CodePreset
    {
        public override string PresetName => "PendulumX";
        public override string Description => "Gentle X-axis pendulum loop";
        public override float DefaultDuration => 2.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PendulumFactory.Create(target, Vector3.right, 6f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft X-axis pendulum loop with small angle.
    /// <para>
    /// <b>Type:</b> Looping (sequence) | <b>Default duration:</b> 2.5s | <b>Default ease:</b> InOutSine<br/>
    /// <b>Strength override:</b> Multiplies swing angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset<PendulumXSoftPreset>().Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PendulumXSoftPreset : CodePreset
    {
        public override string PresetName => "PendulumXSoft";
        public override string Description => "Soft X-axis pendulum loop";
        public override float DefaultDuration => 2.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PendulumFactory.Create(target, Vector3.right, 4f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Wide X-axis pendulum loop with large angle.
    /// <para>
    /// <b>Type:</b> Looping (sequence) | <b>Default duration:</b> 3.5s | <b>Default ease:</b> InOutSine<br/>
    /// <b>Strength override:</b> Multiplies swing angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset<PendulumXHardPreset>().Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PendulumXHardPreset : CodePreset
    {
        public override string PresetName => "PendulumXHard";
        public override string Description => "Wide X-axis pendulum loop";
        public override float DefaultDuration => 3.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PendulumFactory.Create(target, Vector3.right, 14f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Gentle Y-axis pendulum loop. Tilts continuously through its center without snapping.
    /// <para>
    /// <b>Type:</b> Looping (sequence) | <b>Default duration:</b> 2.8s | <b>Default ease:</b> Sine-based continuous swing<br/>
    /// <b>Easing override:</b> Primary ease controls center transitions; secondary ease controls the full sweep.<br/>
    /// <b>Strength override:</b> Multiplies swing angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset<PendulumYPreset>().Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PendulumYPreset : CodePreset
    {
        public override string PresetName => "PendulumY";
        public override string Description => "Gentle Y-axis pendulum loop";
        public override float DefaultDuration => 2.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PendulumFactory.Create(target, Vector3.up, 6f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft Y-axis pendulum loop with small angle.
    /// <para>
    /// <b>Type:</b> Looping (sequence) | <b>Default duration:</b> 2.5s | <b>Default ease:</b> InOutSine<br/>
    /// <b>Strength override:</b> Multiplies swing angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset<PendulumYSoftPreset>().Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PendulumYSoftPreset : CodePreset
    {
        public override string PresetName => "PendulumYSoft";
        public override string Description => "Soft Y-axis pendulum loop";
        public override float DefaultDuration => 2.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PendulumFactory.Create(target, Vector3.up, 4f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Wide Y-axis pendulum loop with large angle.
    /// <para>
    /// <b>Type:</b> Looping (sequence) | <b>Default duration:</b> 3.5s | <b>Default ease:</b> InOutSine<br/>
    /// <b>Strength override:</b> Multiplies swing angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset<PendulumYHardPreset>().Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PendulumYHardPreset : CodePreset
    {
        public override string PresetName => "PendulumYHard";
        public override string Description => "Wide Y-axis pendulum loop";
        public override float DefaultDuration => 3.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return PendulumFactory.Create(target, Vector3.up, 14f, GetDuration(duration, options), options);
        }
    }
}
