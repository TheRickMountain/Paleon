using Newtonsoft.Json;

namespace Technolithic
{
    public class Tool 
    {
        public int MeleeDamage { get; init; }
        public int RangeDamage { get; init; }
        public float Efficiency { get; init; }
        public CreatureType CreatureType { get; init; }
        public float RechargeTime { get; init; }
        public int AmmoTextureId { get; init; }
        public float ProjectileSpeed { get; init; }
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

            if (InteractionTypes.Length > 0)
            {
                info += $"\n{Localization.GetLocalizedText("available_interactions")}:";

                foreach (InteractionType interactionType in InteractionTypes)
                {
                    InteractionData interaction = Engine.InteractionsDatabase.TryGetInteractionData(interactionType);

                    if (interaction == null) continue;

                    info += $"\n- {interaction.DisplayName}";
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
