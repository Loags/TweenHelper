# Tween Helper documentation, demo, and Publisher Portal roadmap

- Status: **IMPLEMENTED — FINAL VALIDATION, MEDIA, AND PORTAL SUBMISSION PENDING**
- Prepared: 2026-08-14
- Working branch: `release-v.1.1.0`
- Comparison base: `main` at `023e84e9bbea2a21911b8ea0868464c6f18641f6`
- Branch head at audit: `899ae6487d2bb18e542e3b15022d3aeb178ffc3e`
- Scope: customer documentation, shipped demos, Publisher Portal copy, and later marketing-media production

## Implementation update — 2026-08-14

Completed in the working tree:

- Added `TweenHelperAnimationGallery.unity` as the only shipped demo and the only scene in Build Settings.
- Added eight mouse-driven categories: all 300 presets, 13 UI recipes, eleven collection recipes, eight destination operations, eleven gameplay-feedback operations, fifteen UI sequences, thirteen text/value examples, and six camera-feedback operations.
- Added search, preset-family filtering, per-category session memory, contextual enum/value controls, reset-before-auto-play, Replay/Reset/Previous/Next, live minimal C# snippets, copy feedback, target/API badges, and presentation mode.
- Added dedicated presentation, world-preview, and camera-feedback cameras. Camera feedback never moves the gallery UI camera.
- Validated Play Mode startup with no package errors and visually checked the 16:9 layout at 1920×1080 and 1280×720.
- Removed the retired 2D/3D scenes, their prefabs/materials, their keyboard/fly-camera/reset/spawner code, and the animation-selection console after a GUID/reference audit.
- Preserved internal review capability by moving its compatibility policy under `_Project` and repurposing legacy audit/build tooling for gallery validation.
- Rewrote the shipped README, installation guide, sample guide, and stale demo/API references; added `CHANGELOG.md` and `Third-Party Notices.txt`.
- Updated Setup & Support and DOTween validation to version 1.1.0, the tested DOTween package `1.2.825` / runtime `1.3.030`, and Built-in/URP compatibility wording.
- Replaced the unpublished 1.0 Portal draft with complete 1.1.0 initial-release fields, dependency/AI disclosures, reviewer note, pricing plan, keywords, and a Recorder-based future media manifest.

Still pending before submission:

- Run the gallery asset validator and all existing automated suites after the final import.
- Complete full manual selection/option/replay/reset coverage and clean Built-in/URP artifact validation.
- Run Asset Store Validator, export the exact package, inspect it, and clean-import that artifact.
- Produce replacement gallery screenshots, the short caption-only marketing reel, and the longer caption-only tutorial with Recorder `5.1.7`.
- Reconfirm the current Portal media specifications and maximum available launch discount, preview the rendered listing, and submit.

## Outcome

Prepare the next Tween Helper release around one coherent product story:

> Tween Helper combines a fluent DOTween workflow, 300 built-in presets, and higher-level animation recipes for collections, destination motion, gameplay feedback, production UI, text and values, and camera feedback.

For count-based copy, the defensible current formulation is **300 registered presets plus 76 semantic/group operations**. The 76 operations include the existing 13 UI recipes and the branch's collection, destination, gameplay-feedback, production-UI, text/value, and camera families. Variants and the 474 internal review configurations are not additional registered presets.

The release should include a new customer-facing **Animation Gallery** scene derived from the useful parts of the internal review scene, but it must not ship review decisions, pass/fail buttons, internal filters, PlayerPrefs review state, or development-only validation code.

This roadmap does not authorize creating the new artwork or video yet. It defines the scene and capture surfaces that should exist before that media is produced.

## Approved release direction

Owner decisions recorded on 2026-08-14:

- Tween Helper has not been published. The 1.0.0 Portal material is an unpublished draft awaiting review and is superseded by this release plan.
- The first public release keeps version **1.1.0**, even though it is the initial release.
- Tween Helper-owned content uses the **Standard Unity Asset Store EULA**, not MIT.
- Customer copy claims the build used in this project: DOTween Free package `1.2.825`, reporting runtime `1.3.030`. Older DOTween versions are untested and are not advertised as supported.
- The new Animation Gallery becomes the only shipped demo scene. The old 2D and 3D scenes, their scene-specific setup/spawner code, and the animation-selection console are retired and removed after the gallery replaces their useful coverage.
- Selection auto-plays after a deterministic reset and retains an explicit Replay button.
- The gallery exposes all 300 registered presets through search and family filters. Semantic variants remain contextual options.
- Each category remembers its last selection and options for the current session, while fixtures always reset before display/playback.
- The gallery targets **16:9 only**, validated at 1920×1080 and 1280×720.
- Snippets use the shortest valid default call and add enum/value arguments when the user changes an option.
- Later media consists of one short marketing reel and one longer tutorial, both caption-only.
- Unity Recorder package `5.1.7`, already installed in the development project, is the approved capture tool for later screenshots and video.
- Canonical customer branding is **Tween Helper** and the support email is **Info@Loags.de**. Code identifiers and namespaces may continue using `TweenHelper` where spaces are invalid.
- Existing 1.0 key artwork remains. Feature screenshots and videos are replaced after the gallery is final.
- Portal positioning remains **Tools > Animation** at **$15**, with the maximum launch discount: currently **50% for two weeks**, subject to confirming the available Portal choices immediately before submission.
- The functional-content AI disclosure remains the existing Codex/ChatGPT disclosure; no additional generative-AI tool needs to be added based on the owner's answer.
- Mouse interaction is the only required gallery input. Touch, keyboard, and gamepad navigation are not release requirements.
- Camera animations use a dedicated preview camera inside an isolated stage and never move the gallery UI camera.
- Customer compatibility text states that Tween Helper is developed and tested with Unity `6000.5.2f1`; lower Unity versions have not been tested and are not claimed as supported.
- Continue advertising Built-in Render Pipeline and URP compatibility. HDRP and custom render pipelines remain explicitly untested.

