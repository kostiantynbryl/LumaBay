using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public static class BoardCellThemeInstaller
    {
        private static readonly HashSet<int> Processed = new HashSet<int>();
        private static int nextScanFrame;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Register()
        {
            Processed.Clear();
            nextScanFrame = 0;
            Canvas.willRenderCanvases -= Apply;
            Canvas.willRenderCanvases += Apply;
        }

        private static void Apply()
        {
            if (Time.frameCount < nextScanFrame) return;
            nextScanFrame = Time.frameCount + 10;

            RectTransform[] rects = Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (RectTransform rect in rects)
            {
                if (!rect.name.StartsWith("Cell_") || !Processed.Add(rect.GetInstanceID())) continue;
                Image image = rect.GetComponent<Image>();
                if (image == null) continue;

                int parity = ParseParity(rect.name);
                Color fill = parity == 0
                    ? new Color(0.055f, 0.16f, 0.27f, 0.96f)
                    : new Color(0.075f, 0.20f, 0.32f, 0.96f);
                image.sprite = ProceduralArt.Rounded($"clean_cell_{parity}", fill, 11);
                image.type = Image.Type.Sliced;
                image.color = Color.white;

                Shadow shadow = rect.GetComponent<Shadow>();
                if (shadow != null) shadow.enabled = false;
                Outline outline = rect.GetComponent<Outline>();
                if (outline != null) outline.enabled = false;
            }
        }

        private static int ParseParity(string name)
        {
            string[] parts = name.Split('_');
            if (parts.Length >= 3 && int.TryParse(parts[1], out int x) && int.TryParse(parts[2], out int y))
                return (x + y) & 1;
            return 0;
        }
    }
}
