# TweenHelper Animation Lifecycle Refactor — Implementation Plan

## Document status

- Planning state: **IMPLEMENTED — 106 LIFECYCLE-AFFECTED REVIEWS PENDING MANUAL REVALIDATION**
- Prepared: 2026-08-13
- Audited branch: `release-v.1.1.0`
- Audited commit: `1f30463`
- Source handoff: [TweenHelperAnimationLifecycleRefactorHandoff.md](TweenHelperAnimationLifecycleRefactorHandoff.md)
- Scope: internal lifecycle and normalized-timeline consolidation for advanced semantic animation families
- Public API impact: none
- Review-catalog impact: no entries or IDs added by this refactor; the 106 affected entries are intentionally reset to `Needs Work` after validation

This document is the implementation tracker for the lifecycle refactor. The implementation phases are complete; visual acceptance remains with the manual review pass described in the final handoff.

### Implementation checkpoint — 2026-08-13

- Rebased after the animation-review coverage task completed successfully: 474 entries, 474 unique IDs, 300 registered presets, and 174 semantic/review entries.
- Added one internal normalized timeline helper and migrated camera feedback, gameplay feedback, TMP text/value, production UI sequences, and spatial collection recipes.
- Intentionally left destination motion, destination Hop, and `TweenStaggerBuilder` outside the helper.
- Preserved public signatures, defaults, root ownership, visual evaluators, endpoint rules, and all 474 review IDs.
- Existing EditMode tests pass: 25 passed, 0 failed, 0 skipped.
- Existing PlayMode tests pass: 8 passed, 0 failed, 0 skipped.
- UI Sequence Phase Validation, Animation Review Coverage Validation, and Animation Lifecycle Refactor Validation all pass.
- Unity 6000.5.2f1 compiled the runtime and validation assemblies successfully.
- Unity MCP was unavailable with `401 Unauthorized`; Editor refresh, compilation, menu validators, tests, and log inspection were performed through the open Unity Editor instead.
- No new NUnit tests were added because the user did not explicitly authorize test additions. A development-only lifecycle validation command provides the required runtime probes.
- Exactly 106 lifecycle-affected review IDs are handed back as `Needs Work`; all unaffected persisted statuses remain untouched.

## 1. Objective

Consolidate repeated normalized `DOTween.To` construction and lifecycle plumbing into a small internal composition utility while preserving every established animation contract:

- public method names, signatures, overloads, and defaults;
- extension-method and `TweenBuilder` behavior;
- explicit-duration, `TweenOptions.Duration`, and family-default precedence;
- root delay, loop, ID, update mode, unscaled-time, callback, and await behavior;
- family-selected internal easing and intentionally linear semantic roots;
- lazy state capture at step start;
- exact completion endpoints;
- family-specific interrupted-kill behavior;
- rewind, restart, and finite Yoyo behavior;
- owner linking and cleanup;
- visual appearance and timing;
- the 474 current review entries and their stable IDs, while changing only the 106 explicitly affected persisted results for manual revalidation.

The refactor is successful only if it improves consistency and auditability without flattening meaningful family differences.

## 2. Explicit non-goals

The following work is outside this refactor:

- Rebuilding semantic animations from the 300 registered presets.
- Replacing family state types with a shared base class.
- Changing any public API or adding direction-specific public methods.
- Adding new animations or the separately planned review-coverage expansion.
- Changing animation curves, phase timing, amplitudes, endpoints, colors, or defaults.
- Making every `Kill()` restore all animated properties.
- Adding channel locking or arbitration for concurrent position, scale, rotation, alpha, color, TMP mesh, or camera-FOV writers.
- Adding a multi-owner or multi-target lifetime system.
- Refactoring preset implementations, `TweenStaggerBuilder`, destination Hop internals, `TweenLifecycleTracker`, or `DoTweenIntegration` merely to reduce line count.
- Changing accepted loop types or correcting unrelated legacy loop behavior during migration.
- Running a Unity batch build unless the user explicitly requests one.
- Adding or extending automated tests without explicit user authorization, per repository policy.

## 3. Audited baseline

### 3.1 Product baseline

After reconciling the completed review-coverage implementation, the current review catalog contains 474 entries:

| Category | Count |
|---|---:|
| Registered presets | 300 |
| UI recipes | 13 |
| Collection entries | 36 |
| Destination-motion entries | 26 |
| Gameplay-feedback entries | 22 |
| Production UI sequences | 39 |
| Text/value animations | 31 |
| Camera-feedback entries | 7 |
| **Total** | **474** |

The review controller persists each result under `TweenHelper.PresetReview.Status.<stable-id>`. This refactor does not rename an enum used in an ID, change an ID prefix, reorder persisted meaning, clear `PlayerPrefs`, or rebuild the catalog with replacement IDs. The final reset writes only the exact 106 affected keys and assigns the existing `Failed`/`Needs Work` value.

### 3.2 Current dirty-worktree warning

At plan creation, the worktree already contained user-owned changes unrelated to this refactor:

- modified `Assets/_Project/TweenHelperDevelopment/README.md`;
- untracked `Assets/_Project/TweenHelperDevelopment/Documentation/AnimationReviewCoverageAndExpansionRoadmap.md`;
- its Unity `.meta` file;
- untracked `TweenHelperAnimationLifecycleRefactorHandoff.md`.

Before every implementation phase, run `git status --short`, preserve unrelated work, and scope diffs only to the phase. The list above is historical context, not permission to overwrite those files if their state changes later.

### 3.3 Core integration facts

The audit confirmed these important mechanics:

