# Guide 02: Wiring GameManager (Match Flow)

This guide walks you through wiring the `GameManager` scaffold so your brawler has working rounds, KOs, respawns, and win conditions. When you're done, the full match loop will work end to end.

**Time:** ~20 minutes

**Prerequisites:** Scene setup complete (Guide 01). DevArena scene with fighters, arena, spawn points, and GameManager all in place.

**File you're editing:** `Assets/_Project/_Shared/Scripts/Core/GameManager.cs`

---

## What You're Building

The match flows through these states:

```
Match Start
    |
    v
+----------+
| Countdown|  "3... 2... 1... GO!"
+----+-----+
     |
     v
+----------+
| Fighting |<----------------+
+----+-----+                 |
     | (KO happens)          |
     v                       |
+----------+                 |
| RoundEnd |  Award point    |
+----+-----+                 |
     |                       |
     v                       |
  Winner? --No-------------->+
     |
    Yes
     |
     v
+----------+
| MatchEnd |  "GAME!"
+----------+
```

The scaffold has 5 TODO steps. Each step is a small code change — mostly uncommenting existing code. Do them in order.

---

## Before You Start

Open `GameManager.cs` and read through it. Notice:

- **`StartMatch()`** already initializes fighters and calls `StartRound()`
- **`StartRound()`** has TODOs for positioning and countdown
- **`OnFighterKO()`** has the KO handler logic but isn't connected yet
- The coroutines and win check are written but commented out in `/* */` blocks

The events system (`GameEvents`) is the communication bus. Components fire events, other components listen. The pattern is always: subscribe with `+=`, unsubscribe with `-=`, fire with `?.Invoke()`.

---

## Step 1: Subscribe to KO Events

**What:** Connect the GameManager to blast zone KO events so it knows when a fighter dies.

**Find this in `Start()` (~line 57):**

```csharp
// TODO STEP 1: Subscribe to GameEvents.OnFighterKO
// GameEvents.OnFighterKO += OnFighterKO;
```

**Change it to:**

```csharp
GameEvents.OnFighterKO += OnFighterKO;
```

**Then find this in `OnDestroy()` (~line 74):**

```csharp
// TODO: Unsubscribe from events
// GameEvents.OnFighterKO -= OnFighterKO;
```

**Change it to:**

```csharp
GameEvents.OnFighterKO -= OnFighterKO;
```

**Why both?** Always unsubscribe in `OnDestroy` to prevent memory leaks and null reference errors. If the GameManager is destroyed but the event still has a reference to it, you get crashes.

**Verify:** Press Play. Knock a fighter into a blast zone. You should see `[GameManager] Player X KO'd via Bottom!` in the Console. Nothing else happens yet — we haven't wired the round end.

---

## Step 2: Position Fighters at Spawn Points

**What:** At the start of each round, teleport fighters to their spawn points and reset their state.

**Find this in `StartRound()` (~line 116):**

```csharp
// TODO STEP 2: Position fighters at spawn points
// for (int i = 0; i < fighters.Length; i++)
// {
//     if (fighters[i] != null && spawnPoints[i] != null)
//     {
//         fighters[i].Respawn(spawnPoints[i].Position);
//     }
// }
```

**Uncomment the loop:**

```csharp
for (int i = 0; i < fighters.Length; i++)
{
    if (fighters[i] != null && spawnPoints[i] != null)
    {
        fighters[i].Respawn(spawnPoints[i].Position);
    }
}
```

**What `Respawn()` does:** Teleports the fighter, zeros out velocity, resets health, and clears any knockback/hitstun state. It also fires `GameEvents.OnFighterRespawn` so UI can react.

**Verify:** Press Play. Both fighters should snap to their spawn point positions when the match starts.

---

## Step 3: Countdown Coroutine

**What:** Add a countdown delay before each round begins. During countdown, the game state is `Countdown` — fighters can't act until it transitions to `Fighting`.

This is a two-part change.

### Part A: Uncomment the coroutine

**Find the commented-out block (~line 130):**

```csharp
/*
 * TODO STEP 3: Implement countdown coroutine
 *
private IEnumerator CountdownCoroutine()
{
    // Fire round start event (for UI)
    GameEvents.OnRoundStart?.Invoke(CurrentRound);

    // Wait for countdown
    yield return new WaitForSeconds(matchConfig.roundStartDelay);

    // Start fighting!
    SetState(GameState.Fighting);
}
*/
```

**Remove the `/*` and `*/` comment markers and the `TODO` label line** so it becomes:

```csharp
private IEnumerator CountdownCoroutine()
{
    // Fire round start event (for UI)
    GameEvents.OnRoundStart?.Invoke(CurrentRound);

    // Wait for countdown
    yield return new WaitForSeconds(matchConfig.roundStartDelay);

    // Start fighting!
    SetState(GameState.Fighting);
}
```

