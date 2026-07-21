using System;
using System.Collections.Generic;

namespace LB.TweenHelper.Demo
{
    public static class UIPresetCompatibility
    {
        public const int ExpectedPresetCount = 198;

        private const float FloatPreviewStrength = 60f;
        private const float SwayPreviewStrength = 60f;
        private const float NudgePreviewStrength = 60f;
        private const float ShakePreviewStrength = 50f;
        private const float JitterPreviewStrength = 120f;
        private const float BouncePreviewStrength = 40f;
        private const float DropPreviewStrength = 25f;
        private const float LaunchPreviewStrength = 50f;
        private const float OrbitPreviewStrength = 50f;
        private const float ZigZagPreviewStrength = 40f;

        private static readonly string[] ExcludedPrefixes =
        {
            "FlipX", "FlipY", "FlipFadeX", "FlipFadeY",
            "SpinX", "SpinY", "SpinDiagonal", "SpinScale",
            "WobbleX", "WobbleY", "WobbleDiagonal", "WobbleFadeX", "WobbleFadeY",
            "PendulumX", "PendulumY", "Nod", "Recoil",
            "OrbitXZ", "OrbitYZ", "Spiral", "Swirl"
        };

        public static bool IsSuitable(ITweenPreset preset)
        {
            if (preset == null || string.IsNullOrEmpty(preset.PresetName)) return false;

            for (int i = 0; i < ExcludedPrefixes.Length; i++)
            {
                if (preset.PresetName.StartsWith(ExcludedPrefixes[i], StringComparison.Ordinal)) return false;
            }

            return true;
        }

        public static List<ITweenPreset> GetSuitablePresets(IEnumerable<ITweenPreset> presets)
        {
            var result = new List<ITweenPreset>();
            foreach (var preset in presets)
            {
                if (IsSuitable(preset)) result.Add(preset);
            }

            result.Sort((left, right) => string.Compare(left.PresetName, right.PresetName, StringComparison.Ordinal));
            return result;
        }

        public static float? GetCanvasPreviewStrength(ITweenPreset preset)
        {
            if (preset == null) return null;

            string name = preset.PresetName;
            if (name.StartsWith("Float", StringComparison.Ordinal)) return FloatPreviewStrength;
            if (name.StartsWith("Sway", StringComparison.Ordinal)) return SwayPreviewStrength;
            if (name.StartsWith("Nudge", StringComparison.Ordinal)) return NudgePreviewStrength;
            if (name.StartsWith("Shake", StringComparison.Ordinal)) return ShakePreviewStrength;
            if (name.StartsWith("Jitter", StringComparison.Ordinal)) return JitterPreviewStrength;
            if (name.StartsWith("BounceLand", StringComparison.Ordinal) || name.StartsWith("BounceCartoon", StringComparison.Ordinal)) return null;
            if (name.StartsWith("Bounce", StringComparison.Ordinal)) return BouncePreviewStrength;
            if (name.StartsWith("Drop", StringComparison.Ordinal)) return DropPreviewStrength;
            if (name.StartsWith("Launch", StringComparison.Ordinal)) return LaunchPreviewStrength;
            if (name.StartsWith("OrbitXY", StringComparison.Ordinal)) return OrbitPreviewStrength;
            if (name.StartsWith("ZigZag", StringComparison.Ordinal)) return ZigZagPreviewStrength;
            return null;
        }
    }
}
