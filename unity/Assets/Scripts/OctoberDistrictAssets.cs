using UnityEngine;

namespace Dopeboyz
{
    public sealed class OctoberDistrictAssets : MonoBehaviour
    {
        public static void Install()
        {
            if (FindFirstObjectByType<OctoberDistrictAssets>() != null) return;
            new GameObject("October District Assets").AddComponent<OctoberDistrictAssets>();
        }
        void Start()
        {
            var city = CityController.Instance;
            if (city == null) return;
            Spawn("CanalOctober/Bench", city.hqPosition + new Vector3(2,0,0), 1.0f, false);
            Spawn("CanalOctober/YardFence", city.hqPosition + new Vector3(-3,0,0), 2.0f, false);
            Spawn("CanalOctober/WallLight", city.hqPosition + new Vector3(0,2,2), .5f, false);
            var companion = Spawn("Characters/InmateThreePlayable", city.hqPosition + new Vector3(0,0,3), 1.8f, true);
            if (companion != null)
            {
                var animator = companion.GetComponentInChildren<Animator>();
                var controller = Resources.Load<RuntimeAnimatorController>("Characters/Resource03/Locomotion");
                if (animator != null && animator.avatar != null && animator.avatar.isValid && controller != null)
                {
                    animator.runtimeAnimatorController = controller;
                    animator.applyRootMotion = false; animator.Rebind(); animator.Update(0);
                    companion.AddComponent<OctoberCompanion>().animator = animator;
                }
                else Debug.LogWarning("October companion has no compatible avatar/controller. Original assets were not modified.");
            }
        }
        GameObject Spawn(string key, Vector3 position, float height, bool character)
        {
            var source = Resources.Load<GameObject>(key);
            if (source == null) { Debug.LogWarning("October asset unavailable: " + key); return null; }
            var go = Instantiate(source, position, Quaternion.identity, transform);
            go.name = "October " + source.name;
            foreach (var missing in go.GetComponentsInChildren<MonoBehaviour>(true))
                if (missing != null) missing.enabled = false;
            var renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) { Debug.LogError("October asset has no renderer: " + key); Destroy(go); return null; }
            Bounds bounds = renderers[0].bounds;
            foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds);
            if (bounds.size.y <= .001f) { Destroy(go); Debug.LogError("Invalid October asset bounds: " + key); return null; }
            go.transform.localScale *= height / bounds.size.y;
            bounds = renderers[0].bounds;
            foreach (var renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
                var mats = renderer.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    if (mats[i] == null) continue;
                    var original = mats[i]; var material = new Material(Shader.Find("Standard"));
                    if (original.HasProperty("_BaseMap")) material.mainTexture = original.GetTexture("_BaseMap");
                    else if (original.HasProperty("_MainTex")) material.mainTexture = original.GetTexture("_MainTex");
                    material.color = original.HasProperty("_BaseColor") ? original.GetColor("_BaseColor") : Color.white;
                    mats[i] = material;
                }
                renderer.materials = mats;
            }
            go.transform.position += Vector3.up * (position.y - bounds.min.y);
            foreach (var collider in go.GetComponentsInChildren<Collider>()) collider.enabled = false;
            if (!character)
            {
                var collider = go.AddComponent<BoxCollider>();
                collider.center = go.transform.InverseTransformPoint(bounds.center + Vector3.up * (position.y-bounds.min.y));
                collider.size = new Vector3(bounds.size.x / go.transform.lossyScale.x, bounds.size.y / go.transform.lossyScale.y, bounds.size.z / go.transform.lossyScale.z);
            }
            return go;
        }
    }

    public sealed class OctoberCompanion : MonoBehaviour
    {
        public Animator animator;
        void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.currentState != GameState.Playing) return;
            var player = PlayerController.Instance;
            if (player == null) return;
            Vector3 direction = player.transform.position - transform.position; direction.y = 0;
            float speed = direction.magnitude > 3.5f && direction.magnitude < 18f ? 2.5f : 0;
            if (speed > 0 && !Physics.Raycast(transform.position + Vector3.up, direction.normalized, 1.2f, ~0, QueryTriggerInteraction.Ignore))
            {
                transform.position += direction.normalized * speed * Time.deltaTime;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 6);
            }
            else speed = 0;
            foreach (var parameter in animator.parameters)
                if (parameter.name == "Speed" && parameter.type == AnimatorControllerParameterType.Float) animator.SetFloat("Speed", speed);
        }
    }
}
