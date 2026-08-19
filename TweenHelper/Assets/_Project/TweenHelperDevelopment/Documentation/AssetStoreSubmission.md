# Tween Helper Asset Store Submission Runbook

Status: **validated replacement candidate ready; Portal version is pending review**

Updated: 2026-08-19

Publisher Portal version `1449210` is currently `pendingReview` with an older 1.1.0 artifact. The runtime, Preset Browser, internal review scene, Unity Pipeline development package, and documentation changed after that upload. The authenticated upload endpoint rejected the validated replacement on 2026-08-19 because no draft package version exists. Do not withdraw the pending review without an explicit publisher decision; upload the replacement only after the Portal exposes a draft again.

Current validated candidate: `TweenHelper-1.1.0-2026-08-19-r2.unitypackage` (`719613` bytes, SHA-256 `9B09173460DE82F4C7DE17F34D647B268876CAA92C5E635786D93632A7119223`).

## Release identity

| Field | Value |
| --- | --- |
| Product | Tween Helper |
| Version | `1.1.0`, initial public release |
| Publisher | Loags |
| Category | Tools > Animation |
| Regular price | $15 USD |
| Planned launch discount | 50% for two weeks; reconfirm in Portal |
| License | Standard Unity Asset Store EULA |
| Unity validation | `6000.5.2f1`; lower versions untested |
| DOTween validation | Package `1.2.825`, runtime `1.3.030`; older versions untested |
| Pipelines | Built-in and URP supported; HDRP/custom pipelines untested |

## Current catalog facts

- 300 registered presets.
- 446 Preset Browser entries: 300 presets plus 146 semantic/component previews.
- 406 shipped Animation Gallery entries across eight categories.
- 527 development-only review configurations with stable, unique IDs.

Never market the 527 review configurations as presets. When a number appears in Portal copy, name the surface it counts.

## Required dependency

Register [DOTween (HOTween v2)](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676) as a required Asset Store dependency. DOTween is acquired, installed, configured, and licensed separately; it is not redistributed with Tween Helper.

## Package boundary

Validate, export, and upload `Assets/Loags/TweenHelper` only. Exclude:

- `Assets/Plugins/Demigiant` and every DOTween file.
- `Assets/_Project/TweenHelperDevelopment`, including tests, review scenes, roadmaps, Portal records, CLI adapters, and validation tools.
- `Packages`, `ProjectSettings`, `Library`, `Temp`, `Logs`, and `UserSettings`.
- Unity Pipeline/MCP, telemetry, Asset Store Publishing Tools, Recorder, and their package data.
- Repository or project-root `PublisherMedia` files.
- Generated IDE projects and local build output.

## Current Unity guideline checks

Recheck the official [Asset Store Submission Guidelines](https://assetstore.unity.com/publishing/submission-guidelines) before upload. The revision dated 2026-05-20 requires applicable submissions to provide one organized root, comprehensive code/setup documentation, disclosed dependencies, a demo scene for Animation-category content, a marketing video demonstrating included animations, and no package-originated warnings/errors after setup. Functional AI-assisted content must be disclosed in the Portal AI field.

Because the artifact is produced with Unity `6000.5.2f1` (Unity 6.5), verify the exact artifact in URP before submission; the current guideline requires Unity 6.5-or-newer submissions to support URP or HDRP.

Treat Portal dimensions, codecs, file-size limits, keyword limits, accepted Editor versions, render-pipeline rules, and discount controls as volatile. Confirm them in the live Portal instead of copying an old limit into the package.

## Replacement artifact sequence

1. Commit and push the final source/documentation revision.
2. Refresh Unity `6000.5.2f1`; confirm compilation and a clean Console after DOTween setup.
3. Run EditMode and PlayMode suites.
4. Run gallery, review-coverage, lifecycle, and relevant Preset Browser validators.
5. Exercise representative world-to-UI, progress, macro, camera, audio, light, particle, renderer, and UI-sequence previews.
6. Verify Built-in and URP presentation.
7. Run Asset Store Publishing Tools Validator against `Assets/Loags/TweenHelper` only.
8. Export `TweenHelper-1.1.0.unitypackage` from that exact root.
9. Inspect the artifact path manifest and compare it to the committed source revision.
10. Import the exact artifact into clean supported projects with DOTween installed separately and **Setup DOTween** completed.
11. Open Setup & Support, the 446-entry Preset Browser, and `TweenHelperAnimationGallery.unity`.
12. Verify all shipped documentation links and copyable examples from the imported artifact.
13. Capture final marketing media from the accepted artifact.
14. Update Portal fields from `PublisherPortalReleaseNotes.md`, register DOTween, preview the listing, and upload the replacement artifact.
15. Submit only after the Portal manifest, media, description, dependency, AI disclosure, and reviewer note match the exact artifact.

## Final checklist

- [x] One customer package root: `Assets/Loags/TweenHelper`.
- [x] Version, setup UI, and documentation identify `1.1.0` as the initial public release.
- [x] The Animation Gallery is the only shipped demo scene.
- [x] Customer documentation covers installation, core API, all shipped families, lifecycle, browser, gallery, and licensing.
- [x] Portal copy and reviewer notes have been rewritten for the expanded runtime and preview surface.
- [x] DOTween dependency and separate licensing are disclosed.
- [x] Unity Pipeline/MCP and local developer telemetry remain outside the customer package.
- [ ] Resolve the 11 current review entries marked Needs Work.
- [x] Run final tests and validators against the committed revision.
- [ ] Validate Built-in and URP with the exact artifact.
- [x] Export, inspect, and clean-import the replacement artifact with DOTween installed separately.
- [ ] Capture replacement screenshots and the required animation demonstration video.
- [ ] Reconfirm current Portal rules and launch discount controls.
- [ ] Obtain a draft version, replace `1449210`, preview the rendered listing, and submit.

## AI disclosure record

OpenAI Codex and ChatGPT assisted selected functional code, refactoring, documentation, validation design, and consistency review. The publisher reviews, integrates, and tests the work. Tween Helper contains no runtime AI, MCP connection, online AI call, or customer/project data transmission.
