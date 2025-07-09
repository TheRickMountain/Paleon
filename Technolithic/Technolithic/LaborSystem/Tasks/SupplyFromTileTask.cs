using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class SupplyFromTileTask : ToolRequiredTask
    {

        private Tile takeFromTile;
        private BuildingCmp putToBuilding;
        private Item itemToSupply;
        private int weightToSupply;

        private bool itemWasTaken = false;

        private bool cancelWasCalled = false;

        public SupplyFromTileTask(CreatureCmp creature, Tile takeFromTile, BuildingCmp putToBuilding, Item item, int weight) 
            : base(creature, ToolType.Hauling)
        {
            takeFromTile.Inventory.ReserveItemWeight(item, weight);
            putToBuilding.Inventory.AddToAddWeight(item, weight);

            this.takeFromTile = takeFromTile;
            this.putToBuilding = putToBuilding;
            itemToSupply = item;
            weightToSupply = weight;

        }

        public override void Begin()
        {
            takeFromTile.Inventory.OnItemRemovedCallback += OnTileInventoryItemRemoved;
            putToBuilding.Inventory.OnRequiredItemWeightChanged += OnPutToBuildingRequiredWeightChanged;
            putToBuilding.OnBuildingDestructedCallback += OnBuildingDeleted;
            putToBuilding.OnBuildingCanceledCallback += OnBuildingDeleted;
        }

        public override void BeforeUpdate()
        {
            base.BeforeUpdate();

            Owner.Movement.SetPath(takeFromTile, false);
        }

        public override void UpdateTask()
        {
            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        if(putToBuilding.GetCenterTile().Room.Id != Owner.Movement.CurrentTile.Room.Id)
                        {
                            State = TaskState.Failed;
                            CustomCancel();
                            return;
                        }

                        if (itemWasTaken == false)
                        {
                            takeFromTile.Inventory.OnItemRemovedCallback -= OnTileInventoryItemRemoved;

                            foreach (var itemContainer in takeFromTile.Inventory.PopItem(itemToSupply, weightToSupply))
                            {
                                Owner.Inventory.AddCargo(itemContainer);
                            }

                            takeFromTile.Inventory.ReserveItemWeight(itemToSupply, -weightToSupply);

                            itemWasTaken = true;

                            Owner.Movement.SetPath(putToBuilding.GetApproachableTile(Owner), false);
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
                        if (itemWasTaken)
                        {
                            Tile tile = putToBuilding.GetApproachableTile(Owner);
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
                            State = TaskState.Failed;
                            CustomCancel();
                        }
                    }
                    break;
                case MovementState.Running:
                    {
                        if (putToBuilding.GetCenterTile().Room.Id != Owner.Movement.CurrentTile.Room.Id)
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

        private void OnBuildingDeleted(BuildingCmp buildingCmp)
        {
            State = TaskState.Failed;
            CustomCancel();
        }

        private void OnPutToBuildingRequiredWeightChanged(Inventory inventory, Item item, int weight)
        {
            int requiredWeight = inventory.GetInventoryRequiredMinusToAddWeight(itemToSupply);
            if (requiredWeight < 0)
            {
                int unrequiredWeight = Math.Abs(requiredWeight);
                if (weightToSupply <= unrequiredWeight)
                {
                    State = TaskState.Failed;
                    CustomCancel();
                }
                else
                {
                    weightToSupply -= unrequiredWeight;

                    if (itemWasTaken == false)
                    {
                        takeFromTile.Inventory.ReserveItemWeight(itemToSupply, -unrequiredWeight);
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

                putToBuilding.Inventory.OnRequiredItemWeightChanged -= OnPutToBuildingRequiredWeightChanged;
                putToBuilding.OnBuildingDestructedCallback -= OnBuildingDeleted;
                putToBuilding.OnBuildingCanceledCallback -= OnBuildingDeleted;

                if (itemWasTaken == false)
                {
                    takeFromTile.Inventory.OnItemRemovedCallback -= OnTileInventoryItemRemoved;
                    takeFromTile.Inventory.ReserveItemWeight(itemToSupply, -weightToSupply);
                }
                else
                {
                    Owner.Inventory.ThrowCargo(Owner.Movement.CurrentTile, itemToSupply, weightToSupply);
                }

                putToBuilding.Inventory.AddToAddWeight(itemToSupply, -weightToSupply);
            }
        }

        private void OnTileInventoryItemRemoved(Inventory inventory, Item removedItem, int removedWeight)
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

        public override void Cancel()
        {
            base.Cancel();

            CustomCancel();
        }
    }

}
