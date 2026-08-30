using System;
using UnityEngine;

namespace LB.TweenHelper
{
    internal sealed class TMPStaggerTiming
    {
        private const float MaximumStartProgress = 0.58f;
        private readonly float[] _visibleCharacterStarts;

        public TMPStaggerTiming(TMPTextElementMap elementMap, TextAnimationUnit unit, StaggerOrder order, float unitStagger, float duration, int seed)
        {
            if (elementMap == null) throw new ArgumentNullException(nameof(elementMap));

            int groupCount = elementMap.GetVisibleGroupCount(unit);
            float[] groupDelays = StaggerDelayUtility.CalculateDelays(groupCount, unitStagger, order, seed);
            float maximumStart = 0f;
            for (int i = 0; i < groupDelays.Length; i++)
            {
                maximumStart = Mathf.Max(maximumStart, duration <= 0f ? 0f : groupDelays[i] / duration);
            }

            float compression = maximumStart > MaximumStartProgress ? MaximumStartProgress / maximumStart : 1f;
            _visibleCharacterStarts = new float[elementMap.VisibleCharacterCount];
            for (int visibleOrder = 0; visibleOrder < _visibleCharacterStarts.Length; visibleOrder++)
            {
                int groupIndex = elementMap.GetVisibleGroupIndex(unit, visibleOrder);
                _visibleCharacterStarts[visibleOrder] = duration <= 0f ? 0f : groupDelays[groupIndex] / duration * compression;
            }
        }

        public float GetLocalProgress(int visibleCharacterOrder, float progress)
        {
            if (visibleCharacterOrder < 0 || visibleCharacterOrder >= _visibleCharacterStarts.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(visibleCharacterOrder));
            }

            float start = _visibleCharacterStarts[visibleCharacterOrder];
            return Mathf.Clamp01((progress - start) / Mathf.Max(0.0001f, 1f - start));
        }
    }
}
