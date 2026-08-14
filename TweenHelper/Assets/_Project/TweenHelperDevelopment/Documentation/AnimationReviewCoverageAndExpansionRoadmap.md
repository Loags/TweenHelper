# Animation Review Coverage and Expansion Roadmap

- Status: **PHASE 1 IMPLEMENTED — AUTOMATED VALIDATION PASSED, MANUAL REVIEW PENDING**
- Last updated: 2026-08-13
- Previous validated baseline: **398 entries**
- Current review catalog: **474 entries**
- Pending manual review: **76 new entries**
- Scope: Manual-review completeness for existing semantic animation APIs, followed by staged implementation of new collection, gameplay, UI, text, destination, and camera animation families.

## Purpose

This roadmap is the implementation and validation checklist for the next TweenHelper animation iterations. It separates three concepts that intentionally have different sizes:

1. **Public runtime API** — stays compact and semantic.
2. **Review catalog** — contains every visually meaningful discrete configuration, including enum variants that call the same public method.
3. **Showcase and Preset Browser** — remain curated and discovery-focused rather than duplicating the complete review matrix.

The review scene is the exhaustive visual-validation surface. A direction, ordering mode, interpolation mode, signed motion mode, or target-context branch does not need a direction-specific public method, but it does need a stable review entry when its appearance or runtime path is meaningfully different.

Example:

```csharp
items.GridDiagonalWave(owner, columns, GridDiagonalDirection.TopRightToBottomLeft);
```

This call should have its own review entry, but TweenHelper must not add a redundant method such as `GridDiagonalTopRightToBottomLeft`.

## Current catalog

The current review scene contains:

| Category | Entries |
| --- | ---: |
| Built-in presets | 300 |
| UI recipes | 13 |
| Collection and stagger entries | 36 |
| Destination motion | 26 |
| Gameplay feedback | 22 |
| Production UI sequences | 39 |
| Text and value animations | 31 |
| Camera feedback | 7 |
| **Total** | **474** |

## Phase 1 implementation record

Phase 1 was implemented on 2026-08-13 without adding any public direction-specific aliases. The review controller now stores variant direction, interpolation, signed magnitude, topology, context, and optional-branch data privately while continuing to dispatch through the existing semantic runtime methods.

- Added exactly 76 stable review IDs; all 98 legacy semantic IDs and 300 preset IDs remain present and unchanged.
- Added an eight-item, three-column incomplete grid; six separated world collection targets; a dedicated drawer backdrop; and a world-space `TextMeshPro` fixture.
- Added interpolation-aware and signed destination guides.
- Extended snapshot, reset, kill, visibility, and replay handling to every new fixture.
- Kept the Preset Browser and showcase curated because no new semantic runtime family was introduced in Phase 1.
- Added [AnimationReviewCoverageValidation](../Validation/Editor/AnimationReviewCoverageValidation.cs), which validates catalog identity, scene wiring, signed destination configuration and trajectories, inward FOV behavior, and smoke playback of all 76 entries without writing manual review statuses.
- Unity compilation, scene regeneration, automated playback validation, and Console inspection passed. Manual visual judgment remains intentionally pending.

Primary implementation surfaces:

- [PresetReviewController](../Validation/PresetReviewController.cs)
- [Preset review scene](../Validation/Scenes/TweenHelperPresetReview.unity)
- [PresetReviewSceneBuilder](../Validation/Editor/PresetReviewSceneBuilder.cs)
- [Grid pattern directions](../../../Loags/TweenHelper/Runtime/Stagger/GridPatternDirection.cs)
- [UI sequence directions](../../../Loags/TweenHelper/Runtime/UISequences/UISequenceDirection.cs)
- [Destination path interpolation](../../../Loags/TweenHelper/Runtime/DestinationMotion/DestinationPathInterpolation.cs)

## Tracking legend

- `[ ]` Not started
- `[~]` In progress
- `[x]` Implemented and statically verified
- Manual validation remains incomplete until the user marks every new entry Passed or Failed in the review scene.

