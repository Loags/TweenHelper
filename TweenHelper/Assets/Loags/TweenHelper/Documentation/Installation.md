# Installation and compatibility

## Validated configuration

Tween Helper 1.3.0-rc.1 was checked with:

- Unity `6000.5.2f1`.
- DOTween Free package `1.2.825`, which reports runtime `1.3.030`.
- Unity UI (uGUI) and TextMesh Pro.
- Built-in Render Pipeline clean import, reference checks, and Windows x64 Mono and IL2CPP player smoke checks; IL2CPP used High managed stripping.
- Universal Render Pipeline 17.5.0 in the development project: live Gallery rendering, recipe/lifecycle validation and Edit Mode/Play Mode tests.

The exported candidate was also imported over the historical 1.2.0 package in a disposable customer project. A customer-owned scene retained its TweenPlayer script and existing recipe reference. Read the migration guide before enabling global DOTween configuration: the new default preserves the host engine.

Player smoke checks cover all 300 registered names, string-only preset playback, retained completion, worker-thread cancellation and repeated cleanup. They are not full rendered-game or platform certification. The tested Windows players used Built-in; a standalone URP player and a second Unity version have not been validated for this candidate.

Other Unity versions, older DOTween versions, macOS/Linux players, mobile, WebGL, HDRP and custom render pipelines have not been tested for this candidate. These statements describe the validated configuration; they are not claims that other configurations cannot work.

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

Open **Tools > Tween Helper > Preset Browser** to confirm the Editor assembly and preview stage are working. The browser contains 461 isolated entries and does not require an active-scene target. Progress previews should show a visible filled bar and percentage, UI sequences should show only the participants required by the selected operation, layout transitions should show numbered list/grid changes, and engine-property entries should display their live meter/readout.

Open **Tools > Tween Helper > Recipe Editor**, create a small recipe, and select **Validate** to confirm the recipe Editor and runtime assembly are available. Add a `TweenPlayer`, assign the recipe, choose **Sync Bindings**, and assign its explicit targets before previewing or entering Play Mode. See [Tween Recipes](TweenRecipes.md) for the complete workflow.

## 4. Open the Animation Gallery

Open `Assets/Loags/TweenHelper/Samples/TweenHelper Demos/Scenes/TweenHelperAnimationGallery.unity` and enter Play Mode.

Use the mouse to select a category and animation. The selection auto-plays after resetting the fixture. Use Replay, Reset, Previous, and Next for repeatable comparison. The 423-entry catalog contains all 300 presets plus 123 curated UI, collection, destination, gameplay/macro, production-UI, text/value, and camera examples. Contextual options update both the preview and the displayed C# call.

The gallery is designed for 16:9 presentation and validated at `1920×1080`. It uses dedicated world and camera preview rigs and does not require the Input System.

Progress-bar, audio, light, particle, and renderer-property examples are available in the Preset Browser and documented in their focused guides. They are not additional registered presets.

## Support

For complete first-use examples, continue with [Quick start](QuickStart.md). When updating an existing project, read [Lifecycle and migration](LifecycleAndMigration.md), particularly the explicit opt-in for global DOTween engine configuration.

Open **Tools > Tween Helper > Setup & Support** to prepare a bug report, feature request, documentation question, or other support email. The form opens your default email client with editable content; it never sends a message automatically.

Optional environment fields are unchecked by default. The form does not collect project names, scenes, assets, logs, files, or machine identifiers.
