using Microsoft.Xna.Framework;
using System;

namespace Technolithic
{
    public class EquipItemTask : Task
    {
        private Timer timer;

        private int processTime = 1;

        private Inventory inventory;

        private Item itemToEquip;
        private int weightToEquip = 1;

        private bool isItemWasTaken = false;

        public EquipItemTask(CreatureCmp creature, Inventory inventory, Item itemToEquip) : base(creature)
        {
            this.inventory = inventory;
            this.itemToEquip = itemToEquip;
        }

        public override void Begin()
        {
            inventory.OnItemRemovedCallback += OnInventoryItemRemoved;
            inventory.ReserveItemWeight(itemToEquip, weightToEquip);
        }

        public override void BeforeUpdate()
        {
            timer = new Timer();

            Tile tile = inventory.GetReachableTile(Owner);
            if (tile == null)
            {
                State = TaskState.Failed;
                return;
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
                        if (isItemWasTaken == false)
                        {
                            inventory.OnItemRemovedCallback -= OnInventoryItemRemoved;

                            // Добавляем предмет в инвентарь существа
                            Owner.Inventory.AddCargo(inventory.PopItem(itemToEquip, weightToEquip)[0]);

                            inventory.ReserveItemWeight(itemToEquip, -weightToEquip);

                            isItemWasTaken = true;
                        }
                        else
                        {
                            float currentTime = timer.GetTime();

                            Owner.Slider.SetValue(0, processTime, currentTime, Color.Orange);

                            if (currentTime > processTime)
                            {
                                Owner.Slider.Active = false;

                                ItemContainer oldEquipmentItemContainer = TryGetOldEquipmentItemContainer();

                                ItemContainer equipmentItemContainer = Owner.Inventory.PopItem(itemToEquip, weightToEquip)[0];

                                if (itemToEquip.Outfit != null)
                                {
                                    if (itemToEquip.Outfit.IsTop)
                                    {
                                        Owner.CreatureEquipment.TopClothingItemContainer = equipmentItemContainer;
                                    }
                                    else
                                    {
                                        Owner.CreatureEquipment.ClothingItemContainer = equipmentItemContainer;
                                    }
                                }
                                else if(itemToEquip.Tool != null)
                                {
                                    Owner.CreatureEquipment.EquipTool(equipmentItemContainer);
                                }

                                // Старую экипировку пробуем выложить либо на тайл, либо в склад
                                if(oldEquipmentItemContainer != null)
                                {
                                    // Предмет был взят со склада
                                    if(inventory.BuildingCmp != null)
                                    {
                                        StorageBuildingCmp storage = (StorageBuildingCmp)inventory.BuildingCmp;

                                        if (storage.EmptySpaceCount > 0 && storage.IsItemAllowed(oldEquipmentItemContainer.Item))
                                        {
                                            storage.Inventory.AddCargo(oldEquipmentItemContainer);
                                        }
                                        else
                                        {
                                            Owner.Movement.CurrentTile.Inventory.AddCargo(oldEquipmentItemContainer);
                                        }
                                    }
                                    else // Предмет был взят с тайла
                                    {
                                        Owner.Movement.CurrentTile.Inventory.AddCargo(oldEquipmentItemContainer);
                                    }
                                }

                                State = TaskState.Success;
                            }
                            else
                            {
                                Owner.Slider.Active = true;
                                State = TaskState.Running;
                            }
                        }
                    }
                    break;
                case MovementState.Failed:
                    {
                        if (inventory.Tile != null)
                        {
                            State = TaskState.Failed;
                        }
                        else if (inventory.BuildingCmp != null)
                        {
                            Tile tile = inventory.GetReachableTile(Owner);
                            if (tile != null)
                            {
                                Owner.Movement.SetPath(tile, false);
                            }
                            else
                            {
                                State = TaskState.Failed;
                            }
                        }
                    }
                    break;
                case MovementState.Running:
                    {
                        if (isItemWasTaken == false)
                        {
                            if (inventory.GetInventoryFactWeight(itemToEquip) <= 0)
                            {
                                State = TaskState.Failed;
                                return;
                            }
                        }

                        State = TaskState.Running;
                    }
                    break;
            }
        }

        private ItemContainer TryGetOldEquipmentItemContainer()
        {
            CreatureEquipment creatureEquipment = Owner.CreatureEquipment;

            if (itemToEquip.Outfit != null)
            {
                if (itemToEquip.Outfit.IsTop)
                {
                    return creatureEquipment.TopClothingItemContainer;
                }
                else
                {
                    return creatureEquipment.ClothingItemContainer;
                }
            }
            else if (itemToEquip.Tool != null)
            {
                return creatureEquipment.TryGetTool(itemToEquip.Tool.ToolType);
            }

            throw new NotImplementedException($"Item '{itemToEquip.Name}' is not equipment");
        }

        public override void Cancel()
        {
            base.Cancel();

            Owner.Movement.ResetPath();

            if (isItemWasTaken == false)
            {
                inventory.OnItemRemovedCallback -= OnInventoryItemRemoved;
                inventory.ReserveItemWeight(itemToEquip, -weightToEquip);
            }
        }

        private void OnInventoryItemRemoved(Inventory inventory, Item removedItem, int removedWeight)
        {
            if (itemToEquip == removedItem)
            {
                int availableWeight = inventory.GetAvailableItemCount(removedItem);
                if (availableWeight < 0)
                {
                    int weightToUnreserve = Math.Abs(availableWeight);
                    if (weightToUnreserve >= weightToEquip)
                    {
                        State = TaskState.Failed;
                    }
                    else
                    {
                        weightToEquip -= weightToUnreserve;
                        inventory.ReserveItemWeight(removedItem, -weightToUnreserve);
                    }
                }
            }
        }
    }
}
