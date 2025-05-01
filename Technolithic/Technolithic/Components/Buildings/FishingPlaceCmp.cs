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

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            Tile waterTile = GetWaterTile();

            if(waterTile != null)
            {
                coveredWaterChunk = waterTile.WaterChunk;
            }

            GameplayScene.WorldManager.FishingPlaceBuildings.Add(this);

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
        }

        public bool IsWaterChunkHasFish()
        {
            if(coveredWaterChunk != null)
            {
                return coveredWaterChunk.CurrentFishCount > 0;
            }

            return false;
        }

        public override void DestructBuilding()
        {
            base.DestructBuilding();

            GameplayScene.WorldManager.FishingPlaceBuildings.Remove(this);
        }

    }
}
