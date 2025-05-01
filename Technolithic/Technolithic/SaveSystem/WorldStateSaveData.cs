using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class WorldStateSaveData
    {

        public Weather CurrentWeather { get; set; }
        public Season CurrentSeason { get; set; }
        public Season LastSeason { get; set; }
        public float CurrentSeasonAlpha { get; set; }
        public float LastSeasonAlpha { get; set; }
        public int CurrentDay { get; set; }
        public int CurrentHour { get; set; }
        public float CurrentMinute { get; set; }
        public Color TimeOfDayColor { get; set; }
        public WindSpeed WindSpeed { get; set; }

    }
}
