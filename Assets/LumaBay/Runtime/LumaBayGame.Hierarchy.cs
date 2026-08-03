using UnityEngine;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private Transform FindInRoot(string objectName)
        {
            if (screenRoot == null || string.IsNullOrEmpty(objectName)) return null;

            Transform[] all = screenRoot.GetComponentsInChildren<Transform>(true);
            foreach (Transform item in all)
            {
                if (item != null && item.name == objectName)
                    return item;
            }

            return null;
        }
    }
}
