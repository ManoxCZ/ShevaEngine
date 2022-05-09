namespace ShevaEngine.Pathfinding;

public class NavigationGraphNodeRecord
{
    public NavigationGraphNode Node;
    public float CostSoFar;
    public float EstimatedTotalCost;
    public NavigationGraphConnection? Connection;

    
    public NavigationGraphNodeRecord(NavigationGraphNode node, NavigationGraphConnection? connection = null,
        float costSoFar = 0.0f, float estimatedTotalCost = float.MaxValue)
    {
        Node = node;
        Connection = connection;
        CostSoFar = costSoFar;
        EstimatedTotalCost = estimatedTotalCost;
    }
}

