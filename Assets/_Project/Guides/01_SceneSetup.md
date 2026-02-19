# Guide 01: Scene Setup (Tech Lead Exercise)

This guide walks the **tech lead** through building the DevArena scene from scratch. When you're done, the project will be playable — two fighters moving, jumping, and hitting each other on a platform with blast zones.

**Time:** ~30-45 minutes

**Parallel work:** While the tech lead does this, the rest of the team fills out `DESIGN_DOC.md` — fighter concepts, game rules, art direction, and milestone planning. Both tracks happen at the same time.

**When this is done:** The dev lead pushes the working scene and invites the team to the repo. Everyone clones a project that runs on first open.

---

## What You're Building

By the end of this guide, your scene will have:

```
DevArena (Scene)
├── --- MANAGERS ---
│   ├── GameManager          Drives match flow (scaffold — wired later)
│   └── HitstopManager       Freeze frames on hit
│
├── --- ARENA ---
│   ├── Ground               Platform the fighters stand on
│   ├── BlastZone_Bottom     KO trigger below arena
│   ├── BlastZone_Top        KO trigger above arena
│   ├── BlastZone_Left       KO trigger left side
│   ├── BlastZone_Right      KO trigger right side
│   ├── SpawnPoint_P1        Where Player 1 spawns
│   └── SpawnPoint_P2        Where Player 2 spawns
│
├── --- FIGHTERS ---
│   ├── ExampleFighter_P1    Player 1 (WASD + Space + J)
│   └── ExampleFighter_P2    Player 2 (Arrows + RCtrl + Numpad1)
│
└── Main Camera              (default)
```

---

## Step 1: Create the Ground Layer

FighterMovement uses a raycast to check if the fighter is standing on something. It needs a physics layer to filter against.

1. **Edit > Project Settings > Tags and Layers**
2. In the **Layers** section, find the first empty **User Layer** slot (typically slot 6)
3. Type `Ground`

You'll assign this layer to your arena platforms in Step 4.

---

## Step 2: Create ScriptableObject Assets

These configure the game systems. Right-click in the Project window to create each one.

### MatchConfig

1. Navigate to `Assets/_Project/_Shared/Configs/`
2. **Right-click > Create > Brawler > Match Config**
3. Name it `DefaultMatchConfig`
4. Leave all defaults (roundsToWin: 2, roundStartDelay: 2s, etc.)

### InputConfig

1. Same folder: `Assets/_Project/_Shared/Configs/`
2. **Right-click > Create > Brawler > Input Config**
3. Name it `DefaultInputConfig`
4. Leave all defaults (deadzone: 0.15, buffer durations: 0.1s)

### MovementConfig

1. Navigate to `Assets/_Project/_FighterBase/Configs/`
2. **Right-click > Create > Brawler > Movement Config**
3. Name it `DefaultMovementConfig`
4. **Set Ground Layer** to `Ground` (the layer you created in Step 1)
5. Leave other defaults — they provide solid platformer feel out of the box

### Example Attack Data

Create at least two attacks so the ExampleFighter can fight. Navigate to `Assets/_Project/_FighterBase/Configs/ExampleAttacks/`.

**Jab (neutral attack):**

1. **Right-click > Create > Brawler > Attack Data**
2. Name it `ExampleJab`
3. Set these values:

| Property | Value |
|----------|-------|
| Attack Name | Jab |
| Damage | 8 |
| Base Knockback | 4 |
| Knockback Angle | (1, 0.3) |
| Startup Frames | 3 |
| Active Frames | 4 |
| Recovery Frames | 10 |
| Hitstun Duration | 0.15 |
| Hitstop Duration | 0.04 |
| Hitbox Offset | (0.6, 0) |
| Hitbox Size | (0.5, 0.4) |
| Context | Neutral |

**Forward Punch:**

1. Create another Attack Data, name it `ExampleForwardPunch`
2. Set these values:

| Property | Value |
|----------|-------|
| Attack Name | Forward Punch |
| Damage | 14 |
| Base Knockback | 8 |
| Knockback Angle | (1, 0.4) |
| Startup Frames | 7 |
| Active Frames | 5 |
| Recovery Frames | 16 |
| Hitstun Duration | 0.25 |
| Hitstop Duration | 0.06 |
| Hitbox Offset | (0.8, 0.1) |
| Hitbox Size | (0.7, 0.5) |
| Context | Forward |

---

## Step 3: Create the DevArena Scene

