# LB.TweenHelper

A high-level animation facade for DoTween (Free) that makes common animations fast to write, safe to run, and easy to reuse.

## Features

- **One-line animations**: Simple, intention-revealing methods for common tasks
- **Centralized defaults**: Project-wide settings with per-call overrides
- **Automatic cleanup**: Tweens are killed automatically when GameObjects are destroyed
- **Named presets**: Reusable animations that can be played by name
- **Sequence composition**: Readable step-by-step animation building
- **Control surface**: Pause, resume, kill operations by target or identifier
- **Async support**: Await tween completion with timeout and cancellation support

## Quick Start

### 1. Create Settings Asset

Use the menu: `LB > TweenHelper > Create Settings Asset`

This creates a `TweenHelperSettings` asset in your Resources folder where you can configure global defaults.

### 2. Basic Animations

```csharp
using LB.TweenHelper;

// Move to position
TweenHelper.MoveTo(transform, new Vector3(5, 0, 0));

// Scale with custom duration
TweenHelper.ScaleTo(transform, Vector3.one * 2f, 1.5f);

// Fade out with options
TweenHelper.FadeOut(canvasGroup, 0.5f, TweenOptions.WithEase(Ease.OutQuart));

// Play a preset animation
TweenHelper.PlayPreset("PopIn", gameObject);
```

### 3. Sequences

```csharp
// Build a sequence step by step
TweenHelper.CreateSequence(gameObject)
    .Move(transform, Vector3.up * 2f)
    .Delay(0.5f)
    .Scale(transform, Vector3.one * 1.5f)
    .Call(() => Debug.Log("Done!"))
    .Play();
```

### 4. Staggered Animations

```csharp
// Animate multiple objects with stagger
var buttons = FindObjectsOfType<Button>();
TweenHelper.StaggerPreset("PopIn", buttons.Select(b => b.gameObject), 0.1f);
```

### 5. Control

```csharp
// Control by target
TweenHelper.Pause(gameObject);
TweenHelper.Resume(gameObject);
TweenHelper.Kill(gameObject);

// Control by identifier
TweenHelper.PlayPreset("FadeIn", gameObject, options: TweenOptions.WithId("ui-transition"));
TweenHelper.PauseById("ui-transition");
```

### 6. Async Support

```csharp
public async void ExampleAsync()
{
    var tween = TweenHelper.MoveTo(transform, Vector3.up);
    
    // Await completion
    await TweenHelper.AwaitCompletion(tween);
    
    // Await with timeout
    bool completed = await TweenHelper.AwaitCompletionWithTimeout(tween, 5f);
    
    // Direct awaiting (using extension method)
    await tween;
}
```

## Core Components

### TweenOptions

Fluent value type for per-call overrides:

```csharp
var options = TweenOptions.WithEase(Ease.OutBounce)
    .SetDelay(0.5f)
    .SetLoops(3, LoopType.Yoyo)
    .SetId("my-animation");

TweenHelper.ScaleTo(transform, Vector3.one * 2f, 1f, options);
```

### Built-in Presets

- `PopIn`: Scale from zero with bouncy ease
- `PopOut`: Scale to zero with sharp ease  
- `Bounce`: Quick scale up and back
- `Shake`: Random position shake
- `FadeIn`: Alpha 0 to 1
- `FadeOut`: Alpha to 0

### Custom Presets

Create custom presets by inheriting from `TweenPresetBase`:

```csharp
[CreateAssetMenu(menuName = "My Game/Button Pop Preset")]
public class ButtonPopPreset : TweenPresetBase
{
    public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
    {
        var finalOptions = CreateDefaultOptions(options);
        return TweenHelper.CreateSequence(target)
            .Scale(target.transform, Vector3.one * 1.2f, (duration ?? DefaultDuration) * 0.3f, finalOptions)
            .Scale(target.transform, Vector3.one, (duration ?? DefaultDuration) * 0.7f, finalOptions.SetEase(Ease.OutElastic))
            .Build(finalOptions);
    }
    
    public override bool CanApplyTo(GameObject target)
    {
        return target != null && target.GetComponent<Button>() != null;
    }
}
```

## Performance Notes

- Settings are applied at startup to avoid allocation during gameplay
- Tweens are automatically linked to GameObjects for safe cleanup
- Use the TweenLifecycleTracker component as a fallback safety net
- Set appropriate capacities in settings to avoid internal array resizes
- Use identifiers for batch operations rather than individual tween references

## Architecture

The system follows a strict implementation order:

1. **Settings & Bootstrap**: Single source of truth for configuration
2. **DoTween Integration**: Central configuration layer
3. **Lifecycle Tracking**: Safety fallback for cleanup
4. **Public Interface**: Main user-facing API
5. **Options Object**: Per-call override system
6. **Preset System**: Named reusable animations
7. **Sequence Composition**: Multi-step animation building
8. **Control Surface**: Operational control by target/identifier
9. **Async Helpers**: Integration with async workflows

Each layer builds upon the previous ones, ensuring a solid foundation with progressive feature enhancement.
