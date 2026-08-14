# TweenHelper development assets

This folder contains repository-only validation assets and must not be included in the Asset Store `.unitypackage`.

- `Tests` contains the EditMode and PlayMode suites plus the development-only runtime assembly access required by those tests.
- `Validation` contains the reset-audit runner, preset integrity tools, and their development-only assembly wiring.
- `Documentation` contains internal release, pipeline, validation, and implementation roadmaps.
- `Documentation/AnimationReviewCoverageAndExpansionRoadmap.md` tracks the planned 76-entry review-completeness pass and the staged animation-family expansion that follows it.

To run a reset audit, open either shipped demo scene, add `Validation/Prefabs/AnimationResetAuditRunner.prefab`, enter Play Mode, and use the runner component's context menu for each audit mode. Reports are written beneath the project `Temp` folder.

For visual review, open `Validation/Scenes/TweenHelperPresetReview.unity` and enter Play Mode. The review now contains 474 entries: 300 presets, 13 UI recipes, 36 collection entries, 26 destination-motion entries, 22 gameplay-feedback entries, 39 production UI sequences, 31 text/value animations, and seven camera-feedback entries. Existing status keys are unchanged, so all 398 previously validated results remain preserved while only the 76 coverage additions appear under **NOT REVIEWED**. Marking an entry still advances and automatically plays the next item.

The coverage additions exercise every visible direction and traversal enum, linear path interpolation, signed destination variants, alternate UI/world branches, dynamic and custom stagger execution, incomplete-grid topology, automatic spatial defaults, the drawer backdrop branch, and representative world-space TextMesh Pro behavior. Run **Tools > Tween Helper Dev > Validate Animation Review Coverage** for the non-destructive catalog, fixture, and playback smoke validation; it does not change manual review statuses.

Run **Tools > Tween Helper Dev > Validate Animation Lifecycle Refactor** for the lifecycle-kernel regression covering lazy capture, interrupted and forced completion, nested cleanup, partial-state preservation, finite Yoyo behavior, and spatial direct-play behavior. After a lifecycle implementation changes, **Mark Lifecycle Refactor Reviews As Needs Work** resets only the 106 affected camera, gameplay-feedback, text/value, production-UI, and spatial-recipe review IDs; every preset, UI recipe, destination, and preset-stagger status remains untouched.
