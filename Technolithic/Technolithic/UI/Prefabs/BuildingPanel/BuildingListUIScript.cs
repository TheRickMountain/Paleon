using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class BuildingListUIScript : MScript
    {

        Dictionary<VeryBigButton, BuildingTemplate> buttonsBuildings;
        Dictionary<BuildingTemplate, VeryBigButton> buildingsButtons;

        public BuildingListUIScript() : base(false)
        {
            buttonsBuildings = new Dictionary<VeryBigButton, BuildingTemplate>();
            buildingsButtons = new Dictionary<BuildingTemplate, VeryBigButton>();
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
        }

        public void Open(BuildingCategory category, string name)
        {
            GameplayScene.WorldManager.SetMyAction(MyAction.Build, null);
            GameplayScene.WorldManager.SetBuilding(null);

            ((RichTextUI)ParentNode.GetChildByName("Label")).Text = name;

            // Получаем список строений переданной категории
            List<BuildingTemplate> categoryBuildings = Engine.Instance.BuildingsByCategories[category];

            ListViewUIScript listView = ParentNode.GetChildByName("ListView").GetComponent<ListViewUIScript>();
            listView.Clear();

            foreach (var building in categoryBuildings)
            {
                bool unlocked = GameplayScene.Instance.ProgressTree.IsBuildingUnlocked(building);

                if (unlocked == false)
                    continue;

                if (unlocked && !buildingsButtons.ContainsKey(building))
                {
                    // Создаем новую ноду
                    VeryBigButton buildingButton = new VeryBigButton(ParentNode.Scene, building.Icons[Direction.DOWN],
                        building.Icons[Direction.DOWN].Width * 2, building.Icons[Direction.DOWN].Height * 2, false);
                    buildingButton.Tooltips = building.GetInformation();
                    buildingButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnBuildingButtonPressed);
                    buildingButton.GetComponent<ButtonScript>().AddOnHoveredCallback(OnBuildingButtonHovered);
                    MImageUI exclamationMarkImage = new MImageUI(ParentNode.Scene);
                    exclamationMarkImage.Image.Texture = ResourceManager.ExclamationMarkIcon;
                    exclamationMarkImage.Width = exclamationMarkImage.Image.Texture.Width * 2;
                    exclamationMarkImage.Height = exclamationMarkImage.Image.Texture.Height * 2;
                    exclamationMarkImage.Y = -5;
                    exclamationMarkImage.X = buildingButton.LocalX + buildingButton.Width - (exclamationMarkImage.Width + 5);
                    exclamationMarkImage.Name = "ExclamationMark";
                    exclamationMarkImage.Active = false;
                    buildingButton.AddChildNode(exclamationMarkImage);

                    buttonsBuildings.Add(buildingButton, building);
                    buildingsButtons.Add(building, buildingButton);
                }

                MNode node = buildingsButtons[building];

                bool justUnlocked = GameplayScene.Instance.ProgressTree.IsJustUnlockedBuilding(building);

                if (justUnlocked)
                {
                    node.GetChildByName("ExclamationMark").Active = true;
                    (node as VeryBigButton).GetComponent<ButtonScript>().SetDefaultColor(Color.Orange, Color.Orange, Color.Orange);
                }
                else
                {
                    node.GetChildByName("ExclamationMark").Active = false;
                    (node as VeryBigButton).GetComponent<ButtonScript>().SetDefaultColor(Color.White, Color.White, Color.White);
                }

                listView.AddItem(node);
            }
        }

        public override void Update(int mouseX, int mouseY)
        {
            if(ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }
        }

        private void OnBuildingButtonPressed(bool value, ButtonScript buttonScript)
        {
            BuildingTemplate buildingTemplate = buttonsBuildings[(VeryBigButton)buttonScript.ParentNode];

            GameplayScene.WorldManager.SetBuilding(buildingTemplate);
            GameplayScene.UIRootNodeScript.OpenBuildingTemplateRecipeUI(buildingTemplate);
        }

        private void OnBuildingButtonHovered(ButtonScript buttonScript)
        {
            BuildingTemplate buildingTemplate = buttonsBuildings[(VeryBigButton)buttonScript.ParentNode];

            GameplayScene.Instance.ProgressTree.RemoveJustUnlockedBuilding(buildingTemplate);

            buttonScript.ParentNode.GetChildByName("ExclamationMark").Active = false;
            (buttonScript.ParentNode as VeryBigButton).GetComponent<ButtonScript>()
                            .SetDefaultColor(Color.White, Color.White, Color.White);

            // Нам больше не нужно будет убирать восклицательный знак с кнопки
            buttonScript.RemoveOnHoveredCallback(OnBuildingButtonHovered);
        }
    }
}
