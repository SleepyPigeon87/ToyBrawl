# BrawlerTemplate

GAME 325 — 2-player arena fighting game template built in Unity 6.

Your team of 4 forks this repo and builds your own brawler. You decide the rules, the fighters, the art, and the team structure. The template gives you working combat, input, and arena systems to build on top of.

---

## Getting Started

### 1. Fork the Repo (Tech Lead)

1. Go to [https://github.com/profangrybeard/BrawlerTemplate](https://github.com/profangrybeard/BrawlerTemplate)
2. Click **Fork** (top right)
3. Create the fork under your own GitHub account

### 2. Invite Your Team (Tech Lead)

1. Open your fork on GitHub
2. Go to **Settings** > **Collaborators** (left sidebar, under "Access")
3. Click **Add people**
4. Search for each teammate by GitHub username and invite them
5. Each teammate accepts the invite from their email or GitHub notifications

### 3. Clone Your Team's Fork (Everyone)

```bash
git clone https://github.com/YOUR-TEAM-LEAD/BrawlerTemplate.git
```

> **Important:** Clone your team's fork, not the upstream template repo.

### 4. Open in Unity

- **Unity Version:** `6000.0.63f1` (Unity 6)
- Open via Unity Hub > **Add** > browse to the cloned folder
- First import takes a few minutes — let it finish

### 5. Start Building

- Open `DESIGN_DOC.md` in the repo root and fill it out as a team
- Read the guides in `Assets/_Project/Guides/` to understand the systems
- Decide how your team wants to divide the work — there are no restrictions

---

## What's in the Template

### Complete Systems (use as-is or extend)

| System | What It Does |
|--------|-------------|
| `FighterBase` | Abstract base class you extend for each fighter |
| `FighterHealth` | Tracks health, calculates knockback vulnerability |
| `KnockbackHandler` | Applies knockback physics with directional control |
| `HitstopManager` | Freeze frames on hit for game feel |
| `Hitbox` / `Hurtbox` | Combat collision detection |
| `BlastZone` | KOs fighters who cross arena boundaries |
| `SpawnPoint` | Marks fighter spawn positions |
| `PlayerInputHandler` | 2-player input (keyboard + gamepad) |
| `AttackData` | ScriptableObject for defining attack properties |
| `AttackController` | Reads input context and activates attacks |
| `FighterMovement` | Default platformer movement (use or replace) |
| `FighterAnimator` | Bridges game state to Animator parameters |
| `MovementConfig` | ScriptableObject for movement tuning |
| `MatchConfig` | ScriptableObject for match rules |
| `InputConfig` | ScriptableObject for input buffering/deadzones |
| `ServiceLocator` | Simple service registry pattern |
| `GameEvents` | Static event bus for game-wide communication |

### Scaffold Systems (have TODOs for your team to complete)

| System | What You Do |
|--------|-------------|
| `GameManager` | Wire up match flow: rounds, KOs, win conditions |
| `HealthBarUI` | Connect to FighterHealth events |
| `RoundDisplayUI` | Connect to round score events |
| `MatchUI` | Wire countdown, state display, winner announcement |
| `Platform` | Complete drop-through logic |

### You Create

- Your fighter scripts (extend `FighterBase`)
- Your attack data (ScriptableObjects)
- Your animations and sprites
- Your game rules and unique mechanics
- Your arena layout

---

## Folder Structure

```
Assets/_Project/
├── _Shared/            Shared game systems (managed by your team together)
│   ├── Scripts/
│   │   ├── Core/       GameManager, GameEvents, MatchConfig, ServiceLocator
│   │   ├── Combat/     Hitbox, Hurtbox, HitstopManager, KnockbackHandler
│   │   ├── Arena/      BlastZone, SpawnPoint, Platform
│   │   ├── UI/         HealthBarUI, MatchUI, RoundDisplayUI
│   │   └── Input/      PlayerInputHandler, InputConfig
│   ├── Prefabs/
│   ├── Configs/
│   └── Scenes/
│
├── _FighterBase/       Reference implementation — read, don't modify
│   ├── Scripts/        FighterBase, FighterHealth, AttackController, etc.
│   └── Prefabs/
│
├── Fighter1/           First fighter character
│   ├── Scripts/
│   ├── Art/
│   ├── Prefabs/
│   └── Configs/
│
├── Fighter2/           Second fighter character
│   ├── Scripts/
│   ├── Art/
│   ├── Prefabs/
│   └── Configs/
│
└── Guides/             Reference documentation
```

---

## Default Controls

**Player 1 (Keyboard):**
| Action | Key |
|--------|-----|
| Move | WASD |
| Jump | Space |
| Attack | J |
| Special | K |
| Dash | Left Shift |

**Player 2 (Keyboard):**
| Action | Key |
|--------|-----|
| Move | Arrow Keys |
| Jump | Right Ctrl / Numpad 0 |
| Attack | Numpad 1 |
| Special | Numpad 2 |
| Dash | Numpad 3 |

**Gamepad:**
| Action | Button |
|--------|--------|
| Move | Left Stick |
| Jump | A (South) |
| Attack | X (West) |
| Special | Y (North) |
| Dash | Right Trigger |

---

## Guides

1. [Getting Started](Assets/_Project/Guides/01_GettingStarted.md) — Setup, project structure, first team meeting
2. [Match Flow and Arena](Assets/_Project/Guides/02_MatchFlowAndArena.md) — How the match system and arena work
3. [UI System](Assets/_Project/Guides/03_UISystem.md) — How UI connects to game events
4. [Fighter Architecture](Assets/_Project/Guides/04_FighterArchitecture.md) — Extending FighterBase, movement, attacks
5. [Combat System](Assets/_Project/Guides/05_CombatSystem.md) — Hitboxes, hurtboxes, attack data, knockback
6. [Art Reference](Assets/_Project/Guides/06_ArtReference.md) — Prefab setup, animator controllers, sprites