For every implementation slice, track these independently:

- Runtime/API
- Review entry
- Review fixture and reset behavior
- Preset Browser or showcase exposure where appropriate
- Documentation
- Compilation and static verification
- User manual validation

## Review coverage policy

### Always create a separate review entry for

- Every value of a public animation enum when it changes visible behavior.
- Both states of a boolean mode when the visual ordering or terminal state changes.
- Each show/hide or in/out operation when their timelines differ.
- Each world/local or UI/3D path when the runtime uses a different position, scale, renderer, or target-binding branch.
- Supported signed inputs when the sign produces a qualitatively different path, such as upward/downward arcs or clockwise/counterclockwise winding.
- Representative topology branches such as complete and incomplete grid rows.
- Runtime execution paths that are not otherwise manually exercised, such as dynamic preset lookup and custom stagger tween factories.

### Do not create separate review entries merely for

- Different durations, amplitudes, distances, colors, seeds, or strength values.
- `Component` versus `GameObject` convenience overloads that delegate to the same implementation.
- Direct extension versus `TweenBuilder` syntax when both execute the same operation.
- Every possible arbitrary vector, origin index, numeric count, formatter, or waypoint list.
- External DOTween `Ease`, `LoopType`, and update-mode permutations.

For continuous inputs, use representative equivalence cases instead of an exhaustive matrix. For example, a grid ripple needs center, corner, and edge origins, not one entry for every cell.

## Stable review identity rules

- Do not rename or remove any of the existing 398 review IDs.
- Existing default entries retain their current ID and validation result.
- New variants append a stable configuration suffix.
- Only newly introduced IDs start as Unreviewed.
- Marking an entry must continue advancing to and automatically playing the next filtered entry.

Examples:

```text
Collection:GridDiagonalWave
Collection:GridDiagonalWave:TopRightToBottomLeft
Collection:GridDiagonalWave:BottomLeftToTopRight
Collection:GridDiagonalWave:BottomRightToTopLeft

UISequence:DrawerShow
UISequence:DrawerShow:Up
UISequence:DrawerShow:Down
UISequence:DrawerShow:Right

TextValue:TextWave
TextValue:TextWave:Down
TextValue:TextWave:Left
TextValue:TextWave:Right
```

The unsuffixed existing ID continues representing its existing default configuration.

# Phase 1 — Existing API review-completeness pass

Phase 1 adds **76 review entries** without adding direction-specific public animation methods. Completion raises the review scene from 398 to 474 entries.

## Phase 1A — Discrete enum and boolean variants

Target: **48 new review entries**.

### Collections — 7 entries

The five `StaggerOrder` values and four `GridWaveDirection` values are already represented and require no additions.

- [x] `GridDiagonalWave:TopRightToBottomLeft`
- [x] `GridDiagonalWave:BottomLeftToTopRight`
- [x] `GridDiagonalWave:BottomRightToTopLeft`
- [x] `GridSpiral:OutsideInCounterClockwise`
- [x] `GridSpiral:InsideOutClockwise`
- [x] `GridSpiral:InsideOutCounterClockwise`
- [x] `GridCheckerboard:Inverted`

Implementation rules:

- Continue calling `GridDiagonalWave`, `GridSpiral`, and `GridCheckerboard` with their existing parameters.
- Do not add public aliases for individual directions.
- Give every entry a description that states its traversal start, traversal end, and winding where applicable.

### Destination interpolation — 2 entries

- [x] `PathThrough3D:Linear`
- [x] `PathLocalThroughUi:Linear`

The current entries remain the `CatmullRom` variants. Path guides must render the actual selected interpolation rather than always drawing the Catmull-Rom preview.

### Production UI directions — 24 entries

`UISequenceDirection` contains Up, Down, Left, and Right. Every directional operation must be visible in every supported direction.

#### Toast

