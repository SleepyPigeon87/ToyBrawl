# Brawler Template - Claude Context Document

> **Last Updated**: 2026-02-17
> **Purpose**: Context restoration for Claude Code sessions
> **Project**: SCAD Applied Programming - Brawler Template

---

## Quick Summary

This is a **Smash Bros-style 2-player arena fighter template** for teaching game development. Teams of 4 students fork the template and build their own brawler. The template provides complete combat, input, and arena systems. Five scaffold scripts (GameManager, 3 UI scripts, Platform) have TODO comments for students to complete. Students design their own game rules, create fighters by extending `FighterBase`, and organize their team however they choose.

**Session 1 workflow:** Tech lead follows `01_SceneSetup.md` to build the DevArena scene (assets, prefab, arena, singletons) while the team fills out `DESIGN_DOC.md`. Once the smoke test passes, the dev lead commits, pushes, and invites the team. Everyone clones a project that runs on first open.

---

## Project Locations

- **This Project**: `C:\SCAD\Projects\BrawlerTemplate`
- **Reference Project**: `C:\SCAD\Projects\PlatformerTemplate` (previous template, can borrow code)
- **GitHub**: https://github.com/profangrybeard/BrawlerTemplate

---

## Tech Stack

- **Unity Version**: 6000.0.63f1 (Unity 6)
- **Input System**: New Unity Input System (not legacy)
- **Rendering**: 2D (SpriteRenderer, not 3D)
- **Physics**: Rigidbody2D with direct velocity control
- **Architecture**: Component-based with ServiceLocator pattern

---

## Game Design Decisions (Template Defaults)

These are the template's starting defaults. Teams may modify or replace them via their DESIGN_DOC.md.

| Decision | Default | Rationale |
|----------|---------|-----------|
| Health System | Health bars with knockback vulnerability | Easier to teach than percentage-based |
| Knockback Formula | `knockback = base * (2 - healthPercent)` | 1x at full health, 2x at zero health |
| Win Condition | Ring-out via blast zones | Avoids violence, promotes physics play |
| Match Format | Best of 3 rounds (first to 2 wins) | Quick matches for testing |
| Time Limit | Configurable (default: none) | Students can add if desired |
| Attack Types | Ground + Aerial (standard Smash-like) | Familiar to students |
| Hitstop | Yes, included | Essential for game feel |

---

## Team Structure

Each game is built by a **team of 4**. Teams self-organize with no prescribed roles — they decide who works on what. Students fork the template on GitHub, with the tech lead creating the fork and inviting teammates as collaborators.

- Shared repository (forked from upstream template)
- Work split decided by the team
- Artists work directly in Unity (prefabs, animators)
- Students can bring Platformer code or start fresh
- Fighter1/ and Fighter2/ are organizational folders for the two fighter characters, not team assignments

---

## Folder Structure

```
Assets/_Project/
├── _Shared/                    Shared systems (team manages together)
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── ServiceLocator.cs      ✓ Complete
│   │   │   ├── GameManager.cs         ⚠ SCAFFOLD
│   │   │   ├── GameEvents.cs          ✓ Complete
│   │   │   └── MatchConfig.cs         ✓ Complete
│   │   ├── Combat/
│   │   │   ├── Hitbox.cs              ✓ Complete
│   │   │   ├── Hurtbox.cs             ✓ Complete
│   │   │   ├── KnockbackHandler.cs    ✓ Complete
│   │   │   └── HitstopManager.cs      ✓ Complete
│   │   ├── Arena/
│   │   │   ├── BlastZone.cs           ✓ Complete
│   │   │   ├── SpawnPoint.cs          ✓ Complete
│   │   │   └── Platform.cs            ⚠ SCAFFOLD
│   │   ├── UI/
│   │   │   ├── HealthBarUI.cs         ⚠ SCAFFOLD
│   │   │   ├── RoundDisplayUI.cs      ⚠ SCAFFOLD
│   │   │   └── MatchUI.cs             ⚠ SCAFFOLD
│   │   └── Input/
│   │       ├── PlayerInputHandler.cs  ✓ Complete
│   │       └── InputConfig.cs         ✓ Complete
│   ├── Configs/                       (ScriptableObjects go here)
│   ├── Prefabs/Arena/
│   ├── Prefabs/UI/
│   └── Scenes/                        (DevArena.unity goes here)
│
├── _FighterBase/               Reference implementation (DO NOT MODIFY)
│   ├── Scripts/
│   │   ├── FighterBase.cs             ✓ Complete (abstract class)
│   │   ├── FighterHealth.cs           ✓ Complete
│   │   ├── FighterMovement.cs         ✓ Complete
│   │   ├── FighterAnimator.cs         ✓ Complete
│   │   ├── AttackController.cs        ✓ Complete
│   │   ├── AttackData.cs              ✓ Complete
│   │   ├── MovementConfig.cs          ✓ Complete
│   │   └── ExampleFighter.cs          ✓ Complete
│   ├── Configs/
│   │   └── ExampleAttacks/            (AttackData assets go here)
│   └── Prefabs/                       (ExampleFighter.prefab goes here)
│
├── Fighter1/                   First fighter character
│   ├── Scripts/
│   ├── Art/Sprites/
│   ├── Art/Animations/
│   ├── Configs/Attacks/
│   └── Prefabs/
│
├── Fighter2/                   Second fighter character
│   ├── Scripts/
│   ├── Art/Sprites/
│   ├── Art/Animations/
│   ├── Configs/Attacks/
│   └── Prefabs/
│
└── Guides/
    ├── 01_SceneSetup.md           Tech lead builds DevArena (Session 1)
    ├── 01_GettingStarted.md       Fork, clone, project overview
    ├── 02_MatchFlowAndArena.md
    ├── 03_UISystem.md
    ├── 04_FighterArchitecture.md
    ├── 05_CombatSystem.md
    └── 06_ArtReference.md
```

