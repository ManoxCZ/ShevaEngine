using System.IO;
using System.Text.Json;


namespace ShevaEngine.Core
{
    public class Serializer
    {

        /// <summary>
        /// Serialize.
        /// </summary>
        public static string Serialize<T>(T value)
        {
            JsonSerializerOptions serializeOptions = new();
            serializeOptions.Converters.Add(new BehaviorSubjectConverterFactory());
            serializeOptions.WriteIndented = true;            

            return JsonSerializer.Serialize(value, serializeOptions);
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        public static void Serialize<T>(Stream stream, T value)
        {
            JsonSerializerOptions serializeOptions = new();
            serializeOptions.Converters.Add(new BehaviorSubjectConverterFactory());
            serializeOptions.WriteIndented = true;

            JsonSerializer.Serialize(stream, value, serializeOptions);
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        public static T? Deserialize<T>(string data)
        {
            JsonSerializerOptions serializeOptions = new();
            serializeOptions.Converters.Add(new BehaviorSubjectConverterFactory());
            serializeOptions.WriteIndented = true;

            return JsonSerializer.Deserialize<T>(data, serializeOptions);
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        public static T? Deserialize<T>(Stream stream)
        {
            JsonSerializerOptions serializeOptions = new();
            serializeOptions.Converters.Add(new BehaviorSubjectConverterFactory());
            serializeOptions.WriteIndented = true;

            return JsonSerializer.Deserialize<T>(stream, serializeOptions);
        }
    }
}