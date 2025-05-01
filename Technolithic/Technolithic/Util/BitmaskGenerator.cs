using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public static class BitmaskGenerator
    {

        private static int[] bitmasks = new int[256];


        static BitmaskGenerator()
        {
            // Размер тайлсета считать как 16x16

            Generate(1, 8 + 16 + 32 + 64 + 128, new int[] { 1, 4 });
            Generate(2, 8 + 32 + 64, new int[] { 1, 4, 128 });
            Generate(3, 64, new int[] { 1, 4, 32, 128 });
            Generate(4, 16 + 64, new int[] { 1, 4, 32 });
            Generate(5, 8 + 32 + 64 + 16, new int[] { 1, 4 });
            Generate(6, 8 + 16 + 64 + 128, new int[] { 1, 4 });
            Generate(7, 8 + 64, new int[] { 1, 4, 128 });
            Generate(8, 8 + 16 + 64, new int[] { 1, 4 });
            Generate(9, 1 + 2 + 8 + 16 + 64 + 128);

            Generate(16, 2 + 4 + 16 + 64 + 128, new int[] { 1, 32 });
            Generate(17, 1 + 2 + 4 + 8 + 16 + 32 + 64 + 128);
            Generate(18, 1 + 2 + 8 + 32 + 64, new int[] { 4, 128 });
            Generate(19, 2 + 64, new int[] { 1, 4, 32, 128 });
            Generate(20, 2 + 4 + 16 + 64, new int[] { 1, 32 });
            Generate(21, 1 + 2 + 4 + 8 + 16 + 32 + 64);
            Generate(22, 1 + 2 + 4 + 8 + 16 + 64 + 128);
            Generate(23, 1 + 2 + 8 + 64, new int[] { 4, 128 });
            Generate(24, 1 + 2 + 4 + 8 + 16 + 64);
            Generate(25, 2 + 4 + 8 + 16 + 32 + 64);

            Generate(32, 2 + 4 + 16, new int[] { 1, 32, 128 });
            Generate(33, 1 + 2 + 4 + 8 + 16, new int[] { 32, 128 });
            Generate(34, 1 + 2 + 8, new int[] { 4, 32, 128 });
            Generate(35, 2, new int[] { 1, 4, 32, 128 });
            Generate(36, 2 + 16 + 64 + 128, new int[] { 1, 32 });
            Generate(37, 1 + 2 + 8 + 16 + 32 + 64 + 128);
            Generate(38, 2 + 4 + 8 + 16 + 32 + 64 + 128);
            Generate(39, 2 + 8 + 32 + 64, new int[] { 4, 128 });
            Generate(40, 2 + 8 + 16 + 32 + 64 + 128);
            Generate(41, 2 + 8 + 16 + 64 + 128);

            Generate(48, 16, new int[] { 1, 4, 32, 128 });
            Generate(49, 8 + 16, new int[] { 1, 4, 32, 128 });
            Generate(50, 8, new int[] { 1, 4, 32, 128 });
            Generate(51, 0, new int[] { 1, 4, 32, 128 });
            Generate(52, 2 + 16, new int[] { 1, 32, 128 });
            Generate(53, 1 + 2 + 8 + 16, new int[] { 32, 128 });
            Generate(54, 2 + 4 + 8 + 16, new int[] { 32, 128 });
            Generate(55, 2 + 8, new int[] { 4, 32, 128 });
            Generate(56, 2 + 8 + 16, new int[] { 32, 128 });
            Generate(57, 2 + 4 + 16 + 8 + 64);

            Generate(64, 16 + 64 + 128, new int[] { 1, 4, 32 });
            Generate(65, 2 + 8 + 16 + 32 + 64);
            Generate(66, 1 + 2 + 8 + 16 + 64);
            Generate(68, 2 + 16 + 64, new int[] { 1, 32 });
            Generate(69, 1 + 2 + 8 + 16 + 32 + 64);
            Generate(70, 8 + 2 + 4 + 16 + 64 + 128);
            Generate(71, 2 + 8 + 64, new int[] { 4, 128 });
            Generate(72, 2 + 8 + 16 + 64);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tileId">Id тайла который будет привязан к сгенерированным битмаскам</param>
        /// <param name="offset">Число которое будет прибавляться к результатам комбинаций</param>
        /// <param name="arr">Числа для комбинирования</param>
        private static void Generate(int tileId, int offset, int[] arr)
        {
            if (arr.Length == 0)
            {
                bitmasks[offset] = tileId;
                return;
            }
            else if(arr.Length == 1)
            {
                bitmasks[offset + arr[0]] = tileId;
                return;
            }

            // Высчитываем количество комбинаций 2 ^ arr.Length
            int count = (int)Math.Pow(2, arr.Length);
            for (int i = 0; i < count; i++)
            {
                // Переводим число в бинарное
                string binary = Convert.ToString(i, 2).PadLeft(arr.Length, '0');

                int resultsNumber = 0;

                for (int c = 0; c < binary.Length; c++)
                {
                    // Если в двоичном числе встречается 1, то прибавляем к результату комбинации
                    if (binary[c] == '1')
                    {
                        resultsNumber += arr[c];
                    }
                }

                bitmasks[resultsNumber + offset] = tileId;
            }
        }

        private static void Generate(int tileId, int offset)
        {
            bitmasks[offset] = tileId;
        }

        // Возвращает Id тайла для данного bitmask
        public static int GetTileNumber(int bitmask)
        {
            return bitmasks[bitmask];
        }

        public static int GetTileNumber(int bitmask, int textureSize, int cellSize, int startCellX, int startCellY)
        {
            int bitmaskId = bitmasks[bitmask];

            int textureSizeInCells = textureSize / cellSize;

            int x = bitmaskId % 16;
            int y = bitmaskId / 16;

            x += startCellX;
            y += startCellY;

            int tileId =  y * textureSizeInCells + x;

            return tileId;
        }

    }
}
