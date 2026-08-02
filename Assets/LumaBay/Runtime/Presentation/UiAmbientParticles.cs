using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed class UiAmbientParticles : MonoBehaviour
    {
        private sealed class Particle
        {
            public RectTransform Rect;
            public Image Image;
            public Vector2 Velocity;
            public float Life;
            public float MaxLife;
            public float Phase;
        }

        private readonly List<Particle> particles = new List<Particle>();
        private RectTransform root;
        private float spawnTimer;
        private System.Random random;

        private void Awake()
        {
            root = transform as RectTransform;
            random = new System.Random(71031);
        }

        private void Update()
        {
            spawnTimer -= Time.unscaledDeltaTime;
            if (spawnTimer <= 0f && particles.Count < 18)
            {
                spawnTimer = 0.22f + (float)random.NextDouble() * 0.42f;
                Spawn();
            }

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                Particle particle = particles[i];
                particle.Life -= Time.unscaledDeltaTime;
                if (particle.Life <= 0f || particle.Rect == null)
                {
                    if (particle.Rect != null) Destroy(particle.Rect.gameObject);
                    particles.RemoveAt(i);
                    continue;
                }

                particle.Rect.anchoredPosition += particle.Velocity * Time.unscaledDeltaTime;
                float normalized = 1f - particle.Life / particle.MaxLife;
                float alpha = Mathf.Sin(normalized * Mathf.PI) * 0.42f;
                alpha *= 0.72f + Mathf.Sin(Time.unscaledTime * 3f + particle.Phase) * 0.28f;
                Color color = particle.Image.color;
                color.a = Mathf.Max(0f, alpha);
                particle.Image.color = color;
                float scale = 0.72f + Mathf.Sin(normalized * Mathf.PI) * 0.55f;
                particle.Rect.localScale = Vector3.one * scale;
            }
        }

        private void Spawn()
        {
            GameObject item = new GameObject("AmbientSparkle", typeof(RectTransform), typeof(Image));
            item.transform.SetParent(root, false);
            RectTransform rect = item.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);
            float width = Mathf.Max(720f, root.rect.width);
            float height = Mathf.Max(1280f, root.rect.height);
            rect.anchoredPosition = new Vector2((float)random.NextDouble() * width, (float)random.NextDouble() * height);
            float size = 5f + (float)random.NextDouble() * 13f;
            rect.sizeDelta = new Vector2(size, size);

            Image image = item.GetComponent<Image>();
            image.sprite = ProceduralArt.Pearl("ambient_pearl");
            image.raycastTarget = false;
            image.color = random.NextDouble() > 0.35
                ? new Color(1f, 0.83f, 0.38f, 0f)
                : new Color(0.45f, 0.85f, 1f, 0f);

            float life = 2.4f + (float)random.NextDouble() * 2.6f;
            particles.Add(new Particle
            {
                Rect = rect,
                Image = image,
                Velocity = new Vector2(-5f + (float)random.NextDouble() * 10f, 8f + (float)random.NextDouble() * 18f),
                Life = life,
                MaxLife = life,
                Phase = (float)random.NextDouble() * 8f
            });
        }
    }
}
