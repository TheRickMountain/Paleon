using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public abstract class EnergySourceCmp : BuildingCmp
    {

        public int AvailablePower { get; set; }

        public EnergySourceCmp(BuildingTemplate buildingTemplate, Direction direction)
            : base(buildingTemplate, direction)
        {

        }

        public override void UpdateCompleted()
        {
            base.UpdateCompleted();

            IsTurnedOn = GetActualEnergyOutput() > 0;
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            GameplayScene.Instance.EnergyManager.AddEnergySource(this);
        }

        public override void DestructBuilding()
        {
            base.DestructBuilding();

            GameplayScene.Instance.EnergyManager.RemoveEnergySource(this);
        }

        public override string GetInformation()
        {
            string info = base.GetInformation();

            if (IsBuilt)
            {
                info += $"\n{Localization.GetLocalizedText("energy_type")}: " +
                        $"{Localization.GetLocalizedText(BuildingTemplate.EnergySourceData.EnergyType.ToString().ToLower())}";
                info += $"\n{Localization.GetLocalizedText("energy_output")}: {GetActualEnergyOutput()}/{BuildingTemplate.EnergySourceData.GeneratedPower}";
                info += $"\n{Localization.GetLocalizedText("energy_available")}: {AvailablePower}";
            }

            return info;
        }

        public abstract int GetActualEnergyOutput();

    }
}
