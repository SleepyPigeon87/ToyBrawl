using UnityEngine;

namespace Brawler.Arena
{
    /// <summary>
    /// A spawn point for fighters.
    /// Place two of these in the arena for Player 1 and Player 2.
    ///
    /// GameManager uses these to position fighters at match start and after respawn.
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Which player spawns here (0 = Player 1, 1 = Player 2).")]
        [SerializeField] private int playerIndex = 0;

        [Tooltip("Direction the fighter should face when spawning (1 = right, -1 = left).")]
        [SerializeField] private int facingDirection = 1;

        [Header("Visual")]
        [SerializeField] private Color gizmoColor = Color.cyan;
        [SerializeField] private float gizmoRadius = 0.5f;

        public int PlayerIndex => playerIndex;
        public int FacingDirection => facingDirection;
        public Vector2 Position => transform.position;

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, gizmoRadius);

            // Draw facing direction arrow
            Vector3 arrowEnd = transform.position + Vector3.right * facingDirection * gizmoRadius * 1.5f;
            Gizmos.DrawLine(transform.position, arrowEnd);

            // Arrow head
            Vector3 arrowHead1 = arrowEnd + new Vector3(-facingDirection * 0.2f, 0.2f, 0);
            Vector3 arrowHead2 = arrowEnd + new Vector3(-facingDirection * 0.2f, -0.2f, 0);
            Gizmos.DrawLine(arrowEnd, arrowHead1);
            Gizmos.DrawLine(arrowEnd, arrowHead2);

            // Draw player number
#if UNITY_EDITOR
            UnityEditor.Handles.Label(
                transform.position + Vector3.up * gizmoRadius * 1.5f,
                $"P{playerIndex + 1}"
            );
#endif
        }
    }
}
