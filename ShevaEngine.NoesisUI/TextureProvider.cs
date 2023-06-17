using Microsoft.Extensions.Logging;
using Noesis;
using ShevaEngine.Core;
using System;
using System.IO;

namespace ShevaEngine.NoesisUI;

internal class TextureProvider : FileTextureProvider
{
    private readonly ILogger _log;


    public TextureProvider()
    {
        _log = ShevaGame.Instance.Services.GetService<ILoggerFactory>().CreateLogger(GetType());
    }

    public override Stream? OpenStream(Uri filename)
    {
        if (ShevaServices.GetService<IEmbeddedFilesService>().TryGetStream(filename.OriginalString, out Stream stream))
        {
            return stream;
        }

        _log.LogError($"Can't find or load file: {filename}");

        return null;
    }
}
