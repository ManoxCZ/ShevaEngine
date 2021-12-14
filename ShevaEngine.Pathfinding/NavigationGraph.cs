using System.Collections.Generic;
using System.Linq;

namespace ShevaEngine.Pathfinding
{
    /// <summary>
    /// Navigation graph class.
    /// </summary>
    public class NavigationGraph
    {
        /// <summary>
        /// Method finds way between start and end.
        /// </summary>
        public static bool FindPath(NavigationGraphNode startNode, NavigationGraphNode endNode, NavigationPath path, Heuristic? heuristic = null)
        {
            path.Clear();

            if (heuristic == null)
            {
                heuristic = new DistanceHeuristic();
            }

            if (startNode == endNode)
            {
                return true;
            }

            heuristic.EndNode = endNode;

            List<NavigationGraphNodeRecord> openNodes = new List<NavigationGraphNodeRecord> { new NavigationGraphNodeRecord(startNode) };
            List<NavigationGraphNodeRecord> closedNodes = new List<NavigationGraphNodeRecord>();

            NavigationGraphNodeRecord currentNode = openNodes.OrderBy(node => node.CostSoFar).First();

            while (openNodes.Count > 0)
            {
                currentNode = openNodes.OrderBy(node => node.EstimatedTotalCost).First();

                if (currentNode.Node == endNode)
                    break;

                foreach (NavigationGraphConnection connection in currentNode.Node.Connections)
                {
                    NavigationGraphNodeRecord endConnectionNode = new NavigationGraphNodeRecord(
                        connection.Node1, connection, currentNode.CostSoFar + connection.Cost);

                    if (currentNode.Node == connection.Node1)
                        endConnectionNode.Node = connection.Node2;

                    if (closedNodes.Contains(endConnectionNode, NavigationGraphNodeRecordEqualityComparer.Instance))
                    {
                        NavigationGraphNodeRecord temp = closedNodes.First(item => item.Node == endConnectionNode.Node);

                        if (temp.CostSoFar <= endConnectionNode.CostSoFar)
                            continue;

                        closedNodes.Remove(endConnectionNode);
                    }
                    else if (openNodes.Contains(endConnectionNode, NavigationGraphNodeRecordEqualityComparer.Instance))
                    {
                        NavigationGraphNodeRecord temp = openNodes.First(item => item.Node == endConnectionNode.Node);

                        if (temp.CostSoFar <= endConnectionNode.CostSoFar)
                            continue;
                    }

                    endConnectionNode.EstimatedTotalCost = heuristic.Estimate(endConnectionNode.Node);

                    openNodes.Add(endConnectionNode);
                }

                openNodes.Remove(currentNode);
                closedNodes.Add(currentNode);
            }

            if (currentNode.Node != endNode)
                return false;

            while (currentNode.Node != startNode)
            {
                path.Push(currentNode.Node);

                NavigationGraphNode temp = currentNode.Connection.Node1;

                if (currentNode.Node == temp)
                    temp = currentNode.Connection.Node2;

                currentNode = closedNodes.First(node => node.Node == temp);
            }

            return true;
        }
    }
}
