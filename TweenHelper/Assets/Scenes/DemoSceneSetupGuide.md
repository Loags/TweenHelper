# TweenHelper Demo Scene Setup Guide

This guide will help you create a fully functional demo scene for LB.TweenHelper that showcases all features with an interactive UI.

## Quick Setup (Recommended)

### Option 1: Use Existing Demo Scene
1. Open `TweenHelperDemo.unity` in `Assets/Scenes/`
2. Verify `TweenDemoSceneSetup` component has demo objects and UI references assigned
3. Press Play - 65 animations should be accessible via dropdown

### Option 2: Manual Setup
Follow the detailed instructions below to create a demo scene from scratch.

## Manual Setup Instructions

### Step 1: Scene Setup
1. Create a new scene or open `Assets/Scenes/TweenHelperDemo.unity`
2. Ensure you have:
   - **Main Camera** positioned at `(0, 8, -12)` with rotation `(30, 0, 0)`
   - **Directional Light** with default settings

### Step 2: Create Demo Objects
1. Create an empty GameObject named **"Demo Objects"**
2. Create 9 child objects arranged in a 3x3 grid:

#### Demo Object Configuration:
For each object (positions given as examples):
- **Object 0**: Cube at `(-3, 0, -3)` - Red material
- **Object 1**: Sphere at `(0, 0, -3)` - Green material  
- **Object 2**: Cylinder at `(3, 0, -3)` - Blue material
- **Object 3**: Capsule at `(-3, 0, 0)` - Yellow material
- **Object 4**: Cube at `(0, 0, 0)` - Cyan material
- **Object 5**: Sphere at `(3, 0, 0)` - Magenta material
- **Object 6**: Cylinder at `(-3, 0, 3)` - Orange material
- **Object 7**: Capsule at `(0, 0, 3)` - Purple material
- **Object 8**: Cube at `(3, 0, 3)` - Spring Green material

#### For Each Demo Object:
1. Add the primitive mesh (Cube, Sphere, etc.)
2. Create a material with the specified color
3. Add a **CanvasGroup** component (for fade demos)
4. Add a **TweenLifecycleTracker** component (for safety)

### Step 3: Create UI Canvas
1. Create a **Canvas** GameObject named **"Demo UI Canvas"**
2. Set Canvas settings:
   - **Render Mode**: Screen Space - Overlay
   - **Canvas Scaler**: Scale With Screen Size
   - **Reference Resolution**: 1920 x 1080

#### UI Hierarchy Structure:
```
Demo UI Canvas
├── Animation Dropdown (TMP_Dropdown)
│   └── Auto-populated with categories and animations
├── Control Panel
│   ├── Play Button (triggers selected animation)
│   ├── Reset Button (resets demo objects)
│   └── Stop Button (kills all tweens)
├── Duration Slider (Optional)
│   └── Adjusts animation duration (0.1 - 5.0)
└── Info Text (Optional)
    └── Status messages and instructions
```

**Note:** The dropdown will automatically populate with format:
- Category headers: `── CATEGORY ──`
- Animation items: `   Animation Name`

### Step 4: Create Demo Controller
1. Create an empty GameObject named **"Demo Controller"**
2. Add the **TweenDemoSceneSetup** script
3. Add all demo provider scripts to the same GameObject:
   - **BasicAnimationDemo** (implements IDemoAnimationProvider)
   - **PresetDemo** (implements IDemoAnimationProvider)
   - **SequenceDemo** (implements IDemoAnimationProvider)
   - **StaggerDemo** (implements IDemoAnimationProvider)
   - **ControlDemo** (implements IDemoAnimationProvider)
   - **AsyncDemo** (implements IDemoAnimationProvider)
   - **OptionsDemo** (implements IDemoAnimationProvider)

**Note:** Providers are auto-discovered via `GetComponentsInChildren<IDemoAnimationProvider>()`

### Step 5: Link Components in Inspector
In the **TweenDemoSceneSetup** component:

#### 1. Demo Objects Array:
- Set size to 9
- Drag all 9 demo objects from Step 2 into the array (in order 0-8)

