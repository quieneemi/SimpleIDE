using System;
using Avalonia.Media;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;

namespace SimpleWebStudio.Models;

public class SimpleCompletionData : ICompletionData
{
    public IImage? Image => null;
    public string Text { get; }
    public object Content => Text;
    public object Description => string.Empty;
    public double Priority { get; } = 0;

    public SimpleCompletionData(string text) => Text = text;

    public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        => textArea.Document.Replace(new SimpleSegment(completionSegment), Text);
}