using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Technolithic
{
    public class AnimalProduct
    {
        public Item Product { get; private set; }
        public float PercentPerDay { get; private set; }
        public Item RequiredItem { get; private set; }

        public AnimalProduct(Item product, float percentPerDay, Item requiredItem)
        {
            Product = product;
            PercentPerDay = percentPerDay;
            RequiredItem = requiredItem;
        }
    }

    public class AnimalTemplate : ITradable
    {
        public string Name { get; private set; }

        public AgeState AgeState { get; private set; }

        public MyTexture Texture { get; private set; }
        public MyTexture SleepTexture { get; private set; }

        public float MovementSpeed { get; private set; }
        public bool CanProtect { get; private set; }

        public int Health { get; private set; }
        public int Hunger { get; private set; }
        public int Energy { get; private set; }
        public int DaysUntilAging { get; private set; }

        public int Damage { get; private set; }
        public int Defense { get; private set; }

        public AnimalProduct AnimalProduct { get; private set; }

        public List<Item> Ration { get; private set; }

        public Dictionary<Item, int> Drop { get; private set; }

        public int AttackChance { get; private set; }

        public List<LaborType> AllowedLabors { get; private set; }

        public Gender Gender { get; private set; }
        public string Kind { get; private set; }

        public int TextureWidth { get; private set; }
        public int TextureHeight { get; private set; }

        public bool IsWild { get; private set; }
        public bool IsPet { get; private set; }

        public PregnancyData PregnancyData { get; private set; }
        public DomesticationData DomesticationData { get; private set; }

        public int Price { get; private set; }

        public string Json { get; private set; }
        public JObject JObject { get; private set; }

        private List<AttributeType> attributes;

        public AnimalTemplate(string json)
        {
            Json = json.Split('.')[0];
            JObject = JObject.Parse(File.ReadAllText(Path.Combine(Engine.ContentDirectory, "Animals", json)));

            Name = Localization.GetLocalizedText(JObject["key"].Value<string>());

            AgeState = (AgeState)Enum.Parse(typeof(AgeState), JObject["AgeState"].Value<string>());

            CanProtect = JObject["canProtect"].Value<bool>();

            IsWild = JObject["isWild"].Value<bool>();
            IsPet = JObject["isPet"].IsNullOrEmpty() ? false : JObject["isPet"].Value<bool>();

            MyTexture texture = ResourceManager.GetTexture(JObject["texture"]["name"].Value<string>());
            TextureWidth = JObject["texture"]["width"].Value<int>();
            TextureHeight = JObject["texture"]["height"].Value<int>();
            Tileset tileset = new Tileset(texture, TextureWidth, TextureHeight);

            Texture = tileset[0];
            SleepTexture = tileset[1];

            MovementSpeed = JObject["movementSpeed"].Value<float>();

            Health = JObject["health"].Value<int>();
            Hunger = JObject["hunger"].Value<int>();
            Energy = JObject["energy"].Value<int>();
            DaysUntilAging = JObject["DaysUntilAging"].Value<int>();

            Damage = JObject["damage"].Value<int>();
            Defense = JObject["defense"].Value<int>();

            AllowedLabors = new List<LaborType>();
            foreach(var labor in JObject["allowedLabors"])
            {
                AllowedLabors.Add((LaborType)Enum.Parse(typeof(LaborType), labor.Value<string>()));
            }

            Gender = Utils.ParseEnum<Gender>(JObject["gender"].Value<string>());
            Kind = JObject["kind"].Value<string>();

            AttackChance = JObject["attackChance"].Value<int>();

            if (JObject["product"].IsNullOrEmpty() == false)
            {
                JToken productJToken = JObject["product"];

                Item item = ItemDatabase.GetItemById(productJToken["item"].Value<int>());
                float percentPerDay = productJToken["percentPerDay"].Value<float>();
                Item requiredItem = ItemDatabase.GetItemById(productJToken["requiredItem"].Value<int>());

                AnimalProduct = new AnimalProduct(item, percentPerDay, requiredItem);
            }

            Ration = new List<Item>();

            foreach(var value in JObject["ration"])
            {
                Ration.Add(ItemDatabase.GetItemByName(value.Value<string>()));
            }

            Drop = new Dictionary<Item, int>();

            foreach(var value in JObject["drop"])
            {
                string itemName = value["item"].Value<string>();
                int count = value["count"].Value<int>();
                Item item = ItemDatabase.GetItemByName(itemName);

                Drop.Add(item, count);
            }

            attributes = new List<AttributeType>();

            foreach(var value in JObject["attributes"])
            {
                AttributeType attributeType = Utils.ParseEnum<AttributeType>(value["name"].Value<string>());

                attributes.Add(attributeType);
            }

            PregnancyData = JObject["PregnancyData"]?.ToObject<PregnancyData>();
            DomesticationData = JObject["DomesticationData"]?.ToObject<DomesticationData>();

            Price = JObject["Price"].IsNullOrEmpty() ? 0 : JObject["Price"].Value<int>();
        }

        public Entity CreateEntity(int daysUntilAging, InteractablesManager interactablesManager)
        {
            Entity entity = new Entity();

            entity.Add(new CreatureThoughts());

            CreatureStats creatureStats = new CreatureStats(Defense, Damage, 1.2f);
            creatureStats.DeactivateAll();

            foreach(AttributeType attributeType in attributes)
            {
                creatureStats.GetAttribute(attributeType).Active = true;
            }

            creatureStats.Health.MaxValue = Health;
            creatureStats.Health.CurrentValue = Health;
            creatureStats.Health.DeadlyIfLower = true;
            creatureStats.Health.DeadlyIfMore = false;
            creatureStats.Health.SatisfiedThreshold = (95f * creatureStats.Health.MaxValue) / 100f;
            creatureStats.Health.DissatisfiedThreshold = (20f * creatureStats.Health.MaxValue) / 100f;

            creatureStats.Hunger.MaxValue = Hunger;
            creatureStats.Hunger.CurrentValue = Hunger;
            creatureStats.Hunger.DeadlyIfLower = false;
            creatureStats.Hunger.DeadlyIfMore = false;
            creatureStats.Hunger.ChangePerDay = -80.0f;
            creatureStats.Hunger.SatisfiedThreshold = (90f * creatureStats.Hunger.MaxValue) / 100f;
            creatureStats.Hunger.DissatisfiedThreshold = (20f * creatureStats.Hunger.MaxValue) / 100f;

            creatureStats.Energy.MaxValue = Energy;
            creatureStats.Energy.CurrentValue = Energy;
            creatureStats.Energy.DeadlyIfLower = false;
            creatureStats.Energy.DeadlyIfMore = false;
            creatureStats.Energy.SatisfiedThreshold = (95f * creatureStats.Energy.MaxValue) / 100f;
            creatureStats.Energy.DissatisfiedThreshold = (20f * creatureStats.Energy.MaxValue) / 100f;

            AnimalCmp animalCmp = new AnimalCmp(creatureStats, this);
            animalCmp.SetInteractablesManager(interactablesManager);
            animalCmp.Initialize();

            animalCmp.DaysUntilAging = daysUntilAging;

            entity.Add(animalCmp);

            foreach(var food in Ration)
            {
                animalCmp.FoodRation.Add(food);
            }

            entity.Add(new SelectableCmp(
                (Engine.TILE_SIZE / 2) - (TextureWidth / 2), 
                -(TextureHeight - Engine.TILE_SIZE), 
                TextureWidth, TextureHeight, SelectableType.Animal));
            entity.Add(new MovementCmp());

            return entity;
        }

        public string GetMarketName()
        {
            return GetNameWithAgeAndSex();
        }

        public string GetMarketInformation()
        {
            return $"/c[#F5E61B]{Localization.GetLocalizedText("value")}: {Price}/cd";
        }

        public int GetMarketPrice()
        {
            return Price;
        }

        public MyTexture GetMarketIcon()
        {
            return Texture;
        }

        public string GetNameWithAgeAndSex()
        {
            string finalString = $"{Name}";

            switch (AgeState)
            {
                case AgeState.Baby:
                    {
                        finalString += $", /c[#BEBEBE]{Localization.GetLocalizedText("child")}";
                    }
                    break;
                case AgeState.Adult:
                    {
                        finalString += $", /c[#BEBEBE]{Localization.GetLocalizedText("adult")}";
                    }
                    break;
                case AgeState.Old:
                    {
                        finalString += $", /c[#BEBEBE]{Localization.GetLocalizedText("old")}";
                    }
                    break;
            }

            switch (Gender)
            {
                case Gender.F:
                    {
                        finalString += $", {Localization.GetLocalizedText("animal_female")}/cd";
                    }
                    break;
                case Gender.M:
                    {
                        finalString += $", {Localization.GetLocalizedText("animal_male")}/cd";
                    }
                    break;
            }

            return finalString;
        }
    }
}
