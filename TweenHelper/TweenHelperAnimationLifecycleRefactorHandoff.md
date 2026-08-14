# TweenHelper Animation Lifecycle Refactor — Planning Handoff

## Purpose of this document

This is context for a future agent that will **plan**, but not immediately implement, a refactor and cleanup roadmap for TweenHelper's advanced animation families.

The intended outcome of that future planning task is to determine whether repeated lifecycle and normalized-timeline code should be consolidated into internal compositional utilities while preserving all existing public APIs, visual behavior, review IDs, completion semantics, and cancellation semantics.

Do not assume that every animation should be rebuilt from the 300 presets. The central question is how to share lifecycle mechanics safely without erasing meaningful differences between animation families.

## Product and implementation state

TweenHelper is a Unity animation helper built on DOTween. It currently contains:

- 300 registered `ITweenPreset` implementations for primarily single-object motions.
- UI recipes.
- Staggered collection recipes and configurable ordering/pattern variants.
- Destination-aware motion.
- Gameplay-feedback sequences.
- Production UI sequences.
- TextMesh Pro text/value animations.
- Camera-feedback sequences.
- A review scene that plays each catalog entry individually and stores review status by stable ID.

At the completion of the most recent large implementation, the review catalog contained 398 entries:

- 300 presets
- 13 UI recipes
- 19 collection entries
- 16 destination-motion entries
- 16 gameplay-feedback entries
- 15 production UI sequences
- 13 text/value animations
- 6 camera-feedback entries

The user subsequently reported that the new animations look good. A future agent should inspect the persisted review data rather than assuming the exact current reviewed/unreviewed count.

## Most recently added advanced animation pack

The latest implementation added 36 review entries across six packs.

### Text and value

- `TextCharacterStaggerOut`
- `TextCharacterBounce`
- `TextColorSweep`
- `TextGlitch`
- `TextEmphasis`
- `TextScrambleReveal`

### Gameplay feedback

- `HealReceive`
- `ShieldBlock`
- `CriticalHit`
- `CooldownReady`
- `LevelUp`
- `LowHealthWarning`

### Production UI

- `DrawerShow`
- `DrawerHide`
- `BottomSheetShow`
- `BottomSheetHide`
- `PagePushTo`
- `PageCrossFadeTo`

### Collections

- `GridDiagonalWave`
- `GridSpiral`
- `GridCheckerboard`
- `CollectionBurstIn`
- `CollectionBurstOut`
- `CollectionGatherTo`

### Destination motion

- `PathThrough` / `PathLocalThrough`
- `SpiralTo` / `SpiralLocalTo`
- `MultiHopTo` / `MultiHopLocalTo`

### Camera feedback

- `CameraImpact`
- `CameraRecoil`
- `CameraLandingImpact`
- `CameraFovKick`
- `CameraFocusZoom`
- `CameraBreathing`

## Important distinction: presets versus semantic animations

The architecture is currently a deliberate mix.

The 300 presets are themselves DOTween implementations wrapped as registered `ITweenPreset` objects. Reusing a preset is therefore not what automatically provides cleanup or state restoration.

Most complex semantic animations do **not** call one of the 300 presets. Instead, their normal path is:

```text
Extension method
    -> TweenBuilder family method
    -> family-specific internal utility
    -> DOTween tween/sequence or normalized DOTween.To timeline
    -> TweenOptions and WithDefaults
    -> owner linking
    -> TweenHandle
```

The family-specific builder methods generally call `AddStep(..., applyBuilderOptions: false)` because the internal utility applies the relevant options itself. This avoids applying delays, loops, easing, and other configuration twice.

### Current preset reuse by family

