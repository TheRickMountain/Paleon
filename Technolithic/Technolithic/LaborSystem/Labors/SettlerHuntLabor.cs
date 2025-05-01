using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class SettlerHuntLabor : Labor
    {

        public SettlerHuntLabor() : base(LaborType.Hunt)
        {
        }

        public override bool Check(CreatureCmp creature)
        {
            if (creature.CreatureStats.Health.IsDissatisfied())
            {
                return false;
            }

            AnimalCmp targetAnimal = null;

            foreach (var animal in GameplayScene.WorldManager.AnimalsToHunt)
            {
                if (animal.IsReserved)
                    continue;

                if (animal.IsDead)
                    continue;

                if (creature.Movement.CurrentTile.GetRoomId() != animal.Movement.CurrentTile.GetRoomId())
                    continue;

                if(creature.IsHidden)
                    continue;

                targetAnimal = animal;
                break;
            }

            if (targetAnimal == null)
                return false;

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
                return false;
            }

            SettlerHuntTask huntTask = new SettlerHuntTask(creature, targetAnimal);
            AddTask(creature, huntTask);

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            throw new NotImplementedException();
        }

    }
}
