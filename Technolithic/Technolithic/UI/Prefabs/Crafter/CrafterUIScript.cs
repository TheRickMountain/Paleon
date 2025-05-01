using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class CrafterUIScript : MScript
    {
        private CraftingRecipe selectedCraftingRecipe;

        private CrafterBuildingCmp selectedCrafter;

        private Dictionary<CraftingRecipe, MNode> craftingRecipeNodePairs = new();
        private Dictionary<MNode, CraftingRecipe> nodeCraftingRecipePairs = new();

        private MButtonUI addButton;
        private MButtonUI subButton;
        private MButtonUI produceEndleslyButton;

        private ListViewUIScript listViewScript;

        public CrafterUIScript() : base(true)
        {
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            addButton = (MButtonUI)ParentNode.GetChildByName("AddButton");
            subButton = (MButtonUI)ParentNode.GetChildByName("SubButton");
            produceEndleslyButton = (MButtonUI)ParentNode.GetChildByName("ProduceEndleslyButton");

            addButton.GetComponent<ButtonScript>().AddOnClickedCallback(Add);
            addButton.Tooltips = "Left Click - Add 1\nShift + Left Click - Add 5\nCtrl + Left Click - Add 10";

            subButton.GetComponent<ButtonScript>().AddOnClickedCallback(Sub);
            subButton.Tooltips = "Left Click - Remove 1\nShift + Left Click - Remove 5\nCtrl + Left Click - Remove 10";

            produceEndleslyButton.GetComponent<ButtonScript>().AddOnClickedCallback(Repeat);
            produceEndleslyButton.Tooltips = Localization.GetLocalizedText("repeat_until_production_limits_are_reached");

            MNode listView = ParentNode.GetChildByName("ListView");
            listViewScript = listView.GetComponent<ListViewUIScript>();
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }
        }

        public void SetCrafter(CrafterBuildingCmp crafter)
        {
            foreach (var kvp in craftingRecipeNodePairs)
            {
                craftingRecipeNodePairs[kvp.Key].GetComponent<ButtonScript>().SetDefaultColor(Color.White * 0.0f, Color.White, Color.White);
            }

            selectedCraftingRecipe = null;

            selectedCrafter = crafter;

            listViewScript.Clear();
            
            foreach(var craftingRecipe in crafter.GetAvailableCraftingRecipes())
            {
                MNode element;

                if (!craftingRecipeNodePairs.ContainsKey(craftingRecipe))
                {
                    element = CreateItemNode(craftingRecipe);

                    craftingRecipeNodePairs.Add(craftingRecipe, element);

                    nodeCraftingRecipePairs.Add(element, craftingRecipe);
                }
                else
                {
                    element = craftingRecipeNodePairs[craftingRecipe];
                }

                listViewScript.AddItem(element);

                string count = crafter.GetCraftingRecipeCraftCount(craftingRecipe);

                ((MyText)element.GetChildByName("RecipesToCraftText")).Text = $"[{count}]";
            }
        }

        private MNode CreateItemNode(CraftingRecipe craftingRecipe)
        {
            MButtonUI element = new MButtonUI(ParentNode.Scene);
            element.Image.Texture = TextureBank.UITexture.GetSubtexture(192, 192, 24, 24);
            element.Image.ImageType = ImageType.Sliced;
            element.Image.BackgroundOverlap = 2;
            element.Image.SetBorder(8, 8, 8, 8);
            element.ButtonScript.AddOnClickedCallback(SelectItemToCraft);
            element.ButtonScript.SetDefaultColor(Color.White * 0.0f, Color.White, Color.White);

            element.Width = ParentNode.Width - (16 + 16); // borders + scroller width
            element.Height = 34;

            int ingredientNumber = 0;
            foreach (var kvp in craftingRecipe.Ingredients)
            {
                MImageUI itemIcon = new MImageUI(ParentNode.Scene);
                itemIcon.Image.Texture = kvp.Key.Icon;
                itemIcon.Width = 32;
                itemIcon.Height = 32;
                itemIcon.X = 8 + ingredientNumber * 32;
                itemIcon.Y = 2;
                itemIcon.Tooltips = kvp.Key.GetInformation();
                element.AddChildNode(itemIcon);

                MyText itemNumber = new MyText(ParentNode.Scene);
                itemNumber.Text = "" + kvp.Value;
                itemNumber.Outlined = true;
                itemNumber.X = (itemIcon.X + 32) - itemNumber.TextWidth;
                itemNumber.Y = 16;
                element.AddChildNode(itemNumber);

                ingredientNumber++;
            }

            MyText recipesToCraftNumber = new MyText(ParentNode.Scene);
            recipesToCraftNumber.Text = "[100]";
            recipesToCraftNumber.X = element.Width - recipesToCraftNumber.TextWidth;
            recipesToCraftNumber.Y = 2;
            recipesToCraftNumber.Name = "RecipesToCraftText";
            element.AddChildNode(recipesToCraftNumber);

            int resultNumber = 0;
            MImageUI lastResultItemIcon = null;
            foreach (var kvp in craftingRecipe.Result)
            {
                Item resultItem = kvp.Key;
                int resultWeight = kvp.Value;

                MImageUI itemIcon = new MImageUI(ParentNode.Scene);
                itemIcon.Image.Texture = kvp.Key.Icon;
                itemIcon.Width = 32;
                itemIcon.Height = 32;
                itemIcon.X = (recipesToCraftNumber.X - recipesToCraftNumber.TextWidth) - resultNumber * 48;
                itemIcon.Y = 2;

                if (craftingRecipe.Result.Count > 1 && resultItem != craftingRecipe.MainItem)
                {
                    itemIcon.Tooltips += $"/c[lightblue]* {Localization.GetLocalizedText("side_item")}/cd\n";
                }

                itemIcon.Tooltips += kvp.Key.GetInformation();
                element.AddChildNode(itemIcon);

                lastResultItemIcon = itemIcon;

                RichTextUI itemNumber = new RichTextUI(ParentNode.Scene);
                itemNumber.Text = "" + kvp.Value;
                itemNumber.Outlined = true;
                itemNumber.X = (itemIcon.X + 32) - itemNumber.TextWidth;
                itemNumber.Y = 16;
                element.AddChildNode(itemNumber);

                if (craftingRecipe.Result.Count > 1 && resultItem != craftingRecipe.MainItem)
                {
                    itemNumber.Text += "/c[lightblue]*/cd";
                }

                resultNumber++;
            }

            MImageUI arrowIcon = new MImageUI(ParentNode.Scene);
            arrowIcon.Image.Texture = ResourceManager.ArrowIcon;
            arrowIcon.Width = 32;
            arrowIcon.Height = 32;
            arrowIcon.X = lastResultItemIcon.LocalX - 40;
            arrowIcon.Y = 2;
            arrowIcon.Tooltips = $"{Localization.GetLocalizedText("time")}: {string.Format("{0:0.0}", (float)craftingRecipe.Time / (float)WorldState.MINUTES_PER_HOUR)} {Localization.GetLocalizedText("hours")}";
            element.AddChildNode(arrowIcon);

            return element;
        }

        public void SelectItemToCraft(bool value, ButtonScript sender)
        {
            foreach (var kvp in craftingRecipeNodePairs)
            {
                craftingRecipeNodePairs[kvp.Key].GetComponent<ButtonScript>().SetDefaultColor(Color.White * 0.0f, Color.White, Color.White);
            }

            selectedCraftingRecipe = nodeCraftingRecipePairs[(MButtonUI)sender.ParentNode];

            craftingRecipeNodePairs[selectedCraftingRecipe].GetComponent<ButtonScript>().SetDefaultColor(Color.Orange, Color.Orange, Color.Orange);
        }

        public void SetCraftingRecipeCount(CraftingRecipe craftingRecipe, string count)
        {
            ((MyText)craftingRecipeNodePairs[craftingRecipe].GetChildByName("RecipesToCraftText")).Text = $"[{count}]";
        }

        private void Add(bool value, ButtonScript sender)
        {
            if (selectedCraftingRecipe != null)
            {
                if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                {
                    selectedCrafter.AddCraftingRecipe(selectedCraftingRecipe, 5);
                }
                else if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                {
                    selectedCrafter.AddCraftingRecipe(selectedCraftingRecipe, 10);
                }
                else
                {
                    selectedCrafter.AddCraftingRecipe(selectedCraftingRecipe, 1);
                }

                string count = selectedCrafter.GetCraftingRecipeCraftCount(selectedCraftingRecipe);

                SetCraftingRecipeCount(selectedCraftingRecipe, count);
            }
        }

        private void Sub(bool value, ButtonScript sender)
        {
            if (selectedCraftingRecipe != null)
            {
                if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                {
                    selectedCrafter.SubCraftingRecipe(selectedCraftingRecipe, 5);
                }
                else if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                {
                    selectedCrafter.SubCraftingRecipe(selectedCraftingRecipe, 10);
                }
                else
                {
                    selectedCrafter.SubCraftingRecipe(selectedCraftingRecipe, 1);
                }

                string count = selectedCrafter.GetCraftingRecipeCraftCount(selectedCraftingRecipe);

                SetCraftingRecipeCount(selectedCraftingRecipe, count);
            }
        }

        private void Repeat(bool value, ButtonScript sender)
        {
            if (selectedCraftingRecipe != null)
            {
                selectedCrafter.RepeatCraftingRecipe(selectedCraftingRecipe);

                string count = selectedCrafter.GetCraftingRecipeCraftCount(selectedCraftingRecipe);

                SetCraftingRecipeCount(selectedCraftingRecipe, count);
            }
        }
    }
}
