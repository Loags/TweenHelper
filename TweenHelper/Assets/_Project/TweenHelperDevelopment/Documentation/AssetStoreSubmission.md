# Tween Helper Asset Store submission notes

Tween Helper uses the standard Asset Store package workflow. Select only `Assets/Loags/TweenHelper` for validation, export, and upload. The publisher name is **Loags**. The support address is used only by the in-Editor Setup & Support workflow and is not copied into Portal fields or customer documentation.

## Release identity

- Version: `1.1.0`, initial public release.
- Category: Tools > Animation.
- Regular price: $15 USD.
- Planned launch promotion: maximum available discount, currently 50% for two weeks; reconfirm in Portal.
- Tween Helper license: Standard Unity Asset Store EULA. Do not claim MIT licensing.
- Developed and tested with Unity `6000.5.2f1`; lower Unity versions are untested.
- Validated with DOTween package `1.2.825` / runtime `1.3.030`; older DOTween versions are untested.
- Built-in and URP are supported; HDRP/custom pipelines are untested.

## Dependency

Register **DOTween (HOTween v2)** as a required Asset Store dependency. DOTween is installed and licensed separately and is not redistributed with Tween Helper.

```text
https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676
```

## Package boundary

The upload must include the complete `Assets/Loags/TweenHelper` root and nothing outside it. In particular, exclude:

- `Assets/Plugins/Demigiant` and all DOTween files.
- `Assets/_Project/TweenHelperDevelopment` tests, review scene, builders, roadmaps, Portal records, and validation tools.
- Repository-level `PublisherMedia` artwork/captures.
- `Packages`, `ProjectSettings`, `Library`, `Temp`, Asset Store Publishing Tools, Recorder, and Unity Pipeline tooling.

## Validation and upload sequence

1. Confirm Unity is stopped, the gallery scene is saved, and the Console has no package-originated errors.
2. Run **Tools > Tween Helper Dev > Validate Animation Gallery Assets**.
3. Run the existing EditMode and PlayMode validation suites.
4. Run Asset Store Publishing Tools Validator against only `Assets/Loags/TweenHelper`.
5. Export the exact artifact from that root and inspect its content list.
6. Import the artifact into a clean Unity project with the separately installed validated DOTween build.
7. Open Setup & Support, Preset Browser, and `TweenHelperAnimationGallery.unity`.
8. Exercise mouse selection, search/filter, contextual options, auto-play, Replay, Reset, Previous/Next, code copy, presentation mode, world preview, and camera preview at 1920×1080.
9. Verify the Built-in and URP presentation in clean projects.
10. Confirm documentation links, changelog, Standard EULA wording, and the separately installed DOTween dependency statement.
11. Copy the exact 1.1.0 fields from `PublisherPortalReleaseNotes.md`, preview the rendered listing, and upload the exact tested artifact.

## Release checklist

- [x] One customer package root: `Assets/Loags/TweenHelper`.
- [x] Version and setup UI updated to `1.1.0`.
- [x] Gallery is the only shipped demo and only scene in development Build Settings.
- [x] Legacy 2D/3D scenes and scene-only setup/console code removed.
- [x] Customer README, installation guide, sample guide, and changelog updated; no third-party notice is shipped because the package redistributes no third-party content requiring attribution.
- [x] Portal description, technical details, dependency, AI disclosure, reviewer note, keywords, and media brief updated.
- [x] DOTween files, internal development assets, Portal media, and Recorder remain outside the upload root.
- [x] Run gallery asset validator after the final documentation import (377 entries across eight categories).
- [x] Run EditMode and PlayMode suites against the final working tree (25/25 EditMode, 8/8 PlayMode).
- [ ] Run Asset Store Validator against the exact root.
- [ ] Export and inspect the exact `.unitypackage`.
- [ ] Clean-import and validate the exact artifact.
- [ ] Capture replacement screenshots and both caption-only videos with Recorder `5.1.7`.
- [ ] Reconfirm Portal media rules and maximum launch discount.
- [ ] Preview and proofread the final Portal draft.
- [ ] Submit for review.

## AI disclosure record

Codex and ChatGPT assisted selected functional code, refactoring, documentation, validation, and consistency review. The publisher reviews and tests all integrated work. There is no runtime AI, online AI call, or project/user data transmission.
