using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class RenameCreatureUI : MyPanelUI
    {

        private MTextInput textInput;
        private BigButton acceptButton;
        private BigButton cancelButton;

        private CreatureCmp selectedCreature;

        public RenameCreatureUI(Scene scene) : base(scene, Localization.GetLocalizedText("rename"), Color.White)
        {
            Width = 200;
            Height = 150;

            textInput = new MTextInput(scene);
            textInput.Width = Width - 16;
            textInput.Height = 40;
            textInput.X = 8;
            textInput.Y = 32 + 8;
            textInput.GetComponent<MTextInputScript>().OnCurrentTextChanges += OnTextInputCurrentTextChanged;

            AddChildNode(textInput);

            acceptButton = new BigButton(scene, ResourceManager.AcceptIcon, false);
            acceptButton.GetChildByName("Icon").GetComponent<MImageCmp>().Color = Engine.GREEN_COLOR;
            acceptButton.Tooltips = Localization.GetLocalizedText("accept");
            acceptButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnAcceptButtonPressed);
            AddChildNode(acceptButton);

            cancelButton = new BigButton(scene, ResourceManager.CancelIcon, false);
            cancelButton.GetChildByName("Icon").GetComponent<MImageCmp>().Color = Engine.RED_COLOR;
            cancelButton.Tooltips = Localization.GetLocalizedText("decline");
            cancelButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnCancelButtonPressed);
            AddChildNode(cancelButton);

            int startX = Width / 2 - (acceptButton.Width + 5 + cancelButton.Width) / 2;
            int startY = Height - acceptButton.Height - 5;

            acceptButton.X = startX;
            acceptButton.Y = startY;

            cancelButton.X = acceptButton.LocalX + acceptButton.Width + 5;
            cancelButton.Y = startY;
        }

        public void Open(CreatureCmp creature)
        {
            selectedCreature = creature;

            textInput.GetComponent<MTextInputScript>().CurrentText = creature.Name;
        }

        private void OnTextInputCurrentTextChanged(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                acceptButton.ButtonScript.IsDisabled = true;
            }
            else
            {
                acceptButton.ButtonScript.IsDisabled = false;

                acceptButton.GetChildByName("Icon").GetComponent<MImageCmp>().Color = Engine.GREEN_COLOR;
            }
        }

        private void OnAcceptButtonPressed(bool value, ButtonScript buttonScript)
        {
            selectedCreature.Name = textInput.GetComponent<MTextInputScript>().CurrentText;

            GameplayScene.UIRootNodeScript.CloseAll();

            GameplayScene.UIRootNodeScript.OpenCreatureUI(selectedCreature);
        }

        private void OnCancelButtonPressed(bool value, ButtonScript buttonScript)
        {
            GameplayScene.UIRootNodeScript.CloseAll();

            GameplayScene.UIRootNodeScript.OpenCreatureUI(selectedCreature);
        }

    }
}
