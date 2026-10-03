# Tween Helper 1.3.0-rc.5 review preparation

Validated on 2026-10-03 with Unity 6000.6.0f1 (the installed Editor).

## Candidate

- Archive: `TweenHelper-1.3.0-rc.5.unitypackage`.
- SHA-256: `03a3957173d4bda29237f62a76220b51e872af50cbdc31b214d6d4842a8d33c5`.
- Size: 804130 bytes; 202 asset paths and 202 unique preserved GUIDs.
- Archive contents match the source, including metadata. Only `Assets/Loags/TweenHelper` is exported. DOTween, development tools, tests, MCP integration and the development Setup window are excluded.

## Verified behavior

- The two rejected UI/Sprite extension calls were replaced with core DOTween APIs. Runtime and demo assemblies have no DOTween.Modules assembly reference.
- Import without DOTween produced zero compilation errors and zero Console warnings. The independent installation checker remains available while dependent assemblies are disabled.
- A new Unity URP project imported the archive. Required dependencies were installed separately: DOTween Free runtime 1.3.030, its Editor libraries and module loader, uGUI 2.6.0 and TMP Essential Resources. Old dependency importer metadata was upgraded through Unity's importer APIs without changing library/font contents.
- DOTweenModuleUI.cs and DOTweenModuleSprite.cs were absent in the runtime fixture. The rest of DOTween's normal module loader installation was retained; deleting its entire Modules folder is not a supported DOTween setup.
- Final import and runtime Console ground truth: zero errors, zero warnings; compilation succeeded. Historical failed experiments and MCP transport failures were recorded separately and are not counted as a successful run.
- All 424 gallery entries (423 built-in entries plus the included custom preset) completed midpoint/completion/reset checks with no failures.
- Input System pointer down/up on the authored Next button changed the selected animation from Attention to AttentionHard. The active module was InputSystemUIInputModule; the legacy module was disabled. Project input settings were preserved.
- Preset Browser and recipe asset navigation opened successfully after import. Scene/prefab component checks passed the official validator.
- Final Edit Mode suite: 42/42 passed. Final Play Mode suite: 19/19 passed. A contaminated Play Mode attempt failed on the official uploader's unrelated package-list error; the isolated repeat passed.
- Two sessions with Domain Reload disabled passed: pending observations canceled, motion preference reset and all 301 built-in/custom registered names were preserved. Original Editor options were restored.
- Asset Store Tools 12.0.6: 36 passes, no errors, one static-variable advisory. This advisory lists mutable statics and does not analyze their reset behavior; the repeated-session validation checks that behavior. It is not a runtime or import Console warning.

## Publisher draft

Draft version ID: 1503172; package ID: 399068. Base price remains $15; the existing launch discount is preserved. DOTween remains declared as an external Asset Store dependency. Release notes, description, technical details and compatibility notes now describe Unity 6.6 and the actual validation scope. The existing AI-assisted development disclosure remains enabled.

Publisher website: https://www.loags.de/. Support page: https://www.loags.de/contact/. Both HTTPS pages were inspected; the portfolio shows Unity work and the contact page offers contact details and a form. The review team's assessment of professional presentation remains theirs.

Commit and push the required source, Unity upgrade, metadata and validation changes before uploading this candidate to the draft. Final review submission is a separate action; no submission or acceptance of new legal terms is included in these checks.

## Limits and setup

This candidate targets Unity 6.6. Fresh player builds were not run; older Unity versions, HDRP, custom pipelines and other platforms are unvalidated. Earlier player-build results are historical and are explicitly described as such in the listing.

Use the shipped Installation guide for separate dependency setup, TMP resource import and importer metadata re-saving. Optional UI/Sprite extensions are unnecessary, but DOTween's own loader must remain intact. Imports do not delete obsolete files: updates from versions with `Editor/Setup` must remove that obsolete folder.

Relevant official references: [submission guidelines](https://assetstore.unity.com/publishing/submission-guidelines), [DOTween license](https://dotween.demigiant.com/license.php), [DOTween download/setup](https://dotween.demigiant.com/download.php).
