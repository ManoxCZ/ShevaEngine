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

            Noesis.GUI.SetLicense(LICENSE_NAME, LICENSE_KEY);
            Noesis.GUI.Init();
                        
            Noesis.GUI.SetXamlProvider(new XamlProvider());
            Noesis.GUI.SetFontProvider(new FontProvider());
            Noesis.GUI.SetTextureProvider(new TextureProvider());

            Noesis.GUI.SetFontFallbacks(new[] { "Arial" });
            Noesis.GUI.SetFontDefaultProperties(15.0f, Noesis.FontWeight.Normal, Noesis.FontStretch.Normal, Noesis.FontStyle.Normal);
            
            Noesis.GUI.LoadApplicationResources(@"Content\Themes\Theme.xaml");
        }

        /// <summary>
        /// Get layer.
        /// </summary>
        public Task<Noesis.FrameworkElement> GetFrameworkElement(string xamlFilename)
        {
            return RunFuncOnUIThread(() =>
            {                
                return (Noesis.FrameworkElement)Noesis.GUI.LoadXaml(xamlFilename);                
            });            
        }

        /// <summary>
        /// Get layer.
        /// </summary>
        public Task<ILayer> GetLayer(string xamlFilename)
        {
            return RunFuncOnUIThread(() =>
            { 
                Noesis.FrameworkElement frameworkElement = (Noesis.FrameworkElement)Noesis.GUI.LoadXaml(xamlFilename);

                Noesis.View view = Noesis.GUI.CreateView(frameworkElement);

                Noesis.RenderDeviceD3D11 device = new Noesis.RenderDeviceD3D11(
                    ((SharpDX.Direct3D11.Device)ShevaGame.Instance.GraphicsDevice.Handle).ImmediateContext.NativePointer,
                    false);

                view.Renderer.Init(device);
                view.SetFlags(Noesis.RenderFlags.LCD | Noesis.RenderFlags.PPAA);

                return (ILayer)new Layer(this, view);
            });
        }

        /// <summary>
        /// Run on UI thread.
        /// </summary>        
        public void RunOnUIThread(Action action)
        {
            ShevaGame.Instance.SynchronizationContext.Send(_ => action(), null);

            //Windows.UI.Core.CoreDispatcher dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

            //if (dispatcher.HasThreadAccess)
            //    action();
            //else
            //    dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            //    {
            //        action();
            //    }).AsTask();
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


            //Windows.UI.Core.CoreDispatcher dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

            //if (dispatcher.HasThreadAccess)
            //    return Task.FromResult(function());
            //else
            //{
            //    TaskCompletionSource<T> taskSource = new TaskCompletionSource<T>();

            //    dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            //    {
            //        taskSource.SetResult(function());
            //    }).AsTask();                    

            //    return taskSource.Task;
            //}
        }
    }
}
