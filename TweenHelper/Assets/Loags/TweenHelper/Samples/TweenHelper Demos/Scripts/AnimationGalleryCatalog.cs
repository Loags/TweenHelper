using System;
using System.Collections.Generic;
using System.Linq;

namespace LB.TweenHelper.Demo
{
    public enum AnimationGalleryCategory
    {
        Presets,
        UIRecipes,
        Collections,
        DestinationMotion,
        GameplayFeedback,
        UISequences,
        TextAndValues,
        CameraFeedback
    }

    public enum AnimationGalleryApiKind
    {
        Preset,
        Recipe,
        BuilderOperation
    }

    public enum AnimationGalleryFixture
    {
        PresetAuto,
        UiTarget,
        List,
        Grid,
        LoadingDots,
        Destination,
        Feedback,
        UISequence,
        TextValue,
        WorldTextValue,
        Camera
    }

    public enum AnimationGalleryOptionKind
    {
        Direction,
        Order,
        GridDirection,
        DiagonalPattern,
        SpiralPattern,
        Phase,
        Interpolation,
        MotionVariant,
        TargetContext,
        ImpactDirection,
        Backdrop
    }

    public enum AnimationGalleryOperation
    {
        Preset,
        UIAppear,
        UIAppearSoft,
        UIDisappear,
        UIDisappearSoft,
        UIHover,
        UIHoverSoft,
        UIPress,
        UIPressHard,
        UIAttention,
        UIAttentionSoft,
        UIAttentionHard,
        UIDisabled,
        UIEnabled,
        ListStaggerIn,
        ListStaggerOut,
        GridWave,
        GridRipple,
        LoadingDots,
        GridDiagonalWave,
        GridSpiral,
        GridCheckerboard,
        CollectionBurstIn,
        CollectionBurstOut,
        CollectionGatherTo,
        ArcTo,
        BezierTo,
        HopTo,
        SpringTo,
        MagneticSnapTo,
        PathThrough,
        SpiralTo,
        MultiHopTo,
        ErrorReject,
        DamageHit,
        SuccessConfirm,
        RewardReveal,
        PickupCollect,
        HealReceive,
        ShieldBlock,
        CriticalHit,
        CooldownReady,
        LevelUp,
        LowHealthWarning,
        ToastShow,
        ToastHide,
        ModalOpen,
        ModalClose,
        TooltipShow,
        TooltipHide,
        DropdownOpen,
        DropdownClose,
        TabSwitch,
        DrawerShow,
        DrawerHide,
        BottomSheetShow,
        BottomSheetHide,
        PagePush,
        PageCrossFade,
        TypewriterReveal,
        TypewriterHide,
        NumberCountUp,
        NumberCountDown,
        TextCharacterStaggerIn,
        TextWave,
        ScoreIncrease,
        TextCharacterStaggerOut,
        TextCharacterBounce,
        TextColorSweep,
        TextGlitch,
        TextEmphasis,
        TextScrambleReveal,
        CameraImpact,
        CameraRecoil,
        CameraLandingImpact,
        CameraFovKick,
        CameraFocusZoom,
        CameraBreathing
    }

    public sealed class AnimationGalleryOptionDescriptor
    {
        public AnimationGalleryOptionDescriptor(AnimationGalleryOptionKind kind, string label, int defaultIndex, params string[] values)
        {
            Kind = kind;
            Label = label;
            DefaultIndex = defaultIndex;
            Values = values;
        }

        public AnimationGalleryOptionKind Kind { get; }
        public string Label { get; }
        public int DefaultIndex { get; }
        public IReadOnlyList<string> Values { get; }
    }

    public sealed class AnimationGalleryEntry
    {
        public AnimationGalleryEntry(string id, string name, AnimationGalleryCategory category, string description,
            AnimationGalleryOperation operation, AnimationGalleryFixture fixture, AnimationGalleryApiKind apiKind,
            string targetBadge, ITweenPreset preset = null, params AnimationGalleryOptionDescriptor[] options)
        {
            Id = id;
            Name = name;
            Category = category;
            Description = description;
            Operation = operation;
            Fixture = fixture;
            ApiKind = apiKind;
            TargetBadge = targetBadge;
            Preset = preset;
            Options = options ?? Array.Empty<AnimationGalleryOptionDescriptor>();
        }

        public string Id { get; }
        public string Name { get; }
        public AnimationGalleryCategory Category { get; }
        public string Description { get; }
        public AnimationGalleryOperation Operation { get; }
        public AnimationGalleryFixture Fixture { get; }
        public AnimationGalleryApiKind ApiKind { get; }
        public string TargetBadge { get; }
        public ITweenPreset Preset { get; }
        public IReadOnlyList<AnimationGalleryOptionDescriptor> Options { get; }
        public string Family => Preset == null ? string.Empty : PresetFamilyClassifier.GetFamilyName(Preset.PresetName);
    }

    public sealed class AnimationGalleryConfiguration
    {
        private readonly int[] _optionIndices;

