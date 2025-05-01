using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class HeightsGenerator
    {

        private float amplitude;
        private int octaves;
        private float roughness;

        private Random random = new Random();

        private float[,] heights;

        private int offset = 64;

        public HeightsGenerator(int width, int height, int seed, float amplitude, int octaves, float roughness)
        {
            random = new Random(seed);

            this.amplitude = amplitude;
            this.octaves = octaves;
            this.roughness = roughness;

            // При сглаживании необходимо выходить за пределы ширины и высоты, пожтому деламем массив на 2 больше
            heights = new float[width + offset * 2, height + offset * 2];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    heights[x, y] = (float)random.NextDouble();
                }
            }
        }

        public float GenerateHeight(int x, int y)
        {
            float newX = x + offset;
            float newY = y + offset;

            float total = 0;
            float d = (float)Math.Pow(2, octaves - 1);
            for (int i = 0; i < octaves; i++)
            {
                float freq = (float)(Math.Pow(2, i) / d);
                float amp = (float) Math.Pow(roughness, i) * amplitude;
                total += GetInerpolatedNoise(newX * freq, newY * freq) * amp;
            }
            return total;
        }

        private float GetInerpolatedNoise(float x, float y)
        {
            int intX = (int)x;
            int intY = (int)y;
            float fracX = x - intX;
            float fracY = y - intY;

            float v1 = GetSmoothNoise(intX, intY);
            float v2 = GetSmoothNoise(intX + 1, intY);
            float v3 = GetSmoothNoise(intX, intY + 1);
            float v4 = GetSmoothNoise(intX + 1, intY + 1);
            float i1 = Interpolate(v1, v2, fracX);
            float i2 = Interpolate(v3, v4, fracX);
            return Interpolate(i1, i2, fracY);
        }

        private float Interpolate(float a, float b, float blend)
        {
            double theta = blend * Math.PI;
            float f = (float)(1f - Math.Cos(theta)) * 0.5f;
            return a * (1f - f) + b * f;
        }
        private float GetSmoothNoise(int x, int y)
        {
            float corners = (GetNoise(x - 1, y - 1) + GetNoise(x + 1, y - 1) + GetNoise(x - 1, y + 1) + GetNoise(x + 1, y + 1)) / 16f;
            float sides = (GetNoise(x - 1, y) + GetNoise(x + 1, y) + GetNoise(x, y - 1) + GetNoise(x, y + 1)) / 8f;
            float center = GetNoise(x, y) / 4f;
            return corners + sides + center;
        }

        private float GetNoise(int x, int y)
        {
            return heights[x, y] * 2f - 1f;
        }

    }
}
