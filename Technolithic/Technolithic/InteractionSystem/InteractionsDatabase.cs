using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace Technolithic
{
    public enum InteractionType
    {
        Chop,
        EmptyFishTrap,
        Destruct
    }

    public class InteractionsDatabase
    {

        private Dictionary<LaborType, List<InteractionData>> laborInteractionsDict = new();

        private List<InteractionData> allInteractions = new();
        private Dictionary<InteractionType, InteractionData> interactionTypeDataDict = new();

        public IReadOnlyList<InteractionData> AllInteractions => allInteractions;

        public InteractionsDatabase()
        {
            AddInteractionData(new InteractionData(
                InteractionType.Chop,
                LaborType.Chop,
                Localization.GetLocalizedText("chop"),
                ResourceManager.ChopIcon,
                Color.Orange));

            AddInteractionData(new InteractionData(InteractionType.EmptyFishTrap,
                LaborType.Fish,
                Localization.GetLocalizedText("automatically_empty_the_trap"),
                ResourceManager.EmptyFishTrapIcon,
                Color.Orange));

            AddInteractionData(new InteractionData(InteractionType.Destruct,
                LaborType.Build,
                Localization.GetLocalizedText("destruct"),
                ResourceManager.DestructIcon,
                Color.Red));
        }

        private void AddInteractionData(InteractionData interactionData)
        {
            Debug.Assert(interactionData != null);

            LaborType laborType = interactionData.LaborType;
            if (laborInteractionsDict.ContainsKey(laborType) == false)
            {
                laborInteractionsDict.Add(laborType, new List<InteractionData>());
            }

            laborInteractionsDict[laborType].Add(interactionData);

            allInteractions.Add(interactionData);

            interactionTypeDataDict.Add(interactionData.InteractionType, interactionData);
        }

        public IReadOnlyList<InteractionData> GetLaborInteractions(LaborType laborType)
        {
            Debug.Assert(laborInteractionsDict.ContainsKey(laborType));

            return laborInteractionsDict[laborType];
        }
    
        public IEnumerable<LaborType> GetInvolvedTypesOfLabor()
        {
            foreach(LaborType laborType in laborInteractionsDict.Keys)
            {
                yield return laborType;
            }
        }

        public InteractionData GetInteractionData(InteractionType interactionType)
        {
            return interactionTypeDataDict[interactionType];
        }
    }
}
