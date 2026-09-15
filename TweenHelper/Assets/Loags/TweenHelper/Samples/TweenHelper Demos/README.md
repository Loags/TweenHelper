# Tween Helper Animation Gallery

Open `Scenes/TweenHelperAnimationGallery.unity` and enter Play Mode. Import TextMesh Pro Essential Resources first.

The gallery is the package's single public demo. Its 423 entries provide mouse-driven access to:

- All 300 built-in registered presets, with search and family filters.
- 17 semantic UI recipes, including four asset-authored Tween Recipes.
- Twenty-three collection, stagger, and layout-transition examples.
- Twelve destination-motion operations, including world-to-UI projection.
- Twenty-five gameplay-feedback and reusable-macro examples.
- Sixteen production UI sequences.
- Twenty-two text and value examples, including representative generalized reveal, active-motion, transform, and ripple operations.
- Eight camera-feedback operations through a dedicated preview camera.

Selecting an animation resets its fixture and auto-plays it. Replay, Reset, Previous, and Next support repeatable comparison. Contextual controls expose relevant options such as direction, order, text unit, deterministic seed, glyph pivot, grid traversal, interpolation, target context, impact direction, and backdrop behavior. The C# panel updates from the same configuration and can copy the displayed call.

The scene is designed for 16:9 desktop and capture use and is validated at `1920×1080`. Presentation mode hides navigation and details when a clean preview is needed. The gallery does not require the Input System or keyboard/gamepad navigation.

## Tween Recipe samples

For direct component playback, add `Prefabs/DirectPresetDemo.prefab` or `Prefabs/CustomPresetDemo.prefab` to an existing scene. Both play on Start and expose their selected animation directly in Tween Player. `Scripts/CustomPulsePreset.cs` demonstrates automatic registration, a custom category, strength metadata, and preview opt-in. These examples reuse the Gallery unlit material. See [Tween Player](../../Documentation/TweenPlayer.md).

The `Recipes` folder contains seven asset-authored examples: PanelMoveFade, IconScalePreset, MultiBindingPopup, DelayedNotification, CollectionEntrance, RewardPresentation, and ProgressReward. The original four remain Gallery entries. The first demonstrates Then/With composition, the second combines direct parameters with the preset registry, the third uses three explicit bindings, and the fourth begins with a finite delay. Matching Gallery entries execute these assets through the same validated executor used by `TweenPlayer`.

The Built-in Render Pipeline and URP are supported. HDRP and custom render pipelines have not been tested.

The Editor Preset Browser is the broader component-preview surface. It contains 461 built-in entries plus custom registered presets, including fourteen progress-bar and nine engine-property examples that are intentionally not duplicated in this capture-focused gallery.
