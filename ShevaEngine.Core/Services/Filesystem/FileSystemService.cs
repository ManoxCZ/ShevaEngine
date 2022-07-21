using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShevaEngine.Core
{
    /// <summary>
    /// Filesystem class.
    /// </summary>
    public class FileSystemService : IFileSystemService
    {
        public string UserDataPath { get; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public FileSystemService()
        {
            if (System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Name is string assemblyName)
            {
                UserDataPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        assemblyName);
            }
            else
            {
                throw new Exception("Can't get assembly name!");
            }

            if (!DirectoryExists(UserDataPath))
                CreateDirectory(UserDataPath);
        }


        public bool FileExists(string path)
        {
            return File.Exists(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    UserDataPath,
                    path));
        }

        public void DeleteFile(string path)
        {
            File.Delete(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    UserDataPath,
                    path));
        }

        /// <summary>
        /// Directory exists?
        /// </summary>
        public bool DirectoryExists(string subdirectoryPath)
        {
            return Directory.Exists(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    UserDataPath,
                    subdirectoryPath));
        }

        /// <summary>
        /// Create directory.
        /// </summary>
        public bool CreateDirectory(string subdirectoryPath)
        {
            if (!DirectoryExists(subdirectoryPath))
            {
                return Directory.CreateDirectory(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        UserDataPath,
                        subdirectoryPath)) != null;
            }

            return true;
        }

        /// <summary>
        /// Get subdirectories.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetSubdirectories(string subdirectoryPath)
        {
            return Directory.GetDirectories(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    System.Reflection.Assembly.GetEntryAssembly().GetName().Name,
                    subdirectoryPath))
                    .Select(item => Path.GetFileName(item));
        }

        /// <summary>
        /// Read file content.
        /// </summary>
        public string? ReadFileContent(string filename)
        {
            string completeFilename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                System.Reflection.Assembly.GetEntryAssembly().GetName().Name,
                filename);

            if (File.Exists(completeFilename))
                return File.ReadAllText(completeFilename);

            return null;
        }

        /// <summary>
        /// Write file content.
        /// </summary>
        public void WriteFileContent(string filename, string content)
        {
            File.WriteAllLines(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    System.Reflection.Assembly.GetEntryAssembly().GetName().Name,
                    filename),
                new[] { content });
        }
    }
}
