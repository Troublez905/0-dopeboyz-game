# Unity follow-up work - October 6, 2026

Unity only. The Godot game is outside this work and must remain unchanged.

## Release gates

- Complete manual keyboard/mouse Play Mode testing: movement, camera collision, shoulder aim, spraying, paintballs, interactions, pursuit, recovery, menus and save/continue.
- Listen to the mix on speakers/headphones. Procedural sound is a functional baseline, not finished music production.
- Inspect the opening district's models, collisions and textures at 1280x720 and 1920x1080. Imported meshes and the new companion require visual acceptance, not just compilation.
- Balance the opening route to 10-15 minutes. The remaining seven districts retain the existing campaign logic and are not claimed as polished new levels.
- Expand save coverage for every temporary gadget, delivery, stash and world event; currently the core checkpoint, economy, ranks, XP and claimed walls are saved.
- Verify all eight upgrade descriptions against their effects. Crew/lookout/influence require further balancing and integration.
- Regression-test all shop entry points; the direct-stat branches have been removed and purchases route through CampaignManager.
- Optimize imported geometry, texture memory and draw calls. Add LODs before expanding the district.
- Animate the original Cap King mesh with a compatible rig; the imported October companion is separate and does not magically rig the OBJ hero.

## Playable route and controls

Open Unity MainScene, press Play, choose New game. WASD moves relative to the camera; mouse orbits; right mouse aims; left mouse uses the selected tool; R switches spray/paintball; Space jumps; Ctrl dashes; Shift sprints; E interacts; C requests a contract; Tab opens upgrades; Escape pauses.

Claim walls, finish a street contract, use the compressor for paint, visit HQ to checkpoint and buy an upgrade. The workbench offers a three-wall/90-second tag sprint. Collect a shipment at the cargo crate and reach the lowrider within 90 seconds. New permanent upgrades use skill credits rather than a second cash-based stat system.

## Assets you can help create

### Cap King rig and animation package
Prompt: "Prepare the supplied Cap King character as a Unity humanoid FBX, meter scale, Y-up, consistent bind pose, clean weights, textured materials and separate idle, walk, run, jump, land, spray, recoil and hit clips. Preserve the existing character silhouette and outfit. In-place locomotion, no baked translation, no embedded cameras or lights. Supply rig validation and a preview turntable."

### Spray action animation
Prompt: "Create a looping upper-body aerosol painting animation for the supplied humanoid rig: right hand sprays a wall at chest height, subtle wrist sweep and shoulder motion, stable feet, clean transitions into and out of idle. Include a compatible avatar and explain bone naming; FBX plus preview."

### Canal district props
Prompt: "Create a coordinated stylized urban canal prop kit for a third-person graffiti game: railings, drainage pipes, bollards, crates, paint-refill station and a workbench. Real meter dimensions, ground pivots, simple collision meshes, texture atlases, three LODs, readable silhouettes, no unrelated branding. Deliver FBX and PNG textures."

### Graffiti decals
Prompt: "Create eight original colorful graffiti murals for STREET SHOOTERS, with clean transparent backgrounds, readable paint edges, layered spray detail and no copyrighted logos. Deliver separate 2048x1024 RGBA PNGs with consistent scale and a labeled contact sheet. Do not include mock walls or UI."

### Ambient sound package
Prompt: "Produce seamless stereo canal-city ambience with distant traffic, water, ventilation and occasional metal rattles; no dialogue, sirens dominating the mix or recognizable music. Supply a 60-second 48kHz WAV loop plus isolated one-shots and license documentation."

### Gameplay sound package
Prompt: "Create original dry WAV effects for aerosol start/loop/stop, paintball launch and splat, sneaker steps on asphalt, jump/land, crate open, vending purchase and wall-complete reward. Three variations per transient, clean tails, no clipping, 48kHz, consistent loudness."

### Music stems
Prompt: "Compose an original instrumental street-art game groove at 96 BPM, playful percussion and bass, no samples requiring clearance. Deliver matching 60-second seamless exploration and pursuit loops, individual stems, a short victory sting, and clear usage rights."

## October resource pack

Keep resources-oct source FBX, BLEND, clips, controllers and prefab files intact. The Unity integration uses regenerated CanalOctober prefabs for Bench, YardFence and WallLight because their supplied prefab source GUIDs were missing. InmateThreePlayable uses the supplied Resource03 Locomotion controller. Other supplied models should enter a curated district layout only after scale, material and collision checks; importing everything is not a level-design pass.
