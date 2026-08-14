# Serialized Animation Recipes and TweenPlayer Roadmap

- Status: **WAITING FOR CLI FOUNDATION — DO NOT IMPLEMENT**
- Created: 2026-08-12
- Scope: Versioned animation recipe assets, explicit target bindings, runtime playback through `TweenPlayer`, Editor authoring and preview, agent-safe Unity Pipeline authoring, documentation, and validation
- Likely release class: Backward-compatible feature release; provisionally `1.1.0`, to be confirmed after the post-CLI review

> [!CAUTION]
> Do not begin production implementation from this document while the wait gate below is closed. The CLI prototype files or partially completed CLI phases are not sufficient. After the CLI foundation is accepted, first audit the implemented code, revise this roadmap against facts, design the recipe-specific agent commands, and validate the revised roadmap. Only then may this document move to **READY FOR IMPLEMENTATION**.

This roadmap is intentionally separate from the [TweenHelper Unity Pipeline CLI Roadmap](TweenHelperPipelineCliRoadmap.md). It describes a new public TweenHelper authoring feature and the later Pipeline commands that can safely create or update that feature.

## Dependency handshake and circular-wait resolution

The CLI roadmap currently gates `tween_helper_apply_plan` on a stable persisted recipe/component representation. This roadmap, in turn, must wait until the CLI planning, schema, preview, and verification architecture is implemented and can be audited.

Therefore, “the CLI pipeline is completely implemented” has this precise dependency-safe meaning for this roadmap:

1. CLI roadmap Phases 0–5 and their exit criteria are complete and accepted.
2. Discovery, descriptors, planning, validation, sandbox preview, verification jobs, developer automation, and the selected public-companion boundary have current evidence.
3. Persistent authoring remains intentionally unavailable because it depends on this roadmap.
4. After the recipe and `TweenPlayer` representation is implemented, CLI roadmap Phase 6 resumes and completes persisted authoring and release hardening.

Requiring CLI Phase 6 to finish before recipes would create an impossible circular dependency. The handoff is instead:

```text
CLI phases 0–5 complete
        |
        v
Post-CLI code audit and capability inventory
        |
        v
Recipe roadmap review + recipe command design + final roadmap validation
        |
        v
Recipe assets, runtime executor, TweenPlayer, and Editor authoring implemented
        |
        v
CLI persisted-authoring commands implemented and both roadmaps validated
```

## Roadmap state machine

Only advance one state when all exit criteria for the current state are recorded with fresh evidence.

| State | Meaning |
| --- | --- |
| `WAITING_FOR_CLI` | Current state. No recipe/TweenPlayer production implementation is allowed. |
| `POST_CLI_AUDIT` | CLI phases 0–5 are accepted; inspect the actual implementation and record its real capabilities and constraints. |
| `ROADMAP_REVIEW` | Replace provisional assumptions in this document with decisions grounded in the audit. |
| `CLI_COMMAND_DESIGN` | Define how agents inspect, plan, validate, preview, create, update, bind, and verify recipes and players. |
| `FINAL_ROADMAP_VALIDATION` | Check architecture, serialization, lifecycle, safety, packaging, migration, and validation coverage as one system. |
| `READY_FOR_IMPLEMENTATION` | No blocking design question remains and the implementation slices are approved. |
| `IMPLEMENTING` | Execute one bounded phase at a time and validate it before expanding operation coverage. |
| `MANUAL_REVIEW` | New showcase/review entries compile and await visual validation. |
| `COMPLETE` | Runtime, Editor authoring, CLI authoring, documentation, packaging, and validation gates pass. |

## Product objective

Allow a user to author a reusable animation sequence as a versioned Unity asset, bind it explicitly to scene or prefab targets through a focused `TweenPlayer` component, preview it in the Editor, and control it at runtime without writing a custom driver script.

The same representation should let an agent:

