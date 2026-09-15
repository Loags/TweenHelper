using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LB.TweenHelper.Editor
{
    internal static class PresetSelectionCatalog
    {
        private static int _version = -1;
        private static List<ITweenPreset> _presets = new List<ITweenPreset>();

        public static IReadOnlyList<ITweenPreset> Presets
        {
            get
            {
                int version = TweenPresetRegistry.Version;
                if (_version == version) return _presets;
                _presets = TweenPresetRegistry.Presets.OrderBy(preset => preset.PresetName, StringComparer.Ordinal).ToList();
                _version = version;
                return _presets;
            }
        }

        public static string GetCategory(ITweenPreset preset)
        {
            string category = preset.GetType().GetCustomAttribute<PresetCategoryAttribute>()?.Category;
            if (!string.IsNullOrWhiteSpace(category)) return category;
            return preset.GetType().Assembly == typeof(CodePreset).Assembly ? PresetFamilyClassifier.GetFamilyName(preset.PresetName) : "Custom";
        }

        public static bool SupportsPreview(ITweenPreset preset) => preset != null && (preset.GetType().Assembly == typeof(CodePreset).Assembly || preset.GetType().IsDefined(typeof(PresetPreviewSupportedAttribute), false));
    }
}