| Family | Directly reuses registered presets? | Notes |
|---|---:|---|
| List/grid stagger recipes | Yes | Commonly uses `PopInFadePreset`, `PopOutFadePreset`, `PulseScalePreset`, or `PulseScaleSoftPreset` per item. |
| Grid diagonal and spiral | Yes | Both schedule `PopInFadePreset` using calculated delay ranks. |
| Grid checkerboard | Yes | Schedules `PulseScalePreset` in two phases. |
| Spatial Burst/Gather | No | Uses a custom normalized timeline and per-item snapshots. |
| Destination motion | No | Uses specialized path evaluation and exact endpoint handling. |
| Gameplay feedback | No | Uses custom synchronized pose/color timelines. |
| Production UI sequences | No | Uses cached shown/invocation poses and multi-target timelines. |
| TMP text/value | No | Uses TMP mesh/text state and normalized evaluation. |
| Camera feedback | No | Uses captured transform/FOV state and normalized evaluation. |

For the most recent 36-entry pack, only `GridDiagonalWave`, `GridSpiral`, and `GridCheckerboard` directly compose registered preset implementations. The other entries use specialized utilities while remaining integrated with TweenHelper.

## Why complex animations should not automatically be rebuilt from presets

Complex semantic families have requirements that differ from ordinary single-object presets:

- Multiple channels must be synchronized precisely.
- Several objects may participate in one semantic operation.
- State often must be captured lazily when the builder step starts, not when the builder is configured.
- Completion can mean restoring the baseline, preserving an endpoint, hiding the object, or restoring only transient visuals.
- Interrupted kill can mean restore everything, preserve current progress, or preserve path position while restoring deformation.
- TMP vertex meshes, camera FOV, anchored UI state, material property blocks, and destination paths need different state models.
- A normalized linear root is often required so each internal phase can apply its own easing without double-warping the complete timeline.

Forcing these operations through several existing presets could create stale state capture, conflicting completion behavior, nested option application, or competing writes to position/scale/color.

## Shared TweenHelper integration already present

Static inspection found that the advanced families are connected to the principal TweenHelper infrastructure:

- They return `TweenHandle`.
- They can be used as `TweenBuilder` steps.
- They support sequential/parallel builder composition.
- They accept `TweenOptions` where appropriate.
- They use `WithDefaults(...)` or the equivalent root configuration path.
- Their root tween is assigned a target and linked to an owner for cleanup when that owner is destroyed.
- Callbacks and awaiting operate on the complete semantic root.
- Family-specific completion, rewind, and interruption behavior is implemented.
- Normalized semantic families deliberately reject speed-based timing where it has no coherent meaning.

Important nuance: semantic utilities frequently force the **root** ease to `Linear` and apply caller-selected or family-selected eases inside individual phases. This is intentional and should not be "fixed" by blindly reapplying the global default ease to the root.

## Cleanup is not the same as visual reset

There are two independent concerns:

1. **Tween/resource cleanup** — stopping and releasing the active DOTween root when explicitly killed or when its linked owner is destroyed.
2. **Visual-state policy** — deciding what target state remains after completion, rewind, or interruption.

The first concern is broadly shared. The second is deliberately family-specific.

### Current lifecycle semantics

| Family | Normal completion | Interrupted kill | Rewind |
|---|---|---|---|
| Transient gameplay feedback | Restores captured position, scale, rotation, and supported color/alpha. | Restores captured state. | Restores captured state. |
| Pickup Collect | Ends at exact destination with hidden scale/alpha endpoint. | Preserves current path position but restores transient visual state. | Restores captured start and visuals. |
| Camera feedback | Restores exact captured local position, rotation, and FOV. | Restores the same captured state. | Restores the same captured state. |
| Transient TMP mesh effects | Restores TMP mesh baseline. | Restores TMP mesh baseline. | Restores TMP mesh baseline. |
| Character Stagger Out | Ends hidden using `maxVisibleCharacters = 0`. | Restores invocation mesh/visibility. | Restores invocation mesh/visibility. |
| Scramble Reveal | Ends with exact source text fully visible. | Restores source text and invocation visibility. | Restores source text and invocation visibility. |
| Score Increase | Leaves exact destination text and restores transient visuals. | Preserves current value while restoring transient visuals. | Restores invocation text and visuals. |
| Destination motion | Ends at exact destination. | Intentionally preserves current position. | Returns through tween progress to the captured start. |
| Destination Hop | Ends at exact destination with scale restored. | Preserves current base path position while removing temporary deformation/grounding offset. | Restores start state. |
| UI show/hide/navigation | Writes the semantic shown/hidden/incoming/outgoing endpoint. | Intentionally preserves current visual state so another transition can continue from it. | Restores every participant's invocation state. |
| Preset-based stagger | Follows child preset behavior. | Does not promise restoration of arbitrary item state. | Follows root/child tween semantics. |
| Spatial Burst/Gather | Burst In restores authored state; Burst Out/Gather end hidden at their requested endpoint. | Restores captured item states. | Restores captured item states. |

