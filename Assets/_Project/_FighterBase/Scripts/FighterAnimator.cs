using UnityEngine;
using Brawler.Combat;

namespace Brawler.Fighter
{
    /// <summary>
    /// Connects fighter state to Animator Controller.
    /// Updates animator parameters based on movement, attacks, and state.
    ///
    /// Expected Animator Parameters:
    ///   | Name            | Type  | Description                    |
    ///   |-----------------|-------|--------------------------------|
    ///   | HorizontalSpeed | Float | Absolute horizontal velocity   |
    ///   | VerticalSpeed   | Float | Vertical velocity (+up/-down)  |
    ///   | IsGrounded      | Bool  | True when on ground            |
    ///   | IsJumping       | Bool  | True during jump rise          |
    ///   | IsFalling       | Bool  | True when falling              |
    ///   | IsAttacking     | Bool  | True during attack             |
    ///   | IsHitstun       | Bool  | True during hitstun            |
    ///   | Attack          | Trigger | Triggered on attack start    |
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class FighterAnimator : MonoBehaviour
    {
        [Header("Flip Settings")]
        [Tooltip("Flip sprite based on facing direction.")]
        [SerializeField] private bool flipSprite = true;

        private Animator animator;
        private FighterBase fighter;
        private FighterMovement movement;
        private AttackController attackController;
        private KnockbackHandler knockbackHandler;

        // Cached parameter hashes
        private static readonly int HorizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
        private static readonly int VerticalSpeedHash = Animator.StringToHash("VerticalSpeed");
        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
        private static readonly int IsFallingHash = Animator.StringToHash("IsFalling");
        private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
        private static readonly int IsHitstunHash = Animator.StringToHash("IsHitstun");
        private static readonly int AttackHash = Animator.StringToHash("Attack");

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            fighter = GetComponentInParent<FighterBase>();
            movement = GetComponentInParent<FighterMovement>();
            attackController = GetComponentInParent<AttackController>();
            knockbackHandler = GetComponentInParent<KnockbackHandler>();

            // Subscribe to attack events if available
            if (attackController != null)
            {
                attackController.OnAttackStarted += OnAttackStarted;
            }
        }

        private void OnDestroy()
        {
            if (attackController != null)
            {
                attackController.OnAttackStarted -= OnAttackStarted;
            }
        }

        private void LateUpdate()
        {
            if (animator == null) return;

            UpdateMovementParameters();
            UpdateStateParameters();
            UpdateFacingDirection();
        }

        private void UpdateMovementParameters()
        {
            if (movement != null)
            {
                animator.SetFloat(HorizontalSpeedHash, Mathf.Abs(movement.HorizontalSpeed));
                animator.SetFloat(VerticalSpeedHash, movement.VerticalSpeed);
            }
            else if (fighter != null)
            {
                // Fallback if no movement component
                var rb = fighter.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    animator.SetFloat(HorizontalSpeedHash, Mathf.Abs(rb.linearVelocity.x));
                    animator.SetFloat(VerticalSpeedHash, rb.linearVelocity.y);
                }
            }
        }

        private void UpdateStateParameters()
        {
            // Ground state
            bool isGrounded = movement != null ? movement.IsGrounded : (fighter != null && fighter.IsGrounded);
            animator.SetBool(IsGroundedHash, isGrounded);

            // Jump/Fall states
            float verticalSpeed = movement != null ? movement.VerticalSpeed : 0f;
            bool isRising = verticalSpeed > 0.1f;
            bool isFalling = verticalSpeed < -0.1f && !isGrounded;

            animator.SetBool(IsJumpingHash, isRising);
            animator.SetBool(IsFallingHash, isFalling);

            // Attack state
            if (attackController != null)
            {
                animator.SetBool(IsAttackingHash, attackController.IsAttacking);
            }

            // Hitstun state
            if (knockbackHandler != null)
            {
                animator.SetBool(IsHitstunHash, knockbackHandler.IsInHitstun);
            }
        }

        private void UpdateFacingDirection()
        {
            if (!flipSprite || fighter == null) return;

            int facing = fighter.FacingDirection;
            Vector3 scale = transform.localScale;
            scale.x = facing * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        private void OnAttackStarted(AttackData attack)
        {
            animator.SetTrigger(AttackHash);

            // Use custom animation trigger if specified
            if (!string.IsNullOrEmpty(attack.animationTrigger) && attack.animationTrigger != "Attack")
            {
                animator.SetTrigger(attack.animationTrigger);
            }
        }

        /// <summary>
        /// Manually trigger an animation.
        /// </summary>
        public void TriggerAnimation(string triggerName)
        {
            animator.SetTrigger(triggerName);
        }

        /// <summary>
        /// Set a bool parameter on the animator.
        /// </summary>
        public void SetBool(string paramName, bool value)
        {
            animator.SetBool(paramName, value);
        }
    }
}
