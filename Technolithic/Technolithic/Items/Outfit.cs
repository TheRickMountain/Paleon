using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class Outfit
    {
        public int Defense { get; set; }
        public bool IsTop { get; set; }

        public string GetInformation()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"\n{Localization.GetLocalizedText("defense")}: {Defense.ToString("+#;-#;0")}");

            if (IsTop)
            {
                sb.Append($"\n{Localization.GetLocalizedText("protects_against_low_temperatures")}");
            }

            return sb.ToString();
        }

    }
}
