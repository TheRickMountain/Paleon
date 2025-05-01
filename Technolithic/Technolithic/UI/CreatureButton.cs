using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Technolithic
{
    public class CreatureButton : MButtonUI
    {
        public CreatureCmp Creature { get; private set; }

        private MImageUI bodyIcon;
        private MImageUI hairIcon;
        private MyText creatureName;

        public CreatureButton(Scene scene) : base(scene)
        {
            Image.Texture = TextureBank.UITexture.GetSubtexture(192, 192, 24, 24);
            Image.ImageType = ImageType.Sliced;
            Image.BackgroundOverlap = 2;
            Image.SetBorder(8, 8, 8, 8);
            ButtonScript.SetDefaultColor(Color.White * 0.0f, Color.White, Color.White);

            bodyIcon = new MImageUI(scene);
            bodyIcon.Width = 48;
            bodyIcon.Height = 48;
            AddChildNode(bodyIcon);

            hairIcon = new MImageUI(scene);
            hairIcon.Width = 48;
            hairIcon.Height = 48;
            AddChildNode(hairIcon);

            creatureName = new MyText(scene);
            creatureName.Width = 100;
            creatureName.Height = 32;
            creatureName.X = bodyIcon.Width + 5;
            AddChildNode(creatureName);

            Width = 180;
            Height = 48;
        }

        public void SetCreature(CreatureCmp creature)
        {
            Creature = creature;

            bodyIcon.Image.Texture = creature.GetBodyIcon();

            hairIcon.Image.Texture = creature.GetHairIcon();
            hairIcon.Image.Color = creature.GetHairColor();

            creatureName.Text = creature.Name;
        }

    }
}
