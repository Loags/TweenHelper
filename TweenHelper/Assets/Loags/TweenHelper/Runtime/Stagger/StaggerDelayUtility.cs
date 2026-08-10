using System;

namespace LB.TweenHelper
{
    /// <summary>
    /// Calculates stable, index-aligned delays for staggered collections.
    /// </summary>
    public static class StaggerDelayUtility
    {
        /// <summary>
        /// Returns one delay per source index using the requested order.
        /// </summary>
        public static float[] CalculateDelays(int count, float interval, StaggerOrder order, int randomSeed = 0)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
            ValidateDelay(interval, nameof(interval));

            var delays = new float[count];
            if (count <= 1 || interval <= 0f) return delays;

            switch (order)
            {
                case StaggerOrder.FirstToLast:
                    for (int i = 0; i < count; i++) delays[i] = i * interval;
                    break;
                case StaggerOrder.LastToFirst:
                    for (int i = 0; i < count; i++) delays[i] = (count - 1 - i) * interval;
                    break;
                case StaggerOrder.FromCenter:
                    FillCenterDelays(delays, interval, false);
                    break;
                case StaggerOrder.ToCenter:
                    FillCenterDelays(delays, interval, true);
                    break;
                case StaggerOrder.Random:
                    FillRandomDelays(delays, interval, randomSeed);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(order), order, "Unknown stagger order.");
            }

            return delays;
        }

        internal static void ValidateDelay(float value, string parameterName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value) || value < 0f)
            {
                throw new ArgumentOutOfRangeException(parameterName, "Delay values must be finite and non-negative.");
            }
        }

        private static void FillCenterDelays(float[] delays, float interval, bool reverse)
        {
            float center = (delays.Length - 1) * 0.5f;
            int maximumLayer = (delays.Length - 1) / 2;

            for (int i = 0; i < delays.Length; i++)
            {
                int layer = (int)Math.Floor(Math.Abs(i - center));
                delays[i] = (reverse ? maximumLayer - layer : layer) * interval;
            }
        }

        private static void FillRandomDelays(float[] delays, float interval, int randomSeed)
        {
            var indices = new int[delays.Length];
            for (int i = 0; i < indices.Length; i++) indices[i] = i;

            var random = new Random(randomSeed);
            for (int i = indices.Length - 1; i > 0; i--)
            {
                int swapIndex = random.Next(i + 1);
                int value = indices[i];
                indices[i] = indices[swapIndex];
                indices[swapIndex] = value;
            }

            for (int rank = 0; rank < indices.Length; rank++) delays[indices[rank]] = rank * interval;
        }
    }
}
