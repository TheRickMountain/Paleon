using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Technolithic
{
    public abstract class Interactable : Component
    {
        public const int MIN_PRIORITY = 1;
        public const int MAX_PRIORITY = 9;
        public const int DEFAULT_PRIORITY = 5;

        private InteractionHandler _interactionHandler;

        private InteractablesManager _interactablesManager;

        private bool _isReserved = false;
        private bool _isDestroyed = false;

        private Dictionary<InteractionType, LaborType> _interactionLaborMap = new();
        private Dictionary<InteractionType, List<Item>> _interactionItemsMap = new();
        private HashSet<InteractionType> _interactionsThatRequireItems = new();

        private int _priority = DEFAULT_PRIORITY;
        public int Priority 
        {
            get => _priority;
            set
            {
                int newPriority = MathHelper.Clamp(value, MIN_PRIORITY, MAX_PRIORITY);

                if (_priority == newPriority) return;

                _priority = newPriority;

                // TODO: resort entities in _interactablesManager
            }
        }

        public IReadOnlySet<InteractionType> AvailableInteractions => _interactionHandler.AvailableInteractions;

        public Interactable(bool active, bool visible) : base(active, visible)
        {
            _interactionHandler = new InteractionHandler();
        }

        public void SetInteractablesManager(InteractablesManager interactablesManager)
        {
            _interactablesManager = interactablesManager;
        }

        protected void AddAvailableInteraction(InteractionType interactionType, LaborType associatedLaborType, bool toolRequired)
        {
            if (_isDestroyed) return;

            Debug.Assert(_interactionLaborMap.ContainsKey(interactionType) == false,
                "The interaction type is already associated with another labor type");

            _interactionLaborMap.Add(interactionType, associatedLaborType);

            _interactionHandler.AddAvailableInteraction(interactionType, toolRequired);
        }

        protected void SetInteractionItems(InteractionType interactionType, bool isItemRequired, params Item[] items)
        {
            _interactionItemsMap.Add(interactionType, items.ToList());

            if (isItemRequired)
            {
                _interactionsThatRequireItems.Add(interactionType);
            }
        }

        public IReadOnlyList<Item> GetInteractionItems(InteractionType interactionType)
        {
            if (_interactionItemsMap.ContainsKey(interactionType) == false) return null;

            return _interactionItemsMap[interactionType];
        }

        public bool DoesInteractionRequireItems(InteractionType interactionType)
        {
            return _interactionsThatRequireItems.Contains(interactionType);
        }

        public LaborType GetAssociatedLaborType(InteractionType interactionType)
        {
            if (_interactionLaborMap.ContainsKey(interactionType) == false)
            {
                return LaborType.None;
            }

            return _interactionLaborMap[interactionType];
        }

        public bool DoesInteractionRequireTool(InteractionType interactionType)
        {
            return _interactionHandler.DoesInteractionRequireTool(interactionType);
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

            LaborType associatedLaborType = _interactionLaborMap[interactionType];

            _interactablesManager.AddInteractable(this, associatedLaborType, interactionType);
        }

        protected void DeactivateInteraction(InteractionType interactionType)
        {
            if (CanDeactivateInteraction(interactionType) == false) return;

            _interactionHandler.DeactivateInteraction(interactionType);

            if (ShouldRemoveFromManager(interactionType) == false) return;

            LaborType associatedLaborType = _interactionLaborMap[interactionType];

            _interactablesManager.RemoveInteractable(this, associatedLaborType, interactionType);
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

            LaborType associatedLaborType = _interactionLaborMap[interactionType];

            _interactablesManager.AddInteractable(this, associatedLaborType, interactionType);
        }

        public void UnmarkInteraction(InteractionType interactionType)
        {
            if (CanUnmarkInteraction(interactionType) == false) return;

            _interactionHandler.UnmarkInteraction(interactionType);

            if (ShouldRemoveFromManager(interactionType) == false) return;

            LaborType associatedLaborType = _interactionLaborMap[interactionType];

            _interactablesManager.RemoveInteractable(this, associatedLaborType, interactionType);
        }

        public float GetInteractionDuration(InteractionType interactionType)
        {
            return _interactionHandler.GetInteractionDuration(interactionType);
        }

        /// <summary>
        /// Sets the time required to complete a specific interaction type.
        /// </summary>
        /// <param name="interactionType">The type of interaction to modify.</param>
        /// <param name="duration">The time in seconds required to complete the interaction. A value of 0 
        /// indicates an endless interaction that will not complete automatically over time.</param>
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

        public void SetInteractionProgressPercent(InteractionType interactionType, float progressPercent)
        {
            float progress = GetInteractionDuration(interactionType) * progressPercent;
            SetInteractionProgress(interactionType, progress);
        }

        public void ResetProgress(in InteractionType interactionType)
        {
            SetInteractionProgress(interactionType, 0);
        }

        public virtual void OnInteractionStarted(InteractionType interactionType, CreatureCmp creature) { }

        public virtual void CompleteInteraction(InteractionType interactionType) { }

        public virtual void ProcessInteraction(InteractionType interactionType, CreatureCmp creature) { }

        public virtual void OnInteractionEnded(InteractionType interactionType, CreatureCmp creature) { }

        public void Reserve()
        {
            if (_isReserved || _isDestroyed) return;

            _isReserved = true; 

            foreach(InteractionType interactionType in _interactionHandler.AvailableInteractions)
            {
                LaborType associatedLaborType = _interactionLaborMap[interactionType];

                _interactablesManager.RemoveInteractable(this, associatedLaborType, interactionType);
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
                    LaborType associatedLaborType = _interactionLaborMap[interactionType];

                    _interactablesManager.AddInteractable(this, associatedLaborType, interactionType);
                }
            }
        }

        public abstract Tile GetApproachableTile(CreatureCmp creature);

        public abstract Tile GetApproachableTile(int zoneId);

        public abstract IEnumerable<Tile> GetApproachableTiles();

        public void Destroy()
        {
            if (_isDestroyed) return;

            foreach (InteractionType interactionType in _interactionHandler.AvailableInteractions)
            {
                UnmarkInteraction(interactionType);

                DeactivateInteraction(interactionType);

                LaborType associatedLaborType = _interactionLaborMap[interactionType];

                _interactablesManager.RemoveInteractable(this, associatedLaborType, interactionType);
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

        public void FillSaveData(InteractableSaveData saveData)
        {
            saveData.MarkedInteractions = new List<InteractionType>();
            saveData.InteractionPercentProgressDict = new Dictionary<InteractionType, float>();
            saveData.Priority = Priority;
            foreach (InteractionType interactionType in AvailableInteractions)
            {
                if (IsInteractionMarked(interactionType))
                {
                    saveData.MarkedInteractions.Add(interactionType);
                }

                float interactionProgress = GetInteractionProgressPercent(interactionType);
                if (interactionProgress > 0)
                {
                    saveData.InteractionPercentProgressDict.Add(interactionType, interactionProgress);
                }
            }
        }
    }
}
