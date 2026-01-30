using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace LB.TweenHelper.Editor
{
    /// <summary>
    /// Editor tool that exports all built-in preset documentation to a .txt file.
    /// Collects preset metadata from the registry and XML doc comments from source.
    /// </summary>
    public static class PresetDocumentationExporter
    {
        private static readonly string[] CategoryOrder =
        {
            PresetCategories.Base,
            PresetCategories.Scale,
            PresetCategories.Movement,
            PresetCategories.Fade,
            PresetCategories.Rotation,
            PresetCategories.Combined
        };

        /// <summary>
        /// Exports all registered preset documentation to a user-chosen .txt file.
        /// </summary>
        [MenuItem("LB/TweenHelper/Export Preset Documentation", false, 4)]
        public static void ExportPresetDocumentation()
        {
            // Ensure presets are scanned
            TweenPresetRegistry.ScanForCodePresets();

            var presets = TweenPresetRegistry.Presets.ToList();
            if (presets.Count == 0)
            {
                EditorUtility.DisplayDialog(
                    "No Presets Found",
                    "No presets are registered. Ensure BuiltInPresets.cs is compiled.",
                    "OK");
                return;
            }

            // Read XML summaries from source
            var xmlSummaries = ExtractXmlSummaries();

            // Ask user where to save
            string path = EditorUtility.SaveFilePanel(
                "Export Preset Documentation",
                Application.dataPath,
                "TweenHelper_Presets",
                "txt");

            if (string.IsNullOrEmpty(path))
                return;

            string content = BuildDocumentation(presets, xmlSummaries);
            File.WriteAllText(path, content, Encoding.UTF8);

            Debug.Log($"Preset documentation exported to: {path}");
            EditorUtility.DisplayDialog(
                "Export Complete",
                $"Exported {presets.Count} presets to:\n{path}",
                "OK");
        }

        private static string BuildDocumentation(List<ITweenPreset> presets, Dictionary<string, string> xmlSummaries)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== TweenHelper — Built-In Animation Presets ===");
            sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd}");
            sb.AppendLine();

            // Group by category
            var grouped = presets
                .GroupBy(p => (p is ICategorizedTweenPreset cat) ? cat.Category : "Uncategorized")
                .ToDictionary(g => g.Key, g => g.OrderBy(p => p.PresetName).ToList());

            foreach (string category in CategoryOrder)
            {
                if (!grouped.TryGetValue(category, out var categoryPresets))
                    continue;

                sb.AppendLine("────────────────────────────────────────");
                sb.AppendLine($"Category: {category}");
                sb.AppendLine("────────────────────────────────────────");
                sb.AppendLine();

                foreach (var preset in categoryPresets)
                {
                    sb.AppendLine($"[{preset.PresetName}]");
                    sb.AppendLine($"Description: {preset.Description}");

                    if (preset is ICategorizedTweenPreset categorized)
                        sb.AppendLine($"Category:    {categorized.Category}");

                    sb.AppendLine();

                    // Append XML summary if available
                    string className = preset.GetType().Name;
                    if (xmlSummaries.TryGetValue(className, out string summary) && !string.IsNullOrWhiteSpace(summary))
                    {
                        sb.AppendLine(summary);
                        sb.AppendLine();
                    }

                    sb.AppendLine("────────────────────────────────────────");
                    sb.AppendLine();
                }

                grouped.Remove(category);
            }

            // Any remaining categories not in CategoryOrder
            foreach (var kvp in grouped.OrderBy(k => k.Key))
            {
                sb.AppendLine("────────────────────────────────────────");
                sb.AppendLine($"Category: {kvp.Key}");
                sb.AppendLine("────────────────────────────────────────");
                sb.AppendLine();

                foreach (var preset in kvp.Value)
                {
                    sb.AppendLine($"[{preset.PresetName}]");
                    sb.AppendLine($"Description: {preset.Description}");

                    if (preset is ICategorizedTweenPreset categorized)
                        sb.AppendLine($"Category:    {categorized.Category}");

                    sb.AppendLine();

                    string className = preset.GetType().Name;
                    if (xmlSummaries.TryGetValue(className, out string summary) && !string.IsNullOrWhiteSpace(summary))
                    {
                        sb.AppendLine(summary);
                        sb.AppendLine();
                    }

                    sb.AppendLine("────────────────────────────────────────");
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        private static Dictionary<string, string> ExtractXmlSummaries()
        {
            var summaries = new Dictionary<string, string>();

            // Find BuiltInPresets.cs via AssetDatabase
            string[] guids = AssetDatabase.FindAssets("BuiltInPresets t:MonoScript");
            if (guids.Length == 0)
            {
                Debug.LogWarning("PresetDocumentationExporter: Could not find BuiltInPresets.cs source file.");
                return summaries;
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            string fullPath = Path.GetFullPath(assetPath);

            if (!File.Exists(fullPath))
            {
                Debug.LogWarning($"PresetDocumentationExporter: Source file not found at {fullPath}");
                return summaries;
            }

            string source = File.ReadAllText(fullPath);

            // Match each XML summary block followed by a class declaration
            // Pattern: /// <summary> ... /// </summary> ... class {Name} : CodePreset
            var pattern = new Regex(
                @"((?:\s*///[^\n]*\n)+)\s*\[AutoRegisterPreset\]\s*\n\s*public\s+class\s+(\w+)",
                RegexOptions.Multiline);

            foreach (Match match in pattern.Matches(source))
            {
                string docBlock = match.Groups[1].Value;
                string className = match.Groups[2].Value;

                string cleaned = CleanXmlDocBlock(docBlock);
                if (!string.IsNullOrWhiteSpace(cleaned))
                {
                    summaries[className] = cleaned;
                }
            }

            return summaries;
        }

        private static string CleanXmlDocBlock(string docBlock)
        {
            // Split into lines and strip /// prefix
            var lines = docBlock.Split('\n')
                .Select(line => line.Trim())
                .Where(line => line.StartsWith("///"))
                .Select(line => line.Length > 3 ? line.Substring(3) : string.Empty)
                .Select(line => line.TrimStart())
                .ToList();

            string joined = string.Join("\n", lines);

            // Remove XML tags
            joined = Regex.Replace(joined, @"</?summary>", string.Empty);
            joined = Regex.Replace(joined, @"</?para>", "\n");
            joined = Regex.Replace(joined, @"<br\s*/?>", "\n");
            joined = Regex.Replace(joined, @"<c>(.*?)</c>", "$1");
            joined = Regex.Replace(joined, @"<b>(.*?)</b>", "$1");
            joined = Regex.Replace(joined, @"</?[a-zA-Z][^>]*>", string.Empty);

            // Clean up whitespace: collapse multiple blank lines, trim each line
            var cleanedLines = joined.Split('\n')
                .Select(l => l.Trim())
                .ToList();

            // Remove leading/trailing empty lines and collapse multiple blank lines
            var result = new List<string>();
            bool lastWasEmpty = true;
            foreach (string line in cleanedLines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    if (!lastWasEmpty)
                        result.Add(string.Empty);
                    lastWasEmpty = true;
                }
                else
                {
                    result.Add(line);
                    lastWasEmpty = false;
                }
            }

            // Trim leading/trailing blank lines
            while (result.Count > 0 && string.IsNullOrEmpty(result[0]))
                result.RemoveAt(0);
            while (result.Count > 0 && string.IsNullOrEmpty(result[result.Count - 1]))
                result.RemoveAt(result.Count - 1);

            return string.Join("\n", result);
        }
    }
}
