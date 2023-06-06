using System;
using System.Collections.Generic;

namespace ShevaEngine.Core.Settings;

public class SettingsService : IDisposable
{
    private readonly SortedDictionary<Type, ShevaGameSettings> _settings = new();


    public void Dispose()
    {
        foreach (var keyValuePair in _settings)
        {
            keyValuePair.Value.Dispose();
        }

        _settings.Clear();
    }

    public T GetSettings<T>() where T : ShevaGameSettings, new()
    {
        if (_settings.TryGetValue(typeof(T), out var settings))
        {
            return (T)settings;
        }

        T newSettings = ShevaGameSettings.Load<T>();

        _settings.Add(typeof(T), newSettings);

        return newSettings;
    }
}
