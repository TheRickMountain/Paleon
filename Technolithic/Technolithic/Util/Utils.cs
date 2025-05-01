using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Technolithic
{

    // !!! Не менять порядок, важно при вращении строений
    public enum Direction
    {
        DOWN,
        LEFT,
        UP,
        RIGHT
    }

    public static class Utils
    {
        public static Color HexToColor(string hex)
        {
            System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml(hex);

            return new Color(col.R, col.G, col.B, col.A);
        }

        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        public static T CheckAndGetCorrectEnum<T>(int checkEnumNum, T defaulValue) where T : struct, IConvertible
        {
            if(Enum.IsDefined(typeof(T), checkEnumNum))
            {
                return (T)Enum.ToObject(typeof(T), checkEnumNum);
            }
            else
            {
                return defaulValue;
            }
        }

        public static float GetAngle(Direction direction, bool inverted = false)
        {
            if (inverted)
            {
                switch (direction)
                {
                    case Direction.DOWN:
                        return 3.14159f;
                    case Direction.LEFT:
                        return 4.71239f;
                    case Direction.UP:
                        return 0;
                    case Direction.RIGHT:
                        return 1.5708f;
                }
            }
            else
            {
                switch (direction)
                {
                    case Direction.DOWN:
                        return 0;
                    case Direction.LEFT:
                        return 1.5708f;
                    case Direction.UP:
                        return 3.14159f;
                    case Direction.RIGHT:
                        return 4.71239f;
                }
            }

            return 0;
        }

        public static float Angle(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }

        public static Vector2 Perpendicular(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        public static Vector2 AngleToVector(float angleRadians, float length)
        {
            return new Vector2((float)Math.Cos(angleRadians) * length, (float)Math.Sin(angleRadians) * length);
        }

        public static int GetDistance(Vector2 pos1, Vector2 pos2)
        {
            return (int)Vector2.Distance(pos1, pos2);
        }

        public static int GetDistance(Tile tile1, Tile tile2)
        {
            return (int)Vector2.Distance(new Vector2(tile1.X, tile1.Y), new Vector2(tile2.X, tile2.Y));
        }

        public static T NextEnum<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }


        public static Color Blend(Color tint, Color color)
        {
            float ecuPercent = tint.R / 255f;
            int newR = (int)(ecuPercent * color.R);


            ecuPercent = tint.G / 255f;
            int newG = (int)(ecuPercent * color.G);


            ecuPercent = tint.B / 255f;
            int newB = (int)(ecuPercent * color.B);

            return new Color(newR, newG, newB);
        }

        public static float ReverseLerp(float min, float max, float value)
        {
            return (value - min) / (max - min);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldMatrix"></param>
        /// <param name="n">Array dimension</param>
        /// <param name="times">How many times to rotate</param>
        /// <param name="ccw">Counter clockwise rotation</param>
        /// <returns></returns>
        public static T[,] RotateMatrix<T>(T[,] oldMatrix, Direction direction)
        {
            int times = 0;

            switch(direction)
            {
                case Direction.DOWN:
                    times = 0;
                    break;
                case Direction.LEFT:
                    times = 1;
                    break;
                case Direction.UP:
                    times = 2;
                    break;
                case Direction.RIGHT:
                    times = 3;
                    break;
            }

            if (times == 0)
                return oldMatrix;

            T[,] newMatrix = oldMatrix;

            for (int t = 0; t < times; t++)
            {
                newMatrix = RotateMatrixCCW(newMatrix);
            }

            return newMatrix;
        }

        private static T[,] RotateMatrixCCW<T>(T[,] oldMatrix)
        {
            T[,] newMatrix = new T[oldMatrix.GetLength(1), oldMatrix.GetLength(0)];

            int newColumn, newRow = 0;

            for(int oldColumn = oldMatrix.GetLength(1) - 1; oldColumn >= 0; oldColumn--)
            {
                newColumn = 0;

                for(int oldRow = 0; oldRow < oldMatrix.GetLength(0); oldRow++)
                {
                    newMatrix[newRow, newColumn] = oldMatrix[oldRow, oldColumn];
                    newColumn++;
                }
                newRow++;
            }

            return newMatrix;
        }

        public static IEnumerable<Tile> GetTilesInCircle(Tile centerTile, int radius)
        {
            int centerX = centerTile.X;
            int centerY = centerTile.Y;

            int rr = radius * radius;

            for (int x = centerX - radius; x < centerX + radius; x++)
            {
                for (int y = centerY - radius; y < centerY + radius; y++)
                {
                    int dx = centerX - x;
                    int dy = centerY - y;

                    if ((dx * dx + dy * dy) < rr)
                    {
                        Tile tile = GameplayScene.Instance.World.GetTileAt(x, y);
                        if (tile != null)
                        {
                            yield return tile;
                        }
                    }
                }
            }
        }
    }
}