1. **File > New Scene** — choose **Basic 2D** (or Basic if 2D isn't listed)
2. **File > Save As** — save to `Assets/_Project/_Shared/Scenes/DevArena.unity`

You should have a scene with just a **Main Camera**. If there's a directional light, delete it — this is 2D.

Set the camera:
1. Select **Main Camera**
2. Set **Position** to `(0, 1, -10)`
3. Set **Size** to `8` (orthographic — gives a good view of the arena)
4. Set **Background** color to something dark (e.g., `#1A1A2E`)

---

## Step 4: Build the Arena

### Ground Platform

1. **GameObject > Create Empty**, name it `Ground`
2. Set **Position** to `(0, -3, 0)`
3. Set **Layer** to `Ground`
4. Add Component: **Box Collider 2D**
   - Size: `(20, 1)`
5. Add Component: **Sprite Renderer** (so you can see it)
   - Sprite: `Square` (search for it in the sprite picker — it's a Unity built-in, under `UI` or use Knob)
   - Color: pick a dark gray or platform color
   - Draw Mode: `Tiled` if available, otherwise set **Scale** to `(20, 1, 1)` on the Transform

> **Tip:** If you can't find a Square sprite, create one: right-click in any folder > Create > 2D > Sprites > Square. Or skip the SpriteRenderer — the collider works without a visual.

### Blast Zones

Create four empty GameObjects. Each gets a **Box Collider 2D** and the **BlastZone** script.

| Name | Position | Collider Size | Zone Type |
|------|----------|---------------|-----------|
| `BlastZone_Bottom` | (0, -8, 0) | (30, 2) | Bottom |
| `BlastZone_Top` | (0, 14, 0) | (30, 2) | Top |
| `BlastZone_Left` | (-14, 2, 0) | (2, 26) | Left |
| `BlastZone_Right` | (14, 2, 0) | (2, 26) | Right |

For each one:
1. **GameObject > Create Empty**, rename it
2. Set the position from the table
3. Add Component: **Box Collider 2D** — the BlastZone script auto-sets it to trigger, but verify **Is Trigger** is checked
4. Add Component: **BlastZone** (namespace: Brawler.Arena)
5. Set **Zone Type** in the Inspector

You'll see red gizmo boxes in the Scene view when you select them. They should form a box around the arena with gaps at the edges of the ground platform.

### Spawn Points

1. **GameObject > Create Empty**, name it `SpawnPoint_P1`
   - Position: `(-3, -2, 0)`
   - Add Component: **SpawnPoint** (namespace: Brawler.Arena)
   - Player Index: `0`
   - Facing Direction: `1` (right)

2. **GameObject > Create Empty**, name it `SpawnPoint_P2`
   - Position: `(3, -2, 0)`
   - Add Component: **SpawnPoint**
   - Player Index: `1`
   - Facing Direction: `-1` (left)

You'll see cyan gizmo circles with arrows showing facing direction.

---

## Step 5: Build the ExampleFighter Prefab

This is the biggest step. You're building a working fighter from components.

### Create the Root GameObject

1. **GameObject > Create Empty**, name it `ExampleFighter`
2. Add these components:
   - **Rigidbody 2D**
     - Freeze Rotation: **check Z**
     - Collision Detection: **Continuous**
     - Interpolation: **Interpolate**
   - **Box Collider 2D**
     - Size: `(0.8, 1.4)`
     - Offset: `(0, 0)`
   - **Example Fighter** (namespace: Brawler.Fighter)
   - **Player Input Handler** (namespace: Brawler.Input)
     - Input Actions: drag in `BrawlerInputActions` from `Assets/_Project/_Shared/`
     - Config: drag in `DefaultInputConfig`
     - (Leave Player Index at `0` — GameManager sets this automatically based on the Fighters array order)
   - **Fighter Movement** (namespace: Brawler.Fighter)
     - Config: drag in `DefaultMovementConfig`
   - **Attack Controller** (namespace: Brawler.Fighter)
     - Neutral Attack: drag in `ExampleJab`
     - Forward Attack: drag in `ExampleForwardPunch`
     - (Leave other slots empty for now)

### Create Child: Sprite

1. Right-click `ExampleFighter` in Hierarchy > **Create Empty**, name it `Sprite`
2. Add Component: **Sprite Renderer**
   - Sprite: `Square` (built-in) or any placeholder
   - Set **Scale** on the Transform to `(0.8, 1.4, 1)` to match the collider
   - Color: leave white (ExampleFighter tints it via `fighterColor`)

### Create Child: GroundCheck

1. Right-click `ExampleFighter` > **Create Empty**, name it `GroundCheck`
2. Set **local position** to `(0, -0.7, 0)` — at the fighter's feet
3. No components needed — this is just a transform reference

**Now wire it:** Select the root `ExampleFighter`, find **Fighter Movement** in the Inspector, and drag `GroundCheck` into the **Ground Check Point** field.

### Create Child: Hurtbox

1. Right-click `ExampleFighter` > **Create Empty**, name it `Hurtbox`
2. Add Component: **Box Collider 2D**
   - **Is Trigger: checked**
   - Size: `(0.7, 1.3)`
   - Offset: `(0, 0)`
3. Add Component: **Hurtbox** (namespace: Brawler.Combat)

> **Note:** The Hitbox is created automatically by AttackController at runtime. You don't need to add one manually.

### Save as Prefab

1. Drag `ExampleFighter` from the **Hierarchy** into `Assets/_Project/_FighterBase/Prefabs/`
2. This creates the prefab asset

---

## Step 6: Place Fighters in the Scene

You need two instances — one per player.

1. Drag the `ExampleFighter` prefab from `_FighterBase/Prefabs/` into the scene
2. Rename it `ExampleFighter_P1`
3. Set Position to `(-3, -2, 0)` (same as SpawnPoint_P1)
4. In the **Example Fighter** component: Fighter Color = pick a color (blue, etc.)

5. Drag the prefab again, rename it `ExampleFighter_P2`
6. Set Position to `(3, -2, 0)`
7. In **Example Fighter**: Fighter Color = a different color (red, etc.)

> **Important:** You don't need to set Player Index on each fighter manually. The GameManager's **Fighters** array controls this — Element 0 = Player 1 (WASD/Gamepad1), Element 1 = Player 2 (Arrows/Gamepad2). The index is assigned automatically when the match starts.

---

## Step 7: Add Singletons

### GameManager

1. **GameObject > Create Empty**, name it `GameManager`
2. Add Component: **GameManager** (namespace: Brawler.Core)
3. Wire these references in the Inspector:
   - **Match Config**: drag in `DefaultMatchConfig`
   - **Spawn Points**: set array size to 2
     - Element 0: drag `SpawnPoint_P1`
     - Element 1: drag `SpawnPoint_P2`
   - **Fighters**: set array size to 2
     - Element 0: drag `ExampleFighter_P1`
     - Element 1: drag `ExampleFighter_P2`

> **Critical:** All three fields must be assigned. `StartMatch()` initializes fighters with their input and movement systems — if MatchConfig or Fighters are missing, nothing works. The Fighters array order determines player index: Element 0 = Player 1, Element 1 = Player 2.
>
> Match flow (rounds, KOs, win conditions) won't work until the team wires the TODO steps in Guide 02. But movement, attacks, and combat work immediately.

### HitstopManager

1. **GameObject > Create Empty**, name it `HitstopManager`
2. Add Component: **HitstopManager** (namespace: Brawler.Combat)
3. Leave defaults (0.05s default duration, 1x multiplier)

---

## Step 8: Organize the Hierarchy

Clean organization helps the whole team. Drag objects to match this order:

```
DevArena
├── Main Camera
├── --- MANAGERS ---        (empty separator object, optional)
│   ├── GameManager
│   └── HitstopManager
├── --- ARENA ---
│   ├── Ground
│   ├── BlastZone_Bottom
│   ├── BlastZone_Top
│   ├── BlastZone_Left
│   ├── BlastZone_Right
│   ├── SpawnPoint_P1
│   └── SpawnPoint_P2
└── --- FIGHTERS ---
    ├── ExampleFighter_P1
    └── ExampleFighter_P2
```

> **Tip:** Create empty GameObjects named `--- MANAGERS ---`, `--- ARENA ---`, and `--- FIGHTERS ---` as section headers. They do nothing but make the hierarchy readable.

---

## Step 9: Smoke Test

**Save the scene** (Ctrl+S), then press **Play**.

### What should work

- [ ] **P1 moves** with WASD, jumps with Space
- [ ] **P2 moves** with Arrow keys, jumps with Right Ctrl or Numpad 0
- [ ] Both fighters **fall and land** on the ground platform
- [ ] **P1 attacks** with J — you should see a hitbox gizmo flash in the Scene view
- [ ] **P2 attacks** with Numpad 1
- [ ] **Hits connect** — check the Console for damage logs like `[Example] Took 8 damage!`
- [ ] Fighters **flash red** on hit (ExampleFighter's damage flash)
- [ ] A brief **freeze frame** happens on hit (hitstop)
- [ ] Fighters get **knocked back** when hit, scaling with damage taken
- [ ] Fighters that go off-screen into blast zones trigger a KO log in Console

### What won't work yet (that's expected)

- Rounds, countdowns, respawns — GameManager scaffold TODOs (Guide 02)
- Health bars, round display, announcements — UI scaffold TODOs (Guide 03)
- Drop-through platforms — Platform scaffold TODO (Guide 02)
- Animations — no animator controllers yet (Guide 06)

### Common issues

| Problem | Fix |
|---------|-----|
| Fighter falls through ground | Ground's **Layer** isn't set to `Ground`, or MovementConfig's **Ground Layer** doesn't include it. Also check the ground has a non-trigger BoxCollider2D. |
| Fighter doesn't jump | **Ground Check Point** not wired in FighterMovement, or GroundCheck child is positioned wrong (should be at the feet). |
| No movement at all | Check that GameManager has both fighters assigned in its **Fighters** array — `StartMatch()` initializes movement and attacks. Also check **Input Actions** field is set on PlayerInputHandler. |
| P2 doesn't respond | Check the **Fighters** array on GameManager — P1 must be index 0, P2 index 1. The GameManager sets each fighter's player index automatically at match start based on array position. |
| Attacks don't hit | Both fighters need a Hurtbox child with a trigger collider. Check it exists and **Is Trigger** is on. |
| No hitstop on hit | HitstopManager isn't in the scene. Add it (Step 7). |
| Console says "MatchConfig not assigned" | Wire **DefaultMatchConfig** on the GameManager. This is **critical** — `StartMatch()` aborts before initializing fighters if MatchConfig is missing, so nothing will move. |

---

## Step 10: Save, Commit, and Invite the Team

Once the smoke test passes, lock it in.

### Save everything

1. **Ctrl+S** to save the scene
2. **File > Save Project** to save all asset changes

### Commit and push

```bash
git add -A
git commit -m "Add DevArena scene with ExampleFighter and arena setup"
git push origin main
```

### Invite the team

1. Go to your fork on GitHub
2. **Settings > Collaborators > Add people**
3. Add each team member by GitHub username or email
4. Team members accept the invite, then clone:

```bash
git clone https://github.com/YOUR-TEAM/BrawlerTemplate.git
```

When they open the project in Unity and load `DevArena`, everything runs. They can immediately move fighters, test attacks, and start building.

---

## What the Team Gets

When your teammates clone and open the project, they have:

- A **playable scene** with two fighters that move, jump, and attack
- **ScriptableObject assets** they can duplicate and tweak (attack data, movement config, match config)
- **5 scaffold scripts** with labeled TODOs ready to wire (GameManager, HealthBarUI, RoundDisplayUI, MatchUI, Platform)
- **ExampleFighter prefab** they can duplicate as a starting point for Fighter1 and Fighter2
- **6 guides** explaining every system

### Suggested next steps for the team

| Who | Task | Guide |
|-----|------|-------|
| GameManager owner | Wire the 5 TODO steps in GameManager.cs | [Guide 02](02_MatchFlowAndArena.md) |
| UI owner | Build the Canvas and wire HealthBarUI, RoundDisplayUI, MatchUI | [Guide 03](03_UISystem.md) |
| Fighter 1 team | Extend FighterBase, create attacks, start on art | [Guide 04](04_FighterArchitecture.md), [Guide 05](05_CombatSystem.md) |
| Fighter 2 team | Same as Fighter 1 | [Guide 04](04_FighterArchitecture.md), [Guide 05](05_CombatSystem.md) |

---

## Guide Index

1. **You are here** — Scene Setup
2. [Getting Started](01_GettingStarted.md) — Fork, clone, project structure
3. [Match Flow and Arena](02_MatchFlowAndArena.md) — GameManager scaffold, arena systems
4. [UI System](03_UISystem.md) — Event-driven UI scaffolds
5. [Fighter Architecture](04_FighterArchitecture.md) — Extending FighterBase
6. [Combat System](05_CombatSystem.md) — Attacks, hitboxes, knockback
7. [Art Reference](06_ArtReference.md) — Sprites, animators, prefab structure
