using UnityEngine;

namespace LumaBay
{
    public static class LumaBayRuntimeBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureGameExists()
        {
#if UNITY_2023_1_OR_NEWER
            LumaBayGame existing = Object.FindFirstObjectByType<LumaBayGame>(FindObjectsInactive.Include);
#else
            LumaBayGame existing = Object.FindObjectOfType<LumaBayGame>(true);
#endif
            if (existing != null) return;

            GameObject root = new GameObject("LumaBayRuntime");
            Object.DontDestroyOnLoad(root);
            root.AddComponent<LumaBayGame>();
        }
    }
}
