# Tween Helper documentation, demo, and Publisher Portal roadmap

- Status: **RELEASE CANDIDATE UPLOADED — DEFERRED MEDIA AND PORTAL SUBMISSION PENDING**
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
- Added a pipeline-neutral faceted blue gallery material and angled 3D fixtures so front, top, and side faces remain readable in Built-in and URP without scene-light dependencies.
- Validated Play Mode startup with no package errors and visually checked every category in Recorder stills at 1920×1080.
- Removed the retired 2D/3D scenes, their prefabs/materials, their keyboard/fly-camera/reset/spawner code, and the animation-selection console after a GUID/reference audit.
- Preserved internal review capability by moving its compatibility policy under `_Project` and repurposing legacy audit/build tooling for gallery validation.
- Rewrote the shipped README, installation guide, sample guide, feature guides, and API index; added `CHANGELOG.md`. No separate license or third-party notice ships because Tween Helper uses the Standard Unity Asset Store EULA and redistributes no third-party dependency.
- Updated Setup & Support and DOTween validation to version 1.1.0, the tested DOTween package `1.2.825` / runtime `1.3.030`, and Built-in/URP compatibility wording.
- Replaced the unpublished 1.0 Portal draft with complete 1.1.0 initial-release fields, dependency/AI disclosures, reviewer note, pricing plan, keywords, and a Recorder-based future media manifest.
- Removed obsolete 2D/3D Publisher captures from the Unity project and established repository-level `PublisherMedia` as the single future delivery location while retaining approved 1.0 source artwork under the development-only Publisher Portal folder.
- Ran the official Asset Store Validator against only `Assets/Loags/TweenHelper`: 35 passed, one reviewed static-state warning, zero failed, and no compilation errors.
- Exported and inspected `TweenHelper-1.1.0.unitypackage`: 130 package paths, all beneath `Assets/Loags/TweenHelper`, with no development tree, Publisher media, Asset Store tooling, or separately installed DOTween assets included.
- Uploaded the 650.6 KB release candidate with the official Asset Store Uploader to Publisher Portal draft `1449210`. The first upload indexed 108 files from Unity `6000.5.2f1`; the final exact replacement was accepted successfully and is awaiting the Portal's asynchronous manifest re-index.
- Updated and saved the rendered Portal listing with version `1.1.0`, current package/feature copy, Built-in and URP compatibility, DOTween dependency, reviewed AI disclosure, $15 price, 50% two-week launch discount, and 15 current keywords. No support email appears in Portal copy.

Still pending before submission:

- Repeat automated and visual checks against the exact exported artifact in clean Built-in and URP projects.
- Clean-import the uploaded artifact into clean supported Unity projects.
- Produce replacement gallery screenshots, the short caption-only marketing reel, and the longer caption-only tutorial with Recorder `5.1.7`.
- Replace the retained 1.0 media only after the new media is accepted, then submit the prepared draft for review.

These are release-production and external submission gates, not missing package implementation. The owner explicitly deferred creating images and videos until the accepted release candidate exists.

## Working-tree validation — 2026-08-15

- Unity `6000.5.2f1` compiled the final implementation without errors.
- The Animation Gallery asset validator passed with 377 entries across eight categories, including all 300 registered presets.
- EditMode passed 25/25 and PlayMode passed 8/8.
- Unity Recorder `5.1.7` captured ten temporary validation stills: every category plus dedicated UI-hover and open-option-menu states at 1920×1080.
- Visual review confirmed readable layout, category selection, contextual option state, synchronized snippets, dedicated camera preview, and faceted 3D fixture depth at 1920×1080.
- The final UI pass removed list-edge bleed, added distinct selectable hover/press tinting, and verified the upward option menu contains all five order values without covering the code or copy controls.
- The Unity Console contained no package-originated errors after rebuild, validation, Play Mode, or Recorder capture.

## Frozen release facts

| Fact | Release value |
| --- | --- |
| Product/version | Tween Helper `1.1.0`, initial public release |
| Unity validation | Developed and tested with Unity `6000.5.2f1`; lower versions are untested |
| DOTween validation | DOTween Free package `1.2.825`, reporting runtime `1.3.030`; older versions are untested |
| Required dependencies | DOTween installed/licensed separately, Unity UI (uGUI), and TextMesh Pro |
| Render pipelines | Built-in and URP supported; HDRP and custom pipelines untested |
| Public namespace | `LB.TweenHelper` |
| Registered presets | 300 |
| Semantic/group operations | 76 across UI, collections, destination motion, gameplay feedback, production UI, text/value, and camera feedback |
| Gallery entries | 377: 300 presets plus 77 curated examples; count-up and count-down separately demonstrate one numeric operation family |
| Shipped scenes | `TweenHelperAnimationGallery.unity` only |
| Gallery presentation | Mouse-only, 16:9, designed and validated at 1920×1080 |
| Support | In-Editor Setup & Support workflow only; no address in Portal copy or customer documentation |
| License | Standard Unity Asset Store EULA; no separate MIT-style license file |
| Third-party content | None redistributed; DOTween remains a separately acquired dependency |

