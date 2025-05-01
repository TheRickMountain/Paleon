using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class CreditsUIScript : MScript
    {
        private SmallButton closeButton;

        public CreditsUIScript() : base(false)
        {

        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            // Кнопка закрытия окна
            closeButton = new SmallButton(ParentNode.Scene, ResourceManager.CancelIcon);
            closeButton.X = ParentNode.Width - closeButton.Width;
            closeButton.Y = 0;
            closeButton.GetComponent<ButtonScript>().AddOnClickedCallback(Close);

            ParentNode.AddChildNode(closeButton);

            ListViewUIScript listView = ParentNode.GetChildByName("ListView").GetComponent<ListViewUIScript>();
            listView.AddItem(CreateText(" Developers:", Color.Yellow, true));
            listView.AddItem(CreateText("- Rinat Latyfullin", Color.White, false));
            listView.AddItem(CreateText(" Music:", Color.Yellow, true));
            listView.AddItem(CreateText("- From Doom To Serenity", Color.White, false));
            listView.AddItem(CreateText("- Georgiy Perevezentsev", Color.White, false));

            listView.AddItem(CreateText(" Sfx:", Color.Yellow, true));
            listView.AddItem(CreateText("- Leohpaz", Color.White, false));
            listView.AddItem(CreateText("- @seafoamselene", Color.White, false));
            listView.AddItem(CreateText("- Kenney", Color.White, false));

            listView.AddItem(CreateText(" Japanese language:", Color.Yellow, true));
            listView.AddItem(CreateText("- Allgames-kari (Alis)", Color.White, false));

            listView.AddItem(CreateText(" German language:", Color.Yellow, true));
            listView.AddItem(CreateText("- Melanie Müller", Color.White, false));

            listView.AddItem(CreateText(" French language:", Color.Yellow, true));
            listView.AddItem(CreateText("- Jessy Bernier", Color.White, false));
            listView.AddItem(CreateText("- CdriX (Cédric Gauché)", Color.White, false));

            listView.AddItem(CreateText(" Portuguese Brazilian language:", Color.Yellow, true));
            listView.AddItem(CreateText("- Thiago Mania", Color.White, false));

            listView.AddItem(CreateText(" Ukrainian language:", Color.Yellow, true));
            listView.AddItem(CreateText("- Toster Game", Color.White, false));

            listView.AddItem(CreateText(" Traditional Chinese:", Color.Yellow, true));
            listView.AddItem(CreateText("- EJane", Color.White, false));

            listView.AddItem(CreateText(" Simplified Chinese:", Color.Yellow, true));
            listView.AddItem(CreateText("- EJane", Color.White, false));

            listView.AddItem(CreateText(" Turkish:", Color.Yellow, true));
            listView.AddItem(CreateText("- :D", Color.White, false));

            listView.AddItem(CreateText(" Silver font:", Color.Yellow, true));
            listView.AddItem(CreateText("- Author: Poppy Works, Wolfgang Wozniak", Color.White, false));
            listView.AddItem(CreateText("- Major Contributors: Itou Hiro, leedheo ", Color.White, false));
            listView.AddItem(CreateText("- Minor Contributors: minipete", Color.White, false));
        }

        private MyText CreateText(string text, Color color, bool outlined)
        {
            MyText textUI = new MyText(ParentNode.Scene);
            textUI.Text = text;
            textUI.Color = color;
            textUI.Outlined = outlined;
            return textUI;
        }

        public override void Update(int mouseX, int mouseY)
        {
        }

        public void Close(bool value, ButtonScript buttonScript)
        {
            ParentNode.Active = false;
        }
    }
}
