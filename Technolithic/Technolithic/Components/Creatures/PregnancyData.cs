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

        public int DurationInDays { get; init; }

        public List<string> Offspring { get; init; }

        [JsonIgnore]
        public List<AnimalTemplate> OffspringTemplates { get; set; }

    }
}
