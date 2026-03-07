using System;
using UnityEngine;

namespace Brawler.Fighter {
    public class MeterController : MonoBehaviour {
        public float currentMeter;
        public float CurrentMeter => currentMeter;

        [Header("Meter")]
        [SerializeField] public float maxMeter = 100f;

        public event Action<float> OnMeterChanged;

        public void AddMeter(float amount) {
            currentMeter = Mathf.Clamp(currentMeter + amount, 0f, maxMeter);
            OnMeterChanged?.Invoke(currentMeter);
        }

        public void ConsumeMeter(float amount) {
            currentMeter = Mathf.Clamp(currentMeter - amount, 0f, maxMeter);
            OnMeterChanged?.Invoke(currentMeter);
        }

        public bool HasEnoughMeter(float cost) {
            return currentMeter >= cost;
        }

    }
}
