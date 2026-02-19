using UnityEngine;
using Brawler.Combat;

namespace Brawler.Fighter
{
    /// <summary>
    /// Example fighter implementation showing how to extend FighterBase.
    ///
    /// This is a complete, working fighter that students can:
    ///   1. Use as a reference for their own fighters
    ///   2. Play against while developing
    ///   3. Copy as a starting point
    ///
    /// To create your own fighter:
    ///   1. Create a new script in your Fighter1/ or Fighter2/ folder
    ///   2. Extend FighterBase
    ///   3. Override the abstract FighterName property
    ///   4. Optionally override virtual methods for custom behavior
    /// </summary>
    public class ExampleFighter : FighterBase
    {
        [Header("Example Fighter Settings")]
        [SerializeField] private string fighterName = "Example";

        [Tooltip("Custom color tint for this fighter.")]
        [SerializeField] private Color fighterColor = Color.white;

        public override string FighterName => fighterName;

        private SpriteRenderer spriteRenderer;

        protected override void Awake()
        {
            base.Awake();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        protected override void OnFighterInitialized()
        {
            // Apply color tint
            if (spriteRenderer != null)
            {
                spriteRenderer.color = fighterColor;
            }

            Debug.Log($"[{FighterName}] Initialized as Player {PlayerIndex + 1}");
        }

        protected override void OnTakeDamage(float damage)
        {
            Debug.Log($"[{FighterName}] Took {damage} damage! Health: {Health.CurrentHealth}/{Health.MaxHealth}");

            // Flash red briefly
            if (spriteRenderer != null)
            {
                StartCoroutine(DamageFlash());
            }
        }

        private System.Collections.IEnumerator DamageFlash()
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }

        protected override void OnKO()
        {
            Debug.Log($"[{FighterName}] KO'd!");
        }

        protected override void OnRespawn(Vector2 position)
        {
            Debug.Log($"[{FighterName}] Respawned at {position}");

            // Brief invincibility flash
            if (spriteRenderer != null)
            {
                StartCoroutine(RespawnFlash());
            }
        }

        private System.Collections.IEnumerator RespawnFlash()
        {
            float duration = 2f;
            float flashRate = 0.1f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(flashRate);
                elapsed += flashRate;
            }

            spriteRenderer.enabled = true;
            EndRespawnInvincibility();
        }
    }
}
