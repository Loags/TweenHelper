using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>One-line finite camera feedback animations backed by composable TweenBuilder operations.</summary>
    public static class CameraFeedbackExtensions
    {
        public static TweenHandle CameraImpact(this Camera camera, float positionStrength = 0.18f, float rotationStrength = 2.4f, float? duration = null, TweenOptions options = default)
            => camera.gameObject.CameraImpact(positionStrength, rotationStrength, duration, options);

        public static TweenHandle CameraImpact(this GameObject target, float positionStrength = 0.18f, float rotationStrength = 2.4f, float? duration = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).CameraImpact(positionStrength, rotationStrength, duration).Play();

        public static TweenHandle CameraRecoil(this Camera camera, float distance = 0.3f, float pitch = 4f, float? duration = null, TweenOptions options = default)
            => camera.gameObject.CameraRecoil(distance, pitch, duration, options);

        public static TweenHandle CameraRecoil(this GameObject target, float distance = 0.3f, float pitch = 4f, float? duration = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).CameraRecoil(distance, pitch, duration).Play();

        public static TweenHandle CameraLandingImpact(this Camera camera, float dropDistance = 0.22f, float fieldOfViewKick = 3f, float? duration = null, TweenOptions options = default)
            => camera.gameObject.CameraLandingImpact(dropDistance, fieldOfViewKick, duration, options);

        public static TweenHandle CameraLandingImpact(this GameObject target, float dropDistance = 0.22f, float fieldOfViewKick = 3f, float? duration = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).CameraLandingImpact(dropDistance, fieldOfViewKick, duration).Play();

        public static TweenHandle CameraFovKick(this Camera camera, float fieldOfViewDelta = 8f, float? duration = null, TweenOptions options = default)
            => camera.gameObject.CameraFovKick(fieldOfViewDelta, duration, options);

        public static TweenHandle CameraFovKick(this GameObject target, float fieldOfViewDelta = 8f, float? duration = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).CameraFovKick(fieldOfViewDelta, duration).Play();

        public static TweenHandle CameraFocusZoom(this Camera camera, Transform focusTarget, float distance = 1.2f, float fieldOfViewDelta = 7f, float? duration = null, TweenOptions options = default)
            => camera.gameObject.CameraFocusZoom(focusTarget, distance, fieldOfViewDelta, duration, options);

        public static TweenHandle CameraFocusZoom(this GameObject target, Transform focusTarget, float distance = 1.2f, float fieldOfViewDelta = 7f, float? duration = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).CameraFocusZoom(focusTarget, distance, fieldOfViewDelta, duration).Play();

        public static TweenHandle CameraBreathing(this Camera camera, float positionAmplitude = 0.035f, float rotationAmplitude = 0.3f, float fieldOfViewAmplitude = 0.45f, float? duration = null, TweenOptions options = default)
            => camera.gameObject.CameraBreathing(positionAmplitude, rotationAmplitude, fieldOfViewAmplitude, duration, options);

        public static TweenHandle CameraBreathing(this GameObject target, float positionAmplitude = 0.035f, float rotationAmplitude = 0.3f, float fieldOfViewAmplitude = 0.45f, float? duration = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).CameraBreathing(positionAmplitude, rotationAmplitude, fieldOfViewAmplitude, duration).Play();

        public static TweenHandle CameraRackFocus(this Camera camera, Transform focusTarget, float fieldOfViewDelta = 5f, float? duration = null, TweenOptions options = default)
            => camera.gameObject.Tween().WithOptions(options).CameraRackFocus(focusTarget, fieldOfViewDelta, duration).Play();

        public static TweenHandle CollectLandingCameraKick(this Camera camera, float? duration = null, TweenOptions options = default)
            => camera.gameObject.Tween().WithOptions(options).CollectLandingCameraKick(duration).Play();
    }
}
