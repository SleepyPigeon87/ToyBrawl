using System.Collections;
using UnityEngine;
using Brawler.Core;

namespace Brawler.UI {
    public class CameraShake : MonoBehaviour {

        [Header("Shake Settings")]
        [SerializeField] private float shakeDuration = 0.15f;
        [SerializeField] private float shakeMagnitude = 0.1f;

        private Vector3 originalPosition;

        private void Start() {
            originalPosition = transform.localPosition;
            GameEvents.OnFighterDamaged += OnFighterDamaged;
        }

        private void OnDestroy() {
            GameEvents.OnFighterDamaged -= OnFighterDamaged;
        }

        private void OnFighterDamaged(FighterDamageEventArgs args) {
            StopAllCoroutines();
            StartCoroutine(ShakeCoroutine());
        }

        private IEnumerator ShakeCoroutine() {
            float elapsed = 0f;

            while (elapsed < shakeDuration) {
                Vector3 offset = Random.insideUnitSphere * shakeMagnitude;
                transform.localPosition = originalPosition + new Vector3(offset.x, offset.y, 0f);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPosition;
        }
    }
}