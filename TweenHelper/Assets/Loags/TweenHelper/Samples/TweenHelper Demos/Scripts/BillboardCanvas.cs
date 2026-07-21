using UnityEngine;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Keeps a world-space canvas facing the active main camera.
    /// </summary>
    public class BillboardCanvas : MonoBehaviour
    {
        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null) return;
            }

            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward, _mainCamera.transform.rotation * Vector3.up);
        }
    }
}
