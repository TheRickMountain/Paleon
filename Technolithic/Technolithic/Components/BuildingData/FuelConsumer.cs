using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class FuelConsumer
    {
        public List<Item> ConsumableFuel { get; private set; }
        public float EnergyConsumption { get; private set; }

        public FuelConsumer(List<Item> requiredFuel, float energyConsumption)
        {
            ConsumableFuel = requiredFuel;
            EnergyConsumption = energyConsumption;
        }

    }
}
