using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ShevaEngine.Core
{
    public class EmbeddedFilesService : IEmbeddedFilesService
    {
        public readonly Dictionary<string, EmbeddedFileProvider> _files = new Dictionary<string, EmbeddedFileProvider>();

        public EmbeddedFilesService()
        {
            Initialize();
        }

        private void Initialize()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()
                .Where(item =>
                {
                    if (item.FullName is string fullname)
                    {
                        return !fullname.Contains("Microsoft.") && !fullname.Contains("System.");
                    }

                    return false;
                }))
            {
                EmbeddedFileProvider embeddedFileProvider = new EmbeddedFileProvider(assembly);

                IDirectoryContents content = embeddedFileProvider.GetDirectoryContents(string.Empty);

                foreach (IFileInfo item in content)
                {
                    _files.Add(item.Name, embeddedFileProvider);
                }
            }
        }

        public bool TryGetStream(string resourceName, out Stream stream)
        {
            if (_files.TryGetValue(resourceName, out EmbeddedFileProvider? provider))
            {
                stream = provider.GetFileInfo(resourceName).CreateReadStream();

                return true;
            }

            stream = null!;

            return false;
        }
    }
}
