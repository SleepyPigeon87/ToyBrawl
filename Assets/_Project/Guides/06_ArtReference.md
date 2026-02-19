# Guide 06: Art Reference

This guide covers prefab setup, animator controllers, sprite handling, and animation clips. Everything you need to get a fighter looking and moving correctly in the game.

---

## Prefab Structure

Every fighter prefab follows this hierarchy:

```
MyFighter (root GameObject)
│
├── Sprite                    Visual representation
│   └── SpriteRenderer
│   └── Animator
│   └── FighterAnimator
│
├── GroundCheck               Empty object at feet
│
├── Hurtbox                   Where fighter can be hit
│   └── BoxCollider2D (trigger)
│   └── Hurtbox script
│
└── Hitbox                    Created by AttackController
    └── BoxCollider2D (trigger)
    └── Hitbox script
```

See [Guide 04: Fighter Architecture](04_FighterArchitecture.md) for the full prefab setup checklist and required components.

---

## Rigidbody2D Settings

| Setting | Value | Why |
|---------|-------|-----|
| Body Type | Dynamic | Responds to physics |
| Mass | 1 | Standard mass |
| Linear Drag | 0 | Movement script handles deceleration |
| Gravity Scale | 1 | Normal gravity (movement may override) |
| Collision Detection | Continuous | Prevents tunneling at high speeds |
| Interpolate | Interpolate | Smooth movement between physics steps |
| Freeze Rotation Z | true | Don't rotate from physics |
| Sleeping Mode | Start Awake | Always active |

---

## Collider Setup

### Main Collider (Physics) — on root object
- **Not** a trigger
- Used for platform collision and physics
- BoxCollider2D or CapsuleCollider2D, sized to match the character's body
- Size to the physical presence, not the full sprite (trim empty space)

### Hurtbox Collider (Combat) — on Hurtbox child
- **Is** a trigger (Is Trigger = true)
- Used for receiving hits
- Usually similar to main collider size
- Smaller = harder to hit, larger = easier to hit

---

## Layer Setup

Recommended physics layers:

| Layer | Used By |
|-------|---------|
| Ground | Platforms, floors |
| Player | Fighter physics colliders |
| Hitbox | Attack hitboxes |
| Hurtbox | Vulnerable areas |

Set up in **Edit > Project Settings > Tags and Layers**.

### Physics2D Collision Matrix
**Edit > Project Settings > Physics 2D > Layer Collision Matrix:**

```
             Ground  Player  Hitbox  Hurtbox
Ground         -       Y       -       -
Player         Y       -       -       -
Hitbox         -       -       -       Y
Hurtbox        -       -       Y       -
```

Players collide with ground but not each other. Hitboxes only detect hurtboxes.

---

## Sprite Setup

### Import Settings
1. Import your sprite sheet image
2. Select it in the Project window
3. In the Inspector:
   - Texture Type: **Sprite (2D and UI)**
   - Sprite Mode: **Multiple**
   - Pixels Per Unit: Match your game scale (16, 32, or 100 are common)
4. Open **Sprite Editor** and slice (Grid By Cell Size works for uniform sheets)
5. Apply

### Pivot Point
- **Bottom**: Position represents the feet (good for ground alignment)
- **Center**: Position represents the body center

Choose one and stay consistent across all animations.

### Flip Method
`FighterAnimator` handles facing direction by scaling X negative. **Make sure your sprites face RIGHT by default.**

### Sorting
Set a Sorting Layer (create "Characters" if needed) and Order in Layer for layering control.

---

## Animator Controller

The animation system flow:

```
Game State → FighterAnimator → Animator Controller → Animation Clips
   (code)      (sets params)      (state machine)      (visuals)
```

`FighterAnimator` reads game state and sets Animator parameters. Your Animator Controller uses those parameters to transition between animation states.

### Creating the Controller
1. Right-click in your fighter's Art/Animations folder
2. **Create > Animator Controller**
3. Name it and double-click to open the Animator window

### Required Parameters

Create these parameters in your Animator Controller — names must match exactly:

| Parameter | Type | What It Represents |
|-----------|------|--------------------|
| `HorizontalSpeed` | Float | Movement speed (0 to max) |
| `VerticalSpeed` | Float | Vertical velocity |
| `IsGrounded` | Bool | On the ground |
| `IsJumping` | Bool | Moving upward |
| `IsFalling` | Bool | Moving downward in air |
| `IsAttacking` | Bool | During an attack |
| `IsHitstun` | Bool | Being hit |
| `Attack` | Trigger | Attack starts |

These are set automatically by `FighterAnimator`:
```csharp
animator.SetFloat("HorizontalSpeed", speed);
animator.SetFloat("VerticalSpeed", velocity);
animator.SetBool("IsGrounded", grounded);
animator.SetBool("IsJumping", rising);
animator.SetBool("IsFalling", falling);
animator.SetBool("IsAttacking", attacking);
animator.SetBool("IsHitstun", stunned);
animator.SetTrigger("Attack");
```

