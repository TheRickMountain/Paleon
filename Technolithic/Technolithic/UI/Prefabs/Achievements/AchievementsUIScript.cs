using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AchievementsUIScript : MScript
    {

        private SmallButton closeButton;

        private Dictionary<InGameAchievement, MNode> itemNodePairs = new();

        private ListViewUIScript listViewScript;

        private MyText achievementsProgressText;

        public AchievementsUIScript() : base(true)
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

            achievementsProgressText = new MyText(ParentNode.Scene);
            achievementsProgressText.Text = "Test";
            achievementsProgressText.X = 8;
            achievementsProgressText.Y = 36;
            ParentNode.AddChildNode(achievementsProgressText);

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

        public void Open()
        {
            achievementsProgressText.Text = Localization.GetLocalizedText("achievements_unlocked_x_slash_y",
                GameplayScene.Instance.AchievementManager.GetUnlockedAchievementsCount(),
                GameplayScene.Instance.AchievementManager.GetTotalAchievementsCount());

            listViewScript.Clear();

            foreach (InGameAchievement achievement in AchievementDatabase.GetAchievements())
            {
                MNode element;

                if (!itemNodePairs.ContainsKey(achievement))
                {
                    element = CreateItemNode(achievement);

                    itemNodePairs.Add(achievement, element);
                }
                else
                {
                    element = itemNodePairs[achievement];
                }

                bool isUnlocked = GameplayScene.Instance.AchievementManager.IsAchievementUnlocked(achievement.Id);

                if (isUnlocked == false)
                {
                    (element.GetChildByName("Icon") as MImageUI).Image.Texture = ResourceManager.QuestionMarkIcon;
                }
                else
                {
                    (element.GetChildByName("Icon") as MImageUI).Image.Texture = achievement.Icon;
                }

                listViewScript.AddItem(element);
            }
        }

        public void Close(bool value, ButtonScript buttonScript)
        {
            GameplayScene.UIRootNodeScript.CloseMainPanel();
        }

        private MNode CreateItemNode(InGameAchievement achievement)
        {
            MNode element = new MNode(ParentNode.Scene);

            MImageUI itemIcon = new MImageUI(ParentNode.Scene);
            itemIcon.Image.Texture = achievement.Icon;
            itemIcon.Name = "Icon";
            itemIcon.X = 8;
            itemIcon.Width = 48;
            itemIcon.Height = 48;

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = $"{achievement.Name}";
            itemName.Name = "Name";
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.X = itemIcon.LocalX + itemIcon.Width + 8;
            itemName.Y = 5;

            element.Tooltips = achievement.Description;

            element.AddChildNode(itemIcon);
            element.AddChildNode(itemName);

            element.Width = ParentNode.Width - (16 + 16); // borders + scroller width
            element.Height = 48;

            return element;
        }
    }
}
