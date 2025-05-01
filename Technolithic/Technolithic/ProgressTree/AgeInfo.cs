using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public enum Age
    {
        Paleolithic,
        Mesolithic,
        Neolithic,
        Chalcolithic,
        BronzeAge,
        IronAge,
        Future,
        None
    }

    public static class AgeInfo
    {
        public static string GetAgeName(Age age)
        {
            switch(age)
            {
                case Age.Paleolithic:
                    return Localization.GetLocalizedText("paleolithic");
                case Age.Mesolithic:
                    return Localization.GetLocalizedText("mesolithic");
                case Age.Neolithic:
                    return Localization.GetLocalizedText("neolithic");
                case Age.Chalcolithic:
                    return Localization.GetLocalizedText("chalcolithic");
                case Age.BronzeAge:
                    return Localization.GetLocalizedText("bronze_age");
                case Age.IronAge:
                    return Localization.GetLocalizedText("iron_age");
                case Age.Future:
                    return Localization.GetLocalizedText("future");
            }

            return null;
        }
    }
}
