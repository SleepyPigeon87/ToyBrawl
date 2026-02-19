using UnityEngine;

namespace Brawler.Arena
{
    /// <summary>
    /// A one-way platform that fighters can jump through from below
    /// and drop through by pressing down.
    ///
    /// SCAFFOLD - Students complete the drop-through logic.
    ///
    /// TODO: See Lesson 02 for implementation guide.
    ///
    /// How it should work:
    ///   1. Fighter can jump through from below (uses PlatformEffector2D)
    ///   2. When pressing down on platform, fighter drops through
    ///   3. Brief period where fighter ignores platform collision
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(PlatformEffector2D))]
    public class Platform : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("How long to disable collision when dropping through.")]
        #pragma warning disable CS0414 // Used by TODO coroutine when students wire drop-through
        [SerializeField] private float dropThroughDuration = 0.25f;
        #pragma warning restore CS0414

        [Header("Debug")]
        [SerializeField] private bool logDropThrough = false;

        private Collider2D platformCollider;
        private PlatformEffector2D effector;

        private void Awake()
        {
            platformCollider = GetComponent<Collider2D>();
            effector = GetComponent<PlatformEffector2D>();

            // Configure effector for one-way platform
            effector.useOneWay = true;
            effector.useOneWayGrouping = true;
            effector.surfaceArc = 180f; // Only collide from above
        }

        /// <summary>
        /// Call this to make a fighter drop through the platform.
        /// TODO: Students implement this method.
        ///
        /// Steps to implement:
        /// 1. Start a coroutine
        /// 2. Temporarily ignore collision between fighter and platform
        /// 3. Wait for dropThroughDuration
        /// 4. Re-enable collision
        ///
        /// Hint: Use Physics2D.IgnoreCollision(fighterCollider, platformCollider, true/false)
        /// </summary>
        public void DropThrough(Collider2D fighterCollider)
        {
            // TODO: Implement drop-through logic
            // See Lesson for step-by-step guide

            if (logDropThrough)
            {
                Debug.Log($"[Platform] DropThrough called - implement this!");
            }

            // STEP 1: Start coroutine to handle drop-through
            // StartCoroutine(DropThroughCoroutine(fighterCollider));
        }

        /*
         * TODO: Uncomment and complete this coroutine
         *
        private System.Collections.IEnumerator DropThroughCoroutine(Collider2D fighterCollider)
        {
            // STEP 2: Disable collision
            Physics2D.IgnoreCollision(fighterCollider, platformCollider, true);

            // STEP 3: Wait
            yield return new WaitForSeconds(dropThroughDuration);

            // STEP 4: Re-enable collision
            Physics2D.IgnoreCollision(fighterCollider, platformCollider, false);
        }
        */

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.5f);

            var col = GetComponent<Collider2D>();
            if (col is BoxCollider2D box)
            {
                Vector3 center = transform.position + (Vector3)box.offset;
                Vector3 size = box.size;
                Gizmos.DrawCube(center, size);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}
