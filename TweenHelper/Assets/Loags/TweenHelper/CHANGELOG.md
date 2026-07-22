# Changelog

All notable changes to TweenHelper are documented in this file.

## [1.0.0] - 2026-07-22

### Added

- Type-safe preset builder and registry APIs with explicit dynamic-name fallbacks.
- Single-root Asset Store `.unitypackage` distribution layout and offline documentation.
- Searchable preset browser with family filters, selected-object preview, restoration, and copyable fluent API examples.
- DOTween setup validation and optional settings-asset workflow.
- Deterministic 300-preset catalog generation and catalog drift validation.
- Prefab-authored 2D recipe and preset showcase plus a full 3D preset showcase.

### Changed

- Built-in fluent methods, documentation, samples, and preset-browser examples now use concrete preset types instead of string literals.
- Raised the supported DOTween baseline to `1.3.030` and aligned the minimum Unity version with its Asset Store package at `2022.3.0f1`.
- Moved Editor commands under **Tools > TweenHelper** to comply with Asset Store menu guidelines.
- Added the MIT release license and expanded the external DOTween licensing and dependency notices.
- Replaced the UPM layout with `Assets/Loags/TweenHelper` for the standard Asset Store publishing workflow.
- Removed the sample assembly's Input System dependency and guarded optional legacy-input shortcuts.
- Builder options are captured per step and support the documented post-step fluent style.
- Completion and kill callback registration is additive.
- Repeated builder-level completion and kill registrations are additive.
- Await cancellation kills the active tween and disposes its cancellation registration.
- Missing `TweenHelperSettings` now uses documented defaults without reporting an initialization error.
- The 2D showcase scales world-unit movement presets for readable canvas previews and exposes the applied strength in its copyable API examples.

### Fixed

- Removed six builder shortcuts that referenced preset names which were never registered.
- Unity 6.5 obsolete API and serialization warnings in the demo tooling.
- External tween kills being reported as successful timeout-aware completion.
- Fade compatibility checks throwing when a target has no Renderer.
- Infinite loop presets continuing after the tween returned to the caller was killed.
