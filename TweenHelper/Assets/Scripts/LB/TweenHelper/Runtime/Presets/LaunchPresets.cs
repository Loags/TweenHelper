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

            return t.DOLocalMoveY(t.localPosition.y + 3f, GetDuration(duration, options))
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
            return LaunchFactory.Create(target, Vector3.down, 3f, GetDuration(duration, options), options);
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
            return LaunchFactory.Create(target, Vector3.left, 3f, GetDuration(duration, options), options);
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
            return LaunchFactory.Create(target, Vector3.right, 3f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Gentle upward launch — shorter distance (1.5 units) for a subtle lift.
    /// <para>
    /// <b>Type:</b> One-shot effect (non-returning) | <b>Default duration:</b> 0.3s | <b>Default ease:</b> OutCubic
    /// </para>
    /// Usage: <c>transform.Tween().Preset("LaunchUpSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class LaunchUpSoftPreset : CodePreset
    {
        public override string PresetName => "LaunchUpSoft";
        public override string Description => "Gentle upward motion with ease-out";
        public override float DefaultDuration => 0.3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return LaunchFactory.Create(target, Vector3.up, 1.5f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Forceful upward launch — longer distance (5 units) for a dramatic lift.
    /// <para>
    /// <b>Type:</b> One-shot effect (non-returning) | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutCubic
    /// </para>
    /// Usage: <c>transform.Tween().Preset("LaunchUpHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class LaunchUpHardPreset : CodePreset
    {
        public override string PresetName => "LaunchUpHard";
        public override string Description => "Forceful upward motion with ease-out";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return LaunchFactory.Create(target, Vector3.up, 5f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Gentle downward launch — shorter distance (1.5 units) for a subtle drop.
    /// <para>
    /// <b>Type:</b> One-shot effect (non-returning) | <b>Default duration:</b> 0.3s | <b>Default ease:</b> OutCubic
    /// </para>
    /// Usage: <c>transform.Tween().Preset("LaunchDownSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class LaunchDownSoftPreset : CodePreset
    {
        public override string PresetName => "LaunchDownSoft";
        public override string Description => "Gentle downward motion with ease-out";
        public override float DefaultDuration => 0.3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return LaunchFactory.Create(target, Vector3.down, 1.5f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Forceful downward launch — longer distance (5 units) for a dramatic drop.
    /// <para>
    /// <b>Type:</b> One-shot effect (non-returning) | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutCubic
    /// </para>
    /// Usage: <c>transform.Tween().Preset("LaunchDownHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class LaunchDownHardPreset : CodePreset
    {
        public override string PresetName => "LaunchDownHard";
        public override string Description => "Forceful downward motion with ease-out";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return LaunchFactory.Create(target, Vector3.down, 5f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Gentle leftward launch — shorter distance (1.5 units) for a subtle slide.
    /// <para>
    /// <b>Type:</b> One-shot effect (non-returning) | <b>Default duration:</b> 0.3s | <b>Default ease:</b> OutCubic
    /// </para>
    /// Usage: <c>transform.Tween().Preset("LaunchLeftSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class LaunchLeftSoftPreset : CodePreset
    {
        public override string PresetName => "LaunchLeftSoft";
        public override string Description => "Gentle leftward motion with ease-out";
        public override float DefaultDuration => 0.3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return LaunchFactory.Create(target, Vector3.left, 1.5f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Forceful leftward launch — longer distance (5 units) for a dramatic exit.
    /// <para>
    /// <b>Type:</b> One-shot effect (non-returning) | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutCubic
    /// </para>
    /// Usage: <c>transform.Tween().Preset("LaunchLeftHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class LaunchLeftHardPreset : CodePreset
    {
        public override string PresetName => "LaunchLeftHard";
        public override string Description => "Forceful leftward motion with ease-out";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return LaunchFactory.Create(target, Vector3.left, 5f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Gentle rightward launch — shorter distance (1.5 units) for a subtle slide.
    /// <para>
    /// <b>Type:</b> One-shot effect (non-returning) | <b>Default duration:</b> 0.3s | <b>Default ease:</b> OutCubic
    /// </para>
    /// Usage: <c>transform.Tween().Preset("LaunchRightSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class LaunchRightSoftPreset : CodePreset
    {
        public override string PresetName => "LaunchRightSoft";
        public override string Description => "Gentle rightward motion with ease-out";
        public override float DefaultDuration => 0.3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return LaunchFactory.Create(target, Vector3.right, 1.5f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Forceful rightward launch — longer distance (5 units) for a dramatic exit.
    /// <para>
    /// <b>Type:</b> One-shot effect (non-returning) | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutCubic
    /// </para>
    /// Usage: <c>transform.Tween().Preset("LaunchRightHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class LaunchRightHardPreset : CodePreset
    {
        public override string PresetName => "LaunchRightHard";
        public override string Description => "Forceful rightward motion with ease-out";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return LaunchFactory.Create(target, Vector3.right, 5f, GetDuration(duration, options), options);
        }
    }
}