#### 2. UI References:
- **Animation Dropdown** → TMP_Dropdown component
- **Play Button** → Play button (optional, can call `PlaySelectedAnimation()` manually)
- **Reset Button** → Reset button (optional, can call `ResetDemoObjects()` manually)
- **Stop Button** → Stop button (optional, can call `StopAllAnimations()` manually)
- **Duration Slider** → Slider component (optional)
- **Info Text** → TextMeshProUGUI component (optional)

#### 3. Settings:
- **Default Duration**: 1.0 (animation duration in seconds)
- **Auto Initialize On Start**: ✓ (checked)

#### 4. Provider Discovery:
No manual linking needed - providers are auto-discovered from child components!

### Step 6: TweenHelper Settings
1. Create `TweenHelperSettings` asset in `Assets/Resources/` if it doesn't exist
2. Use menu: **Assets → Create → LB.TweenHelper → Settings**
3. Configure default settings as desired

## UI Layout Guidelines

### Animation Dropdown:
- **Position**: Top center of screen
- **Width**: 400-600 pixels
- **Auto-populates** with 7 categories and 65 total animations
- **Format**: Category headers (`── CATEGORY ──`) and items (`   Animation Name`)

### Control Panel:
- **Layout**: Horizontal Layout Group
- **Buttons**: Play, Reset, Stop (uniform size)
- **Position**: Below dropdown
- **Spacing**: 10px between buttons

### Duration Slider (Optional):
- **Min Value**: 0.1
- **Max Value**: 5.0
- **Default**: 1.0
- **Position**: Below control panel

### Info Text (Optional):
- **Font Size**: 18-24px
- **Position**: Bottom of screen
- **Content**: Animation status and instructions

## Testing the Setup

### Basic Functionality Test:
1. Press Play in Unity
2. Check dropdown is populated with animations (should show 65 items across 7 categories)
3. Select an animation (e.g., "Basic" → "   MoveTo (Up)")
4. Click Play button (or call `PlaySelectedAnimation()`)
5. Verify demo objects animate as expected
6. Click Reset button to restore original positions

### Full Feature Test:
1. Test animations from each category:
   - Basic (16 animations)
   - Presets (8 animations)
   - Sequences (8 animations)
   - Stagger (9 animations)
   - Control (7 animations)
   - Async (6 animations)
   - Options (11 animations)
2. Verify multi-object animations work correctly
3. Check that Reset properly restores all objects
4. Test duration slider (if implemented)

## Troubleshooting

### Common Issues:

**No animations playing:**
- Check that `TweenHelperSettings` exists in `Assets/Resources/`
- Verify DoTween is properly imported
- Check console for initialization errors

**UI not responding:**
- Ensure `EventSystem` exists in scene (created automatically with Canvas)
- Verify Canvas has `GraphicRaycaster` component
- Check TMP_Dropdown is properly linked in TweenDemoSceneSetup

**Dropdown is empty:**
- Ensure all 7 provider scripts are added to the Demo Controller GameObject
- Check providers implement `IDemoAnimationProvider` interface
- Verify `TweenDemoSceneSetup.Start()` is being called

**Demo objects not found:**
- Check that all 9 objects are linked in the Demo Objects array
- Ensure objects have the required components (CanvasGroup, TweenLifecycleTracker)
- Verify object positions are reasonable for camera view

**Compilation errors:**
- Ensure all demo scripts are in the correct namespace (`LB.TweenHelper.Demo`)
- Check that assembly references are correct
- Verify all required using statements are present

### Performance Tips:
- Keep demo object count reasonable (9 is optimal)
- Use simple materials to avoid rendering overhead
- Position camera to view all demo objects comfortably
- Test on target platform for performance validation

## Customization

### Adding Custom Demo Objects:
1. Create new objects following the pattern in Step 2
2. Add to the Demo Objects array in TweenDemoSceneSetup
3. Ensure they have CanvasGroup and TweenLifecycleTracker components

### Modifying UI Layout:
1. Adjust dropdown size and position as needed
2. Customize button styles and colors
3. Add additional UI elements (duration slider, info text, etc.)

### Extending Demo Functionality (Adding New Animations):
1. Create new script implementing `IDemoAnimationProvider`:
```csharp
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
2. Add the new provider to Demo Controller GameObject
3. Press Play - new category appears in dropdown automatically!

This setup provides a comprehensive testing environment for all LB.TweenHelper features with an intuitive user interface!

