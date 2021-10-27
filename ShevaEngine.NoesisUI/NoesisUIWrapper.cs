using Microsoft.Extensions.Logging;
using ShevaEngine.Core;
using ShevaEngine.Core.UI;
using System;
using System.Threading.Tasks;

namespace ShevaEngine.NoesisUI
{
    public class NoesisUIWrapper : IUISystem
    {
        public static string LICENSE_NAME = "";
        public static string LICENSE_KEY = "";

        private readonly ILogger _log = ShevaGame.Instance.LoggerFactory.CreateLogger<NoesisUIWrapper>();
        private readonly Noesis.RenderDeviceD3D11 _device;

        /// <summary>
        /// Constructor.
        /// </summary>
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

            _device = new Noesis.RenderDeviceD3D11(
                ((SharpDX.Direct3D11.Device)ShevaGame.Instance.GraphicsDevice.Handle).ImmediateContext.NativePointer, false);

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
           
            Noesis.GUI.LoadApplicationResources("Themes.Theme.xaml");
        }
       
        /// <summary>
        /// Run on UI thread.
        /// </summary>        
        public void RunOnUIThread(Action action)
        {
            ShevaGame.Instance.SynchronizationContext.Send(_ => action(), null);            
        }

        /// <summary>
        /// Run on UI thread.
        /// </summary>        
        public Task<T> RunFuncOnUIThread<T>(Func<T> function)
        {
            TaskCompletionSource<T> taskSource = new TaskCompletionSource<T>();

            ShevaGame.Instance.SynchronizationContext.Send(_ =>
            {
                taskSource.SetResult(function());
            }, null);

            return taskSource.Task;            
        }
    }
}
