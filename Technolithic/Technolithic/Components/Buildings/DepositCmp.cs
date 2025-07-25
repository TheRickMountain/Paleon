namespace Technolithic
{
    public class DepositCmp : BuildingCmp
    {
        private int _currentStage;

        private DepositData _depositData;

        public DepositCmp(BuildingTemplate buildingTemplate, Direction direction, InteractablesManager interactablesManager) 
            : base(buildingTemplate, direction, interactablesManager)
        {
            _depositData = buildingTemplate.DepositData;
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

                _currentStage++;

                if (_currentStage == _depositData.Stages)
                {
                    DestructBuilding();
                }
                else
                {
                    Sprite.CurrentAnimation.Frames[0] = _depositData.StagesTextures[_currentStage];
                }
            }
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            SetStage(0);

            DepositData depositData = BuildingTemplate.DepositData;

            AddAvailableInteraction(depositData.InteractionType, depositData.LaborType, depositData.ToolUsageStatus);
            SetInteractionDuration(depositData.InteractionType, depositData.InteractionDurationInHours * WorldState.MINUTES_PER_HOUR);

            ActivateInteraction(depositData.InteractionType);
        }

        public void SetStage(int stage)
        {
            if (_currentStage == stage) return;

            if (stage >= _depositData.Stages) return;
            
            _currentStage = stage;

            Sprite.CurrentAnimation.Frames[0] = _depositData.StagesTextures[_currentStage];
        }

        public override BuildingSaveData GetSaveData()
        {
            var saveData = base.GetSaveData();

            if (IsBuilt)
            {
                saveData.DepositCurrentStage = _currentStage;
            }

            return saveData;
        }
    }
}
