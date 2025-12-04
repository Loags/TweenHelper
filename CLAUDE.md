# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

TweenHelper is a high-level animation facade built on DOTween (Free) for Unity 6. It provides centralized defaults, named animation presets, sequence composition, and automatic cleanup.

**Repository**: https://github.com/Loags/TweenHelper

## Build & Development

This is a Unity project - there are no CLI build commands. Development workflow:

1. Open `TweenHelper/` folder in Unity 6+
2. Create TweenHelperSettings asset via menu: `LB > TweenHelper > Create Settings Asset`
3. Run demo scenes in `Assets/Scenes/` to validate functionality

## Architecture

### Core Runtime (`Assets/Scripts/LB/TweenHelper/Runtime/`)

```
TweenHelperSettings.cs       → Singleton ScriptableObject with global defaults
TweenHelperBootstrapper.cs   → Auto-initializes at BeforeSceneLoad
DoTweenIntegration.cs        → Central configuration hub, merges options with defaults
TweenHelper.cs               → Main public API (~1000 LOC static methods)
TweenOptions.cs              → Lightweight struct for per-call overrides
TweenController.cs           → Control surface (pause/resume/kill by target or ID)
TweenSequenceBuilder.cs      → Fluent builder for multi-step animations
TweenAsync.cs                → Async/await support for tweens
TweenPresetRegistry.cs       → Dynamic preset discovery and lookup
TweenPresetBase.cs           → Abstract base for ScriptableObject presets
BuiltInPresets.cs            → PopIn, PopOut, Bounce, Shake, FadeIn, FadeOut
TweenStagger.cs              → Multi-object animation helpers
TweenLifecycleTracker.cs     → Fallback cleanup when GameObjects destroyed
```

### Assembly Dependencies

```
DOTween (external)
    ↑
LB.TweenHelper.Runtime
    ↑
LB.TweenHelper.Editor (editor-only tools)

LB.TweenHelper.Demo (optional, references Runtime + Unity.ugui)
```

### Key Design Patterns

- **Singleton**: TweenHelperSettings with lazy loading from Resources
- **Builder**: TweenSequenceBuilder for fluent sequence composition
- **Value Type**: TweenOptions as lightweight options carrier (no allocations)
- **Registry**: TweenPresetRegistry for preset management
- **Fluent API**: Method chaining on TweenOptions

### Data Flow

```
TweenHelper.MoveTo(transform, target, duration, options)
    → DoTweenIntegration.ConfigureTween()
        → Merges TweenOptions with TweenHelperSettings defaults
        → Links tween to GameObject for auto-cleanup
    → Returns DOTween Tween object
```

## Coding Standards

From `TweenHelper_txt/CodingStyleGuide.txt`:

- **Namespace**: `LB.TweenHelper` for all code
- **API Design**: Intention-revealing public interface, sensible defaults
- **Parameters**: Optional duration (null → settings default), optional TweenOptions
- **Returns**: Always return DOTween Tween/Sequence objects (don't wrap them)
- **Defaults**: Always pull from `TweenHelperSettings.Instance`
- **Documentation**: XML docs on all public types/members

## Common API Patterns

```csharp
// Simple animation
TweenHelper.MoveTo(transform, targetPos, 1f);

// With options
var options = TweenOptions.WithEase(Ease.OutBounce).SetDelay(0.5f);
TweenHelper.ScaleTo(transform, Vector3.one * 2f, 1f, options);

// Preset
TweenHelper.PlayPreset("PopIn", gameObject, 0.5f);

// Sequence
TweenHelper.CreateSequence(gameObject)
    .Move(transform, Vector3.up * 3f)
    .Scale(transform, Vector3.one * 1.5f)
    .Play();

// Batch control
TweenController.PauseById("ui-group");
TweenController.KillAll();

// Async
await TweenAsync.AwaitCompletion(tween);
```

## Key Files by Task

| Task | Primary File |
|------|--------------|
| Add new animation type | TweenHelper.cs, DoTweenIntegration.cs |
| Modify defaults | TweenHelperSettings.cs |
| Add preset | BuiltInPresets.cs or create TweenPresetBase subclass |
| Control operations | TweenController.cs |
| Sequence building | TweenSequenceBuilder.cs |
| Async support | TweenAsync.cs |
| Multi-object animations | TweenStagger.cs |

## Documentation

- `TweenHelper_txt/SystemDescription.txt` - High-level system overview
- `TweenHelper_txt/ImplementationWorkflow.txt` - 10-step implementation priority
- `TweenHelper_txt/CodingStyleGuide.txt` - Naming and API conventions
- `TweenHelper_txt/PerformanceGuide.txt` - Optimization strategies
- `Assets/Scripts/Demo/README.md` - Demo system documentation
