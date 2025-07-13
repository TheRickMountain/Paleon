using Newtonsoft.Json;
using System.Collections.Generic;

namespace Technolithic
{
    public class DepositData
    {
        public InteractionType InteractionType { get; init; }
        public float InteractionDurationInHours { get; init; }
        public bool ToolRequired { get; init; }
        public IReadOnlyDictionary<string, int> Loot { get; init; }
        public int Stages { get; init; }

        [JsonIgnore]
        public IReadOnlyDictionary<Item, int> RealLoot { get; private set; }

        [JsonIgnore]
        public MyTexture[] StagesTextures { get; private set; }

        public void Initialize(Dictionary<Direction, MyTexture> textures)
        {
            RealLoot = ItemDatabase.ConvertRawLootToReal(Loot);

            StagesTextures = new MyTexture[Stages];

            MyTexture rawTexture = textures[Direction.DOWN];

            int frameWidth = rawTexture.Width / Stages;
            int frameHeight = rawTexture.Height;

            for (int i = 0; i < Stages; i++)
            {
                StagesTextures[i] = new MyTexture(rawTexture, frameWidth * i, 0, frameWidth, frameHeight);
            }
        }
    }
}
