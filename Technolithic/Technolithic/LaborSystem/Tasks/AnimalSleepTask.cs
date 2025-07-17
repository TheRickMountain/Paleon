using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AnimalSleepTask : Task
    {
        private Tile targetTile;

        public AnimalSleepTask(CreatureCmp creature) : base(creature)
        {

        }

        public override void Begin()
        {
            AnimalCmp animal = Owner as AnimalCmp;

            if (animal.IsDomesticated)
            {
                // Trying to find new animal pen
                if (animal.TargetAnimalPen == null || animal.TargetAnimalPen.GetApproachableTile(animal) == null)
                {
                    foreach (var animalPen in GameplayScene.WorldManager.AnimalPenBuildings)
                    {
                        if (animalPen.FreeSlots > 0 && animalPen.GetAnimalTemplateFilter(animal.AnimalTemplate)
                            && animalPen.GetApproachableTile(animal) != null)
                        {
                            if (animal.TargetAnimalPen != null)
                            {
                                animal.TargetAnimalPen.RemoveAnimal(animal);
                            }

                            animalPen.AddAnimal(animal);
                            break;
                        }
                    }
                }

                if (animal.TargetAnimalPen != null)
                {
                    Tile randomTile = animal.TargetAnimalPen.TilesInfosList[MyRandom.Range(animal.TargetAnimalPen.TilesInfosList.Count - 1)].Tile;
                    if (randomTile.Room != null && randomTile.Room.ZoneId == animal.Movement.CurrentTile.Room.ZoneId)
                    {
                        targetTile = randomTile;
                    }
                }
            }

            if (targetTile == null)
                targetTile = Owner.Movement.CurrentTile;
        }

        public override void BeforeUpdate()
        {
            Owner.Movement.SetPath(targetTile, false);
        }

        public override void UpdateTask()
        {
            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        if (IsConditionMet())
                        {
                            Owner.StatusEffectsManager.RemoveStatusEffect(StatusEffectId.Sleeping);

                            Owner.FallAsleep(false);

                            State = TaskState.Success;
                        }
                        else
                        {
                            Owner.StatusEffectsManager.AddStatusEffect(StatusEffectId.Sleeping);

                            Owner.FallAsleep(true);

                            State = TaskState.Running;
                        }
                    }
                    break;
                case MovementState.Failed:
                    {
                        State = TaskState.Failed;
                    }
                    break;
                case MovementState.Running:
                    {
                        State = TaskState.Running;
                    }
                    break;
            }
        }

        private bool IsConditionMet()
        {
            if (Owner.CreatureStats.Energy.IsSatisfied() == false)
                return false;

            if (Owner.CreatureStats.Health.IsSatisfied() == false)
                return false;

            return true;
        }

        public override void Cancel()
        {
            base.Cancel();

            Owner.StatusEffectsManager.RemoveStatusEffect(StatusEffectId.Sleeping);

            Owner.FallAsleep(false);

            Owner.Movement.ResetPath();
        }
    }
}
