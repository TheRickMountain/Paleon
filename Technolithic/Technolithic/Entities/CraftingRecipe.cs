using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class CraftingRecipe
    {
        public int Id { get; private set; }
        public Item MainItem { get; private set; }
        public Dictionary<Item, int> Ingredients { get; private set; }
        public Dictionary<Item, int> Result { get; private set; }
        public Item ItemToShow { get; private set; }
        public int Time { get; private set; }

        public CraftingRecipe(int id, Item mainItem, int time)
        {
            Id = id;
            MainItem = mainItem;
            Time = time;
            Result = new Dictionary<Item, int>();
            Ingredients = new Dictionary<Item, int>();
        }

        public CraftingRecipe AddIngredient(Item item, int weight, bool showItemOnCrafting)
        {
            Ingredients.Add(item, weight);

            if (showItemOnCrafting)
                ItemToShow = item;

            return this;
        }

        public CraftingRecipe AddResult(Item item, int weight)
        {
            Result.Add(item, weight);

            return this;
        }

    }
}
