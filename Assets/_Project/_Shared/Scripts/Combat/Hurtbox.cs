using UnityEngine;
using Brawler.Fighter;

namespace Brawler.Combat
{
    /// <summary>
    /// A hurtbox that receives hits from Hitboxes.
    /// Attach to the fighter's main collider or a dedicated hurtbox child object.
    ///
    /// When hit:
    ///   1. Applies damage to FighterHealth
    ///   2. Applies knockback via KnockbackHandler
    ///   3. Triggers hitstop if HitstopManager exists
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Hurtbox : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool drawGizmos = true;
        [SerializeField] private Color normalColor = new Color(0f, 1f, 0f, 0.3f);
        [SerializeField] private Color invincibleColor = new Color(0f, 0f, 1f, 0.3f);

        /// <summary>The fighter that owns this hurtbox.</summary>
        public FighterBase Owner { get; private set; }

        /// <summary>True when this hurtbox cannot be hit (respawn invincibility, etc.).</summary>
        public bool IsInvincible { get; private set; }

        private Collider2D hurtCollider;
        private FighterHealth health;
        private KnockbackHandler knockback;

        private void Awake()
        {
            hurtCollider = GetComponent<Collider2D>();
            hurtCollider.isTrigger = true;
        }

        private void Start()
        {
            // Find owner in parent hierarchy
            Owner = GetComponentInParent<FighterBase>();

            if (Owner != null)
            {
                health = Owner.GetComponent<FighterHealth>();
                knockback = Owner.GetComponent<KnockbackHandler>();
            }
            else
            {
                Debug.LogWarning("[Hurtbox] No FighterBase found in parent hierarchy!", this);
            }
        }

        /// <summary>
        /// Called by Hitbox when this hurtbox is hit.
        /// </summary>
        public void OnHit(Hitbox hitbox, AttackData attack, int attackerFacingDirection)
        {
            if (IsInvincible) return;
            if (Owner == null) return;
            if (Owner.IsRespawning) return;

            // Apply damage
            if (health != null)
            {
                health.TakeDamage(attack.damage);
            }

            // Apply knockback
            if (knockback != null)
            {
                knockback.ApplyKnockback(attack, attackerFacingDirection);
            }

            // Trigger hitstop
            if (HitstopManager.Instance != null && attack.hitstopDuration > 0f)
            {
                HitstopManager.Instance.TriggerHitstop(attack.hitstopDuration);
            }
        }

        /// <summary>
        /// Make this hurtbox invincible (cannot be hit).
        /// Used for respawn invincibility.
        /// </summary>
        public void SetInvincible(bool invincible)
        {
            IsInvincible = invincible;
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;
            if (hurtCollider == null) hurtCollider = GetComponent<Collider2D>();
            if (hurtCollider == null) return;

            Gizmos.color = IsInvincible ? invincibleColor : normalColor;

            if (hurtCollider is BoxCollider2D box)
            {
                Vector3 center = transform.position + (Vector3)box.offset;
                Vector3 size = box.size;
                Gizmos.DrawCube(center, size);
                Gizmos.color = IsInvincible ? Color.blue : Color.green;
                Gizmos.DrawWireCube(center, size);
            }
            else if (hurtCollider is CircleCollider2D circle)
            {
                Vector3 center = transform.position + (Vector3)(Vector2)circle.offset;
                Gizmos.DrawSphere(center, circle.radius);
            }
            else if (hurtCollider is CapsuleCollider2D capsule)
            {
                Vector3 center = transform.position + (Vector3)capsule.offset;
                Gizmos.DrawWireSphere(center, capsule.size.x / 2f);
            }
        }
    }
}
