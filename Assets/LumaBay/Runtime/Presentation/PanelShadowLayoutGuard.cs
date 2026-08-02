using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public static class PanelShadowLayoutGuard
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Register()
        {
            Canvas.willRenderCanvases -= Apply;
            Canvas.willRenderCanvases += Apply;
        }

        private static void Apply()
        {
            RectTransform[] rects = Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (RectTransform rect in rects)
            {
                if (rect.name != "PanelShadow") continue;
                LayoutElement element = rect.GetComponent<LayoutElement>();
                if (element == null) element = rect.gameObject.AddComponent<LayoutElement>();
                element.ignoreLayout = true;
            }
        }
    }
}
