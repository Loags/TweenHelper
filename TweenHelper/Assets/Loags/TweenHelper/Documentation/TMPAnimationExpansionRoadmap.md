# TMP Animation Expansion Roadmap (post-1.1)

Status: **future proposal — not part of the 1.1.0 release candidate**

Reviewed: 2026-08-19

This document is the implementation plan for expanding TweenHelper's TextMesh Pro animation coverage without duplicating existing effects, ordering logic, lifecycle handling, or builder APIs.

The current 1.1.0 release surface remains the twelve public TMP operation families documented in [TextAndValueAnimations.md](TextAndValueAnimations.md), represented by thirteen gallery/browser examples. Do not advertise the additional effects in this roadmap as shipped features until their runtime APIs, previews, lifecycle checks, and customer documentation are implemented.

## 1) Target outcome

Add the missing glyph-motion families while keeping TweenHelper lightweight and code-first:

- More reveal units and reveal orders
- Smooth per-glyph active motion
- New per-glyph transform transitions
- One-shot spatial reactions
- Optional formation-based entrances and exits

The implementation must preserve all current TMP APIs and reuse the existing text mesh, timeline, stagger, options, builder, and gallery infrastructure.

---

## 2) Existing baseline

Do not recreate these operations:

- `TypewriterReveal` / `TypewriterHide`
- `TextCharacterStaggerIn` / `TextCharacterStaggerOut`
- `TextWave`
- `TextCharacterBounce`
- `TextColorSweep`
- `TextGlitch`
- `TextEmphasis`
- `TextScrambleReveal`
- `NumberCountTo`
- `ScoreIncrease`

### Existing logic that must be reused

| Concern | Existing source | Reuse rule |
| --- | --- | --- |
| TMP mesh capture, rebuild detection, visible-glyph filtering, restoration | `Runtime/TextAnimations/TMPCharacterMeshState.cs` | Extend this state; do not create one state class per animation. |
| Validation and tween creation | `Runtime/TextAnimations/TMPTextAnimationUtility.cs` | Add factories here and continue using its validation and timeline pattern. |
| Normalized progress, loops, yoyo, rewind, and interrupted-kill handling | `Runtime/Core/NormalizedTweenTimeline.cs` | Use through the current `CreateTimeline` path; do not build another lifecycle wrapper. |
| First/last, center-out, edges-in, and seeded-random ordering | `Runtime/Stagger/StaggerDelayUtility.cs` and `StaggerOrder.cs` | Use directly for glyph or group delays; do not add a TMP-specific order enum or shuffle implementation. |
| Cardinal directions | `Runtime/UISequences/UISequenceDirection.cs` | Reuse for directional glyph motion. |
| Duration, ease, strength, loops, update mode, and link behavior | `TweenOptions` and `TweenBuilder` | Respect existing precedence and composition behavior. |
| One-line and builder APIs | `TMPTextAnimationExtensions.cs` and `TweenBuilder.TextAnimations.cs` | Every new operation receives matching entry points. |

---

## 3) Anti-duplication decisions

The following decisions are mandatory:

- Do not add `TextPendulum` and `TextDangle` as separate engines. Implement one `TextSwing` operation with a configurable glyph pivot; top-pivot swing covers both ideas.
- Do not add `TextElasticPop`. The existing character stagger already uses an elastic scale settle and can be configured through duration, easing, and strength.
- Do not add another wave, bounce, shake, scramble, glitch, or emphasis family under a different name.
- Do not add separate APIs such as `TextCenterOutReveal`, `TextReverseReveal`, or `TextRandomReveal`. Express these through `StaggerOrder`.
- Do not add `TextExpand` when the requested appearance is already whole-label scaling, character stagger, or tracking motion. Add only the distinct tracking animation described below.
- Do not implement material, outline, glow, shader, or 3D extrusion effects in this roadmap.
- Do not introduce ScriptableObject profiles, authoring tags, custom inspectors, or editor windows.
- Do not allow two independent TMP mesh writers to run in parallel on the same label. Use `Then()` for separate effects or implement a deliberate combined evaluator inside one operation.
- Do not add a continuous cursor-following controller in the first delivery. Persistent cursor repel and magnet behavior have a different lifecycle from finite tweens.

