using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Helper class for creating staggered animations across multiple targets.
    /// Provides utilities for animating lists of objects with delays between each start.
    /// </summary>
    public static class TweenStagger
    {
        /// <summary>
        /// Creates staggered movement animations for a list of transforms.
        /// </summary>
        /// <param name="transforms">The transforms to animate.</param>
        /// <param name="targetPositions">The target positions (must match transforms count).</param>
        /// <param name="staggerDelay">The delay between each animation start.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The staggered sequence.</returns>
        public static Sequence StaggerMove(IEnumerable<Transform> transforms, IEnumerable<Vector3> targetPositions, 
            float staggerDelay, float? duration = null, TweenOptions options = default)
        {
            var transformList = transforms?.ToList();
            var positionList = targetPositions?.ToList();
            
            if (transformList == null || positionList == null)
            {
                Debug.LogError("TweenStagger: Transforms and target positions cannot be null.");
                return null;
            }
            
            if (transformList.Count != positionList.Count)
            {
                Debug.LogError($"TweenStagger: Transform count ({transformList.Count}) must match target position count ({positionList.Count}).");
                return null;
            }
            
            var sequence = DOTween.Sequence();
            
            for (int i = 0; i < transformList.Count; i++)
            {
                var transform = transformList[i];
                var targetPosition = positionList[i];
                
                if (transform != null)
                {
                    var tween = TweenHelper.MoveTo(transform, targetPosition, duration, options);
                    if (tween != null)
                    {
                        if (i == 0)
                        {
                            sequence.Append(tween);
                        }
                        else
                        {
                            sequence.Insert(i * staggerDelay, tween);
                        }
                    }
                }
            }
            
            return sequence;
        }
        
        /// <summary>
        /// Creates staggered movement animations for a list of transforms to the same target position.
        /// </summary>
        /// <param name="transforms">The transforms to animate.</param>
        /// <param name="targetPosition">The shared target position.</param>
        /// <param name="staggerDelay">The delay between each animation start.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The staggered sequence.</returns>
        public static Sequence StaggerMoveTo(IEnumerable<Transform> transforms, Vector3 targetPosition, 
            float staggerDelay, float? duration = null, TweenOptions options = default)
        {
            var transformList = transforms?.ToList();
            if (transformList == null)
            {
                Debug.LogError("TweenStagger: Transforms cannot be null.");
                return null;
            }
            
            var positions = Enumerable.Repeat(targetPosition, transformList.Count);
            return StaggerMove(transformList, positions, staggerDelay, duration, options);
        }
        
        /// <summary>
        /// Creates staggered scaling animations for a list of transforms.
        /// </summary>
        /// <param name="transforms">The transforms to animate.</param>
        /// <param name="targetScale">The target scale for all transforms.</param>
        /// <param name="staggerDelay">The delay between each animation start.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The staggered sequence.</returns>
        public static Sequence StaggerScale(IEnumerable<Transform> transforms, Vector3 targetScale, 
            float staggerDelay, float? duration = null, TweenOptions options = default)
        {
            var transformList = transforms?.ToList();
            if (transformList == null)
            {
                Debug.LogError("TweenStagger: Transforms cannot be null.");
                return null;
            }
            
            var sequence = DOTween.Sequence();
            
            for (int i = 0; i < transformList.Count; i++)
            {
                var transform = transformList[i];
                
                if (transform != null)
                {
                    var tween = TweenHelper.ScaleTo(transform, targetScale, duration, options);
                    if (tween != null)
                    {
                        if (i == 0)
                        {
                            sequence.Append(tween);
                        }
                        else
                        {
                            sequence.Insert(i * staggerDelay, tween);
                        }
                    }
                }
            }
            
            return sequence;
        }
        
        /// <summary>
        /// Creates staggered scaling animations for a list of transforms with uniform scale.
        /// </summary>
        /// <param name="transforms">The transforms to animate.</param>
        /// <param name="targetScale">The uniform target scale value.</param>
        /// <param name="staggerDelay">The delay between each animation start.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The staggered sequence.</returns>
        public static Sequence StaggerScale(IEnumerable<Transform> transforms, float targetScale, 
            float staggerDelay, float? duration = null, TweenOptions options = default)
        {
            return StaggerScale(transforms, Vector3.one * targetScale, staggerDelay, duration, options);
        }
        
        /// <summary>
        /// Creates staggered preset animations for a list of GameObjects.
        /// </summary>
        /// <param name="presetName">The name of the preset to play.</param>
        /// <param name="targets">The GameObjects to animate.</param>
        /// <param name="staggerDelay">The delay between each animation start.</param>
        /// <param name="duration">Duration override (null uses preset default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The staggered sequence.</returns>
        public static Sequence StaggerPreset(string presetName, IEnumerable<GameObject> targets, 
            float staggerDelay, float? duration = null, TweenOptions options = default)
        {
            var targetList = targets?.ToList();
            if (targetList == null)
            {
                Debug.LogError("TweenStagger: Targets cannot be null.");
                return null;
            }
            
            var sequence = DOTween.Sequence();
            
            for (int i = 0; i < targetList.Count; i++)
            {
                var target = targetList[i];
                
                if (target != null)
                {
                    var tween = TweenHelper.PlayPreset(presetName, target, duration, options);
                    if (tween != null)
                    {
                        if (i == 0)
                        {
                            sequence.Append(tween);
                        }
                        else
                        {
                            sequence.Insert(i * staggerDelay, tween);
                        }
                    }
                }
            }
            
            return sequence;
        }
        
        /// <summary>
        /// Creates staggered fade animations for a list of components.
        /// </summary>
        /// <param name="targets">The components to fade (CanvasGroup, SpriteRenderer, Image, Text).</param>
        /// <param name="targetAlpha">The target alpha value (0-1).</param>
        /// <param name="staggerDelay">The delay between each animation start.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The staggered sequence.</returns>
        public static Sequence StaggerFade(IEnumerable<Component> targets, float targetAlpha, 
            float staggerDelay, float? duration = null, TweenOptions options = default)
        {
            var targetList = targets?.ToList();
            if (targetList == null)
            {
                Debug.LogError("TweenStagger: Targets cannot be null.");
                return null;
            }
            
            var sequence = DOTween.Sequence();
            
            for (int i = 0; i < targetList.Count; i++)
            {
                var target = targetList[i];
                Tween tween = null;
                
                if (target is CanvasGroup canvasGroup)
                {
                    tween = TweenHelper.FadeTo(canvasGroup, targetAlpha, duration, options);
                }
                else if (target is SpriteRenderer spriteRenderer)
                {
                    tween = TweenHelper.FadeTo(spriteRenderer, targetAlpha, duration, options);
                }
                else if (target is UnityEngine.UI.Image image)
                {
                    tween = TweenHelper.FadeTo(image, targetAlpha, duration, options);
                }
                else if (target is UnityEngine.UI.Text text)
                {
                    tween = TweenHelper.FadeTo(text, targetAlpha, duration, options);
                }
                
                if (tween != null)
                {
                    if (i == 0)
                    {
                        sequence.Append(tween);
                    }
                    else
                    {
                        sequence.Insert(i * staggerDelay, tween);
                    }
                }
            }
            
            return sequence;
        }
        
        /// <summary>
        /// Creates a staggered animation using a custom tween factory function.
        /// </summary>
        /// <typeparam name="T">The type of the target objects.</typeparam>
        /// <param name="targets">The target objects to animate.</param>
        /// <param name="tweenFactory">A function that creates a tween for each target.</param>
        /// <param name="staggerDelay">The delay between each animation start.</param>
        /// <returns>The staggered sequence.</returns>
        public static Sequence StaggerCustom<T>(IEnumerable<T> targets, Func<T, int, Tween> tweenFactory, float staggerDelay)
        {
            var targetList = targets?.ToList();
            if (targetList == null)
            {
                Debug.LogError("TweenStagger: Targets cannot be null.");
                return null;
            }
            
            if (tweenFactory == null)
            {
                Debug.LogError("TweenStagger: Tween factory cannot be null.");
                return null;
            }
            
            var sequence = DOTween.Sequence();
            
            for (int i = 0; i < targetList.Count; i++)
            {
                var target = targetList[i];
                
                try
                {
                    var tween = tweenFactory(target, i);
                    if (tween != null)
                    {
                        if (i == 0)
                        {
                            sequence.Append(tween);
                        }
                        else
                        {
                            sequence.Insert(i * staggerDelay, tween);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"TweenStagger: Error creating tween for target {i}: {ex.Message}");
                }
            }
            
            return sequence;
        }
    }
}

