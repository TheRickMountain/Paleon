
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Technolithic
{
    public class EmptyFishTrapTask : Task
    {
        private FishTrap targetFishTrap;
        private Timer timer;

        public EmptyFishTrapTask(CreatureCmp creature, FishTrap targetFishTrap) : base(creature)
        {
            this.targetFishTrap = targetFishTrap;
        }

        public override void Begin()
        {
            targetFishTrap.IsReserved = true;
            timer = new Timer();
        }

        public override void BeforeUpdate()
        {
            Tile targetTile = targetFishTrap.GetApproachableTile(Owner);

            Owner.Movement.SetPath(targetTile, false);
        }

        public override void UpdateTask()
        {
            if (targetFishTrap.IsBuilt == false)
            {
                State = TaskState.Failed;
                return;
            }

            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        float currentTime = timer.GetTime();
                        if (currentTime >= 1.5f)
                        {
                            timer.Reset();

                            targetFishTrap.TryToEmpty(Owner);

                            targetFishTrap.IsReserved = false;

                            State = TaskState.Success;
                            Owner.Slider.Active = false;
                        }
                        else
                        {
                            Owner.Slider.Active = true;
                            Owner.Slider.SetValue(0, 1.5f, currentTime, Color.Orange);
                            State = TaskState.Running;
                        }
                    }
                    break;
                case MovementState.Failed:
                    {
                        Tile tile = targetFishTrap.GetApproachableTile(Owner);
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
                        if (targetFishTrap.IsBuilt == false)
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

            targetFishTrap.IsReserved = false;
        }
    }
}