### Part B: Call it from StartRound

**Find this in `StartRound()` (~line 125):**

```csharp
// TODO STEP 3: Start countdown
// SetState(GameState.Countdown);
// StartCoroutine(CountdownCoroutine());
```

**Uncomment:**

```csharp
SetState(GameState.Countdown);
StartCoroutine(CountdownCoroutine());
```

**Verify:** Press Play. There should be a ~3 second pause before fighters can move (controlled by `MatchConfig.roundStartDelay`). Check the Console — you should see the state change logs: `State changed to: Countdown` then `State changed to: Fighting`.

---

## Step 4: End Round Coroutine

**What:** When a KO happens, award the round to the surviving player, fire events for UI, then pause briefly before the next round.

### Part A: Uncomment the coroutine

**Find the commented-out block (~line 164):**

```csharp
/*
 * TODO STEP 4: Implement end round coroutine
 *
private IEnumerator EndRoundCoroutine(int roundWinner)
{
    SetState(GameState.RoundEnd);

    // Award round win
    RoundWins[roundWinner]++;
    GameEvents.OnRoundEnd?.Invoke(roundWinner);
    GameEvents.OnRoundScoreChanged?.Invoke(roundWinner, RoundWins[roundWinner]);

    Log($"Player {roundWinner} wins round {CurrentRound}! Score: P1={RoundWins[0]} P2={RoundWins[1]}");

    // Wait before next round
    yield return new WaitForSeconds(matchConfig.roundEndDelay);

    // TODO STEP 5: Check win condition
    // CheckMatchEnd(roundWinner);
}
*/
```

**Remove the `/*` and `*/` comment markers and the `TODO` label line:**

```csharp
private IEnumerator EndRoundCoroutine(int roundWinner)
{
    SetState(GameState.RoundEnd);

    // Award round win
    RoundWins[roundWinner]++;
    GameEvents.OnRoundEnd?.Invoke(roundWinner);
    GameEvents.OnRoundScoreChanged?.Invoke(roundWinner, RoundWins[roundWinner]);

    Log($"Player {roundWinner} wins round {CurrentRound}! Score: P1={RoundWins[0]} P2={RoundWins[1]}");

    // Wait before next round
    yield return new WaitForSeconds(matchConfig.roundEndDelay);

    // TODO STEP 5: Check win condition
    // CheckMatchEnd(roundWinner);
}
```

### Part B: Call it from OnFighterKO

**Find this in `OnFighterKO()` (~line 160):**

```csharp
// TODO STEP 4: End the round
// StartCoroutine(EndRoundCoroutine(winnerIndex));
```

**Uncomment:**

```csharp
StartCoroutine(EndRoundCoroutine(winnerIndex));
```

**Verify:** Press Play. Knock a fighter into a blast zone. You should see:
- Console log: `Player X wins round 1! Score: P1=0 P2=1`
- A ~2 second pause (controlled by `MatchConfig.roundEndDelay`)
- ...but nothing happens after the pause yet. That's Step 5.

---

## Step 5: Win Condition Check

**What:** After each round, check if someone has won enough rounds. If yes, end the match. If not, start the next round (which repositions fighters and runs the countdown again).

### Part A: Uncomment the method

**Find the commented-out block (~line 186):**

```csharp
/*
 * TODO STEP 5: Implement win condition check
 *
private void CheckMatchEnd(int lastRoundWinner)
{
    // Check if someone has won enough rounds
    if (RoundWins[lastRoundWinner] >= matchConfig.roundsToWin)
    {
        // Match over!
        EndMatch(lastRoundWinner);
    }
    else
    {
        // Next round
        StartRound();
    }
}
*/
```

**Remove the `/*` and `*/` comment markers and the `TODO` label line:**

```csharp
private void CheckMatchEnd(int lastRoundWinner)
{
    if (RoundWins[lastRoundWinner] >= matchConfig.roundsToWin)
    {
        EndMatch(lastRoundWinner);
    }
    else
    {
        StartRound();
    }
}
```

### Part B: Call it from EndRoundCoroutine

**Find this inside `EndRoundCoroutine` (you just uncommented it):**

```csharp
// TODO STEP 5: Check win condition
// CheckMatchEnd(roundWinner);
```

**Uncomment:**

```csharp
CheckMatchEnd(roundWinner);
```

**Verify:** Press Play. Play through a full match:
1. KO the opponent — round ends, score updates
2. Fighters respawn at spawn points, countdown starts
3. Fight again — KO again
4. After enough round wins (default: 2), you should see `[GameManager] GAME! Player X wins the match!`

