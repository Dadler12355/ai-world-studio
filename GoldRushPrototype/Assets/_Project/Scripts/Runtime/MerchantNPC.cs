using System;
using UnityEngine;

namespace AIWorldStudio.GoldRush
{
    /// <summary>
    /// Placeholder merchant. Walk close, press E to talk, press E to continue each line.
    /// Dialogue text lives in a JSON file (Content/Dialogue) so writers can edit it
    /// without touching code. Trading comes in a later task.
    /// </summary>
    public class MerchantNPC : MonoBehaviour
    {
        [Serializable]
        private class DialogueData
        {
            public string speaker;
            public string[] lines;
        }

        [Header("Dialogue")]
        [Tooltip("JSON file with { \"speaker\": \"...\", \"lines\": [\"...\"] }.")]
        public TextAsset dialogueFile;
        [Tooltip("Used only if no dialogue file is assigned or it cannot be read.")]
        public string fallbackSpeaker = "Merchant";
        [TextArea] public string[] fallbackLines = { "Howdy, stranger. Come back when you've got gold." };

        [Header("Interaction")]
        [Tooltip("How close (meters) the player must be to talk.")]
        public float interactRadius = 3f;

        private string _speaker;
        private string[] _lines;
        private PlayerController _player;
        private bool _inRange;
        private bool _talking;
        private int _lineIndex;
        private GUIStyle _boxStyle, _nameStyle, _textStyle, _hintStyle;

        public bool IsTalking => _talking;

        private void Awake()
        {
            _speaker = fallbackSpeaker;
            _lines = fallbackLines;
            if (dialogueFile != null)
            {
                try
                {
                    var data = JsonUtility.FromJson<DialogueData>(dialogueFile.text);
                    if (data != null && data.lines != null && data.lines.Length > 0)
                    {
                        _lines = data.lines;
                        if (!string.IsNullOrEmpty(data.speaker)) _speaker = data.speaker;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"MerchantNPC: could not read dialogue file '{dialogueFile.name}': {e.Message}", this);
                }
            }
        }

        private void Start()
        {
            _player = FindFirstObjectByType<PlayerController>();
            if (_player == null) Debug.LogWarning("MerchantNPC: no PlayerController found in the scene.", this);
        }

        private void Update()
        {
            if (_player == null) return;

            Vector3 offset = _player.transform.position - transform.position;
            offset.y = 0f;
            _inRange = offset.magnitude <= interactRadius;

            if (_talking)
            {
                // Turn to face the player while talking.
                if (offset.sqrMagnitude > 0.01f)
                {
                    Quaternion target = Quaternion.LookRotation(offset.normalized, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 6f);
                }

                if (PrototypeInput.InteractPressed)
                {
                    _lineIndex++;
                    if (_lineIndex >= _lines.Length) EndConversation();
                }
            }
            else if (_inRange && _lines != null && _lines.Length > 0 && PrototypeInput.InteractPressed)
            {
                StartConversation();
            }
        }

        private void StartConversation()
        {
            _talking = true;
            _lineIndex = 0;
            _player.MovementLocked = true;
        }

        private void EndConversation()
        {
            _talking = false;
            _player.MovementLocked = false;
        }

        private void OnGUI()
        {
            if (!_talking && !_inRange) return;
            EnsureStyles();

            float scale = Screen.height / 1080f;
            if (_talking)
            {
                float w = Mathf.Min(Screen.width - 80f, 1100f * scale);
                float h = 220f * scale;
                var rect = new Rect((Screen.width - w) / 2f, Screen.height - h - 40f * scale, w, h);
                GUI.Box(rect, GUIContent.none, _boxStyle);
                float pad = 24f * scale;
                var inner = new Rect(rect.x + pad, rect.y + pad, rect.width - pad * 2, rect.height - pad * 2);
                GUI.Label(new Rect(inner.x, inner.y, inner.width, 44f * scale), _speaker, _nameStyle);
                GUI.Label(new Rect(inner.x, inner.y + 50f * scale, inner.width, inner.height - 90f * scale),
                    _lines[_lineIndex], _textStyle);
                string hint = _lineIndex < _lines.Length - 1 ? "[E] Continue" : "[E] Goodbye";
                GUI.Label(new Rect(inner.x, inner.yMax - 34f * scale, inner.width, 34f * scale), hint, _hintStyle);
            }
            else
            {
                float w = 420f * scale, h = 60f * scale;
                var rect = new Rect((Screen.width - w) / 2f, Screen.height * 0.62f, w, h);
                GUI.Box(rect, GUIContent.none, _boxStyle);
                GUI.Label(rect, $"[E] Talk to {_speaker}", _hintStyle);
            }
        }

        private void EnsureStyles()
        {
            int size = Mathf.RoundToInt(30 * Screen.height / 1080f);
            if (_textStyle != null && _textStyle.fontSize == size) return;

            var bg = new Texture2D(1, 1);
            bg.SetPixel(0, 0, new Color(0.12f, 0.08f, 0.05f, 0.88f));
            bg.Apply();

            _boxStyle = new GUIStyle(GUI.skin.box) { normal = { background = bg } };
            _nameStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(size * 1.15f), fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.98f, 0.78f, 0.35f) }
            };
            _textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = size, wordWrap = true, normal = { textColor = new Color(0.96f, 0.93f, 0.86f) }
            };
            _hintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(size * 0.85f), alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.98f, 0.78f, 0.35f) }
            };
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.8f, 0.2f, 0.6f);
            Gizmos.DrawWireSphere(transform.position, interactRadius);
        }
    }
}
