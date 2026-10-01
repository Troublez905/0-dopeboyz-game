# Street Shooters

**Godot beta 0.2 | Single player | Windows | Keyboard and mouse**

Claim the wall. Keep the block. Build your crew.

Street Shooters is a top-down graffiti territory game with eight district missions across two illustrated city maps. Paint walls, protect tags, collect supplies, recruit backup and choose where to spend upgrade credits. Police hunt you when heat rises; rivals repaint territory you leave exposed.

## Start Playing

### After Cloning From GitHub

The GitHub repository includes game source, artwork, documentation, and both Godot and Unity project files. Generated editor caches and the local Godot executable are excluded.

Install Godot 4.6.2, import `godot/project.godot` in the editor, allow asset import to finish, then press F5. The Windows launchers expect the executable at `tools/godot/Godot_v4.6.2-stable_win64.exe`; place a matching portable Godot download there to use those launchers. The Unity project is in `unity/`; open it through Unity Hub with the version recorded in `unity/ProjectSettings/ProjectVersion.txt`.

### On This Local Checkout

Double-click **`Play Street Shooters.cmd`** in this folder. Choose **New Run** or **Continue**. No server or account is needed. Keep the `godot` and `tools` folders beside the launcher.

To edit, open **`Open Godot Editor.cmd`**, then press **F5** to run the project. The project is `godot/project.godot`, built with the included Godot **4.6.2** runtime.

The older `index.html` and `urban_vandal_turf_wars_prototype.html` remain the browser prototype. They do **not** contain this expansion, and their browser saves do not migrate into Godot.

## Your First Run

1. Start at the mint HQ circle with paint, a weapon, ammunition and $60.
2. Walk east to the first gold wall. Hold **Space** nearby until its progress bar fills. You cannot move or fire while spraying.
3. Claim three walls. Owned walls generate reputation and cash. Reach 200 rep to clear the district.
4. Collect street supplies by walking over them. Supplies return after a cooldown.
5. Spend level credits on chosen upgrades or bank them for later. Enter the next district when ready.

Press **H** for controls. **P** pauses the simulation. Jail and hospital keep the city moving during your absence.

## Controls

| Action | Input |
| --- | --- |
| Move | WASD or arrows |
| Sprint | Hold Shift; consumes stamina |
| Tag nearest wall | Hold Space; consumes paint |
| Aim and fire | Mouse + left click |
| Fire at nearest visible enemy | F; uses facing if no target is visible |
| Open HQ or supply shop nearby | E |
| Deliver shipment at HQ | E |
| Cycle unlocked crew orders | Q |
| Accept contract / find next lead | C |
| Expand minimap | M |
| Pause / resume | P or Escape |
| Help | H |
| Save now | F5 |

Desktop only in this beta: touch controls, controller support and rebinding are not implemented.

## Eight-District Campaign

The night and daytime chapters use different supplied map images, street graphs, collisions, tag locations and supply routes. Each chapter has twelve tag spots. Districts in the same chapter share its map.

| Level | District | Completion requirements | Credits |
| --- | --- | --- | --- |
| 1 | Canal Ink Yard | Own 3 walls; 200 rep | 3 |
| 2 | Neon Market Strip | Own 4 walls; 450 rep; 1 contract | 3 |
| 3 | Railcut Backlots | Own 5 walls; 750 rep; 2 crew | 4 |
| 4 | Downtown Crown | Own 7 walls at 1100 rep for 30 continuous seconds | 5 |
| 5 | Cap Alley Arrival | Daytime city; own 4 walls; 450 rep; 1 delivery | 3 |
| 6 | Bass Block Takeover | Own 6 walls; 750 rep; 2 contracts | 4 |
| 7 | Sticker Tunnel | Own 7 walls; 1000 rep; defeat rival captain | 4 |
| 8 | Roller Heights Finale | Own 9 walls at 1400 rep with 3 crew for 45 continuous seconds | 5 |

Breaking a required hold resets its timer. Clearing all eight levels unlocks daytime free roam and preserves your completed campaign.

Each district starts a fresh territory contest with a restocked loadout and new patrols. Cash, credits, upgrades, outfits and HQ paint carry forward. Recruit a new field crew for each district; crew training stays purchased. Saving within a district preserves its current crew and world state.

## Choose Your Upgrades

One level credit purchases one rank, up to five ranks per upgrade. Credits are separate from shop cash. Spend them between districts or at HQ.

| Upgrade | Benefit per rank | Unlock |
| --- | --- | --- |
| Runner | +18 movement speed | Start |
| Paint rig | +30 paint capacity; +12% tagging speed | Start |
| Resilience | +25 maximum health | Start |
| Crew training | +1 recruit slot; +8 crew damage | Clear district 1 |
| Headquarters | +50 stash capacity; +20 starting paint up to capacity; slower rival repainting | Clear district 1 |
| Armor & gear | +6 weapon damage; 8% less incoming damage | Clear district 2 |
| Lookout network | Police need 4 more heat before pursuit | Clear district 4 |
| Street influence | +15% passive rep | Clear district 4 |

Starting crew capacity is two. Clearing district 6 adds a third base slot so the finale remains possible regardless of upgrade choices.

