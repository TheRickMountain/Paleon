using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ProblemIndicatorManager
    {
        
        public bool IsHungerIndicatorActive { get; private set; }
        public string HungerIndicatorInfo { get; private set; }

        public bool IsColdIndicatorActive { get; private set; }
        public string ColdIndicatorInfo { get; private set; }

        public bool IsHappinessIndicatorActive { get; private set; }
        public string HappinessIndicatorInfo { get; private set; }


        private float timer = 0.0f;

        public ProblemIndicatorManager()
        {

        }

        public void Update()
        {
            timer += Engine.GameDeltaTime;

            if(timer >= 1.0f)
            {
                timer = 0.0f;

                IsHungerIndicatorActive = false;
                HungerIndicatorInfo = $"{Localization.GetLocalizedText("the_settlers_are_starving")}:";

                IsColdIndicatorActive = false;
                ColdIndicatorInfo = $"{Localization.GetLocalizedText("the_settlers_are_freezing")}:";

                IsHappinessIndicatorActive = false;
                HappinessIndicatorInfo = $"{Localization.GetLocalizedText("the_settlers_are_unhappy")}:";

                foreach (var entity in GameplayScene.Instance.CreatureLayer.Entities)
                {
                    SettlerCmp settler = entity.Get<SettlerCmp>();

                    if (settler != null && settler.IsDead == false)
                    {
                        if (settler.CreatureStats.Hunger.IsDissatisfied())
                        {
                            IsHungerIndicatorActive = true;

                            HungerIndicatorInfo += $"\n- {settler.Name}";
                        }

                        if(settler.CreatureStats.Temperature.IsDissatisfied())
                        {
                            IsColdIndicatorActive = true;

                            ColdIndicatorInfo += $"\n- {settler.Name}";
                        }

                        if(settler.CreatureStats.Happiness.IsDissatisfied())
                        {
                            IsHappinessIndicatorActive = true;

                            HappinessIndicatorInfo += $"\n- {settler.Name}";
                        }
                    }
                }
            }
        }

    }
}
