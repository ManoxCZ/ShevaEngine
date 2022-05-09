namespace ShevaEngine.Pathfinding.Heuristics;

public class DistanceHeuristic : Heuristic
{
    public override float Estimate(NavigationGraphNode node)
    {
        return node.DistanceTo(EndNode);
    }
}
