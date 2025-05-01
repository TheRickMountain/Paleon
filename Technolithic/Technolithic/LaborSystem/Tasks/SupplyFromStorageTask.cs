using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class SupplyFromStorageTask : ToolRequiredTask
    {

        private StorageBuildingCmp takeFromStorage;
        private BuildingCmp putToBuilding;
        private Item itemToSupply;
        private int weightToSupply;

        private bool itemWasTaken = false;

        private bool cancelWasCalled = false;

        public SupplyFromStorageTask(CreatureCmp creature, StorageBuildingCmp takeFromStorage, BuildingCmp putToBuilding, Item item, int weight) 
            : base(creature, ToolType.Hauling)
        {
            takeFromStorage.Inventory.ReserveItemWeight(item, weight);
            putToBuilding.Inventory.AddToAddWeight(item, weight);

            this.takeFromStorage = takeFromStorage;
            this.putToBuilding = putToBuilding;
            this.itemToSupply = item;
            this.weightToSupply = weight;

        }

        public override void Begin()
        {
            takeFromStorage.Inventory.OnItemRemovedCallback += OnStorageInventoryItemRemoved;
            takeFromStorage.OnBuildingDestructedCallback += OnTakeFromStorageDeletedCallback;

            putToBuilding.Inventory.OnRequiredItemWeightChanged += OnPutToBuildingRequiredWeightChanged;
            putToBuilding.OnBuildingDestructedCallback += OnBuildingDeleted;
            putToBuilding.OnBuildingCanceledCallback += OnBuildingDeleted;
        }

        public override void BeforeUpdate()
        {
            base.BeforeUpdate();

            Tile tile = takeFromStorage.GetReachableTile(Owner);

            if (tile == null)
            {
                State = TaskState.Failed;
                CustomCancel();
            }
            else
            {
                Owner.Movement.SetPath(tile, false);
            }
        }

        public override void UpdateTask()
        {
            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        if (putToBuilding.GetReachableTile(Owner) == null)
                        {
                            State = TaskState.Failed;
                            CustomCancel();
                            return;
                        }

                        if (itemWasTaken == false)
                        {
                            takeFromStorage.Inventory.OnItemRemovedCallback -= OnStorageInventoryItemRemoved;

                            foreach (var itemContainer in takeFromStorage.Inventory.PopItem(itemToSupply, weightToSupply))
                            {
                                Owner.Inventory.AddCargo(itemContainer);
                            }

                            takeFromStorage.Inventory.ReserveItemWeight(itemToSupply, -weightToSupply);

                            itemWasTaken = true;

                            Owner.Movement.SetPath(putToBuilding.GetReachableTile(Owner), false);

                            takeFromStorage.OnBuildingDestructedCallback -= OnTakeFromStorageDeletedCallback;
                        }
                        else
                        {
                            putToBuilding.Inventory.OnRequiredItemWeightChanged -= OnPutToBuildingRequiredWeightChanged;
                            putToBuilding.OnBuildingDestructedCallback -= OnBuildingDeleted;
                            putToBuilding.OnBuildingCanceledCallback -= OnBuildingDeleted;

                            putToBuilding.Inventory.AddToAddWeight(itemToSupply, -weightToSupply);

                            foreach (var itemContainer in Owner.Inventory.PopItem(itemToSupply, weightToSupply))
                            {
                                putToBuilding.Inventory.AddCargo(itemContainer);
                            }

                            State = TaskState.Success;
                        }
                    }
                    break;
                case MovementState.Failed:
                    {
                        if(itemWasTaken)
                        {
                            Tile tile = putToBuilding.GetReachableTile(Owner);
                            if (tile != null)
                            {
                                Owner.Movement.SetPath(tile, false);
                            }
                            else
                            {
                                State = TaskState.Failed;
                                CustomCancel();
                            }
                        }
                        else
                        {
                            Tile tile = takeFromStorage.GetReachableTile(Owner);
                            if (tile != null)
                            {
                                Owner.Movement.SetPath(tile, false);
                            }
                            else
                            {
                                State = TaskState.Failed;
                                CustomCancel();
                            }
                        }
                    }
                    break;
                case MovementState.Running:
                    {
                        if (putToBuilding.GetReachableTile(Owner) == null)
                        {
                            State = TaskState.Failed;
                            CustomCancel();
                            return;
                        }

                        State = TaskState.Running;
                    }
                    break;
            }
        }

        private void OnTakeFromStorageDeletedCallback(BuildingCmp buildingCmp)
        {
            if(itemWasTaken == false)
            {
                State = TaskState.Failed;
                CustomCancel();
            }
        }

        private void OnPutToBuildingRequiredWeightChanged(Inventory inventory, Item item, int weight)
        {
            int requiredWeight = inventory.GetInventoryRequiredMinusToAddWeight(itemToSupply);
            if(requiredWeight < 0)
            {
                int unrequiredWeight = Math.Abs(requiredWeight);
                if(weightToSupply <= unrequiredWeight)
                {
                    State = TaskState.Failed;
                    CustomCancel();
                }
                else
                {
                    weightToSupply -= unrequiredWeight;

                    if(itemWasTaken == false)
                    {
                        takeFromStorage.Inventory.ReserveItemWeight(itemToSupply, -unrequiredWeight);
                    }
                    else
                    {
                        Owner.Inventory.ThrowCargo(Owner.Movement.CurrentTile, itemToSupply, unrequiredWeight);
                    }

                    putToBuilding.Inventory.AddToAddWeight(itemToSupply, -unrequiredWeight);
                }
            }
        }

        private void CustomCancel()
        {
            if (!cancelWasCalled)
            {
                cancelWasCalled = true;

                Owner.Movement.ResetPath();

                takeFromStorage.OnBuildingDestructedCallback -= OnTakeFromStorageDeletedCallback;
               
                putToBuilding.Inventory.OnRequiredItemWeightChanged -= OnPutToBuildingRequiredWeightChanged;
                putToBuilding.OnBuildingDestructedCallback -= OnBuildingDeleted;
                putToBuilding.OnBuildingCanceledCallback -= OnBuildingDeleted;

                if (itemWasTaken == false)
                {
                    takeFromStorage.Inventory.OnItemRemovedCallback -= OnStorageInventoryItemRemoved;
                    takeFromStorage.Inventory.ReserveItemWeight(itemToSupply, -weightToSupply);
                }
                else
                {
                    Owner.Inventory.ThrowCargo(Owner.Movement.CurrentTile, itemToSupply, weightToSupply);
                }

                putToBuilding.Inventory.AddToAddWeight(itemToSupply, -weightToSupply);
            }
        }

        private void OnBuildingDeleted(BuildingCmp buildingCmp)
        {
            State = TaskState.Failed;
            CustomCancel();
        }

        public override void Cancel()
        {
            base.Cancel();

            CustomCancel();
        }

        private void OnStorageInventoryItemRemoved(Inventory inventory, Item removedItem, int removedWeight)
        {
            if (itemToSupply == removedItem)
            {
                int availableWeight = inventory.GetAvailableItemCount(removedItem);
                if (availableWeight < 0)
                {
                    int weightToUnreserve = Math.Abs(availableWeight);
                    if (weightToUnreserve >= weightToSupply)
                    {
                        State = TaskState.Failed;
                    }
                    else
                    {
                        weightToSupply -= weightToUnreserve;
                        inventory.ReserveItemWeight(removedItem, -weightToUnreserve);
                        putToBuilding.Inventory.AddToAddWeight(itemToSupply, -weightToUnreserve);
                    }
                }
            }
        }
    }

}
