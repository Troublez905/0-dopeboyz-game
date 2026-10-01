# Godot vs Unity For Street Shooters

## Recommendation

Use **Godot** for the next production version of Street Shooters.

Street Shooters is a top-down 2D game built around fast iteration, territory systems, sprite animation, UI overlays, missions, and arcade feel. Godot fits that better than Unity for the current direction.

## Why Godot Fits Better

- Strong 2D workflow with nodes, scenes, TileMaps, lights, cameras, and signals.
- Fast editor startup and quick iteration for small-team prototyping.
- Lightweight project structure that is easier to keep organized.
- Good fit for top-down movement, patrol AI, pickups, upgrades, save files, and menus.
- Easier to ship small desktop builds without a heavy project footprint.

## When Unity Would Be Better

Choose Unity if the project grows into:

- 3D open-world gameplay.
- Complex multiplayer with a dedicated backend.
- Console-focused production.
- Heavy use of Unity Asset Store packages.
- A larger team already committed to Unity workflows.

## Suggested Godot Port Plan

1. Keep the browser beta as the design prototype.
2. Create a Godot 4 project with scenes for Main Menu, District Map, Player, Crew Member, Police, Rival, Tag Wall, Pickup, HUD, and Upgrade Screen.
3. Move the current level data into Godot resources or JSON.
4. Port the movement, tagging, police chase, rival repainting, pickups, upgrades, and save/load systems.
5. Re-slice the current sprite art into Godot SpriteFrames.
6. Add audio buses for music, UI, combat, police, and graffiti effects.
7. Package the first Windows beta build.

## Current Decision

Godot was selected and the native beta now lives in `godot/project.godot`. Run it using `Play Street Shooters.cmd`. The original browser beta remains as a reference; active expansion work is in Godot. See `EXPANSION.md` for implemented systems, map provenance, verification and remaining beta limitations.
