using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

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
        private TweenStep _firstStep;
        private List<TweenStep> _additionalSteps;
        private int _stepCount;
        private TweenOptions _currentOptions = default;
        private bool _isSequenceMode = false;
        private bool _nextIsParallel = false;
        private bool _isConfiguringNextStep;
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
            AddStep(options => _transform.DOMove(target, ResolveDuration(duration, options), ResolveSnapping(options)));
            return this;
        }

        /// <summary>
        /// Moves to the specified local position.
        /// </summary>
        public TweenBuilder MoveLocal(Vector3 target, float? duration = null)
        {
            AddStep(options => TweenTargetUtility.CreateLocalMoveTween(_gameObject, target, ResolveDuration(duration, options), ResolveSnapping(options)));
            return this;
        }

        /// <summary>
        /// Moves by the specified offset from current position.
        /// </summary>
        public TweenBuilder MoveBy(Vector3 offset, float? duration = null)
        {
            AddStep(options => TweenTargetUtility.CreateRelativeMoveTween(_gameObject, offset, ResolveDuration(duration, options), ResolveSnapping(options)));
            return this;
        }

        /// <summary>
        /// Moves along the X axis.
        /// </summary>
        public TweenBuilder MoveX(float target, float? duration = null)
        {
            AddStep(options => TweenTargetUtility.CreateMoveXTween(_gameObject, target, ResolveDuration(duration, options), ResolveSnapping(options)));
            return this;
        }

        /// <summary>
        /// Moves along the Y axis.
        /// </summary>
        public TweenBuilder MoveY(float target, float? duration = null)
        {
            AddStep(options => TweenTargetUtility.CreateMoveYTween(_gameObject, target, ResolveDuration(duration, options), ResolveSnapping(options)));
            return this;
        }

        /// <summary>
        /// Moves along the Z axis.
        /// </summary>
        public TweenBuilder MoveZ(float target, float? duration = null)
        {
            AddStep(options => _transform.DOMoveZ(target, ResolveDuration(duration, options), ResolveSnapping(options)));
            return this;
        }

        /// <summary>
        /// Punches the position with the specified amount.
        /// </summary>
        public TweenBuilder PunchPosition(Vector3 punch, float? duration = null, int vibrato = 10, float elasticity = 1f)
        {
            AddStep(options => _transform.DOPunchPosition(punch, ResolveDuration(duration, options), vibrato, elasticity));
            return this;
        }

        /// <summary>
        /// Shakes the position.
        /// </summary>
        public TweenBuilder ShakePosition(float? duration = null, float strength = 1f, int vibrato = 10, float randomness = 90f)
        {
            AddStep(options => _transform.DOShakePosition(ResolveDuration(duration, options), strength, vibrato, randomness));
            return this;
        }

        #endregion

        #region Rotation

        /// <summary>
        /// Rotates to the specified euler angles.
        /// </summary>
        public TweenBuilder Rotate(Vector3 target, float? duration = null, RotateMode mode = RotateMode.Fast)
        {
            AddStep(options => _transform.DORotate(target, ResolveDuration(duration, options), mode));
            return this;
        }

        /// <summary>
        /// Rotates to the specified local euler angles.
        /// </summary>
        public TweenBuilder RotateLocal(Vector3 target, float? duration = null, RotateMode mode = RotateMode.Fast)
        {
            AddStep(options => _transform.DOLocalRotate(target, ResolveDuration(duration, options), mode));
            return this;
        }

        /// <summary>
        /// Rotates by the specified euler offset.
        /// </summary>
        public TweenBuilder RotateBy(Vector3 offset, float? duration = null, RotateMode mode = RotateMode.Fast)
        {
            AddStep(options => _transform.DORotate(_transform.eulerAngles + offset, ResolveDuration(duration, options), mode));
            return this;
        }

        /// <summary>
        /// Rotates to the specified quaternion.
        /// </summary>
        public TweenBuilder RotateQuaternion(Quaternion target, float? duration = null)
        {
            AddStep(options => _transform.DORotateQuaternion(target, ResolveDuration(duration, options)));
            return this;
        }

        /// <summary>
        /// Punches the rotation with the specified amount.
        /// </summary>
        public TweenBuilder PunchRotation(Vector3 punch, float? duration = null, int vibrato = 10, float elasticity = 1f)
        {
            AddStep(options => _transform.DOPunchRotation(punch, ResolveDuration(duration, options), vibrato, elasticity));
            return this;
        }

        /// <summary>
        /// Shakes the rotation.
        /// </summary>
        public TweenBuilder ShakeRotation(float? duration = null, float strength = 90f, int vibrato = 10, float randomness = 90f)
        {
            AddStep(options => _transform.DOShakeRotation(ResolveDuration(duration, options), strength, vibrato, randomness));
            return this;
        }

        #endregion

        #region Scale

        /// <summary>
        /// Scales to the specified uniform scale.
        /// </summary>
        public TweenBuilder Scale(float target, float? duration = null)
        {
            AddStep(options => _transform.DOScale(target, ResolveDuration(duration, options)));
            return this;
        }

        /// <summary>
        /// Scales to the specified scale vector.
        /// </summary>
        public TweenBuilder Scale(Vector3 target, float? duration = null)
        {
            AddStep(options => _transform.DOScale(target, ResolveDuration(duration, options)));
            return this;
        }

        /// <summary>
        /// Scales along the X axis.
        /// </summary>
        public TweenBuilder ScaleX(float target, float? duration = null)
        {
            AddStep(options => _transform.DOScaleX(target, ResolveDuration(duration, options)));
            return this;
        }

        /// <summary>
        /// Scales along the Y axis.
        /// </summary>
        public TweenBuilder ScaleY(float target, float? duration = null)
        {
            AddStep(options => _transform.DOScaleY(target, ResolveDuration(duration, options)));
            return this;
        }

        /// <summary>
        /// Scales along the Z axis.
        /// </summary>
        public TweenBuilder ScaleZ(float target, float? duration = null)
        {
            AddStep(options => _transform.DOScaleZ(target, ResolveDuration(duration, options)));
            return this;
        }

        /// <summary>
        /// Punches the scale with the specified amount.
        /// </summary>
        public TweenBuilder PunchScale(Vector3 punch, float? duration = null, int vibrato = 10, float elasticity = 1f)
        {
            AddStep(options => _transform.DOPunchScale(punch, ResolveDuration(duration, options), vibrato, elasticity));
            return this;
        }

        /// <summary>
        /// Shakes the scale.
        /// </summary>
        public TweenBuilder ShakeScale(float? duration = null, float strength = 1f, int vibrato = 10, float randomness = 90f)
        {
            AddStep(options => _transform.DOShakeScale(ResolveDuration(duration, options), strength, vibrato, randomness));
            return this;
        }

        #endregion

        #region Fade

        /// <summary>
        /// Fades to the specified alpha. Auto-detects CanvasGroup, SpriteRenderer, Image, or Text.
        /// </summary>
        public TweenBuilder Fade(float alpha, float? duration = null)
        {
            AddStep(options => CreateFadeTween(alpha, ResolveDuration(duration, options)));
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
            var tween = TweenTargetUtility.CreateFadeTween(_gameObject, alpha, duration);
            if (tween != null)
            {
                return tween;
            }

            Debug.LogWarning($"TweenBuilder: No fadeable component found on {_gameObject.name}. " +
                           "Supported: CanvasGroup, SpriteRenderer, Graphic, TMP_Text, Renderer.");
            return null;
        }

        #endregion

        #region Color

        /// <summary>
        /// Animates color. Auto-detects SpriteRenderer, Image, or Text.
        /// </summary>
        public TweenBuilder Color(Color target, float? duration = null)
        {
            AddStep(options => CreateColorTween(target, ResolveDuration(duration, options)));
            return this;
        }

        private Tween CreateColorTween(Color target, float duration)
        {
            var tween = TweenTargetUtility.CreateColorTween(_gameObject, target, duration);
            if (tween != null)
            {
                return tween;
            }

            Debug.LogWarning($"TweenBuilder: No colorable component found on {_gameObject.name}. " +
                           "Supported: SpriteRenderer, Graphic, TMP_Text.");
            return null;
        }

        #endregion

        #region Options

        /// <summary>
        /// Sets the ease for the current/next step.
        /// </summary>
        public TweenBuilder WithEase(Ease ease)
        {
            UpdateStepOptions(options => options.SetEase(ease));
            return this;
        }

        /// <summary>
        /// Sets the delay for the current/next step.
        /// </summary>
        public TweenBuilder WithDelay(float delay)
        {
            UpdateStepOptions(options => options.SetDelay(delay));
            return this;
        }

        /// <summary>
        /// Sets the identifier for the tween.
        /// </summary>
        public TweenBuilder WithId(string id)
        {
            UpdateStepOptions(options => options.SetId(id));
            return this;
        }

        /// <summary>
        /// Sets the loop configuration.
        /// </summary>
        public TweenBuilder WithLoops(int loops, LoopType loopType = LoopType.Restart)
        {
            UpdateStepOptions(options => options.SetLoops(loops, loopType));
            return this;
        }

        /// <summary>
        /// Applies the specified TweenOptions.
        /// </summary>
        public TweenBuilder WithOptions(TweenOptions options)
        {
            SetStepOptions(options);
            return this;
        }

        /// <summary>
        /// Uses unscaled time for the tween.
        /// </summary>
        public TweenBuilder WithUnscaledTime()
        {
            UpdateStepOptions(options => options.SetUnscaledTime(true));
            return this;
        }

        /// <summary>
        /// Enables snapping for position/rotation tweens.
        /// </summary>
        public TweenBuilder WithSnapping()
        {
            UpdateStepOptions(options => options.SetSnapping(true));
            return this;
        }

        /// <summary>
        /// Sets the tween to speed-based mode.
        /// </summary>
        public TweenBuilder WithSpeedBased()
        {
            UpdateStepOptions(options => options.SetSpeedBased(true));
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
            _isConfiguringNextStep = true;
            return this;
        }

        /// <summary>
        /// Marks the next step to run in parallel with the previous step.
        /// </summary>
        public TweenBuilder With()
        {
            _isSequenceMode = true;
            _nextIsParallel = true;
            _isConfiguringNextStep = true;
            return this;
        }

        /// <summary>
        /// Inserts a delay interval.
        /// </summary>
        public TweenBuilder Delay(float seconds)
        {
            _isSequenceMode = true;
            AddStep(null, intervalDuration: seconds);
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
            AddStep(_ => tween);
            return this;
        }

        /// <summary>
        /// Joins a raw DOTween tween in parallel with the previous step.
        /// </summary>
        public TweenBuilder With(Tween tween)
        {
            _isSequenceMode = true;
            _nextIsParallel = true;
            AddStep(_ => tween);
            return this;
        }

        #endregion

        #region Presets

        /// <summary>
        /// Plays a registered preset using its concrete type.
        /// </summary>
        public TweenBuilder Preset<TPreset>(float? duration = null) where TPreset : class, ITweenPreset
        {
            var preset = TweenPresetRegistry.GetPreset<TPreset>();
            if (preset == null)
            {
                throw new InvalidOperationException($"TweenBuilder: Preset type '{typeof(TPreset).Name}' is not registered. Add [AutoRegisterPreset] or register an instance before use.");
            }

            return Preset(preset, duration);
        }

        /// <summary>
        /// Plays an already resolved preset.
        /// </summary>
        public TweenBuilder Preset(ITweenPreset preset, float? duration = null)
        {
            if (preset == null) throw new ArgumentNullException(nameof(preset));

            AddStep(options => preset.CreateTween(_gameObject, duration, options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>
        /// Plays a dynamically selected registered preset by name.
        /// </summary>
        public TweenBuilder PresetByName(string presetName, float? duration = null)
        {
            AddStep(options =>
            {
                var preset = TweenPresetRegistry.GetPresetByName(presetName);
                if (preset == null)
                {
                    Debug.LogWarning($"TweenBuilder: Preset '{presetName}' not found.");
                    return null;
                }
                return preset.CreateTween(_gameObject, duration, options);
            // Presets already resolve and apply their own defaults. Reapplying builder options here
            // breaks multi-phase/looping presets by changing only the first tween in the chain.
            }, applyBuilderOptions: false);
            return this;
        }

        /// <summary>
        /// Plays a registered preset by name. Prefer <see cref="Preset{TPreset}"/> for statically known presets.
        /// </summary>
        [Obsolete("Use Preset<TPreset>() for statically known presets or PresetByName() for dynamic names.")]
        public TweenBuilder Preset(string presetName, float? duration = null) => PresetByName(presetName, duration);

        #endregion

        #region Direct Preset Methods

        // Scale Presets
        /// <summary>Scales from 0 to original scale, no overshoot.</summary>
        public TweenBuilder PopIn(float? duration = null) => Preset<PopInPreset>(duration);
        /// <summary>Scales from 0 to original scale with overshoot.</summary>
        public TweenBuilder PopInOvershoot(float? duration = null) => Preset<PopInOvershootPreset>(duration);
        /// <summary>Scales to 0, no anticipation.</summary>
        public TweenBuilder PopOut(float? duration = null) => Preset<PopOutPreset>(duration);
        /// <summary>Scales to 0 with anticipation overshoot.</summary>
        public TweenBuilder PopOutOvershoot(float? duration = null) => Preset<PopOutOvershootPreset>(duration);
        /// <summary>Quick scale punch for feedback.</summary>
        public TweenBuilder Punch(float? duration = null) => Preset<PunchPreset>(duration);
        /// <summary>Squash and stretch effect.</summary>
        public TweenBuilder Squash(float? duration = null) => Preset<BouncePreset>(duration);
        /// <summary>Gentle scale pulse loop.</summary>
        public TweenBuilder Breathe(float? duration = null) => Preset<BreathePreset>(duration);
        /// <summary>Double-pulse heartbeat loop.</summary>
        public TweenBuilder Heartbeat(float? duration = null) => Preset<HeartbeatPreset>(duration);
        /// <summary>Quick scale bump for UI feedback.</summary>
        public TweenBuilder PulseScale(float? duration = null) => Preset<PulseScalePreset>(duration);

        // Position Presets
        /// <summary>Random position shake.</summary>
        public TweenBuilder Shake(float? duration = null) => Preset<ShakePreset>(duration);
        /// <summary>Slides down from above.</summary>
        public TweenBuilder SlideInDown(float? duration = null) => Preset<SlideInDownPreset>(duration);
        /// <summary>Slides up from below.</summary>
        public TweenBuilder SlideInUp(float? duration = null) => Preset<SlideInUpPreset>(duration);
        /// <summary>Slides in from the left side.</summary>
        public TweenBuilder SlideInLeft(float? duration = null) => Preset<SlideInLeftPreset>(duration);
        /// <summary>Slides in from the right side.</summary>
        public TweenBuilder SlideInRight(float? duration = null) => Preset<SlideInRightPreset>(duration);
        /// <summary>Slides up off-screen.</summary>
        public TweenBuilder SlideOutUp(float? duration = null) => Preset<SlideOutUpPreset>(duration);
        /// <summary>Slides down off-screen.</summary>
        public TweenBuilder SlideOutDown(float? duration = null) => Preset<SlideOutDownPreset>(duration);
        /// <summary>Slides left off-screen.</summary>
        public TweenBuilder SlideOutLeft(float? duration = null) => Preset<SlideOutLeftPreset>(duration);
        /// <summary>Slides right off-screen.</summary>
        public TweenBuilder SlideOutRight(float? duration = null) => Preset<SlideOutRightPreset>(duration);
        /// <summary>Gentle up/down hovering loop.</summary>
        public TweenBuilder Float(float? duration = null) => Preset<FloatPreset>(duration);
        /// <summary>Gentle horizontal sway loop.</summary>
        public TweenBuilder Sway(float? duration = null) => Preset<SwayPreset>(duration);
        /// <summary>Circles around a point on XZ plane.</summary>
        public TweenBuilder OrbitXZ(float? duration = null) => Preset<OrbitXZPreset>(duration);
        /// <summary>Circles around a point on XZ plane with custom radius.</summary>
        public TweenBuilder OrbitXZ(float startRadius, float? endRadius = null, float? duration = null)
        {
            AddStep(options => OrbitTweenFactory.Create(_gameObject, duration, options, false, startRadius, endRadius ?? startRadius), applyBuilderOptions: false);
            return this;
        }
        /// <summary>Circles clockwise around a point on XZ plane.</summary>
        public TweenBuilder OrbitXZClockwise(float? duration = null) => Preset<OrbitXZClockwisePreset>(duration);
        /// <summary>Circles clockwise around a point on XZ plane with custom radius.</summary>
        public TweenBuilder OrbitXZClockwise(float startRadius, float? endRadius = null, float? duration = null)
        {
            AddStep(options => OrbitTweenFactory.Create(_gameObject, duration, options, true, startRadius, endRadius ?? startRadius), applyBuilderOptions: false);
            return this;
        }
        /// <summary>Circles counter-clockwise around a point on XZ plane.</summary>
        public TweenBuilder OrbitXZCounterClockwise(float? duration = null) => Preset<OrbitXZCounterClockwisePreset>(duration);
        /// <summary>Circles counter-clockwise around a point on XZ plane with custom radius.</summary>
        public TweenBuilder OrbitXZCounterClockwise(float startRadius, float? endRadius = null, float? duration = null)
        {
            AddStep(options => OrbitTweenFactory.Create(_gameObject, duration, options, false, startRadius, endRadius ?? startRadius), applyBuilderOptions: false);
            return this;
        }
        /// <summary>Spirals upward combining rotation and height.</summary>
        public TweenBuilder Spiral(float? duration = null) => Preset<SpiralPreset>(duration);
        /// <summary>Falls from above with bounce on landing.</summary>
        public TweenBuilder DropIn(float? duration = null) => Preset<DropInPreset>(duration);
        /// <summary>Quick upward motion with ease-out.</summary>
        public TweenBuilder LaunchUp(float? duration = null) => Preset<LaunchUpPreset>(duration);
        /// <summary>Positional Y bounce with decreasing hops.</summary>
        public TweenBuilder Bounce(float? duration = null) => Preset<PositionalBouncePreset>(duration);
        /// <summary>Tight rapid vibration.</summary>
        public TweenBuilder Jitter(float? duration = null) => Preset<JitterPreset>(duration);
        /// <summary>Pull back then snap forward on local Z.</summary>
        public TweenBuilder Recoil(float? duration = null) => Preset<RecoilPreset>(duration);
        /// <summary>Small push right then spring back.</summary>
        public TweenBuilder Nudge(float? duration = null) => Preset<NudgePreset>(duration);
        /// <summary>Circular orbit on XY plane.</summary>
        public TweenBuilder OrbitXY(float? duration = null) => Preset<OrbitXYPreset>(duration);
        /// <summary>Alternating diagonal zig-zag movement.</summary>
        public TweenBuilder ZigZag(float? duration = null) => Preset<ZigZagPreset>(duration);

        // Fade Presets
        /// <summary>Rapid alpha on/off loop.</summary>
        public TweenBuilder Blink(float? duration = null) => Preset<BlinkPreset>(duration);
        /// <summary>Smooth alpha pulse loop.</summary>
        public TweenBuilder PulseFade(float? duration = null) => Preset<PulseFadePreset>(duration);
        /// <summary>Randomized alpha flicker effect.</summary>
        public TweenBuilder Flicker(float? duration = null) => Preset<FlickerPreset>(duration);
        /// <summary>Fade in then fade out.</summary>
        public TweenBuilder FadeInOut(float? duration = null) => Preset<FadeInOutPreset>(duration);

        // Rotation Presets
        /// <summary>Spins 360 degrees on Y axis.</summary>
        public TweenBuilder Spin(float? duration = null) => Preset<SpinPreset>(duration);
        public TweenBuilder SpinY(float? duration = null) => Preset<SpinPreset>(duration);
        public TweenBuilder SpinX(float? duration = null) => Preset<SpinXPreset>(duration);
        public TweenBuilder SpinZ(float? duration = null) => Preset<SpinZPreset>(duration);
        public TweenBuilder SpinDiagonalXY(float? duration = null) => Preset<SpinDiagonalXYPreset>(duration);
        public TweenBuilder SpinDiagonalXZ(float? duration = null) => Preset<SpinDiagonalXZPreset>(duration);
        public TweenBuilder SpinDiagonalYZ(float? duration = null) => Preset<SpinDiagonalYZPreset>(duration);
        /// <summary>Wobbles rotation back and forth.</summary>
        public TweenBuilder Wobble(float? duration = null) => Preset<WobblePreset>(duration);
        public TweenBuilder WobbleY(float? duration = null) => Preset<WobblePreset>(duration);
        public TweenBuilder WobbleX(float? duration = null) => Preset<WobbleXPreset>(duration);
        public TweenBuilder WobbleZ(float? duration = null) => Preset<WobbleZPreset>(duration);
        public TweenBuilder WobbleDiagonalXY(float? duration = null) => Preset<WobbleDiagonalXYPreset>(duration);
        public TweenBuilder WobbleDiagonalXZ(float? duration = null) => Preset<WobbleDiagonalXZPreset>(duration);
        public TweenBuilder WobbleDiagonalYZ(float? duration = null) => Preset<WobbleDiagonalYZPreset>(duration);
        /// <summary>Lean on Z then spring back.</summary>
        public TweenBuilder Tilt(float? duration = null) => Preset<TiltPreset>(duration);
        /// <summary>180° flip on X axis.</summary>
        public TweenBuilder FlipX(float? duration = null) => Preset<FlipXPreset>(duration);
        /// <summary>180° flip on Y axis.</summary>
        public TweenBuilder FlipY(float? duration = null) => Preset<FlipYPreset>(duration);
        /// <summary>Gentle Z-axis pendulum loop.</summary>
        public TweenBuilder PendulumZ(float? duration = null) => Preset<PendulumZPreset>(duration);
        /// <summary>X-axis tilt forward then spring back.</summary>
        public TweenBuilder Nod(float? duration = null) => Preset<NodPreset>(duration);

        // Combined Presets
        /// <summary>Scales and fades in together.</summary>
        public TweenBuilder PopInFade(float? duration = null) => Preset<PopInFadePreset>(duration);
        /// <summary>Scales down and fades out together, no anticipation.</summary>
        public TweenBuilder PopOutFade(float? duration = null) => Preset<PopOutFadePreset>(duration);
        /// <summary>Scales down and fades out with anticipation overshoot.</summary>
        public TweenBuilder PopOutFadeOvershoot(float? duration = null) => Preset<PopOutFadeOvershootPreset>(duration);
        /// <summary>Soft scales and fades in together.</summary>
        public TweenBuilder PopInFadeSoft(float? duration = null) => Preset<PopInFadeSoftPreset>(duration);
        /// <summary>Hard scales and fades in together.</summary>
        public TweenBuilder PopInFadeHard(float? duration = null) => Preset<PopInFadeHardPreset>(duration);
        /// <summary>Soft scales down and fades out together.</summary>
        public TweenBuilder PopOutFadeSoft(float? duration = null) => Preset<PopOutFadeSoftPreset>(duration);
        /// <summary>Hard scales down and fades out together.</summary>
        public TweenBuilder PopOutFadeHard(float? duration = null) => Preset<PopOutFadeHardPreset>(duration);
        /// <summary>Soft scales down and fades out with mild overshoot.</summary>
        public TweenBuilder PopOutFadeOvershootSoft(float? duration = null) => Preset<PopOutFadeOvershootSoftPreset>(duration);
        /// <summary>Hard scales down and fades out with strong overshoot.</summary>
        public TweenBuilder PopOutFadeOvershootHard(float? duration = null) => Preset<PopOutFadeOvershootHardPreset>(duration);
        /// <summary>Attention-grabbing pulse.</summary>
        public TweenBuilder Attention(float? duration = null) => Preset<AttentionPreset>(duration);
        /// <summary>Slides up from below with fade in.</summary>
        public TweenBuilder SlideInFadeUp(float? duration = null) => Preset<SlideInFadeUpPreset>(duration);
        /// <summary>Slides down from above with fade in.</summary>
        public TweenBuilder SlideInFadeDown(float? duration = null) => Preset<SlideInFadeDownPreset>(duration);
        /// <summary>Shake position with fade out.</summary>
        public TweenBuilder ShakeFade(float? duration = null) => Preset<ShakeFadePreset>(duration);
        /// <summary>Spin and shrink to zero, no anticipation.</summary>
        public TweenBuilder SpinScaleOut(float? duration = null) => Preset<SpinScaleOutPreset>(duration);
        /// <summary>Spin and shrink to zero with anticipation overshoot.</summary>
        public TweenBuilder SpinScaleOutOvershoot(float? duration = null) => Preset<SpinScaleOutOvershootPreset>(duration);
        /// <summary>Drop with bounce and squash-stretch on landing.</summary>
        public TweenBuilder BounceLand(float? duration = null) => Preset<BounceLandPreset>(duration);
        /// <summary>Scale up and fade out simultaneously.</summary>
        public TweenBuilder Explode(float? duration = null) => Preset<ExplodePreset>(duration);
        /// <summary>Spin and scale in from zero.</summary>
        public TweenBuilder SwirlIn(float? duration = null) => Preset<SwirlInPreset>(duration);
        /// <summary>Slides in from the left with fade in.</summary>
        public TweenBuilder SlideInFadeLeft(float? duration = null) => Preset<SlideInFadeLeftPreset>(duration);
        /// <summary>Slides in from the right with fade in.</summary>
        public TweenBuilder SlideInFadeRight(float? duration = null) => Preset<SlideInFadeRightPreset>(duration);

        // Soft/Hard Intensity Presets
        /// <summary>Soft scale punch for delicate feedback.</summary>
        public TweenBuilder PunchSoft(float? duration = null) => Preset<PunchSoftPreset>(duration);
        /// <summary>Heavy scale punch for emphatic feedback.</summary>
        public TweenBuilder PunchHard(float? duration = null) => Preset<PunchHardPreset>(duration);
        /// <summary>Soft position shake.</summary>
        public TweenBuilder ShakeSoft(float? duration = null) => Preset<ShakeSoftPreset>(duration);
        /// <summary>Heavy position shake.</summary>
        public TweenBuilder ShakeHard(float? duration = null) => Preset<ShakeHardPreset>(duration);
        /// <summary>Soft shake with fade out.</summary>
        public TweenBuilder ShakeFadeSoft(float? duration = null) => Preset<ShakeFadeSoftPreset>(duration);
        /// <summary>Hard shake with fade out.</summary>
        public TweenBuilder ShakeFadeHard(float? duration = null) => Preset<ShakeFadeHardPreset>(duration);
        /// <summary>Soft rapid vibration.</summary>
        public TweenBuilder JitterSoft(float? duration = null) => Preset<JitterSoftPreset>(duration);
        /// <summary>Intense rapid vibration.</summary>
        public TweenBuilder JitterHard(float? duration = null) => Preset<JitterHardPreset>(duration);
        /// <summary>Soft scale bump for light feedback.</summary>
        public TweenBuilder PulseScaleSoft(float? duration = null) => Preset<PulseScaleSoftPreset>(duration);
        /// <summary>Bold scale bump for emphatic feedback.</summary>
        public TweenBuilder PulseScaleHard(float? duration = null) => Preset<PulseScaleHardPreset>(duration);
        /// <summary>Soft Z-axis wobble.</summary>
        public TweenBuilder WobbleZSoft(float? duration = null) => Preset<WobbleZSoftPreset>(duration);
        /// <summary>Heavy Z-axis wobble.</summary>
        public TweenBuilder WobbleZHard(float? duration = null) => Preset<WobbleZHardPreset>(duration);
        /// <summary>Soft Y-axis wobble.</summary>
        public TweenBuilder WobbleYSoft(float? duration = null) => Preset<WobbleYSoftPreset>(duration);
        /// <summary>Heavy Y-axis wobble.</summary>
        public TweenBuilder WobbleYHard(float? duration = null) => Preset<WobbleYHardPreset>(duration);
        /// <summary>Soft X-axis wobble.</summary>
        public TweenBuilder WobbleXSoft(float? duration = null) => Preset<WobbleXSoftPreset>(duration);
        /// <summary>Heavy X-axis wobble.</summary>
        public TweenBuilder WobbleXHard(float? duration = null) => Preset<WobbleXHardPreset>(duration);
        /// <summary>Soft bounce.</summary>
        public TweenBuilder BounceSoft(float? duration = null) => Preset<BounceSoftPreset>(duration);
        /// <summary>Heavy bounce with tall hops.</summary>
        public TweenBuilder BounceHard(float? duration = null) => Preset<BounceHardPreset>(duration);
        /// <summary>Soft squash and stretch.</summary>
        public TweenBuilder SquashSoft(float? duration = null) => Preset<SquashSoftPreset>(duration);
        /// <summary>Hard squash and stretch.</summary>
        public TweenBuilder SquashHard(float? duration = null) => Preset<SquashHardPreset>(duration);
        /// <summary>Soft horizontal sway loop.</summary>
        public TweenBuilder SwaySoft(float? duration = null) => Preset<SwaySoftPreset>(duration);
        /// <summary>Wide horizontal sway loop.</summary>
        public TweenBuilder SwayHard(float? duration = null) => Preset<SwayHardPreset>(duration);
        /// <summary>Soft Z-axis pendulum loop.</summary>
        public TweenBuilder PendulumZSoft(float? duration = null) => Preset<PendulumZSoftPreset>(duration);
        /// <summary>Wide Z-axis pendulum loop.</summary>
        public TweenBuilder PendulumZHard(float? duration = null) => Preset<PendulumZHardPreset>(duration);
        /// <summary>Soft forward tilt and spring back.</summary>
        public TweenBuilder NodSoft(float? duration = null) => Preset<NodSoftPreset>(duration);
        /// <summary>Deep forward tilt and spring back.</summary>
        public TweenBuilder NodHard(float? duration = null) => Preset<NodHardPreset>(duration);
        /// <summary>Soft recoil pull back.</summary>
        public TweenBuilder RecoilSoft(float? duration = null) => Preset<RecoilSoftPreset>(duration);
        /// <summary>Hard recoil pull back.</summary>
        public TweenBuilder RecoilHard(float? duration = null) => Preset<RecoilHardPreset>(duration);
        /// <summary>Soft gentle scale pulse loop.</summary>
        public TweenBuilder BreatheSoft(float? duration = null) => Preset<BreatheSoftPreset>(duration);
        /// <summary>Hard intense scale pulse loop.</summary>
        public TweenBuilder BreatheHard(float? duration = null) => Preset<BreatheHardPreset>(duration);
        /// <summary>Soft double-pulse heartbeat loop.</summary>
        public TweenBuilder HeartbeatSoft(float? duration = null) => Preset<HeartbeatSoftPreset>(duration);
        /// <summary>Hard intense heartbeat loop.</summary>
        public TweenBuilder HeartbeatHard(float? duration = null) => Preset<HeartbeatHardPreset>(duration);
        /// <summary>Soft gentle hovering loop.</summary>
        public TweenBuilder FloatSoft(float? duration = null) => Preset<FloatSoftPreset>(duration);
        /// <summary>Hard pronounced hovering loop.</summary>
        public TweenBuilder FloatHard(float? duration = null) => Preset<FloatHardPreset>(duration);
        /// <summary>Soft alternating diagonal movement.</summary>
        public TweenBuilder ZigZagSoft(float? duration = null) => Preset<ZigZagSoftPreset>(duration);
        /// <summary>Hard alternating diagonal movement.</summary>
        public TweenBuilder ZigZagHard(float? duration = null) => Preset<ZigZagHardPreset>(duration);
        /// <summary>Soft attention-grabbing pulse.</summary>
        public TweenBuilder AttentionSoft(float? duration = null) => Preset<AttentionSoftPreset>(duration);
        /// <summary>Hard attention-grabbing pulse.</summary>
        public TweenBuilder AttentionHard(float? duration = null) => Preset<AttentionHardPreset>(duration);
        /// <summary>Soft lean on Z then spring back.</summary>
        public TweenBuilder TiltSoft(float? duration = null) => Preset<TiltSoftPreset>(duration);
        /// <summary>Hard lean on Z then spring back.</summary>
        public TweenBuilder TiltHard(float? duration = null) => Preset<TiltHardPreset>(duration);
        /// <summary>Soft spin and shrink to zero.</summary>
        public TweenBuilder SpinScaleOutSoft(float? duration = null) => Preset<SpinScaleOutSoftPreset>(duration);
        /// <summary>Hard spin and shrink to zero.</summary>
        public TweenBuilder SpinScaleOutHard(float? duration = null) => Preset<SpinScaleOutHardPreset>(duration);
        /// <summary>Soft spin and shrink with mild anticipation.</summary>
        public TweenBuilder SpinScaleOutOvershootSoft(float? duration = null) => Preset<SpinScaleOutOvershootSoftPreset>(duration);
        /// <summary>Hard spin and shrink with strong anticipation.</summary>
        public TweenBuilder SpinScaleOutOvershootHard(float? duration = null) => Preset<SpinScaleOutOvershootHardPreset>(duration);
        /// <summary>Soft scale exit, no anticipation.</summary>
        public TweenBuilder PopOutSoft(float? duration = null) => Preset<PopOutSoftPreset>(duration);
        /// <summary>Hard scale exit, no anticipation.</summary>
        public TweenBuilder PopOutHard(float? duration = null) => Preset<PopOutHardPreset>(duration);
        /// <summary>Soft scale exit with mild anticipation.</summary>
        public TweenBuilder PopOutOvershootSoft(float? duration = null) => Preset<PopOutOvershootSoftPreset>(duration);
        /// <summary>Hard scale exit with strong anticipation.</summary>
        public TweenBuilder PopOutOvershootHard(float? duration = null) => Preset<PopOutOvershootHardPreset>(duration);

        // Directional Variant Presets
        /// <summary>Small push left then spring back.</summary>
        public TweenBuilder NudgeLeft(float? duration = null) => Preset<NudgeLeftPreset>(duration);
        /// <summary>Small push right then spring back.</summary>
        public TweenBuilder NudgeRight(float? duration = null) => Preset<NudgeRightPreset>(duration);
        /// <summary>Small push up then spring back.</summary>
        public TweenBuilder NudgeUp(float? duration = null) => Preset<NudgeUpPreset>(duration);
        /// <summary>Small push down then spring back.</summary>
        public TweenBuilder NudgeDown(float? duration = null) => Preset<NudgeDownPreset>(duration);
        /// <summary>Pull forward then snap back on local Z.</summary>
        public TweenBuilder RecoilForward(float? duration = null) => Preset<RecoilForwardPreset>(duration);
        /// <summary>Pull back then snap forward on local Z.</summary>
        public TweenBuilder RecoilBack(float? duration = null) => Preset<RecoilBackPreset>(duration);
        /// <summary>Quick downward motion with ease-out.</summary>
        public TweenBuilder LaunchDown(float? duration = null) => Preset<LaunchDownPreset>(duration);
        /// <summary>Quick leftward motion with ease-out.</summary>
        public TweenBuilder LaunchLeft(float? duration = null) => Preset<LaunchLeftPreset>(duration);
        /// <summary>Quick rightward motion with ease-out.</summary>
        public TweenBuilder LaunchRight(float? duration = null) => Preset<LaunchRightPreset>(duration);

        // Timing Style Presets
        /// <summary>Gentle scale entrance, no overshoot.</summary>
        public TweenBuilder PopInSoft(float? duration = null) => Preset<PopInSoftPreset>(duration);
        /// <summary>Fast scale entrance, no overshoot.</summary>
        public TweenBuilder PopInHard(float? duration = null) => Preset<PopInHardPreset>(duration);
        /// <summary>Gentle scale entrance with minimal overshoot.</summary>
        public TweenBuilder PopInOvershootSoft(float? duration = null) => Preset<PopInOvershootSoftPreset>(duration);
        /// <summary>Snappy scale entrance with strong overshoot.</summary>
        public TweenBuilder PopInOvershootHard(float? duration = null) => Preset<PopInOvershootHardPreset>(duration);
        /// <summary>Gentle drop with soft bounce on landing.</summary>
        public TweenBuilder DropInSoft(float? duration = null) => Preset<DropInSoftPreset>(duration);
        /// <summary>Heavy drop with sharp bounce decay.</summary>
        public TweenBuilder DropInHard(float? duration = null) => Preset<DropInHardPreset>(duration);
        /// <summary>Cartoon bounce with squash-stretch on landing.</summary>
        public TweenBuilder BounceCartoon(float? duration = null) => Preset<BounceCartoonPreset>(duration);

        #endregion

        #region Callbacks

        /// <summary>
        /// Sets the OnComplete callback.
        /// </summary>
        public TweenBuilder OnComplete(Action callback)
        {
            _onComplete += callback;
            return this;
        }

        /// <summary>
        /// Sets the OnKill callback.
        /// </summary>
        public TweenBuilder OnKill(Action callback)
        {
            _onKill += callback;
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

            if (_stepCount == 0)
            {
                Debug.LogWarning("TweenBuilder: No animation steps defined.");
                return new TweenHandle(null);
            }

            if (_isSequenceMode || _stepCount > 1)
            {
                result = BuildSequence();
            }
            else
            {
                result = BuildSingleTween();
            }

            // Apply callbacks
            if (result != null && _onComplete != null)
            {
                result.onComplete += _onComplete.Invoke;
            }
            if (result != null && _onKill != null)
            {
                result.onKill += _onKill.Invoke;
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
            var step = _firstStep;
            var tween = step.TweenFactory?.Invoke(step.Options);

            if (tween == null)
            {
                return null;
            }

            if (step.ApplyBuilderOptions)
            {
                ApplyOptions(tween, step.Options);
            }
            tween.SetLink(_gameObject);

            return tween;
        }

        private Sequence BuildSequence()
        {
            var sequence = DOTween.Sequence();

            for (int i = 0; i < _stepCount; i++)
            {
                var step = GetStep(i);
                if (step.Callback != null)
                {
                    if (step.IsParallel)
                    {
                        sequence.JoinCallback(step.Callback.Invoke);
                    }
                    else
                    {
                        sequence.AppendCallback(step.Callback.Invoke);
                    }
                    continue;
                }

                if (step.IntervalDuration.HasValue)
                {
                    sequence.AppendInterval(step.IntervalDuration.Value);
                    continue;
                }

                var tween = step.TweenFactory?.Invoke(step.Options);
                if (tween == null) continue;

                if (step.ApplyBuilderOptions)
                {
                    ApplyOptions(tween, step.Options);
                }

                if (step.IsParallel)
                {
                    sequence.Join(tween);
                }
                else
                {
                    sequence.Append(tween);
                }
            }

            sequence.SetTarget(_gameObject);
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

        private float ResolveDuration(float? explicitDuration, TweenOptions options)
        {
            return explicitDuration ?? options.Duration ?? TweenHelperSettings.Instance.DefaultDuration;
        }

        private bool ResolveSnapping(TweenOptions options) => options.Snapping ?? TweenHelperSettings.Instance.DefaultSnapping;

        private void UpdateStepOptions(Func<TweenOptions, TweenOptions> update)
        {
            if (_stepCount > 0 && !_isConfiguringNextStep)
            {
                int index = _stepCount - 1;
                var step = GetStep(index);
                step.Options = update(step.Options);
                SetStep(index, step);
                return;
            }

            _currentOptions = update(_currentOptions);
        }

        private void SetStepOptions(TweenOptions options)
        {
            if (_stepCount > 0 && !_isConfiguringNextStep)
            {
                int index = _stepCount - 1;
                var step = GetStep(index);
                step.Options = options;
                SetStep(index, step);
                return;
            }

            _currentOptions = options;
        }

        private void AddStep(Func<TweenOptions, Tween> tweenFactory, Action callback = null, float? intervalDuration = null, bool applyBuilderOptions = true)
        {
            var step = new TweenStep
            {
                TweenFactory = tweenFactory,
                Options = _currentOptions,
                ApplyBuilderOptions = applyBuilderOptions,
                IsParallel = _nextIsParallel,
                Callback = callback,
                IntervalDuration = intervalDuration
            };

            if (_stepCount == 0)
            {
                _firstStep = step;
            }
            else
            {
                _additionalSteps ??= new List<TweenStep>(3);
                _additionalSteps.Add(step);
            }
            _stepCount++;

            // Reset for next step
            _nextIsParallel = false;
            _currentOptions = default;
            _isConfiguringNextStep = false;
        }

        private TweenStep GetStep(int index) => index == 0 ? _firstStep : _additionalSteps[index - 1];

        private void SetStep(int index, TweenStep step)
        {
            if (index == 0)
            {
                _firstStep = step;
                return;
            }

            _additionalSteps[index - 1] = step;
        }

        #endregion

        #region Inner Types

        private struct TweenStep
        {
            public Func<TweenOptions, Tween> TweenFactory;
            public TweenOptions Options;
            public bool ApplyBuilderOptions;
            public bool IsParallel;
            public Action Callback;
            public float? IntervalDuration;
        }

        #endregion
    }
}
