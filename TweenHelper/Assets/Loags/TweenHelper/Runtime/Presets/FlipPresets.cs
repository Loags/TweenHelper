using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Internal factory for non-fade Flip variants sharing the same additive local rotation structure.
    /// </summary>
    internal static class FlipFactory
    {
        public static Tween Create(GameObject target, Vector3 axis, float degrees, float duration, Ease defaultEase, TweenOptions options)
        {
            var strength = CodePreset.ResolveStrengthStatic(options);
            var presetOptions = options.Ease.HasValue ? options : options.SetEase(defaultEase);
            var ease = presetOptions.Ease ?? defaultEase;

            return target.transform.DOLocalRotate(axis * (degrees * strength), duration, RotateMode.LocalAxisAdd)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Flips the target 180 degrees on the X axis using additive local rotation.
    /// <para>
    /// Rotates by <c>(180, 0, 0)</c> using <c>RotateMode.LocalAxisAdd</c> with <c>Ease.InOutQuad</c>.
    /// LocalAxisAdd ensures the rotation is added to the current orientation rather than targeting
    /// an absolute angle, making it safe to chain multiple flips.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Card flip, page turn, reveal animation, object inspection.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipX").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipXPreset : CodePreset
    {
        public override string PresetName => "FlipX";
        public override string Description => "180° flip on X axis";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFactory.Create(target, Vector3.right, 180f, GetDuration(duration, options), Ease.InOutQuad, options);
        }
    }

    /// <summary>
    /// Slow 180° flip on the X axis — longer duration for a gentle, cinematic rotation.
    /// <para>
    /// Rotates by <c>(180, 0, 0)</c> using <c>RotateMode.LocalAxisAdd</c> with <c>Ease.InOutSine</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.8s | <b>Default ease:</b> InOutSine<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipXSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipXSoftPreset : CodePreset
    {
        public override string PresetName => "FlipXSoft";
        public override string Description => "Slow 180° flip on X axis";
        public override float DefaultDuration => 0.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFactory.Create(target, Vector3.right, 180f, GetDuration(duration, options), Ease.InOutSine, options);
        }
    }

    /// <summary>
    /// Quick 180° flip on the X axis — shorter duration for a snappy rotation.
    /// <para>
    /// Rotates by <c>(180, 0, 0)</c> using <c>RotateMode.LocalAxisAdd</c> with <c>Ease.OutQuad</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.25s | <b>Default ease:</b> OutQuad<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipXHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipXHardPreset : CodePreset
    {
        public override string PresetName => "FlipXHard";
        public override string Description => "Quick 180° flip on X axis";
        public override float DefaultDuration => 0.25f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFactory.Create(target, Vector3.right, 180f, GetDuration(duration, options), Ease.OutQuad, options);
        }
    }

    /// <summary>
    /// Flips the target 180 degrees on the Y axis using additive local rotation.
    /// <para>
    /// Rotates by <c>(0, 180, 0)</c> using <c>RotateMode.LocalAxisAdd</c> with <c>Ease.InOutQuad</c>.
    /// LocalAxisAdd ensures the rotation is added to the current orientation rather than targeting
    /// an absolute angle, making it safe to chain multiple flips.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Horizontal card flip, character turn-around, mirror reveal, about-face.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipY").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipYPreset : CodePreset
    {
        public override string PresetName => "FlipY";
        public override string Description => "180° flip on Y axis";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFactory.Create(target, Vector3.up, 180f, GetDuration(duration, options), Ease.InOutQuad, options);
        }
    }

    /// <summary>
    /// Slow 180° flip on the Y axis — longer duration for a gentle, cinematic rotation.
    /// <para>
    /// Rotates by <c>(0, 180, 0)</c> using <c>RotateMode.LocalAxisAdd</c> with <c>Ease.InOutSine</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.8s | <b>Default ease:</b> InOutSine<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipYSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipYSoftPreset : CodePreset
    {
        public override string PresetName => "FlipYSoft";
        public override string Description => "Slow 180° flip on Y axis";
        public override float DefaultDuration => 0.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFactory.Create(target, Vector3.up, 180f, GetDuration(duration, options), Ease.InOutSine, options);
        }
    }

    /// <summary>
    /// Quick 180° flip on the Y axis — shorter duration for a snappy rotation.
    /// <para>
    /// Rotates by <c>(0, 180, 0)</c> using <c>RotateMode.LocalAxisAdd</c> with <c>Ease.OutQuad</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.25s | <b>Default ease:</b> OutQuad<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipYHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipYHardPreset : CodePreset
    {
        public override string PresetName => "FlipYHard";
        public override string Description => "Quick 180° flip on Y axis";
        public override float DefaultDuration => 0.25f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFactory.Create(target, Vector3.up, 180f, GetDuration(duration, options), Ease.OutQuad, options);
        }
    }

    /// <summary>
    /// Flips the target 180 degrees on the Z axis using additive local rotation.
    /// <para>
    /// Rotates by <c>(0, 0, 180)</c> using <c>RotateMode.LocalAxisAdd</c> with <c>Ease.InOutQuad</c>.
    /// LocalAxisAdd ensures the rotation is added to the current orientation rather than targeting
    /// an absolute angle, making it safe to chain multiple flips.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Barrel roll, coin spin, stylized rotation transition.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipZPreset : CodePreset
    {
        public override string PresetName => "FlipZ";
        public override string Description => "180° flip on Z axis";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFactory.Create(target, Vector3.forward, 180f, GetDuration(duration, options), Ease.InOutQuad, options);
        }
        }

    /// <summary>
    /// Slow 180° flip on the Z axis — longer duration for a gentle, cinematic rotation.
    /// <para>
    /// Rotates by <c>(0, 0, 180)</c> using <c>RotateMode.LocalAxisAdd</c> with <c>Ease.InOutSine</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.8s | <b>Default ease:</b> InOutSine<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipZSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipZSoftPreset : CodePreset
    {
        public override string PresetName => "FlipZSoft";
        public override string Description => "Slow 180° flip on Z axis";
        public override float DefaultDuration => 0.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFactory.Create(target, Vector3.forward, 180f, GetDuration(duration, options), Ease.InOutSine, options);
        }
    }

    /// <summary>
    /// Quick 180° flip on the Z axis — shorter duration for a snappy rotation.
    /// <para>
    /// Rotates by <c>(0, 0, 180)</c> using <c>RotateMode.LocalAxisAdd</c> with <c>Ease.OutQuad</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.25s | <b>Default ease:</b> OutQuad<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipZHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipZHardPreset : CodePreset
    {
        public override string PresetName => "FlipZHard";
        public override string Description => "Quick 180° flip on Z axis";
        public override float DefaultDuration => 0.25f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFactory.Create(target, Vector3.forward, 180f, GetDuration(duration, options), Ease.OutQuad, options);
        }
    }

    /// <summary>
    /// Internal factory for FlipFade variants sharing the same rotate + optional fade structure.
    /// </summary>
    internal static class FlipFadeFactory
    {
        public static Tween Create(GameObject target, Vector3 axis, float degrees, float duration, Ease defaultFlipEase, TweenOptions options)
        {
            var strength = CodePreset.ResolveStrengthStatic(options);
            var presetOptions = options.Ease.HasValue ? options : options.SetEase(defaultFlipEase);
            var flipEase = presetOptions.Ease ?? defaultFlipEase;
            var endAlpha = CodePreset.ResolveTargetAlphaStatic(options, 0f);

            var seq = DOTween.Sequence();
            seq.Join(target.transform.DOLocalRotate(axis * (degrees * strength), duration, RotateMode.LocalAxisAdd).SetEase(flipEase));

            var startAlpha = CodePreset.ResolveStartAlphaStatic(options, 1f);
            var fadeTween = CodePreset.CreateFadeTweenStatic(target, endAlpha, duration);
            if (fadeTween != null)
            {
                CodePreset.SetAlphaStatic(target, startAlpha);
                seq.Join(fadeTween.SetEase(Ease.Linear));
                seq.OnComplete(() => CodePreset.SetAlphaStatic(target, endAlpha));
            }

            return seq.WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Flips 180° on X while fading out in parallel.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad (flip), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.<br/>
    /// If no fadeable component exists, only the flip is played.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipFadeX").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipFadeXPreset : CodePreset
    {
        public override string PresetName => "FlipFadeX";
        public override string Description => "180° flip on X axis with fade out";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFadeFactory.Create(target, Vector3.right, 180f, GetDuration(duration, options), Ease.InOutQuad, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Slow 180° flip on X while fading out.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.6s | <b>Default ease:</b> InOutSine (flip), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipFadeXSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipFadeXSoftPreset : CodePreset
    {
        public override string PresetName => "FlipFadeXSoft";
        public override string Description => "Slow 180° flip on X axis with fade out";
        public override float DefaultDuration => 1.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFadeFactory.Create(target, Vector3.right, 180f, GetDuration(duration, options), Ease.InOutSine, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Quick 180° flip on X while fading out.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutQuad (flip), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipFadeXHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipFadeXHardPreset : CodePreset
    {
        public override string PresetName => "FlipFadeXHard";
        public override string Description => "Quick 180° flip on X axis with fade out";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFadeFactory.Create(target, Vector3.right, 180f, GetDuration(duration, options), Ease.OutQuad, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Flips 180° on Y while fading out in parallel.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad (flip), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipFadeY").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipFadeYPreset : CodePreset
    {
        public override string PresetName => "FlipFadeY";
        public override string Description => "180° flip on Y axis with fade out";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFadeFactory.Create(target, Vector3.up, 180f, GetDuration(duration, options), Ease.InOutQuad, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Slow 180° flip on Y while fading out.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.6s | <b>Default ease:</b> InOutSine (flip), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipFadeYSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipFadeYSoftPreset : CodePreset
    {
        public override string PresetName => "FlipFadeYSoft";
        public override string Description => "Slow 180° flip on Y axis with fade out";
        public override float DefaultDuration => 1.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFadeFactory.Create(target, Vector3.up, 180f, GetDuration(duration, options), Ease.InOutSine, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Quick 180° flip on Y while fading out.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutQuad (flip), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipFadeYHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipFadeYHardPreset : CodePreset
    {
        public override string PresetName => "FlipFadeYHard";
        public override string Description => "Quick 180° flip on Y axis with fade out";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFadeFactory.Create(target, Vector3.up, 180f, GetDuration(duration, options), Ease.OutQuad, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Flips 180° on Z while fading out in parallel.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad (flip), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipFadeZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipFadeZPreset : CodePreset
    {
        public override string PresetName => "FlipFadeZ";
        public override string Description => "180° flip on Z axis with fade out";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFadeFactory.Create(target, Vector3.forward, 180f, GetDuration(duration, options), Ease.InOutQuad, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Slow 180° flip on Z while fading out.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.6s | <b>Default ease:</b> InOutSine (flip), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipFadeZSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipFadeZSoftPreset : CodePreset
    {
        public override string PresetName => "FlipFadeZSoft";
        public override string Description => "Slow 180° flip on Z axis with fade out";
        public override float DefaultDuration => 1.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFadeFactory.Create(target, Vector3.forward, 180f, GetDuration(duration, options), Ease.InOutSine, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Quick 180° flip on Z while fading out.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutQuad (flip), Linear (fade)<br/>
    /// <b>Strength override:</b> Multiplies flip degrees (default 1.0).<br/>
    /// <b>Alpha override:</b> StartAlpha replaces 1; TargetAlpha replaces 0.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipFadeZHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipFadeZHardPreset : CodePreset
    {
        public override string PresetName => "FlipFadeZHard";
        public override string Description => "Quick 180° flip on Z axis with fade out";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FlipFadeFactory.Create(target, Vector3.forward, 180f, GetDuration(duration, options), Ease.OutQuad, options);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }
}
