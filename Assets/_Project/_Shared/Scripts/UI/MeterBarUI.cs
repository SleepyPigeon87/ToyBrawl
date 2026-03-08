using UnityEngine;
using UnityEngine.UI;
using Brawler.Fighter;

namespace Brawler.UI {
    public class MeterBarUI : MonoBehaviour {
        [Header("References - Wire in Inspector")]
        [SerializeField] private FighterBase fighter;

        [Header("UI Elements")]
        [SerializeField] private Image fillImage;
        [SerializeField] private Image backgroundImage;

        [Header("Colors")]
        [SerializeField] private Color emptyColor = Color.gray;
        [SerializeField] private Color filledColor = Color.yellow;
        [SerializeField] private Color fullColor = Color.cyan;

        [SerializeField] private float fullThreshold = 1f;
        [SerializeField] private float filledThreshold = 0.5f;

        [Header("Animation")]
        [SerializeField] private float lerpSpeed = 5f;

        private MeterController meter;
        private float targetFillAmount = 0f;

        private void Start() {
            if (fighter != null) {
                meter = fighter.GetComponent<MeterController>();
                if (meter != null) {
                    meter.OnMeterChanged += OnMeterChanged;
                    UpdateMeterBar(0f);
                }
            } else {
                Debug.LogWarning("[MeterBarUI] Fighter not assigned!", this);
            }
        }

        private void OnDestroy() {
            if (meter != null)
                meter.OnMeterChanged -= OnMeterChanged;
        }

        private void Update() {
            if (fillImage != null) {
                fillImage.fillAmount = Mathf.Lerp(
                    fillImage.fillAmount,
                    targetFillAmount,
                    Time.deltaTime * lerpSpeed
                );
            }
        }

        private void OnMeterChanged(float newValue) {
            float percent = newValue / meter.maxMeter;
            UpdateMeterBar(percent);
        }

        private void UpdateMeterBar(float percent) {
            targetFillAmount = percent;

            if (fillImage != null) {
                if (percent >= fullThreshold)
                    fillImage.color = fullColor;
                else if (percent >= filledThreshold)
                    fillImage.color = filledColor;
                else
                    fillImage.color = emptyColor;
            }
        }

        public void SetFighter(FighterBase newFighter) {
            if (meter != null)
                meter.OnMeterChanged -= OnMeterChanged;

            fighter = newFighter;
            meter = fighter?.GetComponent<MeterController>();

            if (meter != null) {
                meter.OnMeterChanged += OnMeterChanged;
                UpdateMeterBar(0f);
            }
        }
    }
}