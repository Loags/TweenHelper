# Tween Helper active roadmaps

Status: **pre-release development**

Updated: 2026-08-29

Tween Helper has not had a public release. Existing APIs and catalog counts describe the current development baseline, not a compatibility contract. Prefer clear APIs and small implementations over preserving obsolete development-only names or plans.

## Active roadmaps

| Roadmap | Scope | State |
| --- | --- | --- |
| [TMP Animation Expansion](Assets/Loags/TweenHelper/Documentation/TMPAnimationExpansionRoadmap.md) | Generalized TMP units, glyph motion, transforms, and spatial reactions | Planned |
| [Layout-Difference Collection Transitions](Assets/_Project/TweenHelperDevelopment/Documentation/LayoutDifferenceCollectionTransitionsRoadmap.md) | Animate UI children between authored layout states | Planned |
| [Collection Deal and Grid Serpentine](Assets/_Project/TweenHelperDevelopment/Documentation/CollectionDealAndGridSerpentineRoadmap.md) | Deal In/Out motion and reusable serpentine grid ordering | Planned |
| [TweenRecipe Node Editor and TweenPlayer](Assets/_Project/TweenHelperDevelopment/Documentation/TweenRecipeNodeEditorRoadmap.md) | Visual recipe authoring, runtime bindings, playback, and preview | Planned |

## Planning rules

- Implement one roadmap phase at a time.
- Reuse the current tween, lifecycle, stagger, preview, and catalog infrastructure where it fits.
- Do not add legacy aliases for unreleased APIs.
- Do not build generalized frameworks without a current roadmap consumer.
- Update Gallery, Preset Browser, review coverage, and customer documentation in the same phase as each public feature.
- Recalculate catalog counts after implementation instead of preserving historical totals.
- Do not publish planned features as available before runtime, preview, lifecycle, and documentation validation are complete.

## Documentation cleanup

Completed implementation trackers, superseded lifecycle handoffs, stale release-candidate records, and abandoned CLI-gated recipe plans were moved to the development documentation archive on 2026-08-29. They are excluded from active planning and remain recoverable until explicitly deleted.

The implemented CLI prototype is documented by `Assets/_Project/TweenHelperDevelopment/CLI/README.md`; it has no active feature roadmap and is not a dependency of the public runtime.

Git history remains the source for decisions removed from active documentation.