- Semantic family builder methods call `AddStep(..., applyBuilderOptions: false)`.
- The family utility therefore applies `TweenOptions` and defaults to its own root exactly once.
- A semantic utility normally returns a paused `Tween`; `TweenBuilder.Play()` owns playback.
- Spatial collection recipes are the exception: the utility creates and starts the root immediately and returns an active `TweenHandle`.
- `TweenBuilder.BuildSingleTween()` links the returned root while preserving its existing `onKill` delegate.
- A multi-step builder links the outer `Sequence` to the primary builder owner.
- The installed DOTween documentation states that `SetLink` accepts one `GameObject` and that a child tween's link has no effect after it is added to a `Sequence`.
- Caller callbacks are added after family-internal callbacks. Internal endpoint/restoration work therefore currently occurs before caller completion/kill callbacks.

Do not alter these relationships during the refactor unless a characterization check proves the current implementation already violates its documented contract. Any such defect must be split into a separate bug-fix decision.

## 4. Duplication inventory

### 4.1 Safe, high-value duplication to consolidate

The following utilities repeat substantially equivalent lifecycle plumbing:

| Concern | Current locations |
|---|---|
| Normalized `DOTween.To` root | camera, TMP, transient/pickup feedback, spatial collections, UI sequences, destination paths |
| Lazy one-time initialization | camera, TMP, UI, spatial, destination path; feedback state objects provide their own idempotence |
| Linear semantic root | camera, TMP, feedback, UI, spatial collections, and internally eased destination families |
| Completion-versus-interrupted-kill guard | camera, TMP, feedback, spatial collections |
| Rewind restoration callback | camera, TMP, feedback, UI, spatial collections |
| Even finite Yoyo ends at invocation | TMP, feedback Pickup Collect, UI, spatial collections, destination motion |
| Return paused root | camera, TMP, feedback, UI, destination motion |
| Apply `TweenOptions` and owner link | all advanced families, with the spatial direct-play boundary noted above |

### 4.2 Duplication that remains family-owned

Do not move the following into the lifecycle helper:

- state capture and application (`FeedbackState`, `UISequenceState`, `TMPCharacterMeshState`, camera state, spatial item state, destination bindings);
- family validation and family-specific exception messages;
- `ResolveStrength`, phase easing, phase windows, interpolation, and deformation math;
- duration fallback selection in the partial `TweenBuilder` files;
- alpha/color/component binding logic;
- TMP source-string and mesh restoration logic;
- UI shown-state caching and invocation-pose logic;
- destination coordinate-space bindings and exact endpoint math;
- spatial per-item delay calculation;
- collection target snapshotting;
- preset and stagger construction.

Line-count reduction is not a sufficient reason to move any of these responsibilities.

## 5. Frozen behavior contracts

These contracts must be captured before migration and verified after each family moves.

### 5.1 Cross-family construction contract

| Concern | Required behavior |
|---|---|
| State capture | Lazy: at first actual start/evaluation of the builder step, never when merely configuring the builder. |
| Duration | Explicit method duration > `TweenOptions.Duration` > family fallback. Destination methods using the core fallback retain the existing core rule. |
| Root options | Applied once. `applyBuilderOptions: false` remains on semantic steps. |
| Root ease | Internally eased semantic timelines keep a linear root. A family that currently uses the root ease must keep doing so. |
| Playback | Family utilities used by `TweenBuilder` return paused roots. Direct spatial recipes still return an already-playing handle. |
| Linking | Root remains targeted and linked to its primary owner. |
| Callbacks | Internal state finalization runs before caller callbacks and each callback fires once. |
| Awaiting | Await observes the complete semantic root, not an internal child or channel. |
| Speed-based timing | Existing family rejection rules remain unchanged. |
| Reuse | Restart/loops reuse the state captured by that tween instance; building a new tween captures a new invocation state. |

### 5.2 Camera feedback

Applies to all six camera operations.

| Event | Required state |
|---|---|
| Normal completion | Exact captured local position, local rotation, and FOV. |
| Interrupted kill | Exact captured local position, local rotation, and FOV. |
| Rewind | Exact captured local position, local rotation, and FOV. |
| Restart/Restart loops | No accumulated transform or FOV drift. |
| Finite Yoyo | Ends restored; transient camera feedback never leaves an offset. |
| Focus Zoom capture | Focus position and derived pose remain captured lazily on first evaluation, as currently implemented. |

### 5.3 Gameplay feedback

| Operation group | Normal completion | Interrupted kill | Rewind |
|---|---|---|---|
| Error, Damage, Success, Reward, Heal, Shield, Critical, Cooldown, Level Up, Low Health | Restore captured pose and supported transient color/alpha channels. | Restore all captured transient state. | Restore all captured state. |
| Pickup Collect | Exact requested destination, zero scale, and supported alpha zero. | Preserve current path position; restore transient scale, rotation, and alpha. | Restore captured start position and all visuals. |
| Pickup Collect even finite Yoyo | Captured invocation state. | Not applicable after normal completion; a completed-then-killed root must not apply interrupted cleanup. | Captured invocation state. |

### 5.4 TMP text and value

| Operation | Normal completion | Interrupted kill | Rewind/even Yoyo |
|---|---|---|---|
| Typewriter Reveal/Hide | Fully revealed/hidden endpoint. | Preserve current partial visibility. | Restore invocation visibility. |
| Number Count | Exact formatted destination. | Preserve current displayed value. | Restore invocation text. |
| Character Stagger In | Restore mesh baseline. | Restore mesh baseline. | Restore mesh baseline. |
| Character Stagger Out | Restore mesh, then set `maxVisibleCharacters = 0`. | Restore invocation mesh and visibility. | Restore invocation mesh and visibility. |
| Wave, Bounce, Color Sweep, Glitch, Emphasis | Restore exact mesh/color baseline. | Restore exact mesh/color baseline. | Restore exact mesh/color baseline. |
| Scramble Reveal | Exact original source text fully visible. | Restore source text and invocation visibility. | Restore source text and invocation visibility. |
| Score Increase | Exact destination text; restore temporary scale/rotation/color. | Preserve current text; restore only temporary visuals. | Restore invocation text and visuals. |

