using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Gentle Z-axis pendulum loop. Rocks left then right continuously.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 3.0s (1.5s per leg) | <b>Default ease:</b> InOutSine<br/>
    /// <b>Easing override:</b> Primary ease controls left-rock; secondary ease controls right-rock.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Pendulum swing, cradle rock, hanging sign, idle sway, metronome.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Rock").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class RockPreset : CodePreset
    {
        public override string PresetName => "Rock";
        public override string Description => "Gentle Z-axis pendulum loop";
        public override float DefaultDuration => 3.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalRot = t.localEulerAngles;
            var halfDur = GetDuration(duration) * 0.5f;
            var leftEase = options.Ease ?? Ease.InOutSine;
            var rightEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutSine;

            var leftOptions = MergeWithDefaultEase(options.SetEase(leftEase), leftEase);
            var rightOptions = MergeWithDefaultEase(options.SetEase(rightEase), rightEase);
            bool applyDelay = true;

            Tween tween = null;

            void RockLeft()
            {
                tween = t.DOLocalRotate(originalRot + new Vector3(0f, 0f, 8f), halfDur)
                    .SetEase(leftEase)
                    .WithLoopDefaults(leftOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(RockRight);
            }

            void RockRight()
            {
                tween = t.DOLocalRotate(originalRot + new Vector3(0f, 0f, -8f), halfDur)
                    .SetEase(rightEase)
                    .WithLoopDefaults(rightOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(RockLeft);
            }

            RockLeft();

            return tween;
        }
    }

    /// <summary>
    /// Internal factory for Rock variants sharing the same callback-chain loop structure.
    /// </summary>
    internal static class RockFactory
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

            void RockLeft()
            {
                tween = t.DOLocalRotate(originalRot + new Vector3(0f, 0f, angle), halfDur)
                    .SetEase(leftEase)
                    .WithLoopDefaults(leftOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(RockRight);
            }

            void RockRight()
            {
                tween = t.DOLocalRotate(originalRot + new Vector3(0f, 0f, -angle), halfDur)
                    .SetEase(rightEase)
                    .WithLoopDefaults(rightOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(RockLeft);
            }

            RockLeft();

            return tween;
        }
    }

    /// <summary>
    /// Subtle Z-axis pendulum loop with small angle.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.5s | <b>Default ease:</b> InOutSine
    /// </para>
    /// Usage: <c>transform.Tween().Preset("RockSmall").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class RockSmallPreset : CodePreset
    {
        public override string PresetName => "RockSmall";
        public override string Description => "Subtle Z-axis pendulum loop";
        public override float DefaultDuration => 2.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return RockFactory.Create(target, 4f, GetDuration(duration), options);
        }
    }

    /// <summary>
    /// Medium Z-axis pendulum loop with moderate angle.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.8s | <b>Default ease:</b> InOutSine
    /// </para>
    /// Usage: <c>transform.Tween().Preset("RockMedium").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class RockMediumPreset : CodePreset
    {
        public override string PresetName => "RockMedium";
        public override string Description => "Medium Z-axis pendulum loop";
        public override float DefaultDuration => 2.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return RockFactory.Create(target, 6f, GetDuration(duration), options);
        }
    }

    /// <summary>
    /// Wide Z-axis pendulum loop with large angle.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 3.5s | <b>Default ease:</b> InOutSine
    /// </para>
    /// Usage: <c>transform.Tween().Preset("RockLarge").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class RockLargePreset : CodePreset
    {
        public override string PresetName => "RockLarge";
        public override string Description => "Wide Z-axis pendulum loop";
        public override float DefaultDuration => 3.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return RockFactory.Create(target, 14f, GetDuration(duration), options);
        }
    }
}
