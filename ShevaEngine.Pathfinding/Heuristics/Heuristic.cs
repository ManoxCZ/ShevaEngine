namespace ShevaEngine.Pathfinding.Heuristics;

public abstract class Heuristic
{
    public NavigationGraphNode EndNode { get; set; } = null!;

    public abstract float Estimate(NavigationGraphNode node);
}
