using UnityEngine;

namespace Brawler.Core
{
    /// <summary>
    /// Configuration for match settings.
    /// Create via: Create > Brawler > Match Config
    /// </summary>
    [CreateAssetMenu(fileName = "MatchConfig", menuName = "Brawler/Match Config")]
    public class MatchConfig : ScriptableObject
    {
        [Header("Round Settings")]
        [Tooltip("Number of rounds needed to win the match (e.g., 2 = best of 3).")]
        [Range(1, 5)]
        public int roundsToWin = 2;

        [Header("Timing")]
        [Tooltip("Delay before round starts (countdown).")]
        [Range(0f, 5f)]
        public float roundStartDelay = 2f;

        [Tooltip("Delay after KO before next round.")]
        [Range(0.5f, 3f)]
        public float roundEndDelay = 1.5f;

        [Tooltip("Time limit per round in seconds. 0 = no limit.")]
        [Range(0f, 300f)]
        public float matchTimeLimit = 0f;

        [Header("Respawn")]
        [Tooltip("How long the respawn invincibility lasts.")]
        [Range(0f, 5f)]
        public float respawnInvincibilityDuration = 2f;

        [Header("Sudden Death")]
        [Tooltip("Enable sudden death if time runs out while tied.")]
        public bool enableSuddenDeath = true;

        [Tooltip("In sudden death, one hit KOs.")]
        public bool suddenDeathOneHitKO = true;
    }
}
