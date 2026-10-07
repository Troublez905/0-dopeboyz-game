using System.Collections.Generic;
using UnityEngine;

namespace Dopeboyz
{
    public static class CanalSceneRepair
    {
        public static void Apply()
        {
            var player = PlayerController.Instance;
            var city = CityController.Instance;
            if(city!=null)
            {
                city.hqPosition=new Vector3(30,.1f,30);
                city.lowriderPosition=new Vector3(25,.5f,30);
                city.vendingPosition=new Vector3(34,.5f,30);
                city.tuningBenchPosition=new Vector3(30,.5f,34);
                city.airCompressorPosition=new Vector3(28,.5f,34);
                city.heistCratePosition=new Vector3(24,.5f,24);
                Place("Landmark_3D_Lowrider",city.lowriderPosition);
                Place("Landmark_3D_VendingMachine",city.vendingPosition);
                Place("Landmark_3D_TuningBench",city.tuningBenchPosition);
                Place("Landmark_3D_AirCompressor",city.airCompressorPosition);
                Place("Landmark_3D_HeistCrate",city.heistCratePosition);
            }
            Fit(player.transform.Find("CapKing_3D_Mesh"),1.8f,player.transform.position,true,false,2);
            Fit(player.transform.Find("Equipped_Jetpack_Harness"),.65f,player.transform.TransformPoint(new Vector3(0,1.1f,-.25f)),false,false,1);
            Fit(player.transform.Find("Equipped_Grapple_Launcher"),.4f,player.transform.TransformPoint(new Vector3(.35f,.9f,.35f)),false,false,.8f);
            foreach(var actor in Object.FindObjectsByType<ActorController>(FindObjectsSortMode.None))
                Fit(actor.transform.Find("InkDrip_3D_Mesh"),1.8f,actor.transform.position,true,false,2);
            var buildings = GameObject.Find("=== 3D MODULAR BUILDINGS ===");
            if (buildings != null)
                foreach(Transform building in buildings.transform)
                {
                    var signs = new List<Transform>();
                    foreach(Transform child in building) if(child.name == "Rooftop_NeonSign") signs.Add(child);
                    foreach(var sign in signs) sign.SetParent(null,true);
                    Fit(building,10, new Vector3(building.position.x,0,building.position.z),true,true,7);
                    foreach(var sign in signs) { Fit(sign,1.2f,building.position+Vector3.up*10,true,false,3); sign.SetParent(building,true); }
                }
            var landmarks = GameObject.Find("=== 3D LANDMARKS ===");
            if(landmarks != null)
                foreach(Transform landmark in landmarks.transform)
                {
                    if(!landmark.name.StartsWith("Landmark_3D_")) continue;
                    float height = landmark.name.Contains("Lowrider") ? 1.3f : landmark.name.Contains("Vending") ? 2.0f : landmark.name.Contains("Drone") ? .8f : 1.5f;
                    Vector3 anchor=landmark.position;
                    if(!landmark.name.Contains("Drone")) anchor.y=0;
                    Fit(landmark,height,anchor,true,landmark.GetComponent<Collider>() != null,4.5f);
                }
            foreach(var wall in Object.FindObjectsByType<WallTag>(FindObjectsSortMode.None))
            {
                Transform overlay=wall.graffitiRenderer != null ? wall.graffitiRenderer.transform : null;
                if(overlay!=null) overlay.SetParent(null,true);
                Fit(wall.transform,2.2f,new Vector3(wall.transform.position.x,0,wall.transform.position.z),true,true,3.4f);
                if(overlay!=null)
                {
                    var bounds=wall.GetComponent<BoxCollider>().bounds;
                    overlay.position=new Vector3(bounds.center.x,1.1f,bounds.max.z+.02f);
                    overlay.localScale=new Vector3(2.8f,1.8f,1);
                    overlay.SetParent(wall.transform,true);
                }
            }
            var parks=GameObject.Find("=== 3D URBAN PARK PLOTS ===");
            if(parks!=null) foreach(Transform park in parks.transform)
                Fit(park,.08f,new Vector3(park.position.x,.01f,park.position.z),true,false,5);
            player.transform.position = new Vector3(30, .1f, 30);
            Physics.SyncTransforms();
        }
        static void Place(string name,Vector3 position)
        {
            var item=GameObject.Find(name); if(item!=null) item.transform.position=position;
        }
        static void Fit(Transform root,float height,Vector3 anchor,bool grounded,bool collision,float width)
        {
            if(root==null) return;
            var renderers=root.GetComponentsInChildren<Renderer>();
            if(renderers.Length==0) return;
            Bounds bounds=renderers[0].bounds;
            foreach(var renderer in renderers) bounds.Encapsulate(renderer.bounds);
            if(bounds.size.y<.001f) return;
            float factor=Mathf.Min(height/bounds.size.y,width/Mathf.Max(.001f,Mathf.Max(bounds.size.x,bounds.size.z)));
            root.localScale*=factor;
            bounds=renderers[0].bounds;
            foreach(var renderer in renderers) bounds.Encapsulate(renderer.bounds);
            root.position+=anchor-new Vector3(bounds.center.x,grounded?bounds.min.y:bounds.center.y,bounds.center.z);
            if(collision)
            {
                bounds=renderers[0].bounds;
                foreach(var renderer in renderers) bounds.Encapsulate(renderer.bounds);
                foreach(var collider in root.GetComponentsInChildren<Collider>()) collider.enabled=false;
                var box=root.GetComponent<BoxCollider>(); if(box==null) box=root.gameObject.AddComponent<BoxCollider>();
                box.enabled=true; box.isTrigger=false; box.center=root.InverseTransformPoint(bounds.center);
                Bounds localBounds=new Bounds(root.InverseTransformPoint(bounds.min),Vector3.zero);
                for(int x=0;x<2;x++) for(int y=0;y<2;y++) for(int z=0;z<2;z++)
                    localBounds.Encapsulate(root.InverseTransformPoint(new Vector3(x==0?bounds.min.x:bounds.max.x,y==0?bounds.min.y:bounds.max.y,z==0?bounds.min.z:bounds.max.z)));
                box.center=localBounds.center; box.size=localBounds.size;
            }
        }
    }
}
