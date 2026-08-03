using System.Collections.Generic;
using UnityEngine;

namespace LumaBay
{
    public static class LumaBayArtPack
    {
        private static readonly Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>();
        private static readonly Dictionary<string, AudioClip> AudioCache = new Dictionary<string, AudioClip>();
        private static readonly Dictionary<string, Sprite> TintedFrameCache = new Dictionary<string, Sprite>();

        public static bool IsAvailable => LumaBayArtPackV2.IsAvailable || LoadSprite("ui/panel_glass") != null;

        public static Sprite Background => First(LumaBayArtPackV2.GameplayBackground, LoadSprite("backgrounds/coastal_sunset"));
        public static Sprite MainMenuBackground => First(LumaBayArtPackV2.MainMenuBackground, Background);
        public static Sprite MapBackground => First(LumaBayArtPackV2.MapBackground, Background);
        public static Sprite StoryBackground => First(LumaBayArtPackV2.StoryBackground, Background);
        public static Sprite Panel => First(LumaBayArtPackV2.PanelLarge, LoadSprite("ui/panel_glass"));
        public static Sprite ButtonPrimary => First(LumaBayArtPackV2.PrimaryButton, LoadSprite("ui/button_primary"));
        public static Sprite ButtonSecondary => First(LumaBayArtPackV2.SecondaryButton, LoadSprite("ui/button_secondary"));
        public static Sprite BoosterCard => First(LumaBayArtPackV2.BoosterTray, LoadSprite("ui/booster_card"));
        public static Sprite GoalChip => First(LumaBayArtPackV2.GoalsPanel, LoadSprite("ui/goal_chip"));
        public static Sprite ProgressTrack => First(LumaBayArtPackV2.ProgressTrack, LoadSprite("ui/progress_track"));
        public static Sprite ProgressFill => First(LumaBayArtPackV2.ProgressFill, LoadSprite("ui/progress_fill"));

        public static Sprite Piece(PieceKind kind)
        {
            return First(LumaBayArtPackV2.Piece(kind), LoadSprite($"pieces/piece_{kind.ToString().ToLowerInvariant()}"));
        }

        public static Sprite Booster(string id)
        {
            return First(LumaBayArtPackV2.Booster(id), LoadSprite($"boosters/{id}"));
        }

        public static Sprite LighthouseState(int state)
        {
            return First(LumaBayArtPackV2.LighthouseState(state), LoadSprite($"lighthouse/lighthouse_{Mathf.Clamp(state, 0, 31):00}"));
        }

        public static AudioClip Audio(string id)
        {
            if (AudioCache.TryGetValue(id, out AudioClip cached)) return cached;
            AudioClip clip = Resources.Load<AudioClip>($"Audio/{id}");
            AudioCache[id] = clip;
            return clip;
        }

        public static Sprite TintedPanel(Color tint, bool elevated)
        {
            Sprite source = Panel;
            if (source == null || source.texture == null || !source.texture.isReadable) return source;

            string key = $"{ColorUtility.ToHtmlStringRGBA(tint)}_{elevated}";
            if (TintedFrameCache.TryGetValue(key, out Sprite cached)) return cached;

            Texture2D original = source.texture;
            Color[] pixels = original.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                Color p = pixels[i];
                float goldSignal = Mathf.Clamp01((p.r - p.b) * 2.4f + (p.g - p.b) * 1.2f);
                Color glass = new Color(
                    tint.r * Mathf.Lerp(0.62f, 1.08f, p.r),
                    tint.g * Mathf.Lerp(0.62f, 1.08f, p.g),
                    tint.b * Mathf.Lerp(0.62f, 1.08f, p.b),
                    p.a * Mathf.Clamp01(tint.a + 0.22f));
                Color gold = Color.Lerp(new Color(0.56f, 0.34f, 0.10f, p.a), new Color(1f, 0.84f, 0.42f, p.a), p.r);
                pixels[i] = Color.Lerp(glass, gold, goldSignal);
            }

            var texture = new Texture2D(original.width, original.height, TextureFormat.RGBA32, false)
            {
                name = $"panel_{key}",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            texture.SetPixels(pixels);
            texture.Apply();
            Vector4 border = source.border;
            Sprite result = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), source.pixelsPerUnit, 0, SpriteMeshType.FullRect, border);
            result.name = texture.name;
            TintedFrameCache[key] = result;
            return result;
        }

        public static void ClearRuntimeCache()
        {
            SpriteCache.Clear();
            AudioCache.Clear();
            LumaBayArtPackV2.Clear();
            foreach (Sprite sprite in TintedFrameCache.Values)
            {
                if (sprite == null) continue;
                Texture2D texture = sprite.texture;
                Object.Destroy(sprite);
                if (texture != null) Object.Destroy(texture);
            }
            TintedFrameCache.Clear();
        }

        private static Sprite First(Sprite preferred, Sprite fallback)
        {
            return preferred != null ? preferred : fallback;
        }

        private static Sprite LoadSprite(string relativePath)
        {
            if (SpriteCache.TryGetValue(relativePath, out Sprite cached)) return cached;
            Sprite sprite = Resources.Load<Sprite>($"ArtPack/{relativePath}");
            SpriteCache[relativePath] = sprite;
            return sprite;
        }
    }
}
