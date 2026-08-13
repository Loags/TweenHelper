# TweenHelper Pipeline CLI roadmap

- Status: Simplified repository prototype with default-on local developer telemetry
- Last reviewed: 2026-08-13
- Scope: the smallest useful TweenHelper-specific command surface on Unity Pipeline

## Review outcome

The first prototype proved that custom commands, structured schemas, object references, canonical hashing, and isolated HTTP tests could all work. It also built too much infrastructure before any command could plan or execute an animation.

The prototype had six commands, two assemblies, five domain services, custom result envelopes, opaque cursors, target-state snapshots, a canonical JSON writer, several hashes, and a schema-probe command. Most of that either duplicated Unity Pipeline or existed only for hypothetical later phases.

The revised rule is simple: keep only behavior that provides a unique TweenHelper capability today. Add infrastructure when a concrete consumer needs it, not in anticipation of one.

## Current MVP

The repository exposes one read-only TweenHelper capability and one developer diagnostic:

| Command | Purpose |
| --- | --- |
| `tween_helper_catalog` | List the 300 built-in TweenHelper presets with optional text/family filtering and ordinary offset/limit paging. |
| `tween_helper_dev_telemetry_summary` | Summarize bounded, explicitly enabled metadata about entered TweenHelper CLI handlers. |

Everything else is supplied by Unity Pipeline's existing commands or deferred.

### Command shape

The command uses normal Pipeline arguments instead of a nested versioned input DTO:

| Argument | Type | Default | Meaning |
| --- | --- | --- | --- |
| `query` | string | empty | Case-insensitive match against preset name, family, or description. |
| `family` | string | empty | Case-insensitive exact family filter. |
| `offset` | integer | `0` | Zero-based result offset. |
| `limit` | integer | `50` | Page size from 1 through 100. |

Example request:

```json
{
  "command": "tween_helper_catalog",
  "parameters": {
    "family": "Fade",
    "offset": 0,
    "limit": 1
  }
}
```

The command returns its value directly inside Pipeline's existing `result` field:

```json
{
  "presetCount": 300,
  "matchedCount": 9,
  "offset": 0,
  "returnedCount": 1,
  "hasMore": true,
  "presets": [
    {
      "name": "FadeIn",
      "description": "Fades the target in.",
      "family": "Fade",
      "defaultDuration": 0.3
    }
  ]
}
```

Pipeline's outer envelope already reports success, command ID, timing, and errors. TweenHelper does not add a second status/error/version envelope. The optional local telemetry retains only enough handler metadata to aggregate calls across requests without enabling Pipeline's raw transaction log.

### Catalog source

The catalog scans only `typeof(ITweenPreset).Assembly` for concrete `ITweenPreset` types marked with `[AutoRegisterPreset]`. It constructs those built-in types, sorts them by `PresetName` using ordinal comparison, and exposes only properties backed by the current preset interface plus the established `PresetVariantParser` family.

It deliberately does not:

- call `TweenPresetRegistry.Refresh()` or `ScanForCodePresets()`;
- scan project or development assemblies;
- infer mutation footprints, verification oracles, determinism, loop behavior, or target requirements from naming heuristics;
- manufacture a second operation ID when `PresetName` is already the runtime lookup key;
- hash the catalog before a plan actually depends on catalog identity.

## What was removed

| Removed surface | Reason |
| --- | --- |
| `tween_helper_context` | Duplicated Pipeline Editor status, scene, selection, and package inspection. |
| `tween_helper_setup_status` | Presence of the compiled adapter already proves required compile-time dependencies; optional setup remains in TweenHelper's existing Editor setup UI. |
| `tween_helper_describe_operation` | Catalog rows now contain all currently trustworthy preset metadata. A separate detail command has no distinct job yet. |
| `tween_helper_target_profile` | Pipeline already resolves and inspects objects/components. TweenHelper-specific target fingerprints matter only once a plan is executable. |
| `tween_helper_dev_contract_probe` | It was test scaffolding exposed as a product command. Future structured DTOs should be tested with the command that actually uses them. |
| Canonical JSON and SHA-256 code | No current result is replayed or used as an integrity boundary. Add canonicalization with the first self-contained plan. |
| Opaque bound cursors | A static 300-entry list needs only transparent offset/limit paging. |
| Custom issue/result hierarchy | Pipeline already provides the transport success/error contract. Domain errors can be introduced only when a real domain workflow needs typed recovery. |
| Pipeline-neutral core assembly | One small repository-only command does not justify an abstraction boundary. Extract a domain assembly when a second adapter or non-Pipeline consumer exists. |

## Safety and packaging boundaries

- Code remains under `Assets/_Project/TweenHelperDevelopment/CLI` and is Editor-only.
- The base artifact under `Assets/Loags/TweenHelper` remains free of Unity Pipeline references and keeps its existing Unity compatibility range.
- The command is read-only. It does not modify scenes, prefabs, assets, settings, selection, Play Mode, or the global preset registry.
- It returns no project name, machine path, authentication data, object identity, hierarchy path, source content, or arbitrary component state.
- The development adapter remains pinned to the Pipeline version installed by this repository. No public compatibility claim is made yet.
- A public companion package is considered only after the command surface provides enough value to justify installation and clean-project compatibility testing.

## Demand-driven roadmap

### Stage 1 - Catalog prototype (implemented)

