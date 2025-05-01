using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class StatusEffect
    {
        public string Name { get; private set; }
        public StatusEffectId Id { get; private set; }
        public StatusEffectType Type { get; private set; }
        public float Duration { get; private set; }
        public bool IsPassive { get; private set; }
        public float MovementSpeedChange { get; private set; }
        public IReadOnlyDictionary<AttributeType, float> AttributesChanges { get => attributesChanges; }

        public bool IsExpired { get; private set; }
        public float Progress { get; private set; }

        private Dictionary<AttributeType, float> attributesChanges;

        public StatusEffect(string name, StatusEffectId id, float duration, StatusEffectType type, bool isPassive, float movementSpeedChange)
        {
            Name = name;
            Id = id;
            Duration = duration;
            Type = type;
            IsPassive = isPassive;
            MovementSpeedChange = movementSpeedChange;

            Progress = duration;
            attributesChanges = new Dictionary<AttributeType, float>();
        }

        public StatusEffect(StatusEffect statusEffectToClone)
        {
            Name = statusEffectToClone.Name;
            Id = statusEffectToClone.Id;
            Duration = statusEffectToClone.Duration;
            Type = statusEffectToClone.Type;
            IsPassive = statusEffectToClone.IsPassive;
            MovementSpeedChange = statusEffectToClone.MovementSpeedChange;

            Progress = statusEffectToClone.Duration;
            attributesChanges = new Dictionary<AttributeType, float>(statusEffectToClone.attributesChanges);
        }

        public void Update(float deltaTime)
        {
            if (IsPassive)
                return;

            if (IsExpired)
                return;

            Progress -= deltaTime;

            if(Progress <= 0)
            {
                IsExpired = true;
            }
        }

        public void ResetProgress(float progress)
        {
            Progress = progress;
        }

        public StatusEffect AddAttributeChange(AttributeType attributeType, float changePerDay)
        {
            attributesChanges.Add(attributeType, changePerDay);

            return this;
        }

        public StatusEffect Clone()
        {
            return new StatusEffect(this);
        }

        public string GetInformation()
        {
            string info = "";

            foreach(var kvp in attributesChanges)
            {
                AttributeType attributeType = kvp.Key;
                float attributeChangeValue = kvp.Value;

                info += $"- {attributeType}: {attributeChangeValue.ToString("+#;-#;0")} {Localization.GetLocalizedText("per_day")}\n";
            }

            if(MovementSpeedChange != 0)
            {
                if (string.IsNullOrEmpty(info) == false)
                {
                    info += "\n";
                }

                info += $"{Localization.GetLocalizedText("movement_speed")}: {MovementSpeedChange}%\n";
            }

            if(IsPassive == false)
            {
                info += "\n";

                if (Progress <= WorldState.MINUTES_PER_HOUR)
                {
                    info += Localization.GetLocalizedText("duration_x_minutes", (int)Progress);
                }
                else
                {
                    float durationInHours = Progress / WorldState.MINUTES_PER_HOUR;
                    info += Localization.GetLocalizedText("duration_x_hours", durationInHours);
                }
            }

            return info;
        }

    }
}
