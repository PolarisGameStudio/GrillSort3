using System.Collections.Generic;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    public struct SubNode
    {
        public int id;
        public int score;
        public int cost;
        public int costOrder; // số vị trí cần trong order
    }

    public struct Node
    {
        public int cost;
        public int costOrder; // số vị trí cần trong order
        public List<int> listSubNodes;

        // INSERT_YOUR_CODE
        public static bool operator >(Node a, Node b)
        {
            if (a.cost != b.cost)
                return a.cost > b.cost;
            return a.costOrder > b.costOrder;
        }

        public static bool operator <(Node a, Node b)
        {
            if (a.cost != b.cost)
                return a.cost < b.cost;
            return a.costOrder < b.costOrder;
        }
    }
}