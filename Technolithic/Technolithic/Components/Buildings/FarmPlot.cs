using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Technolithic
{
    public class FarmPlot : BuildingCmp
    {
        private bool fertilize;
        private bool irrigate;

        public bool Fertilize
        {
            get { return fertilize; }
            set
            {
                if (fertilize == value)
                    return;

                fertilize = value;

                if (fertilize == false)
                {
                    if (Inventory.GetInventoryRequiredWeight(manure) >= 1)
                    {
                        Inventory.AddRequiredWeight(manure, -1);
                    }
                }
            }
        }

        public bool Irrigate
        {
            get { return irrigate; }
            set
            {
                if (irrigate == value)
                    return;

                irrigate = value;

                if (irrigate == false)
                {
                    if (Inventory.GetInventoryRequiredWeight(potOfWater) >= 1)
                    {
                        Inventory.AddRequiredWeight(potOfWater, -1);
                    }
                }
            }
        }

        protected float growthPercent = 0;

        private int lastGrowthPercent = -1;

        protected int currentStage;

        public PlantData PlantData { get; private set; }

        private Timer timer;

        private float irrigatedBoost = 2f; // В сколько раз ускоряется рост растения при орошении
        private float moistureAbsorptionRate = 0.0833333333f; // 50% per day / 600 minutes per day
        private float fertilizerAbsorptionRate = 0.0555555555f; // 33% per day / 600 minutes per day

        public bool IsWild { get; private set; }

        private int additionalHarvestScore = 0;

        private Item potOfWater;
        private Item manure;

        private Tile centerTile;

        private Dictionary<Season, int> seasonVariationSet = new Dictionary<Season, int>();

        public FarmPlot(BuildingTemplate buildingTemplate, Direction direction, InteractablesManager interactablesManager)
            : base(buildingTemplate, direction, interactablesManager)
        {
            timer = new Timer();
            PlantData = BuildingTemplate.PlantData;
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            // TODO: для этого действия МОЖНО (но необязательно использовать инструмент)
            if (PlantData.Fruits != null)
            {
                AddAvailableInteraction(InteractionType.AutoHarvest, LaborType.Harvest, false);
            }

            // TODO: для этого действия МОЖНО (но необязательно использовать инструмент)
            AddAvailableInteraction(InteractionType.Uproot, LaborType.Harvest, false);
            ActivateInteraction(InteractionType.Uproot);

            potOfWater = ItemDatabase.GetItemByName("pot_of_water");
            manure = ItemDatabase.GetItemByName("manure");

            centerTile = GetCenterTile();

            additionalHarvestScore = 0;

            growthPercent = 0;

            GetCenterTile().PlantData = PlantData;

            if (PlantData.Fruits != null)
            {
                SetInteractionDuration(InteractionType.AutoHarvest, GetHarvestOrUprootInteractionDuration());
                SetInteractionProgress(InteractionType.AutoHarvest, 0);
            }

            SetInteractionDuration(InteractionType.Uproot, GetHarvestOrUprootInteractionDuration());
            SetInteractionProgress(InteractionType.Uproot, 0);

            seasonVariationSet[Season.Summer] = PlantData.GetSeasonVariation(Season.Summer);
            seasonVariationSet[Season.Autumn] = PlantData.GetSeasonVariation(Season.Autumn);
            seasonVariationSet[Season.Winter] = PlantData.GetSeasonVariation(Season.Winter);
            seasonVariationSet[Season.Spring] = PlantData.GetSeasonVariation(Season.Spring);

            currentStage = -1;
            AddGrowingProgress(0);

            UpdateFarmPlotView();
        }

        public override void UpdateCompleted()
        {
            base.UpdateCompleted();

            if (PlantData.Fruits != null)
            {
                if (growthPercent >= 100)
                {
                    if (IsInteractionActivated(InteractionType.AutoHarvest) == false)
                    {
                        ActivateInteraction(InteractionType.AutoHarvest);
                    }
                }
                else
                {
                    if (IsInteractionActivated(InteractionType.AutoHarvest))
                    {
                        DeactivateInteraction(InteractionType.AutoHarvest);
                    }
                }
            }

            if (timer.GetTime() > 1.0f)
            {
                timer.Reset();

                if (GameplayScene.Instance.WorldState.CurrentWeather == Weather.Precipitation || centerTile.IrrigationLevel > 0)
                {
                    if (centerTile.MoistureLevel <= 0)
                    {
                        centerTile.MoistureLevel = 100;
                        UpdateFarmPlotView();
                    }
                    else
                    {
                        centerTile.MoistureLevel = 100;
                    }
                }

                if (centerTile.MoistureLevel > 0)
                {
                    if (GameplayScene.Instance.WorldState.CurrentSeason != Season.Winter)
                    {
                        centerTile.MoistureLevel -= moistureAbsorptionRate;
                    }

                    if (centerTile.MoistureLevel <= 0)
                    {
                        centerTile.MoistureLevel = 0;

                        UpdateFarmPlotView();
                    }
                }

                if (centerTile.FertilizerLevel > 0)
                {
                    if (GameplayScene.Instance.WorldState.CurrentSeason != Season.Winter)
                    {
                        centerTile.FertilizerLevel -= fertilizerAbsorptionRate;
                    }

                    if (centerTile.FertilizerLevel <= 0)
                    {
                        centerTile.FertilizerLevel = 0;

                        UpdateFarmPlotView();
                    }
                }

                UpdateGrowing();

                if (irrigate && centerTile.MoistureLevel <= 0 && Inventory.GetInventoryRequiredWeight(potOfWater) == 0)
                {
                    Inventory.AddRequiredWeight(potOfWater, 1);
                }

                if (fertilize && centerTile.FertilizerLevel <= 0 && Inventory.GetInventoryRequiredWeight(manure) == 0)
                {
                    Inventory.AddRequiredWeight(manure, 1);
                }
            }
        }

        public void SetPlantParameters(float growthPercent, int additionalHarvestScore)
        {
            this.additionalHarvestScore = additionalHarvestScore;

            this.growthPercent = growthPercent;

            if (PlantData.Fruits != null)
            {
                SetInteractionDuration(InteractionType.AutoHarvest, GetHarvestOrUprootInteractionDuration());
                SetInteractionProgress(InteractionType.AutoHarvest, 0);
            }

            SetInteractionDuration(InteractionType.Uproot, GetHarvestOrUprootInteractionDuration());
            SetInteractionProgress(InteractionType.Uproot, 0);

            currentStage = -1;
            AddGrowingProgress(0);

            UpdateFarmPlotView();
        }

        public override void CompleteInteraction(InteractionType interactionType)
        {
            base.CompleteInteraction(interactionType);

            switch (interactionType)
            {
                case InteractionType.AutoHarvest:
                    {
                        HarvestPlant();

                        if (PlantData.RemoveAfterHarvest)
                        {
                            DestructBuilding();

                            // Если растение было посажено поселенцем, то после сбора урожая это же растения будет посажено заново
                            if (IsWild == false)
                            {
                                Entity newFarmPlotEntity = GameplayScene.WorldManager.TryToBuild(BuildingTemplate,
                                    GetCenterTile().X, GetCenterTile().Y, Direction);
                                FarmPlot newFarmPlot = newFarmPlotEntity.Get<FarmPlot>();
                                newFarmPlot.Irrigate = Irrigate;
                                newFarmPlot.Fertilize = Fertilize;
                            }
                        }
                        else
                        {
                            SetInteractionProgress(InteractionType.AutoHarvest, 0);
                            SetInteractionProgress(InteractionType.Uproot, 0);

                            growthPercent = 70;
                            AddGrowingProgress(0);
                        }
                    }
                    break;
                case InteractionType.Uproot:
                    {
                        HarvestPlant();

                        DestructBuilding();
                    }
                    break;
            }
        }

        private void HarvestPlant()
        {
            if (PlantData.Fruits != null && growthPercent >= 100)
            {
                int additionalFruitsWeight = 0;

                if (additionalHarvestScore >= 20 && additionalHarvestScore <= 50)
                {
                    additionalFruitsWeight += 1;
                }
                else if (additionalHarvestScore > 50 && additionalHarvestScore <= 100)
                {
                    additionalFruitsWeight += 2;
                }

                foreach (var kvp in PlantData.Fruits)
                {
                    Item item = kvp.Key;
                    int weight = kvp.Value + additionalFruitsWeight;
                    GetCenterTile().Inventory.AddCargo(new ItemContainer(item, weight, item.Durability));
                }
            }
        }

        private void UpdateFarmPlotView()
        {
            if (IsWild || IsBuilt == false)
            {
                centerTile.GroundType = GroundType.Ground;
            }
            else
            {
                if (centerTile.MoistureLevel > 0 && centerTile.FertilizerLevel > 0)
                {
                    centerTile.GroundType = GroundType.FarmPlotWetFertilized;
                }
                else if (centerTile.MoistureLevel > 0)
                {
                    centerTile.GroundType = GroundType.FarmPlotWet;
                }
                else if (centerTile.FertilizerLevel > 0)
                {
                    centerTile.GroundType = GroundType.FarmPlotFertilized;
                }
                else
                {
                    centerTile.GroundType = GroundType.FarmPlot;
                }
            }
        }

        public void MakeWild()
        {
            IsWild = true;

            Irrigate = false;
            Fertilize = false;

            UpdateFarmPlotView();
        }

        private float GetGrowthSpeedPerMinute()
        {
            if (GameplayScene.Instance.WorldState.CurrentSeason == Season.Winter)
                return 0;

            float tempValue = PlantData.GrowthSpeed / (float)(WorldState.MINUTES_PER_HOUR * WorldState.HOURS_PER_CYCLE);

            if (centerTile.MoistureLevel > 0)
            {
                return tempValue * irrigatedBoost;
            }

            return tempValue;
        }

        private float GetGrowthSpeedPerDay()
        {
            if (GameplayScene.Instance.WorldState.CurrentSeason == Season.Winter)
                return 0;

            if (centerTile.MoistureLevel > 0)
            {
                return PlantData.GrowthSpeed * irrigatedBoost;
            }

            return PlantData.GrowthSpeed;
        }

        protected void UpdateGrowing()
        {
            float growthSpeedPerMinute = GetGrowthSpeedPerMinute();

            AddGrowingProgress(growthSpeedPerMinute);
        }

        public void AddGrowingProgress(float value)
        {
            growthPercent = MathHelper.Clamp(growthPercent + value, 0, 100);

            if (lastGrowthPercent != (int)growthPercent)
            {
                if (centerTile.FertilizerLevel > 0)
                {
                    additionalHarvestScore++;
                }

                lastGrowthPercent = (int)growthPercent;
            }

            // Вычисляем текущую стадию
            int stagesCount = PlantData.Stages - 1; // Т.к. последняя стадия - это стадия созревания

            float stagePercent = 100f / (float)stagesCount;

            int newStage = (int)(growthPercent / stagePercent);

            if (currentStage != newStage)
            {
                currentStage = newStage;
            }

            if (PlantData.Fruits != null)
            {
                SetInteractionDuration(InteractionType.AutoHarvest, GetHarvestOrUprootInteractionDuration());
            }

            SetInteractionDuration(InteractionType.Uproot, GetHarvestOrUprootInteractionDuration());

            int textureVariationId = seasonVariationSet[GameplayScene.Instance.WorldState.CurrentSeason];
            Sprite.CurrentAnimation.Frames[0] = PlantData.VariationsTextures[textureVariationId][currentStage];
        }

        protected override void OnItemAdded(Inventory senderInventory, Item item, int weight)
        {
            base.OnItemAdded(senderInventory, item, weight);

            if (IsBuilt)
            {
                if (item == potOfWater)
                {
                    centerTile.MoistureLevel = 100;
                    Inventory.PopItem(potOfWater, 1);
                    Item ceramicPot = ItemDatabase.GetItemByName("ceramic_pot");
                    GetCenterTile().Inventory.AddCargo(new ItemContainer(ceramicPot, 1, ceramicPot.Durability));

                    UpdateFarmPlotView();
                }
                else if (item == manure)
                {
                    centerTile.FertilizerLevel = 100;
                    Inventory.PopItem(manure, 1);

                    UpdateFarmPlotView();
                }
            }
        }

        public override void DestructBuilding()
        {
            base.DestructBuilding();

            GetCenterTile().PlantData = null;

            UpdateFarmPlotView();
        }

        public override string GetInformation()
        {
            string info = base.GetInformation();

            if (IsBuilt)
            {
                info += $"- {Localization.GetLocalizedText("growth_progress")}: {(int)growthPercent}%\n";
                info += $"- {Localization.GetLocalizedText("growth_speed")}: {GetGrowthSpeedPerDay()}% {Localization.GetLocalizedText("per_day")}\n\n";

                info += $"{Localization.GetLocalizedText("soil")}\n";
                info += $"- {Localization.GetLocalizedText("fertilizer")}: {(int)centerTile.FertilizerLevel}%\n";
                info += $"- {Localization.GetLocalizedText("moisture")}: {(int)centerTile.MoistureLevel}%\n\n";
            }

            return info;
        }

        public override BuildingSaveData GetSaveData()
        {
            BuildingSaveData buildingSaveData = base.GetSaveData();

            if (IsBuilt)
            {
                buildingSaveData.IsWild = IsWild;
                buildingSaveData.Irrigate = Irrigate;
                buildingSaveData.Fertilize = Fertilize;
                buildingSaveData.GrowthPercent = growthPercent;
                buildingSaveData.AdditionalHarvestScrore = additionalHarvestScore;
            }

            return buildingSaveData;
        }

        private float GetHarvestOrUprootInteractionDuration()
        {
            float duration = PlantData.MaxStrength * (growthPercent / 100f);

            if (duration <= 0)
            {
                duration = 1.0f;
            }

            return duration;
        }
    }
}
