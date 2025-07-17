namespace Technolithic
{
    public class DepositCmp : BuildingCmp
    {
        public int CurrentStage { get; set; }

        private DepositData _depositData;

        public DepositCmp(BuildingTemplate buildingTemplate, Direction direction, InteractablesManager interactablesManager) 
            : base(buildingTemplate, direction, interactablesManager)
        {
            CurrentStage = 0;

            _depositData = buildingTemplate.DepositData;
        }

        public override void Begin()
        {
            base.Begin();

            Sprite.CurrentAnimation.Frames[0] = _depositData.StagesTextures[CurrentStage];
        }

        public override void CompleteInteraction(InteractionType interactionType)
        {
            base.CompleteInteraction(interactionType);

            if (interactionType == _depositData.InteractionType)
            {
                foreach(var kvp in _depositData.RealLoot)
                {
                    GetCenterTile().Inventory.AddCargo(kvp.Key, kvp.Value);
                }

                CurrentStage++;

                if (CurrentStage == _depositData.Stages)
                {
                    DestructBuilding();
                }
                else
                {
                    Sprite.CurrentAnimation.Frames[0] = _depositData.StagesTextures[CurrentStage];
                }
            }
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            DepositData depositData = BuildingTemplate.DepositData;

            AddAvailableInteraction(depositData.InteractionType, depositData.LaborType, depositData.ToolRequired);
            SetInteractionDuration(depositData.InteractionType, depositData.InteractionDurationInHours * WorldState.MINUTES_PER_HOUR);

            ActivateInteraction(depositData.InteractionType);
        }

        public override BuildingSaveData GetSaveData()
        {
            var saveData = base.GetSaveData();

            if (IsBuilt)
            {
                saveData.DepositCurrentStage = CurrentStage;
            }

            return saveData;
        }
    }
}
