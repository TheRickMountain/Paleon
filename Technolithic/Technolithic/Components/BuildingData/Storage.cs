using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class Storage
    {
        public int Capacity { get; private set; }

        public List<int> Filters { get; private set; }

        public int Stages { get; private set; }

        public MyTexture[] StagesTextures;

        public bool IsEditable { get; private set; }
        public bool CanCopySettings { get; private set; }
        public float ShelfLifeOfProducts { get; private set; }

        public Storage(BuildingTemplate buildingTemplate)
        {
            JToken storageToken = buildingTemplate.JObject["storage"];

            Capacity = storageToken["capacity"].Value<int>();

            ShelfLifeOfProducts = storageToken["shelfLifeOfProducts"].Value<float>();

            Filters = new List<int>();

            Stages = storageToken["stages"].Value<int>();

            IsEditable = storageToken["isEditable"].Value<bool>();

            CanCopySettings = storageToken["canCopySettings"].Value<bool>();

            if (Stages > 1)
            {
                StagesTextures = new MyTexture[Stages];

                MyTexture texture = buildingTemplate.Textures[Direction.DOWN];
                int frameWidth = buildingTemplate.TextureWidth;
                int frameHeight = buildingTemplate.TextureHeight;

                for (int i = 0; i < Stages; i++)
                {
                    StagesTextures[i] = texture.GetSubtexture(frameWidth * i, 0, frameWidth, frameHeight);
                }
            }

            if (!storageToken["filters"].IsNullOrEmpty())
            {
                foreach(var filter in storageToken["filters"])
                {
                    Filters.Add(filter.Value<int>());
                }
            }
        }


        public string GetInformation()
        {
            string info = "";

            info += $"\n{Localization.GetLocalizedText("capacity")}: {Capacity}";

            if (ShelfLifeOfProducts > 1)
            {
                info += $"\n{Localization.GetLocalizedText("shelf_life_of_products")}: x{ShelfLifeOfProducts}";
            }

            return info;
        }
    }
}