## Audit evidence

### Branch delta

The release branch is a clean, linear 13-commit line directly on `main`:

- `main...HEAD`: 0 commits on the main side and 13 commits on the branch side.
- Changed files: 148.
- Approximate diff: 46,129 insertions and 2,467 deletions.
- Working tree at audit time: clean.
- Current distributable root: `Assets/Loags/TweenHelper`.
- Current distributable-root size in the development project: approximately 2.3 MB before export compression.

| Commit | What changed | Release/documentation consequence |
| --- | --- | --- |
| `6b4324b` | Added the internal preset review scene and controller. | Supplies the visual-validation model and fixtures for the new gallery, but remains development-only. |
| `e4a2b4f` | Advanced automatically after a review decision. | Review-only behavior; do not carry it into the customer gallery. |
| `6c68e10` | Added review filters and review-scene presentation. | Useful navigation reference, but the customer scene needs category and search navigation rather than status filters. |
| `71f2f47` | Refined reviewed 2D/3D presets and regenerated catalog content. | Existing catalog descriptions and examples need a fresh proofread against final behavior. |
| `b9c9fbc` | Added stagger infrastructure, eleven collection recipes, demo exposure, and documentation. | New public product pillar; Publisher Portal 1.0 copy does not mention it. |
| `add65c4` | Redesigned the Preset Browser with isolated previews and added destination-aware motion. | Changes the Editor-tool story and adds another public product pillar. |
| `b089c52` | Corrected destination-motion behavior and documentation. | Commit subject is too vague for release-history generation; use the actual diff when writing the changelog. |
| `216a665` | Added gameplay-feedback sequences. | New public product pillar and demo category. |
| `0dde29b` | Added production UI sequences. | New public product pillar with direction-dependent examples. |
| `0dec622` | Added TMP text and numeric-value animation sequences. | New public product pillar; distinguish 12 public operations from 13 curated demo examples. |
| `eac7b04` | Added repository-only Pipeline/CLI discovery foundations. | Not part of the customer `.unitypackage`; do not market it as a shipped feature. |
| `1f30463` | Combined developer telemetry work with advanced semantic-animation additions, including camera feedback and new variants. | Separate the internal CLI work from customer-facing animation changes in all release notes. |
| `899ae64` | Expanded the review catalog from 398 to 474 configurations and added validation coverage. | Provides the option/variant matrix for the gallery, but 474 review rows should not become 474 customer navigation rows. |

Commit-message hygiene note: `b089c52` is only `fix`; `216a665` and `0dde29b` contain a stray fenced-code prefix; `1f30463` combines two feature subjects. Do not generate customer release notes directly from these subjects.

### Product surface now present on the branch

The branch retains exactly 300 registered presets and adds semantic operations that intentionally do not expand that registry:

| Surface | Current branch evidence | Customer-facing treatment |
| --- | ---: | --- |
| Registered presets | 300 | Searchable gallery category and generated catalog. |
| Semantic UI recipes | 13 | Curated category. |
| Collection recipes | 11, plus configurable stagger and grid variants | Curated entries with order/direction/topology options. |
| Destination families | 8 world/local families | Curated entries with target space, sign, and interpolation options where relevant. |
| Gameplay-feedback families | 11 | Curated entries with applicable context/direction options. |
| Production UI operations | 15 | Curated entries with direction and optional-target choices where supported. |
| Text/value public operations | 12 | Curated gallery can retain 13 examples by showing count-up and count-down separately. |
| Camera-feedback operations | 6 | Curated entries; inward/outward FOV variants should be options, not separate top-level operations. |
| Internal review configurations | 474 | Development-only exhaustive validation surface. |

Documentation and marketing must name the unit being counted. Avoid unqualified claims such as “474 animations” or “13 text operations” when the number actually refers to review configurations or demo cards.

## Current documentation audit

### What is actually shipped

The current export root contains 11 Markdown files totaling roughly 123 KiB:

