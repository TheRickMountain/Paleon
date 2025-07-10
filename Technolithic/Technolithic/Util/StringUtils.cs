using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using System;

namespace Technolithic
{
    public static class StringUtils
    {
        public static int GamePercentToReal(float gamePercent)
        {
            return (int)Math.Round(gamePercent * 100f);
        }

        public static string Paint(this string text, Color color)
        {
            return $"/c[{color.ToHexString()}]{text}/cd";
        }

        public static string Strikethrough(this string text)
        {
            return $"/ts{text}/td";
        }
    }
}
