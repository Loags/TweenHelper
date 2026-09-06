# Tween Helper publishing implementation roadmap

Status: **M0–M9 implementation delivered; local release checks passed; external publishing checks remain.**

Baseline: `1dbcf6222da297ca509df6c7ce56e0862adddde8` on `main`, Tween Helper 1.2.0 release candidate.

This document turns the repository review into an implementation plan. The user subsequently authorized implementation with computer use. Current progress is recorded in [PublishingImplementationEvidence.md](PublishingImplementationEvidence.md). This plan does not certify release readiness, change the historical 1.2.0 validation results, or authorize a Publisher Portal submission. Checked items have implementation or recorded execution evidence. Unchecked items retain broader manual/Portal acceptance targets and are not claimed complete. M7–M9 were promoted into this implementation by the user's instruction to finish the roadmap.

## 1. Outcome and release scope

Ship an animation package that installs predictably, coexists with an existing DOTween project, behaves consistently during cancellation and teardown, and teaches customers how to build useful motion quickly.

The launch milestone is reliability, compatibility evidence, onboarding, and current publishing material. The follow-up milestone is discovery improvements, reduced-motion support, and a small expansion of useful recipe workflows. More preset variants are not a launch requirement.

### Baseline evidence

| Area | Known state | Remaining uncertainty |
| --- | --- | --- |
| Git | `main` contains all reviewed work; obsolete release branch removed locally and remotely | Check status before each implementation slice |
| Product | 300 registered presets; 461 Browser entries; 423 Gallery entries; 610 internal review configurations | Recompute after changes; these are different surfaces |
| Recipes | 27 operations and four shipped recipe samples | Broader player and preview lifecycle evidence |
| Environment | Release record identifies Unity `6000.5.2f1`, DOTween package `1.2.825`, runtime `1.3.030` | Additional Unity versions, scripting backends, and platforms |
| Artifact | Saved artifact hash matches the release record; 182 paths remain within the product root; product file contents match ignoring line endings | Any implementation change invalidates that artifact as the new release candidate |
| Documentation/assets | Shipped Markdown links resolve and product assets have metadata | Customer usability, update installation, and external publishing copy |
| Validation | Historical Asset Store validation/export recorded 35 passes, one static-variable warning, zero failures | That run explicitly excluded a player build and automated tests; the review had no live Unity MCP connection |

Canonical historical evidence: [Release-1.2.0.md](Release-1.2.0.md). Preserve it. Record subsequent runs separately with their exact commit and artifact identity.

### Release identity decision

- [x] Selected a separate `1.3.0-rc.1` candidate and preserved historical 1.2.0. Exact archive identity and implementation evidence are recorded in the candidate release record. Remote publication state remains unverified.

- [ ] Confirm whether 1.2.0 has actually been published. A local release record or uploaded artifact alone does not prove publication.
- [ ] If unpublished, rebuild the candidate under the selected launch version after all required work passes.
- [ ] If already published, use a new version; never silently replace the historical artifact or validation record.
- [ ] Classify the DOTween configuration change and any public API contract changes explicitly. Provide migration guidance; select a major version if the final public compatibility change warrants it.
- [ ] Do not increment a version merely because this roadmap was created.

## 2. Implementation rules

- Work on `main` in small reviewable commits, matching the requested single-branch workflow. Use release tags for immutable release identity when a release is approved. Verify a clean working tree before starting each slice and preserve unrelated edits.
- Product changes belong in `Assets/Loags/TweenHelper`; this is the deliberate package-specific exception to the general `_Project` preference. Development validators, performance fixtures, internal documentation, and proposed tests belong in `Assets/_Project/TweenHelperDevelopment`.
- Do not modify DOTween/vendor code, `Assets/IX`, or the customer's project-wide settings as a shortcut. Preserve `.meta` files and serialized IDs. Create metadata with every new Unity asset.
- Reuse the existing tween utilities, registry, recipe executor, and preview restoration paths. Do not add a second animation engine, hierarchy-based binding resolution, or general visual-scripting system.
- Treat required prefab and recipe bindings as explicit contracts. Validate at the established boundary; do not add silent recovery or repeated null-check boilerplate.
- Inspect Editor state and Console through Unity MCP before and after Unity-facing work when available. Do not enter Play Mode unless the validation case requires it.
- The subsequent instruction to finish this roadmap authorized its named regression and player-validation work. Edit/Play tests and live Editor player builds were executed; results are recorded separately.
- No Unity Editor batch build was used. Player builds ran through the inspected live Editor. New unrequested batch/CI work remains outside this implementation.
- Release upload/submission and external messages remain separate actions requiring explicit authorization. Prepare the exact artifact, notes, and evidence before requesting the final publishing decision.