Do not establish a universal rule that `Kill()` must reset every property. That would break intentional cancellation and handoff behavior for destination and UI transitions.

## Review-scene behavior versus production behavior

The review controller performs additional cleanup between entries:

- It kills the active handle/tweens.
- It reapplies target snapshots.
- It configures the correct preview group.
- It starts the next requested entry from a known review baseline.

That review harness reset is not the same as production lifecycle behavior. A visually successful normal-playback review does not prove that the animation behaves correctly under:

- kill at 10%, 50%, or 90%;
- rewind before and after completion;
- restart;
- Restart/Yoyo loops;
- owner destruction;
- destruction of a secondary target;
- competing tweens writing the same channel.

Any refactor roadmap should explicitly preserve this distinction.

## Recommended architectural direction

The current specialized-family architecture should remain. The best likely improvement is a small internal compositional abstraction for repeated normalized-timeline and lifecycle plumbing.

Do **not** begin with an inheritance-heavy `BaseAnimationUtility`. The involved state types are too different, and inheritance would likely hide execution semantics behind generic lifecycle hooks.

Prefer an internal delegate/composition-based helper conceptually similar to:

```csharp
NormalizedTweenTimeline.Create(
    owner,
    duration,
    options,
    initialize,
    evaluate,
    complete,
    rewind,
    interruptedKill);
```

Potential responsibilities of this helper:

- Create a normalized `0 -> 1` DOTween timeline.
- Capture state lazily exactly once.
- Track initialized/completed state.
- Apply root `TweenOptions` and TweenHelper defaults exactly once.
- Set root target/link ownership.
- Preserve family-provided `OnKill` behavior when the builder links the returned tween.
- Handle normal completion, rewind, and interrupted kill callbacks consistently.
- Support the existing even-count Yoyo "ends at invocation state" rule.
- Return a paused tween for `TweenBuilder` to own and play.

Family-specific state should remain in composition-oriented objects such as:

- `FeedbackState`
- `UISequenceState`
- `TMPCharacterMeshState`
- camera feedback state
- spatial item state
- destination position bindings and grounded-hop state

An explicit internal lifecycle policy may be useful, but avoid inventing a large enum/configuration graph unless a code audit demonstrates that delegates are insufficient. Likely policies include:

- completion: restore, preserve endpoint, or restore visuals only;
- interrupted kill: restore, preserve current state, or restore visuals only;
- rewind: restore invocation;
- final even Yoyo: restore invocation.

## Known contracts and possible gaps to investigate

### 1. Multi-target lifetime is owner-based

Modal children, dropdown entries, incoming pages, backdrops, and collection items are validated/snapshotted, but the semantic root is linked to one owner. Destroying the owner kills the root. Destroying only a secondary target while the owner survives may not have the same guarantee.

The roadmap should decide whether to:

- keep this as an explicit caller contract;
- stop/kill the root if any required participant is destroyed;
- introduce a narrowly scoped multi-target lifetime mechanism;
- or handle particular families differently.

Do not assume DOTween can attach one tween to several independent `SetLink` owners without investigating its actual semantics.

### 2. Concurrent writers are not arbitrated

