using System.Text.Json.Nodes;

namespace ShevaEngine.Core.Serialization;

public interface ISerializable
{
    void Serialize(JsonNode node);
    void Deserialize(JsonNode node);
}
