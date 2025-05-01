using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class PlantData
    {

        public string Name { get; private set; }
        public MyTexture Icon { get; private set; }
        public bool RemoveAfterHarvest { get; private set; }
        public ToolType ToolType { get; private set; } = ToolType.None;
        public bool ToolRequired { get; private set; }
        public int MaxStrength { get; private set; }

        public Dictionary<int, MyTexture[]> VariationsTextures { get; private set; }

        public Dictionary<Item, int> Fruits { get; private set; }

        public float GrowthSpeed { get; private set; }

        public int Stages { get; private set; }

        public Dictionary<int, int[]> SeasonsVariations { get; private set; }

        public bool IsFlower { get; private set; }

        public PlantData(JObject jobject)
        {
            JToken jtoken = jobject["plant"];

            Name = Localization.GetLocalizedText(jtoken["key"].Value<string>());

            string toolTypeStr = jtoken["toolType"].Value<string>();
            ToolType = Utils.ParseEnum<ToolType>(toolTypeStr);
            ToolRequired = jtoken["toolRequired"].Value<bool>();
            MaxStrength = jtoken["maxStrength"].Value<int>();
            Stages = jtoken["stages"].Value<int>();
            int variations = jtoken["variations"].Value<int>();

            MyTexture texture = ResourceManager.GetTexture(jtoken["texture"]["name"].Value<string>());
            int textureWidth = jtoken["texture"]["width"].Value<int>();
            int textureHeight = jtoken["texture"]["height"].Value<int>();

            GrowthSpeed = jtoken["growthSpeed"].Value<float>();

            VariationsTextures = new Dictionary<int, MyTexture[]>();

            for (int j = 0; j < variations; j++)
            {
                VariationsTextures.Add(j, new MyTexture[Stages]);

                for (int i = 0; i < Stages; i++)
                {
                    VariationsTextures[j][i] = texture.GetSubtexture(i * textureWidth, j * textureHeight, textureWidth, textureHeight);
                }
            }

            SeasonsVariations = new Dictionary<int, int[]>();
            SeasonsVariations.Add(0, jtoken["seasonsVariations"]["0"].Select(x => (int)x).ToArray());
            SeasonsVariations.Add(1, jtoken["seasonsVariations"]["1"].Select(x => (int)x).ToArray());
            SeasonsVariations.Add(2, jtoken["seasonsVariations"]["2"].Select(x => (int)x).ToArray());
            SeasonsVariations.Add(3, jtoken["seasonsVariations"]["3"].Select(x => (int)x).ToArray());

            Icon = VariationsTextures[0][Stages - 1];

            if (jtoken["harvestable"].IsNullOrEmpty() == false)
            {
                JToken harvestableToken = jtoken["harvestable"];

                Fruits = new Dictionary<Item, int>();

                foreach (var token in harvestableToken["harvest"])
                {
                    Item fruit = ItemDatabase.GetItemByName(token["item"].Value<string>());
                    int weight = token["weight"].Value<int>();

                    Fruits.Add(fruit, weight);
                }

                RemoveAfterHarvest = harvestableToken["removeAfterHarvest"].Value<bool>();
            }

            IsFlower = jtoken["isFlower"].Value<bool>();
        }

        public int GetSeasonVariation(Season season)
        {
            int length = SeasonsVariations[(int)season].Length;
            int variationId = MyRandom.Range(0, length);
            return SeasonsVariations[(int)season][variationId];
        }

    }
}
