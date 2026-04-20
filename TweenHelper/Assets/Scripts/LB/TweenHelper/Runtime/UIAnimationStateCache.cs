using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Stores baseline UI values so semantic animations can return to a stable "normal" state.
    /// </summary>
    internal sealed class UIAnimationStateCache : MonoBehaviour
    {
        [SerializeField] private bool _captured;
        [SerializeField] private Vector3 _baseScale;
        [SerializeField] private Vector3 _baseEulerAngles;
        [SerializeField] private Vector2 _baseAnchoredPosition;
        [SerializeField] private bool _hasColor;
        [SerializeField] private Color _baseColor;
        [SerializeField] private bool _hasCanvasGroup;
        [SerializeField] private float _baseCanvasAlpha;

        public Vector3 BaseScale => _baseScale;
        public Vector3 BaseEulerAngles => _baseEulerAngles;
        public Vector2 BaseAnchoredPosition => _baseAnchoredPosition;
        public bool HasColor => _hasColor;
        public Color BaseColor => _baseColor;
        public bool HasCanvasGroup => _hasCanvasGroup;
        public float BaseCanvasAlpha => _baseCanvasAlpha;

        public void CaptureIfNeeded()
        {
            if (_captured)
            {
                return;
            }

            _captured = true;

            var targetTransform = transform;
            _baseScale = targetTransform.localScale;
            _baseEulerAngles = targetTransform.localEulerAngles;

            if (targetTransform is RectTransform rectTransform)
            {
                _baseAnchoredPosition = rectTransform.anchoredPosition;
            }

            if (TweenTargetUtility.TryGetColor(gameObject, out var color))
            {
                _hasColor = true;
                _baseColor = color;
            }

            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                _hasCanvasGroup = true;
                _baseCanvasAlpha = canvasGroup.alpha;
            }
        }

        public static UIAnimationStateCache GetOrCreate(GameObject target)
        {
            var cache = target.GetComponent<UIAnimationStateCache>();
            if (cache == null)
            {
                cache = target.AddComponent<UIAnimationStateCache>();
            }

            cache.CaptureIfNeeded();
            return cache;
        }
    }
}
