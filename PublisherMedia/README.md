# Tween Helper publishing media

Candidate: **1.3.0-rc.1**. [Exact artifact and release copy](../TweenHelper/ReleaseArtifacts/Release-1.3.0-rc.1.md). Remote publication/draft state remains unverified; historical Portal IDs are not a verified current destination.

## Current delivery

- Images/: ten actual Gallery screenshots at 1920×1080, covering all eight categories plus UI hover and a collection option menu.
- [Marketing reel](MarketingReel/TweenHelper-1.3.0-rc.1-Reel.mp4): 32.24 seconds, 1920×1080, H.264, captioned, silent, with eight actual Unity Recorder segments.
- [Captions](MarketingReel/TweenHelper-Reel.srt): editable timing and copy.
- [First-animation tutorial](Tutorial/FirstAnimation.md): dependency setup, first animation, Browser discovery, recipe wiring and the motion-toggle sample.

Recorder 5.1.7 captured the current Gallery in Unity 6000.5.2f1 / URP 17.5.0. The final candidate uses the same runtime/Gallery assets; subsequent documentation and line-ending corrections do not change the footage. Stills and the final reel's caption placement were visually inspected. Counts: 300 presets, 461 Browser entries, 423 Gallery entries, 28 recipe operations, seven samples. Do not add these distinct totals together.

Capture menus: **Tools > Tween Helper Dev > Validation > Capture Animation Gallery Stills** and **Capture Animation Gallery Reel**. Raw output goes to Temp/RoadmapValidation/Recorder; copy accepted output here before restarting Unity. BuildReel.py assembles eight clips using ffmpeg with libass:

```text
python PublisherMedia/BuildReel.py <ffmpeg.exe> <Recorder/Reel> PublisherMedia/MarketingReel
```

This directory stays outside Unity Assets and is excluded from the customer archive.

## Remaining Portal preparation

Native Browser, Setup and Inspector screenshots require a working Windows capture surface. Computer use fails with `SetIsBorderRequired ... 0x80004002`; publisher-tab control also failed its CDP focus operation. Gallery stills/reel succeeded through Recorder. A recorded Editor walkthrough is not claimed.

Before upload, verify the actual draft, category, dimensions/crops, codecs/hosting limits and dependency configuration. Review listing copy, support address and AI disclosure. No media was uploaded or replaced remotely.

Historical icon/card/cover artwork remains under TweenHelper/PublisherMedia and is not automatically approved for reuse. Old 2D/3D showcase captures and TweenHelper-2D-Showcase-1280x720.mp4 predate the current Gallery. Review provenance and visible claims before reusing old artwork.