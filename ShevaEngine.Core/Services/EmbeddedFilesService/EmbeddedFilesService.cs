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
        public readonly Dictionary<string, EmbeddedFileProvider> _files = new();

        public EmbeddedFilesService()
        {
            Initialize();
        }

        private void Initialize()
        {
            foreach (Assembly assembly in GetListOfEntryAssemblyWithReferences()
                .Where(item =>
                {
                    if (item.FullName is string fullname)
                    {
                        return !fullname.Contains("Microsoft.") && !fullname.Contains("System.");
                    }

                    return false;
                }))
            {
                EmbeddedFileProvider embeddedFileProvider = new(assembly);

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

        public IReadOnlyList<string> GetAllResourcesWithExtension(string extension)
        {
            return GetAllResourcesWithExtensions(extension);
        }

        public IReadOnlyList<string> GetAllResourcesWithExtensions(params string[] extensions)
        {
            return _files
                .Where(file => extensions.Contains(Path.GetExtension(file.Key)))
                .Select(file => file.Key)
                .ToList();
        }

        private static List<Assembly> GetListOfEntryAssemblyWithReferences()
        {
            List<Assembly> listOfAssemblies = new();
            
            if (Assembly.GetEntryAssembly() is Assembly mainAssembly)
            {
                listOfAssemblies.Add(mainAssembly);

                foreach (var refAsmName in mainAssembly.GetReferencedAssemblies())
                {
                    listOfAssemblies.Add(Assembly.Load(refAsmName));
                }
            }

            return listOfAssemblies;
        }
    }
}