---

## 4) Shared foundation changes

Complete these internal changes before adding effects.

### 4.1 Text units

Add a public `TextAnimationUnit` enum:

- `Character`
- `Word`
- `Line`

Add one internal text-element map built from `TMP_TextInfo`. It should provide:

- Visible TMP character indices
- Character-to-word membership
- Character-to-line membership
- Ordered groups for the selected unit

Use TMP metadata rather than parsing the source string. Rich-text tags and layout-only characters must remain untouched.

Suggested file: `Runtime/TextAnimations/TMPTextElementMap.cs`.

### 4.2 Shared glyph transform

Extend `TMPCharacterMeshState` with one internal glyph-transform representation supporting:

- Position offset
- Non-uniform scale
- Z rotation
- Horizontal shear
- Alpha and optional tint, preserving current behavior
- Center and top-center pivots

Keep a compatibility overload for the current offset/uniform-scale/alpha/tint call so existing effects retain their behavior. All new transform effects must use the same four-vertex application loop.

Shared types:

- Internal `TMPGlyphTransform`
- Public `TextGlyphPivot` with `Center` and `Top`

Do not create separate vertex mutation code for rotate, shear, swing, or scatter.

### 4.3 Shared stagger timing

Create one internal helper that:

1. Calls `StaggerDelayUtility.CalculateDelays` for the selected group count.
2. Maps each glyph to its unit group's delay.
3. Compresses excessive delays into the requested duration using the existing character-stagger policy.
4. Returns normalized local progress for a glyph or group.

Refactor current stagger-in and stagger-out evaluation to use it before adding new ordered transitions. Existing default output must remain visually equivalent.

### 4.4 Deterministic noise

When scatter is implemented, extract the current deterministic hash from `TMPCharacterMeshState` into a small internal `TMPDeterministicNoise` helper and reuse it for both glitch and scatter. Do not change the existing glitch seed output unless intentionally documented.

---

## 5) Planned public animation API

Names and signatures should be collision-checked before implementation. Existing overloads remain source-compatible and delegate to the shared engines.

### 5.1 Reveal and hide control

Add:

```csharp
TypewriterReveal(TextAnimationUnit unit, float? duration = null)
TypewriterHide(TextAnimationUnit unit, float? duration = null)

TextStaggerIn(
    TextAnimationUnit unit = TextAnimationUnit.Character,
    StaggerOrder order = StaggerOrder.FirstToLast,
    UISequenceDirection direction = UISequenceDirection.Up,
    float distance = 18f,
    float unitStagger = 0.025f,
    int seed = 1337,
    float? duration = null)

TextStaggerOut(
    TextAnimationUnit unit = TextAnimationUnit.Character,
    StaggerOrder order = StaggerOrder.LastToFirst,
    UISequenceDirection direction = UISequenceDirection.Up,
    float distance = 18f,
    float unitStagger = 0.025f,
    int seed = 1337,
    float? duration = null)
```

Compatibility routing:

- Current `TextCharacterStaggerIn` delegates to `TextStaggerIn(Character, FirstToLast, ...)`.
- Current `TextCharacterStaggerOut` delegates to `TextStaggerOut(Character, LastToFirst, ...)`.
- `FromCenter`, `ToCenter`, and `Random` come from the existing `StaggerOrder`; no named reveal variants are added.
- Typewriter remains sequential. Non-sequential reveals use mesh-based stagger so `maxVisibleCharacters` is not misused for arbitrary ordering.

### 5.2 Smooth active motion

Add four distinct one-cycle operations:

- `TextWiggle`: smooth deterministic per-glyph position and slight rotation, with phase offsets.
- `TextFloat`: smooth directional per-glyph oscillation with phase offsets.
- `TextSwing`: per-glyph rotation using `TextGlyphPivot.Center` or `Top`.
- `TextPulse`: per-glyph scale oscillation with phase offsets.

Rules:

- Each operation produces one finite cycle and returns to the captured baseline.
- Repetition uses `TweenOptions.WithLoops`; no operation creates an internal infinite loop.
- `TextWiggle` must remain smooth and deterministic so it is visually distinct from the current abrupt, color-changing `TextGlitch`.
- `TextFloat` must remain phase-based and cyclical so it is distinct from the current traveling `TextWave` and `TextCharacterBounce`.
- `TextPulse` changes per-glyph scale only; whole-label pulse continues to use existing transform presets.

