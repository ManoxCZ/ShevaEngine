using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ShevaEngine.NoesisUI
{
    public class FontProvider : Noesis.FontProvider
    {
        /// <summary>
        /// Open font.
        /// </summary>
        public override Stream OpenFont(string folder, string id)
        {
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
        }

        /// <summary>
        /// Scan folder.
        /// </summary>        
        public override void ScanFolder(string folder)
        {
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
        }
    }
}
