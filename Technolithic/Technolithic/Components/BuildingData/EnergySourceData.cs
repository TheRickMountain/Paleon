using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class EnergySourceData
    {
        public int GeneratedPower { get; set; }
        public EnergyType EnergyType { get; set; }

        public string GetInformation()
        {
            string info = "";

            info += $"\n{Localization.GetLocalizedText("energy_type")}: {Localization.GetLocalizedText(EnergyType.ToString().ToLower())}";

            info += $"\n{Localization.GetLocalizedText("energy_output")}: {GeneratedPower}";

            return info;
        }

    }
}
