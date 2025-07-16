using System.Collections.Generic;

namespace Technolithic
{
    public class InteractableSaveData
    {
        public List<InteractionType> MarkedInteractions;
        public Dictionary<InteractionType, float> InteractionPercentProgressDict;
        public int Priority;
    }
}
