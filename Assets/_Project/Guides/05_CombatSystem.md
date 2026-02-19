# Guide 05: Combat System

This guide covers how the combat system works: hitboxes, hurtboxes, attack data, and knockback.

---

## How Combat Works

```
Attack Input
    │
    ▼
AttackController (determines context: neutral, forward, up, down, aerial)
    │
    ▼
AttackData (ScriptableObject defines damage, timing, knockback, hitbox)
    │
    ▼
Hitbox activates (positioned relative to fighter, active for N frames)
    │
    ▼
Hitbox overlaps Hurtbox (collision detection via Physics2D layers)
    │
    ▼
FighterHealth.TakeDamage() → KnockbackHandler applies force → HitstopManager freezes
```

---

## AttackData

`AttackData` is a ScriptableObject that defines everything about an attack. Create one via **Create > Brawler > Attack Data**.

### Damage

| Property | Description | Typical Range |
|----------|-------------|---------------|
| **Damage** | Health removed from opponent | 5-25 |

Light attacks: 5-10. Heavy attacks: 15-25.

### Knockback

| Property | Description |
|----------|-------------|
| **Base Knockback** | Force applied before health multiplier |
| **Knockback Angle** | Direction vector (X=horizontal, Y=vertical) |

```
Knockback Angle Examples:
(1, 0)    → Pure horizontal (forward)
(1, 0.5)  → Diagonal up-forward (most common)
(0, 1)    → Pure vertical (launcher)
(1, -0.3) → Spike (sends down-forward)
(-1, 0.5) → Backward launch
```

The X component is automatically flipped based on facing direction.

### Timing (frames at 60fps)

| Property | Description | Feel |
|----------|-------------|------|
| **Startup Frames** | Before hitbox activates | Lower = faster |
| **Active Frames** | Hitbox is active | Higher = easier to land |
| **Recovery Frames** | After hitbox, before can act | Higher = more punishable |

```
Fast Jab:      3 startup, 3 active, 8 recovery   (14 frames total)
Medium Punch:  6 startup, 5 active, 15 recovery   (26 frames total)
Heavy Smash:   12 startup, 4 active, 25 recovery   (41 frames total)
```

### Hitstun and Hitstop

| Property | Description |
|----------|-------------|
| **Hitstun Duration** | How long opponent can't act after being hit |
| **Hitstop Duration** | Freeze frame on hit (game feel) |

Hitstop: 0.03-0.08 seconds for most attacks. Makes hits feel impactful.

### Hitbox Dimensions

| Property | Description |
|----------|-------------|
| **Hitbox Offset** | Position relative to fighter center |
| **Hitbox Size** | Width and height |

```
Example for a forward punch:
Offset: (0.8, 0.2)  → In front of fighter, slightly up
Size:   (0.6, 0.4)  → Rectangular punch shape
```

---

## Attack Archetypes

These are starting points to customize, not required values. Use them as reference when creating your attacks.

### Quick Jab
```
Damage: 5  |  Knockback: 3  |  Angle: (1, 0.2)
Startup: 3  |  Active: 3  |  Recovery: 8
Hitbox Offset: (0.5, 0)  |  Size: (0.4, 0.3)
```

### Forward Tilt
```
Damage: 10  |  Knockback: 6  |  Angle: (1, 0.3)
Startup: 6  |  Active: 4  |  Recovery: 12
Hitbox Offset: (0.7, 0)  |  Size: (0.6, 0.4)
```

### Up Attack (Launcher)
```
Damage: 12  |  Knockback: 8  |  Angle: (0.2, 1)
Startup: 5  |  Active: 5  |  Recovery: 15
Hitbox Offset: (0.2, 0.6)  |  Size: (0.5, 0.6)
```

### Down Attack (Spike)
```
Damage: 14  |  Knockback: 7  |  Angle: (0.5, -0.8)
Startup: 8  |  Active: 3  |  Recovery: 20
Hitbox Offset: (0.4, -0.3)  |  Size: (0.5, 0.4)
```

### Aerial Attack
```
Damage: 8  |  Knockback: 5  |  Angle: (1, 0.4)
Startup: 4  |  Active: 6  |  Recovery: 10
Hitbox Offset: (0.5, 0)  |  Size: (0.7, 0.5)
```

### Power Smash
```
Damage: 22  |  Knockback: 12  |  Angle: (1, 0.4)
Startup: 15  |  Active: 4  |  Recovery: 28
Hitbox Offset: (0.9, 0.1)  |  Size: (0.8, 0.6)
```

---

## AttackController Context

The `AttackController` automatically determines which attack to use based on input and state:

```
Not grounded       → Aerial Attack
Holding up         → Up Attack
Holding down       → Down Attack
Holding left/right → Forward Attack
No direction       → Neutral Attack
```

Assign `AttackData` assets to each slot in the AttackController Inspector.

---

## The Knockback Formula

```
finalKnockback = baseKnockback * (2 - healthPercent)
```

| Health | Multiplier | 10 Base Knockback Becomes |
|--------|-----------|--------------------------|
| 100% (full) | 1.0x | 10 |
| 50% | 1.5x | 15 |
| 0% (empty) | 2.0x | 20 |

This means the same attack becomes more dangerous as your opponent takes damage. Design your kill moves around this scaling.

---

## Balancing Tips

**Risk vs Reward:** Fast attacks are safe but low damage. Slow attacks hit hard but are punishable on whiff.

**Combo Design:** Low-knockback attacks with enough hitstun can combo into follow-ups. If hitstun is longer than the next attack's startup, you have a combo.

**Kill Moves:** High base knockback for finishing opponents at low health. Usually slower with more recovery to compensate.

**Character Identity:** Give each fighter a different feel through attack data. A rushdown fighter has fast startup and short recovery. A heavy hitter has high damage but slow startup and long recovery.

---

## Audio

AttackData includes optional audio fields:
- **Attack Sound**: Plays on attack start (whoosh)
- **Hit Sound**: Plays on successful hit (impact)

Assign AudioClips in the Inspector.

---

## Testing and Debug

1. Create AttackData assets and assign to AttackController
2. Enter Play mode and press attack with different directions
3. Watch hitbox gizmos: **red** = active, **gray** = inactive
4. Enable "Log Attacks" on AttackController to see timing in the console
5. Adjust values based on how it feels — iteration is key

---

**Next:** [Guide 06: Art Reference](06_ArtReference.md) — prefab setup, animator controllers, and sprites
