# Unity implementation record - October 6, 2026

## Scope

Unity code only; no Godot edits in this implementation. Original model and animation sources are preserved. Existing MainScene is not regenerated.

## Implemented in source

- Fixed missing collection imports, missing pickup sound enum members, outdated heat references, wall ownership string/enum comparisons, outdated actor enum names and a missing damage-direction argument.
- Added a persistent GameSession with main menu, pause, settings, new-game confirmation, asynchronous scene loading, checkpoint restart and save/continue.
- Kept gameplay managers scene-owned to prevent their retaining destroyed scene references across loads.
- Added camera-relative movement, orbit camera with collision avoidance, shoulder aim and spray/paintball selection. Removed a duplicate Tab handler.
- Added level 1-10 XP progression with one credit per level and first-time reward identifiers; routed permanent shop upgrades through CampaignManager.
- Added core versioned saves with atomic replacement, previous-save backups and explicit read/write errors. QA uses a separate save filename.
- Added contract completion rewards, HQ checkpoints, workbench upgrades, a timed tagging challenge and cargo delivery to the lowrider.
- Added rival firing telegraphs and line-of-sight checks, checkpoint recovery and adjustable master/music/effects levels.
- Added runtime October resource integration for three environmental prefabs and an animated companion when its avatar/controller validate. Added meter-height normalization and material adaptation without editing supplied assets.
- Added a Windows build command, resource validation and a standalone smoke test with screenshots and startup/save/reload assertions.
- Added runtime scale and collider repair for the existing hero, attachments, buildings, walls and landmarks. The saved scene and source meshes are not overwritten by that repair.
- Moved the beta start and core services to the open central plaza for a clear third-person camera and an accessible opening route.

## Verification status

Unity compilation and Windows builds succeeded. The visible standalone smoke test passed 19 startup, save, upgrade, reload and October-resource checks. Screenshot inspection then exposed oversized legacy character attachments and scene colliders, plus an inactive legacy HUD coroutine error. These were corrected and the final build was retested. No manual Editor Play Mode, full-route or listening pass is claimed here.

The final visible player run on October 7 passed all 19 checks with zero runtime error/exception assertions and exit code 0. Rendered menu and gameplay were inspected at 1280x720; loading UI was inspected at 1920x1080. The final plaza-spawn build is covered by unity/Logs/canal-beta-release.log and unity/Logs/canal-release-verification.log. This is a playable development beta, not a completed art-polish or campaign-balancing pass. Existing imported buildings still have visible mesh defects that need asset cleanup.

## Verification commands

- Unity menu: Dopeboyz > Validate October Resources.
- Unity menu: Dopeboyz > Build Canal Windows Player.
- Run the resulting Windows player with --canal-smoke to exercise the isolated automated test. Output is under Application.persistentDataPath/CanalQA; logs contain CANAL TEST PASS/FAIL.
- Compile logs: unity/Logs/canal-compile.log and unity/Logs/canal-compile-2.log.

## Known limits

The original hero OBJ is not skeletally animated. The October companion uses a supplied rig and passes runtime instantiation/controller checks; full animation visual acceptance is still required. The remaining districts, comprehensive save coverage, controller UX, advanced progression effects, final audio production and optimization remain follow-up work. See oct-6-2026-addto game.md for acceptance gates and production prompts.

## Beta launch

For a fresh GitHub checkout, install Git LFS and run `git lfs pull`, open the unity folder with Unity 6000.4.0f1, and use Dopeboyz > Build Canal Windows Player. Local builds and duplicate source ZIP/USDZ exports are not committed. Blender 5.0 is installed on the development machine and may be required to import the supplied .blend-based character prefab dependencies.

Double-click PLAY-UNITY-BETA.cmd in the project root, or unity/Builds/Canal/StreetShooters.exe. Keep the executable beside its Data folder and UnityPlayer.dll. In the Unity Editor, open Assets/Scenes/MainScene.unity and press Play; the session menu is created automatically.

The October environment prefabs originally contained missing source GUIDs. Fresh prefabs are generated under unity/Assets/Resources/CanalOctober from the supplied OBJ meshes; original resource files remain unchanged.
