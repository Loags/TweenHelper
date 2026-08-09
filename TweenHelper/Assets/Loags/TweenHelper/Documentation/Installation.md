# Installation and setup

## 1. Install DOTween

Install DOTween `1.2.025` or newer. TweenHelper does not vendor, redistribute, or automatically download DOTween.

The package minimum is Unity `2022.3.0f1`. TweenHelper was developed and validated with DOTween `1.2.025` as its minimum supported version.

Open **Tools > Demigiant > DOTween Utility Panel** and run **Setup DOTween**. Ensure the UI module is enabled when using TweenHelper with uGUI or TextMesh Pro.

TweenHelper also uses Unity UI (uGUI) and TextMesh Pro. Import TextMesh Pro Essential Resources before opening the included demos.

## 2. Install TweenHelper

Install the Asset Store release from **Window > Package Management > My Assets**, or import a downloaded artifact with **Assets > Import Package > Custom Package**.

The import creates the `Assets/Loags/TweenHelper` root. TweenHelper is distributed as a standard Asset Store `.unitypackage`, not as a UPM package.

## 3. Validate

Tween Helper opens **Tools > Tween Helper > Setup & Support** once for each imported version. Use its installation cards to check DOTween, URP, Unity UI, and TextMesh Pro, and follow the linked setup steps when an item needs attention. The window reports status only and never installs or changes packages automatically.

You can also run **Tools > Tween Helper > Validate > DOTween Setup**. A valid installation reports the loaded DOTween version and confirms its module assembly.

No `TweenHelperSettings` asset is required. Choose **Tools > Tween Helper > Settings > Create Settings Asset** only to override the built-in defaults.

## Support

Reopen **Tools > Tween Helper > Setup & Support** whenever you need assistance. The support form suggests an editable message template based on the selected tags and prepares the tagged email to `Info@Loags.de` in your default email client. You review and send the message yourself.

Environment details are optional and unchecked by default. Only the selected Tween Helper version, Unity version, operating system, and active render pipeline can be included. The form does not collect project names, scenes, assets, logs, files, or machine identifiers.

## 4. Open the samples

The 2D and 3D demos are included under `Assets/Loags/TweenHelper/Samples/TweenHelper Demos/Scenes`. Runtime does not require the Input System or a scriptable render pipeline. The 3D demo materials are authored for URP; the 2D demo and TweenHelper runtime are render-pipeline independent.

## Troubleshooting

### `DG.Tweening` cannot be resolved

DOTween is missing or was installed after TweenHelper without a script refresh. Install DOTween, run its setup panel, then reimport TweenHelper scripts.

### `DOTween.Modules` cannot be resolved

Run **Setup DOTween** from the DOTween Utility Panel. TweenHelper uses the generated modules assembly for UI and Unity component shortcuts.

### Preset browser preview is disabled

Select a non-persistent GameObject in an open scene. The selected preset must also report that it is compatible with that GameObject.
