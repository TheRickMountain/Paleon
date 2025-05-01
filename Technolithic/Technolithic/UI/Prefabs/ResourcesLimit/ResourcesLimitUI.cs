using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ResourcesLimitUI : MyPanelUI
    {

        public ResourcesLimitUI(Scene scene) : base(scene, Localization.GetLocalizedText("production_limit"), Color.White)
        {
            ListViewUI listView = new ListViewUI(scene, 400, 32, 9);
            listView.Name = "ListView";
            listView.X = 8;
            listView.Y = 8 + 32;

            Width = listView.Width + 16;
            Height = listView.Height + 88;

            AddChildNode(listView);

            MButtonUI subButton = CreateAndAddButton(scene, "SubButton", TextureBank.UITexture.GetSubtexture(32, 32, 8, 8), 8, Height - 8 - 32);

            MButtonUI addButton = CreateAndAddButton(scene, "AddButton", TextureBank.UITexture.GetSubtexture(40, 32, 8, 8),
                subButton.LocalX + subButton.Width + 5, Height - 8 - 32);

            MButtonUI produceEndleslyButton = CreateAndAddButton(scene, "ProduceEndleslyButton", TextureBank.UITexture.GetSubtexture(32, 40, 8, 8),
                addButton.LocalX + addButton.Width + 5, Height - 8 - addButton.Height);

            AddComponent(new ResourcesLimitUIScript());
        }

        private MButtonUI CreateAndAddButton(Scene scene, string name, MyTexture icon, int x, int y)
        {
            MButtonUI button = new MButtonUI(scene);
            button.Image.Texture = TextureBank.UITexture.GetSubtexture(16, 48, 16, 16);

            MImageUI buttonSprite = new MImageUI(scene);
            buttonSprite.Image.Texture = icon;
            buttonSprite.X = 8;
            buttonSprite.Y = 8;
            buttonSprite.Width = 16;
            buttonSprite.Height = 16;

            button.AddChildNode(buttonSprite);
            button.Width = 32;
            button.Height = 32;
            button.X = x;
            button.Y = y;
            button.Name = name;
            AddChildNode(button);

            return button;
        }

    }
}
