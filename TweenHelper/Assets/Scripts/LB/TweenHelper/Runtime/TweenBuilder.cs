using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper
{
    /// <summary>
    /// Unified fluent builder for creating both single tweens and sequences.
    /// Use extension methods transform.Tween() or gameObject.Tween() to create.
    /// </summary>
    public class TweenBuilder
    {
        private readonly Transform _transform;
        private readonly GameObject _gameObject;
        private readonly List<TweenStep> _steps = new List<TweenStep>();
        private TweenOptions _currentOptions = default;
        private bool _isSequenceMode = false;
        private bool _nextIsParallel = false;
        private Action _onComplete;
        private Action _onKill;

        #region Constructors

        /// <summary>
        /// Creates a new TweenBuilder for the specified Transform.
        /// </summary>
        public TweenBuilder(Transform transform)
        {
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            _gameObject = transform.gameObject;
        }

        /// <summary>
        /// Creates a new TweenBuilder for the specified GameObject.
        /// </summary>
        public TweenBuilder(GameObject gameObject)
        {
            _gameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
            _transform = gameObject.transform;
        }

        #endregion

        #region Movement

        /// <summary>
        /// Moves to the specified world position.
        /// </summary>
        public TweenBuilder Move(Vector3 target, float? duration = null)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOMove(target, dur));
            return this;
        }

        /// <summary>
        /// Moves to the specified local position.
        /// </summary>
        public TweenBuilder MoveLocal(Vector3 target, float? duration = null)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOLocalMove(target, dur));
            return this;
        }

        /// <summary>
        /// Moves by the specified offset from current position.
        /// </summary>
        public TweenBuilder MoveBy(Vector3 offset, float? duration = null)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOMove(_transform.position + offset, dur));
            return this;
        }

        /// <summary>
        /// Moves along the X axis.
        /// </summary>
        public TweenBuilder MoveX(float target, float? duration = null)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOMoveX(target, dur));
            return this;
        }

        /// <summary>
        /// Moves along the Y axis.
        /// </summary>
        public TweenBuilder MoveY(float target, float? duration = null)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOMoveY(target, dur));
            return this;
        }

        /// <summary>
        /// Moves along the Z axis.
        /// </summary>
        public TweenBuilder MoveZ(float target, float? duration = null)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOMoveZ(target, dur));
            return this;
        }

        /// <summary>
        /// Punches the position with the specified amount.
        /// </summary>
        public TweenBuilder PunchPosition(Vector3 punch, float? duration = null, int vibrato = 10, float elasticity = 1f)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOPunchPosition(punch, dur, vibrato, elasticity));
            return this;
        }

        /// <summary>
        /// Shakes the position.
        /// </summary>
        public TweenBuilder ShakePosition(float? duration = null, float strength = 1f, int vibrato = 10, float randomness = 90f)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOShakePosition(dur, strength, vibrato, randomness));
            return this;
        }

        #endregion

        #region Rotation

        /// <summary>
        /// Rotates to the specified euler angles.
        /// </summary>
        public TweenBuilder Rotate(Vector3 target, float? duration = null, RotateMode mode = RotateMode.Fast)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DORotate(target, dur, mode));
            return this;
        }

        /// <summary>
        /// Rotates to the specified local euler angles.
        /// </summary>
        public TweenBuilder RotateLocal(Vector3 target, float? duration = null, RotateMode mode = RotateMode.Fast)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOLocalRotate(target, dur, mode));
            return this;
        }

        /// <summary>
        /// Rotates by the specified euler offset.
        /// </summary>
        public TweenBuilder RotateBy(Vector3 offset, float? duration = null, RotateMode mode = RotateMode.Fast)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DORotate(_transform.eulerAngles + offset, dur, mode));
            return this;
        }

        /// <summary>
        /// Rotates to the specified quaternion.
        /// </summary>
        public TweenBuilder RotateQuaternion(Quaternion target, float? duration = null)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DORotateQuaternion(target, dur));
            return this;
        }

        /// <summary>
        /// Punches the rotation with the specified amount.
        /// </summary>
        public TweenBuilder PunchRotation(Vector3 punch, float? duration = null, int vibrato = 10, float elasticity = 1f)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOPunchRotation(punch, dur, vibrato, elasticity));
            return this;
        }

        /// <summary>
        /// Shakes the rotation.
        /// </summary>
        public TweenBuilder ShakeRotation(float? duration = null, float strength = 90f, int vibrato = 10, float randomness = 90f)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOShakeRotation(dur, strength, vibrato, randomness));
            return this;
        }

        #endregion

        #region Scale

        /// <summary>
        /// Scales to the specified uniform scale.
        /// </summary>
        public TweenBuilder Scale(float target, float? duration = null)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOScale(target, dur));
            return this;
        }

        /// <summary>
        /// Scales to the specified scale vector.
        /// </summary>
        public TweenBuilder Scale(Vector3 target, float? duration = null)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOScale(target, dur));
            return this;
        }

        /// <summary>
        /// Scales along the X axis.
        /// </summary>
        public TweenBuilder ScaleX(float target, float? duration = null)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOScaleX(target, dur));
            return this;
        }

        /// <summary>
        /// Scales along the Y axis.
        /// </summary>
        public TweenBuilder ScaleY(float target, float? duration = null)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOScaleY(target, dur));
            return this;
        }

        /// <summary>
        /// Scales along the Z axis.
        /// </summary>
        public TweenBuilder ScaleZ(float target, float? duration = null)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOScaleZ(target, dur));
            return this;
        }

        /// <summary>
        /// Punches the scale with the specified amount.
        /// </summary>
        public TweenBuilder PunchScale(Vector3 punch, float? duration = null, int vibrato = 10, float elasticity = 1f)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOPunchScale(punch, dur, vibrato, elasticity));
            return this;
        }

        /// <summary>
        /// Shakes the scale.
        /// </summary>
        public TweenBuilder ShakeScale(float? duration = null, float strength = 1f, int vibrato = 10, float randomness = 90f)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => _transform.DOShakeScale(dur, strength, vibrato, randomness));
            return this;
        }

        #endregion

        #region Fade

        /// <summary>
        /// Fades to the specified alpha. Auto-detects CanvasGroup, SpriteRenderer, Image, or Text.
        /// </summary>
        public TweenBuilder Fade(float alpha, float? duration = null)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => CreateFadeTween(alpha, dur));
            return this;
        }

        /// <summary>
        /// Fades in (alpha to 1).
        /// </summary>
        public TweenBuilder FadeIn(float? duration = null)
        {
            return Fade(1f, duration);
        }

        /// <summary>
        /// Fades out (alpha to 0).
        /// </summary>
        public TweenBuilder FadeOut(float? duration = null)
        {
            return Fade(0f, duration);
        }

        private Tween CreateFadeTween(float alpha, float duration)
        {
            // Try CanvasGroup first
            var canvasGroup = _gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                return canvasGroup.DOFade(alpha, duration);
            }

            // Try SpriteRenderer
            var spriteRenderer = _gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                return spriteRenderer.DOFade(alpha, duration);
            }

            // Try Image
            var image = _gameObject.GetComponent<Image>();
            if (image != null)
            {
                return image.DOFade(alpha, duration);
            }

            // Try Text
            var text = _gameObject.GetComponent<Text>();
            if (text != null)
            {
                return text.DOFade(alpha, duration);
            }

            Debug.LogWarning($"TweenBuilder: No fadeable component found on {_gameObject.name}. " +
                           "Supported: CanvasGroup, SpriteRenderer, Image, Text.");
            return null;
        }

        #endregion

        #region Color

        /// <summary>
        /// Animates color. Auto-detects SpriteRenderer, Image, or Text.
        /// </summary>
        public TweenBuilder Color(Color target, float? duration = null)
        {
            var dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            AddStep(() => CreateColorTween(target, dur));
            return this;
        }

        private Tween CreateColorTween(Color target, float duration)
        {
            var spriteRenderer = _gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                return spriteRenderer.DOColor(target, duration);
            }

            var image = _gameObject.GetComponent<Image>();
            if (image != null)
            {
                return image.DOColor(target, duration);
            }

            var text = _gameObject.GetComponent<Text>();
            if (text != null)
            {
                return text.DOColor(target, duration);
            }

            Debug.LogWarning($"TweenBuilder: No colorable component found on {_gameObject.name}. " +
                           "Supported: SpriteRenderer, Image, Text.");
            return null;
        }

        #endregion

        #region Options

        /// <summary>
        /// Sets the ease for the current/next step.
        /// </summary>
        public TweenBuilder WithEase(Ease ease)
        {
            _currentOptions = _currentOptions.SetEase(ease);
            return this;
        }

        /// <summary>
        /// Sets the delay for the current/next step.
        /// </summary>
        public TweenBuilder WithDelay(float delay)
        {
            _currentOptions = _currentOptions.SetDelay(delay);
            return this;
        }

        /// <summary>
        /// Sets the identifier for the tween.
        /// </summary>
        public TweenBuilder WithId(string id)
        {
            _currentOptions = _currentOptions.SetId(id);
            return this;
        }

        /// <summary>
        /// Sets the loop configuration.
        /// </summary>
        public TweenBuilder WithLoops(int loops, LoopType loopType = LoopType.Restart)
        {
            _currentOptions = _currentOptions.SetLoops(loops, loopType);
            return this;
        }

        /// <summary>
        /// Applies the specified TweenOptions.
        /// </summary>
        public TweenBuilder WithOptions(TweenOptions options)
        {
            _currentOptions = options;
            return this;
        }

        /// <summary>
        /// Uses unscaled time for the tween.
        /// </summary>
        public TweenBuilder WithUnscaledTime()
        {
            _currentOptions = _currentOptions.SetUnscaledTime(true);
            return this;
        }

        /// <summary>
        /// Enables snapping for position/rotation tweens.
        /// </summary>
        public TweenBuilder WithSnapping()
        {
            _currentOptions = _currentOptions.SetSnapping(true);
            return this;
        }

        /// <summary>
        /// Sets the tween to speed-based mode.
        /// </summary>
        public TweenBuilder WithSpeedBased()
        {
            _currentOptions = _currentOptions.SetSpeedBased(true);
            return this;
        }

        #endregion

        #region Sequence Composition

        /// <summary>
        /// Marks the next step to be appended sequentially.
        /// </summary>
        public TweenBuilder Then()
        {
            _isSequenceMode = true;
            _nextIsParallel = false;
            return this;
        }

        /// <summary>
        /// Marks the next step to run in parallel with the previous step.
        /// </summary>
        public TweenBuilder With()
        {
            _isSequenceMode = true;
            _nextIsParallel = true;
            return this;
        }

        /// <summary>
        /// Inserts a delay interval.
        /// </summary>
        public TweenBuilder Delay(float seconds)
        {
            _isSequenceMode = true;
            AddStep(() =>
            {
                // Return a dummy tween that acts as an interval marker
                return DOTween.To(() => 0f, x => { }, 1f, seconds);
            }, isInterval: true);
            return this;
        }

        /// <summary>
        /// Inserts a callback at this point in the sequence.
        /// </summary>
        public TweenBuilder Call(Action callback)
        {
            _isSequenceMode = true;
            AddStep(null, callback: callback);
            return this;
        }

        #endregion

        #region Raw DOTween Injection

        /// <summary>
        /// Appends a raw DOTween tween to the sequence.
        /// </summary>
        public TweenBuilder Then(Tween tween)
        {
            _isSequenceMode = true;
            _nextIsParallel = false;
            AddStep(() => tween);
            return this;
        }

        /// <summary>
        /// Joins a raw DOTween tween in parallel with the previous step.
        /// </summary>
        public TweenBuilder With(Tween tween)
        {
            _isSequenceMode = true;
            _nextIsParallel = true;
            AddStep(() => tween);
            return this;
        }

        #endregion

        #region Presets

        /// <summary>
        /// Plays a registered preset.
        /// </summary>
        public TweenBuilder Preset(string presetName, float? duration = null)
        {
            AddStep(() =>
            {
                var preset = TweenPresetRegistry.GetPreset(presetName);
                if (preset == null)
                {
                    Debug.LogWarning($"TweenBuilder: Preset '{presetName}' not found.");
                    return null;
                }
                return preset.CreateTween(_gameObject, duration, _currentOptions);
            });
            return this;
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Sets the OnComplete callback.
        /// </summary>
        public TweenBuilder OnComplete(Action callback)
        {
            _onComplete = callback;
            return this;
        }

        /// <summary>
        /// Sets the OnKill callback.
        /// </summary>
        public TweenBuilder OnKill(Action callback)
        {
            _onKill = callback;
            return this;
        }

        #endregion

        #region Execution

        /// <summary>
        /// Builds and plays the tween.
        /// </summary>
        public TweenHandle Play()
        {
            var tween = Build();
            tween.Tween?.Play();
            return tween;
        }

        /// <summary>
        /// Builds the tween without playing.
        /// </summary>
        public TweenHandle Build()
        {
            Tween result;

            if (_steps.Count == 0)
            {
                Debug.LogWarning("TweenBuilder: No animation steps defined.");
                return new TweenHandle(null);
            }

            if (_isSequenceMode || _steps.Count > 1)
            {
                result = BuildSequence();
            }
            else
            {
                result = BuildSingleTween();
            }

            // Apply callbacks
            if (_onComplete != null)
            {
                result?.OnComplete(() => _onComplete());
            }
            if (_onKill != null)
            {
                result?.OnKill(() => _onKill());
            }

            return new TweenHandle(result);
        }

        /// <summary>
        /// Builds and plays the tween, returning an awaitable task.
        /// </summary>
        public async Task<TweenHandle> PlayAsync(CancellationToken cancellationToken = default)
        {
            var handle = Play();
            await handle.AwaitCompletion(cancellationToken);
            return handle;
        }

        private Tween BuildSingleTween()
        {
            var step = _steps[0];
            var tween = step.TweenFactory?.Invoke();

            if (tween == null)
            {
                return null;
            }

            ApplyOptions(tween, step.Options);
            tween.SetLink(_gameObject);

            return tween;
        }

        private Sequence BuildSequence()
        {
            var sequence = DOTween.Sequence();

            foreach (var step in _steps)
            {
                if (step.Callback != null)
                {
                    if (step.IsParallel)
                    {
                        sequence.JoinCallback(() => step.Callback());
                    }
                    else
                    {
                        sequence.AppendCallback(() => step.Callback());
                    }
                    continue;
                }

                if (step.IsInterval)
                {
                    var intervalTween = step.TweenFactory?.Invoke();
                    if (intervalTween != null)
                    {
                        sequence.AppendInterval(intervalTween.Duration());
                        intervalTween.Kill();
                    }
                    continue;
                }

                var tween = step.TweenFactory?.Invoke();
                if (tween == null) continue;

                ApplyOptions(tween, step.Options);

                if (step.IsParallel)
                {
                    sequence.Join(tween);
                }
                else
                {
                    sequence.Append(tween);
                }
            }

            // Apply global options to sequence
            ApplyOptions(sequence, _currentOptions);
            sequence.SetLink(_gameObject);

            return sequence;
        }

        private void ApplyOptions(Tween tween, TweenOptions options)
        {
            var settings = TweenHelperSettings.Instance;

            // Apply ease (options override > settings default)
            var ease = options.Ease ?? settings.DefaultEase;
            tween.SetEase(ease);

            // Apply delay
            var delay = options.Delay ?? settings.DefaultDelay;
            if (delay > 0)
            {
                tween.SetDelay(delay);
            }

            // Apply update type and unscaled time
            var updateType = options.UpdateType ?? settings.DefaultUpdateType;
            var unscaledTime = options.UnscaledTime ?? settings.DefaultUnscaledTime;
            tween.SetUpdate(updateType, unscaledTime);

            // Apply loops
            if (options.Loops.HasValue)
            {
                tween.SetLoops(options.Loops.Value, options.LoopType ?? LoopType.Restart);
            }

            // Apply speed-based
            if (options.SpeedBased == true)
            {
                tween.SetSpeedBased(true);
            }

            // Apply ID
            if (!string.IsNullOrEmpty(options.Id))
            {
                tween.SetId(options.Id);
            }

            // Set target for DOTween control
            tween.SetTarget(_gameObject);
        }

        private void AddStep(Func<Tween> tweenFactory, Action callback = null, bool isInterval = false)
        {
            _steps.Add(new TweenStep
            {
                TweenFactory = tweenFactory,
                Options = _currentOptions,
                IsParallel = _nextIsParallel,
                Callback = callback,
                IsInterval = isInterval
            });

            // Reset for next step
            _nextIsParallel = false;
            _currentOptions = default;
        }

        #endregion

        #region Inner Types

        private struct TweenStep
        {
            public Func<Tween> TweenFactory;
            public TweenOptions Options;
            public bool IsParallel;
            public Action Callback;
            public bool IsInterval;
        }

        #endregion
    }
}
