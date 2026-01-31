using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Shakes the target's position randomly, creating an impact or earthquake effect.
    /// <para>
    /// Uses <c>DOShakePosition</c> with strength <c>0.3</c>, vibrato <c>15</c>, randomness <c>90°</c>,
    /// snapping disabled, and fade-out enabled. The high vibrato produces many oscillations within
    /// the duration, creating a convincing tremor effect.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> DOTween shake default<br/>
    /// <b>Easing override:</b> No ease override (shake tweens use internal decay).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Impact feedback, earthquake, error shake, camera shake, damage indication.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Shake").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ShakePreset : CodePreset
    {
        public override string PresetName => "Shake";
        public override string Description => "Random position shake";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOShakePosition(GetDuration(duration), 0.3f, 15, 90f, false, true)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Light position shake with low strength and fewer oscillations.
    /// <para>
    /// Uses <c>DOShakePosition</c> with strength <c>0.15</c>, vibrato <c>10</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.4s
    /// </para>
    /// Usage: <c>transform.Tween().Preset("ShakeSmall").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ShakeSmallPreset : CodePreset
    {
        public override string PresetName => "ShakeSmall";
        public override string Description => "Light position shake";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOShakePosition(GetDuration(duration), 0.15f, 10, 90f, false, true)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Medium position shake with moderate strength and vibrato.
    /// <para>
    /// Uses <c>DOShakePosition</c> with strength <c>0.22</c>, vibrato <c>12</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.45s
    /// </para>
    /// Usage: <c>transform.Tween().Preset("ShakeMedium").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ShakeMediumPreset : CodePreset
    {
        public override string PresetName => "ShakeMedium";
        public override string Description => "Medium position shake";
        public override float DefaultDuration => 0.45f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOShakePosition(GetDuration(duration), 0.22f, 12, 90f, false, true)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Heavy position shake with high strength and many oscillations.
    /// <para>
    /// Uses <c>DOShakePosition</c> with strength <c>0.5</c>, vibrato <c>20</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.6s
    /// </para>
    /// Usage: <c>transform.Tween().Preset("ShakeLarge").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ShakeLargePreset : CodePreset
    {
        public override string PresetName => "ShakeLarge";
        public override string Description => "Heavy position shake";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOShakePosition(GetDuration(duration), 0.5f, 20, 90f, false, true)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Shakes the target's position while simultaneously fading out to transparent, creating a damage/death effect.
    /// <para>
    /// Builds a parallel sequence: <c>DOShakePosition</c> with strength <c>0.3</c>, vibrato <c>15</c>,
    /// randomness <c>90°</c>, and fade-out enabled; joined with a fade to <c>0</c> using <c>Ease.InQuad</c>
    /// (accelerating fade so the object lingers visible during the strongest shaking).
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.8s | <b>Default ease:</b> Linear (sequence), InQuad (fade)<br/>
    /// <b>Easing override:</b> Standard options apply; shake and fade eases are hardcoded.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Enemy death, damage feedback, destruction effect, error dismiss with shake.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("ShakeFade").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ShakeFadePreset : CodePreset
    {
        public override string PresetName => "ShakeFade";
        public override string Description => "Shake position with fade out";
        public override float DefaultDuration => 0.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.Linear);

            var seq = DOTween.Sequence();
            seq.Append(t.DOShakePosition(dur, 0.3f, 15, 90f, false, true));

            var fadeTween = CreateFadeTween(target, 0f, dur);
            if (fadeTween != null)
            {
                fadeTween.SetEase(Ease.InQuad);
                seq.Join(fadeTween);
            }

            return seq.WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }
}
