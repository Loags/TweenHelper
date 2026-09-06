# Tween Helper development assets

This folder is repository-only. Never include it in the Asset Store `.unitypackage`; the distributable root is `Assets/Loags/TweenHelper` only.

## Contents

- `Tests` contains EditMode and PlayMode validation assemblies.
- `Validation` contains gallery audits, preset integrity tools, lifecycle/coverage validators, and the internal review scene.
- `Documentation` contains the current internal release record. Completed implementation roadmaps and superseded release drafts are removed after their facts are folded into customer and release documentation.
- `CLI` contains the development-only Unity Pipeline command adapter and local telemetry documentation.
- Publisher Portal source records and branding remain development-only.

## Current catalog surfaces

| Surface | Count | Purpose |
| --- | ---: | --- |
| Registered preset registry | 300 | Stable customer preset API |
| Preset Browser | 461 | Customer Editor discovery and isolated previews |
| Animation Gallery | 423 | Shipped capture-friendly runtime examples |
| Preset Review scene | 610 | Exhaustive development-only visual configurations |

The review catalog contains 610 unique IDs:

- 300 presets
- 13 UI recipes
- 48 collection recipes and layout-transition configurations
- 10 stagger variants
- 30 destination-motion configurations
- 23 feedback sequences
- 10 gameplay-state configurations
- 39 production UI sequences
- 100 text/value configurations
- 15 progress configurations
- 4 sequence macros
- 9 camera-feedback configurations
- 9 engine-property configurations

## Validation entry points

- **Tools > Tween Helper Dev > Validate Animation Gallery Assets** verifies the shipped gallery scene, catalog, and required assets.
- **Tools > Tween Helper Dev > Validate Animation Review Coverage** verifies review identity, fixtures, variants, and smoke playback without changing manual statuses.
- **Tools > Tween Helper Dev > Validate Animation Lifecycle Refactor** checks lazy capture, completion, interruption, rewind, Yoyo, cleanup, and spatial playback behavior.
- `Validation/Scenes/TweenHelperPresetReview.unity` is the manual visual acceptance surface.

The review scene was rebuilt at scene version 3 so filled Images and property meters use a visible UI sprite, progress text overlays the bar, and dedicated layout-list/layout-grid fixtures reset exactly alongside the expanded world-to-UI/progress/engine fixtures.

## Development state

Tween Helper 1.2.0 is the current release target. The repository root `ROADMAP.md` records the delivered scope and explicitly deferred work; `Documentation/Release-1.2.0.md` is the canonical internal publishing record. Git history retains completed implementation plans and superseded release drafts.
