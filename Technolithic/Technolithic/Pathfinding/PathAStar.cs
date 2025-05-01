using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public static class PathAStar
    {

        public static PathTileGraph<Tile> PathTileGraph { get; private set; }

        public static void Initialize(PathTileGraph<Tile> pathTileGraph)
        {
            PathTileGraph = pathTileGraph;

        }

        public static TilePath CreateStrengthPath(Tile startTile, Tile targetTile)
        {
            Dictionary<Tile, Node<Tile>> nodes = PathTileGraph.Nodes;

            if (!nodes.ContainsKey(startTile) || !nodes.ContainsKey(targetTile))
            {
                Console.WriteLine("Node hasn't start or end tile");
                return null;
            }

            Node<Tile> startNode = nodes[startTile];

            Node<Tile> targetNode = nodes[targetTile];

            Heap<Node<Tile>> openSet = new Heap<Node<Tile>>(nodes.Count);
            HashSet<Node<Tile>> closedSet = new HashSet<Node<Tile>>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node<Tile> currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode.Equals(targetNode))
                {
                    return new TilePath(RetracePath(startNode, currentNode), targetTile, false);
                }

                foreach (Tile n in currentNode.Data.GetAllNeighbourTiles())
                {
                    Node<Tile> neighbourNode = nodes[n];

                    if (IsClippingCorner(currentNode.Data, n))
                        continue;

                    if (closedSet.Contains(neighbourNode))
                        continue;

                    int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbourNode) + neighbourNode.Data.StrengthValue;
                    if (newMovementCostToNeighbour < neighbourNode.GCost || !openSet.Contains(neighbourNode))
                    {
                        neighbourNode.GCost = newMovementCostToNeighbour;
                        neighbourNode.HCost = GetDistance(neighbourNode, targetNode);
                        neighbourNode.Parent = currentNode;

                        if (!openSet.Contains(neighbourNode))
                            openSet.Add(neighbourNode);
                        else
                            openSet.UpdateItem(neighbourNode);
                    }
                }
            }

            return null;
        }

        public static TilePath CreatePath(Tile startTile, Tile targetTile, bool adjacent)
        {
            Dictionary<Tile, Node<Tile>> nodes = PathTileGraph.Nodes;

            if (!nodes.ContainsKey(startTile) || !nodes.ContainsKey(targetTile))
            {
                Console.WriteLine("Node hasn't start or end tile");
                return null;
            }

            Node<Tile> startNode = nodes[startTile];

            Node<Tile> targetNode = nodes[targetTile];

            Heap<Node<Tile>> openSet = new Heap<Node<Tile>>(nodes.Count);
            HashSet<Node<Tile>> closedSet = new HashSet<Node<Tile>>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node<Tile> currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode.Equals(targetNode))
                {
                    return new TilePath(RetracePath(startNode, currentNode), targetTile, adjacent);
                }

                foreach (Tile n in currentNode.Data.GetAllNeighbourTiles())
                {
                    Node<Tile> neighbourNode = nodes[n];

                    if (IsClippingCorner(currentNode.Data, n))
                        continue;

                    if (adjacent)
                    {
                        if (neighbourNode.Equals(targetNode))
                        {
                            return new TilePath(RetracePath(startNode, currentNode), targetTile, adjacent);
                        }
                    }

                    if (!n.IsWalkable)
                        continue;

                    if (closedSet.Contains(neighbourNode))
                        continue;

                    int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbourNode) + (100 - neighbourNode.Data.MovementSpeedPercent);
                    if (newMovementCostToNeighbour < neighbourNode.GCost || !openSet.Contains(neighbourNode))
                    {
                        neighbourNode.GCost = newMovementCostToNeighbour;
                        neighbourNode.HCost = GetDistance(neighbourNode, targetNode);
                        neighbourNode.Parent = currentNode;

                        if (!openSet.Contains(neighbourNode))
                            openSet.Add(neighbourNode);
                        else
                            openSet.UpdateItem(neighbourNode);
                    }
                }
            }

            return null;
        }

        private static List<Tile> RetracePath(Node<Tile> startNode, Node<Tile> endNode)
        {
            List<Tile> path = new List<Tile>();
            Node<Tile> currentNode = endNode;

            while (!currentNode.Equals(startNode))
            {
                path.Add(currentNode.Data);
                currentNode = currentNode.Parent;
            }

            return path;
        }

        private static int GetDistance(Node<Tile> nodeA, Node<Tile> nodeB)
        {
            int dstX = Math.Abs(nodeA.Data.X - nodeB.Data.X);
            int dstY = Math.Abs(nodeA.Data.Y - nodeB.Data.Y);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);

            return 14 * dstX + 10 * (dstY - dstX);
        }

        private static bool IsClippingCorner(Tile curr, Tile neigh)
        {
            int dX = curr.X - neigh.X;
            int dY = curr.Y - neigh.Y;

            if (Math.Abs(dX) + Math.Abs(dY) == 2)
            {
                Tile firstTile;

                if (dX < 0)
                    firstTile = curr.RightTile;
                else
                    firstTile = curr.LeftTile;

                if (!firstTile.IsWalkable)
                    return true;

                Tile secondTile;

                if (dY < 0)
                    secondTile = curr.BottomTile;
                else
                    secondTile = curr.TopTile;

                if (!secondTile.IsWalkable)
                    return true;
            }

            return false;
        }

    }
}
