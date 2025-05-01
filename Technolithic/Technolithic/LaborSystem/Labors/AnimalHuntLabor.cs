using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AnimalHuntLabor : Labor
    {

        public CreatureCmp TargetCreature { get; }

        public AnimalHuntLabor(CreatureCmp targetCreature) : base(LaborType.Hunt)
        {
            TargetCreature = targetCreature;
        }

        public override bool Check(CreatureCmp creature)
        {
            if (CreatureTasks.Count > 0)
                return false;

            if (TargetCreature.IsHidden)
                return false;

            // Проверяем, доступно ли существо
            if (creature.Movement.IsPathAvailable(TargetCreature.Movement.CurrentTile, true) == false)
                return false;

            AnimalHuntTask huntTask = new AnimalHuntTask(creature, TargetCreature);
            AddTask(creature, huntTask);

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            throw new NotImplementedException();
        }

    }
}
