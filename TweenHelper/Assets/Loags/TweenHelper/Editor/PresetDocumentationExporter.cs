using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LB.TweenHelper.Editor
{
    public static class PresetDocumentationExporter
    {
        [MenuItem("Tools/TweenHelper/Export Preset Catalog", false, 4)]
        public static void ExportPresetCatalog()
        {
            if (!TryExportPresetCatalog(out string path, out string error))
            {
                Debug.LogError(error);
                EditorUtility.DisplayDialog("Preset Catalog Failed", error, "OK");
                return;
            }

            Debug.Log($"TweenHelper preset catalog exported to {path}");
            EditorUtility.DisplayDialog("Preset Catalog Exported", $"Exported {TweenPresetRegistry.Count} presets to:\n{path}", "OK");
        }

        [MenuItem("Tools/TweenHelper/Validate/Regenerate Preset Catalog (Non-Interactive)", false, 120)]
        public static void ExportPresetCatalogNonInteractive()
        {
            if (TryExportPresetCatalog(out string path, out string error))
            {
                Debug.Log($"TweenHelper preset catalog validation passed. Exported {TweenPresetRegistry.Count} presets to {path}");
                return;
            }

            Debug.LogError($"TweenHelper preset catalog validation failed: {error}");
        }

        [MenuItem("Tools/TweenHelper/Export Preset Catalog As...", false, 5)]
        public static void ExportPresetCatalogAs()
        {
            if (!TryBuildCatalog(out string content, out string error))
            {
                Debug.LogError(error);
                EditorUtility.DisplayDialog("Preset Catalog Failed", error, "OK");
                return;
            }

            string path = EditorUtility.SaveFilePanel("Export TweenHelper Preset Catalog", Application.dataPath, "PresetCatalog", "md");
            if (string.IsNullOrEmpty(path)) return;

            File.WriteAllText(path, content, new UTF8Encoding(false));
            AssetDatabase.Refresh();
            Debug.Log($"TweenHelper preset catalog exported to {path}");
        }

        public static bool TryExportPresetCatalog(out string path, out string error)
        {
            path = GetDefaultCatalogPath();
            if (!TryBuildCatalog(out string content, out error)) return false;

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, content, new UTF8Encoding(false));
            AssetDatabase.Refresh();
            return true;
        }

        public static bool TryBuildCatalog(out string content, out string error)
        {
            var presetTypes = typeof(ITweenPreset).Assembly.GetTypes()
                .Where(type => !type.IsAbstract &&
                               typeof(ITweenPreset).IsAssignableFrom(type) &&
                               Attribute.IsDefined(type, typeof(AutoRegisterPresetAttribute)))
                .ToList();

            var presets = new List<ITweenPreset>();
            foreach (var type in presetTypes)
            {
                try
                {
                    presets.Add((ITweenPreset)Activator.CreateInstance(type));
                }
                catch (Exception exception)
                {
                    content = null;
                    error = $"Could not construct preset type {type.FullName}: {exception.Message}";
                    return false;
                }
            }

            var duplicateNames = presets.GroupBy(preset => preset.PresetName, StringComparer.Ordinal)
                .Where(group => string.IsNullOrWhiteSpace(group.Key) || group.Count() > 1)
                .Select(group => string.IsNullOrWhiteSpace(group.Key) ? "<empty>" : group.Key)
                .ToList();

            if (duplicateNames.Count > 0)
            {
                content = null;
                error = $"Preset names must be unique before the catalog can be generated: {string.Join(", ", duplicateNames)}";
                return false;
            }

            TweenPresetRegistry.Refresh();
            if (TweenPresetRegistry.Count != presets.Count)
            {
                content = null;
                error = $"Registry count {TweenPresetRegistry.Count} does not match discovered preset count {presets.Count}.";
                return false;
            }

            content = BuildMarkdown(presets);
            error = null;
            return true;
        }

        private static string BuildMarkdown(IEnumerable<ITweenPreset> presets)
        {
            var sorted = presets.OrderBy(preset => PresetFamilyClassifier.GetFamilyName(preset.PresetName), StringComparer.Ordinal)
                .ThenBy(preset => preset.PresetName, StringComparer.Ordinal)
                .ToList();

            var builder = new StringBuilder();
            builder.AppendLine("# TweenHelper preset catalog");
            builder.AppendLine();
            builder.AppendLine("> Generated deterministically from the registered TweenHelper preset types.");
            builder.AppendLine();
            builder.AppendLine($"Built-in presets: **{sorted.Count}**");
            builder.AppendLine();
            builder.AppendLine("Regenerate this file with **Tools > TweenHelper > Export Preset Catalog**. Generation fails on duplicate, empty, or unconstructible presets.");

            foreach (var family in sorted.GroupBy(preset => PresetFamilyClassifier.GetFamilyName(preset.PresetName)))
            {
                builder.AppendLine();
                builder.AppendLine($"## {Escape(family.Key)}");
                builder.AppendLine();
                builder.AppendLine("| Preset | Duration | Loop | Target | Description | Fluent API |");
                builder.AppendLine("| --- | ---: | --- | --- | --- | --- |");

                foreach (var preset in family)
                {
                    string duration = preset.DefaultDuration.ToString("0.###", CultureInfo.InvariantCulture);
                    builder.AppendLine($"| `{Escape(preset.PresetName)}` | {duration}s | {GetLoopBehavior(preset)} | {GetTargetCategory(preset)} | {Escape(preset.Description)} | `target.Tween().Preset(\"{Escape(preset.PresetName)}\").Play();` |");
                }
            }

            return builder.ToString();
        }

        private static string GetDefaultCatalogPath()
        {
            return Path.Combine(Application.dataPath, "Loags", "TweenHelper", "Documentation", "PresetCatalog.md");
        }

        private static string GetLoopBehavior(ITweenPreset preset)
        {
            string family = PresetFamilyClassifier.GetFamilyName(preset.PresetName);
            bool isInfinite = string.Equals(family, "Orbit", StringComparison.Ordinal) ||
                              preset.Description.IndexOf(" loop", StringComparison.OrdinalIgnoreCase) >= 0;
            return isInfinite ? "Infinite" : "One-shot";
        }

        private static string GetTargetCategory(ITweenPreset preset)
        {
            return preset.PresetName.IndexOf("Fade", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   preset.PresetName.StartsWith("Blink", StringComparison.Ordinal) ||
                   preset.PresetName.StartsWith("Flicker", StringComparison.Ordinal)
                ? "Fade-capable GameObject"
                : "GameObject";
        }

        private static string Escape(string value)
        {
            return string.IsNullOrEmpty(value)
                ? string.Empty
                : value.Replace("|", "\\|").Replace("\r", " ").Replace("\n", " ");
        }
    }
}
