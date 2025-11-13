using System.Collections.Generic;

namespace MyGame.SkewerJam.Gameplay.LogicOrder
{
    public struct NodeAsOneLayerInGrill
    {
        public int id;
        public int rootId;
        public int cost;
        public List<int> listCostOrder;
        public int score;
    }

    public struct Node
    {
        public int totalCost;
        public List<int> listRootIds;
        public Dictionary<int, List<int>> dictCostOrder; // rootId -> listCostOrder

        // INSERT_YOUR_CODE
        public static bool operator >(Node a, Node b)
        {
            return a.totalCost > b.totalCost;
        }

        public static bool operator <(Node a, Node b)
        {
            return a.totalCost < b.totalCost;
        }
    }
}