using UnityEngine;

namespace AIWorldStudio.GoldRush
{
    /// <summary>Tiny on-screen help: controls, a crosshair, and a "click to resume" note.</summary>
    public class PrototypeHud : MonoBehaviour
    {
        public PlayerController player;
        [TextArea] public string controlsText = "WASD move  ·  Mouse look  ·  Shift sprint  ·  E talk  ·  Esc free mouse";

        private GUIStyle _style;

        private void Start()
        {
            if (player == null) player = FindFirstObjectByType<PlayerController>();
        }

        private void OnGUI()
        {
            float scale = Screen.height / 1080f;
            if (_style == null || _style.fontSize != Mathf.RoundToInt(22 * scale))
            {
                _style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = Mathf.RoundToInt(22 * scale),
                    normal = { textColor = new Color(1f, 1f, 1f, 0.85f) }
                };
            }

            GUI.Label(new Rect(20 * scale, 16 * scale, Screen.width, 40 * scale), controlsText, _style);

            if (player == null) return;
            if (player.CursorLocked)
            {
                // Simple dot crosshair.
                float s = 6f * scale;
                GUI.Box(new Rect(Screen.width / 2f - s / 2f, Screen.height / 2f - s / 2f, s, s), GUIContent.none);
            }
            else
            {
                var centered = new GUIStyle(_style) { alignment = TextAnchor.MiddleCenter, fontSize = Mathf.RoundToInt(30 * scale) };
                GUI.Label(new Rect(0, Screen.height / 2f - 30 * scale, Screen.width, 60 * scale),
                    "Click to resume", centered);
            }
        }
    }
}