---

## Key Scripts Reference

### FighterBase.cs (Abstract - Students Extend This)

```csharp
public abstract class FighterBase : MonoBehaviour
{
    public abstract string FighterName { get; }
    public int PlayerIndex => playerIndex;
    public int FacingDirection { get; protected set; } = 1;
    public bool CanAct => !Knockback.IsInHitstun && !IsDead && !IsRespawning;

    // Students override these
    protected virtual void OnTakeDamage(float damage) { }
    protected virtual void OnKO() { }
    protected virtual void OnRespawn(Vector2 position) { }
}
```

### FighterHealth.cs (Knockback Vulnerability)

```csharp
public float KnockbackMultiplier => 2f - HealthPercent; // 1x at full, 2x at zero
public event Action<float, float> OnHealthChanged; // oldHealth, newHealth
```

### GameEvents.cs (Static Event Hub)

```csharp
public static class GameEvents
{
    public static Action<FighterKOEventArgs> OnFighterKO;
    public static Action<FighterDamageEventArgs> OnFighterDamaged;
    public static Action<int> OnRoundStart;      // roundNumber
    public static Action<int> OnRoundEnd;        // winnerIndex
    public static Action<int> OnMatchEnd;        // winnerIndex
    public static Action<GameState> OnGameStateChanged;
    public static Action<int, int> OnRoundScoreChanged; // playerIndex, newScore
}

public enum GameState { Waiting, Countdown, Fighting, RoundEnd, MatchEnd, Paused }
```

### AttackData.cs (ScriptableObject)

```csharp
[CreateAssetMenu(menuName = "Brawler/Attack Data")]
public class AttackData : ScriptableObject
{
    public float damage = 10f;
    public float baseKnockback = 5f;
    public Vector2 knockbackAngle = new Vector2(1f, 0.5f);
    public int startupFrames = 3;
    public int activeFrames = 5;
    public int recoveryFrames = 10;
    public float hitstunDuration = 0.2f;
    public float hitstopDuration = 0.05f;
    public Vector2 hitboxOffset;
    public Vector2 hitboxSize;
}
```

---

## Input Bindings

**Player 1 (Keyboard)**
- Move: WASD
- Jump: Space
- Attack: J
- Special: K
- Dash: Left Shift

**Player 2 (Keyboard)**
- Move: Arrow Keys
- Jump: Right Ctrl or Numpad 0
- Attack: Numpad 1
- Special: Numpad 2
- Dash: Numpad 3

**Gamepad (Either Player)**
- Move: Left Stick
- Jump: South Button (A/X)
- Attack: West Button (X/Square)
- Special: North Button (Y/Triangle)
- Dash: Right Trigger

---

## Scaffold Pattern

Scaffolded files contain TODO comments. Note: the TODO comments reference old guide names ("Lesson 02", "Lesson 03") which map to current guides:

| TODO References | Current Guide |
|----------------|---------------|
| "Lesson 02" / "Wiring GameManager" | Guide 02: Match Flow and Arena |
| "Lesson 03" / "Wiring UI" | Guide 03: UI System |

```csharp
public class GameManager : MonoBehaviour
{
    // TODO: See Lesson 02_WiringGameManager.md
    // (Now: Guide 02_MatchFlowAndArena.md)

    // STEP 1: Subscribe to GameEvents.OnFighterKO in OnEnable
    // STEP 2: Position fighters at spawn points in StartRound
    // STEP 3: Implement countdown coroutine
    // ...
}
```

---

## What's Complete vs What Students Do