---

## Final State

After all 5 steps, your `GameManager.cs` should have:

- **`Start()`**: `GameEvents.OnFighterKO += OnFighterKO;` (uncommented)
- **`OnDestroy()`**: `GameEvents.OnFighterKO -= OnFighterKO;` (uncommented)
- **`StartRound()`**: Fighter positioning loop and countdown call (uncommented)
- **`CountdownCoroutine()`**: Fully uncommented method
- **`OnFighterKO()`**: `StartCoroutine(EndRoundCoroutine(winnerIndex));` (uncommented)
- **`EndRoundCoroutine()`**: Fully uncommented method with `CheckMatchEnd` call
- **`CheckMatchEnd()`**: Fully uncommented method

No TODO comments should remain.

---

## Full Match Test

Run through this checklist:

- [ ] **Fighters spawn** at their assigned positions on match start
- [ ] **Countdown** delays before fighting begins (~3 seconds)
- [ ] **Movement and attacks** work during Fighting state
- [ ] **KO triggers round end** when a fighter hits a blast zone
- [ ] **Round wins** are tracked correctly (check Console logs)
- [ ] **Fighters respawn** at spawn points for the next round
- [ ] **Match ends** when a player wins enough rounds (default: best of 3)
- [ ] **Console shows** `GAME! Player X wins the match!` at match end

### Common Issues

| Problem | Fix |
|---------|-----|
| Fighters don't move at all | Make sure GameManager has both fighters in the **Fighters** array (P1 at index 0, P2 at index 1) and **MatchConfig** is assigned. `StartMatch()` aborts before initializing fighters if MatchConfig is missing. |
| KO doesn't trigger round end | Step 1 not done — event subscription is still commented out. |
| Fighters don't respawn between rounds | Step 2 not done — positioning loop is still commented out. |
| No countdown, fighting starts instantly | Step 3 not done — countdown coroutine is still commented out. |
| Round ends but nothing happens after | Step 5 not done — win condition check is still commented out. |
| `NullReferenceException` on `matchConfig` | MatchConfig not assigned in the GameManager Inspector. Drag in `DefaultMatchConfig`. |
| `NullReferenceException` on spawn points | SpawnPoints array not assigned or wrong size. Set to 2 and drag in both SpawnPoint objects. |

---

## Key Events Reference

These are the events the match system fires. When you build UI (Guide 03), you'll subscribe to these.

| Event | Fires When | Data |
|-------|-----------|------|
| `OnFighterKO` | Fighter hits blast zone | `FighterKOEventArgs` (PlayerIndex, ZoneType) |
| `OnRoundStart` | Countdown begins | `int` round number |
| `OnRoundEnd` | Round awarded | `int` winner index |
| `OnRoundScoreChanged` | Score updates | `int` player index, `int` new win count |
| `OnGameStateChanged` | State transitions | `GameState` new state |
| `OnMatchEnd` | Match over | `int` winner index |
| `OnFighterDamaged` | Fighter takes a hit | `FighterDamageEventArgs` (PlayerIndex, Damage, NewHealth, etc.) |
| `OnFighterRespawn` | Fighter respawns | `FighterRespawnEventArgs` (PlayerIndex, SpawnPosition) |

---

## MatchConfig Reference

Tweak these values in your `DefaultMatchConfig` asset to change match rules:

| Property | Default | What It Does |
|----------|---------|-------------|
| `roundsToWin` | 2 | Rounds needed to win (2 = best of 3) |
| `roundStartDelay` | 3.0 | Countdown duration in seconds |
| `roundEndDelay` | 2.0 | Pause after round ends |
| `matchTimeLimit` | 0 | Time limit per round (0 = no limit) |
| `respawnInvincibilityDuration` | 2.0 | Invincibility after respawn |
| `enableSuddenDeath` | false | Sudden death if time runs out |

---

## Arena Systems (Reference)

### BlastZone

Trigger collider that KOs any fighter that enters it. Four around the arena edges define the boundaries. Each has a `ZoneType` (Top, Bottom, Left, Right) so the game knows the KO direction.

### SpawnPoint

Marks where a fighter spawns. Each has a `playerIndex` (0 or 1). The `Position` property returns the spawn location used by `Respawn()`.

### Platform (Scaffold)

`Platform` uses `PlatformEffector2D` for one-way collision — fighters jump up through it but stand on top. The scaffold has a TODO for drop-through: when a fighter presses down, use `Physics2D.IgnoreCollision()` to temporarily disable collision, wait briefly, then re-enable.

---

**Next:** [Guide 03: UI System](03_UISystem.md) — wire health bars, round display, and match announcements to these events