        public AnimationGalleryConfiguration(AnimationGalleryEntry entry, int[] optionIndices = null)
        {
            Entry = entry;
            _optionIndices = optionIndices == null ? entry.Options.Select(option => option.DefaultIndex).ToArray() : (int[])optionIndices.Clone();
        }

        public AnimationGalleryEntry Entry { get; }
        public IReadOnlyList<int> OptionIndices => _optionIndices;

        public int GetIndex(AnimationGalleryOptionKind kind)
        {
            for (int i = 0; i < Entry.Options.Count; i++)
            {
                if (Entry.Options[i].Kind == kind) return _optionIndices[i];
            }

            return -1;
        }

        public string GetValue(AnimationGalleryOptionKind kind)
        {
            int optionIndex = GetOptionIndex(kind);
            if (optionIndex < 0) return string.Empty;
            int valueIndex = _optionIndices[optionIndex];
            return Entry.Options[optionIndex].Values[valueIndex];
        }

        public AnimationGalleryConfiguration WithOption(int optionIndex, int valueIndex)
        {
            int[] values = (int[])_optionIndices.Clone();
            values[optionIndex] = valueIndex;
            return new AnimationGalleryConfiguration(Entry, values);
        }

        private int GetOptionIndex(AnimationGalleryOptionKind kind)
        {
            for (int i = 0; i < Entry.Options.Count; i++)
            {
                if (Entry.Options[i].Kind == kind) return i;
            }

            return -1;
        }
    }

    public static class AnimationGalleryCatalog
    {
        private static readonly AnimationGalleryOptionDescriptor Direction = new AnimationGalleryOptionDescriptor(AnimationGalleryOptionKind.Direction, "Direction", 2, "Up", "Down", "Left", "Right");
        private static readonly AnimationGalleryOptionDescriptor Order = new AnimationGalleryOptionDescriptor(AnimationGalleryOptionKind.Order, "Order", 0, "First to last", "Last to first", "From center", "To center", "Random (seeded)");
        private static readonly AnimationGalleryOptionDescriptor GridDirection = new AnimationGalleryOptionDescriptor(AnimationGalleryOptionKind.GridDirection, "Direction", 0, "Left to right", "Right to left", "Top to bottom", "Bottom to top");
        private static readonly AnimationGalleryOptionDescriptor DiagonalPattern = new AnimationGalleryOptionDescriptor(AnimationGalleryOptionKind.DiagonalPattern, "Pattern", 0, "Top-left to bottom-right", "Top-right to bottom-left", "Bottom-left to top-right", "Bottom-right to top-left");
        private static readonly AnimationGalleryOptionDescriptor SpiralPattern = new AnimationGalleryOptionDescriptor(AnimationGalleryOptionKind.SpiralPattern, "Pattern", 0, "Outside-in clockwise", "Outside-in counter-clockwise", "Inside-out clockwise", "Inside-out counter-clockwise");
        private static readonly AnimationGalleryOptionDescriptor Phase = new AnimationGalleryOptionDescriptor(AnimationGalleryOptionKind.Phase, "Phase", 0, "Normal", "Inverted");
        private static readonly AnimationGalleryOptionDescriptor Interpolation = new AnimationGalleryOptionDescriptor(AnimationGalleryOptionKind.Interpolation, "Interpolation", 1, "Linear", "Catmull-Rom");
        private static readonly AnimationGalleryOptionDescriptor MotionVariant = new AnimationGalleryOptionDescriptor(AnimationGalleryOptionKind.MotionVariant, "Motion variant", 0, "Positive / outward", "Negative / inward");
        private static readonly AnimationGalleryOptionDescriptor TargetContext = new AnimationGalleryOptionDescriptor(AnimationGalleryOptionKind.TargetContext, "Target", 0, "UI / local", "World");
        private static readonly AnimationGalleryOptionDescriptor ImpactDirection = new AnimationGalleryOptionDescriptor(AnimationGalleryOptionKind.ImpactDirection, "Impact direction", 0, "Right", "Left");
        private static readonly AnimationGalleryOptionDescriptor Backdrop = new AnimationGalleryOptionDescriptor(AnimationGalleryOptionKind.Backdrop, "Backdrop", 0, "Enabled", "Disabled");

        public static IReadOnlyList<AnimationGalleryEntry> Build()
        {
            TweenPresetRegistry.ScanForCodePresets();
            var entries = new List<AnimationGalleryEntry>(380);
            entries.AddRange(TweenPresetRegistry.Presets
                .OrderBy(preset => preset.PresetName, StringComparer.Ordinal)
                .Select(preset => new AnimationGalleryEntry($"preset:{preset.PresetName}", preset.PresetName, AnimationGalleryCategory.Presets,
                    preset.Description, AnimationGalleryOperation.Preset, AnimationGalleryFixture.PresetAuto, AnimationGalleryApiKind.Preset,
                    "Compatible target", preset)));

            AddUIRecipes(entries);
            AddCollections(entries);
            AddDestinationMotion(entries);
            AddGameplayFeedback(entries);
            AddUISequences(entries);
            AddTextAndValues(entries);
            AddCameraFeedback(entries);
            return entries;
        }

