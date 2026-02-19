using UnityEngine;
using Brawler.Input;
using Brawler.Combat;
using Brawler.Core;

namespace Brawler.Fighter
{
    /// <summary>
    /// Default movement implementation for fighters.
    /// Students can use this as-is or replace with their own movement from the Platformer project.
    ///
    /// Features:
    ///   - Acceleration-based horizontal movement
    ///   - Variable height jumping with coyote time
    ///   - Air control
    ///   - Gravity scaling (faster falling)
    ///
    /// To use your own movement:
    ///   1. Remove this component
    ///   2. Add your own movement script
    ///   3. Make sure it respects KnockbackHandler.IsInHitstun
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class FighterMovement : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private MovementConfig config;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheckPoint;

        public bool IsGrounded { get; private set; }
        public float HorizontalSpeed => rb.linearVelocity.x;
        public float VerticalSpeed => rb.linearVelocity.y;

        private Rigidbody2D rb;
        private PlayerInputHandler input;
        private KnockbackHandler knockback;
        private AttackController attackController;

        private float lastGroundedTime;
        private bool hasJumpedSinceGrounded;
        private bool isJumping;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            knockback = GetComponent<KnockbackHandler>();
            attackController = GetComponent<AttackController>();
        }

        /// <summary>
        /// Initialize with input handler.
        /// Called by FighterBase.
        /// </summary>
        public void Initialize(PlayerInputHandler inputHandler)
        {
            input = inputHandler;
        }

        private void FixedUpdate()
        {
            if (config == null || input == null) return;

            // Don't process movement during hitstun
            if (knockback != null && knockback.IsInHitstun)
            {
                return;
            }

            // Don't process movement during countdown or between rounds
            var gm = GameManager.Instance;
            if (gm != null && gm.CurrentState != GameState.Fighting && gm.CurrentState != GameState.Waiting)
            {
                return;
            }

            UpdateGroundedState();
            UpdateHorizontalMovement();
            UpdateJump();
            UpdateGravityScale();
        }

        private void UpdateGroundedState()
        {
            bool wasGrounded = IsGrounded;
            IsGrounded = CheckGrounded();

            if (IsGrounded)
            {
                lastGroundedTime = Time.time;

                if (!wasGrounded)
                {
                    hasJumpedSinceGrounded = false;
                    isJumping = false;
                }
            }
        }

        private bool CheckGrounded()
        {
            if (groundCheckPoint == null) return false;

            RaycastHit2D hit = Physics2D.Raycast(
                groundCheckPoint.position,
                Vector2.down,
                config.groundCheckDistance,
                config.groundLayer
            );

            return hit.collider != null;
        }

        private bool CanJump()
        {
            if (hasJumpedSinceGrounded) return false;
            if (IsGrounded) return true;

            float timeSinceGrounded = Time.time - lastGroundedTime;
            return timeSinceGrounded <= config.coyoteTime;
        }

        private void UpdateHorizontalMovement()
        {
            // Don't move during attack
            if (attackController != null && attackController.IsAttacking)
            {
                return;
            }

            float inputX = input.MoveInput.x;
            float targetVelocity = inputX * config.maxSpeed;
            float currentVelocity = rb.linearVelocity.x;

            float rate = CalculateMoveRate(inputX, currentVelocity);

            if (!IsGrounded)
            {
                rate *= config.airControlMultiplier;
            }

            float newVelocity = Mathf.MoveTowards(currentVelocity, targetVelocity, rate * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newVelocity, rb.linearVelocity.y);
        }

        private float CalculateMoveRate(float inputValue, float currentVelocity)
        {
            if (Mathf.Abs(inputValue) < 0.01f)
            {
                return config.deceleration;
            }

            bool isTurningAround = (inputValue > 0 && currentVelocity < -0.1f) ||
                                   (inputValue < 0 && currentVelocity > 0.1f);

            if (isTurningAround)
            {
                return config.deceleration * config.turnAroundMultiplier;
            }

            return config.acceleration;
        }

        private void UpdateJump()
        {
            // Don't jump during attack
            if (attackController != null && attackController.IsAttacking)
            {
                return;
            }

            if (input.JumpBuffered && CanJump())
            {
                ExecuteJump();
                input.ConsumeJumpBuffer();
                return;
            }

            // Variable jump height
            if (isJumping && !input.JumpHeld && rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * config.jumpCutMultiplier);
                isJumping = false;
            }
        }

        private void ExecuteJump()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, config.jumpForce);
            isJumping = true;
            hasJumpedSinceGrounded = true;
        }

        private void UpdateGravityScale()
        {
            if (rb.linearVelocity.y < 0)
            {
                rb.gravityScale = config.gravityScale * config.fallGravityMultiplier;
            }
            else
            {
                rb.gravityScale = config.gravityScale;
            }
        }

        /// <summary>
        /// Reset movement state (on respawn).
        /// </summary>
        public void Reset()
        {
            rb.linearVelocity = Vector2.zero;
            isJumping = false;
            hasJumpedSinceGrounded = false;
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheckPoint == null || config == null) return;

            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawLine(
                groundCheckPoint.position,
                groundCheckPoint.position + Vector3.down * config.groundCheckDistance
            );
            Gizmos.DrawWireSphere(groundCheckPoint.position, 0.05f);
        }
    }
}