- Root `README.md`.
- `Documentation/Installation.md`.
- `Documentation/API.md`.
- `Documentation/PresetCatalog.md`.
- Six focused feature guides: collections, destinations, gameplay feedback, UI sequences, text/value animations, and camera feedback.
- `Samples/TweenHelper Demos/README.md`.

The documentation size is negligible relative to the package. Cleanup should optimize clarity and maintenance, not remove useful reference material merely to save bytes.

### Keep, rewrite, add, or exclude

| Artifact | Decision | Required change |
| --- | --- | --- |
| Package `README.md` | **Keep and rewrite** | Make it a concise landing page: value proposition, requirements, five-minute quick start, demo/gallery entry point, documentation index, support, and license summary. Remove publisher-only instructions. |
| `Installation.md` | **Keep and rewrite** | Make it the only canonical setup, dependency, compatibility, and troubleshooting page. Resolve DOTween package/runtime version wording. Add the new gallery path. |
| `API.md` | **Keep and reduce** | Keep builder fundamentals, sequencing, options, lifecycle, async/cancellation, and a compact feature index. Remove long passages that duplicate the focused guides. |
| `PresetCatalog.md` | **Keep as generated output** | Keep only registered presets and generation facts. Regenerate after final code changes; never hand-edit counts or entries. |
| Six feature guides | **Keep and overhaul** | Each guide owns its complete defaults, supported targets, configuration options, lifecycle contract, and examples. Normalize headings and eliminate duplicate introductory/marketing prose. |
| Demo `README.md` | **Keep and rewrite** | Describe each shipped scene, the new gallery navigation, input-independent controls, render-pipeline facts, and capture-friendly presentation mode if implemented. |
| `CHANGELOG.md` | **Add if this is an update release** | Provide customer-facing version history. Do not derive it mechanically from malformed or combined commit subjects. |
| `Third-Party Notices.txt` | **Do not add solely for DOTween** | DOTween is a separately acquired dependency and is not included in the export. Add this file only if the exact release artifact contains third-party components that require attribution; remove the stale submission-note claim otherwise. |
| Standalone `Lifecycle.md` | **Do not add by default** | The old roadmap claims it exists, but it does not. Keep core lifecycle rules in `API.md` and family-specific exceptions in feature guides unless the rewrite proves a separate page materially improves navigation. |
| Separate `LICENSE.md` | **Do not add** | The owner selected the Standard Unity Asset Store EULA. Package and Portal copy now state that decision; no separate MIT-style license file is shipped. |
| Internal roadmaps, submission notes, Publisher Portal history, tests, CLI, telemetry, and review scene | **Never ship** | Keep under `_Project` or otherwise outside `Assets/Loags/TweenHelper`. |
| Publisher artwork and captured media | **Never ship in the package** | Keep outside the distribution root and upload through Portal artwork/media fields. |

### Confirmed documentation problems

1. The internal 1.0 roadmap claims `CHANGELOG.md`, `LICENSE.md`, `Third-Party Notices.txt`, and `Lifecycle.md` are in the package; neither `main` nor this branch contains them under the export root.
2. The package README tells customers to register DOTween in the Publisher Portal. That is a publisher instruction and should not be customer-facing.
3. The package README, `API.md`, and six feature guides repeat much of the same feature copy and lifecycle wording. This creates avoidable drift.
4. **Resolved:** Publisher copy and package setup code now report version `1.1.0`.
5. **Resolved:** Customer, setup, and Portal copy now name the tested DOTween package `1.2.825` and runtime `1.3.030`; older versions are explicitly untested.
6. **Resolved:** Tween Helper-owned content uses the Standard Unity Asset Store EULA; no MIT claim remains in current release copy.
7. Counts refer to different concepts without a shared glossary: registered presets, semantic operations, demo examples, and exhaustive review configurations.
8. The current sample README describes seven 2D tabs, while the internal review scene has eight high-level categories because it also covers camera feedback.
9. **Resolved:** `PublisherPortalReleaseNotes.md` now contains the full 1.1.0 initial-release listing and archives 1.0.0 as an unpublished superseded draft.
10. Publisher media exists in three locations (`PublisherMedia`, `Assets/PublisherMedia`, and `_Project/.../PublisherPortal/Branding`). The internal Portal record lists only part of that inventory. Consolidate to one canonical source/export structure later; none belongs in the customer export root.
11. `Installation.md` still says Preset Browser previewing requires selecting a non-persistent scene `GameObject`. The redesigned browser now renders isolated in-window previews and must be documented as not mutating the active scene.
12. The new collection, destination, UI-sequence, and text/value guides contain manual-review catalog or coverage commentary. That evidence belongs under `_Project`; customer guides should describe only supported behavior and usage.
13. `PresetCatalog.md` calls itself the output of "internal release tooling." Keep the generated status, but describe it from a customer perspective and provide the regeneration detail only in internal release documentation.
14. **Resolved:** Setup & Support reports 1.1.0 and treats Built-in/URP as supported while flagging HDRP/custom pipelines as untested.
15. Portal technical details describe Preset Browser previews against compatible active-scene targets with state restoration. That is stale after the isolated `PreviewRenderUtility` redesign and undersells the safety improvement.
16. The sample and documentation counts need a glossary-backed distinction: text/value has 12 public operation families but 13 curated sample entries because count-up and count-down are displayed separately.
17. Product spelling varies between `TweenHelper` and `Tween Helper`. Adopt `Tween Helper` for the product name and prose, reserving `TweenHelper` for namespaces, types, menu identifiers, and paths where it is already part of the technical contract.

