using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MineTask : ToolRequiredTask
    {

        private DepositCmp depositCmp;

        public MineTask(CreatureCmp creatureCmp, DepositCmp depositCmp) 
            : base(creatureCmp, depositCmp.BuildingTemplate.Deposit.RequiredToolType)
        {
            this.depositCmp = depositCmp;
        }

        public override void Begin()
        {
            depositCmp.IsReserved = true;
        }

        public override void BeforeUpdate()
        {
            base.BeforeUpdate();

            Owner.Movement.SetPath(depositCmp.GetApproachableTile(), false);

            Owner.Slider.SetValue(0, depositCmp.BuildingTemplate.Deposit.MiningTime, depositCmp.MiningCurrentTime, Color.Orange);
        }

        public override void UpdateTask()
        {
            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        Owner.Slider.SetValue(0, depositCmp.BuildingTemplate.Deposit.MiningTime, depositCmp.MiningCurrentTime, Color.Orange);

                        if (depositCmp.IsMarkedToObtain == false)
                        {
                            State = TaskState.Failed;
                            return;
                        }

                        float additionalBuff = Engine.GameDeltaTime * GetEfficiency();

                        float speed = Engine.GameDeltaTime + additionalBuff;

                        if (depositCmp.ProcessInteraction(speed))
                        {
                            depositCmp.IsReserved = false;

                            Owner.Slider.Active = false;
                            State = TaskState.Success;
                        }
                        else
                        {
                            Owner.Slider.Active = true;

                            State = TaskState.Running;
                        }
                    }
                    break;
                case MovementState.Failed:
                    {
                        Tile tile = depositCmp.GetApproachableTile(Owner);
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
                        if (depositCmp.IsMarkedToObtain == false)
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

            depositCmp.IsReserved = false;
        }
    }
}
