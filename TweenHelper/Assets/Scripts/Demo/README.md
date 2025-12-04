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
2. Add the `TweenDemoController` script to an empty GameObject
3. Create demo objects (cubes, UI elements, etc.) and assign to the `demoObjects` array
4. Add individual demo components (`BasicAnimationDemo`, `PresetDemo`, etc.)
5. Create UI buttons and assign to the controller's button arrays
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
├── Demo Controller (TweenDemoController + all demo components)
├── UI Canvas
│   ├── Demo Buttons
│   ├── Instructions Text
│   └── Current Demo Text
├── Demo Objects (arranged in grid)
│   ├── Cube 1 (Transform + optional fade component)
│   ├── Cube 2
│   └── ...
└── Camera (positioned to view demo objects)
```

## Navigation

### Universal Controls:
- **Arrow Keys**: Left/Right to switch demo sections
- **Space**: Stop all animations
- **R**: Reset all objects to original positions

### Demo-Specific Controls:
Each demo section has its own keyboard shortcuts displayed in the GUI overlay.

### GUI Information:
- Current demo name and instructions
- Keyboard shortcuts overlay
- Real-time status information
- Time scale controls (for unscaled time demos)

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

### Adding New Demo Sections:
1. Create a new demo script inheriting the pattern of existing demos
2. Add initialization, button setup, and keyboard controls
3. Include the new demo in `TweenDemoController.SetupDemos()`
4. Add UI buttons and update the demo names array

### Adding New Demo Objects:
- Implement `IDemoTarget` interface for consistent behavior
- Support common components (Transform, CanvasGroup, Image, etc.)
- Consider fade compatibility for comprehensive testing

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

