# Changelog

## [1.3.0-rc.2] - 2026-09-15

### Added

- Direct Tween Player preset mode with category and searchable animation dropdowns, same-object targeting, optional external targets, duration overrides, and declared preset-specific controls.
- Optional custom category, override-capability, and preview-support attributes; custom animations appear through the existing preset registry.
- Direct and custom preset sample prefabs and a documented custom preset implementation.

### Changed

- Newly added players default to Preset mode; existing serialized players retain Recipe mode and their references/events.
- Recipe target slots synchronize automatically while preserving assignments by binding ID. Removed bindings remain available for explicit cleanup.
- Preset Browser discovery preserves explicit registry registrations.
- Player preview supports direct presets and requires custom presets to opt into supported state restoration.

### Documentation

- Make Inspector animation selection the primary quick-start workflow; document extension metadata, preview boundaries, migration, multi-selection, and automatic binding synchronization.

This is an update draft candidate. Validation evidence and remaining platform limits are recorded with the release artifact.

## [1.3.0-rc.1] - 2026-09-06

### Added

- Browser use-case filters, favorites, a bounded recent-entry list, and filter reset.
- Package, per-call and TweenPlayer motion preferences, including reduced decorative amplitude and orientation-preserving spin alternatives.
- Progress Fill To recipe operation, three complete workflow assets and an authored progress/reward prefab with a motion preference toggle.

### Fixed

- Observe the winning task in `AwaitAny`, detach losing observers, and settle waits for retained completed tweens.
- Dispatch cancellation-triggered tween mutations to the captured Unity synchronization context and release pending observers during teardown.
- Preserve host-global DOTween configuration by default; add an explicit settings opt-in and stop clearing the shared engine at application quit.
- Return paused tweens from builder `Build()` and apply package playback defaults without relying on host-global autoplay/autokill settings.
- Add explicit constructor registrations for all 300 built-in presets to improve linker visibility.
- Add `TweenPlayer.PlayFromEvent()` for persistent Inspector event wiring.
- Release owned handles, canceled waits and Browser previews in Edit Mode without killing unrelated tweens that share their target or ID.

### Documentation

- Add complete quick-start examples, lifecycle contracts, troubleshooting, and configuration/custom-preset migration guidance.

This is a release candidate. Consult the installation guide for the supported configuration and limitations.

## [1.2.0] - 2026-08-30

### Added

- Expanded TextMesh Pro animation with shared character, word, and line element mapping; ordered stagger, scatter, rotate, shear, tracking, impact-ripple, wiggle, float, swing, pulse, and deterministic scramble workflows.
- Added builder and direct-extension parity for the expanded text operations, with one mesh-state owner per label and exact mesh/visibility restoration across completion, rewind, interruption, restart, kill, and Yoyo loops.
- Added collection Deal In/Out, reusable eight-direction grid serpentine ordering, and layout-difference transitions captured around caller-owned layout changes.
- Added reusable Tween Recipe assets, explicit TweenPlayer bindings, a constrained Then/With editor, safe scene/Prefab Mode preview, validation navigation, and a curated 27-operation runtime catalog.

### Changed

- Replaced the unreleased character-specific stagger names with the generalized `TextStaggerIn` and `TextStaggerOut` API; no legacy public aliases remain.
- Expanded the searchable Preset Browser to 461 isolated entries and the shipped Animation Gallery to 423 customer-facing entries while keeping the registered preset catalog at 300.
- Expanded the development-only review surface to 610 unique configurations, including exhaustive text, collection, layout-transition, and recipe coverage.
- Updated setup, API, text/value, collection, recipe, catalog, gallery, browser, and release documentation for the 1.2.0 surface.

### Fixed

- Aligned Text Float phases across multiline TMP content.
- Made character, word, and line stagger/scatter previews visibly distinct and corrected outgoing line transitions so they operate one line at a time.
- Made grouped TMP transform previews use their intended operation rather than an unrelated animation.
- Preserved rich text, whitespace, line breaks, invisible glyphs, multi-material text, TextMeshProUGUI, and world-space TextMeshPro across the expanded mesh effects.

## [1.1.0] - 2026-08-19

### Added

- Introduced the fluent, type-safe DOTween builder, explicit TweenHandle lifecycle, sequencing, joined steps, callbacks, loops, async waiting, cancellation, timeout, rewind, restart, and kill behavior.
- Added 300 registered presets plus semantic UI, collection, destination, gameplay-feedback, production-UI, TextMesh Pro/value, progress, camera, and engine-property APIs.
- Added Setup & Support, DOTween validation, the Preset Browser, the mouse-driven Animation Gallery, focused customer guides, and the generated preset catalog.

[1.2.0]: #120---2026-08-30
