# Publishing implementation evidence

Status: **M0–M9 feature implementation delivered; local release checks passed. External publishing checks remain.**

Candidate: `1.3.0-rc.1`, implemented from `1dbcf6222da297ca509df6c7ce56e0862adddde8` on main. Historical 1.2.0 evidence remains unchanged. Exact source and archive identity: [candidate release record](../../../../ReleaseArtifacts/Release-1.3.0-rc.1.md).

## Implemented milestones

| Milestone | Delivered behavior | Verification |
| --- | --- | --- |
| M0 | Terminal outcomes, thread entry, host ownership and migration contracts | Lifecycle guide and reproducible regression cases |
| M1 | Shared completion observation, retained completion, AwaitAny winner propagation and losing-observer disposal, main-thread cancellation, lifecycle cleanup and exact owned Edit Mode kill | Edit/Play tests, disabled domain reload and both player backends |
| M2 | Preserve host globals by default, explicit ownership opt-in, local tween defaults, paused Build, no global KillAll/Clear shutdown | Host-default/unrelated-tween regressions and migration guidance |
| M3 | Deterministic explicit registrations for 300 built-ins, custom reflection contract retained | Mono and IL2CPP High; additional customer-only preserved preset resolves by string and runs |
| M4 | Three compiled complete examples, setup version constant, lifecycle/migration/troubleshooting guides, event adapter, authored sample | Clean import, real update import, documentation compilation and prefab/event validation |
| M5 | Startup/construction/update/cancellation/cleanup harness and practical performance guide | Nine workloads per backend, 100 cancellation cycles, zero active-tween deltas; allocation counter explicitly unsupported |
| M6 | Audited archive, release copy, ten current stills, captioned reel and written tutorial | Archive/source/GUID audit, clean/update import and Asset Store validation |
| M7 | Use-case filters, local favorites, bounded 20-item history, reset and empty state | Persistence/filter tests; native ListView keyboard selection retained |
| M8 | Package/per-call/player preferences, smaller decorative accents, orientation-preserving spin, captured active-effect preference and Toggle prefab | Amplitude/destination, spin, active-preference and event-wiring tests |
| M9 | ProgressFillTo appended as enum value 27, validation/preview support, three workflow recipes and progress/reward prefab | Seven samples, 28 operations, old enum IDs retained, Filled Image/Slider and rewind tests |

Product code is under Assets/Loags/TweenHelper; tests, fixtures and validators remain development-only. No vendor source was changed.

## Recorded checks

Environment: Unity 6000.5.2f1, DOTween Free package 1.2.825 / runtime 1.3.030, uGUI/TMP 2.5.0, Windows 10.0.19045. Live Editor commands used Unity Pipeline CLI because Unity MCP was unavailable. No Unity Editor batch build was substituted. Standalone smoke executables ran headless.

