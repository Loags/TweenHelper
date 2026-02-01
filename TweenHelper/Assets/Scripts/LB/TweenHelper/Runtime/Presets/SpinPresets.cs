using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Internal factory for single-axis spin presets.
    /// </summary>
    internal static class SpinFactory
    {
        public static Tween Create(GameObject target, Vector3 rotationAxis, float degrees, float duration, TweenOptions options)
        {
            var strength = CodePreset.ResolveStrengthStatic(options);
            var presetOptions = options.Ease.HasValue ? options : options.SetEase(Ease.InOutQuad);
            var ease = presetOptions.Ease ?? Ease.InOutQuad;
            return target.transform.DORotate(rotationAxis * (degrees * strength), duration, RotateMode.FastBeyond360)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Spins the target a full 360 degrees around the Y axis using FastBeyond360 rotation mode.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
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
            return SpinFactory.Create(target, Vector3.up, 360f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft Y-axis spin. Slower, gentler rotation.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinYSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinYSoftPreset : CodePreset
    {
        public override string PresetName => "SpinYSoft";
        public override string Description => "Slow spin on Y axis";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SpinFactory.Create(target, Vector3.up, 360f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Hard Y-axis spin. Fast, snappy rotation.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinYHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinYHardPreset : CodePreset
    {
        public override string PresetName => "SpinYHard";
        public override string Description => "Fast spin on Y axis";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SpinFactory.Create(target, Vector3.up, 360f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Spins the target a full 360 degrees around the X axis using FastBeyond360 rotation mode.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
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
            return SpinFactory.Create(target, Vector3.right, 360f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft X-axis spin. Slower, gentler rotation.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinXSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinXSoftPreset : CodePreset
    {
        public override string PresetName => "SpinXSoft";
        public override string Description => "Slow spin on X axis";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SpinFactory.Create(target, Vector3.right, 360f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Hard X-axis spin. Fast, snappy rotation.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinXHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinXHardPreset : CodePreset
    {
        public override string PresetName => "SpinXHard";
        public override string Description => "Fast spin on X axis";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SpinFactory.Create(target, Vector3.right, 360f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Spins the target a full 360 degrees around the Z axis using FastBeyond360 rotation mode.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
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
            return SpinFactory.Create(target, Vector3.forward, 360f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft Z-axis spin. Slower, gentler rotation.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinZSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinZSoftPreset : CodePreset
    {
        public override string PresetName => "SpinZSoft";
        public override string Description => "Slow spin on Z axis";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SpinFactory.Create(target, Vector3.forward, 360f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Hard Z-axis spin. Fast, snappy rotation.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinZHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinZHardPreset : CodePreset
    {
        public override string PresetName => "SpinZHard";
        public override string Description => "Fast spin on Z axis";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SpinFactory.Create(target, Vector3.forward, 360f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Internal factory for diagonal spin presets.
    /// </summary>
    internal static class SpinDiagonalFactory
    {
        public static Tween Create(GameObject target, Vector3 rotation, float duration, TweenOptions options)
        {
            var strength = CodePreset.ResolveStrengthStatic(options);
            var presetOptions = options.Ease.HasValue ? options : options.SetEase(Ease.InOutQuad);
            var ease = presetOptions.Ease ?? Ease.InOutQuad;
            return target.transform.DORotate(rotation * strength, duration, RotateMode.FastBeyond360)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Spins the target 360 degrees simultaneously on both X and Y axes, creating a diagonal tumble.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
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
            return SpinDiagonalFactory.Create(target, new Vector3(360f, 360f, 0f), GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft diagonal XY spin. Slower rotation.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinDiagonalXYSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinDiagonalXYSoftPreset : CodePreset
    {
        public override string PresetName => "SpinDiagonalXYSoft";
        public override string Description => "Slow diagonal spin across X and Y";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SpinDiagonalFactory.Create(target, new Vector3(360f, 360f, 0f), GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Hard diagonal XY spin. Fast rotation.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinDiagonalXYHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinDiagonalXYHardPreset : CodePreset
    {
        public override string PresetName => "SpinDiagonalXYHard";
        public override string Description => "Fast diagonal spin across X and Y";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SpinDiagonalFactory.Create(target, new Vector3(360f, 360f, 0f), GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Spins the target 360 degrees simultaneously on both X and Z axes, creating a diagonal tumble.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
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
            return SpinDiagonalFactory.Create(target, new Vector3(360f, 0f, 360f), GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft diagonal XZ spin. Slower rotation.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinDiagonalXZSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinDiagonalXZSoftPreset : CodePreset
    {
        public override string PresetName => "SpinDiagonalXZSoft";
        public override string Description => "Slow diagonal spin across X and Z";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SpinDiagonalFactory.Create(target, new Vector3(360f, 0f, 360f), GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Hard diagonal XZ spin. Fast rotation.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinDiagonalXZHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinDiagonalXZHardPreset : CodePreset
    {
        public override string PresetName => "SpinDiagonalXZHard";
        public override string Description => "Fast diagonal spin across X and Z";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SpinDiagonalFactory.Create(target, new Vector3(360f, 0f, 360f), GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Spins the target 360 degrees simultaneously on both Y and Z axes, creating a diagonal tumble.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
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
            return SpinDiagonalFactory.Create(target, new Vector3(0f, 360f, 360f), GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft diagonal YZ spin. Slower rotation.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinDiagonalYZSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinDiagonalYZSoftPreset : CodePreset
    {
        public override string PresetName => "SpinDiagonalYZSoft";
        public override string Description => "Slow diagonal spin across Y and Z";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SpinDiagonalFactory.Create(target, new Vector3(0f, 360f, 360f), GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Hard diagonal YZ spin. Fast rotation.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinDiagonalYZHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinDiagonalYZHardPreset : CodePreset
    {
        public override string PresetName => "SpinDiagonalYZHard";
        public override string Description => "Fast diagonal spin across Y and Z";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SpinDiagonalFactory.Create(target, new Vector3(0f, 360f, 360f), GetDuration(duration, options), options);
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
    /// <b>Easing override:</b> Primary ease controls scale; secondary ease controls spin.<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
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
            var strength = ResolveStrength(options);
            var t = target.transform;
            var dur = GetDuration(duration, options);
            var scaleEase = ResolveEase(options, Ease.InCubic);
            var spinEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);

            var endScale = ResolveTargetScale(options, Vector3.zero);
            return DOTween.Sequence()
                .Join(t.DOScale(endScale, dur).SetEase(scaleEase))
                .Join(t.DORotate(new Vector3(0f, 720f * strength, 0f), dur, RotateMode.FastBeyond360).SetEase(spinEase))
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
    /// <b>Easing override:</b> Primary ease controls scale; secondary ease controls spin.<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
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
        public override float DefaultOvershoot => 1.70158f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            var t = target.transform;
            var dur = GetDuration(duration, options);
            var scaleEase = ResolveEase(options, Ease.InBack);
            var spinEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);
            var os = DefaultOvershoot * ResolveOvershootMultiplier(presetOptions);

            var endScale = ResolveTargetScale(options, Vector3.zero);
            return DOTween.Sequence()
                .Join(t.DOScale(endScale, dur).SetEase(scaleEase, os))
                .Join(t.DORotate(new Vector3(0f, 720f * strength, 0f), dur, RotateMode.FastBeyond360).SetEase(spinEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Soft spin and shrink — slower rotation with gentle scale-down.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.9s | <b>Default ease:</b> InSine (scale), Linear (spin)<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
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
            var strength = ResolveStrength(options);
            var t = target.transform;
            var dur = GetDuration(duration, options);
            var scaleEase = ResolveEase(options, Ease.InSine);
            var spinEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);

            var endScale = ResolveTargetScale(options, Vector3.zero);
            return DOTween.Sequence()
                .Join(t.DOScale(endScale, dur).SetEase(scaleEase))
                .Join(t.DORotate(new Vector3(0f, 360f * strength, 0f), dur, RotateMode.FastBeyond360).SetEase(spinEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Hard spin and shrink — fast triple rotation with snappy scale-down.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InQuart (scale), Linear (spin)<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
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
            var strength = ResolveStrength(options);
            var t = target.transform;
            var dur = GetDuration(duration, options);
            var scaleEase = ResolveEase(options, Ease.InQuart);
            var spinEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);

            var endScale = ResolveTargetScale(options, Vector3.zero);
            return DOTween.Sequence()
                .Join(t.DOScale(endScale, dur).SetEase(scaleEase))
                .Join(t.DORotate(new Vector3(0f, 1080f * strength, 0f), dur, RotateMode.FastBeyond360).SetEase(spinEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Soft spin and shrink with mild anticipation overshoot on scale.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.9s | <b>Default ease:</b> InBack (1.2 overshoot, scale), Linear (spin)<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinScaleOutOvershootSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinScaleOutOvershootSoftPreset : CodePreset
    {
        public override string PresetName => "SpinScaleOutOvershootSoft";
        public override string Description => "Soft spin and shrink with mild anticipation";
        public override float DefaultDuration => 0.9f;
        public override float DefaultOvershoot => 1.2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            var t = target.transform;
            var dur = GetDuration(duration, options);
            var scaleEase = ResolveEase(options, Ease.InBack);
            var spinEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);
            var os = DefaultOvershoot * ResolveOvershootMultiplier(presetOptions);

            var endScale = ResolveTargetScale(options, Vector3.zero);
            return DOTween.Sequence()
                .Join(t.DOScale(endScale, dur).SetEase(scaleEase, os))
                .Join(t.DORotate(new Vector3(0f, 360f * strength, 0f), dur, RotateMode.FastBeyond360).SetEase(spinEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Hard spin and shrink with strong anticipation overshoot on scale.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InBack (2.0 overshoot, scale), Linear (spin)<br/>
    /// <b>Scale override:</b> TargetScale replaces zero.<br/>
    /// <b>Strength override:</b> Multiplies rotation degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinScaleOutOvershootHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinScaleOutOvershootHardPreset : CodePreset
    {
        public override string PresetName => "SpinScaleOutOvershootHard";
        public override string Description => "Hard spin and shrink with strong anticipation";
        public override float DefaultDuration => 0.5f;
        public override float DefaultOvershoot => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            var t = target.transform;
            var dur = GetDuration(duration, options);
            var scaleEase = ResolveEase(options, Ease.InBack);
            var spinEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);
            var os = DefaultOvershoot * ResolveOvershootMultiplier(presetOptions);

            var endScale = ResolveTargetScale(options, Vector3.zero);
            return DOTween.Sequence()
                .Join(t.DOScale(endScale, dur).SetEase(scaleEase, os))
                .Join(t.DORotate(new Vector3(0f, 1080f * strength, 0f), dur, RotateMode.FastBeyond360).SetEase(spinEase))
                .WithDefaults(presetOptions, target);
        }
    }
}
