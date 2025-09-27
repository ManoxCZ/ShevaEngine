using ShevaEngine.Core.Settings;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text.Json.Nodes;

namespace ShevaEngine.Core;

public abstract class ShevaGameSettings : IDisposable
{
    protected List<IDisposable> Disposables;

    public ShevaGameSettings()
    {
        Disposables = [];
    }

    public virtual void Initialize(SettingsService settingsService)
    {

    }

    public virtual void Dispose()
    {
        foreach (IDisposable disposable in Disposables)
            disposable?.Dispose();

        Disposables.Clear();
        Disposables = null!;
    }

    public BehaviorSubject<T> Create<T>(T value)
    {
        BehaviorSubject<T> result = new(value);
        Disposables.Add(result);

        return result;
    }

    public abstract void Serialize(JsonNode node);
    public abstract void Deserialize(JsonNode node);    
}
