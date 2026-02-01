using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Internal factory for SlideIn variants sharing the same entrance animation structure.
    /// </summary>
    internal static class SlideInFactory
    {
        public static Tween Create(GameObject target, Vector3 offsetDirection, float distance, float duration, TweenOptions options)
        {
            var strength = CodePreset.ResolveStrengthStatic(options);
            var t = target.transform;
            var targetPos = t.localPosition;
            t.localPosition = targetPos + offsetDirection * (distance * strength);

            var presetOptions = options.Ease.HasValue ? options : options.SetEase(Ease.OutCubic);
            var ease = presetOptions.Ease ?? Ease.OutCubic;

            return t.DOLocalMove(targetPos, duration)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Slides the target downward from 500 units above its current local position to its original position.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.0s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces OutCubic.<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInDown").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInDownPreset : CodePreset
    {
        public override string PresetName => "SlideInDown";
        public override string Description => "Slides down from above";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFactory.Create(target, Vector3.up, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Slides the target upward from 500 units below its current local position to its original position.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.0s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces OutCubic.<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInUp").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInUpPreset : CodePreset
    {
        public override string PresetName => "SlideInUp";
        public override string Description => "Slides up from below";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFactory.Create(target, Vector3.down, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Slides the target in from 500 units to the left of its current local position.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.0s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces OutCubic.<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInLeft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInLeftPreset : CodePreset
    {
        public override string PresetName => "SlideInLeft";
        public override string Description => "Slides in from the left side";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFactory.Create(target, Vector3.left, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Slides the target in from 500 units to the right of its current local position.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.0s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces OutCubic.<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInRight").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInRightPreset : CodePreset
    {
        public override string PresetName => "SlideInRight";
        public override string Description => "Slides in from the right side";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFactory.Create(target, Vector3.right, 500f, GetDuration(duration, options), options);
        }
    }

    // --- SlideIn Soft variants (duration 1.5s) ---

    /// <summary>
    /// Soft slide in from above. Slower, gentler entrance.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.5s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInDownSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInDownSoftPreset : CodePreset
    {
        public override string PresetName => "SlideInDownSoft";
        public override string Description => "Slowly slides down from above";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFactory.Create(target, Vector3.up, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft slide in from below. Slower, gentler entrance.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.5s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInUpSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInUpSoftPreset : CodePreset
    {
        public override string PresetName => "SlideInUpSoft";
        public override string Description => "Slowly slides up from below";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFactory.Create(target, Vector3.down, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft slide in from the left. Slower, gentler entrance.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.5s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInLeftSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInLeftSoftPreset : CodePreset
    {
        public override string PresetName => "SlideInLeftSoft";
        public override string Description => "Slowly slides in from the left";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFactory.Create(target, Vector3.left, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft slide in from the right. Slower, gentler entrance.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.5s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInRightSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInRightSoftPreset : CodePreset
    {
        public override string PresetName => "SlideInRightSoft";
        public override string Description => "Slowly slides in from the right";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFactory.Create(target, Vector3.right, 500f, GetDuration(duration, options), options);
        }
    }

    // --- SlideIn Hard variants (duration 0.5s) ---

    /// <summary>
    /// Hard slide in from above. Fast, snappy entrance.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInDownHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInDownHardPreset : CodePreset
    {
        public override string PresetName => "SlideInDownHard";
        public override string Description => "Quickly slides down from above";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFactory.Create(target, Vector3.up, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Hard slide in from below. Fast, snappy entrance.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInUpHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInUpHardPreset : CodePreset
    {
        public override string PresetName => "SlideInUpHard";
        public override string Description => "Quickly slides up from below";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFactory.Create(target, Vector3.down, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Hard slide in from the left. Fast, snappy entrance.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInLeftHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInLeftHardPreset : CodePreset
    {
        public override string PresetName => "SlideInLeftHard";
        public override string Description => "Quickly slides in from the left";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFactory.Create(target, Vector3.left, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Hard slide in from the right. Fast, snappy entrance.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInRightHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInRightHardPreset : CodePreset
    {
        public override string PresetName => "SlideInRightHard";
        public override string Description => "Quickly slides in from the right";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFactory.Create(target, Vector3.right, 500f, GetDuration(duration, options), options);
        }
    }

    // --- SlideOut presets (duration fixed from 0.5s to 1.0s) ---

    /// <summary>
    /// Internal factory for SlideOut variants sharing the same exit animation structure.
    /// </summary>
    internal static class SlideOutFactory
    {
        public static Tween Create(GameObject target, Vector3 direction, float distance, float duration, TweenOptions options)
        {
            var strength = CodePreset.ResolveStrengthStatic(options);
            var t = target.transform;
            var presetOptions = options.Ease.HasValue ? options : options.SetEase(Ease.InCubic);
            var ease = presetOptions.Ease ?? Ease.InCubic;
            var endPos = t.localPosition + direction * (distance * strength);

            return t.DOLocalMove(endPos, duration)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Slides the target 500 units upward off-screen, mirroring SlideInDown as an exit animation.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces InCubic.<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideOutUp").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideOutUpPreset : CodePreset
    {
        public override string PresetName => "SlideOutUp";
        public override string Description => "Slides up off-screen";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideOutFactory.Create(target, Vector3.up, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Slides the target 500 units downward off-screen, mirroring SlideInUp as an exit animation.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces InCubic.<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideOutDown").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideOutDownPreset : CodePreset
    {
        public override string PresetName => "SlideOutDown";
        public override string Description => "Slides down off-screen";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideOutFactory.Create(target, Vector3.down, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Slides the target 500 units to the left off-screen, mirroring SlideInRight as an exit animation.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces InCubic.<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideOutLeft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideOutLeftPreset : CodePreset
    {
        public override string PresetName => "SlideOutLeft";
        public override string Description => "Slides left off-screen";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideOutFactory.Create(target, Vector3.left, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Slides the target 500 units to the right off-screen, mirroring SlideInLeft as an exit animation.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces InCubic.<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideOutRight").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideOutRightPreset : CodePreset
    {
        public override string PresetName => "SlideOutRight";
        public override string Description => "Slides right off-screen";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideOutFactory.Create(target, Vector3.right, 500f, GetDuration(duration, options), options);
        }
    }

    // --- SlideOut Soft variants (duration 1.5s) ---

    /// <summary>
    /// Soft slide out upward. Slower exit for gentle dismissal.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 1.5s | <b>Default ease:</b> InCubic<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideOutUpSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideOutUpSoftPreset : CodePreset
    {
        public override string PresetName => "SlideOutUpSoft";
        public override string Description => "Slowly slides up off-screen";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideOutFactory.Create(target, Vector3.up, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft slide out downward. Slower exit for gentle dismissal.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 1.5s | <b>Default ease:</b> InCubic<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideOutDownSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideOutDownSoftPreset : CodePreset
    {
        public override string PresetName => "SlideOutDownSoft";
        public override string Description => "Slowly slides down off-screen";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideOutFactory.Create(target, Vector3.down, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft slide out to the left. Slower exit for gentle dismissal.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 1.5s | <b>Default ease:</b> InCubic<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideOutLeftSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideOutLeftSoftPreset : CodePreset
    {
        public override string PresetName => "SlideOutLeftSoft";
        public override string Description => "Slowly slides left off-screen";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideOutFactory.Create(target, Vector3.left, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft slide out to the right. Slower exit for gentle dismissal.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 1.5s | <b>Default ease:</b> InCubic<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideOutRightSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideOutRightSoftPreset : CodePreset
    {
        public override string PresetName => "SlideOutRightSoft";
        public override string Description => "Slowly slides right off-screen";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideOutFactory.Create(target, Vector3.right, 500f, GetDuration(duration, options), options);
        }
    }

    // --- SlideOut Hard variants (duration 0.5s) ---

    /// <summary>
    /// Hard slide out upward. Fast snappy exit.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InCubic<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideOutUpHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideOutUpHardPreset : CodePreset
    {
        public override string PresetName => "SlideOutUpHard";
        public override string Description => "Quickly slides up off-screen";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideOutFactory.Create(target, Vector3.up, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Hard slide out downward. Fast snappy exit.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InCubic<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideOutDownHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideOutDownHardPreset : CodePreset
    {
        public override string PresetName => "SlideOutDownHard";
        public override string Description => "Quickly slides down off-screen";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideOutFactory.Create(target, Vector3.down, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Hard slide out to the left. Fast snappy exit.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InCubic<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideOutLeftHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideOutLeftHardPreset : CodePreset
    {
        public override string PresetName => "SlideOutLeftHard";
        public override string Description => "Quickly slides left off-screen";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideOutFactory.Create(target, Vector3.left, 500f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Hard slide out to the right. Fast snappy exit.
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InCubic<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideOutRightHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideOutRightHardPreset : CodePreset
    {
        public override string PresetName => "SlideOutRightHard";
        public override string Description => "Quickly slides right off-screen";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideOutFactory.Create(target, Vector3.right, 500f, GetDuration(duration, options), options);
        }
    }

    // --- SlideInFade presets (durations fixed) ---

    /// <summary>
    /// Internal factory for SlideInFade variants sharing the same slide + fade entrance structure.
    /// </summary>
    internal static class SlideInFadeFactory
    {
        public static Tween Create(GameObject target, Vector3 offsetDirection, float distance, float duration, TweenOptions options)
        {
            var strength = CodePreset.ResolveStrengthStatic(options);
            var t = target.transform;
            var targetPos = t.localPosition;
            t.localPosition = targetPos + offsetDirection * (distance * strength);

            var presetOptions = options.Ease.HasValue ? options : options.SetEase(Ease.OutCubic);
            var ease = presetOptions.Ease ?? Ease.OutCubic;

            var seq = DOTween.Sequence();
            seq.Append(t.DOLocalMove(targetPos, duration).SetEase(ease));

            var fadeTween = CodePreset.CreateFadeTweenStatic(target, CodePreset.ResolveTargetAlphaStatic(options, 1f), duration * 0.7f);
            if (fadeTween != null)
            {
                CodePreset.SetAlphaStatic(target, CodePreset.ResolveStartAlphaStatic(options, 0f));
                seq.Join(fadeTween.SetEase(Ease.Linear));
            }

            return seq.WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Slides the target upward from 100 units below while simultaneously fading in (fade completes at 70% duration).
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 2.0s | <b>Default ease:</b> OutCubic (move), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 0; TargetAlpha replaces 1.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInFadeUp").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInFadeUpPreset : CodePreset
    {
        public override string PresetName => "SlideInFadeUp";
        public override string Description => "Slides up from below with fade in";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFadeFactory.Create(target, Vector3.down, 100f, GetDuration(duration, options), options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Slides the target downward from 100 units above while simultaneously fading in (fade completes at 70% duration).
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 2.0s | <b>Default ease:</b> OutCubic (move), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 0; TargetAlpha replaces 1.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInFadeDown").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInFadeDownPreset : CodePreset
    {
        public override string PresetName => "SlideInFadeDown";
        public override string Description => "Slides down from above with fade in";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFadeFactory.Create(target, Vector3.up, 100f, GetDuration(duration, options), options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Slides the target in from 100 units to the left while simultaneously fading in (fade completes at 70% duration).
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 2.5s | <b>Default ease:</b> OutCubic (move), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 0; TargetAlpha replaces 1.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInFadeLeft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInFadeLeftPreset : CodePreset
    {
        public override string PresetName => "SlideInFadeLeft";
        public override string Description => "Slides in from the left with fade in";
        public override float DefaultDuration => 2.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFadeFactory.Create(target, Vector3.left, 100f, GetDuration(duration, options), options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Slides the target in from 100 units to the right while simultaneously fading in (fade completes at 70% duration).
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 2.5s | <b>Default ease:</b> OutCubic (move), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 0; TargetAlpha replaces 1.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInFadeRight").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInFadeRightPreset : CodePreset
    {
        public override string PresetName => "SlideInFadeRight";
        public override string Description => "Slides in from the right with fade in";
        public override float DefaultDuration => 2.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFadeFactory.Create(target, Vector3.right, 100f, GetDuration(duration, options), options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    // --- SlideInFade Soft variants ---

    /// <summary>
    /// Soft slide-in-fade from below. Slower, gentler entrance.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 3.0s | <b>Default ease:</b> OutCubic (move), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 0; TargetAlpha replaces 1.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInFadeUpSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInFadeUpSoftPreset : CodePreset
    {
        public override string PresetName => "SlideInFadeUpSoft";
        public override string Description => "Slowly slides up from below with fade in";
        public override float DefaultDuration => 3.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFadeFactory.Create(target, Vector3.down, 100f, GetDuration(duration, options), options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Soft slide-in-fade from above. Slower, gentler entrance.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 3.0s | <b>Default ease:</b> OutCubic (move), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 0; TargetAlpha replaces 1.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInFadeDownSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInFadeDownSoftPreset : CodePreset
    {
        public override string PresetName => "SlideInFadeDownSoft";
        public override string Description => "Slowly slides down from above with fade in";
        public override float DefaultDuration => 3.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFadeFactory.Create(target, Vector3.up, 100f, GetDuration(duration, options), options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Soft slide-in-fade from the left. Slower, gentler entrance.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 3.5s | <b>Default ease:</b> OutCubic (move), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 0; TargetAlpha replaces 1.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInFadeLeftSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInFadeLeftSoftPreset : CodePreset
    {
        public override string PresetName => "SlideInFadeLeftSoft";
        public override string Description => "Slowly slides in from the left with fade in";
        public override float DefaultDuration => 3.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFadeFactory.Create(target, Vector3.left, 100f, GetDuration(duration, options), options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Soft slide-in-fade from the right. Slower, gentler entrance.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 3.5s | <b>Default ease:</b> OutCubic (move), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 0; TargetAlpha replaces 1.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInFadeRightSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInFadeRightSoftPreset : CodePreset
    {
        public override string PresetName => "SlideInFadeRightSoft";
        public override string Description => "Slowly slides in from the right with fade in";
        public override float DefaultDuration => 3.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFadeFactory.Create(target, Vector3.right, 100f, GetDuration(duration, options), options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    // --- SlideInFade Hard variants ---

    /// <summary>
    /// Hard slide-in-fade from below. Fast, snappy entrance.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.0s | <b>Default ease:</b> OutCubic (move), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 0; TargetAlpha replaces 1.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInFadeUpHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInFadeUpHardPreset : CodePreset
    {
        public override string PresetName => "SlideInFadeUpHard";
        public override string Description => "Quickly slides up from below with fade in";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFadeFactory.Create(target, Vector3.down, 100f, GetDuration(duration, options), options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Hard slide-in-fade from above. Fast, snappy entrance.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.0s | <b>Default ease:</b> OutCubic (move), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 0; TargetAlpha replaces 1.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInFadeDownHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInFadeDownHardPreset : CodePreset
    {
        public override string PresetName => "SlideInFadeDownHard";
        public override string Description => "Quickly slides down from above with fade in";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFadeFactory.Create(target, Vector3.up, 100f, GetDuration(duration, options), options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Hard slide-in-fade from the left. Fast, snappy entrance.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.5s | <b>Default ease:</b> OutCubic (move), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 0; TargetAlpha replaces 1.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInFadeLeftHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInFadeLeftHardPreset : CodePreset
    {
        public override string PresetName => "SlideInFadeLeftHard";
        public override string Description => "Quickly slides in from the left with fade in";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFadeFactory.Create(target, Vector3.left, 100f, GetDuration(duration, options), options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Hard slide-in-fade from the right. Fast, snappy entrance.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.5s | <b>Default ease:</b> OutCubic (move), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies slide distance (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 0; TargetAlpha replaces 1.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInFadeRightHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInFadeRightHardPreset : CodePreset
    {
        public override string PresetName => "SlideInFadeRightHard";
        public override string Description => "Quickly slides in from the right with fade in";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return SlideInFadeFactory.Create(target, Vector3.right, 100f, GetDuration(duration, options), options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }
}
