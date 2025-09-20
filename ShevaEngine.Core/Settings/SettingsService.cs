using System;
using System.Collections.Generic;

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

    public T GetSettings<T>() where T : ShevaGameSettings, new()
    {
        try
        {
            if (_settings.TryGetValue(typeof(T).FullName, out var settings))
            {
                return (T)settings;
            }
        }
        catch (Exception ex)
        {
        }

        T newSettings = ShevaGameSettings.Load<T>();

        _settings.Add(typeof(T).FullName, newSettings);

        return newSettings;
    }
}
