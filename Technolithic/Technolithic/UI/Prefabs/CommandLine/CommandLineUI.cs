using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class CommandLineUI : MyPanelUI
    {
        public CommandLineUI(Scene scene) : base(scene, null, Color.White)
        {
            MTextInput textInput = new MTextInput(scene);
            textInput.Name = "TextInput";
            textInput.Width = Engine.Width - 40;
            textInput.Height = 40;

            Width = textInput.Width;
            Height = textInput.Height;

            AddChildNode(textInput);
            AddComponent(new CommandLineUIScript());
        }

    }
}
