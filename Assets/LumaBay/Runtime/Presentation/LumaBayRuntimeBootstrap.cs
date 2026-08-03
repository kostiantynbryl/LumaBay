using UnityEngine;

namespace LumaBay
{
    public static class LumaBayAudioListenerGuard
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureSingleAudioListener()
        {
            AudioListener[] listeners = Object.FindObjectsByType<AudioListener>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            AudioListener listenerToKeep = null;
            foreach (AudioListener listener in listeners)
            {
                if (listener == null) continue;

                Camera camera = listener.GetComponent<Camera>();
                if (camera != null && camera.CompareTag("MainCamera"))
                {
                    listenerToKeep = listener;
                    break;
                }

                if (listenerToKeep == null)
                    listenerToKeep = listener;
            }

            foreach (AudioListener listener in listeners)
            {
                if (listener == null) continue;
                listener.enabled = listener == listenerToKeep;
            }
        }
    }
}
