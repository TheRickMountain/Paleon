using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class HideTask : Task
    {
        private Tile targetTile;

        private Timer hideTimer;
        private float currentHideTime;
        private int hideTime = 32;

        private CreatureCmp victim;
        private CreatureCmp hunter;

        public HideTask(CreatureCmp victim, CreatureCmp hunter) : base(victim)
        {
            this.victim = victim;
            this.hunter = hunter;
        }
        public override void Begin()
        {
            hideTimer = new Timer();
        }

        public override void BeforeUpdate()
        {
            Tile targetTile = GetRandomTileToHide();
            Owner.Movement.SetPath(targetTile, false);
        }

        public override void UpdateTask()
        {
                
            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        if (currentHideTime >= hideTime)
                        {
                            currentHideTime = 0;
                            hideTimer.Reset();

                            State = TaskState.Success;
                        }
                        else
                        {
                            targetTile = GetRandomTileToHide();
                            Owner.Movement.SetPath(targetTile, false);
                        }
                    }
                    break;
                case MovementState.Failed:
                    {
                        State = TaskState.Failed;
                    }
                    break;
                case MovementState.Running:
                    {
                        currentHideTime = hideTimer.GetTime();

                        State = TaskState.Running;
                    }
                    break;
            }
        }
    
        public Tile GetRandomTileToHide()
        {
            Room victimRoom = victim.Movement.CurrentTile.Room;
            Room hunterRoom = hunter.Movement.CurrentTile.Room;

            // If animal room has no neighbour rooms, or one neighbour room, which has hunter, than get random tile from current room
            if(victimRoom.Neighbours.Count == 0 || 
                victimRoom.Neighbours.Count == 1 && victimRoom.Neighbours.Contains(hunterRoom))
            {
                return victimRoom.Tiles[MyRandom.Range(0, victimRoom.Tiles.Count - 1)];
            }

            int randomNum = MyRandom.Range(0, victimRoom.Neighbours.Count - 1);
            Room randomRoom = victimRoom.Neighbours[randomNum];

            if(randomRoom == hunterRoom)
            {
                randomNum += 1;

                if (randomNum == victimRoom.Neighbours.Count)
                {
                    randomNum = 0;
                }

                randomRoom = victimRoom.Neighbours[randomNum];
            }

            return randomRoom.Tiles[MyRandom.Range(0, randomRoom.Tiles.Count - 1)];
        }

        public override void Cancel()
        {
            base.Cancel();

            Owner.Movement.ResetPath();
        }
    }
}
