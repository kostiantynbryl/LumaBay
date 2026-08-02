using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed class PieceDropMotion : MonoBehaviour
    {
        private RectTransform rect;

        private void Awake()
        {
            rect = transform as RectTransform;
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(Play());
        }

        private IEnumerator Play()
        {
            yield return null;
            Vector2 targetPosition = rect.anchoredPosition;
            Vector3 targetScale = rect.localScale;
            rect.anchoredPosition = targetPosition + new Vector2(0f, 44f);
            rect.localScale = targetScale * 0.72f;

            float duration = 0.24f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float overshoot = 1f + 0.13f * Mathf.Sin(t * Mathf.PI);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                rect.anchoredPosition = Vector2.LerpUnclamped(targetPosition + new Vector2(0f, 44f), targetPosition, eased);
                rect.localScale = Vector3.LerpUnclamped(targetScale * 0.72f, targetScale * overshoot, eased);
                yield return null;
            }

            rect.anchoredPosition = targetPosition;
            rect.localScale = targetScale;
        }
    }

    public static class PieceDropMotionInstaller
    {
        private static int lastFrame = -1;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Register()
        {
            Canvas.willRenderCanvases -= Install;
            Canvas.willRenderCanvases += Install;
        }

        private static void Install()
        {
            if (lastFrame == Time.frameCount) return;
            lastFrame = Time.frameCount;

            Image[] images = Object.FindObjectsByType<Image>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (Image image in images)
            {
                if (image.name != "Piece") continue;
                if (image.GetComponent<PieceDropMotion>() != null) continue;
                image.gameObject.AddComponent<PieceDropMotion>();
            }
        }
    }
}
