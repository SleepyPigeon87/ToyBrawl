using System.Collections;
using UnityEngine;

namespace Brawler.Combat
{
    /// <summary>
    /// Manages hitstop (freeze frames) when attacks connect.
    /// Hitstop is the brief pause that makes hits feel impactful.
    ///
    /// Usage:
    ///   HitstopManager.Instance.TriggerHitstop(0.05f);
    ///
    /// This pauses the game by setting Time.timeScale to 0 briefly.
    /// Objects that should ignore hitstop can check HitstopManager.IsInHitstop.
    /// </summary>
    public class HitstopManager : MonoBehaviour
    {
        public static HitstopManager Instance { get; private set; }

        [Header("Settings")]
        [Tooltip("Default hitstop duration if none specified.")]
        [SerializeField] private float defaultHitstopDuration = 0.05f;

        [Tooltip("Multiply attack hitstop values by this. Use to globally adjust game feel.")]
        [SerializeField] private float hitstopMultiplier = 1f;

        [Header("Debug")]
        [SerializeField] private bool logHitstop = false;

        /// <summary>True while hitstop is active.</summary>
        public bool IsInHitstop { get; private set; }

        /// <summary>Remaining hitstop time.</summary>
        public float HitstopRemaining { get; private set; }

        private Coroutine hitstopCoroutine;
        private float originalTimeScale;
        private float originalFixedDeltaTime;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[HitstopManager] Multiple instances detected. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            originalTimeScale = Time.timeScale;
            originalFixedDeltaTime = Time.fixedDeltaTime;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;

                // Restore time scale if we're destroyed during hitstop
                if (IsInHitstop)
                {
                    RestoreTimeScale();
                }
            }
        }

        /// <summary>
        /// Trigger a hitstop (freeze frame) for the specified duration.
        /// </summary>
        public void TriggerHitstop(float duration = -1f)
        {
            float actualDuration = (duration < 0f ? defaultHitstopDuration : duration) * hitstopMultiplier;

            if (actualDuration <= 0f) return;

            if (hitstopCoroutine != null)
            {
                StopCoroutine(hitstopCoroutine);
            }

            hitstopCoroutine = StartCoroutine(HitstopCoroutine(actualDuration));

            if (logHitstop)
            {
                Debug.Log($"[HitstopManager] Hitstop triggered: {actualDuration:F3}s");
            }
        }

        private IEnumerator HitstopCoroutine(float duration)
        {
            IsInHitstop = true;
            HitstopRemaining = duration;

            // Store current time scale (in case it's already modified)
            originalTimeScale = Time.timeScale;
            originalFixedDeltaTime = Time.fixedDeltaTime;

            // Freeze time
            Time.timeScale = 0f;

            // Wait for duration using unscaled time
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                HitstopRemaining = duration - elapsed;
                yield return null;
            }

            // Restore time
            RestoreTimeScale();

            IsInHitstop = false;
            HitstopRemaining = 0f;
            hitstopCoroutine = null;
        }

        private void RestoreTimeScale()
        {
            Time.timeScale = originalTimeScale > 0f ? originalTimeScale : 1f;
            Time.fixedDeltaTime = originalFixedDeltaTime > 0f ? originalFixedDeltaTime : 0.02f;
        }

        /// <summary>
        /// Cancel any active hitstop immediately.
        /// </summary>
        public void CancelHitstop()
        {
            if (hitstopCoroutine != null)
            {
                StopCoroutine(hitstopCoroutine);
                hitstopCoroutine = null;
            }

            if (IsInHitstop)
            {
                RestoreTimeScale();
            }

            IsInHitstop = false;
            HitstopRemaining = 0f;
        }
    }
}
