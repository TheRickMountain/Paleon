using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class Node<T> : IHeapItem<Node<T>>
    {
        public T Data;
        public int GCost;
        public int HCost;
        public Node<T> Parent;

        public Node()
        {

        }

        public int FCost
        {
            get
            {
                return GCost + HCost;
            }
        }

        public int HeapIndex
        {
            get; set;
        }

        public int CompareTo(Node<T> nodeToCompare)
        {
            int compare = FCost.CompareTo(nodeToCompare.FCost);
            if (compare == 0)
                compare = HCost.CompareTo(nodeToCompare.HCost);

            return -compare;
        }
    }
}
