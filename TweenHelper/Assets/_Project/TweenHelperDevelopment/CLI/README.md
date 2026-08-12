# TweenHelper Pipeline CLI development prototype

This folder contains the repository-only Phase 0/1 implementation from `TweenHelperPipelineCliRoadmap.md`. It is an Editor-only development adapter, not part of the customer TweenHelper artifact under `Assets/Loags/TweenHelper`.

## Distribution and capability boundary

- The base TweenHelper artifact remains Pipeline-free and keeps its Unity `2022.3+` compatibility boundary.
- This adapter is pinned to Unity Pipeline `0.3.1-exp.1`, which requires Unity `6000.0+`.
- Public distribution remains unresolved pending the publisher/product decision and clean-artifact compatibility tests. A separate UPM companion is preferred if publisher enrollment permits it; a separately tested `.unitypackage` is only a fallback.
- Public persistence is disabled. This prototype exposes no planning, preview, verification-job, apply, save, import, fixture, Play Mode, or extension-discovery capability.
- Pipeline is a loopback development service with its own broader command surface. These TweenHelper commands do not sandbox or restrict Pipeline itself.

The DOTween setup policy distinguishes package compatibility from the loaded runtime API: the distributable package floor is `1.2.025`, while this repository's effective runtime floor is `1.3.030`. The development adapter requires the runtime check to pass and reports both values explicitly.

## Assembly boundary

- `Core/LB.TweenHelper.Development.Automation.Editor` contains Pipeline-neutral domain models, hashing, built-in preset descriptors, catalog queries, compatibility diagnostics, and target profiling.
- `Adapter/LB.TweenHelper.Development.Pipeline.Editor` contains Pipeline DTOs, explicit object-reference resolution, result mapping, and `[CliCommand]` entry points.
- Built-in discovery scans only `typeof(ITweenPreset).Assembly`, constructs only attributed built-in preset types, expects exactly 300 descriptors, and never calls `TweenPresetRegistry.Refresh()` or scans project extensions.

## Frozen Phase 0 wire contract

Every command has one required `input` object. `input.schemaVersion` is currently `1`; pure reads may omit `requestId`. Unknown DTO fields and handled domain failures return a TweenHelper body with `status: "invalid"` and typed issues rather than throwing across the transport boundary.

Pipeline owns the outer HTTP envelope. The value in its `result` field is the TweenHelper body:

```json
{
  "schemaVersion": 1,
  "toolVersion": "0.1.0-dev.1",
  "status": "ready|valid|invalid",
  "warnings": [{ "code": "...", "message": "...", "fieldPath": "..." }],
  "errors": [{ "code": "...", "message": "...", "fieldPath": "..." }],
  "data": {},
  "requestId": "optional-caller-id"
}
```

The developer command IDs are:

- `tween_helper_context`
- `tween_helper_setup_status`
- `tween_helper_catalog`
- `tween_helper_describe_operation`
- `tween_helper_target_profile`
- `tween_helper_dev_contract_probe`

`tween_helper_target_profile` accepts exactly one explicit `globalId`, project-relative `path`, `guid` (optionally with `fileId`), decimal-string `instanceId`, or `useSelection: true`. Selection is never an implicit fallback. Results omit hierarchy paths, object names, absolute paths, project names, authentication data, and arbitrary component state.

## Canonical JSON and SHA-256 specification

Canonical values are emitted as UTF-8 without a byte-order mark or insignificant whitespace. Call sites write object properties in their schema-defined ordinal order; arrays preserve semantic order, while set-like inputs are sorted with `StringComparer.Ordinal` before writing.

- Strings are normalized to Unicode NFC and use JSON escaping. Control-character Unicode escapes use four lowercase hexadecimal digits.
- Booleans are lowercase JSON literals. Integers use invariant base-10 formatting.
- `float` and `double` use invariant round-trip (`R`) formatting. Positive and negative zero both encode as `0`; NaN and infinities are rejected.
- Nulls are explicit where the hash schema includes a nullable field. Inclusion or omission of defaults is defined by each hash schema, never by Newtonsoft's property order or ambient serializer settings.
- Enums and identifiers are normalized by the owning schema before writing.
- SHA-256 is calculated over the canonical UTF-8 bytes and returned as `sha256:` plus 64 lowercase hexadecimal digits.

Catalog hashes cover the catalog schema version, built-in scope, and execution-relevant descriptor fields in operation-ID order; documentation-only text is excluded. Catalog cursors are bound to that hash, their normalized filters, and the resulting operation-ID sequence. Target-profile hashes cover the preferred canonical identity (stable `globalId`, then GUID/file ID, then asset path, with instance ID only for transient objects), allowlisted components, activity/UI classification, transform/RectTransform baselines, and allowlisted visual channels; compatible-operation cursors are additionally bound to the catalog hash. Request IDs, timestamps, Pipeline descriptor/token data, machine paths, and unrelated project state are excluded.

The contract probe exists only to prove, through Pipeline's generated `/api/commands` schema and binder, that object references, vectors, and colors remain nested JSON objects with `additionalProperties: false` rather than string fallbacks.
