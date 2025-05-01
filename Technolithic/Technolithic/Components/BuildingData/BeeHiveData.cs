using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class BeeHiveData
    {

        public float HoneyGenerationSpeed { get; private set; }

        public BeeHiveData(JObject jobject)
        {
            JToken jToken = jobject["beeHive"];

            HoneyGenerationSpeed = jToken["honeyGenerationSpeed"].Value<float>();
        }

    }
}
