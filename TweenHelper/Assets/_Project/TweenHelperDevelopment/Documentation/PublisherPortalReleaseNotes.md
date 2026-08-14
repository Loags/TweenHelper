# Tween Helper Publisher Portal content history

Internal record only. Do not include this file in the customer package. Keep the Portal text synchronized with the exact release artifact.

## 1.1.0 — initial public release

- Portal state: Ready for release-candidate validation; not yet submitted.
- Product name: Tween Helper
- Publisher: Loags
- Support: Info@Loags.de
- Category: Tools > Animation
- Regular price: $15 USD
- Launch promotion: maximum Portal discount, planned as 50% for two weeks. Reconfirm the allowed discount immediately before submission.
- License: Standard Unity Asset Store EULA

### Version

```text
1.1.0
```

### Release notes

```text
Tween Helper 1.1.0 — Initial public release

- Added a fluent, type-safe animation builder powered by DOTween.
- Added 300 registered presets for transform, UI, SpriteRenderer, Renderer, and compatible TextMesh Pro workflows.
- Added semantic UI, collection, destination-motion, gameplay-feedback, production-UI, text/value, and camera-feedback APIs.
- Added sequencing, joined steps, per-step options, callbacks, cancellation, timeout handling, and explicit lifecycle control through TweenHandle.
- Added a searchable Preset Browser with isolated previews and copyable code examples.
- Added a mouse-driven Animation Gallery with all 300 presets, contextual options, deterministic reset/replay, live C# examples, presentation mode, and dedicated world/camera fixtures.
- Added Setup & Support, DOTween validation, optional global settings, offline feature guides, and a generated preset catalog.

Tween Helper was developed and tested with Unity 6000.5.2f1 and DOTween Free package 1.2.825 (runtime 1.3.030). Lower Unity and older DOTween versions have not been tested. DOTween is installed separately and is not included.
```

### Summary

```text
Build polished Unity motion with a fluent DOTween API, 300 presets, semantic recipes, live previews, and copyable examples.
```

### Description

```text
Important: DOTween is required, installed separately, and not included with Tween Helper. This release was developed and tested with DOTween Free package 1.2.825 (runtime 1.3.030). Older DOTween versions have not been tested.

Tween Helper turns repeated animation setup into a fluent, type-safe workflow for UI, sprites, transforms, rendered objects, TextMesh Pro, collections, feedback, and cameras.

Start with any of 300 registered presets or compose motion step by step. Sequence animations, run steps together, adjust duration, delay, easing, loops, strength, and other options, then retain one TweenHandle for replay, rewind, completion, cancellation, or cleanup.

Highlights:

- 300 registered presets covering entrances, exits, movement, scale, rotation, fades, attention effects, loops, and more.
- 13 semantic UI recipes for appear, disappear, hover, press, attention, enabled, and disabled states.
- Collection and stagger tools with ordering, grid waves, ripples, diagonal, spiral, checkerboard, burst, gather, and loading-dot recipes.
- Destination-aware arc, Bezier, hop, spring, magnetic snap, waypoint path, spiral, and multi-hop motion.
- Gameplay feedback for error, damage, success, reward, heal, shield block, critical hit, cooldown, level up, low health, and pickup collection.
- Production UI sequences for toast, modal, tooltip, dropdown, tabs, drawers, bottom sheets, and page transitions.
- TextMesh Pro character effects, typewriter animation, numeric counting, score feedback, color sweep, glitch, emphasis, and deterministic scramble reveal.
- Finite camera impact, recoil, landing, FOV, focus-zoom, and breathing feedback with exact pose restoration.
- Searchable Preset Browser with isolated previews that do not modify the active scene.
- Mouse-driven Animation Gallery with search, family filters, contextual enums, replay/reset, live C# calls, copy feedback, and capture-friendly presentation mode.
- Guided Setup & Support, DOTween validation, optional settings, and offline documentation.

Use Tween Helper for menus, HUDs, notifications, buttons, cards, pickups, props, transitions, onboarding, rewards, and general game-feel polish in 2D and 3D projects.

Developed and tested with Unity 6000.5.2f1. Lower Unity versions have not been tested. Built-in Render Pipeline and URP are supported; HDRP and custom render pipelines have not been tested.
```

### Technical details

