namespace ShevaEngine.Pathfinding
{
    /// <summary>
    /// Navigation graph connection.
    /// </summary>
    public class NavigationGraphConnection
    {
        public NavigationGraphNode Node1;
        public NavigationGraphNode Node2;
        public float Cost;

        /// <summary>
        /// Constructor.
        /// </summary>
        public NavigationGraphConnection(NavigationGraphNode node1, NavigationGraphNode node2, float cost)
        {
            Node1 = node1;
            Node2 = node2;
            Cost = cost;
        }
    }
}
