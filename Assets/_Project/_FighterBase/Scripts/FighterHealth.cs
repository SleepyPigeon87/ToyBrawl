using System;
using UnityEngine;
using Brawler.Core;

namespace Brawler.Fighter
{
    /// <summary>
    /// Manages fighter health and calculates knockback vulnerability.
    /// Lower health = more knockback taken (Smash Bros-style).
    ///
    /// DO NOT MODIFY - This ensures consistent knockback mechanics across all fighters.
    ///
    /// The knockback multiplier formula:
    ///   multiplier = 2 - (currentHealth / maxHealth)
    ///   At 100% health: 1.0x knockback
    ///   At 50% health:  1.5x knockback
    ///   At 0% health:   2.0x knockback
    /// </summary>
    public class FighterHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;

        [Header("Debug")]
        [SerializeField] private bool logDamage = false;

        /// <summary>Current health value.</summary>
        public float CurrentHealth { get; private set; }

        /// <summary>Maximum health value.</summary>
        public float MaxHealth => maxHealth;

        /// <summary>Health as a percentage (0.0 to 1.0).</summary>
        public float HealthPercent => Mathf.Clamp01(CurrentHealth / maxHealth);

        /// <summary>
        /// Knockback multiplier based on current health.
        /// Lower health = higher multiplier = more knockback.
        /// Range: 1.0 (full health) to 2.0 (zero health).
        /// </summary>
        public float KnockbackMultiplier => 2f - HealthPercent;

        /// <summary>True if health is zero or below.</summary>
        public bool IsDead => CurrentHealth <= 0f;

        /// <summary>Player index this health belongs to (set by FighterBase).</summary>
        public int PlayerIndex { get; private set; }

        // Events
        public event Action<float, float> OnHealthChanged;   // (oldHealth, newHealth)
        public event Action<float> OnDamageTaken;            // (damageAmount)
        public event Action OnDeath;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        /// <summary>
        /// Initialize health with a player index.
        /// Called by FighterBase during setup.
        /// </summary>
        public void Initialize(int playerIndex)
        {
            PlayerIndex = playerIndex;
            CurrentHealth = maxHealth;
        }

        /// <summary>
        /// Apply damage to this fighter.
        /// Fires OnDamageTaken and OnHealthChanged events.
        /// </summary>
        public void TakeDamage(float amount)
        {
            if (amount <= 0f) return;
            if (IsDead) return;

            float oldHealth = CurrentHealth;
            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);

            if (logDamage)
            {
                Debug.Log($"[FighterHealth P{PlayerIndex}] Took {amount} damage. " +
                          $"Health: {oldHealth} -> {CurrentHealth} " +
                          $"(Knockback multiplier: {KnockbackMultiplier:F2}x)");
            }

            OnDamageTaken?.Invoke(amount);
            OnHealthChanged?.Invoke(oldHealth, CurrentHealth);

            if (IsDead)
            {
                OnDeath?.Invoke();
            }
        }

        /// <summary>
        /// Heal this fighter.
        /// </summary>
        public void Heal(float amount)
        {
            if (amount <= 0f) return;

            float oldHealth = CurrentHealth;
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);

            if (oldHealth != CurrentHealth)
            {
                OnHealthChanged?.Invoke(oldHealth, CurrentHealth);
            }
        }

        /// <summary>
        /// Reset health to maximum.
        /// Called on respawn or round start.
        /// </summary>
        public void Reset()
        {
            float oldHealth = CurrentHealth;
            CurrentHealth = maxHealth;

            if (oldHealth != CurrentHealth)
            {
                OnHealthChanged?.Invoke(oldHealth, CurrentHealth);
            }
        }

        /// <summary>
        /// Set health to a specific value (for testing or special mechanics).
        /// </summary>
        public void SetHealth(float value)
        {
            float oldHealth = CurrentHealth;
            CurrentHealth = Mathf.Clamp(value, 0f, maxHealth);

            if (oldHealth != CurrentHealth)
            {
                OnHealthChanged?.Invoke(oldHealth, CurrentHealth);

                if (IsDead)
                {
                    OnDeath?.Invoke();
                }
            }
        }
    }
}
