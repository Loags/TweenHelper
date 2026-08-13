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

        public static TweenHandle GridDiagonalWave(this IEnumerable<GameObject> targets, GameObject owner, int columns, GridDiagonalDirection direction = GridDiagonalDirection.TopLeftToBottomRight, float duration = 0.32f, float interval = 0.065f, TweenOptions options = default)
        {
            ValidateColumns(columns);
            var snapshot = Snapshot(targets);
            int rows = GetRowCount(snapshot.Count, columns);
            return snapshot.TweenStagger(owner)
                .Preset<PopInFadePreset>(duration, options)
                .DelayBy((_, index) => GetDiagonalStep(index, snapshot.Count, columns, rows, direction) * interval)
                .Play();
        }

        public static TweenHandle GridSpiral(this IEnumerable<GameObject> targets, GameObject owner, int columns, GridSpiralDirection direction = GridSpiralDirection.OutsideInClockwise, float duration = 0.3f, float interval = 0.045f, TweenOptions options = default)
        {
            ValidateColumns(columns);
            var snapshot = Snapshot(targets);
            int[] ranks = CalculateSpiralRanks(snapshot.Count, columns, direction);
            return snapshot.TweenStagger(owner)
                .Preset<PopInFadePreset>(duration, options)
                .DelayBy((_, index) => ranks[index] * interval)
                .Play();
        }

        public static TweenHandle GridCheckerboard(this IEnumerable<GameObject> targets, GameObject owner, int columns, bool inverted = false, float duration = 0.34f, float phaseInterval = 0.16f, TweenOptions options = default)
        {
            ValidateColumns(columns);
            var snapshot = Snapshot(targets);
            return snapshot.TweenStagger(owner)
                .Preset<PulseScalePreset>(duration, options)
                .DelayBy((_, index) => ((((index / columns) + (index % columns)) & 1) == (inverted ? 1 : 0) ? 0f : phaseInterval))
                .Play();
        }

        public static TweenHandle CollectionBurstIn(this IEnumerable<GameObject> targets, GameObject owner, Vector3 origin, float duration = 0.48f, float interval = 0.035f, bool local = true, TweenOptions options = default)
            => SpatialCollectionRecipeUtility.Create(Snapshot(targets), owner, SpatialCollectionAnimation.BurstIn, origin, 0f, duration, interval, local, options);

        public static TweenHandle CollectionBurstOut(this IEnumerable<GameObject> targets, GameObject owner, Vector3 origin, float? distance = null, float duration = 0.42f, float interval = 0.03f, bool local = true, TweenOptions options = default)
        {
            List<GameObject> snapshot = Snapshot(targets);
            return SpatialCollectionRecipeUtility.Create(snapshot, owner, SpatialCollectionAnimation.BurstOut, origin, ResolveSpatialDistance(snapshot, distance, local), duration, interval, local, options);
        }

        public static TweenHandle CollectionGatherTo(this IEnumerable<GameObject> targets, GameObject owner, Vector3 destination, float duration = 0.52f, float interval = 0.04f, bool local = true, TweenOptions options = default)
            => SpatialCollectionRecipeUtility.Create(Snapshot(targets), owner, SpatialCollectionAnimation.GatherTo, destination, 0f, duration, interval, local, options);

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

        public static TweenHandle GridDiagonalWave(this IEnumerable<Component> targets, GameObject owner, int columns, GridDiagonalDirection direction = GridDiagonalDirection.TopLeftToBottomRight, float duration = 0.32f, float interval = 0.065f, TweenOptions options = default)
            => TweenStaggerExtensions.ToGameObjects(targets).GridDiagonalWave(owner, columns, direction, duration, interval, options);

        public static TweenHandle GridSpiral(this IEnumerable<Component> targets, GameObject owner, int columns, GridSpiralDirection direction = GridSpiralDirection.OutsideInClockwise, float duration = 0.3f, float interval = 0.045f, TweenOptions options = default)
            => TweenStaggerExtensions.ToGameObjects(targets).GridSpiral(owner, columns, direction, duration, interval, options);

        public static TweenHandle GridCheckerboard(this IEnumerable<Component> targets, GameObject owner, int columns, bool inverted = false, float duration = 0.34f, float phaseInterval = 0.16f, TweenOptions options = default)
            => TweenStaggerExtensions.ToGameObjects(targets).GridCheckerboard(owner, columns, inverted, duration, phaseInterval, options);

        public static TweenHandle CollectionBurstIn(this IEnumerable<Component> targets, GameObject owner, Vector3 origin, float duration = 0.48f, float interval = 0.035f, bool local = true, TweenOptions options = default)
            => TweenStaggerExtensions.ToGameObjects(targets).CollectionBurstIn(owner, origin, duration, interval, local, options);

        public static TweenHandle CollectionBurstOut(this IEnumerable<Component> targets, GameObject owner, Vector3 origin, float? distance = null, float duration = 0.42f, float interval = 0.03f, bool local = true, TweenOptions options = default)
            => TweenStaggerExtensions.ToGameObjects(targets).CollectionBurstOut(owner, origin, distance, duration, interval, local, options);

        public static TweenHandle CollectionGatherTo(this IEnumerable<Component> targets, GameObject owner, Vector3 destination, float duration = 0.52f, float interval = 0.04f, bool local = true, TweenOptions options = default)
            => TweenStaggerExtensions.ToGameObjects(targets).CollectionGatherTo(owner, destination, duration, interval, local, options);

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

        private static List<GameObject> Snapshot(IEnumerable<GameObject> targets)
            => new List<GameObject>(targets ?? throw new ArgumentNullException(nameof(targets)));

        private static float ResolveSpatialDistance(IReadOnlyList<GameObject> targets, float? distance, bool local)
        {
            if (distance.HasValue) return distance.Value;
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] != null) return local && targets[i].transform is RectTransform ? 120f : 1.2f;
            }

            return 120f;
        }

        private static int GetDiagonalStep(int index, int count, int columns, int rows, GridDiagonalDirection direction)
        {
            int row = index / columns;
            int column = index % columns;
            int itemsInRow = Mathf.Min(columns, count - row * columns);
            int rightColumn = itemsInRow - 1;
            switch (direction)
            {
                case GridDiagonalDirection.TopLeftToBottomRight: return row + column;
                case GridDiagonalDirection.TopRightToBottomLeft: return row + rightColumn - column;
                case GridDiagonalDirection.BottomLeftToTopRight: return rows - 1 - row + column;
                case GridDiagonalDirection.BottomRightToTopLeft: return rows - 1 - row + rightColumn - column;
                default: throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown diagonal direction.");
            }
        }

        private static int[] CalculateSpiralRanks(int count, int columns, GridSpiralDirection direction)
        {
            var ranks = new int[count];
            if (count == 0) return ranks;
            int rows = GetRowCount(count, columns);
            bool insideOut = direction == GridSpiralDirection.InsideOutClockwise || direction == GridSpiralDirection.InsideOutCounterClockwise;
            bool requestedClockwise = direction == GridSpiralDirection.OutsideInClockwise || direction == GridSpiralDirection.InsideOutClockwise;
            bool clockwise = insideOut ? !requestedClockwise : requestedClockwise;
            var order = new List<int>(count);
            var seen = new bool[count];
            int top = 0;
            int bottom = rows - 1;
            int left = 0;
            int right = columns - 1;

            while (top <= bottom && left <= right)
            {
                if (clockwise)
                {
                    for (int column = left; column <= right; column++) AddGridIndex(order, seen, top, column, columns, count);
                    for (int row = top + 1; row <= bottom; row++) AddGridIndex(order, seen, row, right, columns, count);
                    if (top < bottom) for (int column = right - 1; column >= left; column--) AddGridIndex(order, seen, bottom, column, columns, count);
                    if (left < right) for (int row = bottom - 1; row > top; row--) AddGridIndex(order, seen, row, left, columns, count);
                }
                else
                {
                    for (int row = top; row <= bottom; row++) AddGridIndex(order, seen, row, left, columns, count);
                    for (int column = left + 1; column <= right; column++) AddGridIndex(order, seen, bottom, column, columns, count);
                    if (left < right) for (int row = bottom - 1; row >= top; row--) AddGridIndex(order, seen, row, right, columns, count);
                    if (top < bottom) for (int column = right - 1; column > left; column--) AddGridIndex(order, seen, top, column, columns, count);
                }

                top++;
                bottom--;
                left++;
                right--;
            }

            for (int index = 0; index < count; index++)
            {
                if (!seen[index]) order.Add(index);
            }

            if (insideOut) order.Reverse();
            for (int rank = 0; rank < order.Count; rank++) ranks[order[rank]] = rank;
            return ranks;
        }

        private static void AddGridIndex(List<int> order, bool[] seen, int row, int column, int columns, int count)
        {
            int index = row * columns + column;
            if (index < 0 || index >= count || seen[index]) return;
            seen[index] = true;
            order.Add(index);
        }

        private static int GetRowCount(int count, int columns) => count == 0 ? 0 : (count + columns - 1) / columns;

        private static void ValidateColumns(int columns)
        {
            if (columns <= 0) throw new ArgumentOutOfRangeException(nameof(columns), "Columns must be greater than zero.");
        }
    }
}
