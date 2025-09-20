using Noesis;
using ShevaEngine.Core;
using System;
using System.IO;

namespace NoesisApp
{
    public class EmbeddedFontProvider : FontProvider
    {
        public override void ScanFolder(Uri folder)
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
                        string folderName = lastDot != -1 ? name.Substring(0, lastDot).Replace('.', '/') : string.Empty;
                        string filename = lastDot != -1 ? name.Substring(lastDot + 1) : name;

                        RegisterFont(new Uri(folderName, UriKind.RelativeOrAbsolute), filename);
                    }
                }
            }
        }

        public override bool FamilyExists(Uri baseUri, string familyName)
        {
            bool result = base.FamilyExists(baseUri, familyName);

            return result;
        }

        public override FontSource MatchFont(Uri baseUri, string familyName, ref FontWeight weight, ref FontStretch stretch, ref FontStyle style)
        {
            FontSource result = base.MatchFont(baseUri, familyName, ref weight, ref stretch, ref style);

            return result;
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