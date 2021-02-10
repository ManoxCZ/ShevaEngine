using Microsoft.Xna.Framework;

namespace ShevaEngine.Pathfinding
{
    /// <summary>
    /// Distance heuristic.
    /// </summary>
    public class DistanceHeuristic : Heuristic
    {
        /// <summary>
        /// Method computes estimate cost.
        /// </summary>
        public override float Estimate(NavigationGraphNode node)
        {
            return node.DistanceTo(EndNode);
        }
    }
}