## 3. Milestones and dependencies

| ID | Milestone | Depends on | Launch gate |
| --- | --- | --- | --- |
| M0 | Reproduce findings and freeze contracts | Baseline | Yes |
| M1 | Async and cancellation correctness | M0 | Yes |
| M2 | Respect host DOTween configuration | M0 | Yes |
| M3 | Preset registration and player compatibility | M0; final verification after M1/M2 | Yes |
| M4 | Customer installation and learning path | M1/M2 API decisions | Yes |
| M5 | Performance evidence and targeted optimization | M1/M2/M3 | Yes for representative evidence |
| M6 | Artifact, media, and release acceptance | M1–M5 | Yes |
| M7 | Browser discovery improvements | M4; release baseline stable | Follow-up |
| M8 | Reduced-motion controls | M1/M2/M5; contract decision | Follow-up |
| M9 | Curated recipe workflow expansion | M4/M8 integration decision | Follow-up |

Recommended execution: M0 → M1 → M2 → M3 → M4 → M5 → M6. Drafting documentation and planning captures can proceed while technical work is underway, but final examples, measurements, and captures must use the final candidate. M7–M9 must not delay launch unless explicitly promoted to launch scope. These are work dependencies, not an instruction to spawn agents.

## 4. M0 — Reproduce findings and freeze contracts

### Work

- [x] Record current commit, working-tree state, Unity/DOTween versions, Editor state, and compilation errors.
- [x] Reproduce async findings using the scenarios in M1. Classify each result as confirmed defect, compatibility decision, or unverified risk.
- [x] Inventory host-global writes in `Runtime/Core/TweenHelperBootstrapper.cs` and audit `TweenDefaults.cs`, `TweenOptions.cs`, and builder/direct/recipe playback paths for dependence on those globals.
- [x] Inspect registration, built-in preset constructors, and custom preset guidance before choosing a preservation mechanism.
- [x] Record API compatibility decisions before implementation; avoid changing signatures or cancellation semantics incidentally during refactoring.

### Required contract decisions

| Concern | Proposed contract |
| --- | --- |
| Normal completion | Completion awaits settle; retained completed tweens can be awaited immediately |
| External kill | Preserve existing documented behavior; timeout/result-returning waits report false; document that the void-returning completion wait cannot communicate success versus kill |
| Explicit cancellation | Active waits throw `OperationCanceledException`; owned mutation happens on Unity's main thread |
| Timeout | The timed wait kills its active tween once and returns false |
| Completion/cancellation race | Exactly one terminal outcome wins; define the ordering when completion already exists at call entry and when the token is already canceled |
| `AwaitAny` | Observe the winning wait's result/exception; detach losing subscriptions without killing unrelated tweens merely because another tween completed |
| Infinite tween | No natural completion; explicit teardown, cancellation, or timeout required |
| Thread support | Tween API entry points require Unity's main thread; cancellation requests may originate elsewhere |
| No settings asset | Tween Helper uses its own defaults without taking ownership of host-global DOTween configuration |

Acceptance: every behavior change has a reproducible scenario and documented desired outcome; unknowns are not represented as confirmed runtime failures.

## 5. M1 — Async and cancellation correctness

Primary files: `Runtime/Core/TweenAsync.cs`, `TweenHandle.cs`, and relevant builder async entry points. Reuse `Tests/Editor/EditMode/TweenCoreEditModeEditorTests.cs` and `Tests/Editor/PlayMode/TweenLifecyclePlayTests.cs` if regression-test changes are authorized.

### Implementation

