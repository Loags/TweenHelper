using System;
using System.Collections.Generic;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Entry points for creating staggered collection tweens.
    /// </summary>
    public static class TweenStaggerExtensions
    {
        public static TweenStaggerBuilder TweenStagger(this IEnumerable<GameObject> targets, GameObject owner)
        {
            return new TweenStaggerBuilder(targets, owner);
        }

        public static TweenStaggerBuilder TweenStagger(this IEnumerable<GameObject> targets, Component owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            return new TweenStaggerBuilder(targets, owner.gameObject);
        }

        public static TweenStaggerBuilder TweenStagger(this IEnumerable<Component> targets, GameObject owner)
        {
            return new TweenStaggerBuilder(ToGameObjects(targets), owner);
        }

        public static TweenStaggerBuilder TweenStagger(this IEnumerable<Component> targets, Component owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            return new TweenStaggerBuilder(ToGameObjects(targets), owner.gameObject);
        }

        internal static List<GameObject> ToGameObjects(IEnumerable<Component> targets)
        {
            if (targets == null) throw new ArgumentNullException(nameof(targets));
            var gameObjects = new List<GameObject>();
            int index = 0;

            foreach (Component target in targets)
            {
                if (target == null) throw new ArgumentException($"Target component at index {index} is null.", nameof(targets));
                gameObjects.Add(target.gameObject);
                index++;
            }

            return gameObjects;
        }
    }
}
