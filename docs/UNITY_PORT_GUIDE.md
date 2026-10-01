# 404 DOPEBOYZ - Unity Version Architecture & Setup Guide

**Unity Editor Version**: `Unity 6 (6000.4.0f1)`  
**Project Path**: `unity/`  
**Quick Launcher**: `Open Unity Editor.cmd` in project root  

---

## 🏗️ Architecture Overview

The Unity version translates the complete gameplay, combat, territory, and aesthetic systems from Godot into modular, high-performance C# scripts utilizing Unity's 2D engine and Universal Render Pipeline conventions.

### 📜 Core C# Script Library (`unity/Assets/Scripts/`)

1. **[`GameManager.cs`](file:///c:/Users/ghost/Desktop/Ideas-Brainstorms/0-dopeboyz%20game/unity/Assets/Scripts/GameManager.cs)**
   - Global game state machine (`MainMenu`, `Playing`, `Paused`, `Shop`, `HQ`, `BlackMarket`, `GameOver`).
   - Economy tracking: Cash balance, player street cred score, combo multipliers.
   - Law enforcement heat rating (0.0 to 100.0) driving 5 Wanted Stars.
   - District territory control tally (Crew vs Rival vs Unclaimed walls).
   - 100% District Dominance detection for the **24K Golden Mastery Can Award**.

2. **[`CameraController.cs`](file:///c:/Users/ghost/Desktop/Ideas-Brainstorms/0-dopeboyz%20game/unity/Assets/Scripts/CameraController.cs)**
   - Smooth 2D target follow with deadzone and velocity damping.
   - **3-Tier Camera Zoom System**:
     - **Close / Action View (Tier 0)**: Orthographic size `4.5f` (~1.85x zoom - default).
     - **Street Medium View (Tier 1)**: Orthographic size `6.5f` (~1.35x zoom).
     - **Tactical Overview (Tier 2)**: Orthographic size `9.0f` (~0.95x zoom).
     - Toggled on-the-fly via `F1` or Gamepad Right Stick Click.
   - Procedural screen shake on explosions, soundquake bass drops, and hits.

3. **[`PlayerController.cs`](file:///c:/Users/ghost/Desktop/Ideas-Brainstorms/0-dopeboyz%20game/unity/Assets/Scripts/PlayerController.cs)**
   - 8-way skating movement with momentum, acceleration, and friction.
   - Evasive Dash (`Space` / Gamepad A / Left Trigger).
   - Omnidirectional aiming (Mouse cursor / Gamepad Right Stick).
   - Progressive Wall Spraying (`Left Click` / Gamepad Right Trigger) with particle emitters.
   - Special Abilities:
     - **Pressurized Bubble Shield** (`U` / Gamepad B): 4.5s bullet-deflecting barrier.
     - **Adrenaline Surge** (`L` / Gamepad LB): 6s slow-motion matrix focus (`Time.timeScale = 0.5f`).
     - **Neon Ground Underglow** (`O`): Cyan/magenta ground lighting aura beneath skateboard.
     - **Stencil Stamp Placement** (`1`-`6` / Gamepad X).

4. **[`WallTag.cs`](file:///c:/Users/ghost/Desktop/Ideas-Brainstorms/0-dopeboyz%20game/unity/Assets/Scripts/WallTag.cs)**
   - Spot properties (Name, spot size 1-3, owner).
   - Multi-layer spray progress (0.0f to 1.0f).
   - Paint reveal shader/coloring with crew and rival layers.
   - Dripping paint physics calculation.

5. **[`CityController.cs`](file:///c:/Users/ghost/Desktop/Ideas-Brainstorms/0-dopeboyz%20game/unity/Assets/Scripts/CityController.cs)**
   - Road network coordinates and district boundaries.
   - Interactive landmarks:
     - **Lowrider Hydraulic Bounce Car** (`H` key near car triggers shockwave pushback).
     - **Vending Machine Kiosk** (`E` key dispenses Neon Soda sprint boost).
     - **Subway Grate Updraft Vent** (Propels player into high air).
     - **Chill Lounge Neon Pool Table** (Sanctuary zone).
     - **CCTV Security Cameras** (Sweeping detection cones).
     - **Sky Surveillance Blimp** (Patrols across city sky).

6. **[`ActorController.cs`](file:///c:/Users/ghost/Desktop/Ideas-Brainstorms/0-dopeboyz%20game/unity/Assets/Scripts/ActorController.cs)**
   - Base AI for Pedestrians, Rivals, Police Squads, and Crew Members.
   - **SWAT Riot Shield**: Deflects paint shots from the front unless flanked.
   - **Crew Squad Orders**: `FOLLOW`, `GUARD`, `REGROUP`.

7. **[`UIController.cs`](file:///c:/Users/ghost/Desktop/Ideas-Brainstorms/0-dopeboyz%20game/unity/Assets/Scripts/UIController.cs)**
   - Cyber-graffiti styled HUD: Paint spray can meter, stamina bar, cash readout, wanted stars, combo counter.
   - Floating notification toast system for contracts and achievements.

---

## 🎨 Asset Library (`unity/Assets/Sprites/`)

All **85+ generated PNG pixel-art textures and sprites** (Batches 1 through 6) have been copied and linked directly into `unity/Assets/Sprites/`, ready for 2D SpriteRenderer assignment.

---

## 🚀 How to Launch Unity

Double-click:
`Open Unity Editor.cmd` in the root folder, or launch Unity Hub and open the `unity/` folder.