- [x] Await the task returned by `Task.WhenAny`, so cancellation/failure of the winner is observed.
- [x] Separate disposal of an observer from cancellation of the tween. Do not cancel losing waits through a path that kills their tweens on normal `AwaitAny` completion.
- [x] Check completed retained tweens before relying on future callbacks. Apply consistent behavior to `AwaitCompletion`, timeout waits, and the custom tween awaiter.
- [x] Preserve additive callbacks and detach owned callbacks/registrations on every terminal path.
- [x] Route cancellation-triggered `ForceInit`, `Kill`, and any callback-list mutation through a lifecycle-safe Unity main-thread mechanism. Inspect existing scheduling support before adding a small internal dispatcher.
- [x] Register the main-thread mechanism from Unity lifecycle initialization; never capture a worker thread as the Unity thread. Account for Editor preview, disabled domain reload, assembly reload, and shutdown.
- [x] Keep the atomic terminal-state claim separate from Unity mutation. Complete the canceled task only when required cleanup is safe; settle waits during teardown even when future updates will not occur.
- [x] Eliminate redundant timeout kills and define external-cancellation versus timeout precedence when both tokens are signaled.
- [x] Audit `TweenCancellationRegistration` for disposal/callback races, retention of inactive targets, and parent-token cancellation from another thread.
- [x] Keep normal terminal paths quiet; do not add routine cancellation logging that obscures actionable package errors.

### Acceptance cases

| Case | Required observation |
| --- | --- |
| Finite completion and two simultaneous awaiters | Both finish; pre-existing callbacks execute once |
| Already-completed active tween with autokill disabled | All supported await entry points settle immediately |
| External kill / target destruction | No hung task; documented kill result; no retained observer |
| Already-canceled token | Deterministic entry behavior; no unsafe mutation |
| Cancellation on main thread and from worker/timer | Identical documented result; Unity mutations execute on main thread |
| Timeout with `Time.timeScale = 0` | Timeout behavior remains defined and teardown completes |
| Completion, external cancellation, and timeout near the same frame | One terminal outcome and at most one owned kill |
| `AwaitAny` winner canceled/faulted | Winner outcome propagates; no unobserved losing task failure |
| `AwaitAny` one completes while another loops forever | Wrapper returns; losing observation is released; the other tween keeps playing |
| Disable/destroy/reload/application shutdown | No pending subscription or deadlock |

Exit: all cases recorded against the implementation commit; public async documentation agrees with actual behavior.

## 6. M2 — Coexist with the host project's DOTween setup

Primary files: `Runtime/Core/TweenHelperBootstrapper.cs`, `TweenHelperSettings.cs`, `TweenDefaults.cs`, `DoTweenIntegration.cs`, `TweenBuilder.cs`, and the existing Settings/Setup Editor UI.

### Implementation

- [x] Introduce one explicit ownership setting for applying Tween Helper's global DOTween configuration. Default to preserving host configuration for new installations and the no-asset path.
- [x] Preserve safe initialization when DOTween has not been initialized. Avoid reinitializing or resetting a running host engine.
- [x] In preserve-host mode, do not overwrite global autoplay, autokill, update mode, independent-time defaults, easing, safe mode, logging, or capacities.
- [x] Apply Tween Helper-specific behavior to its own root tweens/sequences. Audit builder, direct extensions, presets, collections, and recipes, especially `.Build()`, `.Play()`, and retained recipe handles under host autoplay disabled.
- [x] Apply owned engine capacities only at a safe point before active tweens exist. Do not resize/reset the host engine opportunistically during playback.
- [x] Audit shutdown `KillAll`/`Clear` ownership. Remove package-wide assumptions that Tween Helper owns every tween in a customer project; preserve local handle cleanup.
- [x] Handle existing serialized settings explicitly. Explain the opt-in path for customers relying on old global behavior and avoid silent serialized-field reinterpretation.
- [x] Remove or reconcile duplicate capacity settings only after checking serialization and usage; retain compatibility where necessary.

### Acceptance

- [ ] Capture host defaults and a raw DOTween tween before Tween Helper initialization; preserve-host mode leaves both unchanged.
- [ ] Exercise initialization before and after host initialization and with domain reload enabled/disabled.
- [ ] Test no settings asset, existing settings asset, and explicit engine ownership.
- [ ] Run all public playback entry points with host autoplay/autokill defaults changed.
- [ ] Verify package teardown and preview closure do not stop unrelated tweens.
- [x] Publish a migration example and an explanation of per-tween defaults versus engine-global defaults.

Exit: importing and using Tween Helper does not implicitly reconfigure unrelated game animation.

