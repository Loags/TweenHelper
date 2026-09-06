# Tween Helper 1.3.0-rc.1

Status: locally verified release candidate; not submitted or published.

Source commit: `f3711bd705c987f41ec769086d0fdad21eb2a1ce` (implementation, customer documentation, candidate archive and validation/media tooling). This follow-up record changes release metadata only.

Artifact: [TweenHelper-1.3.0-rc.1.unitypackage](TweenHelper-1.3.0-rc.1.unitypackage), 794,160 bytes, 198 paths/GUIDs.

SHA-256: `8a10b496d2881a9f5ad72d5bed6b922b88416addf68b4c8db0f7fc6b9e4a934a`.

Historical 1.2.0 remains unchanged. Its publication state is unverified; a separate prerelease avoids replacing that artifact. Preserving host-engine configuration is an intentional behavior correction with an explicit opt-in migration path. Public playback signatures remain compatible. Review the public release version before submission.

## Validation

See [implementation evidence](../Assets/_Project/TweenHelperDevelopment/Documentation/PublishingImplementationEvidence.md) and [archive report](RoadmapValidation/Artifact.json). 34 Edit Mode and 19 Play Mode tests passed. Windows Mono and IL2CPP High builds and smoke checks passed, including string-only custom-preset preservation. Clean import, a real 1.2.0 update, two sessions without domain reload, reference/GUID audits and existing runtime validators passed. Asset Store Tools: 35 passes, one static-variable warning, zero failures.

The final archive differs from the player-tested payload only in customer documentation and normalized C# line endings. Runtime code and serialized behavior are unchanged. Final import/source checks verify the archive itself. Media uses the same runtime/Gallery assets; final documentation and line-ending corrections do not affect the visuals.

## Prepared listing copy

**Title:** Tween Helper — Fluent Animation & Reusable Recipes

**Summary:** Build reusable Unity motion with a fluent DOTween API, 300 presets, visual Tween Recipes, isolated previews and practical UI, collection, text and camera workflows.

**Description:** Create animations in code or author reusable recipes with explicit scene bindings. Browse 461 isolated examples, filter by use case, save favorites and copy matching calls. Explore the 423-entry Gallery, then adapt complete quick-start examples to your own objects. Tween Helper owns its handles and preserves the host's DOTween settings by default. Reduced-motion options adjust supported decorative effects while preserving semantic destinations and lifecycle behavior.

**Included:** 300 presets; fluent Then/With sequencing; 28 recipe operations and seven recipe assets; progress/reward prefab with a preference toggle; Browser search, filters, favorites and recent entries; Gallery; setup/support tools; lifecycle, migration and performance guides.

**Dependencies:** DOTween Free is required, separately installed/licensed, and not bundled. Run DOTween Setup with UI support. Unity UI and TextMesh Pro are required; import TMP Essential Resources for samples. Configure the actual DOTween dependency in the verified Portal draft.

**Verified configuration:** Unity 6000.5.2f1; DOTween package 1.2.825 / runtime 1.3.030; Windows x64 Mono and IL2CPP High with Built-in. URP 17.5.0 Gallery/lifecycle behavior checked in the development Editor. Other versions/platforms/pipelines are untested for this candidate.

**Release notes:** Fix async winner handling, retained completion, cancellation ownership and Editor cleanup. Preserve host globals by default. Explicitly register built-ins for stripping. Add discovery preferences, reduced motion, Progress Fill To and three workflow samples. Read migration guidance if raw DOTween calls relied on the previous global-configuration behavior.

**Support:** Info@Loags.de. Setup & Support prepares an editable email and never sends automatically.

**Limitations:** Reduced motion covers documented strength-aware families and does not claim universal accessibility conformance. Headless results are CPU measurements, not FPS or zero-allocation guarantees. Customer presets referenced only by name need explicit preservation or registration. UI Toolkit/Timeline/Cinemachine/UniTask integrations are outside this release.

**AI disclosure draft:** AI assistance was used for implementation, documentation and validation tooling. Review the provenance of reused branding/key art separately before completing the Portal disclosure; this record does not assert an unverified origin for historical artwork.

## Publishing material

[Media manifest](../../PublisherMedia/README.md), [captioned reel](../../PublisherMedia/MarketingReel/TweenHelper-1.3.0-rc.1-Reel.mp4), and [written first-animation tutorial](../../PublisherMedia/Tutorial/FirstAnimation.md).

Native Browser/Setup/Inspector screenshots and live Portal checks remain blocked by the recorded computer-use failures. Upload/submission requires the final publishing decision for this exact candidate.