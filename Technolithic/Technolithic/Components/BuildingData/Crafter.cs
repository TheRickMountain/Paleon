using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class Crafter
    {
        public List<CraftingRecipe> Recipes { get; private set; }
        public Dictionary<int, CraftingRecipe> IdCraftingRecipePair { get; private set; }
        public bool IsAutocrafter { get; private set; }
        public LaborType LaborType {get; private set; }
        public CreatureType CreatureType { get; private set; }

        public Crafter(BuildingTemplate buildingTemplate)
        {
            JToken crafterJToken = buildingTemplate.JObject["crafter"];

            IsAutocrafter = crafterJToken["autocrafter"].Value<bool>();
            LaborType = Utils.ParseEnum<LaborType>(crafterJToken["laborType"].Value<string>());
            CreatureType = CreatureType.Settler;
            if(crafterJToken["creatureType"].IsNullOrEmpty() == false)
            {
                CreatureType = Utils.ParseEnum<CreatureType>(crafterJToken["creatureType"].Value<string>());
            }

            Recipes = new List<CraftingRecipe>();
            IdCraftingRecipePair = new Dictionary<int, CraftingRecipe>();

            if (!crafterJToken["recipes"].IsNullOrEmpty())
            {
                foreach (var recipeJToken in crafterJToken["recipes"])
                {
                    int id;

                    Item mainItem = ItemDatabase.GetItemByName(recipeJToken["main_item"].Value<string>());

                    int time = recipeJToken["time"].Value<int>();

                    if(!recipeJToken["id"].IsNullOrEmpty())
                    {
                        id = recipeJToken["id"].Value<int>();
                    }
                    else
                    {
                        id = mainItem.Id;
                    }

                    CraftingRecipe recipe = new CraftingRecipe(id, mainItem, time);

                    foreach(var resultJToken in recipeJToken["result"])
                    {
                        Item item = ItemDatabase.GetItemByName(resultJToken["item"].Value<string>());
                        int weight = resultJToken["weight"].Value<int>();

                        recipe.AddResult(item, weight);
                    }

                    foreach (var ingredientJToken in recipeJToken["ingredients"])
                    {
                        Item item = ItemDatabase.GetItemByName(ingredientJToken["item"].Value<string>());
                        int weight = ingredientJToken["weight"].Value<int>();

                        if (!ingredientJToken["show"].IsNullOrEmpty())
                        {
                            // Отображение предмета при крафте
                            bool show = ingredientJToken["show"].Value<bool>();

                            recipe.AddIngredient(item, weight, show);
                        }
                        else
                        {
                            recipe.AddIngredient(item, weight, false);
                        }
                    }

                    Recipes.Add(recipe);
                    IdCraftingRecipePair.Add(recipe.Id, recipe);
                }
            }
        }
    }
}