Purple Hoodie is available at the start. Runner, Heavy Hoodie and Ghost unlock after districts 1, 2 and 3. Change looks at HQ or between districts. Outfits are cosmetic; purchased stats apply to every look.

## Street Systems

**Crew orders.** Recruits follow and fight nearby rivals. After district 1, Q cycles Follow, Guard and Regroup. Guard anchors the crew to your current location. Regroup prioritizes returning to you. Recruits have health and can be lost.

**HQ stash.** Deposit or withdraw up to 40 paint at a time. Base capacity is 50. Stored paint survives arrest and carries into later districts.

**Supply shop.** Buy 60 paint ($20), 24 rounds ($25), a medical pack ($30), a replacement weapon ($45), or a recruit ($70). Full supplies and unaffordable purchases do not charge you.

**Street contracts.** Press C to accept the highlighted wall. Finish its tag within 150 seconds for $90 and 60 rep. Press C after completion or expiry for another lead. The minimap ring marks your target. If all walls are owned, contracts wait until a rival reclaims a target.

**Tag chains.** Complete another tag within 40 seconds to grow a chain up to x5. Chains multiply immediate cash and rep rewards, not passive income. Arrest, hospital and timeouts end chains.

**Painted pieces.** Spraying reveals a crowned turquoise 404 graffiti piece directly on the wall. Larger walls display larger artwork. Completed pieces stay visible, including after loading a saved run. Rivals cover your artwork with their own red RZN tag; reclaiming the wall paints your crew's design back over it. Empty walls have subtle corner guides rather than completed-tag labels.

**Supply deliveries.** After district 4, collect a free shipment at the shop. Bring it to HQ and press E for $100 and 75 rep. Carrying it slows movement by 12%. Arrest or hospital loses it; another shipment remains available.

**Rival captain.** Later daytime levels include a marked rival with 320 health. Defeat the captain for bonus cash and the Sticker Tunnel objective. Crew backup and gear upgrades help.

## Health, Heat And Consequences

- Painting and shooting raise heat. Police pursue when they can see you and your heat exceeds their threshold. Buildings block sight and bullets.
- Contact with a pursuing officer sends you to jail for 11 seconds and confiscates paint, ammo and weapon. Cash, upgrades and HQ paint remain.
- Zero health sends you to hospital for 8 seconds and costs up to $35. Return to HQ at full health.
- Rivals keep repainting and contract timers continue during both penalties. A short protection period follows release.
- Free paint and weapon pickups let you recover even without shop cash.

## Saves And Options

Autosaves run every 20 seconds of play, at district completion, after purchases or stash transfers, and on normal window close. Use F5 or Pause > Save Progress for a manual save. Save failures are reported.

Saves preserve progression, inventory, positions, crew, enemies, wall ownership, partial tags, pickup cooldowns, contracts, stash and remaining penalty time. Reopening cannot skip jail. New Run asks before replacing a save.

Windows save location:

```text
%APPDATA%\Godot\app_userdata\Street Shooters\street_shooters_v1.json
```

Options are separate in `options.cfg` in that directory. Music and effects have independent volume sliders. Rain, shake and fullscreen can be toggled. Audio is synthesized locally.

## Project Files

```text
Play Street Shooters.cmd       Native launcher
Open Godot Editor.cmd          Editor launcher
Verify Godot.ps1               Repeatable checks
godot/project.godot            Godot project
godot/scenes/main.tscn         Entry scene
godot/scripts/main.gd          Simulation and gameplay
godot/scripts/campaign.gd      Missions, upgrades and saves
godot/scripts/city.gd          Map geometry, navigation and territory
godot/scripts/actor.gd         Characters and animation
godot/scripts/interface.gd     Menus, HUD and minimap
godot/scripts/audio.gd         Music and effects
godot/shaders/                 Runtime sprite mask
godot/assets/                  Copies of supplied artwork
godot/tests/beta_checks.gd     Functional and screenshot checks
qa/                           Results and captured views
tools/godot/                  Included engine
```

## Verification

Run from this folder in PowerShell:

```powershell
.\'Verify Godot.ps1'
.\'Verify Godot.ps1' -Render
```

The rendered run opens a temporary game window, exercises campaign systems and captures actual game views. Results are in `qa/godot-results.json` and `qa/verification.log`. Tests use a separate save file and do not change your campaign.

These checks do not replace an extended human playthrough. Difficulty and economy pacing still need player feedback.

## Beta Limits

- Two maps, not eight unique maps. Collision follows main streets; every painted alley and doorway is not explorable.
- The supplied sheets mix poses and have painted backgrounds. Selected frames and a runtime mask are used; some sprite edges and direction changes remain approximate.
- Police reuse the 404 sprite with blue identification. The captain uses rival art with a label and more health.
- The launcher runs a native Godot project using the included engine. A signed installer or standalone store export has not been produced.
- Single-player local saves only. No interiors, vehicles, multiplayer or cloud saves.
- Crew sprites can overlap.

Source art and the original browser prototype are preserved. See `docs/EXPANSION.md` for the implementation checklist and artwork provenance.
