using ShevaEngine.Core;
using System;
using System.Threading.Tasks;

namespace ShevaEngine.NoesisUI
{
    public class NoesisUIWrapper
    {
        public static string LICENSE_NAME = "";
        public static string LICENSE_KEY = "";
        

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
                        LogManager.Instance.AddLogMessage(new LogMessage()
                        {
                            DateTime = DateTime.Now,
                            Origin = channel,
                            Message = message,
                            Severity = LogSeverity.Debug
                        });
                        break;
                    case Noesis.LogLevel.Info:
                        LogManager.Instance.AddLogMessage(new LogMessage()
                        {
                            DateTime = DateTime.Now,
                            Origin = channel,
                            Message = message,
                            Severity = LogSeverity.Info
                        });
                        break;
                    case Noesis.LogLevel.Warning:
                        LogManager.Instance.AddLogMessage(new LogMessage()
                        {
                            DateTime = DateTime.Now,
                            Origin = channel,
                            Message = message,
                            Severity = LogSeverity.Warning
                        });
                        break;
                    case Noesis.LogLevel.Error:
                        LogManager.Instance.AddLogMessage(new LogMessage()
                        {
                            DateTime = DateTime.Now,
                            Origin = channel,
                            Message = message,
                            Severity = LogSeverity.Error
                        });
                        break;                    
                }
            });

            Noesis.GUI.Init(LICENSE_NAME, LICENSE_KEY);
                        
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
        public static Task<Noesis.FrameworkElement> GetFrameworkElement(string xamlFilename)
        {
            return RunFuncOnUIThread(() =>
            {                
                return (Noesis.FrameworkElement)Noesis.GUI.LoadXaml(xamlFilename);                
            });            
        }

        /// <summary>
        /// Get layer.
        /// </summary>
        public static Task<Layer> GetLayer(string xamlFilename)
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

                return new Layer(view);
            });
        }

        /// <summary>
        /// Run on UI thread.
        /// </summary>        
        public static void RunOnUIThread(Action action)
        {
            Windows.UI.Core.CoreDispatcher dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

            if (dispatcher.HasThreadAccess)
                action();
            else
                dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    action();
                }).AsTask();
        }

        /// <summary>
        /// Run on UI thread.
        /// </summary>        
        public static Task<T> RunFuncOnUIThread<T>(Func<T> function)
        {
            Windows.UI.Core.CoreDispatcher dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

            if (dispatcher.HasThreadAccess)
                return Task.FromResult(function());
            else
            {
                TaskCompletionSource<T> taskSource = new TaskCompletionSource<T>();

                dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    taskSource.SetResult(function());
                }).AsTask();                    

                return taskSource.Task;
            }
        }
    }
}
