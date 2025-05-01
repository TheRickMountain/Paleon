using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Technolithic
{
    public class PregnancyData
    {

        public int DurationInDays { get; set; }

        public List<string> Offspring { get; set; }

        [JsonIgnore]
        public List<AnimalTemplate> OffspringTemplates { get; set; }

    }
}
