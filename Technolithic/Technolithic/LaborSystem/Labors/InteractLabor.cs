namespace Technolithic
{
    public class InteractLabor : Labor
    {
        private InteractionsDatabase _intearactionsDatabase;
        private InteractablesManager _interactablesManager;

        public InteractLabor(LaborType laborType, InteractionsDatabase intearactionsDatabase, 
            InteractablesManager interactablesManager) : base(laborType)
        {
            _intearactionsDatabase = intearactionsDatabase;
            _interactablesManager = interactablesManager;
        }

        public override bool Check(CreatureCmp creature)
        {
            int creatureZoneId = creature.GetRoomId();

            foreach(InteractionData interactionData in _intearactionsDatabase.GetLaborInteractions(LaborType))
            {
                InteractionType interactionType = interactionData.InteractionType;

                // TODO: It is necessary to check the necessity of the tool to perform the interaction
                // TODO: Required to get "interactables" list to find the closest one
                Interactable interactable = _interactablesManager.GetFirstInteractable(creatureZoneId, interactionType);

                if (interactable == null) continue;

                InteractTask interactTask = new InteractTask(creature, interactable, interactionType);
                AddTask(creature, interactTask);
                return true;
            }

            return false;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
        }
    }
}
