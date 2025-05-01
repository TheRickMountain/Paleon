using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class SettlerSleepLabor : Labor
    {
    
        public SettlerSleepLabor() : base(LaborType.Sleep)
        {
            
        }

        public override bool Check(CreatureCmp creature)
        {
            if (CreatureTasks.Count > 0)
                return false;

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            SettlerSleepTask sleepTask = new SettlerSleepTask(creature);
            AddTask(creature, sleepTask);
        }

    }
}
