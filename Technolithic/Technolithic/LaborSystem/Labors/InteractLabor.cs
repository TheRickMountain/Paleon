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
                if (creature.CreatureEquipment.HasTool(interactionType) == false)
                {
                    var tuplePair = GameplayScene.WorldManager.FindTool(creature, interactionType);

                    if (tuplePair.Item1 != null)
                    {
                        Inventory inventory = tuplePair.Item1;
                        Item item = tuplePair.Item2;

                        EquipItemTask equipTask = new EquipItemTask(creature, inventory, item);
                        AddTask(creature, equipTask);
                    }
                    else if (interactable.DoesInteractionRequireTool(interactionType))
                    {
                        continue;
                    }
                }

                InteractionData interactionData = Engine.InteractionsDatabase.GetInteractionData(interactionType);

                InteractTask interactTask = new InteractTask(creature, interactable, interactionData);
                AddTask(creature, interactTask);
                return true;
            }

            return false;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
        }
    }
}
