using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core.Profiler;
using ShevaEngine.Core.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ShevaEngine.Core;

public class ShevaGame : Game
{
    public static ShevaGame Instance { get; private set; }
    private const string WINDOW_TITLE = "Sheva Engine MonoGame";

    private readonly ILogger _log;

    private readonly ProfilerService _profilerService = new();
    private readonly ProfilerDataDraw _profilerDraw = new();        
    private Type[] _initialComponentTypes;
    private readonly GameSettings _gameSettings = null!;
    public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }        
    private readonly Stack<ShevaGameComponent> _gameComponents = new();
    private bool _showProfilerInfo = false;


    public ShevaGame(Type[] initialComponents, string windowTitle = WINDOW_TITLE)
        : base()
    {
        Instance = this;        

        InitializeServices();            

        _log = Services.GetService<ILoggerFactory>().CreateLogger<ShevaGame>();

        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);

        if (ShevaServices.GetService<SettingsService>().GetSettings<GameSettings>() is GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }            

        _initialComponentTypes = initialComponents;

        _log.LogInformation($"Sheva Engine {VersionUtils.GetVersion()}");
        
        DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

        GraphicsDeviceManager = new GraphicsDeviceManager(this)
        {
            IsFullScreen = false,
            PreferredBackBufferWidth = _gameSettings.Resolution.Value.Width,
            PreferredBackBufferHeight = _gameSettings.Resolution.Value.Height,
            GraphicsProfile = GraphicsProfile.HiDef
        };

        GraphicsDeviceManager.ApplyChanges();

        Content = new ContentManagerEx(Services);

        Window.Title = windowTitle;
        Window.AllowUserResizing = true;

        IsMouseVisible = true;                        
    }

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
        Services.AddService(_profilerService);
        Services.AddService<SettingsService>(new());
        Services.AddService<IEmbeddedFilesService>(new EmbeddedFilesService());
    }

    private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
    {
        if (System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name is string assemblyName &&
             args.ExceptionObject.ToString() is string exceptionText)
        {
            string dataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                assemblyName,
                "crash.log");

            File.WriteAllLines(dataPath, ["UnhandledException", exceptionText]);
        }
    }

    protected override void Initialize()
    {
        _log.LogInformation("Initialization started");            

        base.Initialize();
        
        _gameSettings.Resolution.OnNext(new Resolution(Window.ClientBounds.Width, Window.ClientBounds.Height));

        Observable.FromEventPattern<EventArgs>(
            handler => Window.ClientSizeChanged += handler,
            handler => Window.ClientSizeChanged -= handler).Subscribe((events) =>
            {
                _log.LogInformation($"Window size changed {Window.ClientBounds.Width} {Window.ClientBounds.Height}");

                if (Window.ClientBounds.Width != 0 && Window.ClientBounds.Height != 0)
                {
                    _gameSettings.Resolution.OnNext(new Resolution(Window.ClientBounds.Width, Window.ClientBounds.Height));

                    if (!GraphicsDeviceManager.IsFullScreen)
                    {
                        _gameSettings.WindowedResolution.OnNext(new Resolution(Window.ClientBounds.Width, Window.ClientBounds.Height));
                    }

                    ShevaGameSettings.Save(_gameSettings);
                }
                else
                {
                    _log.LogWarning("Invalid resolution");
                }
            });

        IsFixedTimeStep = false;
        
        _log.LogInformation("Initialization ended");
    }

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

        _profilerDraw.LoadContent(this);

        _log.LogInformation("All game components initialized");
    }

    protected override void EndRun()
    {
        ShevaGameComponent? component = PopGameComponent();
        component?.Dispose();

        base.EndRun();
    }

    protected override void Update(GameTime time)
    {
        using var _ = _profilerService.BeginScope("Update");
        
        base.Update(time);            

        InputState inputState = new(time);
        
        lock (_gameComponents)
        {
            if (_gameComponents.Count > 0)
            {
                _gameComponents.Peek().Update(time, inputState);
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        using (var _ = _profilerService.BeginScope("Draw"))
        {
            if (_gameSettings.Fullscreen.Value != GraphicsDeviceManager.IsFullScreen)
            {
                DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
                GraphicsDeviceManager.HardwareModeSwitch = false;
                GraphicsDeviceManager.IsFullScreen = _gameSettings.Fullscreen.Value;
                GraphicsDeviceManager.PreferredBackBufferWidth = _gameSettings.Fullscreen.Value ? displayMode.Width : _gameSettings.WindowedResolution.Value.Width;
                GraphicsDeviceManager.PreferredBackBufferHeight = _gameSettings.Fullscreen.Value ? displayMode.Height : _gameSettings.WindowedResolution.Value.Height;

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

        if (_showProfilerInfo)
        {
            _profilerDraw.Draw();
        }
    }

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
            {
                _gameComponents.Peek().Deactivate(this);
            }

            _gameComponents.Push(component);
        }
    }

    public async Task<bool> PushGameComponentAsync(ShevaGameComponent component)
    {
        if (component == null)
        {
            return false;
        }

        await Task.Run(() =>
        {
            PushGameComponent(component);
        });

        return true;
    }

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

    public void SetFullscreen(bool fullscreen)
    {
        _gameSettings.Fullscreen.OnNext(fullscreen);

        ShevaGameSettings.Save(_gameSettings);
    }
}
