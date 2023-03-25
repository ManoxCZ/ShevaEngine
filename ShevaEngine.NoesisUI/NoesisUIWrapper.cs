using Microsoft.Extensions.Logging;
using ShevaEngine.Core;
using ShevaEngine.Core.UI;
using System;

namespace ShevaEngine.NoesisUI;

public class NoesisUIWrapper : IUISystem
{
    public static string LICENSE_NAME = "";
    public static string LICENSE_KEY = "";
    public static string THEME_FILENAME = "Themes.Theme";

    private readonly ILogger _log = ShevaServices.GetService<ILoggerFactory>().CreateLogger<NoesisUIWrapper>();        

    public NoesisUIWrapper()
    {
        Noesis.Log.SetLogCallback((level, channel, message) =>
        {
            switch (level)
            {
                case Noesis.LogLevel.Trace:
                case Noesis.LogLevel.Debug:
                    _log.LogDebug(message);
                    break;
                case Noesis.LogLevel.Info:
                    _log.LogInformation(message);
                    break;
                case Noesis.LogLevel.Warning:
                    _log.LogWarning(message);
                    break;
                case Noesis.LogLevel.Error:
                    _log.LogError(message);
                    break;
            }
        });

        Noesis.GUI.SetLicense(LICENSE_NAME, LICENSE_KEY);
        Noesis.GUI.Init();

        Noesis.GUI.SetXamlProvider(new XamlProvider());
        Noesis.GUI.SetFontProvider(new FontProvider());
        Noesis.GUI.SetTextureProvider(new TextureProvider());

        Noesis.GUI.SetFontFallbacks(NoesisApp.Theme.FontFallbacks);
        Noesis.GUI.SetFontDefaultProperties(
            NoesisApp.Theme.DefaultFontSize,
            NoesisApp.Theme.DefaultFontWeight,
            NoesisApp.Theme.DefaultFontStretch,
            NoesisApp.Theme.DefaultFontStyle);

        Noesis.GUI.LoadApplicationResources(THEME_FILENAME + ".xaml");
    }

    public void RunOnUIThread(Action action)
    {
        ShevaGame.Instance.SynchronizationContext?.Send(_ => action(), null);
    }    
}
