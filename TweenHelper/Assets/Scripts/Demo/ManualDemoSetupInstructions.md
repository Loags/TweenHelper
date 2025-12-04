# Manual Demo Scene Setup Instructions

Follow these step-by-step instructions to set up a TweenHelper demo scene from scratch.

## Step 1: Create Demo Objects

1. **Create parent object**:
   - Right-click in Hierarchy → Create Empty
   - Name it "Demo Objects"

2. **Create 9 demo objects** in a 3x3 grid:

   **Row 1 (Z = -3):**
   - Object 0: Cube at (-3, 0, -3) - Red material
   - Object 1: Sphere at (0, 0, -3) - Green material
   - Object 2: Cylinder at (3, 0, -3) - Blue material

   **Row 2 (Z = 0):**
   - Object 3: Capsule at (-3, 0, 0) - Yellow material
   - Object 4: Cube at (0, 0, 0) - Cyan material
   - Object 5: Sphere at (3, 0, 0) - Magenta material

   **Row 3 (Z = 3):**
   - Object 6: Cylinder at (-3, 0, 3) - Orange material
   - Object 7: Capsule at (0, 0, 3) - Purple material
   - Object 8: Cube at (3, 0, 3) - Spring Green material

3. **For each demo object**:
   - Add **CanvasGroup** component (for fade demos)
   - Add **TweenLifecycleTracker** component (for cleanup)
   - Create and assign colored material

## Step 2: Create UI Canvas

1. **Create Canvas**:
   - Right-click in Hierarchy → UI → Canvas
   - Name it "Demo UI Canvas"
   - Set Render Mode to "Screen Space - Overlay"

2. **Add Canvas Scaler**:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920 x 1080

3. **Create UI elements**:

### Animation Dropdown
- Create UI → Dropdown - TextMeshPro (name it "AnimationDropdown")
- Position at top of screen
- This will be populated automatically by TweenDemoSceneSetup
- Format: Category headers "── CATEGORY ──", items "   Animation Name"

### Control Panel
- Create UI → Panel (name "ControlPanel")
- Add three buttons:
  - **Play Button**: Executes selected animation
  - **Reset Button**: Resets demo objects to original positions
  - **Stop Button**: Kills all active tweens

### Duration Slider (Optional)
- Create UI → Slider
- Name it "DurationSlider"
- Min Value: 0.1, Max Value: 5.0, Default: 1.0
- Add label text: "Animation Duration"

### Info Text (Optional)
- Create UI → Text - TextMeshPro
- Name it "InfoText"
- Displays animation info and instructions

## Step 3: Create Demo Controller

1. **Create controller GameObject**:
   - Right-click in Hierarchy → Create Empty
   - Name it "TweenDemoController"

2. **Add TweenDemoSceneSetup script**:
   - Add Component → TweenDemoSceneSetup

3. **Add all demo provider scripts** to the same GameObject:
   - BasicAnimationDemo
   - PresetDemo
   - SequenceDemo
   - StaggerDemo
   - ControlDemo
   - AsyncDemo
   - OptionsDemo

   *Providers are auto-discovered via `GetComponentsInChildren<IDemoAnimationProvider>()`*

## Step 4: Link Everything in Inspector

In the **TweenDemoSceneSetup** component:

1. **Demo Objects Array**:
   - Set size to 9
   - Drag all 9 demo objects from Step 1

2. **UI References**:
   - Animation Dropdown → AnimationDropdown (TMP_Dropdown)
   - Play Button → Play button
   - Reset Button → Reset button
   - Stop Button → Stop button
   - Duration Slider → DurationSlider (optional)
   - Info Text → InfoText (optional)

3. **Settings** (optional):
   - Default Duration: 1.0
   - Auto Initialize On Start: ✓ (checked)

## Step 5: Position Camera

1. Select Main Camera
2. Set Position to (0, 5, -10)
3. Set Rotation to (30, 0, 0)
4. Adjust to frame all demo objects

## Step 6: Test Setup

1. **Press Play**
2. **Check dropdown population**: Should see 7 categories with 65 total animations
3. **Select an animation** from dropdown (e.g., "Basic" → "   MoveTo (Up)")
4. **Click Play button** → Objects should animate
5. **Click Reset button** → Objects return to starting positions
6. **Try different categories**: Presets, Sequences, Stagger, etc.

## Expected Dropdown Structure

```
── Basic ──
   MoveTo (Up)
   MoveTo (Right)
   RotateTo (180° Y)
   ...
── Presets ──
   PopIn
   PopOut
   Bounce
   ...
── Sequences ──
   Simple Sequence
   Parallel Sequence
   ...
```

## Troubleshooting

### Dropdown is empty:
- Ensure all 7 provider scripts are added to TweenDemoController GameObject
- Check TweenDemoSceneSetup has reference to TMP_Dropdown

### Animations don't play:
- Verify all 9 demo objects are assigned in inspector
- Check TweenHelperSettings exists in Resources folder
- Ensure CanvasGroup is on objects for fade animations

### Objects don't reset:
- TweenDemoSceneSetup caches original positions on Start()
- Make sure objects are in correct positions before pressing Play in editor

## Adding Custom Animation Providers

To add your own animations:

1. **Create new script** implementing `IDemoAnimationProvider`:
```csharp
using LB.TweenHelper.Demo;

public class MyCustomDemo : MonoBehaviour, IDemoAnimationProvider
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
                    if (t != null) TweenHelper.MoveTo(t, Vector3.up * 5f, duration);
            }
        };
    }
}
```

2. **Add script** to TweenDemoController GameObject
3. **Press Play** → Your category appears in dropdown automatically!

This setup provides a clean, extensible demo system with all 65 TweenHelper animations accessible via dropdown.