        public static string GetSnippet(AnimationGalleryConfiguration configuration)
        {
            AnimationGalleryEntry entry = configuration.Entry;
            if (entry.Operation == AnimationGalleryOperation.Preset)
            {
                return $"target.Tween().Preset<{entry.Preset.GetType().Name}>().Play();";
            }

            string direction = configuration.GetValue(AnimationGalleryOptionKind.Direction).Replace(" ", string.Empty);
            string targetContext = configuration.GetValue(AnimationGalleryOptionKind.TargetContext);
            bool world = targetContext == "World";
            string localSuffix = world ? string.Empty : "Local";
            switch (entry.Operation)
            {
                case AnimationGalleryOperation.ListStaggerIn:
                    return $"items.TweenStagger(owner).Preset<PopInFadePreset>().Order(StaggerOrder.{GetOrder(configuration)}).DelayBetween(0.08f).Play();";
                case AnimationGalleryOperation.ListStaggerOut:
                    return $"items.TweenStagger(owner).Preset<PopOutFadePreset>().Order(StaggerOrder.{GetOrder(configuration)}).DelayBetween(0.06f).Play();";
                case AnimationGalleryOperation.GridWave:
                    return $"items.GridWave(owner, columns: 3, GridWaveDirection.{GetEnumValue(configuration, AnimationGalleryOptionKind.GridDirection)});";
                case AnimationGalleryOperation.GridRipple:
                    return "items.GridRipple(owner, columns: 3);";
                case AnimationGalleryOperation.LoadingDots:
                    return "dots.LoadingDots(owner);";
                case AnimationGalleryOperation.GridDiagonalWave:
                    return $"items.GridDiagonalWave(owner, columns: 3, GridDiagonalDirection.{GetEnumValue(configuration, AnimationGalleryOptionKind.DiagonalPattern)});";
                case AnimationGalleryOperation.GridSpiral:
                    return $"items.GridSpiral(owner, columns: 3, GridSpiralDirection.{GetEnumValue(configuration, AnimationGalleryOptionKind.SpiralPattern)});";
                case AnimationGalleryOperation.GridCheckerboard:
                    return configuration.GetIndex(AnimationGalleryOptionKind.Phase) == 0 ? "items.GridCheckerboard(owner, columns: 3);" : "items.GridCheckerboard(owner, columns: 3, inverted: true);";
                case AnimationGalleryOperation.CollectionBurstIn:
                    return "items.CollectionBurstIn(owner, origin);";
                case AnimationGalleryOperation.CollectionBurstOut:
                    return "items.CollectionBurstOut(owner, origin);";
                case AnimationGalleryOperation.CollectionGatherTo:
                    return "items.CollectionGatherTo(owner, destination);";
                case AnimationGalleryOperation.ArcTo:
                    return $"target.Tween().Arc{localSuffix}To(destination, {Signed(configuration, world ? "2.1f" : "175f")}).Play();";
                case AnimationGalleryOperation.BezierTo:
                    return $"target.Tween().Bezier{localSuffix}To(destination, controlA, controlB).Play();";
                case AnimationGalleryOperation.HopTo:
                    return $"target.Tween().Hop{localSuffix}To(destination, {Signed(configuration, world ? "2.1f" : "175f")}).Play();";
                case AnimationGalleryOperation.SpringTo:
                    return $"target.Tween().Spring{localSuffix}To(destination).Play();";
                case AnimationGalleryOperation.MagneticSnapTo:
                    return $"target.Tween().MagneticSnap{localSuffix}To(destination).Play();";
                case AnimationGalleryOperation.PathThrough:
                    return $"target.Tween().Path{localSuffix}Through(waypoints, DestinationPathInterpolation.{GetEnumValue(configuration, AnimationGalleryOptionKind.Interpolation)}).Play();";
                case AnimationGalleryOperation.SpiralTo:
                    return $"target.Tween().Spiral{localSuffix}To(destination, radius: {Signed(configuration, world ? "1.1f" : "92f")}).Play();";
                case AnimationGalleryOperation.MultiHopTo:
                    return $"target.Tween().MultiHop{localSuffix}To(destination, hopHeight: {Signed(configuration, world ? "2.1f" : "175f")}, hopCount: 3).Play();";
                case AnimationGalleryOperation.ShieldBlock:
                    return $"target.ShieldBlock(Vector3.{(configuration.GetIndex(AnimationGalleryOptionKind.ImpactDirection) == 0 ? "right" : "left")});";
                case AnimationGalleryOperation.CriticalHit:
                    return $"target.CriticalHit(Vector3.{(configuration.GetIndex(AnimationGalleryOptionKind.ImpactDirection) == 0 ? "right" : "left")});";
                case AnimationGalleryOperation.PickupCollect:
                    return world ? "target.PickupCollectTo(destination);" : "target.PickupCollectLocalTo(destination);";
                case AnimationGalleryOperation.ToastShow:
                case AnimationGalleryOperation.ToastHide:
                case AnimationGalleryOperation.TooltipShow:
                case AnimationGalleryOperation.TooltipHide:
                    return $"target.{entry.Operation}(UISequenceDirection.{direction});";
                case AnimationGalleryOperation.TabSwitch:
                case AnimationGalleryOperation.PagePush:
                    return $"outgoing.{(entry.Operation == AnimationGalleryOperation.TabSwitch ? "TabSwitchTo" : "PagePushTo")}(incoming, UISequenceDirection.{direction});";
                case AnimationGalleryOperation.DrawerShow:
                case AnimationGalleryOperation.DrawerHide:
                    return $"panel.{entry.Operation}(UISequenceDirection.{direction}, {(configuration.GetIndex(AnimationGalleryOptionKind.Backdrop) == 0 ? "backdrop" : "null")});";
                case AnimationGalleryOperation.TextCharacterStaggerIn:
                case AnimationGalleryOperation.TextCharacterStaggerOut:
                case AnimationGalleryOperation.TextCharacterBounce:
                case AnimationGalleryOperation.TextWave:
                case AnimationGalleryOperation.TextEmphasis:
                    return $"text.{entry.Operation}(UISequenceDirection.{direction});";
                case AnimationGalleryOperation.CameraFovKick:
                    return configuration.GetIndex(AnimationGalleryOptionKind.MotionVariant) == 0 ? "camera.CameraFovKick(11f);" : "camera.CameraFovKick(-11f);";
                default:
                    return GetSimpleSnippet(entry.Operation);
            }
        }

