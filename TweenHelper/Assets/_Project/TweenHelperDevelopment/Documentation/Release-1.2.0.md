# Tween Helper 1.2.0 release record

Status: **release candidate validated**

Updated: 2026-08-30

Publisher package: `1453516`

## Release identity

| Field | Value |
| --- | --- |
| Product | Tween Helper |
| Version | `1.2.0` |
| Publisher | Loags |
| Category | Tools > Animation |
| Distribution root | `Assets/Loags/TweenHelper` only |
| Unity validation | `6000.5.2f1` |
| DOTween validation | Free package `1.2.825`, runtime `1.3.030`; installed separately |
| Render pipelines | Built-in and URP supported; HDRP/custom pipelines untested |
| License | Standard Unity Asset Store EULA |

## Release highlights

- Generalized TextMesh Pro animation across character, word, and line units with shared stagger timing, deterministic noise, exact mesh restoration, and builder/direct API parity.
- Deal In/Out, eight-direction grid serpentine ordering, and two-step layout-difference collection transitions.
- Visual Tween Recipe authoring with explicit bindings, a constrained Then/With timeline, validation, safe Editor preview, and 27 supported runtime operations.
- Corrected multiline Float behavior and grouped line/word stagger, scatter, rotate, and outgoing previews.

## Product surface

| Surface | Count | Meaning |
| --- | ---: | --- |
| Registered presets | 300 | Stable `ITweenPreset` registry |
| Preset Browser | 461 | Customer Editor previews, including semantic operations |
| Animation Gallery | 423 | Shipped customer-facing demo entries |
| Internal review | 610 | Development-only exhaustive configurations |

Counts describe different surfaces and must not be combined into an unqualified animation or preset total.

## Package boundary

Export only `Assets/Loags/TweenHelper`. The package must not include DOTween, `Assets/_Project`, tests, review scenes, Unity Pipeline/MCP, telemetry, Publisher media, Asset Store Publishing Tools, project settings, or repository files.

## Publisher release notes

```text
Tween Helper 1.2.0

- Expanded TextMesh Pro animation across character, word, and line units with ordered stagger, scatter, rotate, shear, tracking, ripple, wiggle, float, swing, pulse, and deterministic scramble effects.
- Added exact TMP mesh/visibility restoration and single-writer ownership across completion, rewind, interruption, restart, kill, and Yoyo loops.
- Added collection Deal In/Out, eight-direction grid serpentine ordering, and layout-difference transitions.
- Added visual Tween Recipe assets, explicit TweenPlayer bindings, a constrained Then/With editor, safe Editor preview, validation, and 27 supported runtime operations.
- Expanded the Preset Browser to 461 isolated entries, the Animation Gallery to 423 entries, and internal review coverage to 610 configurations while keeping the registered preset catalog at 300.
- Corrected multiline Float and grouped line/word stagger, scatter, transform, and outgoing previews.

Developed and validated with Unity 6000.5.2f1 and DOTween Free package 1.2.825 (runtime 1.3.030). DOTween is required, installed separately, and not included.
```

## Publisher summary

```text
Build polished Unity motion with a fluent DOTween API, 300 presets, rich TMP and collection effects, visual Tween Recipes, and 461 isolated previews.
```

## Technical details

```text
Version: 1.2.0
Developed and validated with: Unity 6000.5.2f1
Required external dependency: DOTween (HOTween v2), installed and licensed separately
Validated DOTween package/runtime: 1.2.825 / 1.3.030
Unity features: Unity UI (uGUI) and TextMesh Pro
Registered presets: 300
Preset Browser entries: 461
Animation Gallery entries: 423
Internal review configurations: 610 (development only)
Public namespace: LB.TweenHelper
Supported pipelines: Built-in and URP
Untested pipelines: HDRP and custom
Runtime AI, MCP, telemetry, or online services: none
```

## Dependency disclosure

```text
DOTween (HOTween v2) is required and must be installed, configured, and licensed separately. It is not included with Tween Helper. Version 1.2.0 was validated with DOTween Free package 1.2.825 (runtime 1.3.030).
```

## AI disclosure

```text
OpenAI Codex and ChatGPT were used as AI-assisted development tools for selected functional code implementation and refactoring, documentation drafting, validation design, and consistency review. The publisher reviewed, edited, integrated, and tested the work. Tween Helper contains no runtime AI functionality, makes no AI or MCP service calls, and does not transmit customer, project, or user data to an AI system.
```

## Reviewer note

```text
DOTween is a required external Asset Store dependency and is not included. Install DOTween and run Setup DOTween with its UI module before opening Tween Helper's Animation Gallery or Preset Browser. Tween Helper 1.2.0 was developed and validated with Unity 6000.5.2f1 and DOTween package 1.2.825 (runtime 1.3.030). Built-in and URP are supported; HDRP and custom pipelines are untested. The included gallery is mouse-driven and does not require the Input System. Development validation assets, tests, Unity Pipeline/MCP, telemetry, Publisher media, and Asset Store Publishing Tools are not included.
```

## Validation and artifact

Validated on 2026-08-30 with Unity `6000.5.2f1` using Asset Store Tools `12.0.5` against the exact distribution root `Assets/Loags/TweenHelper`.

| Check | Result |
| --- | --- |
| Unity compilation | Pass; no script compilation errors |
| DOTween setup | Pass; package `1.2.825`, runtime `1.3.030`, required modules available |
| Registered presets | Pass; 300 |
| Animation Gallery | Pass; 423 entries and gallery asset validation completed |
| Tween Recipe review | Pass; 4 samples, 4 Gallery entries, 18 expanded operations, 27 total operations |
| Asset Store Tools | `RanToCompletion`; 35 pass, 1 warning, 0 failures |
| Validator warning | `Check Static Variables` |
| Package manifest | Pass; 182 paths, all under `Assets/Loags/TweenHelper` |
| Exclusions | Pass; no development assets, DOTween copy, Asset Store tooling, Publisher media, release tooling, roadmap, or deprecated documentation |

Artifact: `TweenHelper-1.2.0.unitypackage`

Size: 771,832 bytes

SHA-256: `7662E1E30B6D05F41DB638806A9FF2E4F4F9369A79A831E108C61312388B5F63`

The validation run exported the package successfully and exited Unity batch mode normally. This was an Asset Store package validation/export run, not a Player build or automated test run.
