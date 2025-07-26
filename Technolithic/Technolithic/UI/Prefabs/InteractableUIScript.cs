using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Technolithic
{
    public abstract class InteractableUIScript : MScript
    {
        private Interactable selectedInteractable;

        private Dictionary<InteractionType, TBigButtonUI> interactionButtonDict = new();

        public InteractableUIScript() : base(true)
        {
            
        }

        public void SetInteractable(Interactable interactable)
        {
            selectedInteractable = interactable;

            // TODO: refactoring required
            foreach (InteractionType interactionType in selectedInteractable.AvailableInteractions)
            {
                TBigButtonUI interactionButton;

                InteractionData interactionData = Engine.InteractionsDatabase.GetInteractionData(interactionType);

                if (interactionButtonDict.ContainsKey(interactionType) == false)
                {
                    interactionButton = new TBigButtonUI(ParentNode.Scene);
                    interactionButton.Icon = interactionData.Icon;
                    interactionButtonDict[interactionType] = interactionButton;
                    interactionButton.ButtonUp += Interactable_InteractionButton_Pressed;
                    interactionButton.SetMetadata("interaction_data", interactionData);
                }

                interactionButton = interactionButtonDict[interactionType];
                interactionButton.SetMetadata("interactable", selectedInteractable);

                if (selectedInteractable.IsInteractionMarked(interactionType))
                {
                    interactionButton.ExtraIcon = ResourceManager.DisableIcon;
                }
                else
                {
                    interactionButton.ExtraIcon = null;
                }

                interactionButton.Tooltips = GenerateInteractionTooltip(selectedInteractable, interactionData);

                ListViewUIScript buttonsListView = ParentNode.GetChildByName("ButtonsListView").GetComponent<ListViewUIScript>();
                buttonsListView.AddItem(interactionButton);
            }
        }

        private void Interactable_InteractionButton_Pressed(TButtonUI button)
        {
            InteractionData interactionData = button.GetMetadata<InteractionData>("interaction_data");
            Interactable interactable = button.GetMetadata<Interactable>("interactable");
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
                button.ExtraIcon = ResourceManager.DisableIcon;
            }
            else
            {
                button.ExtraIcon = null;
            }

            button.Tooltips = GenerateInteractionTooltip(interactable, interactionData);
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
            if (laborType != LaborType.None)
            {
                tooltip += $"\n{Localization.GetLocalizedText("labor_type")}: {Labor.GetLaborString(laborType)}";
            }

            switch (interactable.GetInteractionToolUsageStatus(interactionType))
            {
                case ToolUsageStatus.Required:
                    {
                        tooltip += $"\n{Localization.GetLocalizedText("tool_required")}:".Paint(Color.Yellow);
                    }
                    break;
                case ToolUsageStatus.Optional:
                    {
                        tooltip += $"\n{Localization.GetLocalizedText("tool_optional")}:".Paint(Color.Yellow);
                    }
                    break;
            }

            switch (interactable.GetInteractionToolUsageStatus(interactionType))
            {
                case ToolUsageStatus.Required:
                case ToolUsageStatus.Optional:
                    {
                        // TODO: show animal tools too
                        var itemsList = ItemDatabase.GetInteractionTypeTools(CreatureType.Settler, interactionType);
                        for (int i = itemsList.Count - 1; i >= 0; i--)
                        {
                            Item item = itemsList[i];

                            tooltip += $"\n- {item.Name}";
                        }
                    }
                    break;
            }

            var interactionItems = interactable.GetInteractionItems(interactionType);
            if (interactionItems != null)
            {
                tooltip += $"\n{Localization.GetLocalizedText("item_required")}:".Paint(Color.Yellow);

                foreach (Item item in interactionItems)
                {
                    tooltip += $"\n- {item.Name}";
                }
            }

            return tooltip;
        }
    }
}
