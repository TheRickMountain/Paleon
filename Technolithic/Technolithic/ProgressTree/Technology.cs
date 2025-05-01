using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class Technology
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool HasIcon { get; set; }
        public Age Age { get; set; }
        public int RequiredXP { get; set; }

        public List<Item> UnlockedItems { get; set; }
        public List<BuildingTemplate> UnlockedBuildings { get; set; }
        public List<AnimalTemplate> UnlockedAnimals { get; set; }
        public List<MyAction> UnlockedActions { get; set; }

        public int[] Parents { get; set; }

        [JsonIgnore]
        public List<Technology> ParentTechnologies { get; set; }

        [JsonIgnore]
        public MyTexture Icon { get; set; }

        [JsonIgnore]
        public int Order { get; set; } = 0;
    }
}
