using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public abstract class MScript : MComponent
    {

        public MScript(bool active) : base(active, false)
        {
            Active = active;
        }

        public override void Render()
        {
        }

    }
}
