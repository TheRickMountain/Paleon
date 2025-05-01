using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{

    public class Item : ITradable
    {
        public int Id { get; private set; }
        public MyTexture Icon { get; private set; }
        public string Name { get; private set; }

        public int Durability { get; private set; }
        public float SpoilageRate { get; private set; }
        public int ItemCategory { get; private set; }
        public bool IsDecayable { get; private set; }

        public int Value { get; private set; }
        public bool IsVirtual { get; private set; }

        private string information;

        public Tool Tool { get; private set; }
        public Outfit Outfit { get; private set; }
        public Consumable Consumable { get; private set; }

        public Equipment Equipment { get; set; }

        public bool IsStackable
        {
            get
            {
                if (Equipment != null)
                    return false;

                return true;
            }
        }

        public Item(int id, MyTexture icon, string name, int itemCategory, int value, int durability, float spoilageRate, 
            bool isDecayable, bool isVirtual)
        {
            Id = id;
            Icon = icon;
            Name = name;
            ItemCategory = itemCategory;
            Value = value;
            Durability = durability;
            SpoilageRate = spoilageRate;
            IsDecayable = isDecayable;
            IsVirtual = isVirtual;

            information = name;

            if (value != 0)
            {
                information += $"\n/c[#F5E61B]{Localization.GetLocalizedText("value")}: {value}/cd";
            }

            if (spoilageRate != 0)
            {
                float days = durability / spoilageRate;
                information += $"\n{Localization.GetLocalizedText("shelf_life")}: {string.Format("{0:0.#}", days)} {Localization.GetLocalizedText("days")}";
            }
        }

        public Item SetAsTool(Tool tool)
        {
            Tool = tool;

            information += Tool.GetInformation();

            return this;
        }

        public Item SetAsOutfit(Outfit outfit)
        {
            Outfit = outfit;

            information += Outfit.GetInformation();

            return this;
        }

        public Item SetAsConsumable(Consumable consumable)
        {
            Consumable = consumable;

            information += Consumable.GetInformation();

            return this;
        }

        public string GetInformation()
        {
            return information;
        }

        public string GetMarketName()
        {
            return Name;
        }

        public string GetMarketInformation()
        {
            return information;
        }

        public int GetMarketPrice()
        {
            return Value;
        }

        public MyTexture GetMarketIcon()
        {
            return Icon;
        }
    }
}
