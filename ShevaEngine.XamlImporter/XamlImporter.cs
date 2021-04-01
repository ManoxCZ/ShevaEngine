using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;

namespace ShevaEngine.XamlImporter
{
    [ContentImporter("xaml", DisplayName="Xaml importer")]
    public class XamlImporter : ContentImporter<string>
    {
        /// <summary>
        /// Import.
        /// </summary>
        public override string Import(string filename, ContentImporterContext context)
        {
            return File.ReadAllText(filename);
        }
    }
}
