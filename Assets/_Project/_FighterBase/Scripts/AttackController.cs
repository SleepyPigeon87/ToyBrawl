using System;
using System.Collections;
using UnityEngine;
using Brawler.Input;
using Brawler.Combat;
using Brawler.Core;

namespace Brawler.Fighter
{
    /// <summary>
    /// Default attack controller for fighters.
    /// Handles attack input, state machine, and hitbox activation.
    ///
    /// Attack flow:
    ///   1. Input detected (Attack button + direction)
    ///   2. Determine attack type (neutral, forward, up, down, aerial)
    ///   3. Start attack state machine (Startup -> Active -> Recovery)
    ///   4. Activate hitbox during Active phase
    ///   5. Return to Idle
    ///
    /// Students can use this as-is or implement their own attack system.
    /// </summary>
    public class AttackController : MonoBehaviour
    {
        [Header("Attack Assignments")]
        [Tooltip("Attack used with no directional input while grounded.")]
        [SerializeField] private AttackData neutralAttack;

        [Tooltip("Attack used with forward input while grounded.")]
        [SerializeField] private AttackData forwardAttack;

        [Tooltip("Attack used with up input while grounded.")]
        [SerializeField] private AttackData upAttack;

        [Tooltip("Attack used with down input while grounded.")]
        [SerializeField] private AttackData downAttack;

        [Tooltip("Default attack used in the air.")]
        [SerializeField] private AttackData aerialAttack;

        [Tooltip("Default grab.")]
        [SerializeField] private AttackData grabAction;

        [Header("Hitbox")]
        [Tooltip("The hitbox component used for attacks. If null, one will be created.")]
        [SerializeField] private Hitbox hitbox;

        [Header("Debug")]
        [SerializeField] private bool logAttacks = false;

        /// <summary>True if currently in any attack state.</summary>
        public bool IsAttacking => currentState != AttackState.Idle;

        /// <summary>Current attack being performed (null if not attacking).</summary>
        public AttackData CurrentAttack { get; private set; }

        /// <summary>Current attack state.</summary>
        public AttackState CurrentState => currentState;

        public event Action<AttackData> OnAttackStarted;
        public event Action<AttackData> OnAttackHitActive;
        public event Action OnAttackEnded;

        private PlayerInputHandler input;
        private FighterBase fighter;
        private FighterMovement movement;

        private AttackState currentState = AttackState.Idle;
        private Coroutine attackCoroutine;

        public enum AttackState
        {
            Idle,
            Startup,
            Active,
            Recovery
        }

        private void Awake()
        {
            movement = GetComponent<FighterMovement>();

            // Create hitbox if not assigned
            if (hitbox == null)
            {
                var hitboxObj = new GameObject("Hitbox");
                hitboxObj.transform.SetParent(transform);
                hitboxObj.transform.localPosition = Vector3.zero;
                hitboxObj.layer = gameObject.layer;
                hitboxObj.AddComponent<BoxCollider2D>();
                hitbox = hitboxObj.AddComponent<Hitbox>();
              
            }
        }

        /// <summary>
        /// Initialize with input handler and fighter reference.
        /// Called by FighterBase.
        /// </summary>
        public void Initialize(PlayerInputHandler inputHandler, FighterBase owner)
        {
            input = inputHandler;
            fighter = owner;

            if (hitbox == null)
            {
                Debug.LogError($"[AttackController] Hitbox is missing on {gameObject.name}! Bypassing attack setup to prevent a crash.");
                return;
            }

            hitbox.Initialize(owner);
        }

        private void Update()
        {
            if (input == null || fighter == null) return;
            if (!fighter.CanAct) return;

            // Don't process attacks during countdown or between rounds
            var gm = GameManager.Instance;
            if (gm != null && gm.CurrentState != GameState.Fighting && gm.CurrentState != GameState.Waiting)
                return;

            // Check for attack input
            if (input.AttackBuffered && !IsAttacking)
            {
                input.ConsumeAttackBuffer();
                TryAttack(DetermineAttackContext());
            }
        }

        /// <summary>
        /// Determine which attack context based on input and state.
        /// </summary>
        private AttackContext DetermineAttackContext()
        {
            bool isGrounded = movement != null ? movement.IsGrounded : fighter.IsGrounded;

            if (!isGrounded)
            {
                return AttackContext.AerialOnly;
            }

            Vector2 moveInput = input.MoveInput;

            // Check directional input
            if (moveInput.y > 0.5f)
            {
                return AttackContext.Up;
            }
            else if (moveInput.y < -0.5f)
            {
                return AttackContext.Down;
            }
            else if (Mathf.Abs(moveInput.x) > 0.5f)
            {
                return AttackContext.Forward;
            }

            return AttackContext.Neutral;
        }

        /// <summary>
        /// Attempt to perform an attack.
        /// </summary>
        public bool TryAttack(AttackContext context)
        {
            if (IsAttacking) return false;

            AttackData attack = GetAttackForContext(context);
            if (attack == null)
            {
                if (logAttacks)
                {
                    Debug.Log($"[AttackController] No attack assigned for context: {context}");
                }
                return false;
            }

            StartAttack(attack);
            return true;
        }

        private AttackData GetAttackForContext(AttackContext context)
        {
            return context switch
            {
                AttackContext.Neutral => neutralAttack,
                AttackContext.GroundedOnly => neutralAttack,
                AttackContext.Forward => forwardAttack ?? neutralAttack,
                AttackContext.Up => upAttack ?? neutralAttack,
                AttackContext.Down => downAttack ?? neutralAttack,
                AttackContext.AerialOnly => aerialAttack ?? neutralAttack,
                AttackContext.Any => neutralAttack,
                _ => neutralAttack
            };
        }

        private void StartAttack(AttackData attack)
        {
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }

            CurrentAttack = attack;
            attackCoroutine = StartCoroutine(AttackCoroutine(attack));

            if (logAttacks)
            {
                Debug.Log($"[AttackController] Starting attack: {attack.attackName}");
            }

            OnAttackStarted?.Invoke(attack);
        }

        private IEnumerator AttackCoroutine(AttackData attack)
        {
            // Startup phase
            currentState = AttackState.Startup;
            yield return new WaitForSeconds(attack.StartupTime);

            // Active phase - hitbox is active
            currentState = AttackState.Active;
            hitbox.Activate(attack);
            OnAttackHitActive?.Invoke(attack);

            yield return new WaitForSeconds(attack.ActiveTime);

            // Deactivate hitbox
            hitbox.Deactivate();

            // Recovery phase
            currentState = AttackState.Recovery;
            yield return new WaitForSeconds(attack.RecoveryTime);

            // Return to idle
            currentState = AttackState.Idle;
            CurrentAttack = null;
            attackCoroutine = null;

            OnAttackEnded?.Invoke();

            if (logAttacks)
            {
                Debug.Log($"[AttackController] Attack ended: {attack.attackName}");
            }
        }

        /// <summary>
        /// Cancel current attack (for interrupts or getting hit).
        /// </summary>
        public void CancelAttack()
        {
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }

            hitbox.Deactivate();
            currentState = AttackState.Idle;
            CurrentAttack = null;

            OnAttackEnded?.Invoke();
        }

        /// <summary>
        /// Reset attack state (on respawn).
        /// </summary>
        public void Reset()
        {
            CancelAttack();
        }
    }
}
