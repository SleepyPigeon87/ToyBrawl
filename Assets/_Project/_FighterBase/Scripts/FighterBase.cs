using UnityEngine;
using Brawler.Core;
using Brawler.Combat;
using Brawler.Input;

namespace Brawler.Fighter
{
    /// <summary>
    /// Base class for all fighters in the brawler.
    /// Students extend this class to create their own fighters.
    ///
    /// DO NOT MODIFY this class - extend it instead.
    ///
    /// Required components (added automatically):
    ///   - FighterHealth: Health and knockback vulnerability
    ///   - KnockbackHandler: Applies knockback physics
    ///   - Rigidbody2D: Physics
    ///   - Collider2D: Collision detection
    ///
    /// Optional components:
    ///   - FighterMovement: Default movement (or implement your own)
    ///   - AttackController: Default attacks (or implement your own)
    ///   - FighterAnimator: Animation handling
    ///
    /// To create your own fighter:
    ///   1. Create a new script that extends FighterBase
    ///   2. Override the abstract properties and methods
    ///   3. Add your own movement/attack logic or use the defaults
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(FighterHealth))]
    [RequireComponent(typeof(KnockbackHandler))]
    public abstract class FighterBase : MonoBehaviour
    {
        [Header("Fighter Identity")]
        [Tooltip("Player index (0 = Player 1, 1 = Player 2). Set by GameManager.")]
        [SerializeField] protected int playerIndex = 0;

        /// <summary>Display name for this fighter.</summary>
        public abstract string FighterName { get; }

        /// <summary>Player index (0 or 1).</summary>
        public int PlayerIndex => playerIndex;

        /// <summary>Which direction the fighter is facing (1 = right, -1 = left).</summary>
        public int FacingDirection { get; protected set; } = 1;

        /// <summary>True if the fighter is on the ground.</summary>
        public bool IsGrounded { get; protected set; }

        /// <summary>True if the fighter can currently act (not in hitstun, etc.).</summary>
        public bool CanAct => !Knockback.IsInHitstun && !IsDead && !IsRespawning && !IsGrabbed;

        /// <summary>True if health is zero.</summary>
        public bool IsDead => Health.IsDead;

        /// <summary>True during respawn invincibility.</summary>
        public bool IsRespawning { get; protected set; }

        //True if fighter is being held
        public bool IsGrabbed { get; protected set; }

        // Component references
        protected Rigidbody2D Rb { get; private set; }
        protected FighterHealth Health { get; private set; }
        protected KnockbackHandler Knockback { get; private set; }
        protected PlayerInputHandler Input { get; private set; }

        // Optional components (may be null)
        protected FighterMovement Movement { get; private set; }
        protected AttackController Attacks { get; private set; }
        protected FighterAnimator Animator { get; private set; }

        protected virtual void Awake()
        {
            // Get required components
            Rb = GetComponent<Rigidbody2D>();
            Health = GetComponent<FighterHealth>();
            Knockback = GetComponent<KnockbackHandler>();

            // Get optional components
            Movement = GetComponent<FighterMovement>();
            Attacks = GetComponent<AttackController>();
            Animator = GetComponent<FighterAnimator>();
            Input = GetComponent<PlayerInputHandler>();

            // Configure rigidbody for fighting game physics
            Rb.freezeRotation = true;
            Rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            Rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        protected virtual void Start()
        {
            // Initialize components with player index
            Health.Initialize(playerIndex);

            // Subscribe to events
            Health.OnDamageTaken += OnDamageTakenInternal;
            Health.OnDeath += OnDeathInternal;
        }

        protected virtual void OnDestroy()
        {
            // Unsubscribe from events
            if (Health != null)
            {
                Health.OnDamageTaken -= OnDamageTakenInternal;
                Health.OnDeath -= OnDeathInternal;
            }
        }

        /// <summary>
        /// Initialize the fighter for a match.
        /// Called by GameManager when setting up fighters.
        /// </summary>
        public virtual void Initialize(int index, PlayerInputHandler inputHandler)
        {
            playerIndex = index;
            Input = inputHandler;

            Health.Initialize(index);

            // Wire input to optional components
            if (Movement != null)
            {
                Movement.Initialize(inputHandler);
            }

            if (Attacks != null)
            {
                Attacks.Initialize(inputHandler, this);
            }

            OnFighterInitialized();
        }

        /// <summary>
        /// Called after the fighter is fully initialized.
        /// Override to perform custom setup.
        /// </summary>
        protected virtual void OnFighterInitialized() { }

        /// <summary>
        /// Called every frame. Override for custom update logic.
        /// Default implementation updates facing direction.
        /// </summary>
        protected virtual void Update()
        {
            if (!CanAct) return;

            UpdateFacingDirection();
        }

        /// <summary>
        /// Updates which direction the fighter is facing based on input.
        /// Override to customize facing behavior.
        /// </summary>
        protected virtual void UpdateFacingDirection()
        {
            if (Input == null) return;

            float inputX = Input.MoveInput.x;
            if (Mathf.Abs(inputX) > 0.1f)
            {
                FacingDirection = inputX > 0 ? 1 : -1;
            }
        }

        /// <summary>
        /// Respawn the fighter at the given position.
        /// Called by GameManager after a KO.
        /// </summary>
        public virtual void Respawn(Vector2 position)
        {
            transform.position = position;
            Rb.linearVelocity = Vector2.zero;
            Health.Reset();
            Knockback.Reset();

            IsRespawning = true;
            OnRespawn(position);

            // Fire respawn event
            GameEvents.OnFighterRespawn?.Invoke(new FighterRespawnEventArgs
            {
                PlayerIndex = playerIndex,
                SpawnPosition = position
            });
        }

        /// <summary>
        /// End respawn invincibility.
        /// Called after respawn delay by GameManager or automatically.
        /// </summary>
        public virtual void EndRespawnInvincibility()
        {
            IsRespawning = false;
        }

        private void OnDamageTakenInternal(float damage)
        {
            OnTakeDamage(damage);

            // Fire damage event
            GameEvents.OnFighterDamaged?.Invoke(new FighterDamageEventArgs
            {
                PlayerIndex = playerIndex,
                Damage = damage,
                NewHealth = Health.CurrentHealth,
                KnockbackForce = Knockback.LastKnockbackVelocity.magnitude,
                KnockbackDirection = Knockback.LastKnockbackVelocity.normalized
            });
        }

        private void OnDeathInternal()
        {
            OnKO();
        }

        // Override these methods to add custom behavior

        /// <summary>Called when the fighter takes damage.</summary>
        protected virtual void OnTakeDamage(float damage) { }

        /// <summary>Called when the fighter is KO'd (enters blast zone or health depleted).</summary>
        protected virtual void OnKO() { }

        /// <summary>Called when the fighter respawns.</summary>
        protected virtual void OnRespawn(Vector2 position) { }

        /// <summary>
        /// Handle an attack input.
        /// Override if not using AttackController.
        /// </summary>
        public virtual void OnAttackInput(AttackContext context)
        {
            if (Attacks != null)
            {
                Attacks.TryAttack(context);
            }
        }

        /// <summary>
        /// Get the current attack hitbox data (for collision detection).
        /// Override if using custom attack system.
        /// </summary>
        public virtual AttackData GetCurrentAttack()
        {
            return Attacks?.CurrentAttack;
        }

        /// <summary>
        /// Called when this fighter's attack hits an opponent.
        /// </summary>
        public virtual void OnAttackHit(FighterBase opponent, AttackData attack)
        {
            // Default: trigger hitstop
            if (HitstopManager.Instance != null)
            {
                HitstopManager.Instance.TriggerHitstop(attack.hitstopDuration);
            }
        }

        /// <summary>
        /// Called when an opponent successfully lands a grab attack on this fighter.
        /// </summary>
        public virtual void GetGrabbedBy(FighterBase attacker) {
            IsGrabbed = true;
            if (Attacks != null) Attacks.CancelAttack();
            Rb.linearVelocity = Vector2.zero;

            //Float in hands, turn grav back on later
            // Rb.gravityScale = 0f; 

            //Snap to attacker's position
            float grabDistance = 1.0f;
            Vector2 holdPosition = attacker.transform.position;
            holdPosition.x += (grabDistance * attacker.FacingDirection);
            transform.position = holdPosition;

            //Fire event Animations!)
            OnGrabbedInternal(attacker);
        }

        /// <summary>Called when the fighter is grabbed by an opponent.</summary>
        protected virtual void OnGrabbedInternal(FighterBase attacker) {
        
        
        }

        /// <summary>
        /// Releases the fighter from a grabbed state.
        /// </summary>
        public virtual void ReleaseGrab() {
            IsGrabbed = false;

            // If you disabled gravity in GetGrabbedBy, turn it back to 1f here!
            // Rb.gravityScale = 1f;
        }
    }
}
