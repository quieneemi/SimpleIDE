using System.IO;

namespace SimpleWebStudio.Models;

public struct ExplorerNode
{
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public NodeType Type { get; set; }

    public ExplorerNode(string path, NodeType type)
    {
        FilePath = path;
        FileName = Path.GetFileName(path);
        Type = type;
    }
}

public enum NodeType
{
    Directory,
    File
}