## Documentation rewrite rules

1. Write for a customer who has just imported the asset, not for the publisher preparing a submission.
2. Put dependency, version, render-pipeline, and unsupported-platform facts in one canonical requirements table and link back to it.
3. Use “preset” only for the 300 `ITweenPreset` registry entries. Use “operation,” “recipe,” “example,” or “review configuration” elsewhere.
4. Show typed compile-time APIs first. Show dynamic-name APIs only where dynamic data is the actual use case.
5. Every code block must be compilable in context or explicitly marked as a fragment.
6. Every feature guide uses the same order: purpose, quick example, supported targets, options/defaults, composition, lifecycle/replay, limitations, and related demo category.
7. Do not claim validation on a Unity version, render pipeline, platform, or scripting backend without recorded evidence.
8. Do not repeat exact counts in several prose files unless a validation step checks them against source data.
9. Preserve customer-facing links when possible. If a file is renamed, update every Markdown link and Unity Editor documentation path in the same change.
10. Run a UTF-8 and typography pass so degree signs, dashes, apostrophes, and Markdown render consistently.

## New shipped Animation Gallery

### Scene intent

Add a new scene under:

```text
Assets/Loags/TweenHelper/Samples/TweenHelper Demos/Scenes/TweenHelperAnimationGallery.unity
```

Working title in the UI: **Tween Helper Animation Gallery**.

The scene is a customer discovery and learning surface. It should feel similar to the internal review scene in how reliably it resets, displays one selection, and exposes meaningful variants, but it must not be an exhaustive QA checklist.

### Category model

Expose these eight categories:

1. Presets
2. UI Recipes
3. Collections
4. Destination Motion
5. Gameplay Feedback
6. UI Sequences
7. Text & Values
8. Camera Feedback

Presets receive search and family filters. Semantic categories show a short curated operation list. Visually different enum values remain options on an operation rather than duplicated list rows.

### Navigation and layout

Use prefab-authored UI and Inspector-wired references. Do not create the interface hierarchy in code.

Recommended desktop/capture layout:

- Left rail: category buttons with selected state and optional per-category counts.
- Secondary rail: searchable/filterable animation list for the active category.
- Center: isolated preview stage with only the fixtures relevant to the selection.
- Details panel: animation name, one-sentence description, relevant option controls, and supported target/context note.
- Code panel: live C# snippet in a scrollable monospace area plus **Copy**.
- Playback bar: **Previous**, **Replay**, **Next**, and **Reset**. No **Wrong**, **Correct**, status, or review-filter controls.

```text
+----------------+---------------------+-----------------------------------------+
| Categories     | Animations          | Preview stage                           |
| Presets        | Search / filter     |                                         |
| UI Recipes     | Selected list item  |       active authored fixture           |
| Collections    | ...                 |                                         |
| Destinations   |                     +-----------------------------------------+
| Feedback       |                     | Name, description, target badges        |
| UI Sequences   |                     | Contextual option 1 | option 2          |
| Text & Values  |                     +-----------------------------------------+
| Camera         |                     | Live C# snippet                 [Copy]   |
+----------------+---------------------+-----------------------------------------+
|                         [Previous] [Replay] [Reset] [Next]                     |
+--------------------------------------------------------------------------------+
```

Selection may auto-play after a deterministic reset, but Replay must always work. Infinite-loop examples must be represented by a safe finite preview cycle or be explicitly stopped before a new selection plays.

### Contextual options

Do not reuse one ambiguously labeled dropdown for unrelated concepts. Provide one or two contextual option slots whose labels and values change with the selected entry.

| Category/example | Option label | Example values |
| --- | --- | --- |
| UI sequence, directional text | Direction | Up, Down, Left, Right |
| List stagger | Order | First to last, Last to first, From center, To center, Random seeded |
| Grid wave | Direction | Left to right, Right to left, Top to bottom, Bottom to top |
| Grid diagonal/spiral | Pattern | Public enum values with readable names |
| Checkerboard | Phase | Normal, Inverted |
| Destination path | Interpolation | Catmull-Rom, Linear |
| Destination arc/spiral/FOV | Motion variant | Positive/negative, clockwise/counter-clockwise, outward/inward as supported |
| Destination and text operations | Target/context | UI/local, world, or supported TMP context |
| Feedback with impact direction | Impact direction | A small curated vector set with readable labels |

Rules:

- Options are shown only when the selected API supports them.
- Defaults match the public API default.
- Changing an option resets the fixture, regenerates the snippet, and replays the selection.
- The snippet uses the actual selected enum value and omits optional arguments only when the displayed result uses the public default.
- Seeded/random examples use a stable seed so replay and capture are deterministic.

