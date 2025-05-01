using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public abstract class MComponent
    {

        public MNode ParentNode { get; set; }

        public bool Active { get; set; }
        public bool Visible { get; set; }

        public bool WasAwakened { get; set; }

        public MComponent(bool active, bool visible)
        {
            Active = active;
            Visible = visible;
        }

        public abstract void Awake();

        public abstract void Begin();

        public abstract void Update(int mouseX, int mouseY);

        public abstract void Render();


    }
}
