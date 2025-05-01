using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    [Serializable]
    public class SaveInfo
    {
        public string DateTime { get; set; }
        public GameVersion GameVersion { get; set; }
        public bool IsAutosave { get; set; }

    }
}
