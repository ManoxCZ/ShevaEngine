using Microsoft.Extensions.Logging;
using ShevaEngine.Core;
using System;
using System.IO;

namespace ShevaEngine.NoesisUI;

internal class XamlProvider : Noesis.XamlProvider
{
    private readonly ILogger _log;


    public XamlProvider()
    {
        _log = ShevaGame.Instance.Services.GetService<ILoggerFactory>().CreateLogger(GetType());
    }

    public override Stream? LoadXaml(Uri filename)
    {
        if (ShevaServices.GetService<IEmbeddedFilesService>().TryGetStream(filename.OriginalString, out Stream stream))
        {
            return stream;
        }            

        _log.LogError($"Can't find xaml file: {filename}");

        return null;
    }
}
