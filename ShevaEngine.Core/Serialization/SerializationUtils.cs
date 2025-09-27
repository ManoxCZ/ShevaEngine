using System.Reactive.Subjects;
using System.Text.Json.Nodes;

namespace ShevaEngine.Core.Serialization;

public class SerializationUtils
{
    public static T? GetValue<T>(JsonNode node, string nodeName)
    {
        if (node[nodeName] is JsonNode foundNode &&
            foundNode.GetValue<T>() is T foundNodeValue)
        {
            return foundNodeValue;
        }

        return default;
    }

    public static void GetValue<T>(JsonNode node, string nodeName, BehaviorSubject<T?> property)
    {
        property.OnNext(GetValue<T>(node, nodeName));
    }
}