## 7. M3 — Registration, stripping, and compatibility

Primary files: `Runtime/Core/TweenPresetRegistry.cs`, `CodePreset.cs`, built-in `Runtime/Presets`, runtime/editor `.asmdef` files, setup validation, and development validation tools.

### Implementation

- [x] Reproduce registry contents in a player where a recipe references a preset only by string, without Gallery/editor/type references masking stripping problems.
- [x] Prefer an explicit generated built-in registration list if reflection-only construction is not robust. Generate it deterministically in development and ship only the needed runtime output.
- Not needed: the explicit generated registration list passed IL2CPP High stripping; no blanket preservation workaround was added.
- [x] Preserve the custom preset extension contract. Document explicit registration or preservation for customer-defined reflection-discovered presets.
- [x] Validate duplicates, missing names, generic/name lookup parity, registry refresh, and repeated play sessions with domain reload disabled.
- [x] Check build-only dependencies: no `UnityEditor`, development assembly, publishing tool, or vendor redistributable leaks into runtime/package output.

### Compatibility matrix

| Environment | Planned evidence | Launch expectation |
| --- | --- | --- |
| Current validated Unity + recorded DOTween | Clean import, Editor compilation, existing EditMode/PlayMode tests, Gallery/Browser/recipe checks | Required |
| Built-in and URP | Clean customer project, visible materials/UI/TMP, Editor preview, representative runtime cases | Required for both advertised pipelines |
| Standalone Mono | Representative player smoke and lifecycle scenarios where backend is available | Required evidence for advertised desktop support |
| Standalone IL2CPP with elevated stripping | All built-in names resolve; custom/name-only recipe case works | Required before claiming stripped-player readiness |
| Additional supported Unity LTS version | Choose an installed/relevant version, then import/compile/run the same focused suite | Broaden advertised support only after passing |
| WebGL/mobile | Platform-specific async, font, rendering, and lifecycle checks | Conditional; do not advertise untested support |
| HDRP/custom pipelines | Deliberately separate assessment | Remain untested until verified |

Do not convert a proposed matrix row into a compatibility claim. If a required build cannot be run, leave the gate pending and identify the missing environment/authorization.

Exit: evidence demonstrates the supported configuration and name-based preset behavior in the actual exported product, with no development references hiding failures.

## 8. M4 — Installation, onboarding, and documentation

Primary surfaces: product `README.md`, `Documentation/Installation.md`, `API.md`, `TweenRecipes.md`, focused guides, `Samples/TweenHelper Demos`, Setup & Support, and `DocumentationExamplesEditorTests.cs` when authorized.

### Implementation

- [x] Build a short learning path: install dependencies → confirm setup → run a one-line animation → compose a sequence with cancellation → bind a reusable recipe to an authored prefab.
- [x] Provide three complete copyable examples with imports, serialized references, start/stop ownership, and expected visual results. Extend existing sample assets instead of creating another large demo framework.
- [x] Add a lifecycle table covering completion, kill, complete-and-kill if supported, rewind, restart, disable/destroy, looping, and restoration. Distinguish transient effects from operations that intentionally finish at a destination.
- [x] Explain the difference between presets, semantic builder operations, Browser entries, Gallery entries, and internal review configurations.
- [x] Document recipe limitations, `Then`/`With` timing, required binding/component contracts, unscaled UI, repeat playback, and safe preview/Prefab Mode behavior.
- [x] Validate documented UnityEvent wiring against the actual Inspector; if a playback method's return type prevents persistent event binding, add a clearly named void adapter without breaking the existing API, or document the supported wrapper workflow.
- [x] Add troubleshooting for missing DOTween modules/assembly definitions, TMP Essential Resources, non-filled progress Images, layout-driven transforms, interrupted tweens, and stale recipe bindings.
- [x] Keep version identity in one dependency-safe source for setup labels/support information; the setup assembly must remain usable when runtime dependencies are missing. Generate or validate Markdown/catalog counts from existing registries.
- [ ] Recheck migration behavior from the last actually distributed version, including removed files, retained GUIDs, obsolete documentation, and serialized recipes/settings.
- [x] Remove customer-facing Publisher Portal edit links from the changelog in favor of a public release/history destination or local documentation.

### Acceptance

