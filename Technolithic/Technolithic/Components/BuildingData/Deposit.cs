using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class Deposit
    {
        public int Stages { get; private set; }
        public int DropPerCollect { get; private set; }
        public ToolType RequiredToolType { get; private set; }
        public Item DepositResource { get; private set; }
        public int MiningTime { get; private set; }
        public MyTexture[] StagesTextures { get; private set; }


        public Deposit(JObject jobject)
        {
            JToken jtoken = jobject["deposit"];

            Stages = jtoken["stages"].Value<int>();
            RequiredToolType = jtoken["requiredToolType"].IsNullOrEmpty() ? ToolType.None : Utils.ParseEnum<ToolType>(jtoken["requiredToolType"].Value<string>());
            DepositResource = ItemDatabase.GetItemByName(jtoken["depositResource"].Value<string>());
            MiningTime = jtoken["miningTime"].Value<int>();
            DropPerCollect = jtoken["dropPerCollect"].Value<int>();

            MyTexture texture = ResourceManager.GetTexture(jobject["texture"]["name"].Value<string>());
            int textureWidth = jobject["texture"]["width"].Value<int>();
            int textureHeight = jobject["texture"]["height"].Value<int>();

            StagesTextures = new MyTexture[Stages];

            for (int i = 0; i < Stages; i++)
            {
                StagesTextures[i] = texture.GetSubtexture(i * textureWidth, 0, textureWidth, textureHeight);
            }
        }

    }
}
