# TweenHelper Implementation Roadmap (v1.0)

## Changelog

- 2026-08-18 — Implemented Epics A-H: world-to-UI projection, progress/value binding, gameplay states, collection topologies, sequence macros and hooks, camera/ambient helpers, engine-property wrappers, gallery catalog coverage, and documentation indexing.

This document is the definitive execution plan for extending TweenHelper toward broad game-development coverage while avoiding redundant animations.

## 1) Objective

Deliver a production-oriented animation system that covers the major missing gameplay and UI animation gaps:

- World-space → UI-space transitions (`3D -> 2D` targeting)
- Resource/progress-value animations (`Image.fillAmount`, `Slider.value`, etc.)
- Gameplay-state families (buff/debuff/readiness/warning/stateful UI feedback)
- New collection motion styles that are not current path/order duplicates
- Composable, reusable animation macros/sequence recipes
- Optional advanced extension path for non-transform targets (audio/light/sfx/system properties)

The target outcome is a practical feature roadmap that is:

- Actionable by another agent with minimal ambiguity
- Backward compatible where possible
- Modular by epic and phase
- Auditable against duplication risk

---

## 2) Scope boundaries

### In scope

- Runtime additions under `Assets/Loags/TweenHelper/Runtime`
- Gallery/samples under `Assets/Loags/TweenHelper/Samples/TweenHelper Demos`
- Docs under `Assets/Loags/TweenHelper/Documentation`
- Preset catalog/API docs updates for discoverability

### Out of scope

- New major editor tooling (custom windows, property drawers)
- Shader rewrites or full cinematic timeline systems
- Cross-project plugin integrations

### Non-goals (explicitly)

- Recreating generic DOTween APIs that already exist
- Adding “mildly renamed” motion families that map 1:1 to existing behavior
- Duplicating current destination/local/world variants with only param name changes

---

## 3) Design constraints and anti-duplication policy

Every candidate addition must satisfy at least one of:

1. New domain target type (e.g. world→UI destination conversion)
2. New animated property type (e.g. fill amount, slider value, component scalar)
3. New animation state model (e.g. charge/drain/critical thresholds)
4. New ordering/topology semantics not represented today

### Duplicates to avoid

- `XIn` and `XOut` style families already covered by existing presets should not be added with only renamed defaults
- Existing bounce/spring/arc path families should not be copied into every category with cosmetic changes only
- Avoid adding many “stronger/weaker” variants; use `TweenOptions.Strength` and `TweenOptions.Duration` instead

### Acceptance quality

- New API name must be discoverable and not conflict with existing extension names
- Each new family includes a meaningful failure path and restore/cancel behavior
- Each feature has at least one gallery example (where category currently exists)

---

## 4) Current architecture map (existing touch points)

Use these files as integration anchors:

- `Assets/Loags/TweenHelper/Runtime/DestinationMotion/TweenBuilder.DestinationMotion.cs`
- `Assets/Loags/TweenHelper/Runtime/DestinationMotion/DestinationMotionUtility.cs`
- `Assets/Loags/TweenHelper/Runtime/Feedback/TweenBuilder.Feedback.cs`
- `Assets/Loags/TweenHelper/Runtime/Feedback/FeedbackSequenceUtility.cs`
- `Assets/Loags/TweenHelper/Runtime/Stagger/StaggerRecipeExtensions.cs`
- `Assets/Loags/TweenHelper/Runtime/Stagger/SpatialCollectionRecipeUtility.cs`
- `Assets/Loags/TweenHelper/Runtime/TextAnimations/TweenBuilder.TextAnimations.cs`
- `Assets/Loags/TweenHelper/Runtime/TextAnimations/TMPTextAnimationUtility.cs`
- `Assets/Loags/TweenHelper/Runtime/UISequences/TweenBuilder.UISequences.cs`
- `Assets/Loags/TweenHelper/Runtime/UISequences/UISequenceUtility.cs`
- `Assets/Loags/TweenHelper/Runtime/Core/TweenTargetUtility.cs`
- `Assets/Loags/TweenHelper/Samples/TweenHelper Demos/Scripts/AnimationGalleryCatalog.cs`
- `Assets/Loags/TweenHelper/Documentation/DestinationMotion.md`
- `Assets/Loags/TweenHelper/Documentation/FeedbackSequences.md`
- `Assets/Loags/TweenHelper/Documentation/StaggeredCollections.md`
- `Assets/Loags/TweenHelper/Documentation/TextAndValueAnimations.md`
- `Assets/Loags/TweenHelper/Documentation/UISequences.md`
- `Assets/Loags/TweenHelper/Documentation/API.md`
- `Assets/Loags/TweenHelper/Documentation/PresetCatalog.md`

---

## 5) Execution model

### Cadence

- Plan for incremental sprints, each producing compile-safe and demo-visible work.
- Prioritize one epic per week where possible.
- Keep each sprint to existing family style and folder structure.
- Do **not** modify unrelated assets unless required.

### Source-control and review convention

- Keep commits focused by epic.
- One commit per epic when possible.
- Preserve `.meta` files and naming consistency.