- discover supported operations and parameter contracts;
- create a deterministic recipe plan without mutating the project;
- validate and preview the plan in an isolated sandbox;
- preflight an exact asset/component change set;
- create or update the recipe and `TweenPlayer` only after explicit confirmation;
- inspect and verify the persisted result using stable identities and hashes.

The base TweenHelper artifact must continue to compile and run without Unity Pipeline installed.

## Current baseline to re-audit later

The following is planning context, not future proof:

- `TweenBuilder` already provides sequential `Then`, parallel `With`, and `Delay` composition, along with callbacks and raw DOTween injection.
- `TweenHandle` already provides pause, resume, kill, restart, rewind, complete, state queries, callbacks, and await support.
- TweenHelper currently has 300 built-in code presets plus non-preset operation families for staggered collections, destination motion, gameplay feedback, production UI sequences, and TMP text/value animations.
- The review scene currently contains 474 stable entries: the preserved 398-entry validated baseline plus 76 Phase 1 coverage entries pending manual review. The post-CLI audit must recount the then-current baseline and preserve all existing review IDs and results.
- `TweenPresetRegistry` supports runtime code-preset discovery, but it is not a sufficient serialized operation schema.
- `UIAnimationStateCache` and several operation-specific utilities already define important baseline, restore, and kill behavior.
- No public, versioned `TweenRecipe` asset or `TweenPlayer` component exists yet.
- Repository-only CLI prototype files may exist while the CLI roadmap is being implemented. Their presence is not evidence that the wait gate is open, and this roadmap does not approve or modify them.

## Binding product decisions

These decisions remain binding unless the post-CLI audit finds concrete evidence that requires a documented change:

1. **Recipe asset plus player component.** The persisted model is a reusable `TweenRecipe : ScriptableObject`; scene and prefab references live on `TweenPlayer : MonoBehaviour`, not inside the reusable asset.
2. **Explicit binding slots.** Recipe steps refer to stable binding IDs. A player maps those IDs to explicit serialized object references or ordered collections. Runtime hierarchy-name lookup is prohibited.
3. **Stable operation IDs.** Serialized steps use versioned catalog IDs shared with automation descriptors. They do not persist arbitrary method names, CLR type names, or reflection instructions supplied by a caller.
4. **Flat deterministic timeline first.** Version 1 uses ordered steps with append/join timing plus explicit intervals. Nested graphs, branches, and state machines are later features.
5. **Typed allowlisted parameters.** Each operation descriptor owns its accepted fields, types, defaults, ranges, target requirements, mutation footprint, and lifecycle semantics. Unknown fields fail validation.
6. **No arbitrary executable payloads.** Serialized recipes exclude delegates, callbacks, raw DOTween objects, arbitrary C# expressions, and formatter functions. `TweenPlayer` may expose ordinary Inspector `UnityEvent`s, but recipe CLI commands do not create or rewrite persistent event targets in the first release.
7. **Pipeline-neutral runtime.** Recipe models, validation, compilation, execution, and `TweenPlayer` live in the base TweenHelper assemblies and never reference `Unity.Pipeline`.
8. **One owned playback by default.** A player owns and controls its active handle. Replay, disable, target-destruction, and conflict policies are explicit and deterministic.
9. **Versioned schema and migrations.** Assets carry a schema version. Breaking serialized changes require deterministic, reviewable migrations and compatibility documentation.
10. **Agent writes are preflighted.** Asset/component authoring uses stable references, canonical hashes, a complete dry-run change set, a short-lived confirmation token, idempotency, Undo where supported, and explicit dirty/save reporting.

## Decisions that must be reopened after the CLI audit

Do not freeze these details before inspecting the completed CLI foundation:

- Which descriptor, catalog, canonical JSON, hashing, validation, sandbox, and job services can move into or be shared with the public base without introducing Pipeline dependencies.
- Whether the serialized step payload should use explicit serializable data types, managed references, or sub-assets. The selected format must survive type renames, domain reload, asset duplication, YAML merge, and canonical wire round trips.
- Exact operation ID format and schema-version ownership.
- Whether public recipe validation belongs in Runtime or a Pipeline-neutral assembly shared by Runtime and Editor.
- The smallest initial operation slice that does not force a second incompatible schema.
- Whether `tween_helper_apply_plan` remains the single write command or delegates to narrower recipe/player authoring commands.
- How the public companion is packaged and which recipe capabilities it may advertise.
- Exact release version and migration support window.

