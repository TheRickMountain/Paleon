using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{

    public class ActionPanelUI : MNode
    {

        public ActionPanelUI(Scene scene) : base(scene)
        {
            Height = 48;
            
            AddComponent(new ActionPanelScript());
        }
    }
}
