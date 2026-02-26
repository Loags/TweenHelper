using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace LB.TweenHelper.Editor
{
    /// <summary>
    /// Editor-side integrity checks for the preset registry, naming matrices, and exported catalog files.
    /// </summary>
    public static class PresetValidationTools
    {
        [MenuItem("LB/TweenHelper/Validate/Registry Integrity", false, 20)]
        public static void ValidateRegistryIntegrity()
        {
            TweenPresetRegistry.Refresh();
            var presets = TweenPresetRegistry.Presets.OrderBy(p => p.PresetName).ToList();
            var errors = new List<string>();
            var warnings = new List<string>();

            if (presets.Count == 0)
            {
                errors.Add("No presets registered.");
            }

            var duplicateNames = presets
                .GroupBy(p => p.PresetName)
                .Where(g => string.IsNullOrWhiteSpace(g.Key) || g.Count() > 1)
                .ToList();

            foreach (var dup in duplicateNames)
            {
                if (string.IsNullOrWhiteSpace(dup.Key))
                {
                    errors.Add("Found preset with null/empty PresetName.");
                }
                else
                {
                    errors.Add($"Duplicate PresetName '{dup.Key}' ({dup.Count()} entries).");
                }
            }

            foreach (var preset in presets)
            {
                try
                {
                    if (preset.CanApplyTo(null))
                    {
                        warnings.Add($"{preset.PresetName}: CanApplyTo(null) returned true.");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"{preset.PresetName}: CanApplyTo(null) threw {ex.GetType().Name}: {ex.Message}");
                }
            }

            ValidateMatrixFamilies(presets, errors, warnings);
            ValidateLayoutGrouping(presets, errors, warnings);

            var familyCounts = presets
                .GroupBy(p => PresetFamilyClassifier.GetFamilyName(p.PresetName))
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)
                .Select(g => $"{g.Key}={g.Count()}")
                .ToArray();

            LogValidationSummary("Registry Integrity", presets.Count, errors, warnings, familyCounts);
        }

        [MenuItem("LB/TweenHelper/Validate/Catalog File Against Registry...", false, 21)]
        public static void ValidateCatalogFileAgainstRegistry()
        {
            TweenPresetRegistry.Refresh();
            var registryNames = new HashSet<string>(TweenPresetRegistry.PresetNames, StringComparer.Ordinal);

            string path = EditorUtility.OpenFilePanel("Select TweenHelper preset catalog (.txt)", Application.dataPath, "txt");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            string content;
            try
            {
                content = System.IO.File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Validation Failed", $"Could not read file:\n{ex.Message}", "OK");
                return;
            }

            var docNames = new HashSet<string>(ExtractCatalogNames(content), StringComparer.Ordinal);
            var codeOnly = registryNames.Except(docNames).OrderBy(n => n).ToList();
            var docOnly = docNames.Except(registryNames).OrderBy(n => n).ToList();

            var errors = new List<string>();
            var warnings = new List<string>();

            if (codeOnly.Count > 0)
            {
                errors.Add($"Code-only presets: {string.Join(", ", codeOnly)}");
            }

            if (docOnly.Count > 0)
            {
                errors.Add($"Catalog-only presets: {string.Join(", ", docOnly)}");
            }

            if (docNames.Count == 0)
            {
                errors.Add("No preset names parsed from catalog file.");
            }

            LogValidationSummary("Catalog File", registryNames.Count, errors, warnings, new[]
            {
                $"Registry names: {registryNames.Count}",
                $"Catalog names: {docNames.Count}",
                $"Path: {path}"
            });
        }

        private static void ValidateMatrixFamilies(List<ITweenPreset> presets, List<string> errors, List<string> warnings)
        {
            var names = new HashSet<string>(presets.Select(p => p.PresetName), StringComparer.Ordinal);

            ExpectMatrix(names, "SlideOutFade", "SlideOutFade",
                new[] { "SlideOutFadeUp", "SlideOutFadeDown", "SlideOutFadeLeft", "SlideOutFadeRight" },
                new[] { string.Empty, "Soft", "Hard" },
                errors, warnings);

            ExpectMatrix(names, "OrbitYZ", "OrbitYZ",
                new[] { "OrbitYZ", "OrbitYZClockwise", "OrbitYZCounterClockwise" },
                new[] { string.Empty, "Soft", "Hard" },
                errors, warnings);

            ExpectMatrix(names, "PendulumAxes", "Pendulum",
                new[] { "PendulumX", "PendulumY", "PendulumZ" },
                new[] { string.Empty, "Soft", "Hard" },
                errors, warnings);

            ExpectMatrix(names, "SpinScaleIn", "SpinScaleIn",
                new[] { "SpinScaleIn", "SpinScaleInOvershoot" },
                new[] { string.Empty, "Soft", "Hard" },
                errors, warnings);

            ExpectMatrix(names, "FlipFade", "FlipFade",
                new[] { "FlipFadeX", "FlipFadeY", "FlipFadeZ" },
                new[] { string.Empty, "Soft", "Hard" },
                errors, warnings);

            ExpectMatrix(names, "WobbleFade", "WobbleFade",
                new[] { "WobbleFadeX", "WobbleFadeY", "WobbleFadeZ" },
                new[] { string.Empty, "Soft", "Hard" },
                errors, warnings);

            ExpectMatrix(names, "PulseScaleFade", "PulseScaleFade",
                new[] { "PulseScaleFade" },
                new[] { string.Empty, "Soft", "Hard" },
                errors, warnings);

            // Symmetry warning (not hard error): SpinScaleIn and SpinScaleOut suffix sets should match.
            var inSuffixes = names.Where(n => n.StartsWith("SpinScaleIn", StringComparison.Ordinal))
                .Select(n => n.Substring("SpinScaleIn".Length))
                .OrderBy(n => n)
                .ToArray();
            var outSuffixes = names.Where(n => n.StartsWith("SpinScaleOut", StringComparison.Ordinal))
                .Select(n => n.Substring("SpinScaleOut".Length))
                .OrderBy(n => n)
                .ToArray();

            if (!inSuffixes.SequenceEqual(outSuffixes))
            {
                warnings.Add($"SpinScaleIn/Out suffix mismatch. In=[{string.Join(", ", inSuffixes)}], Out=[{string.Join(", ", outSuffixes)}]");
            }
        }

        private sealed class ParsedPresetRowEntry
        {
            public string Name;
            public PresetVariantMetadata Meta;
        }

        private static void ValidateLayoutGrouping(List<ITweenPreset> presets, List<string> errors, List<string> warnings)
        {
            var entries = presets.Select(p => new ParsedPresetRowEntry
            {
                Name = p.PresetName,
                Meta = PresetVariantParser.Parse(p.PresetName)
            }).ToList();

            foreach (var familyGroup in entries.GroupBy(e => e.Meta.Family))
            {
                foreach (var rowGroup in familyGroup.GroupBy(e => e.Meta.SubRowKey))
                {
                    var ordered = rowGroup
                        .OrderBy(e => e.Meta.ColumnSortScore)
                        .ThenBy(e => e.Name, StringComparer.Ordinal)
                        .ToList();

                    ValidateIntensityOrderingForRow(familyGroup.Key, rowGroup.Key, ordered, errors);
                }
            }

            ExpectDistinctSubRows(entries, "FlipFade", new[] { "FlipFadeX", "FlipFadeY", "FlipFadeZ" }, errors);
            ExpectDistinctSubRows(entries, "WobbleFade", new[] { "WobbleFadeX", "WobbleFadeY", "WobbleFadeZ" }, errors);
        }

        private static void ValidateIntensityOrderingForRow(string family, string rowKey, List<ParsedPresetRowEntry> ordered, List<string> errors)
        {
            foreach (var variantGroup in ordered.GroupBy(e => e.Meta.NormalizedForLayout))
            {
                var byIntensity = new Dictionary<string, ParsedPresetRowEntry>(StringComparer.Ordinal);
                foreach (var entry in variantGroup)
                {
                    if (byIntensity.ContainsKey(entry.Meta.Intensity))
                    {
                        errors.Add($"Layout row {family}/{rowKey}: duplicate intensity '{entry.Meta.Intensity}' for '{variantGroup.Key}'.");
                        continue;
                    }

                    byIntensity[entry.Meta.Intensity] = entry;
                }

                if (!(byIntensity.ContainsKey("Soft") && byIntensity.ContainsKey("Base") && byIntensity.ContainsKey("Hard")))
                {
                    continue;
                }

                int softIndex = ordered.FindIndex(e => string.Equals(e.Name, byIntensity["Soft"].Name, StringComparison.Ordinal));
                int baseIndex = ordered.FindIndex(e => string.Equals(e.Name, byIntensity["Base"].Name, StringComparison.Ordinal));
                int hardIndex = ordered.FindIndex(e => string.Equals(e.Name, byIntensity["Hard"].Name, StringComparison.Ordinal));

                bool inOrder = softIndex >= 0 &&
                               baseIndex == softIndex + 1 &&
                               hardIndex == baseIndex + 1;

                if (!inOrder)
                {
                    errors.Add($"Layout row {family}/{rowKey}: expected Soft | Default | Hard for '{variantGroup.Key}' but found '{string.Join(" | ", ordered.Select(e => e.Name))}'.");
                }
            }
        }

        private static void ExpectDistinctSubRows(IEnumerable<ParsedPresetRowEntry> entries, string label, IEnumerable<string> presetNames, List<string> errors)
        {
            var expected = presetNames.ToList();
            var matched = entries.Where(e => expected.Contains(e.Name)).ToList();

            if (matched.Count != expected.Count)
            {
                var missing = expected.Except(matched.Select(e => e.Name)).OrderBy(n => n).ToList();
                if (missing.Count > 0)
                {
                    errors.Add($"{label}: Cannot validate row split; missing presets: {string.Join(", ", missing)}");
                }
                return;
            }

            var distinctRows = matched.Select(e => e.Meta.SubRowKey).Distinct(StringComparer.Ordinal).ToList();
            if (distinctRows.Count != expected.Count)
            {
                errors.Add($"{label}: Expected distinct sub-rows for {string.Join(", ", expected)} but got rows [{string.Join(", ", distinctRows)}].");
            }
        }

        private static void ExpectMatrix(
            HashSet<string> allNames,
            string label,
            string unexpectedPrefix,
            IEnumerable<string> stems,
            IEnumerable<string> suffixes,
            List<string> errors,
            List<string> warnings)
        {
            var expected = new List<string>();
            foreach (var stem in stems)
            {
                foreach (var suffix in suffixes)
                {
                    expected.Add(stem + suffix);
                }
            }

            ExpectExact(allNames, label, unexpectedPrefix, expected, errors, warnings);
        }

        private static void ExpectExact(
            HashSet<string> allNames,
            string label,
            IEnumerable<string> expectedNames,
            List<string> errors,
            List<string> warnings)
        {
            ExpectExact(allNames, label, label, expectedNames, errors, warnings);
        }

        private static void ExpectExact(
            HashSet<string> allNames,
            string label,
            string unexpectedPrefix,
            IEnumerable<string> expectedNames,
            List<string> errors,
            List<string> warnings)
        {
            var expected = new HashSet<string>(expectedNames, StringComparer.Ordinal);
            var present = new HashSet<string>(allNames.Where(n => expected.Contains(n)), StringComparer.Ordinal);

            var missing = expected.Except(present).OrderBy(n => n).ToList();
            var unexpected = allNames.Where(n => n.StartsWith(unexpectedPrefix, StringComparison.Ordinal) && !expected.Contains(n)).OrderBy(n => n).ToList();

            if (missing.Count > 0)
            {
                errors.Add($"{label}: Missing expected presets: {string.Join(", ", missing)}");
            }

            if (unexpected.Count > 0)
            {
                warnings.Add($"{label}: Unexpected additional presets: {string.Join(", ", unexpected)}");
            }
        }

        private static IEnumerable<string> ExtractCatalogNames(string content)
        {
            var matches = Regex.Matches(content, "Preset\\(\"([^\"]+)\"\\)");
            for (int i = 0; i < matches.Count; i++)
            {
                var name = matches[i].Groups[1].Value;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    yield return name;
                }
            }
        }

        private static void LogValidationSummary(string kind, int presetCount, List<string> errors, List<string> warnings, IEnumerable<string> extraLines)
        {
            bool ok = errors.Count == 0;
            string title = ok ? $"{kind} Validation Passed" : $"{kind} Validation Failed";

            if (ok)
            {
                Debug.Log($"TweenHelper {kind} validation passed. Presets: {presetCount}");
            }
            else
            {
                Debug.LogError($"TweenHelper {kind} validation failed with {errors.Count} error(s).");
            }

            foreach (var warning in warnings)
            {
                Debug.LogWarning($"TweenHelper {kind}: {warning}");
            }

            foreach (var error in errors)
            {
                Debug.LogError($"TweenHelper {kind}: {error}");
            }

            if (extraLines != null)
            {
                foreach (var line in extraLines)
                {
                    Debug.Log($"TweenHelper {kind}: {line}");
                }
            }

            var message = ok
                ? $"Validation passed.\nPresets: {presetCount}\nWarnings: {warnings.Count}"
                : $"Validation failed.\nErrors: {errors.Count}\nWarnings: {warnings.Count}";

            EditorUtility.DisplayDialog(title, message, "OK");
        }
    }
}