## Wait gate A — CLI foundation acceptance

All items must be complete before the post-CLI audit starts:

- [ ] CLI roadmap Phases 0–5 are marked complete with their exit evidence.
- [ ] The implemented command schemas and stable command IDs have been inspected after a clean recompile.
- [ ] Canonical plans, descriptor/catalog hashes, target fingerprints, and stale-reason behavior are implemented and documented.
- [ ] Sandbox preview and bounded verification jobs demonstrate owned cleanup without source-target mutation.
- [ ] The implemented operation-family capability matrix is explicit; unsupported families are advertised as unsupported.
- [ ] The public companion boundary and base-without-Pipeline compatibility have current clean-project evidence.
- [ ] The CLI roadmap explicitly reports persisted authoring as waiting on `TweenRecipe`/`TweenPlayer`, not accidentally complete.
- [ ] No unresolved CLI schema redesign is expected to invalidate the recipe representation immediately after implementation.

Gate A is not satisfied by untracked prototype files, an isolated Phase 0/1 implementation, roadmap checkmarks without evidence, or a successful compile alone.

## Gate B — mandatory post-CLI implementation audit

When Gate A opens, inspect the code before revising or implementing this roadmap. Produce a dated audit record beside this document containing:

1. The exact Unity, TweenHelper, DOTween, Pipeline, UI, and TMP compatibility tuple.
2. Every implemented public and developer command, its input/output schema version, maturity, side effects, and capability flags.
3. The actual assembly graph and dependency direction.
4. The descriptor/catalog sources of truth and all supported operation families.
5. Canonical plan, JSON, hashing, target-reference, idempotency, and stale-detection behavior.
6. Preview/session/job ownership, cleanup, reload, cancellation, timeout, and observation behavior.
7. Existing services that can be reused by recipe validation, Editor preview, and agent authoring.
8. Gaps, duplicate concepts, unsafe assumptions, and refactors required before the persisted schema can be frozen.
9. Current public API, documentation, showcase/review count, and validation state.
10. A recommendation to proceed, revise, split the feature, or keep the gate closed.

Do not treat the old roadmap description as evidence of what the code does. Inspect the actual implementation, generated command schemas, Unity state, and relevant artifacts.

Gate B exit criteria: another implementer can identify every reusable service, forbidden dependency, schema boundary, and missing capability without guessing.

## Gate C — roadmap review against the audit

Revise this document after Gate B:

- [ ] Replace the provisional architecture with selected concrete types, assembly locations, ownership rules, and dependency direction.
- [ ] Freeze recipe schema v1, operation IDs, binding IDs, step IDs, supported value types, default/omission rules, and migration policy.
- [ ] Map every initial recipe operation to one existing descriptor and runtime executor path.
- [ ] Define exact append/join/interval behavior, duration precedence, loops, ease/options, and baseline/restore semantics.
- [ ] Define `TweenPlayer` replay, conflict, disable, destroy, rewind, and event behavior.
- [ ] Define Editor authoring, Undo, asset dirtiness, prefab behavior, preview isolation, and restoration behavior.
- [ ] Resolve how existing CLI services are reused rather than duplicated.
- [ ] Re-estimate implementation slices and remove any feature that cannot meet the first release contract safely.

Gate C exit criteria: no persisted field or public behavior in the MVP is still described only as “TBD.”

## Gate D — design the agent authoring command slice

After Gate C, review the implemented CLI catalog and add the minimum commands needed for safe recipe and player authoring. Candidate commands are listed later in this document, but names and boundaries are not frozen until this gate.

The design pass must produce:

