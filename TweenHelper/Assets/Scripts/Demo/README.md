# LB.TweenHelper Demo System

A comprehensive demo showcasing all LB.TweenHelper functionality through interactive scenes, prefabs, and runtime scripts.

## Overview

The demo system provides hands-on examples of every feature in LB.TweenHelper, from basic animations to advanced async patterns. Each demo section focuses on specific functionality with both UI controls and keyboard shortcuts.

## Demo Sections

### 1. Basic Animations (`BasicAnimationDemo`)
- **Move**: MoveTo, MoveBy, MoveToLocal
- **Rotate**: RotateTo, RotateBy, LookAt  
- **Scale**: ScaleTo, ScaleBy (uniform and vector)
- **Fade**: FadeTo, FadeIn, FadeOut for various components
- **Combined**: Multiple simultaneous animations

**Controls:**
- Buttons: Move, Rotate, Scale, Fade, Combined
- Keys: 1-5 for each animation type, E for easing comparison

### 2. Preset Animations (`PresetDemo`)
- **Built-in Presets**: PopIn, PopOut, Bounce, Shake, FadeIn, FadeOut
- **Preset Management**: Check compatibility, list available presets
- **Preset Chains**: Sequential preset combinations
- **Custom Options**: Apply TweenOptions to presets

**Controls:**
- Buttons: Individual preset buttons, All Presets, Random
- Keys: 1-6 for presets, A for all, Q for random, C for compatibility

### 3. Sequence Composition (`SequenceDemo`)
- **Simple Sequences**: Step-by-step animations with delays
- **Parallel Actions**: Join multiple animations to run simultaneously
- **Complex Flows**: Multi-phase sequences with callbacks
- **Looped Sequences**: Repeating sequences with different loop types
- **Preset Integration**: Using presets within sequences

**Controls:**
- Buttons: Simple, Parallel, Complex, Looped, Callbacks, Presets
- Keys: 1-6 for sequence types, F for fade sequences, M for multi-object

### 4. Staggered Animations (`StaggerDemo`)
- **Basic Stagger**: Move, Scale, Preset, Fade with delays
- **Wave Effects**: Sine-wave pattern animations
- **Cascade Effects**: Different animations per object
- **Radial Patterns**: Animations based on distance from center
- **Custom Stagger**: Complex custom animation factories

**Controls:**
- Buttons: Stagger types, Wave, Cascade
- Keys: 1-4 for basic stagger, W for wave, C for cascade, R for radial

### 5. Control Surface (`ControlDemo`)
- **Global Control**: Pause, Resume, Kill, Complete all animations
- **Target Control**: Control animations on specific GameObjects
- **ID-based Control**: Group animations by identifier
- **Individual Control**: Select and control single objects
- **Diagnostics**: Count active tweens, show system status

**Controls:**
- Buttons: Start, Pause All, Resume All, Kill All, Complete All
- Keys: S to start, P/R for pause/resume, K/C for kill/complete, D for diagnostics

### 6. Async Operations (`AsyncDemo`)
- **Await Completion**: Wait for animation finish
- **Await Timeout**: Completion with timeout handling
- **Await Multiple**: WaitAll/WaitAny patterns
- **Cancellation**: Interrupt animations with CancellationToken
- **Async Sequences**: Step-by-step async animation flows
- **Direct Await**: Using tween extension methods

**Controls:**
- Buttons: Different async patterns
- Keys: 1-7 for async types, P for progress demo

### 7. Options & Settings (`OptionsDemo`)
- **Easing**: Different easing functions and comparisons
- **Timing**: Delays, loops, unscaled time
- **Behavior**: Snapping, speed-based animations
- **Fluent API**: Method chaining examples
- **Combined Options**: Multiple options on single animations
- **Preset Options**: Applying options to presets

**Controls:**
- Buttons: Individual option types, comparisons, combinations
- Keys: 1-7 for options, E for easing comparison, F for fluent API

## Setup Instructions

### For Unity Scene Setup:
1. Create a new scene for the demo
2. Add the `TweenDemoSceneSetup` script to an empty GameObject
3. Create demo objects (cubes, UI elements, etc.) and assign to the `demoObjects` array
4. Add individual demo provider components (`BasicAnimationDemo`, `PresetDemo`, etc.)
5. Create a TMP_Dropdown UI element and assign to the controller
6. Ensure the `TweenHelperSettings` asset exists in Resources

### For Custom Demo Objects:
Demo objects should have:
- **Transform**: Required for movement, rotation, scaling
- **CanvasGroup**: For UI fading (optional)
- **Image/SpriteRenderer**: For visual fading (optional)
- **Colliders**: For interaction (optional)

