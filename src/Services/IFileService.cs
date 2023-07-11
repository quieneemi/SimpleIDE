namespace SimpleWebStudio.Services;

public interface IFileService
{
    public bool CreateDirectory(string path, string name);
    public bool CreateFile(string path, string name);
    public string RenameFile(string oldPath, string newFileName);
}