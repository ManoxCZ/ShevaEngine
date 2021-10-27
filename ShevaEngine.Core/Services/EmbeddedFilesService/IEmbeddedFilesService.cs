using System.IO;

namespace ShevaEngine.Core.Services.EmbeddedFilesService
{
    public interface IEmbeddedFilesService
    {
        bool TryGetStream(string resourceName, out Stream stream);
    }
}
