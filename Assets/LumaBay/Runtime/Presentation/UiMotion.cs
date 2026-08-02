using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed class UiEntranceMotion : MonoBehaviour
    {
        [SerializeField] private float duration = 0.28f;
        [SerializeField] private float delay;
        [SerializeField] private Vector2 offset = new Vector2(0f, -26f);
        [SerializeField] private float startScale = 0.94f;

        private RectTransform rect;
        private CanvasGroup group;
        private Vector2 targetPosition;

        public void Configure(float animationDelay, Vector2 startOffset, float scale = 0.94f)
        {
            delay = animationDelay;
            offset = startOffset;
            startScale = scale;
        }

        private void Awake()
        {
            rect = transform as RectTransform;
            group = GetComponent<CanvasGroup>();
            if (group == null) group = gameObject.AddComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            if (rect == null) rect = transform as RectTransform;
            if (group == null) group = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
            StopAllCoroutines();
            StartCoroutine(Play());
        }

        private IEnumerator Play()
        {
            yield return null;
            targetPosition = rect.anchoredPosition;
            Vector3 targetScale = Vector3.one;
            rect.anchoredPosition = targetPosition + offset;
            rect.localScale = Vector3.one * startScale;
            group.alpha = 0f;

            if (delay > 0f) yield return new WaitForSecondsRealtime(delay);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                rect.anchoredPosition = Vector2.LerpUnclamped(targetPosition + offset, targetPosition, eased);
                rect.localScale = Vector3.LerpUnclamped(Vector3.one * startScale, targetScale, eased);
                group.alpha = eased;
                yield return null;
            }

            rect.anchoredPosition = targetPosition;
            rect.localScale = targetScale;
            group.alpha = 1f;
        }
    }

    public sealed class UiPressFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private RectTransform rect;
        private Vector3 targetScale = Vector3.one;
        private Coroutine routine;

        private void Awake()
        {
            rect = transform as RectTransform;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            AnimateTo(Vector3.one * 0.955f, 0.07f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            AnimateTo(Vector3.one, 0.10f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            AnimateTo(Vector3.one, 0.10f);
        }

        private void AnimateTo(Vector3 scale, float duration)
        {
            targetScale = scale;
            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(ScaleRoutine(duration));
        }

        private IEnumerator ScaleRoutine(float duration)
        {
            Vector3 start = rect.localScale;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                rect.localScale = Vector3.LerpUnclamped(start, targetScale, eased);
                yield return null;
            }
            rect.localScale = targetScale;
            routine = null;
        }
    }

    public sealed class SoftGlowPulse : MonoBehaviour
    {
        [SerializeField] private float speed = 1.8f;
        [SerializeField] private float minAlpha = 0.28f;
        [SerializeField] private float maxAlpha = 0.62f;

        private Graphic graphic;
        private Color baseColor;

        private void Awake()
        {
            graphic = GetComponent<Graphic>();
            if (graphic != null) baseColor = graphic.color;
        }

        private void Update()
        {
            if (graphic == null) return;
            Color color = baseColor;
            color.a = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.unscaledTime * speed) + 1f) * 0.5f);
            graphic.color = color;
        }
    }
}
