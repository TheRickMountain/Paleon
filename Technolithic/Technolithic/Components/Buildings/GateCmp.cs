using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public enum GateState
    {
        Opened,
        Closed
    }

    public class GateCmp : BuildingCmp
    {

        public GateState State { get; private set; } = GateState.Opened;

        public GateCmp(BuildingTemplate buildingTemplate, Direction direction) 
            : base(buildingTemplate, direction)
        {
        }

        public void SetState(GateState state)
        {
            if (State == state)
                return;

            State = state;

            switch(State)
            {
                case GateState.Opened:
                    foreach (var tileInfo in TilesInfosList)
                    {
                        tileInfo.Tile.IsWalkable = true;
                    }
                    break;
                case GateState.Closed:
                    foreach (var tileInfo in TilesInfosList)
                    {
                        tileInfo.Tile.IsWalkable = false;
                    }
                    break;
            }
        }

        public override void UpdateCompleted()
        {
            base.UpdateCompleted();

            IsTurnedOn = State == GateState.Closed;
        }

    public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            SetState(GateState.Opened);
        }

        public override BuildingSaveData GetSaveData()
        {
            BuildingSaveData saveData = base.GetSaveData();

            if (IsBuilt)
            {
                saveData.GateState = State;
            }

            return saveData;
        }

    }
}
