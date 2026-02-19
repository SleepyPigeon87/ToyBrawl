using UnityEngine;

namespace Brawler.Fighter
{
    /// <summary>
    /// Configuration for fighter movement.
    /// Create via: Create > Brawler > Movement Config
    /// </summary>
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "Brawler/Movement Config")]
    public class MovementConfig : ScriptableObject
    {
        [Header("Horizontal Movement")]
        [Tooltip("Maximum horizontal speed in units per second.")]
        [Range(1f, 20f)]
        public float maxSpeed = 8f;

        [Tooltip("How quickly the fighter reaches max speed.")]
        [Range(10f, 200f)]
        public float acceleration = 60f;

        [Tooltip("How quickly the fighter stops when not moving.")]
        [Range(10f, 200f)]
        public float deceleration = 60f;

        [Header("Turn Around")]
        [Tooltip("Multiplier on deceleration when changing direction.")]
        [Range(1f, 3f)]
        public float turnAroundMultiplier = 1.5f;

        [Header("Air Control")]
        [Tooltip("How much horizontal control in the air. 0 = none, 1 = full.")]
        [Range(0f, 1f)]
        public float airControlMultiplier = 0.85f;

        [Header("Jumping")]
        [Tooltip("Upward velocity applied when jumping.")]
        [Range(5f, 25f)]
        public float jumpForce = 14f;

        [Tooltip("Velocity multiplier when releasing jump early.")]
        [Range(0.1f, 1f)]
        public float jumpCutMultiplier = 0.5f;

        [Tooltip("Seconds after leaving ground where jump still works.")]
        [Range(0f, 0.3f)]
        public float coyoteTime = 0.12f;

        [Header("Gravity")]
        [Tooltip("Base gravity scale. 1 = Unity default.")]
        [Range(0.5f, 3f)]
        public float gravityScale = 1f;

        [Tooltip("Gravity multiplier when falling.")]
        [Range(1f, 3f)]
        public float fallGravityMultiplier = 1.6f;

        [Header("Ground Detection")]
        [Tooltip("How far below the fighter to check for ground.")]
        [Range(0.01f, 0.5f)]
        public float groundCheckDistance = 0.1f;

        [Tooltip("Which layers count as ground.")]
        public LayerMask groundLayer;
    }
}
