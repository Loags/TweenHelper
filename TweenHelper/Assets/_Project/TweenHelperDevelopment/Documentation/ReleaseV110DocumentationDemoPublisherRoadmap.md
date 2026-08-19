# Tween Helper 1.1.0 Documentation, Demo, and Publishing Roadmap

Status: **release candidate reopened; replacement artifact and media pending**

Updated: 2026-08-19

Branch: `release-v.1.1.0`

Portal draft: `1449210`

## Why the release was reopened

The prior 1.1.0 draft artifact and Portal listing were prepared before the final animation-roadmap work. The branch now includes:

- world-to-UI projection and corrected Hop timing;
- normalized Image/Slider progress, synchronized value text, and progress hooks;
- gameplay states, expanded collection topologies, reusable sequence macros, camera helpers, and engine-property animation;
- a 446-entry Preset Browser with purpose-built UI/component fixtures;
- a rebuilt 527-entry development review scene with visible filled-Image/property meters;
- corrected customer, release, validation, and publishing documentation.

The uploaded artifact is therefore historical evidence only. It must be replaced before submission.

## Frozen release facts

| Fact | Release value |
| --- | --- |
| Product/version | Tween Helper `1.1.0`, initial public release |
| Unity | Developed/tested with `6000.5.2f1`; lower versions untested |
| DOTween | Free package `1.2.825`, runtime `1.3.030`; installed separately |
| Required Unity features | Unity UI (uGUI) and TextMesh Pro |
| Pipelines | Built-in and URP supported; HDRP/custom pipelines untested |
| Public namespace | `LB.TweenHelper` |
| Registered presets | 300 |
| Preset Browser | 446 isolated entries |
| Animation Gallery | 406 entries across eight categories |
| Internal review | 527 stable, unique IDs; development-only |
| Shipped scenes | `TweenHelperAnimationGallery.unity` only |
| Package root | `Assets/Loags/TweenHelper` only |
| License | Standard Unity Asset Store EULA |

## Count vocabulary

Use these terms consistently:

- **Preset** means one of the 300 registered `ITweenPreset` types.
- **Operation/recipe** means a semantic API outside that registry.
- **Preset Browser entry** means one isolated Editor preview. There are 446: 300 presets and 146 semantic/component entries.
- **Gallery entry** means one customer-facing runtime demo card. There are 406: 300 presets and 106 curated examples.
- **Review configuration** means one development-only visual/runtime branch. There are 527.

Never advertise “527 animations” or “446 presets.”

## Customer documentation ownership

| Topic | Canonical shipped document |
| --- | --- |
| Product overview, quick start, browser/gallery summary, support, licensing | `README.md` |
| Requirements, DOTween setup, import, validation, gallery startup | `Documentation/Installation.md` |
| Builder, options, callbacks, async/cancellation, lifecycle, feature index | `Documentation/API.md` |
| Family APIs and target/lifecycle limitations | Corresponding focused guide |
| Registered preset names | Generated `Documentation/PresetCatalog.md` |
| Gallery controls and exact category counts | `Samples/TweenHelper Demos/README.md` |
| Customer-visible release history | `CHANGELOG.md` |

Internal Portal fields, prices, draft IDs, media instructions, test evidence, Pipeline/MCP details, and review decisions never ship beneath `Assets/Loags/TweenHelper`.

## Current catalog evidence

Live assembly inspection on 2026-08-19 returned:

### Preset Browser — 446

| Category | Entries |
| --- | ---: |
| Presets | 300 |
| UI Recipes | 13 |
| Collections | 27 |
| Destination Motion | 20 |
| Gameplay Feedback | 27 |
| UI Sequences | 15 |
| TextMesh Pro | 13 |
| Progress Bars | 14 |
| Camera Feedback | 8 |
| Engine Properties | 9 |

### Animation Gallery — 406

| Category | Entries |
| --- | ---: |
| Presets | 300 |
| UI Recipes | 13 |
| Collections | 19 |
| Destination Motion | 12 |
| Gameplay Feedback and Macros | 25 |
| UI Sequences | 16 |
| Text and Values | 13 |
| Camera Feedback | 8 |

### Internal review — 527

| Category | Configurations |
| --- | ---: |
| Presets | 300 |
| UI Recipes | 13 |
| Collection Recipes | 34 |
| Stagger Variants | 10 |
| Destination Motion | 30 |
| Feedback Sequences | 23 |
| Gameplay States | 10 |
| UI Sequences | 39 |
| Text/Value Animation | 31 |
| Progress Animation | 15 |
| Sequence Macros | 4 |
| Camera Feedback | 9 |
| Engine Properties | 9 |

