using System;
using UnityEngine;

namespace Brawler.Core
{
    /// <summary>
    /// Static event hub for game-wide events.
    /// Subscribe in OnEnable, unsubscribe in OnDisable.
    ///
    /// Usage:
    ///   GameEvents.OnFighterKO += HandleKO;
    ///   GameEvents.OnFighterKO -= HandleKO;
    ///   GameEvents.OnFighterKO?.Invoke(fighter, zoneType);
    /// </summary>
    public static class GameEvents
    {
        // Fighter events
        public static Action<FighterKOEventArgs> OnFighterKO;
        public static Action<FighterDamageEventArgs> OnFighterDamaged;
        public static Action<FighterRespawnEventArgs> OnFighterRespawn;

        // Round/Match events
        public static Action<int> OnRoundStart;          // roundNumber
        public static Action<int> OnRoundEnd;            // winnerPlayerIndex
        public static Action<int> OnMatchEnd;            // winnerPlayerIndex
        public static Action<GameState> OnGameStateChanged;

        // UI events
        public static Action<int, int> OnRoundScoreChanged; // playerIndex, newScore

        /// <summary>
        /// Clear all event subscriptions. Call on scene load to prevent leaks.
        /// </summary>
        public static void ClearAll()
        {
            OnFighterKO = null;
            OnFighterDamaged = null;
            OnFighterRespawn = null;
            OnRoundStart = null;
            OnRoundEnd = null;
            OnMatchEnd = null;
            OnGameStateChanged = null;
            OnRoundScoreChanged = null;
        }
    }

    /// <summary>
    /// Event data for when a fighter is knocked out (enters blast zone).
    /// </summary>
    public struct FighterKOEventArgs
    {
        public int PlayerIndex;
        public BlastZoneType ZoneType;
        public Vector2 ExitVelocity;
    }

    /// <summary>
    /// Event data for when a fighter takes damage.
    /// </summary>
    public struct FighterDamageEventArgs
    {
        public int PlayerIndex;
        public float Damage;
        public float NewHealth;
        public float KnockbackForce;
        public Vector2 KnockbackDirection;
    }

    /// <summary>
    /// Event data for when a fighter respawns.
    /// </summary>
    public struct FighterRespawnEventArgs
    {
        public int PlayerIndex;
        public Vector2 SpawnPosition;
    }

    /// <summary>
    /// Which direction the fighter was knocked out.
    /// </summary>
    public enum BlastZoneType
    {
        Top,
        Bottom,
        Left,
        Right
    }

    /// <summary>
    /// Current state of the game/match.
    /// </summary>
    public enum GameState
    {
        Waiting,        // Before match starts
        Countdown,      // 3, 2, 1, GO!
        Fighting,       // Active gameplay
        RoundEnd,       // Brief pause after KO
        MatchEnd,       // Match is over, show winner
        Paused          // Game paused
    }
}
