using System.IO;

namespace ShevaEngine.Core
{
    public interface IEmbeddedFilesService
    {
        bool TryGetStream(string resourceName, out Stream stream);
    }
}