        private static string GetSimpleSnippet(AnimationGalleryOperation operation)
        {
            switch (operation)
            {
                case AnimationGalleryOperation.UIAppear:
                case AnimationGalleryOperation.UIAppearSoft:
                case AnimationGalleryOperation.UIDisappear:
                case AnimationGalleryOperation.UIDisappearSoft:
                case AnimationGalleryOperation.UIHover:
                case AnimationGalleryOperation.UIHoverSoft:
                case AnimationGalleryOperation.UIPress:
                case AnimationGalleryOperation.UIPressHard:
                case AnimationGalleryOperation.UIAttention:
                case AnimationGalleryOperation.UIAttentionSoft:
                case AnimationGalleryOperation.UIAttentionHard:
                case AnimationGalleryOperation.UIDisabled:
                case AnimationGalleryOperation.UIEnabled:
                    return $"target.{operation}();";
                case AnimationGalleryOperation.ErrorReject:
                case AnimationGalleryOperation.DamageHit:
                case AnimationGalleryOperation.SuccessConfirm:
                case AnimationGalleryOperation.RewardReveal:
                case AnimationGalleryOperation.HealReceive:
                case AnimationGalleryOperation.CooldownReady:
                case AnimationGalleryOperation.LevelUp:
                case AnimationGalleryOperation.LowHealthWarning:
                    return $"target.{operation}();";
                case AnimationGalleryOperation.ModalOpen:
                    return "panel.ModalOpen(backdrop, controls);";
                case AnimationGalleryOperation.ModalClose:
                    return "panel.ModalClose(backdrop, controls);";
                case AnimationGalleryOperation.DropdownOpen:
                    return "panel.DropdownOpen(entries);";
                case AnimationGalleryOperation.DropdownClose:
                    return "panel.DropdownClose(entries);";
                case AnimationGalleryOperation.BottomSheetShow:
                    return "panel.BottomSheetShow(backdrop);";
                case AnimationGalleryOperation.BottomSheetHide:
                    return "panel.BottomSheetHide(backdrop);";
                case AnimationGalleryOperation.PageCrossFade:
                    return "outgoing.PageCrossFadeTo(incoming);";
                case AnimationGalleryOperation.TypewriterReveal:
                case AnimationGalleryOperation.TypewriterHide:
                case AnimationGalleryOperation.TextColorSweep:
                case AnimationGalleryOperation.TextGlitch:
                case AnimationGalleryOperation.TextScrambleReveal:
                    return $"text.{operation}();";
                case AnimationGalleryOperation.NumberCountUp:
                    return "text.NumberCountTo(0d, 1250d, \"N0\");";
                case AnimationGalleryOperation.NumberCountDown:
                    return "text.NumberCountTo(1250d, 0d, \"N0\");";
                case AnimationGalleryOperation.ScoreIncrease:
                    return "text.ScoreIncrease(1200d, 1475d, \"N0\");";
                case AnimationGalleryOperation.CameraImpact:
                case AnimationGalleryOperation.CameraRecoil:
                case AnimationGalleryOperation.CameraLandingImpact:
                case AnimationGalleryOperation.CameraFocusZoom:
                case AnimationGalleryOperation.CameraBreathing:
                    return $"camera.{operation}();";
                default:
                    return string.Empty;
            }
        }

