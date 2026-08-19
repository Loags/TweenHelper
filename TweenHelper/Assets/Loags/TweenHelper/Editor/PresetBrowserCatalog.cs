using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LB.TweenHelper.Editor
{
    internal static class PresetBrowserCatalog
    {
        public static List<PresetBrowserEntry> Build()
        {
            TweenPresetRegistry.Refresh();
            var entries = TweenPresetRegistry.Presets
                .OrderBy(preset => preset.PresetName, StringComparer.Ordinal)
                .Select(PresetBrowserEntry.FromPreset)
                .ToList();

            entries.AddRange(CreateUIRecipeEntries());
            entries.AddRange(CreateCollectionEntries());
            entries.AddRange(CreateDestinationEntries());
            entries.AddRange(CreateGameplayEntries());
            entries.AddRange(CreateUISequenceEntries());
            entries.AddRange(CreateTextEntries());
            entries.AddRange(CreateProgressEntries());
            entries.AddRange(CreateCameraEntries());
            entries.AddRange(CreateEnginePropertyEntries());

            string duplicateId = entries.GroupBy(entry => entry.Id, StringComparer.Ordinal).FirstOrDefault(group => group.Count() > 1)?.Key;
            if (!string.IsNullOrEmpty(duplicateId)) throw new InvalidOperationException($"Preset Browser entry ID '{duplicateId}' is duplicated.");
            return entries;
        }

        private static IEnumerable<PresetBrowserEntry> CreateUIRecipeEntries()
        {
            const string category = "UI Recipes";
            const string family = "UI interaction";
            const string badge = "UI";
            const PresetBrowserPreviewKind preview = PresetBrowserPreviewKind.UiTarget;
            yield return Operation(PresetBrowserOperation.UIHover, "Gently raises, scales, and highlights a hovered UI target.", category, family, "0.18s", "target.UIHover();", badge, preview);
            yield return Operation(PresetBrowserOperation.UIHoverSoft, "Applies a restrained hover lift and color response.", category, family, "0.22s", "target.UIHoverSoft();", badge, preview);
            yield return Operation(PresetBrowserOperation.UIPress, "Compresses and releases a pressed UI target.", category, family, "0.16s", "target.UIPress();", badge, preview);
            yield return Operation(PresetBrowserOperation.UIPressHard, "Applies a stronger press with a sharper rebound.", category, family, "0.2s", "target.UIPressHard();", badge, preview);
            yield return Operation(PresetBrowserOperation.UIAppear, "Reveals UI with scale, position, and alpha.", category, family, "0.32s", "target.UIAppear();", badge, preview);
            yield return Operation(PresetBrowserOperation.UIAppearSoft, "Reveals UI with a softer fade and settle.", category, family, "0.38s", "target.UIAppearSoft();", badge, preview);
            yield return Operation(PresetBrowserOperation.UIDisappear, "Hides UI with scale, position, and alpha.", category, family, "0.26s", "target.UIDisappear();", badge, preview);
            yield return Operation(PresetBrowserOperation.UIDisappearSoft, "Hides UI with a restrained fade and drift.", category, family, "0.3s", "target.UIDisappearSoft();", badge, preview);
            yield return Operation(PresetBrowserOperation.UIAttention, "Draws attention with a readable UI pulse.", category, family, "0.42s", "target.UIAttention();", badge, preview);
            yield return Operation(PresetBrowserOperation.UIAttentionSoft, "Draws attention with a subtle UI pulse.", category, family, "0.48s", "target.UIAttentionSoft();", badge, preview);
            yield return Operation(PresetBrowserOperation.UIAttentionHard, "Draws attention with a strong UI impact.", category, family, "0.38s", "target.UIAttentionHard();", badge, preview);
            yield return Operation(PresetBrowserOperation.UIDisabled, "Transitions UI into its disabled visual state.", category, family, "0.24s", "target.UIDisabled();", badge, preview);
            yield return Operation(PresetBrowserOperation.UIEnabled, "Restores UI from its cached disabled state.", category, family, "0.24s", "target.UIDisabled().OnComplete(() => target.UIEnabled());", badge, preview);
        }

        private static IEnumerable<PresetBrowserEntry> CreateCollectionEntries()
        {
            yield return Collection("ListStaggerIn", "Staggers a list into view from the first item to the last.", "0.32s per item", "items.ListStaggerIn(owner);", PresetBrowserPreviewKind.List, PresetBrowserCollectionKind.ListStaggerIn, "First to last");
            yield return Collection("ListStaggerOut", "Staggers a list out of view from the last item to the first.", "0.26s per item", "items.ListStaggerOut(owner);", PresetBrowserPreviewKind.List, PresetBrowserCollectionKind.ListStaggerOut, "Last to first");
            yield return Collection("GridWave", "Reveals grid columns in a left-to-right wave.", "0.32s per item", "items.GridWave(owner, columns: 3);", PresetBrowserPreviewKind.Grid, PresetBrowserCollectionKind.GridWave, "Left to right");
            yield return Collection("GridRipple", "Pulses outward from a centered origin.", "0.32s per item", "items.GridRipple(owner, columns: 3);", PresetBrowserPreviewKind.Grid, PresetBrowserCollectionKind.GridRipple, "Center outward");
            yield return Collection("LoadingDots", "Loops a soft pulse across loading dots.", "0.25s per item", "dots.LoadingDots(owner);", PresetBrowserPreviewKind.LoadingDots, PresetBrowserCollectionKind.LoadingDots, "First to last");
            yield return CreateOrderEntry("OrderFirstToLast", "Applies delays from the first collection item to the last.", "FirstToLast", PresetBrowserCollectionKind.OrderFirstToLast, "First to last");
            yield return CreateOrderEntry("OrderLastToFirst", "Applies delays from the last collection item to the first.", "LastToFirst", PresetBrowserCollectionKind.OrderLastToFirst, "Last to first");
            yield return CreateOrderEntry("OrderFromCenter", "Starts at the center item and moves toward both edges.", "FromCenter", PresetBrowserCollectionKind.OrderFromCenter, "Center outward");
            yield return CreateOrderEntry("OrderToCenter", "Starts at both edges and moves toward the center.", "ToCenter", PresetBrowserCollectionKind.OrderToCenter, "Edges inward");
            yield return PresetBrowserEntry.Collection("OrderRandom", "Uses a deterministic shuffled order with preview seed 1729.", "0.36s per item", "items.TweenStagger(owner).Preset<PulseScalePreset>(0.36f).Order(StaggerOrder.Random).DelayBetween(0.14f).Seed(1729).Play();", PresetBrowserEntryKind.StaggerVariant, PresetBrowserPreviewKind.List, PresetBrowserCollectionKind.OrderRandom, "Seeded random");
            yield return CreateWaveEntry("GridWaveRightToLeft", "Reveals grid columns from right to left.", "RightToLeft", PresetBrowserCollectionKind.GridWaveRightToLeft, "Right to left");
            yield return CreateWaveEntry("GridWaveTopToBottom", "Reveals grid rows from top to bottom.", "TopToBottom", PresetBrowserCollectionKind.GridWaveTopToBottom, "Top to bottom");
            yield return CreateWaveEntry("GridWaveBottomToTop", "Reveals grid rows from bottom to top.", "BottomToTop", PresetBrowserCollectionKind.GridWaveBottomToTop, "Bottom to top");
            yield return Collection("GridDiagonalWave", "Reveals diagonals between selected opposite corners.", "0.32s per item", "items.GridDiagonalWave(owner, columns: 3);", PresetBrowserPreviewKind.Grid, PresetBrowserCollectionKind.GridDiagonalWave, "Top-left diagonal");
            yield return Collection("GridSpiral", "Reveals a configurable outside-in or inside-out spiral.", "0.3s per item", "items.GridSpiral(owner, columns: 3);", PresetBrowserPreviewKind.Grid, PresetBrowserCollectionKind.GridSpiral, "Outside in");
            yield return Collection("GridCheckerboard", "Pulses alternating checkerboard cells in two phases.", "0.34s per item", "items.GridCheckerboard(owner, columns: 3);", PresetBrowserPreviewKind.Grid, PresetBrowserCollectionKind.GridCheckerboard, "Alternating");
            yield return Collection("CollectionBurstIn", "Launches every item from one origin into its authored position.", "0.48s per item", "items.CollectionBurstIn(owner, origin);", PresetBrowserPreviewKind.Grid, PresetBrowserCollectionKind.CollectionBurstIn, "Center outward");
            yield return Collection("CollectionBurstOut", "Scatters items radially while shrinking and fading.", "0.42s per item", "items.CollectionBurstOut(owner, origin);", PresetBrowserPreviewKind.Grid, PresetBrowserCollectionKind.CollectionBurstOut, "Center outward");
            yield return Collection("CollectionGatherTo", "Gathers every item into one destination while shrinking and fading.", "0.52s per item", "items.CollectionGatherTo(owner, destination);", PresetBrowserPreviewKind.Grid, PresetBrowserCollectionKind.CollectionGatherTo, "Edges inward");
            yield return Collection("GridConcentricIn", "Reveals concentric grid rings from the outside toward the center.", "0.32s per item", "items.GridConcentricIn(owner, columns: 3);", PresetBrowserPreviewKind.Grid, PresetBrowserCollectionKind.GridConcentricIn, "Outside in");
            yield return Collection("GridConcentricOut", "Hides concentric grid rings from the center toward the outside.", "0.28s per item", "items.GridConcentricOut(owner, columns: 3);", PresetBrowserPreviewKind.Grid, PresetBrowserCollectionKind.GridConcentricOut, "Inside out");
            yield return Collection("GridQuadrantSweep", "Sweeps grid quadrants clockwise from the top-left.", "0.32s per item", "items.GridQuadrantSweep(owner, columns: 3);", PresetBrowserPreviewKind.Grid, PresetBrowserCollectionKind.GridQuadrantSweep, "Clockwise");
            yield return Collection("ListAccordion", "Unfolds list items with an alternating accordion motion.", "0.44s per item", "items.ListAccordion(owner);", PresetBrowserPreviewKind.List, PresetBrowserCollectionKind.ListAccordion, "Alternating");
            yield return Collection("CollectionOrbitIn", "Orbits items inward from a ring to their authored positions.", "0.62s per item", "items.CollectionOrbitIn(owner, center);", PresetBrowserPreviewKind.Grid, PresetBrowserCollectionKind.CollectionOrbitIn, "Outside in");
            yield return Collection("CollectionOrbitOut", "Orbits items outward from their positions into a ring.", "0.56s per item", "items.CollectionOrbitOut(owner, center);", PresetBrowserPreviewKind.Grid, PresetBrowserCollectionKind.CollectionOrbitOut, "Inside out");
            yield return Collection("LoadingRing", "Loops a radial loading pulse around a ring.", "0.9s cycle", "items.LoadingRing(owner);", PresetBrowserPreviewKind.Grid, PresetBrowserCollectionKind.LoadingRing, "Clockwise");
            yield return Collection("LoadingRibbon", "Loops a traveling wave through a list.", "1.1s cycle", "items.LoadingRibbon(owner);", PresetBrowserPreviewKind.List, PresetBrowserCollectionKind.LoadingRibbon, "First to last");
        }

        private static IEnumerable<PresetBrowserEntry> CreateDestinationEntries()
        {
            const string category = "Destination Motion";
            const string badge = "MOTION";
            const PresetBrowserPreviewKind destination = PresetBrowserPreviewKind.Destination;
            const PresetBrowserPreviewKind worldToUi = PresetBrowserPreviewKind.WorldToUi;
            yield return Operation(PresetBrowserOperation.ArcTo, "Moves through a signed world-space arc and lands exactly.", category, "World destination", "0.75s", "target.Tween().ArcTo(destination, 2f).Play();", badge, destination);
            yield return Operation(PresetBrowserOperation.ArcLocalTo, "Moves through a signed local-space arc and lands exactly.", category, "Local destination", "0.75s", "target.Tween().ArcLocalTo(destination, 2f).Play();", badge, destination);
            yield return Operation(PresetBrowserOperation.BezierTo, "Follows a cubic world-space Bezier curve.", category, "World destination", "0.8s", "target.Tween().BezierTo(destination, controlA, controlB).Play();", badge, destination);
            yield return Operation(PresetBrowserOperation.BezierLocalTo, "Follows a cubic local-space Bezier curve.", category, "Local destination", "0.8s", "target.Tween().BezierLocalTo(destination, controlA, controlB).Play();", badge, destination);
            yield return Operation(PresetBrowserOperation.HopTo, "Hops in world space with anticipation and landing squash.", category, "World destination", "0.72s", "target.Tween().HopTo(destination, 2f).Play();", badge, destination);
            yield return Operation(PresetBrowserOperation.HopLocalTo, "Hops in local space with anticipation and landing squash.", category, "Local destination", "0.72s", "target.Tween().HopLocalTo(destination, 2f).Play();", badge, destination);
            yield return Operation(PresetBrowserOperation.SpringTo, "Overshoots a world destination and settles exactly.", category, "World destination", "0.68s", "target.Tween().SpringTo(destination).Play();", badge, destination);
            yield return Operation(PresetBrowserOperation.SpringLocalTo, "Overshoots a local destination and settles exactly.", category, "Local destination", "0.68s", "target.Tween().SpringLocalTo(destination).Play();", badge, destination);
            yield return Operation(PresetBrowserOperation.MagneticSnapTo, "Pulls back, accelerates past, and snaps to a world destination.", category, "World destination", "0.62s", "target.Tween().MagneticSnapTo(destination).Play();", badge, destination);
            yield return Operation(PresetBrowserOperation.MagneticSnapLocalTo, "Pulls back, accelerates past, and snaps to a local destination.", category, "Local destination", "0.62s", "target.Tween().MagneticSnapLocalTo(destination).Play();", badge, destination);
            yield return Operation(PresetBrowserOperation.PathThrough, "Travels through world-space waypoints.", category, "World path", "0.95s", "target.Tween().PathThrough(waypoints).Play();", badge, destination);
            yield return Operation(PresetBrowserOperation.PathLocalThrough, "Travels through local-space waypoints.", category, "Local path", "0.95s", "target.Tween().PathLocalThrough(waypoints).Play();", badge, destination);
            yield return Operation(PresetBrowserOperation.SpiralTo, "Closes a world-space spiral at the destination.", category, "World destination", "0.9s", "target.Tween().SpiralTo(destination, radius: 1.2f).Play();", badge, destination);
            yield return Operation(PresetBrowserOperation.SpiralLocalTo, "Closes a local-space spiral at the destination.", category, "Local destination", "0.9s", "target.Tween().SpiralLocalTo(destination, radius: 1.2f).Play();", badge, destination);
            yield return Operation(PresetBrowserOperation.MultiHopTo, "Lands at a world destination after diminishing hops.", category, "World destination", "1s", "target.Tween().MultiHopTo(destination, height: 2f).Play();", badge, destination);
            yield return Operation(PresetBrowserOperation.MultiHopLocalTo, "Lands at a local destination after diminishing hops.", category, "Local destination", "1s", "target.Tween().MultiHopLocalTo(destination, height: 2f).Play();", badge, destination);
            yield return Operation(PresetBrowserOperation.ArcToUI, "Projects a world source and arcs into a UI anchor.", category, "3D to 2D", "0.75s", "pickup.Tween().ArcToUI(worldSource, uiTarget, 145f, worldCamera: camera).Play();", badge, worldToUi);
            yield return Operation(PresetBrowserOperation.HopToUI, "Projects a world source and hops into a UI anchor.", category, "3D to 2D", "0.78s", "pickup.Tween().HopToUI(worldSource, uiTarget, 145f, worldCamera: camera).Play();", badge, worldToUi);
            yield return Operation(PresetBrowserOperation.BezierToUI, "Projects a world Bezier curve into a UI anchor.", category, "3D to 2D", "0.8s", "pickup.Tween().BezierToUI(worldSource, controlA, controlB, uiTarget, worldCamera: camera).Play();", badge, worldToUi);
            yield return Operation(PresetBrowserOperation.PathThroughUI, "Projects world landmarks into a UI path.", category, "3D to 2D", "0.95s", "pickup.Tween().PathThroughUI(worldSource, worldWaypoints, uiTarget, worldCamera: camera).Play();", badge, worldToUi);
        }

        private static IEnumerable<PresetBrowserEntry> CreateGameplayEntries()
        {
            const string category = "Gameplay Feedback";
            const string badge = "FEEDBACK";
            const PresetBrowserPreviewKind preview = PresetBrowserPreviewKind.Single;
            yield return Operation(PresetBrowserOperation.ErrorReject, "Rejects an action with shake, tilt, and color feedback.", category, "Core feedback", "0.46s", "target.ErrorReject();", badge, preview);
            yield return Operation(PresetBrowserOperation.DamageHit, "Communicates damage with shake, squash, recoil, and flash.", category, "Core feedback", "0.48s", "target.DamageHit();", badge, preview);
            yield return Operation(PresetBrowserOperation.SuccessConfirm, "Confirms success with a readable positive beat.", category, "Core feedback", "0.52s", "target.SuccessConfirm();", badge, preview);
            yield return Operation(PresetBrowserOperation.RewardReveal, "Reveals a reward with anticipation and settle.", category, "Core feedback", "0.72s", "target.RewardReveal();", badge, preview);
            yield return Operation(PresetBrowserOperation.HealReceive, "Shows a restorative heal response.", category, "Core feedback", "0.62s", "target.HealReceive();", badge, preview);
            yield return Operation(PresetBrowserOperation.ShieldBlock, "Blocks a directional impact.", category, "Core feedback", "0.5s", "target.ShieldBlock(Vector3.right);", badge, preview, "From right");
            yield return Operation(PresetBrowserOperation.CriticalHit, "Emphasizes a high-damage directional impact.", category, "Core feedback", "0.62s", "target.CriticalHit(Vector3.right);", badge, preview, "From right");
            yield return Operation(PresetBrowserOperation.CooldownReady, "Signals that an action is ready again.", category, "Core feedback", "0.58s", "target.CooldownReady();", badge, preview);
            yield return Operation(PresetBrowserOperation.LevelUp, "Celebrates progression with a layered reveal.", category, "Core feedback", "0.9s", "target.LevelUp();", badge, preview);
            yield return Operation(PresetBrowserOperation.LowHealthWarning, "Plays one finite low-health warning cycle.", category, "Core feedback", "0.86s", "target.LowHealthWarning();", badge, preview);
            yield return Operation(PresetBrowserOperation.PickupCollectTo, "Collects an item into a world destination.", category, "Pickup collection", "0.7s", "target.PickupCollectTo(destination);", badge, PresetBrowserPreviewKind.Destination);
            yield return Operation(PresetBrowserOperation.PickupCollectLocalTo, "Collects an item into a local destination.", category, "Pickup collection", "0.7s", "target.PickupCollectLocalTo(destination);", badge, PresetBrowserPreviewKind.Destination);
            yield return Operation(PresetBrowserOperation.PickupCollectToUI, "Collects a projected world pickup into a UI anchor.", category, "Pickup collection", "0.72s", "pickup.PickupCollectToUI(worldSource, uiTarget, worldCamera: camera);", badge, PresetBrowserPreviewKind.WorldToUi);
            yield return Operation(PresetBrowserOperation.AbilityCharging, "Builds anticipation while an ability charges.", category, "Gameplay states", "0.8s", "target.AbilityCharging();", badge, preview);
            yield return Operation(PresetBrowserOperation.AbilityReady, "Signals that an ability is ready.", category, "Gameplay states", "0.58s", "target.AbilityReady();", badge, preview);
            yield return Operation(PresetBrowserOperation.DodgeRoll, "Communicates a fast evasive roll.", category, "Gameplay states", "0.46s", "target.DodgeRoll();", badge, preview);
            yield return Operation(PresetBrowserOperation.StunStart, "Enters a stunned state with wobble and drop.", category, "Gameplay states", "0.62s", "target.StunStart();", badge, preview);
            yield return Operation(PresetBrowserOperation.StunEnd, "Recovers from stun with a clean rebound.", category, "Gameplay states", "0.48s", "target.StunEnd();", badge, preview);
            yield return Operation(PresetBrowserOperation.BuffApplied, "Applies positive status feedback.", category, "Gameplay states", "0.62s", "target.BuffApplied();", badge, preview);
            yield return Operation(PresetBrowserOperation.DebuffApplied, "Applies negative status feedback.", category, "Gameplay states", "0.62s", "target.DebuffApplied();", badge, preview);
            yield return Operation(PresetBrowserOperation.ResourceDepleted, "Warns that a resource has been depleted.", category, "Gameplay states", "0.66s", "target.ResourceDepleted();", badge, preview);
            yield return Operation(PresetBrowserOperation.ResourceRecovered, "Confirms that a resource recovered.", category, "Gameplay states", "0.58s", "target.ResourceRecovered();", badge, preview);
            yield return Operation(PresetBrowserOperation.ObjectiveUnlocked, "Celebrates an unlocked objective.", category, "Gameplay states", "0.82s", "target.ObjectiveUnlocked();", badge, preview);
            yield return Operation(PresetBrowserOperation.CriticalHitSequence, "Plays the reusable critical-hit macro.", category, "Sequence macros", "0.72s", "target.CriticalHitSequence(Vector3.right);", badge, preview);
            yield return Operation(PresetBrowserOperation.RewardRevealSequence, "Plays the reusable reward-reveal macro.", category, "Sequence macros", "0.9s", "target.RewardRevealSequence();", badge, preview);
            yield return Operation(PresetBrowserOperation.WarningLoopSequence, "Plays one reusable warning-loop cycle.", category, "Sequence macros", "0.92s", "target.WarningLoopSequence();", badge, preview);
            yield return Operation(PresetBrowserOperation.CutsceneUIEntranceSequence, "Plays a reusable cutscene UI entrance.", category, "Sequence macros", "0.9s", "target.CutsceneUIEntranceSequence();", badge, PresetBrowserPreviewKind.UiSequence);
        }

        private static IEnumerable<PresetBrowserEntry> CreateUISequenceEntries()
        {
            const string category = "UI Sequences";
            const string family = "UI composition";
            const string badge = "SEQUENCE";
            const PresetBrowserPreviewKind preview = PresetBrowserPreviewKind.UiSequence;
            yield return Operation(PresetBrowserOperation.ToastShow, "Shows a toast from above.", category, family, "0.36s", "toast.ToastShow();", badge, preview, "Up");
            yield return Operation(PresetBrowserOperation.ToastHide, "Hides a toast toward the top.", category, family, "0.3s", "toast.ToastHide();", badge, preview, "Up");
            yield return Operation(PresetBrowserOperation.ModalOpen, "Opens a modal, backdrop, and controls as one sequence.", category, family, "0.46s", "modal.ModalOpen(backdrop, controls);", badge, preview);
            yield return Operation(PresetBrowserOperation.ModalClose, "Closes modal controls, panel, and backdrop cleanly.", category, family, "0.4s", "modal.ModalClose(backdrop, controls);", badge, preview);
            yield return Operation(PresetBrowserOperation.TooltipShow, "Reveals a tooltip from above.", category, family, "0.24s", "tooltip.TooltipShow();", badge, preview, "Up");
            yield return Operation(PresetBrowserOperation.TooltipHide, "Hides a tooltip toward the top.", category, family, "0.2s", "tooltip.TooltipHide();", badge, preview, "Up");
            yield return Operation(PresetBrowserOperation.DropdownOpen, "Opens a dropdown and staggers its entries.", category, family, "0.34s", "dropdown.DropdownOpen(entries);", badge, preview);
            yield return Operation(PresetBrowserOperation.DropdownClose, "Closes dropdown entries and panel in reverse order.", category, family, "0.28s", "dropdown.DropdownClose(entries);", badge, preview);
            yield return Operation(PresetBrowserOperation.TabSwitchTo, "Switches between outgoing and incoming tabs.", category, family, "0.38s", "outgoing.TabSwitchTo(incoming);", badge, preview, "Left");
            yield return Operation(PresetBrowserOperation.DrawerShow, "Shows a left-side drawer with a backdrop.", category, family, "0.42s", "drawer.DrawerShow(backdrop: backdrop);", badge, preview, "Left");
            yield return Operation(PresetBrowserOperation.DrawerHide, "Hides a left-side drawer and backdrop.", category, family, "0.36s", "drawer.DrawerHide(backdrop: backdrop);", badge, preview, "Left");
            yield return Operation(PresetBrowserOperation.BottomSheetShow, "Shows a bottom sheet and backdrop.", category, family, "0.44s", "sheet.BottomSheetShow(backdrop);", badge, preview, "Up");
            yield return Operation(PresetBrowserOperation.BottomSheetHide, "Hides a bottom sheet and backdrop.", category, family, "0.38s", "sheet.BottomSheetHide(backdrop);", badge, preview, "Down");
            yield return Operation(PresetBrowserOperation.PagePushTo, "Pushes from one page to another.", category, family, "0.46s", "outgoing.PagePushTo(incoming);", badge, preview, "Left");
            yield return Operation(PresetBrowserOperation.PageCrossFadeTo, "Cross-fades between pages with subtle depth.", category, family, "0.42s", "outgoing.PageCrossFadeTo(incoming);", badge, preview);
        }

        private static IEnumerable<PresetBrowserEntry> CreateTextEntries()
        {
            const string category = "TextMesh Pro";
            const string family = "TMP text";
            const string badge = "TMP";
            const PresetBrowserPreviewKind preview = PresetBrowserPreviewKind.Text;
            yield return Operation(PresetBrowserOperation.TypewriterReveal, "Reveals visible text one character at a time.", category, family, "0.85s", "label.TypewriterReveal();", badge, preview);
            yield return Operation(PresetBrowserOperation.TypewriterHide, "Hides visible text one character at a time.", category, family, "0.65s", "label.TypewriterHide();", badge, preview);
            yield return Operation(PresetBrowserOperation.NumberCountUp, "Counts a formatted number upward.", category, family, "0.8s", "label.NumberCountTo(0, 1250, \"N0\");", badge, preview);
            yield return Operation(PresetBrowserOperation.NumberCountDown, "Counts a formatted number downward.", category, family, "0.8s", "label.NumberCountTo(1250, 0, \"N0\");", badge, preview);
            yield return Operation(PresetBrowserOperation.TextCharacterStaggerIn, "Staggers visible characters into place.", category, family, "0.65s", "label.TextCharacterStaggerIn();", badge, preview, "Up");
            yield return Operation(PresetBrowserOperation.TextCharacterStaggerOut, "Staggers visible characters out in reverse order.", category, family, "0.58s", "label.TextCharacterStaggerOut();", badge, preview, "Up");
            yield return Operation(PresetBrowserOperation.TextWave, "Sends a directional wave across visible characters.", category, family, "0.8s", "label.TextWave();", badge, preview, "Up");
            yield return Operation(PresetBrowserOperation.TextCharacterBounce, "Sends a finite bounce across visible characters.", category, family, "0.72s", "label.TextCharacterBounce();", badge, preview, "Up");
            yield return Operation(PresetBrowserOperation.TextColorSweep, "Sweeps a highlight color across the text.", category, family, "0.78s", "label.TextColorSweep(Color.cyan);", badge, preview);
            yield return Operation(PresetBrowserOperation.TextGlitch, "Applies deterministic character jitter and color separation.", category, family, "0.52s", "label.TextGlitch(seed: 1337);", badge, preview);
            yield return Operation(PresetBrowserOperation.TextEmphasis, "Lifts, scales, and colors a selected character range.", category, family, "0.55s", "label.TextEmphasis(startCharacter: 6, characterCount: 4);", badge, preview);
            yield return Operation(PresetBrowserOperation.TextScrambleReveal, "Resolves substitute glyphs into the authored text.", category, family, "0.9s", "label.TextScrambleReveal();", badge, preview);
            yield return Operation(PresetBrowserOperation.ScoreIncrease, "Counts a score upward with a punch and color flash.", category, family, "0.9s", "label.ScoreIncrease(900, 1250, \"N0\");", badge, preview);
        }

        private static IEnumerable<PresetBrowserEntry> CreateProgressEntries()
        {
            const string category = "Progress Bars";
            const string badge = "BAR";
            const string image = "Image fill";
            const string slider = "Slider value";
            const PresetBrowserPreviewKind imagePreview = PresetBrowserPreviewKind.ProgressImage;
            const PresetBrowserPreviewKind sliderPreview = PresetBrowserPreviewKind.ProgressSlider;
            yield return Operation(PresetBrowserOperation.ImageFillTo, "Animates an Image fillAmount to a normalized target.", category, image, "0.55s", "image.FillTo(0.85f);", badge, imagePreview, "Increase");
            yield return Operation(PresetBrowserOperation.ImageFillFromTo, "Animates Image fillAmount between explicit values.", category, image, "0.55s", "image.FillFromTo(0.15f, 0.85f);", badge, imagePreview, "Increase");
            yield return Operation(PresetBrowserOperation.ImageValueFillTo, "Animates Image fillAmount and a paired TMP value.", category, image, "0.6s", "image.ValueFillTo(0.85f, valueText);", badge, imagePreview, "Increase");
            yield return Operation(PresetBrowserOperation.ImageFillDrain, "Rapidly drains an Image fill with impact feedback.", category, image, "0.42s", "image.FillDrain(0.2f);", badge, imagePreview, "Decrease");
            yield return Operation(PresetBrowserOperation.ImageFillCharge, "Charges an Image fill with overshoot and settle.", category, image, "0.68s", "image.FillCharge(0.9f);", badge, imagePreview, "Increase");
            yield return Operation(PresetBrowserOperation.ImageFillAlertPulse, "Pulses an Image fill when its value is below a threshold.", category, image, "0.86s", "image.FillAlertPulse(0.25f);", badge, imagePreview, "Alert");
            yield return Operation(PresetBrowserOperation.ImageFillAndText, "Synchronizes Image fillAmount and formatted TMP text.", category, image, "0.65s", "image.FillAndText(0.15f, 0.85f, valueText);", badge, imagePreview, "Increase");
            yield return Operation(PresetBrowserOperation.SliderFillTo, "Animates a Slider normalized value to a target.", category, slider, "0.55s", "slider.FillTo(0.85f);", badge, sliderPreview, "Increase");
            yield return Operation(PresetBrowserOperation.SliderFillFromTo, "Animates a Slider between explicit normalized values.", category, slider, "0.55s", "slider.FillFromTo(0.15f, 0.85f);", badge, sliderPreview, "Increase");
            yield return Operation(PresetBrowserOperation.SliderValueFillTo, "Animates a Slider and a paired TMP value.", category, slider, "0.6s", "slider.ValueFillTo(0.85f, valueText);", badge, sliderPreview, "Increase");
            yield return Operation(PresetBrowserOperation.SliderFillDrain, "Rapidly drains a Slider with impact feedback.", category, slider, "0.42s", "slider.FillDrain(0.2f);", badge, sliderPreview, "Decrease");
            yield return Operation(PresetBrowserOperation.SliderFillCharge, "Charges a Slider with overshoot and settle.", category, slider, "0.68s", "slider.FillCharge(0.9f);", badge, sliderPreview, "Increase");
            yield return Operation(PresetBrowserOperation.SliderFillAlertPulse, "Pulses a Slider when its value is below a threshold.", category, slider, "0.86s", "slider.FillAlertPulse(0.25f);", badge, sliderPreview, "Alert");
            yield return Operation(PresetBrowserOperation.SliderFillAndText, "Synchronizes a Slider and formatted TMP text.", category, slider, "0.65s", "slider.FillAndText(0.15f, 0.85f, valueText);", badge, sliderPreview, "Increase");
        }

        private static IEnumerable<PresetBrowserEntry> CreateCameraEntries()
        {
            const string category = "Camera Feedback";
            const string family = "Camera";
            const string badge = "CAMERA";
            const PresetBrowserPreviewKind preview = PresetBrowserPreviewKind.Camera;
            yield return Operation(PresetBrowserOperation.CameraImpact, "Adds a short position and rotation impact.", category, family, "0.32s", "camera.CameraImpact();", badge, preview);
            yield return Operation(PresetBrowserOperation.CameraRecoil, "Recoils the camera and restores it.", category, family, "0.38s", "camera.CameraRecoil();", badge, preview);
            yield return Operation(PresetBrowserOperation.CameraLandingImpact, "Communicates a heavy landing through camera motion.", category, family, "0.46s", "camera.CameraLandingImpact();", badge, preview);
            yield return Operation(PresetBrowserOperation.CameraFovKick, "Kicks field of view outward and settles.", category, family, "0.4s", "camera.CameraFovKick();", badge, preview);
            yield return Operation(PresetBrowserOperation.CameraFocusZoom, "Moves and zooms toward an authored focus target.", category, family, "0.62s", "camera.CameraFocusZoom(focusTarget);", badge, preview);
            yield return Operation(PresetBrowserOperation.CameraBreathing, "Plays one finite camera breathing cycle.", category, family, "1.2s", "camera.CameraBreathing();", badge, preview);
            yield return Operation(PresetBrowserOperation.CameraRackFocus, "Shifts aim and field of view toward a focus target.", category, family, "0.58s", "camera.CameraRackFocus(focusTarget);", badge, preview);
            yield return Operation(PresetBrowserOperation.CollectLandingCameraKick, "Adds a small pickup-collection landing kick.", category, family, "0.3s", "camera.CollectLandingCameraKick();", badge, preview);
        }

        private static IEnumerable<PresetBrowserEntry> CreateEnginePropertyEntries()
        {
            const string category = "Engine Properties";
            const string badge = "PROPERTY";
            yield return Operation(PresetBrowserOperation.AudioVolumeTo, "Animates AudioSource volume with a live preview meter.", category, "Audio", "0.5s", "audioSource.AudioVolumeTo(1f);", badge, PresetBrowserPreviewKind.Audio, "Increase");
            yield return Operation(PresetBrowserOperation.AudioPitchTo, "Animates AudioSource pitch with a live preview meter.", category, "Audio", "0.5s", "audioSource.AudioPitchTo(1.5f);", badge, PresetBrowserPreviewKind.Audio, "Increase");
            yield return Operation(PresetBrowserOperation.LightIntensityTo, "Animates Light intensity with a live preview meter.", category, "Lighting", "0.55s", "light.LightIntensityTo(4f);", badge, PresetBrowserPreviewKind.Light, "Increase");
            yield return Operation(PresetBrowserOperation.LightColorTo, "Animates a Light toward a destination color.", category, "Lighting", "0.55s", "light.LightColorTo(Color.cyan);", badge, PresetBrowserPreviewKind.Light);
            yield return Operation(PresetBrowserOperation.ParticleEmissionRateTo, "Animates ParticleSystem emission rate.", category, "Particles", "0.6s", "particles.ParticleEmissionRateTo(60f);", badge, PresetBrowserPreviewKind.Particles, "Increase");
            yield return Operation(PresetBrowserOperation.MaterialFloatTo, "Animates a renderer material float through a property block.", category, "Materials", "0.55s", "renderer.MaterialFloatTo(\"_Metallic\", 1f);", badge, PresetBrowserPreviewKind.Material, "Increase");
            yield return Operation(PresetBrowserOperation.MaterialColorTo, "Animates a renderer material color through a property block.", category, "Materials", "0.55s", "renderer.MaterialColorTo(\"_BaseColor\", Color.magenta);", badge, PresetBrowserPreviewKind.Material);
            yield return Operation(PresetBrowserOperation.TorchFlicker, "Applies a finite layered intensity flicker to a Light.", category, "Lighting", "1.2s", "light.TorchFlicker();", badge, PresetBrowserPreviewKind.Light);
            yield return Operation(PresetBrowserOperation.ScannerPulse, "Pulses Light intensity and color like a scanner.", category, "Lighting", "0.9s", "light.ScannerPulse();", badge, PresetBrowserPreviewKind.Light);
        }

        private static PresetBrowserEntry Collection(string name, string description, string duration, string example, PresetBrowserPreviewKind previewKind, PresetBrowserCollectionKind collectionKind, string direction)
            => PresetBrowserEntry.Collection(name, description, duration, example, PresetBrowserEntryKind.CollectionRecipe, previewKind, collectionKind, direction);

        private static PresetBrowserEntry Operation(PresetBrowserOperation operation, string description, string category, string family, string duration, string example, string badge, PresetBrowserPreviewKind previewKind, string direction = null, string axisOrPlane = null)
            => PresetBrowserEntry.Builder(SplitName(operation.ToString()), description, category, family, duration, example, badge, previewKind, operation, direction, axisOrPlane);

        private static PresetBrowserEntry CreateOrderEntry(string name, string description, string order, PresetBrowserCollectionKind collectionKind, string direction)
        {
            string example = $"items.TweenStagger(owner).Preset<PulseScalePreset>(0.36f).Order(StaggerOrder.{order}).DelayBetween(0.14f).Play();";
            return PresetBrowserEntry.Collection(name, description, "0.36s per item", example, PresetBrowserEntryKind.StaggerVariant, PresetBrowserPreviewKind.List, collectionKind, direction);
        }

        private static PresetBrowserEntry CreateWaveEntry(string name, string description, string directionName, PresetBrowserCollectionKind collectionKind, string direction)
        {
            string example = $"items.GridWave(owner, columns: 3, direction: GridWaveDirection.{directionName});";
            return PresetBrowserEntry.Collection(name, description, "0.32s per item", example, PresetBrowserEntryKind.StaggerVariant, PresetBrowserPreviewKind.Grid, collectionKind, direction);
        }

        private static string SplitName(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            string split = Regex.Replace(value, "([A-Z]+)([A-Z][a-z])", "$1 $2");
            split = Regex.Replace(split, "([a-z0-9])([A-Z])", "$1 $2");
            return split.Replace("Fov", "FOV");
        }
    }
}
