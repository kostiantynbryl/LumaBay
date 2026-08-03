using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public static class LumaBayPanelToneGuard
    {
        private static readonly HashSet<int> Processed = new HashSet<int>();
        private static int lastFrame = -20;

        private static readonly HashSet<string> StructuralPanels = new HashSet<string>(StringComparer.Ordinal)
        {
            "MainHero",
            "MapHeader",
            "LighthouseMetaCard",
            "TaskCard",
            "SettingsHeader",
            "SettingsCard",
            "LevelsHeader",
            "LevelScroll",
            "GameHeader",
            "Goals",
            "BoosterTray",
            "ModalPanel"
        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Register()
        {
            Processed.Clear();
            lastFrame = -20;
            Canvas.willRenderCanvases -= Apply;
            Canvas.willRenderCanvases += Apply;
        }

        private static void Apply()
        {
            if (Time.frameCount - lastFrame < 8) return;
            lastFrame = Time.frameCount;

            Image[] images = UnityEngine.Object.FindObjectsByType<Image>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (Image image in images)
            {
                if (image == null || !StructuralPanels.Contains(image.name) || !Processed.Add(image.GetInstanceID())) continue;

                Sprite panel = LumaBayArtPack.Panel;
                if (panel != null)
                {
                    image.sprite = panel;
                    image.type = Image.Type.Sliced;
                }

                image.color = image.name == "ModalPanel"
                    ? new Color(0.12f, 0.27f, 0.38f, 0.99f)
                    : new Color(0.10f, 0.25f, 0.36f, 0.96f);

                Shadow shadow = image.GetComponent<Shadow>();
                if (shadow != null)
                {
                    shadow.effectColor = new Color(0f, 0f, 0f, 0.22f);
                    shadow.effectDistance = new Vector2(1.5f, -2.5f);
                }
            }
        }
    }
}