        private static void AddUIRecipes(ICollection<AnimationGalleryEntry> entries)
        {
            AddOperations(entries, AnimationGalleryCategory.UIRecipes, AnimationGalleryFixture.UiTarget, AnimationGalleryApiKind.Recipe, "UI",
                (AnimationGalleryOperation.UIAppear, "Pop and fade a UI element into view."),
                (AnimationGalleryOperation.UIAppearSoft, "Reveal a UI element with gentler motion."),
                (AnimationGalleryOperation.UIDisappear, "Pop and fade a UI element out."),
                (AnimationGalleryOperation.UIDisappearSoft, "Hide a UI element with gentler motion."),
                (AnimationGalleryOperation.UIHover, "Scale and tint for hover feedback."),
                (AnimationGalleryOperation.UIHoverSoft, "Apply subtle hover feedback."),
                (AnimationGalleryOperation.UIPress, "Play press and release feedback."),
                (AnimationGalleryOperation.UIPressHard, "Play stronger press feedback."),
                (AnimationGalleryOperation.UIAttention, "Draw attention to a UI element."),
                (AnimationGalleryOperation.UIAttentionSoft, "Use subtle attention feedback."),
                (AnimationGalleryOperation.UIAttentionHard, "Use emphatic attention feedback."),
                (AnimationGalleryOperation.UIDisabled, "Transition to a disabled visual state."),
                (AnimationGalleryOperation.UIEnabled, "Restore an enabled visual state."));
        }

        private static void AddCollections(ICollection<AnimationGalleryEntry> entries)
        {
            Add(entries, AnimationGalleryCategory.Collections, AnimationGalleryOperation.ListStaggerIn, AnimationGalleryFixture.List, "Reveal a list with configurable stagger order.", "Collection", Order);
            Add(entries, AnimationGalleryCategory.Collections, AnimationGalleryOperation.ListStaggerOut, AnimationGalleryFixture.List, "Hide a list with configurable stagger order.", "Collection", Order);
            Add(entries, AnimationGalleryCategory.Collections, AnimationGalleryOperation.GridWave, AnimationGalleryFixture.Grid, "Reveal grid rows or columns as a directional wave.", "Collection", GridDirection);
            Add(entries, AnimationGalleryCategory.Collections, AnimationGalleryOperation.GridRipple, AnimationGalleryFixture.Grid, "Pulse outward from the grid center.", "Collection");
            Add(entries, AnimationGalleryCategory.Collections, AnimationGalleryOperation.LoadingDots, AnimationGalleryFixture.LoadingDots, "Play one finite loading-dot preview cycle.", "Collection");
            Add(entries, AnimationGalleryCategory.Collections, AnimationGalleryOperation.GridDiagonalWave, AnimationGalleryFixture.Grid, "Reveal a grid along a chosen diagonal.", "Collection", DiagonalPattern);
            Add(entries, AnimationGalleryCategory.Collections, AnimationGalleryOperation.GridSpiral, AnimationGalleryFixture.Grid, "Reveal a grid using a configurable spiral.", "Collection", SpiralPattern);
            Add(entries, AnimationGalleryCategory.Collections, AnimationGalleryOperation.GridCheckerboard, AnimationGalleryFixture.Grid, "Animate alternating checkerboard cells.", "Collection", Phase);
            Add(entries, AnimationGalleryCategory.Collections, AnimationGalleryOperation.CollectionBurstIn, AnimationGalleryFixture.Grid, "Move every item from a shared origin into place.", "Collection");
            Add(entries, AnimationGalleryCategory.Collections, AnimationGalleryOperation.CollectionBurstOut, AnimationGalleryFixture.Grid, "Scatter every item away from a shared origin.", "Collection");
            Add(entries, AnimationGalleryCategory.Collections, AnimationGalleryOperation.CollectionGatherTo, AnimationGalleryFixture.Grid, "Gather every item into one destination.", "Collection");
        }

