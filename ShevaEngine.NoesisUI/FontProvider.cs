using System;
using System.IO;
#if WINDOWS_UAP
using Windows.Storage;
using Windows.Storage.Streams;
#else
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
#if WINDOWS_UAP
            string filePath = Path.Combine(
                Windows.ApplicationModel.Package.Current.InstalledLocation.Path,
                "Assets",
                folder,
                id);

            Task<StorageFile> storageFileTask = StorageFile.GetFileFromPathAsync(filePath).AsTask();
            storageFileTask.Wait();

            Task<IRandomAccessStream> openFileTask = storageFileTask.Result.OpenAsync(FileAccessMode.Read).AsTask();
            openFileTask.Wait();

            return openFileTask.Result.AsStream();
#else
            return null;
#endif
        }

        /// <summary>
        /// Scan folder.
        /// </summary>        
        public override void ScanFolder(Uri folder)
        {
#if WINDOWS_UAP
            string folderPath = Path.Combine(
                Windows.ApplicationModel.Package.Current.InstalledLocation.Path,
                "Content",
                folder);

            if (!Directory.Exists(folderPath))
                return;

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

#endif
        }
    }
}
