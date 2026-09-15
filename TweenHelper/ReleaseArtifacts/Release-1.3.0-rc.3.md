# Tween Helper 1.3.0-rc.3 — update draft

- Fixed continuous Inspector rebuilds that closed TweenPlayer dropdowns.
- Removed TweenPlayer from Main Camera in the development review scene and saved the scene before export.
- Export-time check confirmed no loaded scene camera carried TweenPlayer. Archive inspection confirmed no TweenPlayer reference in packaged scenes and verified the dropdown fix is present.
- Unity compilation passed; existing focused Inspector selection, category filtering, Undo/Redo and lifecycle checks passed during the fix. Idle Inspector check: zero rebuilds over 100 updates.
- Asset Store Tools validation: 35 passes, zero errors, one static-variable advisory. Full regression suite and player builds were not rerun for rc.3; previous results remain historical.

## Artifact

- File: TweenHelper-1.3.0-rc.3.unitypackage
- Size: 810063 bytes
- SHA-256: 03ABBC62A15B5572CC0DA5B85F92DBDD4DD581F0C2E2066C6524CD2B941DF967
- Archive entries: 206, all under Assets/Loags/TweenHelper.

## Publisher

- New draft ID: 1482254 (package 399068).
- Portal: https://publisher.unity.com/packages/1482254/edit/upload
- Created a new draft because the previous version 1482192 was Pending Processing.
- Version rc.3, release notes and compatibility notes saved through Chrome.
- Official Asset Store Uploader confirmed successful upload of the checksum above.
- No Submit action was invoked.
