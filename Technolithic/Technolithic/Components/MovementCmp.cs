using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MovementCmp
    {
        public Vector2 Position { get; private set; }
        public Tile CurrentTile { get; private set; }
        public MovementState MovementState { get; private set; } = MovementState.Success;

        private Direction direction;
        public Direction Direction
        {
            get { return direction; }
            private set
            {
                if (direction == value)
                    return;

                direction = value;

                cbOnDirectionChanged?.Invoke(direction);
            }
        }

        public float Speed { get; set; }

        public Tile NextTile { get; private set; }
        private float movementPerc;
        private TilePath path;

        private bool rebuildPath;

        private Action<Direction> cbOnDirectionChanged;

        public MovementCmp(Tile initTile)
        {
            Speed = 1;
            Direction = Direction.LEFT;
            CurrentTile = NextTile = initTile;
            Position = new Vector2(CurrentTile.X * Engine.TILE_SIZE, CurrentTile.Y * Engine.TILE_SIZE);
        }

        public void Update()
        {
            switch (MovementState)
            {
                case MovementState.Running:
                    {
                        if (NextTile.Equals(CurrentTile))
                        {
                            if (path.Count == 0)
                            {
                                path = null;

                                MovementState = MovementState.Success;
                            }
                            else
                            {
                                if (rebuildPath)
                                {
                                    rebuildPath = false;
                                    if (!RebuildPath(path))
                                        MovementState = MovementState.Failed;
                                }
                                else
                                {
                                    NextTile = path[path.Count - 1];
                                    path.RemoveAt(path.Count - 1);

                                    if (CurrentTile.X > NextTile.X)
                                        Direction = Direction.LEFT;
                                    else if (CurrentTile.X < NextTile.X)
                                        Direction = Direction.RIGHT;

                                    MovementState = MovementState.Running;
                                }
                            }
                        }

                        UpdateMovement();
                    }
                    break;
                case MovementState.Completion:
                    {
                        if (CurrentTile != NextTile)
                            UpdateMovement();
                        else
                            MovementState = MovementState.Success;
                    }
                    break;
            }
        }

        public void Teleport(Tile tile)
        {
            CurrentTile = NextTile = tile;
            Position = new Vector2(CurrentTile.X * Engine.TILE_SIZE, CurrentTile.Y * Engine.TILE_SIZE);
        }

        public void RebuildPath(Tile tile)
        {
            if (MovementState == MovementState.Running)
            {
                if (path != null && path.Tiles.Contains(tile))
                {
                    rebuildPath = true;
                }
            }
        }

        public void SetPath(Tile targetTile, bool adjacent)
        {
            if (CurrentTile == targetTile)
            {
                MovementState = MovementState.Success;
                return;
            }

            if (IsPathAvailable(targetTile, adjacent) == false)
            {
                path = null;
                MovementState = MovementState.Failed;
                return;
            }

            path = PathAStar.CreatePath(CurrentTile, targetTile, adjacent);
            NextTile = CurrentTile;
            MovementState = MovementState.Running;
        }

        public bool IsPathAvailable(Tile targetTile, bool adjacent)
        {
            if (adjacent)
            {
                foreach (Tile n in targetTile.GetNeighbourTiles())
                    if (n.Room != null && CurrentTile.Room.Id == n.Room.Id)
                        return true;

                return false;
            }

            return targetTile.Room != null && CurrentTile.Room.Id == targetTile.Room.Id;
        }

        public void ResetPath()
        {
            path = null;
            MovementState = MovementState.Completion;
        }

        private void UpdateMovement()
        {
            float distToTravel = MathUtils.Distance(CurrentTile.X, CurrentTile.Y, NextTile.X, NextTile.Y);

            float finalSpeed = (Speed * NextTile.MovementSpeedPercent) / 100.0f;

            float distThisFrame = finalSpeed * Engine.GameDeltaTime;

            float percThisFrame = 0f;

            if (distToTravel > float.Epsilon)
            {
                percThisFrame = distThisFrame / distToTravel;
            }

            movementPerc += percThisFrame;

            if (movementPerc >= 1)
            {
                CurrentTile = NextTile;
                movementPerc = 0;
            }

            float posX = MathUtils.Lerp(CurrentTile.X, NextTile.X, movementPerc) * Engine.TILE_SIZE;
            float posY = MathUtils.Lerp(CurrentTile.Y, NextTile.Y, movementPerc) * Engine.TILE_SIZE;
            Position = new Vector2(posX, posY);
        }

        private bool RebuildPath(TilePath path)
        {
            // Если путь к тайлу существует
            if (path[0].Room != null && CurrentTile.Room.Id == path[0].Room.Id)
            {
                // Проверяем нет ли препятствий на пути
                foreach (Tile tile in path.Tiles)
                {
                    // Если есть, то перестраиваем путь к тайлу
                    if (tile.IsWalkable == false)
                    {
                        SetPath(path.TargetTile, path.Adjacent);
                        return true;
                    }
                }

                return true;
            }

            return false;
        }

        public void RegisterOnDirectionChangedCallback(Action<Direction> callback)
        {
            cbOnDirectionChanged += callback;
        }

        public void UnregisterOnDirectionChangedCallback(Action<Direction> callback)
        {
            cbOnDirectionChanged -= callback;
        }

        public int GetTargetTileDistance()
        {
            return path.Count;
        }

        public Tile GetTargetTile()
        {
            if(path != null)
            {
                return path.TargetTile;
            }
            else
            {
                return CurrentTile;
            }
        }
    }
}
