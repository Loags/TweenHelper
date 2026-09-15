# Tween Helper 1.3.0-rc.2 — update draft

## Delivered

- Direct Tween Player preset selection with category/search filters, same-object or explicit targets, duration and declared preset-specific overrides.
- Optional recipe mode with automatic binding synchronization and preserved serialized compatibility.
- Custom preset categories, override metadata, preview opt-in, and registry discovery that preserves explicit registrations.
- Direct/custom sample prefabs and updated player, quick-start, API, installation, recipe and migration documentation.

## Validation

- Unity 6000.5.2f1 compilation passed.
- Existing regression suite: 34 Edit Mode and 19 Play Mode tests passed.
- Focused checks passed for Inspector dropdown events and Undo/Redo, category filtering, custom discovery, retained manual registrations, missing preset selection, duration/scale overrides, external targets, playback controls, the owner-disable cleanup handler, preview restoration, and recipe binding preservation.
- Asset Store Tools CurrentProjectValidator, UnityPackage mode, Tools/Animation category, package root Assets/Loags/TweenHelper: 35 passes, zero errors, one static-variable warning. The line-ending warning was corrected. Registry state resets and lifecycle paths are covered by the existing regression suite; the static-field advisory is retained in the report.
- Archive inventory: 206 entries, all under Assets/Loags/TweenHelper. Required new scripts, documentation and sample prefabs are present. No development tools or separately licensed DOTween code are included.
- No new player build or clean-import validation was run. Earlier rc.1 results are labeled as historical in the documentation and publisher listing.

## Artifact

- File: TweenHelper-1.3.0-rc.2.unitypackage
- Size: 809551 bytes
- SHA-256: D0CDBC678EE0E0852F695D7AADF41C4CE8890198F7E0DB239CE1E31D89C477F0

## Publisher draft

- Package: Tween Helper (399068)
- Draft version ID: 1482192
- Portal: https://publisher.unity.com/packages/1482192/edit/upload
- Asset Store Uploader returned Success for this exact candidate checksum.
- Version, release notes, summary, description, technical details and compatibility notes were saved through the Chrome extension.
- The Submit action was not invoked.

Reports: AssetStoreValidation-1.3.0-rc.2.json, EditMode-1.3.0-rc.2.json, PlayMode-1.3.0-rc.2.json, PlayerWorkflow-1.3.0-rc.2.json, PackageManifest-1.3.0-rc.2.json and Upload-1.3.0-rc.2.json.

Final uploader metadata confirmed status draft, root Assets/Loags/TweenHelper, and package size 809551 bytes. Chrome disconnected during the final visual refresh after the listing changes had been saved. PublisherDraft-1.3.0-rc.2.json records the server-side draft confirmation.