The null interrupted-kill callbacks for Typewriter and Number Count are intentional current behavior and must not accidentally become restore behavior.

### 5.5 Production UI sequences

| Event | Required state |
|---|---|
| Normal completion | Exact semantic shown/hidden/outgoing/incoming endpoint for every participant. |
| Interrupted kill | Preserve every participant at its current visual state so another transition may continue from it. |
| Rewind | Restore every participant's invocation pose and supported alpha. |
| Restart | Repeat from stable absolute cached/invocation endpoints without drift. |
| Even finite Yoyo | End at each participant's invocation state. |

The helper must not add a restoring `OnKill` callback to UI timelines.

### 5.6 Spatial collections

| Operation | Normal completion | Interrupted kill | Rewind/even Yoyo |
|---|---|---|---|
| Burst In | Authored item position, scale, rotation, and alpha. | Restore every captured item. | Restore every captured item. |
| Burst Out | Requested radial endpoint, zero scale, supported alpha zero. | Restore every captured item. | Restore every captured item. |
| Gather To | Requested destination, zero scale, supported alpha zero. | Restore every captured item. | Restore every captured item. |

The per-item stagger is expressed in elapsed seconds today. A normalized helper adapter must evaluate it as `normalizedProgress * totalDuration` without changing start times.

### 5.7 Destination motion

Destination motion is a regression boundary for this refactor, not an initial migration target.

| Event | Required state |
|---|---|
| Normal completion | Exact requested destination. |
| Interrupted kill | Preserve current path position. |
| Rewind | Return through tween progress to the captured step-start position. |
| Restart | Reuse the original absolute path without drift. |
| Even finite Yoyo | Exact captured step-start position. |
| Hop interrupted kill | Preserve current base path position while removing grounding/deformation and restoring scale. |

Destination path timelines use both root-eased and internally eased modes and a family-specific progress normalizer. Hop is a real DOTween `Sequence` with overlapping grounded-scale tweens. Neither should be forced through the first shared helper.

### 5.8 Preset-based stagger

`TweenStaggerBuilder` and the preset-based grid/list recipes remain unchanged. Their lifecycle follows the root sequence and child preset behavior; they do not promise arbitrary item-state restoration.

## 6. Chosen internal design

### 6.1 Design choice

Add one internal delegate/composition-based normalized timeline helper. Do not add inheritance or a universal animation-state interface.

Recommended file:

- `Assets/Loags/TweenHelper/Runtime/Core/NormalizedTweenTimeline.cs`
- matching Unity `.meta` file

Recommended conceptual API:

```csharp
internal static class NormalizedTweenTimeline
{
    public static Tween Create(
        GameObject owner,
        float duration,
        TweenOptions rootOptions,
        Action initialize,
        Action<float> evaluate,
        Action completeForward,
        Action completeAtStart,
        Action rewind,
        Action interruptedKill = null,
        Action started = null);

    public static bool EndsAtInvocation(TweenOptions options);
}
```

The final naming may change slightly during implementation, but the responsibilities and explicit lifecycle delegates must not be hidden behind an inheritance hierarchy or a broad policy graph.

### 6.2 Required helper semantics

The helper must:

1. Validate only universal construction requirements: non-null owner/delegates and positive finite duration.
2. Create a `0 -> 1` `DOTween.To` root.
3. Keep a private normalized progress value, an `initialized` flag, and a `completed` flag.
4. Call `initialize` lazily and exactly once from both `OnStart` and the value setter, so direct seek/evaluation cannot bypass capture.
5. Clamp evaluation progress to `0..1` for the migrated families.
6. Apply the supplied `rootOptions` through `WithDefaults(rootOptions, owner)` exactly once.
7. Leave the choice of linear versus caller/default root ease explicit at the call site. Migrated semantic families pass `options.SetEase(Ease.Linear)`.
8. On start, clear the completed flag, ensure initialization, then invoke optional `started` behavior.
9. On complete, ensure initialization, set the completed flag before family state mutation, and choose `completeAtStart` only for a positive, even-count `LoopType.Yoyo`; otherwise invoke `completeForward`.
10. On rewind, clear the completed flag and invoke `rewind` only after initialization.
11. Register an `OnKill` callback only when `interruptedKill` is supplied. Invoke it only when initialized and not already completed.
12. Ensure `Kill(true)` follows completion semantics and does not subsequently execute interrupted-kill restoration.
13. Return the root paused.
14. Register internal callbacks before returning so later builder/handle callbacks remain additive and execute afterward.

### 6.3 Why the helper receives `rootOptions`

The helper must not silently overwrite easing. Passing the exact root options makes option ownership visible:

- camera, feedback, TMP, UI, and spatial migrations pass `options.SetEase(Ease.Linear)`;
- a future migration may pass `options` only if the existing family intentionally applies its ease at the root;
- family evaluators continue reading the original `options` value for internal primary/secondary/tertiary easing.

### 6.4 Alternatives rejected

