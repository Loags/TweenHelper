using System;
using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    internal static class CameraMicroFeedbackUtility
    {
        public static Tween CreateRackFocus(GameObject target, Transform focusTarget, float fieldOfViewDelta, float duration, TweenOptions options)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (focusTarget == null) throw new ArgumentNullException(nameof(focusTarget));
            if (float.IsNaN(fieldOfViewDelta) || float.IsInfinity(fieldOfViewDelta)) throw new ArgumentOutOfRangeException(nameof(fieldOfViewDelta));
            if (options.SpeedBased == true) throw new NotSupportedException("Camera rack focus does not support speed-based timing.");
            Camera camera = target.GetComponent<Camera>();
            if (camera == null) throw new InvalidOperationException($"Camera rack focus target '{target.name}' requires a Camera component on the same GameObject.");
            Quaternion invocationRotation = default;
            float invocationFov = 0f;
            float strength = options.Strength ?? 1f;
            if (float.IsNaN(strength) || float.IsInfinity(strength) || strength < 0f) throw new ArgumentOutOfRangeException(nameof(TweenOptions.Strength), strength, "Strength must be finite and non-negative.");
            return NormalizedTweenTimeline.Create(
                target,
                duration,
                options.SetEase(Ease.Linear),
                () =>
                {
                    invocationRotation = target.transform.rotation;
                    invocationFov = camera.fieldOfView;
                },
                progress =>
                {
                    Vector3 direction = focusTarget.position - target.transform.position;
                    if (direction.sqrMagnitude <= 0.000001f) throw new InvalidOperationException("Rack-focus target cannot occupy the camera position.");
                    Quaternion focused = Quaternion.LookRotation(direction.normalized, Vector3.up);
                    float envelope = Mathf.Sin(progress * Mathf.PI);
                    float aim = DOVirtual.EasedValue(0f, 1f, envelope, Ease.InOutSine);
                    target.transform.rotation = Quaternion.SlerpUnclamped(invocationRotation, focused, aim);
                    camera.fieldOfView = Mathf.Clamp(invocationFov - fieldOfViewDelta * envelope * strength, 1f, 179f);
                },
                () => Restore(target.transform, camera, invocationRotation, invocationFov),
                () => Restore(target.transform, camera, invocationRotation, invocationFov),
                () => Restore(target.transform, camera, invocationRotation, invocationFov),
                () => Restore(target.transform, camera, invocationRotation, invocationFov));
        }

        private static void Restore(Transform target, Camera camera, Quaternion rotation, float fieldOfView)
        {
            target.rotation = rotation;
            camera.fieldOfView = fieldOfView;
        }
    }
}
