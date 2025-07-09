
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Technolithic
{
    public class CleanAnimalPenTask : Task
    {
        private AnimalPenBuildingCmp targetAnimalPen;
        private Timer timer;
        private int cleanTime = WorldState.MINUTES_PER_HOUR;

        public CleanAnimalPenTask(CreatureCmp creature, AnimalPenBuildingCmp targetAnimalPen) : base(creature)
        {
            this.targetAnimalPen = targetAnimalPen;
        }

        public override void Begin()
        {
            targetAnimalPen.IsReserved = true;
            timer = new Timer();
        }

        public override void BeforeUpdate()
        {
            Tile targetTile = targetAnimalPen.GetApproachableTile(Owner);

            Owner.Movement.SetPath(targetTile, false);
        }

        public override void UpdateTask()
        {
            if (targetAnimalPen.IsBuilt == false || targetAnimalPen.IsFlaggedToCleanManure == false)
            {
                State = TaskState.Failed;
                return;
            }

            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        

                        float currentTime = timer.GetTime();
                        if (currentTime >= cleanTime)
                        {
                            timer.Reset();

                            targetAnimalPen.CleanManure();

                            targetAnimalPen.IsReserved = false;

                            State = TaskState.Success;
                            Owner.Slider.Active = false;
                        }
                        else
                        {
                            Owner.Slider.Active = true;
                            Owner.Slider.SetValue(0, cleanTime, currentTime, Color.Orange);
                            State = TaskState.Running;
                        }
                    }
                    break;
                case MovementState.Failed:
                    {
                        Tile tile = targetAnimalPen.GetApproachableTile(Owner);
                        if (tile != null)
                        {
                            Owner.Movement.SetPath(tile, false);
                        }
                        else
                        {
                            State = TaskState.Failed;
                        }
                    }
                    break;
                case MovementState.Running:
                    {
                        if (targetAnimalPen.IsBuilt == false)
                        {
                            State = TaskState.Failed;
                        }
                        else
                        {
                            State = TaskState.Running;
                        }
                    }
                    break;
            }
        }

        public override void Cancel()
        {
            base.Cancel();

            Owner.Movement.ResetPath();

            Owner.Slider.Active = false;

            targetAnimalPen.IsReserved = false;
        }
    }
}
