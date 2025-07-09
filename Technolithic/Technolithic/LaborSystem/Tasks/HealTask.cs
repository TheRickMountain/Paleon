using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class HealTask : Task
    {
        private HutBuildingCmp hutBuildingCmp;

        public HealTask(CreatureCmp creature, HutBuildingCmp targetHut) : base(creature)
        {
            hutBuildingCmp = targetHut;
        }

        public override void Begin()
        {
            hutBuildingCmp.AssignCreature(Owner);
        }

        public override void BeforeUpdate()
        {
            Tile targetTile = hutBuildingCmp.GetApproachableTile(Owner);
            if (targetTile == null)
            {
                State = TaskState.Failed;
            }
            else
            {
                Owner.Movement.SetPath(targetTile, false);
            }
        }

        public override void UpdateTask()
        {
            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        if (IsConditionMet())
                        {
                            hutBuildingCmp.Exit(Owner);

                            Owner.StatusEffectsManager.RemoveStatusEffect(StatusEffectId.Sleeping);

                            Owner.FallAsleep(false);

                            State = TaskState.Success;
                        }
                        else
                        {
                            if (Owner.EnteredBuilding == null)
                            {
                                hutBuildingCmp.Enter(Owner);
                            }

                            Owner.StatusEffectsManager.AddStatusEffect(StatusEffectId.Sleeping);

                            Owner.FallAsleep(true);

                            State = TaskState.Running;
                        }
                    }
                    break;
                case MovementState.Failed:
                    {
                        Tile tile = hutBuildingCmp.GetApproachableTile(Owner);
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

        private bool IsConditionMet()
        {
            if (Owner.CreatureStats.Energy.IsSatisfied() == false)
                return false;

            if (Owner.CreatureStats.Health.IsSatisfied() == false)
                return false;

            if (hutBuildingCmp != null && hutBuildingCmp.CurrentFuelCondition > 0
                       && Owner.CreatureStats.Temperature.IsSatisfied() == false)
            {
                return false;
            }

            return true;
        }

        public override void Cancel()
        {
            base.Cancel();

            hutBuildingCmp.Exit(Owner);

            Owner.StatusEffectsManager.RemoveStatusEffect(StatusEffectId.Sleeping);

            Owner.FallAsleep(false);

            Owner.Movement.ResetPath();
        }
    }
}
