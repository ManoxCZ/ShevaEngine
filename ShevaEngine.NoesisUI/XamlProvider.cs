using ShevaEngine.Core;
using System.IO;

namespace ShevaEngine.NoesisUI
{
    /// <summary>
    /// Xaml Provider.
    /// </summary>
    public class XamlProvider : Noesis.XamlProvider
    {        
        /// <summary>
        /// Load xaml.
        /// </summary>
        public override Stream LoadXaml(string filename)
        {
            string filenameWithoutExtension = filename.Replace(".xaml", string.Empty);

            string data = ShevaGame.Instance.Content.Load<string>(filenameWithoutExtension);

            MemoryStream stream = new MemoryStream();

            StreamWriter writer = new StreamWriter(stream);
                writer.Write(data);

            writer.Flush();
            stream.Position = 0;

            return stream;
        }
    }
}
