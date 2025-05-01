using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AnimalEatLabor : Labor
    {

        public AnimalEatLabor() : base(LaborType.Eat)
        {

        }

        public override bool Check(CreatureCmp creature)
        {
            AnimalCmp animal = creature as AnimalCmp;

            if (animal.TargetAnimalPen == null)
                return false;

            if (animal.TargetAnimalPen.Inventory.GetAvailableItemCount(ItemDatabase.GetItemByName("hay")) <= 0)
                return false;

            if (animal.TargetAnimalPen.GetReachableTile(creature) == null)
                return false;

            AddTask(creature, new AnimalEatTask(creature, animal.TargetAnimalPen));

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
        }
    }
}