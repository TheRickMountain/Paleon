using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Technolithic
{
    public class ActionPanelScript : MScript
    {
        public enum ButtonType
        {
            Category,
            Action,
            Back
        }

        private Dictionary<GameActionCategory?, List<MyAction>> categoryActionsDict = new()
        {
            { GameActionCategory.CollectionAndExtractionOfResources, [
                MyAction.GatherWood,
                MyAction.GatherStone,
                MyAction.Chop,
                MyAction.Mine] },

            { GameActionCategory.AnimalHusbandryAndHunting, [
                MyAction.Hunt,
                MyAction.Slaughter ] },

            { GameActionCategory.Agriculture, [
                MyAction.AutoHarvest,
                MyAction.Uproot,
                MyAction.BuildIrrigationCanal,
                MyAction.Plow ] },

            { GameActionCategory.Destruct, [
                MyAction.DestructConstruction,
                MyAction.DestructSurface,
                MyAction.DestructWall,
                MyAction.DestructIrrigationCanal
                ] },

            { GameActionCategory.Cancel, [
                MyAction.Cancel 
                ] }
        };

        private List<TBigButtonUI> categoriesButtonsList = new();
        private List<TBigButtonUI> actionsButtonsList = new();

        private Dictionary<GameActionCategory, TBigButtonUI> categoriesButtonsDict = new();
        private Dictionary<MyAction, TBigButtonUI> actionsButtonsDict = new();

        private TBigButtonUI backButton;

        // TODO: show required technology in tooltips for locked technology
        public ActionPanelScript() : base(true)
        {

        }

        public override void Awake()
        {
            
        }

        public override void Begin()
        {
            CreateGameActionCategoryButtons();

            CreateGameActionButtons();

            LinkButtonsWithHotkeys();

            backButton = new TBigButtonUI(ParentNode.Scene);
            backButton.Icon = TextureBank.UITexture.GetSubtexture(144, 64, 16, 16);
            backButton.ButtonUp += BackButton_ButtonUp;
            backButton.Tooltips = Localization.GetLocalizedText("back") + $" [{Keys.C.ToString()}]";
            backButton.SetMetadata("hotkey", Keys.C);
            backButton.SetMetadata("button_type", ButtonType.Back);

            ArrangeButtons();

            GameplayScene.WorldManager.SetMyAction(MyAction.None, null);

            GameplayScene.Instance.ProgressTree.TechnologyUnlocked += UpdateButtons;

            foreach (var kvp in GameplayScene.Instance.ProgressTree.UnlockedActions)
            {
                if (kvp.Value)
                {
                    actionsButtonsDict[kvp.Key].Disabled = false;
                }
            }
        }

        private void CreateGameActionCategoryButtons()
        {
            foreach (GameActionCategory category in Enum.GetValues(typeof(GameActionCategory)))
            {
                EnumData data = EnumDatabase<GameActionCategory>.GetData(category);

                TBigButtonUI categoryButton = new TBigButtonUI(ParentNode.Scene);
                categoryButton.Icon = data.Icon;
                categoryButton.Tooltips = data.DisplayText;
                categoryButton.ButtonUp += CategoryButton_ButtonUp;
                categoryButton.SetMetadata("category", category);
                categoryButton.SetMetadata("button_type", ButtonType.Category);
                ParentNode.AddChildNode(categoryButton);
                categoriesButtonsList.Add(categoryButton);
                categoriesButtonsDict.Add(category, categoryButton);
            }
        }

        private void CreateGameActionButtons()
        {
            foreach (MyAction myAction in Enum.GetValues(typeof(MyAction)))
            {
                // TODO: fix it
                if (myAction == MyAction.None ||
                    myAction == MyAction.CopySettings ||
                    myAction == MyAction.Build) continue;

                EnumData data = EnumDatabase<MyAction>.GetData(myAction);

                TBigButtonUI actionButton = new TBigButtonUI(ParentNode.Scene);
                actionButton.Icon = data.Icon;
                actionButton.Tooltips = data.DisplayText;
                actionButton.ButtonUp += ActionButton_ButtonUp;
                actionButton.SetMetadata("action", myAction);
                actionButton.SetMetadata("button_type", ButtonType.Action);

                // TODO: fix it
                if (myAction == MyAction.AutoHarvest ||
                    myAction == MyAction.Hunt ||
                    myAction == MyAction.DestructConstruction ||
                    myAction == MyAction.DestructSurface ||
                    myAction == MyAction.Cancel ||
                    myAction == MyAction.DestructWall ||
                    myAction == MyAction.GatherStone ||
                    myAction == MyAction.GatherWood ||
                    myAction == MyAction.Uproot)
                {
                    actionButton.Disabled = false;

                }
                else
                {
                    actionButton.Disabled = true;
                }

                actionsButtonsList.Add(actionButton);
                actionsButtonsDict.Add(myAction, actionButton);
            }
        }

        private void LinkButtonsWithHotkeys()
        {
            int categoryNumber = 0;
            int actionNumber = 0;

            foreach (GameActionCategory gameActionCategory in Enum.GetValues(typeof(GameActionCategory)))
            {
                MNode categoryButton = categoriesButtonsDict[gameActionCategory];
                LinkNodeWithHotkey(categoryButton, Keys.D1 + categoryNumber);

                foreach (MyAction myAction in categoryActionsDict[gameActionCategory])
                {
                    MNode actionButton = actionsButtonsDict[myAction];
                    LinkNodeWithHotkey(actionButton, Keys.D1 + actionNumber);

                    actionNumber++;
                }

                categoryNumber++;
                actionNumber = 0;
            }
        }

        private void LinkNodeWithHotkey(MNode button, Keys key)
        {
            button.Tooltips += $" [{key.ToString()[1]}]";
            button.SetMetadata("hotkey", key);
        }

        private void BackButton_ButtonUp(TButtonUI obj)
        {
            ParentNode.RemoveAllChildren();

            foreach(var categoryButton in categoriesButtonsList)
            {
                ParentNode.AddChildNode(categoryButton);
            }

            ArrangeButtons();
        }

        private void ActionButton_ButtonUp(TButtonUI obj)
        {
            MyAction myAction = obj.GetMetadata<MyAction>("action");

            GameplayScene.WorldManager.SetMyAction(myAction, obj.Icon);

            obj.ExtraIcon = null;

            ArrangeButtons();
        }

        private void CategoryButton_ButtonUp(TButtonUI obj)
        {
            GameActionCategory category = obj.GetMetadata<GameActionCategory>("category");

            if (category == GameActionCategory.Cancel)
            {
                GameplayScene.WorldManager.SetMyAction(MyAction.Cancel, obj.Icon);
            }
            else
            {
                ParentNode.RemoveAllChildren();

                foreach (MyAction myAction in categoryActionsDict[category])
                {
                    var actionButton = actionsButtonsDict[myAction];

                    ParentNode.AddChildNode(actionButton);
                }

                ParentNode.AddChildNode(backButton);

                ArrangeButtons();
            }
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }

            foreach (MNode node in ParentNode.GetChidlrenEnumerable())
            {
                TButtonUI button = node as TButtonUI;

                if (button.Disabled) continue;

                if (MInput.Keyboard.Released(node.GetMetadata<Keys>("hotkey")))
                {
                    switch (node.GetMetadata<ButtonType>("button_type"))
                    {
                        case ButtonType.Action:
                            ActionButton_ButtonUp(button);
                            break;
                        case ButtonType.Category:
                            CategoryButton_ButtonUp(button);
                            break;
                        case ButtonType.Back:
                            BackButton_ButtonUp(button);
                            break;
                    }
                }
            }
        }

        public void ArrangeButtons()
        {
            MNode firstNode = null;

            int count = 0;
            foreach (var node in ParentNode.GetChidlrenEnumerable())
            {
                if (firstNode == null)
                {
                    firstNode = node;
                }

                node.X = count * (node.Width + 5);
                count++;
            }

            if (firstNode != null)
            {
                int totalWidth = count * firstNode.Width + count * 5;

                ParentNode.X = Engine.Width - totalWidth;
                ParentNode.Y = Engine.Height - firstNode.Height - 5;
            }
        }

        private void UpdateButtons(Technology technology)
        {
            if (technology.UnlockedActions != null)
            {
                foreach (var myAction in technology.UnlockedActions)
                {
                    actionsButtonsDict[myAction].Disabled = false;
                    actionsButtonsDict[myAction].ExtraIcon = ResourceManager.ExclamationMarkIcon;
                }
            }
        }

    }
}
