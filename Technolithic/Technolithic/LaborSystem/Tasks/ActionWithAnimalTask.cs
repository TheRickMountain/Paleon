using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ActionWithAnimalTask : Task
    {

        private AnimalCmp animal;

        private Timer timer;

        private ActionWithAnimal actionWithAnimal;

        private Tile targetTile;

        public ActionWithAnimalTask(CreatureCmp creature, AnimalCmp animal, ActionWithAnimal actionWithAnimal) : base(creature)
        {
            this.animal = animal;
            this.actionWithAnimal = actionWithAnimal;
        }

        public override void Begin()
        {
            animal.IsReserved = true;
        }

        public override void BeforeUpdate()
        {
            if (animal.IsDead)
            {
                State = TaskState.Failed;
                return;
            }
            else
            {
                targetTile = animal.Movement.CurrentTile;
                Owner.Movement.SetPath(targetTile, true);

                timer = new Timer();
            }
        }

        public override void UpdateTask()
        {
            if(animal.IsDead)
            {
                State = TaskState.Failed;
                return;
            }

            if(actionWithAnimal == ActionWithAnimal.Domesticating && animal.Domesticate == false)
            {
                State = TaskState.Failed;
                return;
            }

            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        Owner.Slider.Active = true;

                        float currentTime = timer.GetTime();

                        Owner.Slider.SetValue(0, 2, currentTime, Color.Orange);

                        if (currentTime > 2)
                        {
                            timer.Reset();

                            switch (actionWithAnimal)
                            {
                                case ActionWithAnimal.Domesticating:
                                    {
                                        Owner.Inventory.ClearCargo();

                                        AnimalCmp tamedAnimal = animal.TryToDomesticate(Owner);

                                        if (tamedAnimal != null)
                                        {
                                            tamedAnimal.GatherProduct = true;
                                        }
                                    }
                                    break;
                                case ActionWithAnimal.GatherProduct:
                                    {
                                        Owner.Inventory.ClearCargo();

                                        animal.TryToGatherProduct();
                                    }
                                    break;
                                case ActionWithAnimal.Slaughtering:
                                    {
                                        animal.Die("");
                                    }
                                    break;
                            }

                            Owner.Slider.Active = false;
                            State = TaskState.Success;

                            animal.IsReserved = false;
                        }
                        else
                        {
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

        public override void Cancel()
        {
            base.Cancel();

            Owner.Inventory.ThrowCargo(Owner.Movement.CurrentTile);

            Owner.Movement.ResetPath();

            Owner.Slider.Active = false;

            animal.IsReserved = false;
        }
    }
}
