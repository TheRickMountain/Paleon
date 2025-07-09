using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Technolithic
{
    public class TreeBuilding : BuildingCmp
    {
        private TreeData treeData;

        private float _growthProgress = 0.0f;
        private int _growthStage = -1;

        public TreeBuilding(BuildingTemplate buildingTemplate, Direction direction) 
            : base(buildingTemplate, direction)
        {
            treeData = buildingTemplate.TreeData;
        }

        public override void UpdateCompleted()
        {
            base.UpdateCompleted();

            float progressPerFrame = treeData.GrowthRateInDays / (WorldState.HOURS_PER_CYCLE * WorldState.MINUTES_PER_HOUR);
            progressPerFrame *= Engine.GameDeltaTime;

            // TODO: refactoring required (There is no point in updating growth progress every frame)
            if (GameplayScene.Instance.WorldState.CurrentSeason == Season.Winter)
            {
                progressPerFrame = 0;
            }

            SetGrowthProgress(_growthProgress + progressPerFrame);
        }

        public void SetGrowthProgress(float newGrowthProgress)
        {
            _growthProgress = MathHelper.Clamp(newGrowthProgress, 0, 1.0f);

            int newGrowthStage = (int)(_growthProgress * (treeData.GrowthStages.Length - 1) / 1.0f);

            if (newGrowthStage != _growthStage)
            {
                _growthStage = newGrowthStage;

                UpdateSprite();

                SetInteractionDuration(InteractionType.Chop,
                    treeData.GrowthStages[_growthStage].InteractionDurationInHours * WorldState.MINUTES_PER_HOUR);
            }
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            AddAvailableInteraction(InteractionType.Chop);

            // INFO: The tree can be cut down at any stage of growth
            ActivateInteraction(InteractionType.Chop);

            SetGrowthProgress(0);

            UpdateSprite();
        }

        public override void CompleteInteraction(InteractionType interactionType)
        {
            switch (interactionType)
            {
                case InteractionType.Chop:
                    {
                        DestructBuilding();
                    }
                    break;
            }
        }

        public override void DestructBuilding()
        {
            base.DestructBuilding();

            DropLoot();
        }

        private void DropLoot()
        {
            Tile tile = GetApproachableTile();

            foreach (var kvp in treeData.GetGrowthStageLoot(_growthStage))
            {
                Item item = kvp.Key;
                int amount = kvp.Value;
                tile.Inventory.AddCargo(new ItemContainer(item, amount, item.Durability));
            }
        }

        public override string GetInformation()
        {
            var parentInformation = base.GetInformation();

            if (IsBuilt)
            {
                parentInformation += $"- {Localization.GetLocalizedText("growth_progress")}: {(int)(_growthProgress * 100)}%\n";
            }

            return parentInformation;
        }

        public override BuildingSaveData GetSaveData()
        {
            var parentSaveData = base.GetSaveData();

            // TODO: set save data

            return parentSaveData;
        }
    
        private void UpdateSprite()
        {
            Sprite.CurrentAnimation.Frames[0] = treeData.GetGrowthStageTexture(_growthStage,
                    GameplayScene.Instance.WorldState.CurrentSeason);
        }
    }
}
