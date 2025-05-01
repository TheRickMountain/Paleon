using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public enum AchievementId
    {
        BACK_TO_THE_FUTURE = 0,
        I_SURVIVED_100_DAYS = 1,
        DENIED = 2,
        ENGINE_OF_PROGRESS = 3,
        FIRST_BREAD = 4,
        BEST_FRIENDS_FOREVER = 5,
        TRIBE = 6,
        VILLAGE = 7,
        CITY = 8,
        FIRST_CERAMIC = 9,
        FIRST_COPPER = 10,
        FIRST_BRONZE = 11,
        FIRST_IRON = 12,
        MULTITOOL = 13,
        ARE_YOU_SURE_IT_DOESNT_STINK = 14,
        HOMEMADE_PRODUCTION = 15
    }

    public class InGameAchievement
    {
        public AchievementId Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public MyTexture Icon { get; private set; }

        public InGameAchievement(AchievementId id, string name, string description, MyTexture icon)
        {
            Id = id;
            Name = name;
            Description = description;
            Icon = icon;
        }
    }
}
