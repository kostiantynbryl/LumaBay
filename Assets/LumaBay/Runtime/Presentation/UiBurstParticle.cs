using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed class UiBurstParticle : MonoBehaviour
    {
        private RectTransform rect;
        private Image image;

        public void Launch(Vector2 velocity, float duration, float rotationSpeed)
        {
            rect = transform as RectTransform;
            image = GetComponent<Image>();
            StartCoroutine(Animate(velocity, duration, rotationSpeed));
        }

        private IEnumerator Animate(Vector2 velocity, float duration, float rotationSpeed)
        {
            Vector2 start = rect.anchoredPosition;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = 1f - Mathf.Pow(1f - t, 2f);
                rect.anchoredPosition = start + velocity * (eased * duration);
                rect.localRotation = Quaternion.Euler(0f, 0f, rotationSpeed * elapsed);
                rect.localScale = Vector3.one * Mathf.Lerp(0.55f, 1.35f, Mathf.Sin(t * Mathf.PI));
                Color color = image.color;
                color.a = Mathf.Pow(1f - t, 1.4f) * 0.92f;
                image.color = color;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
