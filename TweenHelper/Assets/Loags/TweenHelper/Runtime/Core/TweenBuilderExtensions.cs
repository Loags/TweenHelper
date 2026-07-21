using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Extension methods providing the entry point for the TweenBuilder fluent API.
    /// </summary>
    public static class TweenBuilderExtensions
    {
        /// <summary>
        /// Creates and plays a typed preset without allocating a builder or handle wrapper.
        /// </summary>
        public static Tween PlayPreset<TPreset>(this Transform transform, float? duration = null, TweenOptions options = default) where TPreset : class, ITweenPreset
            => TweenPresetRegistry.PlayUnchecked<TPreset>(transform.gameObject, duration, options);

        /// <summary>
        /// Creates and plays a typed preset without allocating a builder or handle wrapper.
        /// </summary>
        public static Tween PlayPreset<TPreset>(this GameObject gameObject, float? duration = null, TweenOptions options = default) where TPreset : class, ITweenPreset
            => TweenPresetRegistry.PlayUnchecked<TPreset>(gameObject, duration, options);

        /// <summary>
        /// Creates a new TweenBuilder for the specified Transform.
        /// </summary>
        /// <param name="transform">The transform to animate.</param>
        /// <returns>A new TweenBuilder instance.</returns>
        /// <example>
        /// transform.Tween()
        ///     .Move(Vector3.up * 3f)
        ///     .WithEase(Ease.OutBounce)
        ///     .Play();
        /// </example>
        public static TweenBuilder Tween(this Transform transform)
        {
            return new TweenBuilder(transform);
        }

        /// <summary>
        /// Creates a new TweenBuilder for the specified GameObject.
        /// Uses the GameObject's transform for animation.
        /// </summary>
        /// <param name="gameObject">The game object to animate.</param>
        /// <returns>A new TweenBuilder instance.</returns>
        /// <example>
        /// gameObject.Tween()
        ///     .Scale(1.5f)
        ///     .Play();
        /// </example>
        public static TweenBuilder Tween(this GameObject gameObject)
        {
            return new TweenBuilder(gameObject);
        }

        /// <summary>
        /// Creates a new TweenBuilder for the specified Component.
        /// Uses the Component's transform for animation.
        /// </summary>
        /// <param name="component">The component to animate.</param>
        /// <returns>A new TweenBuilder instance.</returns>
        public static TweenBuilder Tween(this Component component)
        {
            return new TweenBuilder(component.gameObject);
        }
    }
}
