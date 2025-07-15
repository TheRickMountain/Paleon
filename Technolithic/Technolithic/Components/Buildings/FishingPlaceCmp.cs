using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class FishingPlaceCmp : BuildingCmp
    {

        private WaterChunk coveredWaterChunk;

        private static bool wasInformed = false;

        public FishingPlaceCmp(BuildingTemplate buildingTemplate, Direction direction) : base(buildingTemplate, direction)
        {

        }

        public override void UpdateCompleted()
        {
            base.UpdateCompleted();

            if(IsWaterChunkHasFish())
            {
                if(IsInteractionActivated(InteractionType.CatchFish) == false)
                {
                    ActivateInteraction(InteractionType.CatchFish);
                }
            }
            else
            {
                if(IsInteractionActivated(InteractionType.CatchFish))
                {
                    DeactivateInteraction(InteractionType.CatchFish);
                }
            }
        }

        public override void CompleteInteraction(InteractionType interactionType)
        {
            base.CompleteInteraction(interactionType);

            switch (interactionType)
            {
                case InteractionType.CatchFish:
                    {
                        CatchFish();

                        if (IsWaterChunkHasFish() == false)
                        {
                            DeactivateInteraction(InteractionType.CatchFish);
                        }
                    }
                    break;
            }
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            AddAvailableInteraction(InteractionType.CatchFish, LaborType.Fish, true);

            // TODO: загружать данные из json файла
            SetInteractionDuration(InteractionType.CatchFish, 125);

            MarkInteraction(InteractionType.CatchFish);

            Tile waterTile = GetWaterTile();

            if(waterTile != null)
            {
                coveredWaterChunk = waterTile.WaterChunk;
            }

            if(wasInformed == false)
            {
                GameplayScene.UIRootNodeScript?.NotificationsUI.GetComponent<NotificationsUIScript>()
                        .AddNotification(Localization.GetLocalizedText("to_do_the_job_of_x_you_need_tools", $"\"{Labor.GetLaborString(LaborType.Fish)}\""),
                        NotificationLevel.INFO, Entity);

                wasInformed = true;
            }
        }

        private Tile GetWaterTile()
        {
            foreach(var tileInfo in TilesInfosList)
            {
                if(tileInfo.GroundPattern == 'H')
                {
                    return tileInfo.Tile;
                }
            }

            return null;
        }

        public void CatchFish()
        {
            coveredWaterChunk.CatchFish();
            // TODO: загружать данные из json файла
            Item fishItem = ItemDatabase.GetItemByName("raw_fish");
            GetCenterTile().Inventory.AddCargo(new ItemContainer(fishItem, 1, fishItem.Durability));
        }

        public bool IsWaterChunkHasFish()
        {
            if(coveredWaterChunk != null)
            {
                return coveredWaterChunk.CurrentFishCount > 0;
            }

            return false;
        }

    }
}