- a complete agent workflow from context/catalog to persisted verification;
- a reuse decision for the existing plan/validate/preview/verify commands;
- exact structured DTOs and generated schemas for recipe assets, steps, bindings, parameters, and player policies;
- read-only inspect/diff/validate commands before write commands;
- dry-run/confirm, preflight token, hash, stable-reference, path, idempotency, Undo, partial-failure, and retry semantics for every write;
- clear separation between creating/updating a recipe asset and adding/updating a scene or prefab `TweenPlayer`;
- capability/version negotiation so an agent never assumes unsupported operation families or schema versions;
- a documented decision to omit delete, event wiring, arbitrary source generation, or other unsafe operations from the initial command set.

Update both this document and the CLI roadmap with the accepted command contract before implementation.

## Gate E — final roadmap validation

Run one final design review after Gates B–D. Mark the roadmap `READY_FOR_IMPLEMENTATION` only when all checks pass:

- [ ] No circular dependency remains between the base feature and CLI authoring.
- [ ] The base package compiles and functions with Pipeline absent.
- [ ] The serialized schema has stable IDs, deterministic defaults, canonical wire mapping, and a migration story.
- [ ] Asset bindings never rely on scene references, hierarchy names, or implicit selection.
- [ ] The runtime executor has one authoritative implementation path per supported operation.
- [ ] Player lifecycle and ownership agree with `TweenHandle`, DOTween linking, and existing reset semantics.
- [ ] Editor preview cannot silently dirty or save the user's source scene, prefab, or assets.
- [ ] CLI writes are bounded, explicit, idempotent, preflighted, and report every mutation.
- [ ] Initial operation coverage, unsupported features, validation method, documentation, samples, and manual-review scope are explicit.
- [ ] Packaging and release-version implications are recorded.
- [ ] The current worktree and baseline review data have been recounted so unrelated changes and prior validation are preserved.

Record the validation date, reviewer, CLI version/commit, TweenHelper version/commit, catalog hash, recipe schema version, accepted risks, and final verdict at the top of this file.

## Provisional architecture

This architecture is the recommended starting point for Gate C, not permission to implement before the gate.

### Serialized domain

- `TweenRecipe` is a reusable `ScriptableObject` containing schema metadata, declared binding slots, recipe-level playback defaults, and an ordered step list.
- Each step has a stable step ID, stable operation ID, target binding IDs, append/join relationship, optional interval/delay, a duration override, typed options, and a descriptor-specific parameter payload.
- The asset stores no direct scene-object references. Tooling identifies an asset through Unity GUID/file ID; runtime execution does not depend on `AssetDatabase`.
- Binding definitions declare a stable key, display label, required target capability/type, single-versus-ordered-collection cardinality, and whether the slot is optional.
- `TweenPlayer` stores the recipe reference and the concrete serialized bindings for its scene object or prefab instance.
- Unknown operation IDs, duplicate step/binding IDs, missing required bindings, invalid join placement, parameter mismatches, and unsupported loops are validation errors.

A stable operation-ID plus typed-payload design is preferred over persisting arbitrary CLR type names because it aligns with canonical CLI plans and survives implementation refactors. The final serialization mechanism must still be selected and round-trip tested at Gate C.

### Timeline semantics

Recipe schema v1 should support:

- append after the current timeline;
- join at the previous step's start;
- explicit interval steps;
- per-step delay and duration override;
- allowlisted ease, loop, update, snapping, strength, overshoot, alpha, scale, direction, destination, color, numeric, and text-format parameters where the descriptor supports them;
- finite recipes by default, with infinite root loops admitted only when lifecycle ownership and stop behavior are explicit.

Schema v1 should not support nested recipes, branches, conditions, arbitrary callbacks, raw tween injection, reflection-provided methods, or runtime script generation.

### Validation and compilation

Use one Pipeline-neutral validation/compiler path for the Inspector, runtime preflight where appropriate, showcase fixtures, and CLI adapter:

