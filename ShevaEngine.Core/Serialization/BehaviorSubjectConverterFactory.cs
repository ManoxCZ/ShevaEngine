using System;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShevaEngine.Core;

public class BehaviorSubjectConverterFactory : JsonConverterFactory
{
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

    public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
    {
        Type valueType = type.GetGenericArguments()[0];

        JsonConverter converter = (JsonConverter)Activator.CreateInstance(
            typeof(BehaviorSubjectConverter<>).MakeGenericType([valueType]),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: [options],
            culture: null);

        return converter;
    }
}