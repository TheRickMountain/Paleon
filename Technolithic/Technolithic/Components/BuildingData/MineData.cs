using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MineData
    {

        public int SpawnTime { get; set; }
        public int SpawnChance { get; set; }

        public string GetInformation()
        {
            return $"\n{Localization.GetLocalizedText("spawn_time")}: {SpawnTime}";
        }

    }
}