1. Resolve schema and migration status.
2. Resolve operation descriptors by stable ID.
3. Validate step structure, parameters, binding cardinality, target capability, timing, and lifecycle.
4. Resolve defaults and duration precedence into a normalized executable model.
5. Build owned TweenHelper/DOTween tweens through allowlisted executors.
6. Return a `TweenHandle` and structured validation/compilation issues.

Do not duplicate operation semantics in the Inspector and CLI adapters. They should present or transport the same domain results.

### TweenPlayer lifecycle

The first public component should provide:

- explicit recipe and binding fields;
- manual playback plus an optional documented automatic trigger;
- `Play`, `Pause`, `Resume`, `Restart`, `Rewind`, `Complete`, and `Kill` controls consistent with `TweenHandle`;
- read-only active/playing/paused/completed state;
- one clearly owned active playback by default;
- explicit replay/conflict and disable/destroy policies;
- additive C# callbacks and focused Inspector events without serializing executable callbacks into the recipe;
- validation in one appropriate lifecycle/editor point rather than repetitive runtime fallback lookups.

The player must never call global DOTween cleanup APIs or control tweens it does not own.

### Editor authoring

The base Editor feature should include:

- asset creation under **Tools > TweenHelper** and the normal `Create` asset menu;
- a searchable operation picker backed by the shared descriptor catalog;
- ordered step editing with append/join visualization, typed parameter controls, binding-slot mapping, duplicate/reorder/remove, and inline validation;
- `TweenPlayer` Inspector controls and clear required-binding diagnostics;
- preview/replay/stop/reset using a Pipeline-neutral isolated preview service or another reviewed non-destructive mechanism;
- Undo for scene/prefab component changes and normal Unity asset dirtiness for asset edits;
- no automatic scene/prefab save and no attempt to erase pre-existing dirty state.

### Assembly and folder boundaries

Provisional public layout:

