using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using ShevaEngine.UserAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Engine base class.
    /// </summary>
    public class ShevaGame : Microsoft.Xna.Framework.Game
    {		
        public static ShevaGame Instance { get; set; }

		private readonly Log _log = new Log(typeof(ShevaGame));
		private TextFileLogReceiver _logReceiver;

		public GameSettings Settings { get; }		
		private Type[] _initialComponentTypes;
		public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }        
        public Input Input { get; private set; }
        public ReplaySubject<InputState> InputState { get; private set; } = new ReplaySubject<InputState>();
		private Stack<ShevaGameComponent> _gameComponents;
		private object _componentsLock = new object();
        public User User { get; private set; }        
        private bool _showDebugUI = false;
        public DebugUI DebugUI { get; private set; }
        

        /// <summary>
        /// Constructor.
        /// </summary>
        public ShevaGame(params Type[] initialComponents)
            : base()
        {
            Instance = this;
            
#if WINDOWS || DESKTOPGL
            string dataPath = System.IO.Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                System.Reflection.Assembly.GetEntryAssembly().GetName().Name);

			if (!System.IO.Directory.Exists(dataPath))
				System.IO.Directory.CreateDirectory(dataPath);
#endif

            _logReceiver = new TextFileLogReceiver("game.log");
            LogManager.AddLogReceiver(_logReceiver);

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);

            Settings = ShevaGameSettings.Load<GameSettings>();

            _initialComponentTypes = initialComponents;

            _log.Info($"Sheva Engine {Version.GetVersion()}");

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
        }        

		/// <summary>
        /// Unhandled exception handler.
        /// </summary>
		private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
   		{      		
      		LogManager.Instance.AddLogMessage(new LogMessage()
			{
				DateTime = DateTime.Now,
				Exception = (Exception)args.ExceptionObject,
				Message = "UnhandledException",
				Origin = "Core",
				Severity = LogSeverity.Error
			});			
		   }

        /// <summary>
        /// Initialize engine.
        /// </summary>
        protected override void Initialize()
        {
			_log.Info("Initialization started");

            Input = new Input();

#if !DEBUG
            if (Environment.GetCommandLineArgs().Any(item => item == "debug"))
#endif
            Input.OnKeyPressed(Microsoft.Xna.Framework.Input.Keys.F1).Subscribe(item =>
            {
                _showDebugUI = !_showDebugUI;
            });

            DebugUI = new DebugUI(this);

            base.Initialize();			

			Settings.Resolution.OnNext(new Resolution(Window.ClientBounds.Width, Window.ClientBounds.Height));			

			Observable.FromEventPattern<EventArgs>(
				handler => Window.ClientSizeChanged += handler,
				handler => Window.ClientSizeChanged -= handler).Subscribe((events) =>
				{
					_log.Info($"Window size changed {Window.ClientBounds.Width} {Window.ClientBounds.Height}");

					if (Window.ClientBounds.Width != 0 && Window.ClientBounds.Height != 0)
					{
						Settings.Resolution.OnNext(new Resolution(Window.ClientBounds.Width,	Window.ClientBounds.Height));

						ShevaGameSettings.Save(Settings);
					}
					else
					{
						_log.Warning("Invalid resolution");
					}
				});			

			IsFixedTimeStep = false;			

			Settings.MusicVolume.Subscribe(item => MediaPlayer.Volume = item);            

			_log.Info("Initialization ended");						
		}

        /// <summary>
        /// Loaf content.
        /// </summary>
        protected override void LoadContent()
        {
			_log.Info("Loading content started");			

            base.LoadContent();

            TextureUtils.Prepare(GraphicsDevice);

			_log.Info("Loading content finished");

			_log.Info("Loading loading screen started");

			CreateLoadingScreen();

			_log.Info("Loading loading screen finished");

			_log.Info("Initialize game components");

			foreach (Type componentType in _initialComponentTypes)
			{
				_log.Info($"Creating new component instance of type: {componentType}");

				object component = Activator.CreateInstance(componentType);

				if (component is ShevaGameComponent gameComponent)
				{
					_log.Info($"Component added to game");

					PushGameComponentAsync(gameComponent);
				}
				else
				{
					_log.Info($"Invalid component type");
				}
			}            

            User = new User(this);

			_log.Info("All game components initialized");
		}

		/// <summary>
		/// On exiting.
		/// </summary>
		protected override void OnExiting(object sender, EventArgs args)
		{
			ShevaGameComponent component =  PopGameComponent();
			component?.Dispose();			

			LogManager.RemoveLogReceiver(_logReceiver);
			_logReceiver.Dispose();
			_logReceiver = null;

			InputState.Dispose();
			User.Dispose();		
			Settings.Dispose();
            Input.Dispose();

			base.OnExiting(sender, args);
		}

		/// <summary>
		/// Create loading screen.
		/// </summary>
		private void CreateLoadingScreen()
		{
			LoadingScreenComponent component = new LoadingScreenComponent();

			PushGameComponent(component);
		}

		/// <summary>
		/// Update().
		/// </summary>        
		protected override void Update(GameTime time)
		{
			base.Update(time);

            Input.Update();

			InputState.OnNext(new InputState(time, Window));

			lock (_gameComponents)
				if (_gameComponents.Count > 0)
					_gameComponents.Peek().Update(time);
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
				if (_gameComponents.Count > 0)
					_gameComponents.Peek().Draw(gameTime);	
				else
					GraphicsDevice.Clear(Color.Black);

			if (_showDebugUI)
				DebugUI?.Draw(gameTime);
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
        public ShevaGameComponent PopGameComponent()
        {
			lock (_gameComponents)
			{
				ShevaGameComponent component = null;

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
