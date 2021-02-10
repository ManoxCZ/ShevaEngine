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
            JsonSerializerOptions serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new BehaviorSubjectConverterFactory());
            serializeOptions.WriteIndented = true;

            return JsonSerializer.Serialize(value, serializeOptions);
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        public static T Deserialize<T>(string data)
        {
            JsonSerializerOptions serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new BehaviorSubjectConverterFactory());
            serializeOptions.WriteIndented = true;            

            return JsonSerializer.Deserialize<T>(data, serializeOptions);
        }
    }
}