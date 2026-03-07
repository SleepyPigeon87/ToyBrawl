using UnityEngine;
using Brawler.Input;
using Brawler.Combat;
using Brawler.Core;
using Brawler.Fighter; 

public class Projectile : MonoBehaviour{
    [SerializeField] private float maxLifetime = 3f;
    private float speed;
    private AttackData attackData;
    private FighterBase owner;
    private Rigidbody2D rb;
    private Hitbox hitbox;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<Hitbox>();
    }

    public void Initialize(AttackData data, FighterBase owner) {
        attackData = data;
        this.owner = owner;
        speed = data.projectileSpeed;
        var col = GetComponent<BoxCollider2D>();
        if (col != null) {
            col.size = attackData.hitboxSize;
        }

        hitbox.Initialize(owner);
        hitbox.Activate(attackData);

        //Blastoff!!
        rb.linearVelocity = new Vector2(speed * owner.FacingDirection, 0f);
        Destroy(gameObject, maxLifetime);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox == null) return;
        if (hurtbox.Owner == owner) return;
        Destroy(gameObject); 

    }

}
