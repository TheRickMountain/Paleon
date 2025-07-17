using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class HutBuildingCmp : BuildingCmp
    {

        public List<CreatureCmp> InsideCreatures { get; private set; }

        public Assignable Assignable { get; private set; }

        public Action<HutBuildingCmp> OnInsiderCreaturesChangedCallback { get; set; }

        private List<Tile> homeAreaTiles;

        private List<CreatureCmp> assignedCreatures;

        public HutBuildingCmp(BuildingTemplate buildingTemplate, Direction direction, InteractablesManager interactablesManager)
            : base(buildingTemplate, direction, interactablesManager)
        {
            Assignable = buildingTemplate.Assignable;
            InsideCreatures = new List<CreatureCmp>();

            assignedCreatures = new List<CreatureCmp>();
            for (int i = 0; i < Assignable.Slots; i++)
            {
                assignedCreatures.Add(null);
            }
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            homeAreaTiles = new List<Tile>();

            GameplayScene.WorldManager.HutBuildingsV2.Add(this);

            Tile centerTile = TilesInfosArray[BuildingTemplate.Width / 2, BuildingTemplate.Height / 2].Tile;

            foreach (var tile in Utils.GetTilesInCircle(centerTile, 6))
            {
                homeAreaTiles.Add(tile);
                tile.HomeArea++;
                if (GameplayScene.WorldManager.HomeArea.Contains(tile) == false)
                {
                    GameplayScene.WorldManager.HomeArea.Add(tile);
                }
            }
        }

        public override void DestructBuilding()
        {
            base.DestructBuilding();

            foreach (var creature in InsideCreatures)
            {
                creature.IsHidden = false;
                creature.Entity.Visible = true;
                creature.EnteredBuilding = null;
            }

            InsideCreatures.Clear();

            UnassignAllCreatures();

            GameplayScene.WorldManager.HutBuildingsV2.Remove(this);

            foreach (var tile in homeAreaTiles)
            {
                tile.HomeArea--;
                if (tile.HomeArea == 0)
                {
                    GameplayScene.WorldManager.HomeArea.Remove(tile);
                }
            }
        }

        public override void UpdateCompleted()
        {
            base.UpdateCompleted();

            if (GameplayScene.Instance.WorldState.CurrentSeason == Season.Winter)
            {
                if (BuildingTemplate.FuelConsumer != null && CurrentFuelCondition > 0 && InsideCreatures.Count > 0)
                {
                    IsTurnedOn = true;
                }
            }

            foreach (var creature in InsideCreatures)
            {
                if (IsTurnedOn)
                {
                    creature.StatusEffectsManager.AddStatusEffect(StatusEffectId.WarmPlace);
                }
                else
                {
                    creature.StatusEffectsManager.RemoveStatusEffect(StatusEffectId.WarmPlace);
                }
            }
        }

        public void Enter(CreatureCmp creature, bool hideCreature = true)
        {
            creature.IsHidden = true;
            creature.Entity.Visible = !hideCreature;
            creature.EnteredBuilding = this;
            InsideCreatures.Add(creature);

            if (InsideCreatures.Count > 0)
            {
                IsTurnedOn = true;
            }

            creature.StatusEffectsManager.AddStatusEffect(Assignable.ApplyStatusEffect);

            OnInsiderCreaturesChangedCallback?.Invoke(this);
        }

        public void Exit(CreatureCmp creature)
        {
            creature.IsHidden = false;
            creature.Entity.Visible = true;
            creature.EnteredBuilding = null;
            InsideCreatures.Remove(creature);

            if (InsideCreatures.Count == 0)
            {
                IsTurnedOn = false;
            }

            creature.StatusEffectsManager.RemoveStatusEffect(Assignable.ApplyStatusEffect);

            creature.StatusEffectsManager.RemoveStatusEffect(StatusEffectId.WarmPlace);

            OnInsiderCreaturesChangedCallback?.Invoke(this);
        }

        public override string GetInformation()
        {
            string info = base.GetInformation();

            if (IsBuilt)
            {
                info += Assignable.GetInformation() + "\n";
            }

            return info;
        }

        public void AssignCreature(CreatureCmp creature)
        {
            if (creature.AssignedHut == this)
                return;

#if DEBUG
            if (creature.AssignedHut != null)
            {
                throw new Exception("Creature is already assigned to a hut");
            }

            if (HasFreeSlots() == false)
            {
                throw new Exception("No free slots");
            }
#endif
            creature.AssignedHut = this;

            int freeSlotIndex = GetFreeSlotIndex();
            assignedCreatures[freeSlotIndex] = creature;
        }

        public void AssignCreature(CreatureCmp creature, int slotIndex)
        {
            if (creature.AssignedHut != null)
            {
                if (creature.AssignedHut == this)
                {
                    return;
                }
                else
                {
                    creature.AssignedHut.UnassignCreature(creature);
                }
            }

            creature.AssignedHut = this;

            if (assignedCreatures[slotIndex] != null)
            {
                assignedCreatures[slotIndex].AssignedHut = null;
            }

            assignedCreatures[slotIndex] = creature;
        }

        public void UnassignCreature(CreatureCmp creature)
        {
#if DEBUG
            if(creature.AssignedHut != this)
            {
                throw new Exception("Creature is not assigned (to this hut)");
            }
#endif
            creature.AssignedHut = null;

            int creatureSlotIndex = GetCreatureSlotIndex(creature);
            assignedCreatures[creatureSlotIndex] = null;
        }

        public bool HasFreeSlots()
        {
            for (int i = 0; i < Assignable.Slots; i++)
            {
                if (assignedCreatures[i] == null)
                {
                    return true;
                }
            }

            return false;
        }

        private void UnassignAllCreatures()
        {
            for (int i = 0; i < Assignable.Slots; i++)
            {
                if(assignedCreatures[i] != null)
                {
                    assignedCreatures[i].AssignedHut = null;
                    assignedCreatures[i] = null;
                }
            }
        }
    
        public CreatureCmp GetSlotCreature(int slotIndex)
        {
            return assignedCreatures[slotIndex];
        }
    
        private int GetFreeSlotIndex()
        {
            for (int i = 0; i < Assignable.Slots; i++)
            {
                if (assignedCreatures[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetCreatureSlotIndex(CreatureCmp creature)
        {
            for (int i = 0; i < Assignable.Slots; i++)
            {
                if (assignedCreatures[i] == creature)
                {
                    return i;
                }
            }
            return -1;
        }

        public override BuildingSaveData GetSaveData()
        {
            var saveData = base.GetSaveData();

            saveData.HutAssignedCreatures = new List<Guid>();
            for (int i = 0; i < Assignable.Slots; i++)
            {
                if(assignedCreatures[i] != null)
                {
                    saveData.HutAssignedCreatures.Add(assignedCreatures[i].Id);
                }
                else
                {
                    saveData.HutAssignedCreatures.Add(Guid.Empty);
                }
            }

            return saveData;
        }
    }
}
