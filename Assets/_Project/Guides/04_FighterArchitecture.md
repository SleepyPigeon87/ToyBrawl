# Guide 04: Fighter Architecture

This guide explains how fighters work in the template and how to build your own.

---

## The FighterBase Pattern

Every fighter extends `FighterBase`. At minimum, you override the `FighterName` property:

```csharp
using UnityEngine;
using Brawler.Fighter;

public class MyFighter : FighterBase
{
    public override string FighterName => "My Fighter";
}
```

That's a working fighter. Everything else is optional customization.

---

## What FighterBase Gives You

| Property | Type | What It Does |
|----------|------|-------------|
| `PlayerIndex` | int | Which player (0 or 1) |
| `FacingDirection` | float | Which way you're facing (1 or -1) |
| `IsGrounded` | bool | Ground contact check |
| `CanAct` | bool | Can move/attack (false during hitstun, knockback) |
| `IsDead` | bool | Health is zero |
| `Health` | FighterHealth | Health component reference |
| `Knockback` | KnockbackHandler | Knockback component reference |
| `Input` | PlayerInputHandler | Input component reference |

---

## Virtual Methods You Can Override

### OnFighterInitialized()
Called after all components are set up. Use for custom initialization.

```csharp
protected override void OnFighterInitialized()
{
    // Load your custom data, set up references, etc.
}
```

### OnTakeDamage(float damage)
Called when you take a hit. Add effects, sounds, screen shake.

```csharp
protected override void OnTakeDamage(float damage)
{
    StartCoroutine(DamageFlash());
}
```

### OnKO()
Called when you're knocked out (hit a blast zone).

```csharp
protected override void OnKO()
{
    AudioSource.PlayClipAtPoint(koSound, transform.position);
}
```

### OnRespawn(Vector2 position)
Called when you respawn at a spawn point.

```csharp
protected override void OnRespawn(Vector2 position)
{
    StartCoroutine(InvincibilityFlash());
}
```

### OnAttackInput(AttackContext context)
Override to handle attacks yourself instead of using AttackController.

```csharp
public override void OnAttackInput(AttackContext context)
{
    switch (context)
    {
        case AttackContext.Neutral:
            StartCoroutine(JabAttack());
            break;
        case AttackContext.Forward:
            StartCoroutine(SidePunch());
            break;
    }
}
```

---

## Movement Options

Each fighter needs movement code. You have several options:

### Use FighterMovement (Default)
Add the `FighterMovement` component to your prefab and assign a `MovementConfig` ScriptableObject. This gives you ground movement, jumping, air control, gravity, and coyote time out of the box. Create a `MovementConfig` via **Create > Brawler > Movement Config** to tune the feel.

### Copy and Modify
Copy `FighterMovement.cs` from `_FighterBase/Scripts/` into your fighter's folder. Modify the copy however you want. This is good if you want the default as a starting point but need custom behavior.

### Bring Your Platformer Code
If you built movement code in the Platformer project, you can bring it in. Just make sure it respects `CanAct` and the current game state — don't allow movement during hitstun, knockback, countdowns, or between rounds:

```csharp
// Check game state first
var gm = GameManager.Instance;
if (gm != null && gm.CurrentState != GameState.Fighting && gm.CurrentState != GameState.Waiting)
    return;

// Then check fighter state
if (fighter.CanAct)
{
    // Your movement code here
}
```

### Build From Scratch
Override `Update()` or `FixedUpdate()` in your fighter script and handle movement yourself. Call `base.Update()` to keep the facing direction logic.

---

## Attack Options

### AttackController (Recommended Starting Point)
Add `AttackController` to your fighter and assign `AttackData` ScriptableObjects to its slots. The controller automatically determines context based on input direction and grounded state:

- **Neutral** — no direction held
- **Forward** — holding left/right
- **Up** — holding up
- **Down** — holding down
- **Aerial** — in the air

See [Guide 05: Combat System](05_CombatSystem.md) for details on creating AttackData.

### Custom Attack System
Override `OnAttackInput()` in your fighter script for full control. This is the way to go for complex mechanics like combos, charge attacks, or stance systems.

---

## Prefab Setup

Your fighter prefab needs these components on the root GameObject:

### Required
- **Rigidbody2D** — Freeze Rotation Z, Collision Detection: Continuous
- **Collider2D** — BoxCollider2D or CapsuleCollider2D sized to character body
- **Your Fighter Script** — extends FighterBase
- **FighterHealth** — auto-added via RequireComponent
- **KnockbackHandler** — auto-added via RequireComponent
- **PlayerInputHandler** — for receiving input (player index is set automatically by GameManager)

### Recommended
- **FighterMovement** — default movement (or your own)
- **AttackController** — default attacks (or your own)
- **FighterAnimator** — connects game state to Animator
- **Animator** — Unity's animation system
- **SpriteRenderer** — on a Sprite child object

### Child Objects
```
MyFighter (root)
├── Sprite          SpriteRenderer, Animator, FighterAnimator
├── GroundCheck     Empty object at feet (transform reference for ground detection)
├── Hurtbox         Collider2D (trigger), Hurtbox script
└── Hitbox          Created by AttackController, or add your own
```

---

## Prefab Checklist

- [ ] Rigidbody2D: Freeze Rotation Z = true
- [ ] Rigidbody2D: Collision Detection = Continuous
- [ ] Fighter script attached (extends FighterBase)
- [ ] PlayerInputHandler with InputActions asset assigned (player index is set by GameManager — don't set it manually)
- [ ] MovementConfig assigned (if using FighterMovement)
- [ ] GroundCheck child positioned at feet
- [ ] Hurtbox collider is set as trigger
- [ ] Layers set correctly for ground detection
- [ ] Sprite faces RIGHT by default

---

## Quick Start: Duplicate ExampleFighter

The fastest way to start:
1. Find `_FighterBase/Prefabs/ExampleFighter`
2. Duplicate it (Ctrl+D)
3. Move the copy to your fighter folder (`Fighter1/Prefabs/` or `Fighter2/Prefabs/`)
4. Rename it
5. Replace the ExampleFighter script with your own fighter script
6. Replace sprites, animator controller, and attack data

---

## Where to Put Your Files

Keep your fighter organized in its folder:
- `Fighter1/Scripts/` — your fighter script and any custom components
- `Fighter1/Art/Sprites/` — sprite sheets
- `Fighter1/Art/Animations/` — animation clips and animator controller
- `Fighter1/Configs/Attacks/` — AttackData ScriptableObjects
- `Fighter1/Prefabs/` — your fighter prefab

Same structure for Fighter2. If your game has more than 2 fighters, create additional folders as needed.

---

**Next:** [Guide 05: Combat System](05_CombatSystem.md) — hitboxes, hurtboxes, attack data, and knockback
