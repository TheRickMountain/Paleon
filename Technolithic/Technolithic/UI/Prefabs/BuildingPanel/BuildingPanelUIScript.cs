using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class BuildingPanelUIScript : MScript
    {

        private Dictionary<BuildingCategory, BigButton> categoriesButtons;
        private Dictionary<BigButton, BuildingCategory> buttonsCategories;

        public BuildingPanelUIScript() : base(true)
        {

        }

        public override void Awake()
        {
            categoriesButtons = new Dictionary<BuildingCategory, BigButton>();
            buttonsCategories = new Dictionary<BigButton, BuildingCategory>();

            CreateCategoryButtons(BuildingCategory.Wall, Localization.GetLocalizedText("wall"), TextureBank.UITexture.GetSubtexture(144, 80, 16, 16));
            CreateCategoryButtons(BuildingCategory.Floor, Localization.GetLocalizedText("floor"), TextureBank.UITexture.GetSubtexture(240, 80, 16, 16));
            CreateCategoryButtons(BuildingCategory.Settlement, Localization.GetLocalizedText("settlement"), TextureBank.UITexture.GetSubtexture(208, 80, 16, 16));
            CreateCategoryButtons(BuildingCategory.Food, Localization.GetLocalizedText("food"), ResourceManager.FoodIcon);
            CreateCategoryButtons(BuildingCategory.Production, Localization.GetLocalizedText("production"), TextureBank.UITexture.GetSubtexture(176, 80, 16, 16));
            CreateCategoryButtons(BuildingCategory.Storage, Localization.GetLocalizedText("storage"), ResourceManager.ResourcesIcon);
            CreateCategoryButtons(BuildingCategory.Agriculture, Localization.GetLocalizedText("agriculture"), ResourceManager.AgricultureIcon);
            CreateCategoryButtons(BuildingCategory.Metallurgy, Localization.GetLocalizedText("metallurgy"), ResourceManager.MetallurgyIcon);
            CreateCategoryButtons(BuildingCategory.Mechanisms, Localization.GetLocalizedText("mechanisms"), ResourceManager.MechanismIcon);
            CreateCategoryButtons(BuildingCategory.Mining, Localization.GetLocalizedText("mining"), ResourceManager.MiningIcon);
            CreateCategoryButtons(BuildingCategory.Knowledge, Localization.GetLocalizedText("knowledge"), ResourceManager.LampIcon);
            CreateCategoryButtons(BuildingCategory.Medicine, Localization.GetLocalizedText("medicine"), ResourceManager.MedicineIcon);


            // check opened buildings
            foreach(var buildingTemplate in GameplayScene.Instance.ProgressTree.GetUnlockedBuildingTemplatesEnumerable())
            {
                if (buildingTemplate.BuildingCategory != BuildingCategory.None)
                    categoriesButtons[buildingTemplate.BuildingCategory].Active = true;
            }

            ArrangeButtons();
        }

        public override void Begin()
        {
            GameplayScene.Instance.ProgressTree.TechnologyUnlocked += UpdateCategories;
            GameplayScene.Instance.ProgressTree.BuildingUnlocked += UpdateCategories;
            GameplayScene.Instance.ProgressTree.BuildingJustUnlocked += UpdateExclamationMarks;
            GameplayScene.Instance.ProgressTree.JustUnlockedBuildingRemoved += UpdateExclamationMarks;
        }

        public override void Update(int mouseX, int mouseY)
        {
        }

        private void UpdateExclamationMarks(BuildingTemplate buildingTemplate)
        {
            BuildingCategory buildingCategory = buildingTemplate.BuildingCategory;

            UpdateExclamationMarks(buildingCategory);
        }

        private void UpdateExclamationMarks(BuildingCategory buildingCategory)
        {
            MNode buttonNode = categoriesButtons[buildingCategory];

            bool isCategoryHasJustUnlockedBuilding = GameplayScene.Instance.ProgressTree.AnyJustUnlockedBuildingsInBuildingCategory(buildingCategory);

            buttonNode.GetChildByName("ExclamationMark").Active = isCategoryHasJustUnlockedBuilding;
        }

        private void CreateCategoryButtons(BuildingCategory buildingCategory, string name, MyTexture icon)
        {
            BigButton categoryButton = new BigButton(ParentNode.Scene, icon, false, true);
            categoryButton.GetComponent<ButtonScript>().AddOnClickedCallback(OpenCategory);
            categoryButton.X = buttonsCategories.Count * (categoryButton.Width + 5);
            categoryButton.Active = false;
            categoryButton.SetMetadata("name", name);
            categoryButton.Tooltips = name;
            ParentNode.AddChildNode(categoryButton);

            categoriesButtons.Add(buildingCategory, categoryButton);
            buttonsCategories.Add(categoryButton, buildingCategory);

            UpdateExclamationMarks(buildingCategory);
        }

        private void ArrangeButtons()
        {
            int count = 0;
            for (int i = 0; i < buttonsCategories.Count; i++)
            {
                var kvp = buttonsCategories.ElementAt(i);

                MNode button = kvp.Key;

                if (button.Active)
                {
                    button.X = count * (button.Width + 5);
                    count++;
                }
            }
        }

        private void OpenCategory(bool value, ButtonScript buttonScript)
        {
            BuildingCategory category = buttonsCategories[(BigButton)buttonScript.ParentNode];

            GameplayScene.UIRootNodeScript.OpenBuildingListUI(category, buttonScript.ParentNode.GetMetadata<string>("name"));
        }

        private void UpdateCategories(Technology technology)
        {
            if (technology.UnlockedBuildings == null)
                return;

            foreach(var building in technology.UnlockedBuildings)
            {
                if(building.BuildingCategory != BuildingCategory.None)
                    categoriesButtons[building.BuildingCategory].Active = true;
            }

            ArrangeButtons();
        }

        private void UpdateCategories(BuildingTemplate buildingTemplate)
        {
            if (buildingTemplate == null)
                return;

            if (buildingTemplate.BuildingCategory != BuildingCategory.None)
                categoriesButtons[buildingTemplate.BuildingCategory].Active = true;

            ArrangeButtons();
        }

    }
}
