using System.Collections.Generic;

namespace ShevaEngine.Core
{
    public interface IFileSystemService
    {
        string UserDataPath { get; }

        bool FileExists(string path);
        void DeleteFile(string path);
        bool DirectoryExists(string subdirectoryPath);
        bool CreateDirectory(string subdirectoryPath);
        IEnumerable<string> GetSubdirectories(string subdirectoryPath);
        string ReadFileContent(string filename);
        void WriteFileContent(string filename, string content);
    }
}
