using ShevaEngine.Core.Serialization;
using ShevaEngine.Core.Settings;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json.Nodes;

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
        Resolution = Create(new Resolution(1200, 720));
        WindowedResolution = Create(new Resolution(1200, 720));
        Fullscreen = Create(false);
        MusicVolume = Create(1.0f);
        GraphicsQuality = Create(Core.GraphicsQuality.Medium);
    }

    public override void Initialize(SettingsService settingsService)
    {        
        Disposables.Add(Resolution.DistinctUntilChanged().Subscribe(item => settingsService.Save<GameSettings>()));
        Disposables.Add(WindowedResolution.DistinctUntilChanged().Subscribe(item => settingsService.Save<GameSettings>()));
        Disposables.Add(Fullscreen.DistinctUntilChanged().Subscribe(item => settingsService.Save<GameSettings>()));
        Disposables.Add(MusicVolume.DistinctUntilChanged().Subscribe(item => settingsService.Save<GameSettings>()));
        Disposables.Add(GraphicsQuality.DistinctUntilChanged().Subscribe(item => settingsService.Save<GameSettings>()));
    }

    public override void Serialize(JsonNode node)
    {
        
    }

    public override void Deserialize(JsonNode node)
    {
        GetValue(node, nameof(Resolution), Resolution);
        GetValue(node, nameof(WindowedResolution), WindowedResolution);
        SerializationUtils.GetValue(node, nameof(Fullscreen), Fullscreen);
        SerializationUtils.GetValue(node, nameof(MusicVolume), MusicVolume);
        GetValue(node, nameof(GraphicsQuality), GraphicsQuality);
    }

    protected void GetValue(JsonNode node, string nodeName, BehaviorSubject<Resolution> property)
    {
        if (node[nodeName] is JsonNode resolutionNode &&
            resolutionNode[nameof(Resolution.Value.Width)] is JsonNode widthNode &&
            widthNode.GetValue<int>() is int width &&
            resolutionNode[nameof(Resolution.Value.Height)] is JsonNode heightNode &&
            heightNode.GetValue<int>() is int height)
        {
            property.OnNext(new Resolution(width, height));
        }
    }

    protected void GetValue(JsonNode node, string nodeName, BehaviorSubject<GraphicsQuality> property)
    {
        if (node[nodeName] is JsonNode graphicsQualityNode &&
            graphicsQualityNode.GetValue<int>() is int graphicsQuality)
        {
            property.OnNext((GraphicsQuality)graphicsQuality);
        }
    }
}
