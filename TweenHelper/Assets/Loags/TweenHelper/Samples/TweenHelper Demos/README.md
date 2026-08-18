# Tween Helper Animation Gallery

Open `Scenes/TweenHelperAnimationGallery.unity` and enter Play Mode. Import TextMesh Pro Essential Resources first.

The gallery is the package's single public demo. It provides mouse-driven access to:

- All 300 registered presets, with search and family filters.
- 13 semantic UI recipes.
- Eleven collection and stagger recipes.
- Eight destination-motion operations.
- Eleven gameplay-feedback sequences.
- Fifteen production UI sequences.
- Thirteen text and value examples.
- Six camera-feedback operations through a dedicated preview camera.

Selecting an animation resets its fixture and auto-plays it. Replay, Reset, Previous, and Next support repeatable comparison. Contextual controls expose relevant options such as direction, order, grid traversal, interpolation, target context, impact direction, and backdrop behavior. The C# panel updates from the same configuration and can copy the displayed call.

The scene is designed for 16:9 desktop and capture use and is validated at `1920×1080`. Presentation mode hides navigation and details when a clean preview is needed. The gallery does not require the Input System or keyboard/gamepad navigation.

The Built-in Render Pipeline and URP are supported. HDRP and custom render pipelines have not been tested.
