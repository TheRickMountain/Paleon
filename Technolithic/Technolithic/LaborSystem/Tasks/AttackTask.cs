using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class AttackTask : Task
    {
        private CreatureCmp targetCreatureCmp;
        private Tile targetTile;

        public AttackTask(CreatureCmp creatureCmp, CreatureCmp targetCreatureCmp) : base(creatureCmp)
        {
            this.targetCreatureCmp = targetCreatureCmp;
        }

        public override void Begin()
        {
            
        }

        public override void BeforeUpdate()
        {
            targetTile = targetCreatureCmp.Movement.CurrentTile;
            Owner.Movement.SetPath(targetTile, true);
        }

        public override void UpdateTask()
        {
            if (targetCreatureCmp.IsHidden || targetCreatureCmp.IsDead)
            {
                State = TaskState.Success;
                return;
            }

            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        if (Owner.CreatureEquipment.HasTool(ToolType.HuntingRange))
                        {
                            int realDistance = Utils.GetDistance(Owner.Movement.CurrentTile, targetCreatureCmp.Movement.CurrentTile);
                            if(realDistance <= 15)
                            {
                                if (realDistance > 1)
                                {
                                    Owner.CreatureEquipment.ToolItemContainer = Owner.CreatureEquipment.TryGetTool(ToolType.HuntingRange);

                                    Owner.DoRangeAttack(targetCreatureCmp);
                                }
                                else
                                {
                                    Owner.CreatureEquipment.ToolItemContainer = Owner.CreatureEquipment.TryGetTool(ToolType.HuntingMelee);

                                    Owner.DoMeleeAttack(targetCreatureCmp);
                                }
                            }
                            else
                            {
                                targetTile = targetCreatureCmp.Movement.CurrentTile;
                                Owner.Movement.SetPath(targetTile, true);
                            }
                        }
                        else
                        {
                            if (targetCreatureCmp.Movement.CurrentTile != targetTile)
                            {
                                targetTile = targetCreatureCmp.Movement.CurrentTile;
                                Owner.Movement.SetPath(targetTile, true);
                            }
                            else
                            {
                                Owner.CreatureEquipment.ToolItemContainer = Owner.CreatureEquipment.TryGetTool(ToolType.HuntingMelee);

                                Owner.DoMeleeAttack(targetCreatureCmp);
                            }
                        }

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
                        if (Owner.CreatureEquipment.HasTool(ToolType.HuntingRange))
                        {
                            Tile targetCreatureTile = targetCreatureCmp.Movement.NextTile;

                            int realDistance = Utils.GetDistance(targetCreatureTile, Owner.Movement.CurrentTile);

                            if (realDistance <= 12)
                            {
                                Owner.Movement.ResetPath();
                            }
                        }
                        else
                        {
                            if (targetCreatureCmp.Movement.CurrentTile != targetTile)
                            {
                                targetTile = targetCreatureCmp.Movement.CurrentTile;
                                Owner.Movement.SetPath(targetTile, true);
                            }
                        }

                        State = TaskState.Running;
                    }
                    break;
            }
        }

        public override void Cancel()
        {
            base.Cancel();

            Owner.CreatureEquipment.ToolItemContainer = null;
        }

        public override void Complete()
        {
            base.Complete();

            Owner.CreatureEquipment.ToolItemContainer = null;
        }
    }
}
