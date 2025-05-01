using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class Consumable
    {

        public Dictionary<AttributeType, float> Statistics;
        public string Container;
        public StatusEffectId ApplyStatusEffect;
        public StatusEffectId RemoveStatusEffect;
        public int StatusEffectChance;

        public float GetValueOf(AttributeType attributeType)
        {
            if (Statistics.ContainsKey(attributeType) == false)
                return 0;

            return Statistics[attributeType];
        }

        public string GetInformation()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var kvp in Statistics)
            {
                AttributeType attributeType = kvp.Key;
                float changeValue = kvp.Value;

                if (changeValue < 0)
                {
                    stringBuilder.Append($"\n/c[#DF2C14]{Localization.GetLocalizedText(attributeType.ToString().ToLower())}: {changeValue}/cd");
                }
                else if(changeValue == 0)
                {
                    stringBuilder.Append($"\n/c[#FFFFFF]{Localization.GetLocalizedText(attributeType.ToString().ToLower())}: {changeValue}/cd");
                }
                else
                {
                    stringBuilder.Append($"\n/c[#00FF00]{Localization.GetLocalizedText(attributeType.ToString().ToLower())}: +{changeValue}/cd");
                }
            }

            if (ApplyStatusEffect != StatusEffectId.None)
            {
                StatusEffect creatureEffect = StatusEffectsManager.Prototypes[ApplyStatusEffect];
                string effectName = creatureEffect.Name;

                switch (creatureEffect.Type)
                {
                    case StatusEffectType.Negative:
                        {
                            stringBuilder.Append($"\n/c[#DF2C14]{Localization.GetLocalizedText("causes")}: {effectName}/cd");
                        }
                        break;
                    case StatusEffectType.Neutral:
                        {
                            stringBuilder.Append($"\n/c[#FFFFFF]{Localization.GetLocalizedText("causes")}: {effectName}/cd");
                        }
                        break;
                    case StatusEffectType.Positive:
                        {
                            stringBuilder.Append($"\n/c[#00FF00]{Localization.GetLocalizedText("causes")}: {effectName}/cd");
                        }
                        break;
                }

                if (StatusEffectChance < 100)
                {
                    stringBuilder.Append($" [{StatusEffectChance}%]");
                }
            }

            if (RemoveStatusEffect != StatusEffectId.None)
            {
                StatusEffect creatureEffect = StatusEffectsManager.Prototypes[RemoveStatusEffect];
                string effectName = creatureEffect.Name;
                stringBuilder.Append($"\n/c[#00FFFF]{Localization.GetLocalizedText("cures")}: {effectName}/cd");

                if (StatusEffectChance < 100)
                {
                    stringBuilder.Append($" [{StatusEffectChance}%]");
                }
            }

            return stringBuilder.ToString();
        }

    }
}
