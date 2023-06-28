using Noesis;
using ShevaEngine.Core;
using System;
using System.IO;

namespace NoesisApp
{
    public class EmbeddedFontProvider : FontProvider
    {
        public EmbeddedFontProvider()
        {
            RegisterFontResources();            
        }

        private void RegisterFontResources()
        {
            if (ShevaServices.GetService<IEmbeddedFilesService>() is IEmbeddedFilesService embeddedFilesService)
            {
                foreach (string name in embeddedFilesService.GetAllResourcesWithExtensions(".ttf", ".ttc", ".otf"))
                {
                    if (name.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) ||
                        name.EndsWith(".ttc", StringComparison.OrdinalIgnoreCase) ||
                        name.EndsWith(".otf", StringComparison.OrdinalIgnoreCase))
                    {
                        int lastDot = name.LastIndexOf('.', name.Length - 5);
                        string folder = lastDot != -1 ? name.Substring(0, lastDot).Replace('.', '/') : string.Empty;
                        string filename = lastDot != -1 ? name.Substring(lastDot + 1) : name;

                        RegisterFont(new Uri(folder, UriKind.RelativeOrAbsolute), filename);
                    }
                }
            }
        }

        public override Stream OpenFont(Uri folder, string filename)
        {
            string path = folder.GetPath();
            if (path.Length > 0 && !path.EndsWith("/")) path += "/";
            path += filename;

            path = path.Replace('/', '.');

            if (ShevaServices.GetService<IEmbeddedFilesService>().TryGetStream(path, out Stream stream))
            {
                return stream;
            }

            return null!;
        }
    }
}