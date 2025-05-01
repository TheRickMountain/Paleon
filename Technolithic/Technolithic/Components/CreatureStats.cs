using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public enum AttributeType
    {
        Satiety,
        Health,
        Energy,
        Temperature,
        Happiness,
        Fuel,
        Productivity
    }

    public partial class CreatureStats
    {

        public bool IsAsleep { get; set; }

        public List<MAttribute> Attribures { get; private set; }

        private Dictionary<AttributeType, MAttribute> attributesDictV2 = new Dictionary<AttributeType, MAttribute>();

        public MAttribute Hunger { get; set; } = new MAttribute
        {
            AttributeType = AttributeType.Satiety,
            MaxValue = 100.0f,
            MinValue = 0.0f,
            CanBeMoreThanMaxValue = true,
            Name = Localization.GetLocalizedText("satiety"),
            DeadMessage = "x_died_of_starvation",
            SatisfiedThreshold = 75.0f,
            DissatisfiedThreshold = 20.0f,
            CurrentValue = 100.0f,
            ChangePerDay = -40.0f,
            Active = true,
            DeadlyIfLower = true,
            DeadlyIfMore = false
        };

        public MAttribute Happiness { get; set; } = new MAttribute
        {
            AttributeType = AttributeType.Happiness,
            MaxValue = 100.0f,
            MinValue = 0.0f,
            CanBeMoreThanMaxValue = false,
            Name = Localization.GetLocalizedText("happiness"),
            DeadMessage = null,
            SatisfiedThreshold = 80.0f,
            DissatisfiedThreshold = 10.0f,
            CurrentValue = 100.0f,
            ChangePerDay = 0.0f,
            Active = true,
            DeadlyIfLower = false,
            DeadlyIfMore = false
        };

        public MAttribute Energy { get; set; } = new MAttribute
        {
            AttributeType = AttributeType.Energy,
            MaxValue = 100.0f,
            MinValue = 0.0f,
            Name = Localization.GetLocalizedText("energy"),
            SatisfiedThreshold = 99.0f,
            DissatisfiedThreshold = 15.0f,
            CurrentValue = 100.0f,
            ChangePerDay = -70.0f,
            Active = true,
            DeadlyIfLower = false,
            DeadlyIfMore = false
        };

        public MAttribute Health { get; set; } = new MAttribute
        {
            AttributeType = AttributeType.Health,
            MaxValue = 100.0f,
            MinValue = 0.0f,
            Name = Localization.GetLocalizedText("health"),
            SatisfiedThreshold = 95f,
            DissatisfiedThreshold = 20f,
            CurrentValue = 100.0f,
            ChangePerDay = 0,
            Active = true,
            DeadlyIfLower = true,
            DeadlyIfMore = false
        };

        public MAttribute Temperature { get; set; } = new MAttribute
        {
            AttributeType = AttributeType.Temperature,
            MaxValue = 100.0f,
            MinValue = 0.0f,
            Name = Localization.GetLocalizedText("temperature"),
            DeadMessage = "x_died_from_the_cold",
            SatisfiedThreshold = 95f,
            DissatisfiedThreshold = 20f,
            CurrentValue = 100.0f,
            ChangePerDay = 0.0f,
            Active = true,
            DeadlyIfLower = true,
            DeadlyIfMore = false
        };

        public MAttribute Productivity { get; set; } = new MAttribute
        {
            AttributeType = AttributeType.Productivity,
            MaxValue = 100.0f,
            MinValue = 0.0f,
            CanBeMoreThanMaxValue = false,
            Name = Localization.GetLocalizedText("productivity"),
            DeadMessage = null,
            SatisfiedThreshold = 0.0f,
            DissatisfiedThreshold = 0.0f,
            CurrentValue = 100.0f,
            ChangePerDay = 50.0f,
            Active = true,
            DeadlyIfLower = false,
            DeadlyIfMore = false
        };

        public int NativeDefense { get; private set; }
        public int NativeMeleeDamage { get; private set; }
        public float NativeRechargeTime { get; private set; }

        public CreatureStats(int nativeDefense, int nativeMeleeDamage, float nativeRechargeTime)
        {
            Attribures = new List<MAttribute>();
            Attribures.Add(Hunger);
            Attribures.Add(Happiness);
            Attribures.Add(Energy);
            Attribures.Add(Health);
            Attribures.Add(Temperature);
            Attribures.Add(Productivity);

            attributesDictV2.Add(Hunger.AttributeType, Hunger);
            attributesDictV2.Add(Happiness.AttributeType, Happiness);
            attributesDictV2.Add(Energy.AttributeType, Energy);
            attributesDictV2.Add(Health.AttributeType, Health);
            attributesDictV2.Add(Temperature.AttributeType, Temperature);
            attributesDictV2.Add(Productivity.AttributeType, Productivity);

            NativeDefense = nativeDefense;
            NativeMeleeDamage = nativeMeleeDamage;
            NativeRechargeTime = nativeRechargeTime;
        }

        public void DeactivateAll()
        {
            foreach (var attr in Attribures)
            {
                attr.Active = false;
            }
        }

        public MAttribute GetAttribute(AttributeType attributeType)
        {
            return attributesDictV2[attributeType];
        }

        public void Consume(Item item)
        {
            if (item.Consumable == null)
                return;

            foreach (var kvp in item.Consumable.Statistics)
            {
                AttributeType attributeType = kvp.Key;
                float changeValue = kvp.Value;

                attributesDictV2[attributeType].CurrentValue += changeValue;
            }
        }

    }
}
