using UnityEngine;

namespace LB.TweenHelper
{
    public partial class TweenBuilder
    {
        /// <summary>Applies a sharp deterministic position and rotation impact to a Camera target.</summary>
        public TweenBuilder CameraImpact(float positionStrength = 0.18f, float rotationStrength = 2.4f, float? duration = null)
        {
            AddStep(options => CameraFeedbackUtility.CreateImpact(_gameObject, positionStrength, rotationStrength, ResolveCameraFeedbackDuration(duration, options, 0.38f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Kicks a Camera backward and upward before settling on its exact captured pose.</summary>
        public TweenBuilder CameraRecoil(float distance = 0.3f, float pitch = 4f, float? duration = null)
        {
            AddStep(options => CameraFeedbackUtility.CreateRecoil(_gameObject, distance, pitch, ResolveCameraFeedbackDuration(duration, options, 0.48f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Communicates a heavy landing with a downward bump, roll aftershock, and small field-of-view kick.</summary>
        public TweenBuilder CameraLandingImpact(float dropDistance = 0.22f, float fieldOfViewKick = 3f, float? duration = null)
        {
            AddStep(options => CameraFeedbackUtility.CreateLandingImpact(_gameObject, dropDistance, fieldOfViewKick, ResolveCameraFeedbackDuration(duration, options, 0.55f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Kicks a Camera's field of view and restores the exact captured value.</summary>
        public TweenBuilder CameraFovKick(float fieldOfViewDelta = 8f, float? duration = null)
        {
            AddStep(options => CameraFeedbackUtility.CreateFovKick(_gameObject, fieldOfViewDelta, ResolveCameraFeedbackDuration(duration, options, 0.42f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Temporarily moves and aims a Camera toward a supplied focus target before restoring it.</summary>
        public TweenBuilder CameraFocusZoom(Transform focusTarget, float distance = 1.2f, float fieldOfViewDelta = 7f, float? duration = null)
        {
            AddStep(options => CameraFeedbackUtility.CreateFocusZoom(_gameObject, focusTarget, distance, fieldOfViewDelta, ResolveCameraFeedbackDuration(duration, options, 0.82f), options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Plays one subtle finite camera-breathing cycle suitable for root looping.</summary>
        public TweenBuilder CameraBreathing(float positionAmplitude = 0.035f, float rotationAmplitude = 0.3f, float fieldOfViewAmplitude = 0.45f, float? duration = null)
        {
            AddStep(options => CameraFeedbackUtility.CreateBreathing(_gameObject, positionAmplitude, rotationAmplitude, fieldOfViewAmplitude, ResolveCameraFeedbackDuration(duration, options, 2.8f), options), applyBuilderOptions: false);
            return this;
        }

        private static float ResolveCameraFeedbackDuration(float? duration, TweenOptions options, float fallback) => duration ?? options.Duration ?? fallback;
    }
}
