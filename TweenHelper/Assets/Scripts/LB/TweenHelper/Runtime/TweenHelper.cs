using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper
{
    /// <summary>
    /// Main public interface for TweenHelper.
    /// Provides clear, intention-revealing one-liner methods for common animations.
    /// </summary>
    public static class TweenHelper
    {
        #region Transform Movement
        
        /// <summary>
        /// Moves a Transform to the specified position.
        /// </summary>
        /// <param name="transform">The Transform to move.</param>
        /// <param name="targetPosition">The target position.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The movement tween.</returns>
        public static Tween MoveTo(Transform transform, Vector3 targetPosition, float? duration = null, TweenOptions options = default)
        {
            return DoTweenIntegration.CreateMoveTween(transform, targetPosition, duration, options);
        }
        
        /// <summary>
        /// Moves a Transform by the specified offset from its current position.
        /// </summary>
        /// <param name="transform">The Transform to move.</param>
        /// <param name="offset">The offset to move by.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The movement tween.</returns>
        public static Tween MoveBy(Transform transform, Vector3 offset, float? duration = null, TweenOptions options = default)
        {
            if (!DoTweenIntegration.ValidateTarget(transform, "MoveBy")) return null;
            return MoveTo(transform, transform.position + offset, duration, options);
        }
        
        /// <summary>
        /// Moves a Transform to the specified local position.
        /// </summary>
        /// <param name="transform">The Transform to move.</param>
        /// <param name="targetLocalPosition">The target local position.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The movement tween.</returns>
        public static Tween MoveToLocal(Transform transform, Vector3 targetLocalPosition, float? duration = null, TweenOptions options = default)
        {
            if (!DoTweenIntegration.ValidateTarget(transform, "MoveToLocal")) return null;
            
            var actualDuration = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            var tween = transform.DOLocalMove(targetLocalPosition, actualDuration);
            DoTweenIntegration.ConfigureTween(tween, transform.gameObject, duration, options);
            return DoTweenIntegration.ApplySnapping(tween, options);
        }
        
        #endregion
        
        #region Transform Rotation
        
        /// <summary>
        /// Rotates a Transform to the specified rotation (in Euler angles).
        /// </summary>
        /// <param name="transform">The Transform to rotate.</param>
        /// <param name="targetRotation">The target rotation in Euler angles.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The rotation tween.</returns>
        public static Tween RotateTo(Transform transform, Vector3 targetRotation, float? duration = null, TweenOptions options = default)
        {
            return DoTweenIntegration.CreateRotateTween(transform, targetRotation, duration, options);
        }
        
        /// <summary>
        /// Rotates a Transform by the specified angle offset.
        /// </summary>
        /// <param name="transform">The Transform to rotate.</param>
        /// <param name="angleOffset">The angle offset to rotate by.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The rotation tween.</returns>
        public static Tween RotateBy(Transform transform, Vector3 angleOffset, float? duration = null, TweenOptions options = default)
        {
            if (!DoTweenIntegration.ValidateTarget(transform, "RotateBy")) return null;
            return RotateTo(transform, transform.eulerAngles + angleOffset, duration, options);
        }
        
        /// <summary>
        /// Rotates a Transform to look at the specified target.
        /// </summary>
        /// <param name="transform">The Transform to rotate.</param>
        /// <param name="target">The target to look at.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The rotation tween.</returns>
        public static Tween LookAt(Transform transform, Vector3 target, float? duration = null, TweenOptions options = default)
        {
            if (!DoTweenIntegration.ValidateTarget(transform, "LookAt")) return null;
            
            var direction = (target - transform.position).normalized;
            var targetRotation = Quaternion.LookRotation(direction);
            return RotateTo(transform, targetRotation.eulerAngles, duration, options);
        }
        
        #endregion
        
        #region Transform Scaling
        
        /// <summary>
        /// Scales a Transform to the specified scale.
        /// </summary>
        /// <param name="transform">The Transform to scale.</param>
        /// <param name="targetScale">The target scale.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The scale tween.</returns>
        public static Tween ScaleTo(Transform transform, Vector3 targetScale, float? duration = null, TweenOptions options = default)
        {
            return DoTweenIntegration.CreateScaleTween(transform, targetScale, duration, options);
        }
        
        /// <summary>
        /// Scales a Transform uniformly to the specified scale value.
        /// </summary>
        /// <param name="transform">The Transform to scale.</param>
        /// <param name="targetScale">The uniform target scale value.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The scale tween.</returns>
        public static Tween ScaleTo(Transform transform, float targetScale, float? duration = null, TweenOptions options = default)
        {
            return ScaleTo(transform, Vector3.one * targetScale, duration, options);
        }
        
        /// <summary>
        /// Scales a Transform by the specified multiplier.
        /// </summary>
        /// <param name="transform">The Transform to scale.</param>
        /// <param name="scaleMultiplier">The scale multiplier.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The scale tween.</returns>
        public static Tween ScaleBy(Transform transform, Vector3 scaleMultiplier, float? duration = null, TweenOptions options = default)
        {
            if (!DoTweenIntegration.ValidateTarget(transform, "ScaleBy")) return null;
            var currentScale = transform.localScale;
            var targetScale = new Vector3(
                currentScale.x * scaleMultiplier.x,
                currentScale.y * scaleMultiplier.y,
                currentScale.z * scaleMultiplier.z
            );
            return ScaleTo(transform, targetScale, duration, options);
        }
        
        /// <summary>
        /// Scales a Transform uniformly by the specified multiplier.
        /// </summary>
        /// <param name="transform">The Transform to scale.</param>
        /// <param name="scaleMultiplier">The uniform scale multiplier.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The scale tween.</returns>
        public static Tween ScaleBy(Transform transform, float scaleMultiplier, float? duration = null, TweenOptions options = default)
        {
            return ScaleBy(transform, Vector3.one * scaleMultiplier, duration, options);
        }
        
        #endregion
        
        #region Fading
        
        /// <summary>
        /// Fades a CanvasGroup to the specified alpha value.
        /// </summary>
        /// <param name="canvasGroup">The CanvasGroup to fade.</param>
        /// <param name="targetAlpha">The target alpha value (0-1).</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The fade tween.</returns>
        public static Tween FadeTo(CanvasGroup canvasGroup, float targetAlpha, float? duration = null, TweenOptions options = default)
        {
            if (!DoTweenIntegration.ValidateTarget(canvasGroup, "FadeTo")) return null;
            
            var actualDuration = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            var tween = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, targetAlpha, actualDuration);
            return DoTweenIntegration.ConfigureTween(tween, canvasGroup.gameObject, duration, options);
        }
        
        /// <summary>
        /// Fades a SpriteRenderer to the specified alpha value.
        /// </summary>
        /// <param name="spriteRenderer">The SpriteRenderer to fade.</param>
        /// <param name="targetAlpha">The target alpha value (0-1).</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The fade tween.</returns>
        public static Tween FadeTo(SpriteRenderer spriteRenderer, float targetAlpha, float? duration = null, TweenOptions options = default)
        {
            if (!DoTweenIntegration.ValidateTarget(spriteRenderer, "FadeTo")) return null;
            
            var actualDuration = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            var tween = DOTween.To(() => spriteRenderer.color.a, 
                                 a => { var c = spriteRenderer.color; c.a = a; spriteRenderer.color = c; }, 
                                 targetAlpha, actualDuration);
            return DoTweenIntegration.ConfigureTween(tween, spriteRenderer.gameObject, duration, options);
        }
        
        /// <summary>
        /// Fades a UI Image to the specified alpha value.
        /// </summary>
        /// <param name="image">The Image to fade.</param>
        /// <param name="targetAlpha">The target alpha value (0-1).</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The fade tween.</returns>
        public static Tween FadeTo(Image image, float targetAlpha, float? duration = null, TweenOptions options = default)
        {
            if (!DoTweenIntegration.ValidateTarget(image, "FadeTo")) return null;
            
            var actualDuration = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            var tween = DOTween.To(() => image.color.a, 
                                 a => { var c = image.color; c.a = a; image.color = c; }, 
                                 targetAlpha, actualDuration);
            return DoTweenIntegration.ConfigureTween(tween, image.gameObject, duration, options);
        }
        
        /// <summary>
        /// Fades a UI Text to the specified alpha value.
        /// </summary>
        /// <param name="text">The Text to fade.</param>
        /// <param name="targetAlpha">The target alpha value (0-1).</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The fade tween.</returns>
        public static Tween FadeTo(Text text, float targetAlpha, float? duration = null, TweenOptions options = default)
        {
            if (!DoTweenIntegration.ValidateTarget(text, "FadeTo")) return null;
            
            var actualDuration = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            var tween = DOTween.To(() => text.color.a, 
                                 a => { var c = text.color; c.a = a; text.color = c; }, 
                                 targetAlpha, actualDuration);
            return DoTweenIntegration.ConfigureTween(tween, text.gameObject, duration, options);
        }
        
        /// <summary>
        /// Fades in a CanvasGroup (alpha to 1).
        /// </summary>
        /// <param name="canvasGroup">The CanvasGroup to fade in.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The fade tween.</returns>
        public static Tween FadeIn(CanvasGroup canvasGroup, float? duration = null, TweenOptions options = default)
        {
            return FadeTo(canvasGroup, 1f, duration, options);
        }
        
        /// <summary>
        /// Fades out a CanvasGroup (alpha to 0).
        /// </summary>
        /// <param name="canvasGroup">The CanvasGroup to fade out.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The fade tween.</returns>
        public static Tween FadeOut(CanvasGroup canvasGroup, float? duration = null, TweenOptions options = default)
        {
            return FadeTo(canvasGroup, 0f, duration, options);
        }
        
        #endregion
        
        #region Common Animation Patterns
        
        /// <summary>
        /// Creates a "pop" effect by scaling from zero to the target scale.
        /// </summary>
        /// <param name="transform">The Transform to animate.</param>
        /// <param name="targetScale">The target scale (default is original scale).</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The pop animation tween.</returns>
        public static Tween PopIn(Transform transform, Vector3? targetScale = null, float? duration = null, TweenOptions options = default)
        {
            if (!DoTweenIntegration.ValidateTarget(transform, "PopIn")) return null;
            
            var finalScale = targetScale ?? transform.localScale;
            transform.localScale = Vector3.zero;
            
            var popOptions = options.SetEase(Ease.OutBack);
            return ScaleTo(transform, finalScale, duration, popOptions);
        }
        
        /// <summary>
        /// Creates a "pop out" effect by scaling to zero.
        /// </summary>
        /// <param name="transform">The Transform to animate.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The pop animation tween.</returns>
        public static Tween PopOut(Transform transform, float? duration = null, TweenOptions options = default)
        {
            var popOptions = options.SetEase(Ease.InBack);
            return ScaleTo(transform, Vector3.zero, duration, popOptions);
        }
        
        /// <summary>
        /// Creates a "bounce" effect by scaling up slightly then back to original.
        /// </summary>
        /// <param name="transform">The Transform to animate.</param>
        /// <param name="bounceScale">The scale multiplier for the bounce (default 1.1).</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The bounce animation sequence.</returns>
        public static Sequence Bounce(Transform transform, float bounceScale = 1.1f, float? duration = null, TweenOptions options = default)
        {
            if (!DoTweenIntegration.ValidateTarget(transform, "Bounce")) return null;
            
            var originalScale = transform.localScale;
            var actualDuration = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            
            var sequence = DOTween.Sequence();
            sequence.Append(ScaleTo(transform, originalScale * bounceScale, actualDuration * 0.5f, options.SetEase(Ease.OutQuad)));
            sequence.Append(ScaleTo(transform, originalScale, actualDuration * 0.5f, options.SetEase(Ease.InQuad)));
            
            return DoTweenIntegration.ConfigureSequence(sequence, transform.gameObject, options);
        }
        
        /// <summary>
        /// Creates a "shake" effect by moving the transform randomly within a radius.
        /// </summary>
        /// <param name="transform">The Transform to shake.</param>
        /// <param name="strength">The strength of the shake effect.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="vibrato">The number of vibrations (default 10).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The shake animation tween.</returns>
        public static Tween Shake(Transform transform, float strength = 1f, float? duration = null, int vibrato = 10, TweenOptions options = default)
        {
            if (!DoTweenIntegration.ValidateTarget(transform, "Shake")) return null;
            
            var actualDuration = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            var tween = transform.DOShakePosition(actualDuration, strength, vibrato);
            return DoTweenIntegration.ConfigureTween(tween, transform.gameObject, duration, options);
        }
        
        #endregion
        
        #region Presets
        
        /// <summary>
        /// Plays a named preset on the specified GameObject.
        /// </summary>
        /// <param name="presetName">The name of the preset to play.</param>
        /// <param name="target">The GameObject to animate.</param>
        /// <param name="duration">Duration override (null uses preset default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The created tween, or null if the preset was not found.</returns>
        public static Tween PlayPreset(string presetName, GameObject target, float? duration = null, TweenOptions options = default)
        {
            return TweenPresetRegistry.PlayPreset(presetName, target, duration, options);
        }
        
        /// <summary>
        /// Plays a named preset on the specified Transform's GameObject.
        /// </summary>
        /// <param name="presetName">The name of the preset to play.</param>
        /// <param name="target">The Transform whose GameObject to animate.</param>
        /// <param name="duration">Duration override (null uses preset default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The created tween, or null if the preset was not found.</returns>
        public static Tween PlayPreset(string presetName, Transform target, float? duration = null, TweenOptions options = default)
        {
            if (target == null)
            {
                Debug.LogWarning("TweenHelper: Cannot play preset on null Transform.");
                return null;
            }
            return PlayPreset(presetName, target.gameObject, duration, options);
        }
        
        /// <summary>
        /// Gets a list of all available preset names.
        /// </summary>
        /// <returns>An enumerable of preset names.</returns>
        public static System.Collections.Generic.IEnumerable<string> GetAvailablePresets()
        {
            return TweenPresetRegistry.PresetNames;
        }
        
        /// <summary>
        /// Gets presets that are compatible with the specified GameObject.
        /// </summary>
        /// <param name="target">The GameObject to check compatibility for.</param>
        /// <returns>An enumerable of compatible preset names.</returns>
        public static System.Collections.Generic.IEnumerable<string> GetCompatiblePresets(GameObject target)
        {
            return TweenPresetRegistry.GetCompatiblePresets(target).Select(p => p.PresetName);
        }
        
        /// <summary>
        /// Checks if a preset with the specified name exists.
        /// </summary>
        /// <param name="presetName">The name of the preset to check.</param>
        /// <returns>True if the preset exists.</returns>
        public static bool HasPreset(string presetName)
        {
            return TweenPresetRegistry.HasPreset(presetName);
        }
        
        #endregion
        
        #region Sequences
        
        /// <summary>
        /// Creates a new sequence builder for building multi-step animations.
        /// </summary>
        /// <param name="defaultTarget">The default target GameObject for linking (optional).</param>
        /// <returns>A new sequence builder.</returns>
        public static TweenSequenceBuilder CreateSequence(GameObject defaultTarget = null)
        {
            return new TweenSequenceBuilder(defaultTarget);
        }
        
        /// <summary>
        /// Creates a new sequence builder for building multi-step animations with a Transform target.
        /// </summary>
        /// <param name="defaultTarget">The default target Transform for linking.</param>
        /// <returns>A new sequence builder.</returns>
        public static TweenSequenceBuilder CreateSequence(Transform defaultTarget)
        {
            return new TweenSequenceBuilder(defaultTarget?.gameObject);
        }
        
        #endregion
        
        #region Staggered Animations
        
        /// <summary>
        /// Creates staggered movement animations for multiple transforms.
        /// </summary>
        /// <param name="transforms">The transforms to animate.</param>
        /// <param name="targetPosition">The shared target position.</param>
        /// <param name="staggerDelay">The delay between each animation start.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The staggered sequence.</returns>
        public static Sequence StaggerMoveTo(System.Collections.Generic.IEnumerable<Transform> transforms, Vector3 targetPosition, 
            float staggerDelay, float? duration = null, TweenOptions options = default)
        {
            return TweenStagger.StaggerMoveTo(transforms, targetPosition, staggerDelay, duration, options);
        }
        
        /// <summary>
        /// Creates staggered scaling animations for multiple transforms.
        /// </summary>
        /// <param name="transforms">The transforms to animate.</param>
        /// <param name="targetScale">The target scale.</param>
        /// <param name="staggerDelay">The delay between each animation start.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The staggered sequence.</returns>
        public static Sequence StaggerScaleTo(System.Collections.Generic.IEnumerable<Transform> transforms, Vector3 targetScale, 
            float staggerDelay, float? duration = null, TweenOptions options = default)
        {
            return TweenStagger.StaggerScale(transforms, targetScale, staggerDelay, duration, options);
        }
        
        /// <summary>
        /// Creates staggered uniform scaling animations for multiple transforms.
        /// </summary>
        /// <param name="transforms">The transforms to animate.</param>
        /// <param name="targetScale">The uniform target scale value.</param>
        /// <param name="staggerDelay">The delay between each animation start.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The staggered sequence.</returns>
        public static Sequence StaggerScaleTo(System.Collections.Generic.IEnumerable<Transform> transforms, float targetScale, 
            float staggerDelay, float? duration = null, TweenOptions options = default)
        {
            return TweenStagger.StaggerScale(transforms, targetScale, staggerDelay, duration, options);
        }
        
        /// <summary>
        /// Creates staggered preset animations for multiple GameObjects.
        /// </summary>
        /// <param name="presetName">The name of the preset to play.</param>
        /// <param name="targets">The GameObjects to animate.</param>
        /// <param name="staggerDelay">The delay between each animation start.</param>
        /// <param name="duration">Duration override (null uses preset default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The staggered sequence.</returns>
        public static Sequence StaggerPreset(string presetName, System.Collections.Generic.IEnumerable<GameObject> targets, 
            float staggerDelay, float? duration = null, TweenOptions options = default)
        {
            return TweenStagger.StaggerPreset(presetName, targets, staggerDelay, duration, options);
        }
        
        /// <summary>
        /// Creates staggered fade animations for multiple components.
        /// </summary>
        /// <param name="targets">The components to fade.</param>
        /// <param name="targetAlpha">The target alpha value (0-1).</param>
        /// <param name="staggerDelay">The delay between each animation start.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The staggered sequence.</returns>
        public static Sequence StaggerFadeTo(System.Collections.Generic.IEnumerable<Component> targets, float targetAlpha, 
            float staggerDelay, float? duration = null, TweenOptions options = default)
        {
            return TweenStagger.StaggerFade(targets, targetAlpha, staggerDelay, duration, options);
        }
        
        #endregion
        
        #region Control Surface
        
        /// <summary>
        /// Pauses all tweens on the specified GameObject.
        /// </summary>
        /// <param name="target">The GameObject whose tweens to pause.</param>
        public static void Pause(GameObject target)
        {
            TweenController.Pause(target);
        }
        
        /// <summary>
        /// Pauses all tweens on the specified Transform.
        /// </summary>
        /// <param name="target">The Transform whose tweens to pause.</param>
        public static void Pause(Transform target)
        {
            TweenController.Pause(target);
        }
        
        /// <summary>
        /// Resumes all tweens on the specified GameObject.
        /// </summary>
        /// <param name="target">The GameObject whose tweens to resume.</param>
        public static void Resume(GameObject target)
        {
            TweenController.Resume(target);
        }
        
        /// <summary>
        /// Resumes all tweens on the specified Transform.
        /// </summary>
        /// <param name="target">The Transform whose tweens to resume.</param>
        public static void Resume(Transform target)
        {
            TweenController.Resume(target);
        }
        
        /// <summary>
        /// Kills all tweens on the specified GameObject.
        /// </summary>
        /// <param name="target">The GameObject whose tweens to kill.</param>
        /// <param name="complete">Whether to complete the tweens before killing them.</param>
        public static void Kill(GameObject target, bool complete = false)
        {
            TweenController.Kill(target, complete);
        }
        
        /// <summary>
        /// Kills all tweens on the specified Transform.
        /// </summary>
        /// <param name="target">The Transform whose tweens to kill.</param>
        /// <param name="complete">Whether to complete the tweens before killing them.</param>
        public static void Kill(Transform target, bool complete = false)
        {
            TweenController.Kill(target, complete);
        }
        
        /// <summary>
        /// Completes all tweens on the specified GameObject immediately.
        /// </summary>
        /// <param name="target">The GameObject whose tweens to complete.</param>
        public static void Complete(GameObject target)
        {
            TweenController.Complete(target);
        }
        
        /// <summary>
        /// Completes all tweens on the specified Transform immediately.
        /// </summary>
        /// <param name="target">The Transform whose tweens to complete.</param>
        public static void Complete(Transform target)
        {
            TweenController.Complete(target);
        }
        
        /// <summary>
        /// Rewinds all tweens on the specified GameObject to their start values.
        /// </summary>
        /// <param name="target">The GameObject whose tweens to rewind.</param>
        public static void Rewind(GameObject target)
        {
            TweenController.Rewind(target);
        }
        
        /// <summary>
        /// Rewinds all tweens on the specified Transform to their start values.
        /// </summary>
        /// <param name="target">The Transform whose tweens to rewind.</param>
        public static void Rewind(Transform target)
        {
            TweenController.Rewind(target);
        }
        
        /// <summary>
        /// Pauses all tweens with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of tweens to pause.</param>
        public static void PauseById(string id)
        {
            TweenController.PauseById(id);
        }
        
        /// <summary>
        /// Resumes all tweens with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of tweens to resume.</param>
        public static void ResumeById(string id)
        {
            TweenController.ResumeById(id);
        }
        
        /// <summary>
        /// Kills all tweens with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of tweens to kill.</param>
        /// <param name="complete">Whether to complete the tweens before killing them.</param>
        public static void KillById(string id, bool complete = false)
        {
            TweenController.KillById(id, complete);
        }
        
        /// <summary>
        /// Completes all tweens with the specified identifier immediately.
        /// </summary>
        /// <param name="id">The identifier of tweens to complete.</param>
        public static void CompleteById(string id)
        {
            TweenController.CompleteById(id);
        }
        
        /// <summary>
        /// Rewinds all tweens with the specified identifier to their start values.
        /// </summary>
        /// <param name="id">The identifier of tweens to rewind.</param>
        public static void RewindById(string id)
        {
            TweenController.RewindById(id);
        }
        
        #endregion
        
        #region Async Helpers
        
        /// <summary>
        /// Awaits the completion of a tween.
        /// </summary>
        /// <param name="tween">The tween to await.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the await.</param>
        /// <returns>A task that completes when the tween finishes.</returns>
        public static System.Threading.Tasks.Task AwaitCompletion(Tween tween, System.Threading.CancellationToken cancellationToken = default)
        {
            return TweenAsync.AwaitCompletion(tween, cancellationToken);
        }
        
        /// <summary>
        /// Awaits the completion of a tween with a timeout.
        /// </summary>
        /// <param name="tween">The tween to await.</param>
        /// <param name="timeoutSeconds">The timeout in seconds.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the await.</param>
        /// <returns>True if the tween completed normally, false if it was killed or timed out.</returns>
        public static System.Threading.Tasks.Task<bool> AwaitCompletionWithTimeout(Tween tween, float timeoutSeconds, System.Threading.CancellationToken cancellationToken = default)
        {
            return TweenAsync.AwaitCompletionWithTimeout(tween, timeoutSeconds, cancellationToken);
        }
        
        /// <summary>
        /// Awaits the completion of multiple tweens.
        /// </summary>
        /// <param name="tweens">The tweens to await.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the await.</param>
        /// <returns>A task that completes when all tweens finish.</returns>
        public static System.Threading.Tasks.Task AwaitAll(Tween[] tweens, System.Threading.CancellationToken cancellationToken = default)
        {
            return TweenAsync.AwaitAll(tweens, cancellationToken);
        }
        
        /// <summary>
        /// Awaits the completion of any one of multiple tweens.
        /// </summary>
        /// <param name="tweens">The tweens to await.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the await.</param>
        /// <returns>A task that completes when the first tween finishes.</returns>
        public static System.Threading.Tasks.Task AwaitAny(Tween[] tweens, System.Threading.CancellationToken cancellationToken = default)
        {
            return TweenAsync.AwaitAny(tweens, cancellationToken);
        }
        
        /// <summary>
        /// Awaits a delay using DoTween's time system.
        /// </summary>
        /// <param name="delaySeconds">The delay in seconds.</param>
        /// <param name="unscaledTime">Whether to use unscaled time.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the delay.</param>
        /// <returns>A task that completes after the delay.</returns>
        public static System.Threading.Tasks.Task Delay(float delaySeconds, bool unscaledTime = false, System.Threading.CancellationToken cancellationToken = default)
        {
            return TweenAsync.Delay(delaySeconds, unscaledTime, cancellationToken);
        }
        
        #endregion
        
        #region Utility
        
        /// <summary>
        /// Gets the number of active tweens on the specified GameObject.
        /// </summary>
        /// <param name="target">The GameObject to check.</param>
        /// <returns>The number of active tweens.</returns>
        public static int GetActiveTweenCount(GameObject target)
        {
            return TweenController.GetActiveTweenCount(target);
        }
        
        /// <summary>
        /// Gets the number of active tweens on the specified Transform.
        /// </summary>
        /// <param name="target">The Transform to check.</param>
        /// <returns>The number of active tweens.</returns>
        public static int GetActiveTweenCount(Transform target)
        {
            return target != null ? GetActiveTweenCount(target.gameObject) : 0;
        }
        
        /// <summary>
        /// Checks if the specified GameObject has any active tweens.
        /// </summary>
        /// <param name="target">The GameObject to check.</param>
        /// <returns>True if the GameObject has active tweens.</returns>
        public static bool HasActiveTweens(GameObject target)
        {
            return TweenController.HasActiveTweens(target);
        }
        
        /// <summary>
        /// Checks if the specified Transform has any active tweens.
        /// </summary>
        /// <param name="target">The Transform to check.</param>
        /// <returns>True if the Transform has active tweens.</returns>
        public static bool HasActiveTweens(Transform target)
        {
            return target != null && HasActiveTweens(target.gameObject);
        }
        
        /// <summary>
        /// Gets the total number of active tweens globally.
        /// </summary>
        /// <returns>The total number of active tweens.</returns>
        public static int GetTotalActiveTweenCount()
        {
            return TweenController.GetTotalActiveTweenCount();
        }
        
        // Legacy methods for backwards compatibility
        /// <summary>
        /// Kills all tweens on the specified GameObject. (Legacy method - use Kill instead)
        /// </summary>
        /// <param name="target">The GameObject whose tweens to kill.</param>
        /// <param name="complete">Whether to complete the tweens before killing them.</param>
        public static void KillTweens(GameObject target, bool complete = false)
        {
            Kill(target, complete);
        }
        
        /// <summary>
        /// Kills all tweens on the specified Transform. (Legacy method - use Kill instead)
        /// </summary>
        /// <param name="target">The Transform whose tweens to kill.</param>
        /// <param name="complete">Whether to complete the tweens before killing them.</param>
        public static void KillTweens(Transform target, bool complete = false)
        {
            Kill(target, complete);
        }
        
        /// <summary>
        /// Pauses all tweens on the specified GameObject. (Legacy method - use Pause instead)
        /// </summary>
        /// <param name="target">The GameObject whose tweens to pause.</param>
        public static void PauseTweens(GameObject target)
        {
            Pause(target);
        }
        
        /// <summary>
        /// Resumes all tweens on the specified GameObject. (Legacy method - use Resume instead)
        /// </summary>
        /// <param name="target">The GameObject whose tweens to resume.</param>
        public static void ResumeTweens(GameObject target)
        {
            Resume(target);
        }
        
        #endregion
    }
}
