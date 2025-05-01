using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class TakeItemFromInventoryToSupplyTask : TakeItemFromInventoryTask
    {

        private BuildingCmp buildingToSupply;
        private Item itemToTake;

        public TakeItemFromInventoryToSupplyTask(CreatureCmp creature, Inventory inventoryToTake, Item itemToTake, int weightToTake,
            BuildingCmp buildingToSupply) : base(creature, inventoryToTake, itemToTake, weightToTake)
        {
            this.buildingToSupply = buildingToSupply;
            this.itemToTake = itemToTake;
        }

        public override void UpdateTask()
        {
            if (buildingToSupply.IsBuilt == false || buildingToSupply.IsFuelAllowed(itemToTake) == false
                           || buildingToSupply.GetCenterTile().Room.Id != Owner.Movement.CurrentTile.Room.Id)
            {
                State = TaskState.Failed;
                return;
            }

            base.UpdateTask();
        }

    }
}
