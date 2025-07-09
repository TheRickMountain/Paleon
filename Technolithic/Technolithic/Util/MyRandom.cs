using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public static class MyRandom
    {

        private static Random random = new Random();

        public static int Range(int min, int max)
        {
            return random.Next(min, max);
        }

        public static float Range(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }

        public static int Range(int maxValue)
        {
            return random.Next(maxValue);
        }

        public static bool ProbabilityChance(int chance)
        {
            return Range(1, 101) <= chance;
        }

        public static float NextFloat()
        {
            return (float)random.NextDouble();
        }

        public static T FromSet<T>(params T[] values)
        {
            if (values.Length == 0)
                throw new ArgumentException("Values array cannot be empty", nameof(values));

            int index = Range(values.Length);
            return values[index];
        }

        public static int FromSet(params int[] values)
        {
            int num = Range(values.Length);
            return values[num];
        }

        public static bool GetRandomBool()
        {
            int num = Range(2);

            return num == 0;
        }

        public static T GetRandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(random.Next(v.Length));
        }

    }
}
