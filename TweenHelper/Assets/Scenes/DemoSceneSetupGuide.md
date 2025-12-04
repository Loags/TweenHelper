# TweenHelper Demo Scene Setup Guide

This guide will help you create a fully functional demo scene for LB.TweenHelper that showcases all features with an interactive UI.

## Quick Setup (Recommended)

### Option 1: Automatic Setup Script
1. Open the `TweenHelperDemo.unity` scene in `Assets/Scenes/`
2. In the Hierarchy, find the `TweenDemoSceneSetup` GameObject
3. In the Inspector, right-click on the `TweenDemoSceneSetup` component
4. Select **"Setup Complete Demo Scene"** from the context menu
5. The script will automatically create all demo objects and UI elements
6. After setup, manually link the UI elements in the `TweenDemoController` (see Manual Setup section for details)

### Option 2: Manual Setup
If the automatic script doesn't work perfectly, follow the manual setup instructions below.

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
├── Header Panel
│   ├── Title Text ("LB.TweenHelper Demo")
│   └── Current Demo Text ("Demo: Basic Animations")
├── Demo Selection Panel
│   ├── Basic Animations Button
│   ├── Presets Button  
│   ├── Sequences Button
│   ├── Stagger Button
│   ├── Control Button
│   ├── Async Button
│   └── Options Button
├── Basic Animation Panel (initially active)
│   ├── Move Button
│   ├── Rotate Button
│   ├── Scale Button
│   ├── Fade Button
│   └── Combined Button
├── Preset Panel (initially hidden)
│   ├── PopIn Button
│   ├── PopOut Button
│   ├── Bounce Button
│   ├── Shake Button
│   ├── FadeIn Button
│   ├── FadeOut Button
│   ├── All Presets Button
│   └── Random Button
├── [Additional Panels for each demo type...]
└── Footer Panel
    └── Instructions Text
```

### Step 4: Create Demo Controller
1. Create an empty GameObject named **"TweenDemoController"**
2. Add the **TweenDemoController** script
3. Add all individual demo scripts:
   - **BasicAnimationDemo**
   - **PresetDemo**
   - **SequenceDemo**
   - **StaggerDemo**
   - **ControlDemo**
   - **AsyncDemo**
   - **OptionsDemo**

### Step 5: Link Components
In the `TweenDemoController` component:

#### Demo Objects Array:
Drag all 9 demo objects into the **Demo Objects** array in order (0-8).

#### UI Text References:
- **Current Demo Text**: Link to the text component showing current demo
- **Instructions Text**: Link to the footer text component

#### Demo Buttons Array:
Link the 7 main demo selection buttons:
1. Basic Animations Button
2. Presets Button
3. Sequences Button
4. Stagger Button
5. Control Button
6. Async Button
7. Options Button

#### Individual Demo Component Linking:
For each demo script, link its specific buttons:

**BasicAnimationDemo**:
- Move Button → moveButton
- Rotate Button → rotateButton
- Scale Button → scaleButton
- Fade Button → fadeButton
- Combined Button → combinedButton

**PresetDemo**:
- PopIn Button → popInButton
- PopOut Button → popOutButton
- Bounce Button → bounceButton
- Shake Button → shakeButton
- FadeIn Button → fadeInButton
- FadeOut Button → fadeOutButton
- All Presets Button → allPresetsButton
- Random Button → randomPresetButton

**[Continue for all other demo types...]**

### Step 6: Panel Visibility Setup
1. Initially activate only the **Basic Animation Panel**
2. Deactivate all other demo panels
3. The `TweenDemoController` will handle switching between panels

### Step 7: TweenHelper Settings
1. Create `TweenHelperSettings` asset in `Assets/Resources/` if it doesn't exist
2. Use menu: **Assets → Create → LB.TweenHelper → Settings**
3. Configure default settings as desired

## UI Layout Guidelines

### Header Panel (Top 10% of screen):
- **Anchors**: Top-stretch
- **Background**: Semi-transparent dark
- **Title Text**: Left-aligned, large font (36px)
- **Current Demo Text**: Right-aligned, medium font (24px)

### Demo Selection Panel (Below header, 10% height):
- **Layout**: Horizontal Layout Group
- **Buttons**: Equal width, uniform spacing
- **Background**: Slightly transparent

### Individual Demo Panels (Main area, 65% height):
- **Layout**: Grid Layout Group for buttons
- **Cell Size**: 180x40 pixels
- **Spacing**: 10px between buttons
- **Padding**: 20px margins

### Footer Panel (Bottom 15% of screen):
- **Instructions Text**: Multi-line, smaller font (18px)
- **Content**: Keyboard shortcuts and usage instructions

## Testing the Setup

### Basic Functionality Test:
1. Press Play in Unity
2. Click **"Basic Animations"** → **"Move"** 
3. Verify that a demo object moves
4. Try switching between demo sections using the top buttons
5. Test keyboard shortcuts (arrows to switch demos, space to stop)

### Full Feature Test:
1. Test each demo section systematically
2. Verify all buttons work
3. Check that keyboard shortcuts function
4. Ensure objects reset properly between demos

## Troubleshooting

### Common Issues:

**No animations playing:**
- Check that `TweenHelperSettings` exists in `Assets/Resources/`
- Verify DoTween is properly imported
- Check console for initialization errors

**UI buttons not responding:**
- Ensure `EventSystem` exists in scene (created automatically with Canvas)
- Check button OnClick events are properly linked
- Verify Canvas has `GraphicRaycaster` component

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
1. Create new objects following the pattern above
2. Add to the Demo Objects array in TweenDemoController
3. Ensure they have CanvasGroup and TweenLifecycleTracker components

### Modifying UI Layout:
1. Adjust panel anchors and sizes as needed
2. Change button layouts using Layout Groups
3. Customize colors and fonts to match your project style

### Extending Demo Functionality:
1. Add new demo methods to existing scripts
2. Create new demo script types following the established pattern
3. Link new controls to the TweenDemoController

This setup provides a comprehensive testing environment for all LB.TweenHelper features with an intuitive user interface!

