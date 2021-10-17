using ShevaEngine.Core;
using System;
using System.IO;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ShevaEngine.NoesisUI
{
    public class NoesisUIWrapper
    {
        public const string LICENSE_NAME = "Project01";
        public const string LICENSE_KEY = "1hxwQ3VVcEN6qq4x1hqS5dQOVsN3yW5kEPgYbnwAR9fJDZRs";

        private readonly ShevaGame _game;


        /// <summary>
        /// Constructor.
        /// </summary>
        public NoesisUIWrapper(ShevaGame game)
        {
            _game = game;

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

            Noesis.GUI.SetLicense(LICENSE_NAME, LICENSE_KEY);
            Noesis.GUI.Init();
                                    
            Noesis.GUI.SetXamlProvider(new XamlProvider());
            Noesis.GUI.SetFontProvider(new FontProvider());
            Noesis.GUI.SetTextureProvider(new TextureProvider(_game));

            Noesis.GUI.SetFontFallbacks(NoesisApp.Theme.FontFallbacks);
            Noesis.GUI.SetFontDefaultProperties(
                NoesisApp.Theme.DefaultFontSize,
                NoesisApp.Theme.DefaultFontWeight,
                NoesisApp.Theme.DefaultFontStretch,
                NoesisApp.Theme.DefaultFontStyle);
           
            Noesis.GUI.LoadApplicationResources(@"Content\Themes\NoesisTheme.DarkOrange.xaml");
        }

        ///// <summary>
        ///// Get layer.
        ///// </summary>
        //public static Task<Noesis.FrameworkElement> GetFrameworkElement(string xamlFilename)
        //{
        //    Task task = Task.FromResult((Noesis.FrameworkElement)Noesis.GUI.LoadXaml(xamlFilename));



        //    return task;
        //}

        /// <summary>
        /// Get layer.
        /// </summary>
        public Task<Layer> GetLayer(string xamlFilename)
        {
            Task<Layer> task = new Task<Layer>(() =>
            { 
                Noesis.FrameworkElement frameworkElement = (Noesis.FrameworkElement)Noesis.GUI.LoadXaml(xamlFilename);

                Noesis.View view = Noesis.GUI.CreateView(frameworkElement);

                Noesis.RenderDeviceD3D11 device = new Noesis.RenderDeviceD3D11(
                    ((SharpDX.Direct3D11.Device)ShevaGame.Instance.GraphicsDevice.Handle).ImmediateContext.NativePointer,
                    false);

                view.Renderer.Init(device);
                view.SetFlags(Noesis.RenderFlags.LCD | Noesis.RenderFlags.PPAA);

                return new Layer(_game, view);
            });

            _game.TasksManager.RunTaskOnMainThread(task);

            return task;
        }

//        /// <summary>
//        /// Run on UI thread.
//        /// </summary>        
//        public static void RunOnUIThread(Action action)
//        {
//#if WINDOWS_UAP
//            Windows.UI.Core.CoreDispatcher dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

//            if (dispatcher.HasThreadAccess)
//                action();
//            else
//                dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
//                {
//                    action();
//                }).AsTask();
//#else
            
//            action();
//#endif
//        }

//        /// <summary>
//        /// Run on UI thread.
//        /// </summary>        
//        public static Task<T> RunFuncOnUIThread<T>(Func<T> function)
//        {
//#if WINDOWS_UAP
//            Windows.UI.Core.CoreDispatcher dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

//            if (dispatcher.HasThreadAccess)
//                return Task.FromResult(function());
//            else
//            {
//                TaskCompletionSource<T> taskSource = new TaskCompletionSource<T>();

//                dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
//                {
//                    taskSource.SetResult(function());
//                }).AsTask();                    

//                return taskSource.Task;
//            }
//#else
//            return Task.FromResult(function());
//#endif
//        }
    }
}
