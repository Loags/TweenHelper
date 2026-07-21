# TweenHelper 1.0 Roadmap

Last updated: 2026-07-21

## Objective

Ship TweenHelper 1.0 through the standard Unity Asset Store `.unitypackage` workflow as a stable, documented DOTween extension with deterministic preset registration, verified reset behavior, critical lifecycle tests, and useful Editor tooling.

The Unity Asset Store Submission Guidelines are the release authority. TweenHelper is not distributed as a Unity Package Manager package.

## Confirmed release baseline

- Publisher: `Loags`
- Product: `TweenHelper`
- Candidate version: `1.0.0-pre.1`
- Distribution root: `Assets/Loags/TweenHelper`
- Minimum Unity version: `2022.3.0f1`
- Minimum DOTween version: `1.3.030`
- License: MIT for TweenHelper-owned content
- External dependency: DOTween, installed and licensed separately
- Runtime namespace: `LB.TweenHelper`
- Editor namespace: `LB.TweenHelper.Editor`
- Demo namespace: `LB.TweenHelper.Demo`
- Test namespaces: `LB.TweenHelper.Tests.Editor` and `LB.TweenHelper.Tests.PlayMode`

## Distribution structure

```text
Assets/Loags/TweenHelper/
|-- README.md
|-- CHANGELOG.md
|-- LICENSE.md
|-- Third-Party Notices.txt
|-- Runtime/
|   |-- LB.TweenHelper.Runtime.asmdef
|   |-- Core/
|   `-- Presets/
|-- Editor/
|   |-- LB.TweenHelper.Editor.asmdef
|   |-- PresetBrowserWindow.cs
|   |-- DOTweenSetupValidator.cs
|   `-- validation and documentation tools
|-- Samples/
|   `-- TweenHelper Demos/
|       |-- Scripts/
|       |-- Scenes/
|       |-- Prefabs/
|       `-- Materials/
`-- Documentation/
    |-- Installation.md
    |-- API.md
    |-- Lifecycle.md
    `-- PresetCatalog.md
```

Only this root is selected in Asset Store Publishing Tools. DOTween remains under `Assets/Plugins/Demigiant`, and Asset Store Publishing Tools remains under `Packages`; neither is included in the upload.

Repository-only tests, reset-audit tooling, and submission notes live under `Assets/_Project/TweenHelperDevelopment`. They compile against the distributable assemblies but are excluded from the `.unitypackage`.

## M1 - Stabilize animations

Status: functionally complete before the DOTween `1.3.030` upgrade; one consolidated upgrade validation remains.

Implemented:

- Preserved per-step builder options and duration precedence.
- Made builder, handle, and async callbacks additive.
- Defined cancellation, external kill, timeout, and normal completion behavior.
- Replaced callback-created infinite loops with killable DOTween sequences.
- Fixed fade-target compatibility and Unity object null handling.
- Added repository-only reset auditing that targets both shipped demo scenes without being included in the export.
- Fixed 2D discovery, reset state capture, report naming, and expected counts.

Existing evidence from the previous DOTween baseline:

- 2D: 11 presets x 4 modes = 44 passing checks.
- 3D: 300 presets x 4 modes = 1,200 passing checks.
- Combined reset matrix: 1,244/1,244 passing checks.

Exit gate:

- [ ] Repeat the complete 1,244-check matrix once with DOTween `1.3.030`.
- [ ] Confirm no package-originated Console errors or warnings.

## M2 - Asset Store package layout

Status: implemented; Unity Editor import validation remains.

Implemented:

- Moved all TweenHelper-owned content under `Assets/Loags/TweenHelper`.
- Removed `package.json`, `Samples~`, `Documentation~`, and Package Manager sample-import tooling.
- Preserved existing script, assembly, scene, prefab, and material GUIDs.
- Kept Runtime, Editor, Samples, and customer documentation separated inside the distribution root.
- Moved automated tests, reset-audit tooling, and submission notes to `Assets/_Project/TweenHelperDevelopment`.
- Kept DOTween, Asset Store Publishing Tools, project settings, generated files, and unrelated project content outside the distribution root.
- Removed the demo assembly's Input System package reference.
- Guarded optional legacy-input shortcuts so demos do not throw when the legacy Input Manager is disabled.
- Updated catalog generation and catalog tests to use the Asset Store folder path.

Exit gate:

- [ ] Confirm Unity imports the new root without missing scripts or references.
- [x] Confirm all assets and folders have one `.meta` file and no duplicate GUIDs.
- [ ] Export the exact `.unitypackage` from only `Assets/Loags/TweenHelper`.
- [ ] Inspect the exported content list for files outside the root.

## M3 - Critical tests

