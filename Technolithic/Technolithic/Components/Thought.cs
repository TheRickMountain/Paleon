using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{

    public class Thought
    {
        public string Name { get; private set; }
        public MyTexture Icon { get; private set; }

        private float showTime = 5;
        private Timer timer;

        public bool IsOver { get; private set; }

        public Thought(string name, MyTexture icon, float showTime)
        {
            this.showTime = showTime;

            Name = name;
            Icon = icon;

            timer = new Timer();
        }

        public void Update()
        {
            if (timer.GetTime() >= showTime)
            {
                IsOver = true;

                timer.Reset();
            }
        }

    }
}
