# Installation and compatibility

## Validated configuration

Tween Helper `1.1.0` was developed and validated with:

- Unity `6000.5.2f1`.
- DOTween Free package `1.2.825`, which reports runtime `1.3.030`.
- Unity UI (uGUI) and TextMesh Pro.
- Built-in Render Pipeline and Universal Render Pipeline workflows.

Lower Unity versions and older DOTween versions have not been tested. HDRP and custom render pipelines are also untested. These statements describe the validated configuration; they are not claims that other configurations cannot work.

DOTween is a separate dependency and is not included with Tween Helper.

## 1. Install and configure DOTween

1. Install DOTween Free separately.
2. Open **Tools > Demigiant > DOTween Utility Panel**.
3. Run **Setup DOTween** and enable the UI module.
4. Import TextMesh Pro Essential Resources when Unity prompts for them, or before opening the Animation Gallery.

## 2. Install Tween Helper

Install the Asset Store release from **Window > Package Management > My Assets**, or import a downloaded artifact with **Assets > Import Package > Custom Package**.

The import creates one product root: `Assets/Loags/TweenHelper`. Tween Helper is distributed as a standard Asset Store package, not as a UPM package.

## 3. Validate the installation

Tween Helper opens **Tools > Tween Helper > Setup & Support** once for each imported version. Its status cards inspect DOTween, the active render pipeline, Unity UI, and TextMesh Pro without installing, removing, or changing packages.

Run **Tools > Tween Helper > Validate > DOTween Setup** for a focused DOTween and module check. No `TweenHelperSettings` asset is required; choose **Tools > Tween Helper > Settings > Create Settings Asset** only when you want to override the built-in defaults.

## 4. Open the Animation Gallery

Open `Assets/Loags/TweenHelper/Samples/TweenHelper Demos/Scenes/TweenHelperAnimationGallery.unity` and enter Play Mode.

Use the mouse to select a category and animation. The selection auto-plays after resetting the fixture. Use Replay, Reset, Previous, and Next for repeatable comparison; preset search and family filters cover all 300 presets. Contextual options update both the preview and the displayed C# call.

The gallery is designed for 16:9 presentation and validated at `1920×1080` and `1280×720`. It uses dedicated world and camera preview rigs and does not require the Input System.

## Support

Open **Tools > Tween Helper > Setup & Support** to prepare a bug report, feature request, documentation question, or other support email to `Info@Loags.de`. The form opens your default email client with editable content; it never sends a message automatically.

Optional environment fields are unchecked by default. The form does not collect project names, scenes, assets, logs, files, or machine identifiers.
