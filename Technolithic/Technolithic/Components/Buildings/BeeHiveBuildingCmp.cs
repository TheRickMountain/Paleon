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

        private float currentPercentProgress = 0;

        private int flowersNumber = 0;

        private float baseSpeedPerMinute;
        private float additionalSpeedPerMinute;

        public BeeHiveBuildingCmp(BuildingTemplate buildingTemplate, Direction direction, InteractablesManager interactablesManager) 
            : base(buildingTemplate, direction, interactablesManager)
        {
            float progressPerHour = BuildingTemplate.BeeHiveData.HoneyGenerationSpeed / WorldState.HOURS_PER_CYCLE;
            baseSpeedPerMinute = progressPerHour / WorldState.MINUTES_PER_HOUR;
        }

        public void SetSaveData(BuildingSaveData buildingSaveData)
        {
            IsEmpty = buildingSaveData.IsBeeHiveEmpty;
            currentPercentProgress = buildingSaveData.BeeHiveCurrentPercentProgress;
        }

        public void SetProgress(float percentProgress)
        {
            currentPercentProgress = percentProgress;
        }

        public override void UpdateCompleted()
        {
            base.UpdateCompleted();

            if (IsEmpty == false)
            {
                if (BuildingTemplate.BeeHiveData.IsWild)
                {
                    if (IsInteractionActivated(InteractionType.CollectWildHoney) == false)
                    {
                        ActivateInteraction(InteractionType.CollectWildHoney);
                    }
                }
                else
                {
                    if (IsInteractionActivated(InteractionType.CollectHoney) == false)
                    {
                        ActivateInteraction(InteractionType.CollectHoney);
                    }
                }
            }

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

            // TODO: время сбора нужно загрузить из json файла
            if(BuildingTemplate.BeeHiveData.IsWild)
            {
                AddAvailableInteraction(InteractionType.CollectWildHoney, LaborType.Gathering, false);
                SetInteractionDuration(InteractionType.CollectWildHoney, 5.0f);
            }
            else
            {
                AddAvailableInteraction(InteractionType.CollectHoney, LaborType.Beekeeping, false);
                SetInteractionDuration(InteractionType.CollectHoney, 5.0f);

                MarkInteraction(InteractionType.CollectHoney);
            }

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

        public override void CompleteInteraction(InteractionType interactionType)
        {
            base.CompleteInteraction(interactionType);

            switch (interactionType)
            {
                case InteractionType.CollectHoney:
                case InteractionType.CollectWildHoney:
                    {
                        DeactivateInteraction(interactionType);

                        // TODO: выдаваемый ресурс нужно получать из json файла
                        Item item = ItemDatabase.GetItemByName("honeycomb");
                        GetCenterTile().Inventory.AddCargo(new ItemContainer(item, 1, item.Durability));
                        IsEmpty = true;
                    }
                    break;
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

        public override BuildingSaveData GetSaveData()
        {
            var saveData = base.GetSaveData();

            if (IsBuilt)
            {
                saveData.IsBeeHiveEmpty = IsEmpty;
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

                if (IsEmpty)
                {
                    info += $"\n/c[#1FD655]{Localization.GetLocalizedText("progress")}: {(int)currentPercentProgress}%/cd";                    
                }
                else
                {
                    info += $"\n/c[#1FD655]{Localization.GetLocalizedText("progress")}: 100%/cd";
                }

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
