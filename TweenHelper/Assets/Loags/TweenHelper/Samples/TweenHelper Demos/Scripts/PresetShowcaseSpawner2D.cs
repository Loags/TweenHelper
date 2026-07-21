using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Lets the user browse and replay semantic UI showcase animations via the new Input System.
    /// </summary>
    public class PresetShowcaseSpawner2D : MonoBehaviour
    {
        [Header("Targets")]
        [SerializeField] private TextMeshProUGUI _presetNameText;
        [SerializeField] private Image _presetImage;
        [SerializeField] private TextMeshProUGUI _animatedText;

        [Header("Replay")]
        [SerializeField] private float _stepDelay = 0.2f;

        [Header("Colors")]
        [SerializeField] private Color _imageHoverColor = new Color(1f, 0.9f, 0.6f, 1f);
        [SerializeField] private Color _textHoverColor = new Color(0.7f, 0.9f, 1f, 1f);
        [SerializeField] private Color _disabledTextColor = new Color(0.65f, 0.65f, 0.65f, 0.55f);

        private Coroutine _playRoutine;
        private UIStateSnapshot _imageState;
        private UIStateSnapshot _textState;
        private TweenHandle _activeTween;
        private int _selectedStepIndex;

        private void Awake()
        {
            if (_presetImage != null)
            {
                _imageState = UIStateSnapshot.Capture(_presetImage.gameObject);
            }

            if (_animatedText != null)
            {
                _textState = UIStateSnapshot.Capture(_animatedText.gameObject);
            }
        }

        private void OnEnable()
        {
            ResetTargets();
            UpdateSelectionLabel();
        }

        private void OnDisable()
        {
            StopPlayback();
        }

#if ENABLE_LEGACY_INPUT_MANAGER
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) ReplaySelected();
            if (Input.GetKeyDown(KeyCode.DownArrow)) SelectNext();
            if (Input.GetKeyDown(KeyCode.UpArrow)) SelectPrevious();
        }
