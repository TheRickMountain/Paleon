using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class AttackLabor : Labor
    {

        public CreatureCmp TargetCreature { get; }

        public AttackLabor(CreatureCmp targetCreature) : base(LaborType.Hunt)
        {
            this.TargetCreature = targetCreature;
        }

        public override bool Check(CreatureCmp creature)
        {
            if (TargetCreature.IsDead)
                return false;

            if (TargetCreature.IsHidden)
                return false;

            if (creature.Movement.CurrentTile.GetRoomId() != TargetCreature.Movement.CurrentTile.GetRoomId())
                return false;

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            AttackTask huntTask = new AttackTask(creature, TargetCreature);
            AddTask(creature, huntTask);
        }
    }
}
