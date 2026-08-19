# Tween Helper 1.1.0 Release Roadmap

Status: **release candidate reopened after animation and preview expansion**

Updated: 2026-08-19

Branch: `release-v.1.1.0`

Tween Helper 1.1.0 remains the initial public release. The previously uploaded Publisher Portal draft is a historical candidate and must be replaced because the working tree now contains additional runtime animations, broader Preset Browser coverage, a rebuilt internal review scene, and corrected preview presentation.

## Release facts

| Fact | Current value |
| --- | --- |
| Product | Tween Helper `1.1.0` — initial public release |
| Unity validation | Unity `6000.5.2f1`; lower versions are untested |
| DOTween validation | Free package `1.2.825`, runtime `1.3.030`; installed separately |
| Distribution root | `Assets/Loags/TweenHelper` only |
| Registered presets | 300 |
| Preset Browser | 446 isolated preview entries |
| Animation Gallery | 406 customer-facing entries across eight categories |
| Internal review scene | 527 stable, unique review configurations |
| Render pipelines | Built-in and URP supported; HDRP/custom pipelines untested |
| License | Standard Unity Asset Store EULA |

Counts name different product surfaces and must not be combined into an unqualified “animation count.” Registered presets are `ITweenPreset` types. Browser entries and gallery entries also include semantic operations, recipes, target variants, and component-property examples. The 527-entry review scene is development-only.

## Delivered product surface

### Runtime

- Fluent typed and dynamic preset playback through `TweenBuilder` and `TweenHandle`.
- Sequential and joined animation composition, callbacks, loops, async waiting, cancellation, timeout handling, rewind, restart, and explicit kill behavior.
- UI recipes, staggered collections, destination motion, gameplay feedback, production UI sequences, TextMesh Pro/value animation, progress animation, camera feedback, and engine-property animation.
- World-to-UI projection through Arc, Hop, Bezier, Path, and pickup-collection workflows.
- Image/Slider progress operations: Fill To, Fill From/To, Value Fill, Drain, Charge, Alert Pulse, Fill + Text, and normalized progress hooks.
- Gameplay-state families, expanded collection topologies, reusable sequence macros, camera rack-focus/collection kick, audio/light/particle/material wrappers, Torch Flicker, and Scanner Pulse.
- Stable 300-entry registered preset catalog; semantic additions intentionally remain non-preset operations.

### Editor discovery

- **Tools > Tween Helper > Preset Browser** exposes 446 entries without reading or modifying the active scene.
- Preview fixtures build only the participants required by each operation: target, backdrop, controls, incoming content, progress bar, camera, audio/light/particle fixture, or renderer proxy.
- Proxy depth, sorting, Graphic color, parent `CanvasGroup` alpha, fill/value state, and projected UI state remain synchronized during playback.
- Details and copied C# examples use the same selected configuration.

### Samples and internal review

- `TweenHelperAnimationGallery.unity` is the only shipped demo scene and exposes 406 entries.
- `TweenHelperPresetReview.unity` is development-only and exposes 527 stable review IDs.
- The rebuilt review scene assigns renderable sprites to filled Images and meters, overlays percentage text on progress bars, and includes visible progress, audio, light, particle, material, world-to-UI, and macro fixtures.
- Hop-to-UI anticipation now precedes travel and landing squash begins only after the destination is reached.

## Validation snapshot

Completed for the current working tree:

- Preset Browser catalog: 446 entries, including all 300 registered presets and 146 semantic/component preview entries.
- Animation Gallery catalog: 406 entries across eight categories.
- Internal review catalog: 527 entries and 527 unique IDs.
- All Image progress previews were sampled at runtime; fill values, percentage text, scale/color pulses, and the 50% hook updated correctly.
- Preset Browser previews were exercised across the complete 446-entry catalog without compilation or runtime errors.
- Unity Play Mode exited cleanly and the Console reported zero errors after the latest review-scene validation.
- Runtime, demo, and validation-editor C# projects compiled with zero warnings and zero errors using isolated build output.

Historical validator, test, export, and upload results do not approve the new working tree. Repeat the exact-artifact gates below after the documentation and code are committed.

## Release gates

### 1. Freeze source and documentation

- [x] Keep version `1.1.0` as the initial public release.
- [x] Preserve exactly 300 registered presets.
- [x] Document the 446-entry Preset Browser, 406-entry gallery, and 527-entry internal review matrix accurately.
- [x] Keep Unity Pipeline/MCP, telemetry, review tooling, and Publisher records outside the distributable root.
- [ ] Commit and push the runtime, Editor, scene, package-tooling, and documentation changes.

### 2. Validate the final working tree

- [ ] Refresh Unity and confirm no package-originated compilation errors or warnings.
- [ ] Run EditMode and PlayMode suites.
- [ ] Run gallery, review-coverage, lifecycle, and relevant preview validators.
- [ ] Re-run a complete manual browser/review sample after the final import.
- [ ] Verify Built-in and URP presentation.

### 3. Build and inspect the exact artifact

- [ ] Run Asset Store Publishing Tools Validator against `Assets/Loags/TweenHelper` only.
- [ ] Export a replacement `TweenHelper-1.1.0.unitypackage`.
- [ ] Inspect the path manifest: no `_Project`, DOTween, `Packages`, Pipeline/MCP, Publisher media, tests, or development tooling.
- [ ] Import the exact artifact into clean supported projects with DOTween installed and configured separately.
- [ ] Open Setup & Support, Preset Browser, and the Animation Gallery from the imported artifact.
- [ ] Repeat core runtime, preview, documentation-link, and render-pipeline checks.

### 4. Refresh publishing content

- [x] Rewrite customer README, feature guides, changelog, gallery guide, and release facts for the expanded surface.
- [x] Rewrite Portal release notes, description, technical details, dependency disclosure, reviewer note, and AI disclosure.
- [ ] Capture replacement screenshots and the required animation demonstration video from the accepted exact artifact.
- [ ] Confirm current Portal dimensions, codecs, file-size limits, keyword limit, and discount controls at upload time.
- [ ] Replace the old draft artifact and obsolete media only after the replacements are accepted.
- [ ] Preview the rendered listing and submit for review.

## Asset Store compliance baseline

Recheck the official [Unity Asset Store Submission Guidelines](https://assetstore.unity.com/publishing/submission-guidelines) immediately before upload. The 2026-05-20 revision requires, among other applicable rules:

- one organized product root;
- comprehensive documentation for code/setup products;
- disclosure and correct Portal registration of external Asset Store dependencies;
- a demo scene for Animation-category submissions;
- a marketing video demonstrating included animations;
- no package-originated errors or warnings after setup;
- transparent functional AI-assistance disclosure when applicable.

The package must not include DOTween, Unity Pipeline/MCP integration, developer telemetry, Asset Store Publishing Tools, Recorder, internal review assets, or Publisher media.

## Publishing decision

Do not submit the currently uploaded draft. Replace it only after the new source, documentation, validation evidence, artifact manifest, clean-import results, and media all describe the same 1.1.0 release.
