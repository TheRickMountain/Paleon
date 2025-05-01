using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class WorldStateUIScript : MScript
    {

        private MNode sunAndMoonImage;
        private MImageCmp sunAndMoonImageCmp;
        private MyText stateText;
        private MyText settlersText;
        private MyText foodText;
        private MyText windText;

        private WorldState worldState;

        public WorldStateUIScript() : base(true)
        {

        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            sunAndMoonImage = ParentNode.GetChildByName("SunAndMoon");
            sunAndMoonImageCmp = sunAndMoonImage.GetComponent<MImageCmp>();
            stateText = (MyText)ParentNode.GetChildByName("State");
            settlersText = (MyText)ParentNode.GetChildByName("Settlers");
            foodText = (MyText)ParentNode.GetChildByName("Food");
            windText = (MyText)ParentNode.GetChildByName("Wind");
            worldState = GameplayScene.Instance.WorldState;

            worldState.OnNextDayStartedCallback += UpdateState;
            GameplayScene.WorldManager.CbOnSettlersCountChanged += UpdateSettlersCount;
            GameplayScene.Instance.TotalResourcesChart.CbOnItemCountChanged += OnItemsCountChanged;

            UpdateState(worldState.CurrentDay, worldState.CurrentSeason);
            UpdateSettlersCount(GameplayScene.WorldManager.TotalSettlersCount);
            OnItemsCountChanged(null, 0);
        }

        public override void Update(int mouseX, int mouseY)
        {
            float rotation = GetEarthRotationInRadians();
            sunAndMoonImageCmp.Rotation = rotation;
        }

        public float GetEarthRotationInRadians()
        {
            float percent = worldState.GetDayProgressPercent();

            float earthAngle = (360 * percent) / 100;

            return MathUtils.ToRadians(earthAngle);
        }

        private void UpdateState(int day, Season season)
        {
            string info = $"{Localization.GetLocalizedText("day")} {day}";

            string seasonName = Engine.Instance.SeasonsLocalizations[(int)season];

            info += $"\n{Localization.GetLocalizedText("season")}: {seasonName}";

            stateText.Text = info;

            WindSpeed windSpeed = GameplayScene.Instance.WorldState.WindSpeed;
            windText.Text = $"{Localization.GetLocalizedText("wind")}: {Localization.GetLocalizedText(windSpeed.ToString().ToLower())}";
        }

        private void UpdateSettlersCount(int settlersCount)
        {
            settlersText.Text = $"{Localization.GetLocalizedText("settlers")}: {settlersCount}";
        }

        private void OnItemsCountChanged(Item item, int count)
        {
            int totalFoodCount = GameplayScene.Instance.TotalResourcesChart.TotalSettlersFoodCount;
            foodText.Text = $"{Localization.GetLocalizedText("food")}: {totalFoodCount}";

            if(totalFoodCount == 0)
            {
                foodText.Color = Color.OrangeRed;
            }
            else
            {
                foodText.Color = Color.White;
            }
        }
    }
}
