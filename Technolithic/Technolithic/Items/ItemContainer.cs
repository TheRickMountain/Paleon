using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ItemContainer
    {
        public Item Item { get; set; }

        public int FactWeight { get; set; }

        public float Durability { get; set; }

        public ItemContainer(Item item, int weight, float durability)
        {
            Item = item;
            FactWeight = weight;
            Durability = durability;
        }

        public string GetDurabilityInfo()
        {
            if (Durability > 0)
            {
                return $"- x{FactWeight} items ({(int)Durability}/{(int)Item.Durability})";
            }

            return "";
        }

    }
}
