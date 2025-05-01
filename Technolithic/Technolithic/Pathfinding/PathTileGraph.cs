using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class PathTileGraph<T>
    {
        public Dictionary<T, Node<T>> Nodes;

        public PathTileGraph(T[,] tiles)
        {
            Nodes = new Dictionary<T, Node<T>>();

            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    T t = tiles[x, y];

                    Node<T> n = new Node<T>();
                    n.Data = t;
                    Nodes.Add(t, n);
                }
            }
        }
    }
}
