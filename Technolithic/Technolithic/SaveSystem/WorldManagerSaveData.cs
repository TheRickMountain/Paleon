using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class WorldManagerSaveData
    {

        public List<int> OpenedItems { get; set; }
        public List<int> HiddenItems { get; set; }
        public Dictionary<int, bool> NewSettlersFoodRationFilters { get; set; }

    }
}
