using System;
using System.Collections.Generic;

namespace Technolithic
{
    public class InteractablesManager
    {
        private Dictionary<int, LaborInteractionsGroup> zoneLaborGroupDict = new();

        private HashSet<int> zoneIdsBuffer = new();

        public InteractablesManager()
        {
            zoneLaborGroupDict = new();
        }

        public void AddInteractable(Interactable interactable, LaborType laborType, InteractionType interactionType)
        {
            zoneIdsBuffer.Clear();

            foreach (Tile tile in interactable.GetApproachableTiles())
            {
                zoneIdsBuffer.Add(tile.GetRoomId());
            }

            foreach (int zoneId in zoneIdsBuffer)
            {
                if (zoneLaborGroupDict.ContainsKey(zoneId) == false)
                {
                    zoneLaborGroupDict.Add(zoneId, new LaborInteractionsGroup());
                }

                zoneLaborGroupDict[zoneId].Add(interactable, laborType, interactionType);
            }
        }

        public void RemoveInteractable(Interactable interactable, LaborType laborType, InteractionType interactionType)
        {
            zoneIdsBuffer.Clear();

            foreach (Tile tile in interactable.GetApproachableTiles())
            {
                zoneIdsBuffer.Add(tile.GetRoomId());
            }

            foreach (int zoneId in zoneIdsBuffer)
            {
                if (zoneLaborGroupDict.ContainsKey(zoneId) == false) continue;

                zoneLaborGroupDict[zoneId].Remove(interactable, laborType, interactionType);
            }
        }

        public void ResortByPriority(Interactable interactable)
        {
            zoneIdsBuffer.Clear();

            foreach (Tile tile in interactable.GetApproachableTiles())
            {
                zoneIdsBuffer.Add(tile.GetRoomId());
            }

            foreach(int zoneId in zoneIdsBuffer)
            {
                if (zoneLaborGroupDict.TryGetValue(zoneId, out LaborInteractionsGroup laborInteractionsGroup))
                {
                    foreach(InteractionType interactionType in interactable.AvailableInteractions)
                    {
                        LaborType laborType = interactable.GetAssociatedLaborType(interactionType);

                        laborInteractionsGroup.ResortByPriority(laborType);
                    }
                }
            }
        }

        public IReadOnlyList<(Interactable, InteractionType)> GetInteractionPairs(int zoneId, LaborType laborType)
        {
            if (zoneLaborGroupDict.ContainsKey(zoneId) == false) return null;

            return zoneLaborGroupDict[zoneId].GetInteractionPairs(laborType);
        }
    }
}
