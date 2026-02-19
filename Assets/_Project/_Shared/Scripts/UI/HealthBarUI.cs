using UnityEngine;
using UnityEngine.UI;
using Brawler.Fighter;

namespace Brawler.UI
{
    /// <summary>
    /// Displays a health bar for a fighter.
    ///
    /// SCAFFOLD - Students wire this to FighterHealth.
    /// See Lesson 03: Wiring UI for step-by-step guide.
    ///
    /// How it should work:
    ///   1. Subscribe to FighterHealth.OnHealthChanged
    ///   2. Update fill amount based on health percent
    ///   3. Optional: Flash/shake on damage
    /// </summary>
    public class HealthBarUI : MonoBehaviour
    {
        [Header("References - Wire in Inspector")]
        [Tooltip("The fighter this health bar displays.")]
        [SerializeField] private FighterBase fighter;

        [Header("UI Elements")]
        [Tooltip("The fill image (set type to Filled).")]
        [SerializeField] private Image fillImage;

        [Tooltip("Optional background image.")]
        [SerializeField] private Image backgroundImage;

        [Header("Colors")]
        [SerializeField] private Color healthyColor = Color.green;
        [SerializeField] private Color damagedColor = Color.yellow;
        [SerializeField] private Color criticalColor = Color.red;

        [Tooltip("Health percent below which bar turns critical.")]
        [SerializeField] private float criticalThreshold = 0.25f;

        [Tooltip("Health percent below which bar turns damaged.")]
        [SerializeField] private float damagedThreshold = 0.5f;

        [Header("Animation")]
        [Tooltip("How fast the bar lerps to target value.")]
        [SerializeField] private float lerpSpeed = 5f;

        private FighterHealth health;
        private float targetFillAmount = 1f;

        private void Start()
        {
            if (fighter != null)
            {
                health = fighter.GetComponent<FighterHealth>();

                // TODO STEP 1: Subscribe to health changes
                // health.OnHealthChanged += OnHealthChanged;

                // Initialize
                UpdateHealthBar(health.HealthPercent);
            }
            else
            {
                Debug.LogWarning("[HealthBarUI] Fighter not assigned!", this);
            }
        }

        private void OnDestroy()
        {
            // TODO: Unsubscribe
            // if (health != null)
            // {
            //     health.OnHealthChanged -= OnHealthChanged;
            // }
        }

        private void Update()
        {
            // Smooth lerp to target
            if (fillImage != null)
            {
                fillImage.fillAmount = Mathf.Lerp(
                    fillImage.fillAmount,
                    targetFillAmount,
                    Time.deltaTime * lerpSpeed
                );
            }
        }

        /// <summary>
        /// Called when health changes.
        /// TODO STEP 1: Subscribe to this event.
        /// </summary>
        private void OnHealthChanged(float oldHealth, float newHealth)
        {
            if (health == null) return;

            float percent = health.HealthPercent;
            UpdateHealthBar(percent);

            // TODO STEP 2: Add visual feedback
            // - Flash the bar
            // - Shake effect
            // - Damage numbers
        }

        private void UpdateHealthBar(float percent)
        {
            targetFillAmount = percent;

            // Update color based on health level
            if (fillImage != null)
            {
                if (percent <= criticalThreshold)
                {
                    fillImage.color = criticalColor;
                }
                else if (percent <= damagedThreshold)
                {
                    fillImage.color = damagedColor;
                }
                else
                {
                    fillImage.color = healthyColor;
                }
            }
        }

        /// <summary>
        /// Set the fighter this health bar displays.
        /// Call this if assigning at runtime.
        /// </summary>
        public void SetFighter(FighterBase newFighter)
        {
            // Unsubscribe from old
            // if (health != null)
            // {
            //     health.OnHealthChanged -= OnHealthChanged;
            // }

            fighter = newFighter;
            health = fighter?.GetComponent<FighterHealth>();

            // Subscribe to new
            // if (health != null)
            // {
            //     health.OnHealthChanged += OnHealthChanged;
            //     UpdateHealthBar(health.HealthPercent);
            // }
        }
    }
}
