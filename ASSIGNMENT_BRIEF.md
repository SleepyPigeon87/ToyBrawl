# Arena Brawler – Combat & Feel (Team)

GAME 326 – Applied Principles: Programming
Professor: Tim Lindsey
Assignment Due: March 12, 2026 (In-Class Live Playtest)

---

## Assignment Overview

In this assignment, your team of 4 will work from a provided Unity template to build a 2-player local arena fighter focused on combat feel, event-driven architecture, and game state management.

**This is not a clone exercise.**

You are not being graded on art quality, character count, or feature bloat. You are being evaluated on whether your team understands how game systems communicate through events, how combat frame data creates feel, and how state management determines when a player can and cannot act.

**You will design your own game rules, create two unique fighters, and build an arena that showcases both.**

A fighter that works but feels bad is incomplete. A fighter that feels good demonstrates understanding. Two fighters that play differently from each other demonstrate mastery.

**This is a team assignment (4 students).**

No prescribed roles. Your team decides who works on what. Everyone contributes code. Everyone contributes to feel.

**Template Repository:**
[https://github.com/profangrybeard/BrawlerTemplate](https://github.com/profangrybeard/BrawlerTemplate)

---

## Core Goal (Non-Negotiable)

Your game must include:

- A working match system (rounds, KOs, win condition)
- **Two unique fighters** with distinct movesets and feel
- An arena with blast zones, spawn points, and at least one platform
- Animations that communicate fighter state
- Visual and audio feedback that makes hits feel impactful (hitstop, screen shake, particles, or equivalent juice)

Both fighters must be playable and feel intentionally different. If both fighters play identically, the assignment is incomplete. Your game rules must be documented in your `DESIGN_DOC.md` — if you changed the template defaults, you must be able to explain why.

---

## Session Schedule

| Session | Date | Focus |
|---------|------|-------|
| 1 | Tue, Feb 17 | **Design & Setup** — Fork repo, form teams, fill out DESIGN_DOC.md, plan game rules |
| 2 | Thu, Feb 19 | **Design & Setup** — Finalize game rules, begin repo setup, assign initial work |
| 3 | Tue, Feb 24 | **Event-Driven Architecture** — GameEvents, observer pattern, wiring GameManager |
| 4 | Thu, Feb 26 | **Combat Frame Data** — Hitbox/Hurtbox lifecycle, startup/active/recovery, AttackData pipeline |
| 5 | Tue, Mar 3 | **Game Feel: Hitstop, Hitstun & Knockback** — Freeze frames, hit reactions, knockback scaling |
| 6 | Thu, Mar 5 | **State Machines & Action Gating** — CanAct, attack states, when players lose control and why |
| 7 | Tue, Mar 10 | **Integration & Polish** — Final wiring, playtesting, balancing, juice pass |
| 8 | Thu, Mar 12 | **Live Playtest & Presentation** (5:00 PM) |

Sessions 3–6 introduce new coding concepts. You are expected to be working on your game outside of class throughout.

---

## Required Architectural Understanding

You are expected to understand, use, and extend the following systems from the template:

- **GameEvents** — How systems communicate without direct references (observer pattern)
- **FighterBase** — How fighters are structured and extended
- **AttackData / AttackController** — How attacks are defined as data and executed as frame-timed sequences
- **FighterHealth / KnockbackHandler** — How damage and knockback interact through the health multiplier
- **HitstopManager** — How freeze frames create impact feel
- **GameManager (scaffold)** — How match state flows from countdown to KO to winner

You must be able to explain what each system does, why it exists, and how a button press becomes an attack that hits an opponent and sends them flying.

---

## New Concepts (Sessions 3–6)

These are the four coding topics taught during this assignment. Your game must demonstrate understanding of each.

| Concept | What You Learn | Where It Shows Up |
|---------|---------------|-------------------|
| **Event-Driven Architecture** | Systems communicate through events, not direct references. Subscribe, fire, react. | GameManager wiring, UI connections, KO handling |
| **Combat Frame Data** | Attacks have startup, active, and recovery phases. Timing is game design. | AttackData ScriptableObjects, hitbox lifecycle, move design |
| **Impact Feel (Hitstop/Hitstun/Knockback)** | Freeze frames sell hits. Hitstun removes control. Knockback scales with damage. | HitstopManager, KnockbackHandler, the knockback formula |
| **State Machines & Action Gating** | Players can't always act. CanAct governs when input is accepted. States have priority. | Hitstun lockout, attack commitment, state transitions |

---

## Content Requirements

### Code Requirements

| Requirement | Description |
|-------------|-------------|
| Two fighter scripts | Each extends FighterBase with a unique FighterName and distinct behavior |
| Attack data sets | At least 3 attacks per fighter, defined as AttackData ScriptableObjects |
| GameManager wired | Match flow works: countdown, fighting, round end, match end |
| UI wired | At least HealthBarUI connected to FighterHealth events |
| Event usage | Systems communicate through GameEvents, not direct references |
| Config ScriptableObjects | Tunable values for movement and match rules (MovementConfig, MatchConfig) |

### Fighter Design Requirements

| Requirement | Description |
|-------------|-------------|
| Distinct feel | Fighters must play differently (speed vs power, range vs aggression, etc.) |
| Unique attacks | Each fighter has attacks with different frame data, knockback, and hitbox placement |
| Movement identity | Fighters may share FighterMovement or use different movement configurations |
| Design rationale | You can explain why each fighter feels the way it does |

### Arena Requirements

| Requirement | Description |
|-------------|-------------|
| Blast zones | 4 blast zones defining the arena boundaries |
| Spawn points | 2 spawn points (one per player) |
| Platform(s) | At least one platform with drop-through support |
| Playable space | Arena feels intentional — not too cramped, not too open |

### Animation Requirements

| Requirement | Description |
|-------------|-------------|
| Core states | Idle, Run, Jump, Fall for both fighters |
| Attack states | Animations for at least 3 attacks per fighter |
| Hit reaction | Hitstun animation when taking damage |
| FighterAnimator | Drives Animator parameters from gameplay state |
| Clean transitions | No animation pops or broken blends between states |

### Juice Requirements

At least **THREE** of the following (combat needs feedback):

- Hitstop on hit (provided in template — must be tuned)
- Screen shake on heavy attacks or KOs
- Particle effects (hit sparks, dust, trails)
- Damage flash or color tint on hit
- Sound effects for attacks, hits, and KOs
- Knockback trails or motion blur
- UI reactions (health bar shake, screen flash)

---

## Rules & Constraints

### Required

- Events are the primary communication channel between systems (GameEvents pattern)
- Attack timing uses frame data (startup/active/recovery), not arbitrary timers
- Tunable values live in ScriptableObject configs, not hardcoded in MonoBehaviours
- Both keyboard and gamepad input must work (template provides this)
- Fighters extend FighterBase — do not bypass the base class architecture
- Animations respond to actual gameplay state, not faked

### Forbidden

- Direct references between fighters (fighters should not know about each other)
- Hardcoded damage, knockback, or timing values in scripts
- Attacks that bypass the Hitbox/Hurtbox collision system
- Animations that don't match gameplay state (e.g., attack animation playing but no hitbox active)
- Game rules that exist only in code with no documentation (if you changed the defaults, it must be in DESIGN_DOC.md)
- One teammate doing all the work (commit history tells the story)

---

## Student Learning Outcomes

By completing this assignment, students will demonstrate the ability to:

1. **Design and implement event-driven architecture** — Use the observer pattern to decouple game systems that need to communicate.
2. **Create combat systems with frame-based timing** — Define attacks as data with startup, active, and recovery phases that directly shape game feel.
3. **Implement impact feel through hitstop, hitstun, and knockback** — Apply freeze frames, hit reactions, and physics-based knockback that scales with damage.
4. **Manage game state and action gating** — Control when players can and cannot act, and handle transitions between states cleanly.
5. **Extend an existing codebase as a team** — Read, understand, and build on a provided template while preserving architectural intent across 4 contributors.
6. **Document and communicate design decisions** — Articulate why your game's rules, fighter designs, and technical choices exist.
7. **Ship a playable game** — Deliver a complete, testable 2-player experience that runs from a fresh clone.

---

## Final Playtest (In-Class — March 12, 5:00 PM)

There are no slides and no formal presentation.

Your team will:

- Launch your game
- Play it live (2 team members play, 2 explain)
- Other teams will play your game

While playing, your team must be able to explain:

- How a button press becomes an attack that hits an opponent
- How the event system connects GameManager, UI, and fighters without direct references
- What makes your two fighters feel different and why
- How hitstop and knockback create impact feel
- When and why a player cannot act (hitstun, attack commitment, knockback)
- One game rule you changed from the template defaults and why
- One technical decision your team made during development

**This is a live playtest, not a speech.**

---

## Grading Rubric

| Component | Points | Description |
|-----------|--------|-------------|
| **Combat & Fighters** | 30 | Both fighters are functional, feel distinct, and have intentionally designed movesets with meaningful frame data. |
| **Game Systems** | 25 | Match flow, events, and state management work correctly. GameManager, UI, and combat systems are properly wired. |
| **Animation & Feedback** | 20 | Animations match gameplay state. Hits feel impactful through hitstop, knockback, and at least 3 juice elements. |
| **Code Quality & Architecture** | 15 | Code follows template patterns, uses events for communication, configs for tuning, and is well-organized across 4 contributors. |
| **Presentation & Design Doc** | 10 | Team can explain their game while playing it. DESIGN_DOC.md is complete and reflects the actual game. |
| **Total** | **100** | |

---

## Project Setup Instructions

### Tech Lead (One Person)

1. Go to [https://github.com/profangrybeard/BrawlerTemplate](https://github.com/profangrybeard/BrawlerTemplate)
2. Click **Fork** (top right) to create a copy under your GitHub account
3. In your fork, go to **Settings > Collaborators** (left sidebar under "Access")
4. Click **Add people** and invite your 3 teammates by GitHub username

### Everyone Else

1. Accept the collaborator invite (check email or GitHub notifications)
2. Clone **your team's fork** (not the upstream template):
   ```
   git clone https://github.com/YOUR-TECH-LEAD/BrawlerTemplate.git
   ```
3. Open in Unity Hub: **Add > Browse** to the cloned folder
4. **Unity version: 6000.0.63f1** (Unity 6) — make sure this is installed

---

## Project Delivery Instructions

You are responsible for delivering a complete, runnable project.

### Submission Format

Submit a single ZIP file named:

**TeamName_GAME326_ArenaBrawler.zip**

Example: ThunderCats_GAME326_ArenaBrawler.zip

### Required Contents

| Item | Description |
|------|-------------|
| **Windows Build** | A folder containing your built .exe and all required files |
| **Project ZIP** | Your complete Unity project as a separate .zip inside the main zip |
| **Video** | A 1-minute (max) gameplay video showing both fighters and a full match |

### Build Instructions

1. **File > Build Settings**
2. Set Platform to **Windows**
3. Click **Build**
4. Choose a folder named Build inside your project
5. Test the build on a different computer if possible

### Video Requirements

- Maximum 1 minute
- Show both fighters being played
- Show a complete match (rounds, KO, winner)
- No narration required (but allowed)
- Screen recording is fine (OBS, Windows Game Bar, etc.)

### Also Required

- Push all code to your team's fork on GitHub
- Ensure project runs from a fresh clone
- Completed DESIGN_DOC.md in the repo root
- All 4 team members participate in the live playtest

**If it only works on your machine, it is not finished.**

---

## Common Submission Errors (Avoid These)

- Missing ScriptableObject assets (AttackData, MatchConfig, MovementConfig not saved)
- Input Actions asset not included or not assigned
- GameManager not wired (match doesn't flow)
- Fighters that play identically (same attacks, same speed, same feel)
- Attacks with no hitbox visualization or broken collision layers
- Animations that don't match gameplay state
- Only one or two teammates' commits in the repo
- DESIGN_DOC.md not filled out or doesn't match the actual game
- Game rules that changed from template defaults with no documentation

These issues indicate lack of testing or incomplete work.

---

## Resources

- [Guide 01: Getting Started](Assets/_Project/Guides/01_GettingStarted.md) — Fork, clone, project structure
- [Guide 02: Match Flow and Arena](Assets/_Project/Guides/02_MatchFlowAndArena.md) — GameManager, events, arena setup
- [Guide 03: UI System](Assets/_Project/Guides/03_UISystem.md) — Connecting UI to game events
- [Guide 04: Fighter Architecture](Assets/_Project/Guides/04_FighterArchitecture.md) — Extending FighterBase, movement, attacks
- [Guide 05: Combat System](Assets/_Project/Guides/05_CombatSystem.md) — Hitboxes, attack data, knockback
- [Guide 06: Art Reference](Assets/_Project/Guides/06_ArtReference.md) — Prefabs, animators, sprites
- [Masahiro Sakurai on Creating Games: Frame Data](https://www.youtube.com/watch?v=_R0hbe8HZj0) — The creator of Smash Bros explains frame data
- [The Art of Screenshake](https://www.youtube.com/watch?v=AJdEqssNZ-U) — Jan Willem Nijman on impact feel
- [Juice It or Lose It](https://www.youtube.com/watch?v=Fy0aCDmgnxg) — Making games feel good

For Git help, see the GitHub collaboration instructions in the [README](README.md).

---

## Questions?

Ask your instructor or post in the course Discord.
