using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Internal factory for non-fade Wobble variants sharing the same punch rotation structure.
    /// </summary>
    internal static class WobbleFactory
    {
        public static Tween Create(GameObject target, Vector3 punchVector, float duration, int vibrato, float elasticity, TweenOptions options)
        {
            var strength = CodePreset.ResolveStrengthStatic(options);
            return target.transform.DOPunchRotation(punchVector * strength, duration, vibrato, elasticity)
                .WithDefaults(options, target);
        }
    }

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
            return WobbleFactory.Create(target, new Vector3(0, 15f, 0), GetDuration(duration, options), 8, 0.5f, options);
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
            return WobbleFactory.Create(target, new Vector3(0, 8f, 0), GetDuration(duration, options), 5, 0.5f, options);
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
            return WobbleFactory.Create(target, new Vector3(0, 25f, 0), GetDuration(duration, options), 12, 0.5f, options);
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
            return WobbleFactory.Create(target, new Vector3(15f, 0, 0), GetDuration(duration, options), 8, 0.5f, options);
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
            return WobbleFactory.Create(target, new Vector3(8f, 0, 0), GetDuration(duration, options), 5, 0.5f, options);
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
            return WobbleFactory.Create(target, new Vector3(25f, 0, 0), GetDuration(duration, options), 12, 0.5f, options);
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
            return WobbleFactory.Create(target, new Vector3(0, 0, 11f), GetDuration(duration, options), 7, 0.5f, options);
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
            return WobbleFactory.Create(target, new Vector3(0, 0, 8f), GetDuration(duration, options), 6, 0.5f, options);
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
            return WobbleFactory.Create(target, new Vector3(0, 0, 25f), GetDuration(duration, options), 10, 0.5f, options);
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
            return WobbleFactory.Create(target, new Vector3(12f, 12f, 0f), GetDuration(duration, options), 8, 0.5f, options);
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
            return WobbleFactory.Create(target, new Vector3(7f, 7f, 0f), GetDuration(duration, options), 5, 0.5f, options);
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
            return WobbleFactory.Create(target, new Vector3(20f, 20f, 0f), GetDuration(duration, options), 12, 0.5f, options);
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
            return WobbleFactory.Create(target, new Vector3(12f, 0f, 12f), GetDuration(duration, options), 8, 0.5f, options);
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
            return WobbleFactory.Create(target, new Vector3(7f, 0f, 7f), GetDuration(duration, options), 5, 0.5f, options);
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
            return WobbleFactory.Create(target, new Vector3(20f, 0f, 20f), GetDuration(duration, options), 12, 0.5f, options);
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
            return WobbleFactory.Create(target, new Vector3(0f, 12f, 12f), GetDuration(duration, options), 8, 0.5f, options);
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
            return WobbleFactory.Create(target, new Vector3(0f, 7f, 7f), GetDuration(duration, options), 5, 0.5f, options);
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
            return WobbleFactory.Create(target, new Vector3(0f, 20f, 20f), GetDuration(duration, options), 12, 0.5f, options);
        }
    }

    /// <summary>
    /// Internal factory for WobbleFade variants sharing the same punch rotation + optional fade structure.
    /// </summary>
    internal static class WobbleFadeFactory
    {
        public static Tween Create(GameObject target, Vector3 punchVector, float duration, int vibrato, float elasticity, TweenOptions options)
        {
            var strength = CodePreset.ResolveStrengthStatic(options);
            var seq = DOTween.Sequence();
            var endAlpha = CodePreset.ResolveTargetAlphaStatic(options, 0f);

            seq.Join(target.transform.DOPunchRotation(punchVector * strength, duration, vibrato, elasticity));

            var startAlpha = CodePreset.ResolveStartAlphaStatic(options, 1f);
            var fadeTween = CodePreset.CreateFadeTweenStatic(target, endAlpha, duration);
            if (fadeTween != null)
            {
                CodePreset.SetAlphaStatic(target, startAlpha);
                seq.Join(fadeTween.SetEase(Ease.Linear));
                seq.OnComplete(() => CodePreset.SetAlphaStatic(target, endAlpha));
            }

            return seq.WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Wobbles on X axis while fading out in parallel.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 2.0s | <b>Default fade:</b> Linear to alpha 0<br/>
    /// <b>Strength override:</b> Multiplies wobble angle (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.<br/>
    /// If no fadeable component exists, only the wobble is played.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleFadeX").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleFadeXPreset : CodePreset
    {
        public override string PresetName => "WobbleFadeX";
        public override string Description => "Wobble on X axis with fade out";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return WobbleFadeFactory.Create(target, new Vector3(15f, 0f, 0f), GetDuration(duration, options), 8, 0.5f, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Soft X-axis wobble while fading out.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.6s | <b>Default fade:</b> Linear to alpha 0<br/>
    /// <b>Strength override:</b> Multiplies wobble angle (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleFadeXSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleFadeXSoftPreset : CodePreset
    {
        public override string PresetName => "WobbleFadeXSoft";
        public override string Description => "Soft wobble on X axis with fade out";
        public override float DefaultDuration => 1.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return WobbleFadeFactory.Create(target, new Vector3(8f, 0f, 0f), GetDuration(duration, options), 5, 0.5f, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Heavy X-axis wobble while fading out.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 2.4s | <b>Default fade:</b> Linear to alpha 0<br/>
    /// <b>Strength override:</b> Multiplies wobble angle (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleFadeXHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleFadeXHardPreset : CodePreset
    {
        public override string PresetName => "WobbleFadeXHard";
        public override string Description => "Heavy wobble on X axis with fade out";
        public override float DefaultDuration => 2.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return WobbleFadeFactory.Create(target, new Vector3(25f, 0f, 0f), GetDuration(duration, options), 12, 0.5f, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Wobbles on Y axis while fading out in parallel.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 2.0s | <b>Default fade:</b> Linear to alpha 0<br/>
    /// <b>Strength override:</b> Multiplies wobble angle (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleFadeY").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleFadeYPreset : CodePreset
    {
        public override string PresetName => "WobbleFadeY";
        public override string Description => "Wobble on Y axis with fade out";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return WobbleFadeFactory.Create(target, new Vector3(0f, 15f, 0f), GetDuration(duration, options), 8, 0.5f, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Soft Y-axis wobble while fading out.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.6s | <b>Default fade:</b> Linear to alpha 0<br/>
    /// <b>Strength override:</b> Multiplies wobble angle (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleFadeYSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleFadeYSoftPreset : CodePreset
    {
        public override string PresetName => "WobbleFadeYSoft";
        public override string Description => "Soft wobble on Y axis with fade out";
        public override float DefaultDuration => 1.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return WobbleFadeFactory.Create(target, new Vector3(0f, 8f, 0f), GetDuration(duration, options), 5, 0.5f, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Heavy Y-axis wobble while fading out.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 2.4s | <b>Default fade:</b> Linear to alpha 0<br/>
    /// <b>Strength override:</b> Multiplies wobble angle (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleFadeYHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleFadeYHardPreset : CodePreset
    {
        public override string PresetName => "WobbleFadeYHard";
        public override string Description => "Heavy wobble on Y axis with fade out";
        public override float DefaultDuration => 2.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return WobbleFadeFactory.Create(target, new Vector3(0f, 25f, 0f), GetDuration(duration, options), 12, 0.5f, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Wobbles on Z axis while fading out in parallel.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.8s | <b>Default fade:</b> Linear to alpha 0<br/>
    /// <b>Strength override:</b> Multiplies wobble angle (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleFadeZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleFadeZPreset : CodePreset
    {
        public override string PresetName => "WobbleFadeZ";
        public override string Description => "Wobble on Z axis with fade out";
        public override float DefaultDuration => 1.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return WobbleFadeFactory.Create(target, new Vector3(0f, 0f, 11f), GetDuration(duration, options), 7, 0.5f, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Soft Z-axis wobble while fading out.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.6s | <b>Default fade:</b> Linear to alpha 0<br/>
    /// <b>Strength override:</b> Multiplies wobble angle (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleFadeZSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleFadeZSoftPreset : CodePreset
    {
        public override string PresetName => "WobbleFadeZSoft";
        public override string Description => "Soft wobble on Z axis with fade out";
        public override float DefaultDuration => 1.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return WobbleFadeFactory.Create(target, new Vector3(0f, 0f, 8f), GetDuration(duration, options), 6, 0.5f, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Heavy Z-axis wobble while fading out.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 2.4s | <b>Default fade:</b> Linear to alpha 0<br/>
    /// <b>Strength override:</b> Multiplies wobble angle (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleFadeZHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleFadeZHardPreset : CodePreset
    {
        public override string PresetName => "WobbleFadeZHard";
        public override string Description => "Heavy wobble on Z axis with fade out";
        public override float DefaultDuration => 2.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return WobbleFadeFactory.Create(target, new Vector3(0f, 0f, 25f), GetDuration(duration, options), 10, 0.5f, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }
}
