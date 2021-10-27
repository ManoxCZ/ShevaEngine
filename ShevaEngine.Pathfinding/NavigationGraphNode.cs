using System.Collections.Generic;

namespace ShevaEngine.Pathfinding
{
    /// <summary>
    /// Navigation graph node.
    /// </summary>
    public abstract class NavigationGraphNode
    {
        /// <summary>Connections.</summary>
        public List<NavigationGraphConnection> Connections;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected NavigationGraphNode()
        {
            Connections = new List<NavigationGraphConnection>();
        }

        /// <summary>
        /// Distance to.
        /// </summary>
        public abstract float DistanceTo(NavigationGraphNode node);
    }
}
