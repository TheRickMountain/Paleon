using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class RainDrop
    {

        public int X { get; set; }
        public int Y { get; set; }
        public int LifeStage { get; private set; }

        private int startX;
        private int startY;

        public bool WaterAnim = false;

        public RainDrop(int x, int y, int lifeStage)
        {
            startX = x;
            startY = y;
            
            LifeStage = lifeStage;

            NextLifeStage();
        }

        public void NextLifeStage()
        {
            LifeStage++;

            if (LifeStage < 10)
            {
                X = startX - 2 * LifeStage;
                Y = startY + 6 * LifeStage;
            }

            if (LifeStage >= 12)
            {
                LifeStage = 0;

                X = startX - 2 * LifeStage;
                Y = startY + 6 * LifeStage;
            }
        }

    }
}