| Check | Result |
| --- | --- |
| [Edit Mode](../../../../ReleaseArtifacts/RoadmapValidation/EditMode.json) | 34 passed, zero failed |
| [Play Mode](../../../../ReleaseArtifacts/RoadmapValidation/PlayMode.json) | 19 passed, zero failed |
| [Disabled domain reload](../../../../ReleaseArtifacts/RoadmapValidation/DomainReloadDisabled.txt) | Two sessions passed; Editor/runtime waits settled, motion reset, registry retained; original Editor options restored |
| [Review coverage](../../../../ReleaseArtifacts/RoadmapValidation/AnimationReviewCoverageValidation.txt) | 610 stable IDs; all 212 expanded runtime cases completed; manual review statuses preserved |
| [Lifecycle](../../../../ReleaseArtifacts/RoadmapValidation/AnimationLifecycleRefactorValidation.txt) | Camera, feedback, TMP, UI, spatial interruption/restoration and 177-ID manifest passed |
| Registry/recipes | 300 presets; four legacy Gallery recipes valid; 28 operations; added samples covered by tests |
| [Clean Built-in import](../../../../ReleaseArtifacts/RoadmapValidation/CleanImport-Builtin.txt) | Registry, prefab scripts, shader and serialized references passed |
| [Update installation](../../../../ReleaseArtifacts/RoadmapValidation/Upgrade-1.2.0-to-1.3.0-rc.1.txt) | Real 1.2.0 import followed by candidate; customer scene, player script and recipe GUID preserved |
| [Mono build](../../../../ReleaseArtifacts/RoadmapValidation/Build-Mono2x.txt) / [player](../../../../ReleaseArtifacts/RoadmapValidation/Player-Mono.json) | Succeeded, zero errors; 300 names and lifecycle/performance checks passed |
| [IL2CPP High build](../../../../ReleaseArtifacts/RoadmapValidation/Build-IL2CPP.txt) / [player](../../../../ReleaseArtifacts/RoadmapValidation/Player-IL2CPP.json) | Succeeded, zero errors; 300 names and lifecycle/performance checks passed |
| [Custom preserved preset](../../../../ReleaseArtifacts/RoadmapValidation/Player-IL2CPP-Custom.json) | 301 = 300 built-ins + customer fixture; string-only custom playback passed under High stripping |
| [Asset Store Tools](../../../../ReleaseArtifacts/RoadmapValidation/AssetStoreValidation.json) | RanToCompletion; 35 passes, one static-variable warning, zero failures; no compilation errors |
| [Archive audit](../../../../ReleaseArtifacts/RoadmapValidation/Artifact.json) | 198 unique paths/GUIDs, product-root boundary and source comparison passed |

The clean project uses separately installed DOTween/TMP Essentials. Its development smoke harness was added after import and is excluded from the archive. The customer-only preset source is retained as a text fixture beside its report, then removed from the disposable project. The main Editor uses URP 17.5.0; current Gallery rendering and lifecycle checks passed there. Standalone builds used Built-in. Documentation distinguishes these configurations.

## Warnings and limits

The static-variable warning remains visible. Registries, immutable delegates and caches intentionally use static state. Preference and pending-observation state have explicit lifecycle resets, exercised across two sessions without domain reload. This is not a universal audit of every possible customer static state. Mixed script line endings were normalized, removing that warning.

Mono has one development-tool warning: Pipeline disables itself because the smoke scene has no runtime Pipeline manager. IL2CPP adds two TMP code-generation notices about large methods receiving separate C++ files. All final builds have zero errors. Earlier development-project results and initial timings are superseded by the clean-project reports.

The allocation API returned zero for a known 4 KB allocation; final reports record unsupported allocation counters and -1 values. Headless CPU timings exclude rendered frames, Canvas/GPU costs and profiler allocations. No FPS or zero-allocation promise is made. Provisional investigation thresholds on this host are median 0.25 ms for either 500-target transform/collection workload and 1.5 ms for the 1,024-glyph TMP wave. Recorded medians stay below those thresholds, so no speculative runtime rewrite was justified. Profile real target scenes before setting game budgets.

## External publishing handoff

- Confirm the actual published version/current Portal draft, category, field limits, crop rules and DOTween dependency configuration. Historical local IDs do not prove remote state.
- Review this concrete candidate and authorize upload/submission. No external publication, message or release tag was created.
- Native Browser/Setup/Inspector screenshots need a working capture surface. Computer use returns `SetIsBorderRequired ... 0x80004002` on this Windows installation; the publisher Chrome tab also failed its CDP focus operation. Unity Recorder captured actual Gallery stills and footage successfully. The written tutorial is complete; a recorded Editor walkthrough is not claimed.
- An unfamiliar human's usability session is not represented by automated tests. Other Unity versions, standalone URP rendering, other OSes, mobile/WebGL and HDRP remain untested and excluded from verified support claims.

The detailed roadmap retains broader manual acceptance checks that have not actually been executed. These are specific publishing/coverage limits, not unfinished runtime features.