# TweenHelper demos

This sample contains a prefab-authored 2D UI showcase and the full 3D preset showcase.

Before opening the scenes, import TextMesh Pro Essential Resources. The 3D scene materials are authored for the Universal Render Pipeline; the 2D scene and TweenHelper runtime are render-pipeline independent.

The 2D scene has seven runtime tabs:

- **UI Recipes** demonstrates all 13 semantic UI helpers on Image and Text targets.
- **2D Preset Library** provides searchable, family-filtered access to the 198 presets that use UI-suitable position, scale, color/alpha, and Z-rotation animation.
- **Collections** demonstrates eleven recipes: list in/out, wave, ripple, diagonal, spiral, checkerboard, burst in/out, gather, and loading dots. For the two list recipes, the existing target dropdown becomes a five-mode stagger-order selector.
- **Destinations** demonstrates anchored Arc, Bezier, Hop, Spring, Magnetic Snap, Waypoint Path, Spiral, and Multi-Hop motion between prefab-authored markers.
- **Feedback** demonstrates eleven semantic responses, including heal, shield block, critical hit, cooldown ready, level up, and low-health warning alongside the original five.
- **UI Sequences** demonstrates fifteen production transitions, including drawers, bottom sheets, page push, and page cross-fade alongside the original toast, modal, tooltip, dropdown, and tab families.
- **Text & Values** demonstrates thirteen operations, including Stagger Out, Character Bounce, Color Sweep, Glitch, Emphasis, and Scramble Reveal alongside the original seven.

Select a recipe, destination motion, feedback sequence, production UI sequence, text/value animation, or preset to preview it, change its relevant target or order, replay or reset it, and copy its typed fluent API example. The showcase controller only configures and instantiates authored UI prefabs from `Prefabs/UI`; it does not construct UI hierarchies at runtime.

World-scale movement presets use an explicit strength override in the 2D canvas preview so their motion remains visible in pixel space. The displayed and copied API example includes that override.

The demos do not require the Input System package. When the legacy Input Manager is enabled:

- In the 2D scene, use **Space** to replay the current selection. All browsing and filtering is available through the authored runtime UI.
- In the 3D scene, use **WASD** to move, **Q/E** to move vertically, hold the right mouse button to look, scroll to change speed, and hold **Shift** to move faster. Click an object to play it, press **R** to reset the last animation, or press **Shift+R** to reset all.

The public component and context-menu controls remain available when the legacy Input Manager is disabled.
