using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public static class Calc
    {
        #region Random
        
        public static Random Random { get; } = new Random();
        
        public static T Choose<T>(this Random random, List<T> choices)
        {
            return choices[random.Next(choices.Count)];
        }

        public static List<T> GetRandomUniqueItems<T>(this Random random, List<T> items, int count)
        {
            if (count > items.Count)
            {
                throw new ArgumentException("The requested number of elements is greater than those available in the list.");
            }

            return items.OrderBy(x => random.Next()).Take(count).ToList();
        }

        #endregion
    }
}
