using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Fades the target in from fully transparent (alpha 0) to fully opaque (alpha 1).
    /// <para>
    /// Initializes alpha to <c>0</c> at the start, then creates a fade tween to <c>1.0</c>.
    /// Uses InQuad ease by default for a slow start, accelerating toward full visibility.
    /// Supports CanvasGroup, SpriteRenderer, Image, Text, and Renderer materials.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entry | <b>Default duration:</b> 3.0s | <b>Default ease:</b> InQuad<br/>
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
            var tween = CreateFadeTween(target, 1f, GetDuration(duration));
            // Slow start to avoid appearing fully visible too early
            var presetOptions = MergeWithDefaultEase(options, Ease.InQuad);
            var ease = ResolveEase(presetOptions, Ease.InQuad);
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
            var tween = CreateFadeTween(target, 0f, GetDuration(duration));
            return tween?.WithDefaults(options, target);
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
            var halfDur = GetDuration(duration) * 0.5f;
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
            var halfDur = GetDuration(duration) * 0.5f;
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
            var dur = GetDuration(duration);
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
    /// Fades in from transparent to opaque in the first half, then fades back out to transparent in the second half.
    /// <para>
    /// Sets initial alpha to <c>0</c>, then builds a 2-step sequence: (1) fade to <c>1.0</c> over 50% duration,
    /// (2) fade to <c>0.0</c> over 50% duration. Returns null if the target lacks a fadeable component.
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
            var dur = GetDuration(duration);
            var fadeInEase = ResolveEase(options, Ease.InOutSine);
            var fadeOutEase = ResolveSecondaryEase(options, Ease.InOutSine);
            var presetOptions = MergeWithDefaultEase(options, fadeInEase);

            SetAlpha(target, 0f);

            var fadeIn = CreateFadeTween(target, 1f, dur * 0.5f);
            var fadeOut = CreateFadeTween(target, 0f, dur * 0.5f);

            if (fadeIn == null || fadeOut == null) return null;

            return DOTween.Sequence()
                .Append(fadeIn.SetEase(fadeInEase))
                .Append(fadeOut.SetEase(fadeOutEase))
                .WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }
}
