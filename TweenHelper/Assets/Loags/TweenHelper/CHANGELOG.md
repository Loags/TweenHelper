# Changelog

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

[1.2.0]: https://publisher.unity.com/packages/1453516/edit/upload
