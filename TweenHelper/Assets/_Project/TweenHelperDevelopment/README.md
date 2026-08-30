# Tween Helper development assets

This folder is repository-only. Never include it in the Asset Store `.unitypackage`; the distributable root is `Assets/Loags/TweenHelper` only.

## Contents

- `Tests` contains EditMode and PlayMode validation assemblies.
- `Validation` contains gallery audits, preset integrity tools, lifecycle/coverage validators, and the internal review scene.
- `Documentation` contains active feature roadmaps and a clearly separated archive of superseded plans.
- `CLI` contains the development-only Unity Pipeline command adapter and local telemetry documentation.
- Publisher Portal source records and branding remain development-only.

## Current catalog surfaces

| Surface | Count | Purpose |
| --- | ---: | --- |
| Registered preset registry | 300 | Stable customer preset API |
| Preset Browser | 460 | Customer Editor discovery and isolated previews |
| Animation Gallery | 418 | Shipped capture-friendly runtime examples |
| Preset Review scene | 608 | Exhaustive development-only visual configurations |

The review catalog contains 608 unique IDs:

- 300 presets
- 13 UI recipes
- 46 collection recipes
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

The review scene was rebuilt at scene version 2 so filled Images and property meters use a visible UI sprite, progress text overlays the bar, and the expanded world-to-UI/progress/engine fixtures are testable. A 2026-08-19 Play Mode sample confirmed all Image progress values and text update correctly; the Unity Console reported zero errors afterward.

## Development state

Tween Helper has not had a public release. Current catalog counts and validation snapshots are development baselines and may change as active roadmaps are implemented. Use the repository root `ROADMAP.md` for active plans; files under `Documentation/Archive` are historical only.
