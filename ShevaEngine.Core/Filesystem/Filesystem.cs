using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Filesystem class.
    /// </summary>
    public class Filesystem
    {

        /// <summary>
        /// Directory exists?
        /// </summary>
        public static bool DirectoryExists(string subdirectoryPath)
        {
#if WINDOWS_UAP
            Task<Windows.Storage.IStorageItem> task = Windows.Storage.ApplicationData.Current.RoamingFolder.TryGetItemAsync(subdirectoryPath).AsTask();

            task.Wait();

            return task.Result != null;
#else
            return System.IO.Directory.Exists(
                System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    System.Reflection.Assembly.GetEntryAssembly().GetName().Name,
                    subdirectoryPath));
#endif         
        }

        /// <summary>
        /// Create directory.
        /// </summary>
        public static bool CreateDirectory(string subdirectoryPath)
        {
            if (!DirectoryExists(subdirectoryPath))
            {
#if WINDOWS_UAP
                Task<Windows.Storage.StorageFolder> task = Windows.Storage.ApplicationData.Current.RoamingFolder.CreateFolderAsync(subdirectoryPath).AsTask();

                task.Wait();

                return task.Result != null;
#else
                return System.IO.Directory.CreateDirectory(
                    System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        System.Reflection.Assembly.GetEntryAssembly().GetName().Name,
                        subdirectoryPath)) != null;
#endif
            }

            return true;
        }

        /// <summary>
        /// Get subdirectories.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetSubdirectories(string subdirectoryPath)
        {
#if WINDOWS_UAP
            Task<Windows.Storage.IStorageItem> task = Windows.Storage.ApplicationData.Current.RoamingFolder.TryGetItemAsync(subdirectoryPath).AsTask();

            task.Wait();                       

            if (task.Result is Windows.Storage.StorageFolder folder)
            {
                Task<IReadOnlyList<Windows.Storage.StorageFolder>> subDirsTask = folder.GetFoldersAsync().AsTask();

                subDirsTask.Wait();

                return subDirsTask.Result.Select(item => item.Name);
            }

            return new List<string>();
#else
            return System.IO.Directory.GetDirectories(
                System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    System.Reflection.Assembly.GetEntryAssembly().GetName().Name,
                    subdirectoryPath))
                    .Select(item => System.IO.Path.GetFileName(item));
#endif
        }

        /// <summary>
        /// Read file content.
        /// </summary>
        public static string ReadFileContent(string filename)
        {
#if WINDOWS_UAP
            Task<Windows.Storage.IStorageItem> task = Windows.Storage.ApplicationData.Current.RoamingFolder.TryGetItemAsync(filename).AsTask();

            task.Wait();

            if (task.Result is Windows.Storage.StorageFile file)
            {
                Task<IList<string>> readTask = Windows.Storage.FileIO.ReadLinesAsync(file).AsTask();

                readTask.Wait();

                return string.Join(Environment.NewLine, readTask.Result);
            }

            return null;
#else
            string completeFilename = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                System.Reflection.Assembly.GetEntryAssembly().GetName().Name,
                filename);

            if (System.IO.File.Exists(completeFilename))
                return System.IO.File.ReadAllText(completeFilename);

            return null;
#endif
        }

        /// <summary>
        /// Write file content.
        /// </summary>
        public static void WriteFileContent(string filename, string content)
        {
#if WINDOWS_UAP
            Task<Windows.Storage.StorageFile> task = Windows.Storage.ApplicationData.Current.RoamingFolder.CreateFileAsync(
                filename, Windows.Storage.CreationCollisionOption.ReplaceExisting).AsTask();

            task.Wait();

            if (task.Result is Windows.Storage.StorageFile file)
            {
                var writeTask = Windows.Storage.FileIO.WriteLinesAsync(file, new[] { content }).AsTask();

                writeTask.Wait();
            }            
#else            
            System.IO.File.WriteAllLines(
                System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    System.Reflection.Assembly.GetEntryAssembly().GetName().Name,
                    filename),
                new[] { content });
#endif
        }
    }
}
