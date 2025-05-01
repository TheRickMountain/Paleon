using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class LevelUIScript : MScript
    {
        private MImageUI icon;
        private MyText knowledgePointsText;
        private MImageUI knowledgePointsIcon;
        private MyText expText;

        public LevelUIScript() : base(true)
        {

        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            ParentNode.Tooltips = Localization.GetLocalizedText("technologies") + " [T]";
            ParentNode.GetComponent<ButtonScript>().AddOnClickedCallback(GameplayScene.UIRootNodeScript.OpenTechTreeUI);

            icon = new MImageUI(ParentNode.Scene);
            icon.Width = 32;
            icon.Height = 32;
            icon.X = 8;
            icon.Y = ParentNode.Height / 2 - icon.Width / 2;
            icon.GetComponent<MImageCmp>().Texture = ResourceManager.LampIcon;
            ParentNode.AddChildNode(icon);

            knowledgePointsText = new MyText(ParentNode.Scene);
            knowledgePointsText.Text = Localization.GetLocalizedText("knowledge_points");
            knowledgePointsText.X = 8;
            knowledgePointsText.Y = 8;

            int width = knowledgePointsText.TextWidth;
            knowledgePointsText.X = (ParentNode.Width / 2 - width / 2) + 16;

            knowledgePointsIcon = new MImageUI(ParentNode.Scene);
            knowledgePointsIcon.Width = 32;
            knowledgePointsIcon.Height = 32;
            knowledgePointsIcon.GetComponent<MImageCmp>().Texture = ItemDatabase.GetItemByName("knowledge_points").Icon;
            knowledgePointsIcon.Y = (knowledgePointsText.LocalY + knowledgePointsText.TextHeight) - 5;
            ParentNode.AddChildNode(knowledgePointsText);

            ParentNode.AddChildNode(knowledgePointsIcon);

            expText = new MyText(ParentNode.Scene);
            expText.Text = "";
            expText.X = 8;
            expText.Y = knowledgePointsText.LocalY + knowledgePointsText.TextHeight;
            expText.Outlined = true;
            ParentNode.AddChildNode(expText);


            GameplayScene.Instance.ProgressTree.OnExpAddedCallback += OnExpChanged;
            GameplayScene.Instance.ProgressTree.TechnologyUnlocked += OnTechnologyUnlocked;

            OnExpChanged(0);
        }

        public override void Update(int mouseX, int mouseY)
        {
            if(ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }
        }

        private void OnExpChanged(int exp)
        {
            ProgressTree levelSystem = GameplayScene.Instance.ProgressTree;

            int currentExp = levelSystem.CurrentExp;

            expText.Text = $"{currentExp}";

            int width = expText.TextWidth;
            expText.X = ParentNode.Width / 2 - width / 2;

            knowledgePointsIcon.X = expText.LocalX + expText.TextWidth + 5;
        }

        private void OnTechnologyUnlocked(Technology technology)
        {
            ProgressTree levelSystem = GameplayScene.Instance.ProgressTree;

            int currentExp = levelSystem.CurrentExp;

            expText.Text = $"{currentExp}";

            int width = expText.TextWidth;
            expText.X = ParentNode.Width / 2 - width / 2;

            knowledgePointsIcon.X = expText.LocalX + expText.TextWidth + 5;
        }
    }
}
