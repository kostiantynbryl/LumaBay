using UnityEngine;

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

            LumaBayMainMenuReleaseV10.MaintainGeometry(screenRoot);
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
