using System.Collections.Generic;
using System.IO;

namespace ShevaEngine.Core
{
    public interface IEmbeddedFilesService
    {
        bool TryGetStream(string resourceName, out Stream stream);
        IReadOnlyList<string> GetAllResourcesWithExtension(string extension);
        IReadOnlyList<string> GetAllResourcesWithExtensions(params string[] extensions);
    }
}
