using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{

    public class Assignable
    {
        public int Slots { get; private set; }

        public StatusEffectId ApplyStatusEffect { get; private set; }

        public Assignable(JObject jobject)
        {
            JToken jtoken = jobject["assignable"];

            Slots = jtoken["slots"].Value<int>();

            string statusEffectIdString = jtoken["ApplyStatusEffect"].Value<string>();
            ApplyStatusEffect = Utils.ParseEnum<StatusEffectId>(statusEffectIdString);
        }

        public string GetInformation()
        {
            string info = "";

            if(Slots != 0)
            {
                info += $"\n{Localization.GetLocalizedText("capacity")}: {Slots}";
            }

            return info;
        }

    }
}
