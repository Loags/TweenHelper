# Tween Helper Publisher Portal content history

This is an internal record of the exact listing and release copy prepared for the Unity Publisher Portal. It is outside `Assets/Loags/TweenHelper` and must not be included in the customer `.unitypackage`.

Keep releases in reverse chronological order. Preserve published entries unchanged and add each new release above the previous one.

## 1.0.0

- Portal status: Draft
- Prepared: 2026-08-09
- Publish only after the exact release artifact passes the final validation checklist.

### Release version

```text
1.0.0
```

### Changelog

```text
Tween Helper 1.0.0 - Initial release

- Added a fluent, type-safe animation builder powered by DOTween.
- Included 300 reusable animation presets for transform, UI, SpriteRenderer, and Renderer targets.
- Added sequencing and joining, per-step options, callbacks, cancellation, timeout handling, and deterministic reset behavior.
- Added a searchable preset browser with family filters, compatible-target previews, state restoration, and copyable code examples.
- Added 13 semantic UI animation helpers and a searchable library of 198 UI-compatible presets.
- Included prefab-authored 2D and 3D demonstration scenes.
- Added optional global settings and a DOTween setup validator.
- Added offline installation instructions, API examples, lifecycle guidance, and a generated preset catalog.

Requires DOTween 1.3.030 or newer, installed separately.
```

### Summary (124 characters)

```text
Create polished 2D, 3D, and UI animations faster with a fluent workflow, 300 ready-to-use presets, and customizable results.
```

### Description

```text
Important: Tween Helper requires DOTween 1.3.030 or newer, installed separately. DOTween is not included.

Bring your UI, sprites, and 3D objects to life without rebuilding the same animation sequences for every project.

Tween Helper gives you 300 ready-to-use presets and a clear, flexible workflow for creating polished motion with DOTween. Choose an effect, adjust its timing and feel, combine it with other steps, and reuse it throughout your project.

Highlights:

- 300 presets covering entrances, exits, movement, scale, rotation, fades, attention effects, loops, and more.
- Works with UI elements, TextMesh Pro, sprites, transforms, and rendered 3D objects.
- Search and preview presets directly in the Editor, then restore the target with one click.
- Combine animations in sequence or play them together.
- Customize duration, easing, delay, loops, strength, scale, and other per-animation options.
- Use 13 semantic UI helpers for common interactions such as appear, disappear, hover, press, attention, enabled, and disabled states.
- Learn quickly with included 2D and 3D demos, copyable examples, and offline documentation.
- Use optional global settings, or start immediately with the built-in defaults.

Tween Helper is suitable for menus, HUDs, notifications, cards, buttons, pickups, props, scene transitions, and general game-feel polish in both 2D and 3D projects.
```

### Technical details

Verify the Unity-version statement against the exact uploaded artifact before publishing.

```text
Version: 1.0.0
Unity version: 2022.3.0f1 or newer
Required dependency: DOTween 1.3.030 or newer, installed separately
Unity packages used: Unity UI (uGUI) and TextMesh Pro

Included presets: 300
Semantic UI helpers: 13
UI-compatible presets demonstrated in the 2D showcase: 198

Supported targets:
- GameObject and Transform
- RectTransform and CanvasGroup
- Unity UI Graphic components
- TextMesh Pro text
- SpriteRenderer
- Renderer materials

Animation workflow:
- Fluent, type-safe builder API
- Sequential and joined animation steps
- Per-step duration, delay, easing, loops, strength, scale, and related options
- Callbacks, cancellation, timeout handling, and target-linked cleanup
- Optional global settings with built-in defaults when no settings asset is present

Editor tools:
- Searchable Preset Browser with family filters
- Compatible-target previews with state restoration
- Copyable code examples
- DOTween setup validator

Render-pipeline compatibility:
- Runtime code and 2D demo are render-pipeline independent
- Included 3D demo materials use the Universal Render Pipeline (URP)

Input:
- Unity Input System package is not required
- Optional demo keyboard shortcuts use the legacy Input Manager when enabled

Included samples: TweenHelperDemo2D and TweenHelperDemo3D
Documentation: Offline installation guide, API examples, lifecycle guidance, and generated preset catalog
Runtime AI or online services: None
```

### Category

```text
Tools
```

If the portal requests a narrower subcategory, use `Tools > Animation` when available.

### Price

```text
$15.00 USD
```

Recommended launch discount: 30% for two weeks, giving a launch price of $10.50 USD. Keep the regular price at $15.00 while the package establishes reviews and sales history, then reassess after meaningful customer feedback and feature updates.

### Keywords

Enter these 15 keywords in priority order:

```text
DOTween
Tweening
UI Animation
GUI Animation
Animation Presets
2D Animation
3D Animation
Editor Tool
C# Animation
Sequence
Easing
TextMesh Pro
uGUI
Menu Animation
Game Feel
```

### AI/ML usage disclosure

```text
OpenAI Codex and ChatGPT were used as AI-assisted development tools. They helped draft and refine documentation, suggest tests, perform validation and consistency checks, and assist with selected code implementation, review, and refactoring. All suggested code and text were reviewed, edited, integrated, and tested by the publisher against the intended design and package requirements. Tween Helper has no runtime AI features, makes no AI service calls, and does not send user or project data to an AI system.
```
