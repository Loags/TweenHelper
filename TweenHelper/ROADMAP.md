# Tween Helper roadmap

Status: **M0–M9 implementation delivered for 1.3.0-rc.1; local release checks passed.**

The [implementation roadmap](Assets/_Project/TweenHelperDevelopment/Documentation/PublishingImplementationRoadmap.md) now records completed source work and verified gates. See [implementation evidence](Assets/_Project/TweenHelperDevelopment/Documentation/PublishingImplementationEvidence.md) and the [exact candidate release record](ReleaseArtifacts/Release-1.3.0-rc.1.md).

| Milestone | Delivered |
| --- | --- |
| M0–M2 | Documented contracts, async/Editor lifecycle correctness and preservation of host DOTween configuration |
| M3 | Explicit built-in registration; Mono and IL2CPP High player checks, including a preserved customer-only preset |
| M4 | Complete quick start, lifecycle/migration guides, event adapter and clean/update installation evidence |
| M5 | Reproducible CPU/cancellation/cleanup measurements with explicit allocation and rendering limitations |
| M6 | Audited archive, 53 passing tests, existing validators, current stills, captioned reel and release copy |
| M7 | Use-case discovery, favorites and 20-entry local Browser history |
| M8 | Package/per-call/player motion preferences, preserved outcomes and authored Toggle |
| M9 | ProgressFillTo, 28 operations, seven recipe samples and an authored progress/reward workflow |

Remaining external checks: actual Publisher Portal draft/publication/dependency settings, native Browser/Setup/Inspector screenshots on a functioning capture surface, optional human usability review, and final upload/submission approval. Other Unity versions/platforms and standalone URP rendering are not advertised as validated. No Unity Editor batch/CI build or external publishing action was performed.

## Delivered for 1.2.0

| Area | State |
| --- | --- |
| Generalized TextMesh Pro animation | Implemented, integrated, documented, and validated |
| Deal In/Out and grid serpentine collections | Implemented, integrated, documented, and validated |
| Layout-difference collection transitions | Implemented, integrated, documented, and validated |
| Tween Recipe assets, TweenPlayer, and visual editor | Implemented, integrated, documented, and validated |
| Gallery, Browser, and review integration | 423 Gallery entries, 461 Browser entries, 610 review configurations |

## Deferred work

TMP formation transitions remain unapproved and are not part of 1.2.0. Persistent curved/circular text layout, material/shader effects, editor authoring systems beyond Tween Recipes, and parameter-only duplicate engines remain out of scope. Any future expansion requires a new explicit roadmap and release decision.

## Documentation state

Completed implementation roadmaps, historical release candidates, deprecated handoffs, and abandoned pipeline plans were removed on 2026-08-30 after their current facts were incorporated into the shipped guides, changelog, and `Assets/_Project/TweenHelperDevelopment/Documentation/Release-1.2.0.md`.

Git history remains the source for removed planning decisions.