- [x] A fresh project follows the documented steps without repository-only tools.
- [x] Existing-project installation preserves the host settings and authored scene.
- [ ] A developer unfamiliar with the package can complete the first-animation and recipe examples; record observed friction and fix it.
- [x] Sample code matches the shipped API; documentation links and menu paths resolve.
- [ ] Preview restores values and dirty state on stop, completion, close, Undo/Redo, reload, and transition to Play Mode.

Exit: customers can install, understand ownership, and adapt a complete example without reverse-engineering the Gallery.

## 9. M5 — Performance evidence and targeted optimization

Keep fixtures/results development-only. Inspect `Runtime/TextAnimations`, `Runtime/Stagger`, registry startup, recipe build/validation, and async observation paths. Do not optimize solely from source size or replace working code without a measurement.

### Measurement plan

- [x] Separate first-use/startup, tween construction, steady playback, interruption, and cleanup costs.
- [x] Measure representative transform/preset playback, collection transitions, rich TMP mesh animation, recipe playback, and repeated cancellation.
- [x] Suggested workloads: 10/100/500 collection targets and 32/256/1,024 visible TMP glyphs. Adjust if a target platform cannot support a useful run; retain the reason.
- [x] Record device, OS, Unity/backend/pipeline, development/profiler configuration, warmup, sample duration, allocations, frame-time distribution, and cleanup counts.
- [x] Include rapid replay and repeated create/play/kill cycles to expose subscription, mesh-state, and target retention.
- [x] Establish numeric budgets after measuring representative hardware. Do not publish invented FPS or zero-allocation promises.
- [x] Recorded the measured cost and provisional investigation thresholds. No speculative optimization was justified; TMP remains the largest steady CPU cost. Rendered profiling and allocation budgets require target-device measurements.
- [x] Publish a short performance guide with practical operating ranges, allocation caveats, pooling/replay advice, and the cost of changing text during an active effect.

Acceptance: measurements are reproducible, supported workloads meet recorded budgets, no unbounded retention occurs across repeated cleanup cycles, and performance claims match the measured build. Capture remaining tradeoffs as known limitations.

## 10. M6 — Release artifact, media, and acceptance

Primary surfaces: `PublisherMedia/README.md` at repository level, the selected release record, product changelog/setup version labels, existing development validators, and `ReleaseArtifacts` output.

### Artifact and publishing preparation

- [ ] Reconcile the media manifest's old 1.1.0 target, 406/446 counts, and draft `1449210` with the selected release and verified Portal record; do not infer the current remote draft from local files.
- [x] Recompute all catalog counts through existing validation/export tools and reconcile guides, release notes, Setup & Support, and marketing copy.
- [x] Run existing Gallery, review-coverage, lifecycle, recipe, and preset validators. Separate static counts, runtime checks, manual visual review, automated test results, and player results in the record.
- [x] Explain the historical static-variable validator warning: determine its current result, document why remaining state is appropriate, and verify resets rather than suppressing warnings broadly.
- [x] Export only `Assets/Loags/TweenHelper`. Inspect the archive manifest and exclude `_Project`, DOTween, tests, MCP/CLI/telemetry, project settings, publishing tools, media, and internal roadmaps.
- [x] Check exported asset metadata/GUIDs and sample references, not just root paths. No missing scripts, missing materials, or accidental references to excluded development assets.
- [x] Import the exact archive into a fresh customer project and execute the supported installation and runtime acceptance cases there.
- [x] Produce release evidence containing commit, Unity/DOTween versions, test/validator outputs, manual results, build configurations, archive path, size, SHA-256, and known limitations.
- [x] Regenerate the artifact and affected evidence after any product edit. Keep historical evidence intact and make the selected candidate unambiguous.

### Media and customer presentation

- [ ] Capture the actual final artifact: overview, menu transition, world-to-UI reward, collection rearrangement, TMP example, recipe workflow, Browser search, and setup.
- [x] Produced a 32.24-second captioned demonstration reel and a written installation/first-animation tutorial. A recorded Editor walkthrough and native Browser/Setup/Inspector captures remain blocked by the documented capture-surface failure.
- [ ] Check current Portal dimensions, crop behavior, video hosting/codec requirements, and field limits immediately before export/upload.
- [ ] Disclose required DOTween installation in the description and verify its Portal dependency configuration. State only compatibility supported by recorded evidence.
- [x] Verify third-party notices for anything actually redistributed and any required provenance disclosures for included content. Do not copy development/vendor assets into the artifact to satisfy a demo dependency.
- [x] Prepare final title, summary, description, technical details, release notes, dependency text, support contact, and known limitations in one reviewable release record.

