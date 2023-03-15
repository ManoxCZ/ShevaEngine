using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core.Profiler;
using ShevaEngine.Core.UI;
using ShevaEngine.Core.UserAccounts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Engine base class.
    /// </summary>
    public class ShevaGame : Game
    {
        public static ShevaGame Instance { get; set; }

        public SynchronizationContext SynchronizationContext { get; }

        private readonly ILogger _log;        

        public GameSettings Settings { get; }
        private Type[] _initialComponentTypes;
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public Input Input { get; private set; }
        public ReplaySubject<InputState> InputState { get; private set; } = new ReplaySubject<InputState>();
        private Stack<ShevaGameComponent> _gameComponents;
        private object _componentsLock = new();
        public IUser User { get; set; }
        public IUISystem UISystem { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public ShevaGame(params Type[] initialComponents)
            : base()
        {
            Instance = this;
            SynchronizationContext = SynchronizationContext.Current;

            InitializeServices();            

            _log = Services.GetService<ILoggerFactory>().CreateLogger<ShevaGame>();

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);

            Settings = ShevaGameSettings.Load<GameSettings>();

            _initialComponentTypes = initialComponents;

            _log.LogInformation($"Sheva Engine {Version.GetVersion()}");

#if WINDOWS_UAP
            Settings.Resolution.OnNext(new Resolution(Window.ClientBounds.Width, Window.ClientBounds.Height));
#endif

            DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferWidth = Settings.Resolution.Value.Width,
                PreferredBackBufferHeight = Settings.Resolution.Value.Height,
                GraphicsProfile = GraphicsProfile.HiDef
            };

            GraphicsDeviceManager.ApplyChanges();

            Content = new ContentManagerEx(Services);

            Window.Title = @"Sheva Engine MG";

            Window.AllowUserResizing = true;
            IsMouseVisible = true;

            _gameComponents = new Stack<ShevaGameComponent>();

            Services.AddService<IEmbeddedFilesService>(new EmbeddedFilesService());
        }

        /// <summary>
        /// Initialize services.
        /// </summary>
        private void InitializeServices()
        {
            Services.AddService<IFileSystemService>(new FileSystemService());
            Services.AddService(LoggerFactory.Create(builder =>
            {
                builder
                    .AddProvider(new TextFileLogReceiver())
#if DEBUG
                    .SetMinimumLevel(LogLevel.Debug);
#else
					;
#endif
            }));
            Services.AddService<ProfilerService>(new ProfilerService());
        }

        /// <summary>
        /// Unhandled exception handler.
        /// </summary>
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            if (System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name is string assemblyName &&
                 args.ExceptionObject.ToString() is string exceptionText)
            {
                string dataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    assemblyName,
                    "crash.log");

                File.WriteAllLines(dataPath, new string[]
                {
                    "UnhandledException",
                    exceptionText
                });
            }
        }

        /// <summary>
        /// Initialize engine.
        /// </summary>
        protected override void Initialize()
        {
            _log.LogInformation("Initialization started");

            Input = new Input();

            base.Initialize();

            Settings.Resolution.OnNext(new Resolution(Window.ClientBounds.Width, Window.ClientBounds.Height));

            Observable.FromEventPattern<EventArgs>(
                handler => Window.ClientSizeChanged += handler,
                handler => Window.ClientSizeChanged -= handler).Subscribe((events) =>
                {
                    _log.LogInformation($"Window size changed {Window.ClientBounds.Width} {Window.ClientBounds.Height}");

                    if (Window.ClientBounds.Width != 0 && Window.ClientBounds.Height != 0)
                    {
                        Settings.Resolution.OnNext(new Resolution(Window.ClientBounds.Width, Window.ClientBounds.Height));

                        ShevaGameSettings.Save(Settings);
                    }
                    else
                    {
                        _log.LogWarning("Invalid resolution");
                    }
                });

            IsFixedTimeStep = false;

            //Settings.MusicVolume.Subscribe(item =>
            //         {
            //             MediaPlayer.Volume = item;

            //         });            

            _log.LogInformation("Initialization ended");
        }

        /// <summary>
        /// Loaf content.
        /// </summary>
        protected override void LoadContent()
        {
            _log.LogInformation("Loading content started");

            base.LoadContent();

            TextureUtils.Prepare(GraphicsDevice);

            _log.LogInformation("Loading content finished");



            _log.LogInformation("Initialize game components");

            foreach (Type componentType in _initialComponentTypes)
            {
                _log.LogInformation($"Creating new component instance of type: {componentType}");

                if (Activator.CreateInstance(componentType) is ShevaGameComponent gameComponent)
                {
                    _log.LogInformation($"Component added to game");

                    PushGameComponentAsync(gameComponent);
                }
                else
                {
                    _log.LogInformation($"Invalid component type");
                }
            }



            _log.LogInformation("All game components initialized");
        }

        /// <summary>
        /// On exiting.
        /// </summary>
        protected override void OnExiting(object sender, EventArgs args)
        {
            ShevaGameComponent? component = PopGameComponent();
            component?.Dispose();

            InputState.Dispose();
            User?.Dispose();
            Settings.Dispose();
            Input.Dispose();            

            base.OnExiting(sender, args);
        }
        
        /// <summary>
        /// Update().
        /// </summary>        
        protected override void Update(GameTime time)
        {
            base.Update(time);

            Input.Update();

            InputState inputState = new InputState(time, Window);
            InputState.OnNext(inputState);

            lock (_gameComponents)
            {
                if (_gameComponents.Count > 0)
                {
                    _gameComponents.Peek().Update(time, inputState);
                }
            }
        }

        /// <summary>
        /// Draw().
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            if (Settings.Fullscreen.Value != GraphicsDeviceManager.IsFullScreen)
            {
                DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

                GraphicsDeviceManager.IsFullScreen = Settings.Fullscreen.Value;
                GraphicsDeviceManager.PreferredBackBufferWidth = Settings.Fullscreen.Value ? displayMode.Width : Settings.Resolution.Value.Width;
                GraphicsDeviceManager.PreferredBackBufferHeight = Settings.Fullscreen.Value ? displayMode.Height : Settings.Resolution.Value.Height;

                GraphicsDeviceManager.ApplyChanges();
            }

            lock (_gameComponents)
            {
                if (_gameComponents.Count > 0)
                {
                    _gameComponents.Peek().Draw(gameTime);
                }
                else
                {
                    GraphicsDevice.Clear(Color.Black);
                }
            }
        }

        /// <summary>
        /// Push game component.
        /// </summary>        
        public void PushGameComponent(ShevaGameComponent component)
        {
            if (component == null)
                return;

            if (!component.IsInitialized)
                component.Initialize(this);

            if (!component.IsContentLoaded)
                component.LoadContent(this);

            component.Activate(this);

            lock (_gameComponents)
            {
                if (_gameComponents.Count > 0)
                    _gameComponents.Peek().Deactivate(this);

                _gameComponents.Push(component);
            }
        }

        /// <summary>
        /// Push game component.
        /// </summary>        
        public void PushGameComponentAsync(ShevaGameComponent component)
        {
            if (component == null)
                return;

            Task.Run(() =>
            {
                if (!component.IsInitialized)
                    component.Initialize(this);

                if (!component.IsContentLoaded)
                    component.LoadContent(this);

                component.Activate(this);

                lock (_gameComponents)
                {
                    if (_gameComponents.Count > 0)
                        _gameComponents.Peek().Deactivate(this);

                    _gameComponents.Push(component);
                }
            });
        }

        /// <summary>
        /// Pop game component.
        /// </summary>        
        public ShevaGameComponent? PopGameComponent()
        {
            lock (_gameComponents)
            {
                ShevaGameComponent? component = null;

                if (_gameComponents.Count > 0)
                    component = _gameComponents.Pop();

                component?.Deactivate(this);

                if (_gameComponents.Count > 0)
                    _gameComponents.Peek().Activate(this);
                else
                    Exit();

                return component;
            }
        }

        /// <summary>
        /// Set fullscreen.
        /// </summary>		
        public void SetFullscreen(bool fullscreen)
        {
            Settings.Fullscreen.OnNext(fullscreen);
            ShevaGameSettings.Save(Settings);
        }


    }
}
