# Guide 03: UI System

This guide explains how the UI scaffold scripts connect to game events. Three UI scripts have TODOs for your team to complete.

---

## The Pattern

All UI in this template follows the same pattern:

```
Game System ──fires event──► UI Script ──updates──► Visual Elements
```

1. A game system (FighterHealth, GameManager) fires an event when something changes
2. A UI script subscribes to that event
3. When the event fires, the UI script updates its visuals (text, images, panels)

This means UI never polls or checks state every frame — it reacts to events. This is cleaner and more performant.

---

## HealthBarUI (Scaffold)

**What it does:** Displays one fighter's health as a filled bar.

**How it works:**
- Each HealthBarUI needs a reference to a specific fighter's `FighterHealth` component
- Subscribe to `FighterHealth.OnHealthChanged` to get notified when health changes
- When health changes, update the fill image's `fillAmount` based on the health percentage
- The scaffold includes smooth lerping in `Update()` so the bar animates instead of snapping
- Color thresholds change the bar color as health drops (green → yellow → red)

**The scaffold TODOs:**
- Subscribe to the health changed event in `Start()`
- Unsubscribe in `OnDestroy()`
- Optionally add damage feedback (flash, shake) in `OnHealthChanged()`

**Scene setup:**
- You need one HealthBarUI per fighter (2 total)
- Each needs a reference to its fighter — assign in the Inspector
- Typical hierarchy: Canvas > Panel > Background Image + Fill Image (set to Filled, Horizontal)

---

## RoundDisplayUI (Scaffold)

**What it does:** Shows how many rounds each player has won (dots, icons, or text).

**How it works:**
- Subscribes to `GameEvents.OnRoundScoreChanged`
- When a round is won, updates the display for that player
- The scaffold has helper methods for updating text and icon visuals

**The scaffold TODO:**
- Subscribe to the round score changed event in `Start()`

This is the simplest scaffold — one event subscription is all it needs.

---

## MatchUI (Scaffold)

**What it does:** Handles countdown display, round announcements, and the winner screen.

**How it works:**
- Subscribes to three events: `OnRoundStart`, `OnGameStateChanged`, `OnMatchEnd`
- `OnRoundStart`: kicks off a countdown coroutine ("3... 2... 1... GO!")
- `OnGameStateChanged`: shows contextual text based on state (e.g., "KO!" on round end)
- `OnMatchEnd`: displays the winner panel

**The scaffold TODOs:**
- Subscribe to all three events in `Start()`
- Implement the countdown coroutine (timing with `WaitForSeconds`)
- Handle state changes in a switch statement
- Show the winner panel with the winning player's name

---

## UI Hierarchy Suggestion

Here's one way to organize your UI. This is a starting point, not a requirement — customize it for your game.

```
Canvas (Screen Space - Overlay)
│
├── Player1Panel
│   ├── HealthBarBG (Image)
│   │   └── HealthBarFill (Image, Filled)
│   └── RoundIcons
│       ├── Round1Icon
│       └── Round2Icon
│
├── Player2Panel
│   └── (same structure as Player1Panel)
│
├── CenterPanel
│   ├── CountdownText (TextMeshPro)
│   ├── AnnouncementText (TextMeshPro)
│   └── TimerText (TextMeshPro, if using time limit)
│
└── WinnerPanel (starts hidden)
    ├── WinnerText (TextMeshPro)
    └── RematchButton (optional)
```

---

## Tips

- **TextMeshPro:** If Unity asks to import TMP Essentials, do it. The UI scripts use TextMeshPro for text rendering.
- **Event timing:** UI events fire after game logic completes, so the data is always up to date when your UI reads it.
- **Inspector references:** Most UI scripts need references dragged in via the Inspector (fill images, text components, panels). Check for null references if things aren't showing up.
- **Canvas Scaler:** Set your Canvas Scaler to "Scale With Screen Size" with a reference resolution (e.g., 1920x1080) for consistent UI across screen sizes.

---

**Next:** [Guide 04: Fighter Architecture](04_FighterArchitecture.md) — how to build your fighters
