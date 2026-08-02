using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public static class PanelShadowLayoutGuard
    {
        private static readonly HashSet<int> Processed = new HashSet<int>();
        private static int lastFrame = -1;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Register()
        {
            Processed.Clear();
            Canvas.willRenderCanvases -= Apply;
            Canvas.willRenderCanvases += Apply;
        }

        private static void Apply()
        {
            if (lastFrame == Time.frameCount) return;
            lastFrame = Time.frameCount;

            RectTransform[] rects = Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (RectTransform rect in rects)
            {
                if (rect.name != "PanelShadow" || !Processed.Add(rect.GetInstanceID())) continue;

                LayoutElement element = rect.GetComponent<LayoutElement>();
                if (element == null) element = rect.gameObject.AddComponent<LayoutElement>();
                element.ignoreLayout = true;

                Image parentImage = rect.parent != null ? rect.parent.GetComponent<Image>() : null;
                if (parentImage != null)
                {
                    Shadow shadow = parentImage.GetComponent<Shadow>();
                    if (shadow == null) shadow = parentImage.gameObject.AddComponent<Shadow>();
                    shadow.effectColor = new Color(0f, 0f, 0f, 0.48f);
                    shadow.effectDistance = new Vector2(4f, -7f);
                    shadow.useGraphicAlpha = true;
                }

                rect.gameObject.SetActive(false);
            }
        }
    }
}
