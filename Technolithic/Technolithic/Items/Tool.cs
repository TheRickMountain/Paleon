using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public enum ToolType
    {
        Woodcutting,
        Pick,
        HuntingMelee,
        Harvesting,
        Plowing,
        Fishing,
        Hauling,
        HuntingRange,
        None
    }

    public class Tool 
    {
        public int MeleeDamage { get; init; }
        public int RangeDamage { get; init; }
        public int Level { get; init; }
        public float Efficiency { get; init; }
        public ToolType ToolType { get; init; }
        public CreatureType CreatureType { get; init; }
        public float RechargeTime { get; init; }
        public int AmmoTextureId { get; init; }
        public float ProjectileSpeed { get; init; }
        public LaborType[] LaborTypes { get; init; }
        public InteractionType[] InteractionTypes { get; init; } = new InteractionType[0];

        [JsonIgnore]
        public MyTexture AmmoTexture { get; set; }

        public void Initialize()
        {
            if (AmmoTextureId != -1)
            {
                AmmoTexture = ResourceManager.AmmoTileset[AmmoTextureId];
            }
            else
            {
                AmmoTexture = RenderManager.Pixel;
            }
        }

        public string GetInformation()
        {
            string info = "";

            if(MeleeDamage != 0)
            {
                info += $"\n{Localization.GetLocalizedText("melee_damage")}: {MeleeDamage}";
            }

            if(RangeDamage != 0)
            {
                info += $"\n{Localization.GetLocalizedText("range_damage")}: {RangeDamage}";
            }

            if(RechargeTime != 0)
            {
                info += $"\n{Localization.GetLocalizedText("recharge_time")}: {RechargeTime}";
            }

            info += $"\n{Localization.GetLocalizedText("tool_type")}: ";

            for (int i = 0; i < LaborTypes.Length; i++)
            {
                LaborType laborType = LaborTypes[i];

                if(i == LaborTypes.Length - 1)
                {
                    info += Labor.GetLaborString(laborType);
                }
                else
                {
                    info += $"{Labor.GetLaborString(laborType)}, ";
                }
            }

            if(Efficiency > 0)
            {
                info += $"\n{Localization.GetLocalizedText("efficiency")}: +{(int)(Efficiency * 100)}%";
            }

            if(ProjectileSpeed != 0)
            {
                info += $"\n{Localization.GetLocalizedText("projectile_speed")}: {ProjectileSpeed}";
            }

            return info;
        }
    }
}