### Code architecture

Do not ship a modified `PresetReviewController`. Build a small customer-facing sample architecture:

- `AnimationGalleryController`: category/list/option navigation and UI wiring.
- `AnimationGalleryCatalog`: pure C# definitions for stable ID, name, category, description, preview fixture, supported options, and snippet generation.
- `AnimationGalleryPlayer`: playback, active-handle ownership, deterministic reset, fixture visibility, and context-specific dispatch.
- `AnimationGalleryPreviewRouter`: activates the correct authored fixture without making catalog entries depend on scene objects.
- `AnimationGalleryCodePresenter`: displays and copies the snippet produced from the same immutable selection/configuration used for playback.
- `AnimationGalleryListItem`: reusable authored list-row view.
- `AnimationGalleryOptionView`: contextual dropdown/toggle binding without knowing animation APIs.

Keep MonoBehaviours narrow. Catalog and option logic should be plain C# where practical. Reuse existing sample snapshot/reset behavior and the review scene's proven fixture concepts, but avoid copying review persistence or status logic.

The internal validation assembly already references `LB.TweenHelper.Demo`. Use that boundary to validate catalog identity and snippets through existing development validation where practical. Per repository policy, do not add a new automated test suite unless explicitly requested.

### Fixture strategy

- Presets and UI recipes: isolated Image/Text and simple world target as compatibility requires.
- Collections: authored list, complete grid, incomplete grid, and loading-dot fixtures.
- Destinations: authored start/destination markers and a path guide that reflects interpolation/sign/winding.
- Gameplay feedback: separate UI and world fixtures where runtime branches differ.
- UI sequences: authored toast, modal/backdrop/controls, tooltip, dropdown, tabs, drawer, bottom sheet, and pages.
- Text & Values: UGUI TMP plus a world-space TMP fixture for supported mesh effects.
- Camera: a dedicated camera and clear reference environment; never disturb the gallery UI camera.

All fixtures must restore their captured authored baseline on Reset and before every playback. A category or option change must kill all relevant child/root tweens before applying snapshots.

### Accessibility and capture readiness

- Mouse/touch UI must provide the complete experience. Optional keyboard shortcuts cannot be required and must not introduce an Input System dependency.
- Provide visible focus/selected states, readable contrast, and no information conveyed only by color.
- Target a clean 16:9 composition at 1920×1080 and verify a usable minimum at 1280×720.
- Reserve a presentation/capture mode that hides navigation chrome while retaining the animation title and concise API snippet.
- Keep branding and fixtures pipeline-neutral where possible. Prefer UI/unlit assets so the gallery works in Built-in and URP without material conversion.
- Make preview timing deterministic enough for repeatable screenshot and video capture.

### Relationship to the current demos

The current 2D showcase already implements seven tabs, replay/reset/copy controls, a searchable 198-preset subset, and several contextual target/order behaviors. It is therefore the best source for reusable authored UI and sample code.

Approved end state: the Animation Gallery is the only shipped demo scene. Remove both `TweenHelperDemo2D.unity` and `TweenHelperDemo3D.unity` after the gallery reproduces the intended public coverage.

Removal includes scene-specific spawners/setup, the 3D animation-selection console, fly-camera/instruction components that no longer have a gallery use, obsolete demo prefabs, build-setting entries, and documentation references. Perform a reference audit first: reuse any authored UI, fixture, material, snapshot, reset, list-item, or display code that benefits the gallery, then delete only assets left unreferenced by the new scene. This is an intentional replacement, so removal happens in the same implementation slice as the gallery and its documentation rather than before a working replacement exists.

## Publisher Portal content plan

### Preserve history, add current copy

`PublisherPortalReleaseNotes.md` lists only an unpublished 1.0.0 draft. Archive that draft as superseded and create a clean **1.1.0 initial-release** entry. Do not describe 1.1.0 as an update from a public 1.0.0 release.

Recommended internal structure after that decision:

