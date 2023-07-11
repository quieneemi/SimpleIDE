namespace SimpleWebStudio.Models;

public static class KeywordsHolder
{
    public static string[] HtmlKeywords { get; } =
    {
        "<html>", "<head>", "<title>", "<body>", "<h1>", "<h2>", "<h3>", "<h4>", "<h5>", "<h6>", "<p>", "<a>", "<img>",
        "<ul>", "<ol>", "<li>", "<div>", "<span>", "<table>", "<tr>", "<td>", "<th>", "<form>", "<input>", "<textarea>",
        "<button>", "<select>", "<option>", "<label>", "<br>", "<hr>", "<iframe>", "<script>", "<style>", "<meta>",
        "id", "class", "style", "title", "src", "href", "alt", "width", "colspan", "disabled", "readonly",
        "placeholder", "required", "checked", "value", "target", "maxlength", "min", "max", "autocomplete", "autoplay"
    };

    public static string[] CssKeywords { get; } =
    {
        "accent-color", "align-content", "align-items", "align-self", "color", "font-size", "font-family",
        "font-weight", "text-align", "text-decoration", "background-color", "background-image", "margin", "padding",
        "border", "width", "height", "display", "position", "top", "right", "bottom", "left", "float", "clear",
        "opacity", "box-shadow", "text-shadow", "transition", "animation", "overflow", "list-style", "cursor"
    };

    public static string[] JavaScriptKeywords { get; } =
    {
        "goto", "in", "instanceof", "static", "finally", "arguments", "public", "do", "else", "const", "function",
        "class", "return", "let", "catch", "eval", "for", "if", "this", "try", "break", "debugger", "yield", "extends",
        "enum", "continue", "export", "null", "switch", "private", "new", "throw", "while", "case", "await", "delete",
        "super", "default", "void", "var", "protected", "package", "interface", "false", "typeof", "implements",
        "with", "import", "true"
    };
}