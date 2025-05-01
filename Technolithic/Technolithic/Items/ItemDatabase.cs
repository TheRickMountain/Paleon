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

        public static void Initialize()
        {
            Tools.Add(CreatureType.Animal, new Dictionary<ToolType, List<Item>>());
            Tools.Add(CreatureType.Settler, new Dictionary<ToolType, List<Item>>());

            foreach(ToolType toolType in Enum.GetValues(typeof(ToolType)))
            {
                Tools[CreatureType.Animal].Add(toolType, new List<Item>());
                Tools[CreatureType.Settler].Add(toolType, new List<Item>());
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

                if (itemData["Tool"].IsNullOrEmpty() == false)
                {
                    JToken toolToken = itemData["Tool"];
                    int meleeDamage = toolToken["MeleeDamage"].Value<int>();
                    int rangeDamage = toolToken["RangeDamage"].Value<int>();
                    int level = toolToken["Level"].Value<int>();
                    float efficiency = toolToken["Efficiency"].Value<float>();
                    ToolType toolType = (ToolType)toolToken["ToolType"].Value<int>();
                    CreatureType creatureType = (CreatureType)toolToken["CreatureType"].Value<int>();
                    float rechargeTime = toolToken["RechargeTime"].Value<int>();
                    int ammoTextureId = toolToken["AmmoTextureId"].Value<int>();
                    float projectileSpeed = toolToken["ProjectileSpeed"].Value<float>();
                    MyTexture ammoTexture;

                    if (ammoTextureId != -1)
                        ammoTexture = ResourceManager.AmmoTileset[ammoTextureId];
                    else
                        ammoTexture = RenderManager.Pixel;

                    LaborType[] laborTypes = new LaborType[toolToken["LaborTypes"].Count()];

                    for (int i = 0; i < toolToken["LaborTypes"].Count(); i++)
                    {
                        laborTypes[i] = Utils.ParseEnum<LaborType>(toolToken["LaborTypes"][i].Value<string>());
                    }

                    item.SetAsTool(new Tool(meleeDamage, rangeDamage, level, efficiency, toolType, creatureType,
                        rechargeTime, ammoTexture, projectileSpeed, laborTypes));

                    Tools[creatureType][toolType].Add(item);
                }

                if (itemData["Outfit"].IsNullOrEmpty() == false)
                {
                    Outfit outfit = JsonSerializer.CreateDefault().Deserialize<Outfit>(itemData["Outfit"].CreateReader());

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
                    Consumable consumable = JsonSerializer.CreateDefault().Deserialize<Consumable>(itemData["Consumable"].CreateReader());

                    item.SetAsConsumable(consumable);
                }

                if (itemData["Equipment"].IsNullOrEmpty() == false)
                {
                    item.Equipment = JsonSerializer.CreateDefault().Deserialize<Equipment>(itemData["Equipment"].CreateReader());
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

    }
}
