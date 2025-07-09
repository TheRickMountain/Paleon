using System.Collections.Generic;

namespace Technolithic
{
    public abstract class Interactable : Component
    {
        private InteractionHandler _interactionHandler;

        private InteractablesManager _interactablesManager;

        private bool _isReserved = false;
        private bool _isDestroyed = false;

        public IReadOnlySet<InteractionType> AvailableInteractions => _interactionHandler.AvailableInteractions;

        public Interactable(bool active, bool visible) : base(active, visible)
        {
            _interactionHandler = new InteractionHandler();
        }

        public void SetInteractablesManager(InteractablesManager interactablesManager)
        {
            _interactablesManager = interactablesManager;
        }

        protected void AddAvailableInteraction(InteractionType interactionType)
        {
            if (_isDestroyed) return;

            _interactionHandler.AddAvailableInteraction(interactionType);
        }

        public bool IsInteractionActivated(InteractionType interactionType)
        {
            return _interactionHandler.IsInteractionActivated(interactionType);
        }

        protected void ActivateInteraction(InteractionType interactionType)
        {
            if (CanActivateInteraction(interactionType) == false) return;

            _interactionHandler.ActivateInteraction(interactionType);

            if (_isReserved) return;

            if (IsInteractionMarked(interactionType) == false) return;

            _interactablesManager?.AddInteractable(interactionType, this);
        }

        protected void DeactivateInteraction(InteractionType interactionType)
        {
            if (CanDeactivateInteraction(interactionType) == false) return;

            _interactionHandler.DeactivateInteraction(interactionType);

            if (ShouldRemoveFromManager(interactionType) == false) return;

            _interactablesManager.RemoveInteractable(interactionType, this);
        }

        public bool IsInteractionMarked(InteractionType interactionType)
        {
            return _interactionHandler.IsInteractionMarked(interactionType);
        }

        public void MarkInteraction(InteractionType interactionType)
        {
            if (CanMarkInteraction(interactionType) == false) return;

            _interactionHandler.MarkInteraction(interactionType);

            // INFO: Since interactable is reserved, it should not be available for interaction. 
            if (_isReserved) return;

            if (_interactionHandler.IsInteractionActivated(interactionType) == false) return;

            _interactablesManager?.AddInteractable(interactionType, this);
        }

        public void UnmarkInteraction(InteractionType interactionType)
        {
            if (CanUnmarkInteraction(interactionType) == false) return;

            _interactionHandler.UnmarkInteraction(interactionType);

            if (ShouldRemoveFromManager(interactionType) == false) return;

            _interactablesManager.RemoveInteractable(interactionType, this);
        }

        public float GetInteractionDuration(InteractionType interactionType)
        {
            return _interactionHandler.GetInteractionDuration(interactionType);
        }

        protected void SetInteractionDuration(InteractionType interactionType, float duration)
        {
            _interactionHandler.SetInteractionDuration(interactionType, duration);
        }

        public float GetInteractionProgress(InteractionType interactionType)
        {
            return _interactionHandler.GetInteractionProgress(interactionType);
        }

        public float GetInteractionProgressPercent(InteractionType interactionType)
        {
            float duration = GetInteractionDuration(interactionType);
            float progress = GetInteractionProgress(interactionType);

            return progress / duration;
        }

        public void SetInteractionProgress(InteractionType interactionType, float progress)
        {
            _interactionHandler.SetInteractionProgress(interactionType, progress);
        }

        public virtual void CompleteInteraction(InteractionType interactionType) { }

        public void Reserve()
        {
            if (_isReserved || _isDestroyed) return;

            _isReserved = true; 

            foreach(InteractionType interactionType in _interactionHandler.AvailableInteractions)
            {
                _interactablesManager.RemoveInteractable(interactionType, this);
            }
        }

        public void Unreserve()
        {
            if (_isReserved == false || _isDestroyed) return;

            _isReserved = false;

            foreach (InteractionType interactionType in _interactionHandler.AvailableInteractions)
            {
                if(_interactionHandler.IsInteractionMarked(interactionType) && 
                    _interactionHandler.IsInteractionActivated(interactionType))
                {
                    _interactablesManager.AddInteractable(interactionType, this);
                }
            }
        }

        public abstract Tile GetApproachableTile(CreatureCmp creature);

        public abstract Tile GetApproachableTile();

        public void Destroy()
        {
            if (_isDestroyed) return;

            foreach (InteractionType interactionType in _interactionHandler.AvailableInteractions)
            {
                UnmarkInteraction(interactionType);

                DeactivateInteraction(interactionType);

                _interactablesManager.RemoveInteractable(interactionType, this);
            }

            _isDestroyed = true;
        }

        #region Helpres

        private bool CanActivateInteraction(InteractionType interactionType)
        {
            if (_isDestroyed) return false;

            return _interactionHandler.AvailableInteractions.Contains(interactionType) &&
                (_interactionHandler.IsInteractionActivated(interactionType) == false);
        }

        private bool CanDeactivateInteraction(InteractionType interactionType)
        {
            if (_isDestroyed) return false;

            return _interactionHandler.AvailableInteractions.Contains(interactionType) &&
                _interactionHandler.IsInteractionActivated(interactionType);
        }

        private bool CanMarkInteraction(InteractionType interactionType)
        {
            if (_isDestroyed) return false;

            return _interactionHandler.AvailableInteractions.Contains(interactionType) &&
                (_interactionHandler.IsInteractionMarked(interactionType) == false);
        }

        private bool CanUnmarkInteraction(InteractionType interactionType)
        {
            if (_isDestroyed) return false;

            return _interactionHandler.AvailableInteractions.Contains(interactionType) &&
                _interactionHandler.IsInteractionMarked(interactionType);
        }

        private bool ShouldRemoveFromManager(InteractionType interactionType)
        {
            if (_isDestroyed || _isReserved) return true;

            return _interactionHandler.IsInteractionMarked(interactionType) == false ||
                _interactionHandler.IsInteractionActivated(interactionType) == false;
        }

        #endregion
    }
}
