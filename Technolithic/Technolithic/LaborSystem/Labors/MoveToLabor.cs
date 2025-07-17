using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MoveToLabor : Labor
    {
        public Tile TargetTile { get; private set; }

        public MoveToLabor(Tile targetTile) : base(LaborType.Waner)
        {
            TargetTile = targetTile;
        }

        public override bool Check(CreatureCmp creature)
        {
            if (TargetTile.Room == null)
                return false;

            if (creature.Movement.CurrentTile.GetZoneId() != TargetTile.GetZoneId())
                return false;

            if (creature.Movement.CurrentTile == TargetTile)
                return false;

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            MoveToTask moveToTask = new MoveToTask(creature, TargetTile);
            AddTask(creature, moveToTask);
        }
    }
}