### Validation policy

- No mandatory runtime/scene tests in this roadmap.
- Minimal validation: compile-time friendliness, API consistency, and gallery wiring.

---

## 6) Epic plan

### Epic A — World-space to UI-space destination motion (highest priority)
#### Goal
Enable direct animation from world-space positions to UI-space (`RectTransform`) targets with consistent camera/canvas handling.

#### Why now
This is the strongest missing gameplay capability after current destination and feedback families.

#### Planned additions

1. Core conversion utility
   - Add a new utility in destination motion module (name convention: `UIWorldProjectionUtility`).
   - Responsibilities:
     - Convert world point to screen point
     - Convert screen point to canvas local position
     - Resolve correct camera from input: explicit `Camera`, `targetCanvas.worldCamera`, fallback to `Camera.main`
     - Support `Screen Space Overlay` and `Screen Space Camera`
   - Fail fast when conversion cannot be completed

2. Destination API additions (`TweenBuilder.DestinationMotion`)
   - Add:
     - `ArcToUI(Vector3 worldDestination, RectTransform uiTarget, float height, float? duration = null, Camera worldCamera = null)`
     - `HopToUI(...)`
     - `BezierToUI(destination, controlA, controlB, RectTransform uiTarget, ...)`
     - `PathThroughUI(...)` (if path through world landmarks to UI target is needed)
   - Behavior:
     - input destination is authoritative world position or anchor source point
     - motion uses existing path math, but converts each endpoint in UI coordinate space where needed
     - if `RectTransform` is in a different canvas, conversion is safe and deterministic

3. Feedback bridge
   - Add `PickupCollectToUI` (and optional `PickupCollectToUIByRect`) in `TweenBuilder.Feedback`.
   - Reuse pickup arc/travel/settle semantics and apply conversion at start and updates.

4. Collection/sequence hooks
   - Optional: add “collect+anchor lock” option to prevent target drift if destination UI moves.

#### Exit criteria

- A world object can be moved to a UI destination in one call without manually converting coords.
- Behavior matches existing world/local destination semantics for easing, options, and loop handling.
- Documented camera/canvas assumptions and failure messages.

---

### Epic B — Value and progress animations
#### Goal
Create production-ready progress-bar and metric-based animation families.

#### Planned additions

1. Runtime value binding
   - Add value binding utility for:
     - `UnityEngine.UI.Image.fillAmount`
     - `UnityEngine.UI.Slider.value`
   - Preserve normalization semantics (`Slider.minValue`/`maxValue`).

2. Core API
   - Add text/value extension style methods:
     - `FillTo(float target, ...)`
     - `FillFromTo(float start, float end, ...)`
     - `ValueFillTo(float target, ... )` with optional paired text
     - `FillDrain(...)` (fast downward transitions + optional pulse/shake)
     - `FillCharge(...)` (upward transitions with optional overshoot + settle)
     - `FillAlertPulse(float threshold, ...)`

3. Combo helper
   - Add a convenience operation to animate both numeric text and fill target together:
     - `FillAndText(...)` (single timeline split)

4. Docs + gallery
   - Add examples:
     - HP drain/recharge
     - Mana charge
     - Objective progress bar fill
     - Critical low threshold warning

#### Exit criteria

- Health/mana/XP bars can be animated via one-liner APIs
- Slider and fill semantics are correct under varying min/max
- Paired text and fill stay in sync visually

---

### Epic C — Gameplay-state feedback families
#### Goal
Add missing semantic effects for battle/state loops that currently require project-specific animation scripts.

#### Planned additions

- `AbilityCharging`
- `AbilityReady`
- `DodgeRoll`
- `StunStart`
- `StunEnd`
- `BuffApplied`
- `DebuffApplied`
- `ResourceDepleted`
- `ResourceRecovered`
- `ObjectiveUnlocked`

#### Implementation notes

- These should be implemented as composed families using existing atomic pieces first when possible.
- Keep kill/release behavior aligned with other feedback families.
- Keep parameters minimal and semantic (`strength`, `duration`, optional colors/effects).

#### Exit criteria

- At least 6 gameplay states added with clear usage examples and category tagging in gallery

---

### Epic D — Collection motion topology expansion
#### Goal
Add non-redundant collection patterns beyond wave/ripple/spiral/burst family.

#### Planned additions

- `GridConcentricIn` / `GridConcentricOut`
- `GridQuadrantSweep`
- `ListAccordion`
- `CollectionOrbitIn` / `CollectionOrbitOut`
- `LoadingRing`
- `LoadingRibbon`

#### Implementation notes

- Prefer index-order logic in existing stagger utility.
- Ensure both local and anchored coordinate behavior remain predictable.
- Validate `null/empty/repeat` cases consistently.

#### Exit criteria

- At least 5 new recipe families available through `Collections` gallery entries
- No behavior duplicates of existing wave/spiral/ripple/burst families

---

### Epic E — Composition and reusable sequences
#### Goal
Introduce reusable behavior composition so agents can combine families safely.

#### Planned additions

