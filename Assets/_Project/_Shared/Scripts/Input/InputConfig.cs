using UnityEngine;

namespace Brawler.Input
{
    /// <summary>
    /// Configuration for input handling. Create via: Create > Brawler > Input Config
    /// </summary>
    [CreateAssetMenu(fileName = "InputConfig", menuName = "Brawler/Input Config")]
    public class InputConfig : ScriptableObject
    {
        [Header("Deadzone")]
        [Tooltip("Stick values below this are treated as zero. Prevents drift.")]
        [Range(0.05f, 0.4f)]
        public float deadzone = 0.15f;

        [Header("Input Buffering")]
        [Tooltip("How long to remember a jump press after the button is hit.")]
        [Range(0f, 0.3f)]
        public float jumpBufferDuration = 0.1f;

        [Tooltip("How long to remember a dash press.")]
        [Range(0f, 0.3f)]
        public float dashBufferDuration = 0.08f;

        [Tooltip("How long to remember an attack press.")]
        [Range(0f, 0.3f)]
        public float attackBufferDuration = 0.1f;

        [Header("Keyboard Settings")]
        [Tooltip("What analog value keyboard input maps to. 1.0 = full speed.")]
        [Range(0.5f, 1f)]
        public float keyboardAnalogValue = 1f;
    }
}
