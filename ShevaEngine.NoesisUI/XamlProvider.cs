using Microsoft.Extensions.Logging;
using ShevaEngine.Core;
using ShevaEngine.Core.Services.EmbeddedFilesService;
using System;
using System.IO;

namespace ShevaEngine.NoesisUI
{
    /// <summary>
    /// Xaml Provider.
    /// </summary>
    public class XamlProvider : Noesis.XamlProvider
    {
        private readonly ILogger _log;


        /// <summary>
        /// Constructor.
        /// </summary>
        public XamlProvider()
        {
            _log = ShevaGame.Instance.Services.GetService<ILoggerFactory>().CreateLogger(GetType());
        }

        /// <summary>
        /// Load xaml.
        /// </summary>
        public override Stream LoadXaml(Uri filename)
        {
            _log.LogInformation($"Loading xaml file: {filename}");

            if (ShevaGame.Instance.Services.GetService<IEmbeddedFilesService>().TryGetStream(filename.OriginalString, out Stream stream))
            {
                _log.LogInformation($"Xaml file found and loaded!");

                return stream;
            }

            _log.LogError($"Can't find xaml file!");

            return null!;
        }
    }
}