| Alternative | Reason rejected |
|---|---|
| `BaseAnimationUtility` inheritance tree | State and endpoint rules are heterogeneous; hooks would obscure lifecycle order and encourage unsafe coupling. |
| One restore-policy enum for every case | Pickup, Score Increase, Character Stagger Out, and UI transitions restore different subsets; delegates express these differences directly. |
| Recompose everything from presets | Presets do not provide the required lazy multi-channel or multi-target state semantics. |
| Make `Kill()` universally rewind | Breaks destination handoff, partial text/value preservation, and interruptible UI transitions. |
| Centralize all validation/easing/duration helpers now | Expands scope without directly improving lifecycle correctness. |
| Add target/channel arbitration | A separate product feature with significant ownership and compatibility decisions. |

## 7. Secondary-target lifetime decision

### Binding decision for this refactor

Keep the current **single primary owner** lifetime model.

- The semantic root is targeted and linked to its primary owner.
- Destroying the primary owner must kill the root.
- Backdrops, controls, entries, incoming pages, focus targets, and collection members remain required participants that callers must keep alive for the operation's lifetime.
- Existing snapshot/build-time validation remains.
- Do not add hidden helper components, per-frame participant polling, repeated null guards, or attempts to call `SetLink` multiple times.
- Do not silently recover if a required secondary is destroyed.

### Rationale

DOTween provides one link target, and child links do not remain effective inside a sequence. A true multi-participant lifetime feature would require explicit policy for which participant owns cancellation, when the root is killed, how partial state is restored, and how nested builder sequences behave. Introducing that policy while moving lifecycle code would make regression attribution difficult.

### Follow-up candidate, not part of this plan

After this refactor is complete, a separate design task may evaluate an opt-in multi-participant guard. It must benchmark overhead, define destruction-time state policy per family, and test nested-sequence behavior before implementation.

## 8. Concurrent-writer decision

No channel arbitration is added in this refactor.

- The documentation must state which channels each semantic family writes.
- Callers remain responsible for preventing competing writers on the same channel.
- The refactor must not call `DOTween.Kill(target)` or kill unrelated tweens as an implicit conflict-resolution mechanism.
- A future channel-ownership system requires a separate public design and compatibility review.

## 9. Phased implementation roadmap

Each phase is a rollback boundary. Prefer one focused commit per completed phase. Do not start the next phase until all exit criteria for the current phase pass.

### Phase 0 — Freeze and characterize the baseline

Status: [x] Complete — baseline reconciled to 474 entries

Tasks:

- [x] Re-read this plan and the handoff.
- [x] Inspect `git status --short` and record all unrelated files to preserve.
- [x] Confirm branch, Unity version, DOTween installation, and current compilation state.
- [x] Attempt Unity MCP inspection; record its `401 Unauthorized` result and use the open Unity Editor fallback.
- [x] Compare the public semantic method surface before and after the runtime migration.
- [x] Record and validate all 474 review IDs before changing persisted statuses.
- [x] Inspect persisted coverage status information instead of assuming every new coverage item was passed.
- [x] Run the existing EditMode and PlayMode tests through Unity.
- [x] Run the existing UI Sequence Phase Validation tool.
- [x] Use the user's previously validated visuals and the completed 474-entry review/runtime coverage pass as the visual baseline.
- [x] Characterize callback order and callback count for a semantic root used alone and nested under `Then()`.
- [x] Characterize interrupted kill, forced completion, rewind, and finite even-Yoyo behavior through runtime probes.
- [x] Follow repository policy and avoid adding or extending automated tests without explicit authorization.

Test-authorization result:

- [ ] Authorized
- [x] Not authorized; use existing tests, development probes, and the validation scene only
- [ ] Pending

Exit criteria:

- [x] Project compiles without new errors.
- [x] Baseline public signatures and review IDs are captured.
- [x] Lifecycle expectations in Section 5 match observed behavior.
- [x] No unrelated lifecycle defect was folded into this migration.

Rollback: no production runtime changes should exist in this phase.

### Phase 1 — Add the internal normalized lifecycle kernel

Status: [x] Complete

Files:

- add `Assets/Loags/TweenHelper/Runtime/Core/NormalizedTweenTimeline.cs`;
- add its `.meta` file;
- if tests are authorized, add focused internal helper tests under the established test folders with matching `.meta` files.

Tasks:

- [x] Implement the helper semantics from Section 6.2.
- [x] Keep the type internal and avoid changes to runtime assembly references.
- [x] Keep comments sparse; the delegate names expose lifecycle ordering at each call site.
- [x] Add the helper as an isolated internal kernel before family migration.
- [x] Verify that the helper returns a paused root with owner target/link and exact options.
- [x] Verify completion, even Yoyo, rewind, interrupted kill, natural auto-kill, and `Kill(true)` callback routing.
- [x] Verify internal callbacks are not lost or duplicated when `TweenBuilder.BuildSingleTween()` links the returned root.
- [x] Verify nested builder composition still invokes internal cleanup when the outer sequence is killed.

Exit criteria:

- [x] Runtime and Editor assemblies compile.
- [x] The helper itself has no public API surface.
- [x] Family-owned evaluators and endpoint policies remain unchanged.
- [x] Callback counts, nested cleanup, and owner-link behavior are covered by existing tests and the lifecycle probe.

Rollback: remove only the new helper and its `.meta`; no family depends on it yet.

### Phase 2 — Pilot migration: camera feedback

Status: [x] Implementation complete — six camera entries plus the FOV-In coverage variant await manual revalidation

Files:

- modify `Assets/Loags/TweenHelper/Runtime/CameraFeedback/CameraFeedbackUtility.cs`;
- update tests only if authorized.

Why camera is the pilot:

