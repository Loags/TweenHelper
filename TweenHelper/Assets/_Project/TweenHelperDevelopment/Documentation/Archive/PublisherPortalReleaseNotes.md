# Tween Helper Publisher Portal Content

Internal publishing record; exclude it from the customer package.

Updated: 2026-08-19

Portal version: `1449210` — **currently `pendingReview`; replacement upload blocked until a draft exists**

Version `1.1.0` remains the intentional initial public release. The submitted candidate predates the current runtime, 446-entry Preset Browser, rebuilt 527-entry review scene, and documentation. On 2026-08-19 the authenticated Asset Store upload endpoint rejected its replacement with `No draft package version found to upload data to.` Do not withdraw the pending review without an explicit publisher decision; replace the artifact only after the Portal provides a draft version again.

Validated replacement artifact: `TweenHelper-1.1.0-2026-08-19-r2.unitypackage` (`719613` bytes, SHA-256 `9B09173460DE82F4C7DE17F34D647B268876CAA92C5E635786D93632A7119223`).

## Release settings

- Product: Tween Helper
- Publisher: Loags
- Category: Tools > Animation
- Version: `1.1.0`
- Regular price: $15 USD
- Launch discount: 50% for two weeks, subject to live Portal confirmation
- License: Standard Unity Asset Store EULA

## Version changes / release notes

```text
Tween Helper 1.1.0 — Initial public release

- Added a fluent, type-safe DOTween builder with sequencing, joined steps, callbacks, lifecycle control, async waiting, cancellation, and timeout handling.
- Added 300 registered presets with typed lookup and a dynamic-name fallback.
- Added semantic UI, staggered collection, destination, gameplay-feedback, production-UI, TextMesh Pro/value, progress, camera, and engine-property APIs.
- Added world-to-UI Arc, Hop, Bezier, Path, and pickup-collection projection for overlay and camera-space canvases.
- Added Image/Slider fill, value/text, drain, charge, alert, and progress-hook operations.
- Added gameplay-state feedback, expanded collection topologies, reusable sequence macros, camera helpers, and audio/light/particle/material-property animation.
- Added a 446-entry Preset Browser with isolated, purpose-built previews and copyable configuration-aware examples.
- Added a 406-entry mouse-driven Animation Gallery with all 300 presets, contextual controls, deterministic reset/replay, live C# examples, presentation mode, and dedicated world/camera fixtures.
- Added Setup & Support, DOTween validation, optional settings, offline feature guides, and the generated 300-preset catalog.
- Corrected UI preview composition, progress-fill visibility, engine-property meters, Torch Flicker presentation, and Hop-to-UI anticipation/landing timing.

Developed and tested with Unity 6000.5.2f1 and DOTween Free package 1.2.825 (runtime 1.3.030). Lower Unity versions and older DOTween versions have not been tested. DOTween is required, installed separately, and not included.
```

## Summary

```text
Build polished Unity motion with a fluent DOTween API, 300 presets, 146 semantic/component previews, and copyable examples.
```

## Description

