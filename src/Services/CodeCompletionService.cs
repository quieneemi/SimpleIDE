using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleWebStudio.Models;

namespace SimpleWebStudio.Services;

public class CodeCompletionService : ICodeCompletionService
{
    public IEnumerable<SimpleCompletionData> GetSuggestions(string input, string documentFileName)
    {
        var fileExtension = Path.GetExtension(documentFileName);
        if (fileExtension is not (".html" or ".css" or ".js"))
            yield break;

        var dictionary = fileExtension switch
        {
            ".html" => KeywordsHolder.HtmlKeywords,
            ".css" => KeywordsHolder.CssKeywords,
            ".js" => KeywordsHolder.JavaScriptKeywords,
            _ => Array.Empty<string>()
        };

        var lastWord = input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Last();
        foreach (var word in dictionary)
        {
            if (word.StartsWith(lastWord, StringComparison.OrdinalIgnoreCase))
                yield return new SimpleCompletionData(word.ToLower());
        }
    }
}