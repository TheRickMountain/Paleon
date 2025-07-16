using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Technolithic
{
    public abstract class InteractableUIScript : MScript
    {
        private Interactable selectedInteractable;

        private Dictionary<InteractionType, BigButton> interactionButtonDict = new();

        public InteractableUIScript() : base(true)
        {
            
        }

        public void SetInteractable(Interactable interactable)
        {
            selectedInteractable = interactable;

            // TODO: refactoring required
            foreach (InteractionType interactionType in selectedInteractable.AvailableInteractions)
            {
                BigButton interactionButton;

                InteractionData interactionData = Engine.InteractionsDatabase.GetInteractionData(interactionType);

                if (interactionButtonDict.ContainsKey(interactionType) == false)
                {
                    interactionButton = new BigButton(ParentNode.Scene, interactionData.Icon, false);
                    interactionButtonDict[interactionType] = interactionButton;
                    interactionButton.ButtonScript.Pressed += Interactable_InteractionButton_Pressed;
                    interactionButton.SetMetadata("interaction_data", interactionData);

                    MImageUI disableIcon = new MImageUI(interactionButton.Scene);
                    disableIcon.X = 8;
                    disableIcon.Y = 8;
                    disableIcon.Width = 32;
                    disableIcon.Height = 32;
                    disableIcon.Name = "disable_icon";
                    disableIcon.Image.Texture = ResourceManager.DisableIcon;

                    interactionButton.AddChildNode(disableIcon);
                }

                interactionButton = interactionButtonDict[interactionType];
                interactionButton.SetMetadata("interactable", selectedInteractable);

                if (selectedInteractable.IsInteractionMarked(interactionType))
                {
                    (interactionButton.GetChildByName("disable_icon") as MImageUI).Image.Visible = true;
                }
                else
                {
                    (interactionButton.GetChildByName("disable_icon") as MImageUI).Image.Visible = false;
                }

                interactionButton.Tooltips = GenerateInteractionTooltip(selectedInteractable, interactionData);

                ListViewUIScript buttonsListView = ParentNode.GetChildByName("ButtonsListView").GetComponent<ListViewUIScript>();
                buttonsListView.AddItem(interactionButton);
            }
        }

        private void Interactable_InteractionButton_Pressed(ButtonScript buttonScript)
        {
            InteractionData interactionData = buttonScript.ParentNode.GetMetadata<InteractionData>("interaction_data");
            Interactable interactable = buttonScript.ParentNode.GetMetadata<Interactable>("interactable");
            InteractionType interactionType = interactionData.InteractionType;

            if (selectedInteractable.IsInteractionMarked(interactionType))
            {
                selectedInteractable.UnmarkInteraction(interactionType);
            }
            else
            {
                selectedInteractable.MarkInteraction(interactionType);
            }

            if (selectedInteractable.IsInteractionMarked(interactionData.InteractionType))
            {
                (buttonScript.ParentNode.GetChildByName("disable_icon") as MImageUI).Image.Visible = true;
            }
            else
            {
                (buttonScript.ParentNode.GetChildByName("disable_icon") as MImageUI).Image.Visible = false;
            }

            buttonScript.ParentNode.Tooltips = GenerateInteractionTooltip(interactable, interactionData);
        }

        private string GenerateInteractionTooltip(Interactable interactable, InteractionData interactionData)
        {
            if (interactable == null) return "";

            if (interactionData == null) return "";

            InteractionType interactionType = interactionData.InteractionType;

            if (interactable.AvailableInteractions.Contains(interactionType) == false) return "";

            string tooltip = "";

            if (interactable.IsInteractionMarked(interactionType))
            {
                tooltip += Localization.GetLocalizedText("cancel_x", interactionData.DisplayName.Paint(Color.Orange))
                    .Paint(Color.Red);
            }
            else
            {
                tooltip += interactionData.DisplayName.Paint(Color.Orange);
            }

            LaborType laborType = interactable.GetAssociatedLaborType(interactionType);

            tooltip += $"\n{Localization.GetLocalizedText("labor_type")}: {Labor.GetLaborString(laborType)}";

            if (interactable.DoesInteractionRequireTool(interactionType))
            {
                tooltip += $"\n{Localization.GetLocalizedText("tool_required")}:".Paint(Color.Yellow);

                var itemsList = ItemDatabase.GetInteractionTypeTools(CreatureType.Settler, interactionType);
                for (int i = itemsList.Count - 1; i >= 0; i--)
                {
                    Item item = itemsList[i];

                    tooltip += $"\n- {item.Name}";
                }
            }

            return tooltip;
        }
    }
}