- [x] Toast Show Down
- [x] Toast Show Left
- [x] Toast Show Right
- [x] Toast Hide Down
- [x] Toast Hide Left
- [x] Toast Hide Right

#### Tooltip

- [x] Tooltip Show Down
- [x] Tooltip Show Left
- [x] Tooltip Show Right
- [x] Tooltip Hide Down
- [x] Tooltip Hide Left
- [x] Tooltip Hide Right

#### Tab switch

- [x] Tab Switch Up
- [x] Tab Switch Down
- [x] Tab Switch Right

#### Drawer

- [x] Drawer Show Up
- [x] Drawer Show Down
- [x] Drawer Show Right
- [x] Drawer Hide Up
- [x] Drawer Hide Down
- [x] Drawer Hide Right

At least the Right show/hide pair must include the optional backdrop. This covers the drawer backdrop branch without adding extra review entries.

#### Page push

- [x] Page Push Up
- [x] Page Push Down
- [x] Page Push Right

`Page Push Right` also serves as the manual-review example for back-navigation or “page pop” behavior. Do not add a `PagePopTo` public alias unless a later prototype proves that back navigation needs a genuinely different timeline.

### Text directions — 15 entries

Add Down, Left, and Right variants for each operation below. The existing unsuffixed entries remain Up.

- [x] Character Stagger In Down
- [x] Character Stagger In Left
- [x] Character Stagger In Right
- [x] Character Stagger Out Down
- [x] Character Stagger Out Left
- [x] Character Stagger Out Right
- [x] Text Wave Down
- [x] Text Wave Left
- [x] Text Wave Right
- [x] Character Bounce Down
- [x] Character Bounce Left
- [x] Character Bounce Right
- [x] Text Emphasis Down
- [x] Text Emphasis Left
- [x] Text Emphasis Right

All entries call the existing operation with another `UISequenceDirection` value. Do not add direction-named text methods.

## Phase 1B — Representative runtime-path and context variants

Target: **28 new review entries**.

### Collections and stagger execution — 10 entries

- [x] Grid Ripple from a corner origin
- [x] Grid Ripple from an edge origin
- [x] Stagger using `PresetByName`
- [x] Stagger using a custom `Animate` factory
- [x] World-space Collection Burst In
- [x] World-space Collection Burst Out
- [x] World-space Collection Gather To
- [x] UI Collection Burst Out using automatic default distance
- [x] Diagonal wave on an incomplete rectangular grid
- [x] Spiral on an incomplete rectangular grid

Fixture requirements:

- Add an explicit world-space collection preview with enough separation to make position, scale, rotation, and alpha behavior readable.
- Add an incomplete grid fixture, preferably eight items with three columns, so the final row and rectangular traversal are visible.
- Use the automatic distance for the world Burst Out entry so it also validates the world default. The separate UI entry validates the canvas-unit default.
- Dynamic preset and custom tween entries should intentionally produce recognizable but distinct pulses so the selected runtime path is obvious.

### Signed destination motion — 8 entries

Add world and UI/local variants for:

- [x] Downward Arc 3D
- [x] Downward Arc UI
- [x] Downward Hop 3D
- [x] Downward Hop UI
- [x] Downward Multi-Hop 3D
- [x] Downward Multi-Hop UI
- [x] Reverse-winding Spiral 3D
- [x] Reverse-winding Spiral UI

Requirements:

- Use negative height for downward Arc, Hop, and Multi-Hop.
- Use negative revolutions for reverse Spiral winding.
- Destination guides must use the exact signed parameters of the active review entry.
- Final positions and restored scale/orientation must remain exact.

### Alternate gameplay-feedback contexts — 6 entries

- [x] Heal Receive UI
- [x] Shield Block UI
- [x] Critical Hit UI
- [x] Cooldown Ready 3D
- [x] Level Up 3D
- [x] Low Health Warning 3D

The current entries remain unchanged. The UI Shield Block and Critical Hit entries should use an impact direction opposite to the current world examples so they cover both the UI target branch and directional reversal.

