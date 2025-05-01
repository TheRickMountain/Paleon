using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AnimalFertilizationTask : Task
    {

        private AnimalCmp targetFemaleAnimal;
        private Timer timer;

        public AnimalFertilizationTask(CreatureCmp creature, AnimalCmp targetFemaleAnimal) 
            : base(creature)
        {
            this.targetFemaleAnimal = targetFemaleAnimal;
        }

        public override void Begin()
        {
            targetFemaleAnimal.IsReserved = true;
            timer = new Timer();
        }

        public override void BeforeUpdate()
        {
            Owner.Movement.SetPath(targetFemaleAnimal.Movement.CurrentTile, true);
        }

        public override void UpdateTask()
        {
            if (targetFemaleAnimal.IsDead || targetFemaleAnimal.Movement.CurrentTile.GetRoomId() != targetFemaleAnimal.Movement.CurrentTile.GetRoomId())
            {
                State = TaskState.Failed;
                return;
            }

            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        float currentTime = timer.GetTime();
                        if (currentTime >= 1.0f)
                        {
                            timer.Reset();

                            targetFemaleAnimal.Fertilize(Owner as AnimalCmp);

                            targetFemaleAnimal.CreatureThoughts.AddThought("love", 5);
                            Owner.CreatureThoughts.AddThought("love", 5);

                            targetFemaleAnimal.IsReserved = false;

                            State = TaskState.Success;
                            Owner.Slider.Active = false;
                        }
                        else
                        {
                            Owner.Slider.Active = true;
                            Owner.Slider.SetValue(0, 1.0f, currentTime, Color.Pink);
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

            targetFemaleAnimal.IsReserved = false;
        }
    }
}