## Customer-document ownership

| Topic | Canonical shipped document |
| --- | --- |
| Product overview, quick start, support, and license summary | `README.md` |
| Tested configuration, dependencies, setup, and troubleshooting | `Documentation/Installation.md` |
| Builder fundamentals, options, lifecycle, async/cancellation, and feature index | `Documentation/API.md` |
| Registered preset names and count | Generated `Documentation/PresetCatalog.md` |
| Advanced family APIs, defaults, targets, lifecycle, and limitations | The corresponding focused feature guide |
| Gallery scene, controls, category counts, and presentation constraints | `Samples/TweenHelper Demos/README.md` |
| Customer-visible version history | `CHANGELOG.md` |
| Portal fields, reviewer notes, media plan, pricing, and submission procedure | Development-only Publisher Portal and submission records; never shipped |

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
- The gallery targets **16:9 only**, validated at 1920×1080.
- Snippets use the shortest valid default call and add enum/value arguments when the user changes an option.
- Later media consists of one short marketing reel and one longer tutorial, both caption-only.
- Unity Recorder package `5.1.7`, already installed in the development project, is the approved capture tool for later screenshots and video.
- Canonical customer branding is **Tween Helper**. The support address remains private to the in-Editor Setup & Support workflow and is omitted from Portal copy and customer documentation. Code identifiers and namespaces may continue using `TweenHelper` where spaces are invalid.
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

1. **Resolved:** The superseded 1.0 roadmap is marked historical. The release ships the approved changelog but no separate license, third-party notice, or lifecycle file.
2. **Resolved:** Publisher-only DOTween registration instructions were removed from the customer README.
3. **Resolved:** `API.md` now owns core builder/lifecycle guidance and links to the focused feature guides instead of duplicating their complete advanced API contracts.
4. **Resolved:** Publisher copy and package setup code now report version `1.1.0`.
5. **Resolved:** Customer, setup, and Portal copy now name the tested DOTween package `1.2.825` and runtime `1.3.030`; older versions are explicitly untested.
6. **Resolved:** Tween Helper-owned content uses the Standard Unity Asset Store EULA; no MIT claim remains in current release copy.
7. **Resolved:** The frozen release-facts table distinguishes registered presets, semantic operations, gallery examples, and internal review configurations.
8. **Resolved:** The sample README documents the gallery's eight categories and the retired scenes are removed.
9. **Resolved:** `PublisherPortalReleaseNotes.md` now contains the full 1.1.0 initial-release listing and archives 1.0.0 as an unpublished superseded draft.
10. **Resolved:** Obsolete 2D/3D captures and `Assets/PublisherMedia` were removed. Retained source artwork is development-only; repository-level `PublisherMedia` is the sole future delivery location.
11. **Resolved:** Installation and README copy describe the isolated Preset Browser preview and no longer require an active-scene target.
12. **Resolved:** Internal review/coverage commentary was removed from all customer feature guides; each guide now points to its related Animation Gallery category.
13. **Resolved:** The generated preset catalog describes customer-facing generation and integrity checks without referring to internal release tooling.
14. **Resolved:** Setup & Support reports 1.1.0 and treats Built-in/URP as supported while flagging HDRP/custom pipelines as untested.
15. **Resolved:** Portal technical details advertise the isolated preview stage and state that it does not modify the active scene.
16. **Resolved:** Release facts and sample documentation distinguish 12 text/value operation families from 13 curated examples.
17. **Resolved:** Customer prose uses Tween Helper; `TweenHelper` remains only where required by code identifiers, namespaces, menus, or paths.

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
- Target and validate a clean 16:9 composition at 1920×1080.
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
- [x] Record the exact DOTween package/runtime version from the release candidate and freeze the wording as package `1.2.825` / runtime `1.3.030`, with older versions untested.

Exit gate: release identity, dependency wording, license, and demo-scene policy are written as facts rather than assumptions.

### Phase 1 — Establish release facts and documentation ownership

- [x] Record one release-facts table for version, Unity tested versions, DOTween requirement, pipelines, dependencies, namespaces, counts, scenes, support email, and license.
- [x] Assign every fact and topic to one canonical customer document.
- [x] Correct the old root roadmap's nonexistent-file and stale-version claims.
- [x] Retain one Publisher Portal history file with the unpublished 1.0 draft explicitly archived beneath the current 1.1 entry.
- [x] Inventory and consolidate Publisher media: approved source artwork remains development-only and repository-level `PublisherMedia` is the sole future delivery location.

