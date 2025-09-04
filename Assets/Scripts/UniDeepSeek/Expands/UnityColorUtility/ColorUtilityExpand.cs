using UnityEngine;

namespace Xiyu.UniDeepSeek.UnityColorUtility
{
    public static class ColorUtilityExpand
    {
        public static string ToHexStringColor(this Color color, bool withNumberSign = true)
        {
            return withNumberSign ? '#' + ColorUtility.ToHtmlStringRGB(color) : ColorUtility.ToHtmlStringRGB(color);
        }
        public static string ToHexStringColorA(this Color color, bool withNumberSign = true)
        {
            return withNumberSign ? '#' + ColorUtility.ToHtmlStringRGBA(color) : ColorUtility.ToHtmlStringRGBA(color);
        }

        public static string OpenColorPicker(this Color color, bool withNumberSign = true)
        {
            return "<color=" + ToHexStringColor(color, withNumberSign) + ">";
        }

        public static string CloseColorPicker()
        {
            return "</color>";
        }
    }
}