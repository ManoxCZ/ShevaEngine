using ShevaEngine.Core;
using System;
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
        public override Stream LoadXaml(Uri filename)
        {
            string filenameWithoutExtension = filename.OriginalString.Replace(".xaml", string.Empty);

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
