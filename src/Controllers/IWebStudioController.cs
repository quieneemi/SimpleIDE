using System.Collections.Generic;
using SimpleWebStudio.Models;

namespace SimpleWebStudio.Controllers;

public interface IWebStudioController
{
    public IEnumerable<SimpleCompletionData> GetSuggestions(string input, string documentFileName);
    public bool CreateDirectory(string path, string name);
    public bool CreateFile(string path, string name);
    public string RenameFile(string oldPath, string newFileName);
}