using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic.UI.Prefabs.BuildingRecipe
{
    public class BuildRecipeUI : MPanel
    {

        public BuildRecipeUI(Scene scene) : base(scene)
        {
            Texture = TextureBank.UITexture.GetSubtexture(112, 64, 24, 24);
            SetBorders(8, 8, 8, 8);
            Width = 282;
            Height = 200;

            MyText title = new MyText(scene);
            title.Name = "Title";
            title.Text = "Test";
            title.X = 8;
            title.Y = 8;
            title.Color = Color.LightBlue;
            AddChildNode(title);

            ListViewUI inventoryListView = new ListViewUI(scene, 250, 32, 4);
            inventoryListView.Name = "RecipeListView";
            inventoryListView.X = 8;
            inventoryListView.Y = 8 + 32;
            AddChildNode(inventoryListView);

            

            AddComponent(new BuildRecipeScript());
        }

    }

}