- one shared transient lifecycle covers all six operations;
- every completion, kill, and rewind restores the same captured channels;
- state is single-target and compact;
- exact local position, rotation, and FOV make regressions measurable;
- it proves lazy capture and callback preservation before multi-target/state-subset cases.

Tasks:

- [x] Replace only `CreateTransient` timeline plumbing with `NormalizedTweenTimeline.Create`.
- [x] Keep `CameraState`, evaluators, validation, FOV clamping, Focus Zoom capture, and all defaults unchanged.
- [x] Pass a linear-root options copy.
- [x] Preserve evaluation at progress zero on start.
- [x] Supply restore for forward completion, completion-at-start, rewind, and interrupted kill.
- [ ] Verify all six review animations visually.
- [ ] Verify kill at approximately 10%, 50%, and 90% restores exact local pose/FOV.
- [ ] Verify Restart and Yoyo loops do not drift.
- [ ] Verify owner destruction kills the root and restores through the existing interrupted-kill path.
- [x] Verify caller `OnComplete`/`OnKill` callbacks fire once and after internal finalization.

Exit criteria:

- [x] Camera public signatures and documented defaults are unchanged.
- [x] Camera review IDs are unchanged; their seven persisted statuses are intentionally reset for manual review.
- [x] Static/runtime validation and Unity compilation pass.
- [x] Unity MCP unavailability is recorded; the open Editor compiled successfully without new script errors.
- [ ] Camera visual baselines require Lucas's review of the seven `Needs Work` entries.

Rollback: revert the camera utility migration; the unused helper may remain or be reverted independently.

### Phase 3 — Gameplay-feedback migration

Status: [x] Implementation complete — 22 gameplay-feedback review entries await manual revalidation

Files:

- modify `Assets/Loags/TweenHelper/Runtime/Feedback/FeedbackSequenceUtility.cs`;
- update tests only if authorized.

Migration order:

1. transient feedback (`CreateTransient`);
2. Pickup Collect as a separate sub-step.

Tasks for transient feedback:

- [x] Replace repeated normalized root/callback plumbing with the shared helper.
- [x] Keep `FeedbackState` idempotence and all pose/color evaluators unchanged.
- [x] Restore all captured state on both completion directions, rewind, and interrupted kill.
- [x] Verify representative UI/world targets and exact baseline restoration through the lifecycle probe and review coverage validator.

Tasks for Pickup Collect:

- [x] Use exact destination/hidden state for forward completion.
- [x] Use full invocation restore for even Yoyo completion and rewind.
- [x] Use visuals-only restoration for interrupted kill, preserving current path position.
- [x] Preserve local/anchored versus world bindings and lazy alpha capture.
- [x] Verify `Kill(true)` leaves the completion endpoint rather than applying interrupted cleanup.

Exit criteria:

- [x] All 22 current gameplay-feedback review entries retain their IDs.
- [x] Transient and Pickup lifecycle classes match Section 5.3.
- [x] No feedback API/default/documented timing changes.
- [x] Compilation, existing tests, and development probes pass.
- [ ] Visual equivalence requires Lucas's review of the 22 `Needs Work` entries.

Rollback: transient and Pickup migrations should be separable commits or, at minimum, separable diff hunks with validation between them.

### Phase 4 — TMP text/value migration

Status: [x] Implementation complete — 31 TMP text/value review entries await manual revalidation

Files:

- modify `Assets/Loags/TweenHelper/Runtime/TextAnimations/TMPTextAnimationUtility.cs`;
- do not redesign `TMPCharacterMeshState.cs`;
- update tests only if authorized.

Tasks:

- [x] Replace the family-local `CreateTimeline` implementation with calls to the shared helper.
- [x] Remove the family-local even-Yoyo helper after mapping completion-at-invocation explicitly.
- [x] Preserve progress-zero evaluation at start.
- [x] Map each operation exactly to the contract in Section 5.4.
- [x] Keep Typewriter and Number Count interrupted-kill callbacks null.
- [x] Keep mesh effects restoring on interruption.
- [x] Keep Character Stagger Out's hidden completion distinct from its invocation restore.
- [x] Keep Score Increase's visuals-only interrupted cleanup.
- [x] Keep Scramble Reveal's exact rich-text source restoration.
- [x] Verify `TextMeshProUGUI` behavior through the lifecycle probe and world-space `TextMeshPro` through the expanded review validator.

Exit criteria:

- [x] All 31 current text/value review entries retain their IDs.
- [x] Rich text, mesh colors, visibility, and exact numeric formatting pass static/runtime coverage.
- [x] Partial kill behavior for Typewriter and Score remains distinct and correct; Number Count continues to use its null interrupted-kill policy.
- [x] Compilation, existing tests, and development probes pass.
- [ ] Visual equivalence requires Lucas's review of the 31 `Needs Work` entries.

Rollback: revert only `TMPTextAnimationUtility.cs`; state classes and public builders remain untouched.

### Phase 5 — Production UI sequence migration

Status: [x] Implementation complete — 39 production UI review entries await manual revalidation

Files:

- modify `Assets/Loags/TweenHelper/Runtime/UISequences/UISequenceUtility.cs`;
- run the existing `UISequencePhaseValidation` without changing its expected contract;
- update tests only if authorized.

Tasks:

