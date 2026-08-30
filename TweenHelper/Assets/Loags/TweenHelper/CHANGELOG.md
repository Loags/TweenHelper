# Changelog

## Unreleased

Tween Helper remains in pre-release development. The entries below describe the current development baseline rather than a published version.

### Runtime and lifecycle

- Added a fluent, type-safe DOTween animation builder with explicit playback handles and lifecycle control.
- Added 300 registered presets with typed and dynamic lookup workflows.
- Added semantic UI, collection, destination-motion, gameplay-feedback, production-UI, TextMesh Pro, numeric-value, and camera-feedback APIs.
- Expanded TextMesh Pro with character/word/line reveals, shared deterministic ordering, finite wiggle/float/swing/pulse motion, scatter/rotate/shear/tracking transitions, and local-space impact ripple while preserving exact mesh and visibility lifecycle state.
- Replaced the unreleased character-specific stagger names with `TextStaggerIn` and `TextStaggerOut`; no legacy public aliases remain.
- Added world-to-UI Arc, Hop, Bezier, Path, and pickup-collection projection for overlay and camera-space canvases.
- Added normalized Image/Slider fill, value/text synchronization, drain, charge, alert pulse, and progress-hook operations.
- Added gameplay-state feedback, collection topology recipes, reusable sequence macros, camera rack-focus/collection kick, and engine-property wrappers for audio, lights, particle emission, and renderer properties.
- Consolidated advanced semantic timelines around consistent capture, completion, rewind, interruption, loop, target-link, and async/cancellation behavior.

### Editor tools and samples

- Added the searchable Preset Browser with 457 isolated previews and copyable configuration-aware examples.
- Added purpose-built preview fixtures for UI sequences, progress bars, cameras, audio, lights, particles, material properties, and projected world-to-UI content.
- Synchronized proxy sorting/depth, Graphic color, parent `CanvasGroup` alpha, fill/value state, and incoming/backdrop participants during Editor previews.
- Added Setup & Support, DOTween validation, and an optional settings asset.
- Added the mouse-driven Animation Gallery with 415 entries, contextual options, replay/reset navigation, live C# examples, presentation mode, and dedicated world/camera fixtures.

### Fixes and documentation

- Fixed Animation Gallery UI fixtures so fades consistently include child text, icons, and other grouped visuals.
- Fixed Hop-to-UI timing so anticipation finishes before travel and landing squash begins after arrival.
- Fixed Image fill and engine-meter review fixtures by assigning renderable filled sprites and placing synchronized percentage text over the bar.
- Improved audio/light preview meters, pitch normalization, Torch Flicker visibility, and alert-pulse descriptions.
- Added customer documentation for installation, API usage, every feature family, the 300-preset catalog, the 457-entry Preset Browser, and the 415-entry Animation Gallery.
