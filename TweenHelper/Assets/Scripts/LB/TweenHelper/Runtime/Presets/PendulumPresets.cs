using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Gentle Z-axis pendulum loop. Swings left then right continuously.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.8s (1.4s per leg) | <b>Default ease:</b> InOutSine<br/>
    /// <b>Easing override:</b> Primary ease controls left-swing; secondary ease controls right-swing.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Pendulum swing, cradle rock, hanging sign, idle sway, metronome.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PendulumZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PendulumZPreset : CodePreset
    {
        public override string PresetName => "PendulumZ";
        public override string Description => "Gentle Z-axis pendulum loop";
        public override float DefaultDuration => 2.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalRot = t.localEulerAngles;
            var halfDur = GetDuration(duration, options) * 0.5f;
            var leftEase = options.Ease ?? Ease.InOutSine;
            var rightEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutSine;

            var leftOptions = MergeWithDefaultEase(options.SetEase(leftEase), leftEase);
            var rightOptions = MergeWithDefaultEase(options.SetEase(rightEase), rightEase);
            bool applyDelay = true;

            Tween tween = null;

            void SwingLeft()
            {
                tween = t.DOLocalRotate(originalRot + new Vector3(0f, 0f, 6f), halfDur)
                    .SetEase(leftEase)
                    .WithLoopDefaults(leftOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(SwingRight);
            }

            void SwingRight()
            {
                tween = t.DOLocalRotate(originalRot + new Vector3(0f, 0f, -6f), halfDur)
                    .SetEase(rightEase)
                    .WithLoopDefaults(rightOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(SwingLeft);
            }

            SwingLeft();

            return tween;
        }
    }

    /// <summary>
    /// Internal factory for Pendulum variants sharing the same callback-chain loop structure.
    /// </summary>
    internal static class PendulumFactory
    {
        public static Tween Create(GameObject target, float angle, float duration, TweenOptions options)
        {
            var t = target.transform;
            var originalRot = t.localEulerAngles;
            var halfDur = duration * 0.5f;
            var leftEase = options.Ease ?? Ease.InOutSine;
            var rightEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutSine;

            var leftOptions = options.Ease.HasValue ? options : options.SetEase(leftEase);
            var rightOptions = options.Ease.HasValue ? options : options.SetEase(rightEase);
            bool applyDelay = true;

            Tween tween = null;

            void SwingLeft()
            {
                tween = t.DOLocalRotate(originalRot + new Vector3(0f, 0f, angle), halfDur)
                    .SetEase(leftEase)
                    .WithLoopDefaults(leftOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(SwingRight);
            }

            void SwingRight()
            {
                tween = t.DOLocalRotate(originalRot + new Vector3(0f, 0f, -angle), halfDur)
                    .SetEase(rightEase)
                    .WithLoopDefaults(rightOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(SwingLeft);
            }

            SwingLeft();

            return tween;
        }
    }

    /// <summary>
    /// Soft Z-axis pendulum loop with small angle.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.5s | <b>Default ease:</b> InOutSine
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PendulumZSoft").Play();</c>
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
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 3.5s | <b>Default ease:</b> InOutSine
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PendulumZHard").Play();</c>
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
}
