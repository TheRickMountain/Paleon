using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class BeeHiveBuildingCmp : BuildingCmp
    {
        public const float PERCENT_PER_FLOWER = 1f;

        private bool isEmpty = true;
        public bool IsEmpty
        {
            get { return isEmpty; }
            private set
            {
                isEmpty = value;
                IsTurnedOn = !value;
            }
        }

        public bool GatherResources { get; set; } = false;

        private float currentPercentProgress = 0;

        private int flowersNumber = 0;

        private float baseSpeedPerMinute;
        private float additionalSpeedPerMinute;

        public BeeHiveBuildingCmp(BuildingTemplate buildingTemplate, Direction direction) : base(buildingTemplate, direction)
        {

        }

        public override void Begin()
        {
            base.Begin();

            float progressPerHour = BuildingTemplate.BeeHiveData.HoneyGenerationSpeed / WorldState.HOURS_PER_CYCLE;
            baseSpeedPerMinute = progressPerHour / WorldState.MINUTES_PER_HOUR;
        }

        public void SetSaveData(BuildingSaveData buildingSaveData)
        {
            IsEmpty = buildingSaveData.IsBeeHiveEmpty;
            GatherResources = buildingSaveData.GatherBeeHiveResources;
            currentPercentProgress = buildingSaveData.BeeHiveCurrentPercentProgress;
        }

        public void SetProgress(float percentProgress)
        {
            currentPercentProgress = percentProgress;
        }

        public override void UpdateCompleted()
        {
            base.UpdateCompleted();

            // Улей генерирует мед только в солнечную погоду и в любой сезон, кроме зимы
            if (IsEmpty && GameplayScene.Instance.WorldState.CurrentWeather == Weather.Sun &&
                GameplayScene.Instance.WorldState.CurrentSeason != Season.Winter)
            {
                currentPercentProgress += (baseSpeedPerMinute + additionalSpeedPerMinute) * Engine.GameDeltaTime;
                if (currentPercentProgress >= 100)
                {
                    currentPercentProgress = 0;

                    IsEmpty = false;
                }
            }
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            GameplayScene.WorldManager.BeeHiveBuildings.Add(this);

            Tile centerTile = GetCenterTile();

            foreach(var tile in RangeTiles)
            {
                tile.PlantDataChanged += OnTilePlantDataChanged;

                if(tile.PlantData != null && tile.PlantData.IsFlower)
                {
                    flowersNumber++;
                    RecalculateAdditionalSpeedPerMinute();
                }
            }
        }

        private void OnTilePlantDataChanged(PlantData previous, PlantData next)
        {
            if (previous == null || previous.IsFlower == false)
            {
                if (next != null && next.IsFlower)
                {
                    flowersNumber++;
                    RecalculateAdditionalSpeedPerMinute();
                    return;
                }
            }

            if(previous != null && previous.IsFlower)
            {
                if(next == null || next.IsFlower == false)
                {
                    flowersNumber--;
                    RecalculateAdditionalSpeedPerMinute();
                    return;
                }
            }
        }

        private void RecalculateAdditionalSpeedPerMinute()
        {
            float progressPerHour = ((float)flowersNumber * PERCENT_PER_FLOWER) / WorldState.HOURS_PER_CYCLE;
            additionalSpeedPerMinute = progressPerHour / WorldState.MINUTES_PER_HOUR;
        }

        public override void DestructBuilding()
        {
            base.DestructBuilding();

            GameplayScene.WorldManager.BeeHiveBuildings.Remove(this);
        }

        public void TryToEmpty(CreatureCmp creatureCmp)
        {
            Item item = ItemDatabase.GetItemByName("honeycomb");
            creatureCmp.Movement.CurrentTile.Inventory.AddCargo(new ItemContainer(item, 1, item.Durability));
            IsEmpty = true;
        }

        public override BuildingSaveData GetSaveData()
        {
            var saveData = base.GetSaveData();

            if (IsBuilt)
            {
                saveData.IsBeeHiveEmpty = IsEmpty;
                saveData.GatherBeeHiveResources = GatherResources;
                saveData.BeeHiveCurrentPercentProgress = currentPercentProgress;
            }

            return saveData;
        }

        public override string GetInformation()
        {
            string info = base.GetInformation();

            if(IsBuilt)
            {
                info += $"/c[#FFA500]{Localization.GetLocalizedText("honey")}/cd";

                if (GameplayScene.Instance.WorldState.CurrentSeason == Season.Winter)
                {
                    info += $"\n/c[#DB4E4E]{Localization.GetLocalizedText("stopped_due_to", Localization.GetLocalizedText("winter"))}";
                }
                else if (GameplayScene.Instance.WorldState.CurrentWeather != Weather.Sun)
                {
                    info += $"\n/c[#DB4E4E]{Localization.GetLocalizedText("stopped_due_to", Localization.GetLocalizedText("rain"))}";
                }

                info += $"\n/c[#1FD655]{Localization.GetLocalizedText("progress")}: {(int)currentPercentProgress}%/cd"; 
                info += $"\n/c[#63E5FF]{Localization.GetLocalizedText("total_speed")}: " +
                $"+{BuildingTemplate.BeeHiveData.HoneyGenerationSpeed + (flowersNumber * PERCENT_PER_FLOWER)}% " +
                    $"{Localization.GetLocalizedText("per_day")}/cd";
                info += $"\n- {Localization.GetLocalizedText("base_speed")}: +{BuildingTemplate.BeeHiveData.HoneyGenerationSpeed}% {Localization.GetLocalizedText("per_day")}";
                info += $"\n- {Localization.GetLocalizedText("flowers")}: +{flowersNumber * PERCENT_PER_FLOWER}% {Localization.GetLocalizedText("per_day")}";
            }

            return info;
        }

    }
}
