using System.Collections;
using UnityEngine;
using TMPro;
using Brawler.Core;

namespace Brawler.UI
{
    /// <summary>
    /// Handles match-level UI: countdown, "GO!", round announcements, and winner display.
    ///
    /// SCAFFOLD - Students wire this to GameEvents.
    /// See Lesson 03: Wiring UI for step-by-step guide.
    /// </summary>
    public class MatchUI : MonoBehaviour
    {
        [Header("Announcement Text")]
        [Tooltip("Large center text for countdown, GO!, GAME!, etc.")]
        [SerializeField] private TextMeshProUGUI announcementText;

        [Header("Round Text")]
        [Tooltip("Shows 'Round 1', 'Round 2', etc.")]
        [SerializeField] private TextMeshProUGUI roundText;

        [Header("Timer (Optional)")]
        [Tooltip("Shows remaining match time.")]
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("Animation Settings")]
        #pragma warning disable CS0414 // Used by TODO coroutines when students wire UI
        [SerializeField] private float countdownDelay = 1f;
        [SerializeField] private float announcementDuration = 1.5f;
        #pragma warning restore CS0414

        [Header("Winner Panel")]
        [Tooltip("Panel shown when match ends.")]
        [SerializeField] private GameObject winnerPanel;
        [Tooltip("Text showing winner name.")]
        [SerializeField] private TextMeshProUGUI winnerText;

        private void Start()
        {
            // Hide announcements initially
            if (announcementText != null)
                announcementText.gameObject.SetActive(false);

            if (winnerPanel != null)
                winnerPanel.SetActive(false);

            // TODO STEP 1: Subscribe to game events
            // GameEvents.OnRoundStart += OnRoundStart;
            // GameEvents.OnGameStateChanged += OnGameStateChanged;
            // GameEvents.OnMatchEnd += OnMatchEnd;
        }

        private void OnDestroy()
        {
            // TODO: Unsubscribe from events
            // GameEvents.OnRoundStart -= OnRoundStart;
            // GameEvents.OnGameStateChanged -= OnGameStateChanged;
            // GameEvents.OnMatchEnd -= OnMatchEnd;
        }

        /// <summary>
        /// Called when a new round starts.
        /// TODO STEP 1: Subscribe to this event.
        /// </summary>
        private void OnRoundStart(int roundNumber)
        {
            // Update round text
            if (roundText != null)
            {
                roundText.text = $"Round {roundNumber}";
            }

            // TODO STEP 2: Start countdown coroutine
            // StartCoroutine(CountdownCoroutine());
        }

        /*
         * TODO STEP 2: Implement countdown coroutine
         *
        private IEnumerator CountdownCoroutine()
        {
            if (announcementText == null) yield break;

            announcementText.gameObject.SetActive(true);

            // 3
            announcementText.text = "3";
            yield return new WaitForSecondsRealtime(countdownDelay);

            // 2
            announcementText.text = "2";
            yield return new WaitForSecondsRealtime(countdownDelay);

            // 1
            announcementText.text = "1";
            yield return new WaitForSecondsRealtime(countdownDelay);

            // GO!
            announcementText.text = "GO!";
            yield return new WaitForSecondsRealtime(announcementDuration);

            announcementText.gameObject.SetActive(false);
        }
        */

        /// <summary>
        /// Called when game state changes.
        /// TODO STEP 1: Subscribe to this event.
        /// </summary>
        private void OnGameStateChanged(GameState newState)
        {
            // TODO STEP 3: Handle different states
            // switch (newState)
            // {
            //     case GameState.RoundEnd:
            //         // Show "KO!" or round winner
            //         break;
            //     case GameState.Paused:
            //         // Show pause menu
            //         break;
            // }
        }

        /// <summary>
        /// Called when match ends.
        /// TODO STEP 1: Subscribe to this event.
        /// </summary>
        private void OnMatchEnd(int winnerIndex)
        {
            // TODO STEP 4: Show winner panel
            // if (winnerPanel != null)
            //     winnerPanel.SetActive(true);

            // if (winnerText != null)
            //     winnerText.text = $"Player {winnerIndex + 1} Wins!";

            // if (announcementText != null)
            // {
            //     announcementText.gameObject.SetActive(true);
            //     announcementText.text = "GAME!";
            // }
        }

        /// <summary>
        /// Show a temporary announcement.
        /// </summary>
        public void ShowAnnouncement(string text, float duration = 1.5f)
        {
            StartCoroutine(ShowAnnouncementCoroutine(text, duration));
        }

        private IEnumerator ShowAnnouncementCoroutine(string text, float duration)
        {
            if (announcementText == null) yield break;

            announcementText.text = text;
            announcementText.gameObject.SetActive(true);

            yield return new WaitForSecondsRealtime(duration);

            announcementText.gameObject.SetActive(false);
        }
    }
}
