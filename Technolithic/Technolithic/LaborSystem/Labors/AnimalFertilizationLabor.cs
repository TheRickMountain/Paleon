using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AnimalFertilizationLabor : Labor
    {

        public AnimalFertilizationLabor() : base(LaborType.Fertilization)
        {

        }

        public override bool Check(CreatureCmp creature)
        {
            AnimalCmp maleAnimal = creature as AnimalCmp;

            if (maleAnimal.IsReadyToFertilization == false)
                return false;

            if (maleAnimal.IsDomesticated == false)
                return false;

            if (maleAnimal.AnimalTemplate.Gender != Gender.M)
                return false;

            if (maleAnimal.AgeState != AgeState.Adult)
                return false;

            if (CreatureTasks.Count > 0)
                return false;

            AnimalCmp targetFemaleAnimal = null;

            foreach (var entity in GameplayScene.Instance.CreatureLayer.Entities)
            {
                AnimalCmp checkAnimal = entity.Get<AnimalCmp>();
                if (checkAnimal != null && checkAnimal.AnimalTemplate.Kind == maleAnimal.AnimalTemplate.Kind
                    && checkAnimal.IsDead == false && checkAnimal.IsDomesticated && checkAnimal.AgeState == AgeState.Adult
                    && checkAnimal.IsPregnant == false && checkAnimal.IsReserved == false
                    && checkAnimal.Movement.CurrentTile.GetRoomId() == maleAnimal.Movement.CurrentTile.GetRoomId()
                    && checkAnimal.AnimalTemplate.Gender == Gender.F
                    && checkAnimal.TargetAnimalPen == maleAnimal.TargetAnimalPen)
                {
                    targetFemaleAnimal = checkAnimal;
                    break;
                }
            }

            if (targetFemaleAnimal == null)
                return false;

            AnimalFertilizationTask animalFertilizationTask = new AnimalFertilizationTask(maleAnimal, targetFemaleAnimal);
            AddTask(maleAnimal, animalFertilizationTask);

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            throw new NotImplementedException();
        }
    }
}
