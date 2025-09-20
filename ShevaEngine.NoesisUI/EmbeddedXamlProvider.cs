using Microsoft.Extensions.Logging;
using ShevaEngine.Core;
using System;
using System.IO;

namespace ShevaEngine.NoesisUI;

internal class EmbeddedXamlProvider : Noesis.XamlProvider
{
    private readonly ILogger _log;


    public EmbeddedXamlProvider()
    {
        _log = ShevaGame.Instance.Services.GetService<ILoggerFactory>().CreateLogger(GetType());
    }

    public override Stream? LoadXaml(Uri filename)
    {
        _log.LogDebug($"Loading XAML: {filename.OriginalString}");

        if (ShevaServices.GetService<IEmbeddedFilesService>().TryGetStream(filename.OriginalString, out Stream stream))
        {
            return stream;
        }            

        _log.LogError($"Can't find xaml file: {filename}");

        return null;
    }
}