- Macro-like sequence presets for:
  - “Critical Hit sequence”
  - “Reward reveal sequence”
  - “Error / warning loop sequence”
  - “Cutscene UI entrance sequence” if scope allows
- Shared completion/progress hook model:
  - hook callback point when animation reaches configurable progress fraction

#### Exit criteria

- At least three sequence macros available and documented
- Hooks do not alter runtime semantics of existing core primitives

---

### Epic F — Camera/game feel micro-features
#### Goal
Strengthen camera + atmosphere without duplicating shake-only category.

#### Planned additions

- `CameraRackFocus` or focus-shift helper
- `CollectLandingCameraKick` (micro kick paired with pickup-to-UI completion)
- Optional ambient helpers:
  - torch flicker pulse
  - subtle scanner/targeting pulse

#### Exit criteria

- At least 2 camera features tied to gameplay events with minimal risk to existing camera feedback families

---

### Epic G — Engine-property extension path (phase-2+)
#### Goal
Add safe support for frequent non-transform property animations.

#### Planned additions

- `AudioSource` volume/pitch wrapper
- `Light` intensity/color wrapper
- Optional `ParticleSystem` emission-rate wrapper
- Optional `MaterialPropertyBlock` scalar/color helper for `Graphic/Renderer` related flows

#### Exit criteria

- Demonstrable one-off example for at least 2 non-transform properties

---

### Epic H — Quality, docs, and discoverability
#### Goal
Stabilize what was added, and keep future agents efficient.

#### Planned additions

- Update:
  - `Documentation/API.md`
  - `Documentation/PresetCatalog.md`
  - relevant category guides (`DestinationMotion`, `FeedbackSequences`, `TextAndValueAnimations`, `StaggeredCollections`, `UISequences`)
- Add a concise “what’s new” migration section
- Maintain naming consistency and XML doc examples

#### Exit criteria

- New APIs are discoverable and self-explaining from documentation and snippets

---

## 7) Delivery schedule (proposed)

### Week 1
- Complete Epic A (World-to-UI motion baseline)
- Document conversion rules and edge cases

### Week 2
- Complete Epic B (progress/fill core)
- Add basic gallery examples for HP/mana/XP

### Week 3
- Complete Epic C first half (`AbilityCharging`, `AbilityReady`, `Debuff/ Buff`, `Stun`)
- Add gameplay gallery scenarios

### Week 4
- Complete Epic D first half (collection topologies)

### Week 5
- Complete Epic D remaining + Epic E first macro preset

### Week 6
- Complete Epic F and Epic E second macro set

### Week 7
- Start Epic G experimental property wrappers (optional depending on scope)
- Add docs + sample call patterns

### Week 8
- Epic H cleanup pass and API/catalog indexing pass
- Final quality sweep and roadmap rollover notes

---

## 8) Detailed implementation checklist (agent handoff)

### Before each epic starts
- Confirm no naming collision in extension methods
- Confirm target file set and `.meta` impact
- Define acceptance criteria for that epic

### During implementation
- Keep methods grouped by existing file boundaries
- Reuse shared utility classes where possible
- Validate input and throw descriptive `InvalidOperationException` where behavior must be deterministic

### After each epic
- Update category docs first
- Then update gallery catalog
- Then finalize API docs and preset catalog snippets

---

## 9) Risks and mitigations

### Risk: World→UI conversion errors under mixed canvas setups
- Mitigation: explicit camera resolution path + clear logs only for recoverable user-fixable cases

### Risk: Feature bloat / duplicated motion families
- Mitigation: apply anti-duplication policy and maintain this roadmap as source-of-truth

### Risk: API sprawl
- Mitigation: one semantic name per use case; avoid cosmetic variants

### Risk: Performance regression from large sequence graphs
- Mitigation: keep macro presets lightweight and reuse existing tween chains

---

## 10) Done criteria for this roadmap version

This roadmap is considered complete for version 1.0 when:

- Epic A and B are shipped and documented
- At least 3 gameplay-state feedback additions are shipped
- Collection recipe additions exceed the current baseline by 4+ families
- The roadmap itself is updated with progress notes and completion dates by another agent

---

## 11) Versioning notes

- This is `v1.0` of the roadmap.
- If major direction changes occur, append a short `Changelog` section at the top with date + scope.
- Keep this document in `Documentation` and link from `API.md` when team onboarding is updated.

---

## 12) Suggested “next agent action” template

If picking up this roadmap mid-way:

1. Read this document end-to-end.
2. Start at the current active epic from latest progress log.
3. Open the target runtime files listed in section 4.
4. Implement the smallest shippable unit in that epic.
5. Update docs in the same cycle.
6. Leave a short changelog note at top of this file when complete.

---

## 13) Implementation status

All roadmap epics are implemented. The runtime exposes the planned APIs, new files include Unity metadata, the gallery catalog covers every new family, and the category/API documentation contains sample calls and lifecycle rules. `TweenHelperPresetReview` now includes authored world-to-UI, Image/Slider progress, gameplay-state, collection-topology, sequence-macro, camera, audio, light, particle, and renderer-property fixtures so the complete roadmap can be replayed and reviewed in one scene.
