using Microsoft.Extensions.Logging;
using ShevaEngine.Core;

namespace ShevaEngine.NoesisUI;

public class NoesisUIWrapper
{    
    public static void Initialize(string licenseName, string licenseKey, string themeFilename = "Themes.Theme")
    {
        ILogger log = ShevaServices.GetService<ILoggerFactory>().CreateLogger<NoesisUIWrapper>();

        Noesis.Log.SetLogCallback((level, channel, message) =>
        {
            switch (level)
            {
                case Noesis.LogLevel.Trace:
                case Noesis.LogLevel.Debug:
                    log.LogDebug(message);
                    break;
                case Noesis.LogLevel.Info:
                    log.LogInformation(message);
                    break;
                case Noesis.LogLevel.Warning:
                    log.LogWarning(message);
                    break;
                case Noesis.LogLevel.Error:
                    log.LogError(message);
                    break;
            }
        });

        NoesisApp.RenderContextGLX contextWGL = new();        
        //contextWGL.Init(1, ShevaGame.Instance.Window.Handle, 1, false, false);
        
        Noesis.GUI.SetLicense(licenseName, licenseKey);
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

        NoesisApp.Application.SetThemeProviders();
        Noesis.GUI.LoadApplicationResources(themeFilename + ".xaml");
    }    
}
