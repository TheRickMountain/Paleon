using Microsoft.Xna.Framework;
using System;

namespace Technolithic
{
    public class ConsumeTask : Task
    {
        private Timer timer;

        private int processTime = 2;

        private Inventory inventoryToTake;

        private Item itemToTake;
        private int weightToTake = 1;

        public ConsumeTask(CreatureCmp creature, Inventory inventoryToTake, Item itemToTake) : base(creature)
        {
            this.inventoryToTake = inventoryToTake;
            this.itemToTake = itemToTake;
        }

        public override void Begin()
        {
            inventoryToTake.OnItemRemovedCallback += OnInventoryItemRemoved;
            inventoryToTake.ReserveItemWeight(itemToTake, weightToTake);
        }

        public override void BeforeUpdate()
        {
            timer = new Timer();

            Tile tile = inventoryToTake.GetReachableTile(Owner);
            Owner.Movement.SetPath(tile, false);
        }

        public override void UpdateTask()
        {
            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        if (inventoryToTake != null)
                        {
                            inventoryToTake.OnItemRemovedCallback -= OnInventoryItemRemoved;

                            // Добавляем предмет в поселенца и удаляем контейнер
                            foreach (var itemContainer in inventoryToTake.PopItem(itemToTake, weightToTake))
                            {
                                Owner.Inventory.AddCargo(itemContainer);
                            }

                            inventoryToTake.ReserveItemWeight(itemToTake, -weightToTake);

                            inventoryToTake = null;
                        }
                        else
                        {
                            float currentTime = timer.GetTime();

                            Owner.Slider.SetValue(0, processTime, currentTime, Color.Orange);

                            if (currentTime > processTime)
                            {
                                Owner.Slider.Active = false;

                                Owner.CreatureStats.Consume(itemToTake);

                                Consumable consumable = itemToTake.Consumable;
                                if (consumable != null)
                                {
                                    if (string.IsNullOrEmpty(consumable.Container) == false)
                                    {
                                        Owner.Movement.CurrentTile.Inventory.AddCargo(ItemDatabase.GetItemByName(itemToTake.Consumable.Container), 1);
                                    }

                                    if(MyRandom.ProbabilityChance(consumable.StatusEffectChance))
                                    {
                                        Owner.StatusEffectsManager.AddStatusEffect(consumable.ApplyStatusEffect);
                                        Owner.StatusEffectsManager.RemoveStatusEffect(consumable.RemoveStatusEffect);
                                    }
                                }

                                Owner.Inventory.PopItem(itemToTake, weightToTake);

                                Owner.CargoImage.Scale = new Vector2(1.0f, 1.0f);

                                Owner.CargoImage.Y = 8;

                                State = TaskState.Success;
                            }
                            else
                            {
                                float cargoScale = (processTime - currentTime) / processTime;

                                Owner.CargoImage.Scale = new Vector2(cargoScale);

                                Owner.CargoImage.Y = 0;

                                Owner.Slider.Active = true;
                                State = TaskState.Running;
                            }
                        }
                    }
                    break;
                case MovementState.Failed:
                    {
                        if (inventoryToTake.Tile != null)
                        {
                            State = TaskState.Failed;
                        }
                        else if (inventoryToTake.BuildingCmp != null)
                        {
                            Tile tile = inventoryToTake.GetReachableTile(Owner);
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
                        if (inventoryToTake != null)
                        {
                            if (inventoryToTake.GetInventoryFactWeight(itemToTake) <= 0)
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

        public override void Cancel()
        {
            base.Cancel();

            Owner.Movement.ResetPath();

            if (inventoryToTake != null)
            {
                inventoryToTake.OnItemRemovedCallback -= OnInventoryItemRemoved;
                inventoryToTake.ReserveItemWeight(itemToTake, -weightToTake);
            }
            else
            {
                Owner.CargoImage.Scale = new Vector2(1.0f, 1.0f);
                Owner.CargoImage.Y = 8;

                Owner.Inventory.ThrowCargo(Owner.Movement.CurrentTile, itemToTake, weightToTake);

                Owner.Slider.Active = false;
            }
        }

        private void OnInventoryItemRemoved(Inventory inventory, Item removedItem, int removedWeight)
        {
            if (itemToTake == removedItem)
            {
                int availableWeight = inventory.GetAvailableItemCount(removedItem);
                if (availableWeight < 0)
                {
                    int weightToUnreserve = Math.Abs(availableWeight);
                    if (weightToUnreserve >= weightToTake)
                    {
                        State = TaskState.Failed;
                    }
                    else
                    {
                        weightToTake -= weightToUnreserve;
                        inventory.ReserveItemWeight(removedItem, -weightToUnreserve);
                    }
                }
            }
        }
    }
}
