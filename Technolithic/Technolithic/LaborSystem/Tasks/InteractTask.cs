using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class InteractTask : Task
    {
        private Interactable _interactable;
        private InteractionData _interactionData;
        private InteractionType _interactionType;
        private bool _isEndless;

        private bool _isInteractionStarted = false;

        public InteractTask(CreatureCmp creatureCmp, Interactable interactable, InteractionData interactionData) 
            : base(creatureCmp)
        {
            _interactable = interactable;
            _interactionData = interactionData;
            _interactionType = _interactionData.InteractionType;
            _isEndless = (int)interactable.GetInteractionDuration(_interactionType) <= 0;
        }

        public override void Begin()
        {
            _interactable.Reserve();
        }

        public override void BeforeUpdate()
        {
            Tile tile = _interactable.GetApproachableTile(Owner);
            if (tile != null)
            {
                Owner.Movement.SetPath(tile, false);
                Owner.Slider.SetValue(0, 1.0f, _interactable.GetInteractionProgressPercent(_interactionType), 
                    _interactionData.ProgressBarColor);
            }
            else
            {
                State = TaskState.Failed;
            }

            Owner.CreatureEquipment.ToolItemContainer = Owner.CreatureEquipment.TryGetTool(_interactionType);
        }

        public override void UpdateTask()
        {
            if (IsTaskValid() == false)
            {
                State = TaskState.Failed;
                return;
            }

            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        if (_isInteractionStarted == false)
                        {
                            _isInteractionStarted = true;

                            _interactable.OnInteractionStarted(_interactionType, Owner);
                        }

                        if (_isEndless)
                        {
                            _interactable.ProcessInteraction(_interactionType, Owner);
                            return;
                        }
                        
                        Owner.Slider.Active = true;

                        float efficiency = Owner.CreatureEquipment.GetInteractionEfficiency(_interactionType);

                        float progressPerFrame = efficiency * Engine.GameDeltaTime;

                        float interactionProgress = _interactable.GetInteractionProgress(_interactionType);

                        interactionProgress += progressPerFrame;

                        _interactable.SetInteractionProgress(_interactionType, interactionProgress);

                        float interactionDuration = _interactable.GetInteractionDuration(_interactionType);
                        interactionProgress = _interactable.GetInteractionProgress(_interactionType);

                        Owner.Slider.SetValue(0, 1.0f, _interactable.GetInteractionProgressPercent(_interactionType),
                            _interactionData.ProgressBarColor);

                        _interactable.ProcessInteraction(_interactionType, Owner);

                        if (interactionProgress >= interactionDuration)
                        {
                            _interactable.CompleteInteraction(_interactionType);

                            _interactable.ResetProgress(_interactionType);

                            Owner.CreatureEquipment.DecreaseToolDurability(_interactionType, 1);

                            Owner.Slider.Active = false;

                            State = TaskState.Success;
                        }
                    }
                    break;
                case MovementState.Failed:
                    {

                    }
                    break;
                case MovementState.Running:
                    {
                        State = TaskState.Running;
                    }
                    break;
            }
        }

        public override void Complete()
        {
            base.Complete();

            Owner.Movement.ResetPath();

            Owner.Slider.Active = false;

            _interactable.Unreserve();

            Owner.CreatureEquipment.ToolItemContainer = null;

            _interactable.OnInteractionEnded(_interactionType, Owner);
        }

        public override void Cancel()
        {
            base.Cancel();

            Owner.Movement.ResetPath();

            Owner.Slider.Active = false;

            _interactable.Unreserve();

            Owner.CreatureEquipment.ToolItemContainer = null;

            _interactable.OnInteractionEnded(_interactionType, Owner);
        }

        private bool IsTaskValid()
        {
            return _interactable.IsInteractionMarked(_interactionType) &&
                _interactable.IsInteractionActivated(_interactionType);
        }
    }
}
