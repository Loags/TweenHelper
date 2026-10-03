# Tween Helper 1.3.0-rc.6 review preparation

Validated on 2026-10-03 with Unity 6000.6.0f1.

## Candidate

- Archive: `TweenHelper-1.3.0-rc.6.unitypackage`.
- SHA-256: `2bc4bb2d213f1ed5b703a5b731603e3768bc60fdcb2447aa2d88f28932c4587c`.
- Size: 775291 bytes; 205 asset paths, 179 asset files.
- Every archived asset and metadata file matches its source. Only `Assets/Loags/TweenHelper` is exported. DOTween, development tools, tests and MCP integration are excluded.
- Exported through Asset Store Tools with `packagemanagermanifest/asset` declaring `com.unity.ugui` version `2.6.0`. DOTween remains a separate Asset Store dependency.

## Verification

- Existing EditMode suite: 42/42 passed. PlayMode suite: 19/19 passed. The CLI connection returned an authentication error during PlayMode transitions; the completed test-status report confirms all 19 results.
- Imported this exact archive into the separate URP validation project with the documented dependencies installed. Unity displayed its local unsigned-package confirmation; import completed after confirming the known local archive.
- The setup window automatically opened for rc.6. DOTween runtime 1.3.030, Editor library/module loader, Unity UI/TMP code and TMP Essential Resources were all ready. Sample launch buttons were enabled.
- All 424 gallery entries passed midpoint/completion/reset checks using the imported rc.6 package. Final runtime Console ground truth: zero errors, zero warnings; compilation succeeded.
- An earlier queued import ran after Play Mode started and was rejected by Unity. That failed tooling attempt was observed separately, then the stopped-Editor import was completed and runtime checks repeated successfully.
- Asset Store Tools 12.0.6: 36 passes, zero errors, one static-variable advisory. This is a static-analysis advisory, not an import/compiler/runtime Console warning. Earlier rc.5 repeated-session checks cover Domain Reload disabled cleanup.
- The previous setup-window validation covered initial import without DOTween and missing TMP resources: zero Console errors/warnings, with unavailable sample-launch buttons disabled. rc.6 changes the package version and export metadata; runtime/setup behavior is otherwise unchanged.

## Publisher draft and scope

Package ID 399068, draft version ID 1503172. Keep the $15 price, DOTween dependency, HTTPS website/support links and existing AI disclosure. `PublisherChangelog-rc6.txt` is the single consolidated latest Portal changelog, combining the review fixes and guided setup update. Preserve historical changelog entries in the shipped documentation.

Commit and push source and release records before uploading the validated archive. Upload does not include submission for review or acceptance of new legal terms.

Earlier Unity versions, HDRP, custom render pipelines and fresh player builds remain unvalidated. TMP Essential Resources and normal DOTween setup are required before opening the gallery.
