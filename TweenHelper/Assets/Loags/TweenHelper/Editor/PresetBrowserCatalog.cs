using System;
using System.Collections.Generic;
using System.Linq;

namespace LB.TweenHelper.Editor
{
    internal static class PresetBrowserCatalog
    {
        public static List<PresetBrowserEntry> Build()
        {
            TweenPresetRegistry.Refresh();
            var entries = TweenPresetRegistry.Presets
                .OrderBy(preset => preset.PresetName, StringComparer.Ordinal)
                .Select(PresetBrowserEntry.FromPreset)
                .ToList();

            entries.AddRange(CreateCollectionEntries());
            return entries;
        }

        private static IEnumerable<PresetBrowserEntry> CreateCollectionEntries()
        {
            yield return PresetBrowserEntry.Collection(
                "ListStaggerIn",
                "Staggers a list into view from the first item to the last.",
                "0.32s per item",
                "items.ListStaggerIn(owner);",
                PresetBrowserEntryKind.CollectionRecipe,
                PresetBrowserPreviewKind.List,
                PresetBrowserCollectionKind.ListStaggerIn,
                "First to last");

            yield return PresetBrowserEntry.Collection(
                "ListStaggerOut",
                "Staggers a list out of view from the last item to the first.",
                "0.26s per item",
                "items.ListStaggerOut(owner);",
                PresetBrowserEntryKind.CollectionRecipe,
                PresetBrowserPreviewKind.List,
                PresetBrowserCollectionKind.ListStaggerOut,
                "Last to first");

            yield return PresetBrowserEntry.Collection(
                "GridWave",
                "Reveals grid columns in a left-to-right wave.",
                "0.32s per item",
                "items.GridWave(owner, columns: 3);",
                PresetBrowserEntryKind.CollectionRecipe,
                PresetBrowserPreviewKind.Grid,
                PresetBrowserCollectionKind.GridWave,
                "Left to right");

            yield return PresetBrowserEntry.Collection(
                "GridRipple",
                "Pulses outward from a centered or explicit origin index.",
                "0.32s per item",
                "items.GridRipple(owner, columns: 3, originIndex: -1);",
                PresetBrowserEntryKind.CollectionRecipe,
                PresetBrowserPreviewKind.Grid,
                PresetBrowserCollectionKind.GridRipple,
                "Center outward");

            yield return PresetBrowserEntry.Collection(
                "LoadingDots",
                "Loops a soft pulse across three loading dots.",
                "0.25s per item",
                "dots.LoadingDots(owner);",
                PresetBrowserEntryKind.CollectionRecipe,
                PresetBrowserPreviewKind.LoadingDots,
                PresetBrowserCollectionKind.LoadingDots,
                "First to last");

            yield return CreateOrderEntry("OrderFirstToLast", "Applies delays from the first collection item to the last.", "FirstToLast", PresetBrowserCollectionKind.OrderFirstToLast, "First to last");
            yield return CreateOrderEntry("OrderLastToFirst", "Applies delays from the last collection item to the first.", "LastToFirst", PresetBrowserCollectionKind.OrderLastToFirst, "Last to first");
            yield return CreateOrderEntry("OrderFromCenter", "Starts at the center item and moves toward both edges.", "FromCenter", PresetBrowserCollectionKind.OrderFromCenter, "Center outward");
            yield return CreateOrderEntry("OrderToCenter", "Starts at both edges and moves toward the center.", "ToCenter", PresetBrowserCollectionKind.OrderToCenter, "Edges inward");
            yield return PresetBrowserEntry.Collection(
                "OrderRandom",
                "Uses a deterministic shuffled order with preview seed 1729.",
                "0.36s per item",
                "items.TweenStagger(owner).Preset<PulseScalePreset>(0.36f).Order(StaggerOrder.Random).DelayBetween(0.14f).Seed(1729).Play();",
                PresetBrowserEntryKind.StaggerVariant,
                PresetBrowserPreviewKind.List,
                PresetBrowserCollectionKind.OrderRandom,
                "Seeded random");

            yield return CreateWaveEntry("GridWaveRightToLeft", "Reveals grid columns from right to left.", "RightToLeft", PresetBrowserCollectionKind.GridWaveRightToLeft, "Right to left");
            yield return CreateWaveEntry("GridWaveTopToBottom", "Reveals grid rows from top to bottom.", "TopToBottom", PresetBrowserCollectionKind.GridWaveTopToBottom, "Top to bottom");
            yield return CreateWaveEntry("GridWaveBottomToTop", "Reveals grid rows from bottom to top.", "BottomToTop", PresetBrowserCollectionKind.GridWaveBottomToTop, "Bottom to top");

            yield return PresetBrowserEntry.Collection(
                "GridDiagonalWave",
                "Reveals diagonals between any selected pair of opposite corners.",
                "0.32s per item",
                "items.GridDiagonalWave(owner, columns: 3, direction: GridDiagonalDirection.TopLeftToBottomRight);",
                PresetBrowserEntryKind.CollectionRecipe,
                PresetBrowserPreviewKind.Grid,
                PresetBrowserCollectionKind.GridDiagonalWave,
                "Top-left diagonal");

            yield return PresetBrowserEntry.Collection(
                "GridSpiral",
                "Reveals a clockwise or counter-clockwise spiral, outside-in or inside-out.",
                "0.3s per item",
                "items.GridSpiral(owner, columns: 3, direction: GridSpiralDirection.OutsideInClockwise);",
                PresetBrowserEntryKind.CollectionRecipe,
                PresetBrowserPreviewKind.Grid,
                PresetBrowserCollectionKind.GridSpiral,
                "Outside in");

            yield return PresetBrowserEntry.Collection(
                "GridCheckerboard",
                "Pulses alternating checkerboard cells in two phases; inverted swaps the first phase.",
                "0.34s per item",
                "items.GridCheckerboard(owner, columns: 3, inverted: false);",
                PresetBrowserEntryKind.CollectionRecipe,
                PresetBrowserPreviewKind.Grid,
                PresetBrowserCollectionKind.GridCheckerboard,
                "Alternating");

            yield return PresetBrowserEntry.Collection(
                "CollectionBurstIn",
                "Launches every item from one origin into its authored position.",
                "0.48s per item",
                "items.CollectionBurstIn(owner, origin);",
                PresetBrowserEntryKind.CollectionRecipe,
                PresetBrowserPreviewKind.Grid,
                PresetBrowserCollectionKind.CollectionBurstIn,
                "Center outward");

            yield return PresetBrowserEntry.Collection(
                "CollectionBurstOut",
                "Scatters items radially while shrinking and fading; omitted distance uses UI or world defaults.",
                "0.42s per item",
                "items.CollectionBurstOut(owner, origin);",
                PresetBrowserEntryKind.CollectionRecipe,
                PresetBrowserPreviewKind.Grid,
                PresetBrowserCollectionKind.CollectionBurstOut,
                "Center outward");

            yield return PresetBrowserEntry.Collection(
                "CollectionGatherTo",
                "Gathers every item into one destination while shrinking and fading.",
                "0.52s per item",
                "items.CollectionGatherTo(owner, destination);",
                PresetBrowserEntryKind.CollectionRecipe,
                PresetBrowserPreviewKind.Grid,
                PresetBrowserCollectionKind.CollectionGatherTo,
                "Edges inward");
        }

        private static PresetBrowserEntry CreateOrderEntry(string name, string description, string order, PresetBrowserCollectionKind collectionKind, string direction)
        {
            string example = $"items.TweenStagger(owner).Preset<PulseScalePreset>(0.36f).Order(StaggerOrder.{order}).DelayBetween(0.14f).Play();";
            return PresetBrowserEntry.Collection(name, description, "0.36s per item", example, PresetBrowserEntryKind.StaggerVariant, PresetBrowserPreviewKind.List, collectionKind, direction);
        }

        private static PresetBrowserEntry CreateWaveEntry(string name, string description, string directionName, PresetBrowserCollectionKind collectionKind, string direction)
        {
            string example = $"items.GridWave(owner, columns: 3, direction: GridWaveDirection.{directionName});";
            return PresetBrowserEntry.Collection(name, description, "0.32s per item", example, PresetBrowserEntryKind.StaggerVariant, PresetBrowserPreviewKind.Grid, collectionKind, direction);
        }
    }
}
