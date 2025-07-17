using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class HaulTask : ToolRequiredTask
    {

        private StorageBuildingCmp putToStorage;
        private Tile takeFromTile;
        private Item item;
        private int weight;


        private bool itemWasTaken = false;

        public HaulTask(CreatureCmp creature, StorageBuildingCmp putToStorage, Tile takeFromTile, Item item, int weight) 
            : base(creature, ToolType.Hauling)
        {
            putToStorage.Inventory.AddToAddWeight(item, weight);
            takeFromTile.Inventory.ReserveItemWeight(item, weight);

            this.putToStorage = putToStorage;
            this.takeFromTile = takeFromTile;
            this.item = item;
            this.weight = weight;    
        }


        public override void Begin()
        {
            takeFromTile.Inventory.OnItemRemovedCallback += OnTileInventoryItemRemoved;
            putToStorage.OnCapacityChangedCallback += CbOnStorageCapacityChanged;
        }

        public override void BeforeUpdate()
        {
            base.BeforeUpdate();

            bool adjacent = false;
            if (takeFromTile.Room == null)
                adjacent = true;

            Owner.Movement.SetPath(takeFromTile, adjacent);
        }

        public override void UpdateTask()
        {
            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        if(putToStorage.IsBuilt == false || putToStorage.Inventory.GetRoom().ZoneId != Owner.Movement.CurrentTile.Room.ZoneId
                            || putToStorage.IsItemAllowed(item) == false)
                        {
                            State = TaskState.Failed;
                            return;
                        }

                        if (itemWasTaken == false)
                        {
                            takeFromTile.Inventory.OnItemRemovedCallback -= OnTileInventoryItemRemoved;

                            foreach (var itemContainer in takeFromTile.Inventory.PopItem(item, weight))
                            {
                                Owner.Inventory.AddCargo(itemContainer);
                            }

                            takeFromTile.Inventory.ReserveItemWeight(item, -weight);

                            itemWasTaken = true;

                            Owner.Movement.SetPath(putToStorage.GetApproachableTile(Owner), true);
                        }
                        else
                        {
                            foreach (var itemContainer in Owner.Inventory.PopItem(item, weight))
                            {
                                putToStorage.Inventory.AddCargo(itemContainer);
                            }

                            putToStorage.Inventory.AddToAddWeight(item, -weight);

                            putToStorage.OnCapacityChangedCallback -= CbOnStorageCapacityChanged;

                            State = TaskState.Success;
                        }
                    }
                    break;
                case MovementState.Failed:
                    {
                        State = TaskState.Failed; 
                    }
                    break;
                case MovementState.Running:
                    {
                        if (putToStorage.IsBuilt == false || putToStorage.Inventory.GetRoom().ZoneId != Owner.Movement.CurrentTile.Room.ZoneId
                            || putToStorage.IsItemAllowed(item) == false)
                        {
                            State = TaskState.Failed;
                            return;
                        }

                        State = TaskState.Running;
                    }
                    break;
            }
        }

        public override void Cancel()
        {
            base.Cancel();

            putToStorage.OnCapacityChangedCallback -= CbOnStorageCapacityChanged;

            if (itemWasTaken == false)
            {
                takeFromTile.Inventory.OnItemRemovedCallback -= OnTileInventoryItemRemoved;
                takeFromTile.Inventory.ReserveItemWeight(item, -weight);
            }
            else
            {
                Owner.Inventory.ThrowCargo(Owner.Movement.CurrentTile);
            }

            putToStorage.Inventory.AddToAddWeight(item, -weight);

            Owner.Movement.ResetPath();
        }

        private void CbOnStorageCapacityChanged(StorageBuildingCmp storage)
        {
            int storageEmptySpaceCount = storage.EmptySpaceCount;

            if(storageEmptySpaceCount < 0)
            {
                int weightToThrow = Math.Abs(storageEmptySpaceCount);

                if(weight > weightToThrow)
                {
                    weight -= weightToThrow;

                    storage.Inventory.AddToAddWeight(item, -weightToThrow);

                    if(itemWasTaken)
                    {
                        Owner.Inventory.ThrowCargo(Owner.Movement.CurrentTile, item, weightToThrow);
                    }
                    else
                    {
                        takeFromTile.Inventory.ReserveItemWeight(item, -weightToThrow);
                    }
                }
                else
                {
                    State = TaskState.Failed;
                }
            }
        }

        private void OnTileInventoryItemRemoved(Inventory inventory, Item removedItem, int removedWeight)
        {
            if (item == removedItem)
            {
                int availableWeight = inventory.GetAvailableItemCount(removedItem);
                if (availableWeight < 0)
                {
                    int weightToUnreserve = Math.Abs(availableWeight);
                    if (weightToUnreserve >= weight)
                    {
                        State = TaskState.Failed;
                    }
                    else
                    {
                        weight -= weightToUnreserve;
                        inventory.ReserveItemWeight(removedItem, -weightToUnreserve);
                        putToStorage.Inventory.AddToAddWeight(item, -weightToUnreserve);
                    }
                }
            }
        }
    }

}
