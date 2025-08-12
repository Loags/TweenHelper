using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper
{
    /// <summary>
    /// Built-in preset for "pop in" animation (scale from zero with back ease).
    /// </summary>
    internal class PopInPreset : ITweenPreset
    {
        public string PresetName => "PopIn";
        public string Description => "Scales from zero to original size with a bouncy ease";
        public float DefaultDuration => 0.5f;
        
        public Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var transform = target.transform;
            var originalScale = transform.localScale;
            transform.localScale = Vector3.zero;
            
            var finalOptions = options.SetEase(options.Ease ?? Ease.OutBack);
            return TweenHelper.ScaleTo(transform, originalScale, duration ?? DefaultDuration, finalOptions);
        }
        
        public bool CanApplyTo(GameObject target)
        {
            return target != null && target.transform != null;
        }
    }
    
    /// <summary>
    /// Built-in preset for "pop out" animation (scale to zero with back ease).
    /// </summary>
    internal class PopOutPreset : ITweenPreset
    {
        public string PresetName => "PopOut";
        public string Description => "Scales to zero with a sharp ease";
        public float DefaultDuration => 0.3f;
        
        public Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var finalOptions = options.SetEase(options.Ease ?? Ease.InBack);
            return TweenHelper.ScaleTo(target.transform, Vector3.zero, duration ?? DefaultDuration, finalOptions);
        }
        
        public bool CanApplyTo(GameObject target)
        {
            return target != null && target.transform != null;
        }
    }
    
    /// <summary>
    /// Built-in preset for bounce animation.
    /// </summary>
    internal class BouncePreset : ITweenPreset
    {
        public string PresetName => "Bounce";
        public string Description => "Bounces by scaling up slightly then back to original";
        public float DefaultDuration => 0.6f;
        
        public Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return TweenHelper.Bounce(target.transform, 1.1f, duration ?? DefaultDuration, options);
        }
        
        public bool CanApplyTo(GameObject target)
        {
            return target != null && target.transform != null;
        }
    }
    
    /// <summary>
    /// Built-in preset for shake animation.
    /// </summary>
    internal class ShakePreset : ITweenPreset
    {
        public string PresetName => "Shake";
        public string Description => "Shakes the object by moving it randomly";
        public float DefaultDuration => 0.5f;
        
        public Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return TweenHelper.Shake(target.transform, 1f, duration ?? DefaultDuration, 10, options);
        }
        
        public bool CanApplyTo(GameObject target)
        {
            return target != null && target.transform != null;
        }
    }
    
    /// <summary>
    /// Built-in preset for fade in animation.
    /// </summary>
    internal class FadeInPreset : ITweenPreset
    {
        public string PresetName => "FadeIn";
        public string Description => "Fades alpha from 0 to 1";
        public float DefaultDuration => 0.5f;
        
        public Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            // Try different fade targets in order of preference
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                return TweenHelper.FadeIn(canvasGroup, duration ?? DefaultDuration, options);
            }
            
            var spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                var color = spriteRenderer.color;
                color.a = 0f;
                spriteRenderer.color = color;
                return TweenHelper.FadeTo(spriteRenderer, 1f, duration ?? DefaultDuration, options);
            }
            
            var image = target.GetComponent<Image>();
            if (image != null)
            {
                var color = image.color;
                color.a = 0f;
                image.color = color;
                return TweenHelper.FadeTo(image, 1f, duration ?? DefaultDuration, options);
            }
            
            var text = target.GetComponent<Text>();
            if (text != null)
            {
                var color = text.color;
                color.a = 0f;
                text.color = color;
                return TweenHelper.FadeTo(text, 1f, duration ?? DefaultDuration, options);
            }
            
            Debug.LogWarning($"FadeInPreset: No fadeable component found on '{target.name}'. Requires CanvasGroup, SpriteRenderer, Image, or Text.");
            return null;
        }
        
        public bool CanApplyTo(GameObject target)
        {
            if (target == null) return false;
            
            return target.GetComponent<CanvasGroup>() != null ||
                   target.GetComponent<SpriteRenderer>() != null ||
                   target.GetComponent<Image>() != null ||
                   target.GetComponent<Text>() != null;
        }
    }
    
    /// <summary>
    /// Built-in preset for fade out animation.
    /// </summary>
    internal class FadeOutPreset : ITweenPreset
    {
        public string PresetName => "FadeOut";
        public string Description => "Fades alpha from current value to 0";
        public float DefaultDuration => 0.5f;
        
        public Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            // Try different fade targets in order of preference
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                return TweenHelper.FadeOut(canvasGroup, duration ?? DefaultDuration, options);
            }
            
            var spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                return TweenHelper.FadeTo(spriteRenderer, 0f, duration ?? DefaultDuration, options);
            }
            
            var image = target.GetComponent<Image>();
            if (image != null)
            {
                return TweenHelper.FadeTo(image, 0f, duration ?? DefaultDuration, options);
            }
            
            var text = target.GetComponent<Text>();
            if (text != null)
            {
                return TweenHelper.FadeTo(text, 0f, duration ?? DefaultDuration, options);
            }
            
            Debug.LogWarning($"FadeOutPreset: No fadeable component found on '{target.name}'. Requires CanvasGroup, SpriteRenderer, Image, or Text.");
            return null;
        }
        
        public bool CanApplyTo(GameObject target)
        {
            if (target == null) return false;
            
            return target.GetComponent<CanvasGroup>() != null ||
                   target.GetComponent<SpriteRenderer>() != null ||
                   target.GetComponent<Image>() != null ||
                   target.GetComponent<Text>() != null;
        }
    }
}
