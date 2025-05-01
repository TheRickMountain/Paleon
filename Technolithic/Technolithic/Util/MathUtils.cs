using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public static class MathUtils
    {

        public static float Distance(float x1, float y1, float x2, float y2)
        {
            float dX = x1 - x2;
            float dY = y1 - y2;
            return (float)Math.Sqrt((dX * dX) + (dY * dY));
        }

        public static float Lerp(float p1, float p2, float alpha)
        {
            return p1 + alpha * (p2 - p1);
        }

        public static float ToRadians(float deg)
        {
            return (float)(deg * (Math.PI / 180.0f));
        }

        public static float ToDegrees(float rad)
        {
            return (float)(rad * (180.0f / Math.PI));
        }

        public static void Replace(ref int x, ref int y)
        {
            int tmp = x;
            x = y;
            y = tmp;
        }

        public static bool InRange(float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        public static Vector2 Floor(this Vector2 val)
        {
            return new Vector2((int)Math.Floor(val.X), (int)Math.Floor(val.Y));
        }

    }
}
