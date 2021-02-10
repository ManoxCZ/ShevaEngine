using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reactive.Subjects;


namespace ShevaEngine.Core
{

    /// <summary>
    /// Behavior subject serializer.
    /// </summary>
    public class BehaviorSubjectConverter<T> : JsonConverter<BehaviorSubject<T>> 
    {
        /// <summary>
        /// Constructor.
        /// </summary>        
        public BehaviorSubjectConverter(JsonSerializerOptions options)
        {                
        }

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
        /// Read method.
        /// </summary>
        public override BehaviorSubject<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {                
            return new BehaviorSubject<T>(JsonSerializer.Deserialize<T>(ref reader, options));
        }

        /// <summary>
        /// Write method.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, BehaviorSubject<T> subject, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, subject.Value, options);                                            
        }        
    }
}