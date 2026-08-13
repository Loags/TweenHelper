using System.Collections.Generic;
using UnityEngine;

namespace LB.TweenHelper
{
    public partial class TweenBuilder
    {
        /// <summary>Slides, fades, and settles a toast on its captured shown state.</summary>
        public TweenBuilder ToastShow(UISequenceDirection direction = UISequenceDirection.Up, float distance = 56f, float? duration = null)
        {
            AddStep(options => UISequenceUtility.CreateToast(_gameObject, true, direction, distance, ResolveUISequenceDuration(duration, options, 0.4f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Anticipates, slides, and fades a toast out of view.</summary>
        public TweenBuilder ToastHide(UISequenceDirection direction = UISequenceDirection.Up, float distance = 56f, float? duration = null)
        {
            AddStep(options => UISequenceUtility.CreateToast(_gameObject, false, direction, distance, ResolveUISequenceDuration(duration, options, 0.28f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Reveals a modal panel with an optional backdrop and staggered controls.</summary>
        public TweenBuilder ModalOpen(GameObject backdrop = null, IEnumerable<GameObject> controls = null, float? duration = null, float childStagger = 0.045f)
        {
            IReadOnlyList<GameObject> capturedControls = UISequenceUtility.SnapshotTargets(controls, nameof(controls), _gameObject, backdrop);
            AddStep(options => UISequenceUtility.CreateModal(_gameObject, backdrop, capturedControls, true, ResolveUISequenceDuration(duration, options, 0.52f), childStagger, options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Dismisses modal controls, panel, and optional backdrop as one sequence.</summary>
        public TweenBuilder ModalClose(GameObject backdrop = null, IEnumerable<GameObject> controls = null, float? duration = null, float childStagger = 0.045f)
        {
            IReadOnlyList<GameObject> capturedControls = UISequenceUtility.SnapshotTargets(controls, nameof(controls), _gameObject, backdrop);
            AddStep(options => UISequenceUtility.CreateModal(_gameObject, backdrop, capturedControls, false, ResolveUISequenceDuration(duration, options, 0.38f), childStagger, options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Subtly raises, scales, and fades a tooltip into view.</summary>
        public TweenBuilder TooltipShow(UISequenceDirection direction = UISequenceDirection.Up, float distance = 16f, float? duration = null)
        {
            AddStep(options => UISequenceUtility.CreateTooltip(_gameObject, true, direction, distance, ResolveUISequenceDuration(duration, options, 0.22f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Moves and fades a tooltip out with restrained scale motion.</summary>
        public TweenBuilder TooltipHide(UISequenceDirection direction = UISequenceDirection.Up, float distance = 16f, float? duration = null)
        {
            AddStep(options => UISequenceUtility.CreateTooltip(_gameObject, false, direction, distance, ResolveUISequenceDuration(duration, options, 0.16f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Expands a dropdown from its authored pivot and staggers entries into view.</summary>
        public TweenBuilder DropdownOpen(IEnumerable<GameObject> entries = null, float? duration = null, float childStagger = 0.035f)
        {
            IReadOnlyList<GameObject> capturedEntries = UISequenceUtility.SnapshotTargets(entries, nameof(entries), _gameObject);
            AddStep(options => UISequenceUtility.CreateDropdown(_gameObject, capturedEntries, true, ResolveUISequenceDuration(duration, options, 0.36f), childStagger, options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Staggers entries out and compresses a dropdown toward its authored pivot.</summary>
        public TweenBuilder DropdownClose(IEnumerable<GameObject> entries = null, float? duration = null, float childStagger = 0.035f)
        {
            IReadOnlyList<GameObject> capturedEntries = UISequenceUtility.SnapshotTargets(entries, nameof(entries), _gameObject);
            AddStep(options => UISequenceUtility.CreateDropdown(_gameObject, capturedEntries, false, ResolveUISequenceDuration(duration, options, 0.26f), childStagger, options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Moves this outgoing tab away while revealing incoming content from the opposite side.</summary>
        public TweenBuilder TabSwitchTo(GameObject incoming, UISequenceDirection direction = UISequenceDirection.Left, float distance = 72f, float? duration = null)
        {
            AddStep(options => UISequenceUtility.CreateTabSwitch(_gameObject, incoming, direction, distance, ResolveUISequenceDuration(duration, options, 0.42f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Slides a drawer from its selected screen edge while optionally fading a backdrop.</summary>
        public TweenBuilder DrawerShow(UISequenceDirection edge = UISequenceDirection.Left, GameObject backdrop = null, float distance = 360f, float? duration = null)
        {
            AddStep(options => UISequenceUtility.CreateDrawer(_gameObject, backdrop, true, edge, distance, ResolveUISequenceDuration(duration, options, 0.44f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Slides a drawer back through its selected screen edge while optionally fading a backdrop.</summary>
        public TweenBuilder DrawerHide(UISequenceDirection edge = UISequenceDirection.Left, GameObject backdrop = null, float distance = 360f, float? duration = null)
        {
            AddStep(options => UISequenceUtility.CreateDrawer(_gameObject, backdrop, false, edge, distance, ResolveUISequenceDuration(duration, options, 0.32f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Raises a bottom sheet into its authored state while optionally fading a backdrop.</summary>
        public TweenBuilder BottomSheetShow(GameObject backdrop = null, float distance = 420f, float? duration = null)
        {
            AddStep(options => UISequenceUtility.CreateBottomSheet(_gameObject, backdrop, true, distance, ResolveUISequenceDuration(duration, options, 0.5f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Dismisses a bottom sheet below its authored state while optionally fading a backdrop.</summary>
        public TweenBuilder BottomSheetHide(GameObject backdrop = null, float distance = 420f, float? duration = null)
        {
            AddStep(options => UISequenceUtility.CreateBottomSheet(_gameObject, backdrop, false, distance, ResolveUISequenceDuration(duration, options, 0.36f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Pushes this page out while an incoming page enters from the opposite side.</summary>
        public TweenBuilder PagePushTo(GameObject incoming, UISequenceDirection direction = UISequenceDirection.Left, float distance = 720f, float? duration = null)
        {
            AddStep(options => UISequenceUtility.CreatePagePush(_gameObject, incoming, direction, distance, ResolveUISequenceDuration(duration, options, 0.52f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Cross-fades this page into an incoming page with restrained depth scaling.</summary>
        public TweenBuilder PageCrossFadeTo(GameObject incoming, float depthScale = 0.04f, float? duration = null)
        {
            AddStep(options => UISequenceUtility.CreatePageCrossFade(_gameObject, incoming, depthScale, ResolveUISequenceDuration(duration, options, 0.4f), options), applyBuilderOptions: false);
            return this;
        }

        private static float ResolveUISequenceDuration(float? duration, TweenOptions options, float fallback) => duration ?? options.Duration ?? fallback;
    }
}
