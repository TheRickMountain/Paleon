using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class GameplayCamera : Entity
    {

        public GameplayCamera(bool ignoreUI)
        {
            Add(new CameraMovementScript(ignoreUI));
        }

    }
}
