namespace ShevaEngine.Pathfinding;

public class NavigationGraphConnection
{
    public NavigationGraphNode Node1 { get; }
    public NavigationGraphNode Node2 { get; }
    public float Cost { get; }

    public NavigationGraphConnection(NavigationGraphNode node1, NavigationGraphNode node2, float cost)
    {
        Node1 = node1;
        Node2 = node2;
        Cost = cost;
    }
}
