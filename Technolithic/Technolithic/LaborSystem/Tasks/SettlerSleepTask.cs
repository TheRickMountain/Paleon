using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class SettlerSleepTask : Task
    {
        private Tile targetTile;

        private HutBuildingCmp hutBuildingCmp;

        public SettlerSleepTask(CreatureCmp creature) : base(creature)
        {
            
        }

        public override void Begin()
        {
            hutBuildingCmp = TryGetTargetHut();

            if(hutBuildingCmp != null)
            {
                hutBuildingCmp.AssignCreature(Owner);
                targetTile = hutBuildingCmp.GetReachableTile(Owner);
            }
            else
            {
                targetTile = Owner.Movement.CurrentTile;
            }
        }

        private HutBuildingCmp TryGetTargetHut()
        {
            if (Owner.AssignedHut != null)
            {
                if (Owner.AssignedHut.GetReachableTile(Owner) == null)
                {
                    return null;
                }
                else
                {
                    return Owner.AssignedHut;
                }
            }

            foreach (var hut in GameplayScene.WorldManager.HutBuildingsV2)
            {
                if(hut.HasFreeSlots() == false)
                {
                    continue;
                }

                if(hut.GetReachableTile(Owner) == null)
                {
                    continue;
                }

                return hut;
            }

            return null;
        }

        public override void BeforeUpdate()
        {
            Owner.Movement.SetPath(targetTile, false);
        }

        public override void UpdateTask()
        {
            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        if(hutBuildingCmp != null && hutBuildingCmp.IsBuilt == false)
                        {
                            hutBuildingCmp = null;
                        }

                        if(IsConditionMet())
                        {
                            if(hutBuildingCmp != null)
                            {
                                hutBuildingCmp.Exit(Owner);
                            }

                            Owner.StatusEffectsManager.RemoveStatusEffect(StatusEffectId.SleepingOnTheGround);

                            Owner.StatusEffectsManager.RemoveStatusEffect(StatusEffectId.Sleeping);

                            Owner.FallAsleep(false);

                            State = TaskState.Success;
                        }
                        else
                        {
                            if (Owner.EnteredBuilding == null && hutBuildingCmp != null)
                            {
                                hutBuildingCmp.Enter(Owner);
                            }

                            if(Owner.EnteredBuilding == null)
                            {
                                Owner.StatusEffectsManager.AddStatusEffect(StatusEffectId.SleepingOnTheGround);
                            }

                            Owner.StatusEffectsManager.AddStatusEffect(StatusEffectId.Sleeping);

                            Owner.FallAsleep(true);

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

            if(hutBuildingCmp != null)
            {
                hutBuildingCmp.Exit(Owner);
            }

            Owner.StatusEffectsManager.RemoveStatusEffect(StatusEffectId.SleepingOnTheGround);

            Owner.StatusEffectsManager.RemoveStatusEffect(StatusEffectId.Sleeping);

            Owner.FallAsleep(false);

            Owner.Movement.ResetPath();
        }
    }
}