- [x] Register one unique `tween_helper_catalog` command.
- [x] Discover built-in presets without mutating `TweenPresetRegistry`.
- [x] Return bounded, filterable preset metadata.
- [x] Keep the customer artifact Pipeline-free.
- [x] Confirm compilation, command discovery, direct behavior, live HTTP behavior, and the repository EditMode suite after this simplification.

Exit criterion: an agent can discover the available TweenHelper presets without custom infrastructure beyond the catalog itself.

### Stage 1b - Developer telemetry (current)

- [x] Add one completion wrapper around every current TweenHelper command.
- [x] Keep recording local, enabled by default, and explicitly developer-controlled through a persistent opt-out.
- [x] Retain only command ID, completion time, duration, outcome, and optional exception type.
- [x] Bound storage to one 5 MiB JSONL file and one backup.
- [x] Add one active-window aggregate summary command.
- [x] Keep command results and exception identity unchanged when recording fails.
- [x] Capture fresh Unity compilation, discovery, and EditMode evidence for the extension.

Exit criterion: a maintainer can count slow or throwing TweenHelper handler calls without storing command payloads or enabling raw Pipeline logs.

Previous validation evidence from 2026-08-13, before recording became enabled by default: the sources compiled with no compiler errors; the running Unity `6000.5.2f1` Editor was ready with Play Mode stopped; command discovery found exactly the catalog and telemetry-summary commands; `LB.TweenHelper.EditorTests` passed 24/24 in EditMode; a live catalog call returned 300 total presets through Pipeline's native result envelope; a live summary call returned the then-default disabled, empty schema-v1 result; and the post-run Console contained no errors.

### Stage 2 - First executable planning slice (only when requested)

Choose one concrete user workflow, such as applying one built-in preset to one explicit target. Before adding commands, write down the exact input, output, supported target types, duration policy, and what execution means.

Likely additions are a plan/validate pair, but their names and schemas are not frozen until the workflow is chosen. At that point:

- add only metadata required to validate that workflow;
- use Pipeline's existing object-reference type/resolver rather than a parallel reference DTO;
- introduce a self-contained plan and canonical hash because downstream replay then requires integrity;
- introduce typed domain issues only for errors callers can act on;
- keep planning and validation read-only;
- add target/configuration fingerprints only over values the operation actually consumes.

Exit criterion: one supported request produces a replayable plan that can be validated after a command round trip without hidden in-memory state.

### Stage 3 - Preview or verification (only after planning is useful)

Add a sandbox lifecycle only if users need visual or behavioral evidence before authoring. Prefer the fewest commands consistent with reliable cleanup. The sandbox must own every temporary object and tween, never mutate the source target, use manual time where practical, and survive retryable cleanup.

Do not build a generic job framework until work is demonstrably asynchronous or exceeds a normal command duration.

Exit criterion: the selected Stage 2 plan can be exercised and cleaned up deterministically without dirtying user content.

### Stage 4 - Persistence (separate product decision)

No `apply` command is planned until TweenHelper has an approved serialized recipe/component representation. Persistence then needs a real dry-run change set, Undo/rollback behavior, idempotency, stale-state checks, and explicit confirmation. This stage is independent from proving the read-only CLI.

Exit criterion: persistence is a supported TweenHelper product feature with migration and runtime ownership rules, not a CLI-only side effect.

## Gate for every new custom command

A proposed command is added only when all answers are yes:

1. Does it provide TweenHelper-specific value not already available through Pipeline?
2. Is there a concrete workflow that consumes its result now?
3. Is a separate command clearer than extending an existing result or using a built-in command?
4. Can its input/output contract be explained in a short example?
5. Is its mutation and cleanup boundary testable?
6. Can it be implemented without building a framework for unrequested future cases?

If a design needs a new hash, cache, cursor, session store, telemetry stream, or assembly, the implementing change must identify the present consumer and failure mode that justifies it.

## Validation matrix

The current slice needs only proportional evidence:

- Unity compiles without new Console errors.
- `CommandRegistry` discovers exactly the catalog command with its four optional arguments and the parameterless developer summary command.
- Three bounded pages cover exactly 300 unique built-in preset names.
- Filtering is case-insensitive and paging rejects invalid ranges.
- Catalog reads do not change the global preset registry.
- Disabled telemetry writes no files; enabled telemetry covers both registered commands without retaining inputs or exception messages.
- Recorder write failures preserve command results, storage rotates to one backup, and summaries count malformed lines instead of failing.
- A live authenticated Pipeline call returns the native result shape.
- The full existing EditMode test assembly passes.
- No Play Mode, batch build, package/manifest edit, scene save, or customer-artifact change occurs.

## Telemetry

The simplified developer telemetry MVP is implemented under the same repository-only Editor adapter. It records one allowlisted completion event per entered TweenHelper handler by default, supports a persistent local opt-out, stores no payloads, uses one size-bounded file plus one backup, and exposes one active-window summary. The exact coverage, retained fields, privacy boundary, and deliberately excluded infrastructure are frozen in [TweenHelperPipelineCliTelemetryRoadmap.md](TweenHelperPipelineCliTelemetryRoadmap.md).

## References

- Unity Pipeline `Documentation~/creating-commands.md`
- Unity Pipeline `Documentation~/safety-and-mutations.md`
- TweenHelper `ITweenPreset`, `AutoRegisterPresetAttribute`, `PresetVariantParser`, and `TweenPresetRegistry`
- [SerializedAnimationRecipesTweenPlayerRoadmap.md](SerializedAnimationRecipesTweenPlayerRoadmap.md) for the separate persistence representation decision
