using UnityEngine;

namespace AIWorldStudio.GoldRush
{
    /// <summary>
    /// Simple first-person walker. Tweak the numbers in the Inspector to change how walking feels.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Normal walking speed in meters per second.")]
        public float walkSpeed = 3.5f;
        [Tooltip("Speed while holding Shift.")]
        public float sprintSpeed = 6f;
        public float gravity = -20f;

        [Header("Looking")]
        [Tooltip("How fast the camera turns with the mouse.")]
        public float lookSensitivity = 2f;
        [Tooltip("The camera (or an object holding it) that tilts up and down.")]
        public Transform cameraPivot;

        /// <summary>Set by other scripts (e.g. during dialogue) to freeze the player.</summary>
        public bool MovementLocked { get; set; }

        public bool CursorLocked { get; private set; }

        private CharacterController _controller;
        private float _pitch;
        private float _verticalVelocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (cameraPivot == null)
            {
                var cam = GetComponentInChildren<Camera>();
                if (cam != null) cameraPivot = cam.transform;
            }
        }

        private void Start()
        {
            SetCursorLocked(true);
        }

        private void Update()
        {
            if (PrototypeInput.EscapePressed) SetCursorLocked(false);
            else if (!CursorLocked && PrototypeInput.ClickPressed) SetCursorLocked(true);

            if (CursorLocked && !MovementLocked)
            {
                Vector2 look = PrototypeInput.Look * lookSensitivity;
                transform.Rotate(0f, look.x, 0f);
                _pitch = Mathf.Clamp(_pitch - look.y, -80f, 80f);
                if (cameraPivot != null) cameraPivot.localEulerAngles = new Vector3(_pitch, 0f, 0f);
            }

            Vector2 move = MovementLocked ? Vector2.zero : PrototypeInput.Move;
            Vector3 direction = transform.right * move.x + transform.forward * move.y;
            float speed = PrototypeInput.Sprint ? sprintSpeed : walkSpeed;

            if (_controller.isGrounded && _verticalVelocity < 0f) _verticalVelocity = -2f;
            _verticalVelocity += gravity * Time.deltaTime;

            _controller.Move((direction * speed + Vector3.up * _verticalVelocity) * Time.deltaTime);
        }

        public void SetCursorLocked(bool locked)
        {
            CursorLocked = locked;
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
