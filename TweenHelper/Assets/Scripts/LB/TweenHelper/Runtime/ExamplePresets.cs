using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper
{
    /// <summary>
    /// Example animation presets demonstrating how to create reusable animations.
    /// These are auto-registered at runtime via [AutoRegisterPreset] attribute.
    /// </summary>

    #region Scale Animations

    /// <summary>
    /// Scales from 0 to original scale with overshoot.
    /// Usage: transform.Tween().Preset("PopIn").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class PopInPreset : CodePreset
    {
        public override string PresetName => "PopIn";
        public override string Description => "Scales from 0 to 1 with overshoot";
        public override float DefaultDuration => 0.4f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;

            return t.DOScale(originalScale, GetDuration(duration))
                .SetEase(Ease.OutBack)
                .WithDefaults(MergeWithDefaultEase(options, Ease.OutBack), target);
        }
    }

    /// <summary>
    /// Scales to 0 with anticipation.
    /// Usage: transform.Tween().Preset("PopOut").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class PopOutPreset : CodePreset
    {
        public override string PresetName => "PopOut";
        public override string Description => "Scales to 0 with anticipation";
        public override float DefaultDuration => 0.3f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOScale(Vector3.zero, GetDuration(duration))
                .SetEase(Ease.InBack)
                .WithDefaults(MergeWithDefaultEase(options, Ease.InBack), target);
        }
    }

    /// <summary>
    /// Quick scale punch for feedback.
    /// Usage: transform.Tween().Preset("Punch").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class PunchPreset : CodePreset
    {
        public override string PresetName => "Punch";
        public override string Description => "Quick scale punch for button feedback";
        public override float DefaultDuration => 0.2f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOPunchScale(Vector3.one * 0.15f, GetDuration(duration), 6, 0.7f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Bouncy scale animation.
    /// Usage: transform.Tween().Preset("Bounce").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class BouncePreset : CodePreset
    {
        public override string PresetName => "Bounce";
        public override string Description => "Bouncy scale up and back";
        public override float DefaultDuration => 0.5f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var dur = GetDuration(duration);

            return DOTween.Sequence()
                .Append(t.DOScale(originalScale * 1.2f, dur * 0.4f).SetEase(Ease.OutQuad))
                .Append(t.DOScale(originalScale, dur * 0.6f).SetEase(Ease.OutBounce))
                .WithDefaults(options, target);
        }
    }

    #endregion

    #region Position Animations

    /// <summary>
    /// Shakes the position randomly.
    /// Usage: transform.Tween().Preset("Shake").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class ShakePreset : CodePreset
    {
        public override string PresetName => "Shake";
        public override string Description => "Random position shake";
        public override float DefaultDuration => 0.5f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOShakePosition(GetDuration(duration), 0.3f, 15, 90f, false, true)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Slides in from the left.
    /// Usage: transform.Tween().Preset("SlideInLeft").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInLeftPreset : CodePreset
    {
        public override string PresetName => "SlideInLeft";
        public override string Description => "Slides in from the left";
        public override float DefaultDuration => 0.4f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var targetPos = t.localPosition;
            t.localPosition = targetPos + Vector3.left * 500f;

            return t.DOLocalMove(targetPos, GetDuration(duration))
                .SetEase(Ease.OutCubic)
                .WithDefaults(MergeWithDefaultEase(options, Ease.OutCubic), target);
        }
    }

    /// <summary>
    /// Slides in from the right.
    /// Usage: transform.Tween().Preset("SlideInRight").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInRightPreset : CodePreset
    {
        public override string PresetName => "SlideInRight";
        public override string Description => "Slides in from the right";
        public override float DefaultDuration => 0.4f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var targetPos = t.localPosition;
            t.localPosition = targetPos + Vector3.right * 500f;

            return t.DOLocalMove(targetPos, GetDuration(duration))
                .SetEase(Ease.OutCubic)
                .WithDefaults(MergeWithDefaultEase(options, Ease.OutCubic), target);
        }
    }

    /// <summary>
    /// Slides in from below.
    /// Usage: transform.Tween().Preset("SlideInUp").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInUpPreset : CodePreset
    {
        public override string PresetName => "SlideInUp";
        public override string Description => "Slides in from below";
        public override float DefaultDuration => 0.4f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var targetPos = t.localPosition;
            t.localPosition = targetPos + Vector3.down * 500f;

            return t.DOLocalMove(targetPos, GetDuration(duration))
                .SetEase(Ease.OutCubic)
                .WithDefaults(MergeWithDefaultEase(options, Ease.OutCubic), target);
        }
    }

    #endregion

    #region Fade Animations

    /// <summary>
    /// Fades in from transparent.
    /// Usage: transform.Tween().Preset("FadeIn").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class FadeInPreset : CodePreset
    {
        public override string PresetName => "FadeIn";
        public override string Description => "Fades in from transparent (requires transparent material)";
        public override float DefaultDuration => 3f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            SetAlpha(target, 0f);
            var tween = CreateFadeTween(target, 1f, GetDuration(duration));
            return tween?.WithDefaults(options, target);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Fades out to transparent.
    /// Usage: transform.Tween().Preset("FadeOut").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class FadeOutPreset : CodePreset
    {
        public override string PresetName => "FadeOut";
        public override string Description => "Fades out to transparent (requires transparent material)";
        public override float DefaultDuration => 3f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var tween = CreateFadeTween(target, 0f, GetDuration(duration));
            return tween?.WithDefaults(options, target);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    #endregion

    #region Rotation Animations

    /// <summary>
    /// Spins 360 degrees on X axis.
    /// Usage: transform.Tween().Preset("SpinX").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class SpinXPreset : CodePreset
    {
        public override string PresetName => "SpinX";
        public override string Description => "Spins 360 degrees on X axis";
        public override float DefaultDuration => 0.6f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DORotate(new Vector3(360f, 0, 0), GetDuration(duration), RotateMode.FastBeyond360)
                .SetEase(Ease.InOutQuad)
                .WithDefaults(MergeWithDefaultEase(options, Ease.InOutQuad), target);
        }
    }

    /// <summary>
    /// Spins 360 degrees on Y axis.
    /// Usage: transform.Tween().Preset("SpinY").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class SpinYPreset : CodePreset
    {
        public override string PresetName => "SpinY";
        public override string Description => "Spins 360 degrees on Y axis";
        public override float DefaultDuration => 0.6f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DORotate(new Vector3(0, 360f, 0), GetDuration(duration), RotateMode.FastBeyond360)
                .SetEase(Ease.InOutQuad)
                .WithDefaults(MergeWithDefaultEase(options, Ease.InOutQuad), target);
        }
    }

    /// <summary>
    /// Spins 360 degrees on Z axis.
    /// Usage: transform.Tween().Preset("SpinZ").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class SpinZPreset : CodePreset
    {
        public override string PresetName => "SpinZ";
        public override string Description => "Spins 360 degrees on Z axis";
        public override float DefaultDuration => 0.6f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DORotate(new Vector3(0, 0, 360f), GetDuration(duration), RotateMode.FastBeyond360)
                .SetEase(Ease.InOutQuad)
                .WithDefaults(MergeWithDefaultEase(options, Ease.InOutQuad), target);
        }
    }

    /// <summary>
    /// Wobbles rotation on X axis.
    /// Usage: transform.Tween().Preset("WobbleX").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleXPreset : CodePreset
    {
        public override string PresetName => "WobbleX";
        public override string Description => "Wobbles rotation on X axis";
        public override float DefaultDuration => 0.5f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOPunchRotation(new Vector3(15f, 0, 0), GetDuration(duration), 8, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Wobbles rotation on Y axis.
    /// Usage: transform.Tween().Preset("WobbleY").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleYPreset : CodePreset
    {
        public override string PresetName => "WobbleY";
        public override string Description => "Wobbles rotation on Y axis";
        public override float DefaultDuration => 0.5f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOPunchRotation(new Vector3(0, 15f, 0), GetDuration(duration), 8, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Wobbles rotation on Z axis.
    /// Usage: transform.Tween().Preset("WobbleZ").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleZPreset : CodePreset
    {
        public override string PresetName => "WobbleZ";
        public override string Description => "Wobbles rotation on Z axis";
        public override float DefaultDuration => 0.5f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOPunchRotation(new Vector3(0, 0, 15f), GetDuration(duration), 8, 0.5f)
                .WithDefaults(options, target);
        }
    }

    #endregion

    #region Combined Animations

    /// <summary>
    /// Pops in with fade (scale + fade together).
    /// Usage: transform.Tween().Preset("PopInFade").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class PopInFadePreset : CodePreset
    {
        public override string PresetName => "PopInFade";
        public override string Description => "Scales and fades in together";
        public override float DefaultDuration => 2f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;

            var dur = GetDuration(duration);
            var seq = DOTween.Sequence();

            seq.Append(t.DOScale(originalScale, dur).SetEase(Ease.OutCubic));

            var fadeTween = CreateFadeTween(target, 1f, dur);
            if (fadeTween != null)
            {
                SetAlpha(target, 0f);
                seq.Join(fadeTween);
            }

            return seq.WithDefaults(options, target); 
        }

        public override bool CanApplyTo(GameObject target)
        {
            return target != null;
        }
    }

    /// <summary>
    /// Attention-grabbing pulse animation.
    /// Usage: transform.Tween().Preset("Attention").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class AttentionPreset : CodePreset
    {
        public override string PresetName => "Attention";
        public override string Description => "Attention-grabbing pulse";
        public override float DefaultDuration => 0.8f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var dur = GetDuration(duration);

            return DOTween.Sequence()
                .Append(t.DOScale(originalScale * 1.1f, dur * 0.15f))
                .Append(t.DOScale(originalScale * 0.95f, dur * 0.15f))
                .Append(t.DOScale(originalScale * 1.05f, dur * 0.15f))
                .Append(t.DOScale(originalScale, dur * 0.15f))
                .SetLoops(2)
                .WithDefaults(options, target);
        }
    }

    #endregion

    #region Helper Methods

    public abstract partial class CodePreset
    {
        /// <summary>
        /// Creates a fade tween for the appropriate component type.
        /// Supports CanvasGroup, SpriteRenderer, Image, Text, and Renderer (material).
        /// </summary>
        protected static Tween CreateFadeTween(GameObject target, float alpha, float duration)
        {
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
                return canvasGroup.DOFade(alpha, duration);

            var spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                return spriteRenderer.DOFade(alpha, duration);

            var image = target.GetComponent<Image>();
            if (image != null)
                return image.DOFade(alpha, duration);

            var text = target.GetComponent<Text>();
            if (text != null)
                return text.DOFade(alpha, duration);

            // Fallback to Renderer material fade
            var renderer = target.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
                return renderer.material.DOFade(alpha, duration);

            return null;
        }

        /// <summary>
        /// Sets the alpha for the appropriate component type.
        /// Supports CanvasGroup, SpriteRenderer, Image, Text, and Renderer (material).
        /// </summary>
        protected static void SetAlpha(GameObject target, float alpha)
        {
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
                return;
            }

            var spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                var c = spriteRenderer.color;
                c.a = alpha;
                spriteRenderer.color = c;
                return;
            }

            var image = target.GetComponent<Image>();
            if (image != null)
            {
                var c = image.color;
                c.a = alpha;
                image.color = c;
                return;
            }

            var text = target.GetComponent<Text>();
            if (text != null)
            {
                var c = text.color;
                c.a = alpha;
                text.color = c;
                return;
            }

            // Fallback to Renderer material
            var renderer = target.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                var c = renderer.material.color;
                c.a = alpha;
                renderer.material.color = c;
            }
        }

        /// <summary>
        /// Checks if the target has a component that supports fading.
        /// </summary>
        protected static bool CanFade(GameObject target)
        {
            if (target == null) return false;

            return target.GetComponent<CanvasGroup>() != null ||
                   target.GetComponent<SpriteRenderer>() != null ||
                   target.GetComponent<Image>() != null ||
                   target.GetComponent<Text>() != null ||
                   (target.GetComponent<Renderer>()?.material != null);
        }
    }

    #endregion
}
