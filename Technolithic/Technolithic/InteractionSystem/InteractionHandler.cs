using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Technolithic
{
    public class InteractionHandler
    {

        private HashSet<InteractionType> availableInteractions;

        private HashSet<InteractionType> activatedInteractions;
        private HashSet<InteractionType> markedInteractions;

        private Dictionary<InteractionType, float> interactionDuration;
        private Dictionary<InteractionType, float> interactionProgress;
        private Dictionary<InteractionType, ToolUsageStatus> interactionToolUsageStatus;

        public IReadOnlySet<InteractionType> AvailableInteractions => availableInteractions;

        public InteractionHandler()
        {
            availableInteractions = new();

            activatedInteractions = new();
            markedInteractions = new();

            interactionDuration = new();
            interactionProgress = new();
            interactionToolUsageStatus = new();
        }

        public void AddAvailableInteraction(InteractionType interactionType, ToolUsageStatus toolUsageStatus)
        {
            availableInteractions.Add(interactionType);

            interactionDuration.Add(interactionType, 1.0f);
            interactionProgress.Add(interactionType, 0.0f);
            interactionToolUsageStatus.Add(interactionType, toolUsageStatus);
        }

        public void RemoveInteraction(InteractionType interactionType)
        {
            availableInteractions.Remove(interactionType);

            activatedInteractions.Remove(interactionType);
            markedInteractions.Remove(interactionType);

            interactionDuration.Remove(interactionType);
            interactionProgress.Remove(interactionType);
            interactionToolUsageStatus.Remove(interactionType);
        }

        public ToolUsageStatus GetInteractionToolUsageStatus(InteractionType interactionType)
        {
            if (interactionToolUsageStatus.ContainsKey(interactionType) == false) return ToolUsageStatus.NotUsed;

            return interactionToolUsageStatus[interactionType];
        }

        public bool IsInteractionActivated(InteractionType interactionType)
        {
            return activatedInteractions.Contains(interactionType);
        }

        public void ActivateInteraction(InteractionType interactionType)
        {
            activatedInteractions.Add(interactionType);
        }

        public void DeactivateInteraction(InteractionType interactionType)
        {
            activatedInteractions.Remove(interactionType);
        }

        public bool IsInteractionMarked(InteractionType interactionType)
        {
            return markedInteractions.Contains(interactionType);
        }

        public void MarkInteraction(InteractionType interactionType)
        {
            markedInteractions.Add(interactionType);
        }

        public void UnmarkInteraction(InteractionType interactionType)
        {
            markedInteractions.Remove(interactionType);
        }
    
        public float GetInteractionDuration(InteractionType interactionType)
        {
            return interactionDuration[interactionType];
        }

        public void SetInteractionDuration(InteractionType interactionType, float duration)
        {
            interactionDuration[interactionType] = duration;
        }

        public float GetInteractionProgress(InteractionType interactionType)
        {
            return interactionProgress[interactionType];
        }

        public void SetInteractionProgress(InteractionType interactionType, float progress)
        {
            float duration = GetInteractionDuration(interactionType);
            interactionProgress[interactionType] = MathHelper.Clamp(progress, 0.0f, duration);
        }
    }
}