Status: implemented on the previous DOTween baseline; upgraded-DOTween run remains.

The suites live under `Assets/_Project/TweenHelperDevelopment/Tests` so they compile against the distributable assemblies without being included in the Asset Store upload.

EditMode coverage:

- Exactly 300 attributed presets with unique, non-empty names.
- Construction of every compatible preset.
- Builder sequencing, joining, duration precedence, pending options, and option capture.
- Additive builder and handle callbacks.
- Await completion, cancellation, external kill, timeout, and cleanup.
- Documentation examples and deterministic catalog drift.

PlayMode coverage:

- Cancellation kills once.
- Finite Yoyo completion and cleanup.
- Linked target destruction.
- One-shot completion and public rewind restoration.
- Infinite-loop termination across representative loop families.
- Multiple awaiters and timeout cleanup.

Exit gate:

- [ ] Run EditMode once against DOTween `1.3.030`.
- [ ] Run PlayMode once against DOTween `1.3.030`.
- [ ] Rerun only a failing suite after fixing a concrete failure.

## M4 - Documentation and licensing

Status: implemented; final proofread remains.

Implemented:

- Offline README, installation, API, lifecycle, and preset documentation.
- MIT license for TweenHelper-owned content.
- `Third-Party Notices.txt` identifying DOTween, its publisher, separate acquisition, and separate license.
- Required DOTween version and setup instructions.
- Asset Store submission checklist and copy-ready listing dependency notice.
- Deterministic 300-preset catalog generation.

Exit gate:

- [ ] Regenerate the preset catalog and confirm no drift.
- [ ] Proofread every Asset Store-facing requirement, dependency, limitation, and link.
- [ ] Change the candidate version to `1.0.0` only after validation passes.

## M5 - Usability

Status: implemented.

Implemented:

- Preset browser under **Tools > TweenHelper > Preset Browser**.
- Name and description search, family filters, compatibility feedback, and copyable fluent API examples.
- Selected-object preview with transform and visual-state restoration.
- Preview cleanup on window close, selection change, compilation, and Play Mode changes.
- DOTween assembly, version, UI module, and settings validation.
- Optional TweenHelper settings asset with safe in-memory defaults.

Exit gate:

- [ ] Open the browser after the asset-layout move and verify preview/restore once.
- [ ] Run the DOTween setup validator against `1.3.030`.

## M6 - Asset Store compliance

Status: official Validator passed with one reviewed static-variable warning.

Applicable Submission Guideline checks:

- [x] One product root under `Assets/Loags/TweenHelper`.
- [x] Content organized by purpose and relationship.
- [x] No executables, archives, vendor binaries, generated libraries, or project settings inside the root.
- [x] Offline documentation included.
- [x] All custom Editor menu commands are under **Tools > TweenHelper**.
- [x] All public types use publisher-owned namespaces.
- [x] MIT license and DOTween third-party notice included.
- [x] DOTween requirement and limitation disclosed.
- [x] Paths and package size are comfortably below limits.
- [x] Run Asset Store Publishing Tools Validator against only the distribution root (35 passed checks).
- [x] Review the single static-variable warning: runtime mutable state is reset during subsystem registration, immutable lookup data is safe, and Editor-only test-runner references are released after each run.
- [ ] Configure DOTween as an external dependency in the Publisher Portal.
- [ ] Add the DOTween notice to the product description.

## M7 - Exact-artifact validation and release

Status: pending.

Validation policy:

- Make all related changes before validation.
- Run one compile check, one EditMode run, one PlayMode run, and one complete reset matrix.
- Rerun only the failing slice after a specific fix.

Release sequence:

1. Restore Unity MCP or Unity batch licensing access.
2. Rerun Asset Store Tools Validator against `Assets/Loags/TweenHelper` after the final code change and record the result.
3. Run the consolidated DOTween `1.3.030` validation cycle.
4. Export the exact `.unitypackage` using Asset Store Publishing Tools.
5. Import that artifact into clean Unity `6000.0.67f1` and `6000.5.2f1` projects.
6. Validate Unity `2022.3.0f1` when that Editor version is available.
7. Confirm the clean import creates no files outside `Assets/Loags/TweenHelper`.
8. Open both demo scenes, the preset browser, and the setup validator.
9. Regenerate the catalog and confirm no diff.
10. Set version `1.0.0`, update the changelog date, and tag only the tested artifact.

## Current environment blockers

- Unity MCP is not currently exposed to this task.
- Authorized Unity batch attempts are blocked before import by the local Unity Licensing Client IPC connection.
- Unity `2022.3.0f1` is not currently installed; only `6000.0.67f1` and `6000.5.2f1` are available locally.

Until exact-artifact validation passes, `1.0.0-pre.1` remains the honest release state.
