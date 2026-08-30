using System.Collections.Generic;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Captures the visual state of a collection before its Unity layout changes.
    /// </summary>
    public sealed class CollectionLayoutSnapshot
    {
        internal CollectionLayoutSnapshot(RectTransform container, Entry[] entries)
        {
            Container = container;
            Entries = entries;
        }

        internal RectTransform Container { get; }
        internal IReadOnlyList<Entry> Entries { get; }

        internal readonly struct Entry
        {
            public Entry(RectTransform child, Vector3 worldPosition, Vector3 localScale, int siblingIndex)
            {
                Child = child;
                WorldPosition = worldPosition;
                LocalScale = localScale;
                SiblingIndex = siblingIndex;
            }

            public RectTransform Child { get; }
            public Vector3 WorldPosition { get; }
            public Vector3 LocalScale { get; }
            public int SiblingIndex { get; }
        }
    }
}
