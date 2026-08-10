using System;
using System.Collections.Generic;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Ready-to-play collection recipes built on TweenStaggerBuilder.
    /// </summary>
    public static class StaggerRecipeExtensions
    {
        public static TweenHandle ListStaggerIn(this IEnumerable<GameObject> targets, GameObject owner, float duration = 0.32f, float interval = 0.06f, TweenOptions options = default)
        {
            return targets.TweenStagger(owner)
                .Preset<PopInFadePreset>(duration, options)
                .Order(StaggerOrder.FirstToLast)
                .DelayBetween(interval)
                .Play();
        }

        public static TweenHandle ListStaggerOut(this IEnumerable<GameObject> targets, GameObject owner, float duration = 0.26f, float interval = 0.04f, TweenOptions options = default)
        {
            return targets.TweenStagger(owner)
                .Preset<PopOutFadePreset>(duration, options)
                .Order(StaggerOrder.LastToFirst)
                .DelayBetween(interval)
                .Play();
        }

        public static TweenHandle GridWave(this IEnumerable<GameObject> targets, GameObject owner, int columns, GridWaveDirection direction = GridWaveDirection.LeftToRight, float duration = 0.32f, float interval = 0.07f, TweenOptions options = default)
        {
            ValidateColumns(columns);
            var snapshot = new List<GameObject>(targets ?? throw new ArgumentNullException(nameof(targets)));
            int rows = GetRowCount(snapshot.Count, columns);

            return snapshot.TweenStagger(owner)
                .Preset<PopInFadePreset>(duration, options)
                .DelayBy((_, index) => GetWaveStep(index, snapshot.Count, columns, rows, direction) * interval)
                .Play();
        }

        public static TweenHandle GridRipple(this IEnumerable<GameObject> targets, GameObject owner, int columns, int originIndex = -1, float duration = 0.32f, float interval = 0.07f, TweenOptions options = default)
        {
            ValidateColumns(columns);
            var snapshot = new List<GameObject>(targets ?? throw new ArgumentNullException(nameof(targets)));
            if (snapshot.Count == 0) return snapshot.TweenStagger(owner).Preset<PulseScalePreset>(duration, options).Play();

            int rows = GetRowCount(snapshot.Count, columns);
            int resolvedOrigin = originIndex < 0 ? GetDefaultOrigin(snapshot.Count, columns, rows) : originIndex;
            if (resolvedOrigin < 0 || resolvedOrigin >= snapshot.Count) throw new ArgumentOutOfRangeException(nameof(originIndex));
            int originRow = resolvedOrigin / columns;
            int originColumn = resolvedOrigin % columns;

            return snapshot.TweenStagger(owner)
                .Preset<PulseScalePreset>(duration, options)
                .DelayBy((_, index) =>
                {
                    int row = index / columns;
                    int column = index % columns;
                    float rowDistance = row - originRow;
                    float columnDistance = column - originColumn;
                    return Mathf.Sqrt(rowDistance * rowDistance + columnDistance * columnDistance) * interval;
                })
                .Play();
        }

        public static TweenHandle LoadingDots(this IEnumerable<GameObject> targets, GameObject owner, float duration = 0.25f, float interval = 0.12f, float loopPause = 0.2f, TweenOptions options = default)
        {
            return targets.TweenStagger(owner)
                .Preset<PulseScaleSoftPreset>(duration, options)
                .Order(StaggerOrder.FirstToLast)
                .DelayBetween(interval)
                .WithTailDelay(loopPause)
                .WithLoops(-1)
                .Play();
        }

        public static TweenHandle ListStaggerIn(this IEnumerable<Component> targets, GameObject owner, float duration = 0.32f, float interval = 0.06f, TweenOptions options = default)
            => TweenStaggerExtensions.ToGameObjects(targets).ListStaggerIn(owner, duration, interval, options);

        public static TweenHandle ListStaggerOut(this IEnumerable<Component> targets, GameObject owner, float duration = 0.26f, float interval = 0.04f, TweenOptions options = default)
            => TweenStaggerExtensions.ToGameObjects(targets).ListStaggerOut(owner, duration, interval, options);

        public static TweenHandle GridWave(this IEnumerable<Component> targets, GameObject owner, int columns, GridWaveDirection direction = GridWaveDirection.LeftToRight, float duration = 0.32f, float interval = 0.07f, TweenOptions options = default)
            => TweenStaggerExtensions.ToGameObjects(targets).GridWave(owner, columns, direction, duration, interval, options);

        public static TweenHandle GridRipple(this IEnumerable<Component> targets, GameObject owner, int columns, int originIndex = -1, float duration = 0.32f, float interval = 0.07f, TweenOptions options = default)
            => TweenStaggerExtensions.ToGameObjects(targets).GridRipple(owner, columns, originIndex, duration, interval, options);

        public static TweenHandle LoadingDots(this IEnumerable<Component> targets, GameObject owner, float duration = 0.25f, float interval = 0.12f, float loopPause = 0.2f, TweenOptions options = default)
            => TweenStaggerExtensions.ToGameObjects(targets).LoadingDots(owner, duration, interval, loopPause, options);

        private static int GetWaveStep(int index, int count, int columns, int rows, GridWaveDirection direction)
        {
            int row = index / columns;
            int column = index % columns;

            switch (direction)
            {
                case GridWaveDirection.LeftToRight: return column;
                case GridWaveDirection.RightToLeft:
                {
                    int itemsInRow = Mathf.Min(columns, count - row * columns);
                    return itemsInRow - 1 - column;
                }
                case GridWaveDirection.TopToBottom: return row;
                case GridWaveDirection.BottomToTop: return rows - 1 - row;
                default: throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown grid wave direction.");
            }
        }

        private static int GetDefaultOrigin(int count, int columns, int rows)
        {
            int origin = ((rows - 1) / 2) * columns + (columns - 1) / 2;
            return Mathf.Min(origin, count - 1);
        }

        private static int GetRowCount(int count, int columns) => count == 0 ? 0 : (count + columns - 1) / columns;

        private static void ValidateColumns(int columns)
        {
            if (columns <= 0) throw new ArgumentOutOfRangeException(nameof(columns), "Columns must be greater than zero.");
        }
    }
}
