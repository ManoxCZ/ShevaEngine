using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace ShevaEngine.Core;

public class Serializer
{
    private static JsonSerializerOptions GetJsonSerializerOptions(
        ReferenceHandler? handler = null,
        IJsonTypeInfoResolver? jsonTypeInfoResolver = null)
    {
        JsonSerializerOptions serializeOptions = new()
        {                
            WriteIndented = true,
            ReferenceHandler = handler,
            TypeInfoResolver = jsonTypeInfoResolver
        };

        serializeOptions.Converters.Add(new BehaviorSubjectConverterFactory());

        return serializeOptions;
    }

    public static string Serialize<T>(T value, 
        ReferenceHandler? handler = null, IJsonTypeInfoResolver? jsonTypeInfoResolver = null)
    {
        return JsonSerializer.Serialize(value, GetJsonSerializerOptions(handler, jsonTypeInfoResolver));
    }

    public static void Serialize<T>(Stream stream, T value,
        ReferenceHandler? handler = null, IJsonTypeInfoResolver? jsonTypeInfoResolver = null)
    {
        JsonSerializer.Serialize(stream, value, GetJsonSerializerOptions(handler, jsonTypeInfoResolver));
    }

    public static void Serialize<T>(string filename, T value,
        ReferenceHandler? handler = null, IJsonTypeInfoResolver? jsonTypeInfoResolver = null)
    {
        using StreamWriter writer = new(filename);

        JsonSerializer.Serialize(writer.BaseStream, value, GetJsonSerializerOptions(handler, jsonTypeInfoResolver));
    }

    public static T? Deserialize<T>(string data,
        ReferenceHandler? handler = null, IJsonTypeInfoResolver? jsonTypeInfoResolver = null)
    {
        return JsonSerializer.Deserialize<T>(data, GetJsonSerializerOptions(handler, jsonTypeInfoResolver));
    }

    public static T? Deserialize<T>(Stream stream,
        ReferenceHandler? handler = null, IJsonTypeInfoResolver? jsonTypeInfoResolver = null)
    {
        return JsonSerializer.Deserialize<T>(stream, GetJsonSerializerOptions(handler, jsonTypeInfoResolver));
    }

    public static T[] DeserializeFiles<T>(IReadOnlyCollection<string> filenames,
        ReferenceHandler? handler = null, IJsonTypeInfoResolver? jsonTypeInfoResolver = null)
    {
        ArrayBufferWriter<byte> outputBuffer = new();

        List<JsonNode> nodes = [];

        foreach (var filename in filenames)
        {
            using (StreamReader fileReader = new(filename))
            {
                if (JsonNode.Parse(fileReader.ReadToEnd()) is JsonArray array)
                {
                    foreach (JsonNode? child in array)
                    {
                        if (child is not null)
                        {
                            nodes.Add(child);
                        }
                    }
                }
            }
        }

        nodes = nodes.OrderBy(item =>
        {
            if (int.TryParse(item["$id"].ToString(), out int result))
            {
                return result;
            }

            return -1;
        }).ToList();

        using (var jsonWriter = new Utf8JsonWriter(outputBuffer, new JsonWriterOptions { Indented = true }))
        {
            jsonWriter.WriteStartArray();

            foreach (var item in nodes)
            {
                item.WriteTo(jsonWriter);
            }

            jsonWriter.WriteEndArray();
        }

        return JsonSerializer.Deserialize<T[]>(outputBuffer.WrittenSpan, GetJsonSerializerOptions(handler, jsonTypeInfoResolver));
    }
}