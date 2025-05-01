using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class PrepareCrafterTask : Task
    {
        private CrafterBuildingCmp targetCrafter;

        private float preparingTimer;
        private int prepTime = 2;

        public PrepareCrafterTask(CreatureCmp creatureCmp, CrafterBuildingCmp targetCrafter) : base(creatureCmp)
        {
            this.targetCrafter = targetCrafter;
        }

        public override void Begin()
        {
            targetCrafter.IsReserved = true;
        }

        public override void BeforeUpdate()
        {
            Tile targetTile = targetCrafter.GetReachableTile(Owner);
            Owner.Movement.SetPath(targetTile, false);

            Owner.Slider.SetValue(0, prepTime, 0, Color.Orange);
        }

        public override void UpdateTask()
        {
            if(targetCrafter.CanCraft == false)
            {
                State = TaskState.Failed;
                return;
            }

            switch(Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        preparingTimer += Engine.GameDeltaTime;
                        if(preparingTimer >= prepTime)
                        {
                            targetCrafter.IsPrepared = true;

                            targetCrafter.IsReserved = false;

                            Owner.Slider.Active = false;

                            State = TaskState.Success;
                            return;
                        }

                        Owner.Slider.SetValue(0, prepTime, preparingTimer, Color.Orange);
                        Owner.Slider.Active = true;
                        State = TaskState.Running;
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

            targetCrafter.IsReserved = false;

            Owner.Slider.Active = false;

            Owner.Movement.ResetPath();
        }
    }
}
