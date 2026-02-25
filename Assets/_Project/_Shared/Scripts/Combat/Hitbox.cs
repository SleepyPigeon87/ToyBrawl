using UnityEngine;
using System.Collections.Generic;
using Brawler.Fighter;

namespace Brawler.Combat
{
    /// <summary>
    /// A hitbox that deals damage when it overlaps with a Hurtbox.
    /// Attach to attack GameObjects (child of fighter or spawned during attack).
    ///
    /// How it works:
    ///   1. AttackController enables hitbox during active frames
    ///   2. Hitbox detects Hurtbox via trigger collision
    ///   3. Hitbox notifies Hurtbox of the hit
    ///   4. Hurtbox applies damage and knockback
    ///
    /// The hitbox tracks which fighters it has already hit to prevent
    /// the same attack from hitting multiple times.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Hitbox : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The attack data this hitbox uses. Set by AttackController.")]
        [SerializeField] private AttackData attackData;

        [Header("Debug")]
        [SerializeField] private bool drawGizmos = true;
        [SerializeField] private Color activeColor = new Color(1f, 0f, 0f, 0.5f);
        [SerializeField] private Color inactiveColor = new Color(0.5f, 0f, 0f, 0.2f);

        /// <summary>The opponent currently being held. Null if not holding anyone.</summary>
        public FighterBase HeldOpponent { get; private set; }

        /// <summary>The fighter that owns this hitbox.</summary>
        public FighterBase Owner { get; private set; }

        /// <summary>Current attack data.</summary>
        public AttackData AttackData => attackData;

        /// <summary>True when the hitbox can deal damage.</summary>
        public bool IsActive { get; private set; }

        private Collider2D hitCollider;
        private HashSet<int> hitFighters = new HashSet<int>();

        private void Awake()
        {
            hitCollider = GetComponent<Collider2D>();
            hitCollider.isTrigger = true;
            Deactivate();
        }

        /// <summary>
        /// Initialize the hitbox with an owner fighter.
        /// </summary>
        public void Initialize(FighterBase owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// Activate the hitbox with the given attack data.
        /// </summary>
        public void Activate(AttackData attack)
        {
            attackData = attack;
            IsActive = true;
            hitCollider.enabled = true;
            hitFighters.Clear();

            // Position hitbox based on attack data and facing direction
            if (Owner != null && attack != null)
            {
                Vector2 offset = attack.hitboxOffset;
                offset.x *= Owner.FacingDirection;
                transform.localPosition = offset;

                // Set size if using BoxCollider2D
                if (hitCollider is BoxCollider2D box)
                {
                    box.size = attack.hitboxSize;
                }
            }
        }

        /// <summary>
        /// Deactivate the hitbox.
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
            hitCollider.enabled = false;
            attackData = null;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!IsActive || attackData == null) return;

            var hurtbox = other.GetComponent<Hurtbox>();
            if (hurtbox == null) return;
            if (hurtbox.Owner == null || hurtbox.Owner == Owner) return;

            int targetId = hurtbox.Owner.GetInstanceID();
            if (hitFighters.Contains(targetId)) return;
            hitFighters.Add(targetId);
            if (attackData.isGrab) {
                // 1. Tell the hurtbox it got grabbed (freezes the defender)
                hurtbox.OnGrabbed(Owner);

                // 2. Look for the AttackController on the Attacker and trigger the success method
                var attackerController = Owner.GetComponent<AttackController>();
                if (attackerController != null) {
                    attackerController.OnGrabSuccess(hurtbox.Owner);
                }

                Debug.Log($"{Owner.name} successfully GRABBED {hurtbox.Owner.name}!");
            } else {
                hurtbox.OnHit(this, attackData, Owner.FacingDirection);
                Owner.OnAttackHit(hurtbox.Owner, attackData);
            }
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;

            Gizmos.color = IsActive ? activeColor : inactiveColor;

            if (hitCollider is BoxCollider2D box)
            {
                Vector3 center = transform.position + (Vector3)box.offset;
                Vector3 size = box.size;
                Gizmos.DrawCube(center, size);
                Gizmos.color = IsActive ? Color.red : Color.gray;
                Gizmos.DrawWireCube(center, size);
            }
            else if (hitCollider is CircleCollider2D circle)
            {
                Vector3 center = transform.position + (Vector3)(Vector2)circle.offset;
                Gizmos.DrawSphere(center, circle.radius);
            }
        }
    }
}
