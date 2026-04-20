using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper
{
    /// <summary>
    /// Shared helpers for component detection and tween creation across world and UI targets.
    /// </summary>
    internal static class TweenTargetUtility
    {
        public static Tween CreateFadeTween(GameObject target, float alpha, float duration)
        {
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                return canvasGroup.DOFade(alpha, duration);
            }

            var spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                return spriteRenderer.DOFade(alpha, duration);
            }

            var graphic = target.GetComponent<Graphic>();
            if (graphic != null)
            {
                return graphic.DOFade(alpha, duration);
            }

            var tmpText = target.GetComponent<TMP_Text>();
            if (tmpText != null)
            {
                return tmpText.DOFade(alpha, duration);
            }

            var renderer = target.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                return renderer.material.DOFade(alpha, duration);
            }

            return null;
        }

        public static void SetAlpha(GameObject target, float alpha)
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
                var color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
                return;
            }

            var graphic = target.GetComponent<Graphic>();
            if (graphic != null)
            {
                var color = graphic.color;
                color.a = alpha;
                graphic.color = color;
                return;
            }

            var tmpText = target.GetComponent<TMP_Text>();
            if (tmpText != null)
            {
                var color = tmpText.color;
                color.a = alpha;
                tmpText.color = color;
                return;
            }

            var renderer = target.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                var color = renderer.material.color;
                color.a = alpha;
                renderer.material.color = color;
            }
        }

        public static bool CanFade(GameObject target)
        {
            if (target == null)
            {
                return false;
            }

            return target.GetComponent<CanvasGroup>() != null ||
                   target.GetComponent<SpriteRenderer>() != null ||
                   target.GetComponent<Graphic>() != null ||
                   target.GetComponent<TMP_Text>() != null ||
                   (target.GetComponent<Renderer>()?.material != null);
        }

        public static Tween CreateColorTween(GameObject target, Color color, float duration)
        {
            var spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                return spriteRenderer.DOColor(color, duration);
            }

            var graphic = target.GetComponent<Graphic>();
            if (graphic != null)
            {
                return graphic.DOColor(color, duration);
            }

            var tmpText = target.GetComponent<TMP_Text>();
            if (tmpText != null)
            {
                return tmpText.DOColor(color, duration);
            }

            return null;
        }

        public static bool TryGetColor(GameObject target, out Color color)
        {
            var spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                color = spriteRenderer.color;
                return true;
            }

            var graphic = target.GetComponent<Graphic>();
            if (graphic != null)
            {
                color = graphic.color;
                return true;
            }

            var tmpText = target.GetComponent<TMP_Text>();
            if (tmpText != null)
            {
                color = tmpText.color;
                return true;
            }

            color = default;
            return false;
        }

        public static bool TrySetColor(GameObject target, Color color)
        {
            var spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
                return true;
            }

            var graphic = target.GetComponent<Graphic>();
            if (graphic != null)
            {
                graphic.color = color;
                return true;
            }

            var tmpText = target.GetComponent<TMP_Text>();
            if (tmpText != null)
            {
                tmpText.color = color;
                return true;
            }

            return false;
        }

        public static Tween CreateLocalMoveTween(GameObject target, Vector3 localTarget, float duration)
        {
            if (TryGetRectTransform(target, out var rectTransform))
            {
                return rectTransform.DOAnchorPos(ToAnchored(localTarget), duration);
            }

            return target.transform.DOLocalMove(localTarget, duration);
        }

        public static Tween CreateRelativeLocalMoveTween(GameObject target, Vector3 localOffset, float duration)
        {
            if (TryGetRectTransform(target, out var rectTransform))
            {
                return rectTransform.DOAnchorPos(rectTransform.anchoredPosition + ToAnchored(localOffset), duration);
            }

            return target.transform.DOLocalMove(localOffset, duration).SetRelative(true);
        }

        public static Tween CreateRelativeMoveTween(GameObject target, Vector3 offset, float duration)
        {
            if (TryGetRectTransform(target, out var rectTransform))
            {
                return rectTransform.DOAnchorPos(rectTransform.anchoredPosition + ToAnchored(offset), duration);
            }

            return target.transform.DOMove(target.transform.position + offset, duration);
        }

        public static Tween CreateMoveXTween(GameObject target, float x, float duration)
        {
            if (TryGetRectTransform(target, out var rectTransform))
            {
                return rectTransform.DOAnchorPosX(x, duration);
            }

            return target.transform.DOMoveX(x, duration);
        }

        public static Tween CreateMoveYTween(GameObject target, float y, float duration)
        {
            if (TryGetRectTransform(target, out var rectTransform))
            {
                return rectTransform.DOAnchorPosY(y, duration);
            }

            return target.transform.DOMoveY(y, duration);
        }

        public static bool TryGetRectTransform(GameObject target, out RectTransform rectTransform)
        {
            rectTransform = target.GetComponent<RectTransform>();
            return rectTransform != null;
        }

        private static Vector2 ToAnchored(Vector3 value)
        {
            return new Vector2(value.x, value.y);
        }
    }
}
