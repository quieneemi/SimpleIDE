using System.Collections.Generic;
using SimpleWebStudio.Models;
using SimpleWebStudio.Services;

namespace SimpleWebStudio.Controllers;

public class WebStudioController : IWebStudioController
{
    private readonly ICodeCompletionService _codeCompletionService;
    private readonly IFileService _fileService;

    public WebStudioController(ICodeCompletionService codeCompletionService, IFileService fileService)
    {
        _codeCompletionService = codeCompletionService;
        _fileService = fileService;
    }

    public IEnumerable<SimpleCompletionData> GetSuggestions(string input, string documentFileName)
        => _codeCompletionService.GetSuggestions(input, documentFileName);

    public bool CreateDirectory(string path, string name)
        => _fileService.CreateDirectory(path, name);

    public bool CreateFile(string path, string name)
        => _fileService.CreateFile(path, name);

    public string RenameFile(string oldPath, string newFileName)
        => _fileService.RenameFile(oldPath, newFileName);
}