using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Launches the target 3 units upward from its current local Y position with a decelerating ease.
    /// <para>
    /// Animates local Y position by <c>+3</c> using <c>DOLocalMoveY</c> with <c>Ease.OutCubic</c>.
    /// The deceleration simulates a projectile losing momentum as it rises. Does not return to origin.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect (non-returning) | <b>Default duration:</b> 0.4s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces OutCubic.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Projectile launch, item toss, upward exit, jump start.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("LaunchUp").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class LaunchUpPreset : CodePreset
    {
        public override string PresetName => "LaunchUp";
        public override string Description => "Quick upward motion with ease-out";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);

            return t.DOLocalMoveY(t.localPosition.y + 3f, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Internal factory for Launch directional variants sharing the same non-returning movement structure.
    /// </summary>
    internal static class LaunchFactory
    {
        public static Tween Create(GameObject target, Vector3 direction, float distance, float duration, TweenOptions options)
        {
            var t = target.transform;
            var ease = options.Ease ?? Ease.OutCubic;
            var presetOptions = options.Ease.HasValue ? options : options.SetEase(ease);

            return t.DOLocalMove(t.localPosition + direction * distance, duration)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Launches the target 3 units downward from its current local position with a decelerating ease.
    /// <para>
    /// <b>Type:</b> One-shot effect (non-returning) | <b>Default duration:</b> 0.4s | <b>Default ease:</b> OutCubic
    /// </para>
    /// Usage: <c>transform.Tween().Preset("LaunchDown").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class LaunchDownPreset : CodePreset
    {
        public override string PresetName => "LaunchDown";
        public override string Description => "Quick downward motion with ease-out";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return LaunchFactory.Create(target, Vector3.down, 3f, GetDuration(duration), options);
        }
    }

    /// <summary>
    /// Launches the target 3 units to the left from its current local position with a decelerating ease.
    /// <para>
    /// <b>Type:</b> One-shot effect (non-returning) | <b>Default duration:</b> 0.4s | <b>Default ease:</b> OutCubic
    /// </para>
    /// Usage: <c>transform.Tween().Preset("LaunchLeft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class LaunchLeftPreset : CodePreset
    {
        public override string PresetName => "LaunchLeft";
        public override string Description => "Quick leftward motion with ease-out";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return LaunchFactory.Create(target, Vector3.left, 3f, GetDuration(duration), options);
        }
    }

    /// <summary>
    /// Launches the target 3 units to the right from its current local position with a decelerating ease.
    /// <para>
    /// <b>Type:</b> One-shot effect (non-returning) | <b>Default duration:</b> 0.4s | <b>Default ease:</b> OutCubic
    /// </para>
    /// Usage: <c>transform.Tween().Preset("LaunchRight").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class LaunchRightPreset : CodePreset
    {
        public override string PresetName => "LaunchRight";
        public override string Description => "Quick rightward motion with ease-out";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return LaunchFactory.Create(target, Vector3.right, 3f, GetDuration(duration), options);
        }
    }
}