- [x] Replace the family-local timeline root with the shared helper.
- [x] Adapt normalized progress back to elapsed semantic time as `progress * totalDuration`.
- [x] Preserve lazy initialization of the panel and all secondary participants.
- [x] Supply exact forward and completion-at-start callbacks.
- [x] Supply invocation restoration for rewind.
- [x] Supply no interrupted-kill callback so partial UI state is preserved.
- [x] Preserve optional backdrop, child stagger, incoming/outgoing, cache, and pivot behavior.
- [x] Run the existing validator checks for exact endpoints, kill preservation, rewind, restart drift, and unscaled time.
- [x] Verify Modal, Dropdown, Tab, Drawer, Bottom Sheet, Page Push, and Page Cross Fade representative paths through the UI validator and review coverage pass.
- [x] Preserve primary-owner linking without introducing secondary-target ownership.

Exit criteria:

- [x] All 39 current UI-sequence review IDs are unchanged.
- [x] The existing UI Sequence Phase Validation passes.
- [x] Interrupted kill still preserves current state for every participant.
- [x] Even Yoyo and rewind restore invocation state.
- [x] Compilation, existing tests, and development probes pass.
- [ ] Visual equivalence requires Lucas's review of the 39 `Needs Work` entries.

Rollback: revert only the UI utility migration. No UI state/cache type should require rollback because it is not redesigned.

### Phase 6 — Spatial collection migration

Status: [x] Implementation complete — seven spatial collection review entries await manual revalidation

Files:

- modify `Assets/Loags/TweenHelper/Runtime/Stagger/SpatialCollectionRecipeUtility.cs`;
- leave `TweenStaggerBuilder.cs` and preset-based recipes unchanged;
- update tests only if authorized.

Tasks:

- [x] Replace normalized root and lifecycle plumbing with the shared helper.
- [x] Convert normalized progress to the existing elapsed schedule using `progress * totalDuration`.
- [x] Preserve per-item lazy snapshots, local/world bindings, radial fallback, alpha binding, and all phase eases.
- [x] Forward completion: Burst In restores authored state; Burst Out/Gather apply exact hidden endpoints.
- [x] Completion-at-start, rewind, and interrupted kill restore every captured item.
- [x] Preserve direct recipe behavior by explicitly playing the helper's paused root before returning `TweenHandle`.
- [x] Keep the empty-collection warning/null-handle path unchanged.
- [x] Verify returned handles are active immediately for non-empty direct recipes.
- [x] Verify direct `Kill()` restores every captured item while retaining the existing primary-owner link.
- [x] Preserve stagger timing and total duration.

Exit criteria:

- [x] All seven spatial collection review IDs are unchanged.
- [x] Preset-based stagger behavior and APIs are untouched.
- [x] Direct-play, completion, rewind, kill, and Yoyo contracts match baseline.
- [x] Compilation, existing tests, and development probes pass.
- [ ] Visual equivalence requires Lucas's review of the seven `Needs Work` entries.

Rollback: revert only the spatial utility migration.

### Phase 7 — Destination/stagger exemption audit and narrow cleanup

Status: [x] Complete — destination motion and preset stagger intentionally exempt

Tasks:

- [x] Re-audit `DestinationMotionUtility.CreatePathTween` after the shared helper is proven across Phases 2–6.
- [x] Keep destination paths out of the helper because their root-eased/internal-eased modes and kill-preserve-current semantics are meaningfully different.
- [x] Keep Destination Hop outside the helper.
- [x] Keep `TweenStaggerBuilder` outside the helper.
- [x] Leave the destination-local even-Yoyo predicate in place to avoid coupling an exempt family to the new helper for one trivial predicate.
- [x] Record destination path lifecycle plumbing as intentional duplication rather than expanding helper configuration.
- [x] Remove only family-local lifecycle code made unused by completed migrations.
- [x] Do not refactor `ResolveStrength`, `EaseValue`, family validation, or duration helpers in this phase.

Exit criteria:

- [x] Destination and stagger contracts are explicitly documented as intentionally exempt.
- [x] No speculative abstraction is introduced.
- [x] All 26 current destination entries and all 36 current collection/stagger entries retain stable IDs and unchanged runtime implementations outside the seven spatial recipes.

Rollback: any optional narrow cleanup is its own commit and can be reverted without undoing family migrations.

### Phase 8 — Documentation, full regression, and handoff

Status: [x] Implementation handoff complete — manual visual acceptance remains

Documentation files to review:

- `Assets/Loags/TweenHelper/Documentation/DestinationMotion.md`;
- `Assets/Loags/TweenHelper/Documentation/FeedbackSequences.md`;
- `Assets/Loags/TweenHelper/Documentation/UISequences.md`;
- `Assets/Loags/TweenHelper/Documentation/TextAndValueAnimations.md`;
- `Assets/Loags/TweenHelper/Documentation/CameraFeedback.md`;
- `Assets/Loags/TweenHelper/Documentation/StaggeredCollections.md`;
- development README/handoff documents only where their current user-owned state allows safe merging.

Tasks:

- [x] Document primary-owner linking versus required secondary-participant lifetime.
- [x] Document interrupted-kill policies where they were not already explicit.
- [x] Document concurrent-writer responsibility and channels written by each family.
- [x] Keep the helper internal and omit it from public API examples.
- [x] Update this plan's phase statuses and record intentional exemptions.
- [x] Compare public signatures before/after.
- [x] Compare all 474 review IDs before/after; the refactor delta is zero.
- [x] Rebaseline from the separately completed review-coverage task and confirm the catalog remains 474.
- [x] Run all existing authorized test/validation paths.
- [x] Attempt Unity MCP and record its `401 Unauthorized` result; use the open Editor for imports, compilation, validators, tests, and log inspection.
- [x] Exercise representative entries from every migrated family through development validators without changing their IDs.
- [x] Compile C# through the active Unity Editor.
- [x] Run `git diff --check` and scan changed code for conflict markers and temporary files.
- [x] Validate Markdown links.
- [x] Record Unity MCP as unavailable while retaining successful direct Unity Editor validation.

