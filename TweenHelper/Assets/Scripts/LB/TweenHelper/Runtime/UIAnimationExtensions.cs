using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Semantic UI animation helpers built on top of TweenBuilder and existing presets.
    /// </summary>
    public static class UIAnimationExtensions
    {
        public static TweenHandle UIHover(this Component component, float? duration = null, Color? hoverColor = null, TweenOptions options = default)
            => UIHover(component.gameObject, duration, hoverColor, options);

        public static TweenHandle UIHover(this GameObject target, float? duration = null, Color? hoverColor = null, TweenOptions options = default)
            => CreateHoverTween(target, duration ?? 0.16f, 1.08f, hoverColor, options);

        public static TweenHandle UIHoverSoft(this Component component, float? duration = null, Color? hoverColor = null, TweenOptions options = default)
            => UIHoverSoft(component.gameObject, duration, hoverColor, options);

        public static TweenHandle UIHoverSoft(this GameObject target, float? duration = null, Color? hoverColor = null, TweenOptions options = default)
            => CreateHoverTween(target, duration ?? 0.2f, 1.04f, hoverColor, options);

        public static TweenHandle UIPress(this Component component, float? duration = null, Color? pressedColor = null, TweenOptions options = default)
            => UIPress(component.gameObject, duration, pressedColor, options);

        public static TweenHandle UIPress(this GameObject target, float? duration = null, Color? pressedColor = null, TweenOptions options = default)
            => CreatePressTween(target, duration ?? 0.18f, 0.94f, pressedColor, options);

        public static TweenHandle UIPressHard(this Component component, float? duration = null, Color? pressedColor = null, TweenOptions options = default)
            => UIPressHard(component.gameObject, duration, pressedColor, options);

        public static TweenHandle UIPressHard(this GameObject target, float? duration = null, Color? pressedColor = null, TweenOptions options = default)
            => CreatePressTween(target, duration ?? 0.22f, 0.88f, pressedColor, options);

        public static TweenHandle UIAppear(this Component component, float? duration = null, TweenOptions options = default)
            => UIAppear(component.gameObject, duration, options);

        public static TweenHandle UIAppear(this GameObject target, float? duration = null, TweenOptions options = default)
            => new TweenBuilder(target).WithOptions(options).PopInFade(duration ?? 0.32f).Play();

        public static TweenHandle UIAppearSoft(this Component component, float? duration = null, TweenOptions options = default)
            => UIAppearSoft(component.gameObject, duration, options);

        public static TweenHandle UIAppearSoft(this GameObject target, float? duration = null, TweenOptions options = default)
            => new TweenBuilder(target).WithOptions(options).PopInFadeSoft(duration ?? 0.4f).Play();

        public static TweenHandle UIDisappear(this Component component, float? duration = null, TweenOptions options = default)
            => UIDisappear(component.gameObject, duration, options);

        public static TweenHandle UIDisappear(this GameObject target, float? duration = null, TweenOptions options = default)
            => new TweenBuilder(target).WithOptions(options).PopOutFade(duration ?? 0.26f).Play();

        public static TweenHandle UIDisappearSoft(this Component component, float? duration = null, TweenOptions options = default)
            => UIDisappearSoft(component.gameObject, duration, options);

        public static TweenHandle UIDisappearSoft(this GameObject target, float? duration = null, TweenOptions options = default)
            => new TweenBuilder(target).WithOptions(options).PopOutFadeSoft(duration ?? 0.32f).Play();

        public static TweenHandle UIAttention(this Component component, float? duration = null, TweenOptions options = default)
            => UIAttention(component.gameObject, duration, options);

        public static TweenHandle UIAttention(this GameObject target, float? duration = null, TweenOptions options = default)
            => new TweenBuilder(target).WithOptions(options).Attention(duration ?? 0.42f).Play();

        public static TweenHandle UIAttentionSoft(this Component component, float? duration = null, TweenOptions options = default)
            => UIAttentionSoft(component.gameObject, duration, options);

        public static TweenHandle UIAttentionSoft(this GameObject target, float? duration = null, TweenOptions options = default)
            => new TweenBuilder(target).WithOptions(options).AttentionSoft(duration ?? 0.5f).Play();

        public static TweenHandle UIAttentionHard(this Component component, float? duration = null, TweenOptions options = default)
            => UIAttentionHard(component.gameObject, duration, options);

        public static TweenHandle UIAttentionHard(this GameObject target, float? duration = null, TweenOptions options = default)
            => new TweenBuilder(target).WithOptions(options).AttentionHard(duration ?? 0.36f).Play();

        public static TweenHandle UIDisabled(this Component component, float? duration = null, Color? disabledColor = null, TweenOptions options = default)
            => UIDisabled(component.gameObject, duration, disabledColor, options);

        public static TweenHandle UIDisabled(this GameObject target, float? duration = null, Color? disabledColor = null, TweenOptions options = default)
        {
            var cache = UIAnimationStateCache.GetOrCreate(target);
            var resolvedDisabledColor = disabledColor ?? ResolveDisabledColor(cache);
            var builder = new TweenBuilder(target)
                .WithOptions(options)
                .Scale(cache.BaseScale, duration ?? 0.22f);

            if (cache.HasColor)
            {
                builder.With().WithOptions(options).Color(resolvedDisabledColor, duration ?? 0.22f);
            }

            if (cache.HasCanvasGroup)
            {
                builder.With().WithOptions(options).Fade(Mathf.Min(cache.BaseCanvasAlpha, 0.55f), duration ?? 0.22f);
            }

            return builder.Play();
        }

        public static TweenHandle UIEnabled(this Component component, float? duration = null, TweenOptions options = default)
            => UIEnabled(component.gameObject, duration, options);

        public static TweenHandle UIEnabled(this GameObject target, float? duration = null, TweenOptions options = default)
        {
            var cache = UIAnimationStateCache.GetOrCreate(target);
            var builder = new TweenBuilder(target)
                .WithOptions(options)
                .Scale(cache.BaseScale, duration ?? 0.22f);

            if (cache.HasColor)
            {
                builder.With().WithOptions(options).Color(cache.BaseColor, duration ?? 0.22f);
            }

            if (cache.HasCanvasGroup)
            {
                builder.With().WithOptions(options).Fade(cache.BaseCanvasAlpha, duration ?? 0.22f);
            }

            return builder.Play();
        }

        private static TweenHandle CreateHoverTween(GameObject target, float duration, float scaleMultiplier, Color? hoverColor, TweenOptions options)
        {
            var cache = UIAnimationStateCache.GetOrCreate(target);
            var builder = new TweenBuilder(target)
                .WithOptions(options)
                .Scale(cache.BaseScale * scaleMultiplier, duration);

            if (cache.HasColor)
            {
                builder.With().WithOptions(options).Color(hoverColor ?? ResolveHoverColor(cache.BaseColor), duration);
            }

            return builder.Play();
        }

        private static TweenHandle CreatePressTween(GameObject target, float duration, float pressedScaleMultiplier, Color? pressedColor, TweenOptions options)
        {
            var cache = UIAnimationStateCache.GetOrCreate(target);
            var resolvedPressedColor = pressedColor ?? ResolvePressedColor(cache);
            var pressDuration = duration * 0.35f;
            var releaseDuration = duration * 0.65f;
            var builder = new TweenBuilder(target)
                .WithOptions(options)
                .Scale(cache.BaseScale * pressedScaleMultiplier, pressDuration);

            if (cache.HasColor)
            {
                builder.With().WithOptions(options).Color(resolvedPressedColor, pressDuration);
            }

            if (cache.HasCanvasGroup)
            {
                builder.With().WithOptions(options).Fade(Mathf.Clamp01(cache.BaseCanvasAlpha * 0.92f), pressDuration);
            }

            builder.Then().WithOptions(options).Scale(cache.BaseScale, releaseDuration);

            if (cache.HasColor)
            {
                builder.With().WithOptions(options).Color(cache.BaseColor, releaseDuration);
            }

            if (cache.HasCanvasGroup)
            {
                builder.With().WithOptions(options).Fade(cache.BaseCanvasAlpha, releaseDuration);
            }

            return builder.Play();
        }

        private static Color ResolveHoverColor(Color baseColor)
        {
            var hoverColor = Color.Lerp(baseColor, Color.white, 0.18f);
            hoverColor.a = baseColor.a;
            return hoverColor;
        }

        private static Color ResolvePressedColor(UIAnimationStateCache cache)
        {
            if (!cache.HasColor)
            {
                return Color.white;
            }

            var pressedColor = Color.Lerp(cache.BaseColor, Color.black, 0.16f);
            pressedColor.a = cache.BaseColor.a;
            return pressedColor;
        }

        private static Color ResolveDisabledColor(UIAnimationStateCache cache)
        {
            if (!cache.HasColor)
            {
                return new Color(1f, 1f, 1f, 0.55f);
            }

            var grayscale = cache.BaseColor.grayscale;
            return new Color(grayscale, grayscale, grayscale, cache.BaseColor.a * 0.55f);
        }
    }
}
