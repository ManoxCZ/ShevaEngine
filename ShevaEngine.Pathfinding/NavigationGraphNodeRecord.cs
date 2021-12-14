using System.Collections.Generic;

namespace ShevaEngine.Pathfinding
{
    /// <summary>
    /// Navigation graph node record.
    /// </summary>
    public class NavigationGraphNodeRecord
    {
        public NavigationGraphNode Node;
        public float CostSoFar;
        public float EstimatedTotalCost;
        public NavigationGraphConnection? Connection;

        /// <summary>
        /// Constructor.
        /// </summary>
        public NavigationGraphNodeRecord(NavigationGraphNode node, NavigationGraphConnection? connection = null,
            float costSoFar = 0.0f, float estimatedTotalCost = float.MaxValue)
        {
            Node = node;
            Connection = connection;
            CostSoFar = costSoFar;
            EstimatedTotalCost = estimatedTotalCost;
        }
    }

    /// <summary>
    /// Navigation graph node record equality comparer.
    /// </summary>
    public class NavigationGraphNodeRecordEqualityComparer : IEqualityComparer<NavigationGraphNodeRecord>
    {
        public static readonly NavigationGraphNodeRecordEqualityComparer Instance = new NavigationGraphNodeRecordEqualityComparer();

        /// <summary>
        /// Method compares two navigation graph node records.
        /// </summary>
        public bool Equals(NavigationGraphNodeRecord? x, NavigationGraphNodeRecord? y)
        {
            return x?.Node == y?.Node;
        }

        /// <summary>
        /// Get hash code.
        /// </summary>        
        public int GetHashCode(NavigationGraphNodeRecord obj)
        {
            return base.GetHashCode();
        }
    }
}