Exit criteria:

- [x] All machine-verifiable compatibility requirements pass.
- [x] No new public method or review entry was introduced by the refactor.
- [x] No unrelated user change was overwritten.
- [x] Documentation matches the final implementation.
- [x] The final diff is phase-scoped and reviewable.
- [ ] Lucas completes visual acceptance for the 106 affected `Needs Work` entries.

## 10. Lifecycle validation matrix

The review scene validates visual playback, not the entire lifecycle. Lifecycle scenarios should be verified separately and should not become fake animation entries in the review catalog.

### 10.1 Required scenarios

For each lifecycle class, validate:

| Scenario | Check |
|---|---|
| Build only | Target state is unchanged; root is paused. |
| Delayed start | Capture occurs after delay/preceding step, not at builder configuration. |
| Progress 0 | Expected start state is applied only for families that currently do so. |
| Progress 10/50/90% | Intermediate state is finite and visually consistent. |
| Normal completion | Exact family endpoint. |
| `Kill(false)` at 10/50/90% | Exact family interruption policy. |
| `Kill(true)` mid-play | Completion endpoint/policy, then one kill callback; no interrupted cleanup. |
| Rewind mid-play | Exact invocation state where promised. |
| Rewind after completion | Use `SetAutoKill(false)` and verify exact invocation state. |
| Restart after completion | Same path and endpoint without drift. |
| Restart loop x2 | No accumulated offsets/deformation. |
| Yoyo x2 | Exact invocation state. |
| Yoyo x3 | Exact forward endpoint where the family has one. |
| Infinite Restart loop then kill | Root stops and family interruption policy runs once. |
| Owner destruction | Root becomes inactive and cleanup policy runs. |
| Direct extension | Same behavior as builder single-step use. |
| `Then()` composition | Lazy capture uses the state produced by the preceding step. |
| `With()` composition | Root callbacks and cleanup remain attached under an outer sequence. |
| Delay/ID/update/unscaled | Options apply to the complete semantic root. |
| Caller callbacks | Internal finalization first; caller callbacks additive and exactly once. |
| Await completion | Completes on semantic root completion. |
| Await cancellation | Existing cancellation behavior kills the root and invokes the correct family cleanup. |

### 10.2 Representative operation set

Use at least these representatives; add more where a state policy is unique:

| Lifecycle class | Representatives |
|---|---|
| Uniform transient restore | Camera Impact, Camera Focus Zoom, Damage Hit |
| Preserve path / restore visuals | Pickup Collect world and UI |
| TMP partial preservation | Typewriter, Number Count, Score Increase |
| TMP exact restoration | Character Wave, Color Sweep, Scramble Reveal, Character Stagger Out |
| UI preserve-current kill | Toast Hide, Modal Open/Close, Page Push |
| Spatial authored restore | Burst In |
| Spatial hidden endpoint | Burst Out, Gather To |
| Destination regression-only | Arc, internally eased Spring/Spiral, Hop |
| Preset stagger regression-only | List Stagger and one grid pattern |

### 10.3 Exact-state tolerances

Use explicit tolerances consistently:

- position: `0.001` world/local units unless UI canvas scale requires a documented `0.01` tolerance;
- scale: `0.001`;
- quaternion angle: `0.01` degrees;
- alpha/color channel: `0.001`;
- camera FOV: `0.001` degrees;
- TMP source text and formatted terminal text: exact string equality;
- `maxVisibleCharacters`: exact integer equality;
- destination endpoints: exact assignment should still be asserted within numeric tolerance.

## 11. Test authorization and fallback validation

The repository contains existing EditMode and PlayMode tests, but repository policy says to add tests only when explicitly authorized.

If authorization is granted, prefer:

- a focused EditMode test file for the internal helper's callback routing and option application using deterministic/manual DOTween progress;
- a focused PlayMode test file for semantic family state, owner destruction, TMP meshes, camera FOV, and nested sequence behavior;
- reuse of existing fixtures/utilities rather than duplicate production logic;
- one test per distinct lifecycle policy, plus parameterized cases where safe.

If authorization is not granted:

- do not commit new test files or test cases;
- use the existing `TweenLifecyclePlayTests` and `UISequencePhaseValidation` as available;
- use temporary, non-committed probes only when necessary;
- use the review scene for visual regression;
- record every unverified lifecycle branch in the phase handoff.

## 12. Compatibility checklist

Before declaring the refactor complete:

### Public surface

- [x] No public type, method, property, overload, default parameter, namespace, or XML API signature changed.
- [x] Direct extensions still return `TweenHandle`.
- [x] Builder methods still return `TweenBuilder` and compose with `Then()`/`With()`.
- [x] The built-in preset count remains 300.

### Options and timing

- [x] Explicit duration precedence is unchanged.
- [x] Semantic fallback durations are unchanged.
- [x] Internal phase eases are unchanged.
- [x] Linear roots remain linear where currently intentional.
- [x] Delay, loops, ID, update type, unscaled time, strength, and family-supported options are unchanged.
- [x] Speed-based rejection remains family-specific and unchanged.

### Lifecycle

- [x] Completion endpoints match Section 5.
- [x] Interrupted kill policies match Section 5.
- [x] Rewind and restart remain drift-free in existing validation paths.
- [x] Even finite Yoyo ends at invocation where promised.
- [x] Natural completion/auto-kill does not execute interrupted cleanup.
- [x] `Kill(true)` does not execute interrupted cleanup after completion.
- [x] Owner destruction kills the root through the preserved `WithDefaults`/`SetLink` path.
- [x] Required secondary lifetime remains an explicit caller contract.

