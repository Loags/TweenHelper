using System;

namespace LB.TweenHelper.Editor
{
    internal enum PresetBrowserEntryKind
    {
        Preset,
        CollectionRecipe,
        StaggerVariant
    }

    internal enum PresetBrowserPreviewKind
    {
        Single,
        List,
        Grid,
        LoadingDots
    }

    internal enum PresetBrowserCollectionKind
    {
        ListStaggerIn,
        ListStaggerOut,
        GridWave,
        GridRipple,
        LoadingDots,
        OrderFirstToLast,
        OrderLastToFirst,
        OrderFromCenter,
        OrderToCenter,
        OrderRandom,
        GridWaveRightToLeft,
        GridWaveTopToBottom,
        GridWaveBottomToTop
    }

    internal sealed class PresetBrowserEntry
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Family { get; }
        public string Intensity { get; }
        public string Direction { get; }
        public string AxisOrPlane { get; }
        public string Duration { get; }
        public string Example { get; }
        public PresetBrowserEntryKind Kind { get; }
        public PresetBrowserPreviewKind PreviewKind { get; }
        public PresetBrowserCollectionKind CollectionKind { get; }
        public ITweenPreset Preset { get; }

        public bool IsCollection => Kind != PresetBrowserEntryKind.Preset;
        public string Category => Kind == PresetBrowserEntryKind.Preset ? "Preset" : Kind == PresetBrowserEntryKind.CollectionRecipe ? "Collection recipe" : "Stagger variant";
        public string Badge => Kind == PresetBrowserEntryKind.Preset ? "PRESET" : Kind == PresetBrowserEntryKind.CollectionRecipe ? "RECIPE" : "ORDER";

        private PresetBrowserEntry(string id, string name, string description, string family, string intensity, string direction, string axisOrPlane, string duration, string example, PresetBrowserEntryKind kind, PresetBrowserPreviewKind previewKind, PresetBrowserCollectionKind collectionKind, ITweenPreset preset)
        {
            Id = id;
            Name = name;
            Description = description;
            Family = family;
            Intensity = intensity;
            Direction = direction;
            AxisOrPlane = axisOrPlane;
            Duration = duration;
            Example = example;
            Kind = kind;
            PreviewKind = previewKind;
            CollectionKind = collectionKind;
            Preset = preset;
        }

        public static PresetBrowserEntry FromPreset(ITweenPreset preset)
        {
            if (preset == null) throw new ArgumentNullException(nameof(preset));
            PresetVariantMetadata metadata = PresetVariantParser.Parse(preset.PresetName);
            string axisOrPlane = string.IsNullOrEmpty(metadata.Axis) ? metadata.Plane : metadata.Axis;
            return new PresetBrowserEntry(
                "Preset:" + preset.PresetName,
                preset.PresetName,
                preset.Description,
                metadata.Family,
                metadata.Intensity,
                metadata.Direction,
                axisOrPlane,
                $"{preset.DefaultDuration:0.###}s",
                $"target.Tween().Preset<{preset.GetType().Name}>().Play();",
                PresetBrowserEntryKind.Preset,
                PresetBrowserPreviewKind.Single,
                default,
                preset);
        }

        public static PresetBrowserEntry Collection(string name, string description, string duration, string example, PresetBrowserEntryKind kind, PresetBrowserPreviewKind previewKind, PresetBrowserCollectionKind collectionKind, string direction = null)
        {
            return new PresetBrowserEntry(
                "Collection:" + collectionKind,
                name,
                description,
                "Collections",
                string.Empty,
                direction ?? string.Empty,
                string.Empty,
                duration,
                example,
                kind,
                previewKind,
                collectionKind,
                null);
        }
    }
}
