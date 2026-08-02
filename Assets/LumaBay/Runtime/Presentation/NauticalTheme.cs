using UnityEngine;

namespace LumaBay
{
    public static class NauticalTheme
    {
        public static readonly Color Midnight = new Color(0.012f, 0.035f, 0.075f, 1f);
        public static readonly Color Navy = new Color(0.018f, 0.095f, 0.180f, 1f);
        public static readonly Color Ocean = new Color(0.030f, 0.235f, 0.330f, 1f);
        public static readonly Color OceanBright = new Color(0.050f, 0.430f, 0.540f, 1f);
        public static readonly Color GoldDark = new Color(0.43f, 0.245f, 0.065f, 1f);
        public static readonly Color Gold = new Color(0.95f, 0.680f, 0.205f, 1f);
        public static readonly Color GoldLight = new Color(1.00f, 0.885f, 0.530f, 1f);
        public static readonly Color Pearl = new Color(0.975f, 0.950f, 0.875f, 1f);
        public static readonly Color Coral = new Color(0.930f, 0.285f, 0.250f, 1f);
        public static readonly Color Emerald = new Color(0.275f, 0.690f, 0.300f, 1f);
        public static readonly Color Purple = new Color(0.485f, 0.225f, 0.700f, 1f);
        public static readonly Color Glass = new Color(0.020f, 0.120f, 0.220f, 0.94f);
        public static readonly Color GlassSoft = new Color(0.025f, 0.175f, 0.265f, 0.88f);
        public static readonly Color Shadow = new Color(0f, 0f, 0f, 0.58f);

        public static Color PanelColor(bool elevated = false)
        {
            return elevated ? GlassSoft : Glass;
        }

        public static Color PieceCell(int x, int y)
        {
            float shift = ((x + y) & 1) == 0 ? 0f : 0.022f;
            return new Color(0.025f + shift, 0.115f + shift, 0.205f + shift, 0.96f);
        }
    }
}
