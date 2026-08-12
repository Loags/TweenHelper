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
        [SerializeField] private float baseAnchoredPositionZ;
        [SerializeField] private bool hasAlpha;
        [SerializeField] private float baseAlpha;

        public bool IsCaptured => _captured;
        public Vector3 BaseScale => _baseScale;
        public Vector3 BaseEulerAngles => _baseEulerAngles;
        public Vector2 BaseAnchoredPosition => _baseAnchoredPosition;
        public Vector3 BaseAnchoredPosition3D => new Vector3(_baseAnchoredPosition.x, _baseAnchoredPosition.y, baseAnchoredPositionZ);
        public bool HasColor => _hasColor;
        public Color BaseColor => _baseColor;
        public bool HasCanvasGroup => _hasCanvasGroup;
        public float BaseCanvasAlpha => _baseCanvasAlpha;
        public bool HasAlpha => hasAlpha;
        public float BaseAlpha => baseAlpha;

        public void CaptureIfNeeded() => Capture(false);

        public void Refresh() => Capture(true);

        private void Capture(bool force)
        {
            if (_captured && !force) return;

            _captured = true;

            var targetTransform = transform;
            _baseScale = targetTransform.localScale;
            _baseEulerAngles = targetTransform.localEulerAngles;

            if (targetTransform is RectTransform rectTransform)
            {
                _baseAnchoredPosition = rectTransform.anchoredPosition;
                baseAnchoredPositionZ = rectTransform.anchoredPosition3D.z;
            }

            _hasColor = false;
            if (TweenTargetUtility.TryGetColor(gameObject, out var color))
            {
                _hasColor = true;
                _baseColor = color;
            }

            var canvasGroup = GetComponent<CanvasGroup>();
            _hasCanvasGroup = canvasGroup != null;
            if (canvasGroup != null)
            {
                _baseCanvasAlpha = canvasGroup.alpha;
            }

            hasAlpha = TweenTargetUtility.TryGetAlphaBinding(gameObject, out var alphaBinding);
            if (hasAlpha) baseAlpha = alphaBinding.GetAlpha();
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
