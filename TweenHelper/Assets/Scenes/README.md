# TweenHelper Demo Scene Package

This directory contains everything needed to create a comprehensive interactive demo scene for LB.TweenHelper.

## 📁 **What's Included**

### **Core Files:**
- **`TweenHelperDemo.unity`** - Basic Unity scene with camera, lighting, and setup script
- **`TweenDemoSceneSetup.cs`** - Automatic scene generation script 
- **`DemoSceneSetupGuide.md`** - Complete manual setup instructions

### **Demo Scripts:** (in `../Scripts/Demo/`)
- **`TweenDemoSceneSetup.cs`** - Main demo controller with dropdown UI
- **`IDemoAnimationProvider.cs`** - Interface for extensible animation providers
- **`BasicAnimationDemo.cs`** - Move, rotate, scale, fade demonstrations
- **`PresetDemo.cs`** - Built-in preset system showcase
- **`SequenceDemo.cs`** - Multi-step animation sequences
- **`StaggerDemo.cs`** - Coordinated animations with delays
- **`ControlDemo.cs`** - Pause, resume, kill, complete operations
- **`AsyncDemo.cs`** - Async/await patterns and cancellation
- **`OptionsDemo.cs`** - Easing, loops, timing, and advanced options

## 🚀 **Quick Start**

### **Option 1: Automatic Setup (Recommended)**
1. Open `TweenHelperDemo.unity` in Unity
2. Find the `TweenDemoSceneSetup` GameObject in the Hierarchy
3. Assign demo objects and UI references in the Inspector
4. Press Play - animations are accessible via dropdown

### **Option 2: Manual Setup**
Follow the comprehensive instructions in `DemoSceneSetupGuide.md`

## 🎮 **Demo Features**

### **Interactive Controls:**
- **TMP_Dropdown**: Select from 65 categorized animations
- **Play Button**: Execute selected animation
- **Reset Button**: Return objects to starting positions
- **Real-time Feedback**: Console logging and visual status updates

### **7 Complete Demo Categories (65 Total Animations):**

#### **1. Basic (16 animations)**
- Move, Rotate, Scale, Fade operations
- Combined multi-property animations
- Easing comparisons

#### **2. Presets (8 animations)**
- PopIn, PopOut, Bounce, Shake, FadeIn, FadeOut
- Preset chaining and custom options

#### **3. Sequences (8 animations)**
- Step-by-step animations with delays
- Parallel actions using Join methods
- Complex multi-phase flows with callbacks

#### **4. Stagger (9 animations)**
- Coordinated animations across multiple objects
- Wave effects and cascade patterns
- Custom stagger timing

#### **5. Control (7 animations)**
- Pause, Resume, Kill, Complete operations
- Target-based and ID-based control
- Real-time diagnostics

#### **6. Async (6 animations)**
- Await completion with timeouts
- Concurrent animation patterns
- Cancellation token support

#### **7. Options (11 animations)**
- Easing functions and comparisons
- Loops, delays, unscaled time
- Speed-based animations
- Fluent API demonstrations

## 📋 **Scene Structure**

### **Demo Objects (9 total):**
- **3x3 Grid Layout**: Cubes, Spheres, Cylinders, Capsules
- **Colored Materials**: Visual differentiation for tracking
- **CanvasGroup Components**: For fade demonstrations
- **TweenLifecycleTracker**: Automatic cleanup safety

### **UI Layout:**
- **Animation Dropdown**: TMP_Dropdown with categorized animations
- **Control Buttons**: Play, Reset, Stop buttons
- **Duration Slider**: Adjustable animation duration (optional)
- **Info Text**: Status and instructions display (optional)

### **Technical Setup:**
- **Camera**: Positioned for optimal viewing (0, 8, -12)
- **Lighting**: Standard directional light
- **Canvas**: Screen-space overlay with responsive scaling

## 🔧 **Customization**

### **Adding New Demo Providers:**
1. Create new MonoBehaviour implementing `IDemoAnimationProvider`
2. Implement `CategoryName`, `Initialize()`, and `GetAnimations()`
3. Add provider component to scene's demo controller GameObject
4. Provider is auto-discovered and animations added to dropdown

### **Modifying Layout:**
- Adjust object grid spacing in `TweenDemoSceneSetup`
- Customize UI panel layouts and colors
- Modify camera positioning for different views

### **Performance Tuning:**
- Reduce demo object count if needed
- Simplify materials for better performance
- Adjust animation durations for target platform

## 🛠️ **Dependencies**

### **Required:**
- **LB.TweenHelper.Runtime** - Core tween functionality
- **DOTween** - Underlying animation engine
- **Unity.ugui** - UI system for controls

### **Optional:**
- **TweenHelperSettings** asset in `Resources/` for default configuration

## ✅ **Testing Checklist**

### **Basic Functionality:**
- [ ] Demo objects visible and properly positioned
- [ ] Dropdown populates with 65 animations across 7 categories
- [ ] Play button executes selected animation
- [ ] Reset button returns objects to original positions

### **Animation Features:**
- [ ] Basic animations (move, rotate, scale, fade) work
- [ ] Presets play correctly (PopIn, Bounce, etc.)
- [ ] Sequences execute in proper order
- [ ] Staggered animations show delays
- [ ] Control operations affect running animations
- [ ] Async patterns complete successfully
- [ ] Options modify animation behavior

### **Error Handling:**
- [ ] No console errors during normal operation
- [ ] Graceful handling of missing components
- [ ] Proper cleanup when switching demos

## 🔍 **Troubleshooting**

### **Common Issues:**

**"No animations playing"**
- Verify `TweenHelperSettings` exists in Resources/
- Check DoTween installation and setup
- Ensure demo objects are properly linked

**"UI not responding"**
- Check EventSystem exists in scene
- Verify Canvas has GraphicRaycaster
- Confirm button events are properly linked

**"Objects not visible"**
- Adjust camera position to view demo area
- Check object positions and scales
- Verify materials are assigned

**"Performance issues"**
- Reduce number of concurrent animations
- Simplify demo object meshes/materials
- Lower animation update frequency

## 📚 **Learning Path**

### **For New Users:**
1. Start with **Basic Animations** to understand core concepts
2. Explore **Presets** for common animation patterns
3. Learn **Sequences** for multi-step animations
4. Try **Options** to understand customization

### **For Advanced Users:**
1. Study **Control Surface** for production integration patterns
2. Explore **Async** for modern C# integration
3. Examine **Stagger** for complex multi-object coordination
4. Review source code for implementation details

This demo scene provides a comprehensive, hands-on way to explore every feature of LB.TweenHelper in an interactive environment! 🎉

