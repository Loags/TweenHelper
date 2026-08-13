using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Unity.Pipeline.Commands;

namespace LB.TweenHelper.Pipeline.Editor
{
    public static class TweenHelperPipelineCommands
    {
        private const int DefaultLimit = 50;
        private const int MaximumLimit = 100;

        private static readonly Lazy<IReadOnlyList<TweenHelperPresetInfo>> Catalog = new Lazy<IReadOnlyList<TweenHelperPresetInfo>>(BuildCatalog);

        [CliCommand(CliCommandTelemetry.CatalogCommandId, "List TweenHelper's built-in presets. Supports optional text/family filters and offset/limit paging.")]
        public static TweenHelperCatalogResult GetCatalog(
            [CliArg("query", "Optional case-insensitive name, family, or description filter.")] string query = null,
            [CliArg("family", "Optional exact preset-family filter.")] string family = null,
            [CliArg("offset", "Zero-based result offset.")] int offset = 0,
            [CliArg("limit", "Maximum results to return (1-100, default 50).")] int limit = DefaultLimit)
        {
            return CliCommandTelemetry.Record(CliCommandTelemetry.CatalogCommandId, () =>
            {
                if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "offset must be zero or greater.");
                if (limit < 1 || limit > MaximumLimit) throw new ArgumentOutOfRangeException(nameof(limit), $"limit must be between 1 and {MaximumLimit}.");

                string normalizedQuery = query?.Trim() ?? string.Empty;
                string normalizedFamily = family?.Trim() ?? string.Empty;
                TweenHelperPresetInfo[] filtered = Catalog.Value
                    .Where(preset => Matches(preset, normalizedQuery, normalizedFamily))
                    .ToArray();
                TweenHelperPresetInfo[] page = filtered.Skip(offset).Take(limit).ToArray();

                return new TweenHelperCatalogResult
                {
                    PresetCount = Catalog.Value.Count,
                    MatchedCount = filtered.Length,
                    Offset = offset,
                    ReturnedCount = page.Length,
                    HasMore = (long)offset + page.Length < filtered.Length,
                    Presets = page
                };
            });
        }

        [CliCommand(CliCommandTelemetry.SummaryCommandId, "Summarize bounded local telemetry for TweenHelper CLI commands.")]
        public static TweenHelperTelemetrySummaryResult GetTelemetrySummary() => CliCommandTelemetry.Record(CliCommandTelemetry.SummaryCommandId, CliCommandTelemetry.BuildSummary);

        private static IReadOnlyList<TweenHelperPresetInfo> BuildCatalog()
        {
            return typeof(ITweenPreset).Assembly.GetTypes()
                .Where(type => !type.IsAbstract && typeof(ITweenPreset).IsAssignableFrom(type) && type.GetCustomAttribute<AutoRegisterPresetAttribute>(false) != null)
                .Select(type => (ITweenPreset)Activator.CreateInstance(type))
                .OrderBy(preset => preset.PresetName, StringComparer.Ordinal)
                .Select(preset =>
                {
                    PresetVariantMetadata metadata = PresetVariantParser.Parse(preset.PresetName);
                    return new TweenHelperPresetInfo
                    {
                        Name = preset.PresetName,
                        Description = preset.Description ?? string.Empty,
                        Family = metadata.Family ?? string.Empty,
                        DefaultDuration = preset.DefaultDuration
                    };
                })
                .ToArray();
        }

        private static bool Matches(TweenHelperPresetInfo preset, string query, string family)
        {
            if (!string.IsNullOrEmpty(family) && !string.Equals(preset.Family, family, StringComparison.OrdinalIgnoreCase)) return false;
            if (string.IsNullOrEmpty(query)) return true;

            return preset.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   preset.Family.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   preset.Description.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }

    public sealed class TweenHelperCatalogResult
    {
        [JsonProperty("presetCount")]
        public int PresetCount { get; set; }

        [JsonProperty("matchedCount")]
        public int MatchedCount { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("returnedCount")]
        public int ReturnedCount { get; set; }

        [JsonProperty("hasMore")]
        public bool HasMore { get; set; }

        [JsonProperty("presets")]
        public TweenHelperPresetInfo[] Presets { get; set; }
    }

    public sealed class TweenHelperPresetInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("family")]
        public string Family { get; set; }

        [JsonProperty("defaultDuration")]
        public float DefaultDuration { get; set; }
    }
}
