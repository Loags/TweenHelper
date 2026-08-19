# TweenHelper Pipeline CLI developer telemetry

- Status: MVP implemented and validated
- Last reviewed: 2026-08-19
- Parent: [TweenHelperPipelineCliRoadmap.md](TweenHelperPipelineCliRoadmap.md)

The development project currently uses Unity Pipeline `0.5.0-exp.1`; MCP connectivity was verified on Unity `6000.5.2f1`. Telemetry remains local, repository-only, and absent from the customer artifact.

## Goal

Answer three maintainer questions without retaining command payloads:

1. Which TweenHelper CLI commands are called?
2. Do they complete or throw?
3. How long do their handlers take?

This is repository-only developer diagnostics. It is not customer analytics, does not use the network, and is enabled by default for this development project.

## Implemented scope

Every registered repository-owned `tween_helper_*` handler runs through one generic completion wrapper. When recording is enabled, the wrapper appends one event after the handler returns or throws.

The event contains only:

| Field | Meaning |
| --- | --- |
| `schemaVersion` | Integer event schema, currently `1`. |
| `timestampUtc` | UTC completion time. |
| `commandId` | A value from the repository-owned command allowlist. |
| `durationMs` | Monotonic handler duration, rounded to three decimal places. |
| `status` | `success` or `exception`. |
| `exceptionType` | Optional sanitized CLR type name; no message or stack. |

Inputs, outputs, query text, paths, object identities, request IDs, exception messages, stack traces, source content, project/user/machine names, authentication data, and Pipeline request/response logs are never copied into an event.

### Honest coverage boundary

The recorder sees registered TweenHelper handlers after Pipeline has bound their arguments. It does not see:

- requests rejected before handler entry;
- Pipeline built-in or third-party commands;
- non-Pipeline tools;
- domain calls that bypass the command adapter.

The summary returns this boundary with every result so it cannot be mistaken for all Unity or all Pipeline activity.

## Enablement and storage

Recording is toggled from `Tools/TweenHelper/Development/Record CLI Telemetry`. A clean project is enabled because the local opt-out marker does not exist. Disabling creates `Library/TweenHelper/cli-telemetry.disabled`; enabling removes it.

- Active events: `Library/TweenHelper/cli-telemetry-v1.jsonl`
- One backup: `Library/TweenHelper/cli-telemetry-v1.old.jsonl`
- Active-file limit: 5 MiB
- Retention: the active file plus one replaced backup
- Write model: synchronous best-effort append after command execution

While disabled, the wrapper performs only the enable-marker check before invoking the command; it does not start a clock, serialize an event, or access the event files. If an enabled write fails, the original command result or exception is unchanged. Disabling recording stops new events but intentionally does not delete existing local files.

JSON Lines keeps valid preceding events usable if the final line is interrupted. The reader skips malformed lines and reports their count. Rotation and deletion operate only on the two exact filenames inside the resolved telemetry directory.

## Developer summary command

`tween_helper_dev_telemetry_summary` reads only the current active file and returns:

- recording state, schema version, coverage, exclusions, and storage limits;
- current active-file bytes, valid call count, malformed-line count, and read-failure state;
- total/success/exception counts plus average/maximum handler duration per command;
- count per `success` or `exception` status.

The summary command is recorded like every other TweenHelper command. Its own completion appears only after the snapshot it returned, which prevents recursive self-counting.

## Deliberately excluded

The MVP has no paired start/end events, sessions, spans, correlation IDs, queues, background worker, age-based retention, percentiles, funnels, event query/export/clear commands, remote reporting, raw Pipeline-log import, settings asset, analytics SDK, or public-package integration.

Add one of those only when a measured diagnostic question cannot be answered by the completion events and bounded summary. A growing command count alone is not sufficient justification.

## Implementation checklist

- [x] Keep telemetry under the repository-only Editor adapter.
- [x] Add an enabled-by-default developer toggle with a persistent local opt-out.
- [x] Route every current TweenHelper command through one wrapper.
- [x] Store one allowlisted completion event per entered handler.
- [x] Preserve the original result and exception when recording fails.
- [x] Bound storage to one 5 MiB active file and one backup.
- [x] Add one bounded aggregate summary command.
- [x] Test disabled mode, registration coverage, success, exception identity, redaction, write failure, rotation, and malformed input.
- [x] Capture fresh Unity compilation and EditMode test evidence.

Previous validation evidence from 2026-08-13, before recording became enabled by default: the Unity-generated adapter and test projects compiled with no compiler errors; the running Unity `6000.5.2f1` Editor recompiled the sources while Play Mode remained stopped; `LB.TweenHelper.EditorTests` passed 24/24 in EditMode, including the original telemetry cases; the live Pipeline summary returned the then-default disabled, empty schema-v1 result; and the post-run Console contained no errors.

## Revisit triggers

Revisit the schema only when a real workflow needs a specific aggregate that the current fields cannot produce. Any proposal must name the question, the smallest additional allowlisted field, its privacy risk, its retention behavior, and a test proving that command behavior remains independent from telemetry.

Telemetry must remain absent from `Assets/Loags/TweenHelper` and any future public Pipeline companion unless a separate product and privacy decision explicitly changes that boundary.
