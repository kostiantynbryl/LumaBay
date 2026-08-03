using UnityEngine;

namespace LumaBay
{
    public static class LumaBayRuntimeBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureRuntime()
        {
            RemoveDuplicateAudioListeners();

            LumaBayGame existing = Object.FindFirstObjectByType<LumaBayGame>();
            if (existing != null) return;

            GameObject runtime = new GameObject("LumaBayRuntime");
            Object.DontDestroyOnLoad(runtime);
            runtime.AddComponent<LumaBayGame>();
        }

        private static void RemoveDuplicateAudioListeners()
        {
            AudioListener[] listeners = Object.FindObjectsByType<AudioListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            bool keptOne = false;
            foreach (AudioListener listener in listeners)
            {
                if (listener == null) continue;
                if (!keptOne)
                {
                    listener.enabled = true;
                    keptOne = true;
                }
                else
                {
                    listener.enabled = false;
                }
            }
        }
    }
}
