# Installation and compatibility

Tween Helper 1.3.0-rc.6 targets Unity 6.6 (`6000.6.0f1`), Unity UI (uGUI) with TextMesh Pro, and DOTween Free runtime `1.3.030` or newer. DOTween is installed and licensed separately, not included in this package. The archive declares Unity UI `com.unity.ugui` version `2.6.0` using Unity's package dependency manifest; TMP Essential Resources still need the official resource import described below.

## Install dependencies

1. Install [DOTween Free](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676) separately from My Assets or [Demigiant](https://dotween.demigiant.com/download.php), including its core and Editor libraries.
2. Run **Tools > Demigiant > DOTween Utility Panel > Setup DOTween**.
3. Ensure **Unity UI** is installed through Unity Package Manager. In Unity 6 this includes TextMesh Pro.
4. Import **TMP Essential Resources** before opening the sample gallery.

Tween Helper uses core DOTween APIs for UI position, color and alpha. Its runtime and demos do not reference `DOTween.Modules`. Optional UI/Sprite extensions and generated module assembly definitions are not required. Keep DOTween's own `DOTweenModuleUtils.cs` loader and the rest of its normal installation intact.

If Unity 6.6 reports old dependency importer metadata, select the affected DOTween DLLs or TMP font assets and re-save their import settings in the Inspector. This upgrades Unity's metadata without modifying the dependency libraries or font files.

## Import Tween Helper

Use **Window > Package Management > My Assets**, or **Assets > Import Package > Custom Package** for a local `.unitypackage`. All distributable files are under `Assets/Loags/TweenHelper`. This is a standard Asset Store package, not a UPM package.

An independent installation assembly checks for DOTween's libraries and Unity UI/TextMesh Pro. It enables `TWEEN_HELPER_DEPENDENCIES_READY` for the active build target when dependencies are available, preserving existing scripting symbols. Until then, dependent runtime, Editor and sample assemblies stay disabled so missing dependencies do not cause compiler errors. Install dependencies before opening samples.

The branded **Tween Helper Setup** window opens once per package version after import. Reopen it from **Tools > Tween Helper > Setup** or **Validate > Dependencies**. It checks the installed DOTween runtime version, its Editor library and module loader, Unity UI/TextMesh Pro code, and TMP Essential Resources (settings and the gallery's font). Its buttons open the official Asset Store, Package Manager, DOTween Utility Panel and TMP resource importer. Complete installation or updates in those tools; in the TMP importer, keep all essentials selected and click **Import**. The checker does not install or change third-party packages. After switching build targets, run **Check again** if the tools have not yet appeared.

The setup window enables its gallery and preset-browser buttons only after all three steps are ready. If a sample scene was already open before importing TMP Essential Resources, reopen it after import so its text components initialize with the resources available. This setup window does not open a sample scene or an external website automatically.

When upgrading an installation that still contains `Assets/Loags/TweenHelper/Editor/Setup`, remove that obsolete folder. Unity package imports do not delete files removed from later releases.

## Validate tools and samples

Open **Tools > Tween Helper > Preset Browser** to browse 461 isolated entries. Open **Tools > Tween Helper > Recipe Editor**, open a recipe, and select **Validate**. Add a `TweenPlayer` and assign its required targets before previewing or entering Play Mode.

No settings asset is required. Use **Tools > Tween Helper > Settings > Create Settings Asset** only to override defaults. Global DOTween configuration is opt-in; see [Lifecycle and migration](LifecycleAndMigration.md).

Open `Assets/Loags/TweenHelper/Samples/TweenHelper Demos/Scenes/TweenHelperAnimationGallery.unity` and enter Play Mode. Select an animation, then use Replay, Reset, Previous and Next. The mouse-driven gallery contains 423 entries, including all 300 registered presets. It uses the project's enabled input backend: legacy input or the optional Input System package.

The gallery uses a 16:9 layout with world and camera preview rigs. Built-in Render Pipeline and URP are supported. HDRP, custom pipelines, earlier Unity versions, mobile, WebGL and macOS/Linux players are unvalidated. This candidate does not claim fresh player-build validation.

## Support

Read [Quick start](QuickStart.md), [Tween Player](TweenPlayer.md) and [Tween Recipes](TweenRecipes.md) for first-use examples.

Contact [info@loags.de](mailto:info@loags.de) or use the [HTTPS contact page](https://www.loags.de/contact/). Include package and Unity versions, dependency versions, reproduction steps and relevant Console messages. The development-only Setup & Support window is not shipped.
