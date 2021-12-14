namespace ShevaEngine.Pathfinding
{
    /// <summary>Heuristic.</summary>
    public abstract class Heuristic
    {
        /// <summary>End node.</summary>
        public NavigationGraphNode EndNode { get; set; } = null!;

        /// <summary>
        /// Method computes estimate cost.
        /// </summary>
        public abstract float Estimate(NavigationGraphNode node);
    }
}
