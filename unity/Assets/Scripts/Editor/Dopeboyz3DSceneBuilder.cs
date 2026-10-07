#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Dopeboyz.Editor
{
    public static class Dopeboyz3DSceneBuilder
    {
        [MenuItem("Dopeboyz/3. Build Full 3D Game Scene (Unity 6 3D)")]
        public static void Build3DGameScene()
        {
            Debug.Log("[Dopeboyz 3D] Starting Full 3D Scene Generation with Imported Models...");

            // 0. Force synchronous import of all newly added 3D models and textures
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            // 1. Create new empty scene
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 2. Root Managers
            GameObject managersObj = new GameObject("=== MANAGERS ===");
            var gameManager = managersObj.AddComponent<GameManager>();
            var campaignManager = managersObj.AddComponent<CampaignManager>();
            var cityController = managersObj.AddComponent<CityController>();
            var soundManager = managersObj.AddComponent<SoundManager>();

            // 3. 3D Lighting & Cyberpunk Atmosphere
            GameObject sunObj = new GameObject("Directional_Moonlight");
            var sunLight = sunObj.AddComponent<Light>();
            sunLight.type = LightType.Directional;
            sunLight.color = new Color(0.35f, 0.42f, 0.65f); // Deep cyber moonlight
            sunLight.intensity = 1.15f;
            sunObj.transform.rotation = Quaternion.Euler(50f, -35f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.12f, 0.14f, 0.22f); // Ambient city darkness

            // 4. 3D Ground & Street Grid
            GameObject groundObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            groundObj.name = "City_Ground_Asphalt";
            groundObj.transform.position = new Vector3(30f, 0f, 30f);
            groundObj.transform.localScale = new Vector3(8f, 1f, 8f); // 80m x 80m pavement
            var groundRen = groundObj.GetComponent<Renderer>();
            Material groundMat = new Material(Shader.Find("Standard"));
            groundMat.color = new Color(0.08f, 0.09f, 0.12f);
            groundMat.SetFloat("_Glossiness", 0.35f);
            groundRen.material = groundMat;

            // 4b. 3D Grassy Park Plots (Using imported 6m x 6m grassy model)
            GameObject parkParent = new GameObject("=== 3D URBAN PARK PLOTS ===");
            Vector3[] parkPositions = new Vector3[]
            {
                new Vector3(30.0f, 0.02f, 30.0f),
                new Vector3(30.0f, 0.02f, 24.0f),
                new Vector3(24.0f, 0.02f, 30.0f)
            };
            for (int p = 0; p < parkPositions.Length; p++)
            {
                var grassObj = LoadAndInstantiateModel(
                    "a 6m x 6m grassy_Textured_5102491877",
                    parkPositions[p],
                    Quaternion.identity,
                    new Vector3(3.2f, 1.0f, 3.2f),
                    parkParent.transform,
                    $"Park_Lawn_Patch_{p}"
                );
            }

            // 5. Preload 3D Models
            GameObject nycBuildingMesh1 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/NYCBuilding/mesh.obj");
            GameObject nycBuildingMesh2 = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/4-story classic New York City_Textured_5110892059/mesh.obj");
            GameObject neonSignMesh = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/NeonSignage/mesh.obj");
            GameObject capKingMesh = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/CapKing/mesh.obj");
            GameObject inkDripMesh = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/InkDrip/mesh.obj");

            Texture2D buildingTex1 = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Models/NYCBuilding/BakedTexture.png");
            Texture2D buildingTex2 = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Models/4-story classic New York City_Textured_5110892059/BakedTexture.png");
            Texture2D neonTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Models/NeonSignage/BakedTexture.png");
            Texture2D capKingTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Models/CapKing/BakedTexture.png");
            Texture2D inkDripTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Models/InkDrip/BakedTexture.png");

            Material buildingMat1 = CreateTexturedMaterial(buildingTex1, "Mat_NYCBuilding");
            Material buildingMat2 = CreateTexturedMaterial(buildingTex2, "Mat_4StoryNYC");
            Material neonMat = CreateTexturedMaterial(neonTex, "Mat_NeonSignage", true);
            Material capKingMat = CreateTexturedMaterial(capKingTex, "Mat_CapKing");
            Material inkDripMat = CreateTexturedMaterial(inkDripTex, "Mat_InkDrip");

            // 6. Build 3D City Blocks & Modular Buildings
            GameObject buildingsParent = new GameObject("=== 3D MODULAR BUILDINGS ===");
            float[] blockX = new float[] { 6f, 18f, 30f, 42f, 54f };
            float[] blockZ = new float[] { 6f, 18f, 30f, 42f, 54f };

            int bldgCount = 0;
            for (int bx = 0; bx < blockX.Length; bx++)
            {
                for (int bz = 0; bz < blockZ.Length; bz++)
                {
                    // Leave center plaza open
                    if (bx == 2 && bz == 2) continue;

                    Vector3 bPos = new Vector3(blockX[bx], 0f, blockZ[bz]);
                    GameObject bldg = null;

                    bool use4Story = (bx + bz) % 2 == 1 && nycBuildingMesh2 != null;
                    GameObject selectedMesh = use4Story ? nycBuildingMesh2 : nycBuildingMesh1;
                    Material selectedMat = use4Story ? buildingMat2 : buildingMat1;

                    if (selectedMesh != null)
                    {
                        bldg = Object.Instantiate(selectedMesh, bPos, Quaternion.Euler(0, (bx * 90) % 360, 0), buildingsParent.transform);
                        bldg.name = $"Building_{(use4Story ? "4Story" : "NYC")}_{bx}_{bz}";
                        bldg.transform.localScale = new Vector3(5.5f, 5.5f, 5.5f);
                        ApplyMaterialToRenderers(bldg, selectedMat);
                    }
                    else
                    {
                        bldg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        bldg.name = $"Building_Cube_{bx}_{bz}";
                        bldg.transform.SetParent(buildingsParent.transform);
                        bldg.transform.position = bPos + Vector3.up * 5.0f;
                        bldg.transform.localScale = new Vector3(7.5f, 10.0f, 7.5f);
                        bldg.GetComponent<Renderer>().material = selectedMat;
                    }

                    bldg.tag = "Obstacle";
                    var bCol = bldg.GetComponent<BoxCollider>();
                    if (bCol == null)
                    {
                        bCol = bldg.AddComponent<BoxCollider>();
                        bCol.size = new Vector3(8.2f, 10.5f, 7.0f);
                        bCol.center = new Vector3(0f, 5.2f, 0f);
                    }

                    // Add Rooftop Neon Signs to perimeter buildings
                    if ((bx + bz) % 2 == 0 && neonSignMesh != null)
                    {
                        Vector3 signPos = bPos + new Vector3(0f, 10.5f, 0f);
                        GameObject sign = Object.Instantiate(neonSignMesh, signPos, Quaternion.Euler(0, (bz * 90) % 360, 0), bldg.transform);
                        sign.name = "Rooftop_NeonSign";
                        sign.transform.localScale = new Vector3(3.2f, 3.2f, 3.2f);
                        ApplyMaterialToRenderers(sign, neonMat);

                        GameObject glow = new GameObject("NeonGlowLight");
                        glow.transform.SetParent(sign.transform, false);
                        var gLight = glow.AddComponent<Light>();
                        gLight.type = LightType.Point;
                        gLight.color = (bldgCount % 2 == 0) ? new Color(0.2f, 0.95f, 0.85f) : new Color(1.0f, 0.2f, 0.5f);
                        gLight.range = 14f;
                        gLight.intensity = 2.5f;
                    }

                    bldgCount++;
                }
            }

            // 7. 3D District Walls (12 Graffiti Wall Tag Spots using imported wall panels)
            GameObject wallsParent = new GameObject("=== 3D DISTRICT WALLS ===");
            Vector3[] wallPositions = new Vector3[]
            {
                new Vector3(10.5f, 1.1f, 6.0f), new Vector3(18.0f, 1.1f, 10.5f), new Vector3(25.5f, 1.1f, 6.0f),
                new Vector3(6.0f, 1.1f, 22.5f), new Vector3(18.0f, 1.1f, 25.5f), new Vector3(30.0f, 1.1f, 18.0f),
                new Vector3(42.0f, 1.1f, 10.5f), new Vector3(42.0f, 1.1f, 25.5f), new Vector3(25.5f, 1.1f, 42.0f),
                new Vector3(10.5f, 1.1f, 42.0f), new Vector3(37.5f, 1.1f, 42.0f), new Vector3(49.5f, 1.1f, 30.0f)
            };

            string[] wallNames = new string[]
            {
                "CANAL CORNER", "INK ALLEY", "MARKET SHUTTERS", "404 CROSSING", "COURT WALL", "STATIC YARD",
                "ROOTZ MURAL", "RAILCUT", "BASS BLOCK", "ROLLER HEIGHTS", "CROWN WALL", "NIGHT MARKET"
            };

            for (int i = 0; i < wallPositions.Length; i++)
            {
                string wallFolder = (i % 2 == 0) ? "wall-panel-01_Textured_4723584732" : "wall-panel-01_Textured_4723591882";
                GameObject wallObj = LoadAndInstantiateModel(
                    wallFolder,
                    wallPositions[i],
                    Quaternion.identity,
                    new Vector3(1.8f, 2.2f, 2.5f),
                    wallsParent.transform,
                    $"WallTag_{i}_{wallNames[i]}",
                    false,
                    true,
                    new Vector3(3.4f, 2.2f, 0.45f),
                    new Vector3(0f, 1.1f, 0f)
                );

                if (wallObj == null)
                {
                    wallObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wallObj.name = $"WallTag_{i}_{wallNames[i]}";
                    wallObj.transform.SetParent(wallsParent.transform);
                    wallObj.transform.position = wallPositions[i];
                    wallObj.transform.localScale = new Vector3(3.2f, 2.2f, 0.35f);
                }

                wallObj.tag = "Wall";

                var wTag = wallObj.AddComponent<WallTag>();
                wTag.spotName = wallNames[i];
                wTag.spotIndex = i;
                wTag.spotSize = 1 + (i % 3);

                // Graffiti display Quad on wall front
                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.name = "GraffitiFace";
                quad.transform.SetParent(wallObj.transform, false);
                quad.transform.localPosition = new Vector3(0f, 1.1f, 0.28f);
                quad.transform.localScale = new Vector3(2.8f, 1.8f, 1.0f);
                var qCol = quad.GetComponent<Collider>();
                if (qCol != null) Object.DestroyImmediate(qCol);

                var qRen = quad.GetComponent<Renderer>();
                Material grafMat = new Material(Shader.Find("Standard"));
                grafMat.color = new Color(0.9f, 0.85f, 0.4f, 0.8f);
                qRen.material = grafMat;
                wTag.graffitiRenderer = qRen;

                cityController.districtWalls.Add(wTag);
            }

            // 8. 3D Street Landmarks (Using Imported AI Models!)
            GameObject landmarksParent = new GameObject("=== 3D LANDMARKS ===");

            // 8a. Hydraulic Lowrider Car (Classic 1964 vintage American lowrider)
            GameObject lowriderObj = LoadAndInstantiateModel(
                "Classic 1964 vintage American lowrider_Textured_5109892043",
                cityController.lowriderPosition,
                Quaternion.Euler(0, 45f, 0),
                new Vector3(2.4f, 2.0f, 2.4f),
                landmarksParent.transform,
                "Landmark_3D_Lowrider",
                false,
                true,
                new Vector3(1.8f, 1.3f, 4.6f),
                new Vector3(0f, 0.65f, 0f)
            );
            if (lowriderObj != null)
            {
                lowriderObj.tag = "Obstacle";
                cityController.lowriderTransform = lowriderObj.transform;
            }

            // 8b. Neon Soda Vending Machine (Retro-futuristic Japanese-style beverage vending machine)
            GameObject vendObj = LoadAndInstantiateModel(
                "Retro-futuristic Japanese-style beverage vending machine_Textured_5110392046",
                cityController.vendingPosition,
                Quaternion.Euler(0, 180f, 0),
                new Vector3(1.15f, 1.15f, 1.15f),
                landmarksParent.transform,
                "Landmark_3D_VendingMachine",
                true,
                true,
                new Vector3(1.7f, 2.2f, 1.1f),
                new Vector3(0f, 1.1f, 0f)
            );
            if (vendObj != null)
            {
                GameObject vLight = new GameObject("VendingNeonGlow");
                vLight.transform.SetParent(vendObj.transform, false);
                vLight.transform.localPosition = new Vector3(0f, 1.2f, 0.8f);
                var vl = vLight.AddComponent<Light>();
                vl.type = LightType.Point;
                vl.color = new Color(0.2f, 0.95f, 0.85f);
                vl.range = 5.5f;
                vl.intensity = 2.4f;
            }

            // 8c. Subway Station Entrance (Underground subway metro station street)
            GameObject subwayObj = LoadAndInstantiateModel(
                "Underground subway metro station street_Textured_5110992060",
                cityController.subwayGratePosition,
                Quaternion.Euler(0, 90f, 0),
                new Vector3(2.2f, 2.0f, 2.2f),
                landmarksParent.transform,
                "Landmark_3D_SubwayEntrance",
                false,
                true,
                new Vector3(3.0f, 2.5f, 3.0f),
                new Vector3(0f, 1.2f, 0f)
            );
            // Trigger collider for upward steam vent
            GameObject grateTrigger = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            grateTrigger.name = "SubwaySteamUpdraftTrigger";
            grateTrigger.transform.SetParent(landmarksParent.transform);
            grateTrigger.transform.position = cityController.subwayGratePosition;
            grateTrigger.transform.localScale = new Vector3(2.2f, 0.05f, 2.2f);
            var grCol = grateTrigger.GetComponent<Collider>();
            if (grCol != null) grCol.isTrigger = true;

            // 8d. Secret Sewer Manhole Cover
            GameObject manholeObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            manholeObj.name = "Landmark_3D_SewerManhole";
            manholeObj.transform.SetParent(landmarksParent.transform);
            manholeObj.transform.position = cityController.manholePosition;
            manholeObj.transform.localScale = new Vector3(1.4f, 0.04f, 1.4f);
            var mhRen = manholeObj.GetComponent<Renderer>();
            Material mhMat = new Material(Shader.Find("Standard"));
            mhMat.color = new Color(0.25f, 0.28f, 0.32f);
            mhRen.material = mhMat;

            // 8e. Street Barrel Fire Warmup Zone
            GameObject barrelObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            barrelObj.name = "Landmark_3D_BarrelFire";
            barrelObj.transform.SetParent(landmarksParent.transform);
            barrelObj.transform.position = cityController.barrelFirePosition;
            barrelObj.transform.localScale = new Vector3(0.9f, 1.2f, 0.9f);

            GameObject fireLightObj = new GameObject("BarrelFireWarmLight");
            fireLightObj.transform.SetParent(barrelObj.transform, false);
            fireLightObj.transform.localPosition = Vector3.up * 0.8f;
            var fLight = fireLightObj.AddComponent<Light>();
            fLight.type = LightType.Point;
            fLight.color = new Color(1.0f, 0.45f, 0.1f);
            fLight.range = 8.5f;
            fLight.intensity = 3.2f;

            // 8f. Rooftop Water Tower Mega-Mural
            GameObject wtObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wtObj.name = "Landmark_3D_WaterTower";
            wtObj.transform.SetParent(landmarksParent.transform);
            wtObj.transform.position = cityController.waterTowerPosition;
            wtObj.transform.localScale = new Vector3(3.2f, 3.5f, 3.2f);
            var wtRen = wtObj.GetComponent<Renderer>();
            Material wtMat = new Material(Shader.Find("Standard"));
            wtMat.color = new Color(0.45f, 0.35f, 0.25f);
            wtRen.material = wtMat;

            // 8g. Industrial Air Compressor Station (Heavy industrial dual-cylinder workshop air)
            GameObject compObj = LoadAndInstantiateModel(
                "Heavy industrial dual-cylinder workshop air_Textured_5110492045",
                cityController.airCompressorPosition,
                Quaternion.identity,
                new Vector3(0.95f, 0.95f, 0.95f),
                landmarksParent.transform,
                "Landmark_3D_AirCompressor",
                false,
                true,
                new Vector3(1.8f, 1.7f, 1.0f),
                new Vector3(0f, 0.85f, 0f)
            );

            // 8h. Nozzle Tuning Workbench & Stencil Plates (Rugged mechanical workshop bench table)
            GameObject benchObj = LoadAndInstantiateModel(
                "Rugged mechanical workshop bench table_Textured_5110592044",
                cityController.tuningBenchPosition,
                Quaternion.Euler(0, 90f, 0),
                new Vector3(1.1f, 1.05f, 1.1f),
                landmarksParent.transform,
                "Landmark_3D_TuningBench",
                false,
                true,
                new Vector3(2.1f, 1.6f, 1.0f),
                new Vector3(0f, 0.8f, 0f)
            );
            // Master Stencil Plates placed on the tuning bench
            LoadAndInstantiateModel(
                "Heavy-gauge industrial laser-cut stainless steel_Textured_5109692051",
                cityController.tuningBenchPosition + new Vector3(0.35f, 0.95f, 0f),
                Quaternion.Euler(90f, 0f, 25f),
                new Vector3(0.4f, 0.4f, 0.4f),
                landmarksParent.transform,
                "Stencil_Stamp_Master_Plates"
            );

            // 8i. DJ Turntable Booth & Street Boombox (Classic 1980s retro-futuristic urban dual-cassette)
            GameObject djObj = LoadAndInstantiateModel(
                "Classic 1980s retro-futuristic urban dual-cassette_Textured_5106092024",
                cityController.djBoothPosition,
                Quaternion.identity,
                new Vector3(1.15f, 1.15f, 1.15f),
                landmarksParent.transform,
                "Landmark_3D_DJBooth",
                true,
                true,
                new Vector3(2.2f, 1.4f, 0.6f),
                new Vector3(0f, 0.7f, 0f)
            );
            // Secondary portable boombox at canal corner
            LoadAndInstantiateModel(
                "Classic 1980s retro-futuristic urban dual-cassette_Textured_5106092024",
                cityController.boomboxPosition,
                Quaternion.Euler(0, 30f, 0),
                new Vector3(0.65f, 0.65f, 0.65f),
                landmarksParent.transform,
                "Street_Boombox_Prop",
                true
            );

            // 8j. Gourmet Food Truck - Taco Wagon (Custom commercial step-van gourmet street)
            GameObject foodTruckObj = LoadAndInstantiateModel(
                "Custom commercial step-van gourmet street_Textured_5111092063",
                cityController.foodTruckPosition,
                Quaternion.Euler(0, -90f, 0),
                new Vector3(2.4f, 2.2f, 2.2f),
                landmarksParent.transform,
                "Landmark_3D_GourmetFoodTruck",
                false,
                true,
                new Vector3(4.5f, 2.5f, 2.3f),
                new Vector3(0f, 1.25f, 0f)
            );
            if (foodTruckObj != null)
            {
                foodTruckObj.tag = "Obstacle";
                GameObject awningLight = new GameObject("FoodTruckAwningLight");
                awningLight.transform.SetParent(foodTruckObj.transform, false);
                awningLight.transform.localPosition = new Vector3(0f, 1.8f, 1.2f);
                var al = awningLight.AddComponent<Light>();
                al.type = LightType.Point;
                al.color = new Color(1.0f, 0.85f, 0.6f);
                al.range = 6.5f;
                al.intensity = 2.8f;
            }

            // 8k. TV News Broadcast Camera Drone (Futuristic city news television camera)
            GameObject newsDroneObj = LoadAndInstantiateModel(
                "Futuristic city news television camera_Textured_5110292047",
                cityController.newsChopperPosition,
                Quaternion.identity,
                new Vector3(1.6f, 1.6f, 1.6f),
                landmarksParent.transform,
                "Landmark_3D_NewsCameraDrone",
                true
            );
            if (newsDroneObj != null)
            {
                cityController.newsChopperTransform = newsDroneObj.transform;

                GameObject spotLightObj = new GameObject("NewsDroneSearchLight");
                spotLightObj.transform.SetParent(newsDroneObj.transform, false);
                var sl = spotLightObj.AddComponent<Light>();
                sl.type = LightType.Spot;
                sl.color = Color.white;
                sl.spotAngle = 45f;
                sl.range = 35f;
                sl.intensity = 4.5f;
                spotLightObj.transform.rotation = Quaternion.Euler(75f, 0f, 0f);
            }

            // 8l. Police Recon Surveillance Drone (Tactical compact quadcopter surveillance drone)
            GameObject reconDroneObj = LoadAndInstantiateModel(
                "Tactical compact quadcopter surveillance drone._Textured_5109592040",
                new Vector3(25f, 6.0f, 25f),
                Quaternion.identity,
                new Vector3(0.75f, 0.75f, 0.75f),
                landmarksParent.transform,
                "Landmark_3D_ReconSurveillanceDrone",
                true
            );
            if (reconDroneObj != null)
            {
                cityController.reconDroneTransform = reconDroneObj.transform;

                // Red/Blue strobe light
                GameObject strobeObj = new GameObject("ReconStrobeLight");
                strobeObj.transform.SetParent(reconDroneObj.transform, false);
                var rl = strobeObj.AddComponent<Light>();
                rl.type = LightType.Point;
                rl.color = new Color(0.2f, 0.5f, 1.0f);
                rl.range = 8f;
                rl.intensity = 3.0f;
            }

            // 8m. Contraband Heist Crates (Military-grade reinforced black Pelican-style polymer)
            GameObject heistCrate1 = LoadAndInstantiateModel(
                "Military-grade reinforced black Pelican-style polymer_Textured_5110792058",
                cityController.heistCratePosition,
                Quaternion.identity,
                new Vector3(0.8f, 0.8f, 0.8f),
                landmarksParent.transform,
                "Landmark_3D_HeistCrate",
                false,
                true,
                new Vector3(1.5f, 0.7f, 1.1f),
                new Vector3(0f, 0.35f, 0f)
            );
            // Secret rooftop heist stash crate
            LoadAndInstantiateModel(
                "Military-grade reinforced black Pelican-style polymer_Textured_5110792058",
                new Vector3(18.0f, 10.5f, 18.0f),
                Quaternion.Euler(0, 45f, 0),
                new Vector3(0.8f, 0.8f, 0.8f),
                landmarksParent.transform,
                "Rooftop_Secret_HeistCrate",
                false,
                true
            );

            // 8n. Tactical Turf War Burning Road Flares (Tactical handheld emergency road flare)
            Vector3[] flarePositions = new Vector3[]
            {
                new Vector3(12.45f, 0.05f, 8.20f),
                new Vector3(18.90f, 0.05f, 14.60f),
                new Vector3(6.15f, 0.05f, 18.00f)
            };
            for (int f = 0; f < flarePositions.Length; f++)
            {
                GameObject flareObj = LoadAndInstantiateModel(
                    "Tactical handheld emergency road flare_Textured_5105992028",
                    flarePositions[f],
                    Quaternion.Euler(-10f, f * 60f, 0),
                    new Vector3(0.45f, 0.45f, 0.45f),
                    landmarksParent.transform,
                    $"TurfWar_RoadFlare_{f}"
                );
                if (flareObj != null)
                {
                    GameObject flLight = new GameObject("FlareCombustionLight");
                    flLight.transform.SetParent(flareObj.transform, false);
                    flLight.transform.localPosition = Vector3.up * 0.9f;
                    var fl = flLight.AddComponent<Light>();
                    fl.type = LightType.Point;
                    fl.color = new Color(1.0f, 0.25f, 0.1f);
                    fl.range = 7.5f;
                    fl.intensity = 3.2f;
                }
            }

            // 8o. Sky Surveillance Blimp
            GameObject blimpObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            blimpObj.name = "Landmark_3D_SkyBlimp";
            blimpObj.transform.SetParent(landmarksParent.transform);
            blimpObj.transform.position = new Vector3(5f, 25f, 30f);
            blimpObj.transform.localScale = new Vector3(4f, 2f, 9f);
            cityController.blimpTransform = blimpObj.transform;

            // 8p. City Hall Victory Crown Monument & Floating Graffiti Ghost Mascot
            GameObject crownObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            crownObj.name = "Landmark_3D_CityHallCrown";
            crownObj.transform.SetParent(landmarksParent.transform);
            crownObj.transform.position = cityController.cityHallMonumentPosition;
            crownObj.transform.localScale = new Vector3(3.5f, 2.5f, 3.5f);
            var crRen = crownObj.GetComponent<Renderer>();
            Material crMat = new Material(Shader.Find("Standard"));
            crMat.color = new Color(1.0f, 0.85f, 0.3f);
            crMat.SetFloat("_Metallic", 0.9f);
            crMat.SetFloat("_Glossiness", 0.9f);
            crRen.material = crMat;

            // Floating Mascot above monument (Graffiti Ghost)
            GameObject ghostPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/GraffitiGhost/graffitighost.glb");
            if (ghostPrefab != null)
            {
                GameObject ghostObj = Object.Instantiate(ghostPrefab, cityController.cityHallMonumentPosition + Vector3.up * 3.8f, Quaternion.identity, landmarksParent.transform);
                ghostObj.name = "GraffitiGhost_Mascot";
                ghostObj.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }

            // 9. 3D Pickups across city sidewalks
            GameObject pickupsParent = new GameObject("=== 3D STREET PICKUPS ===");
            PickupKind[] pKinds = new PickupKind[] { PickupKind.Paint, PickupKind.Ammo, PickupKind.Health, PickupKind.Weapon, PickupKind.Recruit };
            Color[] pColors = new Color[] { new Color(0.3f, 0.96f, 0.84f), new Color(1.0f, 0.4f, 0.1f), new Color(0.95f, 0.2f, 0.2f), new Color(1.0f, 0.88f, 0.2f), new Color(0.6f, 0.3f, 0.95f) };

            for (int pIdx = 0; pIdx < 20; pIdx++)
            {
                float px = 8f + (pIdx % 5) * 10f;
                float pz = 8f + (pIdx / 5) * 10f;
                Vector3 pos = new Vector3(px, 0.65f, pz);

                GameObject pObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                pObj.name = $"Pickup_3D_{pIdx}_{pKinds[pIdx % pKinds.Length]}";
                pObj.transform.SetParent(pickupsParent.transform);
                pObj.transform.position = pos;
                pObj.transform.localScale = Vector3.one * 0.65f;
                pObj.tag = "Pickup";

                var col = pObj.GetComponent<SphereCollider>();
                if (col != null) col.isTrigger = true;

                var ren = pObj.GetComponent<Renderer>();
                Material pMat = new Material(Shader.Find("Standard"));
                pMat.color = pColors[pIdx % pColors.Length];
                pMat.SetFloat("_Glossiness", 0.8f);
                ren.material = pMat;

                var pComp = pObj.AddComponent<PickupItem>();
                pComp.kind = pKinds[pIdx % pKinds.Length];
                pComp.meshRenderer = ren as MeshRenderer;
            }

            // 10. 3D Player GameObject (CapKing + Wearable Jetpack Backpack + Grapple Gun!)
            GameObject playerObj = new GameObject("Player");
            playerObj.transform.position = new Vector3(12.45f, 0.0f, 11.30f);
            playerObj.tag = "Player";

            var pRb = playerObj.AddComponent<Rigidbody>();
            pRb.mass = 75f;
            pRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            var pCol = playerObj.AddComponent<CapsuleCollider>();
            pCol.radius = 0.45f;
            pCol.height = 1.9f;
            pCol.center = new Vector3(0f, 0.95f, 0f);

            var playerCtrl = playerObj.AddComponent<PlayerController>();

            // Instantiate Cap King 3D Character Mesh
            if (capKingMesh != null)
            {
                GameObject ckModel = Object.Instantiate(capKingMesh, playerObj.transform);
                ckModel.name = "CapKing_3D_Mesh";
                ckModel.transform.localPosition = Vector3.zero;
                ckModel.transform.localRotation = Quaternion.identity;
                ckModel.transform.localScale = Vector3.one;
                ApplyMaterialToRenderers(ckModel, capKingMat);
            }
            else
            {
                GameObject tempCapsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                tempCapsule.transform.SetParent(playerObj.transform, false);
                tempCapsule.transform.localPosition = new Vector3(0, 0.95f, 0);
                var cCol = tempCapsule.GetComponent<Collider>();
                if (cCol != null) Object.DestroyImmediate(cCol);
                tempCapsule.GetComponent<Renderer>().material = capKingMat;
            }

            // Wearable Dual-Tank Aerosol Jetpack Harness mounted on Player's back
            LoadAndInstantiateModel(
                "Wearable dual-tank aerosol jetpack harness._Textured_5105891960",
                Vector3.zero,
                Quaternion.Euler(0, 180f, 0),
                new Vector3(0.48f, 0.48f, 0.48f),
                playerObj.transform,
                "Equipped_Jetpack_Harness"
            );
            var jpTransform = playerObj.transform.Find("Equipped_Jetpack_Harness");
            if (jpTransform != null) jpTransform.localPosition = new Vector3(0f, 0.95f, -0.22f);

            // Heavy Pneumatic Grapple Gun Launcher mounted at Player's side
            LoadAndInstantiateModel(
                "Heavy pneumatic grapple gun launcher._Textured_5106192021",
                Vector3.zero,
                Quaternion.Euler(15f, 0f, 0f),
                new Vector3(0.35f, 0.35f, 0.35f),
                playerObj.transform,
                "Equipped_Grapple_Launcher"
            );
            var gpTransform = playerObj.transform.Find("Equipped_Grapple_Launcher");
            if (gpTransform != null) gpTransform.localPosition = new Vector3(0.35f, 0.9f, 0.35f);

            // Player child: Nozzle
            GameObject nozzleObj = new GameObject("NozzleAim");
            nozzleObj.transform.SetParent(playerObj.transform, false);
            nozzleObj.transform.localPosition = new Vector3(0.35f, 1.1f, 0.45f);
            playerCtrl.nozzleTransform = nozzleObj.transform;

            // Player child: Skateboard Neon Underglow Light
            GameObject underglowObj = new GameObject("NeonUnderglowLight");
            underglowObj.transform.SetParent(playerObj.transform, false);
            underglowObj.transform.localPosition = new Vector3(0f, 0.12f, 0f);
            var uLight = underglowObj.AddComponent<Light>();
            uLight.type = LightType.Point;
            uLight.color = new Color(0.2f, 0.95f, 0.85f);
            uLight.range = 4.5f;
            uLight.intensity = 2.0f;
            underglowObj.SetActive(false);
            playerCtrl.neonUnderglowVisual = underglowObj;

            // Player child: Pressurized Bubble Shield 3D Sphere
            GameObject bubbleObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bubbleObj.name = "BubbleShield3DVisual";
            bubbleObj.transform.SetParent(playerObj.transform, false);
            bubbleObj.transform.localPosition = new Vector3(0f, 0.95f, 0f);
            bubbleObj.transform.localScale = Vector3.one * 2.4f;
            var bCol2 = bubbleObj.GetComponent<Collider>();
            if (bCol2 != null) Object.DestroyImmediate(bCol2);
            var bRen = bubbleObj.GetComponent<Renderer>();
            Material bMat = new Material(Shader.Find("Standard"));
            bMat.color = new Color(0.3f, 0.95f, 0.85f, 0.45f);
            bRen.material = bMat;
            bubbleObj.SetActive(false);
            playerCtrl.bubbleShieldVisual = bubbleObj;

            // 11. 3D Rival Boss Captain (Using InkDrip Model!)
            GameObject rivalObj = new GameObject("Rival_Boss_Captain");
            rivalObj.transform.position = new Vector3(38.0f, 0.0f, 38.0f);
            rivalObj.tag = "Enemy";

            var rRb = rivalObj.AddComponent<Rigidbody>();
            rRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            var rCol = rivalObj.AddComponent<CapsuleCollider>();
            rCol.radius = 0.5f;
            rCol.height = 1.9f;
            rCol.center = new Vector3(0f, 0.95f, 0f);

            var rAct = rivalObj.AddComponent<ActorController>();
            rAct.kind = ActorKind.RivalCaptain;

            if (inkDripMesh != null)
            {
                GameObject idModel = Object.Instantiate(inkDripMesh, rivalObj.transform);
                idModel.name = "InkDrip_3D_Mesh";
                idModel.transform.localPosition = Vector3.zero;
                idModel.transform.localRotation = Quaternion.identity;
                idModel.transform.localScale = Vector3.one;
                ApplyMaterialToRenderers(idModel, inkDripMat);
            }
            else
            {
                GameObject tempCapsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                tempCapsule.transform.SetParent(rivalObj.transform, false);
                tempCapsule.transform.localPosition = new Vector3(0, 0.95f, 0);
                var cCol = tempCapsule.GetComponent<Collider>();
                if (cCol != null) Object.DestroyImmediate(cCol);
                tempCapsule.GetComponent<Renderer>().material = inkDripMat;
            }

            // 12. 3D SWAT Riot Shield Officer
            GameObject swatObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            swatObj.name = "SWAT_Riot_Shield_Officer";
            swatObj.transform.position = new Vector3(22.0f, 0.0f, 22.0f);
            swatObj.tag = "Cop";
            var sRb = swatObj.AddComponent<Rigidbody>();
            sRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            var sAct = swatObj.AddComponent<ActorController>();
            sAct.kind = ActorKind.SwatRiot;
            sAct.hasRiotShield = true;

            // 13. Main Camera with 3D CameraController
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            var cam = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>();
            cam.fieldOfView = 50f;
            cam.nearClipPlane = 0.3f;
            cam.farClipPlane = 300f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.06f, 0.07f, 0.11f);
            camObj.transform.position = playerObj.transform.position + new Vector3(0f, 4.8f, -6.2f);

            var camCtrl = camObj.AddComponent<CameraController>();
            camCtrl.target = playerObj.transform;
            camCtrl.cam = cam;
            camCtrl.minBounds = new Vector3(0f, 0f, 0f);
            camCtrl.maxBounds = new Vector3(65f, 50f, 65f);

            // 14. Cyberpunk UI HUD Canvas
            BuildCyberpunkHUD(camObj);

            // 15. Save 3D Scene
            Directory.CreateDirectory("Assets/Scenes");
            string scenePath = "Assets/Scenes/MainScene.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);

            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene(scenePath, true)
            };
            EditorBuildSettings.scenes = buildScenes;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[Dopeboyz 3D] SUCCESS! MainScene.unity rebuilt in FULL 3D with all new AI models at {scenePath}");
        }

        private static GameObject LoadAndInstantiateModel(
            string folderName,
            Vector3 position,
            Quaternion rotation,
            Vector3 scale,
            Transform parent,
            string customName = null,
            bool emissive = false,
            bool addBoxCollider = false,
            Vector3 colSize = default,
            Vector3 colCenter = default)
        {
            string objPath = $"Assets/Models/{folderName}/mesh.obj";
            string texPath = $"Assets/Models/{folderName}/BakedTexture.png";

            GameObject meshPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(objPath);
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);

            if (meshPrefab != null)
            {
                GameObject go = Object.Instantiate(meshPrefab, position, rotation, parent);
                go.name = !string.IsNullOrEmpty(customName) ? customName : folderName;
                go.transform.localScale = scale;

                if (tex != null)
                {
                    Material mat = CreateTexturedMaterial(tex, $"Mat_{folderName}", emissive);
                    ApplyMaterialToRenderers(go, mat);
                }

                if (addBoxCollider)
                {
                    var col = go.GetComponent<BoxCollider>();
                    if (col == null) col = go.AddComponent<BoxCollider>();
                    if (colSize != Vector3.zero) col.size = colSize;
                    if (colCenter != Vector3.zero) col.center = colCenter;
                }

                return go;
            }

            return null;
        }

        private static Material CreateTexturedMaterial(Texture2D tex, string matName, bool emissive = false)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.name = matName;
            if (tex != null)
            {
                mat.mainTexture = tex;
            }
            if (emissive && tex != null)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetTexture("_EmissionMap", tex);
                mat.SetColor("_EmissionColor", new Color(0.3f, 0.95f, 0.85f));
            }
            mat.SetFloat("_Glossiness", 0.45f);
            return mat;
        }

        private static void ApplyMaterialToRenderers(GameObject root, Material mat)
        {
            var renderers = root.GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers)
            {
                r.material = mat;
            }
        }

        private static void BuildCyberpunkHUD(GameObject camObj)
        {
            GameObject canvasObj = new GameObject("Canvas_Cyberpunk_HUD");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            var uiCtrl = canvasObj.AddComponent<UIController>();

            // Top HUD Bar Container
            GameObject topBar = CreateUIElement("TopHUDBar", canvasObj.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -30f), new Vector2(0f, 60f));

            // District Title Text
            uiCtrl.districtTitleText = CreateUIText("DistrictTitle", topBar.transform, "DISTRICT 1: CANAL INK YARD (3D)", 18, Color.yellow, new Vector2(0.02f, 0.5f), new Vector2(320f, 40f));

            // Objective Text
            uiCtrl.objectiveText = CreateUIText("ObjectiveText", topBar.transform, "OBJECTIVE: Claim 3 walls and earn 200 rep", 15, Color.white, new Vector2(0.35f, 0.5f), new Vector2(450f, 40f));

            // Cash & Rep Readouts
            uiCtrl.cashText = CreateUIText("CashText", topBar.transform, "$ 150", 18, new Color(0.3f, 0.96f, 0.84f), new Vector2(0.82f, 0.5f), new Vector2(120f, 40f));
            uiCtrl.scoreText = CreateUIText("ScoreText", topBar.transform, "REP: 0", 18, new Color(1f, 0.88f, 0.43f), new Vector2(0.92f, 0.5f), new Vector2(120f, 40f));

            // Wanted Stars
            uiCtrl.wantedStarsText = CreateUIText("WantedStars", canvasObj.transform, "", 22, new Color(1f, 0.25f, 0.35f), new Vector2(0.92f, 0.88f), new Vector2(160f, 30f));

            // Bottom Left Gauges
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
            GameObject toastObj = CreateUIElement("ToastNotification", canvasObj.transform, new Vector2(0.5f, 0.72f), new Vector2(0.5f, 0.72f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(580f, 50f));
            var toastBg = toastObj.AddComponent<Image>();
            toastBg.color = new Color(0.05f, 0.07f, 0.12f, 0.88f);
            uiCtrl.toastPanel = toastObj;
            uiCtrl.toastText = CreateUIText("ToastLabel", toastObj.transform, "Welcome to 404 DOPEBOYZ 3D Urban Warfare!", 16, Color.cyan, new Vector2(0.5f, 0.5f), new Vector2(550f, 40f));
            toastObj.SetActive(false);

            // Contextual Interaction Prompt Pill HUD
            GameObject promptObj = CreateUIElement("InteractionPromptPill", canvasObj.transform, new Vector2(0.5f, 0.16f), new Vector2(0.5f, 0.16f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(460f, 44f));
            var promptBg = promptObj.AddComponent<Image>();
            promptBg.color = new Color(0.08f, 0.10f, 0.16f, 0.92f);
            uiCtrl.interactionPromptPanel = promptObj;

            uiCtrl.interactionPromptKeyText = CreateUIText("PromptKey", promptObj.transform, "[ E ]", 17, new Color(0.3f, 0.96f, 0.84f), new Vector2(0.12f, 0.5f), new Vector2(70f, 36f));
            uiCtrl.interactionPromptActionText = CreateUIText("PromptAction", promptObj.transform, "Interact with Landmark", 15, Color.white, new Vector2(0.56f, 0.5f), new Vector2(370f, 36f));
            promptObj.SetActive(false);

            // Controls Quick Reference Hint Bar
            GameObject hintsObj = CreateUIElement("ControlsHintBar", canvasObj.transform, new Vector2(0.5f, 0.015f), new Vector2(0.5f, 0.015f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(920f, 26f));
            CreateUIText("HintsLabel", hintsObj.transform, "[WASD] Move/Skate | [SPACE] Jump/Trick/Grind | [LMB] Spray | [RMB] Shoot | [TAB] Black Market | [V] Drone | [T] Nozzle | [1-6] Stencil | [N] Turret | [K] Mine | [U] Bubble | [L] Slow-Mo | [F1] Camera", 11, new Color(0.7f, 0.85f, 1f, 0.85f), new Vector2(0.5f, 0.5f), new Vector2(910f, 24f));
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
            txt.font = UnityEngine.Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (txt.font == null) txt.font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");

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

            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(sliderObj.transform, false);
            RectTransform bgRt = bg.AddComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.sizeDelta = Vector2.zero;
            Image bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0.12f, 0.14f, 0.18f, 0.8f);

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