Exit gate: no contradictory release fact remains across code constants, customer docs, internal release docs, and proposed Portal copy.

### Phase 2 — Rewrite customer documentation

- [x] Rewrite the package README.
- [x] Rewrite installation/setup and troubleshooting.
- [x] Reduce `API.md` to core API/lifecycle plus feature links.
- [x] Normalize and overhaul all six feature guides, removing internal review commentary and linking each to its gallery category.
- [x] Regenerate and proofread the preset catalog.
- [x] Rewrite the demo README for the final scene set.
- [x] Add the approved changelog; ship no separate license or third-party notice because neither is required by the final content/legal decision.
- [x] Validate links, snippets, counts, encoding, terminology, and menu paths.

Exit gate: a new customer can install DOTween, import Tween Helper, open the gallery, run one typed example, understand replay/cancellation, and find every advanced family using only shipped offline documentation.

### Phase 3 — Build the gallery catalog and controller

- [x] Define stable customer-facing category and entry IDs.
- [x] Implement the pure catalog and contextual option descriptors.
- [x] Implement snippet generation from the exact selected configuration.
- [x] Implement isolated playback ownership, reset, kill, and replay.
- [x] Reuse public APIs only; do not call internal validation helpers.
- [x] Cross-check the curated gallery against the 474-entry review matrix so every public family is represented.

Exit gate: catalog inspection proves all public families are discoverable and every visible option maps to a real supported API value.

### Phase 4 — Author and validate the gallery scene

- [x] Author the scene and reusable UI/fixture prefabs inside the distributable Samples folder.
- [x] Wire all required references in the Inspector.
- [x] Add category, search/filter, contextual options, details, snippet/copy, and playback controls.
- [x] Auto-play a new selection after reset, retain Replay, and remember the last selection/options independently for each category during the session.
- [x] Verify deterministic reset before selection, option changes, replay, category changes, disable, and scene unload.
- [x] Verify the 1920×1080 layout.
- [x] Verify the complete gallery can be operated with the mouse and has no Input System dependency; touch, keyboard, and gamepad are optional and not required.
- [ ] Verify Built-in and URP presentation for the gallery itself.
- [x] Use a dedicated isolated preview camera for camera-feedback entries and confirm the gallery UI camera remains stable.
- [x] Remove the superseded 2D/3D scenes and scene-only scripts/prefabs after a reference audit, then leave only the gallery in demo documentation and build settings.
- [x] Check Unity Console after import/compile and perform manual Play Mode review.

Exit gate: every gallery entry and supported option can be selected, replayed, reset, and copied without stale fixture state, hidden tweens, invalid snippets, or package-originated warnings/errors.

### Phase 5 — Rewrite Publisher Portal content

- [x] Archive the unpublished 1.0.0 draft and write a 1.1.0 initial-release entry.
- [x] Rewrite summary, description, technical details, keywords, dependency disclosure, AI description, and reviewer note.
- [x] Keep Tools > Animation and $15; configure the maximum available 50% two-week launch discount.
- [x] State Unity 6000.5.2f1 as the developed/tested version and explicitly state that lower Unity versions are untested.
- [x] State Built-in and URP compatibility, with HDRP and custom pipelines untested.
- [x] Qualify all numerical claims.
- [x] List the final included scenes and documentation.
- [x] Update the media manifest with source files, capture subjects, and upload order; confirm current Portal sizes at production time.
- [x] Preview the draft in the Asset Store and proofread the rendered result.

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

- [x] Update hardcoded package version values only once the release identity is final.
- [x] Run existing repository validations and complete the working-tree manual review gate.
- [x] Run Asset Store Publishing Tools Validator against only `Assets/Loags/TweenHelper` (35 pass, one reviewed warning, zero fail).
- [x] Export the exact `.unitypackage` and inspect its content list (130 paths, all below the distribution root).
- [ ] Import the artifact into clean supported Unity projects.
- [ ] Open every shipped scene, the Preset Browser, and Setup & Support.
- [ ] Confirm customer docs, examples, links, and notices in the imported artifact.
- [x] Confirm no `_Project`, CLI, telemetry, review status, Publisher media, separately installed DOTween assets, or Asset Store tooling leaked into the export.
- [x] Update Portal technical facts from the exact validation record and upload that artifact to draft `1449210`.
- [ ] Submit the prepared artifact after deferred media production and owner approval.

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
9. The gallery targets 16:9 only at 1920×1080.
10. Snippets are minimal for defaults and explicit for user-changed values.
11. Media includes a short marketing reel and a longer tutorial.
12. Both videos are caption-only.
13. Canonical customer naming is Tween Helper; the support address appears only inside the in-Editor Setup & Support workflow.
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
