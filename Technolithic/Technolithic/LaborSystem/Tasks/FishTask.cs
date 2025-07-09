
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Technolithic
{
    public class FishTask : ToolRequiredTask
    {
        private FishingPlaceCmp targetFishingPlace;

        private Timer timer;
        private const int TRY_TIME = 125;

        public FishTask(CreatureCmp creature, FishingPlaceCmp targetFishingPlace) : 
            base(creature, ToolType.Fishing)
        {
            this.targetFishingPlace = targetFishingPlace;
        }

        public override void Begin()
        {
            targetFishingPlace.IsReserved = true;

            timer = new Timer();
        }

        public override void BeforeUpdate()
        {
            base.BeforeUpdate();

            Tile targetTile = targetFishingPlace.GetApproachableTile(Owner);

            if (targetTile == null)
            {
                State = TaskState.Failed;
                return;
            }

            Owner.Movement.SetPath(targetTile, false);
        }

        public override void UpdateTask()
        {
            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        if (targetFishingPlace.IsBuilt == false || targetFishingPlace.IsWaterChunkHasFish() == false)
                        {
                            State = TaskState.Failed;
                            return;
                        }

                        float currentTime = timer.GetTime();

                        float toolEfficiency = GetEfficiency() * 100;

                        if (currentTime >= TRY_TIME - toolEfficiency)
                        {
                            timer.Reset();

                            targetFishingPlace.CatchFish();

                            Item fishItem = ItemDatabase.GetItemByName("raw_fish");
                            Owner.Movement.CurrentTile.Inventory.AddCargo(new ItemContainer(fishItem, 1, fishItem.Durability));

                            targetFishingPlace.IsReserved = false;

                            State = TaskState.Success;
                            Owner.Slider.Active = false;
                        }
                        else
                        {
                            Owner.Slider.Active = true;
                            Owner.Slider.SetValue(0, TRY_TIME - toolEfficiency, currentTime, Color.Orange);
                            State = TaskState.Running;
                        }
                    }
                    break;
                case MovementState.Failed:
                    {
                        Tile tile = targetFishingPlace.GetApproachableTile(Owner);
                        if (tile != null)
                        {
                            Owner.Movement.SetPath(tile, false);
                        }
                        else
                        {
                            State = TaskState.Failed;
                        }
                    }
                    break;
                case MovementState.Running:
                    {
                        if (targetFishingPlace.IsBuilt == false || targetFishingPlace.IsWaterChunkHasFish() == false)
                        {
                            State = TaskState.Failed;
                        }
                        else
                        {
                            State = TaskState.Running;
                        }
                    }
                    break;
            }
        }

        public override void Cancel()
        {
            base.Cancel();

            Owner.Movement.ResetPath();

            Owner.Slider.Active = false;

            targetFishingPlace.IsReserved = false;
        }
    }
}
