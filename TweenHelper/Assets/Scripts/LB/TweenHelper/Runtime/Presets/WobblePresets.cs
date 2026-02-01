using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Wobbles the target's rotation back and forth on the Y axis using DOTween's punch rotation.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(0, 15, 0)</c> (15° amplitude), vibrato <c>8</c>,
    /// and elasticity <c>0.5</c>. Creates a shaking/head-shake oscillation on the yaw axis.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> DOTween punch default<br/>
    /// <b>Easing override:</b> No ease override (punch tweens use internal oscillation).<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> "No" gesture, shake-to-settle, spin wobble, rotational impact.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleY").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobblePreset : CodePreset
    {
        public override string PresetName => "WobbleY";
        public override string Description => "Wobbles rotation back and forth on Y";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(0, 15f * strength, 0), GetDuration(duration, options), 8, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Soft Y-axis wobble with small angle and fewer vibrations.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(0, 8, 0)</c>, vibrato <c>5</c>, elasticity <c>0.5</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.4s<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleYSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleYSoftPreset : CodePreset
    {
        public override string PresetName => "WobbleYSoft";
        public override string Description => "Soft Y-axis wobble";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(0, 8f * strength, 0), GetDuration(duration, options), 5, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Heavy Y-axis wobble with large angle and more vibrations.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(0, 25, 0)</c>, vibrato <c>12</c>, elasticity <c>0.5</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.6s<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleYHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleYHardPreset : CodePreset
    {
        public override string PresetName => "WobbleYHard";
        public override string Description => "Heavy Y-axis wobble";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(0, 25f * strength, 0), GetDuration(duration, options), 12, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Wobbles the target's rotation back and forth on the X axis using DOTween's punch rotation.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(15, 0, 0)</c> (15° amplitude), vibrato <c>8</c>,
    /// and elasticity <c>0.5</c>. Creates a nodding/tipping oscillation on the pitch axis.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> DOTween punch default<br/>
    /// <b>Easing override:</b> No ease override (punch tweens use internal oscillation).<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Nodding motion, forward/back wobble, rocking chair, impact tilt.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleX").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleXPreset : CodePreset
    {
        public override string PresetName => "WobbleX";
        public override string Description => "Wobbles rotation back and forth on X";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(15f * strength, 0, 0), GetDuration(duration, options), 8, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Soft X-axis wobble with small angle and fewer vibrations.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(8, 0, 0)</c>, vibrato <c>5</c>, elasticity <c>0.5</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.4s<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleXSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleXSoftPreset : CodePreset
    {
        public override string PresetName => "WobbleXSoft";
        public override string Description => "Soft X-axis wobble";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(8f * strength, 0, 0), GetDuration(duration, options), 5, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Heavy X-axis wobble with large angle and more vibrations.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(25, 0, 0)</c>, vibrato <c>12</c>, elasticity <c>0.5</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.6s<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleXHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleXHardPreset : CodePreset
    {
        public override string PresetName => "WobbleXHard";
        public override string Description => "Heavy X-axis wobble";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(25f * strength, 0, 0), GetDuration(duration, options), 12, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Wobbles the target's rotation back and forth on the Z axis using DOTween's punch rotation.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(0, 0, 11)</c> (11° amplitude), vibrato <c>7</c>,
    /// and elasticity <c>0.5</c>. Creates a side-to-side tilt oscillation on the roll axis.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.45s | <b>Default ease:</b> DOTween punch default<br/>
    /// <b>Easing override:</b> No ease override (punch tweens use internal oscillation).<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> 2D wobble, pendulum swing, teeter-totter, balance recovery.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleZPreset : CodePreset
    {
        public override string PresetName => "WobbleZ";
        public override string Description => "Wobbles rotation back and forth on Z";
        public override float DefaultDuration => 0.45f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(0, 0, 11f * strength), GetDuration(duration, options), 7, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Soft Z-axis wobble with small angle and fewer vibrations.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(0, 0, 8)</c>, vibrato <c>6</c>, elasticity <c>0.5</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.4s<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleZSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleZSoftPreset : CodePreset
    {
        public override string PresetName => "WobbleZSoft";
        public override string Description => "Soft Z-axis wobble";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(0, 0, 8f * strength), GetDuration(duration, options), 6, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Heavy Z-axis wobble with large angle and more vibrations.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(0, 0, 25)</c>, vibrato <c>10</c>, elasticity <c>0.5</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.6s<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleZHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleZHardPreset : CodePreset
    {
        public override string PresetName => "WobbleZHard";
        public override string Description => "Heavy Z-axis wobble";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(0, 0, 25f * strength), GetDuration(duration, options), 10, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Wobbles the target's rotation diagonally across both X and Y axes simultaneously.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(12, 12, 0)</c> (12° amplitude per axis),
    /// vibrato <c>8</c>, and elasticity <c>0.5</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> DOTween punch default<br/>
    /// <b>Easing override:</b> No ease override (punch tweens use internal oscillation).<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Complex wobble, impact reaction, organic jiggle, multi-axis disturbance.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleDiagonalXY").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleDiagonalXYPreset : CodePreset
    {
        public override string PresetName => "WobbleDiagonalXY";
        public override string Description => "Wobbles rotation diagonally across X and Y";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(12f * strength, 12f * strength, 0f), GetDuration(duration, options), 8, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Soft diagonal XY wobble with smaller angle and fewer vibrations.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.4s<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleDiagonalXYSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleDiagonalXYSoftPreset : CodePreset
    {
        public override string PresetName => "WobbleDiagonalXYSoft";
        public override string Description => "Soft diagonal XY wobble";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(7f * strength, 7f * strength, 0f), GetDuration(duration, options), 5, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Heavy diagonal XY wobble with larger angle and more vibrations.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.6s<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleDiagonalXYHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleDiagonalXYHardPreset : CodePreset
    {
        public override string PresetName => "WobbleDiagonalXYHard";
        public override string Description => "Heavy diagonal XY wobble";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(20f * strength, 20f * strength, 0f), GetDuration(duration, options), 12, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Wobbles the target's rotation diagonally across both X and Z axes simultaneously.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(12, 0, 12)</c> (12° amplitude per axis),
    /// vibrato <c>8</c>, and elasticity <c>0.5</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> DOTween punch default<br/>
    /// <b>Easing override:</b> No ease override (punch tweens use internal oscillation).<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Tumble wobble, unsteady object, physical disturbance, off-balance reaction.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleDiagonalXZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleDiagonalXZPreset : CodePreset
    {
        public override string PresetName => "WobbleDiagonalXZ";
        public override string Description => "Wobbles rotation diagonally across X and Z";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(12f * strength, 0f, 12f * strength), GetDuration(duration, options), 8, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Soft diagonal XZ wobble with smaller angle and fewer vibrations.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.4s<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleDiagonalXZSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleDiagonalXZSoftPreset : CodePreset
    {
        public override string PresetName => "WobbleDiagonalXZSoft";
        public override string Description => "Soft diagonal XZ wobble";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(7f * strength, 0f, 7f * strength), GetDuration(duration, options), 5, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Heavy diagonal XZ wobble with larger angle and more vibrations.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.6s<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleDiagonalXZHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleDiagonalXZHardPreset : CodePreset
    {
        public override string PresetName => "WobbleDiagonalXZHard";
        public override string Description => "Heavy diagonal XZ wobble";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(20f * strength, 0f, 20f * strength), GetDuration(duration, options), 12, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Wobbles the target's rotation diagonally across both Y and Z axes simultaneously.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(0, 12, 12)</c> (12° amplitude per axis),
    /// vibrato <c>8</c>, and elasticity <c>0.5</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> DOTween punch default<br/>
    /// <b>Easing override:</b> No ease override (punch tweens use internal oscillation).<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Sideways wobble, wind-blown object, lateral disturbance, off-kilter reaction.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleDiagonalYZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleDiagonalYZPreset : CodePreset
    {
        public override string PresetName => "WobbleDiagonalYZ";
        public override string Description => "Wobbles rotation diagonally across Y and Z";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(0f, 12f * strength, 12f * strength), GetDuration(duration, options), 8, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Soft diagonal YZ wobble with smaller angle and fewer vibrations.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.4s<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleDiagonalYZSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleDiagonalYZSoftPreset : CodePreset
    {
        public override string PresetName => "WobbleDiagonalYZSoft";
        public override string Description => "Soft diagonal YZ wobble";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(0f, 7f * strength, 7f * strength), GetDuration(duration, options), 5, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Heavy diagonal YZ wobble with larger angle and more vibrations.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.6s<br/>
    /// <b>Strength override:</b> Multiplies rotation angle (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleDiagonalYZHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleDiagonalYZHardPreset : CodePreset
    {
        public override string PresetName => "WobbleDiagonalYZHard";
        public override string Description => "Heavy diagonal YZ wobble";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            return target.transform.DOPunchRotation(new Vector3(0f, 20f * strength, 20f * strength), GetDuration(duration, options), 12, 0.5f)
                .WithDefaults(options, target);
        }
    }
}
