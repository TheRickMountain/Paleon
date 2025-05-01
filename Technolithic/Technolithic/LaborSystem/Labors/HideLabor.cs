using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class HideLabor : Labor
    {

        private CreatureCmp hunter;

        public HideLabor(CreatureCmp hunter) : base(LaborType.Hide)
        {
            this.hunter = hunter;
        }

        public override bool Check(CreatureCmp creature)
        {
            AddTask(creature, new HideTask(creature, hunter));

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            throw new NotImplementedException();
        }

    }
}
