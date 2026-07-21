using System;
using System.Collections.Generic;

namespace LB.TweenHelper
{
    /// <summary>
    /// Parsed metadata and layout hints derived from a preset name.
    /// Shared by runtime demo and editor validation tooling.
    /// </summary>
    public sealed class PresetVariantMetadata
    {
        public string OriginalName { get; internal set; }
        public string Family { get; internal set; }
        public string Intensity { get; internal set; }
        public string Direction { get; internal set; }
        public string Axis { get; internal set; }
        public string Plane { get; internal set; }
        public bool HasFadeModifier { get; internal set; }
        public string[] Modifiers { get; internal set; }
        public string NormalizedForLayout { get; internal set; }
        public string SubRowKey { get; internal set; }
        public int SubRowSortScore { get; internal set; }
        public int ColumnSortScore { get; internal set; }
    }

    /// <summary>
    /// Shared preset-name parser for layout ordering and editor validation.
    /// </summary>
    public static class PresetVariantParser
    {
        private static readonly string[] Directions = { "Forward", "Right", "Left", "Down", "Back", "Up" };
        private static readonly string[] CompoundAxes = { "XY", "XZ", "YZ", "2D" };
        private static readonly string[] SingleAxes = { "X", "Y", "Z" };
        private static readonly string[] Sizes = { "S", "M", "L" };
        private static readonly string[] KnownModifiers =
        {
            "Fade",
            "Overshoot",
            "Clockwise",
            "CounterClockwise",
            "Diagonal",
            "Cartoon",
            "Land",
            "ScaleIn",
            "ScaleOut",
            "In",
            "Out"
        };

        public static PresetVariantMetadata Parse(string presetName)
        {
            var name = presetName ?? string.Empty;
            var family = PresetFamilyClassifier.GetFamilyName(name);
            var normalized = StripIntensitySuffix(name);
            var subRowKey = BuildSubRowKey(family, normalized);

            var modifiers = DetectModifiers(name);
            return new PresetVariantMetadata
            {
                OriginalName = name,
                Family = family,
                Intensity = DetectIntensity(name),
                Direction = DetectDirection(normalized),
                Axis = DetectAxis(normalized),
                Plane = DetectPlane(normalized),
                HasFadeModifier = name.Contains("Fade", StringComparison.Ordinal),
                Modifiers = modifiers.ToArray(),
                NormalizedForLayout = normalized,
                SubRowKey = subRowKey,
                SubRowSortScore = GetSubRowSortScore(subRowKey),
                ColumnSortScore = GetColumnSortScore(name, normalized),
            };
        }

        public static string StripIntensitySuffix(string presetName)
        {
            if (string.IsNullOrEmpty(presetName))
            {
                return string.Empty;
            }

            if (presetName.EndsWith("Soft", StringComparison.Ordinal))
            {
                return presetName.Substring(0, presetName.Length - "Soft".Length);
            }

            if (presetName.EndsWith("Hard", StringComparison.Ordinal))
            {
                return presetName.Substring(0, presetName.Length - "Hard".Length);
            }

            return presetName;
        }

        public static int GetSubRowSortScore(string rowKey)
        {
            if (string.IsNullOrEmpty(rowKey))
            {
                return -1;
            }

            int score = 0;

            if (rowKey.Contains("In", StringComparison.Ordinal) && !rowKey.Contains("Out", StringComparison.Ordinal)) score += 0;
            else if (rowKey.Contains("Out", StringComparison.Ordinal)) score += 100;

            if (rowKey.Contains("Diagonal", StringComparison.Ordinal)) score += 5;
            if (rowKey.Contains("Overshoot", StringComparison.Ordinal)) score += 10;
            if (rowKey.Contains("Land", StringComparison.Ordinal)) score += 15;
            if (rowKey.Contains("Fade", StringComparison.Ordinal)) score += 20;
            if (rowKey.Contains("Cartoon", StringComparison.Ordinal)) score += 25;
            if (rowKey.Contains("Scale", StringComparison.Ordinal)) score += 30;
            if (rowKey.Contains("Clockwise", StringComparison.Ordinal) && !rowKey.Contains("Counter", StringComparison.Ordinal)) score += 40;
            if (rowKey.Contains("CounterClockwise", StringComparison.Ordinal)) score += 45;

            return score;
        }

        public static int GetColumnSortScore(string presetName)
        {
            return GetColumnSortScore(presetName ?? string.Empty, StripIntensitySuffix(presetName));
        }

