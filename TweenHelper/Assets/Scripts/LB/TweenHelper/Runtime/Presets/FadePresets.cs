using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Fades the target in from fully transparent (alpha 0) to fully opaque (alpha 1).
    /// <para>
    /// Initializes alpha to <c>0</c> at the start, then creates a fade tween to <c>1.0</c>.
    /// Uses OutQuad ease by default for a quick start that decelerates toward full visibility.
    /// Supports CanvasGroup, SpriteRenderer, Image, Text, and Renderer materials.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entry | <b>Default duration:</b> 3.0s | <b>Default ease:</b> OutQuad<br/>
    /// <b>Easing override:</b> Standard options apply via <c>WithDefaults</c>.<br/>
    /// <b>Requires:</b> A fadeable component (CanvasGroup, SpriteRenderer, Image, Text, or Renderer with material).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Scene transition reveal, gradual element appearance, cinematic fade-in.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FadeIn").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FadeInPreset : CodePreset
    {
        public override string PresetName => "FadeIn";
        public override string Description => "Fades in from transparent (requires fadeable component)";
        public override float DefaultDuration => 3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            SetAlpha(target, 0f);
            var tween = CreateFadeTween(target, 1f, GetDuration(duration, options));
            // Quick start getting visible, decelerating toward full opacity
            var presetOptions = MergeWithDefaultEase(options, Ease.OutQuad);
            var ease = ResolveEase(presetOptions, Ease.OutQuad);
            return tween?.SetEase(ease).WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Slow fade in from transparent — longer duration for a gentle, gradual appearance.
    /// <para>
    /// Initializes alpha to <c>0</c>, then creates a fade tween to <c>1.0</c>.
    /// Uses <c>Ease.InQuad</c> for a gentle start that slowly accelerates toward full visibility.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entry | <b>Default duration:</b> 5.0s | <b>Default ease:</b> InQuad<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FadeInSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FadeInSoftPreset : CodePreset
    {
        public override string PresetName => "FadeInSoft";
        public override string Description => "Slow fade in from transparent (requires fadeable component)";
        public override float DefaultDuration => 5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            SetAlpha(target, 0f);
            var tween = CreateFadeTween(target, 1f, GetDuration(duration, options));
            var presetOptions = MergeWithDefaultEase(options, Ease.InQuad);
            var ease = ResolveEase(presetOptions, Ease.InQuad);
            return tween?.SetEase(ease).WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Quick fade in from transparent — shorter duration for a snappy appearance.
    /// <para>
    /// Initializes alpha to <c>0</c>, then creates a fade tween to <c>1.0</c>.
    /// Uses <c>Ease.OutQuad</c> for a fast start that decelerates toward full visibility.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entry | <b>Default duration:</b> 1.0s | <b>Default ease:</b> OutQuad<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FadeInHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FadeInHardPreset : CodePreset
    {
        public override string PresetName => "FadeInHard";
        public override string Description => "Quick fade in from transparent (requires fadeable component)";
        public override float DefaultDuration => 1f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            SetAlpha(target, 0f);
            var tween = CreateFadeTween(target, 1f, GetDuration(duration, options));
            var presetOptions = MergeWithDefaultEase(options, Ease.OutQuad);
            var ease = ResolveEase(presetOptions, Ease.OutQuad);
            return tween?.SetEase(ease).WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Fades the target out from its current alpha to fully transparent (alpha 0).
    /// <para>
    /// Creates a fade tween to <c>0.0</c> using the default ease from <c>TweenHelperSettings</c>.
    /// Does not override the initial alpha — fades from whatever the current value is.
    /// Supports CanvasGroup, SpriteRenderer, Image, Text, and Renderer materials.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 3.0s | <b>Default ease:</b> Settings default<br/>
    /// <b>Easing override:</b> Standard options apply via <c>WithDefaults</c>.<br/>
    /// <b>Requires:</b> A fadeable component (CanvasGroup, SpriteRenderer, Image, Text, or Renderer with material).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Scene transition fade-out, element dismissal, death effect, gradual disappearance.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FadeOut").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FadeOutPreset : CodePreset
    {
        public override string PresetName => "FadeOut";
        public override string Description => "Fades out to transparent (requires fadeable component)";
        public override float DefaultDuration => 3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var tween = CreateFadeTween(target, 0f, GetDuration(duration, options));
            return tween?.WithDefaults(options, target);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Slow fade out from current alpha to fully transparent with a gradual start.
    /// <para>
    /// Creates a fade tween to <c>0.0</c> using <c>Ease.InQuad</c> for a gentle, lingering disappearance.
    /// Does not override the initial alpha — fades from whatever the current value is.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 5.0s | <b>Default ease:</b> InQuad<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FadeOutSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FadeOutSoftPreset : CodePreset
    {
        public override string PresetName => "FadeOutSoft";
        public override string Description => "Slow fade out to transparent (requires fadeable component)";
        public override float DefaultDuration => 5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.InQuad);
            var ease = ResolveEase(presetOptions, Ease.InQuad);
            var tween = CreateFadeTween(target, 0f, GetDuration(duration, options));
            return tween?.SetEase(ease).WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Quick fade out from current alpha to fully transparent with a fast start.
    /// <para>
    /// Creates a fade tween to <c>0.0</c> using <c>Ease.OutQuad</c> for a snappy disappearance
    /// that decelerates toward full transparency.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 1.0s | <b>Default ease:</b> OutQuad<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FadeOutHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FadeOutHardPreset : CodePreset
    {
        public override string PresetName => "FadeOutHard";
        public override string Description => "Quick fade out to transparent (requires fadeable component)";
        public override float DefaultDuration => 1f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.OutQuad);
            var ease = ResolveEase(presetOptions, Ease.OutQuad);
            var tween = CreateFadeTween(target, 0f, GetDuration(duration, options));
            return tween?.SetEase(ease).WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Rapidly toggles alpha between fully opaque and fully transparent in a continuous loop.
    /// <para>
    /// Alternates between fading to <c>0</c> and fading to <c>1</c>, each leg taking half the total duration.
    /// Uses callback-chain looping so delay applies only on the first cycle.
    /// Supports CanvasGroup, SpriteRenderer, Image, Text, and Renderer materials.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 0.4s (0.2s per leg) | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease controls fade-off; secondary ease controls fade-on.<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Invincibility blink, warning indicator, cursor blink, selection flash.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Blink").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BlinkPreset : CodePreset
    {
        public override string PresetName => "Blink";
        public override string Description => "Rapid alpha on/off loop";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var halfDur = GetDuration(duration, options) * 0.5f;
            var offEase = options.Ease ?? Ease.InOutQuad;
            var onEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutQuad;

            var offOptions = MergeWithDefaultEase(options.SetEase(offEase), offEase);
            var onOptions = MergeWithDefaultEase(options.SetEase(onEase), onEase);
            bool applyDelay = true;

            Tween tween = null;

            void FadeOff()
            {
                var fadeTween = CreateFadeTween(target, 0f, halfDur);
                if (fadeTween == null) return;
                tween = fadeTween
                    .SetEase(offEase)
                    .WithLoopDefaults(offOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(FadeOn);
            }

            void FadeOn()
            {
                var fadeTween = CreateFadeTween(target, 1f, halfDur);
                if (fadeTween == null) return;
                tween = fadeTween
                    .SetEase(onEase)
                    .WithLoopDefaults(onOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(FadeOff);
            }

            FadeOff();

            return tween;
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Slower blink that toggles alpha between fully opaque and fully transparent in a continuous loop.
    /// <para>
    /// Same callback-chain loop pattern as Blink but with a longer cycle time.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 0.8s (0.4s per leg) | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease controls fade-off; secondary ease controls fade-on.<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("BlinkSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BlinkSoftPreset : CodePreset
    {
        public override string PresetName => "BlinkSoft";
        public override string Description => "Slow alpha on/off loop";
        public override float DefaultDuration => 0.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var halfDur = GetDuration(duration, options) * 0.5f;
            var offEase = options.Ease ?? Ease.InOutQuad;
            var onEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutQuad;

            var offOptions = MergeWithDefaultEase(options.SetEase(offEase), offEase);
            var onOptions = MergeWithDefaultEase(options.SetEase(onEase), onEase);
            bool applyDelay = true;

            Tween tween = null;

            void FadeOff()
            {
                var fadeTween = CreateFadeTween(target, 0f, halfDur);
                if (fadeTween == null) return;
                tween = fadeTween
                    .SetEase(offEase)
                    .WithLoopDefaults(offOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(FadeOn);
            }

            void FadeOn()
            {
                var fadeTween = CreateFadeTween(target, 1f, halfDur);
                if (fadeTween == null) return;
                tween = fadeTween
                    .SetEase(onEase)
                    .WithLoopDefaults(onOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(FadeOff);
            }

            FadeOff();

            return tween;
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Rapid blink that toggles alpha between fully opaque and fully transparent in a continuous loop.
    /// <para>
    /// Same callback-chain loop pattern as Blink but with a shorter cycle time for a more urgent effect.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 0.2s (0.1s per leg) | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease controls fade-off; secondary ease controls fade-on.<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("BlinkHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BlinkHardPreset : CodePreset
    {
        public override string PresetName => "BlinkHard";
        public override string Description => "Rapid alpha on/off loop";
        public override float DefaultDuration => 0.2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var halfDur = GetDuration(duration, options) * 0.5f;
            var offEase = options.Ease ?? Ease.InOutQuad;
            var onEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutQuad;

            var offOptions = MergeWithDefaultEase(options.SetEase(offEase), offEase);
            var onOptions = MergeWithDefaultEase(options.SetEase(onEase), onEase);
            bool applyDelay = true;

            Tween tween = null;

            void FadeOff()
            {
                var fadeTween = CreateFadeTween(target, 0f, halfDur);
                if (fadeTween == null) return;
                tween = fadeTween
                    .SetEase(offEase)
                    .WithLoopDefaults(offOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(FadeOn);
            }

            void FadeOn()
            {
                var fadeTween = CreateFadeTween(target, 1f, halfDur);
                if (fadeTween == null) return;
                tween = fadeTween
                    .SetEase(onEase)
                    .WithLoopDefaults(onOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(FadeOff);
            }

            FadeOff();

            return tween;
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Smoothly pulses alpha between fully opaque and 30% opacity in a continuous loop.
    /// <para>
    /// Alternates between fading down to <c>0.3</c> and fading back to <c>1.0</c>, each leg taking
    /// half the total duration. Unlike Blink (which goes to 0), this maintains partial visibility
    /// throughout. Uses callback-chain looping; delay applies only on the first cycle.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s (1.0s per leg) | <b>Default ease:</b> InOutSine<br/>
    /// <b>Easing override:</b> Primary ease controls fade-down; secondary ease controls fade-up.<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Breathing glow, selection highlight pulse, ambient object shimmer, status indicator.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PulseFade").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PulseFadePreset : CodePreset
    {
        public override string PresetName => "PulseFade";
        public override string Description => "Smooth alpha pulse loop";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var halfDur = GetDuration(duration, options) * 0.5f;
            var fadeOutEase = options.Ease ?? Ease.InOutSine;
            var fadeInEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutSine;

            var outOptions = MergeWithDefaultEase(options.SetEase(fadeOutEase), fadeOutEase);
            var inOptions = MergeWithDefaultEase(options.SetEase(fadeInEase), fadeInEase);
            bool applyDelay = true;

            Tween tween = null;

            void FadeDown()
            {
                var fadeTween = CreateFadeTween(target, 0.3f, halfDur);
                if (fadeTween == null) return;
                tween = fadeTween
                    .SetEase(fadeOutEase)
                    .WithLoopDefaults(outOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(FadeUp);
            }

            void FadeUp()
            {
                var fadeTween = CreateFadeTween(target, 1f, halfDur);
                if (fadeTween == null) return;
                tween = fadeTween
                    .SetEase(fadeInEase)
                    .WithLoopDefaults(inOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(FadeDown);
            }

            FadeDown();

            return tween;
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Slow, gentle alpha pulse between fully opaque and 50% opacity in a continuous loop.
    /// <para>
    /// Same callback-chain loop pattern as PulseFade but with a longer cycle time and shallower
    /// fade depth for a more subtle breathing effect.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 3.0s (1.5s per leg) | <b>Default ease:</b> InOutSine<br/>
    /// <b>Easing override:</b> Primary ease controls fade-down; secondary ease controls fade-up.<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PulseFadeSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PulseFadeSoftPreset : CodePreset
    {
        public override string PresetName => "PulseFadeSoft";
        public override string Description => "Slow gentle alpha pulse loop";
        public override float DefaultDuration => 3.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var halfDur = GetDuration(duration, options) * 0.5f;
            var fadeOutEase = options.Ease ?? Ease.InOutSine;
            var fadeInEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutSine;

            var outOptions = MergeWithDefaultEase(options.SetEase(fadeOutEase), fadeOutEase);
            var inOptions = MergeWithDefaultEase(options.SetEase(fadeInEase), fadeInEase);
            bool applyDelay = true;

            Tween tween = null;

            void FadeDown()
            {
                var fadeTween = CreateFadeTween(target, 0.5f, halfDur);
                if (fadeTween == null) return;
                tween = fadeTween
                    .SetEase(fadeOutEase)
                    .WithLoopDefaults(outOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(FadeUp);
            }

            void FadeUp()
            {
                var fadeTween = CreateFadeTween(target, 1f, halfDur);
                if (fadeTween == null) return;
                tween = fadeTween
                    .SetEase(fadeInEase)
                    .WithLoopDefaults(inOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(FadeDown);
            }

            FadeDown();

            return tween;
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Fast, punchy alpha pulse between fully opaque and 10% opacity in a continuous loop.
    /// <para>
    /// Same callback-chain loop pattern as PulseFade but with a shorter cycle time and deeper
    /// fade depth for a more intense, urgent effect.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 1.0s (0.5s per leg) | <b>Default ease:</b> InOutSine<br/>
    /// <b>Easing override:</b> Primary ease controls fade-down; secondary ease controls fade-up.<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PulseFadeHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PulseFadeHardPreset : CodePreset
    {
        public override string PresetName => "PulseFadeHard";
        public override string Description => "Fast punchy alpha pulse loop";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var halfDur = GetDuration(duration, options) * 0.5f;
            var fadeOutEase = options.Ease ?? Ease.InOutSine;
            var fadeInEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutSine;

            var outOptions = MergeWithDefaultEase(options.SetEase(fadeOutEase), fadeOutEase);
            var inOptions = MergeWithDefaultEase(options.SetEase(fadeInEase), fadeInEase);
            bool applyDelay = true;

            Tween tween = null;

            void FadeDown()
            {
                var fadeTween = CreateFadeTween(target, 0.1f, halfDur);
                if (fadeTween == null) return;
                tween = fadeTween
                    .SetEase(fadeOutEase)
                    .WithLoopDefaults(outOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(FadeUp);
            }

            void FadeUp()
            {
                var fadeTween = CreateFadeTween(target, 1f, halfDur);
                if (fadeTween == null) return;
                tween = fadeTween
                    .SetEase(fadeInEase)
                    .WithLoopDefaults(inOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(FadeDown);
            }

            FadeDown();

            return tween;
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Produces randomized alpha flickering using Perlin noise, snapping to full opacity on completion.
    /// <para>
    /// Uses <c>DOVirtual.Float(0, 1, dur)</c> with a per-invocation random seed to sample
    /// <c>Mathf.PerlinNoise(seed + t * 12, 0)</c> each frame, directly setting the target's alpha.
    /// The <c>12x</c> time multiplier produces rapid, organic fluctuations. On complete, alpha is
    /// explicitly set to <c>1.0</c> to ensure a clean final state.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> Linear (progress)<br/>
    /// <b>Easing override:</b> Ease has minimal visible effect (controls progress, not noise).<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Torch/campfire flicker, electrical malfunction, glitch effect, dying light.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Flicker").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlickerPreset : CodePreset
    {
        public override string PresetName => "Flicker";
        public override string Description => "Randomized alpha flicker effect";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.Linear);
            float seed = Random.Range(0f, 100f);

            return DOVirtual.Float(0f, 1f, dur, t =>
                {
                    float noise = Mathf.PerlinNoise(seed + t * 12f, 0f);
                    SetAlpha(target, noise);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() => SetAlpha(target, 1f))
                .WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Slower, gentler alpha flicker using Perlin noise with half the oscillation frequency.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.5s | <b>Default ease:</b> Linear (progress)<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlickerSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlickerSoftPreset : CodePreset
    {
        public override string PresetName => "FlickerSoft";
        public override string Description => "Slow randomized alpha flicker";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.Linear);
            float seed = Random.Range(0f, 100f);

            return DOVirtual.Float(0f, 1f, dur, t =>
                {
                    float noise = Mathf.PerlinNoise(seed + t * 6f, 0f);
                    SetAlpha(target, noise);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() => SetAlpha(target, 1f))
                .WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Rapid, aggressive alpha flicker using Perlin noise with double the oscillation frequency.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> Linear (progress)<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlickerHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlickerHardPreset : CodePreset
    {
        public override string PresetName => "FlickerHard";
        public override string Description => "Rapid randomized alpha flicker";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.Linear);
            float seed = Random.Range(0f, 100f);

            return DOVirtual.Float(0f, 1f, dur, t =>
                {
                    float noise = Mathf.PerlinNoise(seed + t * 24f, 0f);
                    SetAlpha(target, noise);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() => SetAlpha(target, 1f))
                .WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Fades in from transparent to opaque in the first half, then fades back out to transparent in the second half.
    /// <para>
    /// Sets initial alpha to <c>0</c>, then builds a 2-step sequence: (1) fade to <c>1.0</c> over 35% duration,
    /// (2) fade to <c>0.0</c> over 65% duration. Returns null if the target lacks a fadeable component.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 2.0s | <b>Default ease:</b> InOutSine (both phases)<br/>
    /// <b>Easing override:</b> Primary ease controls fade-in; secondary ease controls fade-out.<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Temporary reveal, ghost appearance, flash highlight, transient notification.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FadeInOut").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FadeInOutPreset : CodePreset
    {
        public override string PresetName => "FadeInOut";
        public override string Description => "Fade in then fade out";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var dur = GetDuration(duration, options);
            var fadeInEase = ResolveEase(options, Ease.InOutSine);
            var fadeOutEase = ResolveSecondaryEase(options, Ease.InOutSine);
            var presetOptions = MergeWithDefaultEase(options, fadeInEase);

            SetAlpha(target, 0f);

            var fadeIn = CreateFadeTween(target, 1f, dur * 0.35f);
            var fadeOut = CreateFadeTween(target, 0f, dur * 0.65f);

            if (fadeIn == null || fadeOut == null) return null;

            return DOTween.Sequence()
                .Append(fadeIn.SetEase(fadeInEase))
                .Append(fadeOut.SetEase(fadeOutEase))
                .WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Internal factory for FadeInOut variants sharing the same fade-in/fade-out sequence structure.
    /// </summary>
    internal static class FadeInOutFactory
    {
        public static Tween Create(GameObject target, float fadeInRatio, float duration, Ease defaultEase, TweenOptions options)
        {
            var fadeInEase = options.Ease ?? defaultEase;
            var fadeOutEase = options.SecondaryEase ?? options.Ease ?? defaultEase;
            var presetOptions = options.Ease.HasValue ? options : options.SetEase(fadeInEase);

            CodePreset.SetAlphaStatic(target, 0f);

            var fadeIn = CodePreset.CreateFadeTweenStatic(target, 1f, duration * fadeInRatio);
            var fadeOut = CodePreset.CreateFadeTweenStatic(target, 0f, duration * (1f - fadeInRatio));

            if (fadeIn == null || fadeOut == null) return null;

            return DOTween.Sequence()
                .Append(fadeIn.SetEase(fadeInEase))
                .Append(fadeOut.SetEase(fadeOutEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Slow fade in then slow fade out — longer duration for a gentle, ambient appearance.
    /// <para>
    /// Sets initial alpha to <c>0</c>, fades in over 35% of duration, fades out over 65%.
    /// Uses <c>Ease.InOutSine</c> for smooth transitions.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 3.0s | <b>Default ease:</b> InOutSine<br/>
    /// <b>Easing override:</b> Primary ease controls fade-in; secondary ease controls fade-out.<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FadeInOutSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FadeInOutSoftPreset : CodePreset
    {
        public override string PresetName => "FadeInOutSoft";
        public override string Description => "Slow fade in then slow fade out";
        public override float DefaultDuration => 3.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FadeInOutFactory.Create(target, 0.35f, GetDuration(duration, options), Ease.InOutSine, options);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Quick fade in then quick fade out — shorter duration for a snappy flash appearance.
    /// <para>
    /// Sets initial alpha to <c>0</c>, fades in over 35% of duration, fades out over 65%.
    /// Uses <c>Ease.InOutSine</c> for smooth transitions.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutSine<br/>
    /// <b>Easing override:</b> Primary ease controls fade-in; secondary ease controls fade-out.<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FadeInOutHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FadeInOutHardPreset : CodePreset
    {
        public override string PresetName => "FadeInOutHard";
        public override string Description => "Quick fade in then quick fade out";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return FadeInOutFactory.Create(target, 0.35f, GetDuration(duration, options), Ease.InOutSine, options);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }
}
