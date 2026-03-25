using UnityEngine;

namespace SugyeongKim.Util
{
    public class ResourceManager : GlobalSingleton<ResourceManager>
    {
        private System.Collections.Generic.Dictionary<string, GameObject> prefabCache = new System.Collections.Generic.Dictionary<string, GameObject>();

        public GameObject LoadPrefab(string path)
        {
            if (prefabCache.TryGetValue(path, out var prefab))
            {
                return prefab;
            }

            prefab = Resources.Load<GameObject>(path);
            if (prefab != null)
            {
                prefabCache[path] = prefab;
            }
            else
            {
                DEBUG.Error($"[ResourceManager] 리소스를 찾을 수 없습니다: {path}");
            }

            return prefab;
        }

        public GameObject InstantiatePrefab(string path, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            var prefab = LoadPrefab(path);
            if (prefab != null)
            {
                return Object.Instantiate(prefab, position, rotation, parent);
            }
            return null;
        }
    }
}