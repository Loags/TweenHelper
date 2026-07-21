using System;
using System.Collections.Generic;

namespace LB.TweenHelper.Demo
{
    public static class UIPresetCompatibility
    {
        public const int ExpectedPresetCount = 198;

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
    }
}
