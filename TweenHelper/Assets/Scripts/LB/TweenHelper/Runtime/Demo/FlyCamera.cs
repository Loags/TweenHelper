using UnityEngine;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Fly camera controller for exploring the preset showcase.
    /// WASD to move, hold right-click + mouse to look around, mousewheel to adjust speed.
    /// </summary>
    public class FlyCamera : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float baseSpeed = 5f;
        [SerializeField] private float minSpeed = 1f;
        [SerializeField] private float maxSpeed = 50f;
        [SerializeField] private float speedScrollSensitivity = 2f;
        [SerializeField] private float shiftMultiplier = 2f;

        [Header("Look")]
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private bool invertY = false;

        [Header("Info")]
        [SerializeField] private float currentSpeed;

        private float _yaw;
        private float _pitch;

        private void Start()
        {
            currentSpeed = baseSpeed;

            // Initialize rotation from current transform
            var euler = transform.eulerAngles;
            _yaw = euler.y;
            _pitch = euler.x;

            // Normalize pitch to -180 to 180 range
            if (_pitch > 180f) _pitch -= 360f;
        }

        private void Update()
        {
            HandleSpeedAdjustment();
            HandleLook();
            HandleMovement();
        }

        private void HandleSpeedAdjustment()
        {
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                currentSpeed += scroll * speedScrollSensitivity * currentSpeed;
                currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
            }
        }

        private void HandleLook()
        {
            // Only look when right mouse button is held
            if (!Input.GetMouseButton(1)) return;

            var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            _yaw += mouseX;
            _pitch += invertY ? mouseY : -mouseY;
            _pitch = Mathf.Clamp(_pitch, -89f, 89f);

            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }

        private void HandleMovement()
        {
            var speed = currentSpeed;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                speed *= shiftMultiplier;
            }

            var moveDir = Vector3.zero;

            // WASD movement
            if (Input.GetKey(KeyCode.W)) moveDir += transform.forward;
            if (Input.GetKey(KeyCode.S)) moveDir -= transform.forward;
            if (Input.GetKey(KeyCode.A)) moveDir -= transform.right;
            if (Input.GetKey(KeyCode.D)) moveDir += transform.right;

            // Up/Down with Q/E
            if (Input.GetKey(KeyCode.E)) moveDir += Vector3.up;
            if (Input.GetKey(KeyCode.Q)) moveDir -= Vector3.up;

            if (moveDir.sqrMagnitude > 0.01f)
            {
                transform.position += moveDir.normalized * speed * Time.deltaTime;
            }
        }

        private void OnGUI()
        {
            // Show controls in corner
            GUI.Label(new Rect(10, 10, 200, 25), $"Speed: {currentSpeed:F1} (scroll to adjust)");
            GUI.Label(new Rect(10, 35, 400, 25), "WASD: Move | Right-Click+Mouse: Look | Q/E: Up/Down | Shift: Fast");
            GUI.Label(new Rect(10, 60, 300, 25), "Click on objects to play animations");

            // Reset controls
            var historyCount = AnimationResetManager.Instance?.HistoryCount ?? 0;
            GUI.Label(new Rect(10, 85, 400, 25), $"R: Reset last | Shift+R: Reset all ({historyCount} in history)");
        }
    }
}
