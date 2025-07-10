using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class InteractTask : Task
    {
        private Interactable _interactable;
        private InteractionType _interactionType;

        public InteractTask(CreatureCmp creatureCmp, Interactable interactable, InteractionType interactionType) 
            : base(creatureCmp)
        {
            _interactable = interactable;
            _interactionType = interactionType;
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
                Owner.Slider.SetValue(0, 1.0f, _interactable.GetInteractionProgressPercent(_interactionType), Color.Orange);
            }
            else
            {
                State = TaskState.Failed;
            }
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
                        Owner.Slider.Active = true;

                        float efficiency = Owner.CreatureEquipment.GetInteractionEfficiency(_interactionType);

                        float progressPerFrame = efficiency * Engine.GameDeltaTime;

                        float interactionProgress = _interactable.GetInteractionProgress(_interactionType);

                        interactionProgress += progressPerFrame;

                        _interactable.SetInteractionProgress(_interactionType, interactionProgress);

                        float interactionDuration = _interactable.GetInteractionDuration(_interactionType);
                        interactionProgress = _interactable.GetInteractionProgress(_interactionType);

                        Owner.Slider.SetValue(0, 1.0f, _interactable.GetInteractionProgressPercent(_interactionType), Color.Orange);

                        if (interactionProgress >= interactionDuration)
                        {
                            _interactable.CompleteInteraction(_interactionType);

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
        }

        public override void Cancel()
        {
            base.Cancel();

            Owner.Movement.ResetPath();

            Owner.Slider.Active = false;

            _interactable.Unreserve();
        }

        private bool IsTaskValid()
        {
            if (_interactable.IsInteractionMarked(_interactionType) == false) return false;

            if (_interactable.IsInteractionActivated(_interactionType) == false) return false;

            return true;
        }
    }
}
