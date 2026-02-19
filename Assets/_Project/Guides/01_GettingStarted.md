# Guide 01: Getting Started

This guide walks your team through setup and gives you a roadmap for the project.

---

## Fork and Clone

If you haven't done this yet, follow the steps in the [README](../../../README.md#getting-started).

Quick version:
1. **Tech lead** forks [profangrybeard/BrawlerTemplate](https://github.com/profangrybeard/BrawlerTemplate) on GitHub
2. **Tech lead** invites teammates: fork Settings > Collaborators > Add people
3. **Everyone** clones the team's fork: `git clone https://github.com/YOUR-LEAD/BrawlerTemplate.git`
4. Open in Unity Hub > Add > browse to cloned folder
5. **Unity version: 6000.0.63f1**

### GitHub Collaboration Tips

- **Always pull before you push:** `git pull` before starting work each session
- **Use branches** for big features to avoid stepping on each other
- **Commit often** with clear messages
- If you get merge conflicts in `.unity` scene files, coordinate — only one person should edit a scene at a time

---

## Project Structure

```
Assets/_Project/
├── _Shared/            Shared game systems — your team manages these together
│   ├── Scripts/        Core, Combat, Arena, UI, Input
│   ├── Prefabs/        Shared prefabs (arena, UI)
│   ├── Configs/        ScriptableObject assets (MatchConfig, InputConfig)
│   └── Scenes/         Game scenes
│
├── _FighterBase/       Reference code — read it, learn from it, don't edit it
│   ├── Scripts/        FighterBase, FighterHealth, AttackController, etc.
│   ├── Prefabs/        ExampleFighter prefab
│   └── Configs/        Example attack data
│
├── Fighter1/           First fighter character
│   ├── Scripts/        Fighter1's custom code
│   ├── Art/            Sprites and animations
│   ├── Prefabs/        Fighter1 prefab
│   └── Configs/        Attack data ScriptableObjects
│
├── Fighter2/           Second fighter character
│   └── (same structure as Fighter1)
│
└── Guides/             You are here
```

**Key points:**
- `_FighterBase/` is reference code. Don't modify these scripts — extend them instead.
- `_Shared/` is where the core game systems live. Your team decides who works on what in here.
- `Fighter1/` and `Fighter2/` are for your two fighter characters. Use them to keep your project organized.
- Need more folders? Create them. `Fighter3/`, `Stages/`, `Audio/` — whatever your game needs.

---

## What's Already Built vs. What You Build

### Complete (use as-is or extend)
The combat system, input handling, knockback physics, blast zones, spawn points, and the FighterBase class are all working. Read the code to understand how they work, then build on top of them.

### Scaffold (TODOs for your team)
Five scripts have TODO comments that walk you through wiring them up:
- **GameManager** — match flow, rounds, KO handling
- **HealthBarUI**, **RoundDisplayUI**, **MatchUI** — connecting UI to game events
- **Platform** — drop-through logic

Open these files and read the comments. Your team decides who handles each one.

### You Create
- Fighter scripts (extend `FighterBase`)
- Attack data (ScriptableObjects)
- Art, animations, sprites
- Your game's unique rules and mechanics
- Arena layouts

---

## A Note on Scaffold Script References

The TODO comments in the scaffold scripts reference guide names like "Lesson 02" and "Lesson 03." These map to the current guides:

| Script Says | See This Guide |
|-------------|---------------|
| "Lesson 02" / "Wiring GameManager" | [Guide 02: Match Flow and Arena](02_MatchFlowAndArena.md) |
| "Lesson 03" / "Wiring UI" | [Guide 03: UI System](03_UISystem.md) |

---

## First Team Meeting

Here are some things to discuss. These are suggestions, not requirements — organize however works for your team.

- **Fill out `DESIGN_DOC.md`** in the repo root. Decide your game's rules, fighter concepts, and art direction.
- **Look at ExampleFighter.** Open `_FighterBase/Scripts/ExampleFighter.cs` to see how a fighter extends `FighterBase`. This is your starting pattern.
- **Decide who works on what.** Some options:
  - Split by system (one person on GameManager/UI, one on fighters, one on art, one on polish)
  - Split by fighter (two people per fighter, handling code + art)
  - Everyone touches everything, coordinate as you go
  - Whatever else makes sense
- **Decide on your movement approach.** Each fighter can use the default `FighterMovement`, bring code from the Platformer project, or build something new.

---

## Guide Index

1. [Scene Setup](01_SceneSetup.md) — Tech lead builds the DevArena scene (do this first)
2. **You are here** — Getting Started
3. [Match Flow and Arena](02_MatchFlowAndArena.md) — How the match system and arena work
4. [UI System](03_UISystem.md) — How UI connects to game events
5. [Fighter Architecture](04_FighterArchitecture.md) — Extending FighterBase, movement options, attacks
6. [Combat System](05_CombatSystem.md) — Hitboxes, hurtboxes, attack data, knockback
7. [Art Reference](06_ArtReference.md) — Prefab setup, animator controllers, sprites
