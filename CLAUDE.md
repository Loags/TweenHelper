# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

TweenHelper is a high-level animation facade built on DOTween (Free) for Unity 6. It provides a fluent builder API, named presets with auto-registration, sequence composition, and automatic cleanup via GameObject linking.

**Repository**: https://github.com/Loags/TweenHelper

## Build & Development

Unity project — no CLI build commands. Open `TweenHelper/` in Unity 6+. Create settings asset via menu: `LB > TweenHelper > Create Settings Asset`.

## Architecture

### Core Runtime (`Assets/Scripts/LB/TweenHelper/Runtime/`)

The main entry point is the fluent builder accessed via extension methods:

```
TweenBuilderExtensions.cs    → .Tween() extension on Transform/GameObject/Component
TweenBuilder.cs              → Fluent builder: move/rotate/scale/fade/preset/sequence composition
TweenHandle.cs               → Wrapper returned by Play()/Build() with control, status, async/await
TweenOptions.cs              → Value type struct for per-call overrides (ease, delay, loops, etc.)
TweenDefaults.cs             → .WithDefaults() extension to apply settings to raw DOTween tweens
```

Preset system:

```
ITweenPreset.cs              → Interface + ICategorizedTweenPreset for grouping
CodePreset.cs                → Abstract base class (partial) with helper methods
BuiltInPresets.cs            → ~30 presets: PopIn, Shake, Spin, Orbit, Float, etc.
PresetCategories.cs          → Category string constants
PresetExtensions.cs          → Preset-related extensions
TweenPresetRegistry.cs       → Registry with [AutoRegisterPreset] attribute scanning
```

Infrastructure:

```
TweenHelperSettings.cs       → Singleton ScriptableObject with global defaults
TweenHelperBootstrapper.cs   → [RuntimeInitializeOnLoadMethod(BeforeSceneLoad)] init
DoTweenIntegration.cs        → DOTween engine configuration
TweenLifecycleTracker.cs     → Fallback cleanup when GameObjects destroyed
TweenAsync.cs                → Async/await utilities (AwaitCompletion, timeout)
```

### Assembly Dependencies

```
DOTween (external)
    ↑
LB.TweenHelper.Runtime
    ↑
LB.TweenHelper.Editor (editor-only: TweenHelperSettingsCreator.cs)
```

### Data Flow

```
transform.Tween().Move(target).WithEase(Ease.OutBounce).Play()
    → TweenBuilder accumulates TweenSteps
    → Build() creates single Tween or DOTween Sequence
    → ApplyOptions() merges TweenOptions with TweenHelperSettings defaults
    → SetLink(gameObject) for auto-cleanup
    → Returns TweenHandle (implicit conversion to/from Tween)
```

### Key Design Patterns

- **Fluent Builder**: `TweenBuilder` with method chaining, `Then()`/`With()` for sequence vs parallel
- **Value Type Options**: `TweenOptions` struct with nullable fields — unset fields fall through to settings defaults
- **Auto-Registration**: `[AutoRegisterPreset]` attribute on `CodePreset` subclasses, scanned by `TweenPresetRegistry`
- **Implicit Conversion**: `TweenHandle` converts to/from DOTween `Tween` transparently
- **Singleton Settings**: `TweenHelperSettings.Instance` lazy-loaded from Resources

### Preset Easing System

Presets support primary/secondary/tertiary ease overrides via `TweenOptions.WithEases()`. Multi-tween presets (e.g., DropIn with fall + bounce) use `ResolveEase()`, `ResolveSecondaryEase()`, `ResolveTertiaryEase()` to pick per-phase easing.

Looping presets (Float, Orbit) use manual callback-based loops with `WithLoopDefaults()` instead of DOTween's built-in loop system, applying delay only on the first cycle.

## Coding Standards

- **Namespace**: `LB.TweenHelper` for all code
- **Parameters**: Optional `float? duration` (null → preset default or settings default), optional `TweenOptions`
- **Returns**: DOTween `Tween`/`Sequence` from presets; `TweenHandle` from builder
- **Defaults**: Always pull from `TweenHelperSettings.Instance`
- **Presets**: Always call `.WithDefaults(options, target)` at the end of `CreateTween()`
- **XML docs**: On all public types and members
- **One public class per file**, file name matches class name

## Adding a New Preset

1. Create a class extending `CodePreset` in `BuiltInPresets.cs` (or a new file)
2. Add `[AutoRegisterPreset]` attribute
3. Override `PresetName`, `Description`, `DefaultDuration`, `Category`
4. Implement `CreateTween()` — end with `.WithDefaults(options, target)`
5. Optionally add a convenience method to `TweenBuilder` in the Direct Preset Methods region

## Key Files by Task

| Task | Primary File |
|------|--------------|
| Add animation to builder | TweenBuilder.cs |
| Add new preset | BuiltInPresets.cs, CodePreset.cs |
| Modify global defaults | TweenHelperSettings.cs |
| Change how defaults apply | TweenDefaults.cs |
| Sequence/parallel composition | TweenBuilder.cs (Then/With methods) |
| Async support | TweenAsync.cs, TweenHandle.cs |
| DOTween engine config | DoTweenIntegration.cs, TweenHelperBootstrapper.cs |

## Reference Documentation

- `TweenHelper_txt/SystemDescription.txt` — High-level system overview
- `TweenHelper_txt/ImplementationWorkflow.txt` — Implementation priority order
- `TweenHelper_txt/CodingStyleGuide.txt` — Naming and API conventions
- `TweenHelper_txt/PerformanceGuide.txt` — Allocation and cleanup strategies
