using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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

        [Header("Input")]
        [SerializeField] private InputActionAsset _inputActions;
        [SerializeField] private string _actionMapName = "Player";
        [SerializeField] private string _replayActionName = "Jump";
        [SerializeField] private string _nextActionName = "Next";
        [SerializeField] private string _previousActionName = "Previous";

        private Coroutine _playRoutine;
        private UIStateSnapshot _imageState;
        private UIStateSnapshot _textState;
        private InputActionMap _actionMap;
        private InputAction _replayAction;
        private InputAction _nextAction;
        private InputAction _previousAction;
        private bool _ownsRuntimeActions;
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
            BindInputActions();
            ResetTargets();
            UpdateSelectionLabel();
        }

        private void OnDisable()
        {
            UnbindInputActions();
            StopPlayback();
        }

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
        }

        private void BindInputActions()
        {
            _ownsRuntimeActions = false;

            if (_inputActions == null)
            {
                CreateFallbackInputMap();
                return;
            }

            _actionMap = _inputActions.FindActionMap(_actionMapName, false);
            if (_actionMap == null)
            {
                Debug.LogWarning($"PresetShowcaseSpawner2D: Action map '{_actionMapName}' not found. Falling back to runtime actions.");
                CreateFallbackInputMap();
                return;
            }

            _replayAction = _actionMap.FindAction(_replayActionName, false);
            _nextAction = _actionMap.FindAction(_nextActionName, false);
            _previousAction = _actionMap.FindAction(_previousActionName, false);

            if (_replayAction == null || _nextAction == null || _previousAction == null)
            {
                Debug.LogWarning("PresetShowcaseSpawner2D: Required actions missing from assigned asset. Falling back to runtime actions.");
                CreateFallbackInputMap();
                return;
            }

            RegisterInputCallbacks();
            _actionMap.Enable();
        }

        private void CreateFallbackInputMap()
        {
            _actionMap = new InputActionMap("TweenHelperDemo2D");
            _replayAction = _actionMap.AddAction("Replay", InputActionType.Button, "<Keyboard>/space");
            _nextAction = _actionMap.AddAction("Next", InputActionType.Button, "<Keyboard>/downArrow");
            _previousAction = _actionMap.AddAction("Previous", InputActionType.Button, "<Keyboard>/upArrow");
            _ownsRuntimeActions = true;
            RegisterInputCallbacks();
            _actionMap.Enable();
        }

        private void RegisterInputCallbacks()
        {
            if (_replayAction != null)
            {
                _replayAction.performed += OnReplayPerformed;
            }

            if (_nextAction != null)
            {
                _nextAction.performed += OnNextPerformed;
            }

            if (_previousAction != null)
            {
                _previousAction.performed += OnPreviousPerformed;
            }
        }

        private void UnbindInputActions()
        {
            if (_replayAction != null)
            {
                _replayAction.performed -= OnReplayPerformed;
            }

            if (_nextAction != null)
            {
                _nextAction.performed -= OnNextPerformed;
            }

            if (_previousAction != null)
            {
                _previousAction.performed -= OnPreviousPerformed;
            }

            _actionMap?.Disable();
            if (_ownsRuntimeActions)
            {
                _replayAction?.Dispose();
                _nextAction?.Dispose();
                _previousAction?.Dispose();
                _actionMap.Dispose();
            }

            _actionMap = null;
            _replayAction = null;
            _nextAction = null;
            _previousAction = null;
            _ownsRuntimeActions = false;
        }

        private void OnReplayPerformed(InputAction.CallbackContext context)
        {
            ReplaySelected();
        }

        private void OnNextPerformed(InputAction.CallbackContext context)
        {
            SelectNext();
        }

        private void OnPreviousPerformed(InputAction.CallbackContext context)
        {
            SelectPrevious();
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

            private static bool TryGetColor(GameObject target, out Color color)
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
