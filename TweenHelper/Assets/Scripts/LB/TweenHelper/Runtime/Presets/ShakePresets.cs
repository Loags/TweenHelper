using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Shakes the target's position randomly, creating an impact or earthquake effect.
    /// <para>
    /// Uses <c>DOShakePosition</c> with strength <c>0.22</c>, vibrato <c>12</c>, randomness <c>90°</c>,
    /// snapping disabled, and fade-out enabled.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.45s | <b>Default ease:</b> DOTween shake default<br/>
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
        public override float DefaultDuration => 0.45f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOShakePosition(GetDuration(duration, options), 0.22f, 12, 90f, false, true)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Soft position shake with low strength and fewer oscillations.
    /// <para>
    /// Uses <c>DOShakePosition</c> with strength <c>0.15</c>, vibrato <c>10</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.4s
    /// </para>
    /// Usage: <c>transform.Tween().Preset("ShakeSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ShakeSoftPreset : CodePreset
    {
        public override string PresetName => "ShakeSoft";
        public override string Description => "Soft position shake";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOShakePosition(GetDuration(duration, options), 0.15f, 10, 90f, false, true)
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
    /// Usage: <c>transform.Tween().Preset("ShakeHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ShakeHardPreset : CodePreset
    {
        public override string PresetName => "ShakeHard";
        public override string Description => "Heavy position shake";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOShakePosition(GetDuration(duration, options), 0.5f, 20, 90f, false, true)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Shakes the target's position while simultaneously fading out to transparent, creating a damage/death effect.
    /// <para>
    /// Builds a parallel sequence: <c>DOShakePosition</c> with strength <c>0.3</c>, vibrato <c>15</c>,
    /// randomness <c>90°</c>, and fade-out enabled; joined with a fade to <c>0</c> using <c>Ease.InQuad</c>.
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
            var dur = GetDuration(duration, options);
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

    /// <summary>
    /// Soft shake with fade out — lighter strength and fewer vibrations.
    /// <para>
    /// Uses <c>DOShakePosition</c> with strength <c>0.15</c>, vibrato <c>10</c>, joined with fade to <c>0</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.6s | <b>Default ease:</b> Linear (sequence), InQuad (fade)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("ShakeFadeSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ShakeFadeSoftPreset : CodePreset
    {
        public override string PresetName => "ShakeFadeSoft";
        public override string Description => "Soft shake with fade out";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.Linear);

            var seq = DOTween.Sequence();
            seq.Append(t.DOShakePosition(dur, 0.15f, 10, 90f, false, true));

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

    /// <summary>
    /// Hard shake with fade out — stronger shaking and more vibrations.
    /// <para>
    /// Uses <c>DOShakePosition</c> with strength <c>0.5</c>, vibrato <c>20</c>, joined with fade to <c>0</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 1.0s | <b>Default ease:</b> Linear (sequence), InQuad (fade)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("ShakeFadeHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ShakeFadeHardPreset : CodePreset
    {
        public override string PresetName => "ShakeFadeHard";
        public override string Description => "Hard shake with fade out";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.Linear);

            var seq = DOTween.Sequence();
            seq.Append(t.DOShakePosition(dur, 0.5f, 20, 90f, false, true));

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
