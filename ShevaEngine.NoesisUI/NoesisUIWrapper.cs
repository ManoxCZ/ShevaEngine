using ShevaEngine.Core;
using System;
using System.Threading.Tasks;

namespace ShevaEngine.NoesisUI
{
    public class NoesisUIWrapper
    {
        public const string LICENSE_NAME = "";
        public const string LICENSE_KEY = "";
        

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
        /// Initialize.
        /// </summary>        
        public void Initialize(ShevaGame game)
        {
            
        }
        
        /// <summary>
        /// Get layer.
        /// </summary>
        public static Task<Layer> GetLayer(string xamlFilename)
        {
            TaskCompletionSource<Layer> taskSource = new TaskCompletionSource<Layer>();

            Windows.UI.Core.CoreDispatcher dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

            dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Noesis.FrameworkElement frameworkElement = (Noesis.FrameworkElement)Noesis.GUI.LoadXaml(xamlFilename);

                Noesis.View view = Noesis.GUI.CreateView(frameworkElement);

                Noesis.RenderDeviceD3D11 device = new Noesis.RenderDeviceD3D11(
                    ((SharpDX.Direct3D11.Device)ShevaGame.Instance.GraphicsDevice.Handle).ImmediateContext.NativePointer,
                    false);

                view.Renderer.Init(device);
                view.SetFlags(Noesis.RenderFlags.LCD | Noesis.RenderFlags.PPAA);

                taskSource.SetResult(new Layer(view));
            });
            
            return taskSource.Task;
        }
    }
}
