using System;

namespace LB.TweenHelper.Editor
{
    internal enum PresetBrowserEntryKind
    {
        Preset,
        CollectionRecipe,
        StaggerVariant,
        BuilderOperation
    }

    internal enum PresetBrowserPreviewKind
    {
        Single,
        List,
        Grid,
        LoadingDots,
        UiTarget,
        Destination,
        WorldToUi,
        UiSequence,
        Text,
        ProgressImage,
        ProgressSlider,
        Camera,
        Audio,
        Light,
        Particles,
        Material
    }

    internal enum PresetBrowserCollectionKind
    {
        None,
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
        GridWaveBottomToTop,
        GridDiagonalWave,
        GridSpiral,
        GridCheckerboard,
        CollectionBurstIn,
        CollectionBurstOut,
        CollectionGatherTo,
        GridConcentricIn,
        GridConcentricOut,
        GridQuadrantSweep,
        ListAccordion,
        CollectionOrbitIn,
        CollectionOrbitOut,
        LoadingRing,
        LoadingRibbon
    }

    internal enum PresetBrowserOperation
    {
        None,
        UIHover,
        UIHoverSoft,
        UIPress,
        UIPressHard,
        UIAppear,
        UIAppearSoft,
        UIDisappear,
        UIDisappearSoft,
        UIAttention,
        UIAttentionSoft,
        UIAttentionHard,
        UIDisabled,
        UIEnabled,
        ArcTo,
        ArcLocalTo,
        BezierTo,
        BezierLocalTo,
        HopTo,
        HopLocalTo,
        SpringTo,
        SpringLocalTo,
        MagneticSnapTo,
        MagneticSnapLocalTo,
        PathThrough,
        PathLocalThrough,
        SpiralTo,
        SpiralLocalTo,
        MultiHopTo,
        MultiHopLocalTo,
        ArcToUI,
        HopToUI,
        BezierToUI,
        PathThroughUI,
        ErrorReject,
        DamageHit,
        SuccessConfirm,
        RewardReveal,
        HealReceive,
        ShieldBlock,
        CriticalHit,
        CooldownReady,
        LevelUp,
        LowHealthWarning,
        PickupCollectTo,
        PickupCollectLocalTo,
        PickupCollectToUI,
        AbilityCharging,
        AbilityReady,
        DodgeRoll,
        StunStart,
        StunEnd,
        BuffApplied,
        DebuffApplied,
        ResourceDepleted,
        ResourceRecovered,
        ObjectiveUnlocked,
        CriticalHitSequence,
        RewardRevealSequence,
        WarningLoopSequence,
        CutsceneUIEntranceSequence,
        ToastShow,
        ToastHide,
        ModalOpen,
        ModalClose,
        TooltipShow,
        TooltipHide,
        DropdownOpen,
        DropdownClose,
        TabSwitchTo,
        DrawerShow,
        DrawerHide,
        BottomSheetShow,
        BottomSheetHide,
        PagePushTo,
        PageCrossFadeTo,
        TypewriterReveal,
        TypewriterHide,
        NumberCountUp,
        NumberCountDown,
        TextCharacterStaggerIn,
        TextCharacterStaggerOut,
        TextWave,
        TextCharacterBounce,
        TextColorSweep,
        TextGlitch,
        TextEmphasis,
        TextScrambleReveal,
        ScoreIncrease,
        ImageFillTo,
        ImageFillFromTo,
        ImageValueFillTo,
        ImageFillDrain,
        ImageFillCharge,
        ImageFillAlertPulse,
        ImageFillAndText,
        SliderFillTo,
        SliderFillFromTo,
        SliderValueFillTo,
        SliderFillDrain,
        SliderFillCharge,
        SliderFillAlertPulse,
        SliderFillAndText,
        CameraImpact,
        CameraRecoil,
        CameraLandingImpact,
        CameraFovKick,
        CameraFocusZoom,
        CameraBreathing,
        CameraRackFocus,
        CollectLandingCameraKick,
        AudioVolumeTo,
        AudioPitchTo,
        LightIntensityTo,
        LightColorTo,
        ParticleEmissionRateTo,
        MaterialFloatTo,
        MaterialColorTo,
        TorchFlicker,
        ScannerPulse
    }

    internal sealed class PresetBrowserEntry
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Category { get; }
        public string Family { get; }
        public string Intensity { get; }
        public string Direction { get; }
        public string AxisOrPlane { get; }
        public string Duration { get; }
        public string Example { get; }
        public string Badge { get; }
        public PresetBrowserEntryKind Kind { get; }
        public PresetBrowserPreviewKind PreviewKind { get; }
        public PresetBrowserCollectionKind CollectionKind { get; }
        public PresetBrowserOperation Operation { get; }
        public ITweenPreset Preset { get; }

        public bool IsPreset => Kind == PresetBrowserEntryKind.Preset;
        public bool IsCollection => Kind == PresetBrowserEntryKind.CollectionRecipe || Kind == PresetBrowserEntryKind.StaggerVariant;

        private PresetBrowserEntry(string id, string name, string description, string category, string family, string intensity, string direction, string axisOrPlane, string duration, string example, string badge, PresetBrowserEntryKind kind, PresetBrowserPreviewKind previewKind, PresetBrowserCollectionKind collectionKind, PresetBrowserOperation operation, ITweenPreset preset)
        {
            Id = id;
            Name = name;
            Description = description;
            Category = category;
            Family = family;
            Intensity = intensity;
            Direction = direction;
            AxisOrPlane = axisOrPlane;
            Duration = duration;
            Example = example;
            Badge = badge;
            Kind = kind;
            PreviewKind = previewKind;
            CollectionKind = collectionKind;
            Operation = operation;
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
                "Presets",
                metadata.Family,
                metadata.Intensity,
                metadata.Direction,
                axisOrPlane,
                $"{preset.DefaultDuration:0.###}s",
                $"target.Tween().Preset<{preset.GetType().Name}>().Play();",
                "PRESET",
                PresetBrowserEntryKind.Preset,
                PresetBrowserPreviewKind.Single,
                PresetBrowserCollectionKind.None,
                PresetBrowserOperation.None,
                preset);
        }

        public static PresetBrowserEntry Collection(string name, string description, string duration, string example, PresetBrowserEntryKind kind, PresetBrowserPreviewKind previewKind, PresetBrowserCollectionKind collectionKind, string direction = null)
        {
            return new PresetBrowserEntry(
                "Collection:" + collectionKind,
                name,
                description,
                "Collections",
                "Collection recipes",
                string.Empty,
                direction ?? string.Empty,
                string.Empty,
                duration,
                example,
                kind == PresetBrowserEntryKind.StaggerVariant ? "ORDER" : "RECIPE",
                kind,
                previewKind,
                collectionKind,
                PresetBrowserOperation.None,
                null);
        }

        public static PresetBrowserEntry Builder(string name, string description, string category, string family, string duration, string example, string badge, PresetBrowserPreviewKind previewKind, PresetBrowserOperation operation, string direction = null, string axisOrPlane = null)
        {
            return new PresetBrowserEntry(
                "Operation:" + operation,
                name,
                description,
                category,
                family,
                string.Empty,
                direction ?? string.Empty,
                axisOrPlane ?? string.Empty,
                duration,
                example,
                badge,
                PresetBrowserEntryKind.BuilderOperation,
                previewKind,
                PresetBrowserCollectionKind.None,
                operation,
                null);
        }
    }
}
