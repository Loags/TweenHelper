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
        private static readonly int BaseColorPropertyId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorPropertyId = Shader.PropertyToID("_Color");

        public static Tween CreateFadeTween(GameObject target, float alpha, float duration)
        {
            return TryGetAlphaBinding(target, out var binding) ? binding.CreateTween(alpha, duration) : null;
        }

        public static void SetAlpha(GameObject target, float alpha)
        {
            if (TryGetAlphaBinding(target, out var binding)) binding.SetAlpha(alpha);
        }

        public static bool CanFade(GameObject target)
        {
            if (target == null)
            {
                return false;
            }

            if (target.GetComponent<CanvasGroup>() != null ||
                target.GetComponent<SpriteRenderer>() != null ||
                target.GetComponent<Graphic>() != null ||
                target.GetComponent<TMP_Text>() != null)
            {
                return true;
            }

            var renderer = target.GetComponent<Renderer>();
            return TryGetRendererColorProperty(renderer, out _);
        }

        internal static bool TryGetAlphaBinding(GameObject target, out TweenAlphaBinding binding)
        {
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                binding = new TweenAlphaBinding(canvasGroup);
                return true;
            }

            var spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                binding = new TweenAlphaBinding(spriteRenderer);
                return true;
            }

            var graphic = target.GetComponent<Graphic>();
            if (graphic != null)
            {
                binding = new TweenAlphaBinding(graphic);
                return true;
            }

            var tmpText = target.GetComponent<TMP_Text>();
            if (tmpText != null)
            {
                binding = new TweenAlphaBinding(tmpText);
                return true;
            }

            var renderer = target.GetComponent<Renderer>();
            if (TryGetRendererColorProperty(renderer, out int colorPropertyId))
            {
                binding = new TweenAlphaBinding(new RendererColorBinding(renderer, colorPropertyId));
                return true;
            }

            binding = default;
            return false;
        }

        public static Tween CreateColorTween(GameObject target, Color color, float duration)
        {
            return TryGetColorBinding(target, out var binding) ? binding.CreateTween(color, duration) : null;
        }

        public static bool TryGetColor(GameObject target, out Color color)
        {
            if (TryGetColorBinding(target, out var binding))
            {
                color = binding.GetColor();
                return true;
            }

            color = default;
            return false;
        }

        public static bool TrySetColor(GameObject target, Color color)
        {
            if (!TryGetColorBinding(target, out var binding)) return false;
            binding.SetColor(color);
            return true;
        }

        internal static bool TryGetColorBinding(GameObject target, out TweenColorBinding binding)
        {
            var spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                binding = new TweenColorBinding(spriteRenderer);
                return true;
            }

            var graphic = target.GetComponent<Graphic>();
            if (graphic != null)
            {
                binding = new TweenColorBinding(graphic);
                return true;
            }

            var tmpText = target.GetComponent<TMP_Text>();
            if (tmpText != null)
            {
                binding = new TweenColorBinding(tmpText);
                return true;
            }

            var renderer = target.GetComponent<Renderer>();
            if (TryGetRendererColorProperty(renderer, out int colorPropertyId))
            {
                binding = new TweenColorBinding(new RendererColorBinding(renderer, colorPropertyId));
                return true;
            }

            binding = default;
            return false;
        }

        public static Tween CreateLocalMoveTween(GameObject target, Vector3 localTarget, float duration, bool snapping = false)
        {
            if (TryGetRectTransform(target, out var rectTransform))
            {
                return rectTransform.DOAnchorPos(ToAnchored(localTarget), duration, snapping);
            }

            return target.transform.DOLocalMove(localTarget, duration, snapping);
        }

        public static Tween CreateRelativeLocalMoveTween(GameObject target, Vector3 localOffset, float duration, bool snapping = false)
        {
            if (TryGetRectTransform(target, out var rectTransform))
            {
                return rectTransform.DOAnchorPos(ToAnchored(localOffset), duration, snapping).SetRelative(true);
            }

            return target.transform.DOLocalMove(localOffset, duration, snapping).SetRelative(true);
        }

        public static Tween CreateRelativeMoveTween(GameObject target, Vector3 offset, float duration, bool snapping = false)
        {
            if (TryGetRectTransform(target, out var rectTransform))
            {
                return rectTransform.DOAnchorPos(ToAnchored(offset), duration, snapping).SetRelative(true);
            }

            return target.transform.DOMove(target.transform.position + offset, duration, snapping);
        }

        public static Tween CreateMoveXTween(GameObject target, float x, float duration, bool snapping = false)
        {
            if (TryGetRectTransform(target, out var rectTransform))
            {
                return rectTransform.DOAnchorPosX(x, duration, snapping);
            }

            return target.transform.DOMoveX(x, duration, snapping);
        }

        public static Tween CreateMoveYTween(GameObject target, float y, float duration, bool snapping = false)
        {
            if (TryGetRectTransform(target, out var rectTransform))
            {
                return rectTransform.DOAnchorPosY(y, duration, snapping);
            }

            return target.transform.DOMoveY(y, duration, snapping);
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

        private static bool TryGetRendererColorProperty(Renderer renderer, out int colorPropertyId)
        {
            var material = renderer != null ? renderer.sharedMaterial : null;
            if (material != null && material.HasProperty(BaseColorPropertyId))
            {
                colorPropertyId = BaseColorPropertyId;
                return true;
            }

            if (material != null && material.HasProperty(ColorPropertyId))
            {
                colorPropertyId = ColorPropertyId;
                return true;
            }

            colorPropertyId = 0;
            return false;
        }

        internal readonly struct TweenColorBinding
        {
            private readonly SpriteRenderer _spriteRenderer;
            private readonly Graphic _graphic;
            private readonly TMP_Text _tmpText;
            private readonly RendererColorBinding _renderer;

            public TweenColorBinding(SpriteRenderer spriteRenderer) : this() => _spriteRenderer = spriteRenderer;
            public TweenColorBinding(Graphic graphic) : this() => _graphic = graphic;
            public TweenColorBinding(TMP_Text tmpText) : this() => _tmpText = tmpText;
            public TweenColorBinding(RendererColorBinding renderer) : this() => _renderer = renderer;

            public Tween CreateTween(Color color, float duration)
            {
                if (_spriteRenderer != null) return _spriteRenderer.DOColor(color, duration);
                if (_graphic != null) return _graphic.DOColor(color, duration);
                if (_tmpText != null) return _tmpText.DOColor(color, duration);
                return _renderer?.CreateTween(color, duration);
            }

            public Color GetColor()
            {
                if (_spriteRenderer != null) return _spriteRenderer.color;
                if (_graphic != null) return _graphic.color;
                if (_tmpText != null) return _tmpText.color;
                return _renderer != null ? _renderer.GetColor() : default;
            }

            public void SetColor(Color color)
            {
                if (_spriteRenderer != null)
                {
                    _spriteRenderer.color = color;
                    return;
                }

                if (_graphic != null)
                {
                    _graphic.color = color;
                    return;
                }

                if (_tmpText != null)
                {
                    _tmpText.color = color;
                    return;
                }

                _renderer?.SetColor(color);
            }

            public void Restore(Color color)
            {
                if (_renderer != null)
                {
                    _renderer.RestoreOriginal();
                    return;
                }

                SetColor(color);
            }
        }

        internal readonly struct TweenAlphaBinding
        {
            private readonly CanvasGroup _canvasGroup;
            private readonly SpriteRenderer _spriteRenderer;
            private readonly Graphic _graphic;
            private readonly TMP_Text _tmpText;
            private readonly RendererColorBinding _renderer;

            public TweenAlphaBinding(CanvasGroup canvasGroup) : this() => _canvasGroup = canvasGroup;
            public TweenAlphaBinding(SpriteRenderer spriteRenderer) : this() => _spriteRenderer = spriteRenderer;
            public TweenAlphaBinding(Graphic graphic) : this() => _graphic = graphic;
            public TweenAlphaBinding(TMP_Text tmpText) : this() => _tmpText = tmpText;
            public TweenAlphaBinding(RendererColorBinding renderer) : this() => _renderer = renderer;

            public Tween CreateTween(float alpha, float duration)
            {
                if (_canvasGroup != null) return _canvasGroup.DOFade(alpha, duration);
                if (_spriteRenderer != null) return _spriteRenderer.DOFade(alpha, duration);
                if (_graphic != null) return _graphic.DOFade(alpha, duration);
                if (_tmpText != null) return _tmpText.DOFade(alpha, duration);
                return _renderer?.CreateTween(alpha, duration);
            }

            public void SetAlpha(float alpha)
            {
                if (_canvasGroup != null)
                {
                    _canvasGroup.alpha = alpha;
                    return;
                }

                if (_spriteRenderer != null)
                {
                    var color = _spriteRenderer.color;
                    color.a = alpha;
                    _spriteRenderer.color = color;
                    return;
                }

                if (_graphic != null)
                {
                    var color = _graphic.color;
                    color.a = alpha;
                    _graphic.color = color;
                    return;
                }

                if (_tmpText != null)
                {
                    var color = _tmpText.color;
                    color.a = alpha;
                    _tmpText.color = color;
                    return;
                }

                _renderer?.SetAlpha(alpha);
            }

            public float GetAlpha()
            {
                if (_canvasGroup != null) return _canvasGroup.alpha;
                if (_spriteRenderer != null) return _spriteRenderer.color.a;
                if (_graphic != null) return _graphic.color.a;
                if (_tmpText != null) return _tmpText.color.a;
                return _renderer != null ? _renderer.GetAlpha() : 1f;
            }

            public void Restore(float alpha)
            {
                if (_renderer != null)
                {
                    _renderer.RestoreOriginal();
                    return;
                }

                SetAlpha(alpha);
            }
        }

        internal sealed class RendererColorBinding
        {
            private readonly Renderer _renderer;
            private readonly MaterialPropertyBlock _originalPropertyBlock;
            private readonly MaterialPropertyBlock _propertyBlock;
            private readonly int _colorPropertyId;
            private Color _color;

            public RendererColorBinding(Renderer renderer, int colorPropertyId)
            {
                _renderer = renderer;
                _colorPropertyId = colorPropertyId;
                _originalPropertyBlock = new MaterialPropertyBlock();
                _propertyBlock = new MaterialPropertyBlock();
                _renderer.GetPropertyBlock(_originalPropertyBlock);
                _renderer.GetPropertyBlock(_propertyBlock);
                _color = _propertyBlock.HasColor(_colorPropertyId)
                    ? _propertyBlock.GetColor(_colorPropertyId)
                    : _renderer.sharedMaterial.GetColor(_colorPropertyId);
            }

            public Tween CreateTween(Color color, float duration) => DOTween.To(GetColor, SetColor, color, duration).SetTarget(_renderer);
            public Tween CreateTween(float alpha, float duration) => DOTween.To(GetAlpha, SetAlpha, alpha, duration).SetTarget(_renderer);
            public Color GetColor() => _color;
            public void SetColor(Color color)
            {
                _color = color;
                _propertyBlock.SetColor(_colorPropertyId, _color);
                _renderer.SetPropertyBlock(_propertyBlock);
            }

            public float GetAlpha() => _color.a;
            public void SetAlpha(float alpha)
            {
                _color.a = alpha;
                _propertyBlock.SetColor(_colorPropertyId, _color);
                _renderer.SetPropertyBlock(_propertyBlock);
            }

            public void RestoreOriginal() => _renderer.SetPropertyBlock(_originalPropertyBlock);
        }
    }
}
