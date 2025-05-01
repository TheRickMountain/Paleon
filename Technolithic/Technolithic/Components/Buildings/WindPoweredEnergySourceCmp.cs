using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class WindPoweredEnergySourceCmp : EnergySourceCmp
    {

        public WindPoweredEnergySourceCmp(BuildingTemplate buildingTemplate, Direction direction) 
            : base(buildingTemplate, direction)
        {
            
        }

        public override int GetActualEnergyOutput()
        {
            float windSpeedModificator = GameplayScene.Instance.WorldState.GetWindSpeedModificator();
            return (int)(BuildingTemplate.EnergySourceData.GeneratedPower * windSpeedModificator);
        }

        public override string GetInformation()
        {
            string info = base.GetInformation();

            if (IsBuilt)
            {
                info += $"\n{Localization.GetLocalizedText("depends_on_wind_speed")}";
            }

            return info;
        }
    }
}
