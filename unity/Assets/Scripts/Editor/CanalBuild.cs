#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Dopeboyz.Editor
{
    public static class CanalBuild
    {
        [MenuItem("Dopeboyz/Prepare October Prefabs")]
        public static void PrepareOctober()
        {
            Directory.CreateDirectory("Assets/Resources/CanalOctober");
            AssetDatabase.Refresh();
            string[] names = {"Bench","YardFence","WallLight"};
            string[] folders = {"BENCH_Textured_4722484705","Tall-Yard-Fence_Textured_4717784612","wall-light_Textured_4722584707"};
            for (int i = 0; i < names.Length; i++)
            {
                string source = "Assets/resources-oct/New Resources 01/" + folders[i];
                var model = AssetDatabase.LoadAssetAtPath<GameObject>(source + "/mesh.obj");
                if (model == null) throw new InvalidOperationException("Missing source mesh: " + source);
                var instance = UnityEngine.Object.Instantiate(model);
                instance.name = names[i];
                string materialPath = "Assets/Resources/CanalOctober/" + names[i] + ".mat";
                var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                if (material == null) { material = new Material(Shader.Find("Standard")); AssetDatabase.CreateAsset(material,materialPath); }
                material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(source + "/BakedTexture.png");
                foreach (var renderer in instance.GetComponentsInChildren<Renderer>()) renderer.sharedMaterial = material;
                PrefabUtility.SaveAsPrefabAsset(instance,"Assets/Resources/CanalOctober/" + names[i] + ".prefab");
                UnityEngine.Object.DestroyImmediate(instance);
            }
            AssetDatabase.SaveAssets();
        }
        [MenuItem("Dopeboyz/Validate October Resources")]
        public static void Validate()
        {
            string[] keys = {"Characters/InmateThreePlayable", "CanalOctober/Bench", "CanalOctober/YardFence", "CanalOctober/WallLight"};
            foreach (string key in keys)
            {
                var asset = Resources.Load<GameObject>(key);
                if (asset == null) throw new InvalidOperationException("Missing October resource: " + key);
                if (asset.GetComponentsInChildren<Renderer>().Length == 0) throw new InvalidOperationException("No renderers: " + key);
                Debug.Log("CANAL ASSET PASS " + key);
            }
        }
        [MenuItem("Dopeboyz/Build Canal Windows Player")]
        public static void BuildWindows()
        {
            PrepareOctober();
            Validate();
            Directory.CreateDirectory("Builds/Canal");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = new[]{"Assets/Scenes/MainScene.unity"},
                locationPathName = "Builds/Canal/StreetShooters.exe", target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development
            });
            if (report.summary.result != BuildResult.Succeeded) throw new InvalidOperationException("Canal build failed: " + report.summary.result);
            Debug.Log("CANAL BUILD SUCCESS");
        }
    }
}
#endif
