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
        public int MeleeDamage { get; private set; }
        public int RangeDamage { get; private set; }
        public int Level { get; private set; }
        public float Efficiency { get; private set; }
        public ToolType ToolType { get; private set; }
        public CreatureType CreatureType { get; private set; }
        public float RechargeTime { get; private set; }
        public float ProjectileSpeed { get; private set; }
        public LaborType[] LaborTypes { get; private set; }

        public MyTexture AmmoTexture { get; set; }

        public Tool(int meleeDamage, int rangeDamage, int level, float efficiency, ToolType toolType, CreatureType creatureType, 
            float rechargeTime, MyTexture ammoTexture, float projectileSpeed, LaborType[] laborTypes)
        {
            MeleeDamage = meleeDamage;
            RangeDamage = rangeDamage;
            Level = level;
            Efficiency = efficiency;
            ToolType = toolType;
            CreatureType = creatureType;
            RechargeTime = rechargeTime;
            AmmoTexture = ammoTexture;
            ProjectileSpeed = projectileSpeed;
            LaborTypes = laborTypes;
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
