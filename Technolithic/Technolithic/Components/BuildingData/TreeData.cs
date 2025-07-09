using Newtonsoft.Json;
using System.Collections.Generic;

namespace Technolithic
{
    public class TreeData
    {
        public float GrowthRateInDays { get; init; }
        public TreeGrowthStage[] GrowthStages { get; init; }

        [JsonIgnore]
        public Dictionary<Season, MyTexture[]> SeasonTextures { get; private set; }

        public void Initialize(Dictionary<Direction, MyTexture> textures)
        {
            SeasonTextures = new Dictionary<Season, MyTexture[]>
            {
                { Season.Spring, new MyTexture[GrowthStages.Length] },
                { Season.Summer, new MyTexture[GrowthStages.Length] },
                { Season.Autumn, new MyTexture[GrowthStages.Length] },
                { Season.Winter, new MyTexture[GrowthStages.Length] }
            };

            MyTexture rawTexture = textures[Direction.DOWN];

            int frameWidth = rawTexture.Width / GrowthStages.Length;
            int frameHeight = rawTexture.Height / 4; // Seasons count

            for (int i = 0; i < GrowthStages.Length; i++)
            {
                SeasonTextures[Season.Spring][i] = new MyTexture(rawTexture, frameWidth * i, 0, frameWidth, frameHeight);
                SeasonTextures[Season.Summer][i] = new MyTexture(rawTexture, frameWidth * i, frameHeight, frameWidth, frameHeight);
                SeasonTextures[Season.Autumn][i] = new MyTexture(rawTexture, frameWidth * i, frameHeight * 2, frameWidth, frameHeight);
                SeasonTextures[Season.Winter][i] = new MyTexture(rawTexture, frameWidth * i, frameHeight * 3, frameWidth, frameHeight);
            }

            foreach (TreeGrowthStage treeGrowthStage in GrowthStages)
            {
                treeGrowthStage.Initialize();
            }
        }

        public MyTexture GetGrowthStageTexture(int growthStage, Season season)
        {
            return SeasonTextures[season][growthStage];
        }

        public IReadOnlyDictionary<Item, int> GetGrowthStageLoot(int growthStage)
        {
            return GrowthStages[growthStage].RealLoot;
        }
    }

    public class TreeGrowthStage
    {
        public float InteractionDurationInHours { get; init; }
        public IReadOnlyDictionary<string, int> Loot { get; init; }

        [JsonIgnore]
        public IReadOnlyDictionary<Item, int> RealLoot { get; private set; }

        public void Initialize()
        {
            RealLoot = ItemDatabase.ConvertRawLootToReal(Loot);
        }
    }
}
