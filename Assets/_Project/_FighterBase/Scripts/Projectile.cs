using UnityEngine;
using Brawler.Combat;
using Brawler.Fighter;

public class Projectile : MonoBehaviour {
    [SerializeField] private float maxLifetime = 3f;
    private float speed;
    private AttackData attackData;
    private FighterBase owner;
    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(AttackData data, FighterBase ownerFighter) {
        attackData = data;
        owner = ownerFighter;
        speed = data.projectileSpeed;
        var col = GetComponent<BoxCollider2D>();
        if (col != null) col.size = attackData.hitboxSize;
        rb.linearVelocity = new Vector2(speed * owner.FacingDirection, 0f);
        Destroy(gameObject, maxLifetime);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox == null) return;
        if (hurtbox.Owner == owner) return;
        hurtbox.OnHit(null, attackData, owner.FacingDirection);
        owner.OnAttackHit(hurtbox.Owner, attackData);
        Destroy(gameObject);
    }
}