---

## State Machine Design

### Minimal Setup (start here)

```
┌──────┐  HorizontalSpeed > 0.1  ┌──────┐
│ Idle │◄────────────────────────►│ Run  │
└──────┘  HorizontalSpeed < 0.1  └──────┘
```

### Full Setup

```
         GROUND
    ┌──────┐    ┌──────┐
    │ Idle │◄──►│ Run  │
    └──┬───┘    └──┬───┘
       │           │
       │ !IsGrounded
       ▼           ▼
         AIR
    ┌──────┐    ┌──────┐
    │ Jump │───►│ Fall │
    └──────┘    └──────┘
       │
       │ IsGrounded
       ▼
    (back to Idle)
```

Plus attack and hitstun states triggered from Any State.

### Transition Settings

For parameter-based transitions (not animation-driven):
- **Has Exit Time:** Unchecked
- **Transition Duration:** 0 (instant)
- **Condition:** The parameter that should trigger the transition

For attack animations that should play fully:
- **Has Exit Time:** Checked
- **Exit Time:** 1.0

---

## Animation Clips

### Required Animations

| Name | Frames | Loop | Notes |
|------|--------|------|-------|
| Idle | 4-8 | Yes | Subtle breathing or movement |
| Run | 6-12 | Yes | Full run cycle |
| Jump | 3-6 | No | Anticipation + rise |
| Fall | 2-4 | Yes | Falling pose |
| Attack_Neutral | 4-8 | No | Quick jab |
| Attack_Forward | 6-10 | No | Punch or kick |
| Attack_Up | 5-8 | No | Upward strike |
| Attack_Down | 5-8 | No | Low attack |
| Attack_Aerial | 4-8 | No | Air attack |
| Hitstun | 2-4 | No | Getting hit reaction |

### Creating Clips from Sprite Sheets
1. Select all frames for an animation in the Project window
2. Drag onto the Scene or Hierarchy
3. Unity creates an Animation Clip and Animator Controller automatically
4. Rename the clip and move it to your animations folder

### Loop Settings
Select a clip in the Project window. In the Inspector:
- **Loop Time: Yes** for Idle, Run, Fall
- **Loop Time: No** for attacks, Jump, Hitstun

---

## Animation Events (Advanced)

For precise attack timing, you can use Animation Events to call methods at specific frames:

1. Open the Animation window (Window > Animation > Animation)
2. Select your attack animation
3. Scrub to the frame where the hitbox should activate
4. Click "Add Event" below the timeline
5. Set the function name (e.g., `ActivateHitbox`)

This is optional — `AttackController` handles hitbox timing automatically via frame data in `AttackData`. Animation Events are for custom attack systems.

---

## Blend Trees (Optional)

For smooth idle-to-run blending instead of hard cuts:

1. In the Animator, right-click > Create State > From New Blend Tree
2. Double-click to enter the Blend Tree
3. Set Parameter to `HorizontalSpeed`
4. Add motions: Idle at threshold 0, Run at your max speed threshold
5. Unity blends between them based on speed

---

## Troubleshooting

**Fighter falls through floor:**
- Check platforms have the Ground layer set
- Check MovementConfig has the correct Ground Layer mask
- Check GroundCheck is positioned at the feet

**Fighter doesn't animate:**
- Check Animator component has the controller assigned
- Check FighterAnimator is on the Sprite child (not the root)
- Check parameter names match exactly (case-sensitive)

**Stuck in a state:**
- Check transition conditions are set
- Make sure "Has Exit Time" is unchecked for parameter-based transitions
- Check for conflicting conditions (both IsJumping and IsGrounded true)

**Attacks don't hit:**
- Check Hitbox layer collides with Hurtbox layer in Physics2D settings
- Check hitbox collider is set as trigger
- Check AttackController has AttackData assigned

**Animation too fast or slow:**
- Adjust the animation clip's Sample Rate
- Or adjust the Speed multiplier in the Animator state settings

**Fighter slides around:**
- Check Rigidbody2D Freeze Rotation Z is enabled
- Check for physics materials with unexpected friction settings

---

## Checklist

- [ ] Animator Controller created in your fighter's Art/Animations folder
- [ ] All 8 required parameters added with correct types
- [ ] Idle state set as default (orange)
- [ ] Transitions have parameter conditions (not just exit time)
- [ ] Attack animations don't loop
- [ ] Idle, Run, Fall animations loop
- [ ] FighterAnimator component on Sprite child object
- [ ] Animator component has your controller assigned
- [ ] SpriteRenderer has a sprite assigned
- [ ] Sprites face right by default
- [ ] Rigidbody2D configured (Freeze Rotation, Continuous collision)
- [ ] Colliders sized correctly (physics collider + hurtbox trigger)
- [ ] Layers set and collision matrix configured