#endif

        [ContextMenu("Replay Selected")]
        public void ReplaySelected()
        {
            StopPlayback();
            ResetTargets();

            _playRoutine = StartCoroutine(PlaySelectedStep());
        }

        [ContextMenu("Select Next")]
        public void SelectNext()
        {
            StopPlayback();
            ResetTargets();
            _selectedStepIndex = (_selectedStepIndex + 1) % ShowcaseStepCount;
            UpdateSelectionLabel();
        }

        [ContextMenu("Select Previous")]
        public void SelectPrevious()
        {
            StopPlayback();
            ResetTargets();
            _selectedStepIndex = (_selectedStepIndex - 1 + ShowcaseStepCount) % ShowcaseStepCount;
            UpdateSelectionLabel();
        }

        internal int AuditStepCount => ShowcaseStepCount;

        internal string GetAuditStepLabel(int index) => GetStep(NormalizeStepIndex(index)).Label;

        internal TweenHandle PlayAuditStep(int index)
        {
            StopPlayback();
            ResetTargets();

            _selectedStepIndex = NormalizeStepIndex(index);
            var step = GetStep(_selectedStepIndex);
            step.ResetBeforePlay?.Invoke();
            _activeTween = step.Play?.Invoke();
            return _activeTween;
        }

        internal void ResetAuditState()
        {
            StopPlayback();
            ResetTargets();
        }

        internal AnimationPresetDisplay.ResetVerificationResult VerifyAuditResetState(
            float positionTolerance = 0.001f,
            float scaleTolerance = 0.001f,
            float rotationAngleTolerance = 0.1f,
            float colorTolerance = 0.01f)
        {
            var imageResult = VerifyTargetState(_presetImage != null ? _presetImage.gameObject : null, _imageState,
                "Image", positionTolerance, scaleTolerance, rotationAngleTolerance, colorTolerance);
            var textResult = VerifyTargetState(_animatedText != null ? _animatedText.gameObject : null, _textState,
                "Text", positionTolerance, scaleTolerance, rotationAngleTolerance, colorTolerance);

            return new AnimationPresetDisplay.ResetVerificationResult
            {
                TransformMatches = imageResult.TransformMatches && textResult.TransformMatches,
                AlphaMatches = imageResult.AlphaMatches && textResult.AlphaMatches,
                NoActiveTweens = imageResult.NoActiveTweens && textResult.NoActiveTweens,
                PositionError = Mathf.Max(imageResult.PositionError, textResult.PositionError),
                ScaleError = Mathf.Max(imageResult.ScaleError, textResult.ScaleError),
                RotationAngleError = Mathf.Max(imageResult.RotationAngleError, textResult.RotationAngleError),
                Details = JoinDetails(imageResult.Details, textResult.Details)
            };
        }

        private IEnumerator PlaySelectedStep()
        {
            var step = GetStep(_selectedStepIndex);
            SetLabel($"{step.Label}  |  Running");

            step.ResetBeforePlay?.Invoke();
            _activeTween = step.Play?.Invoke();

            if (_activeTween?.Tween == null)
            {
                yield break;
            }

            while (_activeTween.IsActive && !_activeTween.IsComplete)
            {
                yield return null;
            }

            _activeTween = null;
            SetLabel($"{step.Label}  |  Ready");
            yield return new WaitForSeconds(_stepDelay);
            _playRoutine = null;
        }

        private void SetLabel(string value)
        {
            if (_presetNameText != null)
            {
                _presetNameText.text = value;
            }
        }

        private void UpdateSelectionLabel()
        {
            var step = GetStep(_selectedStepIndex);
            SetLabel($"{step.Label}  |  Space: Replay  |  Up/Down: Browse");
        }

        private void ResetTargets()
        {
            if (_presetImage != null)
            {
                _imageState.Apply(_presetImage.gameObject);
            }

            if (_animatedText != null)
            {
                _textState.Apply(_animatedText.gameObject);
            }
        }

        private void StopPlayback()
        {
            if (_playRoutine != null)
            {
                StopCoroutine(_playRoutine);
                _playRoutine = null;
            }

            _activeTween?.Kill();
            _activeTween = null;
            KillTargetTweens(_presetImage != null ? _presetImage.gameObject : null);
            KillTargetTweens(_animatedText != null ? _animatedText.gameObject : null);
        }

        private static int NormalizeStepIndex(int index)
        {
            int normalized = index % ShowcaseStepCount;
            return normalized < 0 ? normalized + ShowcaseStepCount : normalized;
        }

        private static void KillTargetTweens(GameObject target)
        {
            if (target == null) return;

            DOTween.Kill(target, false);
            DOTween.Kill(target.transform, false);

            var graphic = target.GetComponent<Graphic>();
            if (graphic != null) DOTween.Kill(graphic, false);

            var text = target.GetComponent<TMP_Text>();
            if (text != null) DOTween.Kill(text, false);

            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null) DOTween.Kill(canvasGroup, false);
        }

        private static AnimationPresetDisplay.ResetVerificationResult VerifyTargetState(
            GameObject target,
            UIStateSnapshot expected,
            string label,
            float positionTolerance,
            float scaleTolerance,
            float rotationAngleTolerance,
            float colorTolerance)
        {
            if (target == null)
            {
                return new AnimationPresetDisplay.ResetVerificationResult
                {
                    TransformMatches = true,
                    AlphaMatches = true,
                    NoActiveTweens = true
                };
            }

            float positionError = target.transform is RectTransform rectTransform
                ? Vector2.Distance(rectTransform.anchoredPosition, expected.AnchoredPosition)
                : 0f;
            float scaleError = Vector3.Distance(target.transform.localScale, expected.Scale);
            float rotationError = Quaternion.Angle(target.transform.localRotation, Quaternion.Euler(expected.EulerAngles));
            bool transformMatches = positionError <= positionTolerance &&
                                    scaleError <= scaleTolerance &&
                                    rotationError <= rotationAngleTolerance;

            bool visualMatches = true;
            string details = "";
            if (expected.HasColor && UIStateSnapshot.TryGetColor(target, out var color))
            {
                float colorError = Mathf.Max(
                    Mathf.Abs(color.r - expected.Color.r),
                    Mathf.Abs(color.g - expected.Color.g),
                    Mathf.Abs(color.b - expected.Color.b),
                    Mathf.Abs(color.a - expected.Color.a));
                if (colorError > colorTolerance)
                {
                    visualMatches = false;
                    details += $"{label} color mismatch ({color} != {expected.Color}). ";
                }
            }

            if (expected.HasCanvasGroup)
            {
                var canvasGroup = target.GetComponent<CanvasGroup>();
                if (canvasGroup == null || Mathf.Abs(canvasGroup.alpha - expected.CanvasAlpha) > colorTolerance)
                {
                    visualMatches = false;
                    details += $"{label} CanvasGroup alpha mismatch. ";
                }
            }

            bool noActiveTweens = !HasActiveTargetTweens(target);
            if (!transformMatches)
            {
                details += $"{label} transform mismatch (pos={positionError:F4}, scale={scaleError:F4}, rot={rotationError:F3}). ";
            }
            if (!noActiveTweens)
            {
                details += $"{label} still has active tweens. ";
            }

            return new AnimationPresetDisplay.ResetVerificationResult
            {
                TransformMatches = transformMatches,
                AlphaMatches = visualMatches,
                NoActiveTweens = noActiveTweens,
                PositionError = positionError,
                ScaleError = scaleError,
                RotationAngleError = rotationError,
                Details = details.Trim()
            };
        }

        private static bool HasActiveTargetTweens(GameObject target)
        {
            if (DOTween.IsTweening(target) || DOTween.IsTweening(target.transform)) return true;

            var graphic = target.GetComponent<Graphic>();
            if (graphic != null && DOTween.IsTweening(graphic)) return true;

            var text = target.GetComponent<TMP_Text>();
            if (text != null && DOTween.IsTweening(text)) return true;

            var canvasGroup = target.GetComponent<CanvasGroup>();
            return canvasGroup != null && DOTween.IsTweening(canvasGroup);
        }

        private static string JoinDetails(string first, string second)
        {
            if (string.IsNullOrEmpty(first)) return second;
            if (string.IsNullOrEmpty(second)) return first;
            return first + " | " + second;
        }

        private ShowcaseStep GetStep(int index)
        {
            switch (index)
            {
                case 0:
                    return new ShowcaseStep("Image.UIAppear", ResetImage, () => _presetImage != null ? _presetImage.UIAppear() : null);
                case 1:
                    return new ShowcaseStep("Image.UIHover", ResetImage, () => _presetImage != null ? _presetImage.UIHover(hoverColor: _imageHoverColor) : null);
                case 2:
                    return new ShowcaseStep("Image.UIPress", ResetImage, () => _presetImage != null ? _presetImage.UIPress() : null);
                case 3:
                    return new ShowcaseStep("Image.UIAttention", ResetImage, () => _presetImage != null ? _presetImage.UIAttention() : null);
                case 4:
                    return new ShowcaseStep("Image.UIDisappear", ResetImage, () => _presetImage != null ? _presetImage.UIDisappear() : null);
                case 5:
                    return new ShowcaseStep("Text.UIAppearSoft", ResetText, () => _animatedText != null ? _animatedText.UIAppearSoft() : null);
                case 6:
                    return new ShowcaseStep("Text.UIHoverSoft", ResetText, () => _animatedText != null ? _animatedText.UIHoverSoft(hoverColor: _textHoverColor) : null);
                case 7:
                    return new ShowcaseStep("Text.UIPress", ResetText, () => _animatedText != null ? _animatedText.UIPress() : null);
                case 8:
                    return new ShowcaseStep("Text.UIAttentionSoft", ResetText, () => _animatedText != null ? _animatedText.UIAttentionSoft() : null);
                case 9:
                    return new ShowcaseStep("Text.UIDisabled", ResetText, () => _animatedText != null ? _animatedText.UIDisabled(disabledColor: _disabledTextColor) : null);
                case 10:
                    return new ShowcaseStep("Text.UIEnabled", ResetText, () => _animatedText != null ? _animatedText.UIEnabled() : null);
                default:
                    return new ShowcaseStep("Image.UIAppear", ResetImage, () => _presetImage != null ? _presetImage.UIAppear() : null);
            }
        }

        private void ResetImage()
        {
            if (_presetImage != null)
            {
                _imageState.Apply(_presetImage.gameObject);
            }
        }

        private void ResetText()
        {
            if (_animatedText != null)
            {
                _textState.Apply(_animatedText.gameObject);
            }
        }

        private const int ShowcaseStepCount = 11;

        private readonly struct ShowcaseStep
        {
            public readonly string Label;
            public readonly System.Action ResetBeforePlay;
            public readonly System.Func<TweenHandle> Play;

            public ShowcaseStep(string label, System.Action resetBeforePlay, System.Func<TweenHandle> play)
            {
                Label = label;
                ResetBeforePlay = resetBeforePlay;
                Play = play;
            }
        }

        private struct UIStateSnapshot
        {
            public Vector3 Scale;
            public Vector3 EulerAngles;
            public Vector2 AnchoredPosition;
            public Color Color;
            public bool HasColor;
            public float CanvasAlpha;
            public bool HasCanvasGroup;

            public static UIStateSnapshot Capture(GameObject target)
            {
                var snapshot = new UIStateSnapshot
                {
                    Scale = target.transform.localScale,
                    EulerAngles = target.transform.localEulerAngles
                };

                if (target.transform is RectTransform rectTransform)
                {
                    snapshot.AnchoredPosition = rectTransform.anchoredPosition;
                }

                if (TryGetColor(target, out var color))
                {
                    snapshot.HasColor = true;
                    snapshot.Color = color;
                }

                var canvasGroup = target.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    snapshot.HasCanvasGroup = true;
                    snapshot.CanvasAlpha = canvasGroup.alpha;
                }

                return snapshot;
            }

            public void Apply(GameObject target)
            {
                target.transform.localScale = Scale;
                target.transform.localEulerAngles = EulerAngles;

                if (target.transform is RectTransform rectTransform)
                {
                    rectTransform.anchoredPosition = AnchoredPosition;
                }

                if (HasColor)
                {
                    TrySetColor(target, Color);
                }

                if (HasCanvasGroup)
                {
                    var canvasGroup = target.GetComponent<CanvasGroup>();
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = CanvasAlpha;
                    }
                }
            }

            internal static bool TryGetColor(GameObject target, out Color color)
            {
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

            private static void TrySetColor(GameObject target, Color color)
            {
                var graphic = target.GetComponent<Graphic>();
                if (graphic != null)
                {
                    graphic.color = color;
                    return;
                }

                var tmpText = target.GetComponent<TMP_Text>();
                if (tmpText != null)
                {
                    tmpText.color = color;
                }
            }
        }
    }
}
