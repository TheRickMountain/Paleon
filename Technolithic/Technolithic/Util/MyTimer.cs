using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MyTimer
    {

        public Action OnTimeout { get; set; }

        private float interval = 1.0f;

        private float time = 0.0f;

        private bool isStarted = false;

        public MyTimer()
        {

        }

        public void Start()
        {
            isStarted = true;
        }

        public void Stop()
        {
            isStarted = false;
        }

        public void Reset()
        {
            time = 0.0f;
        }

        public void SetInterval(float interval)
        {
            this.interval = interval;
        }

        public void Update(float deltaTime)
        {
            if (isStarted)
            {
                if (time >= interval)
                {
                    OnTimeout.Invoke();
                    time = 0.0f;
                }
                else
                {
                    time += deltaTime;
                }
            }
        }

        public void SetTime(float time)
        {
            this.time = time;
        }

        public float GetTime()
        {
            return time;
        }

    }
}