```text
Documentation/PublisherPortal/
|-- CurrentListing.md
|-- Release-1.1.0.md
|-- Archived-1.0.0.md
`-- MediaManifest.md
```

This is a proposed cleanup, not a required move if the existing single history file remains easier to maintain.

### New listing story

Lead with the workflow and result, then quantify it:

1. Fluent, type-safe DOTween composition.
2. 300 ready-to-use registered presets.
3. Higher-level production recipes for collections, destinations, gameplay feedback, UI, text/value, and camera motion.
4. Isolated Editor Preset Browser plus the shipped Animation Gallery.
5. Deterministic reset, interruption, cancellation, and replay behavior.

Place the DOTween dependency and any meaningful render-pipeline limitations near the top, not only in technical details.

The 1.1.0 changelog should be grouped by customer outcome:

- New animation families.
- Improved preset behavior.
- Redesigned Preset Browser.
- Expanded demos and documentation.
- Compatibility, lifecycle, and reset refinements.

Do not mention repository-only Pipeline commands or telemetry as customer features.

Working positioning to refine after the release facts are frozen:

> Build polished DOTween animation faster with 300 typed presets and 76 higher-level operations for collections, destinations, gameplay feedback, production UI, text and values, and cameras.

This deliberately positions Tween Helper as a code-first, type-safe workflow with visible copyable API, rather than as a no-code timeline editor. The first technical paragraph should disclose that DOTween is acquired separately and should name only pipelines and Unity versions verified against the exact artifact.

Working keyword set, subject to the Portal's current field limit and availability: `DOTween`, `Tweening`, `UI Animation`, `Animation Presets`, `Game Feel`, `Text Animation`, `Stagger`, `Camera Shake`, `Motion`, `Sequence`, `Easing`, `TextMesh Pro`, `uGUI`, `Editor Tool`, `Feedback`.

### Portal fields to refresh

- Version and version changes/changelog.
- Summary/tagline.
- Full description.
- Technical details.
- Unity versions actually tested.
- DOTween dependency mapping and Publisher Portal dependency entry.
- Render-pipeline compatibility and demo-specific limitations.
- Included scene list.
- Preset/operation/example counts with qualified labels.
- Keywords within the Portal limit.
- AI description based on the final functional code and documentation changes.
- Reviewer note describing dependency setup and any scene/render-pipeline prerequisite.
- Artwork/media manifest and final upload order.

Current official references to recheck at submission time:

- [Unity Asset Store Submission Guidelines](https://assetstore.unity.com/publishing/submission-guidelines)
- [Unity Manual: Filling in package details](https://docs.unity3d.com/6000.0/Documentation/Manual/AssetStorePkgDetails.html)
- [Unity Manual: Asset package publishing workflow](https://docs.unity3d.com/current/Manual/asset-store-workflow.html)
- [Unity Publisher updates: launch discount choices](https://assetstore.unity.com/publishing/release-updates)

As of the 2026-05-20 guidelines, relevant requirements include disclosure of Asset Store dependencies in the Portal, a demo scene for Animation-category submissions, comprehensive documentation for code/setup products, a video demonstration for animation submissions, and accurate disclosure of functional AI assistance. Revalidate these requirements immediately before submission because Portal rules can change.

## Existing media audit

- `TweenHelper-2D-Showcase.png` is effectively a blank dark frame and should be retired rather than uploaded again.
- `TweenHelper-3D-Showcase.png` presents a dense grid of labels and cubes. It demonstrates breadth but does not communicate an individual customer outcome clearly enough for a primary screenshot.
- `TweenHelper-2D-Showcase-GameView.png` is the strongest existing layout reference: navigation on the left, preview on the right, and Replay/Reset/Copy API controls. Its content and category model are now outdated, so use it as a composition seed rather than final media.
- The existing Feature Overview and Key Visual establish a useful dark cyan/purple visual language and can remain brand references if their claims stay accurate.
- The actual Setup & Support capture visibly says `1.0.0`; replace it only after version, dependency, and pipeline wording are finalized.

## Future media plan — do not produce yet

### Key images and screenshots

Build the gallery so it can later supply these captures:

1. Gallery overview showing category navigation, preview, and code together.
2. Collections/grid frame that makes direction or ordering visually obvious.
3. Destination-motion frame with start, path, and destination visible.
4. Production UI frame showing modal/drawer/page transition fixtures.
5. Text/value and gameplay-feedback frame with strong readable results.
6. Preset Browser screenshot demonstrating isolated preview and search.
7. Setup & Support screenshot showing dependency validation without overemphasizing support tooling.

Key art should communicate motion and workflow without being a plain Unity Editor screenshot. Unity's current manual requires exact key-image sizes and restricts text differently per image type; regenerate each composition for its target rather than mechanically cropping one master.

### Video set

Produce two videos after the gallery and copy are final:

1. **Product overview reel** — approximately 60–90 seconds, capture-first, showing category navigation, contextual enum changes, synchronized code snippets, replay, and the strongest results from every semantic family.
2. **Setup and complete feature tour** — longer tutorial covering DOTween setup, opening the gallery, the Preset Browser, the builder workflow, and a broader animation catalog. This satisfies the need for technical setup guidance without bloating the package with video files.

Both videos are caption-only. Capture screenshots and video with Unity Recorder `5.1.7`. Keep the existing 1.0 key artwork and replace only outdated feature screenshots/video or artwork whose factual claims no longer match the package.

Suggested overview sequence:

- 0–5s: product promise and immediate polished motion.
- 5–15s: gallery category navigation.
- 15–25s: change a direction/order enum and show the code update live.
- 25–50s: collections, destination motion, feedback, and UI sequences.
- 50–65s: text/value and camera feedback.
- 65–80s: 300-preset search and isolated Preset Browser preview.
- Final beat: dependency note, Unity compatibility, and product title.

Do not capture final media until names, snippets, defaults, colors, layout, version text, and listing claims are frozen.

## Implementation roadmap

### Phase 0 — Resolve release decisions

- [x] Define 1.1.0 as the initial public release rather than a published-package update.
- [x] Record the 1.0.0 Portal copy as an unpublished, superseded draft.
- [x] Select the Standard Unity Asset Store EULA for Tween Helper-owned content.
- [x] Approve a validation-based DOTween statement: the exact tested version is supported; older versions are untested.
- [x] Make the Animation Gallery the only shipped demo and retire both legacy demo scenes plus obsolete supporting code.
- [x] Approve the curated-operation-plus-contextual-options model instead of exposing all 474 review rows.
- [x] Keep Tools > Animation, $15, and the maximum launch discount currently available: 50% for two weeks.
- [x] Retain the existing Codex/ChatGPT functional-content AI disclosure with no additional AI tool.
- [x] Require mouse interaction only and use a dedicated preview camera for camera examples.
- [x] Advertise development/testing on Unity 6000.5.2f1 and do not claim support for untested lower versions.
- [x] Continue Built-in and URP compatibility claims while marking HDRP/custom pipelines untested.
- [ ] Record the exact DOTween package/runtime version from the final release-candidate validation and freeze the resulting wording.

Exit gate: release identity, dependency wording, license, and demo-scene policy are written as facts rather than assumptions.

### Phase 1 — Establish release facts and documentation ownership

- [ ] Record one release-facts table for version, Unity minimum/tested versions, DOTween requirement, pipelines, dependencies, namespaces, counts, scenes, support email, and license.
- [ ] Assign every fact and topic to one canonical customer document.
- [ ] Correct the old root roadmap's nonexistent-file and stale-version claims.
- [ ] Decide whether to restructure Publisher Portal history files or retain the single file.
- [ ] Inventory and consolidate Publisher media source/export paths outside the distribution root.

Exit gate: no contradictory release fact remains across code constants, customer docs, internal release docs, and proposed Portal copy.

### Phase 2 — Rewrite customer documentation

- [ ] Rewrite the package README.
- [ ] Rewrite installation/setup and troubleshooting.
- [ ] Reduce `API.md` to core API/lifecycle plus feature links.
- [ ] Normalize and overhaul all six feature guides.
- [ ] Regenerate and proofread the preset catalog.
- [ ] Rewrite the demo README for the final scene set.
- [ ] Add the approved changelog and license/notice files only where the final artifact and legal decision require them.
- [ ] Validate links, snippets, counts, encoding, terminology, and menu paths.

Exit gate: a new customer can install DOTween, import Tween Helper, open the gallery, run one typed example, understand replay/cancellation, and find every advanced family using only shipped offline documentation.

### Phase 3 — Build the gallery catalog and controller

- [ ] Define stable customer-facing category and entry IDs.
- [ ] Implement the pure catalog and contextual option descriptors.
- [ ] Implement snippet generation from the exact selected configuration.
- [ ] Implement isolated playback ownership, reset, kill, and replay.
- [ ] Reuse public APIs only; do not call internal validation helpers.
- [ ] Cross-check the curated gallery against the 474-entry review matrix so every public family is represented.

Exit gate: catalog inspection proves all public families are discoverable and every visible option maps to a real supported API value.

### Phase 4 — Author and validate the gallery scene

- [ ] Author the scene and reusable UI/fixture prefabs inside the distributable Samples folder.
- [ ] Wire all required references in the Inspector.
- [ ] Add category, search/filter, contextual options, details, snippet/copy, and playback controls.
- [ ] Auto-play a new selection after reset, retain Replay, and remember the last selection/options independently for each category during the session.
- [ ] Verify deterministic reset before selection, option changes, replay, category changes, disable, and scene unload.
- [ ] Verify 1920×1080 and 1280×720 layouts.
- [ ] Verify the complete gallery can be operated with the mouse and has no Input System dependency; touch, keyboard, and gamepad are optional and not required.
- [ ] Verify Built-in and URP presentation for the gallery itself.
- [ ] Use a dedicated isolated preview camera for camera-feedback entries and confirm the gallery UI camera remains stable.
- [ ] Remove the superseded 2D/3D scenes and scene-only scripts/prefabs after a reference audit, then leave only the gallery in demo documentation and build settings.
- [ ] Check Unity Console after import/compile and perform manual Play Mode review.

Exit gate: every gallery entry and supported option can be selected, replayed, reset, and copied without stale fixture state, hidden tweens, invalid snippets, or package-originated warnings/errors.

### Phase 5 — Rewrite Publisher Portal content

- [x] Archive the unpublished 1.0.0 draft and write a 1.1.0 initial-release entry.
- [x] Rewrite summary, description, technical details, keywords, dependency disclosure, AI description, and reviewer note.
- [ ] Keep Tools > Animation and $15; configure a 50% two-week launch discount if those remain the maximum Portal options at submission time.
- [x] State Unity 6000.5.2f1 as the developed/tested version and explicitly state that lower Unity versions are untested.
- [x] State Built-in and URP compatibility, with HDRP and custom pipelines untested.
- [x] Qualify all numerical claims.
- [x] List the final included scenes and documentation.
- [x] Update the media manifest with source files, capture subjects, and upload order; confirm current Portal sizes at production time.
- [ ] Preview the draft in the Asset Store and proofread the rendered result.

Exit gate: Portal copy matches the exact exported package and makes no unsupported, stale, or internal-only claim.

### Phase 6 — Produce media later

- [ ] Capture only from the release-candidate gallery and Editor tools using Unity Recorder `5.1.7`.
- [ ] Produce purpose-built key-image variants at the then-current required sizes.
- [ ] Retain the existing 1.0 key artwork and replace outdated feature captures.
- [ ] Produce the short overview reel and longer setup/feature tour.
- [ ] Keep both videos caption-only, with on-screen labels that remain readable without audio.
- [ ] Verify media shows the exact public names and snippets shipped in the release.

Exit gate: marketing media accurately represents the final asset and satisfies the then-current Portal and Animation-category rules.

### Phase 7 — Exact-artifact release verification

- [ ] Update hardcoded package version values only once the release identity is final.
- [ ] Run existing repository validations and complete the pending manual review gate.
- [ ] Run Asset Store Publishing Tools Validator against only `Assets/Loags/TweenHelper`.
- [ ] Export the exact `.unitypackage` and inspect its content list.
- [ ] Import the artifact into clean supported Unity projects.
- [ ] Open every shipped scene, the Preset Browser, and Setup & Support.
- [ ] Confirm customer docs, examples, links, and notices in the imported artifact.
- [ ] Confirm no `_Project`, CLI, telemetry, review status, Publisher media, DOTween files, or Asset Store tooling leaked into the export.
- [ ] Update Portal technical facts from the exact validation record and submit that artifact.

Exit gate: package, documentation, Portal copy, media, and validation evidence all describe the same release.

## Owner decisions

### Resolved on 2026-08-14

1. The 1.0.0 Portal material is an unpublished draft awaiting review.
2. The initial public release remains version 1.1.0.
3. Tween Helper uses the Standard Unity Asset Store EULA.
4. Documentation will claim the exact DOTween version proven by final validation; older versions remain explicitly untested.
5. The gallery replaces both legacy demo scenes and their obsolete support/selection code.
6. Selection auto-plays after reset and includes Replay.
7. All 300 presets are searchable; semantic variants use contextual options.
8. Category selections/options are remembered for the current session, with reset before playback.
9. The gallery targets 16:9 only at 1920×1080 and 1280×720.
10. Snippets are minimal for defaults and explicit for user-changed values.
11. Media includes a short marketing reel and a longer tutorial.
12. Both videos are caption-only.
13. Canonical customer naming is Tween Helper with support at `Info@Loags.de`.
14. Existing 1.0 key artwork remains; outdated feature captures are replaced.
15. Unity Recorder `5.1.7` is the approved later capture tool.
16. Portal positioning remains Tools > Animation at $15 with the maximum launch discount, currently 50% for two weeks.
17. The functional-content AI disclosure remains the existing Codex/ChatGPT disclosure; no additional AI tool is added.
18. Mouse is the only required gallery input.
19. Camera feedback uses a dedicated preview camera.
20. Compatibility copy names Unity 6000.5.2f1 as the developed/tested version and states that lower versions are untested.
21. Built-in and URP remain advertised as compatible; HDRP and custom pipelines remain untested.

### Open owner decisions

None. Remaining unchecked items are implementation or release-candidate validation tasks, not missing product decisions.

## Suggested additions

- Add a visible “Why this API?” micro-panel in the gallery that labels an item as Preset, Recipe, or Builder Operation. This teaches the product model without inflating documentation.
- Add **Featured** and **New in 1.ss1** filters so first-time users see a concise tour without losing access to the exhaustive 300-preset search.
- Add a small target-compatibility badge beside the selection name: UI, Transform, Renderer, TMP, Camera, or Collection.
- Add a deterministic capture/presentation mode rather than maintaining a separate media-only scene.
- Add a “Copy code” success state and ensure the copied text never contains rich-text markup from the display component.
- Include a one-click **Open Animation Gallery** action in Setup & Support if it can be added without reopening or redirecting users unexpectedly.
- Generate or validate repeated catalog counts during release preparation so README, demo, Portal, and catalog cannot silently diverge.
- Keep the internal 474-entry review scene as the exhaustive truth for visual variants and the customer gallery as the curated truth for discovery. Do not force one surface to serve both jobs.

## Definition of done

This initiative is complete when:

- Customer documentation is concise, internally consistent, offline, and matches the final APIs.
- Only customer-useful documentation and samples ship in `Assets/Loags/TweenHelper`.
- The new gallery provides category selection, relevant enum/configuration controls, deterministic play/replay/reset, live accurate snippets, and copy support.
- The existing demo-scene policy is deliberate and documented.
- Publisher Portal copy reflects the branch's real customer-facing features and exact dependencies.
- New images and video are produced only from the accepted release candidate.
- The exact exported artifact passes validation and clean-import review without development-only content.
