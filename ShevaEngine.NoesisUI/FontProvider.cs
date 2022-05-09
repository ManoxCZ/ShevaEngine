using System;
using System.IO;

namespace ShevaEngine.NoesisUI;

internal class FontProvider : Noesis.FontProvider
{
    public override Stream OpenFont(Uri folder, string id)
    {
        string filePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Assets",
            folder.OriginalString,
            id);

        return new StreamReader(filePath).BaseStream;
    }

    public override void ScanFolder(Uri folder)
    {
        string folderPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Content",
            folder.OriginalString);

        if (!Directory.Exists(folderPath))
        {
            return;
        }

        foreach (string filename in Directory.GetFiles(folderPath))
        {
            if (string.Compare(Path.GetExtension(filename), ".ttf", true) == 0 ||
                string.Compare(Path.GetExtension(filename), ".otf", true) == 0)
            {
                RegisterFont(folder, filename);
            }
        }
    }
}
