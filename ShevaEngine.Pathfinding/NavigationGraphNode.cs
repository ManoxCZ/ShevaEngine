using System.Collections.Generic;

namespace ShevaEngine.Pathfinding;

public abstract class NavigationGraphNode
{
    public List<NavigationGraphConnection> Connections;

    protected NavigationGraphNode()
    {
        Connections = new List<NavigationGraphConnection>();
    }

    public abstract float DistanceTo(NavigationGraphNode node);
}
