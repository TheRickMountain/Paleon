using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class DayColorChanger
    {

        public static Color GetTimeOfDayColor(float currentMinute, int minutesPerHour, Weather currentWeather, WorldState.HourInfo[] hourInfos)
        {
            int currentHour = (int)(currentMinute / minutesPerHour);
            int nextHour = currentHour + 1 == 24 ? 0 : currentHour + 1;

            float amount = Utils.ReverseLerp(currentHour * minutesPerHour, nextHour == 0 ? 600 : nextHour * minutesPerHour, currentMinute);

            WorldState.HourInfo currentHourInfo = hourInfos[currentHour];
            WorldState.HourInfo nextHourInfo = hourInfos[nextHour];

            if(currentWeather == Weather.Precipitation)
            {
                return Color.Lerp(currentHourInfo.RainColor, nextHourInfo.RainColor, amount);
            }
            else
            {
                return Color.Lerp(currentHourInfo.Color, nextHourInfo.Color, amount);
            }
        }

    }
}
