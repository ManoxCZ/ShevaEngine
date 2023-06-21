using Microsoft.Extensions.Logging;
using Noesis;
using ShevaEngine.Core;

namespace ShevaEngine.NoesisUI;

public class NoesisUIWrapper
{
#if DESKTOPGL          
    public static RenderDeviceGL Device { get; private set; } = null!;
#endif                     

    public static void Initialize(string licenseName, string licenseKey, string themeFilename = "Themes.Theme")
    {
        ILogger log = ShevaServices.GetService<ILoggerFactory>().CreateLogger<NoesisUIWrapper>();

        Noesis.Log.SetLogCallback((level, channel, message) =>
        {
            string completeMessage = string.Join(' ', channel, message);

            switch (level)
            {
                case Noesis.LogLevel.Trace:
                case Noesis.LogLevel.Debug:
                    log.LogDebug(completeMessage);
                    break;
                case Noesis.LogLevel.Info:
                    log.LogInformation(completeMessage);
                    break;
                case Noesis.LogLevel.Warning:
                    log.LogWarning(completeMessage);
                    break;
                case Noesis.LogLevel.Error:
                    log.LogError(completeMessage);
                    break;
            }
        });

        GUI.SetLicense(licenseName, licenseKey);
        GUI.Init();

        GUI.SetXamlProvider(new XamlProvider());
        GUI.SetFontProvider(new FontProvider());
        GUI.SetTextureProvider(new TextureProvider());

        GUI.LoadApplicationResources(themeFilename + ".xaml");

#if DESKTOPGL
        Device = new RenderDeviceGL(false);
#endif
    }    
}
