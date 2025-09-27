using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ShevaEngine.Core.Settings;

public class SettingsService : IDisposable
{
    private readonly SortedDictionary<string, ShevaGameSettings> _settings = new();


    public void Dispose()
    {
        foreach (var keyValuePair in _settings)
        {
            keyValuePair.Value.Dispose();
        }

        _settings.Clear();
    }

    public T? GetSettings<T>() where T : ShevaGameSettings, new()
    {
        if (typeof(T).FullName is string typeFullName &&
            !string.IsNullOrEmpty(typeFullName))
        {
            if (_settings.TryGetValue(typeFullName, out var settings))
            {
                return (T)settings;
            }

            T newSettings = new();
            _settings.Add(typeFullName, newSettings);
            
            newSettings.Initialize(this);

            ReadOnlySpan<byte> bytes = ShevaServices.GetService<IFileSystemService>().ReadAllBytes($"{typeof(T).Name}.settings");
            Utf8JsonReader reader = new(bytes);

            if (JsonNode.Parse(ref reader) is JsonNode rootNode)
            {
                newSettings.Deserialize(rootNode);
            }

            return newSettings;
        }

        return default;
    }

    public void Save<T>() where T : ShevaGameSettings, new()
    {
        if (GetSettings<T>() is T settings)
        {
            //ShevaServices.GetService<IFileSystemService>().WriteFileContent($"{typeof(T).Name}.settings", Serializer.Serialize(settings));
        }
    }
}