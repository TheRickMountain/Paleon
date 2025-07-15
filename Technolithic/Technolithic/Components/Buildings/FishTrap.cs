using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class FishTrap : BuildingCmp
    {

        private List<WaterChunk> coveredWaterChunks;

        public Item CatchedItem { get; set; }

        public float CurrentTime { get; set; } = 0;

        // TODO: load from json
        private const float TRY_TIME = WorldState.HOURS_PER_CYCLE * WorldState.MINUTES_PER_HOUR;

        private Vector2 catchedItemDrawPosition;

        private Slider progressSlider;

        private const int MAX_NUMBER_OF_USES = 6;
        public int CurrentNumberOfUses { get; set; } = 6;

        public FishTrap(BuildingTemplate buildingTemplate, Direction direction) : base(buildingTemplate, direction)
        {

        }

        public override void UpdateCompleted()
        {
            base.UpdateCompleted();

            if(CatchedItem != null && IsInteractionActivated(InteractionType.EmptyFishTrap) == false)
            {
                ActivateInteraction(InteractionType.EmptyFishTrap);
            }

            if (CatchedItem == null)
            {
                CurrentTime += Engine.GameDeltaTime;
                if (CurrentTime >= TRY_TIME)
                {
                    CurrentTime = 0;

                    CatchedItem = GetRandomCatchedItem();
                }
            }
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            AddAvailableInteraction(InteractionType.EmptyFishTrap, LaborType.Fish, false);
            SetInteractionDuration(InteractionType.EmptyFishTrap, 5.0f);

            MarkInteraction(InteractionType.EmptyFishTrap);

            progressSlider = new Slider(BuildingTemplate.Width * Engine.TILE_SIZE, 4, Color.Black, Color.Orange);

            coveredWaterChunks = new List<WaterChunk>();

            foreach (var tileInfo in TilesInfosList)
            {
                if(tileInfo.GroundPattern == 'B')
                {
                    WaterChunk waterChunk = tileInfo.Tile.WaterChunk;

                    if (waterChunk != null)
                    {
                        if (coveredWaterChunks.Contains(waterChunk) == false)
                        {
                            coveredWaterChunks.Add(waterChunk);
                        }
                    }
                }
            }

            catchedItemDrawPosition = CenterPosition - new Vector2(0, 16);
        }

        public override void CompleteInteraction(InteractionType interactionType)
        {
            base.CompleteInteraction(interactionType);

            switch (interactionType)
            {
                case InteractionType.EmptyFishTrap:
                    {
                        ItemContainer itemContainer = new ItemContainer(CatchedItem, 1, CatchedItem.Durability);

                        GetCenterTile().Inventory.AddCargo(itemContainer);
                        
                        CatchedItem = null;
                        
                        CurrentNumberOfUses--;

                        if (CurrentNumberOfUses <= 0)
                        {
                            ThrowBuildingRecipeItemsAfterDestructing = false;

                            DestructBuilding();

                            GameplayScene.WorldManager.TryToBuild(BuildingTemplate, GetCenterTile().X, GetCenterTile().Y, Direction);
                        }
                        else
                        {
                            DeactivateInteraction(InteractionType.EmptyFishTrap);
                        }
                    }
                    break;
            }
        }

        // TODO: load from json
        private Item GetRandomCatchedItem()
        {
            if(MyRandom.ProbabilityChance(70))
            {
                foreach (var waterChunk in coveredWaterChunks)
                {
                    if (waterChunk.CurrentFishCount > 0)
                    {
                        waterChunk.CatchFish();

                        return ItemDatabase.GetItemByName("raw_fish");
                    }
                }
            }

            if(MyRandom.ProbabilityChance(50))
            {
                return ItemDatabase.GetItemByName("wood");
            }
            else
            {
                return ItemDatabase.GetItemByName("stone");
            }
        }

        public override void Render()
        {
            base.Render();

            if (IsBuilt)
            {
                if (CatchedItem != null)
                {
                    ResourceManager.CloudTexture.DrawCentered(catchedItemDrawPosition, Color.White * 0.75f);
                    CatchedItem.Icon.DrawCentered(catchedItemDrawPosition, Color.White * 0.75f);
                }
                else
                {
                    progressSlider.SetValue(0, TRY_TIME, CurrentTime, Color.Orange);
                    progressSlider.Position = new Vector2(Entity.Position.X, Entity.Position.Y - 5);
                    progressSlider.Render();
                }
            }
        }

        public override string GetInformation()
        {
            string baseInfo = base.GetInformation();

            if (IsBuilt)
            {
                baseInfo += $"- {Localization.GetLocalizedText("progress")}: {(int)CurrentTime}/{TRY_TIME}\n";
                baseInfo += $"- {Localization.GetLocalizedText("number_of_uses")}: {CurrentNumberOfUses}/{MAX_NUMBER_OF_USES}";
            }

            return baseInfo;
        }

        public override BuildingSaveData GetSaveData()
        {
            var saveData = base.GetSaveData();

            if (IsBuilt)
            {
                if (CatchedItem != null)
                {
                    saveData.FishTrapCatchedItemId = CatchedItem.Id;
                }
                else
                {
                    saveData.FishTrapCatchedItemId = -1;
                }
                saveData.FishTrapCurrentTime = CurrentTime;
                saveData.FishTrapCurrentNumberOfUses = CurrentNumberOfUses;
            }

            return saveData;
        }


    }
}