### Camera lens direction — 1 entry

- [x] Camera FOV Kick In

Use a negative field-of-view delta. The current positive entry remains the FOV kick outward. The camera must restore its exact captured FOV after completion, rewind, and interrupted playback.

### World-space TextMesh Pro representatives — 3 entries

- [x] World TMP Character Stagger In
- [x] World TMP Color Sweep
- [x] World TMP Scramble Reveal

These representative entries cover mesh displacement, vertex-color mutation, and source-string mutation on `TextMeshPro`, while the current entries continue covering `TextMeshProUGUI`.

## Phase 1 review-controller design

Do not grow the public runtime API to support the review matrix. The review controller may use private data structures and private enums.

Preferred controller direction:

- Preserve the semantic family enum already used to select playback behavior.
- Add private per-entry configuration fields for direction, interpolation, signed values, preview context, and variant suffix.
- Generate review entries from small private configuration tables where several entries share one method.
- Avoid a separate public type for review-only configuration.
- Keep switch logic responsible for semantic operation selection, not for duplicating every direction as a distinct runtime method.

Every `ReviewItem` should be able to provide:

- Stable ID
- Display name
- Description
- Category label
- Preview fixture
- Semantic operation kind
- Variant key
- Relevant direction/interpolation/mode data
- Any representative numeric parameters required by that entry

## Phase 1 scene and fixture work

- [x] Add an incomplete-grid preview group.
- [x] Add a world-space collection preview group.
- [x] Add world-space TextMesh Pro preview targets.
- [x] Ensure a backdrop can be shown for directional drawer reviews.
- [x] Extend target snapshots and reset logic for every new fixture.
- [x] Extend tween-kill cleanup for every new fixture.
- [x] Update preview visibility so only the active fixture is shown.
- [x] Update destination guides for interpolation and signed variants.
- [x] Preserve existing scene and prefab references.
- [x] Preserve all Unity `.meta` files and add them for new assets.

## Phase 1 browser, showcase, and documentation policy

The review scene is exhaustive; the other discovery surfaces are not.

### Preset Browser

- Keep one catalog entry per existing semantic family unless a variant is independently useful for discovery.
- Update descriptions and code snippets to enumerate supported enum values.
- Do not add 24 near-identical UI cards solely because the review scene contains 24 direction configurations.
- Ensure previews can still render the default family example.

### Showcase

- Keep representative examples rather than the complete matrix.
- Add a world collection or world TMP example only if it materially improves discoverability.
- Avoid turning the showcase into a second validation scene.

### Documentation

- Enumerate every public enum value.
- Explain that each enum configuration is manually covered even when only one browser card exists.
- Document signed path inputs with upward/downward and winding examples.
- Document world/local spatial collection defaults.
- Update the review totals only after the implementation count is verified from code.

## Phase 1 verification gate

### Static and compile verification

- [x] Confirm the review catalog contains 474 unique IDs.
- [x] Confirm all previous 398 IDs still exist unchanged.
- [x] Confirm exactly 76 new IDs initially resolve to Unreviewed.
- [x] Confirm no duplicate review IDs.
- [x] Confirm every public animation enum value has the planned visual coverage.
- [x] Confirm all new scene references resolve.
- [x] Confirm all new Unity assets have unique `.meta` GUIDs.
- [x] Compile runtime, editor, demo, and validation assemblies.
- [x] Run `git diff --check`.
- [x] Check Unity Console compilation errors through the available Editor connection (Unity Pipeline; Unity MCP was unavailable).
- [x] Do not run a batch build unless explicitly requested.

### Manual validation gate

- [ ] Open `TweenHelperPresetReview.unity`.
- [ ] Select `NOT REVIEWED`.
- [ ] Validate all 76 additions one by one.
- [ ] Confirm state restoration by replaying representative transient entries.
- [ ] Confirm incomplete grid traversal does not skip or reorder the final row incorrectly.
- [ ] Confirm left/right and clockwise/counterclockwise variants are true reversals.
- [ ] Confirm vertical UI transitions remain inside the visible review framing.
- [ ] Confirm all 76 entries are marked Passed or Failed before Phase 2 implementation begins.

