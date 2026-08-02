using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private RectTransform fixedLevelContent;

        private void LateUpdate()
        {
            RepairLevelSelectLayout();
            EnsureBoardClipping();
        }

        private void RepairLevelSelectLayout()
        {
            if (screenRoot == null) return;

            Transform contentTransform = screenRoot.Find("LevelScroll/Viewport/Content");
            if (contentTransform == null)
            {
                fixedLevelContent = null;
                return;
            }

            RectTransform content = contentTransform as RectTransform;
            if (content == null || fixedLevelContent == content) return;

            ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>();
            if (fitter != null) fitter.enabled = false;

            const int columns = 4;
            const float cellHeight = 126f;
            const float spacing = 12f;
            const float verticalPadding = 32f;
            int rows = Mathf.CeilToInt(LevelCatalog.Count / (float)columns);
            float contentHeight = verticalPadding + rows * cellHeight + Mathf.Max(0, rows - 1) * spacing;

            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = new Vector2(0f, contentHeight);

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);

            ScrollRect scroll = content.GetComponentInParent<ScrollRect>();
            if (scroll != null)
            {
                scroll.verticalNormalizedPosition = 1f;
                scroll.StopMovement();
            }

            fixedLevelContent = content;
        }

        private void EnsureBoardClipping()
        {
            if (boardGrid == null) return;
            if (boardGrid.GetComponent<RectMask2D>() == null)
            {
                boardGrid.gameObject.AddComponent<RectMask2D>();
            }
        }
    }
}