```text
Important: DOTween is required, installed and licensed separately, and not included with Tween Helper. This release was developed and tested with DOTween Free package 1.2.825 (runtime 1.3.030). Older DOTween versions have not been tested.

Tween Helper turns repeated animation setup into a fluent, type-safe workflow for UI, transforms, sprites, rendered objects, TextMesh Pro, collections, progress values, feedback, cameras, audio, lights, particles, and material properties.

Start with any of 300 registered presets or compose motion step by step. Sequence animations, run steps together, configure duration, delay, easing, loops, strength, and related options, then retain one TweenHandle for replay, rewind, completion, cancellation, async waiting, timeout handling, or cleanup.

Highlights:

- 300 registered presets for entrances, exits, movement, scale, rotation, fades, attention effects, loops, and more.
- Semantic UI recipes for appear, disappear, hover, press, attention, enabled, and disabled states.
- Collection and stagger tools with list/grid timing, diagonal, spiral, checkerboard, concentric, quadrant, accordion, orbit, loading, burst, and gather patterns.
- Destination-aware Arc, Bezier, Hop, Spring, Magnetic Snap, waypoint Path, Spiral, and Multi-Hop motion.
- World-to-UI Arc, Hop, Bezier, Path, and pickup collection for overlay and camera-space canvases.
- Gameplay feedback for error, damage, success, reward, healing, defense, critical hits, cooldown, progression, low resources, charging, readiness, dodge, stun, buffs/debuffs, resource recovery, and objectives.
- Production UI sequences for toast, modal, tooltip, dropdown, tabs, drawers, bottom sheets, page transitions, and cutscene entrances.
- TextMesh Pro typewriter, numeric counting, character stagger/wave/bounce, color sweep, glitch, emphasis, score, and deterministic scramble effects.
- Image and Slider progress fills with synchronized percentage text, drain/charge accents, fixed-value alert pulses, and normalized progress hooks.
- Finite camera impact, recoil, landing, FOV, focus, breathing, rack-focus, and collection-kick feedback with exact state restoration.
- Audio volume/pitch, light intensity/color, particle emission, renderer float/color, Torch Flicker, and Scanner Pulse wrappers.
- Searchable Preset Browser with 446 isolated previews that never modify the active scene.
- Mouse-driven Animation Gallery with 406 entries, contextual controls, live C# calls, copy feedback, replay/reset, and capture-friendly presentation mode.
- Guided Setup & Support, DOTween validation, optional settings, and comprehensive offline documentation.

Use Tween Helper for menus, HUDs, notifications, buttons, cards, pickups, rewards, status effects, progress displays, world-to-screen collection, transitions, onboarding, and general game-feel polish in 2D and 3D projects.

Developed and tested with Unity 6000.5.2f1. Lower Unity versions have not been tested. Built-in Render Pipeline and URP are supported; HDRP and custom render pipelines have not been tested.
```

## Technical details

```text
Version: 1.1.0 (initial public release)
Developed and tested with: Unity 6000.5.2f1
Lower Unity versions: not tested
Required external dependency: DOTween (HOTween v2), installed and licensed separately
Validated DOTween package: 1.2.825
Validated DOTween runtime: 1.3.030
Older DOTween versions: not tested
Unity packages used: Unity UI (uGUI) and TextMesh Pro

Registered presets: 300
Preset Browser entries: 446 total — 300 presets plus 146 semantic/component previews
Animation Gallery entries: 406 total — 300 presets plus 106 curated examples
Internal review configurations: 527 — development validation only, not shipped or marketed as presets

Preset Browser categories:
- Presets: 300
- UI Recipes: 13
- Collections: 27
- Destination Motion: 20
- Gameplay Feedback: 27
- UI Sequences: 15
- TextMesh Pro: 13
- Progress Bars: 14
- Camera Feedback: 8
- Engine Properties: 9

Animation Gallery categories:
- Presets: 300
- UI Recipes: 13
- Collections: 19
- Destination Motion: 12
- Gameplay Feedback and Macros: 25
- UI Sequences: 16
- Text and Values: 13
- Camera Feedback: 8

Supported targets:
- GameObject and Transform
- RectTransform and CanvasGroup
- Unity UI Graphic, Image, and Slider
- TextMesh Pro text
- SpriteRenderer and Renderer material properties
- Collections with an explicit owner
- Camera, AudioSource, Light, and ParticleSystem

Workflow:
- Fluent typed builder and direct extension APIs
- Sequential and joined steps
- Per-step duration, delay, easing, loops, strength, scale, and related options
- TweenHandle playback, rewind, completion, kill, callbacks, async waiting, cancellation, and timeout handling
- Target-linked cleanup and documented endpoint/restoration behavior
- Optional global settings with built-in defaults

Editor tools:
- Setup & Support
- DOTween setup validator
- Searchable 446-entry Preset Browser with isolated preview fixtures
- Configuration-aware copyable API examples

Included sample:
- TweenHelperAnimationGallery.unity
- 406 entries across eight categories
- Mouse-only navigation; no Input System dependency
- 16:9 layout validated at 1920x1080
- Dedicated presentation, world-preview, and camera-feedback cameras

Render pipelines:
- Built-in Render Pipeline: supported
- Universal Render Pipeline: supported
- HDRP and custom pipelines: not tested

Runtime AI, MCP, telemetry, or online services: none
```

