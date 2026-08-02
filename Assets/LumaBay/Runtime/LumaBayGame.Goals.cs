using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private int configuredGoalsPanelId = int.MinValue;

        private void RefreshGoalPresentationIfNeeded()
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
                layout.childForceExpandHeight = true;
                layout.spacing = 7f;
            }

            Image targetIcon = CreateImage(goals, "TargetGoalIcon",
                LumaBayArtPack.Piece(currentLevel.TargetPiece) ?? ProceduralArt.Piece(currentLevel.TargetPiece), Color.white);
            targetIcon.raycastTarget = false;
            SetLayout(targetIcon.rectTransform, 62f, 62f);
            targetIcon.transform.SetSiblingIndex(0);

            if (collectGoalLabel != null)
            {
                SetLayout(collectGoalLabel.rectTransform, -1f, 1f);
                collectGoalLabel.transform.SetSiblingIndex(1);
            }

            if (currentLevel.FogCount <= 0 || fogGoalLabel == null) return;

            RectTransform fogMedallion = CreateRect(goals, "FogGoalIcon");
            Image fogBackground = fogMedallion.gameObject.AddComponent<Image>();
            fogBackground.sprite = LumaBayArtPack.GoalChip ??
                                   ProceduralArt.Rounded("fog_goal", new Color(0.19f, 0.50f, 0.65f, 0.72f), 15);
            fogBackground.type = Image.Type.Sliced;
            fogBackground.color = new Color(0.74f, 0.94f, 1f, 0.88f);
            SetLayout(fogMedallion, 54f, 54f);

            Text fogIcon = CreateText(fogMedallion, "≈", 31, TextAnchor.MiddleCenter,
                new Color(0.08f, 0.26f, 0.38f), FontStyle.Bold);
            Stretch(fogIcon.rectTransform, 3f);
            fogIcon.raycastTarget = false;
            fogMedallion.SetSiblingIndex(2);

            SetLayout(fogGoalLabel.rectTransform, -1f, 1f);
            fogGoalLabel.transform.SetSiblingIndex(3);
        }
    }
}
