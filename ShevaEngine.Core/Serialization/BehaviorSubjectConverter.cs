using System;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShevaEngine.Core;

public class BehaviorSubjectConverter<T> : JsonConverter<BehaviorSubject<T>>
{
    public BehaviorSubjectConverter(JsonSerializerOptions options)
    {
    }

    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
        {
            return false;
        }

        if (typeToConvert.GetGenericTypeDefinition() != typeof(BehaviorSubject<>))
        {
            return false;
        }

        return true;
    }

    public override BehaviorSubject<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (JsonSerializer.Deserialize<T>(ref reader, options) is T value)
        {
            return new BehaviorSubject<T>(value);
        }

        return new BehaviorSubject<T>(default!);
    }

    public override void Write(Utf8JsonWriter writer, BehaviorSubject<T> subject, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, subject.Value, options);
    }
}