### Recommended Scene Layout:
```
DemoScene
├── Demo Controller (TweenDemoSceneSetup + all IDemoAnimationProvider components)
├── UI Canvas
│   ├── Animation Dropdown (TMP_Dropdown)
│   ├── Play Button
│   ├── Reset Button
│   └── Duration Slider
├── Demo Objects (arranged in grid)
│   ├── Cube 1 (Transform + optional fade component)
│   ├── Cube 2
│   └── ...
└── Camera (positioned to view demo objects)
```

## Navigation

### Universal Controls:
- **Dropdown**: Select animation from categorized list (format: "── CATEGORY ──" headers, "   Animation Name" items)
- **Play Button**: Execute selected animation
- **Reset Button**: Reset all objects to original positions
- **Duration Slider**: Adjust animation duration

### Demo Organization:
All animations are organized by category in the dropdown, with providers automatically discovered via `IDemoAnimationProvider` interface.

## Code Examples

### Basic Usage:
```csharp
// Simple animation
TweenHelper.MoveTo(transform, Vector3.up * 2f);

// With options
var options = TweenOptions.WithEase(Ease.OutBounce).SetDelay(0.5f);
TweenHelper.ScaleTo(transform, Vector3.one * 2f, 1f, options);

// Preset animation
TweenHelper.PlayPreset("PopIn", gameObject);
```

### Sequence Building:
```csharp
TweenHelper.CreateSequence(gameObject)
    .Move(transform, Vector3.up * 3f)
    .Delay(0.5f)
    .Scale(transform, Vector3.one * 1.5f)
    .JoinRotate(transform, Vector3.up * 180f)
    .Play();
```

### Async Operations:
```csharp
var tween = TweenHelper.MoveTo(transform, Vector3.right * 3f);
await TweenHelper.AwaitCompletion(tween);
Debug.Log("Animation completed!");
```

### Control Operations:
```csharp
// Start animations with ID
var options = TweenOptions.WithId("ui-group");
TweenHelper.MoveTo(transform, Vector3.up, 1f, options);

// Control by ID
TweenHelper.PauseById("ui-group");
TweenHelper.ResumeById("ui-group");
TweenHelper.KillById("ui-group");
```

## Performance Notes

- Demo objects are reset efficiently using cached original positions
- Animations are properly cleaned up when switching demos
- Object pooling patterns can be applied for production use
- Diagnostic tools help monitor active tween counts

## Extending the Demo

### Adding New Demo Providers:
1. Create a new MonoBehaviour implementing `IDemoAnimationProvider`
2. Implement `CategoryName` property (e.g., "Custom")
3. Implement `Initialize(GameObject[] objects)` to receive demo objects
4. Implement `GetAnimations()` to return `IEnumerable<DemoAnimation>`
5. Add the provider component to the scene's demo controller GameObject
6. The provider will be auto-discovered and animations added to the dropdown

Example:
```csharp
public class CustomDemo : MonoBehaviour, IDemoAnimationProvider
{
    private GameObject[] demoObjects;

    public string CategoryName => "Custom";

    public void Initialize(GameObject[] objects)
    {
        demoObjects = objects;
    }

    public IEnumerable<DemoAnimation> GetAnimations()
    {
        yield return new DemoAnimation
        {
            Name = "My Animation",
            Category = CategoryName,
            Execute = (transforms, duration) =>
            {
                foreach (var t in transforms)
                    if (t != null) TweenHelper.MoveTo(t, Vector3.up * 2f, duration);
            }
        };
    }
}
```

### Custom Presets:
Create ScriptableObject-based presets by inheriting from `TweenPresetBase`:
```csharp
[CreateAssetMenu(menuName = "Demo/Custom Preset")]
public class CustomDemoPreset : TweenPresetBase
{
    public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
    {
        // Custom animation logic
        return TweenHelper.CreateSequence(target)
            .Scale(target.transform, Vector3.one * 2f, duration ?? DefaultDuration)
            .Build(options);
    }
}
```

## Troubleshooting

### Common Issues:
- **No animations playing**: Check that TweenHelperSettings exists in Resources
- **UI not responsive**: Verify UI buttons are properly assigned in inspector
- **Objects not resetting**: Ensure demo objects array is populated
- **Compilation errors**: Verify assembly references include LB.TweenHelper.Runtime

### Debug Information:
- Enable `showDebugInfo` flags in demo components for detailed logging
- Use the Diagnostics button in Control Demo to check active tween counts
- Monitor the console for demo-specific log messages

## Integration Tips

### For Production Use:
- Extract individual demo patterns as reusable utility methods
- Implement object pooling for frequently animated objects
- Use the preset system for consistent animation branding
- Apply the async patterns for UI flow coordination

### For Learning:
- Run demos in parallel with the source code to understand implementation
- Experiment with different option combinations
- Create custom presets for your specific use cases
- Use the diagnostic tools to understand performance characteristics

