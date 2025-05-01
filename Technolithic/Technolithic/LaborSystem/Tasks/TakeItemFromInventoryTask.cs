using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class TakeItemFromInventoryTask : ToolRequiredTask
    {

        private Inventory inventoryToTake;
        private Item itemToTake;
        private int itemWeightToTake;

        public TakeItemFromInventoryTask(CreatureCmp creature, Inventory inventoryToTake, Item itemToTake, int itemWeightToTake) 
            : base(creature, ToolType.Hauling)
        {
            this.inventoryToTake = inventoryToTake;
            this.itemToTake = itemToTake;
            this.itemWeightToTake = itemWeightToTake;
        }

        public override void Begin()
        {
            inventoryToTake.OnItemRemovedCallback += OnInventoryItemRemoved;
            inventoryToTake.ReserveItemWeight(itemToTake, itemWeightToTake);
        }

        public override void BeforeUpdate()
        {
            base.BeforeUpdate();

            Tile tile = inventoryToTake.GetReachableTile(Owner);

            if(tile == null)
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
            Tile tile = inventoryToTake.GetReachableTile(Owner);

            if(tile == null)
            {
                State = TaskState.Failed;
                return;
            }

            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        inventoryToTake.OnItemRemovedCallback -= OnInventoryItemRemoved;

                        foreach (var itemContainer in inventoryToTake.PopItem(itemToTake, itemWeightToTake))
                        {
                            Owner.Inventory.AddCargo(itemContainer);
                        }

                        inventoryToTake.ReserveItemWeight(itemToTake, -itemWeightToTake);

                        State = TaskState.Success;
                    }
                    break;
                case MovementState.Failed:
                    {
                        State = TaskState.Failed;
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

            inventoryToTake.OnItemRemovedCallback -= OnInventoryItemRemoved;
            inventoryToTake.ReserveItemWeight(itemToTake, -itemWeightToTake);
        }

        private void OnInventoryItemRemoved(Inventory inventory, Item removedItem, int removedWeight)
        {
            if (itemToTake == removedItem)
            {
                int availableWeight = inventoryToTake.GetAvailableItemCount(removedItem);
                if (availableWeight < 0)
                {
                    int weightToUnreserve = Math.Abs(availableWeight);
                    if (weightToUnreserve >= itemWeightToTake)
                    {
                        State = TaskState.Failed;
                    }
                    else
                    {
                        itemWeightToTake -= weightToUnreserve;
                        inventoryToTake.ReserveItemWeight(removedItem, -weightToUnreserve);
                    }
                }
            }
        }

    }
}