TweenHelper does not currently appear to provide channel ownership for position, scale, rotation, alpha, color, TMP mesh vertices, or camera FOV. Two animations writing the same channel can conflict.

The roadmap should distinguish documentation/validation from a genuine channel-locking system. A full arbitration layer may be outside the desired scope.

### 3. Lifecycle behavior has not been validated as comprehensively as visuals

The latest implementation was statically checked and compiled successfully through the generated C# projects. Unity MCP was unavailable during that implementation, so a fresh Unity Editor console/play-mode validation was not performed by the implementing agent. The review scene primarily validates visual playback and resets snapshots between entries.

A future roadmap should define a lifecycle validation matrix. Repository rules require explicit user authorization before adding automated tests.

### 4. Do not conflate global defaults with semantic-family defaults

Explicit method duration generally wins over `TweenOptions.Duration`, which wins over the family default. Semantic families may use internal phase eases and a linear root. A refactor must preserve these precedence rules exactly.

## Suggested audit matrix for roadmap planning

For every public semantic method and configurable variant, record:

| Concern | Questions |
|---|---|
| Construction | Is state captured at builder configuration, build, play, or step start? |
| Root ownership | Which `GameObject` is target/link owner? |
| Participants | Which secondary objects are required, optional, or collection members? |
| Options | Which `TweenOptions` fields are supported, overridden, or rejected? |
| Completion | What exact state must remain? |
| Interrupted kill | Which channels restore and which preserve current progress? |
| Rewind | Does every involved target return to its invocation state? |
| Restart | Is the operation drift-free and based on stable absolute endpoints? |
| Loops | Are Restart and Yoyo semantics correct, including even Yoyo counts? |
| Owner destruction | Is the root killed cleanly? |
| Secondary destruction | What is the defined behavior? |
| Concurrency | Which channels must not be written simultaneously? |
| Review coverage | Is every meaningful enum/direction/mode represented without adding redundant public methods? |

## Review-catalog principle established with the user

The review catalog should be broader than the public method list.

When one public method accepts an enum or other meaningful mode, each visually meaningful configuration should receive its own review entry even when no separate public method is needed. Examples include:

- all four `GridDiagonalDirection` values;
- all four `GridSpiralDirection` values;
- inverted checkerboard;
- different meaningful ripple origins;
- directional UI variants such as drawer edges and page-push direction.

The rule is:

- Same behavior with a configuration difference: reuse the public method and add review variants.
- Genuinely different movement/timing/semantic purpose: consider a new method.

Any lifecycle refactor must preserve stable review IDs and previously stored review results. Only genuinely new review entries should begin unreviewed.

## High-value files to inspect

### Core builder and lifecycle

- `Assets/Loags/TweenHelper/Runtime/Core/TweenBuilder.cs`
- `Assets/Loags/TweenHelper/Runtime/Core/TweenDefaults.cs`
- `Assets/Loags/TweenHelper/Runtime/Core/TweenHandle.cs`
- `Assets/Loags/TweenHelper/Runtime/Core/TweenOptions.cs`
- `Assets/Loags/TweenHelper/Runtime/Core/DoTweenIntegration.cs`
- `Assets/Loags/TweenHelper/Runtime/Core/TweenLifecycleTracker.cs`
- `Assets/Loags/TweenHelper/Runtime/Core/TweenTargetUtility.cs`

### Stagger and collections

- `Assets/Loags/TweenHelper/Runtime/Stagger/TweenStaggerBuilder.cs`
- `Assets/Loags/TweenHelper/Runtime/Stagger/StaggerRecipeExtensions.cs`
- `Assets/Loags/TweenHelper/Runtime/Stagger/SpatialCollectionRecipeUtility.cs`
- `Assets/Loags/TweenHelper/Runtime/Stagger/GridPatternDirection.cs`

### Semantic animation families

