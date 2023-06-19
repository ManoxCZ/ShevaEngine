using Microsoft.Extensions.Logging;
using ShevaEngine.Core;
using System.Text;

namespace ShevaEngine.NoesisUI;

public class NoesisUIWrapper
{    
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
        
        Noesis.GUI.SetLicense(licenseName, licenseKey);
        Noesis.GUI.Init();
        
        Noesis.GUI.SetXamlProvider(new XamlProvider());
        Noesis.GUI.SetFontProvider(new FontProvider());
        Noesis.GUI.SetTextureProvider(new TextureProvider());        
        
        Noesis.GUI.LoadApplicationResources(themeFilename + ".xaml");
    }    
}
