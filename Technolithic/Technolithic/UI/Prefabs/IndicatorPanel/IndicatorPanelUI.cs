using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class IndicatorPanelUI : ListViewUI
    {

        private MImageUI hungerIndicator;
        private MImageUI coldIndicator;
        private MImageUI happinessIndicator;

        private ProblemIndicatorManager indicatorManager;

        private bool blinkState = true;
        private float blinkInterval = 2.0f;
        private float blinkProgress = 0.0f;

        public IndicatorPanelUI(Scene scene) : base(scene, 48, 48, 1, 10, false, false)
        {
            hungerIndicator = new MImageUI(scene);
            hungerIndicator.Width = 48;
            hungerIndicator.Height = 48;
            hungerIndicator.Image.Texture = ResourceManager.HungerIndicator;

            coldIndicator = new MImageUI(scene);
            coldIndicator.Width = 48;
            coldIndicator.Height = 48;
            coldIndicator.Image.Texture = ResourceManager.ColdIndicator;

            happinessIndicator = new MImageUI(scene);
            happinessIndicator.Width = 48;
            happinessIndicator.Height = 48;
            happinessIndicator.Image.Texture = ResourceManager.UnhappyIndicator;

            indicatorManager = GameplayScene.Instance.ProblemIndicatorManager;

            GetComponent<ListViewUIScript>().AddItem(hungerIndicator);
            GetComponent<ListViewUIScript>().AddItem(coldIndicator);
            GetComponent<ListViewUIScript>().AddItem(happinessIndicator);
            GetComponent<ListViewUIScript>().GrabMouse = false;
        }

        public override void Update(int mouseX, int mouseY)
        {
            base.Update(mouseX, mouseY);

            if(blinkState)
            {
                blinkProgress += Engine.DeltaTime;

                if(blinkProgress >= blinkInterval)
                {
                    blinkState = false;
                }
            }
            else
            {
                blinkProgress -= Engine.GameDeltaTime;

                if (blinkProgress <= 1.0f)
                {
                    blinkState = true;
                }
            }

            hungerIndicator.Active = indicatorManager.IsHungerIndicatorActive;
            hungerIndicator.Tooltips = indicatorManager.HungerIndicatorInfo;
            hungerIndicator.Image.Color = Color.White * (blinkProgress / blinkInterval);

            coldIndicator.Active = indicatorManager.IsColdIndicatorActive;
            coldIndicator.Tooltips = indicatorManager.ColdIndicatorInfo;
            coldIndicator.Image.Color = Color.White * (blinkProgress / blinkInterval);

            happinessIndicator.Active = indicatorManager.IsHappinessIndicatorActive;
            happinessIndicator.Tooltips = indicatorManager.HappinessIndicatorInfo;
            happinessIndicator.Image.Color = Color.White * (blinkProgress / blinkInterval);
        }

    }
}
