using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace AIWorldStudio.GoldRush
{
    /// <summary>
    /// One place that reads keyboard and mouse, so the prototype works with either of
    /// Unity's input systems (the new Input System package or the older Input Manager).
    /// Controls: WASD/arrows move, mouse looks, Shift sprints, E interacts, Esc frees the mouse.
    /// </summary>
    public static class PrototypeInput
    {
        /// <summary>Movement on X (strafe) and Y (forward), each between -1 and 1.</summary>
        public static Vector2 Move
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                var k = Keyboard.current;
                if (k == null) return Vector2.zero;
                float x = (k.dKey.isPressed || k.rightArrowKey.isPressed ? 1f : 0f)
                        - (k.aKey.isPressed || k.leftArrowKey.isPressed ? 1f : 0f);
                float y = (k.wKey.isPressed || k.upArrowKey.isPressed ? 1f : 0f)
                        - (k.sKey.isPressed || k.downArrowKey.isPressed ? 1f : 0f);
                return Vector2.ClampMagnitude(new Vector2(x, y), 1f);
#else
                return Vector2.ClampMagnitude(
                    new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1f);
#endif
            }
        }

        /// <summary>Mouse movement this frame, scaled so both input systems feel the same.</summary>
        public static Vector2 Look
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                var m = Mouse.current;
                return m == null ? Vector2.zero : m.delta.ReadValue() * 0.1f;
#else
                return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
#endif
            }
        }

        public static bool Sprint
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                var k = Keyboard.current;
                return k != null && (k.leftShiftKey.isPressed || k.rightShiftKey.isPressed);
#else
                return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
#endif
            }
        }

        public static bool InteractPressed
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                var k = Keyboard.current;
                return k != null && k.eKey.wasPressedThisFrame;
#else
                return Input.GetKeyDown(KeyCode.E);
#endif
            }
        }

        public static bool EscapePressed
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                var k = Keyboard.current;
                return k != null && k.escapeKey.wasPressedThisFrame;
#else
                return Input.GetKeyDown(KeyCode.Escape);
#endif
            }
        }

        public static bool ClickPressed
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                var m = Mouse.current;
                return m != null && m.leftButton.wasPressedThisFrame;
#else
                return Input.GetMouseButtonDown(0);
#endif
            }
        }
    }
}
