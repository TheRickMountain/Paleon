using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    [Serializable]
    public class SaveData
    {
        public WorldSaveData WorldSaveData { get; set; }
        public WorldManagerSaveData WorldManagerSaveData { get; set; }
        public WorldStateSaveData WorldStateSaveData { get; set; }
        public ProgressTreeSaveData ProgressTreeSaveData { get; set; }
        public List<BuildingSaveData> BuildingSaveDatas { get; set; }
        public List<CreatureSaveData> CreatureSaveDatas { get; set; }
        public List<WaterChunkSaveData> WaterChunkSaveDatas { get; set; }
        public NomadsManagerSaveData NomadsManagerSaveData { get; set; }
        public int WorldSize { get; set; }
        public List<Point> IrrigationCanalsToCheck { get; set; }
        public Dictionary<int, int> ResourcesLimits { get; set; }
        public List<AchievementId> UnlockedAchievements { get; set; }
    }
}