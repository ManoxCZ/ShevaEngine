using System.Collections.Generic;

namespace ShevaEngine.Pathfinding;

public class NavigationGraphNodeRecordEqualityComparer : IEqualityComparer<NavigationGraphNodeRecord>
{
    public static readonly NavigationGraphNodeRecordEqualityComparer Instance = new NavigationGraphNodeRecordEqualityComparer();

    public bool Equals(NavigationGraphNodeRecord? x, NavigationGraphNodeRecord? y)
    {
        return x?.Node == y?.Node;
    }

    public int GetHashCode(NavigationGraphNodeRecord obj)
    {
        return base.GetHashCode();
    }
}