        private static void AddDestinationMotion(ICollection<AnimationGalleryEntry> entries)
        {
            Add(entries, AnimationGalleryCategory.DestinationMotion, AnimationGalleryOperation.ArcTo, AnimationGalleryFixture.Destination, "Move along a signed arc and land exactly at the destination.", "Transform / UI", TargetContext, MotionVariant);
            Add(entries, AnimationGalleryCategory.DestinationMotion, AnimationGalleryOperation.BezierTo, AnimationGalleryFixture.Destination, "Follow a cubic Bezier path through two controls.", "Transform / UI", TargetContext);
            Add(entries, AnimationGalleryCategory.DestinationMotion, AnimationGalleryOperation.HopTo, AnimationGalleryFixture.Destination, "Hop to a destination with anticipation and landing squash.", "Transform / UI", TargetContext, MotionVariant);
            Add(entries, AnimationGalleryCategory.DestinationMotion, AnimationGalleryOperation.SpringTo, AnimationGalleryFixture.Destination, "Overshoot the destination and settle exactly.", "Transform / UI", TargetContext);
            Add(entries, AnimationGalleryCategory.DestinationMotion, AnimationGalleryOperation.MagneticSnapTo, AnimationGalleryFixture.Destination, "Pull away, accelerate past, and snap into place.", "Transform / UI", TargetContext);
            Add(entries, AnimationGalleryCategory.DestinationMotion, AnimationGalleryOperation.PathThrough, AnimationGalleryFixture.Destination, "Travel through authored waypoints with chosen interpolation.", "Transform / UI", TargetContext, Interpolation);
            Add(entries, AnimationGalleryCategory.DestinationMotion, AnimationGalleryOperation.SpiralTo, AnimationGalleryFixture.Destination, "Close a clockwise or counter-clockwise spiral at the destination.", "Transform / UI", TargetContext, MotionVariant);
            Add(entries, AnimationGalleryCategory.DestinationMotion, AnimationGalleryOperation.MultiHopTo, AnimationGalleryFixture.Destination, "Land after three diminishing hops.", "Transform / UI", TargetContext, MotionVariant);
        }

        private static void AddGameplayFeedback(ICollection<AnimationGalleryEntry> entries)
        {
            Add(entries, AnimationGalleryCategory.GameplayFeedback, AnimationGalleryOperation.ErrorReject, AnimationGalleryFixture.Feedback, "Reject an action with shake, tilt, and color feedback.", "Transform / UI", TargetContext);
            Add(entries, AnimationGalleryCategory.GameplayFeedback, AnimationGalleryOperation.DamageHit, AnimationGalleryFixture.Feedback, "Communicate damage with shake, squash, recoil, and flash.", "Transform / UI", TargetContext);
            Add(entries, AnimationGalleryCategory.GameplayFeedback, AnimationGalleryOperation.SuccessConfirm, AnimationGalleryFixture.Feedback, "Confirm success with a readable positive beat.", "Transform / UI", TargetContext);
            Add(entries, AnimationGalleryCategory.GameplayFeedback, AnimationGalleryOperation.RewardReveal, AnimationGalleryFixture.Feedback, "Reveal a reward with anticipation and settle.", "Transform / UI", TargetContext);
            Add(entries, AnimationGalleryCategory.GameplayFeedback, AnimationGalleryOperation.PickupCollect, AnimationGalleryFixture.Feedback, "Collect an item into a destination.", "Transform / UI", TargetContext);
            Add(entries, AnimationGalleryCategory.GameplayFeedback, AnimationGalleryOperation.HealReceive, AnimationGalleryFixture.Feedback, "Show a restorative heal response.", "Transform / UI", TargetContext);
            Add(entries, AnimationGalleryCategory.GameplayFeedback, AnimationGalleryOperation.ShieldBlock, AnimationGalleryFixture.Feedback, "Block an impact from a selected direction.", "Transform / UI", TargetContext, ImpactDirection);
            Add(entries, AnimationGalleryCategory.GameplayFeedback, AnimationGalleryOperation.CriticalHit, AnimationGalleryFixture.Feedback, "Emphasize a high-damage directional impact.", "Transform / UI", TargetContext, ImpactDirection);
            Add(entries, AnimationGalleryCategory.GameplayFeedback, AnimationGalleryOperation.CooldownReady, AnimationGalleryFixture.Feedback, "Signal that an action is ready again.", "Transform / UI", TargetContext);
            Add(entries, AnimationGalleryCategory.GameplayFeedback, AnimationGalleryOperation.LevelUp, AnimationGalleryFixture.Feedback, "Celebrate progression with a layered reveal.", "Transform / UI", TargetContext);
            Add(entries, AnimationGalleryCategory.GameplayFeedback, AnimationGalleryOperation.LowHealthWarning, AnimationGalleryFixture.Feedback, "Play one finite low-health warning cycle.", "Transform / UI", TargetContext);
        }

