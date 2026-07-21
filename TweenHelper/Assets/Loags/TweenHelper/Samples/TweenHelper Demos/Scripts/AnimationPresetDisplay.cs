using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Displays animation preset info and plays the animation when clicked.
    /// Uses an authored world-space label prefab to show the preset name and description.
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
        [SerializeField] private PresetWorldLabelView worldLabelPrefab;

        [Header("Visual Feedback")]
        [SerializeField] private Color hoverColor = new Color(0.3f, 0.7f, 1f);
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private bool verboseResetLogging;

        private PresetWorldLabelView _worldLabel;
        private CanvasGroup _canvasGroup;
        private SpriteRenderer _spriteRenderer;
        private Image _image;
        private Text _text;
        private Renderer _renderer;
        private ITweenPreset _preset;
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
                _preset = string.IsNullOrEmpty(value) ? null : TweenPresetRegistry.GetPresetByName(value);
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

            if (!string.IsNullOrEmpty(presetName))
            {
                _preset = TweenPresetRegistry.GetPresetByName(presetName);
            }

            SaveOriginalState();
        }

        private void Start()
        {
            _worldLabel = Instantiate(worldLabelPrefab, transform);
            _worldLabel.transform.localPosition = labelOffset;
            _worldLabel.transform.localScale = Vector3.one * canvasScale;
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

        private void UpdateLabel()
        {
            if (_worldLabel != null) _worldLabel.SetContent(presetName, presetDescription);
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
            if (_preset != null)
            {
                _currentHandle = transform.Tween().Preset(_preset).Play();
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
            _preset = string.IsNullOrEmpty(name) ? null : TweenPresetRegistry.GetPresetByName(name);
            UpdateLabel();
        }

        /// <summary>
        /// Sets up this display with an already resolved preset.
        /// </summary>
        public void Setup(ITweenPreset preset)
        {
            _preset = preset;
            presetName = preset.PresetName;
            presetDescription = preset.Description;
            UpdateLabel();
        }

        public void Setup(ITweenPreset preset, PresetWorldLabelView labelPrefab)
        {
            worldLabelPrefab = labelPrefab;
            Setup(preset);
        }

        private void OnDestroy()
        {
            KillTweensForReset();
        }
    }

}