### Launch acceptance checklist

- [x] Automated M1/M2 correctness/coexistence cases pass; the broader manual matrix below remains explicitly tracked.
- [x] Required M3 compatibility rows pass or the advertised scope is explicitly narrowed with the remaining limitation documented.
- [x] Existing tests and required validators have current results; any unexecuted or unauthorized proposed regressions are identified.
- [x] Clean import and update installation pass for the advertised package/version path.
- [x] M4 examples compile and the recorded preview rewind/lifecycle cases pass; the full manual Undo/Prefab interaction matrix remains tracked.
- [x] M5 representative performance evidence is recorded.
- [x] Exact artifact matches the recorded identity and contains only intended customer content.
- [ ] Customer documentation, media, counts, version, and dependency claims match that artifact.
- [ ] No unresolved launch-blocking defect remains. Remaining known limitations are specific and visible.
- [ ] Final publishing approval is obtained for the concrete candidate before upload/submission.

Exit: a reviewed, reproducible release candidate is ready for the authorized publishing action. After publishing, record the actual public version/date/link; create the release tag against the exact source commit when authorized.

## 11. M7 — Browser discovery follow-up

Primary files: `Editor/PresetBrowserEntry.cs`, `PresetBrowserCatalog.cs`, `PresetBrowserWindow.cs`, existing preview assets, and customer discovery documentation.

- [x] Add curated use-case metadata for menus, inventory, rewards, text, progress, and camera feedback using stable existing entry IDs.
- [x] Add favorites and a bounded recent-entry list stored as local Editor preferences, not as customer scene changes or runtime assets.
- [x] Preserve filters/search/contextual controls and ensure copied C# matches the selected operation and parameters.
- [x] Handle removed or renamed entries without breaking the Browser; retain deterministic catalog order.
- [x] Retained native ListView keyboard selection and added a clear empty-result message/reset action.

Acceptance: customers can locate a use-case example, favorite/reopen it, and copy a matching call; preview remains isolated and leaves the active scene untouched. Adding discovery metadata does not inflate preset counts.

## 12. M8 — Reduced-motion support follow-up

This is an explicit new runtime behavior surface. Define and review the contract before adding a global multiplier.

- [x] Design a package-local motion preference with normal/reduced behavior and explicit per-call overrides; do not mutate DOTween global speed or unrelated tweens.
- [x] Classify effects as decorative motion, semantic destination/value transitions, and looping feedback. Provide a reduced alternative per supported family.
- [x] Reduce or replace shake, spin, overshoot, camera kicks, and large spatial travel while preserving final values, visibility, destination, and required callbacks.
- [x] Prefer reduced spatial motion or a brief fade for important UI transitions; do not remove the information conveyed by feedback.
- [x] Define when preference changes take effect. Recommended first slice: newly built root animations only, with explicit replay/rebuild for active effects.
- [x] Define infinite-loop behavior, zero-duration handling, joined sequence ordering, text restoration, and cancellation before implementation.
- [x] Integrate defaults/options, builder/direct APIs, and recipes through shared utilities. Expose a sample preference toggle in an authored prefab.
- [x] Document custom-preset participation and unsupported cases; do not claim universal accessibility conformance from a motion toggle.

Acceptance: supported effects retain semantic outcomes and lifecycle ordering in both modes; essential progress and feedback remain understandable; mode changes do not mutate unrelated animation. Publish the covered-family matrix and limitations.

## 13. M9 — Curated recipe workflow expansion follow-up

Primary files: `Runtime/Recipes/TweenRecipeOperationCatalog.cs`, `TweenRecipe.cs`, `TweenRecipeValidator.cs`, `TweenRecipeExecutor.cs`, `Editor/Recipes`, and existing recipe sample assets.

