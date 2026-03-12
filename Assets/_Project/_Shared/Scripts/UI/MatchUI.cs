using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Brawler.Core;
using Brawler.Fighter;

namespace Brawler.UI {
    /// <summary>
    /// Handles match-level UI: countdown, "GO!", round announcements, and winner display.
    ///
    /// SCAFFOLD - Students wire this to GameEvents.
    /// See Lesson 03: Wiring UI for step-by-step guide.
    /// </summary>
    public class MatchUI : MonoBehaviour {
        [Header("Announcement Text")]
        [Tooltip("Large center text for countdown, GO!, GAME!, etc.")]
        [SerializeField] private TextMeshProUGUI announcementText;

        [Header("Round Image")]
        [Tooltip("Sprites for each round, index 0 = Round 1, index 1 = Round 2, etc.")]
        [SerializeField] private Image roundImage;
        [SerializeField] private Sprite[] roundSprites;

        [Header("KO Announcement")]
        [SerializeField] private TextMeshProUGUI koText;

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

        [SerializeField] private Button restartButton;

        private float remainingTime;
        private bool timerRunning = false;

        private void Start() {
            // Hide announcements initially
            if (announcementText != null)
                announcementText.gameObject.SetActive(false);

            if (winnerPanel != null)
                winnerPanel.SetActive(false);

            if (timerText != null)
                timerText.gameObject.SetActive(false);

            if (koText != null)
                koText.gameObject.SetActive(false);

            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);

            // TODO STEP 1: Subscribe to game events
            GameEvents.OnRoundStart += OnRoundStart;
            GameEvents.OnGameStateChanged += OnGameStateChanged;
            GameEvents.OnMatchEnd += OnMatchEnd;
            GameEvents.OnFighterKO += OnFighterKO;
        }

        private void OnDestroy() {
            // TODO: Unsubscribe from events
            GameEvents.OnRoundStart -= OnRoundStart;
            GameEvents.OnGameStateChanged -= OnGameStateChanged;
            GameEvents.OnMatchEnd -= OnMatchEnd;
            GameEvents.OnFighterKO -= OnFighterKO;
        }

        private void Update() {
            if (!timerRunning) return;
            remainingTime -= Time.deltaTime;
            if (timerText != null)
                timerText.text = Mathf.CeilToInt(remainingTime).ToString();
            if (remainingTime <= 0) {
                timerRunning = false;
                HandleTimeout();
            }
        }

        /// <summary>
        /// Called when a new round starts.
        /// TODO STEP 1: Subscribe to this event.
        /// </summary>
        private void OnRoundStart(int roundNumber) {
            StartCoroutine(CountdownCoroutine(roundNumber));
        }

        private IEnumerator CountdownCoroutine(int roundNumber) {
            if (announcementText == null) yield break;

            // Show round banner
            int spriteIndex = roundNumber - 1;
            if (roundImage != null && spriteIndex < roundSprites.Length) {
                roundImage.sprite = roundSprites[spriteIndex];
                roundImage.gameObject.SetActive(true);
                yield return new WaitForSecondsRealtime(announcementDuration);
                roundImage.gameObject.SetActive(false);
            }

            // Countdown
            announcementText.gameObject.SetActive(true);

            announcementText.text = "3";
            yield return new WaitForSecondsRealtime(countdownDelay);

            announcementText.text = "2";
            yield return new WaitForSecondsRealtime(countdownDelay);

            announcementText.text = "1";
            yield return new WaitForSecondsRealtime(countdownDelay);

            announcementText.text = "GO!";
            yield return new WaitForSecondsRealtime(announcementDuration);

            announcementText.gameObject.SetActive(false);
            timerText.gameObject.SetActive(true);
            remainingTime = 99f;
            timerRunning = true;
        }

        private void HandleTimeout() {
            var gm = GameManager.Instance;
            if (gm == null) return;

            var health0 = gm.GetFighter(0).GetComponent<FighterHealth>();
            var health1 = gm.GetFighter(1).GetComponent<FighterHealth>();

            int winner = health0.CurrentHealth >= health1.CurrentHealth ? 0 : 1;
            gm.EndMatch(winner);
        }

        /// <summary>
        /// Called when game state changes.
        /// TODO STEP 1: Subscribe to this event.
        /// </summary>
        private void OnGameStateChanged(GameState newState) {
            switch (newState) {
                case GameState.Fighting: 
                    break;
                case GameState.RoundEnd:
                case GameState.MatchEnd:
                case GameState.Paused:
                    timerRunning = false;
                    break;
            }
        }

        private void OnFighterKO(FighterKOEventArgs args) {
            StartCoroutine(KOCoroutine());
        }

        private IEnumerator KOCoroutine() {
            if (koText == null) yield break;

            koText.gameObject.SetActive(true);
            koText.text = "KO!";
            yield return new WaitForSecondsRealtime(announcementDuration);
            koText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Called when match ends.
        /// TODO STEP 1: Subscribe to this event.
        /// </summary>
        private void OnMatchEnd(int winnerIndex) {
            // TODO STEP 4: Show winner panel
            if (winnerPanel != null)
                winnerPanel.SetActive(true);

            if (winnerText != null)
                winnerText.text = $"Player {winnerIndex + 1} Wins!";

            if (announcementText != null) {
                announcementText.gameObject.SetActive(true);
                announcementText.text = "GAME!";
            }
        }

        private void OnRestartClicked() {
            if (winnerPanel != null)
                winnerPanel.SetActive(false);

            if (announcementText != null)
                announcementText.gameObject.SetActive(false);

            timerText.gameObject.SetActive(false);
            timerRunning = false;

            GameManager.Instance.StartMatch();
        }

        /// <summary>
        /// Show a temporary announcement.
        /// </summary>
        public void ShowAnnouncement(string text, float duration = 1.5f) {
            StartCoroutine(ShowAnnouncementCoroutine(text, duration));
        }

        private IEnumerator ShowAnnouncementCoroutine(string text, float duration) {
            if (announcementText == null) yield break;

            announcementText.text = text;
            announcementText.gameObject.SetActive(true);

            yield return new WaitForSecondsRealtime(duration);

            announcementText.gameObject.SetActive(false);
        }
    }
}
