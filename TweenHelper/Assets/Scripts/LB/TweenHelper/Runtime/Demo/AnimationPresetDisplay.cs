using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Displays animation preset info and plays the animation when clicked.
    /// Creates a world space canvas showing the preset name and description.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class AnimationPresetDisplay : MonoBehaviour
    {
        [Header("Preset")]
        [SerializeField] private string presetName;
        [SerializeField] private string presetDescription;

        [Header("Display Settings")]
        [SerializeField] private Vector3 labelOffset = new Vector3(0, 1.5f, 0);
        [SerializeField] private float canvasScale = 0.01f;
        [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.7f);
        [SerializeField] private Color textColor = Color.white;

        [Header("Visual Feedback")]
        [SerializeField] private Color hoverColor = new Color(0.3f, 0.7f, 1f);
        [SerializeField] private Color normalColor = Color.white;

        private Canvas _worldCanvas;
        private TextMeshProUGUI _nameText;
        private TextMeshProUGUI _descriptionText;
        private Renderer _renderer;
        private Color _originalColor;
        private TweenHandle _currentHandle;
        private Vector3 _originalPosition;
        private Vector3 _originalScale;
        private Quaternion _originalRotation;

        public string PresetName
        {
            get => presetName;
            set
            {
                presetName = value;
                UpdateLabel();
            }
        }

        public string PresetDescription
        {
            get => presetDescription;
            set
            {
                presetDescription = value;
                UpdateLabel();
            }
        }

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer != null)
            {
                _originalColor = _renderer.material.color;
            }

            SaveOriginalState();
        }

        private void Start()
        {
            CreateWorldSpaceCanvas();
            UpdateLabel();
        }

        private void SaveOriginalState()
        {
            _originalPosition = transform.localPosition;
            _originalScale = transform.localScale;
            _originalRotation = transform.localRotation;
        }

        private void ResetToOriginal()
        {
            transform.localPosition = _originalPosition;
            transform.localScale = _originalScale;
            transform.localRotation = _originalRotation;

            // Reset alpha for fade presets
            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null) canvasGroup.alpha = 1f;

            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                var c = spriteRenderer.color;
                c.a = 1f;
                spriteRenderer.color = c;
            }

            // Reset material alpha
            if (_renderer != null && _renderer.material != null)
            {
                var c = _renderer.material.color;
                c.a = 1f;
                _renderer.material.color = c;
            }
        }

        private void CreateWorldSpaceCanvas()
        {
            // Create canvas GameObject
            var canvasGO = new GameObject($"{presetName}_Label");
            canvasGO.transform.SetParent(transform);
            canvasGO.transform.localPosition = labelOffset;
            canvasGO.transform.localScale = Vector3.one * canvasScale;

            // Add Canvas component
            _worldCanvas = canvasGO.AddComponent<Canvas>();
            _worldCanvas.renderMode = RenderMode.WorldSpace;

            // Add CanvasScaler
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 10f;

            // Set canvas size
            var rectTransform = canvasGO.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(300, 100);

            // Create background panel
            var panelGO = new GameObject("Panel");
            panelGO.transform.SetParent(canvasGO.transform, false);
            var panelRect = panelGO.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;
            panelRect.anchoredPosition = Vector2.zero;

            var panelImage = panelGO.AddComponent<Image>();
            panelImage.color = backgroundColor;

            // Create vertical layout
            var layoutGroup = panelGO.AddComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset(10, 10, 5, 5);
            layoutGroup.spacing = 2;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlHeight = true;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;

            // Create name text
            var nameGO = new GameObject("NameText");
            nameGO.transform.SetParent(panelGO.transform, false);
            _nameText = nameGO.AddComponent<TextMeshProUGUI>();
            _nameText.fontSize = 24;
            _nameText.fontStyle = FontStyles.Bold;
            _nameText.color = textColor;
            _nameText.alignment = TextAlignmentOptions.Center;
            _nameText.enableWordWrapping = false;

            var nameLayout = nameGO.AddComponent<LayoutElement>();
            nameLayout.preferredHeight = 30;

            // Create description text
            var descGO = new GameObject("DescriptionText");
            descGO.transform.SetParent(panelGO.transform, false);
            _descriptionText = descGO.AddComponent<TextMeshProUGUI>();
            _descriptionText.fontSize = 14;
            _descriptionText.color = new Color(textColor.r, textColor.g, textColor.b, 0.8f);
            _descriptionText.alignment = TextAlignmentOptions.Center;
            _descriptionText.enableWordWrapping = true;

            var descLayout = descGO.AddComponent<LayoutElement>();
            descLayout.preferredHeight = 20;

            // Add billboard behavior
            canvasGO.AddComponent<BillboardCanvas>();
        }

        private void UpdateLabel()
        {
            if (_nameText != null)
            {
                _nameText.text = presetName;
            }
            if (_descriptionText != null)
            {
                _descriptionText.text = presetDescription;
            }
        }

        private void OnMouseEnter()
        {
            if (_renderer != null)
            {
                _renderer.material.color = hoverColor;
            }
        }

        private void OnMouseExit()
        {
            if (_renderer != null)
            {
                _renderer.material.color = _originalColor;
            }
        }

        private void OnMouseDown()
        {
            PlayPreset();
        }

        public void PlayPreset()
        {
            // Kill any existing animation
            _currentHandle?.Kill();
            ResetToOriginal();

            // Play the preset
            if (!string.IsNullOrEmpty(presetName))
            {
                _currentHandle = transform.Tween().Preset(presetName).Play();
                Debug.Log($"Playing preset: {presetName}");

                // Register with reset manager
                if (AnimationResetManager.Instance != null)
                {
                    AnimationResetManager.Instance.RegisterAnimated(this);
                }
            }
        }

        /// <summary>
        /// Resets the animation to its original state.
        /// Called by AnimationResetManager or manually.
        /// </summary>
        public void ResetAnimation()
        {
            _currentHandle?.Kill();
            _currentHandle = null;
            ResetToOriginal();
        }

        /// <summary>
        /// Sets up this display with preset data.
        /// </summary>
        public void Setup(string name, string description)
        {
            presetName = name;
            presetDescription = description;
            UpdateLabel();
        }

        private void OnDestroy()
        {
            _currentHandle?.Kill();
        }
    }

    /// <summary>
    /// Makes the canvas always face the camera.
    /// </summary>
    public class BillboardCanvas : MonoBehaviour
    {
        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null) return;
            }

            // Face the camera
            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                _mainCamera.transform.rotation * Vector3.up);
        }
    }
}
