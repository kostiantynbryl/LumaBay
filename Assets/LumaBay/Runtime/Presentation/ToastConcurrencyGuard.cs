using UnityEngine;

namespace LumaBay
{
    /// <summary>
    /// ToastRoutine historically searches for an object named exactly "Toast" and
    /// destroys it before showing the next notification. Cascades can request two
    /// notifications close together, leaving the older coroutine with a destroyed
    /// CanvasGroup. Rename the live object after creation so each coroutine owns its
    /// own toast until it completes naturally.
    /// </summary>
    public sealed class ToastConcurrencyGuard : MonoBehaviour
    {
        private Canvas canvas;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindFirstObjectByType<ToastConcurrencyGuard>() != null) return;
            GameObject host = new GameObject("ToastConcurrencyGuard");
            DontDestroyOnLoad(host);
            host.AddComponent<ToastConcurrencyGuard>();
        }

        private void LateUpdate()
        {
            if (canvas == null) canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null) return;

            Transform toast = canvas.transform.Find("Toast");
            if (toast == null) return;
            toast.name = "ToastActive_" + toast.GetInstanceID();
        }
    }
}