- [x] Start with customer workflows: a popup with backdrop/content, a reward presentation, a progress/value update, and a collection entrance.
- [x] Solve each workflow with the current 27 operations first. Add a runtime operation only where the existing catalog cannot express the intended result clearly.
- [x] Candidate additions for assessment: progress fill to a value, a selected generalized TMP transition, and a selected collection deal operation. Choose only after confirming binding and preview requirements; this is not full builder parity.
- [x] For each accepted operation, implement runtime reuse, complete validation, bindings, inspector parameters, preview capture/restoration, sample, copied usage, and documentation in the same slice.
- [x] Preserve serialized enum values and stable node/binding IDs. Add migration/version handling before making a data-format change; existing recipe assets must continue to deserialize correctly.
- [x] Respect M8's motion policy where relevant, without storing scene references in recipe assets.
- [ ] Review Undo/Redo, duplication, invalid values, missing/stale bindings, reordering, Prefab Mode, and preview interruption.

Acceptance: existing recipes remain compatible; every new operation has complete authoring/preview/runtime support; at least one complete customer workflow demonstrates why it was added. Recompute actual counts only after integration.

## 14. Work deliberately outside this plan

- A general node graph, branches, conditions, variables, state machines, or unrestricted runtime recipe authoring.
- Broad integrations such as Timeline, Cinemachine, UI Toolkit, UniTask, or UPM redistribution without demonstrated demand and separate compatibility planning.
- Persistent curved/circular text, TMP formation transitions, a new shader/material animation engine, or parameter-only duplicate presets.
- A package-wide rewrite, DOTween fork, new runtime analytics, licensing enforcement, or online account requirement.
- Batch/CI infrastructure solely to replace unavailable Editor validation. CI can be planned separately once the local release checks are reproducible and explicitly authorized.

## 15. Delivery and evidence workflow

For each slice:

1. Confirm baseline and relevant Editor state.
2. Record the scenario and desired behavior.
3. Implement the smallest coherent change using existing abstractions.
4. Run the affected existing validation paths and authorized regressions; record missing environment checks honestly.
5. Update customer-facing contracts/examples where behavior changed.
6. Review diff, metadata, package boundary, and unintended project-setting changes.
7. Commit with a Conventional Commit subject; push completed slices according to the authorized Git workflow.
8. Mark roadmap tasks complete only with evidence tied to that commit.

Suggested commit slices:

| Slice | Suggested subject |
| --- | --- |
| Async outcome handling | `fix(async): propagate winner outcomes and settle completed tween waits` |
| Cancellation ownership | `fix(async): marshal tween cancellation and release wait subscriptions` |
| Host configuration | `fix(core): preserve host DOTween configuration by default` |
| Registration | `fix(presets): preserve name-based preset registration in player builds` |
| Learning path | `docs: add customer quick start and lifecycle contracts` |
| Measured optimization | `perf: reduce measured animation overhead` with the affected scope in the final subject |
| Release preparation | `chore(release): validate and prepare the selected release candidate` |
| Discovery | `feat(editor): add use-case discovery and browser favorites` |
| Motion preferences | `feat(accessibility): add reduced-motion animation options` |
| Recipe workflows | `feat(recipes): add curated customer animation workflows` |

These subjects are suggestions, not preapproved claims. Use `!`/a breaking-change footer if the actual compatibility change requires it. Do not create commits claiming a player fix or completed validation before obtaining that evidence.

### Evidence record template

```text
Milestone/task:
Status: planned / in progress / verified / blocked
Source commit:
Environment and configuration:
Scenario and expected behavior:
Observed result:
Validator/test/build output location:
Artifact SHA-256, if applicable:
Known limitations or missing checks:
Customer documentation updated:
```

## 16. References

- [Internal release record](Release-1.2.0.md)
- [Development assets and validation entry points](../README.md)
- [Product documentation](../../../Loags/TweenHelper/README.md)
- [Installation and current compatibility](../../../Loags/TweenHelper/Documentation/Installation.md)
- [Tween Recipes](../../../Loags/TweenHelper/Documentation/TweenRecipes.md)
- [Microsoft: Task.WhenAny behavior](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.whenany)
- [Microsoft: cancellation callback registration](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken.register)
- [Unity: code preservation and stripping](https://docs.unity3d.com/6000.0/Documentation/Manual/managed-code-stripping-preserving.html)
- [Unity Asset Store submission guidelines](https://assetstore.unity.com/publishing/submission-guidelines)

External requirements must be checked again at the publishing milestone; the repository review is not a permanent statement of Portal policy.
