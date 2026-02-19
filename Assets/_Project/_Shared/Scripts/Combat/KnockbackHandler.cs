using System.Collections;
using UnityEngine;
using Brawler.Fighter;

namespace Brawler.Combat
{
    /// <summary>
    /// Handles applying knockback physics to a fighter.
    /// Uses FighterHealth to scale knockback based on damage taken.
    ///
    /// Knockback flow:
    ///   1. Attack hits via Hitbox -> Hurtbox collision
    ///   2. Hurtbox notifies KnockbackHandler
    ///   3. KnockbackHandler calculates knockback with health multiplier
    ///   4. Velocity is applied, hitstun begins
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(FighterHealth))]
    public class KnockbackHandler : MonoBehaviour
    {
        [Header("Hitstun Settings")]
        [Tooltip("Minimum hitstun duration regardless of knockback.")]
        [SerializeField] private float minimumHitstun = 0.1f;

        [Tooltip("Hitstun per unit of knockback force applied.")]
        [SerializeField] private float hitstunPerForce = 0.01f;

        [Tooltip("Maximum hitstun duration cap.")]
        [SerializeField] private float maximumHitstun = 1.5f;

        [Header("Knockback Settings")]
        [Tooltip("Base knockback multiplier for all attacks.")]
        [SerializeField] private float baseKnockbackMultiplier = 1f;

        [Header("Debug")]
        [SerializeField] private bool logKnockback = false;

        /// <summary>True while in hitstun (cannot act).</summary>
        public bool IsInHitstun { get; private set; }

        /// <summary>Remaining hitstun time in seconds.</summary>
        public float HitstunRemaining { get; private set; }

        /// <summary>The knockback velocity from the last hit.</summary>
        public Vector2 LastKnockbackVelocity { get; private set; }

        private Rigidbody2D rb;
        private FighterHealth health;
        private Coroutine hitstunCoroutine;
        private float originalGravityScale;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<FighterHealth>();
            originalGravityScale = rb.gravityScale;
        }

        /// <summary>
        /// Apply knockback to this fighter.
        /// Called when hit by an attack.
        /// </summary>
        /// <param name="direction">Direction of knockback (will be normalized).</param>
        /// <param name="baseForce">Base knockback force before health scaling.</param>
        /// <param name="hitstunDuration">Base hitstun duration. If 0, calculated from force.</param>
        public void ApplyKnockback(Vector2 direction, float baseForce, float hitstunDuration = 0f)
        {
            // Calculate knockback with health-based multiplier
            float healthMultiplier = health.KnockbackMultiplier;
            float finalForce = baseForce * healthMultiplier * baseKnockbackMultiplier;

            // Apply velocity
            Vector2 knockback = direction.normalized * finalForce;
            rb.linearVelocity = knockback;
            LastKnockbackVelocity = knockback;

            // Calculate hitstun
            float calculatedHitstun = hitstunDuration > 0f
                ? hitstunDuration
                : Mathf.Clamp(finalForce * hitstunPerForce, minimumHitstun, maximumHitstun);

            // Start hitstun
            StartHitstun(calculatedHitstun);

            if (logKnockback)
            {
                Debug.Log($"[KnockbackHandler P{health.PlayerIndex}] " +
                          $"Knockback: {knockback.magnitude:F1} (base {baseForce:F1} x health {healthMultiplier:F2}x) " +
                          $"Hitstun: {calculatedHitstun:F2}s");
            }
        }

        /// <summary>
        /// Apply knockback from attack data.
        /// Convenience method for common use case.
        /// </summary>
        public void ApplyKnockback(AttackData attackData, int attackerFacingDirection)
        {
            // Calculate knockback direction based on attacker facing
            Vector2 direction = attackData.knockbackAngle;
            direction.x *= attackerFacingDirection;

            ApplyKnockback(direction, attackData.baseKnockback, attackData.hitstunDuration);
        }

        private void StartHitstun(float duration)
        {
            if (hitstunCoroutine != null)
            {
                StopCoroutine(hitstunCoroutine);
            }

            hitstunCoroutine = StartCoroutine(HitstunCoroutine(duration));
        }

        private IEnumerator HitstunCoroutine(float duration)
        {
            IsInHitstun = true;
            HitstunRemaining = duration;

            while (HitstunRemaining > 0f)
            {
                HitstunRemaining -= Time.deltaTime;
                yield return null;
            }

            HitstunRemaining = 0f;
            IsInHitstun = false;
            hitstunCoroutine = null;
        }

        /// <summary>
        /// Immediately end hitstun (for testing or special mechanics).
        /// </summary>
        public void EndHitstun()
        {
            if (hitstunCoroutine != null)
            {
                StopCoroutine(hitstunCoroutine);
                hitstunCoroutine = null;
            }

            IsInHitstun = false;
            HitstunRemaining = 0f;
        }

        /// <summary>
        /// Reset knockback state (on respawn).
        /// </summary>
        public void Reset()
        {
            EndHitstun();
            rb.linearVelocity = Vector2.zero;
            LastKnockbackVelocity = Vector2.zero;
        }
    }
}
