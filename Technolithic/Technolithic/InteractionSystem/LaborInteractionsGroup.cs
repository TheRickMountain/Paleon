using System;
using System.Collections.Generic;

namespace Technolithic
{
    public class LaborInteractionsGroup
    {

        private Dictionary<LaborType, List<(Interactable, InteractionType)>> interactables;

        public LaborInteractionsGroup()
        {
            interactables = new();

            foreach (LaborType laborType in Enum.GetValues(typeof(LaborType)))
            {
                interactables.Add(laborType, new List<(Interactable, InteractionType)>());
            }
        }

        public void Add(Interactable interactable, LaborType laborType, InteractionType interactionType)
        {
            interactables[laborType].Add((interactable, interactionType));

            ResortByPriority(laborType);
        }

        public void Remove(Interactable interactable, LaborType laborType, InteractionType interactionType)
        {
            interactables[laborType].Remove((interactable, interactionType));
        }

        public IReadOnlyList<(Interactable, InteractionType)> GetInteractionPairs(LaborType laborType)
        {
            return interactables[laborType];
        }

        public void ResortByPriority(LaborType laborType)
        {
            var interactionList = interactables[laborType];
            interactionList.Sort((a, b) => b.Item1.Priority.CompareTo(a.Item1.Priority));
        }
    }
}
