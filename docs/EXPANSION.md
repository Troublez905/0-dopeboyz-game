# Street Shooters Godot Expansion

## Delivered Scope

The five previously requested improvements are implemented in the Godot project:

1. Local synth sound effects and a musical pulse, with separate volume controls.
2. Save/continue for progression, upgrades, inventory, territory and active world state.
3. Four selectable character looks with progression unlocks.
4. Named district missions with different completion requirements.
5. A native project split into gameplay, campaign, city, actor, interface, audio, shader and test files.

Five additional systems are implemented: crew orders, protected HQ paint storage, a cash supply shop, timed wall contracts, and tag-chain rewards.

The subsequent expansion adds a second city chapter, eight total levels, supply deliveries, rival captains, lookout and influence upgrades, and a sustained-defense finale.

## Artwork Used

Original files in the parent directory are preserved. Native project copies:

| Project asset | Supplied source |
| --- | --- |
| `assets/title.png` | `ChatGPT Image Aug 13, 2026, 01_27_42 AM (2).png` |
| `assets/city.png` | `Codex Image Sep 2, 2026, 02_41_24 AM.png` |
| `assets/city_day.png` | `Codex Image Sep 2, 2026, 02_57_20 AM.png` |
| `assets/hoodie1.png` | `hoodie1.png` |
| `assets/hoodie2.png` | `hoodie2.png` |
| `assets/hoodie3.png` | `hoodie3.png` |
| `assets/ghost1.png` | `ghost1.png` |
| `assets/rival.png` | `404kid1.png` |

The daytime map has slanted north-south streets. Its graph uses separate intersection coordinates for each row and polygon building collisions. Each chapter contains twelve claimable walls.

## Engine And Distribution

Runtime: Godot 4.6.2, GDScript, Compatibility renderer. Physics uses CharacterBody2D; routes use AStar2D; bullet checks use Geometry2D.

The portable Godot executable was extracted from the user's existing `Godot_v4.6.2-stable_win64.exe.zip`. Launch scripts resolve their own folder so spaces in paths work.

The launcher runs the native project through the included engine. A signed standalone installer has not been produced. For later export, install matching export templates and configure a Windows Desktop export. [Official export instructions](https://docs.godotengine.org/en/4.6/tutorials/export/exporting_projects.html).

## Quality Evidence

Run `Verify Godot.ps1 -Render`. Tests exercise both map configurations, all eight progression gates, combat, inventory costs, passive income, repainting during jail, hospital, crew orders, recruitment limits, contracts, deliveries, captain defeat, upgrade locks, saves, invalid world-data rejection and real movement input.

Rendered checks capture the actual Godot viewport and verify nonblank images. Exact results are written to `qa/godot-results.json`. They do not replace extended manual balancing or broad hardware testing.

## Next Playtest Focus

Play the first two districts without developer assistance. Check the cash economy after repeated arrests, daytime street edges, and crew crowding. Use those observations to tune pacing and animation before adding multiplayer or further maps.
