using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ItemDatabase
    {

        public static Dictionary<string, Item> Items { get; private set; } = new Dictionary<string, Item>();
        private static Dictionary<int, Item> idItemPair = new Dictionary<int, Item>();
        public static List<Item> Clothes = new List<Item>();
        public static List<Item> TopClothes = new List<Item>();
        public static List<Item> Decayable = new List<Item>();
        public static Dictionary<CreatureType, Dictionary<ToolType, List<Item>>> Tools = new Dictionary<CreatureType, Dictionary<ToolType, List<Item>>>();
        public static Dictionary<int, string> ItemCategories = new Dictionary<int, string>();
        public static Dictionary<StatusEffectId, List<Item>> StatusEffectRemovers = new Dictionary<StatusEffectId, List<Item>>();
        
        private static Dictionary<CreatureType, Dictionary<InteractionType, List<Item>>> interactionTypeTools = new();

        public static void Initialize()
        {
            Tools.Add(CreatureType.Animal, new Dictionary<ToolType, List<Item>>());
            Tools.Add(CreatureType.Settler, new Dictionary<ToolType, List<Item>>());

            foreach(ToolType toolType in Enum.GetValues(typeof(ToolType)))
            {
                Tools[CreatureType.Animal].Add(toolType, new List<Item>());
                Tools[CreatureType.Settler].Add(toolType, new List<Item>());
            }

            foreach(CreatureType creatureType in  Enum.GetValues(typeof(CreatureType)))
            {
                interactionTypeTools.Add(creatureType, new Dictionary<InteractionType, List<Item>>());

                foreach (InteractionType interactionType in Enum.GetValues(typeof(InteractionType)))
                {
                    interactionTypeTools[creatureType].Add(interactionType, new List<Item>());
                }
            }

            JObject jobject = JObject.Parse(File.ReadAllText(Path.Combine(Engine.ContentDirectory, "itemCategories.json")));

            foreach (var kvp in jobject)
            {
                int key = int.Parse(kvp.Key);
                string name = Localization.GetLocalizedText(kvp.Value["key"].Value<string>());
                ItemCategories.Add(key, name);
            }

            jobject = JObject.Parse(File.ReadAllText(Path.Combine(Engine.ContentDirectory, "items.json")));

            foreach (var itemToken in jobject)
            {
                var key = itemToken.Key;
                var itemData = itemToken.Value;

                int id = itemData["Id"].Value<int>();
                string name = Localization.GetLocalizedText(itemData["key"].Value<string>());
                int durability = itemData["Durability"].Value<int>();
                int itemCategory = itemData["ItemCategory"].Value<int>();
                int value = itemData["Value"].Value<int>();
                bool isDecayable = itemData["IsDecayable"].IsNullOrEmpty() ? false : itemData["IsDecayable"].Value<bool>();
                bool isVirtual = itemData["IsVirtual"].IsNullOrEmpty() ? false : itemData["IsVirtual"].Value<bool>();
                float spoilageRate = itemData["SpoilageRate"].IsNullOrEmpty() ? 0 : itemData["SpoilageRate"].Value<float>();
                MyTexture icon = TextureBank.GroundTileset[id];

                Item item = new Item(id, icon, name, itemCategory, value, durability, spoilageRate, isDecayable, isVirtual);

                JsonSerializer serializer = JsonSerializer.CreateDefault();

                if (itemData["Tool"].IsNullOrEmpty() == false)
                {
                    Tool tool = serializer.Deserialize<Tool>(itemData["Tool"].CreateReader());

                    tool.Initialize();
                    item.SetAsTool(tool);

                    Tools[tool.CreatureType][tool.ToolType].Add(item);
                    
                    foreach(InteractionType interactionType in tool.InteractionTypes)
                    {
                        interactionTypeTools[tool.CreatureType][interactionType].Add(item);
                    }
                }

                if (itemData["Outfit"].IsNullOrEmpty() == false)
                {
                    Outfit outfit = serializer.Deserialize<Outfit>(itemData["Outfit"].CreateReader());

                    item.SetAsOutfit(outfit);

                    if (outfit.IsTop)
                    {
                        TopClothes.Add(item);
                    }
                    else
                    {
                        Clothes.Add(item);
                    }
                }

                if (itemData["Consumable"].IsNullOrEmpty() == false)
                {
                    Consumable consumable = serializer.Deserialize<Consumable>(itemData["Consumable"].CreateReader());

                    item.SetAsConsumable(consumable);
                }

                if (itemData["Equipment"].IsNullOrEmpty() == false)
                {
                    item.Equipment = serializer.Deserialize<Equipment>(itemData["Equipment"].CreateReader());
                }

                AddItem(key, item);
            }

            foreach (var toolType in Enum.GetValues(typeof(ToolType)))
            {
                if (Tools[CreatureType.Settler].ContainsKey((ToolType)toolType))
                {
                    List<Item> settlerToolsItems = Tools[CreatureType.Settler][(ToolType)toolType];
                    Tools[CreatureType.Settler][(ToolType)toolType] = settlerToolsItems.OrderBy(x => x.Tool.Level).ToList();
                    Tools[CreatureType.Settler][(ToolType)toolType].Reverse();
                }

                if (Tools[CreatureType.Animal].ContainsKey((ToolType)toolType))
                {
                    List<Item> animalToolsItems = Tools[CreatureType.Animal][(ToolType)toolType];
                    Tools[CreatureType.Animal][(ToolType)toolType] = animalToolsItems.OrderBy(x => x.Tool.Level).ToList();
                    Tools[CreatureType.Animal][(ToolType)toolType].Reverse();
                }
            }

            SortInteractionTypeToolsByEfficiency();

            foreach (var kvp in Items)
            {
                Item item = kvp.Value;
                if (item.IsDecayable)
                {
                    Decayable.Add(item);
                }

                Consumable consumable = item.Consumable;

                if (consumable != null && consumable.RemoveStatusEffect != StatusEffectId.None)
                {
                    if(StatusEffectRemovers.ContainsKey(consumable.RemoveStatusEffect) == false)
                    {
                        StatusEffectRemovers.Add(consumable.RemoveStatusEffect, new List<Item>());
                    }

                    StatusEffectRemovers[consumable.RemoveStatusEffect].Add(item);
                }
            }
        }

        private static void AddItem(string name, Item item)
        {
            Items.Add(name, item);
            idItemPair.Add(item.Id, item);
        }

        public static Item GetItemByName(string name)
        {
            return Items[name];
        }

        public static Item GetItemById(int id)
        {
            if (id == -1)
                return null;

            if (!idItemPair.ContainsKey(id))
                return null;

            return idItemPair[id];
        }

        public static Dictionary<Item, int> ConvertRawLootToReal(IReadOnlyDictionary<string, int> rawLoot)
        {
            if (rawLoot == null || rawLoot.Count == 0) return new Dictionary<Item, int>();

            Dictionary<Item, int> realLoot = new Dictionary<Item, int>();

            foreach(var kvp in rawLoot)
            {
                Item item = GetItemByName(kvp.Key);
                int amount = kvp.Value;

                realLoot.Add(item, amount);
            }

            return realLoot;
        }

        public static IReadOnlyList<Item> GetInteractionTypeTools(CreatureType creatureType, InteractionType interactionType)
        {
            return interactionTypeTools[creatureType][interactionType];
        }

        private static void SortInteractionTypeToolsByEfficiency()
        {
            foreach (CreatureType creatureType in Enum.GetValues(typeof(CreatureType)))
            {
                foreach (InteractionType interactionType in Enum.GetValues(typeof(InteractionType)))
                {
                    interactionTypeTools[creatureType][interactionType]
                        .Sort((a, b) => b.Tool.Efficiency.CompareTo(a.Tool.Efficiency));
                }
            }
        }
    }
}
