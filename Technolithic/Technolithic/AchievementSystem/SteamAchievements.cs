using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;

namespace Technolithic
{
    public static class SteamAchievements
    {
        public static void Unlock(AchievementId id)
        {
            try
            {
                if (!SteamClient.IsValid) return;

                new Achievement(id.ToString()).Trigger();
            }
            catch (Exception)
            {
            }
        }

        public static void Sync(IEnumerable<AchievementId> unlockedIds)
        {
            try
            {
                if (!SteamClient.IsValid) return;

                bool changed = false;

                foreach (AchievementId id in unlockedIds)
                {
                    Achievement achievement = new Achievement(id.ToString());
                    if (achievement.State) continue;

                    achievement.Trigger(apply: false);
                    changed = true;
                }

                if (changed)
                    SteamUserStats.StoreStats();
            }
            catch (Exception)
            {
            }
        }
    }
}
