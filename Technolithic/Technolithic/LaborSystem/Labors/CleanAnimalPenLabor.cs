using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class CleanAnimalPenLabor : Labor
    {

        public CleanAnimalPenLabor() : base(LaborType.Ranching)
        {

        }

        public override bool Check(CreatureCmp creature)
        {
            if (GameplayScene.WorldManager.AnimalPenBuildings.Count == 0)
                return false;

            AnimalPenBuildingCmp targetAnimalPen = null;

            foreach (var animalPen in GameplayScene.WorldManager.AnimalPenBuildings)
            {
                if (animalPen.IsReserved)
                    continue;

                if (animalPen.IsFlaggedToCleanManure == false)
                    continue;

                if (animalPen.IsFullOfManure() == false)
                    continue;

                if (animalPen.GetReachableTile(creature) == null)
                    continue;

                targetAnimalPen = animalPen;
                break;
            }

            if (targetAnimalPen == null)
                return false;

            AddTask(creature, new CleanAnimalPenTask(creature, targetAnimalPen));

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
        }
    }
}
