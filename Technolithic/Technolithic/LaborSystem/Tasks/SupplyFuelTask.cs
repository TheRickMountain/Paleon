using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class SupplyFuelTask : ToolRequiredTask
    {

        private BuildingCmp buildingToSupply;
        private Item itemToTake;
        private int weightToTake;

        public SupplyFuelTask(CreatureCmp creatureCmp, BuildingCmp buildingToSupply, Item itemToTake) 
            : base(creatureCmp, ToolType.Hauling)
        {
            this.buildingToSupply = buildingToSupply;
            this.itemToTake = itemToTake;
            weightToTake = 1;
        }

        public override void Begin()
        {
            buildingToSupply.ReservedToSupplyFuel = true;
        }

        public override void BeforeUpdate()
        {
            base.BeforeUpdate();

            Tile tile = buildingToSupply.GetApproachableTile(Owner);

            if (tile == null)
            {
                State = TaskState.Failed;
            }
            else
            {
                Owner.Movement.SetPath(tile, false);
            }
        }

        public override void UpdateTask()
        {
            if (buildingToSupply.IsBuilt == false || buildingToSupply.IsFuelAllowed(itemToTake) == false
                           || buildingToSupply.GetCenterTile().Room.ZoneId != Owner.Movement.CurrentTile.Room.ZoneId)
            {
                State = TaskState.Failed;
                return;
            }

            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        Owner.Inventory.PopItem(itemToTake, weightToTake);
                        buildingToSupply.SupplyFuel(itemToTake);

                        buildingToSupply.ReservedToSupplyFuel = false;

                        State = TaskState.Success;
                    }
                    break;
                case MovementState.Failed:
                    {
                        Tile tile = buildingToSupply.GetApproachableTile(Owner);
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
                        State = TaskState.Running;
                    }
                    break;
            }
        }

        public override void Cancel()
        {
            base.Cancel();

            Owner.Movement.ResetPath();

            buildingToSupply.ReservedToSupplyFuel = false;

            if (Owner.Inventory.GetAvailableItemCount(itemToTake) > 0)
            {
                Owner.Inventory.ThrowCargo(Owner.Movement.CurrentTile, itemToTake, weightToTake);
            }
        }
    }
}
