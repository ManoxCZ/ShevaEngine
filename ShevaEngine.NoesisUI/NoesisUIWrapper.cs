using Microsoft.Extensions.Logging;
using Noesis;
using NoesisApp;
using ShevaEngine.Core;

namespace ShevaEngine.NoesisUI;

public class NoesisUIWrapper
{
    public static RenderDevice? Device { get; private set; }
    public static Dispatcher Dispatcher { get; private set; } = null!;


    public static void Initialize(string licenseName, string licenseKey, string themeFilename = "Themes.Theme")
    {
        ILogger log = ShevaServices.GetService<ILoggerFactory>().CreateLogger<NoesisUIWrapper>();

        Dispatcher = Dispatcher.CurrentDispatcher;

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
        
        GUI.SetXamlProvider(new XamlProvider());
        GUI.SetFontProvider(new EmbeddedFontProvider());        
        GUI.SetTextureProvider(new TextureProvider());
        GUI.SetFontFallbacks(new[] { "Agdasima" });
        GUI.SetFontDefaultProperties(17, FontWeight.Normal, FontStretch.Normal, FontStyle.Normal);
        
        GUI.LoadApplicationResources(themeFilename + ".xaml");

//#if WINDOWSDX
//        Device = new RenderDeviceD3D11(((SharpDX.Direct3D11.Device)ShevaGame.Instance.GraphicsDevice.Handle).ImmediateContext.NativePointer, false);
//#elif DESKTOPGL
//        Device = new RenderDeviceGL(false);        
//#endif
    }
}