```text
Version: 1.1.0 (initial public release)
Developed and tested with: Unity 6000.5.2f1
Lower Unity versions: not tested
External dependency: DOTween (HOTween v2), installed and licensed separately
Validated DOTween package: 1.2.825
Validated DOTween runtime: 1.3.030
Older DOTween versions: not tested
Unity packages used: Unity UI (uGUI) and TextMesh Pro

Registered presets: 300
Semantic/group operations: 76 across UI, collections, destination motion, gameplay feedback, production UI, text/value, and camera feedback
Internal review configurations: 474 (development validation only; not marketed as presets)

Supported targets:
- GameObject and Transform
- RectTransform and CanvasGroup
- Unity UI Graphic components
- TextMesh Pro text
- SpriteRenderer
- Renderer materials
- Collections with an explicit owner
- Camera

Workflow:
- Fluent typed builder and direct extension APIs
- Sequential and joined steps
- Per-step duration, delay, easing, loops, strength, scale, and related options
- TweenHandle playback, rewind, completion, kill, callbacks, async waiting, cancellation, and timeout handling
- Target-linked cleanup and deterministic transient-state restoration
- Optional global settings with built-in defaults

Editor tools:
- Setup & Support
- DOTween setup validator
- Searchable Preset Browser with isolated preview stage
- Copyable API examples

Included sample:
- TweenHelperAnimationGallery.unity
- Mouse-only navigation; no Input System dependency
- Validated 16:9 layouts at 1920×1080 and 1280×720
- Dedicated presentation, world-preview, and camera-feedback cameras

Render pipelines:
- Built-in Render Pipeline: supported
- Universal Render Pipeline: supported
- HDRP and custom pipelines: not tested

Documentation:
- README and changelog
- Installation and compatibility
- API examples
- Focused collection, destination, feedback, UI, text/value, and camera guides
- Generated 300-preset catalog
- Third-party notice

Runtime AI or online services: none
Support: Info@Loags.de
```

### Keywords

Use in this priority order, subject to the Portal's current keyword limit:

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
Menu Animation
Camera Shake
2D Animation
3D Animation
Editor Tool
```

### Required dependency field

```text
DOTween (HOTween v2) — https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676
```

Additional dependency copy:

```text
DOTween is required and must be installed and licensed separately. It is not included with Tween Helper. This release was validated with DOTween Free package 1.2.825 (runtime 1.3.030); older versions have not been tested.
```

### AI/ML usage disclosure

```text
OpenAI Codex and ChatGPT were used as AI-assisted development tools for selected code implementation and refactoring, documentation drafting, test suggestions, validation, and consistency review. The publisher reviewed, edited, integrated, and tested the resulting work against the intended design and release requirements. Tween Helper contains no runtime AI features, makes no AI service calls, and does not transmit user or project data to an AI system.
```

### Reviewer note

```text
Tween Helper 1.1.0 is the product's initial public release; the version number is intentional. DOTween is a required external dependency and is not included. Please install DOTween and run its Setup DOTween step before opening the included Animation Gallery. The package was developed and tested with Unity 6000.5.2f1 and DOTween package 1.2.825 (runtime 1.3.030). Lower Unity and older DOTween versions are untested. Built-in and URP are supported; HDRP/custom pipelines are untested. The gallery is mouse-driven and contains the public demo experience; the former 2D and 3D draft scenes are not included.
```

### Media manifest — production intentionally pending

Retain the existing 1.0 key artwork. Do not upload the obsolete 2D/3D showcase screenshots or draft video because those scenes no longer ship.

Canonical retained source artwork:

```text
Assets/_Project/TweenHelperDevelopment/PublisherPortal/Branding/TweenHelperLogo.png
Assets/_Project/TweenHelperDevelopment/PublisherPortal/Branding/TweenHelperKeyVisual.png
Assets/_Project/TweenHelperDevelopment/PublisherPortal/Branding/TweenHelperFeatureOverview.png
Assets/_Project/TweenHelperDevelopment/PublisherPortal/Branding/TweenHelperSetupAndSupport.png
```

Future captures use Unity Recorder `5.1.7` from the accepted release-candidate gallery:

1. Gallery overview at 1920×1080.
2. All-presets search and family filtering.
3. Contextual direction/order option with the matching C# snippet.
4. Collection/grid recipe.
5. Text/value animation.
6. Dedicated camera-feedback preview.
7. Preset Browser isolated preview.
8. Setup & Support status screen.
9. Short caption-only marketing reel.
10. Longer caption-only setup and feature tutorial.

Confirm current Portal image/video dimensions and limits immediately before export. Store final delivery files beneath the repository-level `PublisherMedia` directory, never beneath `Assets/Loags/TweenHelper`.

## 1.0.0 — archived unpublished draft

- Portal state: Unpublished draft; superseded by 1.1.0.
- Prepared: 2026-08-09.
- Do not submit or reuse its obsolete dependency wording, 2D/3D demo claims, preview behavior, price discount, or media list.
