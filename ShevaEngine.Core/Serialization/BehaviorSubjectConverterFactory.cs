using System;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace ShevaEngine.Core
{
    public class BehaviorSubjectConverterFactory : JsonConverterFactory
    {
        /// <summary>
        /// Can convert.
        /// </summary>
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

        /// <summary>
        /// Create converter.
        /// </summary>
        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
        {            
            Type valueType = type.GetGenericArguments()[0];

            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(BehaviorSubjectConverter<>).MakeGenericType(
                    new Type[] { valueType }),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null);

            return converter;
        }
    }
}