| System | Status | Student Action |
|--------|--------|----------------|
| ServiceLocator | ✓ Complete | Use as-is |
| FighterBase | ✓ Complete | Extend it |
| FighterHealth | ✓ Complete | Use as-is |
| KnockbackHandler | ✓ Complete | Use as-is |
| HitstopManager | ✓ Complete | Use as-is |
| Hitbox/Hurtbox | ✓ Complete | Use as-is |
| BlastZone | ✓ Complete | Place in scene |
| SpawnPoint | ✓ Complete | Place in scene |
| FighterMovement | ✓ Complete | Use OR replace with own |
| AttackController | ✓ Complete | Use OR replace with own |
| PlayerInputHandler | ✓ Complete | Use as-is |
| **GameManager** | ⚠ Scaffold | Wire up (Guide 02) |
| **HealthBarUI** | ⚠ Scaffold | Wire up (Guide 03) |
| **RoundDisplayUI** | ⚠ Scaffold | Wire up (Guide 03) |
| **MatchUI** | ⚠ Scaffold | Wire up (Guide 03) |
| **Platform** | ⚠ Scaffold | Complete drop-through |

---

## Scene Setup Status

The `01_SceneSetup.md` guide walks the tech lead through creating all Unity-side assets. After completing it, the project has:

### Created by Tech Lead (Session 1)
- `_Shared/Scenes/DevArena.unity` — playable arena with blast zones, spawn points, ground
- `_Shared/Configs/DefaultMatchConfig.asset` — match rules
- `_Shared/Configs/DefaultInputConfig.asset` — input deadzone/buffering
- `_FighterBase/Configs/DefaultMovementConfig.asset` — movement tuning
- `_FighterBase/Configs/ExampleAttacks/ExampleJab.asset` — neutral attack
- `_FighterBase/Configs/ExampleAttacks/ExampleForwardPunch.asset` — forward attack
- `_FighterBase/Prefabs/ExampleFighter.prefab` — working reference fighter
- Ground layer for physics raycasts

### Still Created by Students
- Fighter1 and Fighter2 scripts, prefabs, art, animations, and AttackData
- Additional AttackData (Up, Down, Aerial) for ExampleFighter if desired
- UI Canvas with HealthBarUI, RoundDisplayUI, MatchUI
- Additional arena platforms

---

## Git Status

- **Branch**: main
- **Remote**: origin (GitHub)
- **Last Push**: 2026-02-15
- **Students fork the repo** — upstream stays clean as template

---

## Namespaces Used

All scripts use the `Brawler` namespace with sub-namespaces:

- `Brawler.Core` - ServiceLocator, GameManager, GameEvents, MatchConfig
- `Brawler.Combat` - Hitbox, Hurtbox, KnockbackHandler, HitstopManager
- `Brawler.Fighters` - FighterBase, FighterHealth, FighterMovement, etc.
- `Brawler.Arena` - BlastZone, SpawnPoint, Platform
- `Brawler.Input` - PlayerInputHandler, InputConfig
- `Brawler.UI` - HealthBarUI, RoundDisplayUI, MatchUI

---

## Common Patterns

### Event Subscription
```csharp
private void OnEnable()
{
    GameEvents.OnFighterKO += HandleFighterKO;
}

private void OnDestroy()
{
    GameEvents.OnFighterKO -= HandleFighterKO;
}
```

### Component Access
```csharp
// FighterBase provides these to subclasses
protected FighterHealth Health { get; private set; }
protected KnockbackHandler Knockback { get; private set; }
protected PlayerInputHandler Input { get; private set; }
```

### Direct Velocity Control
```csharp
// Movement uses direct velocity, not AddForce
rb.linearVelocity = new Vector2(horizontalSpeed, rb.linearVelocity.y);
```

---

## Notes for Future Sessions

1. **Unity 6 API Changes**: Uses `rb.linearVelocity` not `rb.velocity`
2. **Input System**: New Input System only, no legacy Input.GetKey
3. **No Time Estimates**: Never give timeline estimates to user
4. **Scaffold Pattern**: TODOs in C# reference old guide names ("Lesson 02/03") — see mapping table above
5. **Team Philosophy**: No prescribed roles. Teams self-organize. Fighter1/ and Fighter2/ are organizational convention, not team assignments.
6. **Game Rules**: Template defaults are starting points. Teams customize via DESIGN_DOC.md.
7. **Session 1 Flow**: Tech lead does scene setup (01_SceneSetup.md) while team does design (DESIGN_DOC.md). Dev lead commits, pushes, and invites team after smoke test passes.

---

## User Preferences (Prof. Angrybeard)

- Prefers concise responses
- Teaching at SCAD (Savannah College of Art and Design)
- Applied Programming class
- Teams of 4 students, self-organized (no prescribed roles)
- Values clear folder organization for project management
- Wants students to be able to bring Platformer code or start fresh
- Wants students to design their own game rules, not just wire up prescriptive templates