# Phase 2 — New collection and production UI behavior

Phase 2 begins only after the Phase 1 manual gate. Every new semantic method must be accompanied by all required review configurations in the same implementation slice.

## Collections

### Grid Serpentine

Visual behavior: Traverse alternating rows or columns like a snake, reversing direction at each boundary.

API direction:

- One `GridSerpentine` public family.
- Use a compact traversal enum or mode object to select row/column traversal and starting corner.
- Do not add one public method per corner.

Review requirements:

- [ ] Every discrete traversal enum value
- [ ] At least one incomplete rectangular grid
- [ ] Clearly visible numbering or labels so traversal order can be judged

### Grid Ring Wave

Visual behavior: Animate concentric rectangular rings around a selected origin.

API direction:

- One `GridRingWave` family.
- Discrete Center Out and Outside In modes.
- Optional origin index; default to the centered item.

Review requirements:

- [ ] Center Out
- [ ] Outside In
- [ ] Off-center representative origin
- [ ] Incomplete rectangular grid if the algorithm has a separate topology branch

### Collection Deal In and Deal Out

Visual behavior: Deal cards or inventory items from a shared stack anchor into authored positions, and return them in reverse order.

API direction:

- Separate In and Out methods are justified because they have different terminal states.
- Accept a shared origin/destination, stagger interval, coordinate-space selection, and restrained relative rotation.

Review requirements:

- [ ] Deal In UI
- [ ] Deal Out UI
- [ ] World-space representative if the public API supports world coordinates

### Collection Fan In and Fan Out

Visual behavior: Distribute a stack into a configurable arc and collapse it back.

API gate:

- Prototype before freezing the API.
- If this is merely authored destination motion, keep it as a documented review recipe.
- Add a public family only if TweenHelper owns arc layout calculation and exact restoration.

Review requirements if promoted to public API:

- [ ] Fan Out
- [ ] Fan In
- [ ] Clockwise/counterclockwise or signed-arc representative modes

## Production UI

### Accordion Expand and Collapse

Visual behavior: Animate a panel dimension and coordinate content alpha without fighting Unity layout.

API and architecture gate:

- Decide whether the operation owns `RectTransform.sizeDelta`, `LayoutElement.preferredHeight`, or a dedicated wrapper.
- Start with vertical accordions unless horizontal behavior can be supported without complicating layout contracts.
- Separate Expand and Collapse methods are justified by terminal state.

Review requirements:

- [ ] Expand
- [ ] Collapse
- [ ] Layout-controlled fixture
- [ ] Interrupted playback and exact final dimension restoration

### Context Menu Show and Hide

Decision gate:

- First prototype the visual using existing `DropdownOpen` and `DropdownClose` with a context-menu pivot.
- If the result is behaviorally identical, add review recipes and documentation only.
- Add public methods only if the context menu needs a distinct directional pop, anchor behavior, or entry timeline.

### Carousel Step

Visual behavior: Coordinate outgoing and incoming cards with translation, restrained depth scale, and overlap.

API direction:

- One `CarouselStepTo` family using `UISequenceDirection`.
- Do not add Left/Right/Up/Down method aliases.

Review requirements:

- [ ] Up
- [ ] Down
- [ ] Left
- [ ] Right

### Back navigation / page pop

- Use `PagePushTo(..., UISequenceDirection.Right)` as the initial implementation and review example.
- Do not add `PagePopTo` unless a distinct back-navigation timeline is designed and manually judged better.

### Progress Complete and selection movement

Candidate review recipes:

- Progress reaches its endpoint, flashes, and settles.
- Selection indicator moves to a supplied destination and performs a restrained confirmation pulse.

Only promote these to public methods if they coordinate channels or state not conveniently expressible through existing destination and feedback families.

