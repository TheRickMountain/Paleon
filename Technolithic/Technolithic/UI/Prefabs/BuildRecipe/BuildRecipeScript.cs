using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class BuildRecipeScript : MScript
    {
        private Dictionary<Item, MNode> itemsNodes;

        public BuildRecipeScript() : base(true)
        {
            itemsNodes = new Dictionary<Item, MNode>();
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }
        }

        public void SetBuildingTemplate(BuildingTemplate buildingTemplate)
        {
            ((MyText)ParentNode.GetChildByName("Title")).Text = buildingTemplate.Name;

            MNode listView = ParentNode.GetChildByName("RecipeListView");
            ListViewUIScript listViewScript = listView.GetComponent<ListViewUIScript>();
            listViewScript.Clear();

            listViewScript.AddItem(CreateTextNode($"{Localization.GetLocalizedText("ingredients")}:"));

            if (buildingTemplate.ConstructionData != null)
            {
                // Устанавливаем новые количества для предметов
                foreach (var kvp in buildingTemplate.ConstructionData.RealIngredients)
                {
                    Item item = kvp.Key;
                    int weight = kvp.Value;

                    // Создаем ноду, если предмет не был на складе
                    if (!itemsNodes.ContainsKey(item))
                    {
                        MNode itemNode = CreateItemNode(item);
                        itemsNodes.Add(item, itemNode);
                    }

                    listViewScript.AddItem(itemsNodes[item]);

                    ((MyText)itemsNodes[item].GetChildByName("Name")).Text = $"{item.Name} [{weight}]";
                }
            }
        }

        private MNode CreateTextNode(string text)
        {
            MNode element = new MNode(ParentNode.Scene);

            MyText textNode = new MyText(ParentNode.Scene);
            textNode.Text = text;
            textNode.Name = "Name";
            textNode.Width = 100;
            textNode.Height = 32;

            element.Width = 32 + 5 + 100;
            element.Height = 32;

            element.AddChildNode(textNode);

            return element;
        }

        private MNode CreateItemNode(Item item)
        {
            MNode element = new MNode(ParentNode.Scene);

            MImageUI itemIcon = new MImageUI(ParentNode.Scene);
            itemIcon.Image.Texture = item.Icon;
            itemIcon.Name = "Icon";
            itemIcon.Width = 32;
            itemIcon.Height = 32;

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = $"{item.Name} [0]";
            itemName.Name = "Name";
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.X = itemIcon.Width + 5;

            element.Width = 32 + 5 + 100;
            element.Height = 32;

            element.AddChildNode(itemIcon);
            element.AddChildNode(itemName);

            return element;
        }
    }
}