Suggested default cycle durations are `0.65s` for wiggle, `0.9s` for float, `0.8s` for swing, and `0.7s` for pulse.

### 5.3 Transform transitions

Add:

- `TextScatterIn` / `TextScatterOut`
- `TextRotateIn` / `TextRotateOut`
- `TextShear`
- `TextTrackingPulse`

Implementation rules:

- Scatter uses deterministic seeded start/end poses and the shared stagger timing. It must not reuse the time-sliced jitter behavior of glitch.
- Rotate-in/out uses the shared glyph transform and stagger timing; do not create a rotation-specific vertex loop.
- Shear is a finite deform-and-return animation using the shared affine transform.
- Tracking pulse offsets glyphs from the label's visual center and returns them. It must not change `TMP_Text.characterSpacing`, because doing so would trigger layout rebuilds and conflict with mesh restoration.
- Scatter and rotate completion semantics match character stagger: in leaves text normally visible; out restores the mesh and leaves the label hidden through `maxVisibleCharacters`.

### 5.4 Spatial reaction

Add one finite `TextImpactRipple` operation:

- Input is an impact point in the TMP object's local coordinate space.
- Strength is based on distance from each captured glyph center.
- A radial wave moves and optionally scales glyphs, then returns them to baseline.
- Reuse the existing normalized timeline and glyph transform.

Do not add separate repel and magnet implementations in this phase. If later required, introduce one shared spatial-field evaluator with a signed strength or mode rather than duplicating distance/falloff code.

### 5.5 Conditional formation transitions

Only start this phase after the preceding effects ship and the gallery still shows a meaningful gap.

Candidate entrances/exits:

- Arc formation to authored text
- Circle formation to authored text
- Wave-path formation to authored text

Use one internal formation-position provider and shared stagger engine. Do not create a class per formation. Do not add grid formation initially; it overlaps strongly with deterministic scatter and TMP's existing multiline layout.

Persistent curved or circular text layout is out of scope. These are finite transitions whose completed in-state is the normal authored TMP layout.

---

## 6) Delivery phases

### Phase 0 — Foundation refactor

Files:

- Modify `TMPCharacterMeshState.cs`
- Modify `TMPTextAnimationUtility.cs`
- Add `TextAnimationUnit.cs`
- Add `TextGlyphPivot.cs`
- Add `TMPTextElementMap.cs`
- Add Unity `.meta` files for every new file

Work:

- Introduce shared text-unit mapping, affine glyph transforms, pivots, and stagger timing.
- Route existing mesh effects through compatibility paths without changing public behavior.
- Keep `NormalizedTweenTimeline`, `TweenOptions`, `StaggerOrder`, and `StaggerDelayUtility` unchanged unless a verified shared bug requires a fix.

Exit criteria:

- Existing TMP examples behave as before.
- Both `TextMeshProUGUI` and world-space `TextMeshPro` remain supported.
- Current restore, rewind, kill, loop, and text-rebuild behavior remains intact.

### Phase 1 — Reveal units and ordering

Work:

- Add word and line typewriter units.
- Add `TextStaggerIn` and `TextStaggerOut` with unit, order, and seed.
- Make legacy character-stagger APIs delegate to the generalized engine.
- Add builder and direct extension parity.

Exit criteria:

- Character, word, and line groups reveal correctly.
- All five existing `StaggerOrder` values work without new ordering code.
- Rich text, whitespace, line breaks, and multi-material text remain stable.

### Phase 2 — Smooth active motion

Work:

- Add wiggle, float, swing, and pulse evaluators to `TMPCharacterMeshState`.
- Add matching factories, builder methods, and extensions.
- Use options loops for repeated motion.

Exit criteria:

- Every operation starts and ends at the captured baseline.
- Wiggle is visibly distinct from glitch; float is visibly distinct from wave and bounce.
- Top-pivot swing covers pendulum and dangle presentations without extra engines.

### Phase 3 — Transform transitions

Work:

- Extract and reuse deterministic noise.
- Add scatter in/out, rotate in/out, shear, and tracking pulse.
- Reuse the generalized stagger and glyph-transform paths.

Exit criteria:

- Seeded scatter is stable across replay and gallery capture.
- Out transitions finish hidden and restore correctly when rewound or interrupted.
- No operation mutates TMP layout properties.

### Phase 4 — Spatial reaction

Work:

- Add `TextImpactRipple` using captured glyph centers and one radial falloff evaluator.
- Document conversion of screen or world input into TMP-local coordinates at the call site; do not embed camera lookup into the animation engine.

Exit criteria:

- Ripple origin and radius behave consistently for UI and world-space TMP.
- Completion, kill, and rewind restore the captured mesh.

### Phase 5 — Conditional formation transitions

Work only if approved after Phase 4:

- Implement one shared formation provider.
- Add arc, circle, and wave-path in/out transitions.
- Do not add persistent layout state or editor authoring.

Exit criteria:

- Each formation is visually distinct from scatter.
- All variants use one formation engine and the shared stagger timing.

---

## 7) Lifecycle and safety contract

Every new operation must follow these rules:

- Capture state when playback starts, not when the builder is created.
- Recapture when TMP reports changed text, properties, character count, or mesh data.
- Animate visible glyphs only; preserve rich-text tags and layout-only characters.
- Restore the invocation baseline on rewind and interrupted kill.
- Finite active effects restore the baseline on forward completion.
- In transitions finish at the normal visible baseline.
- Out transitions restore mesh data, then set `maxVisibleCharacters` to `0`.
- Respect `TweenOptions` duration, ease, strength, loops, update type, unscaled time, ID, and target linking through existing infrastructure.
- Required `TMP_Text` wiring continues to fail immediately through the current same-object requirement.
- Document that independent mesh-writing operations on one TMP target must be sequenced, not joined.

---

## 8) Gallery and documentation work

Update only after each runtime phase compiles:

- `Documentation/TextAndValueAnimations.md`
- `Documentation/API.md`
- `Documentation/PresetCatalog.md` only if this project intentionally indexes non-preset text operations there
- `Samples/TweenHelper Demos/Scripts/AnimationGalleryCatalog.cs`
- The existing gallery operation dispatch and preview code

Gallery policy:

- Add one representative entry per new engine, not one entry per order, direction, pivot, or unit.
- Expose order, unit, direction, seed, or pivot as gallery controls where the current gallery architecture supports them.
- Do not add separate gallery rows for pendulum/dangle, center-out/edges-in, or other parameter-only variants.

---

## 9) Validation checklist

No new automated tests are required by this roadmap unless explicitly requested. After each phase:

1. Use Unity MCP to check compilation and Console errors when the Editor is available.
2. Validate both `TextMeshProUGUI` and world-space `TextMeshPro` gallery targets.
3. Check empty text, one glyph, long text, multiline text, whitespace, rich text, and multiple font/material references.
4. Change text during playback and confirm safe recapture.
5. Check forward completion, rewind, interrupted kill, restart loops, and yoyo loops.
6. Verify deterministic ordering and scatter with repeated seeds.
7. Confirm existing TMP examples have not changed visually unless a change was explicitly approved.
8. Confirm every added or moved Unity file has its `.meta` file.

Do not run a Unity batch build unless explicitly requested.

---

## 10) Recommended implementation order

1. Foundation refactor with zero intended visual changes.
2. Generalized stagger ordering and word/line units.
3. Wiggle, float, swing, and pulse.
4. Scatter and rotation transitions.
5. Shear and tracking pulse.
6. Impact ripple.
7. Reassess formation transitions before implementing them.

Each phase should be independently compile-safe, gallery-visible, documented, and suitable for a focused commit.

---

## 11) References

External packages were used only as feature inspiration:

- Text Studio overview: `https://www.wetzold.com/tools/text-studio/`
- Text Studio effect gallery: `https://www.wetzold.com/tools/text-studio/gallery/`
- Text Studio 3D interaction reference: `https://www.wetzold.com/tools/text-studio/3d/docs/animate-3d-text-and-add-interaction/`

Do not copy their implementation or expand TweenHelper into a competing authoring framework.
