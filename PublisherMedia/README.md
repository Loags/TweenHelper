# Tween Helper Publisher Media Manifest

Updated: 2026-08-19

Release target: Tween Helper `1.1.0`, initial public release

This repository-level directory is the publishing manifest and final upload staging area. It is outside the Unity project and cannot enter the customer `.unitypackage` accidentally.

## Current media locations

Existing generated files currently live under `TweenHelper/PublisherMedia`, outside the Unity `Assets` folder:

- `TweenHelper-Icon-160x160.png`
- `TweenHelper-Card-420x280.png`
- `TweenHelper-Cover-1950x1300.png`
- `TweenHelper-Marketing-16x9.png`
- icon/key-art source images
- retired 2D/3D showcase screenshots and video

Development-only brand references also remain under `TweenHelper/Assets/_Project/TweenHelperDevelopment/PublisherPortal/Branding`.

Nothing under either location belongs in `Assets/Loags/TweenHelper` or the exported customer artifact.

## Reuse policy

The icon, card, cover, and marketing artwork are candidates for reuse only after their visible copy and claims are checked against the final 1.1.0 artifact. Retire:

- `TweenHelper-2D-Showcase-1280x720.mp4`
- `Screenshots/TweenHelper-2D-Showcase.png`
- `Screenshots/TweenHelper-2D-Showcase-GameView.png`
- `Screenshots/TweenHelper-3D-Showcase.png`

Those captures predate the 406-entry Animation Gallery and 446-entry Preset Browser.

## Required replacement set

Create final upload folders only after the replacement release artifact passes validation and clean import:

```text
PublisherMedia/
├── Images/
├── MarketingReel/
└── Tutorial/
```

Capture subjects:

1. Animation Gallery overview at 1920×1080.
2. World-to-UI destination or pickup collection.
3. Collection topology with its contextual option visible.
4. Production UI preview with correct backdrop/incoming composition.
5. Text/value or Image fill + percentage example.
6. Dedicated camera feedback.
7. Preset Browser search and isolated preview.
8. Progress or engine-property browser meter.
9. Setup & Support dependency status.
10. Short caption-only animation demonstration video.
11. Optional longer caption-only setup and complete feature tour.

Use Unity Recorder `5.1.7`. Capture only from the exact artifact intended for draft `1449210`.

## Upload checks

- Confirm current Portal image dimensions, crop rules, codecs, duration/file-size limits, and field limits immediately before export.
- Verify every visible version, count, API call, menu path, dependency, and compatibility claim.
- Do not replace existing Portal media until its accepted replacement is ready.
- Do not include videos inside the package; host tutorial/video documentation through Portal-supported external media.
- Keep the required DOTween dependency and AI disclosure in Portal text, not baked into small artwork.

The official [Unity Asset Store Submission Guidelines](https://assetstore.unity.com/publishing/submission-guidelines) require Animation-category submissions to include a video demonstration in their marketing material. Recheck the live rules before upload.
