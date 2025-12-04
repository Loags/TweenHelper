using UnityEngine;
using System.Collections.Generic;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Represents a single demo animation entry.
    /// </summary>
    public class DemoAnimation
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public System.Action<Transform[], float> Execute { get; set; }
        public bool RequiresMultipleObjects { get; set; }
    }

    /// <summary>
    /// Interface for classes that provide demo animations.
    /// </summary>
    public interface IDemoAnimationProvider
    {
        /// <summary>
        /// Gets the category name for this provider.
        /// </summary>
        string CategoryName { get; }

        /// <summary>
        /// Gets all animations provided by this provider.
        /// </summary>
        IEnumerable<DemoAnimation> GetAnimations();

        /// <summary>
        /// Initializes the provider with demo objects.
        /// </summary>
        void Initialize(GameObject[] objects);
    }
}
