using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Logging;
using Epic.OnlineServices.Platform;
using Microsoft.Extensions.Logging;
using ShevaEngine.Core;
using ShevaEngine.Core.UserAccounts;
using System.IO;
using System.Threading.Tasks;
using System.Timers;

namespace ShevaEngine.EOS
{
    public class EOSUser : IUser
    {
        public static string PRODUCT_NAME = "";
        public static string PRODUCT_VERSION = "";
        public static string PRODUCT_ID = "";
        public static string CLIENT_ID = "";
        public static string CLIENT_SECRET = "";
        public static string SANDBOX_ID = "";
        public static string DEPLOYMENT_ID = "";
        

        private readonly ILogger _log = null!;
        private readonly PlatformInterface _platformInterface;
        private readonly Timer _timer = new Timer(100); 


        public EOSUser(ShevaGame game)
        {
            if (game.Services.GetService<ILoggerFactory>() is ILoggerFactory loggerFactory)
            {
                _log = loggerFactory.CreateLogger<EOSUser>();
            }

            _platformInterface = Initialize();
        }

        private PlatformInterface Initialize()
        {
            InitializeOptions initializeOptions = new InitializeOptions()
            {
                ProductName = PRODUCT_NAME,
                ProductVersion = PRODUCT_VERSION
            };

            Result result = PlatformInterface.Initialize(initializeOptions);

            if (result != Result.Success)
            {
                throw new System.Exception("Failed to initialize platform: " + result);
            }

            LoggingInterface.SetLogLevel(LogCategory.AllCategories, Epic.OnlineServices.Logging.LogLevel.VeryVerbose);
            LoggingInterface.SetCallback((LogMessage logMessage) =>
            {
                switch (logMessage.Level)
                {
                    case Epic.OnlineServices.Logging.LogLevel.Off:                        
                    case Epic.OnlineServices.Logging.LogLevel.Fatal:
                    case Epic.OnlineServices.Logging.LogLevel.Error:
                        _log.LogError(logMessage.Message);
                        break;
                    case Epic.OnlineServices.Logging.LogLevel.Warning:
                        _log.LogWarning(logMessage.Message);
                        break;
                    case Epic.OnlineServices.Logging.LogLevel.Info:
                        _log.LogInformation(logMessage.Message);
                        break;
                    case Epic.OnlineServices.Logging.LogLevel.Verbose:
                    case Epic.OnlineServices.Logging.LogLevel.VeryVerbose:
                        _log.LogDebug(logMessage.Message);
                        break;
                    default:
                        break;
                }                
            });

            string xAudio29DllFilename = Path.Combine(Directory.GetCurrentDirectory(), "xaudio2_9redist.dll");
            
            if (!File.Exists(xAudio29DllFilename))
            {
                _log.LogError($"Unable to find xaudio2 library: '{xAudio29DllFilename}'.");                               
            }

            WindowsOptions options = new WindowsOptions()
            {
                ProductId = PRODUCT_ID,
                SandboxId = SANDBOX_ID,
                DeploymentId = DEPLOYMENT_ID,
                ClientCredentials = new ClientCredentials()
                {
                    ClientId = CLIENT_ID,
                    ClientSecret = CLIENT_SECRET,
                },
                RTCOptions = new WindowsRTCOptions()
                {
                    PlatformSpecificOptions = new WindowsRTCOptionsPlatformSpecificOptions()
                    {
                        XAudio29DllPath = xAudio29DllFilename
                    }
                },
                Flags = PlatformFlags.DisableOverlay,
                IsServer = false,
            };            

            if (PlatformInterface.Create(options) is PlatformInterface platformInterface)
            {
                _timer.Elapsed += Update;
                _timer.Start();

                return platformInterface;
            }
            else
            {
                throw new System.Exception("Failed to create platform");
            }            
        }

        public Task<bool> ConnectToService(bool silently = false)
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                Credentials credentials = new Credentials()
                {
                    Type = LoginCredentialType.AccountPortal,
                    ExternalType = ExternalCredentialType.Epic,
                    Id = string.Empty,
                    Token = string.Empty
                };

                LoginOptions loginOptions = new LoginOptions()
                {
                    Credentials = credentials,
                    ScopeFlags = AuthScopeFlags.NoFlags,
                };

                OnLoginCallback loginHandler = (info) => 
                {
                    taskCompletionSource.SetResult(info.ResultCode == Result.Success);
                };

                _platformInterface.GetAuthInterface().Login(loginOptions, null!, loginHandler);
            });

            return taskCompletionSource.Task;
        }        

        public void Dispose()
        {
            _timer.Elapsed -= Update;
            _timer.Stop();

            _platformInterface.Release();

            PlatformInterface.Shutdown();
        }      
        
        private void Update(object sender, ElapsedEventArgs args)
        {
            _platformInterface.Tick();
        }
    }
}
