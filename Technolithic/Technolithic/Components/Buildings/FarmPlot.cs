using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Technolithic
{
    public class FarmPlot : BuildingCmp
    {

        private bool harvest;
        private bool chop;
        private bool fertilize;
        private bool irrigate;

        public bool Harvest
        {
            get { return harvest; }
            set
            {
                if (harvest == value)
                    return;

                harvest = value;

                if (harvest)
                {
                    Chop = false;
                }
                else if (harvest == false)
                {
                    if (harvestLabor != null)
                    {
                        harvestLabor.CancelAndClearAllTasksAndComplete();
                        harvestLabor = null;
                    }
                }
            }
        }

        public bool Chop
        {
            get { return chop; }
            set
            {
                if (chop == value)
                    return;

                chop = value;

                if (chop)
                {
                    Harvest = false;

                    MarkType markType = MarkType.None;
                    LaborType laborType = LaborType.None;

                    switch (PlantData.ToolType)
                    {
                        case ToolType.Harvesting:
                            markType = MarkType.CutCompletely;
                            laborType = LaborType.Harvest;
                            break;
                        case ToolType.Woodcutting:
                            markType = MarkType.ChopCompletely;
                            laborType = LaborType.Chop;
                            break;
                    }

                    chopLabor = new ChopLabor(this, markType, laborType);
                    GameplayScene.WorldManager.LaborManager.Add(chopLabor);
                }
                else
                {
                    if (chopLabor != null)
                    {
                        chopLabor.CancelAndClearAllTasksAndComplete();
                        chopLabor = null;
                    }
                }
            }
        }

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

        protected ChopLabor chopLabor;
        protected HarvestLabor harvestLabor;

        protected float growthPercent = 0;

        private int lastGrowthPercent = -1;

        protected int currentStage;

        public PlantData PlantData { get; private set; }

        private Timer timer;

        private float irrigatedBoost = 2f; // В сколько раз ускоряется рост растения при орошении
        private float moistureAbsorptionRate = 0.0833333333f; // 50% per day / 600 minutes per day
        private float fertilizerAbsorptionRate = 0.0555555555f; // 33% per day / 600 minutes per day

        public float DestructingCurrentProgress { get; set; }
        public float DestructingMaxProgress { get; private set; }

        public bool IsWild { get; private set; }

        private int additionalHarvestScore = 0;

        private Item potOfWater;
        private Item manure;

        private Tile centerTile;

        private Dictionary<Season, int> seasonVariationSet = new Dictionary<Season, int>();

        public FarmPlot(BuildingTemplate buildingTemplate, Direction direction)
            : base(buildingTemplate, direction)
        {
            timer = new Timer();
            PlantData = BuildingTemplate.PlantData;
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            potOfWater = ItemDatabase.GetItemByName("pot_of_water");
            manure = ItemDatabase.GetItemByName("manure");

            centerTile = GetCenterTile();

            additionalHarvestScore = 0;

            growthPercent = 0;

            GetCenterTile().PlantData = PlantData;

            DestructingMaxProgress = PlantData.MaxStrength * (growthPercent / 100f);
            DestructingCurrentProgress = 0;

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

            DestructingMaxProgress = PlantData.MaxStrength * (growthPercent / 100f);
            DestructingCurrentProgress = 0;

            currentStage = -1;
            AddGrowingProgress(0);

            UpdateFarmPlotView();
        }

        public void TryToChop()
        {
            HarvestPlant();

            DestructBuilding();
        }

        public void TryToHarvest(CreatureCmp creatureCmp)
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
                DestructingCurrentProgress = 0;
                growthPercent = 70;
                AddGrowingProgress(0);
            }
        }

        private void HarvestPlant()
        {
            Tile tile = GetApproachableTile();

            if (PlantData.Fruits != null && growthPercent == 100)
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
                    tile.Inventory.AddCargo(new ItemContainer(item, weight, item.Durability));
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

            if (PlantData.Fruits != null)
            {
                if (growthPercent == 100)
                {
                    if (harvest == true && (harvestLabor == null || harvestLabor.IsCompleted))
                    {
                        MarkType markType = MarkType.None;
                        LaborType laborType = LaborType.None;

                        switch (PlantData.ToolType)
                        {
                            case ToolType.Harvesting:
                                markType = MarkType.Cut;
                                laborType = LaborType.Harvest;
                                break;
                            case ToolType.Woodcutting:
                                markType = MarkType.Chop;
                                laborType = LaborType.Chop;
                                break;
                        }

                        harvestLabor = new HarvestLabor(this, markType, laborType);
                        GameplayScene.WorldManager.LaborManager.Add(harvestLabor);
                    }
                }
            }
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

            DestructingMaxProgress = PlantData.MaxStrength * (growthPercent / 100f);

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
                    GetApproachableTile().Inventory.AddCargo(new ItemContainer(ceramicPot, 1, ceramicPot.Durability));

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
                buildingSaveData.Chop = Chop;
                buildingSaveData.Harvest = Harvest;
                buildingSaveData.GrowthPercent = growthPercent;
                buildingSaveData.AdditionalHarvestScrore = additionalHarvestScore;
                buildingSaveData.HarvestingCurrentProgress = DestructingCurrentProgress;
            }

            return buildingSaveData;
        }

    }
}
