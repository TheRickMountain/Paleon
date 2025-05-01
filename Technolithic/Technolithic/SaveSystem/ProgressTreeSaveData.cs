using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ProgressTreeSaveData
    {

        public int CurrentExp { get; set; }

        public Dictionary<int, int> TechnologiesStates { get; set; }

        public List<string> JustUnlockedBuildingTemplates { get; set; }

    }
}
