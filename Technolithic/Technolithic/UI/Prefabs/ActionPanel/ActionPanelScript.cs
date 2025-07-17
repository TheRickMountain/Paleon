using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ActionPanelScript : MScript
    {
        private BigButton lastSelectedAction;

        private Dictionary<MyAction, BigButton> actionsButtons;
        private Dictionary<BigButton, MyAction> buttonsActions;

        public ActionPanelScript() : base(true)
        {

        }

        public override void Awake()
        {
            
        }

        public override void Begin()
        {
            actionsButtons = new Dictionary<MyAction, BigButton>();
            buttonsActions = new Dictionary<BigButton, MyAction>();

            CreateActionButton(MyAction.GatherStone, ResourceManager.GatherStoneIcon, Localization.GetLocalizedText("gather_stone"), 
                true, Keys.G);

            CreateActionButton(MyAction.GatherWood, ResourceManager.GatherWoodIcon, Localization.GetLocalizedText("gather_wood"),
                true, Keys.Y);

            CreateActionButton(MyAction.Mine, ResourceManager.MineIcon, Localization.GetLocalizedText("mine"), 
                false, Keys.M);
            
            CreateActionButton(MyAction.Chop, ResourceManager.ChopIcon, Localization.GetLocalizedText("chop"),
                false, Keys.N);
            
            CreateActionButton(MyAction.AutoHarvest, ResourceManager.AutoHarvestIcon, Localization.GetLocalizedText("auto_harvest") + "\n" +
                "/c[#919090]" + Localization.GetLocalizedText("сut_automatically_description") + "/cd", 
                true, Keys.B);
            
            CreateActionButton(MyAction.Uproot, ResourceManager.UprootIcon, Localization.GetLocalizedText("uproot"), 
                true, Keys.F);
            
            CreateActionButton(MyAction.Hunt, ResourceManager.HuntIcon, Localization.GetLocalizedText("hunt"), 
                true, Keys.H);
            
            CreateActionButton(MyAction.Slaughter, ResourceManager.SlaughterIcon, Localization.GetLocalizedText("slaughter"), 
                false, Keys.V);
            
            CreateActionButton(MyAction.BuildIrrigationCanal, TextureBank.UITexture.GetSubtexture(112, 160, 16, 16),
                Localization.GetLocalizedText("build_irrigation_canal"), 
                false, Keys.K);
            
            CreateActionButton(MyAction.DestructIrrigationCanal, TextureBank.UITexture.GetSubtexture(128, 160, 16, 16), 
                Localization.GetLocalizedText("destruct_irrigation_canal"), 
                false, Keys.J);
            
            CreateActionButton(MyAction.Cancel, TextureBank.UITexture.GetSubtexture(48, 0, 16, 16), Localization.GetLocalizedText("cancel"), 
                true, Keys.C);
            
            CreateActionButton(MyAction.Destruct, ResourceManager.DestructIcon, Localization.GetLocalizedText("destruct"), 
                true, Keys.X);
            
            CreateActionButton(MyAction.DestructSurface, ResourceManager.DestructSurfaceIcon, Localization.GetLocalizedText("destruct_surface"), 
                true, Keys.Z);
            

            GameplayScene.WorldManager.SetMyAction(MyAction.None, null);

            GameplayScene.Instance.ProgressTree.TechnologyUnlocked += UpdateButtons;

            foreach(var kvp in GameplayScene.Instance.ProgressTree.UnlockedActions)
            {
                if(kvp.Value)
                {
                    actionsButtons[kvp.Key].Active = true;
                }
            }

            ArrangeButtons();
        }

        private void CreateActionButton(MyAction myAction, MyTexture icon, string actionName, bool active, Keys hotkey)
        {
            BigButton actionButton = new BigButton(ParentNode.Scene, icon, false, true);
            actionButton.GetComponent<ButtonScript>().AddOnClickedCallback(SetAction);
            actionButton.X = buttonsActions.Count * (actionButton.Width + 5);
            actionButton.Active = active;
            actionButton.Tooltips = $"[{hotkey}] " + actionName;
            actionButton.SetMetadata("hotkey", hotkey);
            ParentNode.AddChildNode(actionButton);

            actionsButtons.Add(myAction, actionButton);
            buttonsActions.Add(actionButton, myAction);
        }

        public override void Update(int mouseX, int mouseY)
        {
            if(ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }

            foreach(var kvp in buttonsActions)
            {
                BigButton button = kvp.Key;
                if(button.Active == false)
                {
                    continue;
                }
                
                Keys key = button.GetMetadata<Keys>("hotkey");
                if (MInput.Keyboard.Pressed(key))
                {
                    SetAction(false, button.ButtonScript);
                }
            }
        }

        public void UnselectAll()
        {
            if (lastSelectedAction != null)
            {
                lastSelectedAction.GetComponent<ButtonScript>().IsSelected = false;
                lastSelectedAction = null;
            }
        }

        private void SetAction(bool value, ButtonScript buttonScript)
        {
            if(lastSelectedAction != null)
            {
                lastSelectedAction.GetComponent<ButtonScript>().IsSelected = false;
            }

            BigButton pressedButton = (BigButton)buttonScript.ParentNode;

            lastSelectedAction = pressedButton;
            lastSelectedAction.GetComponent<ButtonScript>().IsSelected = true;
            pressedButton.GetChildByName("ExclamationMark").Active = false;

            MyAction selectedAction = buttonsActions[pressedButton];

            MImageUI buttonIcon = (MImageUI)pressedButton.GetChildByName("Icon");
            GameplayScene.WorldManager.SetMyAction(selectedAction, buttonIcon.GetComponent<MImageCmp>().Texture);
        }

        public void ArrangeButtons()
        {
            int count = 0;
            for (int i = 0; i < buttonsActions.Count; i++)
            {
                var kvp = buttonsActions.ElementAt(i);

                MNode button = kvp.Key;

                if (button.Active)
                {
                    button.X = count * (button.Width + 5);
                    count++;
                }
            }

            int totalWidth = count * buttonsActions.ElementAt(0).Key.Width + count * 5;

            ParentNode.X = Engine.Width - totalWidth;
            ParentNode.Y = Engine.Height - buttonsActions.ElementAt(0).Key.Height - 5;
        }

        private void UpdateButtons(Technology technology)
        {
            if (technology.UnlockedActions != null)
            {
                bool actionWasUnlocked = false;

                foreach (var myAction in technology.UnlockedActions)
                {
                    actionsButtons[myAction].Active = true;
                    actionsButtons[myAction].GetChildByName("ExclamationMark").Active = true;
                    actionWasUnlocked = true;
                }

                if (actionWasUnlocked)
                    ArrangeButtons();
            }
        }

    }
}