- `Assets/Loags/TweenHelper/Runtime/DestinationMotion/DestinationMotionUtility.cs`
- `Assets/Loags/TweenHelper/Runtime/DestinationMotion/TweenBuilder.DestinationMotion.cs`
- `Assets/Loags/TweenHelper/Runtime/Feedback/FeedbackSequenceUtility.cs`
- `Assets/Loags/TweenHelper/Runtime/Feedback/TweenBuilder.Feedback.cs`
- `Assets/Loags/TweenHelper/Runtime/UISequences/UISequenceUtility.cs`
- `Assets/Loags/TweenHelper/Runtime/UISequences/TweenBuilder.UISequences.cs`
- `Assets/Loags/TweenHelper/Runtime/TextAnimations/TMPTextAnimationUtility.cs`
- `Assets/Loags/TweenHelper/Runtime/TextAnimations/TMPCharacterMeshState.cs`
- `Assets/Loags/TweenHelper/Runtime/TextAnimations/TweenBuilder.TextAnimations.cs`
- `Assets/Loags/TweenHelper/Runtime/CameraFeedback/CameraFeedbackUtility.cs`
- `Assets/Loags/TweenHelper/Runtime/CameraFeedback/TweenBuilder.CameraFeedback.cs`

### Review and documentation

- `Assets/_Project/TweenHelperDevelopment/Validation/PresetReviewController.cs`
- `Assets/_Project/TweenHelperDevelopment/Validation/Editor/PresetReviewSceneBuilder.cs`
- `Assets/_Project/TweenHelperDevelopment/Validation/Scenes/TweenHelperPresetReview.unity`
- `Assets/Loags/TweenHelper/Documentation/StaggeredCollections.md`
- `Assets/Loags/TweenHelper/Documentation/DestinationMotion.md`
- `Assets/Loags/TweenHelper/Documentation/FeedbackSequences.md`
- `Assets/Loags/TweenHelper/Documentation/UISequences.md`
- `Assets/Loags/TweenHelper/Documentation/TextAndValueAnimations.md`
- `Assets/Loags/TweenHelper/Documentation/CameraFeedback.md`

## Repository constraints for a future implementation

- Preserve Unity `.meta` files for anything added, moved, or renamed under `Assets`.
- Do not modify vendor/third-party areas unless explicitly required.
- Preserve unrelated user changes in a dirty worktree. Previous work observed unrelated CLI/telemetry changes under the TweenHelper development area.
- Prefer composition over inheritance.
- Keep public APIs stable unless a roadmap identifies and justifies a breaking change.
- Only add tests when the user explicitly authorizes them.
- Prefer Unity MCP for Editor state, scene wiring, imports, and console validation when available.
- Do not run a Unity batch build unless explicitly requested.
- Preserve all established visual endpoints and review-status IDs.

## Requested output from the future roadmap agent

The future agent should first perform a read-only code audit and then produce a staged implementation roadmap containing:

1. An inventory of duplicated lifecycle/timeline code.
2. A behavior contract for each family before refactoring.
3. A minimal internal composition design, with alternatives and tradeoffs.
4. A migration order beginning with one low-risk pilot family.
5. Compatibility requirements for public methods, options, callbacks, loops, and review IDs.
6. A secondary-target lifetime decision.
7. A validation strategy covering completion, kill, rewind, restart, loops, destruction, and exact endpoints.
8. Documentation changes required by the refactor.
9. Rollback boundaries so each phase remains independently reviewable.
10. Explicit identification of anything that should remain duplicated because sharing it would obscure semantics.

The roadmap should not recommend a broad rewrite merely to reduce line count. Its goal is lifecycle consistency, auditability, and safer future animation expansion.

## Working conclusion from this discussion

The existing direction is sound:

- Use presets for genuinely preset-shaped operations.
- Use specialized semantic utilities for coordinated animations.
- Keep family-specific state and endpoint rules.
- Consolidate only the repeated normalized timeline and lifecycle plumbing where the audit proves behavior is equivalent.
- Prefer internal composition over a shared inheritance hierarchy.
- Treat owner cleanup and visual reset as separate policies.
- Expand review coverage for meaningful configurations without multiplying redundant public methods.

