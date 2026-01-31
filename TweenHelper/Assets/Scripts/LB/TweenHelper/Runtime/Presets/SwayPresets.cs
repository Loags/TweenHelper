using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Gentle horizontal sway loop moving the target left and right.
    /// <para>
    /// Creates a callback-chain loop alternating between rightward (+0.35) and leftward (-0.35) relative X movements.
    /// Each leg takes half the total duration; the cycle repeats indefinitely until stopped.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 3.5s | <b>Default ease:</b> InOutSine<br/>
    /// <b>Easing override:</b> Primary ease controls rightward leg; secondary ease controls leftward leg.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Pendulum motion, hanging object sway, idle animation, ambient horizontal drift.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Sway").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SwayPreset : CodePreset
    {
        public override string PresetName => "Sway";
        public override string Description => "Gentle horizontal sway loop";
        public override float DefaultDuration => 3.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var halfDur = GetDuration(duration) * 0.5f;
            var moveRightEase = options.Ease ?? Ease.InOutSine;
            var moveLeftEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutSine;

            var rightOptions = MergeWithDefaultEase(options.SetEase(moveRightEase), moveRightEase);
            var leftOptions = MergeWithDefaultEase(options.SetEase(moveLeftEase), moveLeftEase);
            bool applyDelay = true;

            Tween tween = null;

            void MoveRight()
            {
                tween = t.DOLocalMoveX(0.35f, halfDur)
                    .SetRelative(true)
                    .SetEase(moveRightEase)
                    .WithLoopDefaults(rightOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(MoveLeft);
            }

            void MoveLeft()
            {
                tween = t.DOLocalMoveX(-0.35f, halfDur)
                    .SetRelative(true)
                    .SetEase(moveLeftEase)
                    .WithLoopDefaults(leftOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(MoveRight);
            }

            MoveRight();

            return tween;
        }
    }

    /// <summary>
    /// Internal factory for Sway variants sharing the same callback-chain loop structure.
    /// </summary>
    internal static class SwayFactory
    {
        public static Tween Create(GameObject target, float amplitude, float duration, TweenOptions options)
        {
            var t = target.transform;
            var halfDur = duration * 0.5f;
            var moveRightEase = options.Ease ?? Ease.InOutSine;
            var moveLeftEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutSine;

            var rightOptions = options.Ease.HasValue ? options : options.SetEase(moveRightEase);
            var leftOptions = options.Ease.HasValue ? options : options.SetEase(moveLeftEase);
            bool applyDelay = true;

            Tween tween = null;

            void MoveRight()
            {
                tween = t.DOLocalMoveX(amplitude, halfDur)
                    .SetRelative(true)
                    .SetEase(moveRightEase)
                    .WithLoopDefaults(rightOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(MoveLeft);
            }

            void MoveLeft()
            {
                tween = t.DOLocalMoveX(-amplitude, halfDur)
                    .SetRelative(true)
                    .SetEase(moveLeftEase)
                    .WithLoopDefaults(leftOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(MoveRight);
            }

            MoveRight();

            return tween;
        }
    }

    /// <summary>
    /// Soft horizontal sway loop with small amplitude.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 3.0s | <b>Default ease:</b> InOutSine
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SwaySoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SwaySoftPreset : CodePreset
    {
        public override string PresetName => "SwaySoft";
        public override string Description => "Soft horizontal sway loop";
        public override float DefaultDuration => 3.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SwayFactory.Create(target, 0.25f, GetDuration(duration), options);
        }
    }

    /// <summary>
    /// Wide horizontal sway loop with large amplitude.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 5.0s | <b>Default ease:</b> InOutSine
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SwayHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SwayHardPreset : CodePreset
    {
        public override string PresetName => "SwayHard";
        public override string Description => "Wide horizontal sway loop";
        public override float DefaultDuration => 5.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SwayFactory.Create(target, 0.8f, GetDuration(duration), options);
        }
    }
}
