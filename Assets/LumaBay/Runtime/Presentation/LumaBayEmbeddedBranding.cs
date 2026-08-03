using UnityEngine;

namespace LumaBay
{
    internal static class LumaBayEmbeddedBranding
    {
        private static Sprite logo;

        public static Sprite Logo
        {
            get
            {
                if (logo == null)
                    logo = Resources.Load<Sprite>("Branding/LumaBayLogo");
                return logo;
            }
        }
    }
}
