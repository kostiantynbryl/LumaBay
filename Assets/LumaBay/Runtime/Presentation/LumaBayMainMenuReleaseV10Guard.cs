using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    [DefaultExecutionOrder(10000)]
    [DisallowMultipleComponent]
    public sealed class LumaBayMainMenuReleaseV10Guard : MonoBehaviour
    {
        private Canvas canvas;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            LumaBayGame game = FindFirstObjectByType<LumaBayGame>();
            if (game != null && game.GetComponent<LumaBayMainMenuReleaseV10Guard>() == null)
                game.gameObject.AddComponent<LumaBayMainMenuReleaseV10Guard>();
        }

        private void LateUpdate()
        {
            if (canvas == null) canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null) return;

            Transform screenRoot = Find(canvas.transform, "ScreenRoot");
            Transform hero = screenRoot != null ? Find(screenRoot, "MainHero") : null;
            Transform marker = screenRoot != null ? Find(screenRoot, "ReleaseMenuV10Marker") : null;
            if (screenRoot == null || hero == null || marker == null) return;

            RectTransform vertical = Find(screenRoot, "VerticalScreen") as RectTransform;
            RectTransform logo = Find(screenRoot, "LogoBlock") as RectTransform;
            RectTransform row = Find(screenRoot, "MainSecondaryRow") as RectTransform;
            RectTransform lighthouse = Find(hero, "PremiumLighthouseVisual") as RectTransform;
            RectTransform caption = Find(hero, "HeroCaption") as RectTransform;

            SetHeight(logo, 230f);
            SetHeight(hero as RectTransform, 570f);
            SetHeight(row, 150f);

            if (vertical != null)
            {
                VerticalLayoutGroup layout = vertical.GetComponent<VerticalLayoutGroup>();
                if (layout != null)
                {
                    layout.padding = new RectOffset(18, 18, 8, 12);
                    layout.spacing = 7f;
                }

                Button play = null;
                for (int i = 0; i < vertical.childCount; i++)
                {
                    Button candidate = vertical.GetChild(i).GetComponent<Button>();
                    if (candidate != null)
                    {
                        play = candidate;
                        break;
                    }
                }
                if (play != null) SetHeight(play.transform as RectTransform, 108f);
            }

            if (lighthouse != null)
            {
                lighthouse.anchorMin = new Vector2(0.015f, 0.075f);
                lighthouse.anchorMax = new Vector2(0.985f, 0.985f);
                lighthouse.offsetMin = Vector2.zero;
                lighthouse.offsetMax = Vector2.zero;
                Image image = lighthouse.GetComponent<Image>();
                if (image != null) image.preserveAspect = true;
            }

            if (caption != null)
            {
                caption.anchorMin = new Vector2(0.07f, 0.025f);
                caption.anchorMax = new Vector2(0.93f, 0.145f);
                caption.offsetMin = Vector2.zero;
                caption.offsetMax = Vector2.zero;
            }

            if (row != null)
            {
                HorizontalLayoutGroup horizontal = row.GetComponent<HorizontalLayoutGroup>();
                if (horizontal != null)
                {
                    horizontal.padding = new RectOffset(2, 2, 2, 2);
                    horizontal.spacing = 8f;
                    horizontal.childForceExpandWidth = true;
                    horizontal.childForceExpandHeight = true;
                }

                for (int i = 0; i < row.childCount; i++)
                {
                    RectTransform tile = row.GetChild(i) as RectTransform;
                    SetHeight(tile, 142f);
                    Image image = tile != null ? tile.GetComponent<Image>() : null;
                    if (image != null)
                    {
                        Sprite sprite = LumaBayArtPackV2.MediumPanel ?? LumaBayArtPackV2.SecondaryButton;
                        if (sprite != null) image.sprite = sprite;
                        image.type = Image.Type.Sliced;
                        image.color = Color.white;
                    }
                }
            }

            if (vertical != null) LayoutRebuilder.ForceRebuildLayoutImmediate(vertical);
        }

        private static void SetHeight(RectTransform rect, float height)
        {
            if (rect == null) return;
            LayoutElement element = rect.GetComponent<LayoutElement>();
            if (element == null) element = rect.gameObject.AddComponent<LayoutElement>();
            element.minHeight = height;
            element.preferredHeight = height;
            element.flexibleHeight = 0f;
        }

        private static Transform Find(Transform root, string name)
        {
            if (root == null) return null;
            if (root.name == name) return root;
            for (int i = 0; i < root.childCount; i++)
            {
                Transform result = Find(root.GetChild(i), name);
                if (result != null) return result;
            }
            return null;
        }
    }
}