```text
Assets/Loags/TweenHelper/
|-- Runtime/
|   |-- Recipes/
|   |   |-- Models/
|   |   |-- Validation/
|   |   `-- Execution/
|   `-- Player/
|       `-- TweenPlayer.cs
|-- Editor/
|   `-- Recipes/
|       |-- Authoring/
|       |-- Preview/
|       `-- Migration/
|-- Documentation/
|   |-- SerializedRecipes.md
|   |-- TweenPlayer.md
|   `-- RecipeMigration.md
`-- Samples/TweenHelper Demos/
    `-- recipe assets, prefabs, and showcase content

Assets/_Project/TweenHelperDevelopment/
|-- CLI/                                  # repository-only adapter and developer commands
|-- Validation/                           # review integration and validation fixtures
`-- Documentation/
    |-- TweenHelperPipelineCliRoadmap.md
    |-- SerializedAnimationRecipesTweenPlayerRoadmap.md
    `-- future post-CLI audit record

Separate public Pipeline companion root/package
`-- recipe/player command adapters only; no duplicated runtime model
```

The exact layout may change at Gate C, but runtime assemblies must not reference Editor, development, or Pipeline assemblies.

## Implementation phases after all gates open

Each phase is independently reviewable. Do not implement all phases as one unvalidated change.

### Phase 1 — freeze the recipe contract

- [ ] Add the versioned `TweenRecipe` model, binding definitions, step/timing model, typed parameter representation, and recipe-level policies.
- [ ] Define stable ID generation and duplication behavior for steps and binding slots.
- [ ] Add structured issue codes and a pure validation result model.
- [ ] Add deterministic schema migration entry points without silently rewriting assets at runtime.
- [ ] Document schema v1 and unsupported constructs.

Exit criteria: assets serialize, reload, duplicate, and round-trip without losing identity or meaning; invalid data produces precise field/step issues.

### Phase 2 — descriptors, validation, and executable normalization

- [ ] Connect recipe operations to the reviewed shared descriptor catalog.
- [ ] Validate target capabilities, binding cardinality, parameters, timing, loops, and lifecycle policy.
- [ ] Resolve defaults and duration precedence into a normalized Pipeline-neutral executable model.
- [ ] Reject unknown/unsupported operations and extra payload fields rather than guessing.
- [ ] Prove base compilation with Pipeline absent.

Exit criteria: the same recipe, bindings, settings, and catalog produce the same normalized execution model and issues.

### Phase 3 — runtime executor and TweenPlayer

- [ ] Compile the initial operation slice into owned TweenHelper/DOTween tweens.
- [ ] Implement `TweenPlayer` binding resolution and playback controls over one active `TweenHandle`.
- [ ] Implement replay/conflict, disable, destroy, completion, kill, and rewind behavior.
- [ ] Expose focused runtime state, callbacks, and Inspector events.
- [ ] Prevent global cleanup and unrelated tween ownership.

Exit criteria: repeated play, pause/resume, restart, rewind, complete, early kill, disable, and target destruction follow the documented state contract.

### Phase 4 — Editor authoring and preview

- [ ] Add recipe asset creation and a focused authoring Inspector/window.
- [ ] Add operation search, typed fields, ordered timeline editing, and binding diagnostics.
- [ ] Add `TweenPlayer` Inspector authoring and safe controls.
- [ ] Reuse or extract the audited Pipeline-neutral sandbox/preview services where appropriate.
- [ ] Prove preview cleanup and source scene/prefab/asset dirty-state preservation.

Exit criteria: a designer can create, bind, validate, preview, stop, reset, and replay an MVP recipe without writing code or leaving hidden tween state.

### Phase 5 — operation-family coverage

Admit one family only after its descriptors, serializer, validator, executor, preview observation, lifecycle behavior, and review fixture agree:

1. finite built-in presets;
2. allowlisted core fluent steps plus append/join/interval;
3. destination motion with explicit local/world/anchored semantics;
4. gameplay feedback with documented restore behavior;
5. staggered collections with ordered collection bindings and finite-child rules;
6. production UI sequences with secondary/collection bindings and cache side effects;
7. TMP text/value operations using serializable format strings, not arbitrary formatter delegates;
8. infinite root loops only after explicit stop/kill policy validation.

Exit criteria: each advertised family has one authoritative descriptor/executor path and unsupported overloads remain explicitly unavailable.

### Phase 6 — showcase, review workflow, and documentation

- [ ] Add a Recipes/Players showcase section using shipped sample assets and prefab wiring.
- [ ] Append stable review IDs without altering historical IDs or reviewed states.
- [ ] Cover at least preset chaining, parallel move/fade, delayed feedback, destination motion, collection stagger, multi-target UI, TMP/value animation, looping lifecycle, and player controls where supported.
- [ ] Update the review controller so only new entries begin as unreviewed and normal next-entry playback continues.
- [ ] Document API, asset authoring, binding, lifecycle, migration, operation coverage, limitations, and code/Inspector parity.

Exit criteria: Unity compiles, references resolve, prior review data is preserved, and the newly added entries are ready for manual visual validation.

### Phase 7 — CLI recipe and TweenPlayer authoring

- [ ] Implement the Gate D command contract in the repository adapter first.
- [ ] Reuse existing plan, validation, preview, verification, hashing, references, and job services rather than creating a second semantics layer.
- [ ] Implement read-only inspect/diff/validate workflows before mutation.
- [ ] Implement exact dry-run preflight and token-confirmed, idempotent asset/component writes with Undo and complete mutation reporting.
- [ ] Verify the persisted recipe and player through the same normalized plan and sandbox verification path.
- [ ] Extract only generic reviewed commands to the selected public companion.
- [ ] Return to CLI roadmap Phase 6 and mark persisted authoring complete only after its release-hardening gates pass.

Exit criteria: an agent can safely create or update a supported recipe and player, and a retry cannot duplicate assets, components, steps, or bindings.

### Phase 8 — final validation and release readiness

- [ ] Complete manual visual review of every new recipe/player entry.
- [ ] Confirm serialized assets survive domain reload, Editor restart, duplication, prefab instantiation, and supported schema migration.
- [ ] Confirm base import/compile with Pipeline absent and companion behavior in every declared compatibility tuple.
- [ ] Confirm no missing references, unexpected dirtiness, leaked tweens, hidden objects, or stale preview/job sessions.
- [ ] Validate documentation, changelog, packaging roots, dependency disclosure, and exact exported content during an explicitly requested release task.
- [ ] Set the release version only after the exact-artifact evidence passes.

Exit criteria: both the designer workflow and agent workflow are stable, documented, manually reviewed, and packaged without weakening the base product boundary.

## Candidate CLI command changes for Gate D

These are design candidates, not approved command IDs. Prefer extending existing commands over adding overlapping aliases.

| Candidate | Side effects | Purpose |
| --- | --- | --- |
| Extend `tween_helper_catalog` / `describe_operation` | Read-only | Advertise recipe support, serialized parameter shapes, binding requirements, and recipe schema compatibility for each operation. |
| Extend `tween_helper_plan` | Read-only | Emit a recipe-compatible normalized plan, stable step/binding IDs, and resolved defaults. |
| `tween_helper_recipe_inspect` | Read-only | Read a persisted recipe by stable asset reference and return its normalized model, schema/migration status, issues, and content hash. |
| `tween_helper_recipe_validate` | Read-only | Validate either an in-memory recipe specification or a persisted asset against the current catalog without mutating it. |
| `tween_helper_recipe_diff` | Read-only | Compare a proposed recipe plan with a persisted recipe and return an exact semantic change set. |
| Reuse or specialize `tween_helper_apply_plan` | Project write after preflight | Create or update one recipe asset through dry-run plus token-confirmed apply. Gate D decides whether a narrower `recipe_apply` command is clearer. |
| `tween_helper_player_inspect` | Read-only | Describe a `TweenPlayer`, recipe reference, policies, explicit binding identities, validation issues, and current dirty/save context. |
| `tween_helper_player_apply` | Scene/prefab write after preflight | Add or update a player and its explicit bindings without hierarchy lookup, implicit selection, auto-save, or event rewiring. |
| Extend preview/verify commands | Owned temporary state only | Accept a recipe plan or persisted recipe plus explicit bindings and return the same sandbox lifecycle evidence. |
| `tween_helper_recipe_migrate` | Gated future write | Preflight and migrate an older supported recipe schema. Omit from the first command slice unless a real migration exists. |

Initial agent authoring should not expose recipe deletion, arbitrary asset moves, nested recipe expansion, persistent UnityEvent wiring, raw YAML editing, or arbitrary source-file generation.

## Validation matrix

| Area | Required evidence |
| --- | --- |
| Serialization | Asset create/save/reload/duplicate; stable step/binding IDs; deterministic defaults; unknown-version refusal; supported migration result. |
| Catalog | Stable operation IDs; descriptor/schema match; no arbitrary reflection; explicit unsupported families and overloads. |
| Bindings | Self, explicit object, secondary object, and ordered collection where supported; missing/wrong/duplicate bindings rejected; no hierarchy strings. |
| Timeline | Append, join, interval, per-step delay, duration precedence, root loops, invalid first-step join, and deterministic ordering. |
| Runtime lifecycle | Play, replay, pause/resume, restart, rewind, complete, early kill, disable, destroy, target loss, and no unrelated tween control. |
| Operation families | Endpoint/baseline/cleanup behavior for each advertised family before its capability becomes true. |
| Editor authoring | Undo, prefab instance/asset behavior, validation messages, asset dirtiness, no automatic save, and no lost serialized data. |
| Preview | Owned sandbox, source unchanged, no hidden objects or handles after stop/error/reload/timeout, and explicit nondeterminism labels. |
| CLI reads | Generated schemas, canonical hashes, stable references, inspect/diff/validate idempotency, bounded payloads, and safe path reporting. |
| CLI writes | Dry-run no-op, exact preflight token, expiry/mismatch, duplicate request ID, stale state, path confinement, Undo limits, partial failure, and changed identities. |
| Compatibility | Base on supported Unity with Pipeline absent; companion only on declared Unity/Pipeline tuples; DOTween/UI/TMP requirements disclosed. |
| Review scene | All historical IDs/results preserved; only appended entries unreviewed; every new entry manually observed and resolved. |
| Packaging | No development CLI/tests/reports in the base export; no Pipeline reference in Runtime/base Editor; companion contains no project-only fixtures. |

Automated test additions remain subject to the repository rule requiring explicit authorization. Static checks, existing validation paths, Unity compilation, Editor inspection, and the manual review scene are still mandatory for each implementation phase.

## Risks to resolve before implementation

- Unity serialization can make polymorphic payloads and type renames fragile; do not select `[SerializeReference]` solely for implementation convenience.
- A ScriptableObject cannot safely own reusable scene references; keep concrete targets on the player.
- Multi-target UI and collection operations require cardinality-aware binding schemas and cannot be forced into a one-target preset model.
- TMP formatter delegates, builder callbacks, and raw tween factories are code behavior, not safe serialized data.
- Editor preview can dirty source objects even if values are restored; use the audited sandbox pattern and report unsupported previews.
- Operation catalog duplication between Runtime, base Editor, development CLI, and public companion will drift unless one Pipeline-neutral source of truth is selected.
- Changing operation IDs or schema defaults after assets ship is a compatibility change requiring migration and release notes.
- Concurrent agent/user edits require optimistic hashes and revalidation immediately before mutation.
- The feature is large enough to justify a minor release rather than silently expanding a patch release; confirm this at Gate E.

## Non-goals for the first release

- A general visual scripting system, state machine, Timeline replacement, or Animator replacement.
- Nested recipes, conditionals, branching, variables driven by arbitrary expressions, or event graphs.
- Arbitrary C# callbacks, raw DOTween injection, custom formatter delegates, or caller-supplied reflected methods.
- Runtime hierarchy/name search or implicit target discovery.
- Automatic package installation, scene/prefab saving, source-code generation, or asset deletion.
- Editing persistent UnityEvents through the first CLI command slice.
- Supporting every TweenBuilder overload before the descriptor/executor contract is proven family by family.
- Requiring Pipeline in the base TweenHelper product.

## Resume handoff

When the CLI foundation is reported complete, the next task must not be “implement TweenRecipe.” The next task is:

1. Read this roadmap and the current CLI roadmap completely.
2. Capture fresh worktree, Unity Editor, package, assembly, command-schema, Console, scene/prefab, and validation state.
3. Verify Gate A with evidence.
4. Perform Gate B and create the post-CLI audit record.
5. Revise the roadmap through Gate C.
6. Design and document the agent command slice through Gate D.
7. Run Gate E and change the status to `READY_FOR_IMPLEMENTATION` only if it passes.
8. Implement Phase 1 only, validate it, and then proceed phase by phase.

## References

- `ROADMAP.md`
- `Assets/_Project/TweenHelperDevelopment/Documentation/TweenHelperPipelineCliRoadmap.md`
- `Assets/Loags/TweenHelper/Runtime/Core/TweenBuilder.cs`
- `Assets/Loags/TweenHelper/Runtime/Core/TweenHandle.cs`
- `Assets/Loags/TweenHelper/Runtime/Core/TweenPresetRegistry.cs`
- `Assets/Loags/TweenHelper/Runtime/Core/TweenOptions.cs`
- `Assets/Loags/TweenHelper/Runtime/Core/UIAnimationStateCache.cs`
- `Assets/Loags/TweenHelper/Editor/PresetBrowserPreview.cs`
- `Assets/Loags/TweenHelper/Documentation/API.md`
- `Assets/Loags/TweenHelper/Documentation/Lifecycle.md`
- operation-family documentation under `Assets/Loags/TweenHelper/Documentation`
- review tooling under `Assets/_Project/TweenHelperDevelopment/Validation`
