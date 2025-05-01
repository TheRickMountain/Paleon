using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public static class AchievementDatabase
    {

        private static Dictionary<AchievementId, InGameAchievement> achievements;

        private static Tileset iconsTileset;

        private static bool isInitialized = false;

        public static void Initialize(Tileset tileset)
        {
            if (isInitialized)
            {
                throw new Exception("Achievement database is already initialized");
            }

            isInitialized = true;

            iconsTileset = tileset;

            achievements = new();

            foreach(AchievementId id in Enum.GetValues(typeof(AchievementId)))
            {
                string name = Localization.GetLocalizedText($"achv_name_{id.ToString()}");
                string description = Localization.GetLocalizedText($"achv_desc_{id.ToString()}");

                CreateAchievement(id, name, description);
            }
        }

        private static void CreateAchievement(AchievementId id, string name, string description)
        {
            MyTexture icon = iconsTileset[(int)id];

            InGameAchievement achievement = new InGameAchievement(id, name, description, icon);

            achievements.Add(id, achievement);
        }

        public static InGameAchievement GetAchievement(AchievementId id)
        {
            return achievements[id];
        }

        public static IEnumerable<InGameAchievement> GetAchievements()
        {
            return achievements.Values;
        }
    }
}
