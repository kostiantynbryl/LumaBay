using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private int configuredGoalsPanelId = int.MinValue;

        private void Update()
        {
            if (screenRoot == null || currentLevel == null || board == null)
            {
                configuredGoalsPanelId = int.MinValue;
                return;
            }

            Transform goalsTransform = screenRoot.Find("Goals");
            if (goalsTransform == null)
            {
                configuredGoalsPanelId = int.MinValue;
                return;
            }

            int instanceId = goalsTransform.GetInstanceID();
            if (configuredGoalsPanelId == instanceId) return;
            configuredGoalsPanelId = instanceId;
            ConfigureGoalIcons(goalsTransform as RectTransform);
        }

        private void ConfigureGoalIcons(RectTransform goals)
        {
            if (goals == null || goals.Find("TargetGoalIcon") != null) return;

            HorizontalLayoutGroup layout = goals.GetComponent<HorizontalLayoutGroup>();
            if (layout != null)
            {
                layout.childForceExpandWidth = false;
                layout.spacing = 8f;
            }

            Image targetIcon = CreateImage(goals, "TargetGoalIcon", ProceduralArt.Piece(currentLevel.TargetPiece), Color.white);
            targetIcon.raycastTarget = false;
            SetLayout(targetIcon.rectTransform, 64f, 64f);
            targetIcon.transform.SetSiblingIndex(1);

            if (collectGoalLabel != null)
            {
                SetLayout(collectGoalLabel.rectTransform, -1f, 1f);
            }

            if (currentLevel.FogCount > 0 && fogGoalLabel != null)
            {
                RectTransform fogMedallion = CreatePanel(goals, "FogGoalIcon", new Color(0.19f, 0.50f, 0.65f, 0.72f));
                SetLayout(fogMedallion, 58f, 58f);
                Text fogIcon = CreateText(fogMedallion, "≈", 34, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
                Stretch(fogIcon.rectTransform, 4f);
                fogIcon.raycastTarget = false;
                fogMedallion.SetSiblingIndex(Mathf.Max(1, fogGoalLabel.transform.GetSiblingIndex()));
                SetLayout(fogGoalLabel.rectTransform, -1f, 1f);
            }
        }
    }
}
