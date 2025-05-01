using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class DomesticationData
    {
        public string TamedForm { get; set; }
        public int Chance { get; set; }

        [JsonIgnore]
        public AnimalTemplate TamedFormAnimalTemplate { get; set; }
    }
}
