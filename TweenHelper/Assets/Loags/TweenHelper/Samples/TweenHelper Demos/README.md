# TweenHelper demos

This sample contains a prefab-authored 2D UI showcase and the full 3D preset showcase.

Before opening the scenes, import TextMesh Pro Essential Resources. The 3D scene materials are authored for the Universal Render Pipeline; the 2D scene and TweenHelper runtime are render-pipeline independent.

The 2D scene has two runtime tabs:

- **UI Recipes** demonstrates all 13 semantic UI helpers on Image and Text targets.
- **2D Preset Library** provides searchable, family-filtered access to the 198 presets that use UI-suitable position, scale, color/alpha, and Z-rotation animation.

Select a recipe or preset to preview it, switch the preview target between Image and Text, replay or reset it, and copy its typed fluent API example. The showcase controller only configures and instantiates authored UI prefabs from `Prefabs/UI`; it does not construct UI hierarchies at runtime.

World-scale movement presets use an explicit strength override in the 2D canvas preview so their motion remains visible in pixel space. The displayed and copied API example includes that override.

The demos do not require the Input System package. When the legacy Input Manager is enabled:

- In the 2D scene, use **Space** to replay the current selection. All browsing and filtering is available through the authored runtime UI.
- In the 3D scene, use **WASD** to move, **Q/E** to move vertically, hold the right mouse button to look, scroll to change speed, and hold **Shift** to move faster. Click an object to play it, press **R** to reset the last animation, or press **Shift+R** to reset all.

The public component and context-menu controls remain available when the legacy Input Manager is disabled.
