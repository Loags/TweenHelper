using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Spins the target a full 360 degrees around the Y axis using FastBeyond360 rotation mode.
    /// <para>
    /// Rotates to <c>(0, 360, 0)</c> with <c>RotateMode.FastBeyond360</c> and <c>Ease.InOutQuad</c>.
    /// The InOutQuad ease creates a smooth acceleration/deceleration spin. FastBeyond360 prevents
    /// DOTween from clamping the rotation to the shortest path.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Coin spin, character turnaround, loading spinner, reveal rotation.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinY").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinPreset : CodePreset
    {
        public override string PresetName => "SpinY";
        public override string Description => "Spins 360 degrees on Y axis";
        public override float DefaultDuration => 1f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutQuad);
            var ease = ResolveEase(presetOptions, Ease.InOutQuad);
            return target.transform.DORotate(new Vector3(0, 360f, 0), GetDuration(duration), RotateMode.FastBeyond360)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Spins the target a full 360 degrees around the X axis using FastBeyond360 rotation mode.
    /// <para>
    /// Rotates to <c>(360, 0, 0)</c> with <c>RotateMode.FastBeyond360</c> and <c>Ease.InOutQuad</c>.
    /// Creates a forward/backward tumble effect. Identical structure to SpinY but on the X axis.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Forward flip, tumble, barrel roll (pitch axis), acrobatic motion.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinX").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinXPreset : CodePreset
    {
        public override string PresetName => "SpinX";
        public override string Description => "Spins 360 degrees on X axis";
        public override float DefaultDuration => 1f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutQuad);
            var ease = ResolveEase(presetOptions, Ease.InOutQuad);
            return target.transform.DORotate(new Vector3(360f, 0, 0), GetDuration(duration), RotateMode.FastBeyond360)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Spins the target a full 360 degrees around the Z axis using FastBeyond360 rotation mode.
    /// <para>
    /// Rotates to <c>(0, 0, 360)</c> with <c>RotateMode.FastBeyond360</c> and <c>Ease.InOutQuad</c>.
    /// Creates a clockface rotation effect. Ideal for 2D objects that rotate in-plane.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> 2D spin, loading spinner, wheel rotation, propeller, compass needle.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinZPreset : CodePreset
    {
        public override string PresetName => "SpinZ";
        public override string Description => "Spins 360 degrees on Z axis";
        public override float DefaultDuration => 1f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutQuad);
            var ease = ResolveEase(presetOptions, Ease.InOutQuad);
            return target.transform.DORotate(new Vector3(0, 0, 360f), GetDuration(duration), RotateMode.FastBeyond360)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Spins the target 360 degrees simultaneously on both X and Y axes, creating a diagonal tumble.
    /// <para>
    /// Rotates to <c>(360, 360, 0)</c> with <c>RotateMode.FastBeyond360</c> and <c>Ease.InOutQuad</c>.
    /// The combined dual-axis rotation produces a complex, visually interesting tumbling motion.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Dramatic object tumble, acrobatic flip, item toss, complex rotation flourish.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinDiagonalXY").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinDiagonalXYPreset : CodePreset
    {
        public override string PresetName => "SpinDiagonalXY";
        public override string Description => "Spins 360 degrees across X and Y axes";
        public override float DefaultDuration => 1f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutQuad);
            var ease = ResolveEase(presetOptions, Ease.InOutQuad);
            return target.transform.DORotate(new Vector3(360f, 360f, 0f), dur, RotateMode.FastBeyond360)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Spins the target 360 degrees simultaneously on both X and Z axes, creating a diagonal tumble.
    /// <para>
    /// Rotates to <c>(360, 0, 360)</c> with <c>RotateMode.FastBeyond360</c> and <c>Ease.InOutQuad</c>.
    /// Combines pitch and roll for an off-axis spinning effect.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Off-axis tumble, item toss, chaotic rotation, dramatic spinning exit.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinDiagonalXZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinDiagonalXZPreset : CodePreset
    {
        public override string PresetName => "SpinDiagonalXZ";
        public override string Description => "Spins 360 degrees across X and Z axes";
        public override float DefaultDuration => 1f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutQuad);
            var ease = ResolveEase(presetOptions, Ease.InOutQuad);
            return target.transform.DORotate(new Vector3(360f, 0f, 360f), dur, RotateMode.FastBeyond360)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Spins the target 360 degrees simultaneously on both Y and Z axes, creating a diagonal tumble.
    /// <para>
    /// Rotates to <c>(0, 360, 360)</c> with <c>RotateMode.FastBeyond360</c> and <c>Ease.InOutQuad</c>.
    /// Combines yaw and roll for a sideways spinning tumble effect.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Sideways tumble, stylized rotation, coin flip variant, dynamic transition.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinDiagonalYZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinDiagonalYZPreset : CodePreset
    {
        public override string PresetName => "SpinDiagonalYZ";
        public override string Description => "Spins 360 degrees across Y and Z axes";
        public override float DefaultDuration => 1f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutQuad);
            var ease = ResolveEase(presetOptions, Ease.InOutQuad);
            return target.transform.DORotate(new Vector3(0f, 360f, 360f), dur, RotateMode.FastBeyond360)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Spins the target 720 degrees on the Y axis while shrinking to zero scale, creating a collectible pickup effect.
    /// <para>
    /// Builds a parallel sequence: scale to <c>Vector3.zero</c> with <c>Ease.InCubic</c> (no anticipation),
    /// joined with 720° Y rotation using <c>RotateMode.FastBeyond360</c> and <c>Ease.Linear</c>
    /// (uniform spin speed). The two full rotations during shrink create a vortex-like exit.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.7s | <b>Default ease:</b> InCubic (scale), Linear (spin)<br/>
    /// <b>Easing override:</b> Primary ease controls scale; secondary ease controls spin.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Collectible pickup, item absorption, vortex exit, magical disappearance.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinScaleOut").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinScaleOutPreset : CodePreset
    {
        public override string PresetName => "SpinScaleOut";
        public override string Description => "Spin and shrink to zero, no anticipation";
        public override float DefaultDuration => 0.7f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration);
            var scaleEase = ResolveEase(options, Ease.InCubic);
            var spinEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);

            return DOTween.Sequence()
                .Join(t.DOScale(Vector3.zero, dur).SetEase(scaleEase))
                .Join(t.DORotate(new Vector3(0f, 720f, 0f), dur, RotateMode.FastBeyond360).SetEase(spinEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Shrinks the target to zero while spinning 720° on Y, with anticipation overshoot on scale.
    /// <para>
    /// Builds a parallel sequence: scale to <c>Vector3.zero</c> with <c>Ease.InBack</c> (anticipation),
    /// joined with 720° Y rotation using <c>RotateMode.FastBeyond360</c> and <c>Ease.Linear</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.7s | <b>Default ease:</b> InBack (scale), Linear (spin)<br/>
    /// <b>Easing override:</b> Primary ease controls scale; secondary ease controls spin.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Collectible pickup, item absorption, vortex exit, magical disappearance.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinScaleOutOvershoot").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinScaleOutOvershootPreset : CodePreset
    {
        public override string PresetName => "SpinScaleOutOvershoot";
        public override string Description => "Spin and shrink to zero with anticipation overshoot";
        public override float DefaultDuration => 0.7f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration);
            var scaleEase = ResolveEase(options, Ease.InBack);
            var spinEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);

            return DOTween.Sequence()
                .Join(t.DOScale(Vector3.zero, dur).SetEase(scaleEase))
                .Join(t.DORotate(new Vector3(0f, 720f, 0f), dur, RotateMode.FastBeyond360).SetEase(spinEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Soft spin and shrink — slower rotation with gentle scale-down.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.9s | <b>Default ease:</b> InSine (scale), Linear (spin)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinScaleOutSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinScaleOutSoftPreset : CodePreset
    {
        public override string PresetName => "SpinScaleOutSoft";
        public override string Description => "Soft spin and shrink to zero";
        public override float DefaultDuration => 0.9f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration);
            var scaleEase = ResolveEase(options, Ease.InSine);
            var spinEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);

            return DOTween.Sequence()
                .Join(t.DOScale(Vector3.zero, dur).SetEase(scaleEase))
                .Join(t.DORotate(new Vector3(0f, 360f, 0f), dur, RotateMode.FastBeyond360).SetEase(spinEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Hard spin and shrink — fast triple rotation with snappy scale-down.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InQuart (scale), Linear (spin)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinScaleOutHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinScaleOutHardPreset : CodePreset
    {
        public override string PresetName => "SpinScaleOutHard";
        public override string Description => "Hard spin and shrink to zero";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration);
            var scaleEase = ResolveEase(options, Ease.InQuart);
            var spinEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);

            return DOTween.Sequence()
                .Join(t.DOScale(Vector3.zero, dur).SetEase(scaleEase))
                .Join(t.DORotate(new Vector3(0f, 1080f, 0f), dur, RotateMode.FastBeyond360).SetEase(spinEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Soft spin and shrink with mild anticipation overshoot on scale.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.9s | <b>Default ease:</b> InBack (1.2 overshoot, scale), Linear (spin)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinScaleOutOvershootSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinScaleOutOvershootSoftPreset : CodePreset
    {
        public override string PresetName => "SpinScaleOutOvershootSoft";
        public override string Description => "Soft spin and shrink with mild anticipation";
        public override float DefaultDuration => 0.9f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration);
            var scaleEase = ResolveEase(options, Ease.InBack);
            var spinEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);

            return DOTween.Sequence()
                .Join(t.DOScale(Vector3.zero, dur).SetEase(scaleEase, 1.2f))
                .Join(t.DORotate(new Vector3(0f, 360f, 0f), dur, RotateMode.FastBeyond360).SetEase(spinEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Hard spin and shrink with strong anticipation overshoot on scale.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InBack (2.0 overshoot, scale), Linear (spin)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinScaleOutOvershootHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinScaleOutOvershootHardPreset : CodePreset
    {
        public override string PresetName => "SpinScaleOutOvershootHard";
        public override string Description => "Hard spin and shrink with strong anticipation";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration);
            var scaleEase = ResolveEase(options, Ease.InBack);
            var spinEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);

            return DOTween.Sequence()
                .Join(t.DOScale(Vector3.zero, dur).SetEase(scaleEase, 2.0f))
                .Join(t.DORotate(new Vector3(0f, 1080f, 0f), dur, RotateMode.FastBeyond360).SetEase(spinEase))
                .WithDefaults(presetOptions, target);
        }
    }
}
