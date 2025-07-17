using Steamworks.Ugc;
using System;
using System.Collections.Generic;

namespace Technolithic
{
    public class InteractLabor : Labor
    {
        private InteractablesManager _interactablesManager;

        public InteractLabor(LaborType laborType, InteractablesManager interactablesManager) : base(laborType)
        {
            _interactablesManager = interactablesManager;
        }

        public override bool Check(CreatureCmp creature)
        {
            int creatureZoneId = creature.GetRoomId();

            var interactionPairs = _interactablesManager.GetInteractionPairs(creatureZoneId, LaborType);

            if (interactionPairs == null) return false;

            // TODO: выбрать наиближайшую сущность (по комнате) для взаимодействия
            // TODO: временно выбираем первую сущность из списка
            foreach ((Interactable interactable, InteractionType interactionType) in interactionPairs)
            {
                if (interactionType == InteractionType.Hunt)
                {
                    // TODO: нужно придумать как один раз вычислить наличие оружия у поселенца, чтобы не искать
                    // его для каждого взаимодействия (это относится и для поиска обычного взаимодействия)
                    bool hasAnyWeapon = false;

                    if (creature.CreatureEquipment.HasTool(ToolType.HuntingMelee) == false)
                    {
                        var tuplePair = GameplayScene.WorldManager.FindTool(creature, ToolType.HuntingMelee);

                        if (tuplePair?.Item1 != null)
                        {
                            Inventory inventory = tuplePair.Item1;
                            Item item = tuplePair.Item2;

                            EquipItemTask equipTask = new EquipItemTask(creature, inventory, item);
                            AddTask(creature, equipTask);

                            hasAnyWeapon = true;
                        }
                    }
                    else
                    {
                        hasAnyWeapon = true;
                    }

                    if (creature.CreatureEquipment.HasTool(ToolType.HuntingRange) == false)
                    {
                        var tuplePair = GameplayScene.WorldManager.FindTool(creature, ToolType.HuntingRange);

                        if (tuplePair?.Item1 != null)
                        {
                            Inventory inventory = tuplePair.Item1;
                            Item item = tuplePair.Item2;

                            EquipItemTask equipTask = new EquipItemTask(creature, inventory, item);
                            AddTask(creature, equipTask);

                            hasAnyWeapon = true;
                        }
                    }
                    else
                    {
                        hasAnyWeapon = true;
                    }

                    if (hasAnyWeapon == false)
                    {
                        continue;
                    }

                    SettlerHuntTask huntTask = new SettlerHuntTask(creature, interactable as AnimalCmp);
                    AddTask(creature, huntTask);
                    return true;
                }
                else
                {
                    Inventory inventoryWithTool = null;
                    Item requiredTool = null;

                    if (creature.CreatureEquipment.HasTool(interactionType) == false)
                    {
                        var invToolTuple = TryFindTool(creature, interactionType);
                        if (invToolTuple.Item1 != null)
                        {
                            inventoryWithTool = invToolTuple.Item1;
                            requiredTool = invToolTuple.Item2;
                        }
                        else
                        {
                            if (interactable.DoesInteractionRequireTool(interactionType))
                            {
                                continue;
                            }
                        }
                    }

                    Inventory inventoryWithItem = null;
                    Item requiredItem = null;

                    var invItemTuple = TryFindItem(creature, interactable.GetInteractionItems(interactionType));
                    if (invItemTuple.Item1 != null)
                    {
                        inventoryWithItem = invItemTuple.Item1;
                        requiredItem = invItemTuple.Item2;
                    }
                    else
                    {
                        if (interactable.DoesInteractionRequireItems(interactionType))
                        {
                            continue;
                        }
                    }

                    if (inventoryWithTool != null)
                    {
                        AddTask(creature, new EquipItemTask(creature, inventoryWithTool, requiredTool));
                    }

                    if (inventoryWithItem != null)
                    {
                        AddTask(creature, new TakeItemFromInventoryTask(creature, inventoryWithItem, requiredItem, 1));
                    }

                    InteractionData interactionData = Engine.InteractionsDatabase.GetInteractionData(interactionType);

                    InteractTask interactTask = new InteractTask(creature, interactable, interactionData);
                    AddTask(creature, interactTask);
                    return true;
                }
            }

            return false;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
        }

        private (Inventory, Item) TryFindTool(CreatureCmp creature, InteractionType interactionType)
        {
            return GameplayScene.WorldManager.FindTool(creature, interactionType);
        }

        private (Inventory, Item) TryFindItem(CreatureCmp creature,  IReadOnlyList<Item> items)
        {
            if (items == null) return (null, null);

            foreach (Item item in items)
            {
                Inventory inventory = GetStorageInventoryWithItem(item, creature);
                if (inventory == null)
                {
                    inventory = GetTileInventoryWithItem(item, creature);
                }

                if (inventory == null) continue;

                return (inventory, item);
            }

            return (null, null);
        }

        private Inventory GetStorageInventoryWithItem(Item item, CreatureCmp creatureCmp)
        {
            int zondId = creatureCmp.Movement.CurrentTile.GetRoomId();

            if (!GameplayScene.WorldManager.StoragesThatHaveItemsV2.ContainsKey(item))
                return null;

            if (GameplayScene.WorldManager.StoragesThatHaveItemsV2[item].Count == 0)
                return null;

            foreach (var inventory in GameplayScene.WorldManager.StoragesThatHaveItemsV2[item])
            {
                if (inventory.GetAvailableItemCount(item) > 0 && inventory.GetReachableTile(creatureCmp) != null)
                    return inventory;
            }

            return null;
        }

        private Inventory GetTileInventoryWithItem(Item item, CreatureCmp creatureCmp)
        {
            int zondId = creatureCmp.Movement.CurrentTile.GetRoomId();

            if (!GameplayScene.WorldManager.TilesThatHaveItems[zondId].ContainsKey(item))
                return null;

            if (GameplayScene.WorldManager.TilesThatHaveItems[zondId][item].Count == 0)
                return null;

            foreach (var inventory in GameplayScene.WorldManager.TilesThatHaveItems[zondId][item])
            {
                if (inventory.GetAvailableItemCount(item) > 0)
                    return inventory;
            }

            return null;
        }
    }
}