        private static int GetColumnSortScore(string presetName, string normalizedName)
        {
            int score = 0;

            // Intensity modifiers: Soft, Default, Hard
            if (presetName.EndsWith("Soft", StringComparison.Ordinal)) score += 0;
            else if (presetName.EndsWith("Hard", StringComparison.Ordinal)) score += 2;
            else score += 1;

            // Directions
            if (normalizedName.Contains("Up", StringComparison.Ordinal)) score += 0;
            else if (normalizedName.Contains("Down", StringComparison.Ordinal)) score += 10;
            else if (normalizedName.Contains("Left", StringComparison.Ordinal)) score += 20;
            else if (normalizedName.Contains("Right", StringComparison.Ordinal)) score += 30;
            else if (normalizedName.Contains("Forward", StringComparison.Ordinal)) score += 40;
            else if (normalizedName.Contains("Back", StringComparison.Ordinal) && !normalizedName.Contains("Backflip", StringComparison.Ordinal)) score += 50;

            // Axes / planes
            if (normalizedName.EndsWith("XY", StringComparison.Ordinal)) score += 100;
            else if (normalizedName.EndsWith("XZ", StringComparison.Ordinal)) score += 110;
            else if (normalizedName.EndsWith("YZ", StringComparison.Ordinal)) score += 120;
            else if (normalizedName.EndsWith("2D", StringComparison.Ordinal)) score += 130;
            else if (normalizedName.EndsWith("X", StringComparison.Ordinal)) score += 100;
            else if (normalizedName.EndsWith("Y", StringComparison.Ordinal)) score += 110;
            else if (normalizedName.EndsWith("Z", StringComparison.Ordinal)) score += 120;

            // Sizes
            if (normalizedName.EndsWith("S", StringComparison.Ordinal) && !normalizedName.EndsWith("XS", StringComparison.Ordinal)) score += 1000;
            else if (normalizedName.EndsWith("M", StringComparison.Ordinal)) score += 1100;
            else if (normalizedName.EndsWith("L", StringComparison.Ordinal)) score += 1200;

            return score;
        }

        private static string BuildSubRowKey(string family, string normalizedName)
        {
            var name = normalizedName ?? string.Empty;
            if (!string.IsNullOrEmpty(family) && name.StartsWith(family, StringComparison.Ordinal))
            {
                name = name.Substring(family.Length);
            }

            return StripTrailingLeaf(name);
        }

        private static string StripTrailingLeaf(string variant)
        {
            if (string.IsNullOrEmpty(variant))
            {
                return string.Empty;
            }

            // Directions — keep as part of row key (distinct row groupings)
            for (int i = 0; i < Directions.Length; i++)
            {
                if (variant.EndsWith(Directions[i], StringComparison.Ordinal))
                {
                    return variant;
                }
            }

            // Compound axes — keep as part of row key
            for (int i = 0; i < CompoundAxes.Length; i++)
            {
                if (variant.EndsWith(CompoundAxes[i], StringComparison.Ordinal))
                {
                    return variant;
                }
            }

            // Single axes — keep as part of row key (e.g. FadeX/FadeY/FadeZ)
            for (int i = 0; i < SingleAxes.Length; i++)
            {
                if (variant.EndsWith(SingleAxes[i], StringComparison.Ordinal))
                {
                    return variant;
                }
            }

            // Sizes — strip only if remaining is non-empty
            for (int i = 0; i < Sizes.Length; i++)
            {
                if (variant.EndsWith(Sizes[i], StringComparison.Ordinal))
                {
                    var remaining = variant.Substring(0, variant.Length - 1);
                    if (remaining.Length > 0)
                    {
                        return remaining;
                    }
                }
            }

            return variant;
        }

        private static string DetectIntensity(string presetName)
        {
            if (string.IsNullOrEmpty(presetName))
            {
                return "Base";
            }

            if (presetName.EndsWith("Soft", StringComparison.Ordinal)) return "Soft";
            if (presetName.EndsWith("Hard", StringComparison.Ordinal)) return "Hard";
            return "Base";
        }

        private static string DetectDirection(string normalizedName)
        {
            if (string.IsNullOrEmpty(normalizedName))
            {
                return string.Empty;
            }

            string[] order = { "Forward", "Back", "Up", "Down", "Left", "Right" };
            for (int i = 0; i < order.Length; i++)
            {
                if (normalizedName.Contains(order[i], StringComparison.Ordinal))
                {
                    return order[i];
                }
            }

            return string.Empty;
        }

        private static string DetectPlane(string normalizedName)
        {
            if (string.IsNullOrEmpty(normalizedName))
            {
                return string.Empty;
            }

            if (normalizedName.Contains("XY", StringComparison.Ordinal)) return "XY";
            if (normalizedName.Contains("XZ", StringComparison.Ordinal)) return "XZ";
            if (normalizedName.Contains("YZ", StringComparison.Ordinal)) return "YZ";
            if (normalizedName.Contains("2D", StringComparison.Ordinal)) return "2D";
            return string.Empty;
        }

        private static string DetectAxis(string normalizedName)
        {
            if (string.IsNullOrEmpty(normalizedName))
            {
                return string.Empty;
            }

            if (normalizedName.Contains("Diagonal", StringComparison.Ordinal))
            {
                return string.Empty;
            }

            if (normalizedName.Contains("XY", StringComparison.Ordinal) ||
                normalizedName.Contains("XZ", StringComparison.Ordinal) ||
                normalizedName.Contains("YZ", StringComparison.Ordinal))
            {
                return string.Empty;
            }

            if (normalizedName.EndsWith("X", StringComparison.Ordinal)) return "X";
            if (normalizedName.EndsWith("Y", StringComparison.Ordinal)) return "Y";
            if (normalizedName.EndsWith("Z", StringComparison.Ordinal)) return "Z";

            return string.Empty;
        }

        private static List<string> DetectModifiers(string presetName)
        {
            var modifiers = new List<string>();
            if (string.IsNullOrEmpty(presetName))
            {
                return modifiers;
            }

            for (int i = 0; i < KnownModifiers.Length; i++)
            {
                var modifier = KnownModifiers[i];
                if (presetName.Contains(modifier, StringComparison.Ordinal) && !modifiers.Contains(modifier))
                {
                    modifiers.Add(modifier);
                }
            }

            return modifiers;
        }
    }
}
