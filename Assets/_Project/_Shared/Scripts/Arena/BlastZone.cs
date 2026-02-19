using UnityEngine;
using Brawler.Core;
using Brawler.Fighter;

namespace Brawler.Arena
{
    /// <summary>
    /// A blast zone that KOs fighters who enter it.
    /// Place these at the edges of the arena (top, bottom, left, right).
    ///
    /// When a fighter enters:
    ///   1. Fires GameEvents.OnFighterKO
    ///   2. GameManager handles respawn logic
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class BlastZone : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Which direction this blast zone represents.")]
        [SerializeField] private BlastZoneType zoneType = BlastZoneType.Bottom;

        [Header("Visual")]
        [Tooltip("Color to show in editor.")]
        [SerializeField] private Color gizmoColor = new Color(1f, 0f, 0f, 0.3f);

        public BlastZoneType ZoneType => zoneType;

        private Collider2D zoneCollider;

        private void Awake()
        {
            zoneCollider = GetComponent<Collider2D>();
            zoneCollider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if it's a fighter
            var fighter = other.GetComponentInParent<FighterBase>();
            if (fighter == null) return;

            // Don't KO respawning fighters
            if (fighter.IsRespawning) return;

            // Get exit velocity for event data
            var rb = fighter.GetComponent<Rigidbody2D>();
            Vector2 exitVelocity = rb != null ? rb.linearVelocity : Vector2.zero;

            Debug.Log($"[BlastZone] Player {fighter.PlayerIndex} KO'd via {zoneType} blast zone!");

            // Fire KO event
            GameEvents.OnFighterKO?.Invoke(new FighterKOEventArgs
            {
                PlayerIndex = fighter.PlayerIndex,
                ZoneType = zoneType,
                ExitVelocity = exitVelocity
            });
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;

            var col = GetComponent<Collider2D>();
            if (col is BoxCollider2D box)
            {
                Vector3 center = transform.position + (Vector3)box.offset;
                Vector3 size = box.size;
                Gizmos.DrawCube(center, size);
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}
