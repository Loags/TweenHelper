# TweenHelper development assets

This folder contains repository-only validation assets and must not be included in the Asset Store `.unitypackage`.

- `Tests` contains the EditMode and PlayMode suites plus the development-only runtime assembly access required by those tests.
- `Validation` contains the reset-audit runner, preset integrity tools, and their development-only assembly wiring.
- `Documentation` contains the internal Asset Store submission checklist.

To run a reset audit, open either shipped demo scene, add `Validation/Prefabs/AnimationResetAuditRunner.prefab`, enter Play Mode, and use the runner component's context menu for each audit mode. Reports are written beneath the project `Temp` folder.