# Phase 3 — New gameplay and text behavior

## Gameplay feedback

### Recommended semantic families

- `DodgeEvade(direction)` — compress, lean, displace, and elastically return.
- `PerfectParry(direction)` — sharp compression, bright flash, recoil, and controlled aftershock.
- `StatusApplied` — contained pulse, relative rotation accent, and configurable status-color flash.
- `StatusCleansed` — lift, bright release flash, and exact restoration.
- `ComboMilestone` — intensity-scalable punch for significant combo thresholds.
- `ObjectiveComplete` — confirmation snap, staged pulse, and success flash.

Review policy:

- Do not enumerate arbitrary vectors exhaustively.
- Review cardinal or opposite-direction representatives where direction changes recoil or rotation sign.
- Review UI and world contexts whenever implementation constants or target bindings differ.
- Review at least normal and strong `ComboMilestone` intensity if intensity changes phase behavior rather than only magnitude.

### Deferred gameplay candidates

- Defeat
- Combo Break
- Armor Break
- Mana or Resource Spend
- Buff Expired
- Item Equipped

`Defeat` is deferred until its terminal-state contract is explicit. A persistent defeated pose is not interchangeable with a transient animation that restores its baseline.

## Text and value animation

### Word and line staggering

Preferred API direction:

- Add a generalized text-unit stagger family with a `Character`, `Word`, or `Line` unit enum.
- Preserve existing character methods for compatibility.
- Avoid separate direction-named methods.

Required review matrix for new units:

- [ ] Word In Up, Down, Left, Right
- [ ] Word Out Up, Down, Left, Right
- [ ] Line In Up, Down, Left, Right
- [ ] Line Out Up, Down, Left, Right

Rich-text tags, whitespace, line breaks, and invisible glyphs must not corrupt grouping.

### Score Decrease

Visual behavior: Count downward with a downward scale/position accent and configurable warning flash.

- [ ] Standard decrease
- [ ] Formatter representative
- Do not add separate public methods for currency, health, or resource labels if formatters already cover them.

### Text Rotate Wave

Visual behavior: Send a finite per-character rotation wave across the text and restore the exact mesh baseline.

- Use `UISequenceDirection` only if direction changes the actual wave traversal or displacement meaningfully.
- Review every discrete supported direction.

### Odometer Count

Prototype as a distinct numeric animation in which digits roll rather than merely replacing the formatted string. Promote it only if it can preserve formatting, sign, separators, and exact destination text reliably.

# Phase 4 — New destination and camera behavior

## Destination motion

### Throw

- `ThrowTo`
- `ThrowLocalTo`

Visual behavior: Ballistic signed arc plus configurable relative spin, exact destination landing, and explicit rotation-restoration rules.

Review entries:

- [ ] Throw 3D
- [ ] Throw UI/local
- [ ] Reverse-spin representative if spin is signed

### Ricochet Through

- `RicochetThrough`
- `RicochetLocalThrough`

Visual behavior: Traverse waypoints with a small impact accent at each contact while reaching the final point exactly.

Review entries:

- [ ] Ricochet 3D
- [ ] Ricochet UI/local
- [ ] Multi-contact representative path

### Sling

- `SlingTo`
- `SlingLocalTo`

Visual behavior: Pull away from the destination, hold briefly, launch quickly, and settle exactly.

Before implementation, compare it directly with `MagneticSnapTo`. Add Sling only if the prototype has a clearly different anticipation and travel rhythm.

### Deferred destination candidates

- Boomerang return
- Orbit and settle
- Funnel convergence
- Zig-zag travel
- Dynamic homing toward a moving target

Dynamic homing remains deferred because it introduces live target tracking, update ownership, cancellation, and moving-destination lifecycle semantics rather than another finite path evaluator.

## Camera feedback

### Camera Directional Hit

One method accepting a direction vector. Review representative cardinal directions without creating cardinal method aliases.

