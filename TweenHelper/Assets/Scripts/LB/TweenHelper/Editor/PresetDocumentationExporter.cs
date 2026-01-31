using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DG.Tweening;
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
                    "No presets are registered. Ensure preset scripts are compiled.",
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

            sb.Append(BuildProjectInfoHeader(presets));

            // Alphabetical flat list
            var sorted = presets.OrderBy(p => p.PresetName).ToList();

            sb.AppendLine("────────────────────────────────────────");
            sb.AppendLine("Presets");
            sb.AppendLine("────────────────────────────────────────");
            sb.AppendLine();

            foreach (var preset in sorted)
            {
                sb.AppendLine($"[{preset.PresetName}]");
                sb.AppendLine($"Description: {preset.Description}");
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

            return sb.ToString();
        }

        private static string BuildProjectInfoHeader(List<ITweenPreset> presets)
        {
            const string separator = "────────────────────────────────────────";
            var sb = new StringBuilder();

            // 1. Project Metadata
            sb.AppendLine(separator);
            sb.AppendLine("Project Information");
            sb.AppendLine(separator);
            sb.AppendLine();
            sb.AppendLine($"Unity Version:   {Application.unityVersion}");
            sb.AppendLine($"DOTween Version: {DOTween.Version}");
            sb.AppendLine($"Repository:      https://github.com/Loags/TweenHelper");
            sb.AppendLine($"Author:          Loags");
            sb.AppendLine($"License:         MIT");
            sb.AppendLine();

            // 2. Project Summary
            sb.AppendLine(separator);
            sb.AppendLine("About TweenHelper");
            sb.AppendLine(separator);
            sb.AppendLine();
            sb.AppendLine("TweenHelper is a high-level animation facade built on DOTween for Unity.");
            sb.AppendLine("It provides a fluent builder API, named presets with auto-registration,");
            sb.AppendLine("sequence composition, and automatic cleanup via GameObject linking.");
            sb.AppendLine();

            // 3. What's Included
            sb.AppendLine(separator);
            sb.AppendLine("What's Included");
            sb.AppendLine(separator);
            sb.AppendLine();
            sb.AppendLine($"Total Presets: {presets.Count}");
            sb.AppendLine();
            sb.AppendLine("Builder Capabilities:");
            sb.AppendLine("  Move, Rotate, Scale, Fade, Sequence (Then/With), Async/Await");
            sb.AppendLine();

            // 4. Architecture Overview
            sb.AppendLine(separator);
            sb.AppendLine("Architecture Overview");
            sb.AppendLine(separator);
            sb.AppendLine();
            sb.AppendLine("Fluent Builder    — transform.Tween().Move(...).WithEase(...).Play()");
            sb.AppendLine("Preset System     — [AutoRegisterPreset] attribute + TweenPresetRegistry auto-discovery");
            sb.AppendLine("TweenHandle       — Wrapper with control (Pause/Resume/Kill), status, and async/await");
            sb.AppendLine("TweenOptions      — Value-type struct with nullable fields for per-call overrides");
            sb.AppendLine("Auto-Cleanup      — SetLink(gameObject) + TweenLifecycleTracker fallback");
            sb.AppendLine("Global Defaults   — TweenHelperSettings singleton ScriptableObject");
            sb.AppendLine();

            // 5. Current Default Settings
            var settings = TweenHelperSettings.Instance;
            sb.AppendLine(separator);
            sb.AppendLine("Current Default Settings");
            sb.AppendLine(separator);
            sb.AppendLine();
            sb.AppendLine($"Default Duration:  {settings.DefaultDuration}s");
            sb.AppendLine($"Default Ease:      {settings.DefaultEase}");
            sb.AppendLine($"Default Delay:     {settings.DefaultDelay}s");
            sb.AppendLine();
            sb.AppendLine($"Safe Mode:         {settings.UseSafeMode}");
            sb.AppendLine($"Auto Play:         {settings.EnableAutoPlay}");
            sb.AppendLine($"Auto Kill:         {settings.EnableAutoKill}");
            sb.AppendLine();
            sb.AppendLine($"Tweener Capacity:  {settings.MaxTweenersCapacity}");
            sb.AppendLine($"Sequence Capacity: {settings.MaxSequencesCapacity}");
            sb.AppendLine();
            sb.AppendLine();

            return sb.ToString();
        }

        private static Dictionary<string, string> ExtractXmlSummaries()
        {
            var summaries = new Dictionary<string, string>();

            // Scan all .cs files in the Presets folder
            string[] guids = AssetDatabase.FindAssets("t:MonoScript",
                new[] { "Assets/Scripts/LB/TweenHelper/Runtime/Presets" });

            if (guids.Length == 0)
            {
                Debug.LogWarning("PresetDocumentationExporter: Could not find any scripts in Runtime/Presets/.");
                return summaries;
            }

            var pattern = new Regex(
                @"((?:\s*///[^\n]*\n)+)\s*\[AutoRegisterPreset\]\s*\n\s*public\s+class\s+(\w+)",
                RegexOptions.Multiline);

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string fullPath = Path.GetFullPath(assetPath);

                if (!File.Exists(fullPath))
                    continue;

                string source = File.ReadAllText(fullPath);

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
