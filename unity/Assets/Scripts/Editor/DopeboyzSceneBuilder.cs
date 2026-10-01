#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Dopeboyz.Editor
{
    public static class DopeboyzSceneBuilder
    {
        [MenuItem("Dopeboyz/1. Configure All Sprites as 2D Sprites")]
        public static void ConfigureAllSprites()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Sprites" });
            int count = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null)
                {
                    bool changed = false;
                    if (importer.textureType != TextureImporterType.Sprite)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        changed = true;
                    }
                    if (importer.spritePixelsPerUnit != 100f)
                    {
                        importer.spritePixelsPerUnit = 100f;
                        changed = true;
                    }
                    if (!importer.alphaIsTransparency)
                    {
                        importer.alphaIsTransparency = true;
                        changed = true;
                    }
                    if (changed)
                    {
                        importer.SaveAndReimport();
                        count++;
                    }
                }
            }
            AssetDatabase.Refresh();
            Debug.Log($"[Dopeboyz] Successfully configured and re-imported {count} sprites!");
        }

        [MenuItem("Dopeboyz/2. Build Main Game Scene (Complete 404 DOPEBOYZ)")]
        public static void BuildMainGameScene()
        {
            ConfigureAllSprites();

            // Create new 2D scene
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Root Managers
            GameObject managersObj = new GameObject("=== MANAGERS ===");
            var gameManager = managersObj.AddComponent<GameManager>();
            var campaignManager = managersObj.AddComponent<CampaignManager>();
            var cityController = managersObj.AddComponent<CityController>();
            var soundManager = managersObj.AddComponent<SoundManager>();

            // 2. City Map Background
            GameObject mapObj = new GameObject("City_Map_Background");
            var mapRenderer = mapObj.AddComponent<SpriteRenderer>();
            Sprite citySprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/city.png");
            if (citySprite != null)
            {
                mapRenderer.sprite = citySprite;
                mapRenderer.sortingOrder = -10;
                // Center map at (12.8, 12.8) so it spans (0,0) to (25.6, 25.6)
                mapObj.transform.position = new Vector3(12.8f, 12.8f, 0f);
            }

            // 3. Building Obstacle Colliders
            GameObject obstaclesObj = new GameObject("=== BUILDING OBSTACLES ===");
            float[] roadX = new float[] { 3.10f, 6.15f, 8.55f, 12.45f, 15.70f, 18.90f, 23.10f };
            float[] roadY = new float[] { 2.50f, 5.20f, 8.20f, 11.30f, 14.60f, 18.00f, 21.90f };

            for (int y = 0; y < roadY.Length - 1; y++)
            {
                for (int x = 0; x < roadX.Length - 1; x++)
                {
                    GameObject block = new GameObject($"Block_{x}_{y}");
                    block.transform.SetParent(obstaclesObj.transform);
                    block.tag = "Obstacle";

                    float minX = roadX[x] + 0.4f;
                    float maxX = roadX[x + 1] - 0.4f;
                    float minY = roadY[y] + 0.4f;
                    float maxY = roadY[y + 1] - 0.4f;

                    float centerX = (minX + maxX) * 0.5f;
                    float centerY = (minY + maxY) * 0.5f;
                    float width = maxX - minX;
                    float height = maxY - minY;

                    block.transform.position = new Vector3(centerX, centerY, 0f);
                    var col = block.AddComponent<BoxCollider2D>();
                    col.size = new Vector2(width, height);
                }
            }

            // 4. Wall Tag Spots (12 District Walls)
            GameObject wallsParent = new GameObject("=== DISTRICT WALLS ===");
            Vector2[] wallSpots = new Vector2[]
            {
                new Vector2(4.50f, 4.92f), new Vector2(7.45f, 4.92f), new Vector2(11.00f, 7.92f), new Vector2(14.00f, 4.92f),
                new Vector2(17.30f, 11.02f), new Vector2(20.70f, 7.92f), new Vector2(4.50f, 14.32f), new Vector2(10.80f, 17.72f),
                new Vector2(17.30f, 17.72f), new Vector2(20.70f, 21.62f), new Vector2(11.00f, 21.62f), new Vector2(20.70f, 14.32f)
            };
            string[] wallNames = new string[]
            {
                "CANAL CORNER", "INK ALLEY", "MARKET SHUTTERS", "404 CROSSING", "COURT WALL", "STATIC YARD",
                "ROOTZ MURAL", "RAILCUT", "BASS BLOCK", "ROLLER HEIGHTS", "CROWN WALL", "NIGHT MARKET"
            };

            Sprite tagSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/wildstyle.png");
            if (tagSprite == null) tagSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/bubble.png");

            for (int i = 0; i < wallSpots.Length; i++)
            {
                GameObject wallObj = new GameObject($"Wall_{i}_{wallNames[i]}");
                wallObj.transform.SetParent(wallsParent.transform);
                wallObj.transform.position = new Vector3(wallSpots[i].x, wallSpots[i].y, 0f);
                wallObj.tag = "Wall";

                var col = wallObj.AddComponent<BoxCollider2D>();
                col.size = new Vector2(1.8f, 0.8f);

                var tagComp = wallObj.AddComponent<WallTag>();
                tagComp.spotName = wallNames[i];
                tagComp.spotIndex = i;
                tagComp.spotSize = 1 + (i % 3);

                // Child graffiti renderer
                GameObject grafObj = new GameObject("GraffitiArt");
                grafObj.transform.SetParent(wallObj.transform);
                grafObj.transform.localPosition = Vector3.zero;
                var gRen = grafObj.AddComponent<SpriteRenderer>();
                gRen.sortingOrder = 2;

                string[] tagStyles = new string[] {
                    "Assets/Sprites/wildstyle.png",
                    "Assets/Sprites/throwup.png",
                    "Assets/Sprites/bubble.png",
                    "Assets/Sprites/blockbuster.png",
                    "Assets/Sprites/chrome.png",
                    "Assets/Sprites/stencil.png"
                };
                Sprite wallTagSprite = AssetDatabase.LoadAssetAtPath<Sprite>(tagStyles[i % tagStyles.Length]);
                if (wallTagSprite != null) gRen.sprite = wallTagSprite;
                tagComp.graffitiRenderer = gRen;

                cityController.districtWalls.Add(tagComp);
            }

            // 5. Interactive Landmarks
            GameObject landmarksParent = new GameObject("=== LANDMARKS ===");

            // 5a. Lowrider Car
            GameObject lowriderObj = new GameObject("Landmark_Lowrider");
            lowriderObj.transform.SetParent(landmarksParent.transform);
            lowriderObj.transform.position = new Vector3(6.15f, 8.20f, 0f);
            var lRen = lowriderObj.AddComponent<SpriteRenderer>();
            lRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/lowrider_hydraulic_car.png");
            lRen.sortingOrder = 3;
            var lCol = lowriderObj.AddComponent<BoxCollider2D>();
            lCol.size = new Vector2(1.6f, 0.9f);
            cityController.lowriderTransform = lowriderObj.transform;

            // 5b. Vending Machine
            GameObject vendObj = new GameObject("Landmark_VendingMachine");
            vendObj.transform.SetParent(landmarksParent.transform);
            vendObj.transform.position = new Vector3(12.45f, 11.30f, 0f);
            var vRen = vendObj.AddComponent<SpriteRenderer>();
            vRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/vending_machine_kiosk.png");
            vRen.sortingOrder = 3;
            var vCol = vendObj.AddComponent<BoxCollider2D>();
            vCol.size = new Vector2(0.8f, 1.0f);

            // 5c. Subway Updraft Grate
            GameObject grateObj = new GameObject("Landmark_SubwayGrate");
            grateObj.transform.SetParent(landmarksParent.transform);
            grateObj.transform.position = new Vector3(8.55f, 5.20f, 0f);
            var grRen = grateObj.AddComponent<SpriteRenderer>();
            grRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/subway_grate_updraft.png");
            grRen.sortingOrder = 1;

            // 5d. Surveillance Blimp
            GameObject blimpObj = new GameObject("Landmark_SkyBlimp");
            blimpObj.transform.SetParent(landmarksParent.transform);
            blimpObj.transform.position = new Vector3(4.0f, 21.0f, 0f);
            var bRen = blimpObj.AddComponent<SpriteRenderer>();
            bRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/surveillance_blimp_airship.png");
            bRen.sortingOrder = 15; // Above player & buildings
            cityController.blimpTransform = blimpObj.transform;

            // 5e. Rooftop Pool Table Chill Lounge
            GameObject poolObj = new GameObject("Landmark_PoolTable");
            poolObj.transform.SetParent(landmarksParent.transform);
            poolObj.transform.position = new Vector3(3.10f, 8.20f, 0f);
            var pRen = poolObj.AddComponent<SpriteRenderer>();
            pRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/rooftop_lounge_props.png");
            pRen.sortingOrder = 2;

            // 5f. Street Boombox
            GameObject boomObj = new GameObject("Landmark_Boombox");
            boomObj.transform.SetParent(landmarksParent.transform);
            boomObj.transform.position = new Vector3(11.00f, 7.92f, 0f);
            var bmRen = boomObj.AddComponent<SpriteRenderer>();
            bmRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/boombox_prop.png");
            bmRen.sortingOrder = 3;

            // 5g. Black Market Van & Contraband Vendor Stall
            GameObject bmVendorObj = new GameObject("Landmark_BlackMarketStall");
            bmVendorObj.transform.SetParent(landmarksParent.transform);
            bmVendorObj.transform.position = new Vector3(18.90f, 5.20f, 0f);
            var bmvRen = bmVendorObj.AddComponent<SpriteRenderer>();
            bmvRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/black_market_van.png");
            bmvRen.sortingOrder = 3;
            var bmvCol = bmVendorObj.AddComponent<BoxCollider2D>();
            bmvCol.size = new Vector2(1.8f, 1.2f);

            // 5h. Police Radio Jammer
            GameObject jammerObj = new GameObject("Landmark_RadioJammer");
            jammerObj.transform.SetParent(landmarksParent.transform);
            jammerObj.transform.position = new Vector3(15.70f, 18.00f, 0f);
            var jRen = jammerObj.AddComponent<SpriteRenderer>();
            jRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/police_radio_jammer.png");
            jRen.sortingOrder = 3;

            // 5i. Rooftop Water Tower Mural
            GameObject waterTowerObj = new GameObject("Landmark_WaterTowerMural");
            waterTowerObj.transform.SetParent(landmarksParent.transform);
            waterTowerObj.transform.position = new Vector3(18.90f, 18.00f, 0f);
            var wtRen = waterTowerObj.AddComponent<SpriteRenderer>();
            wtRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/water_tower_mural.png");
            wtRen.sortingOrder = 6;
            var wtCol = waterTowerObj.AddComponent<CircleCollider2D>();
            wtCol.radius = 1.2f;

            // 5j. Northern Elevated Subway Train Car
            GameObject subwayTrainObj = new GameObject("Landmark_SubwayTrainCar");
            subwayTrainObj.transform.SetParent(landmarksParent.transform);
            subwayTrainObj.transform.position = new Vector3(12.45f, 22.80f, 0f);
            var stRen = subwayTrainObj.AddComponent<SpriteRenderer>();
            stRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/subway_train_car.png");
            stRen.sortingOrder = 4;
            var stCol = subwayTrainObj.AddComponent<BoxCollider2D>();
            stCol.size = new Vector2(4.2f, 1.4f);

            // 5k. Scrambler Motorcycle
            GameObject motoObj = new GameObject("Landmark_Motorcycle");
            motoObj.transform.SetParent(landmarksParent.transform);
            motoObj.transform.position = new Vector3(4.80f, 8.20f, 0f);
            var mRen = motoObj.AddComponent<SpriteRenderer>();
            mRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/motorcycle.png");
            mRen.sortingOrder = 3;
            var mCol = motoObj.AddComponent<BoxCollider2D>();
            mCol.size = new Vector2(1.1f, 0.6f);

            // 5l. Alley Dumpsters
            for (int dIdx = 0; dIdx < 3; dIdx++)
            {
                float[] dX = new float[] { 7.5f, 14.2f, 20.5f };
                float[] dY = new float[] { 3.8f, 9.8f, 16.5f };
                GameObject dumpObj = new GameObject($"Landmark_Dumpster_{dIdx}");
                dumpObj.transform.SetParent(landmarksParent.transform);
                dumpObj.transform.position = new Vector3(dX[dIdx], dY[dIdx], 0f);
                var dRen = dumpObj.AddComponent<SpriteRenderer>();
                dRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/alley_dumpster_prop.png");
                dRen.sortingOrder = 3;
                var dCol = dumpObj.AddComponent<BoxCollider2D>();
                dCol.size = new Vector2(1.0f, 0.8f);
            }

            // 5m. Tactical Police Roadblock
            GameObject roadblockObj = new GameObject("Landmark_PoliceRoadblock");
            roadblockObj.transform.SetParent(landmarksParent.transform);
            roadblockObj.transform.position = new Vector3(12.45f, 14.60f, 0f);
            var rbRen = roadblockObj.AddComponent<SpriteRenderer>();
            rbRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/police_roadblock.png");
            rbRen.sortingOrder = 3;
            var rbCol = roadblockObj.AddComponent<BoxCollider2D>();
            rbCol.size = new Vector2(2.5f, 0.6f);

            // 5n. Block Party DJ Turntable Booth
            GameObject djObj = new GameObject("Landmark_DJBooth");
            djObj.transform.SetParent(landmarksParent.transform);
            djObj.transform.position = new Vector3(18.90f, 11.30f, 0f);
            var djRen = djObj.AddComponent<SpriteRenderer>();
            djRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/dj_turntable_booth.png");
            djRen.sortingOrder = 3;
            var djCol = djObj.AddComponent<BoxCollider2D>();
            djCol.size = new Vector2(2.2f, 1.0f);

            // 5o. Street Hydrant Geyser
            GameObject geyserObj = new GameObject("Landmark_HydrantGeyser");
            geyserObj.transform.SetParent(landmarksParent.transform);
            geyserObj.transform.position = new Vector3(15.70f, 11.30f, 0f);
            var geyRen = geyserObj.AddComponent<SpriteRenderer>();
            geyRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/hydrant_geyser.png");
            geyRen.sortingOrder = 4;

            // 5p. Sentry Paint Turret
            GameObject turretObj = new GameObject("Landmark_PaintTurret");
            turretObj.transform.SetParent(landmarksParent.transform);
            turretObj.transform.position = new Vector3(8.55f, 11.30f, 0f);
            var tRen = turretObj.AddComponent<SpriteRenderer>();
            tRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/paint_turret_prop.png");
            tRen.sortingOrder = 3;

            // 5q. Contraband Heist Crate
            GameObject crateObj = new GameObject("Landmark_HeistCrate");
            crateObj.transform.SetParent(landmarksParent.transform);
            crateObj.transform.position = new Vector3(6.15f, 14.60f, 0f);
            var crRen = crateObj.AddComponent<SpriteRenderer>();
            crRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/heist_cargo_crate.png");
            crRen.sortingOrder = 3;

            // 5r. Nozzle Tuning Bench & Air Compressor
            GameObject benchObj = new GameObject("Landmark_TuningBench");
            benchObj.transform.SetParent(landmarksParent.transform);
            benchObj.transform.position = new Vector3(3.10f, 11.30f, 0f);
            var bchRen = benchObj.AddComponent<SpriteRenderer>();
            bchRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/nozzle_tuning_bench.png");
            bchRen.sortingOrder = 3;

            GameObject compObj = new GameObject("Landmark_AirCompressor");
            compObj.transform.SetParent(landmarksParent.transform);
            compObj.transform.position = new Vector3(4.20f, 11.30f, 0f);
            var cpRen = compObj.AddComponent<SpriteRenderer>();
            cpRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/air_compressor_station.png");
            cpRen.sortingOrder = 3;

            // 5s. Street Food Truck
            GameObject foodTruckObj = new GameObject("Landmark_FoodTruck");
            foodTruckObj.transform.SetParent(landmarksParent.transform);
            foodTruckObj.transform.position = new Vector3(21.50f, 8.20f, 0f);
            var ftRen = foodTruckObj.AddComponent<SpriteRenderer>();
            ftRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/food_truck_station.png");
            ftRen.sortingOrder = 3;
            var ftCol = foodTruckObj.AddComponent<BoxCollider2D>();
            ftCol.size = new Vector2(2.6f, 1.4f);

            // 5t. Street Barrel Fire Warmup
            GameObject barrelObj = new GameObject("Landmark_BarrelFire");
            barrelObj.transform.SetParent(landmarksParent.transform);
            barrelObj.transform.position = new Vector3(9.80f, 3.80f, 0f);
            var bfRen = barrelObj.AddComponent<SpriteRenderer>();
            bfRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/barrel_fire_warmup.png");
            bfRen.sortingOrder = 3;
            var bfCol = barrelObj.AddComponent<CircleCollider2D>();
            bfCol.radius = 0.5f;

            // 5u. Block Breakdancer Performer
            GameObject dancerObj = new GameObject("Landmark_Breakdancer");
            dancerObj.transform.SetParent(landmarksParent.transform);
            dancerObj.transform.position = new Vector3(17.50f, 11.30f, 0f);
            var bdRen = dancerObj.AddComponent<SpriteRenderer>();
            bdRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/breakdancer_performer.png");
            bdRen.sortingOrder = 4;

            // Wire landmark positions to cityController
            cityController.hydrantGeyserPosition = geyserObj.transform.position;
            cityController.djBoothPosition = djObj.transform.position;
            cityController.heistCratePosition = crateObj.transform.position;
            cityController.tuningBenchPosition = benchObj.transform.position;
            cityController.airCompressorPosition = compObj.transform.position;
            cityController.motorcyclePosition = motoObj.transform.position;
            cityController.waterTowerPosition = waterTowerObj.transform.position;
            cityController.foodTruckPosition = foodTruckObj.transform.position;
            cityController.barrelFirePosition = barrelObj.transform.position;

            // 6. Street Pickups
            GameObject pickupsParent = new GameObject("=== STREET PICKUPS ===");
            PickupKind[] kinds = new PickupKind[] { PickupKind.Paint, PickupKind.Ammo, PickupKind.Health, PickupKind.Weapon, PickupKind.Recruit };
            for (int i = 0; i < 20; i++)
            {
                int rx = i % (roadX.Length - 1);
                int ry = (i / (roadX.Length - 1)) % roadY.Length;
                float px = (roadX[rx] + roadX[rx + 1]) * 0.5f;
                float py = roadY[ry];

                GameObject pObj = new GameObject($"Pickup_{i}_{kinds[i % kinds.Length]}");
                pObj.transform.SetParent(pickupsParent.transform);
                pObj.transform.position = new Vector3(px, py, 0f);
                pObj.tag = "Pickup";

                var col = pObj.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
                col.radius = 0.45f;

                var pickup = pObj.AddComponent<PickupItem>();
                pickup.kind = kinds[i % kinds.Length];

                var ren = pObj.AddComponent<SpriteRenderer>();
                ren.sortingOrder = 2;
                if (pickup.kind == PickupKind.Paint) ren.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/golden_mastery_can.png");
                else if (pickup.kind == PickupKind.Ammo) ren.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/paint_mine_trap.png");
                else if (pickup.kind == PickupKind.Health) ren.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/vending_machine_kiosk.png");
                else if (pickup.kind == PickupKind.Weapon) ren.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/grappling_hook_launcher.png");
                else if (pickup.kind == PickupKind.Recruit) ren.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/crew_classes_badges.png");
                pickup.spriteRenderer = ren;
            }

            // 7. Player GameObject
            GameObject playerObj = new GameObject("Player");
            playerObj.transform.position = new Vector3(3.10f, 5.20f, 0f);
            playerObj.tag = "Player";

            var pSpriteRen = playerObj.AddComponent<SpriteRenderer>();
            Sprite playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/hoodie1.png");
            if (playerSprite == null) playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/subway_bomber.jpg");
            pSpriteRen.sprite = playerSprite;
            pSpriteRen.sortingOrder = 5;

            var pRb = playerObj.AddComponent<Rigidbody2D>();
            pRb.gravityScale = 0f;
            pRb.freezeRotation = true;

            var pCol = playerObj.AddComponent<CircleCollider2D>();
            pCol.radius = 0.35f;

            var playerCtrl = playerObj.AddComponent<PlayerController>();

            // Player child: Nozzle
            GameObject nozzleObj = new GameObject("NozzleAim");
            nozzleObj.transform.SetParent(playerObj.transform);
            nozzleObj.transform.localPosition = new Vector3(0.35f, 0f, 0f);
            playerCtrl.nozzleTransform = nozzleObj.transform;

            // Player child: Neon Underglow Visual
            GameObject underglowObj = new GameObject("NeonUnderglowVisual");
            underglowObj.transform.SetParent(playerObj.transform);
            underglowObj.transform.localPosition = new Vector3(0f, -0.15f, 0f);
            var uRen = underglowObj.AddComponent<SpriteRenderer>();
            uRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/neon_underglow_fx.png");
            uRen.sortingOrder = 4;
            underglowObj.SetActive(false);
            playerCtrl.neonUnderglowVisual = underglowObj;

            // Player child: Bubble Shield Visual
            GameObject bubbleObj = new GameObject("BubbleShieldVisual");
            bubbleObj.transform.SetParent(playerObj.transform);
            bubbleObj.transform.localPosition = Vector3.zero;
            var bRen2 = bubbleObj.AddComponent<SpriteRenderer>();
            bRen2.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/bubble_shield_barrier.png");
            bRen2.sortingOrder = 6;
            bubbleObj.SetActive(false);
            playerCtrl.bubbleShieldVisual = bubbleObj;

            // 8. Main Camera with CameraController
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            var cam = camObj.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 4.5f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.08f, 0.09f, 0.12f);
            camObj.transform.position = new Vector3(3.10f, 5.20f, -10f);

            var camCtrl = camObj.AddComponent<CameraController>();
            camCtrl.target = playerObj.transform;
            camCtrl.cam = cam;
            camCtrl.minBounds = new Vector2(1.5f, 1.5f);
            camCtrl.maxBounds = new Vector2(24.0f, 24.0f);

            // 9. UI Canvas with UIController
            GameObject canvasObj = new GameObject("Canvas_Cyberpunk_HUD");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            var uiCtrl = canvasObj.AddComponent<UIController>();

            // Top HUD Bar Container
            GameObject topBar = CreateUIElement("TopHUDBar", canvasObj.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -30f), new Vector2(0f, 60f));

            // District Title Text
            uiCtrl.districtTitleText = CreateUIText("DistrictTitle", topBar.transform, "DISTRICT 1: CANAL INK YARD", 18, Color.yellow, new Vector2(0.02f, 0.5f), new Vector2(300f, 40f));

            // Objective Text
            uiCtrl.objectiveText = CreateUIText("ObjectiveText", topBar.transform, "OBJECTIVE: Claim 3 walls and earn 200 rep", 15, Color.white, new Vector2(0.35f, 0.5f), new Vector2(450f, 40f));

            // Cash & Rep Readouts
            uiCtrl.cashText = CreateUIText("CashText", topBar.transform, "$ 150", 18, new Color(0.3f, 0.96f, 0.84f), new Vector2(0.82f, 0.5f), new Vector2(120f, 40f));
            uiCtrl.scoreText = CreateUIText("ScoreText", topBar.transform, "REP: 0", 18, new Color(1f, 0.88f, 0.43f), new Vector2(0.92f, 0.5f), new Vector2(120f, 40f));

            // Wanted Stars
            uiCtrl.wantedStarsText = CreateUIText("WantedStars", canvasObj.transform, "", 22, new Color(1f, 0.25f, 0.35f), new Vector2(0.92f, 0.88f), new Vector2(160f, 30f));

            // Bottom Left Gauges (Health, Paint, Stamina, Ammo)
            GameObject bottomGauges = CreateUIElement("BottomGauges", canvasObj.transform, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(20f, 20f), new Vector2(250f, 140f));

            uiCtrl.healthSlider = CreateUISlider("HealthBar", bottomGauges.transform, new Color(0.95f, 0.2f, 0.2f), new Vector2(100f, 110f));
            uiCtrl.paintSlider = CreateUISlider("PaintGauge", bottomGauges.transform, new Color(0.3f, 0.96f, 0.84f), new Vector2(100f, 75f));
            uiCtrl.staminaSlider = CreateUISlider("StaminaGauge", bottomGauges.transform, new Color(0.46f, 1f, 0.01f), new Vector2(100f, 40f));
            uiCtrl.ammoText = CreateUIText("AmmoText", bottomGauges.transform, "AMMO: 48 / 72", 15, Color.white, new Vector2(0.5f, 0.1f), new Vector2(200f, 25f));

            // Combo & Camera View Text
            uiCtrl.comboText = CreateUIText("ComboBanner", canvasObj.transform, "x1 COMBO", 22, new Color(1f, 0.23f, 0.47f), new Vector2(0.5f, 0.82f), new Vector2(250f, 40f));
            uiCtrl.comboText.gameObject.SetActive(false);

            uiCtrl.cameraViewText = CreateUIText("CameraViewText", canvasObj.transform, "CLOSE / ACTION VIEW (1.85x)", 13, new Color(0.7f, 0.8f, 0.9f), new Vector2(0.5f, 0.05f), new Vector2(300f, 25f));

            // Toast Notification Panel
            GameObject toastObj = CreateUIElement("ToastNotification", canvasObj.transform, new Vector2(0.5f, 0.72f), new Vector2(0.5f, 0.72f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(550f, 50f));
            var toastBg = toastObj.AddComponent<Image>();
            toastBg.color = new Color(0.05f, 0.07f, 0.12f, 0.88f);
            uiCtrl.toastPanel = toastObj;
            uiCtrl.toastText = CreateUIText("ToastLabel", toastObj.transform, "Welcome to 404 DOPEBOYZ Street Shooters!", 16, Color.cyan, new Vector2(0.5f, 0.5f), new Vector2(520f, 40f));
            toastObj.SetActive(false);

            // Contextual Interaction Prompt Pill HUD
            GameObject promptObj = CreateUIElement("InteractionPromptPill", canvasObj.transform, new Vector2(0.5f, 0.16f), new Vector2(0.5f, 0.16f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(460f, 44f));
            var promptBg = promptObj.AddComponent<Image>();
            promptBg.color = new Color(0.08f, 0.10f, 0.16f, 0.92f);
            uiCtrl.interactionPromptPanel = promptObj;

            uiCtrl.interactionPromptKeyText = CreateUIText("PromptKey", promptObj.transform, "[ E ]", 17, new Color(0.3f, 0.96f, 0.84f), new Vector2(0.12f, 0.5f), new Vector2(70f, 36f));
            uiCtrl.interactionPromptActionText = CreateUIText("PromptAction", promptObj.transform, "Interact with Landmark", 15, Color.white, new Vector2(0.56f, 0.5f), new Vector2(370f, 36f));
            promptObj.SetActive(false);

            // Controls Quick Reference Hint Bar at bottom
            GameObject hintsObj = CreateUIElement("ControlsHintBar", canvasObj.transform, new Vector2(0.5f, 0.015f), new Vector2(0.5f, 0.015f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(850f, 26f));
            CreateUIText("HintsLabel", hintsObj.transform, "[WASD] Move | [LMB] Spray | [RMB/F] Shoot | [SPACE] Dash | [TAB] Upgrades | [Q] Crew Order | [C] Contract | [U] Bubble | [L] Slow-Mo | [J] Neon", 12, new Color(0.6f, 0.7f, 0.85f, 0.85f), new Vector2(0.5f, 0.5f), new Vector2(840f, 24f));

            // 10. Sample Rival & Cop Encounter
            GameObject rivalObj = new GameObject("Sample_Rival");
            rivalObj.transform.position = new Vector3(8.55f, 8.20f, 0f);
            rivalObj.tag = "Enemy";
            var rRen = rivalObj.AddComponent<SpriteRenderer>();
            rRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/rival.png");
            rRen.sortingOrder = 5;
            var rCol = rivalObj.AddComponent<CircleCollider2D>();
            rCol.radius = 0.35f;
            var rRb = rivalObj.AddComponent<Rigidbody2D>();
            rRb.gravityScale = 0f;
            rRb.freezeRotation = true;
            var rAct = rivalObj.AddComponent<ActorController>();
            rAct.kind = ActorKind.Rival;

            // 10. High-Fidelity Bricko Encounters
            // 10a. Rival Boss Captain
            GameObject captainObj = new GameObject("Encounter_Rival_Captain");
            captainObj.transform.position = new Vector3(23.10f, 21.90f, 0f);
            captainObj.tag = "Enemy";
            var capRen = captainObj.AddComponent<SpriteRenderer>();
            capRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/rival_boss_captain.png");
            capRen.sortingOrder = 5;
            var capCol = captainObj.AddComponent<CircleCollider2D>();
            capCol.radius = 0.45f;
            var capRb = captainObj.AddComponent<Rigidbody2D>();
            capRb.gravityScale = 0f;
            capRb.freezeRotation = true;
            var capAct = captainObj.AddComponent<ActorController>();
            capAct.kind = ActorKind.RivalCaptain;

            // 10b. Heavy Rival Enforcer
            GameObject heavyObj = new GameObject("Encounter_Heavy_Enforcer");
            heavyObj.transform.position = new Vector3(15.70f, 14.60f, 0f);
            heavyObj.tag = "Enemy";
            var hRen = heavyObj.AddComponent<SpriteRenderer>();
            hRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/heavy_enforcer.png");
            hRen.sortingOrder = 5;
            var hCol = heavyObj.AddComponent<CircleCollider2D>();
            hCol.radius = 0.5f;
            var hRb = heavyObj.AddComponent<Rigidbody2D>();
            hRb.gravityScale = 0f;
            hRb.freezeRotation = true;
            var hAct = heavyObj.AddComponent<ActorController>();
            hAct.kind = ActorKind.Rival;
            hAct.health = 250f;
            hAct.maxHealth = 250f;

            // 10c. SWAT Riot Shield Officer
            GameObject riotObj = new GameObject("Encounter_SWAT_Riot_Shield");
            riotObj.transform.position = new Vector3(11.20f, 14.60f, 0f);
            riotObj.tag = "Cop";
            var riotRen = riotObj.AddComponent<SpriteRenderer>();
            riotRen.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/riot_shield_officer.png");
            riotRen.sortingOrder = 5;
            var riotCol = riotObj.AddComponent<CircleCollider2D>();
            riotCol.radius = 0.4f;
            var riotRb = riotObj.AddComponent<Rigidbody2D>();
            riotRb.gravityScale = 0f;
            riotRb.freezeRotation = true;
            var riotAct = riotObj.AddComponent<ActorController>();
            riotAct.kind = ActorKind.SwatRiot;
            riotAct.hasRiotShield = true;

            // 10d. Police Cyber K-9 Unit
            GameObject k9Obj = new GameObject("Encounter_Police_K9");
            k9Obj.transform.position = new Vector3(13.60f, 14.60f, 0f);
            k9Obj.tag = "Cop";
            var k9Ren = k9Obj.AddComponent<SpriteRenderer>();
            k9Ren.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/police_k9_unit.png");
            k9Ren.sortingOrder = 5;
            var k9Col = k9Obj.AddComponent<CircleCollider2D>();
            k9Col.radius = 0.35f;
            var k9Rb = k9Obj.AddComponent<Rigidbody2D>();
            k9Rb.gravityScale = 0f;
            k9Rb.freezeRotation = true;
            var k9Act = k9Obj.AddComponent<ActorController>();
            k9Act.kind = ActorKind.Cop;
            k9Act.moveSpeed = 7.0f;

            // 11. Save Scene
            Directory.CreateDirectory("Assets/Scenes");
            string scenePath = "Assets/Scenes/MainScene.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);

            // Add scene to EditorBuildSettings
            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene(scenePath, true)
            };
            EditorBuildSettings.scenes = buildScenes;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[Dopeboyz] SUCCESS! MainScene.unity created and configured at {scenePath}");
        }

        private static GameObject CreateUIElement(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            RectTransform rt = obj.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;
            return obj;
        }

        private static Text CreateUIText(string name, Transform parent, string content, int fontSize, Color color, Vector2 anchor, Vector2 size)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);
            RectTransform rt = textObj.AddComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;

            Text txt = textObj.AddComponent<Text>();
            txt.text = content;
            txt.fontSize = fontSize;
            txt.color = color;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (txt.font == null) txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            return txt;
        }

        private static Slider CreateUISlider(string name, Transform parent, Color fillCol, Vector2 anchoredPos)
        {
            GameObject sliderObj = new GameObject(name);
            sliderObj.transform.SetParent(parent, false);
            RectTransform rt = sliderObj.AddComponent<RectTransform>();
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = new Vector2(180f, 18f);

            Slider slider = sliderObj.AddComponent<Slider>();

            // Background
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(sliderObj.transform, false);
            RectTransform bgRt = bg.AddComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.sizeDelta = Vector2.zero;
            Image bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0.12f, 0.14f, 0.18f, 0.8f);

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObj.transform, false);
            RectTransform faRt = fillArea.AddComponent<RectTransform>();
            faRt.anchorMin = Vector2.zero;
            faRt.anchorMax = Vector2.one;
            faRt.sizeDelta = Vector2.zero;

            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fillRt = fill.AddComponent<RectTransform>();
            fillRt.anchorMin = Vector2.zero;
            fillRt.anchorMax = Vector2.one;
            fillRt.sizeDelta = Vector2.zero;
            Image fillImg = fill.AddComponent<Image>();
            fillImg.color = fillCol;

            slider.fillRect = fillRt;
            slider.value = 1f;

            return slider;
        }
    }
}
#endif
