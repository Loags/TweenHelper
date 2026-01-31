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

        #region Direct Preset Methods

        // Scale Presets
        /// <summary>Scales from 0 to original scale, no overshoot.</summary>
        public TweenBuilder PopIn(float? duration = null) => Preset("PopIn", duration);
        /// <summary>Scales from 0 to original scale with overshoot.</summary>
        public TweenBuilder PopInOvershoot(float? duration = null) => Preset("PopInOvershoot", duration);
        /// <summary>Scales to 0, no anticipation.</summary>
        public TweenBuilder PopOut(float? duration = null) => Preset("PopOut", duration);
        /// <summary>Scales to 0 with anticipation overshoot.</summary>
        public TweenBuilder PopOutOvershoot(float? duration = null) => Preset("PopOutOvershoot", duration);
        /// <summary>Quick scale punch for feedback.</summary>
        public TweenBuilder Punch(float? duration = null) => Preset("Punch", duration);
        /// <summary>Squash and stretch effect.</summary>
        public TweenBuilder Squash(float? duration = null) => Preset("Squash", duration);
        /// <summary>Gentle scale pulse loop.</summary>
        public TweenBuilder Breathe(float? duration = null) => Preset("Breathe", duration);
        /// <summary>Double-pulse heartbeat loop.</summary>
        public TweenBuilder Heartbeat(float? duration = null) => Preset("Heartbeat", duration);
        /// <summary>Scale from 0 with tight elastic oscillation.</summary>
        public TweenBuilder ElasticSnapIn(float? duration = null) => Preset("ElasticSnapIn", duration);
        /// <summary>Quick scale bump for UI feedback.</summary>
        public TweenBuilder PulseScale(float? duration = null) => Preset("PulseScale", duration);

        // Position Presets
        /// <summary>Random position shake.</summary>
        public TweenBuilder Shake(float? duration = null) => Preset("Shake", duration);
        /// <summary>Slides down from above.</summary>
        public TweenBuilder SlideInDown(float? duration = null) => Preset("SlideInDown", duration);
        /// <summary>Slides up from below.</summary>
        public TweenBuilder SlideInUp(float? duration = null) => Preset("SlideInUp", duration);
        /// <summary>Slides in from the left side.</summary>
        public TweenBuilder SlideInLeft(float? duration = null) => Preset("SlideInLeft", duration);
        /// <summary>Slides in from the right side.</summary>
        public TweenBuilder SlideInRight(float? duration = null) => Preset("SlideInRight", duration);
        /// <summary>Slides up off-screen.</summary>
        public TweenBuilder SlideOutUp(float? duration = null) => Preset("SlideOutUp", duration);
        /// <summary>Slides down off-screen.</summary>
        public TweenBuilder SlideOutDown(float? duration = null) => Preset("SlideOutDown", duration);
        /// <summary>Slides left off-screen.</summary>
        public TweenBuilder SlideOutLeft(float? duration = null) => Preset("SlideOutLeft", duration);
        /// <summary>Slides right off-screen.</summary>
        public TweenBuilder SlideOutRight(float? duration = null) => Preset("SlideOutRight", duration);
        /// <summary>Gentle up/down hovering loop.</summary>
        public TweenBuilder Float(float? duration = null) => Preset("Float", duration);
        /// <summary>Gentle horizontal sway loop.</summary>
        public TweenBuilder Sway(float? duration = null) => Preset("Sway", duration);
        /// <summary>Circles around a point on XZ plane.</summary>
        public TweenBuilder Orbit(float? duration = null) => Preset("Orbit", duration);
        /// <summary>Circles around a point on XZ plane with custom radius.</summary>
        public TweenBuilder Orbit(float startRadius, float? endRadius = null, float? duration = null)
        {
            AddStep(() => OrbitTweenFactory.Create(_gameObject, duration, _currentOptions, false, startRadius, endRadius ?? startRadius));
            return this;
        }
        /// <summary>Circles clockwise around a point on XZ plane.</summary>
        public TweenBuilder OrbitClockwise(float? duration = null) => Preset("OrbitClockwise", duration);
        /// <summary>Circles clockwise around a point on XZ plane with custom radius.</summary>
        public TweenBuilder OrbitClockwise(float startRadius, float? endRadius = null, float? duration = null)
        {
            AddStep(() => OrbitTweenFactory.Create(_gameObject, duration, _currentOptions, true, startRadius, endRadius ?? startRadius));
            return this;
        }
        /// <summary>Circles counter-clockwise around a point on XZ plane.</summary>
        public TweenBuilder OrbitCounterClockwise(float? duration = null) => Preset("OrbitCounterClockwise", duration);
        /// <summary>Circles counter-clockwise around a point on XZ plane with custom radius.</summary>
        public TweenBuilder OrbitCounterClockwise(float startRadius, float? endRadius = null, float? duration = null)
        {
            AddStep(() => OrbitTweenFactory.Create(_gameObject, duration, _currentOptions, false, startRadius, endRadius ?? startRadius));
            return this;
        }
        /// <summary>Spirals upward combining rotation and height.</summary>
        public TweenBuilder Spiral(float? duration = null) => Preset("Spiral", duration);
        /// <summary>Falls from above with bounce on landing.</summary>
        public TweenBuilder DropIn(float? duration = null) => Preset("DropIn", duration);
        /// <summary>Quick upward motion with ease-out.</summary>
        public TweenBuilder LaunchUp(float? duration = null) => Preset("LaunchUp", duration);
        /// <summary>Positional Y bounce with decreasing hops.</summary>
        public TweenBuilder Bounce(float? duration = null) => Preset("Bounce", duration);
        /// <summary>Tight rapid vibration.</summary>
        public TweenBuilder Jitter(float? duration = null) => Preset("Jitter", duration);
        /// <summary>Pull back then snap forward on local Z.</summary>
        public TweenBuilder Recoil(float? duration = null) => Preset("Recoil", duration);
        /// <summary>Small push right then spring back.</summary>
        public TweenBuilder Nudge(float? duration = null) => Preset("Nudge", duration);
        /// <summary>Circular orbit on XY plane.</summary>
        public TweenBuilder Orbit2D(float? duration = null) => Preset("Orbit2D", duration);
        /// <summary>Alternating diagonal zig-zag movement.</summary>
        public TweenBuilder ZigZag(float? duration = null) => Preset("ZigZag", duration);

        // Fade Presets
        /// <summary>Rapid alpha on/off loop.</summary>
        public TweenBuilder Blink(float? duration = null) => Preset("Blink", duration);
        /// <summary>Smooth alpha pulse loop.</summary>
        public TweenBuilder PulseFade(float? duration = null) => Preset("PulseFade", duration);
        /// <summary>Randomized alpha flicker effect.</summary>
        public TweenBuilder Flicker(float? duration = null) => Preset("Flicker", duration);
        /// <summary>Fade in then fade out.</summary>
        public TweenBuilder FadeInOut(float? duration = null) => Preset("FadeInOut", duration);

        // Rotation Presets
        /// <summary>Spins 360 degrees on Y axis.</summary>
        public TweenBuilder Spin(float? duration = null) => Preset("SpinY", duration);
        public TweenBuilder SpinY(float? duration = null) => Preset("SpinY", duration);
        public TweenBuilder SpinX(float? duration = null) => Preset("SpinX", duration);
        public TweenBuilder SpinZ(float? duration = null) => Preset("SpinZ", duration);
        public TweenBuilder SpinDiagonalXY(float? duration = null) => Preset("SpinDiagonalXY", duration);
        public TweenBuilder SpinDiagonalXZ(float? duration = null) => Preset("SpinDiagonalXZ", duration);
        public TweenBuilder SpinDiagonalYZ(float? duration = null) => Preset("SpinDiagonalYZ", duration);
        /// <summary>Wobbles rotation back and forth.</summary>
        public TweenBuilder Wobble(float? duration = null) => Preset("WobbleY", duration);
        public TweenBuilder WobbleY(float? duration = null) => Preset("WobbleY", duration);
        public TweenBuilder WobbleX(float? duration = null) => Preset("WobbleX", duration);
        public TweenBuilder WobbleZ(float? duration = null) => Preset("WobbleZ", duration);
        public TweenBuilder WobbleDiagonalXY(float? duration = null) => Preset("WobbleDiagonalXY", duration);
        public TweenBuilder WobbleDiagonalXZ(float? duration = null) => Preset("WobbleDiagonalXZ", duration);
        public TweenBuilder WobbleDiagonalYZ(float? duration = null) => Preset("WobbleDiagonalYZ", duration);
        /// <summary>Lean on Z then spring back.</summary>
        public TweenBuilder Tilt(float? duration = null) => Preset("Tilt", duration);
        /// <summary>180° flip on X axis.</summary>
        public TweenBuilder FlipX(float? duration = null) => Preset("FlipX", duration);
        /// <summary>180° flip on Y axis.</summary>
        public TweenBuilder FlipY(float? duration = null) => Preset("FlipY", duration);
        /// <summary>Gentle Z-axis pendulum loop.</summary>
        public TweenBuilder Rock(float? duration = null) => Preset("Rock", duration);
        /// <summary>X-axis tilt forward then spring back.</summary>
        public TweenBuilder Nod(float? duration = null) => Preset("Nod", duration);
        /// <summary>Wind up rotation then snap forward and settle.</summary>
        public TweenBuilder WindUp(float? duration = null) => Preset("WindUp", duration);

        // Combined Presets
        /// <summary>Scales and fades in together.</summary>
        public TweenBuilder PopInFade(float? duration = null) => Preset("PopInFade", duration);
        /// <summary>Scales down and fades out together, no anticipation.</summary>
        public TweenBuilder PopOutFade(float? duration = null) => Preset("PopOutFade", duration);
        /// <summary>Scales down and fades out with anticipation overshoot.</summary>
        public TweenBuilder PopOutFadeOvershoot(float? duration = null) => Preset("PopOutFadeOvershoot", duration);
        /// <summary>Attention-grabbing pulse.</summary>
        public TweenBuilder Attention(float? duration = null) => Preset("Attention", duration);
        /// <summary>Slides up from below with fade in.</summary>
        public TweenBuilder SlideInFadeUp(float? duration = null) => Preset("SlideInFadeUp", duration);
        /// <summary>Slides down from above with fade in.</summary>
        public TweenBuilder SlideInFadeDown(float? duration = null) => Preset("SlideInFadeDown", duration);
        /// <summary>Shake position with fade out.</summary>
        public TweenBuilder ShakeFade(float? duration = null) => Preset("ShakeFade", duration);
        /// <summary>Spin and shrink to zero, no anticipation.</summary>
        public TweenBuilder SpinScaleOut(float? duration = null) => Preset("SpinScaleOut", duration);
        /// <summary>Spin and shrink to zero with anticipation overshoot.</summary>
        public TweenBuilder SpinScaleOutOvershoot(float? duration = null) => Preset("SpinScaleOutOvershoot", duration);
        /// <summary>Drop with bounce and squash-stretch on landing.</summary>
        public TweenBuilder BounceLand(float? duration = null) => Preset("BounceLand", duration);
        /// <summary>Scale up and fade out simultaneously.</summary>
        public TweenBuilder Explode(float? duration = null) => Preset("Explode", duration);
        /// <summary>Spin and scale in from zero.</summary>
        public TweenBuilder SwirlIn(float? duration = null) => Preset("SwirlIn", duration);
        /// <summary>Slides in from the left with fade in.</summary>
        public TweenBuilder SlideInFadeLeft(float? duration = null) => Preset("SlideInFadeLeft", duration);
        /// <summary>Slides in from the right with fade in.</summary>
        public TweenBuilder SlideInFadeRight(float? duration = null) => Preset("SlideInFadeRight", duration);

        // Intensity Tier Presets
        /// <summary>Subtle scale punch for delicate feedback.</summary>
        public TweenBuilder PunchS(float? duration = null) => Preset("PunchS", duration);
        /// <summary>Medium scale punch for moderate feedback.</summary>
        public TweenBuilder PunchM(float? duration = null) => Preset("PunchM", duration);
        /// <summary>Heavy scale punch for emphatic feedback.</summary>
        public TweenBuilder PunchL(float? duration = null) => Preset("PunchL", duration);
        /// <summary>Light position shake.</summary>
        public TweenBuilder ShakeS(float? duration = null) => Preset("ShakeS", duration);
        /// <summary>Medium position shake.</summary>
        public TweenBuilder ShakeM(float? duration = null) => Preset("ShakeM", duration);
        /// <summary>Heavy position shake.</summary>
        public TweenBuilder ShakeL(float? duration = null) => Preset("ShakeL", duration);
        /// <summary>Subtle rapid vibration.</summary>
        public TweenBuilder JitterS(float? duration = null) => Preset("JitterS", duration);
        /// <summary>Medium rapid vibration.</summary>
        public TweenBuilder JitterM(float? duration = null) => Preset("JitterM", duration);
        /// <summary>Intense rapid vibration.</summary>
        public TweenBuilder JitterL(float? duration = null) => Preset("JitterL", duration);
        /// <summary>Subtle scale bump for light feedback.</summary>
        public TweenBuilder PulseScaleS(float? duration = null) => Preset("PulseScaleS", duration);
        /// <summary>Medium scale bump for moderate feedback.</summary>
        public TweenBuilder PulseScaleM(float? duration = null) => Preset("PulseScaleM", duration);
        /// <summary>Bold scale bump for emphatic feedback.</summary>
        public TweenBuilder PulseScaleL(float? duration = null) => Preset("PulseScaleL", duration);
        /// <summary>Subtle Z-axis wobble.</summary>
        public TweenBuilder WobbleZS(float? duration = null) => Preset("WobbleZS", duration);
        /// <summary>Medium Z-axis wobble.</summary>
        public TweenBuilder WobbleZM(float? duration = null) => Preset("WobbleZM", duration);
        /// <summary>Heavy Z-axis wobble.</summary>
        public TweenBuilder WobbleZL(float? duration = null) => Preset("WobbleZL", duration);
        /// <summary>Light bounce.</summary>
        public TweenBuilder BounceS(float? duration = null) => Preset("BounceS", duration);
        /// <summary>Medium bounce.</summary>
        public TweenBuilder BounceM(float? duration = null) => Preset("BounceM", duration);
        /// <summary>Heavy bounce with tall hops.</summary>
        public TweenBuilder BounceL(float? duration = null) => Preset("BounceL", duration);
        /// <summary>Subtle horizontal sway loop.</summary>
        public TweenBuilder SwayS(float? duration = null) => Preset("SwayS", duration);
        /// <summary>Medium horizontal sway loop.</summary>
        public TweenBuilder SwayM(float? duration = null) => Preset("SwayM", duration);
        /// <summary>Wide horizontal sway loop.</summary>
        public TweenBuilder SwayL(float? duration = null) => Preset("SwayL", duration);
        /// <summary>Subtle Z-axis pendulum loop.</summary>
        public TweenBuilder RockS(float? duration = null) => Preset("RockS", duration);
        /// <summary>Medium Z-axis pendulum loop.</summary>
        public TweenBuilder RockM(float? duration = null) => Preset("RockM", duration);
        /// <summary>Wide Z-axis pendulum loop.</summary>
        public TweenBuilder RockL(float? duration = null) => Preset("RockL", duration);
        /// <summary>Subtle forward tilt and spring back.</summary>
        public TweenBuilder NodS(float? duration = null) => Preset("NodS", duration);
        /// <summary>Medium forward tilt and spring back.</summary>
        public TweenBuilder NodM(float? duration = null) => Preset("NodM", duration);
        /// <summary>Deep forward tilt and spring back.</summary>
        public TweenBuilder NodL(float? duration = null) => Preset("NodL", duration);

        // Directional Variant Presets
        /// <summary>Small push left then spring back.</summary>
        public TweenBuilder NudgeLeft(float? duration = null) => Preset("NudgeLeft", duration);
        /// <summary>Small push right then spring back.</summary>
        public TweenBuilder NudgeRight(float? duration = null) => Preset("NudgeRight", duration);
        /// <summary>Small push up then spring back.</summary>
        public TweenBuilder NudgeUp(float? duration = null) => Preset("NudgeUp", duration);
        /// <summary>Small push down then spring back.</summary>
        public TweenBuilder NudgeDown(float? duration = null) => Preset("NudgeDown", duration);
        /// <summary>Pull forward then snap back on local Z.</summary>
        public TweenBuilder RecoilForward(float? duration = null) => Preset("RecoilForward", duration);
        /// <summary>Pull back then snap forward on local Z.</summary>
        public TweenBuilder RecoilBack(float? duration = null) => Preset("RecoilBack", duration);
        /// <summary>Quick downward motion with ease-out.</summary>
        public TweenBuilder LaunchDown(float? duration = null) => Preset("LaunchDown", duration);
        /// <summary>Quick leftward motion with ease-out.</summary>
        public TweenBuilder LaunchLeft(float? duration = null) => Preset("LaunchLeft", duration);
        /// <summary>Quick rightward motion with ease-out.</summary>
        public TweenBuilder LaunchRight(float? duration = null) => Preset("LaunchRight", duration);

        // Timing Style Presets
        /// <summary>Gentle scale entrance, no overshoot.</summary>
        public TweenBuilder PopInSoft(float? duration = null) => Preset("PopInSoft", duration);
        /// <summary>Gentle scale entrance with minimal overshoot.</summary>
        public TweenBuilder PopInSoftOvershoot(float? duration = null) => Preset("PopInSoftOvershoot", duration);
        /// <summary>Fast scale entrance, no overshoot.</summary>
        public TweenBuilder PopInHard(float? duration = null) => Preset("PopInHard", duration);
        /// <summary>Snappy scale entrance with strong overshoot.</summary>
        public TweenBuilder PopInHardOvershoot(float? duration = null) => Preset("PopInHardOvershoot", duration);
        /// <summary>Heavy drop with sharp bounce decay.</summary>
        public TweenBuilder DropInHeavy(float? duration = null) => Preset("DropInHeavy", duration);
        /// <summary>Cartoon bounce with squash-stretch on landing.</summary>
        public TweenBuilder BounceCartoon(float? duration = null) => Preset("BounceCartoon", duration);

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
