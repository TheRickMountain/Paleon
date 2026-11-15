using System;
using System.Collections.Generic;
using System.Linq;

namespace Technolithic
{
    public class AchievementManager
    {

        private Dictionary<AchievementId, bool> achievementUnlocks = new();

        public AchievementManager(List<AchievementId> saveData)
        {
            foreach (AchievementId id in Enum.GetValues(typeof(AchievementId)))
            {
                achievementUnlocks[id] = false;
            }

            if (saveData != null)
            {
                foreach (AchievementId id in saveData)
                {
                    achievementUnlocks[id] = true;
                }

                SteamAchievements.Sync(saveData);
            }
        }

        public void UnlockAchievement(AchievementId id)
        {
            if (IsAchievementUnlocked(id)) return;

            achievementUnlocks[id] = true;

            InGameAchievement achievement = AchievementDatabase.GetAchievement(id);

            string notificationMessage = Localization.GetLocalizedText("achievement_unlocked", achievement.Name);

            GameplayScene.UIRootNodeScript?.NotificationsUI.GetComponent<NotificationsUIScript>()
                        .AddNotification(notificationMessage, NotificationLevel.ACHIEVEMENT, null);

            SteamAchievements.Unlock(id);
        }

        public bool IsAchievementUnlocked(AchievementId id)
        {
            return achievementUnlocks[id];
        }

        public int GetUnlockedAchievementsCount()
        {
            return achievementUnlocks.Count(kvp => kvp.Value);
        }

        public int GetTotalAchievementsCount()
        {
            return achievementUnlocks.Count;
        }

        public List<AchievementId> GetUnlockedAchievements()
        {
            return achievementUnlocks.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
        }
    }
}
