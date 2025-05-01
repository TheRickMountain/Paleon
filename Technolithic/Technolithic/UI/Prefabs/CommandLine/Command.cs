using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class Command : Attribute
    {

        public string Name { get; private set; }

        public Command(string name)
        {
            Name = name;
        }

    }
}
