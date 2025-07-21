using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{

    public enum MyAction
    {
        Build,
        Uproot,
        Chop,
        AutoHarvest,
        Hunt,
        Slaughter,
        Cancel,
        Destruct,
        DestructSurface,
        DestructWall,
        CopySettings,
        Mine,
        GatherStone,
        GatherWood,
        BuildIrrigationCanal,
        DestructIrrigationCanal,
        None
    }

    public class ActionPanelUI : MNode
    {

        public ActionPanelUI(Scene scene) : base(scene)
        {
            Height = 48;
            
            AddComponent(new ActionPanelScript());
        }
    }
}