## Keywords

Use in priority order, subject to the Portal's current limit:

```text
DOTween
Tweening
UI Animation
Animation Presets
Game Feel
C# Animation
Sequence
Easing
TextMesh Pro
uGUI
Progress Bar
World To UI
Camera Shake
Editor Tool
Feedback
```

## Dependency field

```text
DOTween (HOTween v2) — https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676
```

```text
DOTween is required and must be installed, configured, and licensed separately. It is not included with Tween Helper. This release was validated with DOTween Free package 1.2.825 (runtime 1.3.030); older versions have not been tested.
```

## Compatibility field

```text
Developed and tested with Unity 6000.5.2f1. Lower Unity versions have not been tested. Built-in Render Pipeline and Universal Render Pipeline are supported. HDRP and custom render pipelines have not been tested.
```

## AI description

```text
OpenAI Codex and ChatGPT were used as AI-assisted development tools for selected functional code implementation and refactoring, documentation drafting, validation design, and consistency review. The publisher reviewed, edited, integrated, and tested the work against the intended product and release requirements. Tween Helper contains no runtime AI functionality, makes no AI or MCP service calls, and does not transmit customer, project, or user data to an AI system.
```

## Reviewer note

```text
Tween Helper 1.1.0 is the product's intentional initial public release. DOTween is a required external Asset Store dependency and is not included. Install DOTween and run Setup DOTween with its UI module before opening Tween Helper's Animation Gallery or Preset Browser. The package was developed and tested with Unity 6000.5.2f1 and DOTween package 1.2.825 (runtime 1.3.030); lower Unity versions and older DOTween versions are untested. Built-in and URP are supported; HDRP/custom pipelines are untested. The included gallery is mouse-driven and does not require the Input System. Unity Pipeline/MCP, developer telemetry, internal review tooling, tests, and Publisher media are repository-only and are not included in the customer package.
```

## Media manifest

Current branded candidates live outside the distributable `Assets` root under `TweenHelper/PublisherMedia`. Review every image against the final listing before reuse:

- `TweenHelper-Icon-160x160.png`
- `TweenHelper-Card-420x280.png`
- `TweenHelper-Cover-1950x1300.png`
- `TweenHelper-Marketing-16x9.png`
- source artwork for the icon and 3:2 key art

Retire the old `TweenHelper-2D-Showcase-1280x720.mp4` and 2D/3D showcase screenshots; they do not represent the 406-entry gallery or the 446-entry Preset Browser.

Required replacement capture set:

1. Gallery overview with category, result, and live C# call.
2. World-to-UI destination or pickup collection.
3. Collection topology with a visible contextual option.
4. Production UI sequence with correctly composed backdrop/incoming content.
5. Text/value or progress example with readable output.
6. Dedicated camera-feedback example.
7. Preset Browser showing isolated preview and search.
8. Preset Browser progress or engine-property meter.
9. Setup & Support dependency status.
10. Animation demonstration video covering the shipped feature families.
11. Optional longer caption-only setup/tutorial video.

Use Unity Recorder `5.1.7` and capture only from the accepted exact artifact. Confirm current Portal dimensions, codecs, file-size limits, and field rules before export.

## Submission status

- [x] Portal listing structure and current copy prepared.
- [x] DOTween dependency, compatibility, AI disclosure, and reviewer note rewritten.
- [x] Expanded catalog counts verified from live assemblies.
- [ ] Resolve the 11 current internal review entries marked Needs Work.
- [x] Run final tests and validators against the committed revision.
- [x] Export, inspect, and clean-import the replacement artifact.
- [ ] Capture final screenshots and the required animation demonstration video.
- [ ] Obtain a draft version, then replace the artifact and obsolete media for `1449210`.
- [ ] Preview the rendered listing and submit for review.

## Archived 1.0.0 draft

The unpublished 1.0.0 draft is superseded. Do not reuse its obsolete demo, preview, dependency, version, media, or feature-count claims.
