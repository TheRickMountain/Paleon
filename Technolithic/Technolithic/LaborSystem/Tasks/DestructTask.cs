using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class DestructTask : Task
    {
        private BuildingCmp targetBuilding;

        public DestructTask(CreatureCmp creature, BuildingCmp building) : base(creature)
        {
            this.targetBuilding = building;
        }

        public override void Begin()
        {
            targetBuilding.ReservedToDestruct = true;
        }

        public override void BeforeUpdate()
        {
            Tile tile = targetBuilding.GetReachableTile(Owner);
            Owner.Movement.SetPath(tile, false);
        }

        public override void UpdateTask()
        {
            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        if(targetBuilding.Destruct == false)
                        {
                            State = TaskState.Failed;
                            return;
                        }

                        Owner.Slider.Active = true;
                        targetBuilding.DeconstructionProgress += Engine.GameDeltaTime;

                        Owner.Slider.SetValue(0, targetBuilding.BuildingTemplate.ConstructionTime, targetBuilding.DeconstructionProgress, Color.Red);

                        if (targetBuilding.DeconstructionProgress >= targetBuilding.BuildingTemplate.ConstructionTime)
                        {
                            targetBuilding.DestructBuilding();
                            targetBuilding.ReservedToDestruct = false;

                            State = TaskState.Success;

                            Owner.Slider.Active = false;
                        }
                        else
                        {
                            State = TaskState.Running;
                        }
                    }
                    break;
                case MovementState.Failed:
                    {
                        Tile tile = targetBuilding.GetReachableTile(Owner);
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
                        if (targetBuilding.Destruct == false)
                        {
                            State = TaskState.Failed;
                            return;
                        }

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

            targetBuilding.ReservedToDestruct = false;
        }
    }
}
