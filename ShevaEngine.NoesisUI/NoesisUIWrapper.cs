using Microsoft.Extensions.Logging;
using Noesis;
using Noesis.MonoGame;
using NoesisApp;
using ShevaEngine.Core;

namespace ShevaEngine.NoesisUI;

public class NoesisUIWrapper
{
    public static RenderDevice? Device { get; private set; }


    public static void Initialize(string licenseName, string licenseKey, string themeFilename = "Themes.Theme")
    {
        ILogger log = ShevaServices.GetService<ILoggerFactory>().CreateLogger<NoesisUIWrapper>();
        
        Log.SetLogCallback((level, channel, message) =>
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
        
        GUI.SetXamlProvider(new EmbeddedXamlProvider());
        GUI.SetFontProvider(new EmbeddedFontProvider());        
        GUI.SetTextureProvider(new EmbeddedTextureProvider());
        //GUI.SetFontFallbacks(["Resources/Fonts/#Agdasima"]);
        //GUI.SetFontDefaultProperties(25.0f, FontWeight.Normal, FontStretch.Normal, FontStyle.Normal);

        GUI.LoadApplicationResources(themeFilename + ".xaml");

        Device = new NoesisRenderDevice(ShevaGame.Instance.GraphicsDevice, ShevaGame.Instance.Content)
        {
            GlyphCacheHeight = 2048,
            OffscreenSampleCount = 0
        };
    }
}
