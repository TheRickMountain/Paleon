using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class CellularAutomata
    {

        public int Size { get; private set; }
        private List<int> bRule;
        private List<int> sRule;
        private int[,] map;

        private Random random;

        public CellularAutomata(int _size, List<int> _bRule, List<int> _sRule)
        {
            Size = _size;
            bRule = _bRule;
            sRule = _sRule;

            map = new int[Size, Size];
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    map[x, y] = 0;
                }
            }

            random = new Random((int)DateTime.Now.Ticks);
        }

        public void GenerateRandomMap()
        {
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    map[x, y] = random.Next(0, 1 + 1);
                }
            }
        }

        private int[,] GetSubmap(int x, int y, int width, int height)
        {
            int[,] subMap = new int[width, height];

            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    subMap[i - x, j - y] = map[i, j];
                }
            }

            return subMap;
        }


        public void ZoomIn(int startX, int startY, int zoomSize)
        {
            int[,] subMap = GetSubmap(startX, startY, zoomSize, zoomSize);

            float ratio = (float)zoomSize / (float)Size;

            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    int newX = (int)((float)x * ratio);
                    int newY = (int)((float)y * ratio);
                    map[x, y] = subMap[newX, newY];
                }
            }
        }

        private int GetNumberOfNeighbours(int x, int y)
        {
            int neighsNum = 0;

            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i == x && j == y)
                        continue;

                    neighsNum += GetCell(i, j);
                }
            }

            return neighsNum;
        }

        public void SetCell(int x, int y, int value)
        {
            if (value != 0 && value != 1)
                return;

            map[x, y] = value;
        }

        public int GetCell(int x, int y)
        {
            if (x < 0 || x >= Size || y < 0 || y >= Size)
                return 0;

            return map[x, y];
        }

        public void Update(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int[,] map2 = UpdateLife();

                Array.Copy(map2, map, map2.Length);
            }
        }

        private int[,] UpdateLife()
        {
            int[,] map2 = new int[Size, Size];

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    int neighsNum = GetNumberOfNeighbours(x, y);
                    if (bRule.Contains(neighsNum))
                    {
                        map2[x, y] = 1;
                        continue;
                    }

                    if (sRule.Contains(neighsNum))
                    {
                        map2[x, y] = map[x, y];
                        continue;
                    }

                    map2[x, y] = 0;
                }
            }

            return map2;
        }

    }
}
