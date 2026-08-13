using System.Collections.Generic;
using UnityEngine;

namespace LB.TweenHelper
{
    public partial class TweenBuilder
    {
        /// <summary>
        /// Moves to a world-space destination along a signed vertical arc.
        /// </summary>
        public TweenBuilder ArcTo(Vector3 destination, float height, float? duration = null)
        {
            AddStep(options => DestinationMotionUtility.CreateArc(_gameObject, destination, height, ResolveDuration(duration, options), options, false), applyBuilderOptions: false);
            return this;
        }

        /// <summary>
        /// Moves to a local-space destination along a signed vertical arc. RectTransform targets use anchoredPosition3D.
        /// </summary>
        public TweenBuilder ArcLocalTo(Vector3 destination, float height, float? duration = null)
        {
            AddStep(options => DestinationMotionUtility.CreateArc(_gameObject, destination, height, ResolveDuration(duration, options), options, true), applyBuilderOptions: false);
            return this;
        }

        /// <summary>
        /// Moves to a world-space destination along a cubic Bezier curve with two world-space control points.
        /// </summary>
        public TweenBuilder BezierTo(Vector3 destination, Vector3 controlA, Vector3 controlB, float? duration = null)
        {
            AddStep(options => DestinationMotionUtility.CreateBezier(_gameObject, destination, controlA, controlB, ResolveDuration(duration, options), options, false), applyBuilderOptions: false);
            return this;
        }

        /// <summary>
        /// Moves to a local-space destination along a cubic Bezier curve. RectTransform values use anchored coordinates.
        /// </summary>
        public TweenBuilder BezierLocalTo(Vector3 destination, Vector3 controlA, Vector3 controlB, float? duration = null)
        {
            AddStep(options => DestinationMotionUtility.CreateBezier(_gameObject, destination, controlA, controlB, ResolveDuration(duration, options), options, true), applyBuilderOptions: false);
            return this;
        }

        /// <summary>
        /// Anticipates, hops to a world-space destination, squashes on landing, and restores the captured scale.
        /// </summary>
        public TweenBuilder HopTo(Vector3 destination, float height, float? duration = null)
        {
            AddStep(options => DestinationMotionUtility.CreateHop(_gameObject, destination, height, ResolveDuration(duration, options), options, false), applyBuilderOptions: false);
            return this;
        }

        /// <summary>
        /// Anticipates, hops to a local-space destination, squashes on landing, and restores the captured scale.
        /// </summary>
        public TweenBuilder HopLocalTo(Vector3 destination, float height, float? duration = null)
        {
            AddStep(options => DestinationMotionUtility.CreateHop(_gameObject, destination, height, ResolveDuration(duration, options), options, true), applyBuilderOptions: false);
            return this;
        }

        /// <summary>
        /// Moves past a world-space destination along the travel direction, then settles exactly on it.
        /// </summary>
        public TweenBuilder SpringTo(Vector3 destination, float? duration = null, float overshoot = 0.35f)
        {
            AddStep(options => DestinationMotionUtility.CreateSpring(_gameObject, destination, ResolveDuration(duration, options), overshoot, options, false), applyBuilderOptions: false);
            return this;
        }

        /// <summary>
        /// Moves past a local-space destination along the travel direction, then settles exactly on it.
        /// </summary>
        public TweenBuilder SpringLocalTo(Vector3 destination, float? duration = null, float overshoot = 0.35f)
        {
            AddStep(options => DestinationMotionUtility.CreateSpring(_gameObject, destination, ResolveDuration(duration, options), overshoot, options, true), applyBuilderOptions: false);
            return this;
        }

        /// <summary>
        /// Pulls away, accelerates past a world-space destination, then settles exactly on it.
        /// </summary>
        public TweenBuilder MagneticSnapTo(Vector3 destination, float? duration = null, float pullback = 0.2f, float overshoot = 0.25f)
        {
            AddStep(options => DestinationMotionUtility.CreateMagneticSnap(_gameObject, destination, ResolveDuration(duration, options), pullback, overshoot, options, false), applyBuilderOptions: false);
            return this;
        }

        /// <summary>
        /// Pulls away, accelerates past a local-space destination, then settles exactly on it.
        /// </summary>
        public TweenBuilder MagneticSnapLocalTo(Vector3 destination, float? duration = null, float pullback = 0.2f, float overshoot = 0.25f)
        {
            AddStep(options => DestinationMotionUtility.CreateMagneticSnap(_gameObject, destination, ResolveDuration(duration, options), pullback, overshoot, options, true), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Moves through world-space waypoints using linear or Catmull-Rom interpolation.</summary>
        public TweenBuilder PathThrough(IEnumerable<Vector3> waypoints, DestinationPathInterpolation interpolation = DestinationPathInterpolation.CatmullRom, float? duration = null)
        {
            IReadOnlyList<Vector3> snapshot = DestinationMotionUtility.SnapshotWaypoints(waypoints);
            AddStep(options => DestinationMotionUtility.CreateWaypointPath(_gameObject, snapshot, interpolation, ResolveDuration(duration, options), options, false), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Moves through local-space waypoints. RectTransform targets use anchoredPosition3D.</summary>
        public TweenBuilder PathLocalThrough(IEnumerable<Vector3> waypoints, DestinationPathInterpolation interpolation = DestinationPathInterpolation.CatmullRom, float? duration = null)
        {
            IReadOnlyList<Vector3> snapshot = DestinationMotionUtility.SnapshotWaypoints(waypoints);
            AddStep(options => DestinationMotionUtility.CreateWaypointPath(_gameObject, snapshot, interpolation, ResolveDuration(duration, options), options, true), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Progresses to a world-space destination through a finite spiral that closes exactly at both endpoints.</summary>
        public TweenBuilder SpiralTo(Vector3 destination, float radius, float revolutions = 1.5f, float? duration = null)
        {
            AddStep(options => DestinationMotionUtility.CreateSpiral(_gameObject, destination, radius, revolutions, ResolveDuration(duration, options), options, false), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Progresses to a local destination through a finite spiral. RectTransform targets use anchoredPosition3D.</summary>
        public TweenBuilder SpiralLocalTo(Vector3 destination, float radius, float revolutions = 1.5f, float? duration = null)
        {
            AddStep(options => DestinationMotionUtility.CreateSpiral(_gameObject, destination, radius, revolutions, ResolveDuration(duration, options), options, true), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Advances to a world-space destination through several diminishing hops.</summary>
        public TweenBuilder MultiHopTo(Vector3 destination, float height, int hopCount = 3, float decay = 1.25f, float? duration = null)
        {
            AddStep(options => DestinationMotionUtility.CreateMultiHop(_gameObject, destination, height, hopCount, decay, ResolveDuration(duration, options), options, false), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Advances to a local destination through several diminishing hops. RectTransform targets use anchoredPosition3D.</summary>
        public TweenBuilder MultiHopLocalTo(Vector3 destination, float height, int hopCount = 3, float decay = 1.25f, float? duration = null)
        {
            AddStep(options => DestinationMotionUtility.CreateMultiHop(_gameObject, destination, height, hopCount, decay, ResolveDuration(duration, options), options, true), applyBuilderOptions: false);
            return this;
        }
    }
}
