using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Brawler.Core;

namespace Brawler.UI
{
    /// <summary>
    /// Displays round wins for both players.
    ///
    /// SCAFFOLD - Students wire this to GameEvents.
    /// See Lesson 03: Wiring UI for step-by-step guide.
    /// </summary>
    public class RoundDisplayUI : MonoBehaviour
    {
        [Header("Player 1")]
        [Tooltip("Text showing P1 round wins.")]
        [SerializeField] private TextMeshProUGUI player1ScoreText;

        [Tooltip("Round win icons for P1 (enable/disable based on wins).")]
        [SerializeField] private Image[] player1RoundIcons;

        [Header("Player 2")]
        [Tooltip("Text showing P2 round wins.")]
        [SerializeField] private TextMeshProUGUI player2ScoreText;

        [Tooltip("Round win icons for P2.")]
        [SerializeField] private Image[] player2RoundIcons;

        [Header("Colors")]
        [SerializeField] private Color winColor = Color.yellow;
        [SerializeField] private Color emptyColor = Color.gray;

        private void Start()
        {
            // TODO STEP 1: Subscribe to round score changes
            // GameEvents.OnRoundScoreChanged += OnRoundScoreChanged;

            // Initialize display
            UpdateDisplay(0, 0);
            UpdateDisplay(1, 0);
        }

        private void OnDestroy()
        {
            // TODO: Unsubscribe
            // GameEvents.OnRoundScoreChanged -= OnRoundScoreChanged;
        }

        /// <summary>
        /// Called when round score changes.
        /// TODO STEP 1: Subscribe to this event.
        /// </summary>
        private void OnRoundScoreChanged(int playerIndex, int newScore)
        {
            UpdateDisplay(playerIndex, newScore);
        }

        private void UpdateDisplay(int playerIndex, int score)
        {
            // Update text
            var text = playerIndex == 0 ? player1ScoreText : player2ScoreText;
            if (text != null)
            {
                text.text = score.ToString();
            }

            // Update icons
            var icons = playerIndex == 0 ? player1RoundIcons : player2RoundIcons;
            if (icons != null)
            {
                for (int i = 0; i < icons.Length; i++)
                {
                    if (icons[i] != null)
                    {
                        icons[i].color = i < score ? winColor : emptyColor;
                    }
                }
            }
        }
    }
}