### Review and docs

- [x] All 474 pre-refactor review IDs still exist exactly once.
- [x] No lifecycle scenario was added as a redundant animation review entry.
- [x] Persisted review results remain readable; only the exact affected set is reset for revalidation.
- [x] Documentation describes final behavior, not the helper's internal mechanics alone.

## 13. Risks and mitigations

| Risk | Mitigation |
|---|---|
| Internal `OnKill` callback lost when builder relinks root | Characterize single/nested builder behavior in Phase 0; register helper callbacks before return; verify exactly-once execution in Phase 1. |
| Natural auto-kill mistaken for interruption | Set `completed = true` before completion state mutation; guard interrupted cleanup. |
| `Kill(true)` performs both endpoint and restore | Treat completion as authoritative before kill; validate explicitly. |
| State captured too early | Initialize only from actual start/value evaluation; test after a preceding builder step. |
| Root ease applied twice or changed | Continue `applyBuilderOptions: false`; pass explicit root options once; retain family evaluator easing. |
| UI kill begins restoring participants | Pass no interrupted-kill callback for UI. |
| Pickup loses current path position on kill | Keep visuals-only interrupted cleanup. |
| Typewriter/Number Count kill begins restoring | Preserve their null interrupted-kill actions. |
| Spatial recipes stop auto-playing | Explicitly play the shared helper root in the direct recipe utility. |
| Spatial stagger timing changes | Convert normalized progress back to exact elapsed `totalDuration`. |
| Destination behavior accidentally normalized like other families | Keep it exempt unless a later proof demonstrates exact compatibility. |
| Secondary destruction behavior changes accidentally | Keep the single-owner contract and do not add participant guards in migration diffs. |
| Review validation masks production lifecycle bugs | Use the separate lifecycle matrix; do not rely on review reset snapshots as proof. |
| Dirty worktree causes overwritten user changes | Scope phase diffs, inspect status before/after, and never restore unrelated files. |

## 14. Rollback strategy

Use the following commit/rollback boundaries:

1. Baseline/characterization records only.
2. Internal helper only.
3. Camera pilot.
4. Feedback transient.
5. Feedback Pickup Collect.
6. TMP text/value.
7. UI sequences.
8. Spatial collections.
9. Optional destination loop-predicate cleanup only.
10. Documentation/final validation.

Never mix visual retuning, new animations, new review variants, or unrelated bug fixes into these commits. If a phase fails its exit gate, revert that phase and keep the last validated phase as the working baseline.

## 15. Definition of done

The refactor is complete when:

- the shared internal normalized timeline owns only genuinely common lifecycle mechanics;
- camera, feedback, TMP, UI, and spatial utilities use it where behavior is equivalent;
- family state/evaluation/endpoint rules remain explicit at each call site;
- destination motion and preset stagger are either intentionally exempt or only narrowly share a proven loop predicate;
- all public APIs and documented defaults are unchanged;
- all lifecycle policies in Section 5 are verified in proportion to risk;
- all existing review IDs and all unaffected persisted results are preserved;
- the exact 106 migrated review entries are marked `Needs Work` for visual acceptance;
- compilation, existing validation, authorized tests, static checks, and documentation-link checks pass;
- Unity Editor validation is performed through Unity MCP when available, or its absence is explicitly reported;
- no unrelated user work is changed;
- this document records every completed phase and any intentional deviation.

## 16. Final implementation handoff template

Implementation handoff — 2026-08-13:

- Completed phases: 0–8.
- Intentionally skipped phases: none; Phase 7 concluded with the planned exemption decision rather than a destination/stagger migration.
- Helper file and final API shape: internal `NormalizedTweenTimeline` with delegate-based initialize, normalized evaluate, forward completion, completion-at-invocation, rewind, interrupted-kill, and optional start policies. It returns a paused owner-linked root.
- Migrated families: camera feedback, gameplay feedback, TMP text/value, production UI sequences, and spatial collection recipes.
- Explicit exemptions: destination paths, destination Hop, and `TweenStaggerBuilder`.
- Secondary-target policy: the primary owner controls root lifetime; callers must keep secondary participants alive for the tween lifetime.
- Automated tests authorized: no new tests were explicitly authorized or added.
- Existing tests run and results: EditMode 25/25 passed; PlayMode 8/8 passed.
- Existing validators: UI Sequence Phase Validation passed; Animation Review Coverage Validation passed for all 474 entries; Animation Lifecycle Refactor Validation passed all lifecycle probes.
- Unity MCP/Editor validation performed: Unity MCP returned `401 Unauthorized`; Unity 6000.5.2f1 was controlled directly to refresh, compile, run tests/validators, and inspect logs. Compilation completed successfully.
- Manual review entries sampled: the prior user review and completed 474-entry coverage pass provide the baseline; post-refactor visual acceptance is intentionally handed back rather than pre-approved.
- Review ID count before/after: 474 / 474, all unique; refactor delta zero.
- Review status reset: the final catalog-backed manifest contains exactly 106 existing IDs. The final guarded pass changed zero and confirmed all 106 already at `Needs Work`; non-catalog keys are absent and unaffected keys were not written.
- Public signature comparison result: no existing public/protected signature line changed in the five migrated runtime utilities; the new helper type is internal.
- Known remaining risks: visual equivalence and subjective timing feel still require the requested manual review; secondary-target destruction and concurrent writers remain documented caller responsibilities.
- Suggested next task: open `TweenHelperPresetReview`, select `NEEDS WORK`, and manually validate the 106 affected entries.
