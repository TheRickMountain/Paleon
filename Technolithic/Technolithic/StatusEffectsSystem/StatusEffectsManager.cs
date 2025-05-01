using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public enum StatusEffectId
    {
        Test1 = -2,
        Test2 = -1,
        None = 0,
        FoodPoisoning = 1,
        Sleeping = 2,
        Vomiting = 3,
        SleepingOnTheGround = 4,
        WinterCold = 5,
        WarmClothing = 6,
        CozyHut = 7,
        CozyTent = 8,
        WarmPlace = 9,
        DirtyAnimalPen = 10,
        Hunger = 11,
        LowLight = 12
    }

    public enum StatusEffectType
    {
        Positive,
        Negative,
        Neutral
    }

    public class StatusEffectsManager
    {
        public static Dictionary<StatusEffectId, StatusEffect> Prototypes { get; private set; } = new Dictionary<StatusEffectId, StatusEffect>()
        {
            {StatusEffectId.FoodPoisoning, new StatusEffect(Localization.GetLocalizedText("food_poisoning"), StatusEffectId.FoodPoisoning, 900, StatusEffectType.Negative, false, -20) },
            
            { StatusEffectId.Vomiting, new StatusEffect(Localization.GetLocalizedText("vomiting"), StatusEffectId.Vomiting, 4, StatusEffectType.Negative, false, 0)
                .AddAttributeChange(AttributeType.Satiety, -3000)},

            { StatusEffectId.Sleeping, new StatusEffect(Localization.GetLocalizedText("sleeping"), StatusEffectId.Sleeping, 0, StatusEffectType.Positive, true, 0)
                .AddAttributeChange(AttributeType.Energy, 870)
                .AddAttributeChange(AttributeType.Health, 600)},

            { StatusEffectId.SleepingOnTheGround, new StatusEffect(Localization.GetLocalizedText("sleeping_on_the_ground"), StatusEffectId.SleepingOnTheGround, 0, StatusEffectType.Negative, true, 0)
                .AddAttributeChange(AttributeType.Happiness, -150)},

            { StatusEffectId.WinterCold, new StatusEffect(Localization.GetLocalizedText("winter_cold"), StatusEffectId.WinterCold, 0, StatusEffectType.Negative, true, 0)
                .AddAttributeChange(AttributeType.Temperature, -150)},

            { StatusEffectId.WarmClothing, new StatusEffect(Localization.GetLocalizedText("warm_clothing"), StatusEffectId.WarmClothing, 0, StatusEffectType.Positive, true, 0)
                .AddAttributeChange(AttributeType.Temperature, 100)},

            { StatusEffectId.CozyHut, new StatusEffect(Localization.GetLocalizedText("cozy_hut"), StatusEffectId.CozyHut, 0, StatusEffectType.Positive, true, 0)
                .AddAttributeChange(AttributeType.Energy, 100)
                .AddAttributeChange(AttributeType.Health, 100)
                .AddAttributeChange(AttributeType.Happiness, 50)},

            { StatusEffectId.CozyTent, new StatusEffect(Localization.GetLocalizedText("cozy_tent"), StatusEffectId.CozyTent, 0, StatusEffectType.Positive, true, 0)
                .AddAttributeChange(AttributeType.Energy, 50)
                .AddAttributeChange(AttributeType.Health, 50)
                .AddAttributeChange(AttributeType.Happiness, 25)},

            {StatusEffectId.WarmPlace, new StatusEffect(Localization.GetLocalizedText("warm_place"), StatusEffectId.WarmPlace, 0, StatusEffectType.Positive, true, 0)
                .AddAttributeChange(AttributeType.Temperature, 650)},

            {StatusEffectId.DirtyAnimalPen, new StatusEffect(Localization.GetLocalizedText("dirty_animal_pen"), StatusEffectId.DirtyAnimalPen, 0, StatusEffectType.Negative, true, 0)
                .AddAttributeChange(AttributeType.Productivity, -150)},

            {StatusEffectId.Hunger, new StatusEffect(Localization.GetLocalizedText("hunger"), StatusEffectId.Hunger, 0, StatusEffectType.Negative, true, 0)
                .AddAttributeChange(AttributeType.Productivity, -150)},

            {StatusEffectId.LowLight, new StatusEffect(Localization.GetLocalizedText("low_light"), StatusEffectId.LowLight, 0, StatusEffectType.Negative, true, -10)}
        };

        public IReadOnlyDictionary<AttributeType, float> TotalAttributesChanges { get => totalAttributesChanges; }
        
        public float TotalMovementSpeedChange { get; private set; }

        public Action<StatusEffect> StatusEffectAdded { get; set; }

        private Dictionary<AttributeType, float> totalAttributesChanges;

        private Dictionary<StatusEffectId, StatusEffect> activeStatusEffects;

        private List<StatusEffectId> statusEffectsToRemove;

        public StatusEffectsManager()
        {
            activeStatusEffects = new Dictionary<StatusEffectId, StatusEffect>();

            totalAttributesChanges = new Dictionary<AttributeType, float>();

            statusEffectsToRemove = new List<StatusEffectId>();
        }

        public void Update(float deltaTime)
        {
            foreach(var kvp in activeStatusEffects)
            {
                StatusEffect statusEffect = kvp.Value;

                statusEffect.Update(deltaTime);

                if(statusEffect.IsExpired)
                {
                    statusEffectsToRemove.Add(statusEffect.Id);
                }
            }

            foreach(var statusEffectId in statusEffectsToRemove)
            {
                RemoveStatusEffect(statusEffectId);
            }

            statusEffectsToRemove.Clear();
        }

        public StatusEffect AddStatusEffect(StatusEffectId statusEffectId)
        {
            if (statusEffectId == StatusEffectId.None)
                return null;

            if (Prototypes.ContainsKey(statusEffectId) == false)
                return null;

            StatusEffect prototype = Prototypes[statusEffectId];

            if (activeStatusEffects.ContainsKey(statusEffectId))
            {
                activeStatusEffects[statusEffectId].ResetProgress(prototype.Duration);
            }
            else
            {
                StatusEffect statusEffect = prototype.Clone();

                activeStatusEffects.Add(statusEffectId, statusEffect);

                ApplyAttributesChanges(statusEffect);

                StatusEffectAdded?.Invoke(statusEffect);
            }

            
            return activeStatusEffects[statusEffectId];
        }

        public void RemoveStatusEffect(StatusEffectId statusEffectId)
        {
            if (statusEffectId == StatusEffectId.None)
                return;

            if (activeStatusEffects.ContainsKey(statusEffectId) == false)
                return;

            UnapplyAttributesChanges(activeStatusEffects[statusEffectId]);

            activeStatusEffects.Remove(statusEffectId);
        }

        public bool ContainsStatusEffect(StatusEffectId statusEffectId)
        {
            return activeStatusEffects.ContainsKey(statusEffectId);
        }

        private void ApplyAttributesChanges(StatusEffect statusEffect)
        {
            foreach (var kvp in statusEffect.AttributesChanges)
            {
                AttributeType attrType = kvp.Key;
                float attrChange = kvp.Value;

                if (totalAttributesChanges.ContainsKey(attrType))
                {
                    totalAttributesChanges[attrType] += attrChange;
                }
                else
                {
                    totalAttributesChanges.Add(attrType, attrChange);
                }
            }

            TotalMovementSpeedChange += statusEffect.MovementSpeedChange;
        }

        private void UnapplyAttributesChanges(StatusEffect statusEffect)
        {
            foreach (var kvp in statusEffect.AttributesChanges)
            {
                AttributeType attrType = kvp.Key;
                float attrChange = kvp.Value;

                if (totalAttributesChanges.ContainsKey(attrType))
                {
                    totalAttributesChanges[attrType] -= attrChange;
                }
            }

            TotalMovementSpeedChange -= statusEffect.MovementSpeedChange;
        }

        public IEnumerable<StatusEffect> GetStatusEffects()
        {
            foreach(var kvp in activeStatusEffects)
            {
                yield return kvp.Value;
            }
        }

        public float CalculateMovementSpeed(float movementSpeed)
        {
            float reductionFactor = movementSpeed * (TotalMovementSpeedChange / 100f);
            float updatedSpeed = movementSpeed + reductionFactor;

            return updatedSpeed;
        }

    }
}
