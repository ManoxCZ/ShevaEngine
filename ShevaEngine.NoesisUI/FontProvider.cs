using System;
using System.IO;
using System.Threading.Tasks;
#if WINDOWS_UAP
using Windows.Storage;
using Windows.Storage.Streams;
#endif

namespace ShevaEngine.NoesisUI
{
    public class FontProvider : Noesis.FontProvider
    {
        /// <summary>
        /// Open font.
        /// </summary>
        public override Stream OpenFont(Uri folder, string id)
        {
            string filePath = Path.Combine(
#if WINDOWS_UAP
                Windows.ApplicationModel.Package.Current.InstalledLocation.Path,
#else
                Directory.GetCurrentDirectory(),
#endif
                "Assets",
                folder.OriginalString,
                id);

#if WINDOWS_UAP
            Task<StorageFile> storageFileTask = StorageFile.GetFileFromPathAsync(filePath).AsTask();
            storageFileTask.Wait();

            Task<IRandomAccessStream> openFileTask = storageFileTask.Result.OpenAsync(FileAccessMode.Read).AsTask();
            openFileTask.Wait();

            return openFileTask.Result.AsStream();
#else

            return new StreamReader(filePath).BaseStream;
#endif
        }

        /// <summary>
        /// Scan folder.
        /// </summary>        
        public override void ScanFolder(Uri folder)
        {
            string folderPath = Path.Combine(
#if WINDOWS_UAP
                Windows.ApplicationModel.Package.Current.InstalledLocation.Path,
#else
                Directory.GetCurrentDirectory(),
#endif
                "Content",
                folder.OriginalString);

            if (!Directory.Exists(folderPath))
                return;

#if WINDOWS_UAP
            Task<StorageFolder> getFolderTask = StorageFolder.GetFolderFromPathAsync(folderPath).AsTask();
            getFolderTask.Wait();

            Task<IReadOnlyList<StorageFile>> listFilesTask = getFolderTask.Result.GetFilesAsync().AsTask();
            listFilesTask.Wait();

            foreach (StorageFile file in listFilesTask.Result)
            {
                if (string.Compare(file.FileType, ".ttf", true) == 0 ||
                    string.Compare(file.FileType, ".otf", true) == 0)
                {
                    RegisterFont(folder, file.Name);
                }
            }
#else
            foreach (string filename in Directory.GetFiles(folderPath))
            {
                if (string.Compare(Path.GetExtension(filename), ".ttf", true) == 0 ||
                    string.Compare(Path.GetExtension(filename), ".otf", true) == 0)
                {
                    RegisterFont(folder, filename);
                }
            }
#endif
        }
    }
}
