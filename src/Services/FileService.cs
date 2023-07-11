using System.IO;

namespace SimpleWebStudio.Services;

public class FileService : IFileService
{
    public bool CreateDirectory(string path, string name)
    {
        try
        {
            var filePath = Path.Combine(path, name);
            if (!Directory.Exists(path) || File.Exists(filePath))
                return false;

            Directory.CreateDirectory(filePath);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public bool CreateFile(string path, string name)
    {
        try
        {
            var filePath = Path.Combine(path, name);
            if (!Directory.Exists(path) || File.Exists(filePath))
                return false;

            File.Create(filePath).Close();
        }
        catch
        {
            return false;
        }

        return true;
    }

    public string RenameFile(string oldPath, string newFileName)
    {
        try
        {
            var filePath = Path.GetDirectoryName(oldPath);
            if (filePath is null)
                return string.Empty;

            var newPath = Path.Combine(filePath, newFileName);

            if (!File.Exists(oldPath) || File.Exists(newPath))
                return string.Empty;

            File.Move(oldPath, newPath);

            return newPath;
        }
        catch
        {
            return string.Empty;
        }
    }
}