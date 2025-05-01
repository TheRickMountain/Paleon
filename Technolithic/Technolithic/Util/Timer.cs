using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class Timer
    {

        public float Time { get; set; }

        public Timer()
        {

        }

        public float GetTime()
        {
            Time += Engine.GameDeltaTime;
            return Time;
        }

        public void Reset()
        {
            Time = 0;
        }

    }
}
