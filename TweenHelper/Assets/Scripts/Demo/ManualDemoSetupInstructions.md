# Manual Demo Scene Setup Instructions

If the automatic setup doesn't work perfectly, follow these step-by-step instructions:

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

3. **Create UI structure**:

### Header Panel
- Create UI → Panel, name "Header Panel"
- Anchor: Top stretch (0,0.9) to (1,1)
- Add two Text components:
  - "Title Text" - "LB.TweenHelper Demo"
  - "CurrentDemoText" - "Demo: Basic Animations"

### Demo Selection Panel  
- Create UI → Panel, name "Demo Selection Panel"
- Anchor: (0,0.8) to (1,0.9)
- Add Horizontal Layout Group
- Create 7 buttons with text:
  - "Basic Animations", "Presets", "Sequences", "Stagger", "Control", "Async", "Options"

### Individual Demo Panels
Create separate panels for each demo type with specific buttons:

**Basic Animation Panel:**
- Buttons: "Move", "Rotate", "Scale", "Fade", "Combined"

**Preset Panel:**
- Buttons: "PopIn", "PopOut", "Bounce", "Shake", "FadeIn", "FadeOut", "All", "Random"

**Sequence Panel:**
- Buttons: "Simple", "Parallel", "Complex", "Looped", "Callbacks", "Presets"

**Stagger Panel:**
- Buttons: "Move", "Scale", "Preset", "Fade", "Wave", "Cascade"

**Control Panel:**
- Buttons: "Start", "Pause All", "Resume All", "Kill All", "Complete All", "ID Control", "Diagnostics"
- Individual controls: "Pause Obj", "Resume Obj", "Kill Obj"

**Async Panel:**
- Buttons: "Await Complete", "Await Timeout", "Await All", "Await Any", "Cancellation", "Async Sequence", "Direct Await"

**Options Panel:**
- Buttons: "Easing", "Delay", "Loops", "Unscaled Time", "Snapping", "Speed Based", "Combined", "Fluent API", "Comparison"

### Footer Panel
- Create UI → Panel, name "Footer Panel"
- Anchor: Bottom stretch (0,0) to (1,0.15)
- Add Text: "InstructionsText" with usage instructions

## Step 3: Create Demo Controller

1. **Create controller object**:
   - Right-click in Hierarchy → Create Empty
   - Name it "TweenDemoController"

2. **Add all demo scripts**:
   - TweenDemoController
   - BasicAnimationDemo
   - PresetDemo
   - SequenceDemo
   - StaggerDemo
   - ControlDemo
   - AsyncDemo
   - OptionsDemo

## Step 4: Link Everything

In the TweenDemoController component:

1. **Link Demo Objects**: Drag all 9 objects into the array
2. **Link UI Elements**: Connect all text and button references
3. **Link Individual Demo Controls**: Connect each demo's specific buttons

## Button Linking Reference

### Main Demo Buttons (in demoButtons array):
- Index 0: Basic Animations Button
- Index 1: Presets Button
- Index 2: Sequences Button
- Index 3: Stagger Button
- Index 4: Control Button
- Index 5: Async Button
- Index 6: Options Button

### BasicAnimationDemo buttons:
- moveButton → "Move" button
- rotateButton → "Rotate" button  
- scaleButton → "Scale" button
- fadeButton → "Fade" button
- combinedButton → "Combined" button

### Continue for all other demo types...

## Step 5: Test Setup

1. Press Play
2. Try clicking "Move" in Basic Animations
3. Verify objects animate
4. Test demo switching with top buttons
5. Check keyboard shortcuts work

This manual setup ensures everything works exactly as intended!

