# TweenHelper Pipeline CLI prototype

This repository-only Editor integration intentionally exposes one product command and one developer diagnostic command:

- `tween_helper_catalog` lists TweenHelper's built-in presets with optional `query`, `family`, `offset`, and `limit` arguments.
- `tween_helper_dev_telemetry_summary` summarizes local developer telemetry without accepting arguments.

That is the smallest useful custom surface. Unity Pipeline already exposes Editor status, package inspection, selection, object resolution, scenes, and component inspection, so TweenHelper does not duplicate those commands.

## Example

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

Pipeline owns the response envelope. The command returns this result directly:

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
      "description": "...",
      "family": "Fade",
      "defaultDuration": 0.3
    }
  ]
}
```

Invalid ranges use Pipeline's normal command-error response. `offset` must be non-negative and `limit` must be between 1 and 100.

## Developer telemetry

Telemetry is local, repository-only, enabled by default, and toggled from `Tools/TweenHelper/Development/Record CLI Telemetry`. Each entered `tween_helper_*` handler appends one completion event to `Library/TweenHelper/cli-telemetry-v1.jsonl`. Turning recording off creates the local opt-out marker `Library/TweenHelper/cli-telemetry.disabled`; turning it back on removes that marker.

Each event contains only schema version, UTC completion time, allowlisted command ID, handler duration, `success`/`exception` status, and an optional sanitized exception type. It never stores command inputs or outputs, query text, object or project identities, paths, request IDs, exception messages, stacks, source content, authentication data, or raw Pipeline logs.

The active file rotates at 5 MiB to one backup named `cli-telemetry-v1.old.jsonl`. Recorder failures never change the command result or exception. The summary command reads the active file only and reports total/success/exception counts and average/maximum duration by command plus overall status counts; malformed lines are skipped and counted. Because its completion is appended after its snapshot, a summary does not include itself until the next call.

Coverage begins after Pipeline argument binding. Requests rejected before handler entry, built-in/third-party Pipeline commands, non-Pipeline tools, and direct domain calls are not observed. See [TweenHelperPipelineCliTelemetryRoadmap.md](../Documentation/TweenHelperPipelineCliTelemetryRoadmap.md) for the exact contract and exclusions.

## Boundaries

- The command is read-only and scans only the TweenHelper runtime assembly for attributed built-in presets.
- It never refreshes or modifies `TweenPresetRegistry`, and it never discovers project-defined extension presets.
- It stays outside `Assets/Loags/TweenHelper`, so the base customer artifact remains Pipeline-free and compatible with its existing Unity version range.
- Target profiling, operation hashes, canonical plans, preview sessions, verification jobs, and persistence are deferred until a concrete workflow needs them.
- Telemetry has no network path, public-package integration, raw log import, session/span model, background queue, or event management command set.
