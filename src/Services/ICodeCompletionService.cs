using System.Collections.Generic;
using SimpleWebStudio.Models;

namespace SimpleWebStudio.Services;

public interface ICodeCompletionService
{
    public IEnumerable<SimpleCompletionData> GetSuggestions(string input, string documentFileName);
}