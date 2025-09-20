using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;


namespace ShevaEngine.Core;

public class GameSettings : ShevaGameSettings
{
    public BehaviorSubject<Resolution> Resolution { get; set; }
    public BehaviorSubject<Resolution> WindowedResolution { get; set; }
    public BehaviorSubject<bool> Fullscreen { get; set; }
    public BehaviorSubject<float> MusicVolume { get; set; }
    public BehaviorSubject<GraphicsQuality> GraphicsQuality { get; set; }


    public GameSettings()
    {
        Resolution = Create(new Resolution(1200, 1000));
        WindowedResolution = Create(new Resolution(1200, 1000));
        Fullscreen = Create(false);
        MusicVolume = Create(1.0f);
        GraphicsQuality = Create(Core.GraphicsQuality.Medium);

        Initialize();
    }

    public override void Initialize()
    {
        Disposables.Add(Resolution.DistinctUntilChanged().Subscribe(item => Save(this)));
        Disposables.Add(WindowedResolution.DistinctUntilChanged().Subscribe(item => Save(this)));
        Disposables.Add(Fullscreen.DistinctUntilChanged().Subscribe(item => Save(this)));
        Disposables.Add(MusicVolume.DistinctUntilChanged().Subscribe(item => Save(this)));
        Disposables.Add(GraphicsQuality.DistinctUntilChanged().Subscribe(item => Save(this)));
    }
}
