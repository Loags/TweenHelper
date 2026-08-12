# TweenHelper development assets

This folder contains repository-only validation assets and must not be included in the Asset Store `.unitypackage`.

- `Tests` contains the EditMode and PlayMode suites plus the development-only runtime assembly access required by those tests.
- `Validation` contains the reset-audit runner, preset integrity tools, and their development-only assembly wiring.
- `Documentation` contains the internal Asset Store submission checklist.

To run a reset audit, open either shipped demo scene, add `Validation/Prefabs/AnimationResetAuditRunner.prefab`, enter Play Mode, and use the runner component's context menu for each audit mode. Reports are written beneath the project `Temp` folder.

For visual review, open `Validation/Scenes/TweenHelperPresetReview.unity` and enter Play Mode. The review now contains 362 entries: 300 presets, 13 UI recipes, 13 collection entries, 10 destination-motion entries, 10 gameplay-feedback entries covering every world/UI pair, nine production UI sequences, and seven text/value animations. Existing status keys are unchanged, so all 355 completed results remain preserved while only the new `TextValue:*` entries appear under **NOT REVIEWED**. Marking an entry still advances and automatically plays the next item.
