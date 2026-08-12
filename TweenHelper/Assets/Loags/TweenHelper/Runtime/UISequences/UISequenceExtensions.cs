using System.Collections.Generic;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>One-line production UI transitions backed by composable TweenBuilder operations.</summary>
    public static class UISequenceExtensions
    {
        public static TweenHandle ToastShow(this Component component, UISequenceDirection direction = UISequenceDirection.Up, float distance = 56f, float? duration = null, TweenOptions options = default)
            => ToastShow(component.gameObject, direction, distance, duration, options);

        public static TweenHandle ToastShow(this GameObject target, UISequenceDirection direction = UISequenceDirection.Up, float distance = 56f, float? duration = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).ToastShow(direction, distance, duration).Play();

        public static TweenHandle ToastHide(this Component component, UISequenceDirection direction = UISequenceDirection.Up, float distance = 56f, float? duration = null, TweenOptions options = default)
            => ToastHide(component.gameObject, direction, distance, duration, options);

        public static TweenHandle ToastHide(this GameObject target, UISequenceDirection direction = UISequenceDirection.Up, float distance = 56f, float? duration = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).ToastHide(direction, distance, duration).Play();

        public static TweenHandle ModalOpen(this Component component, GameObject backdrop = null, IEnumerable<GameObject> controls = null, float? duration = null, float childStagger = 0.045f, TweenOptions options = default)
            => ModalOpen(component.gameObject, backdrop, controls, duration, childStagger, options);

        public static TweenHandle ModalOpen(this GameObject target, GameObject backdrop = null, IEnumerable<GameObject> controls = null, float? duration = null, float childStagger = 0.045f, TweenOptions options = default)
            => target.Tween().WithOptions(options).ModalOpen(backdrop, controls, duration, childStagger).Play();

        public static TweenHandle ModalClose(this Component component, GameObject backdrop = null, IEnumerable<GameObject> controls = null, float? duration = null, float childStagger = 0.045f, TweenOptions options = default)
            => ModalClose(component.gameObject, backdrop, controls, duration, childStagger, options);

        public static TweenHandle ModalClose(this GameObject target, GameObject backdrop = null, IEnumerable<GameObject> controls = null, float? duration = null, float childStagger = 0.045f, TweenOptions options = default)
            => target.Tween().WithOptions(options).ModalClose(backdrop, controls, duration, childStagger).Play();

        public static TweenHandle TooltipShow(this Component component, UISequenceDirection direction = UISequenceDirection.Up, float distance = 16f, float? duration = null, TweenOptions options = default)
            => TooltipShow(component.gameObject, direction, distance, duration, options);

        public static TweenHandle TooltipShow(this GameObject target, UISequenceDirection direction = UISequenceDirection.Up, float distance = 16f, float? duration = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).TooltipShow(direction, distance, duration).Play();

        public static TweenHandle TooltipHide(this Component component, UISequenceDirection direction = UISequenceDirection.Up, float distance = 16f, float? duration = null, TweenOptions options = default)
            => TooltipHide(component.gameObject, direction, distance, duration, options);

        public static TweenHandle TooltipHide(this GameObject target, UISequenceDirection direction = UISequenceDirection.Up, float distance = 16f, float? duration = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).TooltipHide(direction, distance, duration).Play();

        public static TweenHandle DropdownOpen(this Component component, IEnumerable<GameObject> entries = null, float? duration = null, float childStagger = 0.035f, TweenOptions options = default)
            => DropdownOpen(component.gameObject, entries, duration, childStagger, options);

        public static TweenHandle DropdownOpen(this GameObject target, IEnumerable<GameObject> entries = null, float? duration = null, float childStagger = 0.035f, TweenOptions options = default)
            => target.Tween().WithOptions(options).DropdownOpen(entries, duration, childStagger).Play();

        public static TweenHandle DropdownClose(this Component component, IEnumerable<GameObject> entries = null, float? duration = null, float childStagger = 0.035f, TweenOptions options = default)
            => DropdownClose(component.gameObject, entries, duration, childStagger, options);

        public static TweenHandle DropdownClose(this GameObject target, IEnumerable<GameObject> entries = null, float? duration = null, float childStagger = 0.035f, TweenOptions options = default)
            => target.Tween().WithOptions(options).DropdownClose(entries, duration, childStagger).Play();

        public static TweenHandle TabSwitchTo(this Component component, GameObject incoming, UISequenceDirection direction = UISequenceDirection.Left, float distance = 72f, float? duration = null, TweenOptions options = default)
            => TabSwitchTo(component.gameObject, incoming, direction, distance, duration, options);

        public static TweenHandle TabSwitchTo(this GameObject target, GameObject incoming, UISequenceDirection direction = UISequenceDirection.Left, float distance = 72f, float? duration = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).TabSwitchTo(incoming, direction, distance, duration).Play();

        /// <summary>Recaptures the current authored UI state used as the shown endpoint.</summary>
        public static void RefreshUIAnimationState(this Component component) => RefreshUIAnimationState(component.gameObject);

        /// <summary>Recaptures the current authored UI state used as the shown endpoint.</summary>
        public static void RefreshUIAnimationState(this GameObject target) => UIAnimationStateCache.GetOrCreate(target).Refresh();
    }
}