        private static void AddUISequences(ICollection<AnimationGalleryEntry> entries)
        {
            Add(entries, AnimationGalleryCategory.UISequences, AnimationGalleryOperation.ToastShow, AnimationGalleryFixture.UISequence, "Show a toast from a chosen direction.", "UI", Direction);
            Add(entries, AnimationGalleryCategory.UISequences, AnimationGalleryOperation.ToastHide, AnimationGalleryFixture.UISequence, "Hide a toast toward a chosen direction.", "UI", Direction);
            Add(entries, AnimationGalleryCategory.UISequences, AnimationGalleryOperation.ModalOpen, AnimationGalleryFixture.UISequence, "Open a modal, backdrop, and controls as one sequence.", "UI");
            Add(entries, AnimationGalleryCategory.UISequences, AnimationGalleryOperation.ModalClose, AnimationGalleryFixture.UISequence, "Close modal controls, panel, and backdrop cleanly.", "UI");
            Add(entries, AnimationGalleryCategory.UISequences, AnimationGalleryOperation.TooltipShow, AnimationGalleryFixture.UISequence, "Reveal a tooltip from a chosen direction.", "UI", Direction);
            Add(entries, AnimationGalleryCategory.UISequences, AnimationGalleryOperation.TooltipHide, AnimationGalleryFixture.UISequence, "Hide a tooltip toward a chosen direction.", "UI", Direction);
            Add(entries, AnimationGalleryCategory.UISequences, AnimationGalleryOperation.DropdownOpen, AnimationGalleryFixture.UISequence, "Open a dropdown and stagger its entries.", "UI");
            Add(entries, AnimationGalleryCategory.UISequences, AnimationGalleryOperation.DropdownClose, AnimationGalleryFixture.UISequence, "Close dropdown entries and panel in reverse order.", "UI");
            Add(entries, AnimationGalleryCategory.UISequences, AnimationGalleryOperation.TabSwitch, AnimationGalleryFixture.UISequence, "Switch between outgoing and incoming tabs.", "UI", Direction);
            Add(entries, AnimationGalleryCategory.UISequences, AnimationGalleryOperation.DrawerShow, AnimationGalleryFixture.UISequence, "Show a directional drawer with optional backdrop.", "UI", Direction, Backdrop);
            Add(entries, AnimationGalleryCategory.UISequences, AnimationGalleryOperation.DrawerHide, AnimationGalleryFixture.UISequence, "Hide a directional drawer with optional backdrop.", "UI", Direction, Backdrop);
            Add(entries, AnimationGalleryCategory.UISequences, AnimationGalleryOperation.BottomSheetShow, AnimationGalleryFixture.UISequence, "Show a bottom sheet and backdrop.", "UI");
            Add(entries, AnimationGalleryCategory.UISequences, AnimationGalleryOperation.BottomSheetHide, AnimationGalleryFixture.UISequence, "Hide a bottom sheet and backdrop.", "UI");
            Add(entries, AnimationGalleryCategory.UISequences, AnimationGalleryOperation.PagePush, AnimationGalleryFixture.UISequence, "Push from one page to another.", "UI", Direction);
            Add(entries, AnimationGalleryCategory.UISequences, AnimationGalleryOperation.PageCrossFade, AnimationGalleryFixture.UISequence, "Cross-fade between pages.", "UI");
        }

        private static void AddTextAndValues(ICollection<AnimationGalleryEntry> entries)
        {
            Add(entries, AnimationGalleryCategory.TextAndValues, AnimationGalleryOperation.TypewriterReveal, AnimationGalleryFixture.TextValue, "Reveal text one visible character at a time.", "TMP");
            Add(entries, AnimationGalleryCategory.TextAndValues, AnimationGalleryOperation.TypewriterHide, AnimationGalleryFixture.TextValue, "Hide text one visible character at a time.", "TMP");
            Add(entries, AnimationGalleryCategory.TextAndValues, AnimationGalleryOperation.NumberCountUp, AnimationGalleryFixture.TextValue, "Count a formatted number upward.", "TMP");
            Add(entries, AnimationGalleryCategory.TextAndValues, AnimationGalleryOperation.NumberCountDown, AnimationGalleryFixture.TextValue, "Count a formatted number downward.", "TMP");
            Add(entries, AnimationGalleryCategory.TextAndValues, AnimationGalleryOperation.TextCharacterStaggerIn, AnimationGalleryFixture.TextValue, "Stagger visible characters into place.", "TMP", Direction, TargetContext);
            Add(entries, AnimationGalleryCategory.TextAndValues, AnimationGalleryOperation.TextWave, AnimationGalleryFixture.TextValue, "Move visible characters in a directional wave.", "TMP", Direction, TargetContext);
            Add(entries, AnimationGalleryCategory.TextAndValues, AnimationGalleryOperation.ScoreIncrease, AnimationGalleryFixture.TextValue, "Count a score while emphasizing the gain.", "TMP");
            Add(entries, AnimationGalleryCategory.TextAndValues, AnimationGalleryOperation.TextCharacterStaggerOut, AnimationGalleryFixture.TextValue, "Stagger visible characters out of view.", "TMP", Direction, TargetContext);
            Add(entries, AnimationGalleryCategory.TextAndValues, AnimationGalleryOperation.TextCharacterBounce, AnimationGalleryFixture.TextValue, "Bounce visible characters along a direction.", "TMP", Direction, TargetContext);
            Add(entries, AnimationGalleryCategory.TextAndValues, AnimationGalleryOperation.TextColorSweep, AnimationGalleryFixture.TextValue, "Sweep a highlight color across visible characters.", "TMP", TargetContext);
            Add(entries, AnimationGalleryCategory.TextAndValues, AnimationGalleryOperation.TextGlitch, AnimationGalleryFixture.TextValue, "Apply deterministic seeded character jitter.", "TMP", TargetContext);
            Add(entries, AnimationGalleryCategory.TextAndValues, AnimationGalleryOperation.TextEmphasis, AnimationGalleryFixture.TextValue, "Emphasize a selected character range.", "TMP", Direction, TargetContext);
            Add(entries, AnimationGalleryCategory.TextAndValues, AnimationGalleryOperation.TextScrambleReveal, AnimationGalleryFixture.TextValue, "Reveal text through a deterministic scramble.", "TMP", TargetContext);
        }

