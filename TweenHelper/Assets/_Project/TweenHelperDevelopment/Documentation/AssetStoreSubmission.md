# Asset Store submission notes

TweenHelper uses the standard Asset Store `.unitypackage` workflow. All distributable content is nested beneath the single root `Assets/Loags/TweenHelper`. The publisher name is **Loags**.

The development project contains DOTween, Asset Store Publishing Tools, automated tests, and the reset-audit harness outside that root. They are validation or external dependency content and must not be selected for the TweenHelper upload.

TweenHelper also uses the standard Unity UI (uGUI) and TextMesh Pro packages. The 3D demonstration materials use URP, but TweenHelper runtime code is render-pipeline independent. Preserve the appropriate Unity package dependency information during export and disclose the URP demo requirement in the listing.

## Required external dependency

TweenHelper requires **DOTween (HOTween v2)** version `1.3.030` or newer. DOTween is not redistributed with TweenHelper. Register DOTween as a required Asset Store dependency in the Publisher Portal and place this notice prominently in the listing:

> Requires DOTween (HOTween v2) 1.3.030 or newer, available separately from Demigiant on the Unity Asset Store. DOTween is not included. See Third-Party Notices.txt in the package for license details.

## Validation and upload

1. Open **Tools > Asset Store > Validator**.
2. Choose the standard asset-package validation type and validate only `Assets/Loags/TweenHelper`.
3. Resolve every package-originated error or warning that applies to a code/tool asset.
4. Open **Tools > Asset Store > Uploader** and select only `Assets/Loags/TweenHelper` as the package content root.
5. Confirm the uploader preview contains no files from `Assets/Plugins/Demigiant`, `Packages`, `ProjectSettings`, `Library`, `Temp`, or unrelated project folders.
6. Upload from a supported Unity Editor version and record the exact Editor and DOTween versions in the release notes.

### Static-variable Validator warning

The Validator's static-variable check is intentionally conservative and reports types without proving whether their state is unsafe. TweenHelper resets all mutable runtime static state with `RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)`, including its settings cache, registry state, bootstrap state, and demo singleton references. This makes the runtime code compatible with Fast Enter Play Mode when domain reload is disabled. The demo's static family-order array is immutable.

## Release checklist

- The upload has one root folder: `Assets/Loags/TweenHelper`.
- Runtime, Editor, Samples, and customer documentation are separated beneath that root.
- Repository-only tests, reset auditing, and submission notes remain under `Assets/_Project/TweenHelperDevelopment` and are absent from the upload.
- Every asset and folder has exactly one `.meta` file and no GUID is duplicated.
- All custom Editor menu commands are under **Tools > TweenHelper**.
- All public code is contained in `LB.TweenHelper` namespaces.
- DOTween binaries, modules, settings, documentation, and license files are absent from the upload.
- Asset Store Publishing Tools is absent from the upload.
- The package contains no executables, archives, generated libraries, project settings, temporary files, or unrelated dependencies.
- Paths are below 150 characters and the artifact is below the Asset Store size limit.
- Offline installation, API, lifecycle, and preset documentation is included.
- Both demo scenes open, and all four reset-audit modes pass with DOTween `1.3.030` or newer.
- EditMode and PlayMode suites pass against the exact exported content.
- A clean project imports the exact `.unitypackage` without leaving files outside `Assets/Loags/TweenHelper`.
- The Publisher Portal listing discloses the required DOTween version and separate license.
