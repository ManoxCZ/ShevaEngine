using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;
using Noesis;
using ShevaEngine.Core;
using ShevaEngine.Core.Services.EmbeddedFilesService;
using System;
using System.IO;

namespace ShevaEngine.NoesisUI
{
    public class TextureProvider : FileTextureProvider
    {
        private readonly ILogger _log;


        /// <summary>
        /// Constructor.
        /// </summary>
        public TextureProvider()
        {
            _log = ShevaGame.Instance.Services.GetService<ILoggerFactory>().CreateLogger(GetType());
        }

        /// <summary>
        /// Load xaml.
        /// </summary>
        public override Stream OpenStream(Uri filename)
        {
            _log.LogInformation($"Loading file: {filename}");

            if (ShevaGame.Instance.Services.GetService<IEmbeddedFilesService>().TryGetStream(filename.OriginalString, out Stream stream))
            {
                _log.LogInformation($"File found and loaded!");

                return stream;
            }

            _log.LogError($"Can't find file!");

            return null!;
        }
    }
}