- [ ] Left impact
- [ ] Right impact
- [ ] Up impact
- [ ] Down impact

### Camera Explosion Shockwave

Coordinate positional kick, rotational aftershock, and lens expansion in one finite root that restores the exact pose and FOV.

- [ ] Standard shockwave
- [ ] Strong representative if strength alters more than magnitude

### Camera Whip Pan

Aim rapidly toward a supplied target with controlled overshoot. Decide explicitly whether normal completion restores the starting pose or remains focused on the destination.

- [ ] Left-side focus target
- [ ] Right-side focus target

### Camera Sprint Step

One finite position/rotation/FOV cycle suitable for caller-controlled looping.

- [ ] One complete cycle
- [ ] Repeated root-loop representative if lifecycle behavior differs

### Deferred camera candidates

- Death Fall
- Victory Rise
- Earthquake
- Strafe Lean
- Boss Reveal Orbit
- Cinematic Pullback

Death and victory camera families remain deferred until their persistent-versus-transient terminal state is explicit.

# API admission checklist for every new animation

Before adding a public method, answer all of the following:

- [ ] Does this animation have a timeline or terminal state not already expressible through one existing semantic operation plus an enum value?
- [ ] Is the behavior broadly reusable outside the review scene and sample project?
- [ ] Can its state capture, completion, rewind, restart, loop, and kill behavior be defined precisely?
- [ ] Can it preserve authored position, scale, rotation, alpha, color, text, and camera state as applicable?
- [ ] Are world/local and UI/3D semantics explicit?
- [ ] Can discrete variants be parameters or enums rather than extra public method names?
- [ ] Does every enum value have a planned review entry?
- [ ] Are continuous parameters covered by representative equivalence cases?
- [ ] Is the API distinct enough to justify documentation and long-term compatibility?

If the answer to the first or second question is No, implement the idea as a review recipe, showcase composition, or documentation example rather than a new public API.

# Per-slice completion checklist

Use this checklist after each bounded implementation slice.

## Implementation

- [ ] Runtime implementation complete where required
- [ ] No redundant public aliases added for enum configurations
- [ ] Direct extension and builder forms remain consistent
- [ ] State restoration and terminal state documented
- [ ] New files have Unity `.meta` files

## Review integration

- [ ] Every new semantic animation has at least one review entry
- [ ] Every discrete enum value has a review entry
- [ ] Representative continuous inputs are covered
- [ ] Existing IDs and results are preserved
- [ ] Only new IDs start Unreviewed
- [ ] Preview visibility, reset, kill, and replay behavior work

## Discovery and documentation

- [ ] Preset Browser updated for genuinely new semantic families
- [ ] Showcase updated with representative examples only
- [ ] API documentation updated
- [ ] Catalog and review totals regenerated and verified
- [ ] Usage examples prefer enums over direction-specific aliases

## Verification

- [ ] Runtime assembly compiles
- [ ] Editor assembly compiles
- [ ] Demo assembly compiles
- [ ] Validation assembly compiles
- [ ] Unity Console checked through MCP when available
- [ ] No unresolved scene or prefab references
- [ ] No duplicate review IDs
- [ ] Markdown links resolve
- [ ] `git diff --check` passes
- [ ] User manually validates every new entry before the next slice

# Resume instructions

When implementation resumes:

1. Read this roadmap and the current development README.
2. Inspect the actual review controller and count the current entries; do not assume the documented count if intervening work changed it.
3. Check the working tree and preserve unrelated user changes.
4. Continue from the first unchecked item in the active phase.
5. Implement only one bounded slice before compiling and updating review integration.
6. Preserve existing review IDs even if the controller representation is refactored.
7. Record material design decisions directly in this document.
8. Stop at each manual-validation gate and hand the new `NOT REVIEWED` entries to the user.

The immediate next task is **manual validation of the 76 Phase 1 coverage entries** under `NOT REVIEWED`. New semantic APIs begin only after that gate is complete.
