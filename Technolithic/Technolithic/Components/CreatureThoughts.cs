using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Technolithic
{
    public class CreatureThoughts : Component
    {

        private List<Thought> thoughts = new List<Thought>();

        private Thought currentThought;

        private Timer breakTimer;
        private bool makeBreak = false;

        private Dictionary<string, MyTexture> thoughtsIcons = new Dictionary<string, MyTexture>();

        public CreatureThoughts() : base(true, true)
        {
            breakTimer = new Timer();
        }

        public override void Begin()
        {
            thoughtsIcons.Add("hunger", ResourceManager.HungerThought);
            thoughtsIcons.Add("sleep", ResourceManager.SleepThought);
            thoughtsIcons.Add("health", ResourceManager.HealthThought);
            thoughtsIcons.Add("happy", ResourceManager.HappyThought);
            thoughtsIcons.Add("nevermind", ResourceManager.NevermindThought);
            thoughtsIcons.Add("love", ResourceManager.LoveThought);
            thoughtsIcons.Add("poisoned", ResourceManager.PoisonedThought);
            thoughtsIcons.Add("unhappy", ResourceManager.UnhappyThought);
            thoughtsIcons.Add("cold", ResourceManager.ColdThought);
        }

        public void AddThought(string thought, float showTime)
        {
            if (HasThought(thought))
                return;

            thoughts.Add(new Thought(thought, thoughtsIcons[thought], showTime));
        }

        public bool HasThought(string thought)
        {
            return thoughts.Any(x => x.Name == thought);
        }

        public override void Update()
        {
            if (makeBreak)
            {
                if (breakTimer.GetTime() >= 3)
                {
                    makeBreak = false;
                    breakTimer.Reset();
                }
                else
                {
                    return;
                }
            }

            if (currentThought == null && thoughts.Count > 0)
            {
                currentThought = thoughts[0];
            }

            if (currentThought != null)
            {
                currentThought.Update();

                if (currentThought.IsOver)
                {
                    thoughts.Remove(currentThought);
                    currentThought = null;

                    makeBreak = true;
                }
            }
        }

        public override void Render()
        {
            if(currentThought != null)
            {
                currentThought.Icon.Draw(Entity.Position - Microsoft.Xna.Framework.Vector2.UnitY * 22);
            }
        }

    }
}
