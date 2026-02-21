using UnityEngine;

namespace Brawler.Combat
{
    /// <summary>
    /// ScriptableObject defining an attack's properties.
    /// Create via: Create > Brawler > Attack Data
    ///
    /// Students create these for each attack their fighter can perform.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAttack", menuName = "Brawler/Attack Data")]
    public class AttackData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Display name for this attack (e.g., 'Jab', 'Forward Smash').")]
        public string attackName = "New Attack";

        [Header("Damage")]
        [Tooltip("Damage dealt to the opponent's health.")]
        [Range(1f, 50f)]
        public float damage = 10f;

        [Header("Knockback")]
        [Tooltip("Base knockback force before health multiplier. Higher = launches farther.")]
        [Range(1f, 30f)]
        public float baseKnockback = 5f;

        [Tooltip("Direction of knockback. X is horizontal (1=forward, -1=backward), Y is vertical. " +
                 "Will be flipped based on attacker facing direction.")]
        public Vector2 knockbackAngle = new Vector2(1f, 0.5f);

        [Header("Timing (in frames at 60fps)")]
        [Tooltip("Frames before the hitbox becomes active. Higher = slower startup.")]
        [Range(1, 60)]
        public int startupFrames = 3;

        [Tooltip("Frames the hitbox is active. Higher = easier to land.")]
        [Range(1, 30)]
        public int activeFrames = 5;

        [Tooltip("Frames after active before the fighter can act again. Higher = more punishable.")]
        [Range(1, 60)]
        public int recoveryFrames = 10;

        [Header("Hitstun")]
        [Tooltip("How long the opponent can't act after being hit. 0 = auto-calculate from knockback.")]
        [Range(0f, 1f)]
        public float hitstunDuration = 0.2f;

        [Header("Hitstop")]
        [Tooltip("Freeze frame duration on hit. Creates impact feel.")]
        [Range(0f, 0.2f)]
        public float hitstopDuration = 0.05f;

        [Header("Hitbox")]
        [Tooltip("Offset from fighter center to hitbox center.")]
        public Vector2 hitboxOffset = new Vector2(0.5f, 0f);

        [Tooltip("Size of the hitbox.")]
        public Vector2 hitboxSize = new Vector2(1f, 1f);

        [Header("Attack Type")]
        [Tooltip("When this attack can be used.")]
        public AttackContext context = AttackContext.Any;

        [Header("Audio/Visual (Optional)")]
        [Tooltip("Sound effect to play on attack start.")]
        public AudioClip attackSound;

        [Tooltip("Sound effect to play on hit.")]
        public AudioClip hitSound;

        [Tooltip("Animation trigger name for this attack.")]
        public string animationTrigger = "Attack";

        // Convenience properties for timing in seconds (at 60fps)
        public float StartupTime => startupFrames / 60f;
        public float ActiveTime => activeFrames / 60f;
        public float RecoveryTime => recoveryFrames / 60f;
        public float TotalTime => (startupFrames + activeFrames + recoveryFrames) / 60f;
    }

    /// <summary>
    /// When an attack can be used.
    /// </summary>
    public enum AttackContext
    {
        Any,            // Can use anytime
        GroundedOnly,   // Must be on ground
        AerialOnly,     // Must be in air
        Neutral,        // No directional input
        Forward,        // Forward input
        Up,             // Up input
        Down,            // Down input
        Grab            // Grab input 
    }
}
