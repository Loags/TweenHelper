using DG.Tweening;
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
        internal struct ResetVerificationResult
        {
            public bool TransformMatches;
            public bool AlphaMatches;
            public bool NoActiveTweens;
            public float PositionError;
            public float ScaleError;
            public float RotationAngleError;
            public string Details;

            public bool Passed => TransformMatches && AlphaMatches && NoActiveTweens;

            public override string ToString()
            {
                return $"Passed={Passed}, Transform={TransformMatches}, Alpha={AlphaMatches}, " +
                       $"TweensClear={NoActiveTweens}, PosErr={PositionError:F4}, ScaleErr={ScaleError:F4}, " +
                       $"RotErr={RotationAngleError:F3}, Details={Details}";
            }
        }

        private struct OriginalVisualStateSnapshot
        {
            public Vector3 LocalPosition;
            public Vector3 LocalScale;
            public Quaternion LocalRotation;

            public bool HasCanvasGroup;
            public float CanvasGroupAlpha;

            public bool HasSpriteRenderer;
            public Color SpriteColor;

            public bool HasImage;
            public Color ImageColor;

            public bool HasText;
            public Color TextColor;

            public bool HasRenderer;
            public Color RendererColor;
        }

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
        [SerializeField] private bool verboseResetLogging;

        private Canvas _worldCanvas;
        private TextMeshProUGUI _nameText;
        private TextMeshProUGUI _descriptionText;
        private CanvasGroup _canvasGroup;
        private SpriteRenderer _spriteRenderer;
        private Image _image;
        private Text _text;
        private Renderer _renderer;
        private Color _originalColor;
        private TweenHandle _currentHandle;
        private OriginalVisualStateSnapshot _originalState;
        private int _playGeneration;

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
            _canvasGroup = GetComponent<CanvasGroup>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _image = GetComponent<Image>();
            _text = GetComponent<Text>();
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

        /// <summary>
        /// Saves the current transform as the original state used for animation resets.
        /// Call after repositioning to update the reset target.
        /// </summary>
        public void SaveOriginalState()
        {
            _originalState.LocalPosition = transform.localPosition;
            _originalState.LocalScale = transform.localScale;
            _originalState.LocalRotation = transform.localRotation;
            CaptureOriginalVisualState();
        }

        private void ResetToOriginal()
        {
            transform.localPosition = _originalState.LocalPosition;
            transform.localScale = _originalState.LocalScale;
            transform.localRotation = _originalState.LocalRotation;

            if (_originalState.HasCanvasGroup && _canvasGroup != null)
            {
                _canvasGroup.alpha = _originalState.CanvasGroupAlpha;
            }

            if (_originalState.HasSpriteRenderer && _spriteRenderer != null)
            {
                _spriteRenderer.color = _originalState.SpriteColor;
            }

            if (_originalState.HasImage && _image != null)
            {
                _image.color = _originalState.ImageColor;
            }

            if (_originalState.HasText && _text != null)
            {
                _text.color = _originalState.TextColor;
            }

            if (_originalState.HasRenderer && _renderer != null && _renderer.material != null)
            {
                _renderer.material.color = _originalState.RendererColor;
            }
        }

        private void CaptureOriginalVisualState()
        {
            _originalState.HasCanvasGroup = _canvasGroup != null;
            if (_originalState.HasCanvasGroup)
            {
                _originalState.CanvasGroupAlpha = _canvasGroup.alpha;
            }

            _originalState.HasSpriteRenderer = _spriteRenderer != null;
            if (_originalState.HasSpriteRenderer)
            {
                _originalState.SpriteColor = _spriteRenderer.color;
            }

            _originalState.HasImage = _image != null;
            if (_originalState.HasImage)
            {
                _originalState.ImageColor = _image.color;
            }

            _originalState.HasText = _text != null;
            if (_originalState.HasText)
            {
                _originalState.TextColor = _text.color;
            }

            _originalState.HasRenderer = _renderer != null && _renderer.material != null;
            if (_originalState.HasRenderer)
            {
                _originalState.RendererColor = _renderer.material.color;
                _originalColor = _originalState.RendererColor;
            }
        }

        private int KillTweensForReset()
        {
            int killed = 0;

            if (_currentHandle?.Tween != null && _currentHandle.Tween.IsActive())
            {
                _currentHandle.Kill();
            }

            killed += DOTween.Kill(gameObject, false);
            killed += DOTween.Kill(transform, false);

            if (_canvasGroup != null) killed += DOTween.Kill(_canvasGroup, false);
            if (_spriteRenderer != null) killed += DOTween.Kill(_spriteRenderer, false);
            if (_image != null) killed += DOTween.Kill(_image, false);
            if (_text != null) killed += DOTween.Kill(_text, false);
            if (_renderer != null)
            {
                killed += DOTween.Kill(_renderer, false);
                if (_renderer.material != null)
                {
                    killed += DOTween.Kill(_renderer.material, false);
                }
            }

            _currentHandle = null;
            return killed;
        }

        internal bool HasActiveTweensLinkedToThisDisplay()
        {
            if (DOTween.IsTweening(gameObject)) return true;
            if (DOTween.IsTweening(transform)) return true;
            if (_canvasGroup != null && DOTween.IsTweening(_canvasGroup)) return true;
            if (_spriteRenderer != null && DOTween.IsTweening(_spriteRenderer)) return true;
            if (_image != null && DOTween.IsTweening(_image)) return true;
            if (_text != null && DOTween.IsTweening(_text)) return true;
            if (_renderer != null && DOTween.IsTweening(_renderer)) return true;
            if (_renderer != null && _renderer.material != null && DOTween.IsTweening(_renderer.material)) return true;
            return false;
        }

        internal ResetVerificationResult VerifyResetState(
            float positionTolerance = 0.001f,
            float scaleTolerance = 0.001f,
            float rotationAngleTolerance = 0.1f,
            float alphaTolerance = 0.01f)
        {
            float posErr = Vector3.Distance(transform.localPosition, _originalState.LocalPosition);
            float scaleErr = Vector3.Distance(transform.localScale, _originalState.LocalScale);
            float rotErr = Quaternion.Angle(transform.localRotation, _originalState.LocalRotation);

            bool transformMatches = posErr <= positionTolerance &&
                                    scaleErr <= scaleTolerance &&
                                    rotErr <= rotationAngleTolerance;

            bool alphaMatches = true;
            string alphaDetails = "";

            if (_originalState.HasCanvasGroup && _canvasGroup != null &&
                Mathf.Abs(_canvasGroup.alpha - _originalState.CanvasGroupAlpha) > alphaTolerance)
            {
                alphaMatches = false;
                alphaDetails += $"CanvasGroup alpha {_canvasGroup.alpha:F3}!={_originalState.CanvasGroupAlpha:F3}; ";
            }

            if (_originalState.HasSpriteRenderer && _spriteRenderer != null &&
                Mathf.Abs(_spriteRenderer.color.a - _originalState.SpriteColor.a) > alphaTolerance)
            {
                alphaMatches = false;
                alphaDetails += $"Sprite alpha {_spriteRenderer.color.a:F3}!={_originalState.SpriteColor.a:F3}; ";
            }

            if (_originalState.HasImage && _image != null &&
                Mathf.Abs(_image.color.a - _originalState.ImageColor.a) > alphaTolerance)
            {
                alphaMatches = false;
                alphaDetails += $"Image alpha {_image.color.a:F3}!={_originalState.ImageColor.a:F3}; ";
            }

            if (_originalState.HasText && _text != null &&
                Mathf.Abs(_text.color.a - _originalState.TextColor.a) > alphaTolerance)
            {
                alphaMatches = false;
                alphaDetails += $"Text alpha {_text.color.a:F3}!={_originalState.TextColor.a:F3}; ";
            }

            if (_originalState.HasRenderer && _renderer != null && _renderer.material != null &&
                Mathf.Abs(_renderer.material.color.a - _originalState.RendererColor.a) > alphaTolerance)
            {
                alphaMatches = false;
                alphaDetails += $"Renderer alpha {_renderer.material.color.a:F3}!={_originalState.RendererColor.a:F3}; ";
            }

            bool noActiveTweens = !HasActiveTweensLinkedToThisDisplay();

            string details = "";
            if (!transformMatches)
            {
                details += $"Transform mismatch (pos={posErr:F4}, scale={scaleErr:F4}, rot={rotErr:F3}). ";
            }
            if (!alphaMatches)
            {
                details += alphaDetails;
            }
            if (!noActiveTweens)
            {
                details += "Active tweens still detected. ";
            }

            return new ResetVerificationResult
            {
                TransformMatches = transformMatches,
                AlphaMatches = alphaMatches,
                NoActiveTweens = noActiveTweens,
                PositionError = posErr,
                ScaleError = scaleErr,
                RotationAngleError = rotErr,
                Details = details.Trim()
            };
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
            _nameText.textWrappingMode = TextWrappingModes.Normal;

            var nameLayout = nameGO.AddComponent<LayoutElement>();
            nameLayout.preferredHeight = 30;

            // Create description text
            var descGO = new GameObject("DescriptionText");
            descGO.transform.SetParent(panelGO.transform, false);
            _descriptionText = descGO.AddComponent<TextMeshProUGUI>();
            _descriptionText.fontSize = 14;
            _descriptionText.color = new Color(textColor.r, textColor.g, textColor.b, 0.8f);
            _descriptionText.alignment = TextAlignmentOptions.Center;
            _nameText.textWrappingMode = TextWrappingModes.Normal;

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
                var current = _renderer.material.color;
                var next = hoverColor;
                next.a = current.a;
                _renderer.material.color = next;
            }
        }

        private void OnMouseExit()
        {
            if (_renderer != null)
            {
                var current = _renderer.material.color;
                var next = _originalColor;
                next.a = current.a;
                _renderer.material.color = next;
            }
        }

        private void OnMouseDown()
        {
            PlayPreset();
        }
         
        public void PlayPreset()
        {
            // Kill any existing animation
            _playGeneration++;
            KillTweensForReset();
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
            _playGeneration++;
            int killedTweens = KillTweensForReset();
            ResetToOriginal();

            if (verboseResetLogging)
            {
                var verification = VerifyResetState();
                Debug.Log($"AnimationPresetDisplay: Reset '{presetName}' (gen={_playGeneration}, killed={killedTweens}) -> {verification}");
            }
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
            KillTweensForReset();
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
