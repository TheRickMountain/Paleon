using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class Animation
    {
        public MyTexture[] Frames;
        private int framesPerSecond;
        private Timer timer;

        public bool IsLastFrame
        {
            get { return CurrentFrame == Frames.Length - 1; }
        }

        public int FramesPerSecond
        {
            get { return framesPerSecond; }
            set
            {
                framesPerSecond = MathHelper.Clamp(value, 0, 60);
            }
        }

        public int CurrentFrame
        {
            get; set;
        }

        public int DefaultFrame
        {
            get; private set;
        }

        public Animation(MyTexture texture, int frameCount, int defaultFrame, int frameWidth, int frameHeight, int xOffset, int yOffset, int speed = 5, bool readVertical = false)
        {
            Frames = new MyTexture[frameCount];
            DefaultFrame = defaultFrame;

            for (int i = 0; i < frameCount; i++)
            {
                if (readVertical)
                {
                    Frames[i] = texture.GetSubtexture(xOffset, yOffset + (frameHeight * i), frameWidth, frameHeight);
                }
                else
                {
                    Frames[i] = texture.GetSubtexture(xOffset + (frameWidth * i), yOffset, frameWidth, frameHeight);
                }
            }
            
            FramesPerSecond = speed;
            timer = new Timer();
            Reset();
        }

        public Animation(int defaultFrame, int speed, params MyTexture[] frames)
        {
            Frames = frames;
            DefaultFrame = defaultFrame;

            FramesPerSecond = speed;
            timer = new Timer();
            Reset();
        }

        public void Update()
        {
            if (Frames.Length == 1)
            {
                CurrentFrame = DefaultFrame;
            }
            else
            {
                if (FramesPerSecond == 0)
                {
                    CurrentFrame = DefaultFrame;
                }
                else
                {
                    if (timer.GetTime() >= (1.0f / FramesPerSecond))
                    {
                        CurrentFrame = (CurrentFrame + 1) % Frames.Length;
                        timer.Reset();
                    }
                }
            }
        }

        public void Reset()
        {
            CurrentFrame = DefaultFrame;
            timer.Reset();
        }
    }
}