The latest review snapshot showed 527 reviewed, 516 Correct, and 11 Needs Work. Those 11 must be resolved before the replacement artifact is approved.

## Documentation overhaul record

- [x] Rewrote the root release roadmap around the reopened 1.1.0 candidate.
- [x] Expanded the customer README and changelog for every new runtime/editor family.
- [x] Updated installation and sample guides with 446-browser/406-gallery facts.
- [x] Documented required `Image.Type.Filled`/sprite setup and fixed-value Alert Pulse behavior.
- [x] Documented isolated UI preview composition and engine-property meters.
- [x] Converted the 1.1 implementation roadmap into a delivered implementation record.
- [x] Updated lifecycle/review roadmaps so historical 398/474 counts are not presented as current.
- [x] Rewrote Portal description, release notes, technical details, disclosures, reviewer note, and media manifest.
- [x] Rewrote the Asset Store runbook for a replacement artifact.

## Publishing compliance baseline

The official [Unity Asset Store Submission Guidelines](https://assetstore.unity.com/publishing/submission-guidelines) were last updated 2026-05-20 at this documentation checkpoint. Applicable requirements include comprehensive code/setup documentation, disclosure/registration of external Asset Store dependencies, one organized root, a demo scene for Animation-category submissions, a marketing video demonstrating included animations, and transparent disclosure of functional AI assistance.

The replacement artifact is produced with Unity `6000.5.2f1` (Unity 6.5), so the exact-artifact gate must include URP. The current guideline requires submissions made with Unity 6.5 or newer to support URP or HDRP.

Revalidate all current requirements immediately before upload. Portal media dimensions, codecs, file-size limits, keyword count, discount controls, accepted Editor versions, and render-pipeline requirements are external state and must not be frozen from an old draft.

## Replacement release workflow

### Phase 1 — Source freeze

- [ ] Resolve the 11 review entries marked Needs Work.
- [ ] Review the final code and documentation diff.
- [ ] Commit and push the two implementation commits and documentation overhaul.
- [ ] Record the exact source revision used for validation/export.

### Phase 2 — Working-tree validation

- [ ] Refresh Unity and confirm no package-originated compilation warnings/errors.
- [ ] Run EditMode and PlayMode suites.
- [ ] Run gallery, review coverage, lifecycle, and preview validators.
- [ ] Recheck representative world-to-UI, progress, macro, camera, audio, light, particle, renderer, and UI sequence behavior.
- [ ] Verify Built-in and URP presentation.

### Phase 3 — Exact artifact

- [ ] Validate `Assets/Loags/TweenHelper` with Asset Store Publishing Tools.
- [ ] Export `TweenHelper-1.1.0.unitypackage` from that root only.
- [ ] Inspect the complete path manifest.
- [ ] Confirm no DOTween, `_Project`, tests, review data, Pipeline/MCP, telemetry, Publisher media, Recorder, or Publishing Tools leaked into the artifact.
- [ ] Clean-import the artifact with DOTween installed separately.
- [ ] Open Setup & Support, Preset Browser, and the Animation Gallery.
- [ ] Recheck documentation links, copied examples, Console, and render pipelines.

### Phase 4 — Media

- [ ] Capture current gallery and Editor-tool screenshots from the accepted artifact.
- [ ] Produce the required animation demonstration video.
- [ ] Optionally produce the longer caption-only setup/feature tutorial.
- [ ] Retire the old 2D/3D showcase media.
- [ ] Verify every visible product name, count, API call, and compatibility claim.

### Phase 5 — Portal replacement

- [ ] Copy the exact fields from `PublisherPortalReleaseNotes.md`.
- [ ] Register DOTween as the external Asset Store dependency.
- [ ] Confirm version, compatibility, category, $15 price, discount, keywords, and AI disclosure.
- [ ] Upload the replacement artifact and wait for manifest processing.
- [ ] Upload accepted media and preview the rendered listing.
- [ ] Submit for review only after the listing and artifact match.

## Media direction

Lead with visible customer outcomes and the code workflow:

1. polished motion and the gallery overview;
2. changing a contextual option while the C# call updates;
3. world-to-UI collection;
4. collection topology and production UI composition;
5. text/value/progress and gameplay feedback;
6. camera and engine-property previews;
7. the 300-preset search and 446-entry isolated Preset Browser;
8. DOTween dependency, Unity compatibility, and product title.

Both the short marketing reel and optional longer tutorial should be caption-only. Capture with Unity Recorder `5.1.7`. Do not place video files inside the customer package.

## Definition of done

The 1.1.0 release is ready to submit only when source, customer documentation, internal validation evidence, exported artifact, clean-import results, Portal fields, and media all describe the same committed revision with no stale count, scene, dependency, compatibility, or feature claim.
