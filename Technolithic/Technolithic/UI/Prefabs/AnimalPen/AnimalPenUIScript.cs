using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AnimalPenUIScript : MScript
    {
        private Dictionary<AnimalTemplate, MNode> animalTemplatesNodes;

        private AnimalPenBuildingCmp selectedAnimalPen;

        public AnimalPenUIScript() : base(true)
        {
            animalTemplatesNodes = new Dictionary<AnimalTemplate, MNode>();
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
        }

        public void SetAnimalPen(AnimalPenBuildingCmp stable)
        {
            selectedAnimalPen = stable;

            MNode listView = ParentNode.GetChildByName("ListView");
            ListViewUIScript listViewScript = listView.GetComponent<ListViewUIScript>();

            listViewScript.Clear();

            foreach (var kvp in stable.GetAnimalsFilters())
            {
                AnimalTemplate animalTemplate = kvp.Key;
                bool flag = kvp.Value;

                // Создаем ноду, если животное не было в коллекции
                if (!animalTemplatesNodes.ContainsKey(animalTemplate))
                {
                    MNode listElementNode = CreateListElementNode(animalTemplate);
                    animalTemplatesNodes.Add(animalTemplate, listElementNode);
                }

                MNode node = animalTemplatesNodes[animalTemplate];

                listViewScript.AddItem(node);

                ToggleScript toggleScript = node.GetChildByName("Toggle").GetComponent<ToggleScript>();
                toggleScript.SilentCheck(flag);
            }
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }
        }

        private MNode CreateListElementNode(AnimalTemplate animalTemplate)
        {
            MNode element = new MNode(ParentNode.Scene);

            MToggleUI toggleUI = new MToggleUI(ParentNode.Scene, true);
            toggleUI.Name = "Toggle";
            toggleUI.GetComponent<ToggleScript>().AddOnValueChangedCallback(SetToggle);
            toggleUI.X = 8;

            MImageUI animalIcon = new MImageUI(ParentNode.Scene);
            animalIcon.Image.Texture = animalTemplate.Texture;
            animalIcon.Name = "Icon";
            animalIcon.X = toggleUI.LocalX + toggleUI.Width + 5;
            animalIcon.Width = 32;
            animalIcon.Height = 32;

            RichTextUI animalName = new RichTextUI(ParentNode.Scene);
            animalName.Text = animalTemplate.GetNameWithAgeAndSex();
            animalName.Name = "Name";
            animalName.Width = 100;
            animalName.Height = 32;
            animalName.X = animalIcon.LocalX + animalIcon.Width + 5;

            element.Width = toggleUI.LocalX + animalName.LocalX + animalName.Width;
            element.Height = animalIcon.Height;

            element.AddChildNode(toggleUI);
            element.AddChildNode(animalIcon);
            element.AddChildNode(animalName);

            return element;
        }

        private void SetToggle(bool value, MToggleUI toggleUI)
        {
            AnimalTemplate animalTemplate = GetAnimalTemplateByNode(toggleUI.ParentNode);
            selectedAnimalPen.SetAnimalFilter(animalTemplate, value);
        }

        public AnimalTemplate GetAnimalTemplateByNode(MNode node)
        {
            return animalTemplatesNodes.Where(x => x.Value == node).Select(x => x.Key).FirstOrDefault();
        }

    }
}