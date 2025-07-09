using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class EmptyBeeHiveTask : Task
    {

        private BeeHiveBuildingCmp targetBeeHive;
        private Timer timer;

        public EmptyBeeHiveTask(CreatureCmp creature, BeeHiveBuildingCmp targetBeeHive) : base(creature)
        {
            this.targetBeeHive = targetBeeHive;
        }

        public override void Begin()
        {
            targetBeeHive.IsReserved = true;
            timer = new Timer();
        }

        public override void BeforeUpdate()
        {
            Tile targetTile = targetBeeHive.GetApproachableTile(Owner);

            Owner.Movement.SetPath(targetTile, false);
        }

        public override void UpdateTask()
        {
            if (targetBeeHive.IsBuilt == false || targetBeeHive.GatherResources == false)
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

                            targetBeeHive.TryToEmpty(Owner);

                            targetBeeHive.IsReserved = false;

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
                        Tile tile = targetBeeHive.GetApproachableTile(Owner);
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
                        State = TaskState.Running;
                    }
                    break;
            }
        }

        public override void Cancel()
        {
            base.Cancel();

            Owner.Movement.ResetPath();

            Owner.Slider.Active = false;

            targetBeeHive.IsReserved = false;
        }

    }
}
