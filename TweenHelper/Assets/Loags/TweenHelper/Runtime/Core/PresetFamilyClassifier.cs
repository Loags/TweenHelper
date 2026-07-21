using System;

namespace LB.TweenHelper
{
    /// <summary>
    /// Shared preset-name classification helpers used by runtime/demo/editor tooling.
    /// </summary>
    public static class PresetFamilyClassifier
    {
        private static readonly string[] KnownSuffixes =
        {
            "CounterClockwise",
            "Overshoot",
            "Clockwise",
            "Diagonal",
            "Cartoon",
            "Heavy",
            "Fade",
            "Land",
            "Soft",
            "Hard",
            "Down",
            "Left",
            "Right",
            "Up",
            "Out",
            "In",
            "XY",
            "XZ",
            "YZ",
            "2D",
            "S",
            "M",
            "L",
            "X",
            "Y",
            "Z",
        };

        private static readonly (string PresetName, string FamilyName)[] FamilyOverrides =
        {
            ("SwirlIn", "Spiral"),
            ("Swirl", "Spiral"),
            ("Blink", "Misc"),
            ("Flicker", "Misc"),
            ("Attention", "Misc"),
            ("Explode", "Misc"),
        };

        public static string GetFamilyName(string presetName)
        {
            if (string.IsNullOrEmpty(presetName))
            {
                return string.Empty;
            }

            if (TryGetOverride(presetName, out var familyOverride))
            {
                return familyOverride;
            }

            var candidate = presetName;
            bool stripped = true;

            while (stripped && candidate.Length > 0)
            {
                stripped = false;

                foreach (var suffix in KnownSuffixes)
                {
                    if (suffix.Length == 1 && candidate.Length > 1 && candidate.EndsWith(suffix, StringComparison.Ordinal))
                    {
                        var remaining = candidate.Substring(0, candidate.Length - 1);
                        if (remaining.Length >= 3 && char.IsLower(remaining[remaining.Length - 1]))
                        {
                            candidate = remaining;
                            stripped = true;
                            break;
                        }
                    }
                    else if (suffix.Length > 1 && candidate.Length > suffix.Length && candidate.EndsWith(suffix, StringComparison.Ordinal))
                    {
                        candidate = candidate.Substring(0, candidate.Length - suffix.Length);
                        stripped = true;
                        break;
                    }
                }
            }

            if (TryGetOverride(candidate, out var strippedOverride))
            {
                return strippedOverride;
            }

            if (candidate.Length < 2)
            {
                candidate = presetName;
            }

            return candidate;
        }

        private static bool TryGetOverride(string presetName, out string familyName)
        {
            for (int i = 0; i < FamilyOverrides.Length; i++)
            {
                if (string.Equals(FamilyOverrides[i].PresetName, presetName, StringComparison.Ordinal))
                {
                    familyName = FamilyOverrides[i].FamilyName;
                    return true;
                }
            }

            familyName = null;
            return false;
        }
    }
}