        private static void AddCameraFeedback(ICollection<AnimationGalleryEntry> entries)
        {
            Add(entries, AnimationGalleryCategory.CameraFeedback, AnimationGalleryOperation.CameraImpact, AnimationGalleryFixture.Camera, "Add a short position and rotation impact.", "Camera");
            Add(entries, AnimationGalleryCategory.CameraFeedback, AnimationGalleryOperation.CameraRecoil, AnimationGalleryFixture.Camera, "Recoil and recover a dedicated preview camera.", "Camera");
            Add(entries, AnimationGalleryCategory.CameraFeedback, AnimationGalleryOperation.CameraLandingImpact, AnimationGalleryFixture.Camera, "Communicate a heavy landing through camera motion.", "Camera");
            Add(entries, AnimationGalleryCategory.CameraFeedback, AnimationGalleryOperation.CameraFovKick, AnimationGalleryFixture.Camera, "Kick field of view outward or inward.", "Camera", MotionVariant);
            Add(entries, AnimationGalleryCategory.CameraFeedback, AnimationGalleryOperation.CameraFocusZoom, AnimationGalleryFixture.Camera, "Move and zoom toward an authored focus target.", "Camera");
            Add(entries, AnimationGalleryCategory.CameraFeedback, AnimationGalleryOperation.CameraBreathing, AnimationGalleryFixture.Camera, "Play one finite breathing preview cycle.", "Camera");
        }

        private static void AddOperations(ICollection<AnimationGalleryEntry> entries, AnimationGalleryCategory category,
            AnimationGalleryFixture fixture, AnimationGalleryApiKind apiKind, string targetBadge,
            params (AnimationGalleryOperation Operation, string Description)[] definitions)
        {
            foreach ((AnimationGalleryOperation operation, string description) in definitions)
            {
                entries.Add(new AnimationGalleryEntry($"{category}:{operation}", SplitName(operation.ToString()), category, description, operation, fixture, apiKind, targetBadge));
            }
        }

        private static void Add(ICollection<AnimationGalleryEntry> entries, AnimationGalleryCategory category,
            AnimationGalleryOperation operation, AnimationGalleryFixture fixture, string description, string targetBadge,
            params AnimationGalleryOptionDescriptor[] options)
        {
            entries.Add(new AnimationGalleryEntry($"{category}:{operation}", SplitName(operation.ToString()), category, description,
                operation, fixture, AnimationGalleryApiKind.BuilderOperation, targetBadge, null, options));
        }

        private static string GetOrder(AnimationGalleryConfiguration configuration)
        {
            switch (configuration.GetIndex(AnimationGalleryOptionKind.Order))
            {
                case 1: return "LastToFirst";
                case 2: return "FromCenter";
                case 3: return "ToCenter";
                case 4: return "Random";
                default: return "FirstToLast";
            }
        }

        private static string GetEnumValue(AnimationGalleryConfiguration configuration, AnimationGalleryOptionKind kind)
        {
            int index = configuration.GetIndex(kind);
            switch (kind)
            {
                case AnimationGalleryOptionKind.GridDirection:
                    return new[] { "LeftToRight", "RightToLeft", "TopToBottom", "BottomToTop" }[index];
                case AnimationGalleryOptionKind.DiagonalPattern:
                    return new[] { "TopLeftToBottomRight", "TopRightToBottomLeft", "BottomLeftToTopRight", "BottomRightToTopLeft" }[index];
                case AnimationGalleryOptionKind.SpiralPattern:
                    return new[] { "OutsideInClockwise", "OutsideInCounterClockwise", "InsideOutClockwise", "InsideOutCounterClockwise" }[index];
                case AnimationGalleryOptionKind.Interpolation:
                    return index == 0 ? "Linear" : "CatmullRom";
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        private static string Signed(AnimationGalleryConfiguration configuration, string magnitude)
            => configuration.GetIndex(AnimationGalleryOptionKind.MotionVariant) <= 0 ? magnitude : $"-{magnitude}";

        private static string SplitName(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var characters = new List<char>(value.Length + 8) { value[0] };
            for (int i = 1; i < value.Length; i++)
            {
                if (char.IsUpper(value[i]) && !char.IsUpper(value[i - 1])) characters.Add(' ');
                characters.Add(value[i]);
            }
            return new string(characters.ToArray()).Replace("UI ", "UI ").Replace("Fov", "FOV");
        }
    }
}
